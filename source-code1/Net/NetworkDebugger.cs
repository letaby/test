// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Net.NetworkDebugger
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.OpenIdConnect;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
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

#nullable disable
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
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.webBrowserList.SetWriterFunction((Action<XmlWriter>) (writer => this.inputPane = this.webBrowserList.GetElementFromPoint(new Point(0, 0)).Document.All.GetElementsByName("inputpane")[0]));
  }

  private void buttonTestNetwork_Click(object sender, EventArgs e)
  {
    this.buttonTestNetwork.Enabled = this.buttonCopyToClipboard.Enabled = this.buttonClose.Enabled = false;
    this.inputPane.InnerHtml = string.Empty;
    this.backgroundWorker.RunWorkerAsync();
  }

  private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
  {
    bool flag = false;
    bool useOidc = NetworkSettings.GlobalInstance.UseOidc;
    this.CheckSettings("DLBrokerServer", useOidc ? "dlbroker-dtna-oidc.prd.freightliner.com" : "dtna-DlrInfo.prd.freightliner.com", NetworkSettings.GlobalInstance.DLBrokerServer);
    this.CheckSettings("DLBrokerPort", useOidc ? (ushort) 443.ToString((IFormatProvider) CultureInfo.CurrentCulture) : (ushort) 48481.ToString((IFormatProvider) CultureInfo.CurrentCulture), NetworkSettings.GlobalInstance.DLBrokerPort.ToString((IFormatProvider) CultureInfo.CurrentCulture));
    this.CheckSettings("LogOnLocation", useOidc ? "https://idp-dtna.prd.freightliner.com" : "https://dtna-iservices2.prd.freightliner.com/AuthN-Basic-SM", NetworkSettings.GlobalInstance.LogOnLocation);
    this.CheckSettings("TechlaneLocation", useOidc ? "https://techlane-dtna-oidc.prd.freightliner.com/techlane" : "https://techlane-dtna.prd.freightliner.com", NetworkSettings.GlobalInstance.TechlaneLocation);
    this.CheckSettings("EdexPersistServiceLocation", useOidc ? "https://edex-persist-dtna-oidc.prd.freightliner.com/edex-persist-service" : "https://edex.daimler.com/", NetworkSettings.GlobalInstance.EdexServiceLocation);
    this.CheckSettings("EdexFileStoreLocation", useOidc ? "https://edexfs-dtna-oidc.prd.freightliner.com/edex-file-store-service" : "https://dtna-iservices2.prd.freightliner.com", NetworkSettings.GlobalInstance.EdexFileStoreLocation);
    if (!useOidc && this.CheckForInternetConnection(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://{0}:{1}/DiagnosticLinkAuthN?WSDL", (object) NetworkSettings.GlobalInstance.DLBrokerServer, (object) NetworkSettings.GlobalInstance.DLBrokerPort)))
    {
      flag = true;
      this.PingHost(NetworkSettings.GlobalInstance.DLBrokerServer);
    }
    if (this.CheckLogonAuthentication())
    {
      flag = true;
      this.PingHost(NetworkSettings.GlobalInstance.LogOnLocation);
    }
    if (flag)
      this.CheckForInternetConnection("http://google.com/generate_204");
    e.Result = (object) !flag;
  }

  private void AddListItem(string action, string settingName, Color foreColor, string result)
  {
    this.backgroundWorker.ReportProgress(0, (object) Tuple.Create<string, string, string, Color>(action, settingName, result, foreColor));
  }

  private void CheckSettings(string settingName, string defaultValue, string currentValue)
  {
    this.AddListItem(Resources.Message_NetworkDebuggerVerifySetting, settingName, defaultValue != currentValue ? Color.Orange : Color.Green, defaultValue != currentValue ? Resources.Message_NetworkDebuggerSettingNotDefault : Resources.Message_NetworkDebuggerOK);
  }

  private bool CheckForInternetConnection(string siteAddress)
  {
    this.AddListItem(Resources.Message_NetworkDebuggerConnectTo, siteAddress, Color.Black, Resources.Message_NetworkDebuggerInProgress);
    bool flag = true;
    string str = Resources.Message_NetworkDebuggerFailure;
    try
    {
      using (WebClient webClient = new WebClient())
      {
        using (webClient.OpenRead(siteAddress))
          flag = false;
      }
    }
    catch (Exception ex) when (ex is ArgumentNullException || ex is WebException)
    {
      str = NetworkDebugger.GetInnermostException(ex).Message;
    }
    this.AddListItem(Resources.Message_NetworkDebuggerConnectTo, siteAddress, flag ? Color.Red : Color.Green, flag ? str : Resources.Message_NetworkDebuggerOK);
    return flag;
  }

  private bool CheckLogonAuthentication()
  {
    this.AddListItem(Resources.Message_NetworkDebuggerConnectTo, NetworkSettings.GlobalInstance.LogOnLocation, Color.Black, Resources.Message_NetworkDebuggerInProgress);
    bool flag = true;
    string str = Resources.Message_NetworkDebuggerFailure;
    if (NetworkSettings.GlobalInstance.UseOidc)
    {
      str = OidcAuthorization.TestOidcConnection();
      flag = !string.IsNullOrEmpty(str);
    }
    else
    {
      try
      {
        Task<HttpResponseMessage> task = Task.Run<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () => await SiteMinderAuthentication.Authenticate(false)));
        task.Wait();
        if (!task.Result.IsSuccessStatusCode)
        {
          if (task.Result.StatusCode != HttpStatusCode.Unauthorized)
            goto label_7;
        }
        flag = false;
      }
      catch (Exception ex) when (ex is AggregateException || ex is HttpRequestException)
      {
        str = NetworkDebugger.GetInnermostException(ex).Message;
      }
    }
label_7:
    this.AddListItem(Resources.Message_NetworkDebuggerConnectTo, NetworkSettings.GlobalInstance.LogOnLocation, flag ? Color.Red : Color.Green, flag ? str : Resources.Message_NetworkDebuggerOK);
    return flag;
  }

  public void PingHost(string nameOrAddress)
  {
    this.AddListItem(Resources.Message_NetworkDebuggerPing, nameOrAddress, Color.Black, Resources.Message_NetworkDebuggerInProgress);
    bool flag = true;
    string str = Resources.Message_NetworkDebuggerFailure;
    try
    {
      Uri uri = new Uri(nameOrAddress);
      using (Ping ping = new Ping())
        flag = ping.Send(uri.Host).Status != 0;
    }
    catch (Exception ex) when (
    {
      // ISSUE: unable to correctly present filter
      int num;
      switch (ex)
      {
        case ArgumentNullException _:
        case NotSupportedException _:
        case InvalidOperationException _:
        case PingException _:
          num = 1;
          break;
        default:
          num = ex is UriFormatException ? 1 : 0;
          break;
      }
      if ((uint) num > 0U)
      {
        SuccessfulFiltering;
      }
      else
        throw;
    }
    )
    {
      str = NetworkDebugger.GetInnermostException(ex).Message;
    }
    this.AddListItem(Resources.Message_NetworkDebuggerPing, nameOrAddress, flag ? Color.Red : Color.Green, flag ? str : Resources.Message_NetworkDebuggerOK);
  }

  private void buttonCopyToClipboard_Click(object sender, EventArgs e)
  {
    Clipboard.SetText(this.inputPane.InnerText);
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, Resources.Message_NetworkDebuggerMessageBox, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
  }

  private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
  {
    if (this.lastColor == Color.Black)
      this.inputPane.Children[this.inputPane.Children.Count - 1].OuterHtml = string.Empty;
    Tuple<string, string, string, Color> userState = (Tuple<string, string, string, Color>) e.UserState;
    this.inputPane.InnerHtml += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<div><div>{0} <i>{1}</i></div><div style='margin-left:25px; color:{3}'><span style='color:black'>&#10140; </span>{2}</div></div>", (object) userState.Item1, (object) userState.Item2, (object) userState.Item3, (object) ColorTranslator.ToHtml(userState.Item4));
    this.lastColor = userState.Item4;
  }

  private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    bool result = (bool) e.Result;
    string str = !result ? Resources.Message_NetworkDebuggerCompleteIssuesFound : Resources.Message_NetworkDebuggerCompleteNoIssuesFound;
    this.inputPane.InnerHtml += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<p style='color:{0}'><b>{1}</b></p>", (object) ColorTranslator.ToHtml(result ? Color.Green : Color.Red), (object) str);
    this.buttonTestNetwork.Enabled = this.buttonCopyToClipboard.Enabled = this.buttonClose.Enabled = true;
  }

  private static Exception GetInnermostException(Exception exception)
  {
    return exception.InnerException == null ? exception : NetworkDebugger.GetInnermostException(exception.InnerException);
  }

  private void NetworkDebugger_FormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.backgroundWorker.IsBusy;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (NetworkDebugger));
    this.buttonClose = new Button();
    this.buttonTestNetwork = new Button();
    this.buttonCopyToClipboard = new Button();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.webBrowserList = new WebBrowserList();
    this.backgroundWorker = new BackgroundWorker();
    this.tableLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.Cancel;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonTestNetwork, "buttonTestNetwork");
    this.buttonTestNetwork.Name = "buttonTestNetwork";
    this.buttonTestNetwork.UseVisualStyleBackColor = true;
    this.buttonTestNetwork.Click += new EventHandler(this.buttonTestNetwork_Click);
    componentResourceManager.ApplyResources((object) this.buttonCopyToClipboard, "buttonCopyToClipboard");
    this.buttonCopyToClipboard.Name = "buttonCopyToClipboard";
    this.buttonCopyToClipboard.UseVisualStyleBackColor = true;
    this.buttonCopyToClipboard.Click += new EventHandler(this.buttonCopyToClipboard_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonClose, 1, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonTestNetwork, 1, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonCopyToClipboard, 1, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.webBrowserList, 0, 0);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.webBrowserList, "webBrowserList");
    ((Control) this.webBrowserList).Name = "webBrowserList";
    this.tableLayoutPanel1.SetRowSpan((Control) this.webBrowserList, 4);
    this.backgroundWorker.DoWork += new DoWorkEventHandler(this.backgroundWorker_DoWork);
    this.backgroundWorker.WorkerReportsProgress = true;
    this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
    this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonClose;
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (NetworkDebugger);
    this.ShowInTaskbar = false;
    this.FormClosing += new FormClosingEventHandler(this.NetworkDebugger_FormClosing);
    this.tableLayoutPanel1.ResumeLayout(false);
    this.ResumeLayout(false);
  }
}
