using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages_SCR_DPF__MY13_.panel;

public class UserPanel : CustomPanel
{
	private DigitalReadoutInstrument DigitalReadoutInstrument7;

	private DigitalReadoutInstrument DigitalReadoutInstrument4;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument43;

	private DigitalReadoutInstrument DigitalReadoutInstrument23a;

	private DigitalReadoutInstrument DigitalReadoutInstrument8a;

	private DigitalReadoutInstrument DigitalReadoutInstrument5a;

	private DigitalReadoutInstrument DigitalReadoutInstrument2a;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private TextBox progresstextBox;

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
		ReportProgress(Resources.Message_StoppedAcquiringSensorVoltageSignals);
	}

	private void OnStartButtonClick(object sender, EventArgs e)
	{
		ClearResults();
		SetMarkedState(set: true);
		ReportProgress(Resources.Message_StartedAcquiringSensorVoltageSignals);
	}

	private void SetMarkedState(bool set)
	{
		SetMarkedState("ACM21T", "RT_Sensor_Voltage_DPF_Outlet_Pressure", set);
		SetMarkedState("ACM21T", "RT_Sensor_Voltage_DOC_Inlet_Pressure", set);
		SetMarkedState("ACM21T", "RT_Sensor_Voltage_DOC_Inlet_Temp", set);
		SetMarkedState("ACM21T", "RT_Sensor_Voltage_DOC_Outlet_Temp", set);
		SetMarkedState("ACM21T", "RT_Sensor_Voltage_DPF_Outlet_Temp", set);
	}

	private void SetMarkedState(string ecu, string qualifier, bool state)
	{
		Instrument instrument = ((CustomPanel)this).GetInstrument(ecu, qualifier);
		if (instrument != null)
		{
			instrument.Marked = state;
		}
	}

	private void ClearResults()
	{
		if (progresstextBox != null)
		{
			progresstextBox.Text = "";
		}
	}

	private void ReportProgress(string message)
	{
		if (progresstextBox != null)
		{
			StringBuilder stringBuilder = new StringBuilder(progresstextBox.Text);
			stringBuilder.AppendLine(message);
			progresstextBox.Text = stringBuilder.ToString();
			progresstextBox.SelectionStart = progresstextBox.TextLength;
			progresstextBox.SelectionLength = 0;
			progresstextBox.ScrollToCaret();
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
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
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		start = new Button();
		stop = new Button();
		DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument53a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument5a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument8a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument23a = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		progresstextBox = new TextBox();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(start, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(stop, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument7, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument53a, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2a, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument5a, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument8a, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument43, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument4, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument23a, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(progresstextBox, 1, 5);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS097_DEF_Temperature_Sensor_Voltage");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(start, "start");
		start.Name = "start";
		start.UseCompatibleTextRendering = true;
		start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(stop, "stop");
		stop.Name = "stop";
		stop.UseCompatibleTextRendering = true;
		stop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS098_DEF_Pressure_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument53a, "DigitalReadoutInstrument53a");
		DigitalReadoutInstrument53a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS093_DEF_Tank_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument53a).Name = "DigitalReadoutInstrument53a";
		((SingleInstrumentBase)DigitalReadoutInstrument53a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2a, "DigitalReadoutInstrument2a");
		DigitalReadoutInstrument2a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2a).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS094_DEF_Tank_Level_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument2a).Name = "DigitalReadoutInstrument2a";
		((SingleInstrumentBase)DigitalReadoutInstrument2a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument5a, "DigitalReadoutInstrument5a");
		DigitalReadoutInstrument5a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument5a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument5a).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS095_SCR_Inlet_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument5a).Name = "DigitalReadoutInstrument5a";
		((SingleInstrumentBase)DigitalReadoutInstrument5a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument8a, "DigitalReadoutInstrument8a");
		DigitalReadoutInstrument8a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument8a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument8a).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS096_SCR_Oulet_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument8a).Name = "DigitalReadoutInstrument8a";
		((SingleInstrumentBase)DigitalReadoutInstrument8a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "RT_Sensor_Voltage_DOC_Inlet_Temp");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
		DigitalReadoutInstrument43.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument43).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "RT_Sensor_Voltage_DOC_Outlet_Temp");
		((Control)(object)DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
		((SingleInstrumentBase)DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "RT_Sensor_Voltage_DPF_Outlet_Temp");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument23a, "DigitalReadoutInstrument23a");
		DigitalReadoutInstrument23a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument23a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument23a).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "RT_Sensor_Voltage_DOC_Inlet_Pressure");
		((Control)(object)DigitalReadoutInstrument23a).Name = "DigitalReadoutInstrument23a";
		((SingleInstrumentBase)DigitalReadoutInstrument23a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "RT_Sensor_Voltage_DPF_Outlet_Pressure");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		progresstextBox.BackColor = SystemColors.Control;
		componentResourceManager.ApplyResources(progresstextBox, "progresstextBox");
		progresstextBox.Name = "progresstextBox";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRandDPFVoltages");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
