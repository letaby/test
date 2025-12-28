using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value0", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value6", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_ValueDD15", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_12Bit_Request_Results_Sensor_Value0", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_12Bit_Request_Results_Sensor_Value1", set);
		SetMarkedState("MCM", "RT_SR001_Sensor_Voltage_3V_12Bit_Request_Results_Sensor_Value2", set);
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
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		start = new Button();
		stop = new Button();
		DigitalReadoutInstrument17a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument14a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument10 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument19 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(start, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(stop, 1, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(start, "start");
		start.Name = "start";
		start.UseCompatibleTextRendering = true;
		start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(stop, "stop");
		stop.Name = "stop";
		stop.UseCompatibleTextRendering = true;
		stop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument17a, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument4, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument14a, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument43, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument7, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument10, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument19, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 1, 5);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument17a, "DigitalReadoutInstrument17a");
		DigitalReadoutInstrument17a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument17a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument17a).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4");
		((Control)(object)DigitalReadoutInstrument17a).Name = "DigitalReadoutInstrument17a";
		((SingleInstrumentBase)DigitalReadoutInstrument17a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument14a, "DigitalReadoutInstrument14a");
		DigitalReadoutInstrument14a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument14a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument14a).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7");
		((Control)(object)DigitalReadoutInstrument14a).Name = "DigitalReadoutInstrument14a";
		((SingleInstrumentBase)DigitalReadoutInstrument14a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
		DigitalReadoutInstrument43.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument43).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4");
		((Control)(object)DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
		((SingleInstrumentBase)DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument10, "DigitalReadoutInstrument10");
		DigitalReadoutInstrument10.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument10).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0");
		((Control)(object)DigitalReadoutInstrument10).Name = "DigitalReadoutInstrument10";
		((SingleInstrumentBase)DigitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument19, "DigitalReadoutInstrument19");
		DigitalReadoutInstrument19.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument19).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument19).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5");
		((Control)(object)DigitalReadoutInstrument19).Name = "DigitalReadoutInstrument19";
		((SingleInstrumentBase)DigitalReadoutInstrument19).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_ValueDD15");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Voltages");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
