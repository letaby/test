using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

public class UserPanel : CustomPanel
{
	private WarningManager warningManager;

	private Channel tcm;

	private OuterTest outerTest = null;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private BarInstrument barInstrumentClutchDesiredValue;

	private BarInstrument barInstrumentClutchDisplacement;

	private SeekTimeListView seekTimeListViewLog;

	private TableLayoutPanel tableLayoutPanelTestControls;

	private Checkmark checkmarkTestStatus;

	private System.Windows.Forms.Label scalingLabelTestStatus;

	private Button buttonStartTest;

	private TimerControl timerControl1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheckStatus;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private BarInstrument barInstrumentAirSupplyPressure;

	private BarInstrument barInstrumentCounterShaftSpeed;

	private BarInstrument barInstrumentActualEngineSpeed;

	private Checkmark checkmarkTestResult1;

	private Checkmark checkmarkTestResult2;

	private Checkmark checkmarkTestResult3;

	private ScalingLabel scalingLabelTestResult1;

	private ScalingLabel scalingLabelTestResult2;

	private ScalingLabel scalingLabelTestResult3;

	private TableLayoutPanel tableLayoutPanel3;

	private System.Windows.Forms.Label labelDirections;

	private Button buttonStopTest;

	private TableLayoutPanel tableLayoutPanelResults;

	private System.Windows.Forms.Label labelAdditionalInfo;

	private DigitalReadoutInstrument digitalReadoutInstrumentGearEngaged;

	private bool CanClose => outerTest == null || !outerTest.OuterTestIsRunning;

