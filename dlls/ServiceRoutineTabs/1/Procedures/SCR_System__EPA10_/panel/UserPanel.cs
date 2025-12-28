// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__EPA10_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private BarInstrument BarInstrument13;
  private BarInstrument BarInstrument12;
  private BarInstrument BarInstrument11;
  private BarInstrument BarInstrument10;
  private BarInstrument BarInstrument9;
  private ListInstrument ListInstrument1;
  private BarInstrument BarInstrument6;
  private BarInstrument BarInstrument7;
  private BarInstrument BarInstrument5;
  private BarInstrument BarInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument3;
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanelThermometers;
  private TextBox textBoxProgress;
  private Button buttonStart;
  private Button buttonStop;
  private BarInstrument barInstrument8;
  private BarInstrument barInstrument2;
  private BarInstrument barInstrument3;
  private BarInstrument barInstrument4;
  private BarInstrument barInstrument14;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument7;
  private DigitalReadoutInstrument digitalReadoutInstrument8;
  private DigitalReadoutInstrument digitalReadoutInstrument9;
  private TableLayoutPanel tableLayoutPanelStartandStopButtons;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private Checkmark checkmarkStatusIndicator;
  private Label labelStatusMessage;
  private TableLayoutPanel tableLayoutPanelOutput;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;

  public UserPanel() => this.InitializeComponent();

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.ListInstrument1 = new ListInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.BarInstrument1 = new BarInstrument();
    this.BarInstrument7 = new BarInstrument();
    this.BarInstrument6 = new BarInstrument();
    this.tableLayoutPanelThermometers = new TableLayoutPanel();
    this.BarInstrument9 = new BarInstrument();
    this.BarInstrument10 = new BarInstrument();
    this.BarInstrument11 = new BarInstrument();
    this.BarInstrument12 = new BarInstrument();
    this.BarInstrument13 = new BarInstrument();
    this.barInstrument4 = new BarInstrument();
    this.barInstrument14 = new BarInstrument();
    this.BarInstrument5 = new BarInstrument();
    this.barInstrument2 = new BarInstrument();
    this.barInstrument8 = new BarInstrument();
    this.barInstrument3 = new BarInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument9 = new DigitalReadoutInstrument();
    this.tableLayoutPanelStartandStopButtons = new TableLayoutPanel();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.tableLayoutPanelOutput = new TableLayoutPanel();
    this.checkmarkStatusIndicator = new Checkmark();
    this.textBoxProgress = new TextBox();
    this.labelStatusMessage = new Label();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanelThermometers).SuspendLayout();
    ((Control) this.tableLayoutPanelStartandStopButtons).SuspendLayout();
    ((Control) this.tableLayoutPanelOutput).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ListInstrument1, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument7, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument6, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelThermometers, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument5, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument2, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument8, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument3, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument9, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelStartandStopButtons, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument7, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument8, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument3, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelOutput, 3, 7);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.ListInstrument1, 4);
    componentResourceManager.ApplyResources((object) this.ListInstrument1, "ListInstrument1");
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Switches", new Qualifier[5]
    {
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS001_Clutch_Open"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS001_Parking_Brake"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS006_Neutral_Switch"),
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Heaters", new Qualifier[6]
    {
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_1"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_2"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_3"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_4"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS005_Coolant_Valve"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS008_Diffuser_Heater")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("SCR Model Temperatures", new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS117_SCR_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS118_SCR_Heat_Generation")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Zone/Level", new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS024_DEF_Tank_Level"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS065_Actual_DPF_zone")
    }));
    ((Control) this.ListInstrument1).Name = "ListInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.ListInstrument1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.DigitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument1, "BarInstrument1");
    this.BarInstrument1.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.BarInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.BarInstrument1).Name = "BarInstrument1";
    ((AxisSingleInstrumentBase) this.BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument7, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument7, "BarInstrument7");
    this.BarInstrument7.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.BarInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure");
    ((Control) this.BarInstrument7).Name = "BarInstrument7";
    ((SingleInstrumentBase) this.BarInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument6, "BarInstrument6");
    this.BarInstrument6.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.BarInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
    ((Control) this.BarInstrument6).Name = "BarInstrument6";
    ((SingleInstrumentBase) this.BarInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelThermometers, "tableLayoutPanelThermometers");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelThermometers, 4);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.BarInstrument9, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.BarInstrument10, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.BarInstrument11, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.BarInstrument12, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.BarInstrument13, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.barInstrument4, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.barInstrument14, 6, 0);
    ((Control) this.tableLayoutPanelThermometers).Name = "tableLayoutPanelThermometers";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanelThermometers, 5);
    this.BarInstrument9.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument9.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument9, "BarInstrument9");
    this.BarInstrument9.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.BarInstrument9).Name = "BarInstrument9";
    ((AxisSingleInstrumentBase) this.BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument9).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument9).UnitAlignment = StringAlignment.Near;
    this.BarInstrument10.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument10.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument10, "BarInstrument10");
    this.BarInstrument10.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.BarInstrument10).Name = "BarInstrument10";
    ((AxisSingleInstrumentBase) this.BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument10).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument10).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument10).UnitAlignment = StringAlignment.Near;
    this.BarInstrument11.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument11.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument11, "BarInstrument11");
    this.BarInstrument11.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument11).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature");
    ((Control) this.BarInstrument11).Name = "BarInstrument11";
    ((AxisSingleInstrumentBase) this.BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument11).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument11).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument11).UnitAlignment = StringAlignment.Near;
    this.BarInstrument12.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument12.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument12, "BarInstrument12");
    this.BarInstrument12.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument12).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument12).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature");
    ((Control) this.BarInstrument12).Name = "BarInstrument12";
    ((AxisSingleInstrumentBase) this.BarInstrument12).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument12).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument12).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument12).UnitAlignment = StringAlignment.Near;
    this.BarInstrument13.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument13.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument13, "BarInstrument13");
    this.BarInstrument13.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.BarInstrument13).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument13).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature");
    ((Control) this.BarInstrument13).Name = "BarInstrument13";
    ((AxisSingleInstrumentBase) this.BarInstrument13).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument13).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument13).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument13).UnitAlignment = StringAlignment.Near;
    this.barInstrument4.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument4.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument4, "barInstrument4");
    this.barInstrument4.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.barInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS021_DEF_Temperature");
    ((Control) this.barInstrument4).Name = "barInstrument4";
    ((AxisSingleInstrumentBase) this.barInstrument4).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((SingleInstrumentBase) this.barInstrument4).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument4).UnitAlignment = StringAlignment.Near;
    this.barInstrument14.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument14.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument14, "barInstrument14");
    this.barInstrument14.FontGroup = "SCRSystem_Thermometers";
    ((SingleInstrumentBase) this.barInstrument14).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument14).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS022_DEF_tank_Temperature");
    ((Control) this.barInstrument14).Name = "barInstrument14";
    ((AxisSingleInstrumentBase) this.barInstrument14).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((SingleInstrumentBase) this.barInstrument14).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument14).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument14).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument5, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument5, "BarInstrument5");
    this.BarInstrument5.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.BarInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.BarInstrument5).Name = "BarInstrument5";
    ((AxisSingleInstrumentBase) this.BarInstrument5).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletPressure");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument8, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineload");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((AxisSingleInstrumentBase) this.barInstrument8).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument3, "barInstrument3");
    this.barInstrument3.FontGroup = "SCRSystem HorizontalBars";
    ((SingleInstrumentBase) this.barInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure");
    ((Control) this.barInstrument3).Name = "barInstrument3";
    ((AxisSingleInstrumentBase) this.barInstrument3).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.barInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument9, "digitalReadoutInstrument9");
    this.digitalReadoutInstrument9.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS036_SCR_Inlet_NOx_Sensor");
    ((Control) this.digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelStartandStopButtons, "tableLayoutPanelStartandStopButtons");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelStartandStopButtons, 3);
    ((TableLayoutPanel) this.tableLayoutPanelStartandStopButtons).Controls.Add((Control) this.buttonStart, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelStartandStopButtons).Controls.Add((Control) this.buttonStop, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelStartandStopButtons).Controls.Add((Control) this.sharedProcedureSelection1, 0, 0);
    ((Control) this.tableLayoutPanelStartandStopButtons).Name = "tableLayoutPanelStartandStopButtons";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.ForeColor = SystemColors.ControlText;
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanelStartandStopButtons).SetColumnSpan((Control) this.sharedProcedureSelection1, 2);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[3]
    {
      "SP_ChassisDynoBasicScrConversionCheck_EPA10",
      "SP_OutputComponentTest_EPA10",
      "SP_ParkedScrEfficiencyTest_EPA10"
    });
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS035_SCR_Outlet_NOx_Sensor");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS101_Nox_conversion_efficiency");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS053_Ambient_Air_Temperature");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
    this.DigitalReadoutInstrument3.FontGroup = "SCRSystem Readouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS019_Barometric_Pressure");
    ((Control) this.DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelOutput, "tableLayoutPanelOutput");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelOutput, 4);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).Controls.Add((Control) this.checkmarkStatusIndicator, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).Controls.Add((Control) this.textBoxProgress, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).Controls.Add((Control) this.labelStatusMessage, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelOutput).Name = "tableLayoutPanelOutput";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanelOutput, 2);
    componentResourceManager.ApplyResources((object) this.checkmarkStatusIndicator, "checkmarkStatusIndicator");
    ((Control) this.checkmarkStatusIndicator).Name = "checkmarkStatusIndicator";
    ((TableLayoutPanel) this.tableLayoutPanelOutput).SetColumnSpan((Control) this.textBoxProgress, 2);
    componentResourceManager.ApplyResources((object) this.textBoxProgress, "textBoxProgress");
    this.textBoxProgress.Name = "textBoxProgress";
    this.textBoxProgress.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.labelStatusMessage, "labelStatusMessage");
    this.labelStatusMessage.Name = "labelStatusMessage";
    this.labelStatusMessage.UseCompatibleTextRendering = true;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatusMessage;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmarkStatusIndicator;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) this.textBoxProgress;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent1.StopAllButton = this.buttonStop;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SCRSystem");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanelThermometers).ResumeLayout(false);
    ((Control) this.tableLayoutPanelStartandStopButtons).ResumeLayout(false);
    ((Control) this.tableLayoutPanelStartandStopButtons).PerformLayout();
    ((Control) this.tableLayoutPanelOutput).ResumeLayout(false);
    ((Control) this.tableLayoutPanelOutput).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
