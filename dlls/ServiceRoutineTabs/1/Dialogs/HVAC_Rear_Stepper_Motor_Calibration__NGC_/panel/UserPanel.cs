// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Rear_Stepper_Motor_Calibration__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Rear_Stepper_Motor_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentStepperRear;
  private BarInstrument barInstrumentMixDoorPosition;
  private DigitalReadoutInstrument digitalReadoutInstrumentDischargeTempResults;
  private RunSharedProcedureButton runSharedProcedureButton;
  private TableLayoutPanel tableLayoutPanel1;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!((RunSharedProcedureButtonBase) this.runSharedProcedureButton).InProgress)
      return;
    e.Cancel = true;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.runSharedProcedureButton = new RunSharedProcedureButton();
    this.barInstrumentMixDoorPosition = new BarInstrument();
    this.digitalReadoutInstrumentDischargeTempResults = new DigitalReadoutInstrument();
    this.sharedProcedureCreatorComponentStepperRear = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runSharedProcedureButton, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentMixDoorPosition, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentDischargeTempResults, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.runSharedProcedureButton, "runSharedProcedureButton");
    ((Control) this.runSharedProcedureButton).Name = "runSharedProcedureButton";
    this.runSharedProcedureButton.Qualifier = "SP_HVACRStepperCal";
    componentResourceManager.ApplyResources((object) this.barInstrumentMixDoorPosition, "barInstrumentMixDoorPosition");
    this.barInstrumentMixDoorPosition.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentMixDoorPosition).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentMixDoorPosition).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_R01T", "DT_Mix_Door_Actuator_Position_feedback_Mix_Door_Actuator_Position_feedback");
    ((Control) this.barInstrumentMixDoorPosition).Name = "barInstrumentMixDoorPosition";
    ((SingleInstrumentBase) this.barInstrumentMixDoorPosition).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDischargeTempResults, "digitalReadoutInstrumentDischargeTempResults");
    this.digitalReadoutInstrumentDischargeTempResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).FreezeValue = false;
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentDischargeTempResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor");
    ((Control) this.digitalReadoutInstrumentDischargeTempResults).Name = "digitalReadoutInstrumentDischargeTempResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).TitleLengthPercentOfControl = 50;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDischargeTempResults).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureCreatorComponentStepperRear.Suspend();
    this.sharedProcedureCreatorComponentStepperRear.MonitorCall = new ServiceCall("HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor");
    this.sharedProcedureCreatorComponentStepperRear.MonitorGradient.Initialize((ValueState) 0, 4);
    this.sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(1, 0.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponentStepperRear.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentStepperRear, "sharedProcedureCreatorComponentStepperRear");
    this.sharedProcedureCreatorComponentStepperRear.Qualifier = "SP_HVACRStepperCal";
    this.sharedProcedureCreatorComponentStepperRear.StartCall = new ServiceCall("HVAC_R01T", "RT_Stepper_Motor_Calibration_Start");
    this.sharedProcedureCreatorComponentStepperRear.StopCall = new ServiceCall("HVAC_R01T", "SES_Extended_P2s_CAN_ECU_max_physical");
    this.sharedProcedureCreatorComponentStepperRear.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
