// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
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
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__MY13_.panel;

public class UserPanel : CustomPanel
{
  private readonly string CpcName = "CPC04T";
  private readonly string EngineSpeedInstrumentQualifier = "DT_ASL_Actual_Engine_Speed";
  private readonly string DynoModeTestStartServiceQualifier = "RT_Dynamometer_test_mode_status_Start";
  private readonly string DynoModeTestStopServiceQualifier = "RT_Dynamometer_test_mode_status_Stop";
  private readonly Dictionary<string, int> valueMap = new Dictionary<string, int>()
  {
    {
      Resources.Message_AutomaticShifting,
      0
    },
    {
      Resources.Message_ManualShifting,
      1
    }
  };
  private Channel cpc = (Channel) null;
  private Channel tcm = (Channel) null;
  private UserPanel.TestState testState;
  private Instrument engineSpeedInstrument;
  private SeekTimeListView seekTimeListView1;
  private Button buttonStop;
  private BarInstrument barInstrumentTransOilTemp;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private BarInstrument barInstrument1;
  private BarInstrument barInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private TableLayoutPanel tableLayoutPanelLeft;
  private ComboBox comboBoxShiftType;
  private Button buttonStart;
  private BarInstrument barInstrument3;
  private TableLayoutPanel tableLayoutPanel3;
  private TableLayoutPanel tableLayoutPanelCheckmarkAndLabel;
  private Checkmark checkmarkStartTest;
  private TextBox textBoxStartTest;
  private TableLayoutPanel tableLayoutPanelAll;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.testState = UserPanel.TestState.NotRunning;
    this.comboBoxShiftType.DataSource = (object) this.valueMap.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (x => x.Key)).ToList<string>();
    this.comboBoxShiftType.SelectedIndex = 0;
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetCPC(this.GetChannel(this.CpcName));
    this.SetTcm(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7));
    this.UpdateUserInterface();
  }

  private void SetCPC(Channel cpc)
  {
    if (this.cpc == cpc)
      return;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      if (this.engineSpeedInstrument != (Instrument) null)
        this.engineSpeedInstrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.EngineSpeedUpdateEvent);
    }
    this.cpc = cpc;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.engineSpeedInstrument = cpc.Instruments[this.EngineSpeedInstrumentQualifier];
      if (this.engineSpeedInstrument != (Instrument) null)
        this.engineSpeedInstrument.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.EngineSpeedUpdateEvent);
    }
  }

  private void SetTcm(Channel tcm)
  {
    if (this.tcm == tcm)
      return;
    this.tcm = tcm;
    if (this.tcm != null)
    {
      foreach (SingleInstrumentBase singleInstrumentBase1 in CustomPanel.GetControlsOfType(((Control) this).Controls, typeof (SingleInstrumentBase)))
      {
        Qualifier instrument = singleInstrumentBase1.Instrument;
        Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier) ref instrument).Ecu);
        if (ecuByName != null && ecuByName.Identifier == tcm.Ecu.Identifier && ecuByName.Name != tcm.Ecu.Name)
        {
          SingleInstrumentBase singleInstrumentBase2 = singleInstrumentBase1;
          instrument = singleInstrumentBase1.Instrument;
          QualifierTypes type = ((Qualifier) ref instrument).Type;
          string name1 = tcm.Ecu.Name;
          instrument = singleInstrumentBase1.Instrument;
          string name2 = ((Qualifier) ref instrument).Name;
          Qualifier qualifier1 = new Qualifier(type, name1, name2);
          singleInstrumentBase2.Instrument = qualifier1;
          if (singleInstrumentBase1.DataItem == null)
          {
            QualifierTypes qualifierTypes1 = (QualifierTypes) 0;
            instrument = singleInstrumentBase1.Instrument;
            if (((Qualifier) ref instrument).Type == 1)
            {
              qualifierTypes1 = (QualifierTypes) 8;
            }
            else
            {
              instrument = singleInstrumentBase1.Instrument;
              if (((Qualifier) ref instrument).Type == 8)
                qualifierTypes1 = (QualifierTypes) 1;
            }
            SingleInstrumentBase singleInstrumentBase3 = singleInstrumentBase1;
            QualifierTypes qualifierTypes2 = qualifierTypes1;
            string name3 = tcm.Ecu.Name;
            instrument = singleInstrumentBase1.Instrument;
            string name4 = ((Qualifier) ref instrument).Name;
            Qualifier qualifier2 = new Qualifier(qualifierTypes2, name3, name4);
            singleInstrumentBase3.Instrument = qualifier2;
          }
        }
      }
    }
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private static object GetInstrumentCurrentValue(Instrument instrument)
  {
    object instrumentCurrentValue = (object) null;
    if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
      instrumentCurrentValue = instrument.InstrumentValues.Current.Value;
    return instrumentCurrentValue;
  }

  private bool Online
  {
    get => this.cpc != null && this.cpc.CommunicationsState == CommunicationsState.Online;
  }

  private bool EngineIsRunning
  {
    get
    {
      object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.engineSpeedInstrument);
      int result;
      return instrumentCurrentValue != null && int.TryParse(instrumentCurrentValue.ToString(), out result) && result > 0;
    }
  }

  private void EngineSpeedUpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.testState == UserPanel.TestState.WaitingForEngineToStart && this.EngineIsRunning)
      this.ContinueTest();
    else if (this.testState == UserPanel.TestState.TestRunning && !this.EngineIsRunning)
    {
      this.LogMessage(Resources.Message_StoppingTheDynoTestModeBecauseTheEngineStopped);
      this.StopTest();
    }
    this.UpdateUserInterface();
  }

  private void StartTest()
  {
    this.testState = UserPanel.TestState.WaitingForEngineToStart;
    this.LogMessage(Resources.Message_TheStartButtonHasBeenPressed);
    this.LogMessage(Resources.Message_WaitingForTheEngineToBeStarted);
    this.UpdateUserInterface();
  }

  private void StopTest()
  {
    Service service = this.GetService(this.CpcName, this.DynoModeTestStopServiceQualifier);
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StopDynoModeServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.testState = UserPanel.TestState.NotRunning;
  }

  private void ContinueTest()
  {
    int rawValue = this.valueMap[this.comboBoxShiftType.SelectedValue as string];
    Service service = this.GetService(this.CpcName, this.DynoModeTestStartServiceQualifier);
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 1);
      service.InputValues[1].Value = (object) service.InputValues[1].Choices.GetItemFromRawValue((object) rawValue);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StartDynoModeServiceCompleteEvent);
      service.Execute(false);
    }
    else
    {
      this.LogMessage(Resources.Message_EndingTheTestTheService + this.DynoModeTestStartServiceQualifier + Resources.Message_IsNotAvailable);
      this.StopTest();
    }
  }

  private void StartDynoModeServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StartDynoModeServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.testState = UserPanel.TestState.TestRunning;
      this.LogMessage(Resources.Message_DynamometerTestModeIsRunning);
    }
    else
    {
      this.LogMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_ErrorStartingDynamometerTestMode0, (object) e.Exception.Message));
      this.StopTest();
    }
  }

  private void StopDynoModeServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StopDynoModeServiceCompleteEvent);
    this.LogMessage(Resources.Message_DynamometerTestModeHasStopped);
    this.testState = UserPanel.TestState.NotRunning;
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    this.textBoxStartTest.Text = Resources.Message_PleaseStartTheEngine;
    this.StartTest();
  }

  private void buttonStop_Click(object sender, EventArgs e) => this.StopTest();

  private void UpdateUserInterface()
  {
    if (!this.Online)
    {
      this.buttonStart.Enabled = false;
      this.buttonStop.Enabled = false;
      this.comboBoxShiftType.Enabled = false;
      this.checkmarkStartTest.Checked = false;
      this.textBoxStartTest.Text = Resources.Message_TheCPCIsOffline;
    }
    else
    {
      this.buttonStart.Enabled = !this.EngineIsRunning;
      this.buttonStop.Enabled = true;
      this.comboBoxShiftType.Enabled = this.testState == UserPanel.TestState.NotRunning;
      switch (this.testState)
      {
        case UserPanel.TestState.NotRunning:
          if (!this.EngineIsRunning)
          {
            this.checkmarkStartTest.Checked = true;
            this.textBoxStartTest.Text = Resources.Message_ReadyToStartTheDynamometerTestMode;
            break;
          }
          this.checkmarkStartTest.Checked = false;
          this.textBoxStartTest.Text = Resources.Message_TheDynamometerTestModeCannotBeStartedUntilTheEngineIsOffAndTheIgnitionIsOn;
          break;
        case UserPanel.TestState.WaitingForEngineToStart:
          if (this.EngineIsRunning)
          {
            this.checkmarkStartTest.Checked = true;
            this.textBoxStartTest.Text = Resources.Message_TheEngineHasBeenStartedAndTheDynamometerTestModeHasStarted;
            break;
          }
          this.buttonStart.Enabled = false;
          this.checkmarkStartTest.Checked = false;
          this.textBoxStartTest.Text = Resources.Message_WaitingForYouToStartTheEngine;
          break;
        case UserPanel.TestState.TestRunning:
          if (this.EngineIsRunning)
          {
            this.checkmarkStartTest.Checked = true;
            this.textBoxStartTest.Text = Resources.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit;
            break;
          }
          this.checkmarkStartTest.Checked = false;
          this.textBoxStartTest.Text = Resources.Message_TheEngineHasBeenStoppedWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode;
          break;
      }
    }
  }

  private void LogMessage(string text)
  {
    this.AddStatusMessage(text);
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelAll = new TableLayoutPanel();
    this.barInstrument1 = new BarInstrument();
    this.barInstrument2 = new BarInstrument();
    this.tableLayoutPanelLeft = new TableLayoutPanel();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    this.comboBoxShiftType = new ComboBox();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanelCheckmarkAndLabel = new TableLayoutPanel();
    this.checkmarkStartTest = new Checkmark();
    this.textBoxStartTest = new TextBox();
    this.barInstrument3 = new BarInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.barInstrumentTransOilTemp = new BarInstrument();
    ((Control) this.tableLayoutPanelAll).SuspendLayout();
    ((Control) this.tableLayoutPanelLeft).SuspendLayout();
    ((Control) this.tableLayoutPanelCheckmarkAndLabel).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAll, "tableLayoutPanelAll");
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrument1, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrument2, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.tableLayoutPanelLeft, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrument3, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.digitalReadoutInstrument2, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.digitalReadoutInstrument3, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.tableLayoutPanel3, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrumentTransOilTemp, 4, 1);
    ((Control) this.tableLayoutPanelAll).Name = "tableLayoutPanelAll";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument1, "barInstrument1");
    this.barInstrument1.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.barInstrument1).Name = "barInstrument1";
    ((SingleInstrumentBase) this.barInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLeft, "tableLayoutPanelLeft");
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.tableLayoutPanelLeft, 2);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.buttonStart, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.buttonStop, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.comboBoxShiftType, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.seekTimeListView1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.tableLayoutPanelCheckmarkAndLabel, 0, 0);
    ((Control) this.tableLayoutPanelLeft).Name = "tableLayoutPanelLeft";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetRowSpan((Control) this.tableLayoutPanelLeft, 5);
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    this.buttonStop.Click += new EventHandler(this.buttonStop_Click);
    this.comboBoxShiftType.DisplayMember = "transmissionType";
    componentResourceManager.ApplyResources((object) this.comboBoxShiftType, "comboBoxShiftType");
    this.comboBoxShiftType.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxShiftType.FormattingEnabled = true;
    this.comboBoxShiftType.Items.AddRange(new object[2]
    {
      (object) componentResourceManager.GetString("comboBoxShiftType.Items"),
      (object) componentResourceManager.GetString("comboBoxShiftType.Items1")
    });
    this.comboBoxShiftType.Name = "comboBoxShiftType";
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((TableLayoutPanel) this.tableLayoutPanelLeft).SetColumnSpan((Control) this.seekTimeListView1, 3);
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "DetTransDynoMode";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelCheckmarkAndLabel, "tableLayoutPanelCheckmarkAndLabel");
    ((TableLayoutPanel) this.tableLayoutPanelLeft).SetColumnSpan((Control) this.tableLayoutPanelCheckmarkAndLabel, 3);
    ((TableLayoutPanel) this.tableLayoutPanelCheckmarkAndLabel).Controls.Add((Control) this.checkmarkStartTest, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelCheckmarkAndLabel).Controls.Add((Control) this.textBoxStartTest, 1, 0);
    ((Control) this.tableLayoutPanelCheckmarkAndLabel).Name = "tableLayoutPanelCheckmarkAndLabel";
    ((TableLayoutPanel) this.tableLayoutPanelLeft).SetRowSpan((Control) this.tableLayoutPanelCheckmarkAndLabel, 2);
    componentResourceManager.ApplyResources((object) this.checkmarkStartTest, "checkmarkStartTest");
    ((Control) this.checkmarkStartTest).Name = "checkmarkStartTest";
    this.textBoxStartTest.Cursor = Cursors.Arrow;
    componentResourceManager.ApplyResources((object) this.textBoxStartTest, "textBoxStartTest");
    this.textBoxStartTest.Name = "textBoxStartTest";
    this.textBoxStartTest.ReadOnly = true;
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument3, "barInstrument3");
    this.barInstrument3.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.barInstrument3).Name = "barInstrument3";
    ((SingleInstrumentBase) this.barInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd07_Sollgang_Sollgang");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd08_Istgang_Istgang");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.tableLayoutPanel3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument5, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument6, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument4, 2, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS001_Requested_Torque");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineTorque");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_Accelerator_Pedal_Kick_Down_Status");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    this.barInstrumentTransOilTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentTransOilTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentTransOilTemp, "barInstrumentTransOilTemp");
    this.barInstrumentTransOilTemp.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd16_Getriebe_Oelltemperatur_Getriebe_Oelltemperatur");
    ((Control) this.barInstrumentTransOilTemp).Name = "barInstrumentTransOilTemp";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetRowSpan((Control) this.barInstrumentTransOilTemp, 4);
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_Transmission_Dyno_Mode");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelAll);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelAll).ResumeLayout(false);
    ((Control) this.tableLayoutPanelLeft).ResumeLayout(false);
    ((Control) this.tableLayoutPanelCheckmarkAndLabel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelCheckmarkAndLabel).PerformLayout();
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum TestState
  {
    NotRunning,
    WaitingForEngineToStart,
    TestRunning,
  }
}
