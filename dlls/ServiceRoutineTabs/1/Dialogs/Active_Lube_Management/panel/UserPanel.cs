// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel;

public class UserPanel : CustomPanel
{
  private const string ALMGroup = "VCD_ACS_Active_Lubrication_Management";
  private const string ALMActivationTemp = "PACS_ALM_activation_temp";
  private const string ALMLookupV00M4 = "PACS_ALM_Lookup_v00_M_4";
  private const string ALMLookupV00M5 = "PACS_ALM_Lookup_v00_M_5";
  private const string ALMActivationTempShortName = "Activation Temp";
  private const string ALMLookupV00M4ShortName = "Lookup M4";
  private const string ALMLookupV00M5ShortName = "Lookup M5";
  private const string ALMAvl = "PACS_ALM_avl";
  private const string ALMPresent = "PACS_ALM_present";
  private const int TestModeAlmActivation = 5;
  private const int TestModeAlmLookup4 = 20;
  private const int TestModeAlmLookup5 = 20;
  private const int DefaultAlmActivation = 40;
  private const int DefaultAlmLookup4 = 255 /*0xFF*/;
  private const int DefaultAlmLookup5 = 255 /*0xFF*/;
  private Channel sSam = (Channel) null;
  private Parameter almActivationParameter = (Parameter) null;
  private Parameter almLookup4Parameter = (Parameter) null;
  private Parameter almLookup5Parameter = (Parameter) null;
  private Parameter almAvlParameter = (Parameter) null;
  private int almActivationPreTestValue = 0;
  private int almLookup4PreTestValue = 0;
  private int almLookup5PreTestValue = 0;
  private Button buttonBegin;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelSSAMStatus;
  private TableLayoutPanel tableLayoutPanel4;
  private Button buttonClose;
  private Checkmark SSAMCheck;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutInstrumentALMFunctionStat;
  private DigitalReadoutInstrument digitalReadoutInstrumentALMValve;
  private TableLayoutPanel tableLayoutPanelBottom;
  private TimerControl timerControl;

