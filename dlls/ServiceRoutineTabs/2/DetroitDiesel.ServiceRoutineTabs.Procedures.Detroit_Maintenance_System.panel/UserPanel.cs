using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel;

public class UserPanel : CustomPanel
{
	private bool customEngineMessage = false;

	private List<Checkmark> enginePreconditions = new List<Checkmark>();

	private bool customTransmissionMessage = false;

	private List<Checkmark> transmissionPreconditions = new List<Checkmark>();

	private bool customAxle1Message = false;

	private List<Checkmark> axle1Preconditions = new List<Checkmark>();

	private bool customAxle2Message = false;

	private List<Checkmark> axle2Preconditions = new List<Checkmark>();

	private List<Tuple<string, string>> oilSystems = new List<Tuple<string, string>>(new Tuple<string, string>[4]
	{
		new Tuple<string, string>("Engine", "MOT"),
		new Tuple<string, string>("Transmission", "GET"),
		new Tuple<string, string>("Rear_axle_1", "HA1"),
		new Tuple<string, string>("Rear_axle_2", "HA2")
	});

	private List<Tuple<string, string, string, string, int, bool>> oilSystemValues = new List<Tuple<string, string, string, string, int, bool>>();

	private bool resetInProgress = false;

	private HtmlElement systemInputPane;

	private Channel ms01t = null;

	private Channel cgw05t = null;

	private Channel activeChannel = null;

	private static string resetServiceQualifier = "DL_Reset_service_information_selected_channel";

	private static string minimumOperatingTimeQualifier = "Minimum_operating_time_reset_PAR_BzMinRs_FZG";

	private static string minimumDrivenDistanceQualifier = "Minimum_driven_distance_reset_PAR_FsMinRs_FZG";

	private TableLayoutPanel tableLayoutPanelMain;

	private TableLayoutPanel tableLayoutPanelEngine;

	private TableLayoutPanel tableLayoutPanelEngineStatus;

	private TableLayoutPanel tableLayoutPanel6;

	private Checkmark checkmarkVehicleSpeedEngine;

	private System.Windows.Forms.Label label2;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmarkIgnitionSwitchEngine;

	private System.Windows.Forms.Label labelIgnitionSwitchEngine;

	private TableLayoutPanel tableLayoutPanel4;

	private System.Windows.Forms.Label labelOperatingTimeEngine;

	private Checkmark checkmarkOperatingTimeEngine;

	private TableLayoutPanel tableLayoutPanel1;

	private Checkmark checkmarkSystemActiveEngine;

	private System.Windows.Forms.Label labelSystemActiveEngine;

	private TableLayoutPanel tableLayoutPanel3;

	private System.Windows.Forms.Label labelDrivenDistanceEngine;

	private Checkmark checkmarkDrivenDistanceEngine;

	private WebBrowser webBrowsersystemInputs;

	private TableLayoutPanel tableLayoutPanelEngineStatusMessage;

	private System.Windows.Forms.Label labelEngineStatus;

	private RunServiceButton runServiceButtonNewOilFilterEngine;

	private Checkmark checkmarkEngineStatus;

	private TableLayoutPanel tableLayoutPanelHeader;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentIgnitionSwitch;

	private DigitalReadoutInstrument digitalReadoutInstrumentMinimumDrivenDistance;

	private Panel panelEngine;

	private Panel panelTransmission;

	private TableLayoutPanel tableLayoutPanelTransmission;

	private TableLayoutPanel tableLayoutPanelTransmissionConditions;

	private TableLayoutPanel tableLayoutPanel10;

	private System.Windows.Forms.Label label4;

	private Checkmark checkmarkOperatingTimeTransmission;

	private TableLayoutPanel tableLayoutPanel11;

	private Checkmark checkmarkSystemActiveTransmission;

	private System.Windows.Forms.Label label5;

	private TableLayoutPanel tableLayoutPanel12;

	private System.Windows.Forms.Label label6;

	private Checkmark checkmarkIgnitionSwitchTransmission;

	private TableLayoutPanel tableLayoutPanel13;

	private Checkmark checkmarkVehicleSpeedTransmission;

	private System.Windows.Forms.Label label7;

	private TableLayoutPanel tableLayoutPanel14;

	private System.Windows.Forms.Label label8;

	private Checkmark checkmarkDrivenDistanceTransmission;

	private TableLayoutPanel tableLayoutPanelTransmissionStatus;

	private System.Windows.Forms.Label labelTransmissionStatus;

	private Checkmark checkmarkTransmissionStatus;

	private RunServiceButton runServiceButtonNewOilFilterTransmission;

	private System.Windows.Forms.Label label10;

	private Panel panelAxle1;

	private TableLayoutPanel tableLayoutPanelAxle1;

	private TableLayoutPanel tableLayoutPanelAxle1Conditions;

	private TableLayoutPanel tableLayoutPanel19;

	private System.Windows.Forms.Label label1;

	private Checkmark checkmarkOperatingTimeAxle1;

	private TableLayoutPanel tableLayoutPanel20;

	private Checkmark checkmarkSystemActiveAxle1;

	private System.Windows.Forms.Label label9;

	private TableLayoutPanel tableLayoutPanel21;

	private System.Windows.Forms.Label label11;

	private Checkmark checkmarkIgnitionSwitchAxle1;

	private TableLayoutPanel tableLayoutPanel22;

	private Checkmark checkmarkVehicleSpeedAxle1;

	private System.Windows.Forms.Label label12;

	private TableLayoutPanel tableLayoutPanel23;

	private System.Windows.Forms.Label label13;

	private Checkmark checkmarkDrivenDistanceAxle1;

	private TableLayoutPanel tableLayoutPanelAxle1Status;

	private System.Windows.Forms.Label labelAxle1Status;

	private Checkmark checkmarkAxle1Status;

	private RunServiceButton runServiceButtonNewOilFilterAxle1;

	private System.Windows.Forms.Label label15;

	private Panel panelAxle2;

	private TableLayoutPanel tableLayoutPanelAxle2;

	private TableLayoutPanel Axle2Conditions;

	private TableLayoutPanel tableLayoutPanel28;

	private System.Windows.Forms.Label label14;

	private Checkmark checkmarkOperatingTimeAxle2;

	private TableLayoutPanel tableLayoutPanel29;

	private Checkmark checkmarkSystemActiveAxle2;

	private System.Windows.Forms.Label label16;

	private TableLayoutPanel tableLayoutPanel30;

	private System.Windows.Forms.Label label17;

	private Checkmark checkmarkIgnitionSwitchAxle2;

	private TableLayoutPanel tableLayoutPanel31;

	private Checkmark checkmarkVehicleSpeedAxle2;

	private System.Windows.Forms.Label label18;

	private TableLayoutPanel tableLayoutPanel32;

	private System.Windows.Forms.Label label19;

	private Checkmark checkmarkDrivenDistanceAxle2;

	private TableLayoutPanel tableLayoutPanelAxle2Status;

	private System.Windows.Forms.Label labelAxle2Status;

	private Checkmark checkmarkAxle2Status;

	private RunServiceButton runServiceButtonNewOilFilterAxle2;

	private System.Windows.Forms.Label label21;

	private TableLayoutPanel tableLayoutPanel5;

	private Label label20;

	private Label label22;

	private ScalingLabel scalingLabelMinimumTime;

	private System.Windows.Forms.Label label3;

