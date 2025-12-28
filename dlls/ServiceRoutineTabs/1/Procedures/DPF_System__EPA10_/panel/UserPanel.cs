// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__EPA10_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private const string FuelCutOffValveQualifier = "DT_AS077_Fuel_Cut_Off_Valve";
  private Instrument fuelCutOffValve = (Instrument) null;
  private BarInstrument BarInstrument13;
  private BarInstrument BarInstrument12;
  private BarInstrument BarInstrument11;
  private BarInstrument BarInstrument10;
  private BarInstrument BarInstrument9;
  private ListInstrument ListInstrument1;
  private BarInstrument barFuelPressureAtDoser;
  private BarInstrument barFuelPressure;
  private BarInstrument BarInstrument16;
  private DigitalReadoutInstrument DigitalReadoutInstrument4;
  private BarInstrument BarInstrument6;
  private BarInstrument BarInstrument7;
  private BarInstrument BarInstrument5;
  private BarInstrument BarInstrument4;
  private DigitalReadoutInstrument DigitalReadoutInstrument7;
  private BarInstrument BarInstrument2;
  private DigitalReadoutInstrument DigitalReadoutInstrument5;
  private BarInstrument BarInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument6;
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonStart;
  private Button buttonStop;
  private BarInstrument barInstrument8;
  private BarInstrument barDoserFuelLineGaugePressure;
  private BarInstrument barFuelCompensationGaugePressure;
  private DigitalReadoutInstrument digitalReadoutInstrument8;
  private SharedProcedureSelection sharedProcedureSelection;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private TableLayoutPanel tableLayoutPanel3;
  private System.Windows.Forms.Label statusLabel;
  private Checkmark checkmarkStatus;
  private TextBox textBoxProgress;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.InitFuelCutOffValveControls();
  }

  public virtual void OnChannelsChanged() => this.UpdateFuelCutOffValve();

  private void InitFuelCutOffValveControls() => this.SetFuelPressuresVisibility(false);

  private void UpdateFuelCutOffValve()
  {
    if (!UserPanel.UpdateInstrumentReference("MCM02T", "DT_AS077_Fuel_Cut_Off_Valve", ref this.fuelCutOffValve, new InstrumentUpdateEventHandler(this.OnFuelCutOffValveDataChanged)))
      return;
    this.UpdateFuelCutOffValveAffectedValues();
  }

  private void OnFuelCutOffValveDataChanged(object sender, ResultEventArgs e)
  {
    this.UpdateFuelCutOffValveAffectedValues();
  }

  private void UpdateFuelCutOffValveAffectedValues()
  {
    bool show = false;
    if (this.fuelCutOffValve != (Instrument) null)
    {
      double d = UserPanel.InstrumentToDouble(this.fuelCutOffValve.InstrumentValues.Current, this.fuelCutOffValve.Units);
      if (!double.IsNaN(d) && d == 100.0)
        show = true;
    }
    this.SetFuelPressuresVisibility(show);
  }

  private static Channel GetActiveChannel(string ecuName)
  {
    Channel activeChannel1 = (Channel) null;
    if (!string.IsNullOrEmpty(ecuName) && SapiManager.GlobalInstance != null)
    {
      foreach (Channel activeChannel2 in SapiManager.GlobalInstance.ActiveChannels)
      {
        if (activeChannel2.Ecu.Name == ecuName)
        {
          activeChannel1 = activeChannel2;
          break;
        }
      }
    }
    return activeChannel1;
  }

  private static bool UpdateInstrumentReference(
    string ecuName,
    string qualifier,
    ref Instrument instrumentVariable,
    InstrumentUpdateEventHandler updateHandler)
  {
    Instrument instrument = (Instrument) null;
    Channel activeChannel = UserPanel.GetActiveChannel(ecuName);
    if (activeChannel != null)
      instrument = activeChannel.Instruments[qualifier];
    bool flag = false;
    if (instrument != instrumentVariable)
    {
      if (instrumentVariable != (Instrument) null && updateHandler != null)
        instrumentVariable.InstrumentUpdateEvent -= updateHandler;
      instrumentVariable = instrument;
      if (instrumentVariable != (Instrument) null && updateHandler != null)
        instrumentVariable.InstrumentUpdateEvent += updateHandler;
      flag = true;
    }
    return flag;
  }

  private static double InstrumentToDouble(InstrumentValue value, string units)
  {
    double num = double.NaN;
    if (value != null)
      num = UserPanel.ObjectToDouble(value.Value, units);
    return num;
  }

  private static double ObjectToDouble(object value, string units)
  {
    double num = double.NaN;
    if (value != null)
    {
      Choice choice = value as Choice;
      if (choice != (object) null)
      {
        num = Convert.ToDouble(choice.RawValue);
      }
      else
      {
        try
        {
          num = Convert.ToDouble(value);
          Conversion conversion = Converter.GlobalInstance.GetConversion(units);
          if (conversion != null)
            num = conversion.Convert(num);
        }
        catch (InvalidCastException ex)
        {
          num = double.NaN;
        }
        catch (FormatException ex)
        {
          num = double.NaN;
        }
      }
    }
    return num;
  }

  private void SetFuelPressuresVisibility(bool show)
  {
    ((Control) this.barFuelPressure).Visible = show;
    ((Control) this.barFuelPressureAtDoser).Visible = show;
    ((Control) this.barDoserFuelLineGaugePressure).Visible = show;
    ((Control) this.barFuelCompensationGaugePressure).Visible = show;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.BarInstrument1 = new BarInstrument();
    this.DigitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.BarInstrument2 = new BarInstrument();
    this.DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.BarInstrument4 = new BarInstrument();
    this.BarInstrument5 = new BarInstrument();
    this.BarInstrument7 = new BarInstrument();
    this.BarInstrument6 = new BarInstrument();
    this.DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.BarInstrument16 = new BarInstrument();
    this.barFuelPressure = new BarInstrument();
    this.barFuelPressureAtDoser = new BarInstrument();
    this.ListInstrument1 = new ListInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.BarInstrument9 = new BarInstrument();
    this.BarInstrument10 = new BarInstrument();
    this.BarInstrument11 = new BarInstrument();
    this.BarInstrument12 = new BarInstrument();
    this.BarInstrument13 = new BarInstrument();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    this.barInstrument8 = new BarInstrument();
    this.barFuelCompensationGaugePressure = new BarInstrument();
    this.barDoserFuelLineGaugePressure = new BarInstrument();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.textBoxProgress = new TextBox();
    this.statusLabel = new System.Windows.Forms.Label();
    this.checkmarkStatus = new Checkmark();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument6, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument5, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument7, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument4, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument5, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument7, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument6, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument4, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument16, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barFuelPressure, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barFuelPressureAtDoser, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ListInstrument1, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 0, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStop, 1, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument8, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barFuelCompensationGaugePressure, 4, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barDoserFuelLineGaugePressure, 4, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument8, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection, 0, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 2, 10);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.DigitalReadoutInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument6, "DigitalReadoutInstrument6");
    this.DigitalReadoutInstrument6.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS022_Active_Governor_Type");
    ((Control) this.DigitalReadoutInstrument6).Name = "DigitalReadoutInstrument6";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument1, "BarInstrument1");
    this.BarInstrument1.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.BarInstrument1).Name = "BarInstrument1";
    ((AxisSingleInstrumentBase) this.BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument5, "DigitalReadoutInstrument5");
    this.DigitalReadoutInstrument5.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS019_Barometric_Pressure");
    ((Control) this.DigitalReadoutInstrument5).Name = "DigitalReadoutInstrument5";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument2, "BarInstrument2");
    this.BarInstrument2.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletPressure");
    ((Control) this.BarInstrument2).Name = "BarInstrument2";
    ((SingleInstrumentBase) this.BarInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
    this.DigitalReadoutInstrument7.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS071_Smoke_Control_Status");
    ((Control) this.DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument4, "BarInstrument4");
    this.BarInstrument4.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS033_Throttle_Valve_Commanded_Value");
    ((Control) this.BarInstrument4).Name = "BarInstrument4";
    ((AxisSingleInstrumentBase) this.BarInstrument4).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument4).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument5, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument5, "BarInstrument5");
    this.BarInstrument5.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.BarInstrument5).Name = "BarInstrument5";
    ((AxisSingleInstrumentBase) this.BarInstrument5).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument7, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument7, "BarInstrument7");
    this.BarInstrument7.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS005_DOC_Inlet_Pressure");
    ((Control) this.BarInstrument7).Name = "BarInstrument7";
    ((AxisSingleInstrumentBase) this.BarInstrument7).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
    ((SingleInstrumentBase) this.BarInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument6, "BarInstrument6");
    this.BarInstrument6.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure");
    ((Control) this.BarInstrument6).Name = "BarInstrument6";
    ((AxisSingleInstrumentBase) this.BarInstrument6).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
    ((SingleInstrumentBase) this.BarInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS077_Fuel_Cut_Off_Valve");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.BarInstrument16, "BarInstrument16");
    this.BarInstrument16.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.BarInstrument16).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument16).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS035_Fuel_Doser_Injection_Status");
    ((Control) this.BarInstrument16).Name = "BarInstrument16";
    ((SingleInstrumentBase) this.BarInstrument16).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barFuelPressure, "barFuelPressure");
    this.barFuelPressure.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.barFuelPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barFuelPressure).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "fuelPressure");
    ((Control) this.barFuelPressure).Name = "barFuelPressure";
    ((AxisSingleInstrumentBase) this.barFuelPressure).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
    ((SingleInstrumentBase) this.barFuelPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barFuelPressureAtDoser, "barFuelPressureAtDoser");
    this.barFuelPressureAtDoser.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).FreezeValue = false;
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS038_Doser_Fuel_Line_Pressure");
    ((Control) this.barFuelPressureAtDoser).Name = "barFuelPressureAtDoser";
    ((AxisSingleInstrumentBase) this.barFuelPressureAtDoser).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.ListInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.ListInstrument1, "ListInstrument1");
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Switches", new Qualifier[5]
    {
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS001_Clutch_Open"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS001_Parking_Brake"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS006_Neutral_Switch"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS008_DPF_Regen_Switch_Status"),
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Regeneration", new Qualifier[6]
    {
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS111_DOC_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS112_DOC_Out_Model_Delay_Non_fueling"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS113_DPF_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS064_DPF_Regen_State"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS065_Actual_DPF_zone"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS120_DPF_Target_Temperature")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Compressor", new Qualifier[3]
    {
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS055_Temperature_Compressor_In"),
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS058_Temperature_Compressor_Out"),
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS056_Pressure_Compressor_Out")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Engine Brake", new Qualifier[1]
    {
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS069_Jake_Brake_1_PWM07")
    }));
    ((Control) this.ListInstrument1).Name = "ListInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.ListInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument9, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument10, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument11, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument12, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument13, 4, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel2, 5);
    this.BarInstrument9.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument9.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument9, "BarInstrument9");
    this.BarInstrument9.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.BarInstrument9).Name = "BarInstrument9";
    ((AxisSingleInstrumentBase) this.BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument9).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument9).UnitAlignment = StringAlignment.Near;
    this.BarInstrument10.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument10.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument10, "BarInstrument10");
    this.BarInstrument10.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.BarInstrument10).Name = "BarInstrument10";
    ((AxisSingleInstrumentBase) this.BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument10).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument10).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument10).UnitAlignment = StringAlignment.Near;
    this.BarInstrument11.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument11.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument11, "BarInstrument11");
    this.BarInstrument11.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrument11).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS009_DPF_Oultlet_Temperature");
    ((Control) this.BarInstrument11).Name = "BarInstrument11";
    ((AxisSingleInstrumentBase) this.BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrument11).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument11).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument11).UnitAlignment = StringAlignment.Near;
    this.BarInstrument12.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument12.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument12, "BarInstrument12");
    this.BarInstrument12.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrument12).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument12).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.BarInstrument12).Name = "BarInstrument12";
    ((AxisSingleInstrumentBase) this.BarInstrument12).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((SingleInstrumentBase) this.BarInstrument12).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument12).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument12).UnitAlignment = StringAlignment.Near;
    this.BarInstrument13.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument13.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument13, "BarInstrument13");
    this.BarInstrument13.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrument13).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument13).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletTemp");
    ((Control) this.BarInstrument13).Name = "BarInstrument13";
    ((AxisSingleInstrumentBase) this.BarInstrument13).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((SingleInstrumentBase) this.BarInstrument13).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument13).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument13).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.ForeColor = SystemColors.ControlText;
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineload");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((AxisSingleInstrumentBase) this.barInstrument8).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barFuelCompensationGaugePressure, "barFuelCompensationGaugePressure");
    this.barFuelCompensationGaugePressure.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressureEPA10");
    ((Control) this.barFuelCompensationGaugePressure).Name = "barFuelCompensationGaugePressure";
    ((AxisSingleInstrumentBase) this.barFuelCompensationGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barDoserFuelLineGaugePressure, "barDoserFuelLineGaugePressure");
    this.barDoserFuelLineGaugePressure.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressureEPA10");
    ((Control) this.barDoserFuelLineGaugePressure).Name = "barDoserFuelLineGaugePressure";
    ((AxisSingleInstrumentBase) this.barDoserFuelLineGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressureEPA10");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.sharedProcedureSelection, 2);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[4]
    {
      "SP_HCDoserPurge_EPA10",
      "SP_OverTheRoadRegen_EPA10",
      "SP_ParkedRegen_EPA10",
      "SP_DisableHcDoserParkedRegen_EPA10"
    });
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.textBoxProgress, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.statusLabel, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).SetColumnSpan((Control) this.textBoxProgress, 3);
    componentResourceManager.ApplyResources((object) this.textBoxProgress, "textBoxProgress");
    this.textBoxProgress.Name = "textBoxProgress";
    this.textBoxProgress.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.statusLabel, "statusLabel");
    this.statusLabel.Name = "statusLabel";
    this.statusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.statusLabel;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmarkStatus;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) this.textBoxProgress;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = this.buttonStop;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DPFSystem");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