  public UserPanel()
  {
    this.InitializeComponent();
    this.InProgress = false;
    this.EnablePanel = true;
    this.buttonBegin.Click += new EventHandler(this.OnClickBegin);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetSSAM((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetSSAM(this.GetChannel("SSAM02T"));
    this.UpdateUserInterface();
  }

  private void SetSSAM(Channel sSam)
  {
    if (this.sSam == sSam)
      return;
    if (this.sSam != null)
    {
      this.sSam.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      if (this.timerControl.IsTimerRunning && sSam == null)
        this.StopTimer();
    }
    this.sSam = sSam;
    if (this.sSam != null)
    {
      this.sSam.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.almActivationParameter = this.sSam.Parameters["PACS_ALM_activation_temp"];
      this.almLookup4Parameter = this.sSam.Parameters["PACS_ALM_Lookup_v00_M_4"];
      this.almLookup5Parameter = this.sSam.Parameters["PACS_ALM_Lookup_v00_M_5"];
      this.almAvlParameter = this.sSam.Parameters["PACS_ALM_avl"] ?? this.sSam.Parameters["PACS_ALM_present"];
      if (this.HasNeededParameters && this.sSam.CommunicationsState == CommunicationsState.Online)
        this.ReadInitialParameters();
    }
  }

  private bool Online
  {
    get => this.sSam != null && this.sSam.CommunicationsState == CommunicationsState.Online;
  }

  private bool HasNeededParameters
  {
    get
    {
      return this.almActivationParameter != null && this.almLookup4Parameter != null && this.almLookup5Parameter != null && this.almAvlParameter != null;
    }
  }

  private bool CanBegin
  {
    get
    {
      return this.EnablePanel && this.Online && this.HasNeededParameters && this.SSAMCheck.Checked && !this.InProgress;
    }
  }

  private bool CanClose => !this.InProgress || this.sSam == null;

  private bool InProgress { get; set; }

  private bool EnablePanel { get; set; }

  private void UpdateSSAMStatus()
  {
    bool flag = false;
    string str = Resources.Message_SSAM02TIsNotConnected;
    if (this.sSam != null)
    {
      if (this.HasNeededParameters)
      {
        if (this.sSam.CommunicationsState == CommunicationsState.Online)
        {
          str = Resources.Message_SSAM02TIsConnected;
          flag = true;
        }
        else
          str = Resources.Message_SSAM02TIsBusy;
      }
      else
        str = Resources.Message_ThisSSAMDoesNotHaveTheNeededParameters;
    }
    ((Control) this.labelSSAMStatus).Text = str;
    this.SSAMCheck.Checked = flag;
  }

  private void UpdateButtonStatus()
  {
    this.buttonBegin.Enabled = this.CanBegin;
    this.buttonClose.Enabled = this.CanClose;
  }

  private void UpdateUserInterface()
  {
    this.UpdateSSAMStatus();
    this.UpdateButtonStatus();
    ((Control) this.digitalReadoutInstrumentALMValve).Enabled = this.EnablePanel;
    ((Control) this.digitalReadoutInstrumentALMFunctionStat).Enabled = this.EnablePanel;
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (this.sSam.CommunicationsState == CommunicationsState.Online && !this.almAvlParameter.HasBeenReadFromEcu)
      this.ReadInitialParameters();
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void OnClickBegin(object sender, EventArgs e)
  {
    if (!this.CanBegin)
      return;
    this.ReadALMParameters();
  }

  private void ReadInitialParameters()
  {
    if (this.sSam == null || this.sSam.Parameters == null)
      return;
    this.sSam.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    this.InProgress = true;
    this.UpdateUserInterface();
    this.sSam.Parameters.ReadGroup("VCD_ACS_Active_Lubrication_Management", false, false);
  }

  private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.sSam.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    if (!e.Succeeded)
      return;
    if ((!(this.almAvlParameter.Type == typeof (Choice)) ? Convert.ToInt32(this.almAvlParameter.Value) : Convert.ToInt32((this.almAvlParameter.Value as Choice).RawValue)) == 1)
    {
      this.EnablePanel = true;
      this.AddParametersToLog(Resources.Message_CurrentAlmValues);
      int int32_1 = Convert.ToInt32(this.almActivationParameter.Value);
      int int32_2 = Convert.ToInt32(this.almLookup4Parameter.Value);
      int int32_3 = Convert.ToInt32(this.almLookup5Parameter.Value);
      if (int32_1 == 5 && int32_2 == 20 && int32_3 == 20)
      {
        this.AddLogLabel(Resources.Message_SettingDefaultValues);
        this.almActivationParameter.Value = (object) 40;
        this.almLookup4Parameter.Value = (object) (int) byte.MaxValue;
        this.almLookup5Parameter.Value = (object) (int) byte.MaxValue;
        this.sSam.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersDefaultBeginWriteCompleteEvent);
        this.sSam.Parameters.Write(false);
      }
      else
        this.InProgress = false;
    }
    else
    {
      this.EnablePanel = false;
      this.InProgress = false;
      this.AddLogLabel(Resources.Message_ActiveLubeManagementNotEnabled);
    }
    this.UpdateUserInterface();
  }

  private void Parameters_ParametersDefaultBeginWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.sSam.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersDefaultBeginWriteCompleteEvent);
    this.AddParametersToLog(Resources.Message_ParametersSetToDefaultValues);
    this.InProgress = false;
    this.UpdateUserInterface();
  }

  private void ReadALMParameters()
  {
    if (this.sSam == null || this.sSam.Parameters == null)
      return;
    this.sSam.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
    this.InProgress = true;
    this.UpdateUserInterface();
    this.sSam.Parameters.ReadGroup("VCD_ACS_Active_Lubrication_Management", false, false);
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.sSam.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
    if (!e.Succeeded)
      return;
    this.almActivationPreTestValue = Convert.ToInt32(this.almActivationParameter.Value);
    this.almLookup4PreTestValue = Convert.ToInt32(this.almLookup4Parameter.Value);
    this.almLookup5PreTestValue = Convert.ToInt32(this.almLookup5Parameter.Value);
    this.AddLogLabel(Resources.Message_EnteringTestMode);
    this.almActivationParameter.Value = (object) 5;
    this.almLookup4Parameter.Value = (object) 20;
    this.almLookup5Parameter.Value = (object) 20;
    this.sSam.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersBeginTestWriteCompleteEvent);
    this.sSam.Parameters.Write(false);
  }

  private void Parameters_ParametersBeginTestWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.sSam.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersBeginTestWriteCompleteEvent);
    this.AddParametersToLog(Resources.Message_RemainInTestMode);
    this.timerControl.Start();
  }

  private void timerControl_TimerCountdownCompleted(object sender, EventArgs e)
  {
    if (!this.Online)
      return;
    this.StopTimer();
    this.AddLogLabel(Resources.Message_ResettingParameters);
    this.almActivationParameter.Value = (object) this.almActivationPreTestValue;
    this.almLookup4Parameter.Value = (object) this.almLookup4PreTestValue;
    this.almLookup5Parameter.Value = (object) this.almLookup5PreTestValue;
    this.sSam.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersEndTestWriteCompleteEvent);
    this.sSam.Parameters.Write(false);
  }

  private void StopTimer() => this.timerControl.Stop();

  private void Parameters_ParametersEndTestWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.sSam.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersEndTestWriteCompleteEvent);
    this.AddParametersToLog(Resources.Message_ParametersReset);
    this.AddLogLabel(Resources.Message_Finished);
    this.InProgress = false;
    this.UpdateUserInterface();
  }

  private void AddParametersToLog(string text)
  {
    this.AddLogLabel($"{text} {"Activation Temp"} {this.almActivationParameter.Value}, {"Lookup M4"} {this.almLookup4Parameter.Value}, {"Lookup M5"} {this.almLookup5Parameter.Value}");
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentALMFunctionStat = new DigitalReadoutInstrument();
    this.timerControl = new TimerControl();
    this.seekTimeListView = new SeekTimeListView();
    this.digitalReadoutInstrumentALMValve = new DigitalReadoutInstrument();
    this.tableLayoutPanelBottom = new TableLayoutPanel();
    this.SSAMCheck = new Checkmark();
    this.buttonClose = new Button();
    this.buttonBegin = new Button();
    this.labelSSAMStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this.tableLayoutPanelBottom).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentALMFunctionStat, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.timerControl, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.seekTimeListView, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentALMValve, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.tableLayoutPanelBottom, 0, 3);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentALMFunctionStat, "digitalReadoutInstrumentALMFunctionStat");
    this.digitalReadoutInstrumentALMFunctionStat.FontGroup = "DigitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentALMFunctionStat).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentALMFunctionStat).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ALMFunction_Stat");
    ((Control) this.digitalReadoutInstrumentALMFunctionStat).Name = "digitalReadoutInstrumentALMFunctionStat";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentALMFunctionStat).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.timerControl, "timerControl");
    this.timerControl.Duration = TimeSpan.Parse("00:00:30");
    this.timerControl.FontGroup = (string) null;
    ((Control) this.timerControl).Name = "timerControl";
    ((TableLayoutPanel) this.tableLayoutPanel4).SetRowSpan((Control) this.timerControl, 2);
    this.timerControl.TimerCountdownCompletedDisplayMessage = " ";
    this.timerControl.TimerCountdownCompleted += new EventHandler(this.timerControl_TimerCountdownCompleted);
    ((TableLayoutPanel) this.tableLayoutPanel4).SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Active Lube Management";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentALMValve, "digitalReadoutInstrumentALMValve");
    this.digitalReadoutInstrumentALMValve.FontGroup = "DigitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentALMValve).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentALMValve).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ActvLubMgntValve");
    ((Control) this.digitalReadoutInstrumentALMValve).Name = "digitalReadoutInstrumentALMValve";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentALMValve).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBottom, "tableLayoutPanelBottom");
    ((TableLayoutPanel) this.tableLayoutPanel4).SetColumnSpan((Control) this.tableLayoutPanelBottom, 2);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.SSAMCheck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.buttonClose, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.buttonBegin, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.labelSSAMStatus, 1, 0);
    ((Control) this.tableLayoutPanelBottom).Name = "tableLayoutPanelBottom";
    componentResourceManager.ApplyResources((object) this.SSAMCheck, "SSAMCheck");
    ((Control) this.SSAMCheck).Name = "SSAMCheck";
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonBegin, "buttonBegin");
    this.buttonBegin.Name = "buttonBegin";
    this.buttonBegin.UseCompatibleTextRendering = true;
    this.buttonBegin.UseVisualStyleBackColor = true;
    this.labelSSAMStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelSSAMStatus, "labelSSAMStatus");
    ((Control) this.labelSSAMStatus).Name = "labelSSAMStatus";
    this.labelSSAMStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelSSAMStatus.ShowBorder = false;
    this.labelSSAMStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel4);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this.tableLayoutPanelBottom).ResumeLayout(false);
    ((Control) this.tableLayoutPanelBottom).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
