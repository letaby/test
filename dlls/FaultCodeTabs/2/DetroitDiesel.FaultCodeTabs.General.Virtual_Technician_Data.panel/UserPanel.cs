using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SapiLayer1;

namespace DetroitDiesel.FaultCodeTabs.General.Virtual_Technician_Data.panel;

public class UserPanel : CustomPanel
{
	private const string VTURLBase = "https://hq.detroitconnect.com/DiagnosticLink/";

	private const string VTURLVINSuffix = "signin?";

	private string currentVehicleIdentification;

	private string InternetNotConnected = Resources.Message_HtmlBodyDivH1YouAreNotConnectedToTheInternetPleaseClickTheRefreshButtonAfterConnectingToInternetH1DivBodyHtml;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonBack;

	private Button buttonForward;

	private System.Windows.Forms.Label labelStatus;

	private WebView2Control webView2VT;

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
		SapiManager.GlobalInstance.ConnectedUnitChanged += GlobalInstance_ConnectedUnitChanged;
		currentVehicleIdentification = GetConnectedVehicleIdentification();
		labelStatus.Text = ((!string.IsNullOrEmpty(currentVehicleIdentification)) ? (Resources.Message_ConnectedVIN + currentVehicleIdentification) : Resources.Message_NoConnectedVIN);
		((UserControl)this).OnLoad(e);
	}

	private void DisplayErrorPage()
	{
		IsVTWebsiteConnected = false;
		if (webView2VT != null && webView2VT.WebView2InitializationException == null)
		{
			((WebView2)webView2VT).NavigateToString(InternetNotConnected);
		}
	}

	private void ConnectToVTWebsite()
	{
		if (IsInternetConnectionAvailable && webView2VT != null && ((WebView2)webView2VT).CoreWebView2 != null)
		{
			string text = "https://hq.detroitconnect.com/DiagnosticLink/";
			if (!string.IsNullOrEmpty(currentVehicleIdentification))
			{
				text = text + "signin?" + currentVehicleIdentification;
			}
			((WebView2)webView2VT).CoreWebView2.Navigate(text);
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

	private void CoreWebView2_HistoryChanged(object sender, object e)
	{
		buttonBack.Enabled = ((WebView2)webView2VT).CanGoBack;
		buttonForward.Enabled = ((WebView2)webView2VT).CanGoForward;
	}

	private void buttonBack_Click(object sender, EventArgs e)
	{
		((WebView2)webView2VT).GoBack();
	}

	private void buttonForward_Click(object sender, EventArgs e)
	{
		((WebView2)webView2VT).GoForward();
	}

	private void buttonRefresh_Click(object sender, EventArgs e)
	{
		if (IsInternetConnectionAvailable)
		{
			if (IsVTWebsiteConnected)
			{
				((WebView2)webView2VT).CoreWebView2.Reload();
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

	private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
	{
		if (e.IsSuccess)
		{
			IsVTWebsiteConnected = true;
		}
	}

	private void GlobalInstance_ConnectedUnitChanged(object sender, EventArgs e)
	{
		string connectedVehicleIdentification = GetConnectedVehicleIdentification();
		labelStatus.Text = ((!string.IsNullOrEmpty(connectedVehicleIdentification)) ? (Resources.Message_ConnectedVIN1 + connectedVehicleIdentification) : Resources.Message_NoConnectedVIN1);
		if (!string.IsNullOrEmpty(connectedVehicleIdentification) && currentVehicleIdentification != connectedVehicleIdentification)
		{
			currentVehicleIdentification = connectedVehicleIdentification;
			ConnectToVTWebsite();
		}
	}

	private void webView2VT_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
	{
		((WebView2)webView2VT).CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
		((WebView2)webView2VT).CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
		ConnectToVTWebsite();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		buttonBack = new Button();
		buttonRefresh = new Button();
		buttonForward = new Button();
		labelStatus = new System.Windows.Forms.Label();
		webView2VT = new WebView2Control();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((ISupportInitialize)webView2VT).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonBack, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonRefresh, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonForward, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 3, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(buttonBack, "buttonBack");
		buttonBack.FlatAppearance.BorderSize = 0;
		buttonBack.Name = "buttonBack";
		buttonBack.UseCompatibleTextRendering = true;
		buttonBack.UseVisualStyleBackColor = true;
		buttonBack.Click += buttonBack_Click;
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
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		((WebView2)webView2VT).AllowExternalDrop = true;
		((WebView2)webView2VT).CreationProperties = null;
		((WebView2)webView2VT).DefaultBackgroundColor = Color.White;
		componentResourceManager.ApplyResources(webView2VT, "webView2VT");
		((Control)(object)webView2VT).Name = "webView2VT";
		((WebView2)webView2VT).ZoomFactor = 1.0;
		((WebView2)webView2VT).CoreWebView2InitializationCompleted += webView2VT_CoreWebView2InitializationCompleted;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)webView2VT);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((ISupportInitialize)webView2VT).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
