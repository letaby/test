// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MY13_.panel.UserPanel
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string HPLeakActualValueQualifier = "DT_AS115_HP_Leak_Actual_Value";
  private const string HPLeakLearnedCounterQualifier = "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter";
  private const string HPLeakLearnedValueQualifier = "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value";
  private SharedProcedureBase selectedProcedure;
  private Channel mcm;
  private Instrument hpLeakActualValue;
  private EcuInfo hpLeakLearnedCounter;
  private EcuInfo hpLeakLearnedValue;
  private bool adrResult = false;
  private string adrMessage = Resources.Message_Test_Not_Run;
  private bool userCanceled = false;
  private SharedProcedureSelection sharedProcedureSelection1;
  private Button buttonStart;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private Checkmark checkmark1;
  private System.Windows.Forms.Label label1;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakLearnedValue;
  private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakLearnedCounter;
  private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakCounter;
  private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakActualValue;
  private DigitalReadoutInstrument digitalReadoutInstrument11;
  private DigitalReadoutInstrument digitalReadoutInstrument12;
  private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private SeekTimeListView seekTimeListView1;
  private ChartInstrument chartInstrument1;
  private TableLayoutPanel tableLayoutPanel2;
  private TableLayoutPanel tableLayoutPanel1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.SubscribeToEvents(this.sharedProcedureSelection1.SelectedProcedure);
    this.sharedProcedureSelection1.SelectionChanged += new EventHandler(this.sharedProcedureSelection1_SelectionChanged);
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
  }

  private void sharedProcedureSelection1_SelectionChanged(object sender, EventArgs e)
  {
    this.SubscribeToEvents(this.sharedProcedureSelection1.SelectedProcedure);
  }

  private void SubscribeToEvents(SharedProcedureBase procedure)
  {
    if (procedure == this.selectedProcedure)
      return;
    if (this.selectedProcedure != null)
      this.selectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StopComplete);
    this.selectedProcedure = procedure;
    if (this.selectedProcedure != null)
      this.selectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StopComplete);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM21T", (CustomPanel.ChannelLookupOptions) 1));
  }

  private void SetMCM(Channel channel)
  {
    if (this.mcm == channel)
      return;
    this.mcm = channel;
    if (this.mcm != null)
    {
      this.hpLeakActualValue = this.mcm.Instruments["DT_AS115_HP_Leak_Actual_Value"];
      this.hpLeakLearnedCounter = this.mcm.EcuInfos["DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter"];
      this.hpLeakLearnedValue = this.mcm.EcuInfos["DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"];
    }
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    if (this.sharedProcedureSelection1.SelectedProcedure.State == 3 || this.sharedProcedureSelection1.SelectedProcedure.State == 0)
      this.userCanceled = true;
    else
      this.userCanceled = false;
  }

  private static object GetInstrumentCurrentValue(Instrument instrument)
  {
    object instrumentCurrentValue = (object) null;
    if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
      instrumentCurrentValue = instrument.InstrumentValues.Current.Value;
    return instrumentCurrentValue;
  }

  private static double InstrumentToDouble(Instrument instrument)
  {
    double num = double.NaN;
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(instrument);
    if (instrumentCurrentValue != null)
      num = Convert.ToDouble(instrumentCurrentValue, (IFormatProvider) CultureInfo.InvariantCulture);
    return num;
  }

  private static double StoredDataToDouble(EcuInfo ecuInfo)
  {
    double num = double.NaN;
    if (ecuInfo != null && !string.IsNullOrEmpty(ecuInfo.Value))
      num = Convert.ToDouble(ecuInfo.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    return num;
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private string EvaluateHPLeakParameters()
  {
    double num1 = UserPanel.InstrumentToDouble(this.hpLeakActualValue);
    double num2 = UserPanel.StoredDataToDouble(this.hpLeakLearnedCounter);
    double num3 = UserPanel.StoredDataToDouble(this.hpLeakLearnedValue);
    string empty = string.Empty;
    return !num1.Equals(double.NaN) && !num2.Equals(double.NaN) && !num3.Equals(double.NaN) ? (num2 < 10.0 ? (num1 >= 0.5 && num1 <= 2.25 ? Resources.Message_System_Not_Leaking : Resources.Message_System_Leaking) : (num1 <= 2.5 ? (num1 - num3 <= 1.5 ? Resources.Message_System_Not_Leaking : Resources.Message_System_Leaking) : Resources.Message_System_Leaking)) : Resources.Message_Error_Reading_Values;
  }

  private void SelectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    if (this.sharedProcedureSelection1.SelectedProcedure.Result == 1)
    {
      if (string.Equals(this.sharedProcedureSelection1.SelectedProcedure.Name, "FSIC Fuel Filter Pressure Check", StringComparison.Ordinal))
      {
        this.adrMessage = "Fuel Filter Test complete.";
        this.adrResult = true;
      }
      else
      {
        this.adrMessage = this.EvaluateHPLeakParameters();
        this.adrResult = this.adrMessage.Equals(Resources.Message_System_Not_Leaking, StringComparison.Ordinal);
      }
    }
    else
    {
      this.adrMessage = this.userCanceled ? Resources.Message_Selected_Procedure_Canceled : Resources.Message_Selected_Procedure_Aborted;
      this.adrResult = false;
    }
    this.LogText(this.adrMessage);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    if (this.buttonStart != null)
      this.buttonStart.Click -= new EventHandler(this.buttonStart_Click);
    this.SubscribeToEvents((SharedProcedureBase) null);
    this.SetMCM((Channel) null);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrResult,
      (object) this.adrMessage
    };
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.chartInstrument1 = new ChartInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHPLeakActualValue = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHPLeakCounter = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHPLeakLearnedCounter = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHPLeakLearnedValue = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument11 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument12 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.label1 = new System.Windows.Forms.Label();
    this.checkmark1 = new Checkmark();
    this.buttonStart = new Button();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.chartInstrument1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentHPLeakActualValue, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentHPLeakCounter, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentHPLeakLearnedCounter, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentHPLeakLearnedValue, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument11, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument12, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCoolantTemperature, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 7);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "FSIC";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView1, 3);
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.chartInstrument1, "chartInstrument1");
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS098_desired_rail_pressure"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS001_KW_NW_validity_signal"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS013_Coolant_Temperature"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS014_Fuel_Temperature"));
    ((Control) this.chartInstrument1).Name = "chartInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.chartInstrument1, 4);
    this.chartInstrument1.SelectedTime = new DateTime?();
    this.chartInstrument1.ShowButtonPanel = false;
    this.chartInstrument1.ShowEvents = false;
    this.chartInstrument1.ShowLabels = false;
    this.chartInstrument1.ShowLegend = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS098_desired_rail_pressure");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHPLeakActualValue, "digitalReadoutInstrumentHPLeakActualValue");
    this.digitalReadoutInstrumentHPLeakActualValue.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakActualValue).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakActualValue).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS115_HP_Leak_Actual_Value");
    ((Control) this.digitalReadoutInstrumentHPLeakActualValue).Name = "digitalReadoutInstrumentHPLeakActualValue";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakActualValue).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHPLeakCounter, "digitalReadoutInstrumentHPLeakCounter");
    this.digitalReadoutInstrumentHPLeakCounter.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakCounter).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakCounter).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Counter");
    ((Control) this.digitalReadoutInstrumentHPLeakCounter).Name = "digitalReadoutInstrumentHPLeakCounter";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakCounter).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHPLeakLearnedCounter, "digitalReadoutInstrumentHPLeakLearnedCounter");
    this.digitalReadoutInstrumentHPLeakLearnedCounter.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakLearnedCounter).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakLearnedCounter).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter");
    ((Control) this.digitalReadoutInstrumentHPLeakLearnedCounter).Name = "digitalReadoutInstrumentHPLeakLearnedCounter";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakLearnedCounter).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHPLeakLearnedValue, "digitalReadoutInstrumentHPLeakLearnedValue");
    this.digitalReadoutInstrumentHPLeakLearnedValue.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakLearnedValue).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakLearnedValue).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value");
    ((Control) this.digitalReadoutInstrumentHPLeakLearnedValue).Name = "digitalReadoutInstrumentHPLeakLearnedValue";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHPLeakLearnedValue).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument11, "digitalReadoutInstrument11");
    this.digitalReadoutInstrument11.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass");
    ((Control) this.digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument12, "digitalReadoutInstrument12");
    this.digitalReadoutInstrument12.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS001_KW_NW_validity_signal");
    ((Control) this.digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    this.digitalReadoutInstrument6.Gradient.Initialize((ValueState) 3, 1, "°C");
    this.digitalReadoutInstrument6.Gradient.Modify(1, 10.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
    this.digitalReadoutInstrumentCoolantTemperature.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState) 3, 1, "°C");
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Modify(1, 65.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStart, 3, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[3]
    {
      "SP_FuelSystemIntegrityCheck_Automatic_MY13",
      "SP_FuelSystemIntegrityCheck_LeakTest_MY13",
      "SP_FuelSystemIntegrityCheck_Manual_MY13"
    });
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.label1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_FuelSystemIntegrityCheck");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
