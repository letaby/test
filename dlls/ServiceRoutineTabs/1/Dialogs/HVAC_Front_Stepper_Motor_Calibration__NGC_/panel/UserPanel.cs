// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Front_Stepper_Motor_Calibration__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
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

  public UserPanel() => this.InitializeComponent();

  private byte GetRawValue(Service service)
  {
    Choice choice = service.OutputValues[0].Value as Choice;
    return choice != (object) null ? Convert.ToByte(choice.RawValue) : byte.MaxValue;
  }

  private void sharedProcedureCreatorComponentStepperFront_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      Service monitorService = e.Service;
      IEnumerable<Service> first = monitorService.Channel.Services.Where<Service>((Func<Service, bool>) (s => s != monitorService && s.RequestMessage != null && s.RequestMessage.ToString() == monitorService.RequestMessage.ToString()));
      foreach (Service service in first)
      {
        try
        {
          service.Execute(true);
        }
        catch (CaesarException ex)
        {
          ((MonitorResultEventArgs) e).Action = (MonitorAction) 1;
          return;
        }
      }
      IEnumerable<Service> source = first.Union<Service>(Enumerable.Repeat<Service>(monitorService, 1));
      ((MonitorResultEventArgs) e).Action = source.Any<Service>((Func<Service, bool>) (s => this.GetRawValue(s) == (byte) 1 || this.GetRawValue(s) == (byte) 3)) ? (MonitorAction) 0 : (MonitorAction) 1;
    }
    else
      ((MonitorResultEventArgs) e).Action = (MonitorAction) 1;
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!((RunSharedProcedureButtonBase) this.runSharedProcedureButton).InProgress)
      return;
    e.Cancel = true;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrumentPanelOutletResults = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFloorOutletResults = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDefrostOutletResults = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentAirInletRecircResults = new DigitalReadoutInstrument();
    this.barInstrumentDefrostPos = new BarInstrument();
    this.barInstrumentRecircDoorPos = new BarInstrument();
    this.runSharedProcedureButton = new RunSharedProcedureButton();
    this.barInstrumentMixDoorPos = new BarInstrument();
    this.barInstrumentPanelPos = new BarInstrument();
    this.barInstrumentFloorPos = new BarInstrument();
    this.digitalReadoutInstrumentDischargeTempResults = new DigitalReadoutInstrument();
    this.sharedProcedureCreatorComponentStepperFront = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentPanelOutletResults, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentFloorOutletResults, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentDefrostOutletResults, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentAirInletRecircResults, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentDefrostPos, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentRecircDoorPos, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runSharedProcedureButton, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentMixDoorPos, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentPanelPos, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentFloorPos, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentDischargeTempResults, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPanelOutletResults, "digitalReadoutInstrumentPanelOutletResults");
    this.digitalReadoutInstrumentPanelOutletResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPanelOutletResults).FreezeValue = false;
    this.digitalReadoutInstrumentPanelOutletResults.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentPanelOutletResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPanelOutletResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Panel_Outlet_Air_Distribution_Motor");
    ((Control) this.digitalReadoutInstrumentPanelOutletResults).Name = "digitalReadoutInstrumentPanelOutletResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPanelOutletResults).TitleLengthPercentOfControl = 50;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPanelOutletResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFloorOutletResults, "digitalReadoutInstrumentFloorOutletResults");
    this.digitalReadoutInstrumentFloorOutletResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFloorOutletResults).FreezeValue = false;
    this.digitalReadoutInstrumentFloorOutletResults.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentFloorOutletResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFloorOutletResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Floor_Outlet_Air_Distribution_Motor");
    ((Control) this.digitalReadoutInstrumentFloorOutletResults).Name = "digitalReadoutInstrumentFloorOutletResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFloorOutletResults).TitleLengthPercentOfControl = 50;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFloorOutletResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDefrostOutletResults, "digitalReadoutInstrumentDefrostOutletResults");
    this.digitalReadoutInstrumentDefrostOutletResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefrostOutletResults).FreezeValue = false;
    this.digitalReadoutInstrumentDefrostOutletResults.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentDefrostOutletResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefrostOutletResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Defrost_Outlet_Air_Distribution_Motor");
    ((Control) this.digitalReadoutInstrumentDefrostOutletResults).Name = "digitalReadoutInstrumentDefrostOutletResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefrostOutletResults).TitleLengthPercentOfControl = 50;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefrostOutletResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAirInletRecircResults, "digitalReadoutInstrumentAirInletRecircResults");
    this.digitalReadoutInstrumentAirInletRecircResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirInletRecircResults).FreezeValue = false;
    this.digitalReadoutInstrumentAirInletRecircResults.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentAirInletRecircResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirInletRecircResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor");
    ((Control) this.digitalReadoutInstrumentAirInletRecircResults).Name = "digitalReadoutInstrumentAirInletRecircResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirInletRecircResults).TitleLengthPercentOfControl = 50;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirInletRecircResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrumentDefrostPos, "barInstrumentDefrostPos");
    this.barInstrumentDefrostPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentDefrostPos).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDefrostPos).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Defrost");
    ((Control) this.barInstrumentDefrostPos).Name = "barInstrumentDefrostPos";
    ((SingleInstrumentBase) this.barInstrumentDefrostPos).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrumentRecircDoorPos, "barInstrumentRecircDoorPos");
    this.barInstrumentRecircDoorPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentRecircDoorPos).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentRecircDoorPos).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Recirc_Actuator_Door_Position_feedback_Recirc_Actuator_Door_Position_feedback");
    ((Control) this.barInstrumentRecircDoorPos).Name = "barInstrumentRecircDoorPos";
    ((SingleInstrumentBase) this.barInstrumentRecircDoorPos).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runSharedProcedureButton, "runSharedProcedureButton");
    ((Control) this.runSharedProcedureButton).Name = "runSharedProcedureButton";
    this.runSharedProcedureButton.Qualifier = "SP_HVACFStepperCal";
    componentResourceManager.ApplyResources((object) this.barInstrumentMixDoorPos, "barInstrumentMixDoorPos");
    this.barInstrumentMixDoorPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentMixDoorPos).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentMixDoorPos).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Blend_Door_Actuator_Position_Mix_Door_Actuator_Position_feedback");
    ((Control) this.barInstrumentMixDoorPos).Name = "barInstrumentMixDoorPos";
    ((SingleInstrumentBase) this.barInstrumentMixDoorPos).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrumentPanelPos, "barInstrumentPanelPos");
    this.barInstrumentPanelPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentPanelPos).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentPanelPos).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Panel");
    ((Control) this.barInstrumentPanelPos).Name = "barInstrumentPanelPos";
    ((SingleInstrumentBase) this.barInstrumentPanelPos).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrumentFloorPos, "barInstrumentFloorPos");
    this.barInstrumentFloorPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentFloorPos).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentFloorPos).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Floor");
    ((Control) this.barInstrumentFloorPos).Name = "barInstrumentFloorPos";
    ((SingleInstrumentBase) this.barInstrumentFloorPos).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDischargeTempResults, "digitalReadoutInstrumentDischargeTempResults");
    this.digitalReadoutInstrumentDischargeTempResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).FreezeValue = false;
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor");
    ((Control) this.digitalReadoutInstrumentDischargeTempResults).Name = "digitalReadoutInstrumentDischargeTempResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).TitleLengthPercentOfControl = 50;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureCreatorComponentStepperFront.Suspend();
    this.sharedProcedureCreatorComponentStepperFront.MonitorCall = new ServiceCall("HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentStepperFront, "sharedProcedureCreatorComponentStepperFront");
    this.sharedProcedureCreatorComponentStepperFront.Qualifier = "SP_HVACFStepperCal";
    this.sharedProcedureCreatorComponentStepperFront.StartCall = new ServiceCall("HVAC_F01T", "RT_Stepper_Motor_Calibration_Start");
    this.sharedProcedureCreatorComponentStepperFront.StopCall = new ServiceCall("HVAC_F01T", "SES_Extended_P2s_CAN_ECU_max_physical");
    this.sharedProcedureCreatorComponentStepperFront.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponentStepperFront_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentStepperFront.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_HVAC_Front_Stepper_Motor_Calibration");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
