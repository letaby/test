using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.OpenIdConnect;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

namespace DetroitDiesel.Net;

public class NetworkDebugger : Form
{
	private HtmlElement inputPane;

	private Color lastColor;

	private IContainer components;

	private Button buttonClose;

	private Button buttonTestNetwork;

	private Button buttonCopyToClipboard;

	private TableLayoutPanel tableLayoutPanel1;

	private WebBrowserList webBrowserList;

	private BackgroundWorker backgroundWorker;

	public NetworkDebugger()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		webBrowserList.SetWriterFunction((Action<XmlWriter>)delegate
		{
			inputPane = webBrowserList.GetElementFromPoint(new Point(0, 0)).Document.All.GetElementsByName("inputpane")[0];
		});
	}

	private void buttonTestNetwork_Click(object sender, EventArgs e)
	{
		Button button = buttonTestNetwork;
		Button button2 = buttonCopyToClipboard;
		bool flag = (buttonClose.Enabled = false);
		bool enabled = (button2.Enabled = flag);
		button.Enabled = enabled;
		inputPane.InnerHtml = string.Empty;
		backgroundWorker.RunWorkerAsync();
	}

	private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		bool flag = false;
		bool useOidc = NetworkSettings.GlobalInstance.UseOidc;
		CheckSettings("DLBrokerServer", useOidc ? "dlbroker-dtna-oidc.prd.freightliner.com" : "dtna-DlrInfo.prd.freightliner.com", NetworkSettings.GlobalInstance.DLBrokerServer);
		CheckSettings("DLBrokerPort", useOidc ? ((ushort)443).ToString(CultureInfo.CurrentCulture) : ((ushort)48481).ToString(CultureInfo.CurrentCulture), NetworkSettings.GlobalInstance.DLBrokerPort.ToString(CultureInfo.CurrentCulture));
		CheckSettings("LogOnLocation", useOidc ? "https://idp-dtna.prd.freightliner.com" : "https://dtna-iservices2.prd.freightliner.com/AuthN-Basic-SM", NetworkSettings.GlobalInstance.LogOnLocation);
		CheckSettings("TechlaneLocation", useOidc ? "https://techlane-dtna-oidc.prd.freightliner.com/techlane" : "https://techlane-dtna.prd.freightliner.com", NetworkSettings.GlobalInstance.TechlaneLocation);
		CheckSettings("EdexPersistServiceLocation", useOidc ? "https://edex-persist-dtna-oidc.prd.freightliner.com/edex-persist-service" : "https://edex.daimler.com/", NetworkSettings.GlobalInstance.EdexServiceLocation);
		CheckSettings("EdexFileStoreLocation", useOidc ? "https://edexfs-dtna-oidc.prd.freightliner.com/edex-file-store-service" : "https://dtna-iservices2.prd.freightliner.com", NetworkSettings.GlobalInstance.EdexFileStoreLocation);
		if (!useOidc)
		{
			string siteAddress = string.Format(CultureInfo.InvariantCulture, "https://{0}:{1}/DiagnosticLinkAuthN?WSDL", NetworkSettings.GlobalInstance.DLBrokerServer, NetworkSettings.GlobalInstance.DLBrokerPort);
			if (CheckForInternetConnection(siteAddress))
			{
				flag = true;
				PingHost(NetworkSettings.GlobalInstance.DLBrokerServer);
			}
		}
		if (CheckLogonAuthentication())
		{
			flag = true;
			PingHost(NetworkSettings.GlobalInstance.LogOnLocation);
		}
		if (flag)
		{
			CheckForInternetConnection("http://google.com/generate_204");
		}
		e.Result = !flag;
	}

	private void AddListItem(string action, string settingName, Color foreColor, string result)
	{
		backgroundWorker.ReportProgress(0, Tuple.Create(action, settingName, result, foreColor));
	}

	private void CheckSettings(string settingName, string defaultValue, string currentValue)
	{
		AddListItem(Resources.Message_NetworkDebuggerVerifySetting, settingName, (defaultValue != currentValue) ? Color.Orange : Color.Green, (defaultValue != currentValue) ? Resources.Message_NetworkDebuggerSettingNotDefault : Resources.Message_NetworkDebuggerOK);
	}

	private bool CheckForInternetConnection(string siteAddress)
	{
		AddListItem(Resources.Message_NetworkDebuggerConnectTo, siteAddress, Color.Black, Resources.Message_NetworkDebuggerInProgress);
		bool flag = true;
		string text = Resources.Message_NetworkDebuggerFailure;
		try
		{
			using WebClient webClient = new WebClient();
			using (webClient.OpenRead(siteAddress))
			{
				flag = false;
			}
		}
		catch (Exception ex) when (ex is ArgumentNullException || ex is WebException)
		{
			text = GetInnermostException(ex).Message;
		}
		AddListItem(Resources.Message_NetworkDebuggerConnectTo, siteAddress, flag ? Color.Red : Color.Green, flag ? text : Resources.Message_NetworkDebuggerOK);
		return flag;
	}

	private bool CheckLogonAuthentication()
	{
		AddListItem(Resources.Message_NetworkDebuggerConnectTo, NetworkSettings.GlobalInstance.LogOnLocation, Color.Black, Resources.Message_NetworkDebuggerInProgress);
		bool flag = true;
		string text = Resources.Message_NetworkDebuggerFailure;
		if (NetworkSettings.GlobalInstance.UseOidc)
		{
			text = OidcAuthorization.TestOidcConnection();
			flag = !string.IsNullOrEmpty(text);
		}
		else
		{
			try
			{
				Task<HttpResponseMessage> task = Task.Run(async () => await SiteMinderAuthentication.Authenticate(false));
				task.Wait();
				if (task.Result.IsSuccessStatusCode || task.Result.StatusCode == HttpStatusCode.Unauthorized)
				{
					flag = false;
				}
			}
			catch (Exception ex) when (ex is AggregateException || ex is HttpRequestException)
			{
				text = GetInnermostException(ex).Message;
			}
		}
		AddListItem(Resources.Message_NetworkDebuggerConnectTo, NetworkSettings.GlobalInstance.LogOnLocation, flag ? Color.Red : Color.Green, flag ? text : Resources.Message_NetworkDebuggerOK);
		return flag;
	}

	public void PingHost(string nameOrAddress)
	{
		AddListItem(Resources.Message_NetworkDebuggerPing, nameOrAddress, Color.Black, Resources.Message_NetworkDebuggerInProgress);
		bool flag = true;
		string text = Resources.Message_NetworkDebuggerFailure;
		try
		{
			Uri uri = new Uri(nameOrAddress);
			using Ping ping = new Ping();
			PingReply pingReply = ping.Send(uri.Host);
			flag = pingReply.Status != IPStatus.Success;
		}
		catch (Exception ex) when (ex is ArgumentNullException || ex is NotSupportedException || ex is InvalidOperationException || ex is PingException || ex is UriFormatException)
		{
			text = GetInnermostException(ex).Message;
		}
		AddListItem(Resources.Message_NetworkDebuggerPing, nameOrAddress, flag ? Color.Red : Color.Green, flag ? text : Resources.Message_NetworkDebuggerOK);
	}

	private void buttonCopyToClipboard_Click(object sender, EventArgs e)
	{
		Clipboard.SetText(inputPane.InnerText);
		ControlHelpers.ShowMessageBox((Control)this, Resources.Message_NetworkDebuggerMessageBox, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
	}

	private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		if (lastColor == Color.Black)
		{
			inputPane.Children[inputPane.Children.Count - 1].OuterHtml = string.Empty;
		}
		Tuple<string, string, string, Color> tuple = (Tuple<string, string, string, Color>)e.UserState;
		inputPane.InnerHtml += string.Format(CultureInfo.InvariantCulture, "<div><div>{0} <i>{1}</i></div><div style='margin-left:25px; color:{3}'><span style='color:black'>&#10140; </span>{2}</div></div>", tuple.Item1, tuple.Item2, tuple.Item3, ColorTranslator.ToHtml(tuple.Item4));
		lastColor = tuple.Item4;
	}

	private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		bool flag = (bool)e.Result;
		string arg = ((!flag) ? Resources.Message_NetworkDebuggerCompleteIssuesFound : Resources.Message_NetworkDebuggerCompleteNoIssuesFound);
		inputPane.InnerHtml += string.Format(CultureInfo.InvariantCulture, "<p style='color:{0}'><b>{1}</b></p>", ColorTranslator.ToHtml(flag ? Color.Green : Color.Red), arg);
		Button button = buttonTestNetwork;
		Button button2 = buttonCopyToClipboard;
		bool flag2 = (buttonClose.Enabled = true);
		bool enabled = (button2.Enabled = flag2);
		button.Enabled = enabled;
	}

	private static Exception GetInnermostException(Exception exception)
	{
		if (exception.InnerException == null)
		{
			return exception;
		}
		return GetInnermostException(exception.InnerException);
	}

	private void NetworkDebugger_FormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = backgroundWorker.IsBusy;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Net.NetworkDebugger));
		this.buttonClose = new System.Windows.Forms.Button();
		this.buttonTestNetwork = new System.Windows.Forms.Button();
		this.buttonCopyToClipboard = new System.Windows.Forms.Button();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.webBrowserList = new WebBrowserList();
		this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.buttonClose, "buttonClose");
		this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonClose.Name = "buttonClose";
		this.buttonClose.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.buttonTestNetwork, "buttonTestNetwork");
		this.buttonTestNetwork.Name = "buttonTestNetwork";
		this.buttonTestNetwork.UseVisualStyleBackColor = true;
		this.buttonTestNetwork.Click += new System.EventHandler(buttonTestNetwork_Click);
		resources.ApplyResources(this.buttonCopyToClipboard, "buttonCopyToClipboard");
		this.buttonCopyToClipboard.Name = "buttonCopyToClipboard";
		this.buttonCopyToClipboard.UseVisualStyleBackColor = true;
		this.buttonCopyToClipboard.Click += new System.EventHandler(buttonCopyToClipboard_Click);
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(this.buttonClose, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.buttonTestNetwork, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.buttonCopyToClipboard, 1, 1);
		this.tableLayoutPanel1.Controls.Add((System.Windows.Forms.Control)(object)this.webBrowserList, 0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		resources.ApplyResources(this.webBrowserList, "webBrowserList");
		((System.Windows.Forms.Control)(object)this.webBrowserList).Name = "webBrowserList";
		this.tableLayoutPanel1.SetRowSpan((System.Windows.Forms.Control)(object)this.webBrowserList, 4);
		this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
		this.backgroundWorker.WorkerReportsProgress = true;
		this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
		this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonClose;
		base.Controls.Add(this.tableLayoutPanel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "NetworkDebugger";
		base.ShowInTaskbar = false;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(NetworkDebugger_FormClosing);
		this.tableLayoutPanel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
