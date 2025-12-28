using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__NGC_.panel;

public class UserPanel : CustomPanel
{
	private enum TestState
	{
		NotRunning,
		WaitingForEngineToStart,
		TestRunning
	}

	private readonly string McmName = "MCM21T";

	private readonly string Cpc3Name = "CPC302T";

	private readonly string Cpc5Name = "CPC501T";

	private readonly string Cpc5ceName = "CPC502T";

	private readonly string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private readonly Dictionary<string, int> valueMap = new Dictionary<string, int>
	{
		{
			Resources.Message_AutomaticShifting,
			3
		},
		{
			Resources.Message_ManualShifting,
			1
		}
	};

	private string dynoModeTestStartServiceQualifier;

	private string dynoModeTestStopServiceQualifier;

	private Channel cpc = null;

	private Channel mcm = null;

	private Channel tcm = null;

	private TestState testState;

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

	private DigitalReadoutInstrument digitalReadoutInstrumentKickdown;

	private TableLayoutPanel tableLayoutPanelLeft;

	private ComboBox comboBoxShiftType;

	private Button buttonStart;

	private BarInstrument barInstrument3;

	private TableLayoutPanel tableLayoutPanel3;

	private TableLayoutPanel tableLayoutPanelCheckmarkAndLabel;

	private Checkmark checkmarkStartTest;

	private TextBox textBoxStartTest;

	private TableLayoutPanel tableLayoutPanelAll;

	private bool Online => cpc != null && cpc.CommunicationsState == CommunicationsState.Online && mcm != null && mcm.CommunicationsState == CommunicationsState.Online;

	private bool EngineIsRunning
	{
		get
		{
			int num = Convert.ToInt32(GetInstrumentCurrentValue(engineSpeedInstrument));
			return num > 0;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		testState = TestState.NotRunning;
		comboBoxShiftType.DataSource = valueMap.Select((KeyValuePair<string, int> x) => x.Key).ToList();
		comboBoxShiftType.SelectedIndex = 0;
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		SetCPC(((CustomPanel)this).GetChannel("CPC302T", (ChannelLookupOptions)7));
		SetMcm(((CustomPanel)this).GetChannel(McmName));
		SetTcm(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
		UpdateUserInterface();
	}

	private void SetCPC(Channel cpc)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (this.cpc == cpc)
		{
			return;
		}
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
		}
		dynoModeTestStartServiceQualifier = string.Empty;
		dynoModeTestStopServiceQualifier = string.Empty;
		this.cpc = cpc;
		if (this.cpc != null)
		{
			if (string.Equals(this.cpc.Ecu.Name, Cpc3Name, StringComparison.OrdinalIgnoreCase))
			{
				dynoModeTestStartServiceQualifier = "RT_RC0412_Test_bench_status_Start";
				dynoModeTestStopServiceQualifier = "RT_RC0412_Test_bench_status_Stop";
				((SingleInstrumentBase)digitalReadoutInstrumentKickdown).Instrument = new Qualifier((QualifierTypes)1, this.cpc.Ecu.Name, "DT_DS255_Blocktransfer_Kickdown");
			}
			else if (string.Equals(this.cpc.Ecu.Name, Cpc5Name, StringComparison.OrdinalIgnoreCase))
			{
				dynoModeTestStartServiceQualifier = "RT_Activate_Test_Bench_Mode_Start";
				dynoModeTestStopServiceQualifier = "RT_Activate_Test_Bench_Mode_Stop";
				((SingleInstrumentBase)digitalReadoutInstrumentKickdown).Instrument = new Qualifier((QualifierTypes)1, this.cpc.Ecu.Name, "DT_DS255_Blocktransfer_Kickdown");
			}
			else if (string.Equals(this.cpc.Ecu.Name, Cpc5ceName, StringComparison.OrdinalIgnoreCase))
			{
				dynoModeTestStartServiceQualifier = "RT_Dyno_Mode_Activate_Test_Bench_Mode_Start";
				dynoModeTestStopServiceQualifier = "RT_Dyno_Mode_Activate_Test_Bench_Mode_Stop";
				((SingleInstrumentBase)digitalReadoutInstrumentKickdown).Instrument = new Qualifier((QualifierTypes)1, this.cpc.Ecu.Name, "DT_DS255_Blocktransfer_Kickdown");
			}
			this.cpc.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
		}
	}

