// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__MY13_.panel.UserPanel
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
using System.Windows.Forms.Layout;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string FuelCutOffValveQualifier = "DT_AS077_Fuel_Cut_Off_Valve";
  private Channel cpc;
  private Channel mcm;
  private Channel acm;
  private Instrument fuelCutOffValve = (Instrument) null;
  private BarInstrument BarInstrumentEngineIntakeTemperature;
  private BarInstrument BarInstrument12;
  private BarInstrument BarInstrumentDPFOutletTemperature;
  private BarInstrument BarInstrumentDOCOutletTemperature;
  private BarInstrument BarInstrumentDOCInletTemperature;
  private ListInstrument ListInstrument1;
  private BarInstrument barFuelPressureAtDoser;
  private BarInstrument barFuelPressure;
  private BarInstrument BarInstrumentDoserStatus;
  private DigitalReadoutInstrument DigitalReadoutInstrumentFuelCutOffValve;
  private BarInstrument BarInstrumentDPFOutletPressure;
  private BarInstrument BarInstrumentDOCInletPressure;
  private BarInstrument BarInstrumentThrottleValve;
  private BarInstrument BarInstrumentThrottleControl;
  private DigitalReadoutInstrument DigitalReadoutInstrumentSmokeControlStatus;
  private BarInstrument BarInstrument2;
  private DigitalReadoutInstrument DigitalReadoutInstrumentBarametricPressure;
  private BarInstrument BarInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrumentGovernerType;
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

  public virtual void OnChannelsChanged() => this.UpdateInstruments();

  private void InitFuelCutOffValveControls() => this.SetFuelPressuresVisibility(false);

  private void UpdateInstruments()
  {
    bool flag = this.SetCpc(this.GetChannel("CPC302T", (CustomPanel.ChannelLookupOptions) 7)) | this.SetMcm(this.GetChannel("MCM21T", (CustomPanel.ChannelLookupOptions) 7));
    this.SetAcm(this.GetChannel("ACM21T", (CustomPanel.ChannelLookupOptions) 7));
    if (!flag)
      return;
    this.SetSharedProcedureQualifiers();
  }

  private bool SetCpc(Channel cpc)
  {
    bool flag = this.cpc != cpc;
    if (flag)
      this.cpc = cpc;
    return flag;
  }

  private bool SetMcm(Channel mcm)
  {
    bool flag = this.mcm != mcm;
    if (flag)
    {
      this.mcm = mcm;
      if (this.mcm != null)
      {
        if (UserPanel.UpdateInstrumentReference(this.mcm.Ecu.Name, "DT_AS077_Fuel_Cut_Off_Valve", ref this.fuelCutOffValve, new InstrumentUpdateEventHandler(this.OnFuelCutOffValveDataChanged)))
          this.UpdateFuelCutOffValveAffectedValues();
        this.SetFakeInstrumentQualifiers(mcm);
        this.SetInstrumentQualifiers((Control) this, "MCM", mcm);
      }
    }
    return flag;
  }

  private void SetAcm(Channel acm)
  {
    if (this.acm == acm)
      return;
    this.acm = acm;
    if (this.acm != null)
      this.SetInstrumentQualifiers((Control) this, "ACM", acm);
  }

  private void SetSharedProcedureQualifiers()
  {
    string str = "SP_HCDoserPurge_MY13";
    if (this.mcm != null && string.Equals("MCM30T", this.mcm.Ecu.Name, StringComparison.OrdinalIgnoreCase))
      str = "SP_HCDoserPurge_MY25";
    if (this.cpc != null && string.Equals("CPC302T", this.cpc.Ecu.Name, StringComparison.OrdinalIgnoreCase))
    {
      this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[4]
      {
        str,
        "SP_OverTheRoadRegen_NGC",
        "SP_ParkedRegen_NGC",
        "SP_DisableHcDoserParkedRegen_NGC"
      });
      ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_105");
    }
    else if (this.cpc != null && string.Equals("CPC501T", this.cpc.Ecu.Name, StringComparison.OrdinalIgnoreCase))
    {
      this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[4]
      {
        str,
        "SP_OverTheRoadRegen_CPC5",
        "SP_ParkedRegen_CPC5",
        "SP_DisableHcDoserParkedRegen_CPC5"
      });
      ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_105");
    }
    else if (this.cpc != null && string.Equals("CPC502T", this.cpc.Ecu.Name, StringComparison.OrdinalIgnoreCase))
    {
      this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[4]
      {
        str,
        "SP_OverTheRoadRegen_MY25",
        "SP_ParkedRegen_MY25",
        "SP_DisableHcDoserParkedRegen_MY25"
      });
      ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_105");
    }
    else
    {
      this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[4]
      {
        str,
        "SP_OverTheRoadRegen_MY13",
        "SP_ParkedRegen_MY13",
        "SP_DisableHcDoserParkedRegen_MY13"
      });
      ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletTemp");
    }
    ((Control) this.BarInstrumentEngineIntakeTemperature).Refresh();
  }

  private void SetFakeInstrumentQualifiers(Channel mcm)
  {
    if (mcm == null)
      return;
    if (string.Equals("MCM30T", mcm.Ecu.Name, StringComparison.OrdinalIgnoreCase))
    {
      ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressureMY25");
      ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressureMY25");
      ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressureMY25");
    }
    else
    {
      ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressureMY13");
      ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressureMY13");
      ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressureMY13");
    }
  }

  private void SetInstrumentQualifiers(Control parent, string channelPrefix, Channel channel)
  {
    if (channel == null)
      return;
    foreach (Control control in (ArrangedElementCollection) parent.Controls)
    {
      int num1;
      switch (control)
      {
        case SingleInstrumentBase singleInstrumentBase1:
          Qualifier instrument1 = singleInstrumentBase1.Instrument;
          Qualifier instrument2 = singleInstrumentBase1.Instrument;
          int num2;
          if (((Qualifier) ref instrument2).Ecu != null)
          {
            instrument2 = singleInstrumentBase1.Instrument;
            if (((Qualifier) ref instrument2).Ecu.StartsWith(channelPrefix, StringComparison.OrdinalIgnoreCase))
            {
              InstrumentCollection instruments = channel.Instruments;
              instrument2 = singleInstrumentBase1.Instrument;
              string name = ((Qualifier) ref instrument2).Name;
              num2 = !(instruments[name] != (Instrument) null) ? 1 : 0;
              goto label_8;
            }
          }
          num2 = 1;
label_8:
          if (num2 == 0)
          {
            SingleInstrumentBase singleInstrumentBase = singleInstrumentBase1;
            instrument2 = singleInstrumentBase1.Instrument;
            QualifierTypes type = ((Qualifier) ref instrument2).Type;
            string name1 = channel.Ecu.Name;
            instrument2 = singleInstrumentBase1.Instrument;
            string name2 = ((Qualifier) ref instrument2).Name;
            Qualifier qualifier = new Qualifier(type, name1, name2);
            singleInstrumentBase.Instrument = qualifier;
            goto label_15;
          }
          goto label_15;
        case TableLayoutPanel _:
        case Panel _:
          num1 = 0;
          break;
        default:
          num1 = !(control is FlowLayoutPanel) ? 1 : 0;
          break;
      }
      if (num1 == 0)
        this.SetInstrumentQualifiers(control, channelPrefix, channel);
label_15:;
    }
    parent.Refresh();
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
    this.DigitalReadoutInstrumentGovernerType = new DigitalReadoutInstrument();
    this.BarInstrument1 = new BarInstrument();
    this.DigitalReadoutInstrumentBarametricPressure = new DigitalReadoutInstrument();
    this.BarInstrument2 = new BarInstrument();
    this.DigitalReadoutInstrumentSmokeControlStatus = new DigitalReadoutInstrument();
    this.BarInstrumentThrottleControl = new BarInstrument();
    this.BarInstrumentThrottleValve = new BarInstrument();
    this.BarInstrumentDOCInletPressure = new BarInstrument();
    this.BarInstrumentDPFOutletPressure = new BarInstrument();
    this.DigitalReadoutInstrumentFuelCutOffValve = new DigitalReadoutInstrument();
    this.BarInstrumentDoserStatus = new BarInstrument();
    this.barFuelPressure = new BarInstrument();
    this.barFuelPressureAtDoser = new BarInstrument();
    this.ListInstrument1 = new ListInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.BarInstrumentDOCInletTemperature = new BarInstrument();
    this.BarInstrumentDOCOutletTemperature = new BarInstrument();
    this.BarInstrumentDPFOutletTemperature = new BarInstrument();
    this.BarInstrument12 = new BarInstrument();
    this.BarInstrumentEngineIntakeTemperature = new BarInstrument();
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
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrumentGovernerType, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrumentBarametricPressure, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrument2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrumentSmokeControlStatus, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrumentThrottleControl, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrumentThrottleValve, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrumentDOCInletPressure, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrumentDPFOutletPressure, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrumentFuelCutOffValve, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.BarInstrumentDoserStatus, 2, 6);
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
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.DigitalReadoutInstrumentGovernerType, 2);
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrumentGovernerType, "DigitalReadoutInstrumentGovernerType");
    this.DigitalReadoutInstrumentGovernerType.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentGovernerType).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentGovernerType).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS022_Active_Governor_Type");
    ((Control) this.DigitalReadoutInstrumentGovernerType).Name = "DigitalReadoutInstrumentGovernerType";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentGovernerType).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument1, "BarInstrument1");
    this.BarInstrument1.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.BarInstrument1).Name = "BarInstrument1";
    ((AxisSingleInstrumentBase) this.BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrumentBarametricPressure, "DigitalReadoutInstrumentBarametricPressure");
    this.DigitalReadoutInstrumentBarametricPressure.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentBarametricPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentBarametricPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS019_Barometric_Pressure");
    ((Control) this.DigitalReadoutInstrumentBarametricPressure).Name = "DigitalReadoutInstrumentBarametricPressure";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentBarametricPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument2, "BarInstrument2");
    this.BarInstrument2.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletPressure");
    ((Control) this.BarInstrument2).Name = "BarInstrument2";
    ((SingleInstrumentBase) this.BarInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrumentSmokeControlStatus, "DigitalReadoutInstrumentSmokeControlStatus");
    this.DigitalReadoutInstrumentSmokeControlStatus.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentSmokeControlStatus).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentSmokeControlStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS071_Smoke_Control_Status");
    ((Control) this.DigitalReadoutInstrumentSmokeControlStatus).Name = "DigitalReadoutInstrumentSmokeControlStatus";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentSmokeControlStatus).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrumentThrottleControl, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrumentThrottleControl, "BarInstrumentThrottleControl");
    this.BarInstrumentThrottleControl.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrumentThrottleControl).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentThrottleControl).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value");
    ((Control) this.BarInstrumentThrottleControl).Name = "BarInstrumentThrottleControl";
    ((AxisSingleInstrumentBase) this.BarInstrumentThrottleControl).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrumentThrottleControl).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrumentThrottleValve, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrumentThrottleValve, "BarInstrumentThrottleValve");
    this.BarInstrumentThrottleValve.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrumentThrottleValve).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentThrottleValve).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.BarInstrumentThrottleValve).Name = "BarInstrumentThrottleValve";
    ((AxisSingleInstrumentBase) this.BarInstrumentThrottleValve).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.BarInstrumentThrottleValve).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrumentDOCInletPressure, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrumentDOCInletPressure, "BarInstrumentDOCInletPressure");
    this.BarInstrumentDOCInletPressure.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrumentDOCInletPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentDOCInletPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure");
    ((Control) this.BarInstrumentDOCInletPressure).Name = "BarInstrumentDOCInletPressure";
    ((AxisSingleInstrumentBase) this.BarInstrumentDOCInletPressure).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
    ((SingleInstrumentBase) this.BarInstrumentDOCInletPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.BarInstrumentDPFOutletPressure, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrumentDPFOutletPressure, "BarInstrumentDPFOutletPressure");
    this.BarInstrumentDPFOutletPressure.FontGroup = "horizontalBarLarge";
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure");
    ((Control) this.BarInstrumentDPFOutletPressure).Name = "BarInstrumentDPFOutletPressure";
    ((AxisSingleInstrumentBase) this.BarInstrumentDPFOutletPressure).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrumentFuelCutOffValve, "DigitalReadoutInstrumentFuelCutOffValve");
    this.DigitalReadoutInstrumentFuelCutOffValve.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentFuelCutOffValve).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentFuelCutOffValve).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS077_Fuel_Cut_Off_Valve");
    ((Control) this.DigitalReadoutInstrumentFuelCutOffValve).Name = "DigitalReadoutInstrumentFuelCutOffValve";
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentFuelCutOffValve).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.BarInstrumentDoserStatus, "BarInstrumentDoserStatus");
    this.BarInstrumentDoserStatus.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.BarInstrumentDoserStatus).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentDoserStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS035_Fuel_Doser_Injection_Status");
    ((Control) this.BarInstrumentDoserStatus).Name = "BarInstrumentDoserStatus";
    ((SingleInstrumentBase) this.BarInstrumentDoserStatus).UnitAlignment = StringAlignment.Near;
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
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS038_Doser_Fuel_Line_Pressure");
    ((Control) this.barFuelPressureAtDoser).Name = "barFuelPressureAtDoser";
    ((AxisSingleInstrumentBase) this.barFuelPressureAtDoser).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
    ((SingleInstrumentBase) this.barFuelPressureAtDoser).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.ListInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.ListInstrument1, "ListInstrument1");
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Switches", new Qualifier[7]
    {
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
      new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
      new Qualifier((QualifierTypes) 1, "virtual", "NeutralSwitch"),
      new Qualifier((QualifierTypes) 1, "virtual", "ClutchSwitch"),
      new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_MSC_GetSwState_033"),
      new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_DS019_Vehicle_Check_Status")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Regeneration", new Qualifier[24]
    {
      new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS122_DOC_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
      new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS125_DPF_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS064_DPF_Regen_State"),
      new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
      new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS120_DPF_Target_Temperature"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS027_Turbo_Speed_1"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS090_Wastegate_return_position"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS122_DOC_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM311T", "DT_AS122_DOC_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM311T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
      new Qualifier((QualifierTypes) 1, "ACM311T", "DT_AS125_DPF_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS125_DPF_Out_Model_Delay"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS064_DPF_Regen_State"),
      new Qualifier((QualifierTypes) 1, "ACM311T", "DT_AS064_DPF_Regen_State"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS065_Actual_DPF_zone"),
      new Qualifier((QualifierTypes) 1, "ACM311T", "DT_AS065_Actual_DPF_zone"),
      new Qualifier((QualifierTypes) 1, "ACM311T", "DT_AS120_DPF_Target_Temperature"),
      new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS120_DPF_Target_Temperature"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_AS032_EGR_Actual_Valve_Position"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_AS027_Turbo_Speed_1"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_AS090_Wastegate_return_position")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Compressor", new Qualifier[4]
    {
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS055_Temperature_Compressor_In"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS058_Temperature_Compressor_Out"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_AS055_Temperature_Compressor_In"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_ASL005_Temperature_Compressor_Out")
    }));
    ((Collection<QualifierGroup>) this.ListInstrument1.Groups).Add(new QualifierGroup("Engine Brake", new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS069_Jake_Brake_1_PWM13"),
      new Qualifier((QualifierTypes) 1, "MCM30T", "DT_AS069_Jake_Brake_1_PWM13")
    }));
    ((Control) this.ListInstrument1).Name = "ListInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.ListInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrumentDOCInletTemperature, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrumentDOCOutletTemperature, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrumentDPFOutletTemperature, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument12, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrumentEngineIntakeTemperature, 4, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel2, 5);
    this.BarInstrumentDOCInletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrumentDOCInletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrumentDOCInletTemperature, "BarInstrumentDOCInletTemperature");
    this.BarInstrumentDOCInletTemperature.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrumentDOCInletTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentDOCInletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.BarInstrumentDOCInletTemperature).Name = "BarInstrumentDOCInletTemperature";
    ((AxisSingleInstrumentBase) this.BarInstrumentDOCInletTemperature).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrumentDOCInletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrumentDOCInletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrumentDOCInletTemperature).UnitAlignment = StringAlignment.Near;
    this.BarInstrumentDOCOutletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrumentDOCOutletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrumentDOCOutletTemperature, "BarInstrumentDOCOutletTemperature");
    this.BarInstrumentDOCOutletTemperature.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrumentDOCOutletTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentDOCOutletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.BarInstrumentDOCOutletTemperature).Name = "BarInstrumentDOCOutletTemperature";
    ((AxisSingleInstrumentBase) this.BarInstrumentDOCOutletTemperature).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrumentDOCOutletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrumentDOCOutletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrumentDOCOutletTemperature).UnitAlignment = StringAlignment.Near;
    this.BarInstrumentDPFOutletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrumentDPFOutletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrumentDPFOutletTemperature, "BarInstrumentDPFOutletTemperature");
    this.BarInstrumentDPFOutletTemperature.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature");
    ((Control) this.BarInstrumentDPFOutletTemperature).Name = "BarInstrumentDPFOutletTemperature";
    ((AxisSingleInstrumentBase) this.BarInstrumentDPFOutletTemperature).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrumentDPFOutletTemperature).UnitAlignment = StringAlignment.Near;
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
    this.BarInstrumentEngineIntakeTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.BarInstrumentEngineIntakeTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.BarInstrumentEngineIntakeTemperature, "BarInstrumentEngineIntakeTemperature");
    this.BarInstrumentEngineIntakeTemperature.FontGroup = "thermometer";
    ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "airInletTemp");
    ((Control) this.BarInstrumentEngineIntakeTemperature).Name = "BarInstrumentEngineIntakeTemperature";
    ((AxisSingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.BarInstrumentEngineIntakeTemperature).UnitAlignment = StringAlignment.Near;
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
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressureMY13");
    ((Control) this.barFuelCompensationGaugePressure).Name = "barFuelCompensationGaugePressure";
    ((AxisSingleInstrumentBase) this.barFuelCompensationGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
    ((SingleInstrumentBase) this.barFuelCompensationGaugePressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barDoserFuelLineGaugePressure, "barDoserFuelLineGaugePressure");
    this.barDoserFuelLineGaugePressure.FontGroup = "horizontalBarSmall";
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressureMY13");
    ((Control) this.barDoserFuelLineGaugePressure).Name = "barDoserFuelLineGaugePressure";
    ((AxisSingleInstrumentBase) this.barDoserFuelLineGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
    ((SingleInstrumentBase) this.barDoserFuelLineGaugePressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = "digitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressureMY13");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.sharedProcedureSelection, 2);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[4]
    {
      "SP_HCDoserPurge_MY13",
      "SP_OverTheRoadRegen_MY13",
      "SP_ParkedRegen_MY13",
      "SP_DisableHcDoserParkedRegen_MY13"
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
