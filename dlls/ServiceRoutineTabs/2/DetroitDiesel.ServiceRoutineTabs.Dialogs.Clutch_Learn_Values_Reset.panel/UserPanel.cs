using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clutch_Learn_Values_Reset.panel;

public class UserPanel : CustomPanel
{
	private Channel tcm = null;

	private StringBuilder message = new StringBuilder();

	private bool canStart = false;

	private bool isBusy = false;

	private bool procedureHasBeenRun = false;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrumentNoShifting;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineStandstill;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleStandStill;

	private DigitalReadoutInstrument digitalReadoutInstrumentClutchMinimum;

	private Checkmark checkmarkStatus;

	private TableLayoutPanel tableLayoutPanel2;

	private TableLayoutPanel tableLayoutPanel3;

	private CheckBox checkBoxClutchWasReplaced;

	private DigitalReadoutInstrument digitalReadoutInstrumentNoLearningProcedure;

	private DigitalReadoutInstrument digitalReadoutInstrumentNeutral;

	private TextBox textBoxStartConditionStatus;

	private TableLayoutPanel tableLayoutPanel4;

	private DigitalReadoutInstrument digitalReadoutInstrumentClutchMaximum;

	private DigitalReadoutInstrument digitalReadoutInstrumentClutchFacingWearActualValue;

	private DigitalReadoutInstrument digitalReadoutInstrumentClutchFacingRemainingThickness;

	private RunServicesButton runServiceButtonStart;

	private SeekTimeListView seekTimeListView1;