	public UserPanel()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += OnParentFormClosing;
		warningManager = new WarningManager(string.Empty, (string)null, seekTimeListViewLog.RequiredUserLabelPrefix);
		outerTest = new OuterTest(LogText, DisplayDirections, UpdateTestResults, barInstrumentClutchDisplacement, barInstrumentCounterShaftSpeed, barInstrumentActualEngineSpeed, timerControl1);
		buttonStartTest.Click += buttonStartTest_Click;
		buttonStopTest.Click += buttonStopTest_Click;
		barInstrumentAirSupplyPressure.RepresentedStateChanged += TestConditionChanged;
		barInstrumentActualEngineSpeed.RepresentedStateChanged += TestConditionChanged;
		digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += TestConditionChanged;
		digitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += TestConditionChanged;
		digitalReadoutInstrumentGearEngaged.RepresentedStateChanged += TestConditionChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((CustomPanel)this).ParentFormClosing -= OnParentFormClosing;
			SetTCM(null);
			((Control)this).Tag = new object[2]
			{
				false,
				string.Empty
			};
		}
	}

	public override void OnChannelsChanged()
	{
		SetTCM(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)5));
		if (tcm == null && outerTest != null && outerTest.OuterTestIsRunning)
		{
			StopTest();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			SetTCM(null);
			outerTest.Dispose(disposing);
		}
	}

	private void SetTCM(Channel tcm)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Invalid comparison between Unknown and I4
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Invalid comparison between Unknown and I4
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (this.tcm == tcm)
		{
			return;
		}
		if (this.tcm != null)
		{
			this.tcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		this.tcm = tcm;
		if (this.tcm == null)
		{
			return;
		}
		this.tcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
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

	private void StopTest()
	{
		outerTest.StopTest();
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewLog.RequiredUserLabelPrefix, text);
	}

	private void DisplayDirections(string directions)
	{
		labelDirections.Text = directions;
	}

	private void TestConditionChanged(object sender, EventArgs e)
	{
		UpdateControlState();
	}

	public void UpdateTestResults(int testNumber, TestResults results, string errorString)
	{
		string text = string.Empty;
		CheckState checkState = CheckState.Indeterminate;
		switch (results)
		{
		case TestResults.Success:
			text = string.Format(CultureInfo.CurrentCulture, Resources.TestNPassed, testNumber);
			checkState = CheckState.Checked;
			break;
		case TestResults.Fail:
			text = string.Format(CultureInfo.CurrentCulture, Resources.TestNFailed, testNumber);
			checkState = CheckState.Unchecked;
			break;
		case TestResults.Error:
			text = string.Format(CultureInfo.CurrentCulture, Resources.ErrorAnErrorOccurred, testNumber, errorString);
			break;
		case TestResults.NotRun:
			text = string.Format(CultureInfo.CurrentCulture, Resources.TestNNotRun, testNumber);
			break;
		case TestResults.StopTest:
			text = string.Format(CultureInfo.CurrentCulture, Resources.TestNStopped, testNumber);
			checkState = CheckState.Unchecked;
			break;
		}
		Checkmark val = null;
		ScalingLabel val2 = null;
		switch (testNumber)
		{
		case 1:
			val = checkmarkTestResult1;
			val2 = scalingLabelTestResult1;
			break;
		case 2:
			val = checkmarkTestResult2;
			val2 = scalingLabelTestResult2;
			break;
		case 3:
			val = checkmarkTestResult3;
			val2 = scalingLabelTestResult3;
			break;
		}
		((Control)(object)val2).Text = text;
		val.CheckState = checkState;
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateControlState();
	}

	private void UpdateControlState()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Invalid comparison between Unknown and I4
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Invalid comparison between Unknown and I4
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Invalid comparison between Unknown and I4
		bool flag = true;
		StringBuilder stringBuilder = new StringBuilder();
		labelAdditionalInfo.Text = string.Empty;
		if (!outerTest.OuterTestIsRunning)
		{
			if ((int)barInstrumentAirSupplyPressure.RepresentedState != 1)
			{
				flag = false;
				stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.TransmissionAirSupplyPressureTooLow, ((SingleInstrumentBase)barInstrumentAirSupplyPressure).Title));
			}
			if ((int)digitalReadoutInstrumentGearEngaged.RepresentedState != 1)
			{
				flag = false;
				stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.TestStateTransmissionInGear, ((SingleInstrumentBase)digitalReadoutInstrumentGearEngaged).Title));
			}
			if ((int)digitalReadoutInstrumentVehicleCheckStatus.RepresentedState != 1)
			{
				flag = false;
				stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.VehicleCheckStatusFalse, ((SingleInstrumentBase)digitalReadoutInstrumentGearEngaged).Title));
			}
			if ((int)digitalReadoutInstrumentParkingBrake.RepresentedState != 1)
			{
				flag = false;
				stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.TestStatusParkingBrakeOff, ((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Title));
			}
			if ((int)barInstrumentActualEngineSpeed.RepresentedState != 1)
			{
				flag = false;
				stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.TestStatusEngineRunning, ((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Title));
			}
			checkmarkTestStatus.Checked = flag;
			scalingLabelTestStatus.Text = (flag ? Resources.TestStateTestCanStart : Resources.TestStateTestCannotStart);
			labelAdditionalInfo.Text = stringBuilder.ToString();
		}
		else
		{
			scalingLabelTestStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.TestStateTestNIsRunning, outerTest.TestNumber);
		}
		buttonStartTest.Enabled = flag && !outerTest.OuterTestIsRunning;
		buttonStopTest.Enabled = outerTest.OuterTestIsRunning;
	}

	private void buttonStartTest_Click(object sender, EventArgs e)
	{
		UpdateTestResults(1, TestResults.NotRun, string.Empty);
		UpdateTestResults(2, TestResults.NotRun, string.Empty);
		UpdateTestResults(3, TestResults.NotRun, string.Empty);
		outerTest.StartTest(tcm);
	}

	private void buttonStopTest_Click(object sender, EventArgs e)
	{
		StopTest();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
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
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelTestControls = new TableLayoutPanel();
		buttonStartTest = new Button();
		checkmarkTestStatus = new Checkmark();
		buttonStopTest = new Button();
		scalingLabelTestStatus = new System.Windows.Forms.Label();
		timerControl1 = new TimerControl();
		labelAdditionalInfo = new System.Windows.Forms.Label();
		tableLayoutPanel3 = new TableLayoutPanel();
		digitalReadoutInstrumentGearEngaged = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		tableLayoutPanelResults = new TableLayoutPanel();
		scalingLabelTestResult3 = new ScalingLabel();
		scalingLabelTestResult2 = new ScalingLabel();
		scalingLabelTestResult1 = new ScalingLabel();
		checkmarkTestResult3 = new Checkmark();
		checkmarkTestResult1 = new Checkmark();
		checkmarkTestResult2 = new Checkmark();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		barInstrumentClutchDesiredValue = new BarInstrument();
		barInstrumentClutchDisplacement = new BarInstrument();
		labelDirections = new System.Windows.Forms.Label();
		seekTimeListViewLog = new SeekTimeListView();
		barInstrumentAirSupplyPressure = new BarInstrument();
		barInstrumentCounterShaftSpeed = new BarInstrument();
		barInstrumentActualEngineSpeed = new BarInstrument();
		((Control)(object)tableLayoutPanelTestControls).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanelResults).SuspendLayout();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelTestControls, "tableLayoutPanelTestControls");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanelTestControls, 3);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(buttonStartTest, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)checkmarkTestStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(buttonStopTest, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(scalingLabelTestStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)timerControl1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(labelAdditionalInfo, 0, 1);
		((Control)(object)tableLayoutPanelTestControls).Name = "tableLayoutPanelTestControls";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)(object)tableLayoutPanelTestControls, 2);
		componentResourceManager.ApplyResources(buttonStartTest, "buttonStartTest");
		buttonStartTest.Name = "buttonStartTest";
		buttonStartTest.UseCompatibleTextRendering = true;
		buttonStartTest.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(checkmarkTestStatus, "checkmarkTestStatus");
		((Control)(object)checkmarkTestStatus).Name = "checkmarkTestStatus";
		componentResourceManager.ApplyResources(buttonStopTest, "buttonStopTest");
		buttonStopTest.Name = "buttonStopTest";
		buttonStopTest.UseCompatibleTextRendering = true;
		buttonStopTest.UseVisualStyleBackColor = true;
		scalingLabelTestStatus.BackColor = SystemColors.Info;
		componentResourceManager.ApplyResources(scalingLabelTestStatus, "scalingLabelTestStatus");
		scalingLabelTestStatus.Name = "scalingLabelTestStatus";
		scalingLabelTestStatus.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)(object)timerControl1, 2);
		componentResourceManager.ApplyResources(timerControl1, "timerControl1");
		timerControl1.Duration = TimeSpan.Parse("00:01:00");
		timerControl1.FontGroup = null;
		((Control)(object)timerControl1).Name = "timerControl1";
		timerControl1.TimerCountdownCompletedDisplayMessage = null;
		labelAdditionalInfo.BackColor = SystemColors.Info;
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)labelAdditionalInfo, 2);
		componentResourceManager.ApplyResources(labelAdditionalInfo, "labelAdditionalInfo");
		labelAdditionalInfo.Name = "labelAdditionalInfo";
		labelAdditionalInfo.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentGearEngaged, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleCheckStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 0, 2);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)(object)tableLayoutPanel3, 5);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentGearEngaged, "digitalReadoutInstrumentGearEngaged");
		digitalReadoutInstrumentGearEngaged.FontGroup = "bars";
		((SingleInstrumentBase)digitalReadoutInstrumentGearEngaged).FreezeValue = false;
		digitalReadoutInstrumentGearEngaged.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentGearEngaged.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentGearEngaged.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentGearEngaged).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang");
		((Control)(object)digitalReadoutInstrumentGearEngaged).Name = "digitalReadoutInstrumentGearEngaged";
		((SingleInstrumentBase)digitalReadoutInstrumentGearEngaged).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleCheckStatus, "digitalReadoutInstrumentVehicleCheckStatus");
		digitalReadoutInstrumentVehicleCheckStatus.FontGroup = "bars";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrumentVehicleCheckStatus).Name = "digitalReadoutInstrumentVehicleCheckStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelResults, "tableLayoutPanelResults");
		((TableLayoutPanel)(object)tableLayoutPanelResults).Controls.Add((Control)(object)scalingLabelTestResult3, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelResults).Controls.Add((Control)(object)scalingLabelTestResult2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelResults).Controls.Add((Control)(object)scalingLabelTestResult1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelResults).Controls.Add((Control)(object)checkmarkTestResult3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelResults).Controls.Add((Control)(object)checkmarkTestResult1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelResults).Controls.Add((Control)(object)checkmarkTestResult2, 0, 1);
		((Control)(object)tableLayoutPanelResults).Name = "tableLayoutPanelResults";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)(object)tableLayoutPanelResults, 2);
		scalingLabelTestResult3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelTestResult3, "scalingLabelTestResult3");
		scalingLabelTestResult3.FontGroup = "TestResults";
		scalingLabelTestResult3.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelTestResult3).Name = "scalingLabelTestResult3";
		scalingLabelTestResult2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelTestResult2, "scalingLabelTestResult2");
		scalingLabelTestResult2.FontGroup = "TestResults";
		scalingLabelTestResult2.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelTestResult2).Name = "scalingLabelTestResult2";
		scalingLabelTestResult1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelTestResult1, "scalingLabelTestResult1");
		scalingLabelTestResult1.FontGroup = "TestResults";
		scalingLabelTestResult1.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelTestResult1).Name = "scalingLabelTestResult1";
		checkmarkTestResult3.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkTestResult3, "checkmarkTestResult3");
		((Control)(object)checkmarkTestResult3).Name = "checkmarkTestResult3";
		checkmarkTestResult1.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkTestResult1, "checkmarkTestResult1");
		((Control)(object)checkmarkTestResult1).Name = "checkmarkTestResult1";
		checkmarkTestResult2.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkTestResult2, "checkmarkTestResult2");
		((Control)(object)checkmarkTestResult2).Name = "checkmarkTestResult2";
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentClutchDesiredValue, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentClutchDisplacement, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelDirections, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelTestControls, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentAirSupplyPressure, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentCounterShaftSpeed, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentActualEngineSpeed, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanel3, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelResults, 3, 6);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentClutchDesiredValue, 3);
		componentResourceManager.ApplyResources(barInstrumentClutchDesiredValue, "barInstrumentClutchDesiredValue");
		barInstrumentClutchDesiredValue.FontGroup = "bars";
		((SingleInstrumentBase)barInstrumentClutchDesiredValue).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentClutchDesiredValue).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2314_Kupplungssollwert_Kupplungssollwert");
		((Control)(object)barInstrumentClutchDesiredValue).Name = "barInstrumentClutchDesiredValue";
		((SingleInstrumentBase)barInstrumentClutchDesiredValue).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentClutchDisplacement, 3);
		componentResourceManager.ApplyResources(barInstrumentClutchDisplacement, "barInstrumentClutchDisplacement");
		barInstrumentClutchDisplacement.FontGroup = "bars";
		((SingleInstrumentBase)barInstrumentClutchDisplacement).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentClutchDisplacement).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung");
		((Control)(object)barInstrumentClutchDisplacement).Name = "barInstrumentClutchDisplacement";
		((SingleInstrumentBase)barInstrumentClutchDisplacement).UnitAlignment = StringAlignment.Near;
		labelDirections.BackColor = SystemColors.Info;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)labelDirections, 4);
		componentResourceManager.ApplyResources(labelDirections, "labelDirections");
		labelDirections.Name = "labelDirections";
		labelDirections.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)seekTimeListViewLog, 4);
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "DetroitTransmissionClutchControl";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)(object)seekTimeListViewLog, 3);
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentAirSupplyPressure, 3);
		componentResourceManager.ApplyResources(barInstrumentAirSupplyPressure, "barInstrumentAirSupplyPressure");
		barInstrumentAirSupplyPressure.FontGroup = "bars";
		((SingleInstrumentBase)barInstrumentAirSupplyPressure).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentAirSupplyPressure).Gradient.Initialize((ValueState)3, 1, "psi");
		((AxisSingleInstrumentBase)barInstrumentAirSupplyPressure).Gradient.Modify(1, 100.0, (ValueState)1);
		((SingleInstrumentBase)barInstrumentAirSupplyPressure).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
		((Control)(object)barInstrumentAirSupplyPressure).Name = "barInstrumentAirSupplyPressure";
		((SingleInstrumentBase)barInstrumentAirSupplyPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentCounterShaftSpeed, 3);
		componentResourceManager.ApplyResources(barInstrumentCounterShaftSpeed, "barInstrumentCounterShaftSpeed");
		barInstrumentCounterShaftSpeed.FontGroup = "bars";
		((SingleInstrumentBase)barInstrumentCounterShaftSpeed).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentCounterShaftSpeed).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle");
		((Control)(object)barInstrumentCounterShaftSpeed).Name = "barInstrumentCounterShaftSpeed";
		((AxisSingleInstrumentBase)barInstrumentCounterShaftSpeed).PreferredAxisRange = new AxisRange(0.0, 100.0, (string)null);
		((SingleInstrumentBase)barInstrumentCounterShaftSpeed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentActualEngineSpeed, 3);
		componentResourceManager.ApplyResources(barInstrumentActualEngineSpeed, "barInstrumentActualEngineSpeed");
		barInstrumentActualEngineSpeed.FontGroup = "bars";
		((SingleInstrumentBase)barInstrumentActualEngineSpeed).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentActualEngineSpeed).Gradient.Initialize((ValueState)0, 0, "rpm");
		((SingleInstrumentBase)barInstrumentActualEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)barInstrumentActualEngineSpeed).Name = "barInstrumentActualEngineSpeed";
		((AxisSingleInstrumentBase)barInstrumentActualEngineSpeed).PreferredAxisRange = new AxisRange(0.0, 2000.0, (string)null);
		((SingleInstrumentBase)barInstrumentActualEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelTestControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelResults).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
