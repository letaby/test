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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Actuator_Slow_Learn__MY13_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel1;

	private Checkmark engineSpeedCheck;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private Button buttonStart;

	private Button buttonClose;

	private Button buttonStopAll;

	private SharedProcedureSelection sharedProcedureSelection;

	private TableLayoutPanel tableLayoutPanel2;

	private TextBox textBoxResults;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private TableLayoutPanel tableLayoutPanel3;

	private Label labelNote;

	private Label engineStatusLabel;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_0d28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc5: Expected O, but got Unknown
		//IL_10e4: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		textBoxResults = new TextBox();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		tableLayoutPanel1 = new TableLayoutPanel();
		engineSpeedCheck = new Checkmark();
		buttonStart = new Button();
		buttonClose = new Button();
		engineStatusLabel = new Label();
		buttonStopAll = new Button();
		sharedProcedureSelection = new SharedProcedureSelection();
		tableLayoutPanel3 = new TableLayoutPanel();
		labelNote = new Label();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textBoxResults, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		textBoxResults.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)textBoxResults, 5);
		componentResourceManager.ApplyResources(textBoxResults, "textBoxResults");
		textBoxResults.Name = "textBoxResults";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)2, 100);
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(5, 4.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(6, 5.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(7, 6.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(8, 7.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(9, 8.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(10, 9.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(11, 10.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(12, 11.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(13, 12.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(14, 13.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(15, 14.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(16, 15.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(17, 16.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(18, 17.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(19, 18.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(20, 19.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(21, 20.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(22, 21.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(23, 22.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(24, 23.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(25, 30.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(26, 31.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(27, 32.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(28, 33.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(29, 34.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(30, 35.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(31, 36.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(32, 50.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(33, 51.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(34, 52.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(35, 53.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(36, 54.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(37, 55.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(38, 56.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(39, 60.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(40, 62.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(41, 63.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(42, 64.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(43, 65.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(44, 66.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(45, 67.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(46, 68.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(47, 69.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(48, 70.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(49, 100.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(50, 101.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(51, 103.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(52, 104.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(53, 105.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(54, 106.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(55, 107.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(56, 108.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(57, 109.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(58, 110.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(59, 111.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(60, 112.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(61, 113.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(62, 115.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(63, 116.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(64, 117.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(65, 118.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(66, 119.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(67, 121.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(68, 122.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(69, 123.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(70, 124.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(71, 140.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(72, 141.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(73, 142.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(74, 143.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(75, 144.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(76, 145.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(77, 146.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(78, 200.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(79, 201.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(80, 202.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(81, 203.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(82, 204.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(83, 205.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(84, 206.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(85, 236.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(86, 237.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(87, 238.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(88, 239.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(89, 240.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(90, 241.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(91, 243.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(92, 244.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(93, 245.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(94, 247.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(95, 248.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(96, 249.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(97, 250.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(98, 251.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(99, 252.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(100, 255.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS050_SRA3_Status_Code");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)engineSpeedCheck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(engineStatusLabel, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStopAll, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(engineSpeedCheck, "engineSpeedCheck");
		((Control)(object)engineSpeedCheck).Name = "engineSpeedCheck";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)buttonStart, 2);
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)engineStatusLabel, 4);
		componentResourceManager.ApplyResources(engineStatusLabel, "engineStatusLabel");
		engineStatusLabel.Name = "engineStatusLabel";
		engineStatusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonStopAll, "buttonStopAll");
		buttonStopAll.Name = "buttonStopAll";
		buttonStopAll.UseCompatibleTextRendering = true;
		buttonStopAll.UseVisualStyleBackColor = true;
		((Control)(object)sharedProcedureSelection).BackColor = SystemColors.Control;
		((Control)(object)sharedProcedureSelection).ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_EGRActuatorSlowLearn_MY13" });
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)labelNote, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)tableLayoutPanel1, 0, 1);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		labelNote.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelNote, "labelNote");
		((Control)(object)labelNote).Name = "labelNote";
		labelNote.Orientation = (TextOrientation)1;
		labelNote.UseSystemColors = true;
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = engineStatusLabel;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = engineSpeedCheck;
		sharedProcedureIntegrationComponent.ResultsTarget = textBoxResults;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent.StopAllButton = buttonStopAll;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_EGRActuatorSlowLearn");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel3);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
