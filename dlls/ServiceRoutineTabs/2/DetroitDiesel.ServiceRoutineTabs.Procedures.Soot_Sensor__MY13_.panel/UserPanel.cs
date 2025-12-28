using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Soot_Sensor__MY13_.panel;

public class UserPanel : CustomPanel
{
	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private Checkmark checkmarkStatus;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument9;

	private DigitalReadoutInstrument digitalReadoutInstrument13;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private SeekTimeListView seekTimeListView;

	private TableLayoutPanel tableStatusIndicators;

	private Label labelStatus;

	private Button buttonStart;

	private BarInstrument BarInstrument11;

	private BarInstrument BarInstrument10;

	private BarInstrument BarInstrument9;

	private BarInstrument barInstrument14;

	private BarInstrument BarInstrument13;

	private BarInstrument BarInstrument12;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument10;

	private DigitalReadoutInstrument digitalReadoutInstrument11;

	private SharedProcedureSelection sharedProcedureSelection1;

	private DigitalReadoutInstrument digitalReadoutInstrument12;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
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
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1250: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_158e: Unknown result type (might be due to invalid IL or missing references)
		//IL_176b: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1879: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1952: Unknown result type (might be due to invalid IL or missing references)
		//IL_198b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e13: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableStatusIndicators = new TableLayoutPanel();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		buttonStart = new Button();
		checkmarkStatus = new Checkmark();
		labelStatus = new Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument12 = new DigitalReadoutInstrument();
		digitalReadoutInstrument11 = new DigitalReadoutInstrument();
		digitalReadoutInstrument10 = new DigitalReadoutInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		digitalReadoutInstrument13 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		BarInstrument11 = new BarInstrument();
		BarInstrument10 = new BarInstrument();
		BarInstrument9 = new BarInstrument();
		barInstrument14 = new BarInstrument();
		BarInstrument13 = new BarInstrument();
		BarInstrument12 = new BarInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableStatusIndicators).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableStatusIndicators, "tableStatusIndicators");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableStatusIndicators, 8);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add((Control)(object)sharedProcedureSelection1, 3, 0);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add(buttonStart, 0, 0);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add((Control)(object)checkmarkStatus, 1, 0);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add(labelStatus, 2, 0);
		((Control)(object)tableStatusIndicators).Name = "tableStatusIndicators";
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_PMSensorInspection" });
		((Control)(object)sharedProcedureSelection1).TabStop = false;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		checkmarkStatus.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument12, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument11, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument10, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument13, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument11, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument10, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument9, 7, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument14, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument13, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument12, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableStatusIndicators, 0, 8);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument12, "digitalReadoutInstrument12");
		digitalReadoutInstrument12.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument12).FreezeValue = false;
		digitalReadoutInstrument12.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument12.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument12.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument12.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrument12.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrument12.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrument12.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument12.Gradient.Modify(1, 0.0, (ValueState)2);
		digitalReadoutInstrument12.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument12.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument12.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrument12.Gradient.Modify(5, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_End_of_DSR_shutdown_in_case_of_cooled_down_sensor");
		((Control)(object)digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
		((SingleInstrumentBase)digitalReadoutInstrument12).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument11, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument11, "digitalReadoutInstrument11");
		digitalReadoutInstrument11.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument11).FreezeValue = false;
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrument11.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument11.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument11.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument11.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument11.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet");
		((Control)(object)digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
		((SingleInstrumentBase)digitalReadoutInstrument11).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument10, "digitalReadoutInstrument10");
		digitalReadoutInstrument10.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument10).FreezeValue = false;
		digitalReadoutInstrument10.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrument10.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrument10.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrument10.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
		digitalReadoutInstrument10.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
		digitalReadoutInstrument10.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument10.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument10.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrument10.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument10.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS013_Soot_Sensor_Data_Prot_tube_monitoring_release");
		((Control)(object)digitalReadoutInstrument10).Name = "digitalReadoutInstrument10";
		((SingleInstrumentBase)digitalReadoutInstrument10).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS195_isp_dps_cid");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		digitalReadoutInstrument6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
		digitalReadoutInstrument6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
		digitalReadoutInstrument6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
		digitalReadoutInstrument6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
		digitalReadoutInstrument6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
		digitalReadoutInstrument6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
		digitalReadoutInstrument6.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument6.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument6.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(5, 255.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_DPS_short_circuit_failure_value");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument7, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		digitalReadoutInstrument7.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
		digitalReadoutInstrument7.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
		digitalReadoutInstrument7.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
		digitalReadoutInstrument7.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
		digitalReadoutInstrument7.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
		digitalReadoutInstrument7.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
		digitalReadoutInstrument7.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument7.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument7.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument7.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument7.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrument7.Gradient.Modify(5, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_Measurement_active_value");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 6);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "PMSensorInspection";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 4);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
		digitalReadoutInstrument4.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrument4.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument4.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS012_PM_sensor_active_status");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS012_PM_sensor_regen_status");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText38"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText39"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText40"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText41"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText42"));
		digitalReadoutInstrument5.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument5.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument5.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument5.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument5.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS014_Soot_Sensor_Data_regeneration_cycle_finished");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		digitalReadoutInstrument9.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText43"));
		digitalReadoutInstrument9.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText44"));
		digitalReadoutInstrument9.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText45"));
		digitalReadoutInstrument9.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText46"));
		digitalReadoutInstrument9.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText47"));
		digitalReadoutInstrument9.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument9.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS013_Soot_Sensor_Data_Regeneration_active");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((SingleInstrumentBase)digitalReadoutInstrument9).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument13, "digitalReadoutInstrument13");
		digitalReadoutInstrument13.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument13).FreezeValue = false;
		digitalReadoutInstrument13.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText48"));
		digitalReadoutInstrument13.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText49"));
		digitalReadoutInstrument13.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText50"));
		digitalReadoutInstrument13.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText51"));
		digitalReadoutInstrument13.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText52"));
		digitalReadoutInstrument13.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText53"));
		digitalReadoutInstrument13.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument13.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument13.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument13.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument13.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrument13.Gradient.Modify(5, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument13).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_DPS_sum_failure");
		((Control)(object)digitalReadoutInstrument13).Name = "digitalReadoutInstrument13";
		((SingleInstrumentBase)digitalReadoutInstrument13).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument13).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)DigitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS189_absolute_current_of_PM_sensor");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		BarInstrument11.BarOrientation = (ControlOrientation)1;
		BarInstrument11.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument11, "BarInstrument11");
		BarInstrument11.FontGroup = "SootSensor_Thermometers";
		((SingleInstrumentBase)BarInstrument11).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS019_SCR_Outlet_Temperature");
		((Control)(object)BarInstrument11).Name = "BarInstrument11";
		((AxisSingleInstrumentBase)BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument11, 4);
		((SingleInstrumentBase)BarInstrument11).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument11).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument11).UnitAlignment = StringAlignment.Near;
		BarInstrument10.BarOrientation = (ControlOrientation)1;
		BarInstrument10.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument10, "BarInstrument10");
		BarInstrument10.FontGroup = "SootSensor_Thermometers";
		((SingleInstrumentBase)BarInstrument10).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS190_measured_temp_at_PM_sensor");
		((Control)(object)BarInstrument10).Name = "BarInstrument10";
		((AxisSingleInstrumentBase)BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument10, 4);
		((SingleInstrumentBase)BarInstrument10).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument10).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument10).UnitAlignment = StringAlignment.Near;
		BarInstrument9.BarOrientation = (ControlOrientation)1;
		BarInstrument9.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument9, "BarInstrument9");
		BarInstrument9.FontGroup = "SootSensor_Thermometers";
		((SingleInstrumentBase)BarInstrument9).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS194_isp_dps_t_mea_iv");
		((Control)(object)BarInstrument9).Name = "BarInstrument9";
		((AxisSingleInstrumentBase)BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument9, 4);
		((SingleInstrumentBase)BarInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument9).UnitAlignment = StringAlignment.Near;
		barInstrument14.BarOrientation = (ControlOrientation)1;
		barInstrument14.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument14, "barInstrument14");
		barInstrument14.FontGroup = "SootSensor_Thermometers";
		((SingleInstrumentBase)barInstrument14).FreezeValue = false;
		((SingleInstrumentBase)barInstrument14).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS053_Ambient_Air_Temperature");
		((Control)(object)barInstrument14).Name = "barInstrument14";
		((AxisSingleInstrumentBase)barInstrument14).PreferredAxisRange = new AxisRange(-20.0, 80.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument14, 4);
		((SingleInstrumentBase)barInstrument14).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument14).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument14).UnitAlignment = StringAlignment.Near;
		BarInstrument13.BarOrientation = (ControlOrientation)1;
		BarInstrument13.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument13, "BarInstrument13");
		BarInstrument13.FontGroup = "SootSensor_Thermometers";
		((SingleInstrumentBase)BarInstrument13).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument13).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)BarInstrument13).Name = "BarInstrument13";
		((AxisSingleInstrumentBase)BarInstrument13).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument13, 4);
		((SingleInstrumentBase)BarInstrument13).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument13).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument13).UnitAlignment = StringAlignment.Near;
		BarInstrument12.BarOrientation = (ControlOrientation)1;
		BarInstrument12.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument12, "BarInstrument12");
		BarInstrument12.FontGroup = "SootSensor_Thermometers";
		((SingleInstrumentBase)BarInstrument12).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument12).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS018_SCR_Inlet_Temperature");
		((Control)(object)BarInstrument12).Name = "BarInstrument12";
		((AxisSingleInstrumentBase)BarInstrument12).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument12, 4);
		((SingleInstrumentBase)BarInstrument12).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument12).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument12).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = "Soot Sensor Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS191_PM_sensor_PWM_control");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Soot_Sensor");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableStatusIndicators).ResumeLayout(performLayout: false);
		((Control)(object)tableStatusIndicators).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
