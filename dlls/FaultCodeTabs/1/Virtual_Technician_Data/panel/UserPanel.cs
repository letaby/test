// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.Virtual_Technician_Data.panel.UserPanel
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
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

  [DllImport("WININET", CharSet = CharSet.Auto)]
  private static extern bool InternetGetConnectedState(ref int lpdwFlags, int dwReserved);

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    SapiManager.GlobalInstance.ConnectedUnitChanged += new EventHandler(this.GlobalInstance_ConnectedUnitChanged);
    this.currentVehicleIdentification = this.GetConnectedVehicleIdentification();
    this.labelStatus.Text = !string.IsNullOrEmpty(this.currentVehicleIdentification) ? Resources.Message_ConnectedVIN + this.currentVehicleIdentification : Resources.Message_NoConnectedVIN;
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void DisplayErrorPage()
  {
    this.IsVTWebsiteConnected = false;
    if (this.webView2VT == null || this.webView2VT.WebView2InitializationException != null)
      return;
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).NavigateToString(this.InternetNotConnected);
  }

  private void ConnectToVTWebsite()
  {
    if (this.IsInternetConnectionAvailable && this.webView2VT != null && ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CoreWebView2 != null)
    {
      string str = "https://hq.detroitconnect.com/DiagnosticLink/";
      if (!string.IsNullOrEmpty(this.currentVehicleIdentification))
        str = $"{str}signin?{this.currentVehicleIdentification}";
      ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CoreWebView2.Navigate(str);
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

  private void CoreWebView2_HistoryChanged(object sender, object e)
  {
    this.buttonBack.Enabled = ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CanGoBack;
    this.buttonForward.Enabled = ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CanGoForward;
  }

  private void buttonBack_Click(object sender, EventArgs e)
  {
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).GoBack();
  }

  private void buttonForward_Click(object sender, EventArgs e)
  {
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).GoForward();
  }

  private void buttonRefresh_Click(object sender, EventArgs e)
  {
    if (this.IsInternetConnectionAvailable)
    {
      if (this.IsVTWebsiteConnected)
        ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CoreWebView2.Reload();
      else
        this.ConnectToVTWebsite();
    }
    else
      this.DisplayErrorPage();
  }

  private void CoreWebView2_NavigationCompleted(
    object sender,
    CoreWebView2NavigationCompletedEventArgs e)
  {
    if (!e.IsSuccess)
      return;
    this.IsVTWebsiteConnected = true;
  }

  private void GlobalInstance_ConnectedUnitChanged(object sender, EventArgs e)
  {
    string vehicleIdentification = this.GetConnectedVehicleIdentification();
    this.labelStatus.Text = !string.IsNullOrEmpty(vehicleIdentification) ? Resources.Message_ConnectedVIN1 + vehicleIdentification : Resources.Message_NoConnectedVIN1;
    if (string.IsNullOrEmpty(vehicleIdentification) || !(this.currentVehicleIdentification != vehicleIdentification))
      return;
    this.currentVehicleIdentification = vehicleIdentification;
    this.ConnectToVTWebsite();
  }

  private void webView2VT_CoreWebView2InitializationCompleted(
    object sender,
    CoreWebView2InitializationCompletedEventArgs e)
  {
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CoreWebView2.HistoryChanged += new EventHandler<object>(this.CoreWebView2_HistoryChanged);
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CoreWebView2.NavigationCompleted += new EventHandler<CoreWebView2NavigationCompletedEventArgs>(this.CoreWebView2_NavigationCompleted);
    this.ConnectToVTWebsite();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.buttonBack = new Button();
    this.buttonRefresh = new Button();
    this.buttonForward = new Button();
    this.labelStatus = new System.Windows.Forms.Label();
    this.webView2VT = new WebView2Control();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((ISupportInitialize) this.webView2VT).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonBack, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonRefresh, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonForward, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 3, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.buttonBack, "buttonBack");
    this.buttonBack.FlatAppearance.BorderSize = 0;
    this.buttonBack.Name = "buttonBack";
    this.buttonBack.UseCompatibleTextRendering = true;
    this.buttonBack.UseVisualStyleBackColor = true;
    this.buttonBack.Click += new EventHandler(this.buttonBack_Click);
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
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).AllowExternalDrop = true;
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CreationProperties = (CoreWebView2CreationProperties) null;
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).DefaultBackgroundColor = Color.White;
    componentResourceManager.ApplyResources((object) this.webView2VT, "webView2VT");
    ((Control) this.webView2VT).Name = "webView2VT";
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).ZoomFactor = 1.0;
    ((Microsoft.Web.WebView2.WinForms.WebView2) this.webView2VT).CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(this.webView2VT_CoreWebView2InitializationCompleted);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.webView2VT);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((ISupportInitialize) this.webView2VT).EndInit();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
