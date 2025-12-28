using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Front_Stepper_Motor_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentStepperFront;

	private DigitalReadoutInstrument digitalReadoutInstrumentPanelOutletResults;

	private DigitalReadoutInstrument digitalReadoutInstrumentFloorOutletResults;

	private DigitalReadoutInstrument digitalReadoutInstrumentDefrostOutletResults;

	private DigitalReadoutInstrument digitalReadoutInstrumentAirInletRecircResults;

	private BarInstrument barInstrumentDefrostPos;

	private BarInstrument barInstrumentRecircDoorPos;

	private BarInstrument barInstrumentMixDoorPos;

	private BarInstrument barInstrumentPanelPos;

	private BarInstrument barInstrumentFloorPos;

	private DigitalReadoutInstrument digitalReadoutInstrumentDischargeTempResults;

	private RunSharedProcedureButton runSharedProcedureButton;

	private TableLayoutPanel tableLayoutPanel1;

	public UserPanel()
	{
		InitializeComponent();
	}

	private byte GetRawValue(Service service)
	{
		Choice choice = service.OutputValues[0].Value as Choice;
		if (choice != null)
		{
			return Convert.ToByte(choice.RawValue);
		}
		return byte.MaxValue;
	}

	private void sharedProcedureCreatorComponentStepperFront_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			Service monitorService = e.Service;
			IEnumerable<Service> enumerable = monitorService.Channel.Services.Where((Service s) => s != monitorService && s.RequestMessage != null && s.RequestMessage.ToString() == monitorService.RequestMessage.ToString());
			foreach (Service item in enumerable)
			{
				try
				{
					item.Execute(synchronous: true);
				}
				catch (CaesarException)
				{
					((MonitorResultEventArgs)e).Action = (MonitorAction)1;
					return;
				}
			}
			IEnumerable<Service> source = enumerable.Union(Enumerable.Repeat(monitorService, 1));
			((MonitorResultEventArgs)e).Action = (MonitorAction)(!source.Any((Service s) => GetRawValue(s) == 1 || GetRawValue(s) == 3));
		}
		else
		{
			((MonitorResultEventArgs)e).Action = (MonitorAction)1;
		}
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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
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
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrumentPanelOutletResults = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFloorOutletResults = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDefrostOutletResults = new DigitalReadoutInstrument();
		digitalReadoutInstrumentAirInletRecircResults = new DigitalReadoutInstrument();
		barInstrumentDefrostPos = new BarInstrument();
		barInstrumentRecircDoorPos = new BarInstrument();
		runSharedProcedureButton = new RunSharedProcedureButton();
		barInstrumentMixDoorPos = new BarInstrument();
		barInstrumentPanelPos = new BarInstrument();
		barInstrumentFloorPos = new BarInstrument();
		digitalReadoutInstrumentDischargeTempResults = new DigitalReadoutInstrument();
		sharedProcedureCreatorComponentStepperFront = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentPanelOutletResults, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentFloorOutletResults, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentDefrostOutletResults, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentAirInletRecircResults, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentDefrostPos, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentRecircDoorPos, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runSharedProcedureButton, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentMixDoorPos, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentPanelPos, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentFloorPos, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentDischargeTempResults, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPanelOutletResults, "digitalReadoutInstrumentPanelOutletResults");
		digitalReadoutInstrumentPanelOutletResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPanelOutletResults).FreezeValue = false;
		digitalReadoutInstrumentPanelOutletResults.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentPanelOutletResults).Instrument = new Qualifier((QualifierTypes)64, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Panel_Outlet_Air_Distribution_Motor");
		((Control)(object)digitalReadoutInstrumentPanelOutletResults).Name = "digitalReadoutInstrumentPanelOutletResults";
		((SingleInstrumentBase)digitalReadoutInstrumentPanelOutletResults).TitleLengthPercentOfControl = 50;
		((SingleInstrumentBase)digitalReadoutInstrumentPanelOutletResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFloorOutletResults, "digitalReadoutInstrumentFloorOutletResults");
		digitalReadoutInstrumentFloorOutletResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentFloorOutletResults).FreezeValue = false;
		digitalReadoutInstrumentFloorOutletResults.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentFloorOutletResults).Instrument = new Qualifier((QualifierTypes)64, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Floor_Outlet_Air_Distribution_Motor");
		((Control)(object)digitalReadoutInstrumentFloorOutletResults).Name = "digitalReadoutInstrumentFloorOutletResults";
		((SingleInstrumentBase)digitalReadoutInstrumentFloorOutletResults).TitleLengthPercentOfControl = 50;
		((SingleInstrumentBase)digitalReadoutInstrumentFloorOutletResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDefrostOutletResults, "digitalReadoutInstrumentDefrostOutletResults");
		digitalReadoutInstrumentDefrostOutletResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDefrostOutletResults).FreezeValue = false;
		digitalReadoutInstrumentDefrostOutletResults.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentDefrostOutletResults).Instrument = new Qualifier((QualifierTypes)64, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Defrost_Outlet_Air_Distribution_Motor");
		((Control)(object)digitalReadoutInstrumentDefrostOutletResults).Name = "digitalReadoutInstrumentDefrostOutletResults";
		((SingleInstrumentBase)digitalReadoutInstrumentDefrostOutletResults).TitleLengthPercentOfControl = 50;
		((SingleInstrumentBase)digitalReadoutInstrumentDefrostOutletResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAirInletRecircResults, "digitalReadoutInstrumentAirInletRecircResults");
		digitalReadoutInstrumentAirInletRecircResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAirInletRecircResults).FreezeValue = false;
		digitalReadoutInstrumentAirInletRecircResults.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentAirInletRecircResults).Instrument = new Qualifier((QualifierTypes)64, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor");
		((Control)(object)digitalReadoutInstrumentAirInletRecircResults).Name = "digitalReadoutInstrumentAirInletRecircResults";
		((SingleInstrumentBase)digitalReadoutInstrumentAirInletRecircResults).TitleLengthPercentOfControl = 50;
		((SingleInstrumentBase)digitalReadoutInstrumentAirInletRecircResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrumentDefrostPos, "barInstrumentDefrostPos");
		barInstrumentDefrostPos.FontGroup = null;
		((SingleInstrumentBase)barInstrumentDefrostPos).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDefrostPos).Instrument = new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Defrost");
		((Control)(object)barInstrumentDefrostPos).Name = "barInstrumentDefrostPos";
		((SingleInstrumentBase)barInstrumentDefrostPos).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrumentRecircDoorPos, "barInstrumentRecircDoorPos");
		barInstrumentRecircDoorPos.FontGroup = null;
		((SingleInstrumentBase)barInstrumentRecircDoorPos).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentRecircDoorPos).Instrument = new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Recirc_Actuator_Door_Position_feedback_Recirc_Actuator_Door_Position_feedback");
		((Control)(object)barInstrumentRecircDoorPos).Name = "barInstrumentRecircDoorPos";
		((SingleInstrumentBase)barInstrumentRecircDoorPos).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runSharedProcedureButton, "runSharedProcedureButton");
		((Control)(object)runSharedProcedureButton).Name = "runSharedProcedureButton";
		runSharedProcedureButton.Qualifier = "SP_HVACFStepperCal";
		componentResourceManager.ApplyResources(barInstrumentMixDoorPos, "barInstrumentMixDoorPos");
		barInstrumentMixDoorPos.FontGroup = null;
		((SingleInstrumentBase)barInstrumentMixDoorPos).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentMixDoorPos).Instrument = new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Blend_Door_Actuator_Position_Mix_Door_Actuator_Position_feedback");
		((Control)(object)barInstrumentMixDoorPos).Name = "barInstrumentMixDoorPos";
		((SingleInstrumentBase)barInstrumentMixDoorPos).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrumentPanelPos, "barInstrumentPanelPos");
		barInstrumentPanelPos.FontGroup = null;
		((SingleInstrumentBase)barInstrumentPanelPos).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentPanelPos).Instrument = new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Panel");
		((Control)(object)barInstrumentPanelPos).Name = "barInstrumentPanelPos";
		((SingleInstrumentBase)barInstrumentPanelPos).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrumentFloorPos, "barInstrumentFloorPos");
		barInstrumentFloorPos.FontGroup = null;
		((SingleInstrumentBase)barInstrumentFloorPos).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentFloorPos).Instrument = new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Floor");
		((Control)(object)barInstrumentFloorPos).Name = "barInstrumentFloorPos";
		((SingleInstrumentBase)barInstrumentFloorPos).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDischargeTempResults, "digitalReadoutInstrumentDischargeTempResults");
		digitalReadoutInstrumentDischargeTempResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).FreezeValue = false;
		digitalReadoutInstrumentDischargeTempResults.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).Instrument = new Qualifier((QualifierTypes)64, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor");
		((Control)(object)digitalReadoutInstrumentDischargeTempResults).Name = "digitalReadoutInstrumentDischargeTempResults";
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).TitleLengthPercentOfControl = 50;
		((SingleInstrumentBase)digitalReadoutInstrumentDischargeTempResults).UnitAlignment = StringAlignment.Near;
		sharedProcedureCreatorComponentStepperFront.Suspend();
		sharedProcedureCreatorComponentStepperFront.MonitorCall = new ServiceCall("HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentStepperFront, "sharedProcedureCreatorComponentStepperFront");
		sharedProcedureCreatorComponentStepperFront.Qualifier = "SP_HVACFStepperCal";
		sharedProcedureCreatorComponentStepperFront.StartCall = new ServiceCall("HVAC_F01T", "RT_Stepper_Motor_Calibration_Start");
		sharedProcedureCreatorComponentStepperFront.StopCall = new ServiceCall("HVAC_F01T", "SES_Extended_P2s_CAN_ECU_max_physical");
		sharedProcedureCreatorComponentStepperFront.MonitorServiceComplete += sharedProcedureCreatorComponentStepperFront_MonitorServiceComplete;
		sharedProcedureCreatorComponentStepperFront.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_HVAC_Front_Stepper_Motor_Calibration");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
