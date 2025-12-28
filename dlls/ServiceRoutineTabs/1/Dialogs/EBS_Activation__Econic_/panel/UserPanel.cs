// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EBS_Activation__Econic_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EBS_Activation__Econic_.panel;

public class UserPanel : CustomPanel
{
  private const string ChannelName = "EBS01T";
  private Channel ebs;
  private Timer timer;
  private TableLayoutPanel tableLayoutPanelMain;
  private System.Windows.Forms.Label labelStatus;
  private Button buttonClose;
  private TableLayoutPanel tableLayoutPanelLeftColumn;
  private TableLayoutPanel tableLayoutPanelSensorRotors;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private System.Windows.Forms.Label labelIOMonitorLabel;
  private TableLayoutPanel tableLayoutPanelHeading;
  private System.Windows.Forms.Label label15;
  private TableLayoutPanel tableLayoutPanelInterlocks;
  private System.Windows.Forms.Label labelInterlockWarning;
  private System.Windows.Forms.Label label9;
  private System.Windows.Forms.Label label8;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private TableLayoutPanel tableLayoutPanelRightColumn;
  private TableLayoutPanel tableLayoutPanelControls;
  private RunServicesButton runServicesButton5;
  private RunServicesButton runServicesButton4;
  private RunServicesButton runServicesButton3;
  private RunServicesButton runServicesButton2;
  private RunServicesButton runServicesButton1;
  private System.Windows.Forms.Label labelControls;
  private RunServicesButton runServicesButtonIncreaseBrakePressureLeft;
  private RunServicesButton runServicesButtonHoldBrakePressureFARight;
  private TableLayoutPanel tableLayoutPanelPneumaticConnections;
  private DigitalReadoutInstrument digitalReadoutInstrument8;
  private DigitalReadoutInstrument digitalReadoutInstrument7;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private System.Windows.Forms.Label label1;
  private RunServicesButton runServicesButtonHoldBrakePressureFALeft;

  public UserPanel()
  {
    this.InitializeComponent();
    this.timer = new Timer();
    this.timer.Interval = 2500;
    this.timer.Tick += new EventHandler(this.timer_Tick);
    this.UpdatePreconditionState();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
  }

  public virtual void OnChannelsChanged() => this.SetEbsChannel("EBS01T");

  private void SetEbsChannel(string ecuName)
  {
    if (this.ebs != null)
      this.ebs.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    this.ebs = this.GetChannel(ecuName);
    if (this.ebs == null)
      return;
    this.ebs.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
  }

