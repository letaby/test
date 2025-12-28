using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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
		InitializeComponent();
		start.Click += OnStartButtonClick;
		stop.Click += OnStopButtonClick;
	}

	private void OnStopButtonClick(object sender, EventArgs e)
	{
		SetMarkedState(set: false);
	}

	private void OnStartButtonClick(object sender, EventArgs e)
	{
		SetMarkedState(set: true);
	}

	private void SetMarkedState(bool set)
	{
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10", set);
		SetMarkedState("MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value12", set);
	}

	private void SetMarkedState(string ecu, string qualifier, bool state)
	{
		Instrument instrument = ((CustomPanel)this).GetInstrument(ecu, qualifier);
		if (instrument != null)
		{
			instrument.Marked = state;
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
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
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		DigitalReadoutInstrument23a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument20a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument17a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument53a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument14a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument11a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument5a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument8a = new DigitalReadoutInstrument();
		panel1 = new Panel();
		stop = new Button();
		start = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		panel1.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument23a, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument20a, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument17a, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument7, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument53a, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument4, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument14a, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2a, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument43, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument11a, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument5a, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument8a, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(panel1, 1, 6);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument23a, "DigitalReadoutInstrument23a");
		DigitalReadoutInstrument23a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument23a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument23a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0");
		((Control)(object)DigitalReadoutInstrument23a).Name = "DigitalReadoutInstrument23a";
		((SingleInstrumentBase)DigitalReadoutInstrument23a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument20a, "DigitalReadoutInstrument20a");
		DigitalReadoutInstrument20a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument20a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument20a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8");
		((Control)(object)DigitalReadoutInstrument20a).Name = "DigitalReadoutInstrument20a";
		((SingleInstrumentBase)DigitalReadoutInstrument20a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument17a, "DigitalReadoutInstrument17a");
		DigitalReadoutInstrument17a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument17a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument17a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6");
		((Control)(object)DigitalReadoutInstrument17a).Name = "DigitalReadoutInstrument17a";
		((SingleInstrumentBase)DigitalReadoutInstrument17a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument53a, "DigitalReadoutInstrument53a");
		DigitalReadoutInstrument53a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7");
		((Control)(object)DigitalReadoutInstrument53a).Name = "DigitalReadoutInstrument53a";
		((SingleInstrumentBase)DigitalReadoutInstrument53a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument14a, "DigitalReadoutInstrument14a");
		DigitalReadoutInstrument14a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument14a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument14a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5");
		((Control)(object)DigitalReadoutInstrument14a).Name = "DigitalReadoutInstrument14a";
		((SingleInstrumentBase)DigitalReadoutInstrument14a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2a, "DigitalReadoutInstrument2a");
		DigitalReadoutInstrument2a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4");
		((Control)(object)DigitalReadoutInstrument2a).Name = "DigitalReadoutInstrument2a";
		((SingleInstrumentBase)DigitalReadoutInstrument2a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
		DigitalReadoutInstrument43.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument43).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1");
		((Control)(object)DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
		((SingleInstrumentBase)DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument11a, "DigitalReadoutInstrument11a");
		DigitalReadoutInstrument11a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument11a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument11a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4");
		((Control)(object)DigitalReadoutInstrument11a).Name = "DigitalReadoutInstrument11a";
		((SingleInstrumentBase)DigitalReadoutInstrument11a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument5a, "DigitalReadoutInstrument5a");
		DigitalReadoutInstrument5a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument5a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument5a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9");
		((Control)(object)DigitalReadoutInstrument5a).Name = "DigitalReadoutInstrument5a";
		((SingleInstrumentBase)DigitalReadoutInstrument5a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument8a, "DigitalReadoutInstrument8a");
		DigitalReadoutInstrument8a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument8a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument8a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10");
		((Control)(object)DigitalReadoutInstrument8a).Name = "DigitalReadoutInstrument8a";
		((SingleInstrumentBase)DigitalReadoutInstrument8a).UnitAlignment = StringAlignment.Near;
		panel1.Controls.Add(stop);
		panel1.Controls.Add(start);
		componentResourceManager.ApplyResources(panel1, "panel1");
		panel1.Name = "panel1";
		componentResourceManager.ApplyResources(stop, "stop");
		stop.Name = "stop";
		stop.UseCompatibleTextRendering = true;
		stop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(start, "start");
		start.Name = "start";
		start.UseCompatibleTextRendering = true;
		start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Voltages");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		panel1.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
