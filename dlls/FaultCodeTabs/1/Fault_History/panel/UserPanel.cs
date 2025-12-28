// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.Fault_History.panel.UserPanel
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.Fault_History.panel;

public class UserPanel : CustomPanel
{
  private const string URLBase = "https://ddcapps.detroitdiesel.com/reservoir-dl-ext-web/faultcode";
  private const string URLVINSuffix = "?vin=";
  private const string URLPad = "**";
  private string currentVehicleIdentification;
  private string InternetNotConnected = $"<html><body><div><h1>{Resources.Message_YouAreNotConnected}</h1></div></body></html>";
  private WebBrowser webBrowserVT;
  private Button buttonBack;
  private Button buttonForward;
  private ProgressBar progressBar;
  private System.Windows.Forms.Label labelStatus;
  private TableLayoutPanel tableLayoutPanelToolBar;
  private Button buttonRefresh;

  [DllImport("WININET", CharSet = CharSet.Auto)]
  private static extern bool InternetGetConnectedState(ref int lpdwFlags, int dwReserved);

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    this.webBrowserVT.CanGoBackChanged += new EventHandler(this.webBrowserVT_CanGoBackChanged);
    this.webBrowserVT.CanGoForwardChanged += new EventHandler(this.webBrowserVT_CanGoForwardChanged);
    SapiManager.GlobalInstance.ConnectedUnitChanged += new EventHandler(this.GlobalInstance_ConnectedUnitChanged);
    this.currentVehicleIdentification = this.GetConnectedVehicleIdentification();
    this.labelStatus.Text = !string.IsNullOrEmpty(this.currentVehicleIdentification) ? Resources.Message_ConnectedVIN + this.currentVehicleIdentification : Resources.Message_NoConnectedVIN;
    this.ConnectToVTWebsite();
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void DisplayErrorPage()
  {
    this.IsVTWebsiteConnected = false;
    if (!(this.webBrowserVT.Document != (HtmlDocument) null))
      return;
    this.webBrowserVT.Document.OpenNew(true);
    this.webBrowserVT.Document.Write(this.InternetNotConnected);
  }

  private void DisplayBlankPage()
  {
    if (this.webBrowserVT.IsDisposed)
      return;
    this.webBrowserVT.Navigate("about:blank");
  }

  private void ConnectToVTWebsite()
  {
    if (this.IsInternetConnectionAvailable)
    {
      string uriString = "https://ddcapps.detroitdiesel.com/reservoir-dl-ext-web/faultcode";
      if (!string.IsNullOrEmpty(this.currentVehicleIdentification))
      {
        string str = FileEncryptionProvider.EncryptData($"**{this.currentVehicleIdentification}**{DateTime.Now.ToBinary().ToString()}");
        uriString = $"{uriString}?vin={WebUtility.UrlEncode(str).Replace("+", "%2B")}";
      }
      this.webBrowserVT.Url = new Uri(uriString, UriKind.Absolute);
    }
    else
      this.DisplayErrorPage();
  }

  private string GetConnectedVehicleIdentification()
  {
    return SapiManager.GlobalInstance.ActiveChannels.Select<Channel, string>((Func<Channel, string>) (c => SapiManager.GetVehicleIdentificationNumber(c))).Where<string>((Func<string, bool>) (v => v != null)).FirstOrDefault<string>((Func<string, bool>) (v => Utility.ValidateVehicleIdentificationNumber(v)));
  }

  public bool IsInternetConnectionAvailable
  {
    get
    {
      int lpdwFlags = 0;
      return UserPanel.InternetGetConnectedState(ref lpdwFlags, 0);
    }
  }

  public bool IsVTWebsiteConnected { get; set; }

  private void webBrowserVT_CanGoBackChanged(object sender, EventArgs e)
  {
    this.buttonBack.Enabled = this.webBrowserVT.CanGoBack;
  }

  private void webBrowserVT_CanGoForwardChanged(object sender, EventArgs e)
  {
    this.buttonForward.Enabled = this.webBrowserVT.CanGoForward;
  }

  private void buttonBack_Click(object sender, EventArgs e) => this.webBrowserVT.GoBack();

  private void buttonForward_Click(object sender, EventArgs e) => this.webBrowserVT.GoForward();

  private void buttonRefresh_Click(object sender, EventArgs e)
  {
    if (this.IsInternetConnectionAvailable)
    {
      if (this.IsVTWebsiteConnected)
        this.webBrowserVT.Refresh();
      else
        this.ConnectToVTWebsite();
    }
    else
      this.DisplayErrorPage();
  }