	public override string StyleSheet
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<style type=\"text/css\">td {padding-right: 10px}");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "h1 {{ font-family:{0}; font-size:{1}pt }}", ((Control)(object)this).Font.FontFamily.Name, (((Control)(object)this).Font.SizeInPoints * 2f).ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "p {{ font-family:{0}; font-size:{1}pt }}", ((Control)(object)this).Font.FontFamily.Name, ((Control)(object)this).Font.SizeInPoints.ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "table {{ font-family:{0}; font-size:{1}pt; border: 1px solid black; border-collapse: collapse; width:100%}}", ((Control)(object)this).Font.FontFamily.Name, ((Control)(object)this).Font.SizeInPoints.ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendLine("th { border-width: 1px; padding: 4px; border: 1px solid black; background: #f4f4f4 }");
			stringBuilder.AppendLine("td { border-width: 1px; padding: 4px; border: 1px solid black; }");
			stringBuilder.AppendLine("tr:nth-child(odd) { background: #f1fdf1 }");
			stringBuilder.AppendLine("tr:nth-child(even) { background: #f1f1fd }");
			stringBuilder.AppendLine("tr.disabled { color: #aaaaaa }");
			stringBuilder.Append("</style>");
			return stringBuilder.ToString();
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_{1}", Resources.Message_DrivenDistance, "km", 1000, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_{1}", Resources.Message_OperatingTime, "days", 86400, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "Channel_activation_PAR_KanAktiv_{1}", Resources.Message_SystemActive, "", 1, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Prediction_data_EEPROM_Remaining_driving_distance_PRD_RFS_{1}", Resources.Message_RemainingDrivingDistance, "km", 1000, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Prediction_data_EEPROM_Remaining_operating_time_PRD_RBZ_{1}", Resources.Message_RemainingOperatingTime, "days", 86400, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Service_Life_Dates_EEPROM_Life_cycle_consumption_relative_LLD_LDVRel_{1}", Resources.Message_LifeCycleConsumption, "%", 1, item6: false));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Engine", "DT_STO_{0}_Service_Life_Dates_EEPROM_Life_cycle_consumption_relative_due_to_load_accumulative_LLD_LDVRelBelAkk_{1}", Resources.Message_LoadLifeCycleConsumption, "%", 1, item6: false));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Date", "DT_STO_{0}_Prediction_data_EEPROM_Maintenance_date_year_PRD_WartJahr_{1}", Resources.Message_MaintenanceDateYear, "", 1, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Date", "DT_STO_{0}_Prediction_data_EEPROM_Maintenance_date_month_PRD_WartMonat_{1}", Resources.Message_MaintenanceDateMonth, "", 1, item6: true));
		oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Date", "DT_STO_{0}_Prediction_data_EEPROM_Maintenance_date_day_PRD_WartTag_{1}", Resources.Message_MaintenanceDateDay, "", 1, item6: true));
		enginePreconditions.AddRange((IEnumerable<Checkmark>)(object)new Checkmark[5] { checkmarkIgnitionSwitchEngine, checkmarkVehicleSpeedEngine, checkmarkSystemActiveEngine, checkmarkDrivenDistanceEngine, checkmarkOperatingTimeEngine });
		transmissionPreconditions.AddRange((IEnumerable<Checkmark>)(object)new Checkmark[5] { checkmarkIgnitionSwitchTransmission, checkmarkVehicleSpeedTransmission, checkmarkSystemActiveTransmission, checkmarkDrivenDistanceTransmission, checkmarkOperatingTimeTransmission });
		axle1Preconditions.AddRange((IEnumerable<Checkmark>)(object)new Checkmark[5] { checkmarkIgnitionSwitchAxle1, checkmarkVehicleSpeedAxle1, checkmarkSystemActiveAxle1, checkmarkDrivenDistanceAxle1, checkmarkOperatingTimeAxle1 });
		axle2Preconditions.AddRange((IEnumerable<Checkmark>)(object)new Checkmark[5] { checkmarkIgnitionSwitchAxle2, checkmarkVehicleSpeedAxle2, checkmarkSystemActiveAxle2, checkmarkDrivenDistanceAxle2, checkmarkOperatingTimeAxle2 });
		webBrowsersystemInputs.DocumentText = "<HTML><HEAD><meta http-equiv='X-UA-Compatible' content='IE=edge'>" + ((CustomPanel)this).StyleSheet + "\n</HEAD><BODY><DIV id=\"systemInputPane\" name=\"systemInputPane\" class=\"standard\"><br></DIV></BODY></HTML>";
		ReadParameters();
		((CustomPanel)this).OnChannelsChanged();
		UpdateWebBrowser();
		UpdateUI();
	}

	private void UpdateUI()
	{
		Parameter parameter = null;
		if (activeChannel != null && (parameter = activeChannel.Parameters[minimumOperatingTimeQualifier]) != null && parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()))
		{
			((Control)(object)scalingLabelMinimumTime).Text = ConvertAndScaleValue(parameter.Value.ToString(), "", 1440);
		}
		else
		{
			((Control)(object)scalingLabelMinimumTime).Text = Resources.Message_Unavaiable;
		}
		UpdateEngineConditions();
		UpdateTransmissionConditions();
		UpdateAxle1Conditions();
		UpdateAxle2Conditions();
	}

	private void UpdateEngineConditions()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		checkmarkIgnitionSwitchEngine.CheckState = (((int)digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		checkmarkVehicleSpeedEngine.CheckState = (((int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		((Control)(object)runServiceButtonNewOilFilterEngine).Enabled = false;
		if (activeChannel != null)
		{
			if (activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
			{
				Checkmark obj = checkmarkEngineStatus;
				Checkmark obj2 = checkmarkDrivenDistanceEngine;
				Checkmark obj3 = checkmarkSystemActiveEngine;
				CheckState checkState = (checkmarkOperatingTimeEngine.CheckState = CheckState.Unchecked);
				checkState = (obj3.CheckState = checkState);
				checkState = (obj2.CheckState = checkState);
				obj.CheckState = checkState;
				((Control)(object)runServiceButtonNewOilFilterEngine).Enabled = false;
				labelEngineStatus.Text = Resources.Message_ReadingECUInformation;
				customEngineMessage = false;
				return;
			}
			Checkmark obj4 = checkmarkSystemActiveEngine;
			uint? num = RawValue("Channel_activation_PAR_KanAktiv_MOT");
			obj4.CheckState = ((num == 1 && num.HasValue && RawValue("System_activation_PAR_SysAktiv_FZG") == 1) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkDrivenDistanceEngine.CheckState = ((RawValue(MassageQualifier("DT_STO_Engine_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_MOT")) >= RawValue(minimumDrivenDistanceQualifier) * 1000) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkOperatingTimeEngine.CheckState = ((RawValue(MassageQualifier("DT_STO_Engine_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_MOT")) >= RawValue(minimumOperatingTimeQualifier) * 60) ? CheckState.Checked : CheckState.Unchecked);
			string text = Resources.Message_ReadyToReset;
			CheckState checkState5 = CheckState.Checked;
			foreach (Checkmark enginePrecondition in enginePreconditions)
			{
				if ((checkmarkEngineStatus.CheckState = enginePrecondition.CheckState) == CheckState.Unchecked)
				{
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetEngineSystem01, Environment.NewLine, (string)((Control)(object)enginePrecondition).Tag);
					checkState5 = CheckState.Unchecked;
					customEngineMessage = false;
					break;
				}
			}
			checkmarkEngineStatus.CheckState = checkState5;
			labelEngineStatus.Text = ((!customEngineMessage) ? text : labelEngineStatus.Text);
			((Control)(object)runServiceButtonNewOilFilterEngine).Enabled = checkmarkEngineStatus.CheckState == CheckState.Checked && activeChannel.Online && !resetInProgress;
		}
		else
		{
			Checkmark obj5 = checkmarkEngineStatus;
			Checkmark obj6 = checkmarkDrivenDistanceEngine;
			Checkmark obj7 = checkmarkSystemActiveEngine;
			CheckState checkState = (checkmarkOperatingTimeEngine.CheckState = CheckState.Unchecked);
			checkState = (obj7.CheckState = checkState);
			checkState = (obj6.CheckState = checkState);
			obj5.CheckState = checkState;
			((Control)(object)runServiceButtonNewOilFilterEngine).Enabled = false;
			labelEngineStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetEngineSystem0MS01TOffline, Environment.NewLine);
			customEngineMessage = false;
		}
	}

	private void UpdateTransmissionConditions()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		checkmarkIgnitionSwitchTransmission.CheckState = (((int)digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		checkmarkVehicleSpeedTransmission.CheckState = (((int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		((Control)(object)runServiceButtonNewOilFilterTransmission).Enabled = false;
		if (activeChannel != null)
		{
			if (activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
			{
				Checkmark obj = checkmarkTransmissionStatus;
				Checkmark obj2 = checkmarkDrivenDistanceTransmission;
				Checkmark obj3 = checkmarkSystemActiveTransmission;
				CheckState checkState = (checkmarkOperatingTimeTransmission.CheckState = CheckState.Unchecked);
				checkState = (obj3.CheckState = checkState);
				checkState = (obj2.CheckState = checkState);
				obj.CheckState = checkState;
				((Control)(object)runServiceButtonNewOilFilterTransmission).Enabled = false;
				labelTransmissionStatus.Text = Resources.Message_ReadingECUInformation;
				customTransmissionMessage = false;
				return;
			}
			Checkmark obj4 = checkmarkSystemActiveTransmission;
			uint? num = RawValue("Channel_activation_PAR_KanAktiv_GET");
			obj4.CheckState = ((num == 1 && num.HasValue && RawValue("System_activation_PAR_SysAktiv_FZG") == 1) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkDrivenDistanceTransmission.CheckState = ((RawValue(MassageQualifier("DT_STO_Transmission_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_GET")) >= RawValue(minimumDrivenDistanceQualifier) * 1000) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkOperatingTimeTransmission.CheckState = ((RawValue(MassageQualifier("DT_STO_Transmission_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_GET")) >= RawValue(minimumOperatingTimeQualifier) * 60) ? CheckState.Checked : CheckState.Unchecked);
			string text = Resources.Message_ReadyToReset;
			CheckState checkState5 = CheckState.Checked;
			foreach (Checkmark transmissionPrecondition in transmissionPreconditions)
			{
				if ((checkmarkTransmissionStatus.CheckState = transmissionPrecondition.CheckState) == CheckState.Unchecked)
				{
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetTransmissionSystem01, Environment.NewLine, (string)((Control)(object)transmissionPrecondition).Tag);
					checkState5 = CheckState.Unchecked;
					customTransmissionMessage = false;
					break;
				}
			}
			checkmarkTransmissionStatus.CheckState = checkState5;
			labelTransmissionStatus.Text = ((!customTransmissionMessage) ? text : labelTransmissionStatus.Text);
			((Control)(object)runServiceButtonNewOilFilterTransmission).Enabled = checkmarkTransmissionStatus.CheckState == CheckState.Checked && activeChannel.Online && !resetInProgress;
		}
		else
		{
			Checkmark obj5 = checkmarkTransmissionStatus;
			Checkmark obj6 = checkmarkDrivenDistanceTransmission;
			Checkmark obj7 = checkmarkSystemActiveTransmission;
			CheckState checkState = (checkmarkOperatingTimeTransmission.CheckState = CheckState.Unchecked);
			checkState = (obj7.CheckState = checkState);
			checkState = (obj6.CheckState = checkState);
			obj5.CheckState = checkState;
			((Control)(object)runServiceButtonNewOilFilterTransmission).Enabled = false;
			labelTransmissionStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetTransmissionSystem0MS01TOffline, Environment.NewLine);
			customTransmissionMessage = false;
		}
	}

	private void UpdateAxle1Conditions()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		checkmarkIgnitionSwitchAxle1.CheckState = (((int)digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		checkmarkVehicleSpeedAxle1.CheckState = (((int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		((Control)(object)runServiceButtonNewOilFilterAxle1).Enabled = false;
		if (activeChannel != null)
		{
			if (activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
			{
				Checkmark obj = checkmarkAxle1Status;
				Checkmark obj2 = checkmarkDrivenDistanceAxle1;
				Checkmark obj3 = checkmarkSystemActiveAxle1;
				CheckState checkState = (checkmarkOperatingTimeAxle1.CheckState = CheckState.Unchecked);
				checkState = (obj3.CheckState = checkState);
				checkState = (obj2.CheckState = checkState);
				obj.CheckState = checkState;
				((Control)(object)runServiceButtonNewOilFilterAxle1).Enabled = false;
				labelAxle1Status.Text = Resources.Message_ReadingECUInformation;
				customAxle1Message = false;
				return;
			}
			Checkmark obj4 = checkmarkSystemActiveAxle1;
			uint? num = RawValue("Channel_activation_PAR_KanAktiv_HA1");
			obj4.CheckState = ((num == 1 && num.HasValue && RawValue("System_activation_PAR_SysAktiv_FZG") == 1) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkDrivenDistanceAxle1.CheckState = ((RawValue(MassageQualifier("DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA1")) >= RawValue(minimumDrivenDistanceQualifier) * 1000) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkOperatingTimeAxle1.CheckState = ((RawValue(MassageQualifier("DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA1")) >= RawValue(minimumOperatingTimeQualifier) * 60) ? CheckState.Checked : CheckState.Unchecked);
			string text = Resources.Message_ReadyToReset;
			CheckState checkState5 = CheckState.Checked;
			foreach (Checkmark axle1Precondition in axle1Preconditions)
			{
				if ((checkmarkAxle1Status.CheckState = axle1Precondition.CheckState) == CheckState.Unchecked)
				{
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle1System01, Environment.NewLine, (string)((Control)(object)axle1Precondition).Tag);
					checkState5 = CheckState.Unchecked;
					customAxle1Message = false;
					break;
				}
			}
			checkmarkAxle1Status.CheckState = checkState5;
			labelAxle1Status.Text = ((!customAxle1Message) ? text : labelAxle1Status.Text);
			((Control)(object)runServiceButtonNewOilFilterAxle1).Enabled = checkmarkAxle1Status.CheckState == CheckState.Checked && activeChannel.Online && !resetInProgress;
		}
		else
		{
			Checkmark obj5 = checkmarkAxle1Status;
			Checkmark obj6 = checkmarkDrivenDistanceAxle1;
			Checkmark obj7 = checkmarkSystemActiveAxle1;
			CheckState checkState = (checkmarkOperatingTimeAxle1.CheckState = CheckState.Unchecked);
			checkState = (obj7.CheckState = checkState);
			checkState = (obj6.CheckState = checkState);
			obj5.CheckState = checkState;
			((Control)(object)runServiceButtonNewOilFilterAxle1).Enabled = false;
			labelAxle1Status.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle1System0MS01TOffline, Environment.NewLine);
			customAxle1Message = false;
		}
	}

	private void UpdateAxle2Conditions()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		checkmarkIgnitionSwitchAxle2.CheckState = (((int)digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		checkmarkVehicleSpeedAxle2.CheckState = (((int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1) ? CheckState.Checked : CheckState.Unchecked);
		((Control)(object)runServiceButtonNewOilFilterAxle2).Enabled = false;
		if (activeChannel != null)
		{
			if (activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
			{
				Checkmark obj = checkmarkAxle2Status;
				Checkmark obj2 = checkmarkDrivenDistanceAxle2;
				Checkmark obj3 = checkmarkSystemActiveAxle2;
				CheckState checkState = (checkmarkOperatingTimeAxle2.CheckState = CheckState.Unchecked);
				checkState = (obj3.CheckState = checkState);
				checkState = (obj2.CheckState = checkState);
				obj.CheckState = checkState;
				((Control)(object)runServiceButtonNewOilFilterAxle2).Enabled = false;
				labelAxle2Status.Text = Resources.Message_ReadingECUInformation;
				customAxle2Message = false;
				return;
			}
			Checkmark obj4 = checkmarkSystemActiveAxle2;
			uint? num = RawValue("Channel_activation_PAR_KanAktiv_HA2");
			obj4.CheckState = ((num == 1 && num.HasValue && RawValue("System_activation_PAR_SysAktiv_FZG") == 1) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkDrivenDistanceAxle2.CheckState = ((RawValue(MassageQualifier("DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA2")) >= RawValue(minimumDrivenDistanceQualifier) * 1000) ? CheckState.Checked : CheckState.Unchecked);
			checkmarkOperatingTimeAxle2.CheckState = ((RawValue(MassageQualifier("DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA2")) >= RawValue(minimumOperatingTimeQualifier) * 60) ? CheckState.Checked : CheckState.Unchecked);
			string text = Resources.Message_ReadyToReset;
			CheckState checkState5 = CheckState.Checked;
			foreach (Checkmark axle2Precondition in axle2Preconditions)
			{
				if ((checkmarkAxle2Status.CheckState = axle2Precondition.CheckState) == CheckState.Unchecked)
				{
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle2System01, Environment.NewLine, (string)((Control)(object)axle2Precondition).Tag);
					checkState5 = CheckState.Unchecked;
					customAxle2Message = false;
					break;
				}
			}
			checkmarkAxle2Status.CheckState = checkState5;
			labelAxle2Status.Text = ((!customAxle2Message) ? text : labelAxle2Status.Text);
			((Control)(object)runServiceButtonNewOilFilterAxle2).Enabled = checkmarkAxle2Status.CheckState == CheckState.Checked && activeChannel.Online && !resetInProgress;
		}
		else
		{
			Checkmark obj5 = checkmarkAxle2Status;
			Checkmark obj6 = checkmarkDrivenDistanceAxle2;
			Checkmark obj7 = checkmarkSystemActiveAxle2;
			CheckState checkState = (checkmarkOperatingTimeAxle2.CheckState = CheckState.Unchecked);
			checkState = (obj7.CheckState = checkState);
			checkState = (obj6.CheckState = checkState);
			obj5.CheckState = checkState;
			((Control)(object)runServiceButtonNewOilFilterAxle2).Enabled = false;
			labelAxle2Status.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle2System0MS01TOffline, Environment.NewLine);
			customAxle2Message = false;
		}
	}

	private string PrintValue(string estimatedReplacementDate, string qualifier, string units, int scalingFactor)
	{
		if (activeChannel != null)
		{
			if (estimatedReplacementDate == Resources.Message_NotEnoughOperatingTime && qualifier.Contains("Prediction"))
			{
				return Resources.Message_NotEnoughOperatingTime;
			}
			EcuInfo ecuInfo = null;
			if ((ecuInfo = activeChannel.EcuInfos[qualifier]) != null && !string.IsNullOrEmpty(ecuInfo.Value))
			{
				return string.IsNullOrEmpty(units) ? ecuInfo.Value.ToString() : ConvertAndScaleValue(ecuInfo.Value.ToString(), units, scalingFactor);
			}
			Parameter parameter = null;
			if ((parameter = activeChannel.Parameters[qualifier]) != null && parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()))
			{
				return string.IsNullOrEmpty(units) ? parameter.Value.ToString() : ConvertAndScaleValue(parameter.Value.ToString(), units, scalingFactor);
			}
		}
		return Resources.Message_Unavaiable;
	}

	private static string ConvertAndScaleValue(string value, string units, int scalingFactor)
	{
		double result = 0.0;
		if (double.TryParse(value, out result))
		{
			string arg = units;
			result /= (double)scalingFactor;
			if (units == "km")
			{
				Conversion conversion = Converter.GlobalInstance.GetConversion("km");
				arg = conversion.OutputUnit;
				result = conversion.Convert(result);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0:#.00} {1} ", result, arg);
		}
		return Resources.Message_Unavaiable;
	}

	private uint? RawValue(string qualifier)
	{
		if (activeChannel != null)
		{
			EcuInfo ecuInfo = null;
			uint result;
			if ((ecuInfo = activeChannel.EcuInfos[qualifier]) != null && !string.IsNullOrEmpty(ecuInfo.Value))
			{
				return uint.TryParse(ecuInfo.Value.ToString(), out result) ? new uint?(result) : ((uint?)null);
			}
			Parameter parameter = null;
			if ((parameter = activeChannel.Parameters[qualifier]) != null && parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()))
			{
				Choice choice = parameter.Value as Choice;
				if (choice != null)
				{
					return uint.TryParse(choice.RawValue.ToString(), out result) ? new uint?(result) : ((uint?)null);
				}
				return uint.TryParse(parameter.Value.ToString(), out result) ? new uint?(result) : ((uint?)null);
			}
		}
		return null;
	}

	public override void OnChannelsChanged()
	{
		SetChannelMS01T(((CustomPanel)this).GetChannel("MS01T", (ChannelLookupOptions)3));
		SetChannelCGW05T(((CustomPanel)this).GetChannel("CGW05T", (ChannelLookupOptions)3));
		UpdateUI();
	}

	private void SetChannelMS01T(Channel channel)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if ((channel != null && ms01t == null) || (channel == null && ms01t != null))
		{
			if (ms01t != null)
			{
				ms01t.Parameters.ParametersWriteCompleteEvent -= ParametersWriteCompleteEvent;
				ms01t.Parameters.ParametersReadCompleteEvent -= ParametersReadCompleteEvent;
				ms01t.EcuInfos.EcuInfosReadCompleteEvent -= EcuInfosReadComplete;
			}
			ms01t = channel;
			activeChannel = channel;
			if (ms01t != null)
			{
				((SingleInstrumentBase)digitalReadoutInstrumentMinimumDrivenDistance).Instrument = new Qualifier((QualifierTypes)4, "MS01T", minimumDrivenDistanceQualifier);
				runServiceButtonNewOilFilterEngine.ServiceCall = CreateServiceCall("MSF01T", 1u);
				runServiceButtonNewOilFilterTransmission.ServiceCall = CreateServiceCall("MSF01T", 2u);
				runServiceButtonNewOilFilterAxle1.ServiceCall = CreateServiceCall("MSF01T", 6u);
				runServiceButtonNewOilFilterAxle2.ServiceCall = CreateServiceCall("MSF01T", 20u);
				ms01t.Parameters.ParametersWriteCompleteEvent += ParametersWriteCompleteEvent;
				ms01t.Parameters.ParametersReadCompleteEvent += ParametersReadCompleteEvent;
				ms01t.EcuInfos.EcuInfosReadCompleteEvent += EcuInfosReadComplete;
				ReadParameters();
			}
			resetInProgress = (customEngineMessage = (customTransmissionMessage = (customAxle1Message = (customAxle2Message = false))));
			UpdateWebBrowser();
		}
	}

	private void SetChannelCGW05T(Channel channel)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if ((channel != null && cgw05t == null) || (channel == null && cgw05t != null))
		{
			if (cgw05t != null)
			{
				cgw05t.Parameters.ParametersWriteCompleteEvent -= ParametersWriteCompleteEvent;
				cgw05t.Parameters.ParametersReadCompleteEvent -= ParametersReadCompleteEvent;
				cgw05t.EcuInfos.EcuInfosReadCompleteEvent -= EcuInfosReadComplete;
			}
			cgw05t = channel;
			activeChannel = channel;
			if (cgw05t != null)
			{
				((SingleInstrumentBase)digitalReadoutInstrumentMinimumDrivenDistance).Instrument = new Qualifier((QualifierTypes)4, "CGW05T", minimumDrivenDistanceQualifier);
				runServiceButtonNewOilFilterEngine.ServiceCall = CreateServiceCall("CGW05T", 1u);
				runServiceButtonNewOilFilterTransmission.ServiceCall = CreateServiceCall("CGW05T", 2u);
				runServiceButtonNewOilFilterAxle1.ServiceCall = CreateServiceCall("CGW05T", 6u);
				runServiceButtonNewOilFilterAxle2.ServiceCall = CreateServiceCall("CGW05T", 20u);
				cgw05t.Parameters.ParametersWriteCompleteEvent += ParametersWriteCompleteEvent;
				cgw05t.Parameters.ParametersReadCompleteEvent += ParametersReadCompleteEvent;
				cgw05t.EcuInfos.EcuInfosReadCompleteEvent += EcuInfosReadComplete;
				ReadParameters();
			}
			resetInProgress = (customEngineMessage = (customTransmissionMessage = (customAxle1Message = (customAxle2Message = false))));
			UpdateWebBrowser();
		}
	}

	private ServiceCall CreateServiceCall(string ecuName, uint channelNumber)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		return new ServiceCall(ecuName, MassageQualifier(resetServiceQualifier), (IEnumerable<string>)new string[2]
		{
			$"Channel_number={channelNumber}",
			"Filter_condition_Diesel_particle_filter_only=0"
		});
	}

	private string MassageQualifier(string qualifier)
	{
		if (activeChannel == cgw05t && !qualifier.Contains("_MS_"))
		{
			return qualifier.Replace("DT_STO_", "DT_STO_MS_").Replace("DL_", "DL_MS_");
		}
		return qualifier;
	}

	private void RefreshEcuInfo(string systemName, string systemAcronym)
	{
		if (activeChannel == null || activeChannel.CommunicationsState != CommunicationsState.Online)
		{
			return;
		}
		foreach (Tuple<string, string, string, string, int, bool> item in oilSystemValues.Where((Tuple<string, string, string, string, int, bool> v) => (v.Item2 == systemName || v.Item2 == "All" || v.Item2 == "Date") && v.Item3.Contains("EEPROM")))
		{
			activeChannel.EcuInfos[MassageQualifier(string.Format(CultureInfo.InvariantCulture, item.Item2, systemName, systemAcronym))].Read(synchronous: false);
		}
	}

	private void ReadParameters()
	{
		if (activeChannel == null || activeChannel.CommunicationsState != CommunicationsState.Online)
		{
			return;
		}
		ReadParameter(minimumDrivenDistanceQualifier);
		ReadParameter(minimumOperatingTimeQualifier);
		foreach (Tuple<string, string> oilSystem in oilSystems)
		{
			foreach (Tuple<string, string, string, string, int, bool> item in oilSystemValues.Where((Tuple<string, string, string, string, int, bool> ev) => ev.Item2.Contains("_PAR_")))
			{
				ReadParameter(MassageQualifier(string.Format(CultureInfo.InvariantCulture, item.Item2, oilSystem.Item1, oilSystem.Item2)));
			}
		}
	}

	private void ReadParameter(string qualifier)
	{
		if (activeChannel != null && activeChannel.CommunicationsState == CommunicationsState.Online && activeChannel.Parameters[qualifier] != null && !activeChannel.Parameters[qualifier].HasBeenReadFromEcu)
		{
			activeChannel.Parameters.ReadGroup(activeChannel.Parameters[qualifier].GroupQualifier, fromCache: true, synchronous: false);
		}
	}

	private void EcuInfosReadComplete(object sender, ResultEventArgs result)
	{
		UpdateWebBrowser();
		UpdateUI();
		ReadParameters();
		resetInProgress = (customEngineMessage = (customTransmissionMessage = (customAxle1Message = (customAxle2Message = false))));
	}

	private void ParametersReadCompleteEvent(object sender, ResultEventArgs result)
	{
		UpdateWebBrowser();
		UpdateUI();
		ReadParameters();
		resetInProgress = (customEngineMessage = (customTransmissionMessage = (customAxle1Message = (customAxle2Message = false))));
	}

	private void ParametersWriteCompleteEvent(object sender, ResultEventArgs result)
	{
		UpdateWebBrowser();
		UpdateUI();
		resetInProgress = (customEngineMessage = (customTransmissionMessage = (customAxle1Message = (customAxle2Message = false))));
	}

	private void webBrowserEngineValues_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		if (webBrowsersystemInputs.Document.All.Count > 0)
		{
			HtmlElementCollection elementsByName = webBrowsersystemInputs.Document.All.GetElementsByName("systemInputPane");
			if (elementsByName != null && elementsByName.Count > 0)
			{
				systemInputPane = elementsByName[0];
				UpdateWebBrowser();
			}
		}
		((CustomPanel)this).OnChannelsChanged();
	}

	private void UpdateWebBrowser()
	{
		if (systemInputPane != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
			{
				UpdateContent(writer);
			}
			systemInputPane.InnerHtml = stringBuilder.ToString();
			webBrowsersystemInputs.Size = webBrowsersystemInputs.Document.Body.ScrollRectangle.Size;
		}
	}

	private void UpdateContent(XmlWriter writer)
	{
		if (activeChannel != null)
		{
			writer.WriteStartElement("table");
			foreach (Tuple<string, string> oilSystem in oilSystems)
			{
				writer.WriteStartElement("tr");
				writer.WriteStartElement("th");
				writer.WriteStartAttribute("colspan");
				writer.WriteString("2");
				writer.WriteEndAttribute();
				writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0OilSystemValues, oilSystem.Item1.Replace("_", " ")));
				writer.WriteEndElement();
				writer.WriteEndElement();
				WriteRow(writer, Resources.Message_Description, Resources.Message_Value);
				string text = EstimatedReplacementDate(oilSystem.Item1, oilSystem.Item2);
				WriteRow(writer, Resources.Message_EstimatedReplacementDate, text);
				List<Tuple<string, string, string, string, int, bool>> source = oilSystemValues;
				Func<Tuple<string, string, string, string, int, bool>, bool> predicate = (Tuple<string, string, string, string, int, bool> ev) => ev.Item1 == oilSystem.Item1 || ev.Item1 == "All";
				foreach (Tuple<string, string, string, string, int, bool> item in source.Where(predicate))
				{
					if (item.Item6 || ApplicationInformation.Branding.IsProductName("Engineering"))
					{
						WriteRow(writer, item.Item3, PrintValue(text, MassageQualifier(string.Format(CultureInfo.InvariantCulture, item.Item2, oilSystem.Item1, oilSystem.Item2)), item.Item4, item.Item5));
					}
				}
			}
			writer.WriteEndElement();
		}
		else
		{
			writer.WriteString(Resources.Message_MS01TOffline);
		}
	}

	private static void WriteRow(XmlWriter writer, params string[] columns)
	{
		writer.WriteStartElement("tr");
		foreach (string text in columns)
		{
			writer.WriteStartElement("td");
			writer.WriteString(text);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}

	private string EstimatedReplacementDate(string system, string systemAcronym)
	{
		string text = string.Empty;
		int num = 0;
		foreach (Tuple<string, string, string, string, int, bool> item in oilSystemValues.Where((Tuple<string, string, string, string, int, bool> ev) => ev.Item1 == "Date"))
		{
			string qualifier = MassageQualifier(string.Format(CultureInfo.InvariantCulture, item.Item2, system, systemAcronym));
			EcuInfo ecuInfo = null;
			if (activeChannel != null && (ecuInfo = activeChannel.EcuInfos[qualifier]) != null && ecuInfo.Value != null)
			{
				text += string.Format(CultureInfo.InvariantCulture, "{0}-", ecuInfo.Value);
				num++;
			}
		}
		string text2 = ((num == 3) ? text.TrimEnd('-') : Resources.Message_Unavaiable);
		return (text2 == "2255-255-255") ? Resources.Message_NotEnoughOperatingTime : text2;
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterEngine_Started(object sender, PassFailResultEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Result == 1)
		{
			labelEngineStatus.Text = Resources.Message_ResetComplete;
			RefreshEcuInfo(oilSystems[0].Item1, oilSystems[0].Item2);
		}
		else
		{
			labelEngineStatus.Text = ((ResultEventArgs)(object)e).Exception.Message;
		}
		customEngineMessage = true;
		resetInProgress = false;
		UpdateWebBrowser();
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterEngine_Starting(object sender, CancelEventArgs e)
	{
		resetInProgress = true;
		customEngineMessage = true;
		labelEngineStatus.Text = Resources.Message_Starting;
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterTransmission_Started(object sender, PassFailResultEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Result == 1)
		{
			labelTransmissionStatus.Text = Resources.Message_ResetComplete;
			RefreshEcuInfo(oilSystems[1].Item1, oilSystems[1].Item2);
		}
		else
		{
			labelTransmissionStatus.Text = ((ResultEventArgs)(object)e).Exception.Message;
		}
		customTransmissionMessage = true;
		resetInProgress = false;
		UpdateWebBrowser();
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterTransmission_Starting(object sender, CancelEventArgs e)
	{
		resetInProgress = true;
		customTransmissionMessage = true;
		labelTransmissionStatus.Text = Resources.Message_Starting;
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterAxle1_Started(object sender, PassFailResultEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Result == 1)
		{
			labelAxle1Status.Text = Resources.Message_ResetComplete;
			RefreshEcuInfo(oilSystems[2].Item1, oilSystems[2].Item2);
		}
		else
		{
			labelAxle1Status.Text = ((ResultEventArgs)(object)e).Exception.Message;
		}
		customAxle1Message = true;
		resetInProgress = false;
		UpdateWebBrowser();
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterAxle1_Starting(object sender, CancelEventArgs e)
	{
		resetInProgress = true;
		customAxle1Message = true;
		labelAxle1Status.Text = Resources.Message_Starting;
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterAxle2_Started(object sender, PassFailResultEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Result == 1)
		{
			labelAxle2Status.Text = Resources.Message_ResetComplete;
			RefreshEcuInfo(oilSystems[3].Item1, oilSystems[3].Item2);
		}
		else
		{
			labelAxle2Status.Text = ((ResultEventArgs)(object)e).Exception.Message;
		}
		customAxle2Message = true;
		resetInProgress = false;
		UpdateWebBrowser();
		UpdateUI();
	}

	private void runServiceButtonReplaceOilFilterAxle2_Starting(object sender, CancelEventArgs e)
	{
		resetInProgress = true;
		customAxle2Message = true;
		labelAxle2Status.Text = Resources.Message_Starting;
		UpdateUI();
	}

	private void UserPanel_SizeChanged(object sender, EventArgs e)
	{
		if (webBrowsersystemInputs != null && webBrowsersystemInputs.Document != null && webBrowsersystemInputs.Document.Body != null)
		{
			webBrowsersystemInputs.Size = webBrowsersystemInputs.Document.Body.ScrollRectangle.Size;
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Expected O, but got Unknown
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Expected O, but got Unknown
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Expected O, but got Unknown
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Expected O, but got Unknown
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Expected O, but got Unknown
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Expected O, but got Unknown
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Expected O, but got Unknown
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Expected O, but got Unknown
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Expected O, but got Unknown
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Expected O, but got Unknown
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Expected O, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected O, but got Unknown
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Expected O, but got Unknown
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Expected O, but got Unknown
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Expected O, but got Unknown
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Expected O, but got Unknown
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Expected O, but got Unknown
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Expected O, but got Unknown
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Expected O, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Expected O, but got Unknown
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Expected O, but got Unknown
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Expected O, but got Unknown
		//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_114f: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_265a: Unknown result type (might be due to invalid IL or missing references)
		//IL_26e9: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		panelTransmission = new Panel();
		tableLayoutPanelTransmission = new TableLayoutPanel();
		tableLayoutPanelTransmissionConditions = new TableLayoutPanel();
		tableLayoutPanel10 = new TableLayoutPanel();
		label4 = new System.Windows.Forms.Label();
		checkmarkOperatingTimeTransmission = new Checkmark();
		tableLayoutPanel11 = new TableLayoutPanel();
		checkmarkSystemActiveTransmission = new Checkmark();
		label5 = new System.Windows.Forms.Label();
		tableLayoutPanel12 = new TableLayoutPanel();
		label6 = new System.Windows.Forms.Label();
		checkmarkIgnitionSwitchTransmission = new Checkmark();
		tableLayoutPanel13 = new TableLayoutPanel();
		checkmarkVehicleSpeedTransmission = new Checkmark();
		label7 = new System.Windows.Forms.Label();
		tableLayoutPanel14 = new TableLayoutPanel();
		label8 = new System.Windows.Forms.Label();
		checkmarkDrivenDistanceTransmission = new Checkmark();
		tableLayoutPanelTransmissionStatus = new TableLayoutPanel();
		labelTransmissionStatus = new System.Windows.Forms.Label();
		checkmarkTransmissionStatus = new Checkmark();
		runServiceButtonNewOilFilterTransmission = new RunServiceButton();
		label10 = new System.Windows.Forms.Label();
		webBrowsersystemInputs = new WebBrowser();
		tableLayoutPanelHeader = new TableLayoutPanel();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentIgnitionSwitch = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMinimumDrivenDistance = new DigitalReadoutInstrument();
		tableLayoutPanel5 = new TableLayoutPanel();
		label20 = new Label();
		label22 = new Label();
		scalingLabelMinimumTime = new ScalingLabel();
		panelEngine = new Panel();
		tableLayoutPanelEngine = new TableLayoutPanel();
		tableLayoutPanelEngineStatus = new TableLayoutPanel();
		tableLayoutPanel4 = new TableLayoutPanel();
		labelOperatingTimeEngine = new System.Windows.Forms.Label();
		checkmarkOperatingTimeEngine = new Checkmark();
		tableLayoutPanel1 = new TableLayoutPanel();
		checkmarkSystemActiveEngine = new Checkmark();
		labelSystemActiveEngine = new System.Windows.Forms.Label();
		tableLayoutPanel2 = new TableLayoutPanel();
		labelIgnitionSwitchEngine = new System.Windows.Forms.Label();
		checkmarkIgnitionSwitchEngine = new Checkmark();
		tableLayoutPanel6 = new TableLayoutPanel();
		checkmarkVehicleSpeedEngine = new Checkmark();
		label2 = new System.Windows.Forms.Label();
		tableLayoutPanel3 = new TableLayoutPanel();
		labelDrivenDistanceEngine = new System.Windows.Forms.Label();
		checkmarkDrivenDistanceEngine = new Checkmark();
		tableLayoutPanelEngineStatusMessage = new TableLayoutPanel();
		labelEngineStatus = new System.Windows.Forms.Label();
		checkmarkEngineStatus = new Checkmark();
		runServiceButtonNewOilFilterEngine = new RunServiceButton();
		label3 = new System.Windows.Forms.Label();
		panelAxle1 = new Panel();
		tableLayoutPanelAxle1 = new TableLayoutPanel();
		tableLayoutPanelAxle1Conditions = new TableLayoutPanel();
		tableLayoutPanel19 = new TableLayoutPanel();
		label1 = new System.Windows.Forms.Label();
		checkmarkOperatingTimeAxle1 = new Checkmark();
		tableLayoutPanel20 = new TableLayoutPanel();
		checkmarkSystemActiveAxle1 = new Checkmark();
		label9 = new System.Windows.Forms.Label();
		tableLayoutPanel21 = new TableLayoutPanel();
		label11 = new System.Windows.Forms.Label();
		checkmarkIgnitionSwitchAxle1 = new Checkmark();
		tableLayoutPanel22 = new TableLayoutPanel();
		checkmarkVehicleSpeedAxle1 = new Checkmark();
		label12 = new System.Windows.Forms.Label();
		tableLayoutPanel23 = new TableLayoutPanel();
		label13 = new System.Windows.Forms.Label();
		checkmarkDrivenDistanceAxle1 = new Checkmark();
		tableLayoutPanelAxle1Status = new TableLayoutPanel();
		labelAxle1Status = new System.Windows.Forms.Label();
		checkmarkAxle1Status = new Checkmark();
		runServiceButtonNewOilFilterAxle1 = new RunServiceButton();
		label15 = new System.Windows.Forms.Label();
		panelAxle2 = new Panel();
		tableLayoutPanelAxle2 = new TableLayoutPanel();
		Axle2Conditions = new TableLayoutPanel();
		tableLayoutPanel28 = new TableLayoutPanel();
		label14 = new System.Windows.Forms.Label();
		checkmarkOperatingTimeAxle2 = new Checkmark();
		tableLayoutPanel29 = new TableLayoutPanel();
		checkmarkSystemActiveAxle2 = new Checkmark();
		label16 = new System.Windows.Forms.Label();
		tableLayoutPanel30 = new TableLayoutPanel();
		label17 = new System.Windows.Forms.Label();
		checkmarkIgnitionSwitchAxle2 = new Checkmark();
		tableLayoutPanel31 = new TableLayoutPanel();
		checkmarkVehicleSpeedAxle2 = new Checkmark();
		label18 = new System.Windows.Forms.Label();
		tableLayoutPanel32 = new TableLayoutPanel();
		label19 = new System.Windows.Forms.Label();
		checkmarkDrivenDistanceAxle2 = new Checkmark();
		tableLayoutPanelAxle2Status = new TableLayoutPanel();
		labelAxle2Status = new System.Windows.Forms.Label();
		checkmarkAxle2Status = new Checkmark();
		runServiceButtonNewOilFilterAxle2 = new RunServiceButton();
		label21 = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		panelTransmission.SuspendLayout();
		((Control)(object)tableLayoutPanelTransmission).SuspendLayout();
		((Control)(object)tableLayoutPanelTransmissionConditions).SuspendLayout();
		((Control)(object)tableLayoutPanel10).SuspendLayout();
		((Control)(object)tableLayoutPanel11).SuspendLayout();
		((Control)(object)tableLayoutPanel12).SuspendLayout();
		((Control)(object)tableLayoutPanel13).SuspendLayout();
		((Control)(object)tableLayoutPanel14).SuspendLayout();
		((Control)(object)tableLayoutPanelTransmissionStatus).SuspendLayout();
		((Control)(object)tableLayoutPanelHeader).SuspendLayout();
		((Control)(object)tableLayoutPanel5).SuspendLayout();
		panelEngine.SuspendLayout();
		((Control)(object)tableLayoutPanelEngine).SuspendLayout();
		((Control)(object)tableLayoutPanelEngineStatus).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel6).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanelEngineStatusMessage).SuspendLayout();
		panelAxle1.SuspendLayout();
		((Control)(object)tableLayoutPanelAxle1).SuspendLayout();
		((Control)(object)tableLayoutPanelAxle1Conditions).SuspendLayout();
		((Control)(object)tableLayoutPanel19).SuspendLayout();
		((Control)(object)tableLayoutPanel20).SuspendLayout();
		((Control)(object)tableLayoutPanel21).SuspendLayout();
		((Control)(object)tableLayoutPanel22).SuspendLayout();
		((Control)(object)tableLayoutPanel23).SuspendLayout();
		((Control)(object)tableLayoutPanelAxle1Status).SuspendLayout();
		panelAxle2.SuspendLayout();
		((Control)(object)tableLayoutPanelAxle2).SuspendLayout();
		((Control)(object)Axle2Conditions).SuspendLayout();
		((Control)(object)tableLayoutPanel28).SuspendLayout();
		((Control)(object)tableLayoutPanel29).SuspendLayout();
		((Control)(object)tableLayoutPanel30).SuspendLayout();
		((Control)(object)tableLayoutPanel31).SuspendLayout();
		((Control)(object)tableLayoutPanel32).SuspendLayout();
		((Control)(object)tableLayoutPanelAxle2Status).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelTransmission, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(webBrowsersystemInputs, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelEngine, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelAxle1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelAxle2, 0, 4);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(panelTransmission, "panelTransmission");
		panelTransmission.BorderStyle = BorderStyle.FixedSingle;
		panelTransmission.Controls.Add((Control)(object)tableLayoutPanelTransmission);
		panelTransmission.Name = "panelTransmission";
		componentResourceManager.ApplyResources(tableLayoutPanelTransmission, "tableLayoutPanelTransmission");
		((TableLayoutPanel)(object)tableLayoutPanelTransmission).Controls.Add((Control)(object)tableLayoutPanelTransmissionConditions, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTransmission).Controls.Add((Control)(object)tableLayoutPanelTransmissionStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelTransmission).Controls.Add(label10, 0, 0);
		((Control)(object)tableLayoutPanelTransmission).Name = "tableLayoutPanelTransmission";
		componentResourceManager.ApplyResources(tableLayoutPanelTransmissionConditions, "tableLayoutPanelTransmissionConditions");
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionConditions).Controls.Add((Control)(object)tableLayoutPanel10, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionConditions).Controls.Add((Control)(object)tableLayoutPanel11, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionConditions).Controls.Add((Control)(object)tableLayoutPanel12, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionConditions).Controls.Add((Control)(object)tableLayoutPanel13, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionConditions).Controls.Add((Control)(object)tableLayoutPanel14, 0, 0);
		((Control)(object)tableLayoutPanelTransmissionConditions).Name = "tableLayoutPanelTransmissionConditions";
		componentResourceManager.ApplyResources(tableLayoutPanel10, "tableLayoutPanel10");
		((TableLayoutPanel)(object)tableLayoutPanel10).Controls.Add(label4, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel10).Controls.Add((Control)(object)checkmarkOperatingTimeTransmission, 0, 0);
		((Control)(object)tableLayoutPanel10).Name = "tableLayoutPanel10";
		componentResourceManager.ApplyResources(label4, "label4");
		label4.Name = "label4";
		label4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkOperatingTimeTransmission, "checkmarkOperatingTimeTransmission");
		((Control)(object)checkmarkOperatingTimeTransmission).Name = "checkmarkOperatingTimeTransmission";
		((Control)(object)checkmarkOperatingTimeTransmission).Tag = "Operating Time too low.";
		componentResourceManager.ApplyResources(tableLayoutPanel11, "tableLayoutPanel11");
		((TableLayoutPanel)(object)tableLayoutPanel11).Controls.Add((Control)(object)checkmarkSystemActiveTransmission, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel11).Controls.Add(label5, 1, 0);
		((Control)(object)tableLayoutPanel11).Name = "tableLayoutPanel11";
		componentResourceManager.ApplyResources(checkmarkSystemActiveTransmission, "checkmarkSystemActiveTransmission");
		((Control)(object)checkmarkSystemActiveTransmission).Name = "checkmarkSystemActiveTransmission";
		((Control)(object)checkmarkSystemActiveTransmission).Tag = "Oil System Inactive.";
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel12, "tableLayoutPanel12");
		((TableLayoutPanel)(object)tableLayoutPanel12).Controls.Add(label6, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel12).Controls.Add((Control)(object)checkmarkIgnitionSwitchTransmission, 0, 0);
		((Control)(object)tableLayoutPanel12).Name = "tableLayoutPanel12";
		componentResourceManager.ApplyResources(label6, "label6");
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkIgnitionSwitchTransmission, "checkmarkIgnitionSwitchTransmission");
		((Control)(object)checkmarkIgnitionSwitchTransmission).Name = "checkmarkIgnitionSwitchTransmission";
		((Control)(object)checkmarkIgnitionSwitchTransmission).Tag = "Ignition Switch Off.";
		componentResourceManager.ApplyResources(tableLayoutPanel13, "tableLayoutPanel13");
		((TableLayoutPanel)(object)tableLayoutPanel13).Controls.Add((Control)(object)checkmarkVehicleSpeedTransmission, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel13).Controls.Add(label7, 1, 0);
		((Control)(object)tableLayoutPanel13).Name = "tableLayoutPanel13";
		componentResourceManager.ApplyResources(checkmarkVehicleSpeedTransmission, "checkmarkVehicleSpeedTransmission");
		((Control)(object)checkmarkVehicleSpeedTransmission).Name = "checkmarkVehicleSpeedTransmission";
		((Control)(object)checkmarkVehicleSpeedTransmission).Tag = "Vehicle Speed Invalid.";
		componentResourceManager.ApplyResources(label7, "label7");
		label7.Name = "label7";
		label7.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel14, "tableLayoutPanel14");
		((TableLayoutPanel)(object)tableLayoutPanel14).Controls.Add(label8, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel14).Controls.Add((Control)(object)checkmarkDrivenDistanceTransmission, 0, 0);
		((Control)(object)tableLayoutPanel14).Name = "tableLayoutPanel14";
		componentResourceManager.ApplyResources(label8, "label8");
		label8.Name = "label8";
		label8.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkDrivenDistanceTransmission, "checkmarkDrivenDistanceTransmission");
		((Control)(object)checkmarkDrivenDistanceTransmission).Name = "checkmarkDrivenDistanceTransmission";
		((Control)(object)checkmarkDrivenDistanceTransmission).Tag = "Driven Distance too low.";
		componentResourceManager.ApplyResources(tableLayoutPanelTransmissionStatus, "tableLayoutPanelTransmissionStatus");
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionStatus).Controls.Add(labelTransmissionStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionStatus).Controls.Add((Control)(object)checkmarkTransmissionStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionStatus).Controls.Add((Control)(object)runServiceButtonNewOilFilterTransmission, 2, 0);
		((Control)(object)tableLayoutPanelTransmissionStatus).Name = "tableLayoutPanelTransmissionStatus";
		componentResourceManager.ApplyResources(labelTransmissionStatus, "labelTransmissionStatus");
		labelTransmissionStatus.Name = "labelTransmissionStatus";
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionStatus).SetRowSpan((Control)labelTransmissionStatus, 2);
		labelTransmissionStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkTransmissionStatus, "checkmarkTransmissionStatus");
		((Control)(object)checkmarkTransmissionStatus).Name = "checkmarkTransmissionStatus";
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionStatus).SetRowSpan((Control)(object)checkmarkTransmissionStatus, 2);
		componentResourceManager.ApplyResources(runServiceButtonNewOilFilterTransmission, "runServiceButtonNewOilFilterTransmission");
		((Control)(object)runServiceButtonNewOilFilterTransmission).Name = "runServiceButtonNewOilFilterTransmission";
		((TableLayoutPanel)(object)tableLayoutPanelTransmissionStatus).SetRowSpan((Control)(object)runServiceButtonNewOilFilterTransmission, 2);
		runServiceButtonNewOilFilterTransmission.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=2", "Filter_condition_Diesel_particle_filter_only=0" });
		((Control)(object)runServiceButtonNewOilFilterTransmission).Tag = "Engine";
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterTransmission).Starting += runServiceButtonReplaceOilFilterTransmission_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterTransmission).Started += runServiceButtonReplaceOilFilterTransmission_Started;
		componentResourceManager.ApplyResources(label10, "label10");
		label10.Name = "label10";
		label10.UseCompatibleTextRendering = true;
		webBrowsersystemInputs.AllowWebBrowserDrop = false;
		componentResourceManager.ApplyResources(webBrowsersystemInputs, "webBrowsersystemInputs");
		webBrowsersystemInputs.IsWebBrowserContextMenuEnabled = false;
		webBrowsersystemInputs.Name = "webBrowsersystemInputs";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)webBrowsersystemInputs, 5);
		webBrowsersystemInputs.Url = new Uri("about: blank", UriKind.Absolute);
		webBrowsersystemInputs.DocumentCompleted += webBrowserEngineValues_DocumentCompleted;
		componentResourceManager.ApplyResources(tableLayoutPanelHeader, "tableLayoutPanelHeader");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelHeader, 2);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentIgnitionSwitch, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentMinimumDrivenDistance, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)tableLayoutPanel5, 1, 0);
		((Control)(object)tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentIgnitionSwitch, "digitalReadoutInstrumentIgnitionSwitch");
		digitalReadoutInstrumentIgnitionSwitch.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).FreezeValue = false;
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Initialize((ValueState)3, 3);
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(3, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus");
		((Control)(object)digitalReadoutInstrumentIgnitionSwitch).Name = "digitalReadoutInstrumentIgnitionSwitch";
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentIgnitionSwitch.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMinimumDrivenDistance, "digitalReadoutInstrumentMinimumDrivenDistance");
		digitalReadoutInstrumentMinimumDrivenDistance.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentMinimumDrivenDistance).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMinimumDrivenDistance).Instrument = new Qualifier((QualifierTypes)4, "MS01T", "Minimum_driven_distance_reset_PAR_FsMinRs_FZG");
		((Control)(object)digitalReadoutInstrumentMinimumDrivenDistance).Name = "digitalReadoutInstrumentMinimumDrivenDistance";
		((SingleInstrumentBase)digitalReadoutInstrumentMinimumDrivenDistance).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel5, "tableLayoutPanel5");
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add((Control)(object)label20, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add((Control)(object)label22, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add((Control)(object)scalingLabelMinimumTime, 0, 1);
		((Control)(object)tableLayoutPanel5).Name = "tableLayoutPanel5";
		label20.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label20, "label20");
		((Control)(object)label20).Name = "label20";
		label20.Orientation = (TextOrientation)1;
		label22.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(label22, "label22");
		((Control)(object)label22).Name = "label22";
		label22.Orientation = (TextOrientation)1;
		scalingLabelMinimumTime.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel5).SetColumnSpan((Control)(object)scalingLabelMinimumTime, 2);
		componentResourceManager.ApplyResources(scalingLabelMinimumTime, "scalingLabelMinimumTime");
		scalingLabelMinimumTime.FontGroup = null;
		scalingLabelMinimumTime.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelMinimumTime).Name = "scalingLabelMinimumTime";
		componentResourceManager.ApplyResources(panelEngine, "panelEngine");
		panelEngine.BorderStyle = BorderStyle.FixedSingle;
		panelEngine.Controls.Add((Control)(object)tableLayoutPanelEngine);
		panelEngine.Name = "panelEngine";
		componentResourceManager.ApplyResources(tableLayoutPanelEngine, "tableLayoutPanelEngine");
		((TableLayoutPanel)(object)tableLayoutPanelEngine).Controls.Add((Control)(object)tableLayoutPanelEngineStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelEngine).Controls.Add((Control)(object)tableLayoutPanelEngineStatusMessage, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelEngine).Controls.Add(label3, 0, 0);
		((Control)(object)tableLayoutPanelEngine).Name = "tableLayoutPanelEngine";
		componentResourceManager.ApplyResources(tableLayoutPanelEngineStatus, "tableLayoutPanelEngineStatus");
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatus).Controls.Add((Control)(object)tableLayoutPanel4, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatus).Controls.Add((Control)(object)tableLayoutPanel1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatus).Controls.Add((Control)(object)tableLayoutPanel2, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatus).Controls.Add((Control)(object)tableLayoutPanel6, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatus).Controls.Add((Control)(object)tableLayoutPanel3, 0, 0);
		((Control)(object)tableLayoutPanelEngineStatus).Name = "tableLayoutPanelEngineStatus";
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(labelOperatingTimeEngine, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)checkmarkOperatingTimeEngine, 0, 0);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		componentResourceManager.ApplyResources(labelOperatingTimeEngine, "labelOperatingTimeEngine");
		labelOperatingTimeEngine.Name = "labelOperatingTimeEngine";
		labelOperatingTimeEngine.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkOperatingTimeEngine, "checkmarkOperatingTimeEngine");
		((Control)(object)checkmarkOperatingTimeEngine).Name = "checkmarkOperatingTimeEngine";
		((Control)(object)checkmarkOperatingTimeEngine).Tag = "Operating Time too low.";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkSystemActiveEngine, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelSystemActiveEngine, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmarkSystemActiveEngine, "checkmarkSystemActiveEngine");
		((Control)(object)checkmarkSystemActiveEngine).Name = "checkmarkSystemActiveEngine";
		((Control)(object)checkmarkSystemActiveEngine).Tag = "Oil System Inactive.";
		componentResourceManager.ApplyResources(labelSystemActiveEngine, "labelSystemActiveEngine");
		labelSystemActiveEngine.Name = "labelSystemActiveEngine";
		labelSystemActiveEngine.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelIgnitionSwitchEngine, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmarkIgnitionSwitchEngine, 0, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(labelIgnitionSwitchEngine, "labelIgnitionSwitchEngine");
		labelIgnitionSwitchEngine.Name = "labelIgnitionSwitchEngine";
		labelIgnitionSwitchEngine.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkIgnitionSwitchEngine, "checkmarkIgnitionSwitchEngine");
		((Control)(object)checkmarkIgnitionSwitchEngine).Name = "checkmarkIgnitionSwitchEngine";
		((Control)(object)checkmarkIgnitionSwitchEngine).Tag = "Ignition Switch Off.";
		componentResourceManager.ApplyResources(tableLayoutPanel6, "tableLayoutPanel6");
		((TableLayoutPanel)(object)tableLayoutPanel6).Controls.Add((Control)(object)checkmarkVehicleSpeedEngine, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel6).Controls.Add(label2, 1, 0);
		((Control)(object)tableLayoutPanel6).Name = "tableLayoutPanel6";
		componentResourceManager.ApplyResources(checkmarkVehicleSpeedEngine, "checkmarkVehicleSpeedEngine");
		((Control)(object)checkmarkVehicleSpeedEngine).Name = "checkmarkVehicleSpeedEngine";
		((Control)(object)checkmarkVehicleSpeedEngine).Tag = "Vehicle Speed Invalid.";
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(labelDrivenDistanceEngine, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)checkmarkDrivenDistanceEngine, 0, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(labelDrivenDistanceEngine, "labelDrivenDistanceEngine");
		labelDrivenDistanceEngine.Name = "labelDrivenDistanceEngine";
		labelDrivenDistanceEngine.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkDrivenDistanceEngine, "checkmarkDrivenDistanceEngine");
		((Control)(object)checkmarkDrivenDistanceEngine).Name = "checkmarkDrivenDistanceEngine";
		((Control)(object)checkmarkDrivenDistanceEngine).Tag = "Driven Distance too low.";
		componentResourceManager.ApplyResources(tableLayoutPanelEngineStatusMessage, "tableLayoutPanelEngineStatusMessage");
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatusMessage).Controls.Add(labelEngineStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatusMessage).Controls.Add((Control)(object)checkmarkEngineStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatusMessage).Controls.Add((Control)(object)runServiceButtonNewOilFilterEngine, 2, 0);
		((Control)(object)tableLayoutPanelEngineStatusMessage).Name = "tableLayoutPanelEngineStatusMessage";
		componentResourceManager.ApplyResources(labelEngineStatus, "labelEngineStatus");
		labelEngineStatus.Name = "labelEngineStatus";
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatusMessage).SetRowSpan((Control)labelEngineStatus, 2);
		labelEngineStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkEngineStatus, "checkmarkEngineStatus");
		((Control)(object)checkmarkEngineStatus).Name = "checkmarkEngineStatus";
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatusMessage).SetRowSpan((Control)(object)checkmarkEngineStatus, 2);
		componentResourceManager.ApplyResources(runServiceButtonNewOilFilterEngine, "runServiceButtonNewOilFilterEngine");
		((Control)(object)runServiceButtonNewOilFilterEngine).Name = "runServiceButtonNewOilFilterEngine";
		((TableLayoutPanel)(object)tableLayoutPanelEngineStatusMessage).SetRowSpan((Control)(object)runServiceButtonNewOilFilterEngine, 2);
		runServiceButtonNewOilFilterEngine.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=1", "Filter_condition_Diesel_particle_filter_only=0" });
		((Control)(object)runServiceButtonNewOilFilterEngine).Tag = "Engine";
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterEngine).Starting += runServiceButtonReplaceOilFilterEngine_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterEngine).Started += runServiceButtonReplaceOilFilterEngine_Started;
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		label3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(panelAxle1, "panelAxle1");
		panelAxle1.BorderStyle = BorderStyle.FixedSingle;
		panelAxle1.Controls.Add((Control)(object)tableLayoutPanelAxle1);
		panelAxle1.Name = "panelAxle1";
		componentResourceManager.ApplyResources(tableLayoutPanelAxle1, "tableLayoutPanelAxle1");
		((TableLayoutPanel)(object)tableLayoutPanelAxle1).Controls.Add((Control)(object)tableLayoutPanelAxle1Conditions, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1).Controls.Add((Control)(object)tableLayoutPanelAxle1Status, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1).Controls.Add(label15, 0, 0);
		((Control)(object)tableLayoutPanelAxle1).Name = "tableLayoutPanelAxle1";
		componentResourceManager.ApplyResources(tableLayoutPanelAxle1Conditions, "tableLayoutPanelAxle1Conditions");
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Conditions).Controls.Add((Control)(object)tableLayoutPanel19, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Conditions).Controls.Add((Control)(object)tableLayoutPanel20, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Conditions).Controls.Add((Control)(object)tableLayoutPanel21, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Conditions).Controls.Add((Control)(object)tableLayoutPanel22, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Conditions).Controls.Add((Control)(object)tableLayoutPanel23, 0, 0);
		((Control)(object)tableLayoutPanelAxle1Conditions).Name = "tableLayoutPanelAxle1Conditions";
		componentResourceManager.ApplyResources(tableLayoutPanel19, "tableLayoutPanel19");
		((TableLayoutPanel)(object)tableLayoutPanel19).Controls.Add(label1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel19).Controls.Add((Control)(object)checkmarkOperatingTimeAxle1, 0, 0);
		((Control)(object)tableLayoutPanel19).Name = "tableLayoutPanel19";
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkOperatingTimeAxle1, "checkmarkOperatingTimeAxle1");
		((Control)(object)checkmarkOperatingTimeAxle1).Name = "checkmarkOperatingTimeAxle1";
		((Control)(object)checkmarkOperatingTimeAxle1).Tag = "Operating Time too low.";
		componentResourceManager.ApplyResources(tableLayoutPanel20, "tableLayoutPanel20");
		((TableLayoutPanel)(object)tableLayoutPanel20).Controls.Add((Control)(object)checkmarkSystemActiveAxle1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel20).Controls.Add(label9, 1, 0);
		((Control)(object)tableLayoutPanel20).Name = "tableLayoutPanel20";
		componentResourceManager.ApplyResources(checkmarkSystemActiveAxle1, "checkmarkSystemActiveAxle1");
		((Control)(object)checkmarkSystemActiveAxle1).Name = "checkmarkSystemActiveAxle1";
		((Control)(object)checkmarkSystemActiveAxle1).Tag = "Oil System Inactive.";
		componentResourceManager.ApplyResources(label9, "label9");
		label9.Name = "label9";
		label9.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel21, "tableLayoutPanel21");
		((TableLayoutPanel)(object)tableLayoutPanel21).Controls.Add(label11, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel21).Controls.Add((Control)(object)checkmarkIgnitionSwitchAxle1, 0, 0);
		((Control)(object)tableLayoutPanel21).Name = "tableLayoutPanel21";
		componentResourceManager.ApplyResources(label11, "label11");
		label11.Name = "label11";
		label11.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkIgnitionSwitchAxle1, "checkmarkIgnitionSwitchAxle1");
		((Control)(object)checkmarkIgnitionSwitchAxle1).Name = "checkmarkIgnitionSwitchAxle1";
		((Control)(object)checkmarkIgnitionSwitchAxle1).Tag = "Ignition Switch Off.";
		componentResourceManager.ApplyResources(tableLayoutPanel22, "tableLayoutPanel22");
		((TableLayoutPanel)(object)tableLayoutPanel22).Controls.Add((Control)(object)checkmarkVehicleSpeedAxle1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel22).Controls.Add(label12, 1, 0);
		((Control)(object)tableLayoutPanel22).Name = "tableLayoutPanel22";
		componentResourceManager.ApplyResources(checkmarkVehicleSpeedAxle1, "checkmarkVehicleSpeedAxle1");
		((Control)(object)checkmarkVehicleSpeedAxle1).Name = "checkmarkVehicleSpeedAxle1";
		((Control)(object)checkmarkVehicleSpeedAxle1).Tag = "Vehicle Speed Invalid.";
		componentResourceManager.ApplyResources(label12, "label12");
		label12.Name = "label12";
		label12.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel23, "tableLayoutPanel23");
		((TableLayoutPanel)(object)tableLayoutPanel23).Controls.Add(label13, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel23).Controls.Add((Control)(object)checkmarkDrivenDistanceAxle1, 0, 0);
		((Control)(object)tableLayoutPanel23).Name = "tableLayoutPanel23";
		componentResourceManager.ApplyResources(label13, "label13");
		label13.Name = "label13";
		label13.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkDrivenDistanceAxle1, "checkmarkDrivenDistanceAxle1");
		((Control)(object)checkmarkDrivenDistanceAxle1).Name = "checkmarkDrivenDistanceAxle1";
		((Control)(object)checkmarkDrivenDistanceAxle1).Tag = "Driven Distance too low.";
		componentResourceManager.ApplyResources(tableLayoutPanelAxle1Status, "tableLayoutPanelAxle1Status");
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Status).Controls.Add(labelAxle1Status, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Status).Controls.Add((Control)(object)checkmarkAxle1Status, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Status).Controls.Add((Control)(object)runServiceButtonNewOilFilterAxle1, 2, 0);
		((Control)(object)tableLayoutPanelAxle1Status).Name = "tableLayoutPanelAxle1Status";
		componentResourceManager.ApplyResources(labelAxle1Status, "labelAxle1Status");
		labelAxle1Status.Name = "labelAxle1Status";
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Status).SetRowSpan((Control)labelAxle1Status, 2);
		labelAxle1Status.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkAxle1Status, "checkmarkAxle1Status");
		((Control)(object)checkmarkAxle1Status).Name = "checkmarkAxle1Status";
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Status).SetRowSpan((Control)(object)checkmarkAxle1Status, 2);
		componentResourceManager.ApplyResources(runServiceButtonNewOilFilterAxle1, "runServiceButtonNewOilFilterAxle1");
		((Control)(object)runServiceButtonNewOilFilterAxle1).Name = "runServiceButtonNewOilFilterAxle1";
		((TableLayoutPanel)(object)tableLayoutPanelAxle1Status).SetRowSpan((Control)(object)runServiceButtonNewOilFilterAxle1, 2);
		runServiceButtonNewOilFilterAxle1.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=6", "Filter_condition_Diesel_particle_filter_only=0" });
		((Control)(object)runServiceButtonNewOilFilterAxle1).Tag = "Engine";
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterAxle1).Starting += runServiceButtonReplaceOilFilterAxle1_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterAxle1).Started += runServiceButtonReplaceOilFilterAxle1_Started;
		componentResourceManager.ApplyResources(label15, "label15");
		label15.Name = "label15";
		label15.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(panelAxle2, "panelAxle2");
		panelAxle2.BorderStyle = BorderStyle.FixedSingle;
		panelAxle2.Controls.Add((Control)(object)tableLayoutPanelAxle2);
		panelAxle2.Name = "panelAxle2";
		componentResourceManager.ApplyResources(tableLayoutPanelAxle2, "tableLayoutPanelAxle2");
		((TableLayoutPanel)(object)tableLayoutPanelAxle2).Controls.Add((Control)(object)Axle2Conditions, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelAxle2).Controls.Add((Control)(object)tableLayoutPanelAxle2Status, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelAxle2).Controls.Add(label21, 0, 0);
		((Control)(object)tableLayoutPanelAxle2).Name = "tableLayoutPanelAxle2";
		componentResourceManager.ApplyResources(Axle2Conditions, "Axle2Conditions");
		((TableLayoutPanel)(object)Axle2Conditions).Controls.Add((Control)(object)tableLayoutPanel28, 1, 0);
		((TableLayoutPanel)(object)Axle2Conditions).Controls.Add((Control)(object)tableLayoutPanel29, 0, 1);
		((TableLayoutPanel)(object)Axle2Conditions).Controls.Add((Control)(object)tableLayoutPanel30, 2, 0);
		((TableLayoutPanel)(object)Axle2Conditions).Controls.Add((Control)(object)tableLayoutPanel31, 2, 1);
		((TableLayoutPanel)(object)Axle2Conditions).Controls.Add((Control)(object)tableLayoutPanel32, 0, 0);
		((Control)(object)Axle2Conditions).Name = "Axle2Conditions";
		componentResourceManager.ApplyResources(tableLayoutPanel28, "tableLayoutPanel28");
		((TableLayoutPanel)(object)tableLayoutPanel28).Controls.Add(label14, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel28).Controls.Add((Control)(object)checkmarkOperatingTimeAxle2, 0, 0);
		((Control)(object)tableLayoutPanel28).Name = "tableLayoutPanel28";
		componentResourceManager.ApplyResources(label14, "label14");
		label14.Name = "label14";
		label14.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkOperatingTimeAxle2, "checkmarkOperatingTimeAxle2");
		((Control)(object)checkmarkOperatingTimeAxle2).Name = "checkmarkOperatingTimeAxle2";
		((Control)(object)checkmarkOperatingTimeAxle2).Tag = "Operating Time too low.";
		componentResourceManager.ApplyResources(tableLayoutPanel29, "tableLayoutPanel29");
		((TableLayoutPanel)(object)tableLayoutPanel29).Controls.Add((Control)(object)checkmarkSystemActiveAxle2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel29).Controls.Add(label16, 1, 0);
		((Control)(object)tableLayoutPanel29).Name = "tableLayoutPanel29";
		componentResourceManager.ApplyResources(checkmarkSystemActiveAxle2, "checkmarkSystemActiveAxle2");
		((Control)(object)checkmarkSystemActiveAxle2).Name = "checkmarkSystemActiveAxle2";
		((Control)(object)checkmarkSystemActiveAxle2).Tag = "Oil System Inactive.";
		componentResourceManager.ApplyResources(label16, "label16");
		label16.Name = "label16";
		label16.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel30, "tableLayoutPanel30");
		((TableLayoutPanel)(object)tableLayoutPanel30).Controls.Add(label17, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel30).Controls.Add((Control)(object)checkmarkIgnitionSwitchAxle2, 0, 0);
		((Control)(object)tableLayoutPanel30).Name = "tableLayoutPanel30";
		componentResourceManager.ApplyResources(label17, "label17");
		label17.Name = "label17";
		label17.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkIgnitionSwitchAxle2, "checkmarkIgnitionSwitchAxle2");
		((Control)(object)checkmarkIgnitionSwitchAxle2).Name = "checkmarkIgnitionSwitchAxle2";
		((Control)(object)checkmarkIgnitionSwitchAxle2).Tag = "Ignition Switch Off.";
		componentResourceManager.ApplyResources(tableLayoutPanel31, "tableLayoutPanel31");
		((TableLayoutPanel)(object)tableLayoutPanel31).Controls.Add((Control)(object)checkmarkVehicleSpeedAxle2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel31).Controls.Add(label18, 1, 0);
		((Control)(object)tableLayoutPanel31).Name = "tableLayoutPanel31";
		componentResourceManager.ApplyResources(checkmarkVehicleSpeedAxle2, "checkmarkVehicleSpeedAxle2");
		((Control)(object)checkmarkVehicleSpeedAxle2).Name = "checkmarkVehicleSpeedAxle2";
		((Control)(object)checkmarkVehicleSpeedAxle2).Tag = "Vehicle Speed Invalid.";
		componentResourceManager.ApplyResources(label18, "label18");
		label18.Name = "label18";
		label18.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel32, "tableLayoutPanel32");
		((TableLayoutPanel)(object)tableLayoutPanel32).Controls.Add(label19, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel32).Controls.Add((Control)(object)checkmarkDrivenDistanceAxle2, 0, 0);
		((Control)(object)tableLayoutPanel32).Name = "tableLayoutPanel32";
		componentResourceManager.ApplyResources(label19, "label19");
		label19.Name = "label19";
		label19.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkDrivenDistanceAxle2, "checkmarkDrivenDistanceAxle2");
		((Control)(object)checkmarkDrivenDistanceAxle2).Name = "checkmarkDrivenDistanceAxle2";
		((Control)(object)checkmarkDrivenDistanceAxle2).Tag = "Driven Distance too low.";
		componentResourceManager.ApplyResources(tableLayoutPanelAxle2Status, "tableLayoutPanelAxle2Status");
		((TableLayoutPanel)(object)tableLayoutPanelAxle2Status).Controls.Add(labelAxle2Status, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAxle2Status).Controls.Add((Control)(object)checkmarkAxle2Status, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelAxle2Status).Controls.Add((Control)(object)runServiceButtonNewOilFilterAxle2, 2, 0);
		((Control)(object)tableLayoutPanelAxle2Status).Name = "tableLayoutPanelAxle2Status";
		componentResourceManager.ApplyResources(labelAxle2Status, "labelAxle2Status");
		labelAxle2Status.Name = "labelAxle2Status";
		((TableLayoutPanel)(object)tableLayoutPanelAxle2Status).SetRowSpan((Control)labelAxle2Status, 2);
		labelAxle2Status.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkAxle2Status, "checkmarkAxle2Status");
		((Control)(object)checkmarkAxle2Status).Name = "checkmarkAxle2Status";
		((TableLayoutPanel)(object)tableLayoutPanelAxle2Status).SetRowSpan((Control)(object)checkmarkAxle2Status, 2);
		componentResourceManager.ApplyResources(runServiceButtonNewOilFilterAxle2, "runServiceButtonNewOilFilterAxle2");
		((Control)(object)runServiceButtonNewOilFilterAxle2).Name = "runServiceButtonNewOilFilterAxle2";
		((TableLayoutPanel)(object)tableLayoutPanelAxle2Status).SetRowSpan((Control)(object)runServiceButtonNewOilFilterAxle2, 2);
		runServiceButtonNewOilFilterAxle2.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=20", "Filter_condition_Diesel_particle_filter_only=0" });
		((Control)(object)runServiceButtonNewOilFilterAxle2).Tag = "Engine";
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterAxle2).Starting += runServiceButtonReplaceOilFilterAxle2_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonNewOilFilterAxle2).Started += runServiceButtonReplaceOilFilterAxle2_Started;
		componentResourceManager.ApplyResources(label21, "label21");
		label21.Name = "label21";
		label21.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Detroit_Maintenance_System");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)this).SizeChanged += UserPanel_SizeChanged;
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		panelTransmission.ResumeLayout(performLayout: false);
		panelTransmission.PerformLayout();
		((Control)(object)tableLayoutPanelTransmission).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTransmission).PerformLayout();
		((Control)(object)tableLayoutPanelTransmissionConditions).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTransmissionConditions).PerformLayout();
		((Control)(object)tableLayoutPanel10).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel11).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel12).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel13).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel14).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTransmissionStatus).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTransmissionStatus).PerformLayout();
		((Control)(object)tableLayoutPanelHeader).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel5).ResumeLayout(performLayout: false);
		panelEngine.ResumeLayout(performLayout: false);
		panelEngine.PerformLayout();
		((Control)(object)tableLayoutPanelEngine).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelEngine).PerformLayout();
		((Control)(object)tableLayoutPanelEngineStatus).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelEngineStatus).PerformLayout();
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel6).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelEngineStatusMessage).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelEngineStatusMessage).PerformLayout();
		panelAxle1.ResumeLayout(performLayout: false);
		panelAxle1.PerformLayout();
		((Control)(object)tableLayoutPanelAxle1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle1).PerformLayout();
		((Control)(object)tableLayoutPanelAxle1Conditions).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle1Conditions).PerformLayout();
		((Control)(object)tableLayoutPanel19).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel20).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel21).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel22).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel23).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle1Status).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle1Status).PerformLayout();
		panelAxle2.ResumeLayout(performLayout: false);
		panelAxle2.PerformLayout();
		((Control)(object)tableLayoutPanelAxle2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle2).PerformLayout();
		((Control)(object)Axle2Conditions).ResumeLayout(performLayout: false);
		((Control)(object)Axle2Conditions).PerformLayout();
		((Control)(object)tableLayoutPanel28).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel29).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel30).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel31).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel32).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle2Status).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelAxle2Status).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