	private void SetMcm(Channel mcm)
	{
		if (this.mcm == mcm)
		{
			return;
		}
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			if (engineSpeedInstrument != null)
			{
				engineSpeedInstrument.InstrumentUpdateEvent -= EngineSpeedUpdateEvent;
			}
		}
		this.mcm = mcm;
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			engineSpeedInstrument = mcm.Instruments[EngineSpeedInstrumentQualifier];
			if (engineSpeedInstrument != null)
			{
				engineSpeedInstrument.InstrumentUpdateEvent += EngineSpeedUpdateEvent;
			}
		}
	}

	private void SetTcm(Channel tcm)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Invalid comparison between Unknown and I4
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (this.tcm == tcm)
		{
			return;
		}
		this.tcm = tcm;
		if (this.tcm == null)
		{
			return;
		}
		Ecu ecu = null;
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			ecu = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecu == null || !(ecu.Identifier == tcm.Ecu.Identifier) || !(ecu.Name != tcm.Ecu.Name))
			{
				continue;
			}
			instrument = val.Instrument;
			QualifierTypes type = ((Qualifier)(ref instrument)).Type;
			string name = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			if (val.DataItem != null)
			{
				continue;
			}
			QualifierTypes val2 = (QualifierTypes)0;
			instrument = val.Instrument;
			if ((int)((Qualifier)(ref instrument)).Type == 1)
			{
				val2 = (QualifierTypes)8;
			}
			else
			{
				instrument = val.Instrument;
				if ((int)((Qualifier)(ref instrument)).Type == 8)
				{
					val2 = (QualifierTypes)1;
				}
			}
			QualifierTypes val3 = val2;
			string name2 = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(val3, name2, ((Qualifier)(ref instrument)).Name);
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
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

	private void EngineSpeedUpdateEvent(object sender, ResultEventArgs e)
	{
		if (testState == TestState.WaitingForEngineToStart && EngineIsRunning)
		{
			ContinueTest();
		}
		else if (testState == TestState.TestRunning && !EngineIsRunning)
		{
			LogMessage(Resources.Message_StoppingTheDynoTestModeBecauseTheEngineStopped);
			StopTest();
		}
		UpdateUserInterface();
	}

	private void StartTest()
	{
		testState = TestState.WaitingForEngineToStart;
		LogMessage(Resources.Message_TheStartButtonHasBeenPressed);
		LogMessage(Resources.Message_WaitingForTheEngineToBeStarted);
		UpdateUserInterface();
	}

	private void StopTest()
	{
		Service service = ((CustomPanel)this).GetService(cpc.Ecu.Name, dynoModeTestStopServiceQualifier);
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

	private void ContinueTest()
	{
		int num = valueMap[comboBoxShiftType.SelectedValue as string];
		Service service = ((CustomPanel)this).GetService(cpc.Ecu.Name, dynoModeTestStartServiceQualifier);
		if (service != null)
		{
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(num);
			service.InputValues[1].Value = service.InputValues[1].Choices.GetItemFromRawValue(3);
			service.InputValues[2].Value = service.InputValues[2].Choices.GetItemFromRawValue(3);
			service.ServiceCompleteEvent += StartDynoModeServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
		else
		{
			LogMessage(Resources.Message_EndingTheTestTheService + dynoModeTestStartServiceQualifier + Resources.Message_IsNotAvailable);
			StopTest();
		}
	}

	private void StartDynoModeServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= StartDynoModeServiceCompleteEvent;
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
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		textBoxStartTest.Text = Resources.Message_PleaseStartTheEngine;
		StartTest();
	}

	private void buttonStop_Click(object sender, EventArgs e)
	{
		StopTest();
	}

	private void UpdateUserInterface()
	{
		if (!Online)
		{
			buttonStart.Enabled = false;
			buttonStop.Enabled = false;
			comboBoxShiftType.Enabled = false;
			checkmarkStartTest.Checked = false;
			textBoxStartTest.Text = Resources.Message_TheCPCIsOffline;
			return;
		}
		buttonStart.Enabled = !EngineIsRunning;
		buttonStop.Enabled = true;
		comboBoxShiftType.Enabled = testState == TestState.NotRunning;
		switch (testState)
		{
		case TestState.NotRunning:
			if (!EngineIsRunning)
			{
				checkmarkStartTest.Checked = true;
				textBoxStartTest.Text = Resources.Message_ReadyToStartTheDynamometerTestMode;
			}
			else
			{
				checkmarkStartTest.Checked = false;
				textBoxStartTest.Text = Resources.Message_TheDynamometerTestModeCannotBeStartedUntilTheEngineIsOffAndTheIgnitionIsOn;
			}
			break;
		case TestState.WaitingForEngineToStart:
			if (EngineIsRunning)
			{
				checkmarkStartTest.Checked = true;
				textBoxStartTest.Text = Resources.Message_TheEngineHasBeenStartedAndTheDynamometerTestModeHasStarted;
			}
			else
			{
				buttonStart.Enabled = false;
				checkmarkStartTest.Checked = false;
				textBoxStartTest.Text = Resources.Message_WaitingForYouToStartTheEngine;
			}
			break;
		case TestState.TestRunning:
			if (EngineIsRunning)
			{
				checkmarkStartTest.Checked = true;
				textBoxStartTest.Text = Resources.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit;
			}
			else
			{
				checkmarkStartTest.Checked = false;
				textBoxStartTest.Text = Resources.Message_TheEngineHasBeenStoppedWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode;
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
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
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentKickdown = new DigitalReadoutInstrument();
		tableLayoutPanelCheckmarkAndLabel = new TableLayoutPanel();
		checkmarkStartTest = new Checkmark();
		textBoxStartTest = new TextBox();
		tableLayoutPanelLeft = new TableLayoutPanel();
		buttonStart = new Button();
		buttonStop = new Button();
		comboBoxShiftType = new ComboBox();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanelAll = new TableLayoutPanel();
		barInstrument1 = new BarInstrument();
		barInstrument2 = new BarInstrument();
		barInstrument3 = new BarInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		barInstrumentTransOilTemp = new BarInstrument();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).SuspendLayout();
		((Control)(object)tableLayoutPanelLeft).SuspendLayout();
		((Control)(object)tableLayoutPanelAll).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)tableLayoutPanel3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument6, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentKickdown, 2, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS001_Requested_Torque");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineTorque");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentKickdown, "digitalReadoutInstrumentKickdown");
		digitalReadoutInstrumentKickdown.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrumentKickdown).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentKickdown).Instrument = new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_Kickdown");
		((Control)(object)digitalReadoutInstrumentKickdown).Name = "digitalReadoutInstrumentKickdown";
		((SingleInstrumentBase)digitalReadoutInstrumentKickdown).UnitAlignment = StringAlignment.Near;
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
		componentResourceManager.ApplyResources(tableLayoutPanelLeft, "tableLayoutPanelLeft");
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)tableLayoutPanelLeft, 2);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(buttonStart, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(buttonStop, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add(comboBoxShiftType, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add((Control)(object)seekTimeListView1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelLeft).Controls.Add((Control)(object)tableLayoutPanelCheckmarkAndLabel, 0, 0);
		((Control)(object)tableLayoutPanelLeft).Name = "tableLayoutPanelLeft";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetRowSpan((Control)(object)tableLayoutPanelLeft, 5);
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
		comboBoxShiftType.DisplayMember = "transmissionType";
		componentResourceManager.ApplyResources(comboBoxShiftType, "comboBoxShiftType");
		comboBoxShiftType.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxShiftType.FormattingEnabled = true;
		comboBoxShiftType.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBoxShiftType.Items"),
			componentResourceManager.GetString("comboBoxShiftType.Items1")
		});
		comboBoxShiftType.Name = "comboBoxShiftType";
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
		componentResourceManager.ApplyResources(tableLayoutPanelAll, "tableLayoutPanelAll");
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrument1, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrument2, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)tableLayoutPanelLeft, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrument3, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)digitalReadoutInstrument2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)digitalReadoutInstrument3, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)tableLayoutPanel3, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAll).Controls.Add((Control)(object)barInstrumentTransOilTemp, 4, 1);
		((Control)(object)tableLayoutPanelAll).Name = "tableLayoutPanelAll";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrument1, 2);
		componentResourceManager.ApplyResources(barInstrument1, "barInstrument1");
		barInstrument1.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrument1).FreezeValue = false;
		((SingleInstrumentBase)barInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)barInstrument1).Name = "barInstrument1";
		((SingleInstrumentBase)barInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrument2, 2);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetColumnSpan((Control)(object)barInstrument3, 2);
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = "TDMBar";
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd07_Sollgang_Sollgang");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "TDMReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		barInstrumentTransOilTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentTransOilTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentTransOilTemp, "barInstrumentTransOilTemp");
		barInstrumentTransOilTemp.FontGroup = "";
		((SingleInstrumentBase)barInstrumentTransOilTemp).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentTransOilTemp).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd16_Getriebe_Oelltemperatur_Getriebe_Oelltemperatur");
		((Control)(object)barInstrumentTransOilTemp).Name = "barInstrumentTransOilTemp";
		((TableLayoutPanel)(object)tableLayoutPanelAll).SetRowSpan((Control)(object)barInstrumentTransOilTemp, 4);
		((SingleInstrumentBase)barInstrumentTransOilTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentTransOilTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentTransOilTemp).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Transmission_Dyno_Mode");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelAll);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelCheckmarkAndLabel).PerformLayout();
		((Control)(object)tableLayoutPanelLeft).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAll).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
