// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel.UserPanel
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
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string DynoModeTestStartServiceQualifier = "RT_OTF_DynoMode_Start";
  private const string DynoModeTestStopServiceQualifier = "RT_OTF_DynoMode_Stop";
  private const string TransEmotNumParameterQualifier = "ptconf_p_Trans_EmotNum_u8";
  private ParameterDataItem transEmotNumParameterDataItem = (ParameterDataItem) null;
  private Channel cpc = (Channel) null;
  private UserPanel.TestState testState;
  private BarInstrument barInstrumentTransOilTemp;
  private BarInstrument barInstrumentMotor1Speed;
  private BarInstrument barInstrument2;
  private BarInstrument barInstrument3;
  private TableLayoutPanel tableLayoutPanelLeft;
  private Button buttonStart;
  private Button buttonStop;
  private SeekTimeListView seekTimeListView1;
  private TableLayoutPanel tableLayoutPanelCheckmarkAndLabel;
  private Checkmark checkmarkStartTest;
  private TextBox textBoxStartTest;
  private ComboBox comboBoxAxleSelect;
  private TableLayoutPanel tableLayoutPanel3;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrumentgnitionStatus;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineTorque1;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineTorque2;
  private DigitalReadoutInstrument digitalReadoutInstrumentKickdown;
  private BarInstrument barInstrumentMotor3Speed;
  private System.Windows.Forms.Label labelAxelSelect;
  private TableLayoutPanel tableLayoutPanelAll;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.testState = UserPanel.TestState.NotRunning;
    this.UpdateUserInterface(false);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetCPC(this.GetChannel("ECPC01T", (CustomPanel.ChannelLookupOptions) 3));
  }

  private void SetCPC(Channel cpc)
  {
    if (this.cpc == cpc)
      return;
    if (this.cpc != null && this.transEmotNumParameterDataItem != null)
      ((DataItem) this.transEmotNumParameterDataItem).UpdateEvent -= new EventHandler<ResultEventArgs>(this.transEmotNumParameterDataItem_UpdateEvent);
    this.cpc = cpc;
    if (this.cpc != null)
    {
      this.transEmotNumParameterDataItem = new ParameterDataItem(this.cpc.Parameters["ptconf_p_Trans_EmotNum_u8"], new Qualifier("ECPC01T", "ptconf_p_Trans_EmotNum_u8"));
      if (this.transEmotNumParameterDataItem != null)
        ((DataItem) this.transEmotNumParameterDataItem).UpdateEvent += new EventHandler<ResultEventArgs>(this.transEmotNumParameterDataItem_UpdateEvent);
    }
  }

  private static object GetInstrumentCurrentValue(Instrument instrument)
  {
    object instrumentCurrentValue = (object) null;
    if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
      instrumentCurrentValue = instrument.InstrumentValues.Current.Value;
    return instrumentCurrentValue;
  }

  private bool Online => this.cpc != null && this.cpc.Online;

  private int TransEmotNumber
  {
    get
    {
      int result = (int) byte.MaxValue;
      return this.transEmotNumParameterDataItem == null || ((DataItem) this.transEmotNumParameterDataItem).Value == null || !int.TryParse(((DataItem) this.transEmotNumParameterDataItem).Value.ToString(), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (int) byte.MaxValue : result;
    }
  }

  private bool IgnitionIsOn => this.digitalReadoutInstrumentgnitionStatus.RepresentedState == 1;

  private void StartTest()
  {
    this.LogMessage(Resources.Message_TheStartButtonHasBeenPressed);
    Service service = this.GetService(this.cpc.Ecu.Name, "RT_OTF_DynoMode_Start");
    if (service != (Service) null && service.InputValues.Count == 5)
    {
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 0);
      service.InputValues[1].Value = (object) service.InputValues[1].Choices.GetItemFromRawValue((object) 3);
      service.InputValues[2].Value = (object) service.InputValues[2].Choices.GetItemFromRawValue((object) 3);
      service.InputValues[3].Value = (object) service.InputValues[3].Choices.GetItemFromRawValue((object) int.Parse(((KeyValuePair<string, string>) this.comboBoxAxleSelect.SelectedItem).Key));
      service.InputValues[4].Value = (object) service.InputValues[4].Choices.GetItemFromRawValue((object) 0);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StartDynoModeServiceCompleteEvent);
      service.Execute(false);
    }
    else
    {
      this.LogMessage($"{Resources.Message_EndingTheTestTheService}RT_OTF_DynoMode_Start{Resources.Message_IsNotAvailable}");
      this.StopTest();
    }
    this.UpdateUserInterface(false);
  }

  private void StopTest()
  {
    Service service = this.GetService(this.cpc.Ecu.Name, "RT_OTF_DynoMode_Stop");
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StopDynoModeServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.testState = UserPanel.TestState.NotRunning;
  }

  private void StartDynoModeServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StartDynoModeServiceCompleteEvent);
    this.comboBoxAxleSelect.Enabled = false;
    this.buttonStart.Enabled = false;
    this.buttonStop.Enabled = true;
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
    this.comboBoxAxleSelect.Enabled = true;
    this.comboBoxAxleSelect.SelectedIndex = 0;
    this.UpdateUserInterface(false);
  }

  private void comboBoxAxleSelect_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface(false);
  }

  private void digitalReadoutInstrumentgnitionStatus_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface(false);
  }

  private void transEmotNumParameterDataItem_UpdateEvent(object sender, ResultEventArgs e)
  {
    this.RemoveItemsFromDropDown();
    this.UpdateUserInterface(true);
  }

  private void buttonStart_Click(object sender, EventArgs e) => this.StartTest();

  private void buttonStop_Click(object sender, EventArgs e) => this.StopTest();

  private void RemoveItemsFromDropDown()
  {
    this.comboBoxAxleSelect.SelectedIndexChanged -= new EventHandler(this.comboBoxAxleSelect_SelectedIndexChanged);
    this.comboBoxAxleSelect.DataSource = (object) null;
    this.comboBoxAxleSelect.Items.Clear();
    this.comboBoxAxleSelect.SelectedIndexChanged += new EventHandler(this.comboBoxAxleSelect_SelectedIndexChanged);
  }

  private void PopulateDropDownList(bool emotNumChanged)
  {
    if (this.comboBoxAxleSelect.Items.Count != 0 && !emotNumChanged)
      return;
    this.comboBoxAxleSelect.SelectedIndexChanged -= new EventHandler(this.comboBoxAxleSelect_SelectedIndexChanged);
    Dictionary<string, string> dataSource = new Dictionary<string, string>();
    dataSource.Add("-1", Resources.Value_None);
    switch (this.TransEmotNumber)
    {
      case 1:
      case 2:
        dataSource.Add("1", Resources.Value_FirstRearDriveAxleActive);
        break;
      default:
        dataSource.Add("0", Resources.Value_TwoRearDriveAxles);
        dataSource.Add("1", Resources.Value_FirstRearDriveAxleActive);
        dataSource.Add("2", Resources.Value_SecondRearDriveAxleActive);
        break;
    }
    this.comboBoxAxleSelect.DataSource = (object) new BindingSource((object) dataSource, (string) null);
    this.comboBoxAxleSelect.DisplayMember = "Value";
    this.comboBoxAxleSelect.ValueMember = "Key";
    this.comboBoxAxleSelect.SelectedIndex = 0;
    this.comboBoxAxleSelect.SelectedIndexChanged += new EventHandler(this.comboBoxAxleSelect_SelectedIndexChanged);
  }

  private void UpdateUserInterface(bool emotNumChange)
  {
    this.PopulateDropDownList(emotNumChange);
    ((Control) this.digitalReadoutInstrumentEngineTorque2).Visible = this.TransEmotNumber == 3;
    ((Control) this.barInstrumentMotor3Speed).Visible = this.TransEmotNumber == 3;
    if (!this.Online)
    {
      this.buttonStart.Enabled = false;
      this.buttonStop.Enabled = false;
      this.comboBoxAxleSelect.Enabled = false;
      this.checkmarkStartTest.Checked = false;
      this.textBoxStartTest.Text = Resources.Message_TheCPCIsOffline;
    }
    else
    {
      this.comboBoxAxleSelect.Enabled = true;
      this.buttonStop.Enabled = this.testState == UserPanel.TestState.TestRunning;
      if (((KeyValuePair<string, string>) this.comboBoxAxleSelect.SelectedItem).Key == "-1")
        this.buttonStart.Enabled = false;
      else
        this.buttonStart.Enabled = this.comboBoxAxleSelect.Enabled = this.testState == UserPanel.TestState.NotRunning && this.IgnitionIsOn;
      switch (this.testState)
      {
        case UserPanel.TestState.NotRunning:
          if (this.IgnitionIsOn)
          {
            this.checkmarkStartTest.Checked = true;
            this.textBoxStartTest.Text = Resources.Message_ReadyToStartTheDynamometerTestMode;
            break;
          }
          this.checkmarkStartTest.Checked = false;
          this.textBoxStartTest.Text = Resources.Message_TheDynamometerTestModeCannotBeStartedUntilTheIgnitionIsOn;
          break;
        case UserPanel.TestState.TestRunning:
          if (this.IgnitionIsOn)
          {
            this.checkmarkStartTest.Checked = true;
            this.textBoxStartTest.Text = Resources.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit;
            break;
          }
          this.checkmarkStartTest.Checked = false;
          this.textBoxStartTest.Text = Resources.Message_TheIgnitionHasBeenTurnedOffWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode;
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
    this.barInstrumentMotor3Speed = new BarInstrument();
    this.barInstrumentMotor1Speed = new BarInstrument();
    this.barInstrument2 = new BarInstrument();
    this.tableLayoutPanelLeft = new TableLayoutPanel();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanelCheckmarkAndLabel = new TableLayoutPanel();
    this.checkmarkStartTest = new Checkmark();
    this.textBoxStartTest = new TextBox();
    this.comboBoxAxleSelect = new ComboBox();
    this.labelAxelSelect = new System.Windows.Forms.Label();
    this.barInstrument3 = new BarInstrument();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentgnitionStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineTorque2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineTorque1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentKickdown = new DigitalReadoutInstrument();
    this.barInstrumentTransOilTemp = new BarInstrument();
    ((Control) this.tableLayoutPanelAll).SuspendLayout();
    ((Control) this.tableLayoutPanelLeft).SuspendLayout();
    ((Control) this.tableLayoutPanelCheckmarkAndLabel).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAll, "tableLayoutPanelAll");
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrumentMotor3Speed, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrumentMotor1Speed, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrument2, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.tableLayoutPanelLeft, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrument3, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.tableLayoutPanel3, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAll).Controls.Add((Control) this.barInstrumentTransOilTemp, 4, 2);
    ((Control) this.tableLayoutPanelAll).Name = "tableLayoutPanelAll";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrumentMotor3Speed, 2);
    componentResourceManager.ApplyResources((object) this.barInstrumentMotor3Speed, "barInstrumentMotor3Speed");
    this.barInstrumentMotor3Speed.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrumentMotor3Speed).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentMotor3Speed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3");
    ((Control) this.barInstrumentMotor3Speed).Name = "barInstrumentMotor3Speed";
    ((AxisSingleInstrumentBase) this.barInstrumentMotor3Speed).PreferredAxisRange = new AxisRange(0.0, 10000.0, (string) null);
    ((SingleInstrumentBase) this.barInstrumentMotor3Speed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrumentMotor1Speed, 2);
    componentResourceManager.ApplyResources((object) this.barInstrumentMotor1Speed, "barInstrumentMotor1Speed");
    this.barInstrumentMotor1Speed.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrumentMotor1Speed).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentMotor1Speed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1");
    ((Control) this.barInstrumentMotor1Speed).Name = "barInstrumentMotor1Speed";
    ((AxisSingleInstrumentBase) this.barInstrumentMotor1Speed).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
    ((SingleInstrumentBase) this.barInstrumentMotor1Speed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLeft, "tableLayoutPanelLeft");
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.tableLayoutPanelLeft, 2);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.buttonStart, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.buttonStop, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.seekTimeListView1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.tableLayoutPanelCheckmarkAndLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.comboBoxAxleSelect, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).Controls.Add((Control) this.labelAxelSelect, 0, 2);
    ((Control) this.tableLayoutPanelLeft).Name = "tableLayoutPanelLeft";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetRowSpan((Control) this.tableLayoutPanelLeft, 6);
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
    componentResourceManager.ApplyResources((object) this.comboBoxAxleSelect, "comboBoxAxleSelect");
    this.comboBoxAxleSelect.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxAxleSelect.FormattingEnabled = true;
    this.comboBoxAxleSelect.Name = "comboBoxAxleSelect";
    this.comboBoxAxleSelect.SelectedIndexChanged += new EventHandler(this.comboBoxAxleSelect_SelectedIndexChanged);
    ((TableLayoutPanel) this.tableLayoutPanelLeft).SetColumnSpan((Control) this.labelAxelSelect, 3);
    componentResourceManager.ApplyResources((object) this.labelAxelSelect, "labelAxelSelect");
    this.labelAxelSelect.Name = "labelAxelSelect";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.barInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument3, "barInstrument3");
    this.barInstrument3.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.barInstrument3).Name = "barInstrument3";
    ((AxisSingleInstrumentBase) this.barInstrument3).PreferredAxisRange = new AxisRange(0.0, 100.0, (string) null);
    ((SingleInstrumentBase) this.barInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetColumnSpan((Control) this.tableLayoutPanel3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentgnitionStatus, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentEngineTorque2, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentEngineTorque1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentKickdown, 0, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanelAll).SetRowSpan((Control) this.tableLayoutPanel3, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS052_CalculatedGear_CalculatedGear");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Desired_Gear_current_value");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentgnitionStatus, "digitalReadoutInstrumentgnitionStatus");
    this.digitalReadoutInstrumentgnitionStatus.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentgnitionStatus).FreezeValue = false;
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Initialize((ValueState) 0, 8);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(5, 10.0, (ValueState) 0);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(6, 14.0, (ValueState) 0);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(7, 15.0, (ValueState) 0);
    this.digitalReadoutInstrumentgnitionStatus.Gradient.Modify(8, (double) int.MaxValue, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentgnitionStatus).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS063_IgnitionSwitchStatus_IgnitionSwitchStatus");
    ((Control) this.digitalReadoutInstrumentgnitionStatus).Name = "digitalReadoutInstrumentgnitionStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentgnitionStatus).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentgnitionStatus.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentgnitionStatus_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineTorque2, "digitalReadoutInstrumentEngineTorque2");
    this.digitalReadoutInstrumentEngineTorque2.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineTorque2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineTorque2).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS134_Current_Torque_Axle_2_Current_Torque_Axle_2");
    ((Control) this.digitalReadoutInstrumentEngineTorque2).Name = "digitalReadoutInstrumentEngineTorque2";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineTorque2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineTorque1, "digitalReadoutInstrumentEngineTorque1");
    this.digitalReadoutInstrumentEngineTorque1.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineTorque1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineTorque1).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS133_Current_Torque_Axle_1_Current_Torque_Axle_1");
    ((Control) this.digitalReadoutInstrumentEngineTorque1).Name = "digitalReadoutInstrumentEngineTorque1";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineTorque1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentKickdown, "digitalReadoutInstrumentKickdown");
    this.digitalReadoutInstrumentKickdown.FontGroup = "TDMReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentKickdown).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentKickdown).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS004_Kickdown");
    ((Control) this.digitalReadoutInstrumentKickdown).Name = "digitalReadoutInstrumentKickdown";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentKickdown).UnitAlignment = StringAlignment.Near;
    this.barInstrumentTransOilTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentTransOilTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentTransOilTemp, "barInstrumentTransOilTemp");
    this.barInstrumentTransOilTemp.FontGroup = "TDMBar";
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentTransOilTemp).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Transmission_Oil_Temperature_current_value");
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
    TestRunning,
  }
}
