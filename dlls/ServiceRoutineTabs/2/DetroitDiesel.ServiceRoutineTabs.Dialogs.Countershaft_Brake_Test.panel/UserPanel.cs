using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel;

public class UserPanel : CustomPanel
{
	private Channel tcm = null;

	private TableLayoutPanel tableLayoutPanel1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrumentCountershaftMaxSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DialInstrument dialInstrument1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private System.Windows.Forms.Label label1;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmark1;

	private TableLayoutPanel tableLayoutPanel3;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private TableLayoutPanel tableLayoutPanelOutput;

	private TableLayoutPanel tableLayoutPanelOutputMessage;

	private Checkmark checkmarkResults;

	private SeekTimeListView seekTimeListView1;

	private ScalingLabel scalingLabelResult;

	private DigitalReadoutInstrument digitalReadoutInstrumentCountershaftSpeed;

	private Button button1;

	public UserPanel()
	{
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		if (sharedProcedureSelection1.SelectedProcedure != null)
		{
			sharedProcedureSelection1.SelectedProcedure.StartComplete += SelectedProcedure_StartComplete;
			sharedProcedureSelection1.SelectedProcedure.StopComplete += SelectedProcedure_StopComplete;
		}
		DisplayResult(display: false);
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection1.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((CustomPanel)this).ParentFormClosing -= this_ParentFormClosing;
			if (sharedProcedureSelection1.SelectedProcedure != null)
			{
				sharedProcedureSelection1.SelectedProcedure.StartComplete -= SelectedProcedure_StartComplete;
				sharedProcedureSelection1.SelectedProcedure.StopComplete -= SelectedProcedure_StopComplete;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		((CustomPanel)this).OnChannelsChanged();
		SetTcm(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
	}

	private void SetTcm(Channel tcm)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Invalid comparison between Unknown and I4
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (this.tcm == tcm)
		{
			return;
		}
		this.tcm = tcm;
		if (this.tcm == null)
		{
			return;
		}
		Ecu ecu = null;
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			ecu = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecu == null || !(ecu.Identifier == tcm.Ecu.Identifier) || !(ecu.Name != tcm.Ecu.Name))
			{
				continue;
			}
			instrument = val.Instrument;
			QualifierTypes type = ((Qualifier)(ref instrument)).Type;
			string name = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			if (val.DataItem != null)
			{
				continue;
			}
			QualifierTypes val2 = (QualifierTypes)0;
			instrument = val.Instrument;
			if ((int)((Qualifier)(ref instrument)).Type == 1)
			{
				val2 = (QualifierTypes)8;
			}
			else
			{
				instrument = val.Instrument;
				if ((int)((Qualifier)(ref instrument)).Type == 8)
				{
					val2 = (QualifierTypes)1;
				}
			}
			QualifierTypes val3 = val2;
			string name2 = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(val3, name2, ((Qualifier)(ref instrument)).Name);
		}
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_CountershaftBrakeTest_" + this.tcm.Ecu.Name });
	}

	private void SelectedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		DisplayResult(display: false);
	}

	private string GetInstrumentValue(SingleInstrumentBase instrument)
	{
		if (instrument != null && instrument.DataItem != null && instrument.DataItem.Value != null)
		{
			return instrument.DataItem.ValueAsString(instrument.DataItem.Value) + " " + instrument.DataItem.Units;
		}
		return null;
	}

	private void SelectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Invalid comparison between Unknown and I4
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		if (seekTimeListView1.Text.EndsWith("Timeout\r\n", StringComparison.OrdinalIgnoreCase))
		{
			((Control)(object)scalingLabelResult).Text = Resources.MessageTimeout;
			checkmarkResults.CheckState = CheckState.Indeterminate;
			scalingLabelResult.RepresentedState = (ValueState)2;
		}
		else
		{
			string instrumentValue = GetInstrumentValue((SingleInstrumentBase)(object)digitalReadoutInstrumentCountershaftMaxSpeed);
			((Control)(object)scalingLabelResult).Text = (((object)e.Result).ToString() + ": " + instrumentValue) ?? Resources.MessageCounterShaftBrakeValueUnavailable;
			checkmarkResults.CheckState = (((int)e.Result == 1) ? CheckState.Checked : CheckState.Unchecked);
			scalingLabelResult.RepresentedState = (ValueState)(((int)e.Result == 1) ? 1 : 3);
		}
		DisplayResult(display: true);
	}

	private void DisplayResult(bool display)
	{
		((Control)(object)tableLayoutPanelOutputMessage).Visible = display;
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
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
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Expected O, but got Unknown
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d20: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		dialInstrument1 = new DialInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		button1 = new Button();
		checkmark1 = new Checkmark();
		label1 = new System.Windows.Forms.Label();
		tableLayoutPanel3 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCountershaftMaxSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		tableLayoutPanelOutput = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanelOutputMessage = new TableLayoutPanel();
		scalingLabelResult = new ScalingLabel();
		checkmarkResults = new Checkmark();
		digitalReadoutInstrumentCountershaftSpeed = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanelOutput).SuspendLayout();
		((Control)(object)tableLayoutPanelOutputMessage).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCountershaftMaxSpeed, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelOutput, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCountershaftSpeed, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(dialInstrument1, "dialInstrument1");
		dialInstrument1.FontGroup = "dial";
		((SingleInstrumentBase)dialInstrument1).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrument1).Gradient.Initialize((ValueState)3, 4, "rpm");
		((AxisSingleInstrumentBase)dialInstrument1).Gradient.Modify(1, 550.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrument1).Gradient.Modify(2, 650.0, (ValueState)2);
		((AxisSingleInstrumentBase)dialInstrument1).Gradient.Modify(3, 1900.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrument1).Gradient.Modify(4, 2000.0, (ValueState)3);
		((SingleInstrumentBase)dialInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)dialInstrument1).Name = "dialInstrument1";
		((AxisSingleInstrumentBase)dialInstrument1).PreferredAxisRange = new AxisRange(500.0, 2000.0, "");
		((SingleInstrumentBase)dialInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(button1, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label1, 2, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[2] { "SP_CountershaftBrakeTest_TCM01T", "SP_CountershaftBrakeTest_TCM05T" });
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument5, 2, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)1, 2);
		digitalReadoutInstrument1.Gradient.Modify(1, 1.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(2, 95.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel3).SetColumnSpan((Control)(object)digitalReadoutInstrument4, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		digitalReadoutInstrument4.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument4.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument4.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		digitalReadoutInstrument5.Gradient.Initialize((ValueState)3, 1, "psi");
		digitalReadoutInstrument5.Gradient.Modify(1, 90.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCountershaftMaxSpeed, "digitalReadoutInstrumentCountershaftMaxSpeed");
		digitalReadoutInstrumentCountershaftMaxSpeed.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrumentCountershaftMaxSpeed).FreezeValue = false;
		digitalReadoutInstrumentCountershaftMaxSpeed.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentCountershaftMaxSpeed.Gradient.Modify(1, -10000.0, (ValueState)1);
		digitalReadoutInstrumentCountershaftMaxSpeed.Gradient.Modify(2, -3000.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentCountershaftMaxSpeed).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_231A_Maximaler_Gradient_Vorgelegewellen_Drehzahl_max_Gradient_Vorgelegewellen_Drehzahl");
		((Control)(object)digitalReadoutInstrumentCountershaftMaxSpeed).Name = "digitalReadoutInstrumentCountershaftMaxSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentCountershaftMaxSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		digitalReadoutInstrument6.Gradient.Initialize((ValueState)3, 8);
		digitalReadoutInstrument6.Gradient.Modify(1, -1.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(2, 0.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(3, 1.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(4, 2.0, (ValueState)0);
		digitalReadoutInstrument6.Gradient.Modify(5, 3.0, (ValueState)1);
		digitalReadoutInstrument6.Gradient.Modify(6, 4.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(7, 5.0, (ValueState)3);
		digitalReadoutInstrument6.Gradient.Modify(8, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelOutput, "tableLayoutPanelOutput");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelOutput, 2);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).Controls.Add((Control)(object)seekTimeListView1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).Controls.Add((Control)(object)tableLayoutPanelOutputMessage, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelOutput).Name = "tableLayoutPanelOutput";
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "CountershaftBrakeTest";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(tableLayoutPanelOutputMessage, "tableLayoutPanelOutputMessage");
		((TableLayoutPanel)(object)tableLayoutPanelOutputMessage).Controls.Add((Control)(object)scalingLabelResult, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelOutputMessage).Controls.Add((Control)(object)checkmarkResults, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelOutputMessage).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelOutputMessage).Name = "tableLayoutPanelOutputMessage";
		scalingLabelResult.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelResult, "scalingLabelResult");
		scalingLabelResult.FontGroup = null;
		scalingLabelResult.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelResult).Name = "scalingLabelResult";
		componentResourceManager.ApplyResources(checkmarkResults, "checkmarkResults");
		((Control)(object)checkmarkResults).Name = "checkmarkResults";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCountershaftSpeed, "digitalReadoutInstrumentCountershaftSpeed");
		digitalReadoutInstrumentCountershaftSpeed.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrumentCountershaftSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCountershaftSpeed).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle");
		((Control)(object)digitalReadoutInstrumentCountershaftSpeed).Name = "digitalReadoutInstrumentCountershaftSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentCountershaftSpeed).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = label1;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = button1;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CountershaftBrakeTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelOutput).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelOutputMessage).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
