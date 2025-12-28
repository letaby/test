// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.VoltagesSCRDPF.panel.UserPanel
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
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.VoltagesSCRDPF.panel;

public class UserPanel : CustomPanel
{
  private DigitalReadoutInstrument DigitalReadoutInstrument10;
  private DigitalReadoutInstrument DigitalReadoutInstrument7;
  private DigitalReadoutInstrument DigitalReadoutInstrument4;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument43;
  private DigitalReadoutInstrument DigitalReadoutInstrument23a;
  private DigitalReadoutInstrument DigitalReadoutInstrument8a;
  private DigitalReadoutInstrument DigitalReadoutInstrument5a;
  private DigitalReadoutInstrument DigitalReadoutInstrument2a;
  private TableLayoutPanel tableLayoutPanel1;
  private Button start;
  private Button stop;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel2;
  private TextBox progresstextBox;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument DigitalReadoutInstrument53a;

  public UserPanel()
  {
    this.InitializeComponent();
    this.start.Click += new EventHandler(this.OnStartButtonClick);
    this.stop.Click += new EventHandler(this.OnStopButtonClick);
  }

  private void OnStopButtonClick(object sender, EventArgs e)
  {
    this.SetMarkedState(false);
    this.ReportProgress(Resources.Message_StoppedAcquiringSensorVoltageSignals);
  }

  private void OnStartButtonClick(object sender, EventArgs e)
  {
    this.ClearResults();
    this.SetMarkedState(true);
    this.ReportProgress(Resources.Message_StartedAcquiringSensorVoltageSignals);
  }

  private void SetMarkedState(bool set)
  {
    this.SetMarkedState("ACM02T", "RT_Sensor_Voltage_DPF_Outlet_Pressure", set);
    this.SetMarkedState("ACM02T", "RT_Sensor_Voltage_DOC_Inlet_Pressure", set);
    this.SetMarkedState("ACM02T", "RT_Sensor_Voltage_DOC_Inlet_Temp", set);
    this.SetMarkedState("ACM02T", "RT_Sensor_Voltage_DOC_Outlet_Temp", set);
    this.SetMarkedState("ACM02T", "RT_Sensor_Voltage_DPF_Outlet_Temp", set);
  }

  private void SetMarkedState(string ecu, string qualifier, bool state)
  {
    Instrument instrument = this.GetInstrument(ecu, qualifier);
    if (!(instrument != (Instrument) null))
      return;
    instrument.Marked = state;
  }

  private void ClearResults()
  {
    if (this.progresstextBox == null)
      return;
    this.progresstextBox.Text = "";
  }

  private void ReportProgress(string message)
  {
    if (this.progresstextBox == null)
      return;
    StringBuilder stringBuilder = new StringBuilder(this.progresstextBox.Text);
    stringBuilder.Append(message);
    stringBuilder.Append("\r\n");
    this.progresstextBox.Text = stringBuilder.ToString();
    this.progresstextBox.SelectionStart = this.progresstextBox.TextLength;
    this.progresstextBox.SelectionLength = 0;
    this.progresstextBox.ScrollToCaret();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.progresstextBox = new TextBox();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.start = new Button();
    this.stop = new Button();
    this.DigitalReadoutInstrument10 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument53a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument5a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument8a = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument23a = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.progresstextBox, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel2, 2);
    this.progresstextBox.BackColor = SystemColors.Control;
    componentResourceManager.ApplyResources((object) this.progresstextBox, "progresstextBox");
    this.progresstextBox.Name = "progresstextBox";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.start, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.stop, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument10, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument7, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument53a, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2a, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument5a, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument8a, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument43, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument4, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument23a, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 4);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS097_DEF_Temperature_Sensor_Voltage");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.start, "start");
    this.start.Name = "start";
    this.start.UseCompatibleTextRendering = true;
    this.start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.stop, "stop");
    this.stop.Name = "stop";
    this.stop.UseCompatibleTextRendering = true;
    this.stop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument10, "DigitalReadoutInstrument10");
    this.DigitalReadoutInstrument10.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS099_DEF_Air_Pressure_Sensor_Voltage");
    ((Control) this.DigitalReadoutInstrument10).Name = "DigitalReadoutInstrument10";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
    this.DigitalReadoutInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS098_DEF_Pressure_Sensor_Voltage");
    ((Control) this.DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument53a, "DigitalReadoutInstrument53a");
    this.DigitalReadoutInstrument53a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument53a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument53a).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS093_DEF_Tank_Temperature_Sensor_Voltage");
    ((Control) this.DigitalReadoutInstrument53a).Name = "DigitalReadoutInstrument53a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument53a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2a, "DigitalReadoutInstrument2a");
    this.DigitalReadoutInstrument2a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2a).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS094_DEF_Tank_Level_Sensor_Voltage");
    ((Control) this.DigitalReadoutInstrument2a).Name = "DigitalReadoutInstrument2a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument5a, "DigitalReadoutInstrument5a");
    this.DigitalReadoutInstrument5a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5a).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS095_SCR_Inlet_Temperature_Sensor_Voltage");
    ((Control) this.DigitalReadoutInstrument5a).Name = "DigitalReadoutInstrument5a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument5a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument8a, "DigitalReadoutInstrument8a");
    this.DigitalReadoutInstrument8a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument8a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument8a).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS096_SCR_Oulet_Temperature_Sensor_Voltage");
    ((Control) this.DigitalReadoutInstrument8a).Name = "DigitalReadoutInstrument8a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument8a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "RT_Sensor_Voltage_DOC_Inlet_Temp");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
    this.DigitalReadoutInstrument43.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "RT_Sensor_Voltage_DOC_Outlet_Temp");
    ((Control) this.DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "RT_Sensor_Voltage_DPF_Outlet_Temp");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument23a, "DigitalReadoutInstrument23a");
    this.DigitalReadoutInstrument23a.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument23a).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument23a).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "RT_Sensor_Voltage_DOC_Inlet_Pressure");
    ((Control) this.DigitalReadoutInstrument23a).Name = "DigitalReadoutInstrument23a";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument23a).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "RT_Sensor_Voltage_DPF_Outlet_Pressure");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SCRandDPFVoltages");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
