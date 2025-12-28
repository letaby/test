using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__MDEG_.panel;

public class UserPanel : CustomPanel
{
	private DigitalReadoutInstrument DigitalReadoutInstrument7;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument23a;

	private DigitalReadoutInstrument DigitalReadoutInstrument20a;

	private DigitalReadoutInstrument DigitalReadoutInstrument17a;

	private DigitalReadoutInstrument DigitalReadoutInstrument11a;

	private DigitalReadoutInstrument DigitalReadoutInstrument8a;

	private DigitalReadoutInstrument DigitalReadoutInstrument5a;

	private DigitalReadoutInstrument DigitalReadoutInstrument2a;

	private TableLayoutPanel tableLayoutPanel1;

	private Button start;

	private Button stop;

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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		foreach (DigitalReadoutInstrument item in ((TableLayoutPanel)(object)tableLayoutPanel1).Controls.OfType<DigitalReadoutInstrument>())
		{
			Qualifier instrument = ((SingleInstrumentBase)item).Instrument;
			string ecu = ((Qualifier)(ref instrument)).Ecu;
			instrument = ((SingleInstrumentBase)item).Instrument;
			SetMarkedState(ecu, ((Qualifier)(ref instrument)).Name, set);
		}
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
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
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		DigitalReadoutInstrument23a = new DigitalReadoutInstrument();
		start = new Button();
		DigitalReadoutInstrument20a = new DigitalReadoutInstrument();
		stop = new Button();
		DigitalReadoutInstrument17a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument53a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument11a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument5a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument8a = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument23a, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(start, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument20a, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(stop, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument17a, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument7, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument53a, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2a, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument11a, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument5a, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument8a, 0, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument23a, "DigitalReadoutInstrument23a");
		DigitalReadoutInstrument23a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument23a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument23a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0");
		((Control)(object)DigitalReadoutInstrument23a).Name = "DigitalReadoutInstrument23a";
		((SingleInstrumentBase)DigitalReadoutInstrument23a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(start, "start");
		start.Name = "start";
		start.UseCompatibleTextRendering = true;
		start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument20a, "DigitalReadoutInstrument20a");
		DigitalReadoutInstrument20a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument20a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument20a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8");
		((Control)(object)DigitalReadoutInstrument20a).Name = "DigitalReadoutInstrument20a";
		((SingleInstrumentBase)DigitalReadoutInstrument20a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(stop, "stop");
		stop.Name = "stop";
		stop.UseCompatibleTextRendering = true;
		stop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument17a, "DigitalReadoutInstrument17a");
		DigitalReadoutInstrument17a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument17a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument17a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6");
		((Control)(object)DigitalReadoutInstrument17a).Name = "DigitalReadoutInstrument17a";
		((SingleInstrumentBase)DigitalReadoutInstrument17a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value3");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument53a, "DigitalReadoutInstrument53a");
		DigitalReadoutInstrument53a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7");
		((Control)(object)DigitalReadoutInstrument53a).Name = "DigitalReadoutInstrument53a";
		((SingleInstrumentBase)DigitalReadoutInstrument53a).UnitAlignment = StringAlignment.Near;
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
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Voltages");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
