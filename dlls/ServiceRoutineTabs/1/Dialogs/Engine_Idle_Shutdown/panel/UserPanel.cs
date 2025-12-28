// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown.panel;

public class UserPanel : CustomPanel
{
  private readonly string startServiceQualifier = "RT_Prevent_Engine_Shutdown_Start";
  private readonly string checkServiceQualifier = "RT_Prevent_Engine_Shutdown_Request_Results_Status";
  private readonly string stopServiceQualifier = "RT_Prevent_Engine_Shutdown_Stop";
  private Channel ecu = (Channel) null;
  private TableLayoutPanel tableMainLayout;
  private ScalingLabel labelStatus;
  private Button preventButton;
  private Button allowButton;
  private Button buttonClose;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelTestStatus;
  private System.Windows.Forms.Label label1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.allowButton.Click += new EventHandler(this.OnAllowButtonClick);
    this.preventButton.Click += new EventHandler(this.OnPreventButtonClick);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.UpdateStatus(Resources.Message_NotSupported);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  public virtual void OnChannelsChanged() => this.SetECU(this.GetChannel("CPC02T"));

  private void SetECU(Channel ecu)
  {
    if (this.ecu == ecu)
      return;
    if (this.ecu != null)
      this.ecu.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.ecu = ecu;
    if (this.ecu != null)
    {
      this.ecu.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.CheckEngineIdleShutdownStatus();
    }
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.preventButton.Enabled = this.CanStart;
    this.allowButton.Enabled = this.CanStop;
  }

  private void UpdateStatus(string status)
  {
    ((Control) this.labelStatus).Text = status;
    ((Control) this.labelStatus).Update();
  }

  private void ReportResult(string text) => this.AddStatusMessage(text);

  private Service StartService
  {
    get => this.ecu == null ? (Service) null : this.ecu.Services[this.startServiceQualifier];
  }

  private Service CheckService
  {
    get => this.ecu == null ? (Service) null : this.ecu.Services[this.checkServiceQualifier];
  }

  private Service StopService
  {
    get => this.ecu == null ? (Service) null : this.ecu.Services[this.stopServiceQualifier];
  }

  private bool CanCheckStatus => this.Online && this.CheckService != (Service) null;

  private bool Online
  {
    get => this.ecu != null && this.ecu.CommunicationsState == CommunicationsState.Online;
  }

  private bool CanStart
  {
    get
    {
      return this.Online && this.CanStop && this.StartService != (Service) null && this.CanCheckStatus;
    }
  }

  private bool CanStop => this.Online && this.StopService != (Service) null && this.CanCheckStatus;

  private void OnPreventButtonClick(object sender, EventArgs e)
  {
    if (!this.CanStart)
      return;
    Service startService = this.StartService;
    startService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnPreventComplete);
    startService.Execute(false);
    this.UpdateUserInterface();
  }

  private void OnPreventComplete(object sender, ResultEventArgs e)
  {
    ((Service) sender).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnPreventComplete);
    if (e.Succeeded)
      this.ReportResult(Resources.Message_RequestedEngineIdleShutdownPrevention);
    else
      this.ReportResult(Resources.Message_FailureWhileTryingToPreventEngineIdleShutdown + e.Exception.Message);
    this.CheckEngineIdleShutdownStatus();
  }

  private void OnCheckStatusComplete(object sender, ResultEventArgs e)
  {
    Service service = (Service) sender;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnCheckStatusComplete);
    string messageNotDefined = Resources.Message_NotDefined;
    if (service.OutputValues.Count <= 0)
      return;
    Choice itemFromRawValue = service.OutputValues[0].Value as Choice;
    byte maxValue = byte.MaxValue;
    if (itemFromRawValue != (object) null)
      maxValue = Convert.ToByte(itemFromRawValue.RawValue);
    else
      itemFromRawValue = service.OutputValues[0].Choices.GetItemFromRawValue((object) maxValue);
    if (itemFromRawValue != (object) null)
    {
      switch (maxValue)
      {
        case 0:
          messageNotDefined = itemFromRawValue.Name.ToString();
          break;
        case 1:
          messageNotDefined = itemFromRawValue.Name.ToString();
          break;
        default:
          this.ReportResult(string.Format(Resources.MessageFormat_EngineIdleShutdownPreventionHasFailed0, (object) itemFromRawValue.Name));
          messageNotDefined = itemFromRawValue.Name.ToString();
          break;
      }
    }
    this.UpdateStatus(messageNotDefined);
  }

  private void OnAllowButtonClick(object sender, EventArgs e)
  {
    if (!this.CanStop)
      return;
    Service stopService = this.StopService;
    stopService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnAllowComplete);
    stopService.Execute(false);
    this.UpdateUserInterface();
  }

  private void OnAllowComplete(object sender, ResultEventArgs e)
  {
    ((Service) sender).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnAllowComplete);
    if (e.Succeeded)
      this.ReportResult(Resources.Message_RequestedEngineIdleShutdownAllowingRequested);
    else
      this.ReportResult(Resources.Message_FailureWhileTryingToAllowEngineIdleShutdown + e.Exception.Message);
    this.CheckEngineIdleShutdownStatus();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void CheckEngineIdleShutdownStatus()
  {
    if (!this.CanCheckStatus)
      return;
    Service checkService = this.CheckService;
    checkService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnCheckStatusComplete);
    checkService.Execute(false);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableMainLayout = new TableLayoutPanel();
    this.labelTestStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonClose = new Button();
    this.label1 = new System.Windows.Forms.Label();
    this.labelStatus = new ScalingLabel();
    this.preventButton = new Button();
    this.allowButton = new Button();
    ((Control) this.tableMainLayout).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableMainLayout, "tableMainLayout");
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.labelTestStatus, 0, 1);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.buttonClose, 2, 3);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.labelStatus, 0, 2);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.preventButton, 1, 3);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.allowButton, 0, 3);
    ((Control) this.tableMainLayout).Name = "tableMainLayout";
    this.labelTestStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.labelTestStatus, 3);
    componentResourceManager.ApplyResources((object) this.labelTestStatus, "labelTestStatus");
    ((Control) this.labelTestStatus).Name = "labelTestStatus";
    this.labelTestStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.label1.BackColor = Color.White;
    this.label1.BorderStyle = BorderStyle.Fixed3D;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.label1, 3);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.ForeColor = Color.Black;
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    this.labelStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.labelStatus, 3);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.FontGroup = "";
    this.labelStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.labelStatus).Name = "labelStatus";
    componentResourceManager.ApplyResources((object) this.preventButton, "preventButton");
    this.preventButton.Name = "preventButton";
    this.preventButton.UseCompatibleTextRendering = true;
    this.preventButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.allowButton, "allowButton");
    this.allowButton.Name = "allowButton";
    this.allowButton.UseCompatibleTextRendering = true;
    this.allowButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_EngineIdleShutdown");
    ((Control) this).Controls.Add((Control) this.tableMainLayout);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableMainLayout).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
