using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Rear_Stepper_Motor_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentStepperRear;

	private BarInstrument barInstrumentMixDoorPosition;

	private DigitalReadoutInstrument digitalReadoutInstrumentDischargeTempResults;

	private RunSharedProcedureButton runSharedProcedureButton;

	private TableLayoutPanel tableLayoutPanel1;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (((RunSharedProcedureButtonBase)runSharedProcedureButton).InProgress)
		{
			e.Cancel = true;
		}
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanel1 = new TableLayoutPanel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		runSharedProcedureButton = new RunSharedProcedureButton();
		barInstrumentMixDoorPosition = new BarInstrument();
		digitalReadoutInstrumentDischargeTempResults = new DigitalReadoutInstrument();
		sharedProcedureCreatorComponentStepperRear = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runSharedProcedureButton, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentMixDoorPosition, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentDischargeTempResults, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(runSharedProcedureButton, "runSharedProcedureButton");
		((Control)(object)runSharedProcedureButton).Name = "runSharedProcedureButton";
		runSharedProcedureButton.Qualifier = "SP_HVACRStepperCal";
		componentResourceManager.ApplyResources(barInstrumentMixDoorPosition, "barInstrumentMixDoorPosition");
		barInstrumentMixDoorPosition.FontGroup = null;
		((SingleInstrumentBase)barInstrumentMixDoorPosition).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentMixDoorPosition).Instrument = new Qualifier((QualifierTypes)1, "HVAC_R01T", "DT_Mix_Door_Actuator_Position_feedback_Mix_Door_Actuator_Position_feedback");
		((Control)(object)barInstrumentMixDoorPosition).Name = "barInstrumentMixDoorPosition";
		((SingleInstrumentBase)barInstrumentMixDoorPosition).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDischargeTempResults, "digitalReadoutInstrumentDischargeTempResults");
		digitalReadoutInstrumentDischargeTempResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).FreezeValue = false;
		digitalReadoutInstrumentDischargeTempResults.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).Instrument = new Qualifier((QualifierTypes)64, "HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor");
		((Control)(object)digitalReadoutInstrumentDischargeTempResults).Name = "digitalReadoutInstrumentDischargeTempResults";
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).TitleLengthPercentOfControl = 50;
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).UnitAlignment = StringAlignment.Near;
		sharedProcedureCreatorComponentStepperRear.Suspend();
		sharedProcedureCreatorComponentStepperRear.MonitorCall = new ServiceCall("HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor");
		sharedProcedureCreatorComponentStepperRear.MonitorGradient.Initialize((ValueState)0, 4);
		sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(1, 0.0, (ValueState)0);
		sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentStepperRear, "sharedProcedureCreatorComponentStepperRear");
		sharedProcedureCreatorComponentStepperRear.Qualifier = "SP_HVACRStepperCal";
		sharedProcedureCreatorComponentStepperRear.StartCall = new ServiceCall("HVAC_R01T", "RT_Stepper_Motor_Calibration_Start");
		sharedProcedureCreatorComponentStepperRear.StopCall = new ServiceCall("HVAC_R01T", "SES_Extended_P2s_CAN_ECU_max_physical");
		sharedProcedureCreatorComponentStepperRear.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
