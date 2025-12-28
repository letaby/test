// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__Euro4_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__Euro4_.panel;

public class UserPanel : CustomPanel
{
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private DigitalReadoutInstrument DigitalReadoutInstrument10;
  private DigitalReadoutInstrument DigitalReadoutInstrument7;
  private DigitalReadoutInstrument DigitalReadoutInstrument4;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument43;
  private DigitalReadoutInstrument DigitalReadoutInstrument17a;
  private DigitalReadoutInstrument DigitalReadoutInstrument14a;
  private TableLayoutPanel tableLayoutPanel1;
  private Button start;
  private Button stop;
  private DigitalReadoutInstrument DigitalReadoutInstrument19;
  private TableLayoutPanel tableLayoutPanel2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;

  public UserPanel()
  {
    this.InitializeComponent();
    this.start.Click += new EventHandler(this.OnStartButtonClick);
    this.stop.Click += new EventHandler(this.OnStopButtonClick);
  }

  private void OnStopButtonClick(object sender, EventArgs e) => this.SetMarkedState(false);

  private void OnStartButtonClick(object sender, EventArgs e) => this.SetMarkedState(true);

  private void SetMarkedState(bool set)
  {
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value0", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value6", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_ValueDD15", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_12Bit_Request_Results_Sensor_Value0", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_12Bit_Request_Results_Sensor_Value1", set);
    this.SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_12Bit_Request_Results_Sensor_Value2", set);
  }

  private void SetMarkedState(string ecu, string qualifier, bool state)
  {
    Instrument instrument = this.GetInstrument(ecu, qualifier);
    if (!(instrument != (Instrument) null))
      return;
    instrument.Marked = state;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.start = new Button();
    this.stop = new Button();
    this.DigitalReadoutInstrument17a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument14a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument10 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument19 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.start, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.stop, 1, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.start, "start");
    this.start.Name = "start";
    this.start.UseCompatibleTextRendering = true;
    this.start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.stop, "stop");
    this.stop.Name = "stop";
    this.stop.UseCompatibleTextRendering = true;
    this.stop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument17a, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument4, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument14a, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument43, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument7, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument10, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument19, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 1, 5);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument17a, "DigitalReadoutInstrument17a");
    this.DigitalReadoutInstrument17a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument17a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument17a).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4");
    ((Control) this.DigitalReadoutInstrument17a).Name = "DigitalReadoutInstrument17a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument17a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument14a, "DigitalReadoutInstrument14a");
    this.DigitalReadoutInstrument14a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument14a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument14a).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7");
    ((Control) this.DigitalReadoutInstrument14a).Name = "DigitalReadoutInstrument14a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument14a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
    this.DigitalReadoutInstrument43.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4");
    ((Control) this.DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
    this.DigitalReadoutInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8");
    ((Control) this.DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument10, "DigitalReadoutInstrument10");
    this.DigitalReadoutInstrument10.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0");
    ((Control) this.DigitalReadoutInstrument10).Name = "DigitalReadoutInstrument10";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument19, "DigitalReadoutInstrument19");
    this.DigitalReadoutInstrument19.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument19).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument19).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5");
    ((Control) this.DigitalReadoutInstrument19).Name = "DigitalReadoutInstrument19";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument19).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_ValueDD15");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_Voltages");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
