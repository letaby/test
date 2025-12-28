using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages_SCR_DPF__Stage_V_.panel;

public class UserPanel : CustomPanel
{
	private DigitalReadoutInstrument DigitalReadoutInstrument7;

	private DigitalReadoutInstrument DigitalReadoutInstrument4;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument43;

	private DigitalReadoutInstrument DigitalReadoutInstrument8a;

	private DigitalReadoutInstrument DigitalReadoutInstrument2a;

	private TableLayoutPanel tableLayoutPanel1;

	private TextBox progresstextBox;

	private Button start;

	private Button stop;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

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
		SetMarkedState("ACM301T", "DT_AS146_T_DOC_Inlet_Temperature_Sensor_Voltage", set);
		SetMarkedState("ACM301T", "DT_AS147_T_DOC_Outlet_Temperature_Sensor_Voltage", set);
		SetMarkedState("ACM301T", "DT_AS148_DPF_Outlet_Temperature_Sensor_Voltage", set);
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
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
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		start = new Button();
		stop = new Button();
		DigitalReadoutInstrument53a = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2a = new DigitalReadoutInstrument();
		progresstextBox = new TextBox();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument43 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument8a = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(start, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(stop, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument53a, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2a, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(progresstextBox, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument43, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument7, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument8a, 1, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(start, "start");
		start.Name = "start";
		start.UseCompatibleTextRendering = true;
		start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(stop, "stop");
		stop.Name = "stop";
		stop.UseCompatibleTextRendering = true;
		stop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument53a, "DigitalReadoutInstrument53a");
		DigitalReadoutInstrument53a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument53a).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS093_DEF_Tank_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument53a).Name = "DigitalReadoutInstrument53a";
		((SingleInstrumentBase)DigitalReadoutInstrument53a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2a, "DigitalReadoutInstrument2a");
		DigitalReadoutInstrument2a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2a).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS094_DEF_Tank_Level_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument2a).Name = "DigitalReadoutInstrument2a";
		((SingleInstrumentBase)DigitalReadoutInstrument2a).UnitAlignment = StringAlignment.Near;
		progresstextBox.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)progresstextBox, 2);
		componentResourceManager.ApplyResources(progresstextBox, "progresstextBox");
		progresstextBox.Name = "progresstextBox";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS148_DPF_Outlet_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument43, "DigitalReadoutInstrument43");
		DigitalReadoutInstrument43.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument43).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument43).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS147_T_DOC_Outlet_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument43).Name = "DigitalReadoutInstrument43";
		((SingleInstrumentBase)DigitalReadoutInstrument43).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS146_T_DOC_Inlet_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS098_DEF_Pressure_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS192_PM_sensor_supply_voltage");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument8a, "DigitalReadoutInstrument8a");
		DigitalReadoutInstrument8a.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument8a).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument8a).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS096_SCR_Oulet_Temperature_Sensor_Voltage");
		((Control)(object)DigitalReadoutInstrument8a).Name = "DigitalReadoutInstrument8a";
		((SingleInstrumentBase)DigitalReadoutInstrument8a).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRandDPFVoltages");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
