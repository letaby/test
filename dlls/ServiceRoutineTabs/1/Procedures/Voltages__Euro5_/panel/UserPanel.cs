// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__Euro5_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__Euro5_.panel;

public class UserPanel : CustomPanel
{
  private DigitalReadoutInstrument DigitalReadoutInstrument7;
  private DigitalReadoutInstrument DigitalReadoutInstrument4;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument43;
  private DigitalReadoutInstrument DigitalReadoutInstrument23a;
  private DigitalReadoutInstrument DigitalReadoutInstrument20a;
  private DigitalReadoutInstrument DigitalReadoutInstrument17a;
  private DigitalReadoutInstrument DigitalReadoutInstrument14a;
  private DigitalReadoutInstrument DigitalReadoutInstrument11a;
  private DigitalReadoutInstrument DigitalReadoutInstrument8a;
  private DigitalReadoutInstrument DigitalReadoutInstrument5a;
  private DigitalReadoutInstrument DigitalReadoutInstrument2a;
  private TableLayoutPanel tableLayoutPanel1;
  private Button start;
  private Button stop;
  private Panel panel1;
  private DigitalReadoutInstrument DigitalReadoutInstrument53a;

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
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10", set);
    this.SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value12", set);
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
    this.DigitalReadoutInstrument23a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument20a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument17a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument53a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument14a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument11a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument5a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument8a = new DigitalReadoutInstrument();
    this.panel1 = new Panel();
    this.stop = new Button();
    this.start = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    this.panel1.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument23a, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument20a, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument17a, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument7, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument53a, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument4, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument14a, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2a, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument43, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument11a, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument5a, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument8a, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.panel1, 1, 6);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument23a, "DigitalReadoutInstrument23a");
    this.DigitalReadoutInstrument23a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument23a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument23a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0");
    ((Control) this.DigitalReadoutInstrument23a).Name = "DigitalReadoutInstrument23a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument23a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument20a, "DigitalReadoutInstrument20a");
    this.DigitalReadoutInstrument20a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument20a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument20a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8");
    ((Control) this.DigitalReadoutInstrument20a).Name = "DigitalReadoutInstrument20a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument20a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument17a, "DigitalReadoutInstrument17a");
    this.DigitalReadoutInstrument17a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument17a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument17a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6");
    ((Control) this.DigitalReadoutInstrument17a).Name = "DigitalReadoutInstrument17a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument17a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
    this.DigitalReadoutInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5");
    ((Control) this.DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument53a, "DigitalReadoutInstrument53a");
    this.DigitalReadoutInstrument53a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument53a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument53a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7");
    ((Control) this.DigitalReadoutInstrument53a).Name = "DigitalReadoutInstrument53a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument53a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument14a, "DigitalReadoutInstrument14a");
    this.DigitalReadoutInstrument14a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument14a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument14a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5");
    ((Control) this.DigitalReadoutInstrument14a).Name = "DigitalReadoutInstrument14a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument14a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2a, "DigitalReadoutInstrument2a");
    this.DigitalReadoutInstrument2a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4");
    ((Control) this.DigitalReadoutInstrument2a).Name = "DigitalReadoutInstrument2a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
    this.DigitalReadoutInstrument43.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1");
    ((Control) this.DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument11a, "DigitalReadoutInstrument11a");
    this.DigitalReadoutInstrument11a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument11a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument11a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4");
    ((Control) this.DigitalReadoutInstrument11a).Name = "DigitalReadoutInstrument11a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument11a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument5a, "DigitalReadoutInstrument5a");
    this.DigitalReadoutInstrument5a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9");
    ((Control) this.DigitalReadoutInstrument5a).Name = "DigitalReadoutInstrument5a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument8a, "DigitalReadoutInstrument8a");
    this.DigitalReadoutInstrument8a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument8a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument8a).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10");
    ((Control) this.DigitalReadoutInstrument8a).Name = "DigitalReadoutInstrument8a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument8a).UnitAlignment = StringAlignment.Near;
    this.panel1.Controls.Add((Control) this.stop);
    this.panel1.Controls.Add((Control) this.start);
    componentResourceManager.ApplyResources((object) this.panel1, "panel1");
    this.panel1.Name = "panel1";
    componentResourceManager.ApplyResources((object) this.stop, "stop");
    this.stop.Name = "stop";
    this.stop.UseCompatibleTextRendering = true;
    this.stop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.start, "start");
    this.start.Name = "start";
    this.start.UseCompatibleTextRendering = true;
    this.start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_Voltages");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    this.panel1.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
