using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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
		InitializeComponent();
		timer = new Timer();
		timer.Interval = 2500;
		timer.Tick += timer_Tick;
		UpdatePreconditionState();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += ParentForm_FormClosing;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
		((UserControl)this).OnLoad(e);
	}

	private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= ParentForm_FormClosing;
			digitalReadoutInstrumentParkBrake.RepresentedStateChanged -= digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
			digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged -= digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
		}
	}

	public override void OnChannelsChanged()
	{
		SetEbsChannel("EBS01T");
	}

	private void SetEbsChannel(string ecuName)
	{
		if (ebs != null)
		{
			ebs.Services.ServiceCompleteEvent -= Services_ServiceCompleteEvent;
		}
		ebs = ((CustomPanel)this).GetChannel(ecuName);
		if (ebs != null)
		{
			ebs.Services.ServiceCompleteEvent += Services_ServiceCompleteEvent;
		}
	}

	private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			ResetError();
			return;
		}
		labelStatus.Text = ((e.Exception != null) ? e.Exception.Message : Resources.ErrorUnknown);
		timer.Start();
	}

	private void ResetError()
	{
		timer.Stop();
		labelStatus.Text = string.Empty;
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		ResetError();
	}

	private IEnumerable<Control> GetAllControls(Control source)
	{
		yield return source;
		foreach (Control item in source.Controls.OfType<Control>().SelectMany((Control c) => GetAllControls(c)))
		{
			yield return item;
		}
	}

	private void UpdatePreconditionState()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		bool flag = (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1 || (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1;
		bool flag2 = ebs != null && ebs.Online;
		foreach (RunServicesButton item in GetAllControls((Control)(object)this).OfType<RunServicesButton>())
		{
			((Control)(object)item).Enabled = flag && flag2;
		}
		labelInterlockWarning.Visible = !flag;
	}

	private void digitalReadoutInstrumentIoInterlock_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdatePreconditionState();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_103b: Unknown result type (might be due to invalid IL or missing references)
		//IL_108e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1134: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1217: Unknown result type (might be due to invalid IL or missing references)
		//IL_126a: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		labelStatus = new System.Windows.Forms.Label();
		buttonClose = new Button();
		tableLayoutPanelLeftColumn = new TableLayoutPanel();
		tableLayoutPanelPneumaticConnections = new TableLayoutPanel();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		tableLayoutPanelSensorRotors = new TableLayoutPanel();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		labelIOMonitorLabel = new System.Windows.Forms.Label();
		tableLayoutPanelHeading = new TableLayoutPanel();
		label15 = new System.Windows.Forms.Label();
		tableLayoutPanelInterlocks = new TableLayoutPanel();
		labelInterlockWarning = new System.Windows.Forms.Label();
		label9 = new System.Windows.Forms.Label();
		label8 = new System.Windows.Forms.Label();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		tableLayoutPanelRightColumn = new TableLayoutPanel();
		tableLayoutPanelControls = new TableLayoutPanel();
		runServicesButton5 = new RunServicesButton();
		runServicesButton4 = new RunServicesButton();
		runServicesButton3 = new RunServicesButton();
		runServicesButton2 = new RunServicesButton();
		runServicesButton1 = new RunServicesButton();
		labelControls = new System.Windows.Forms.Label();
		runServicesButtonIncreaseBrakePressureLeft = new RunServicesButton();
		runServicesButtonHoldBrakePressureFARight = new RunServicesButton();
		runServicesButtonHoldBrakePressureFALeft = new RunServicesButton();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelLeftColumn).SuspendLayout();
		((Control)(object)tableLayoutPanelPneumaticConnections).SuspendLayout();
		((Control)(object)tableLayoutPanelSensorRotors).SuspendLayout();
		((Control)(object)tableLayoutPanelHeading).SuspendLayout();
		((Control)(object)tableLayoutPanelInterlocks).SuspendLayout();
		((Control)(object)tableLayoutPanelRightColumn).SuspendLayout();
		((Control)(object)tableLayoutPanelControls).SuspendLayout();
		((ISupportInitialize)runServicesButton5).BeginInit();
		((ISupportInitialize)runServicesButton4).BeginInit();
		((ISupportInitialize)runServicesButton3).BeginInit();
		((ISupportInitialize)runServicesButton2).BeginInit();
		((ISupportInitialize)runServicesButton1).BeginInit();
		((ISupportInitialize)runServicesButtonIncreaseBrakePressureLeft).BeginInit();
		((ISupportInitialize)runServicesButtonHoldBrakePressureFARight).BeginInit();
		((ISupportInitialize)runServicesButtonHoldBrakePressureFALeft).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelLeftColumn, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelHeading, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInterlocks, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelRightColumn, 3, 1);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)labelStatus, 3);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.ForeColor = Color.Red;
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanelLeftColumn, "tableLayoutPanelLeftColumn");
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)tableLayoutPanelPneumaticConnections, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)tableLayoutPanelSensorRotors, 0, 0);
		((Control)(object)tableLayoutPanelLeftColumn).Name = "tableLayoutPanelLeftColumn";
		componentResourceManager.ApplyResources(tableLayoutPanelPneumaticConnections, "tableLayoutPanelPneumaticConnections");
		((TableLayoutPanel)(object)tableLayoutPanelPneumaticConnections).Controls.Add((Control)(object)digitalReadoutInstrument8, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelPneumaticConnections).Controls.Add((Control)(object)digitalReadoutInstrument7, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelPneumaticConnections).Controls.Add((Control)(object)digitalReadoutInstrument6, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelPneumaticConnections).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelPneumaticConnections).Controls.Add(label1, 0, 0);
		((Control)(object)tableLayoutPanelPneumaticConnections).Name = "tableLayoutPanelPneumaticConnections";
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd34_Pressure_Rear_Axle_Nominal_Value_Pressure_Rear_Axle_Nominal_Value");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument8).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument8).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd26_Pressure_Front_Axle_Actual_Value_Pressure_Front_Axle_Actual_Value");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument7).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument7).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd32_Pressure_Front_Axle_Nominal_Value_Pressure_Front_Axle_Nominal_Value");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument6).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument6).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd30_Brakevalue_BST_Position_Brakevalue_BST_Position");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument3).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument3).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.BorderStyle = BorderStyle.FixedSingle;
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelSensorRotors, "tableLayoutPanelSensorRotors");
		((TableLayoutPanel)(object)tableLayoutPanelSensorRotors).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelSensorRotors).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelSensorRotors).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelSensorRotors).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelSensorRotors).Controls.Add(labelIOMonitorLabel, 0, 0);
		((Control)(object)tableLayoutPanelSensorRotors).Name = "tableLayoutPanelSensorRotors";
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd03_Wheel_Speed_Rear_Axle_Left_Wheel_Speed_Rear_Axle_Left");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument5).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument5).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd04_Wheel_Speed_Rear_Axle_Right_Wheel_Speed_Rear_Axle_Right");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument1).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd02_Wheel_Speed_Front_Axle_Right_Wheel_Speed_Front_Axle_Right");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument4).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd01_Wheel_Speed_Front_Axle_Left_Wheel_Speed_Front_Axle_Left");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument2).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument2).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		labelIOMonitorLabel.BorderStyle = BorderStyle.FixedSingle;
		componentResourceManager.ApplyResources(labelIOMonitorLabel, "labelIOMonitorLabel");
		labelIOMonitorLabel.Name = "labelIOMonitorLabel";
		labelIOMonitorLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelHeading, "tableLayoutPanelHeading");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelHeading, 4);
		((TableLayoutPanel)(object)tableLayoutPanelHeading).Controls.Add(label15, 0, 0);
		((Control)(object)tableLayoutPanelHeading).Name = "tableLayoutPanelHeading";
		componentResourceManager.ApplyResources(label15, "label15");
		label15.Name = "label15";
		label15.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelInterlocks, "tableLayoutPanelInterlocks");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelInterlocks, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add(labelInterlockWarning, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add(label9, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add(label8, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 3);
		((Control)(object)tableLayoutPanelInterlocks).Name = "tableLayoutPanelInterlocks";
		componentResourceManager.ApplyResources(labelInterlockWarning, "labelInterlockWarning");
		labelInterlockWarning.ForeColor = Color.Red;
		labelInterlockWarning.Name = "labelInterlockWarning";
		labelInterlockWarning.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label9, "label9");
		label9.Name = "label9";
		label9.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label8, "label8");
		label8.BorderStyle = BorderStyle.FixedSingle;
		label8.Name = "label8";
		label8.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)1, 2, "mph");
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 5.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "J1939-0", "DT_84");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelRightColumn, "tableLayoutPanelRightColumn");
		((TableLayoutPanel)(object)tableLayoutPanelRightColumn).Controls.Add((Control)(object)tableLayoutPanelControls, 0, 2);
		((Control)(object)tableLayoutPanelRightColumn).Name = "tableLayoutPanelRightColumn";
		componentResourceManager.ApplyResources(tableLayoutPanelControls, "tableLayoutPanelControls");
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButton5, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButton4, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButton3, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButton2, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButton1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(labelControls, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonIncreaseBrakePressureLeft, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHoldBrakePressureFARight, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHoldBrakePressureFALeft, 0, 3);
		((Control)(object)tableLayoutPanelControls).Name = "tableLayoutPanelControls";
		componentResourceManager.ApplyResources(runServicesButton5, "runServicesButton5");
		((Control)(object)runServicesButton5).Name = "runServicesButton5";
		runServicesButton5.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(runServicesButton4, "runServicesButton4");
		((Control)(object)runServicesButton4).Name = "runServicesButton4";
		runServicesButton4.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(runServicesButton3, "runServicesButton3");
		((Control)(object)runServicesButton3).Name = "runServicesButton3";
		runServicesButton3.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(runServicesButton2, "runServicesButton2");
		((Control)(object)runServicesButton2).Name = "runServicesButton2";
		runServicesButton2.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(runServicesButton1, "runServicesButton1");
		((Control)(object)runServicesButton1).Name = "runServicesButton1";
		runServicesButton1.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		labelControls.BorderStyle = BorderStyle.FixedSingle;
		componentResourceManager.ApplyResources(labelControls, "labelControls");
		labelControls.Name = "labelControls";
		labelControls.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServicesButtonIncreaseBrakePressureLeft, "runServicesButtonIncreaseBrakePressureLeft");
		((Control)(object)runServicesButtonIncreaseBrakePressureLeft).Name = "runServicesButtonIncreaseBrakePressureLeft";
		runServicesButtonIncreaseBrakePressureLeft.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHoldBrakePressureFARight, "runServicesButtonHoldBrakePressureFARight");
		((Control)(object)runServicesButtonHoldBrakePressureFARight).Name = "runServicesButtonHoldBrakePressureFARight";
		runServicesButtonHoldBrakePressureFARight.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHoldBrakePressureFALeft, "runServicesButtonHoldBrakePressureFALeft");
		((Control)(object)runServicesButtonHoldBrakePressureFALeft).Name = "runServicesButtonHoldBrakePressureFALeft";
		runServicesButtonHoldBrakePressureFALeft.ServiceCalls.Add(new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }));
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelLeftColumn).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelLeftColumn).PerformLayout();
		((Control)(object)tableLayoutPanelPneumaticConnections).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelPneumaticConnections).PerformLayout();
		((Control)(object)tableLayoutPanelSensorRotors).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelHeading).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelHeading).PerformLayout();
		((Control)(object)tableLayoutPanelInterlocks).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInterlocks).PerformLayout();
		((Control)(object)tableLayoutPanelRightColumn).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelControls).ResumeLayout(performLayout: false);
		((ISupportInitialize)runServicesButton5).EndInit();
		((ISupportInitialize)runServicesButton4).EndInit();
		((ISupportInitialize)runServicesButton3).EndInit();
		((ISupportInitialize)runServicesButton2).EndInit();
		((ISupportInitialize)runServicesButton1).EndInit();
		((ISupportInitialize)runServicesButtonIncreaseBrakePressureLeft).EndInit();
		((ISupportInitialize)runServicesButtonHoldBrakePressureFARight).EndInit();
		((ISupportInitialize)runServicesButtonHoldBrakePressureFALeft).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