  private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
    {
      this.ResetError();
    }
    else
    {
      this.labelStatus.Text = e.Exception != null ? e.Exception.Message : Resources.ErrorUnknown;
      this.timer.Start();
    }
  }

  private void ResetError()
  {
    this.timer.Stop();
    this.labelStatus.Text = string.Empty;
  }

  private void timer_Tick(object sender, EventArgs e) => this.ResetError();

  private IEnumerable<Control> GetAllControls(Control source)
  {
    yield return source;
    foreach (Control child in source.Controls.OfType<Control>().SelectMany<Control, Control>((Func<Control, IEnumerable<Control>>) (c => this.GetAllControls(c))))
      yield return child;
  }

  private void UpdatePreconditionState()
  {
    bool flag1 = this.digitalReadoutInstrumentParkBrake.RepresentedState == 1 || this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1;
    bool flag2 = this.ebs != null && this.ebs.Online;
    foreach (Control control in this.GetAllControls((Control) this).OfType<RunServicesButton>())
      control.Enabled = flag1 && flag2;
    this.labelInterlockWarning.Visible = !flag1;
  }

  private void digitalReadoutInstrumentIoInterlock_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdatePreconditionState();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.labelStatus = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.tableLayoutPanelLeftColumn = new TableLayoutPanel();
    this.tableLayoutPanelPneumaticConnections = new TableLayoutPanel();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.tableLayoutPanelSensorRotors = new TableLayoutPanel();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.labelIOMonitorLabel = new System.Windows.Forms.Label();
    this.tableLayoutPanelHeading = new TableLayoutPanel();
    this.label15 = new System.Windows.Forms.Label();
    this.tableLayoutPanelInterlocks = new TableLayoutPanel();
    this.labelInterlockWarning = new System.Windows.Forms.Label();
    this.label9 = new System.Windows.Forms.Label();
    this.label8 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.tableLayoutPanelRightColumn = new TableLayoutPanel();
    this.tableLayoutPanelControls = new TableLayoutPanel();
    this.runServicesButton5 = new RunServicesButton();
    this.runServicesButton4 = new RunServicesButton();
    this.runServicesButton3 = new RunServicesButton();
    this.runServicesButton2 = new RunServicesButton();
    this.runServicesButton1 = new RunServicesButton();
    this.labelControls = new System.Windows.Forms.Label();
    this.runServicesButtonIncreaseBrakePressureLeft = new RunServicesButton();
    this.runServicesButtonHoldBrakePressureFARight = new RunServicesButton();
    this.runServicesButtonHoldBrakePressureFALeft = new RunServicesButton();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelLeftColumn).SuspendLayout();
    ((Control) this.tableLayoutPanelPneumaticConnections).SuspendLayout();
    ((Control) this.tableLayoutPanelSensorRotors).SuspendLayout();
    ((Control) this.tableLayoutPanelHeading).SuspendLayout();
    ((Control) this.tableLayoutPanelInterlocks).SuspendLayout();
    ((Control) this.tableLayoutPanelRightColumn).SuspendLayout();
    ((Control) this.tableLayoutPanelControls).SuspendLayout();
    ((ISupportInitialize) this.runServicesButton5).BeginInit();
    ((ISupportInitialize) this.runServicesButton4).BeginInit();
    ((ISupportInitialize) this.runServicesButton3).BeginInit();
    ((ISupportInitialize) this.runServicesButton2).BeginInit();
    ((ISupportInitialize) this.runServicesButton1).BeginInit();
    ((ISupportInitialize) this.runServicesButtonIncreaseBrakePressureLeft).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHoldBrakePressureFARight).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHoldBrakePressureFALeft).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelLeftColumn, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelHeading, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInterlocks, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelRightColumn, 3, 1);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.labelStatus, 3);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.ForeColor = Color.Red;
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLeftColumn, "tableLayoutPanelLeftColumn");
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.tableLayoutPanelPneumaticConnections, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.tableLayoutPanelSensorRotors, 0, 0);
    ((Control) this.tableLayoutPanelLeftColumn).Name = "tableLayoutPanelLeftColumn";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelPneumaticConnections, "tableLayoutPanelPneumaticConnections");
    ((TableLayoutPanel) this.tableLayoutPanelPneumaticConnections).Controls.Add((Control) this.digitalReadoutInstrument8, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelPneumaticConnections).Controls.Add((Control) this.digitalReadoutInstrument7, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelPneumaticConnections).Controls.Add((Control) this.digitalReadoutInstrument6, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelPneumaticConnections).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelPneumaticConnections).Controls.Add((Control) this.label1, 0, 0);
    ((Control) this.tableLayoutPanelPneumaticConnections).Name = "tableLayoutPanelPneumaticConnections";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd34_Pressure_Rear_Axle_Nominal_Value_Pressure_Rear_Axle_Nominal_Value");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd26_Pressure_Front_Axle_Actual_Value_Pressure_Front_Axle_Actual_Value");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd32_Pressure_Front_Axle_Nominal_Value_Pressure_Front_Axle_Nominal_Value");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd30_Brakevalue_BST_Position_Brakevalue_BST_Position");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.BorderStyle = BorderStyle.FixedSingle;
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelSensorRotors, "tableLayoutPanelSensorRotors");
    ((TableLayoutPanel) this.tableLayoutPanelSensorRotors).Controls.Add((Control) this.digitalReadoutInstrument5, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelSensorRotors).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelSensorRotors).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelSensorRotors).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelSensorRotors).Controls.Add((Control) this.labelIOMonitorLabel, 0, 0);
    ((Control) this.tableLayoutPanelSensorRotors).Name = "tableLayoutPanelSensorRotors";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd03_Wheel_Speed_Rear_Axle_Left_Wheel_Speed_Rear_Axle_Left");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd04_Wheel_Speed_Rear_Axle_Right_Wheel_Speed_Rear_Axle_Right");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd02_Wheel_Speed_Front_Axle_Right_Wheel_Speed_Front_Axle_Right");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd01_Wheel_Speed_Front_Axle_Left_Wheel_Speed_Front_Axle_Left");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    this.labelIOMonitorLabel.BorderStyle = BorderStyle.FixedSingle;
    componentResourceManager.ApplyResources((object) this.labelIOMonitorLabel, "labelIOMonitorLabel");
    this.labelIOMonitorLabel.Name = "labelIOMonitorLabel";
    this.labelIOMonitorLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelHeading, "tableLayoutPanelHeading");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelHeading, 4);
    ((TableLayoutPanel) this.tableLayoutPanelHeading).Controls.Add((Control) this.label15, 0, 0);
    ((Control) this.tableLayoutPanelHeading).Name = "tableLayoutPanelHeading";
    componentResourceManager.ApplyResources((object) this.label15, "label15");
    this.label15.Name = "label15";
    this.label15.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInterlocks, "tableLayoutPanelInterlocks");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelInterlocks, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.labelInterlockWarning, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.label9, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.label8, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 3);
    ((Control) this.tableLayoutPanelInterlocks).Name = "tableLayoutPanelInterlocks";
    componentResourceManager.ApplyResources((object) this.labelInterlockWarning, "labelInterlockWarning");
    this.labelInterlockWarning.ForeColor = Color.Red;
    this.labelInterlockWarning.Name = "labelInterlockWarning";
    this.labelInterlockWarning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label9, "label9");
    this.label9.Name = "label9";
    this.label9.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    this.label8.BorderStyle = BorderStyle.FixedSingle;
    this.label8.Name = "label8";
    this.label8.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 1, 2, "mph");
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 5.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_84");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelRightColumn, "tableLayoutPanelRightColumn");
    ((TableLayoutPanel) this.tableLayoutPanelRightColumn).Controls.Add((Control) this.tableLayoutPanelControls, 0, 2);
    ((Control) this.tableLayoutPanelRightColumn).Name = "tableLayoutPanelRightColumn";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelControls, "tableLayoutPanelControls");
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButton5, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButton4, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButton3, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButton2, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButton1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.labelControls, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonIncreaseBrakePressureLeft, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHoldBrakePressureFARight, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHoldBrakePressureFALeft, 0, 3);
    ((Control) this.tableLayoutPanelControls).Name = "tableLayoutPanelControls";
    componentResourceManager.ApplyResources((object) this.runServicesButton5, "runServicesButton5");
    ((Control) this.runServicesButton5).Name = "runServicesButton5";
    this.runServicesButton5.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_rechts_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButton4, "runServicesButton4");
    ((Control) this.runServicesButton4).Name = "runServicesButton4";
    this.runServicesButton4.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_links_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButton3, "runServicesButton3");
    ((Control) this.runServicesButton3).Name = "runServicesButton3";
    this.runServicesButton3.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_rechts_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButton2, "runServicesButton2");
    ((Control) this.runServicesButton2).Name = "runServicesButton2";
    this.runServicesButton2.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_links_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButton1, "runServicesButton1");
    ((Control) this.runServicesButton1).Name = "runServicesButton1";
    this.runServicesButton1.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_rechts_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    this.labelControls.BorderStyle = BorderStyle.FixedSingle;
    componentResourceManager.ApplyResources((object) this.labelControls, "labelControls");
    this.labelControls.Name = "labelControls";
    this.labelControls.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServicesButtonIncreaseBrakePressureLeft, "runServicesButtonIncreaseBrakePressureLeft");
    ((Control) this.runServicesButtonIncreaseBrakePressureLeft).Name = "runServicesButtonIncreaseBrakePressureLeft";
    this.runServicesButtonIncreaseBrakePressureLeft.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_links_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHoldBrakePressureFARight, "runServicesButtonHoldBrakePressureFARight");
    ((Control) this.runServicesButtonHoldBrakePressureFARight).Name = "runServicesButtonHoldBrakePressureFARight";
    this.runServicesButtonHoldBrakePressureFARight.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_rechts_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHoldBrakePressureFALeft, "runServicesButtonHoldBrakePressureFALeft");
    ((Control) this.runServicesButtonHoldBrakePressureFALeft).Name = "runServicesButtonHoldBrakePressureFALeft";
    this.runServicesButtonHoldBrakePressureFALeft.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_links_Start", (IEnumerable<string>) new string[1]
    {
      "Timing_Parameter=2000"
    }));
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelLeftColumn).ResumeLayout(false);
    ((Control) this.tableLayoutPanelLeftColumn).PerformLayout();
    ((Control) this.tableLayoutPanelPneumaticConnections).ResumeLayout(false);
    ((Control) this.tableLayoutPanelPneumaticConnections).PerformLayout();
    ((Control) this.tableLayoutPanelSensorRotors).ResumeLayout(false);
    ((Control) this.tableLayoutPanelHeading).ResumeLayout(false);
    ((Control) this.tableLayoutPanelHeading).PerformLayout();
    ((Control) this.tableLayoutPanelInterlocks).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInterlocks).PerformLayout();
    ((Control) this.tableLayoutPanelRightColumn).ResumeLayout(false);
    ((Control) this.tableLayoutPanelControls).ResumeLayout(false);
    ((ISupportInitialize) this.runServicesButton5).EndInit();
    ((ISupportInitialize) this.runServicesButton4).EndInit();
    ((ISupportInitialize) this.runServicesButton3).EndInit();
    ((ISupportInitialize) this.runServicesButton2).EndInit();
    ((ISupportInitialize) this.runServicesButton1).EndInit();
    ((ISupportInitialize) this.runServicesButtonIncreaseBrakePressureLeft).EndInit();
    ((ISupportInitialize) this.runServicesButtonHoldBrakePressureFARight).EndInit();
    ((ISupportInitialize) this.runServicesButtonHoldBrakePressureFALeft).EndInit();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
