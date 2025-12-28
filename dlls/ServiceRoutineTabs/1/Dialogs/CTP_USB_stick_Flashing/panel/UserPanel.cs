// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_USB_stick_Flashing.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_USB_stick_Flashing.panel;

public class UserPanel : CustomPanel
{
  private const string ReprogrammingFlashSeedServiceQualifier = "DJ_Reprogramming_Flash_Seed";
  private const string SoftwareUpdateViaUsbStartStatusServiceQualifier = "RT_Shutdown_Software_Update_via_USB_Start_RoutineStartStatus";
  private const string EcuMaxFunctionalQualifier = "SES_Programming_P2_CAN_ECU_max_physical";
  private const int ValidStartStatusService = 0;
  private const string ResultsServiceQualifier = "RT_Shutdown_Software_Update_via_USB_Request_Results_ShutdownUpdateStatus";
  private const int ShutdownUpdateComplete = 5;
  private const string HardResetQualifier = "FN_HardReset";
  private const string HardwarePartNumberQualifier = "CO_HardwarePartNumber";
  private const string SoftwarePartNumberQualifier = "CO_SoftwarePartNumber";
  private readonly int[] ValidShutdownUpdateStatus = new int[5]
  {
    0,
    1,
    3,
    4,
    5
  };
  private readonly List<string> ValidHardwarePartNumbers = new List<string>()
  {
    "66-10777-001",
    "66-13928-001",
    "66-13931-001",
    "66-19901-003",
    "66-19901-001",
    "66-19901-501",
    "66-19901-503",
    "66-05466-001",
    "66-13931-501"
  };
  private Channel ctp;
  private ProcessState state = ProcessState.NotRunning;
  private Timer monitoringTimer;
  private string origionalSoftwarePartNumber = string.Empty;
  private string currentSoftwarePartNumber = string.Empty;
  private string currentHardwarePartNumber = string.Empty;
  private TableLayoutPanel tableLayoutPanelMain;
  private ScalingLabel scalingLabelStatus;
  private Checkmark checkmarkStatus;
  private ProgressBar progressBarMarquee;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private SeekTimeListView seekTimeListView;
  private ScalingLabel scalingLabelConnect;
  private Checkmark checkmarkConnect;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private Button buttonStart;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private System.Windows.Forms.Label labelWarning;

  public bool CtpBusy
  {
    get => this.ctp != null && this.ctp.CommunicationsState != CommunicationsState.Online;
  }

  public bool ProcessRunning
  {
    get => this.state != ProcessState.NotRunning && this.state != ProcessState.Complete;
  }

  public bool CanFlash => !this.ProcessRunning && this.ValidHardware;

  public bool ValidHardware
  {
    get => this.ValidHardwarePartNumbers.Contains(this.CurrentHardwarePartNumber);
  }

  public string CurrentSoftwarePartNumber
  {
    get
    {
      return string.IsNullOrEmpty(this.currentSoftwarePartNumber) ? (string) null : this.currentSoftwarePartNumber;
    }
    set
    {
      if (value == null)
        return;
      this.currentSoftwarePartNumber = value;
    }
  }

  public string CurrentHardwarePartNumber
  {
    get
    {
      return string.IsNullOrEmpty(this.currentHardwarePartNumber) ? (string) null : this.currentHardwarePartNumber;
    }
    set
    {
      if (value == null)
        return;
      this.currentHardwarePartNumber = value;
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.progressBarMarquee.Hide();
    this.monitoringTimer = new Timer();
    this.monitoringTimer.Interval = 5000;
    this.monitoringTimer.Tick += new EventHandler(this.timer_Tick);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && this.ProcessRunning)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetCTP((Channel) null);
    this.monitoringTimer.Stop();
    this.monitoringTimer.Tick -= new EventHandler(this.timer_Tick);
    this.monitoringTimer.Dispose();
  }

