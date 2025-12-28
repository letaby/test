// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__MY20_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__MY20_.panel;

public class UserPanel : CustomPanel
{
  private BarInstrument BarInstrument13;
  private BarInstrument BarInstrument11;
  private BarInstrument BarInstrument10;
  private BarInstrument BarInstrument9;
  private ListInstrument ListInstrument1;
  private BarInstrument BarInstrument5;
  private BarInstrument BarInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonStart;
  private Button buttonStop;
  private BarInstrument barInstrument8;
  private BarInstrument barInstrument14;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument7;
  private DigitalReadoutInstrument digitalReadoutInstrument8;
  private DigitalReadoutInstrument digitalReadoutInstrument9;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private DigitalReadoutInstrument digitalReadoutInstrument10;
  private DigitalReadoutInstrument digitalReadoutInstrumentDefPressure;
  private TableLayoutPanel tableStatusIndicators;
  private Checkmark checkmarkStatus;
  private Label labelStatus;
  private TextBox textBoxProgress;
  private DigitalReadoutInstrument digitalReadoutInstrument11;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument13;
  private DigitalReadoutInstrument digitalReadoutInstrument12;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;

  public UserPanel() => this.InitializeComponent();

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableStatusIndicators = new TableLayoutPanel();
    this.textBoxProgress = new TextBox();
    this.checkmarkStatus = new Checkmark();
    this.labelStatus = new Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrument13 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument12 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument11 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.buttonStop = new Button();
    this.ListInstrument1 = new ListInstrument();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.BarInstrument11 = new BarInstrument();
    this.BarInstrument10 = new BarInstrument();
    this.BarInstrument9 = new BarInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDefPressure = new DigitalReadoutInstrument();
    this.barInstrument14 = new BarInstrument();
    this.BarInstrument13 = new BarInstrument();
    this.barInstrument8 = new BarInstrument();
    this.BarInstrument1 = new BarInstrument();
    this.BarInstrument5 = new BarInstrument();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.buttonStart = new Button();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument10 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument9 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableStatusIndicators).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableStatusIndicators, "tableStatusIndicators");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableStatusIndicators, 5);
    ((TableLayoutPanel) this.tableStatusIndicators).Controls.Add((Control) this.textBoxProgress, 0, 1);
    ((TableLayoutPanel) this.tableStatusIndicators).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((TableLayoutPanel) this.tableStatusIndicators).Controls.Add((Control) this.labelStatus, 1, 0);
    ((Control) this.tableStatusIndicators).Name = "tableStatusIndicators";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableStatusIndicators, 3);
    ((TableLayoutPanel) this.tableStatusIndicators).SetColumnSpan((Control) this.textBoxProgress, 6);
    componentResourceManager.ApplyResources((object) this.textBoxProgress, "textBoxProgress");
    this.textBoxProgress.Name = "textBoxProgress";
    this.textBoxProgress.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument13, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument12, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument11, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStop, 1, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ListInstrument1, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 0, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument11, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument10, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument9, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentDefPressure, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument14, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument13, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument8, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument5, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 0, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableStatusIndicators, 2, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument10, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument8, 1, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument9, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument7, 1, 7);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument13, "digitalReadoutInstrument13");
    this.digitalReadoutInstrument13.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument13).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument13).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS019_Barometric_Pressure");
    ((Control) this.digitalReadoutInstrument13).Name = "digitalReadoutInstrument13";
    ((SingleInstrumentBase) this.digitalReadoutInstrument13).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument12, "digitalReadoutInstrument12");
    this.digitalReadoutInstrument12.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletPressure");
    ((Control) this.digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument11, "digitalReadoutInstrument11");
    this.digitalReadoutInstrument11.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).FreezeValue = false;
    this.digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrument11.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrument11.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet");
    ((Control) this.digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.ForeColor = SystemColors.ControlText;
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.ListInstrument1, 5);
    componentResourceManager.ApplyResources((object) this.ListInstrument1, "ListInstrument1");
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Switches", new Qualifier[10]
    {
      new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_Clutch_Open"),
      new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_Parking_Brake"),
      new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_Neutral_Switch"),
      new Qualifier((QualifierTypes) 1, "CPC302T", "DT_DS255_Blocktransfer_ClutchOpen"),
      new Qualifier((QualifierTypes) 1, "CPC302T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
      new Qualifier((QualifierTypes) 1, "CPC302T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
      new Qualifier((QualifierTypes) 1, "CPC501T", "DT_DS255_Blocktransfer_ClutchStatus"),
      new Qualifier((QualifierTypes) 1, "CPC501T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
      new Qualifier((QualifierTypes) 1, "CPC501T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Heaters", new Qualifier[5]
    {
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS004_Line_Heater_1"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS004_Line_Heater_2"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS004_Line_Heater_3"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS004_Line_Heater_4"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS011_heater_state_dosing_unit")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("SCR Model Temperatures", new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS128_SCR_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS129_SCR_Heat_Generation")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Zone/Level", new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS024_DEF_Tank_Level"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS065_Actual_DPF_zone")
    }));
    ((Control) this.ListInstrument1).Name = "ListInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.ListInstrument1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.sharedProcedureSelection1, 2);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[3]
    {
      "SP_ChassisDynoBasicScrConversionCheck_MY20",
      "SP_OutputComponentTest_MY20",
      "SP_ParkedScrEfficiencyTest_MY20"
    });
    this.BarInstrument11.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument11.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument11, "BarInstrument11");
    this.BarInstrument11.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument11).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS009_DPF_Oultlet_Temperature");
    ((Control) this.BarInstrument11).Name = "BarInstrument11";
    ((AxisSingleInstrumentBase) this.BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.BarInstrument11, 4);
    ((SingleInstrumentBase) this.BarInstrument11).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument11).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument11).UnitAlignment = StringAlignment.Near;
    this.BarInstrument10.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument10.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument10, "BarInstrument10");
    this.BarInstrument10.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.BarInstrument10).Name = "BarInstrument10";
    ((AxisSingleInstrumentBase) this.BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.BarInstrument10, 4);
    ((SingleInstrumentBase) this.BarInstrument10).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument10).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument10).UnitAlignment = StringAlignment.Near;
    this.BarInstrument9.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument9.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument9, "BarInstrument9");
    this.BarInstrument9.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.BarInstrument9).Name = "BarInstrument9";
    ((AxisSingleInstrumentBase) this.BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.BarInstrument9, 4);
    ((SingleInstrumentBase) this.BarInstrument9).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS080_ADS_DEF_Quantity_Request");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDefPressure, "digitalReadoutInstrumentDefPressure");
    this.digitalReadoutInstrumentDefPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure");
    ((Control) this.digitalReadoutInstrumentDefPressure).Name = "digitalReadoutInstrumentDefPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDefPressure).UnitAlignment = StringAlignment.Near;
    this.barInstrument14.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument14.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument14, "barInstrument14");
    this.barInstrument14.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.barInstrument14).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument14).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS022_DEF_tank_Temperature");
    ((Control) this.barInstrument14).Name = "barInstrument14";
    ((AxisSingleInstrumentBase) this.barInstrument14).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.barInstrument14, 4);
    ((SingleInstrumentBase) this.barInstrument14).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument14).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument14).UnitAlignment = StringAlignment.Near;
    this.BarInstrument13.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument13.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument13, "BarInstrument13");
    this.BarInstrument13.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument13).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument13).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS019_SCR_Outlet_Temperature");
    ((Control) this.BarInstrument13).Name = "BarInstrument13";
    ((AxisSingleInstrumentBase) this.BarInstrument13).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.BarInstrument13, 4);
    ((SingleInstrumentBase) this.BarInstrument13).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument13).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument13).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument8, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineload");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((AxisSingleInstrumentBase) this.barInstrument8).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument1, "BarInstrument1");
    this.BarInstrument1.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.BarInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.BarInstrument1).Name = "BarInstrument1";
    ((AxisSingleInstrumentBase) this.BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument5, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument5, "BarInstrument5");
    this.BarInstrument5.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.BarInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.BarInstrument5).Name = "BarInstrument5";
    ((AxisSingleInstrumentBase) this.BarInstrument5).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument10, "digitalReadoutInstrument10");
    this.digitalReadoutInstrument10.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS143_ADS_Pump_Speed");
    ((Control) this.digitalReadoutInstrument10).Name = "digitalReadoutInstrument10";
    ((SingleInstrumentBase) this.digitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS101_Nox_conversion_efficiency");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS053_Ambient_Air_Temperature");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument9, "digitalReadoutInstrument9");
    this.digitalReadoutInstrument9.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS036_SCR_Inlet_NOx_Sensor");
    ((Control) this.digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS035_SCR_Outlet_NOx_Sensor");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmarkStatus;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) this.textBoxProgress;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent1.StopAllButton = this.buttonStop;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SCRSystem");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableStatusIndicators).ResumeLayout(false);
    ((Control) this.tableStatusIndicators).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
