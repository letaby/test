// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Coolant_Fan_Control__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Coolant_Fan_Control__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string FanCtrlRequestResultsQualifier = "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan";
  private const string CpcName = "ECPC01T";
  private TableLayoutPanel tableLayoutPanel1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private TableLayoutPanel tableLayoutPanelFooter;
  private Checkmark checkmarkReady;
  private System.Windows.Forms.Label labelStatus;
  private SharedProcedureSelection sharedProcedureSelectionCoolantFanControl;
  private Button buttonClose;
  private Button buttonStartStop;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private System.Windows.Forms.Label labelStatus2;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponent1;

  public UserPanel() => this.InitializeComponent();

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void sharedProcedureCreatorComponent1_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    ServiceCall serviceCall;
    // ISSUE: explicit constructor call
    ((ServiceCall) ref serviceCall).\u002Ector("ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan");
    Service service = ((ServiceCall) ref serviceCall).GetService();
    if (!(service != (Service) null))
      return;
    service.Execute(false);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.tableLayoutPanelFooter = new TableLayoutPanel();
    this.labelStatus2 = new System.Windows.Forms.Label();
    this.checkmarkReady = new Checkmark();
    this.labelStatus = new System.Windows.Forms.Label();
    this.sharedProcedureSelectionCoolantFanControl = new SharedProcedureSelection();
    this.buttonClose = new Button();
    this.buttonStartStop = new Button();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponent1 = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanelFooter).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelFooter, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "CoolantControls";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS252_DrvCircInTemp_u16_DrvCircInTemp_u16");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "CoolantControls";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "CoolantControls";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).ShowValueReadout = false;
    ((Control) this.digitalReadoutInstrument3).TabStop = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "CoolantControls";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 3, 2, "mph");
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((Control) this.digitalReadoutInstrument1).TabStop = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFooter, "tableLayoutPanelFooter");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelFooter, 3);
    ((TableLayoutPanel) this.tableLayoutPanelFooter).Controls.Add((Control) this.labelStatus2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFooter).Controls.Add((Control) this.checkmarkReady, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFooter).Controls.Add((Control) this.labelStatus, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFooter).Controls.Add((Control) this.sharedProcedureSelectionCoolantFanControl, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFooter).Controls.Add((Control) this.buttonClose, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFooter).Controls.Add((Control) this.buttonStartStop, 4, 0);
    ((Control) this.tableLayoutPanelFooter).Name = "tableLayoutPanelFooter";
    componentResourceManager.ApplyResources((object) this.labelStatus2, "labelStatus2");
    this.labelStatus2.Name = "labelStatus2";
    this.labelStatus2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkReady, "checkmarkReady");
    ((Control) this.checkmarkReady).Name = "checkmarkReady";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionCoolantFanControl, "sharedProcedureSelectionCoolantFanControl");
    ((Control) this.sharedProcedureSelectionCoolantFanControl).Name = "sharedProcedureSelectionCoolantFanControl";
    this.sharedProcedureSelectionCoolantFanControl.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_CoolantFanControl"
    });
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "CoolantControls";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelectionCoolantFanControl;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmarkReady;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStartStop;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponent1.Suspend();
    this.sharedProcedureCreatorComponent1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponent1, "sharedProcedureCreatorComponent1");
    this.sharedProcedureCreatorComponent1.Qualifier = "SP_CoolantFanControl";
    this.sharedProcedureCreatorComponent1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Start", (IEnumerable<string>) new string[3]
    {
      "Requested_duty_cycle_for_Brake_Resistor_1=0",
      "Requested_duty_cycle_for_Brake_Resistor_2=0",
      "Requested_duty_cycle_for_Edrive_Fan=50"
    });
    dataItemCondition1.Gradient.Initialize((ValueState) 3, 2, "mph");
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    dataItemCondition2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    dataItemCondition2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    dataItemCondition2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    dataItemCondition2.Gradient.Initialize((ValueState) 0, 2);
    dataItemCondition2.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    this.sharedProcedureCreatorComponent1.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponent1.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponent1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Stop", (IEnumerable<string>) new string[3]
    {
      "OTF_ETHM_FanCtrl_FanBrakeResistor1=0",
      "OTF_ETHM_FanCtrl_FanBrakeResistor2=0",
      "OTF_ETHM_FanCtrl_eDriveFan=0"
    });
    this.sharedProcedureCreatorComponent1.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent1_StopServiceComplete);
    this.sharedProcedureCreatorComponent1.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Cooling_Fan_Control");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFooter).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