  private void AddLogLabel(string text, bool updateStatus)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
    if (updateStatus)
      ((Control) this.scalingLabelStatus).Text = text;
  }

  public virtual void OnChannelsChanged()
  {
    this.SetCTP(this.GetChannel("CTP01T", (CustomPanel.ChannelLookupOptions) 3));
  }

  private void SetCTP(Channel ctp)
  {
    if (this.ctp != ctp)
    {
      if (this.ctp != null)
      {
        this.ctp.EcuInfos.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.EcuInfos_EcuInfoUpdateEvent);
        this.ctp.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.ctp_CommunicationsStateUpdateEvent);
        if (this.ProcessRunning)
          this.Abort("CTP Disconnected");
        this.state = ProcessState.NotRunning;
      }
      this.ctp = ctp;
      if (this.ctp != null)
      {
        this.state = ProcessState.NotRunning;
        this.ctp.EcuInfos.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.EcuInfos_EcuInfoUpdateEvent);
        this.ctp.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.ctp_CommunicationsStateUpdateEvent);
        this.UpdatePartNumbers();
      }
    }
    this.UpdateUserInterface();
  }

  private void EcuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdatePartNumbers();
    this.UpdateUserInterface();
  }

  private void UpdatePartNumbers()
  {
    this.CurrentSoftwarePartNumber = this.ReadEcuInfoData("CO_SoftwarePartNumber");
    this.CurrentHardwarePartNumber = this.ReadEcuInfoData("CO_HardwarePartNumber");
  }

  private void UpdateUserInterface()
  {
    if (this.ctp != null)
    {
      ((Control) this.scalingLabelConnect).Text = Resources.Message_ConnectedToCTP;
      this.checkmarkConnect.CheckState = CheckState.Checked;
      if (this.state == ProcessState.NotRunning)
      {
        if (this.CtpBusy)
        {
          this.checkmarkStatus.CheckState = CheckState.Indeterminate;
          ((Control) this.scalingLabelStatus).Text = Resources.Message_CTPBusy;
        }
        else if (!this.ValidHardware)
        {
          this.checkmarkStatus.CheckState = CheckState.Unchecked;
          ((Control) this.scalingLabelStatus).Text = Resources.Message_InvalidHardwarePartNumber;
        }
        else if (this.CanFlash)
        {
          this.checkmarkStatus.CheckState = CheckState.Checked;
          ((Control) this.scalingLabelStatus).Text = Resources.Message_ReadyToStart;
        }
      }
    }
    else
    {
      ((Control) this.scalingLabelConnect).Text = Resources.Message_NotConnectedToCTP;
      this.checkmarkConnect.CheckState = CheckState.Unchecked;
    }
    this.buttonStart.Enabled = this.ctp != null && !this.ProcessRunning && !this.CtpBusy && this.CanFlash;
    this.buttonClose.Enabled = !this.ProcessRunning;
  }

  private void GoMachine()
  {
    switch (this.state)
    {
      case ProcessState.StartHardReset1:
        this.origionalSoftwarePartNumber = this.CurrentSoftwarePartNumber;
        this.checkmarkStatus.CheckState = CheckState.Indeterminate;
        this.progressBarMarquee.Show();
        this.AddLogLabel(Resources.Message_ResettingCTPBeforeFlashing, true);
        this.ExecuteService("FN_HardReset", new ServiceCompleteEventHandler(this.hardResetService_ServiceCompleteEvent));
        break;
      case ProcessState.WaitingforResetToFinish1:
        this.AddLogLabel(Resources.Message_WaitingForCTPToReset, true);
        break;
      case ProcessState.SetMaxFunctional:
        this.AddLogLabel("Set Max Functional", true);
        this.ExecuteService("SES_Programming_P2_CAN_ECU_max_physical", new ServiceCompleteEventHandler(this.Services_MaxFunctionalServiceCompleteEvent));
        break;
      case ProcessState.ReprogrammingFlashSeed:
        this.AddLogLabel(Resources.Message_Unlocking, true);
        this.ExecuteService("DJ_Reprogramming_Flash_Seed", new ServiceCompleteEventHandler(this.Services_FlashSeedServiceCompleteEvent));
        break;
      case ProcessState.StartReprogramming:
        this.AddLogLabel(Resources.Message_Reprogramming, true);
        this.ExecuteService("RT_Shutdown_Software_Update_via_USB_Start_RoutineStartStatus", new ServiceCompleteEventHandler(this.Services_StartReprogrammingServiceCompleteEvent));
        break;
      case ProcessState.MonitorReprogramming:
        this.AddLogLabel(Resources.Message_CopyingData, true);
        this.monitoringTimer.Start();
        break;
      case ProcessState.StartHardReset2:
        this.AddLogLabel(Resources.Message_ResettingCTPAfterFlashing, true);
        this.ExecuteService("FN_HardReset", new ServiceCompleteEventHandler(this.hardResetService_ServiceCompleteEvent));
        break;
      case ProcessState.WaitingforResetToFinish2:
        this.AddLogLabel(Resources.Message_WaitingForCTPToReset1, true);
        break;
      case ProcessState.Complete:
        this.progressBarMarquee.Hide();
        this.checkmarkStatus.CheckState = CheckState.Checked;
        this.AddStationLogEntry();
        this.AddLogLabel(Resources.Message_FlashingComplete, true);
        this.UpdateUserInterface();
        break;
    }
    this.UpdateUserInterface();
  }

  private void AddStationLogEntry()
  {
    ServerDataManager.UpdateEventsFile(this.ctp, (IDictionary<string, string>) new Dictionary<string, string>()
    {
      ["reasontext"] = "ReasonCTPUSBStickFlashing"
    }, (IDictionary<string, string>) new Dictionary<string, string>()
    {
      ["Old_SW_PartNumber"] = this.origionalSoftwarePartNumber,
      ["New_SW_PartNumber"] = this.CurrentSoftwarePartNumber
    }, "CTPUSBStickFlashing", string.Empty, this.ReadEcuInfoData("CO_VIN"), "OK", "DESCRIPTION", string.Empty);
  }

  private string ReadEcuInfoData(string qualifier)
  {
    string str = string.Empty;
    if (this.ctp != null)
    {
      EcuInfo ecuInfo = this.ctp.EcuInfos[qualifier] ?? this.ctp.EcuInfos.GetItemContaining(qualifier);
      if (ecuInfo != null)
        str = ecuInfo.Value.ToString().Trim();
    }
    return str.Trim();
  }

  private void ExecuteService(
    string serviceQualifier,
    ServiceCompleteEventHandler serviceCompleteEvent)
  {
    Service service = this.ctp.Services[serviceQualifier];
    if (service != (Service) null)
    {
      if (serviceCompleteEvent != null)
        service.ServiceCompleteEvent += serviceCompleteEvent;
      service.Execute(false);
    }
    else
      this.Abort(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_ServiceDoesNotExist0, (object) serviceQualifier));
  }

  private static void RemoveServiceComplete(
    Service service,
    ServiceCompleteEventHandler serviceCompleteEventHandler)
  {
    if (!(service != (Service) null))
      return;
    service.ServiceCompleteEvent -= serviceCompleteEventHandler;
  }

  private void timer_Tick(object sender, EventArgs e)
  {
    if (this.ctp == null || this.state != ProcessState.MonitorReprogramming || this.CtpBusy)
      return;
    this.ExecuteService("RT_Shutdown_Software_Update_via_USB_Request_Results_ShutdownUpdateStatus", new ServiceCompleteEventHandler(this.Services_MonitoringServiceCompleteEvent));
  }

  private void Services_MaxFunctionalServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    UserPanel.RemoveServiceComplete(sender as Service, new ServiceCompleteEventHandler(this.Services_MaxFunctionalServiceCompleteEvent));
    if (e.Succeeded)
    {
      ++this.state;
      this.GoMachine();
    }
    else
      this.Abort(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_MaxFunctionalServiceFailed0, (object) e.Exception.Message));
  }

  private void Services_FlashSeedServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    UserPanel.RemoveServiceComplete(sender as Service, new ServiceCompleteEventHandler(this.Services_FlashSeedServiceCompleteEvent));
    ++this.state;
    this.GoMachine();
  }

  private void Services_StartReprogrammingServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    UserPanel.RemoveServiceComplete(sender as Service, new ServiceCompleteEventHandler(this.Services_StartReprogrammingServiceCompleteEvent));
    if (e.Succeeded)
    {
      Service service = sender as Service;
      if (service != (Service) null && (int) ((Choice) service.OutputValues[0].Value).RawValue == 0)
      {
        ++this.state;
        this.GoMachine();
      }
      else
        this.Abort(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.Message_CouldNotStartReprogrammingCheckUSBDrive));
    }
    else
      this.Abort(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_CouldNotStartReprogramming0, (object) e.Exception.Message));
  }

  private void Services_MonitoringServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    UserPanel.RemoveServiceComplete(service, new ServiceCompleteEventHandler(this.Services_MonitoringServiceCompleteEvent));
    if (e.Succeeded)
    {
      Choice choice = (Choice) service.OutputValues[0].Value;
      this.AddLogLabel(choice.ToString(), false);
      int rawValue = (int) choice.RawValue;
      if (((IEnumerable<int>) this.ValidShutdownUpdateStatus).Contains<int>(rawValue))
      {
        if (5 != rawValue)
          return;
        this.monitoringTimer.Stop();
        ++this.state;
        this.GoMachine();
      }
      else
        this.Abort(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_FlashFailed0, (object) service.OutputValues[0].Value.ToString()));
    }
    else
      this.Abort(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_FlashFailed01, (object) e.Exception.Message));
  }

  private void Abort(string reason)
  {
    this.progressBarMarquee.Hide();
    this.state = ProcessState.NotRunning;
    this.AddLogLabel(Resources.Message_Failed + reason, true);
    this.checkmarkStatus.CheckState = CheckState.Unchecked;
    this.UpdatePartNumbers();
    this.UpdateUserInterface();
  }

  private void hardResetService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    UserPanel.RemoveServiceComplete(sender as Service, new ServiceCompleteEventHandler(this.hardResetService_ServiceCompleteEvent));
    if (e.Succeeded)
    {
      ++this.state;
      this.GoMachine();
    }
    else
      this.Abort(Resources.Message_HardResetFailed);
  }

  private void ctp_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdatePartNumbers();
    this.UpdateUserInterface();
    if (e.CommunicationsState != CommunicationsState.Online || this.state != ProcessState.WaitingforResetToFinish1 && this.state != ProcessState.WaitingforResetToFinish2)
      return;
    ++this.state;
    this.GoMachine();
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    this.state = ProcessState.StartHardReset1;
    this.GoMachine();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.scalingLabelStatus = new ScalingLabel();
    this.checkmarkStatus = new Checkmark();
    this.scalingLabelConnect = new ScalingLabel();
    this.buttonClose = new Button();
    this.seekTimeListView = new SeekTimeListView();
    this.checkmarkConnect = new Checkmark();
    this.progressBarMarquee = new ProgressBar();
    this.buttonStart = new Button();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.labelWarning = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.scalingLabelStatus, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.checkmarkStatus, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.scalingLabelConnect, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 4, 7);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListView, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.checkmarkConnect, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.progressBarMarquee, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonStart, 3, 7);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrument1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrument3, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelWarning, 0, 5);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    this.scalingLabelStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.scalingLabelStatus, 4);
    componentResourceManager.ApplyResources((object) this.scalingLabelStatus, "scalingLabelStatus");
    this.scalingLabelStatus.FontGroup = (string) null;
    this.scalingLabelStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelStatus).Name = "scalingLabelStatus";
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    this.checkmarkStatus.IndeterminateImage = (Image) componentResourceManager.GetObject("checkmarkStatus.IndeterminateImage");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    this.scalingLabelConnect.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.scalingLabelConnect, 4);
    componentResourceManager.ApplyResources((object) this.scalingLabelConnect, "scalingLabelConnect");
    this.scalingLabelConnect.FontGroup = (string) null;
    this.scalingLabelConnect.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelConnect).Name = "scalingLabelConnect";
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.seekTimeListView, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "CTPUSBFlashing";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.checkmarkConnect, "checkmarkConnect");
    ((Control) this.checkmarkConnect).Name = "checkmarkConnect";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.progressBarMarquee, 5);
    componentResourceManager.ApplyResources((object) this.progressBarMarquee, "progressBarMarquee");
    this.progressBarMarquee.Maximum = 1000;
    this.progressBarMarquee.Name = "progressBarMarquee";
    this.progressBarMarquee.Step = 1;
    this.progressBarMarquee.Style = ProgressBarStyle.Marquee;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 0, 8);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 6605466001.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 6605466002.0, (ValueState) 0);
    this.digitalReadoutInstrument2.Gradient.Modify(3, 6610777001.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(4, 6610777002.0, (ValueState) 0);
    this.digitalReadoutInstrument2.Gradient.Modify(5, 6613928001.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(6, 6613928002.0, (ValueState) 0);
    this.digitalReadoutInstrument2.Gradient.Modify(7, 6613931001.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(8, 6613931002.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 8, "CTP01T", "CO_HardwarePartNumber");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 14485460001.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 14485460002.0, (ValueState) 0);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 14487260001.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(4, 14487260002.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 8, "CTP01T", "CO_SoftwarePartNumber");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutInstrument3, 3);
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 8, "CTP01T", "DT_STO_ID_FBS_Sw_Version_fbsVersion");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.labelWarning, 5);
    componentResourceManager.ApplyResources((object) this.labelWarning, "labelWarning");
    this.labelWarning.Name = "labelWarning";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_CTP_USB_Stick_Flash");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