  private void webBrowserVT_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
  {
    if (!e.Url.AbsoluteUri.StartsWith("https://ddcapps.detroitdiesel.com/reservoir-dl-ext-web/faultcode", StringComparison.OrdinalIgnoreCase))
      return;
    this.IsVTWebsiteConnected = true;
  }

  private void GlobalInstance_ConnectedUnitChanged(object sender, EventArgs e)
  {
    string vehicleIdentification = this.GetConnectedVehicleIdentification();
    this.labelStatus.Text = !string.IsNullOrEmpty(vehicleIdentification) ? Resources.Message_ConnectedVIN + vehicleIdentification : Resources.Message_NoConnectedVIN;
    if (!string.IsNullOrEmpty(vehicleIdentification) && this.currentVehicleIdentification != vehicleIdentification)
    {
      this.currentVehicleIdentification = vehicleIdentification;
      this.ConnectToVTWebsite();
    }
    else
    {
      if (!string.IsNullOrEmpty(vehicleIdentification))
        return;
      this.currentVehicleIdentification = (string) null;
      this.DisplayBlankPage();
    }
  }

  private void webBrowserVT_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
  {
    if (e.CurrentProgress > -1L && e.MaximumProgress > -1L)
    {
      this.progressBar.Maximum = (int) e.MaximumProgress;
      this.progressBar.Value = (int) e.CurrentProgress;
    }
    this.progressBar.Visible = e.CurrentProgress != e.MaximumProgress;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.webBrowserVT = new WebBrowser();
    this.tableLayoutPanelToolBar = new TableLayoutPanel();
    this.buttonBack = new Button();
    this.labelStatus = new System.Windows.Forms.Label();
    this.buttonRefresh = new Button();
    this.buttonForward = new Button();
    this.progressBar = new ProgressBar();
    ((Control) this.tableLayoutPanelToolBar).SuspendLayout();
    ((Control) this).SuspendLayout();
    this.webBrowserVT.AllowWebBrowserDrop = false;
    componentResourceManager.ApplyResources((object) this.webBrowserVT, "webBrowserVT");
    this.webBrowserVT.Name = "webBrowserVT";
    this.webBrowserVT.Url = new Uri("", UriKind.Relative);
    this.webBrowserVT.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowserVT_DocumentCompleted);
    this.webBrowserVT.ProgressChanged += new WebBrowserProgressChangedEventHandler(this.webBrowserVT_ProgressChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelToolBar, "tableLayoutPanelToolBar");
    ((TableLayoutPanel) this.tableLayoutPanelToolBar).Controls.Add((Control) this.buttonBack, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelToolBar).Controls.Add((Control) this.labelStatus, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelToolBar).Controls.Add((Control) this.buttonRefresh, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelToolBar).Controls.Add((Control) this.buttonForward, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelToolBar).Controls.Add((Control) this.progressBar, 4, 0);
    ((Control) this.tableLayoutPanelToolBar).Name = "tableLayoutPanelToolBar";
    componentResourceManager.ApplyResources((object) this.buttonBack, "buttonBack");
    this.buttonBack.FlatAppearance.BorderSize = 0;
    this.buttonBack.Name = "buttonBack";
    this.buttonBack.UseCompatibleTextRendering = true;
    this.buttonBack.UseVisualStyleBackColor = true;
    this.buttonBack.Click += new EventHandler(this.buttonBack_Click);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonRefresh, "buttonRefresh");
    this.buttonRefresh.FlatAppearance.BorderSize = 0;
    this.buttonRefresh.Name = "buttonRefresh";
    this.buttonRefresh.UseCompatibleTextRendering = true;
    this.buttonRefresh.UseVisualStyleBackColor = true;
    this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
    componentResourceManager.ApplyResources((object) this.buttonForward, "buttonForward");
    this.buttonForward.FlatAppearance.BorderSize = 0;
    this.buttonForward.Name = "buttonForward";
    this.buttonForward.UseCompatibleTextRendering = true;
    this.buttonForward.UseVisualStyleBackColor = true;
    this.buttonForward.Click += new EventHandler(this.buttonForward_Click);
    componentResourceManager.ApplyResources((object) this.progressBar, "progressBar");
    this.progressBar.Name = "progressBar";
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.webBrowserVT);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelToolBar);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelToolBar).ResumeLayout(false);
    ((Control) this.tableLayoutPanelToolBar).PerformLayout();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
