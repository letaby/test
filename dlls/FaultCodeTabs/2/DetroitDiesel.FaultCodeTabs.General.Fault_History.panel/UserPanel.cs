using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.FaultCodeTabs.General.Fault_History.panel;

public class UserPanel : CustomPanel
{
	private const string URLBase = "https://ddcapps.detroitdiesel.com/reservoir-dl-ext-web/faultcode";

	private const string URLVINSuffix = "?vin=";

	private const string URLPad = "**";

	private string currentVehicleIdentification;

	private string InternetNotConnected = "<html><body><div><h1>" + Resources.Message_YouAreNotConnected + "</h1></div></body></html>";

	private WebBrowser webBrowserVT;

	private Button buttonBack;

	private Button buttonForward;

	private ProgressBar progressBar;

	private System.Windows.Forms.Label labelStatus;

	private TableLayoutPanel tableLayoutPanelToolBar;

	private Button buttonRefresh;

	public bool IsInternetConnectionAvailable
	{
		get
		{
			int lpdwFlags = 0;
			return InternetGetConnectedState(ref lpdwFlags, 0);
		}
	}

	public bool IsVTWebsiteConnected { get; set; }

	[DllImport("WININET", CharSet = CharSet.Auto)]
	private static extern bool InternetGetConnectedState(ref int lpdwFlags, int dwReserved);

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		webBrowserVT.CanGoBackChanged += webBrowserVT_CanGoBackChanged;
		webBrowserVT.CanGoForwardChanged += webBrowserVT_CanGoForwardChanged;
		SapiManager.GlobalInstance.ConnectedUnitChanged += GlobalInstance_ConnectedUnitChanged;
		currentVehicleIdentification = GetConnectedVehicleIdentification();
		labelStatus.Text = ((!string.IsNullOrEmpty(currentVehicleIdentification)) ? (Resources.Message_ConnectedVIN + currentVehicleIdentification) : Resources.Message_NoConnectedVIN);
		ConnectToVTWebsite();
		((UserControl)this).OnLoad(e);
	}

	private void DisplayErrorPage()
	{
		IsVTWebsiteConnected = false;
		if (webBrowserVT.Document != null)
		{
			webBrowserVT.Document.OpenNew(replaceInHistory: true);
			webBrowserVT.Document.Write(InternetNotConnected);
		}
	}

	private void DisplayBlankPage()
	{
		if (!webBrowserVT.IsDisposed)
		{
			webBrowserVT.Navigate("about:blank");
		}
	}

	private void ConnectToVTWebsite()
	{
		if (IsInternetConnectionAvailable)
		{
			string text = "https://ddcapps.detroitdiesel.com/reservoir-dl-ext-web/faultcode";
			if (!string.IsNullOrEmpty(currentVehicleIdentification))
			{
				string value = FileEncryptionProvider.EncryptData("**" + currentVehicleIdentification + "**" + DateTime.Now.ToBinary());
				text = text + "?vin=" + WebUtility.UrlEncode(value).Replace("+", "%2B");
			}
			webBrowserVT.Url = new Uri(text, UriKind.Absolute);
		}
		else
		{
			DisplayErrorPage();
		}
	}

	private string GetConnectedVehicleIdentification()
	{
		return (from c in SapiManager.GlobalInstance.ActiveChannels
			select SapiManager.GetVehicleIdentificationNumber(c) into v
			where v != null
			select v).FirstOrDefault((string v) => Utility.ValidateVehicleIdentificationNumber(v));
	}

	private void webBrowserVT_CanGoBackChanged(object sender, EventArgs e)
	{
		buttonBack.Enabled = webBrowserVT.CanGoBack;
	}

	private void webBrowserVT_CanGoForwardChanged(object sender, EventArgs e)
	{
		buttonForward.Enabled = webBrowserVT.CanGoForward;
	}

	private void buttonBack_Click(object sender, EventArgs e)
	{
		webBrowserVT.GoBack();
	}

	private void buttonForward_Click(object sender, EventArgs e)
	{
		webBrowserVT.GoForward();
	}

	private void buttonRefresh_Click(object sender, EventArgs e)
	{
		if (IsInternetConnectionAvailable)
		{
			if (IsVTWebsiteConnected)
			{
				webBrowserVT.Refresh();
			}
			else
			{
				ConnectToVTWebsite();
			}
		}
		else
		{
			DisplayErrorPage();
		}
	}

	private void webBrowserVT_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		if (e.Url.AbsoluteUri.StartsWith("https://ddcapps.detroitdiesel.com/reservoir-dl-ext-web/faultcode", StringComparison.OrdinalIgnoreCase))
		{
			IsVTWebsiteConnected = true;
		}
	}

	private void GlobalInstance_ConnectedUnitChanged(object sender, EventArgs e)
	{
		string connectedVehicleIdentification = GetConnectedVehicleIdentification();
		labelStatus.Text = ((!string.IsNullOrEmpty(connectedVehicleIdentification)) ? (Resources.Message_ConnectedVIN + connectedVehicleIdentification) : Resources.Message_NoConnectedVIN);
		if (!string.IsNullOrEmpty(connectedVehicleIdentification) && currentVehicleIdentification != connectedVehicleIdentification)
		{
			currentVehicleIdentification = connectedVehicleIdentification;
			ConnectToVTWebsite();
		}
		else if (string.IsNullOrEmpty(connectedVehicleIdentification))
		{
			currentVehicleIdentification = null;
			DisplayBlankPage();
		}
	}

	private void webBrowserVT_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
	{
		if (e.CurrentProgress > -1 && e.MaximumProgress > -1)
		{
			progressBar.Maximum = (int)e.MaximumProgress;
			progressBar.Value = (int)e.CurrentProgress;
		}
		progressBar.Visible = e.CurrentProgress != e.MaximumProgress;
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		webBrowserVT = new WebBrowser();
		tableLayoutPanelToolBar = new TableLayoutPanel();
		buttonBack = new Button();
		labelStatus = new System.Windows.Forms.Label();
		buttonRefresh = new Button();
		buttonForward = new Button();
		progressBar = new ProgressBar();
		((Control)(object)tableLayoutPanelToolBar).SuspendLayout();
		((Control)this).SuspendLayout();
		webBrowserVT.AllowWebBrowserDrop = false;
		componentResourceManager.ApplyResources(webBrowserVT, "webBrowserVT");
		webBrowserVT.Name = "webBrowserVT";
		webBrowserVT.Url = new Uri("", UriKind.Relative);
		webBrowserVT.DocumentCompleted += webBrowserVT_DocumentCompleted;
		webBrowserVT.ProgressChanged += webBrowserVT_ProgressChanged;
		componentResourceManager.ApplyResources(tableLayoutPanelToolBar, "tableLayoutPanelToolBar");
		((TableLayoutPanel)(object)tableLayoutPanelToolBar).Controls.Add(buttonBack, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelToolBar).Controls.Add(labelStatus, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelToolBar).Controls.Add(buttonRefresh, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelToolBar).Controls.Add(buttonForward, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelToolBar).Controls.Add(progressBar, 4, 0);
		((Control)(object)tableLayoutPanelToolBar).Name = "tableLayoutPanelToolBar";
		componentResourceManager.ApplyResources(buttonBack, "buttonBack");
		buttonBack.FlatAppearance.BorderSize = 0;
		buttonBack.Name = "buttonBack";
		buttonBack.UseCompatibleTextRendering = true;
		buttonBack.UseVisualStyleBackColor = true;
		buttonBack.Click += buttonBack_Click;
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonRefresh, "buttonRefresh");
		buttonRefresh.FlatAppearance.BorderSize = 0;
		buttonRefresh.Name = "buttonRefresh";
		buttonRefresh.UseCompatibleTextRendering = true;
		buttonRefresh.UseVisualStyleBackColor = true;
		buttonRefresh.Click += buttonRefresh_Click;
		componentResourceManager.ApplyResources(buttonForward, "buttonForward");
		buttonForward.FlatAppearance.BorderSize = 0;
		buttonForward.Name = "buttonForward";
		buttonForward.UseCompatibleTextRendering = true;
		buttonForward.UseVisualStyleBackColor = true;
		buttonForward.Click += buttonForward_Click;
		componentResourceManager.ApplyResources(progressBar, "progressBar");
		progressBar.Name = "progressBar";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add(webBrowserVT);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelToolBar);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelToolBar).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelToolBar).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
