// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HC_Doser__EPA10_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HC_Doser__EPA10_.panel;

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
  private DigitalReadoutInstrument DigitalReadoutInstrument3;
  private DigitalReadoutInstrument DigitalReadoutInstrument6;
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private TextBox textBoxProgress;
  private BarInstrument barInstrument3;
  private BarInstrument barInstrument8;
  private BarInstrument barDoserFuelLineGaugePressure;
  private BarInstrument barFuelCompensationGaugePressure;
  private Button buttonClose;
  private RunSharedProcedureButton hcDoserButton;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.InitFuelCutOffValveControls();
    ((RunSharedProcedureButtonBase) this.hcDoserButton).ProgressReport += new EventHandler<ProgressReportEventArgs>(this.OnProgressReport);
  }

  private void OnProgressReport(object sender, ProgressReportEventArgs e)
  {
    this.textBoxProgress.AppendText($"{DateTime.Now.ToShortTimeString()} {sender.ToString()}: {e.Message}\r\n");
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && ((RunSharedProcedureButtonBase) this.hcDoserButton).InProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) (((RunSharedProcedureButtonBase) this.hcDoserButton).Result == 1),
      (object) this.textBoxProgress.Text
    };
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
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.BarInstrument9 = new BarInstrument();
    this.BarInstrument10 = new BarInstrument();
    this.BarInstrument11 = new BarInstrument();
    this.BarInstrument12 = new BarInstrument();
    this.BarInstrument13 = new BarInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
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
    this.textBoxProgress = new TextBox();
    this.barInstrument3 = new BarInstrument();
    this.barInstrument8 = new BarInstrument();
    this.barFuelCompensationGaugePressure = new BarInstrument();
    this.barDoserFuelLineGaugePressure = new BarInstrument();
    this.buttonClose = new Button();
    this.hcDoserButton = new RunSharedProcedureButton();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument6, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument3, 1, 1);
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
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxProgress, 2, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument3, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument8, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barFuelCompensationGaugePressure, 4, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barDoserFuelLineGaugePressure, 4, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 1, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.hcDoserButton, 0, 11);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
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
    this.BarInstrument9.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.BarInstrument9).Name = "BarInstrument9";
    ((SingleInstrumentBase) this.BarInstrument9).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument9).UnitAlignment = StringAlignment.Near;
    this.BarInstrument10.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument10.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument10, "BarInstrument10");
    this.BarInstrument10.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.BarInstrument10).Name = "BarInstrument10";
    ((SingleInstrumentBase) this.BarInstrument10).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument10).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument10).UnitAlignment = StringAlignment.Near;
    this.BarInstrument11.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument11.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument11, "BarInstrument11");
    this.BarInstrument11.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument11).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS009_DPF_Oultlet_Temperature");
    ((Control) this.BarInstrument11).Name = "BarInstrument11";
    ((SingleInstrumentBase) this.BarInstrument11).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument11).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument11).UnitAlignment = StringAlignment.Near;
    this.BarInstrument12.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument12.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument12, "BarInstrument12");
    this.BarInstrument12.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument12).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument12).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS013_Coolant_Temperature");
    ((Control) this.BarInstrument12).Name = "BarInstrument12";
    ((SingleInstrumentBase) this.BarInstrument12).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument12).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument12).UnitAlignment = StringAlignment.Near;
    this.BarInstrument13.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrument13.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrument13, "BarInstrument13");
    this.BarInstrument13.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument13).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument13).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS017_Inlet_Manifold_Temperature");
    ((Control) this.BarInstrument13).Name = "BarInstrument13";
    ((SingleInstrumentBase) this.BarInstrument13).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrument13).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrument13).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS010_Engine_Speed");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS012_Vehicle_Speed");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument6, "DigitalReadoutInstrument6");
    this.DigitalReadoutInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS022_Active_Governor_Type");
    ((Control) this.DigitalReadoutInstrument6).Name = "DigitalReadoutInstrument6";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
    this.DigitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS023_Engine_State");
    ((Control) this.DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument1, "BarInstrument1");
    this.BarInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "CPC02T", "DT_AS005_Accelerator_Pedal_Position");
    ((Control) this.BarInstrument1).Name = "BarInstrument1";
    ((SingleInstrumentBase) this.BarInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument5, "DigitalReadoutInstrument5");
    this.DigitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS019_Barometric_Pressure");
    ((Control) this.DigitalReadoutInstrument5).Name = "DigitalReadoutInstrument5";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument2, "BarInstrument2");
    this.BarInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS018_Inlet_Manifold_Pressure");
    ((Control) this.BarInstrument2).Name = "BarInstrument2";
    ((SingleInstrumentBase) this.BarInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
    this.DigitalReadoutInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS071_Smoke_Control_Status");
    ((Control) this.DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument4, "BarInstrument4");
    this.BarInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS033_Throttle_Valve_Commanded_Value");
    ((Control) this.BarInstrument4).Name = "BarInstrument4";
    ((SingleInstrumentBase) this.BarInstrument4).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument5, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument5, "BarInstrument5");
    this.BarInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.BarInstrument5).Name = "BarInstrument5";
    ((SingleInstrumentBase) this.BarInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument7, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument7, "BarInstrument7");
    this.BarInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS005_DOC_Inlet_Pressure");
    ((Control) this.BarInstrument7).Name = "BarInstrument7";
    ((SingleInstrumentBase) this.BarInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument6, "BarInstrument6");
    this.BarInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure");
    ((Control) this.BarInstrument6).Name = "BarInstrument6";
    ((SingleInstrumentBase) this.BarInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS077_Fuel_Cut_Off_Valve");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.BarInstrument16, "BarInstrument16");
    this.BarInstrument16.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument16).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument16).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS035_Fuel_Doser_Injection_Status");
    ((Control) this.BarInstrument16).Name = "BarInstrument16";
    ((SingleInstrumentBase) this.BarInstrument16).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barFuelPressure, "barFuelPressure");
    this.barFuelPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barFuelPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barFuelPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure");
    ((Control) this.barFuelPressure).Name = "barFuelPressure";
    ((SingleInstrumentBase) this.barFuelPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barFuelPressureAtDoser, "barFuelPressureAtDoser");
    this.barFuelPressureAtDoser.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).FreezeValue = false;
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS038_Doser_Fuel_Line_Pressure");
    ((Control) this.barFuelPressureAtDoser).Name = "barFuelPressureAtDoser";
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
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS065_Actual_DPF_zone"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS064_DPF_Regen_State"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS119_Regeneration_Time"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS120_DPF_Target_Temperature"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS111_DOC_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS112_DOC_Out_Model_Delay_Non_fueling")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Compressor", new Qualifier[3]
    {
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS055_Temperature_Compressor_In"),
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS058_Temperature_Compressor_Out"),
      new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS056_Pressure_Compressor_Out")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Engine Brake", new Qualifier[3]
    {
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS003_Engine_Brake_Disable"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS003_Engine_Brake_Low"),
      new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS003_Engine_Brake_Medium")
    }));
    ((Control) this.ListInstrument1).Name = "ListInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.ListInstrument1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxProgress, 3);
    componentResourceManager.ApplyResources((object) this.textBoxProgress, "textBoxProgress");
    this.textBoxProgress.Name = "textBoxProgress";
    this.textBoxProgress.ReadOnly = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.textBoxProgress, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument3, "barInstrument3");
    this.barInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument3).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressureEPA10");
    ((Control) this.barInstrument3).Name = "barInstrument3";
    ((SingleInstrumentBase) this.barInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineload");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barFuelCompensationGaugePressure, "barFuelCompensationGaugePressure");
    this.barFuelCompensationGaugePressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressureEPA10");
    ((Control) this.barFuelCompensationGaugePressure).Name = "barFuelCompensationGaugePressure";
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barDoserFuelLineGaugePressure, "barDoserFuelLineGaugePressure");
    this.barDoserFuelLineGaugePressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressureEPA10");
    ((Control) this.barDoserFuelLineGaugePressure).Name = "barDoserFuelLineGaugePressure";
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).UnitAlignment = StringAlignment.Near;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.hcDoserButton, "hcDoserButton");
    ((Control) this.hcDoserButton).Name = "hcDoserButton";
    this.hcDoserButton.Qualifier = "SP_HCDoserPurge_EPA10";
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
