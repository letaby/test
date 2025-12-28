using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel;

public class UserPanel : CustomPanel
{
	private enum TestState
	{
		NotRunning,
		TestRunning
	}

	private const string DynoModeTestStartServiceQualifier = "RT_OTF_DynoMode_Start";

	private const string DynoModeTestStopServiceQualifier = "RT_OTF_DynoMode_Stop";

	private const string TransEmotNumParameterQualifier = "ptconf_p_Trans_EmotNum_u8";

	private ParameterDataItem transEmotNumParameterDataItem = null;

	private Channel cpc = null;

	private TestState testState;

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

	private bool Online => cpc != null && cpc.Online;

	private int TransEmotNumber
	{
		get
		{
			int result = 255;
			return (transEmotNumParameterDataItem != null && ((DataItem)transEmotNumParameterDataItem).Value != null && int.TryParse(((DataItem)transEmotNumParameterDataItem).Value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result)) ? result : 255;
		}
	}

	private bool IgnitionIsOn => (int)digitalReadoutInstrumentgnitionStatus.RepresentedState == 1;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		testState = TestState.NotRunning;
		UpdateUserInterface(emotNumChange: false);
	}

	public override void OnChannelsChanged()
	{
		SetCPC(((CustomPanel)this).GetChannel("ECPC01T", (ChannelLookupOptions)3));
	}

	private void SetCPC(Channel cpc)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		if (this.cpc == cpc)
		{
			return;
		}
		if (this.cpc != null && transEmotNumParameterDataItem != null)
		{
			((DataItem)transEmotNumParameterDataItem).UpdateEvent -= transEmotNumParameterDataItem_UpdateEvent;
		}
		this.cpc = cpc;
		if (this.cpc != null)
		{
			Parameter parameter = this.cpc.Parameters["ptconf_p_Trans_EmotNum_u8"];
			transEmotNumParameterDataItem = new ParameterDataItem(parameter, new Qualifier("ECPC01T", "ptconf_p_Trans_EmotNum_u8"));
			if (transEmotNumParameterDataItem != null)
			{
				((DataItem)transEmotNumParameterDataItem).UpdateEvent += transEmotNumParameterDataItem_UpdateEvent;
			}
		}
	}

	private static object GetInstrumentCurrentValue(Instrument instrument)
	{
		object result = null;
		if (instrument != null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
		{
			result = instrument.InstrumentValues.Current.Value;
		}
		return result;
	}

	private void StartTest()
	{
		LogMessage(Resources.Message_TheStartButtonHasBeenPressed);
		Service service = ((CustomPanel)this).GetService(cpc.Ecu.Name, "RT_OTF_DynoMode_Start");
		if (service != null && service.InputValues.Count == 5)
		{
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(0);
			service.InputValues[1].Value = service.InputValues[1].Choices.GetItemFromRawValue(3);
			service.InputValues[2].Value = service.InputValues[2].Choices.GetItemFromRawValue(3);
			service.InputValues[3].Value = service.InputValues[3].Choices.GetItemFromRawValue(int.Parse(((KeyValuePair<string, string>)comboBoxAxleSelect.SelectedItem).Key));
			service.InputValues[4].Value = service.InputValues[4].Choices.GetItemFromRawValue(0);
			service.ServiceCompleteEvent += StartDynoModeServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
		else
		{
			LogMessage(Resources.Message_EndingTheTestTheService + "RT_OTF_DynoMode_Start" + Resources.Message_IsNotAvailable);
			StopTest();
		}
		UpdateUserInterface(emotNumChange: false);
	}

	private void StopTest()
	{
		Service service = ((CustomPanel)this).GetService(cpc.Ecu.Name, "RT_OTF_DynoMode_Stop");
		if (service != null)
		{
			service.ServiceCompleteEvent += StopDynoModeServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
		else
		{
			testState = TestState.NotRunning;
		}
	}

	private void StartDynoModeServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= StartDynoModeServiceCompleteEvent;
		comboBoxAxleSelect.Enabled = false;
		buttonStart.Enabled = false;
		buttonStop.Enabled = true;
		if (e.Succeeded)
		{
			testState = TestState.TestRunning;
			LogMessage(Resources.Message_DynamometerTestModeIsRunning);
		}
		else
		{
			LogMessage(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_ErrorStartingDynamometerTestMode0, e.Exception.Message));
			StopTest();
		}
	}

	private void StopDynoModeServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= StopDynoModeServiceCompleteEvent;
		LogMessage(Resources.Message_DynamometerTestModeHasStopped);
		testState = TestState.NotRunning;
		comboBoxAxleSelect.Enabled = true;
		comboBoxAxleSelect.SelectedIndex = 0;
		UpdateUserInterface(emotNumChange: false);
	}

	private void comboBoxAxleSelect_SelectedIndexChanged(object sender, EventArgs e)
	{
		UpdateUserInterface(emotNumChange: false);
	}

	private void digitalReadoutInstrumentgnitionStatus_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface(emotNumChange: false);
	}

	private void transEmotNumParameterDataItem_UpdateEvent(object sender, ResultEventArgs e)
	{
		RemoveItemsFromDropDown();
		UpdateUserInterface(emotNumChange: true);
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		StartTest();
	}

	private void buttonStop_Click(object sender, EventArgs e)
	{
		StopTest();
	}

	private void RemoveItemsFromDropDown()
	{
		comboBoxAxleSelect.SelectedIndexChanged -= comboBoxAxleSelect_SelectedIndexChanged;
		comboBoxAxleSelect.DataSource = null;
		comboBoxAxleSelect.Items.Clear();
		comboBoxAxleSelect.SelectedIndexChanged += comboBoxAxleSelect_SelectedIndexChanged;
	}

	private void PopulateDropDownList(bool emotNumChanged)
	{
		if (comboBoxAxleSelect.Items.Count == 0 || emotNumChanged)
		{
			comboBoxAxleSelect.SelectedIndexChanged -= comboBoxAxleSelect_SelectedIndexChanged;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("-1", Resources.Value_None);
			switch (TransEmotNumber)
			{
			case 1:
			case 2:
				dictionary.Add("1", Resources.Value_FirstRearDriveAxleActive);
				break;
			default:
				dictionary.Add("0", Resources.Value_TwoRearDriveAxles);
				dictionary.Add("1", Resources.Value_FirstRearDriveAxleActive);
				dictionary.Add("2", Resources.Value_SecondRearDriveAxleActive);
				break;
			}
			comboBoxAxleSelect.DataSource = new BindingSource(dictionary, null);
			comboBoxAxleSelect.DisplayMember = "Value";
			comboBoxAxleSelect.ValueMember = "Key";
			comboBoxAxleSelect.SelectedIndex = 0;
			comboBoxAxleSelect.SelectedIndexChanged += comboBoxAxleSelect_SelectedIndexChanged;
		}
	}

	private void UpdateUserInterface(bool emotNumChange)
	{
		PopulateDropDownList(emotNumChange);
		((Control)(object)digitalReadoutInstrumentEngineTorque2).Visible = TransEmotNumber == 3;
		((Control)(object)barInstrumentMotor3Speed).Visible = TransEmotNumber == 3;
		if (!Online)
		{
			buttonStart.Enabled = false;
			buttonStop.Enabled = false;
			comboBoxAxleSelect.Enabled = false;
			checkmarkStartTest.Checked = false;
			textBoxStartTest.Text = Resources.Message_TheCPCIsOffline;
			return;
		}
		comboBoxAxleSelect.Enabled = true;
		buttonStop.Enabled = testState == TestState.TestRunning;
		if (((KeyValuePair<string, string>)comboBoxAxleSelect.SelectedItem).Key == "-1")
		{
			buttonStart.Enabled = false;
		}
		else
		{
			Button button = buttonStart;
			bool enabled = (comboBoxAxleSelect.Enabled = testState == TestState.NotRunning && IgnitionIsOn);
			button.Enabled = enabled;
		}
		switch (testState)
		{
		case TestState.NotRunning:
			if (IgnitionIsOn)
			{
				checkmarkStartTest.Checked = true;
				textBoxStartTest.Text = Resources.Message_ReadyToStartTheDynamometerTestMode;
			}
			else
			{
				checkmarkStartTest.Checked = false;
				textBoxStartTest.Text = Resources.Message_TheDynamometerTestModeCannotBeStartedUntilTheIgnitionIsOn;
			}
			break;
		case TestState.TestRunning:
			if (IgnitionIsOn)
			{
				checkmarkStartTest.Checked = true;
				textBoxStartTest.Text = Resources.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit;
			}
			else
			{
				checkmarkStartTest.Checked = false;
				textBoxStartTest.Text = Resources.Message_TheIgnitionHasBeenTurnedOffWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode;
			}
			break;
		}
	}

	private void LogMessage(string text)
	{
		((CustomPanel)this).AddStatusMessage(text);
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelAll = new TableLayoutPanel();
		barInstrumentMotor3Speed = new BarInstrument();
		barInstrumentMotor1Speed = new BarInstrument();
		barInstrument2 = new BarInstrument();
		tableLayoutPanelLeft = new TableLayoutPanel();
		buttonStart = new Button();
		buttonStop = new Button();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanelCheckmarkAndLabel = new TableLayoutPanel();
		checkmarkStartTest = new Checkmark();
		textBoxStartTest = new TextBox();
		comboBoxAxleSelect = new ComboBox();
		labelAxelSelect = new System.Windows.Forms.Label();
		barInstrument3 = new BarInstrument();
		tableLayoutPanel3 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentgnitionStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineTorque2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineTorque1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentKickdown = new DigitalReadoutInstrument();
		barInstrumentTransOilTemp = new BarInstrument();
		((Control)(object)tableLayoutPanelAll).SuspendLayout();
		((Control)(object)tableLayoutPanelLeft).SuspendLayout();
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelAll, "tableLayoutPanelAll");
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrumentMotor3Speed, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrumentMotor1Speed, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrument2, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)tableLayoutPanelLeft, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrument3, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)tableLayoutPanel3, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrumentTransOilTemp, 4, 2);
		((Control)(object)tableLayoutPanelAll).Name = "tableLayoutPanelAll";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrumentMotor3Speed, 2);
		componentResourceManager.ApplyResources(barInstrumentMotor3Speed, "barInstrumentMotor3Speed");
		barInstrumentMotor3Speed.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrumentMotor3Speed).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentMotor3Speed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3");
		((Control)(object)barInstrumentMotor3Speed).Name = "barInstrumentMotor3Speed";
		((AxisSingleInstrumentBase)barInstrumentMotor3Speed).PreferredAxisRange = new AxisRange(0.0, 10000.0, (string)null);
		((SingleInstrumentBase)barInstrumentMotor3Speed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrumentMotor1Speed, 2);
		componentResourceManager.ApplyResources(barInstrumentMotor1Speed, "barInstrumentMotor1Speed");
		barInstrumentMotor1Speed.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrumentMotor1Speed).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentMotor1Speed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1");
		((Control)(object)barInstrumentMotor1Speed).Name = "barInstrumentMotor1Speed";
		((AxisSingleInstrumentBase)barInstrumentMotor1Speed).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
		((SingleInstrumentBase)barInstrumentMotor1Speed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrument2, 2);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelLeft, "tableLayoutPanelLeft");
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)tableLayoutPanelLeft, 2);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(buttonStart, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(buttonStop, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add((Control)(object)seekTimeListView1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add((Control)(object)tableLayoutPanelCheckmarkAndLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(comboBoxAxleSelect, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(labelAxelSelect, 0, 2);
		((Control)(object)tableLayoutPanelLeft).Name = "tableLayoutPanelLeft";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetRowSpan((Control)(object)tableLayoutPanelLeft, 6);
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		buttonStop.Click += buttonStop_Click;
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((TableLayoutPanel)(object)tableLayoutPanelLeft).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "DetTransDynoMode";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(tableLayoutPanelCheckmarkAndLabel, "tableLayoutPanelCheckmarkAndLabel");
		((TableLayoutPanel)(object)tableLayoutPanelLeft).SetColumnSpan((Control)(object)tableLayoutPanelCheckmarkAndLabel, 3);
		((TableLayoutPanel)(object)tableLayoutPanelCheckmarkAndLabel).Controls.Add((Control)(object)checkmarkStartTest, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelCheckmarkAndLabel).Controls.Add(textBoxStartTest, 1, 0);
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).Name = "tableLayoutPanelCheckmarkAndLabel";
		((TableLayoutPanel)(object)tableLayoutPanelLeft).SetRowSpan((Control)(object)tableLayoutPanelCheckmarkAndLabel, 2);
		componentResourceManager.ApplyResources(checkmarkStartTest, "checkmarkStartTest");
		((Control)(object)checkmarkStartTest).Name = "checkmarkStartTest";
		textBoxStartTest.Cursor = Cursors.Arrow;
		componentResourceManager.ApplyResources(textBoxStartTest, "textBoxStartTest");
		textBoxStartTest.Name = "textBoxStartTest";
		textBoxStartTest.ReadOnly = true;
		componentResourceManager.ApplyResources(comboBoxAxleSelect, "comboBoxAxleSelect");
		comboBoxAxleSelect.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxAxleSelect.FormattingEnabled = true;
		comboBoxAxleSelect.Name = "comboBoxAxleSelect";
		comboBoxAxleSelect.SelectedIndexChanged += comboBoxAxleSelect_SelectedIndexChanged;
		((TableLayoutPanel)(object)tableLayoutPanelLeft).SetColumnSpan((Control)labelAxelSelect, 3);
		componentResourceManager.ApplyResources(labelAxelSelect, "labelAxelSelect");
		labelAxelSelect.Name = "labelAxelSelect";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrument3, 2);
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((AxisSingleInstrumentBase)barInstrument3).PreferredAxisRange = new AxisRange(0.0, 100.0, (string)null);
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)tableLayoutPanel3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentgnitionStatus, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentEngineTorque2, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentEngineTorque1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentKickdown, 0, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetRowSpan((Control)(object)tableLayoutPanel3, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS052_CalculatedGear_CalculatedGear");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Desired_Gear_current_value");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentgnitionStatus, "digitalReadoutInstrumentgnitionStatus");
		digitalReadoutInstrumentgnitionStatus.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentgnitionStatus).FreezeValue = false;
		digitalReadoutInstrumentgnitionStatus.Gradient.Initialize((ValueState)0, 8);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(5, 10.0, (ValueState)0);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(6, 14.0, (ValueState)0);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(7, 15.0, (ValueState)0);
		digitalReadoutInstrumentgnitionStatus.Gradient.Modify(8, 2147483647.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentgnitionStatus).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS063_IgnitionSwitchStatus_IgnitionSwitchStatus");
		((Control)(object)digitalReadoutInstrumentgnitionStatus).Name = "digitalReadoutInstrumentgnitionStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentgnitionStatus).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentgnitionStatus.RepresentedStateChanged += digitalReadoutInstrumentgnitionStatus_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineTorque2, "digitalReadoutInstrumentEngineTorque2");
		digitalReadoutInstrumentEngineTorque2.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineTorque2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineTorque2).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS134_Current_Torque_Axle_2_Current_Torque_Axle_2");
		((Control)(object)digitalReadoutInstrumentEngineTorque2).Name = "digitalReadoutInstrumentEngineTorque2";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineTorque2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineTorque1, "digitalReadoutInstrumentEngineTorque1");
		digitalReadoutInstrumentEngineTorque1.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineTorque1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineTorque1).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS133_Current_Torque_Axle_1_Current_Torque_Axle_1");
		((Control)(object)digitalReadoutInstrumentEngineTorque1).Name = "digitalReadoutInstrumentEngineTorque1";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineTorque1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentKickdown, "digitalReadoutInstrumentKickdown");
		digitalReadoutInstrumentKickdown.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentKickdown).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentKickdown).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS004_Kickdown");
		((Control)(object)digitalReadoutInstrumentKickdown).Name = "digitalReadoutInstrumentKickdown";
		((SingleInstrumentBase)digitalReadoutInstrumentKickdown).UnitAlignment = StringAlignment.Near;
		barInstrumentTransOilTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentTransOilTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentTransOilTemp, "barInstrumentTransOilTemp");
		barInstrumentTransOilTemp.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrumentTransOilTemp).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentTransOilTemp).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Transmission_Oil_Temperature_current_value");
		((Control)(object)barInstrumentTransOilTemp).Name = "barInstrumentTransOilTemp";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetRowSpan((Control)(object)barInstrumentTransOilTemp, 4);
		((SingleInstrumentBase)barInstrumentTransOilTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentTransOilTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentTransOilTemp).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Transmission_Dyno_Mode");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelAll);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelAll).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelLeft).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).PerformLayout();
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