	private bool TcmOnline => tcm != null && tcm.Online;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (((RunSharedProcedureButtonBase)runServiceButtonStart).IsBusy)
		{
			e.Cancel = true;
		}
	}

	private void SetTcm(Channel tcm)
	{
		if (this.tcm != tcm)
		{
			checkBoxClutchWasReplaced.Checked = false;
			this.tcm = tcm;
		}
	}

	public override void OnChannelsChanged()
	{
		((CustomPanel)this).OnChannelsChanged();
		SetTcm(((CustomPanel)this).GetChannel("TCM05T"));
		UpdateUserInterface();
	}

	private void UpdateLog(string logMessage)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, logMessage);
	}

	private void UpdateCannotStartMessage(string condition)
	{
		if (canStart)
		{
			message.AppendLine(procedureHasBeenRun ? Resources.MessageTheProcedureCannotBeRunAgain : Resources.MessageTheProcedureCannotStart);
		}
		else
		{
			message.AppendLine(",");
		}
		message.AppendFormat(CultureInfo.CurrentCulture, " {0}", condition);
		canStart = false;
	}

	private void UpdateUserInterface()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Invalid comparison between Unknown and I4
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Invalid comparison between Unknown and I4
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Invalid comparison between Unknown and I4
		message.Clear();
		if (isBusy)
		{
			message.Append(Resources.MessageTheProcedureIsRunning);
		}
		else
		{
			canStart = TcmOnline;
			if (!canStart)
			{
				message.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}", Resources.MessageTheProcedureCannotStart, Resources.ConditionTcmOnline);
			}
			else
			{
				if ((int)digitalReadoutInstrumentVehicleStandStill.RepresentedState != 1)
				{
					UpdateCannotStartMessage(Resources.ConditionVehicleStandstill);
				}
				if ((int)digitalReadoutInstrumentParkBrake.RepresentedState != 1)
				{
					UpdateCannotStartMessage(Resources.ConditionParkingBrakeSet);
				}
				if ((int)digitalReadoutInstrumentEngineStandstill.RepresentedState != 1)
				{
					UpdateCannotStartMessage(Resources.ConditionEngineStopped);
				}
				if ((int)digitalReadoutInstrumentNoShifting.RepresentedState != 1)
				{
					UpdateCannotStartMessage(Resources.ConditionNoShifting);
				}
				if ((int)digitalReadoutInstrumentNeutral.RepresentedState != 1)
				{
					UpdateCannotStartMessage(Resources.ConditionTransmissionInNeutral);
				}
				if ((int)digitalReadoutInstrumentNoLearningProcedure.RepresentedState != 1)
				{
					UpdateCannotStartMessage(Resources.ConditionNoLearningProcess);
				}
				if (!checkBoxClutchWasReplaced.Checked)
				{
					UpdateCannotStartMessage(Resources.ConditionClutchReplacedChecked);
				}
				if (canStart)
				{
					message.Append(procedureHasBeenRun ? Resources.MessageTheProcedureHasAlreadyBeenRun : Resources.MessageTheProcedureCanStart);
				}
			}
		}
		message.Append(".");
		((Control)(object)runServiceButtonStart).Enabled = canStart && !isBusy;
		checkmarkStatus.Checked = canStart && !isBusy;
		textBoxStartConditionStatus.Text = message.ToString();
	}

	private void digitalReadoutInstrumentVehicleStandStill_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentEngineStandstill_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentNoShifting_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentNeutral_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentNoLearningProcedure_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentRepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void checkBoxClutchWasReplaced_MouseClick(object sender, MouseEventArgs e)
	{
		if (checkBoxClutchWasReplaced.Checked)
		{
			UpdateLog(Resources.MessageClutchReplacedChecked);
		}
		UpdateUserInterface();
	}

	private void UpdateLogFileWithNewClutchInstrumentValues()
	{
		UpdateLog(string.Format(CultureInfo.CurrentCulture, Resources.MessageNewClutchMinimumValue, (((SingleInstrumentBase)digitalReadoutInstrumentClutchMinimum).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentClutchMinimum).DataItem.Value != null) ? ((SingleInstrumentBase)digitalReadoutInstrumentClutchMinimum).DataItem.Value : Resources.MessageValueNotAvailable));
		UpdateLog(string.Format(CultureInfo.CurrentCulture, Resources.MessageNewClutchMaximumValue, (((SingleInstrumentBase)digitalReadoutInstrumentClutchMaximum).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentClutchMaximum).DataItem.Value != null) ? ((SingleInstrumentBase)digitalReadoutInstrumentClutchMaximum).DataItem.Value : Resources.MessageValueNotAvailable));
		UpdateLog(string.Format(CultureInfo.CurrentCulture, Resources.MessageNewClutchFacingWearActualValue, (((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingWearActualValue).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingWearActualValue).DataItem.Value != null) ? ((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingWearActualValue).DataItem.Value : Resources.MessageValueNotAvailable));
		UpdateLog(string.Format(CultureInfo.CurrentCulture, Resources.MessageNewClutchFacingRemainingThickness, (((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingRemainingThickness).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingRemainingThickness).DataItem.Value != null) ? ((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingRemainingThickness).DataItem.Value : Resources.MessageValueNotAvailable));
	}

	private void runServiceButtonStart_Complete(object sender, PassFailResultEventArgs e)
	{
		isBusy = false;
		procedureHasBeenRun = true;
		checkBoxClutchWasReplaced.Checked = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			UpdateLog(Resources.MessageTheProcedureRanSuccessfully);
			string text = string.Format(CultureInfo.CurrentCulture, "{0}\r\n\r\n1. {1}\r\n2. {2}\r\n3. {3}\r\n4. {4}", Resources.MessageRunTheTCMLearnProcedurePart1, Resources.MessageRunTheTCMLearnProcedurePart2, Resources.MessageRunTheTCMLearnProcedurePart3, Resources.MessageRunTheTCMLearnProcedurePart4, Resources.MessageRunTheTCMLearnProcedurePart5);
			MessageBox.Show(text, Resources.CaptionRunTheTCMLearnProcedure, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification, displayHelpButton: false);
			UpdateLog(string.Format(CultureInfo.CurrentCulture, Resources.MessageTcmOnline, (tcm != null) ? tcm.CommunicationsState.ToString() : Resources.MessageOffline));
			UpdateLogFileWithNewClutchInstrumentValues();
			UpdateLog(text);
		}
		else
		{
			UpdateLog(Resources.MessageTheProcedureFailed);
			if (((ResultEventArgs)(object)e).Exception != null)
			{
				UpdateLog(string.Format(CultureInfo.CurrentCulture, Resources.MessageErrorReturned, ((ResultEventArgs)(object)e).Exception.Message));
			}
		}
		UpdateUserInterface();
	}

	private void runServiceButtonStart_Click(object sender, EventArgs e)
	{
		isBusy = true;
		UpdateLog(Resources.MessageTheProcedureStarted);
		UpdateUserInterface();
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
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
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmarkStatus = new Checkmark();
		textBoxStartConditionStatus = new TextBox();
		digitalReadoutInstrumentClutchMinimum = new DigitalReadoutInstrument();
		digitalReadoutInstrumentClutchMaximum = new DigitalReadoutInstrument();
		tableLayoutPanel3 = new TableLayoutPanel();
		checkBoxClutchWasReplaced = new CheckBox();
		runServiceButtonStart = new RunServicesButton();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentNoShifting = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleStandStill = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineStandstill = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentNeutral = new DigitalReadoutInstrument();
		digitalReadoutInstrumentNoLearningProcedure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentClutchFacingWearActualValue = new DigitalReadoutInstrument();
		digitalReadoutInstrumentClutchFacingRemainingThickness = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((ISupportInitialize)runServiceButtonStart).BeginInit();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentClutchMinimum, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentClutchMaximum, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel4, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentClutchFacingWearActualValue, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentClutchFacingRemainingThickness, 1, 5);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 2);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "ClutchLearnValuesReset";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textBoxStartConditionStatus, 1, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		textBoxStartConditionStatus.BorderStyle = BorderStyle.None;
		componentResourceManager.ApplyResources(textBoxStartConditionStatus, "textBoxStartConditionStatus");
		textBoxStartConditionStatus.Name = "textBoxStartConditionStatus";
		textBoxStartConditionStatus.ReadOnly = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentClutchMinimum, "digitalReadoutInstrumentClutchMinimum");
		digitalReadoutInstrumentClutchMinimum.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchMinimum).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentClutchMinimum).Instrument = new Qualifier((QualifierTypes)8, "TCM05T", "DT_STO_2316_Clutch_Minimum_learned_value_Clutch_Minimum_learned_value");
		((Control)(object)digitalReadoutInstrumentClutchMinimum).Name = "digitalReadoutInstrumentClutchMinimum";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchMinimum).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentClutchMaximum, "digitalReadoutInstrumentClutchMaximum");
		digitalReadoutInstrumentClutchMaximum.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchMaximum).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentClutchMaximum).Instrument = new Qualifier((QualifierTypes)8, "TCM05T", "DT_STO_2317_Clutch_Maximum_learned_value_Clutch_Maximum_learned_value");
		((Control)(object)digitalReadoutInstrumentClutchMaximum).Name = "digitalReadoutInstrumentClutchMaximum";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchMaximum).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(checkBoxClutchWasReplaced, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)runServiceButtonStart, 1, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(checkBoxClutchWasReplaced, "checkBoxClutchWasReplaced");
		checkBoxClutchWasReplaced.Name = "checkBoxClutchWasReplaced";
		checkBoxClutchWasReplaced.UseVisualStyleBackColor = true;
		checkBoxClutchWasReplaced.MouseClick += checkBoxClutchWasReplaced_MouseClick;
		componentResourceManager.ApplyResources(runServiceButtonStart, "runServiceButtonStart");
		((Control)(object)runServiceButtonStart).Name = "runServiceButtonStart";
		runServiceButtonStart.ServiceCalls.Add(new ServiceCall("TCM05T", "RT_0528_Reset_Clutch_learned_values_Start", (IEnumerable<string>)new string[1] { "Reset_Clutch_learned_values=4" }));
		runServiceButtonStart.ServiceCalls.Add(new ServiceCall("TCM05T", "DL_B101_Clutch_replacement_Actual_clutch_facing_wear", (IEnumerable<string>)new string[1] { "Clutch_replacement_Actual_clutch_facing_wear=0" }));
		runServiceButtonStart.ServiceCalls.Add(new ServiceCall("TCM05T", "FN_HardReset"));
		runServiceButtonStart.Complete += runServiceButtonStart_Complete;
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentNoShifting, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleStandStill, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentEngineStandstill, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentNeutral, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentNoLearningProcedure, 2, 1);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel4, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNoShifting, "digitalReadoutInstrumentNoShifting");
		digitalReadoutInstrumentNoShifting.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentNoShifting).FreezeValue = false;
		digitalReadoutInstrumentNoShifting.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentNoShifting.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentNoShifting.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentNoShifting).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_No_gearshift_active");
		((Control)(object)digitalReadoutInstrumentNoShifting).Name = "digitalReadoutInstrumentNoShifting";
		((SingleInstrumentBase)digitalReadoutInstrumentNoShifting).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentNoShifting.RepresentedStateChanged += digitalReadoutInstrumentNoShifting_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleStandStill, "digitalReadoutInstrumentVehicleStandStill");
		digitalReadoutInstrumentVehicleStandStill.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStandStill).FreezeValue = false;
		digitalReadoutInstrumentVehicleStandStill.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentVehicleStandStill.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleStandStill.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStandStill).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Vehicle_standstill");
		((Control)(object)digitalReadoutInstrumentVehicleStandStill).Name = "digitalReadoutInstrumentVehicleStandStill";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStandStill).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleStandStill.RepresentedStateChanged += digitalReadoutInstrumentVehicleStandStill_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineStandstill, "digitalReadoutInstrumentEngineStandstill");
		digitalReadoutInstrumentEngineStandstill.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStandstill).FreezeValue = false;
		digitalReadoutInstrumentEngineStandstill.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentEngineStandstill.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentEngineStandstill.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStandstill).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Engine_standstill");
		((Control)(object)digitalReadoutInstrumentEngineStandstill).Name = "digitalReadoutInstrumentEngineStandstill";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStandstill).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineStandstill.RepresentedStateChanged += digitalReadoutInstrumentEngineStandstill_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Park_brake_activated");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrumentParkBrake_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNeutral, "digitalReadoutInstrumentNeutral");
		digitalReadoutInstrumentNeutral.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentNeutral).FreezeValue = false;
		digitalReadoutInstrumentNeutral.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentNeutral.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentNeutral.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentNeutral).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Transmission_in_neutral");
		((Control)(object)digitalReadoutInstrumentNeutral).Name = "digitalReadoutInstrumentNeutral";
		((SingleInstrumentBase)digitalReadoutInstrumentNeutral).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentNeutral.RepresentedStateChanged += digitalReadoutInstrumentNeutral_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNoLearningProcedure, "digitalReadoutInstrumentNoLearningProcedure");
		digitalReadoutInstrumentNoLearningProcedure.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentNoLearningProcedure).FreezeValue = false;
		digitalReadoutInstrumentNoLearningProcedure.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentNoLearningProcedure.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentNoLearningProcedure.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentNoLearningProcedure).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_No_learn_procedure_active");
		((Control)(object)digitalReadoutInstrumentNoLearningProcedure).Name = "digitalReadoutInstrumentNoLearningProcedure";
		((SingleInstrumentBase)digitalReadoutInstrumentNoLearningProcedure).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentNoLearningProcedure.RepresentedStateChanged += digitalReadoutInstrumentNoLearningProcedure_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentClutchFacingWearActualValue, "digitalReadoutInstrumentClutchFacingWearActualValue");
		digitalReadoutInstrumentClutchFacingWearActualValue.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingWearActualValue).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingWearActualValue).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_2651_Clutch_facing_data_Clutch_facing_wear_Actual_value");
		((Control)(object)digitalReadoutInstrumentClutchFacingWearActualValue).Name = "digitalReadoutInstrumentClutchFacingWearActualValue";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingWearActualValue).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentClutchFacingRemainingThickness, "digitalReadoutInstrumentClutchFacingRemainingThickness");
		digitalReadoutInstrumentClutchFacingRemainingThickness.FontGroup = "Main";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingRemainingThickness).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingRemainingThickness).Instrument = new Qualifier((QualifierTypes)1, "TCM05T", "DT_2651_Clutch_facing_data_Clutch_facing_Remaining_thickness");
		((Control)(object)digitalReadoutInstrumentClutchFacingRemainingThickness).Name = "digitalReadoutInstrumentClutchFacingRemainingThickness";
		((SingleInstrumentBase)digitalReadoutInstrumentClutchFacingRemainingThickness).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((ISupportInitialize)runServiceButtonStart).EndInit();
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
