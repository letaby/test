using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Actuator_Functional_Check.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel;

	private Button buttonStart;

	private DigitalReadoutInstrument digitalReadoutEGRSCGND;

	private DigitalReadoutInstrument digitalReadoutEGRSCBATT;

	private DigitalReadoutInstrument digitalReadoutEGRSRH;

	private DigitalReadoutInstrument digitalReadoutEGRSRL;

	private DigitalReadoutInstrument digitalReadoutEGROL;

	private DigitalReadoutInstrument digitalReadoutHS2SC;

	private DigitalReadoutInstrument digitalReadoutHS2SCUB;

	private SeekTimeListView seekTimeListView;

	private DigitalReadoutInstrument digitalReadoutEngineState;

	private Panel panel1;

	private Checkmark checkmarkCanStart;

	private Label labelCanStart;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private BarInstrument barInstrumentCoolantTemp;

	private SharedProcedureSelection sharedProcedureSelection;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
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
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanel = new TableLayoutPanel();
		panel1 = new Panel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		sharedProcedureSelection = new SharedProcedureSelection();
		labelCanStart = new Label();
		checkmarkCanStart = new Checkmark();
		buttonStart = new Button();
		digitalReadoutEGRSCGND = new DigitalReadoutInstrument();
		digitalReadoutEGRSCBATT = new DigitalReadoutInstrument();
		digitalReadoutEGRSRH = new DigitalReadoutInstrument();
		digitalReadoutEGRSRL = new DigitalReadoutInstrument();
		digitalReadoutEGROL = new DigitalReadoutInstrument();
		digitalReadoutHS2SC = new DigitalReadoutInstrument();
		digitalReadoutHS2SCUB = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		digitalReadoutEngineState = new DigitalReadoutInstrument();
		barInstrumentCoolantTemp = new BarInstrument();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		panel1.SuspendLayout();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)panel1, 2);
		panel1.Controls.Add((Control)(object)sharedProcedureSelection);
		panel1.Controls.Add(labelCanStart);
		panel1.Controls.Add((Control)(object)checkmarkCanStart);
		panel1.Controls.Add(buttonStart);
		componentResourceManager.ApplyResources(panel1, "panel1");
		panel1.Name = "panel1";
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_EGR_IAE_EPA07" });
		componentResourceManager.ApplyResources(labelCanStart, "labelCanStart");
		labelCanStart.Name = "labelCanStart";
		labelCanStart.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutEGRSCGND, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutEGRSCBATT, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutEGRSRH, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutEGRSRL, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutEGROL, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutHS2SC, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutHS2SCUB, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutEngineState, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(panel1, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)barInstrumentCoolantTemp, 1, 1);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(digitalReadoutEGRSCGND, "digitalReadoutEGRSCGND");
		digitalReadoutEGRSCGND.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutEGRSCGND).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutEGRSCGND).Instrument = new Qualifier((QualifierTypes)32, "MCM", "C80600");
		((Control)(object)digitalReadoutEGRSCGND).Name = "digitalReadoutEGRSCGND";
		((SingleInstrumentBase)digitalReadoutEGRSCGND).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutEGRSCBATT, "digitalReadoutEGRSCBATT");
		digitalReadoutEGRSCBATT.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutEGRSCBATT).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutEGRSCBATT).Instrument = new Qualifier((QualifierTypes)32, "MCM", "C80700");
		((Control)(object)digitalReadoutEGRSCBATT).Name = "digitalReadoutEGRSCBATT";
		((SingleInstrumentBase)digitalReadoutEGRSCBATT).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutEGRSRH, "digitalReadoutEGRSRH");
		digitalReadoutEGRSRH.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutEGRSRH).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutEGRSRH).Instrument = new Qualifier((QualifierTypes)32, "MCM", "9A0F00");
		((Control)(object)digitalReadoutEGRSRH).Name = "digitalReadoutEGRSRH";
		((SingleInstrumentBase)digitalReadoutEGRSRH).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutEGRSRL, "digitalReadoutEGRSRL");
		digitalReadoutEGRSRL.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutEGRSRL).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutEGRSRL).Instrument = new Qualifier((QualifierTypes)32, "MCM", "9A1000");
		((Control)(object)digitalReadoutEGRSRL).Name = "digitalReadoutEGRSRL";
		((SingleInstrumentBase)digitalReadoutEGRSRL).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutEGROL, "digitalReadoutEGROL");
		digitalReadoutEGROL.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutEGROL).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutEGROL).Instrument = new Qualifier((QualifierTypes)32, "MCM", "C80900");
		((Control)(object)digitalReadoutEGROL).Name = "digitalReadoutEGROL";
		((SingleInstrumentBase)digitalReadoutEGROL).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutHS2SC, "digitalReadoutHS2SC");
		digitalReadoutHS2SC.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutHS2SC).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutHS2SC).Instrument = new Qualifier((QualifierTypes)32, "MCM", "4E0800");
		((Control)(object)digitalReadoutHS2SC).Name = "digitalReadoutHS2SC";
		((SingleInstrumentBase)digitalReadoutHS2SC).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutHS2SCUB, "digitalReadoutHS2SCUB");
		digitalReadoutHS2SCUB.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutHS2SCUB).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutHS2SCUB).Instrument = new Qualifier((QualifierTypes)32, "MCM", "4E0500");
		((Control)(object)digitalReadoutHS2SCUB).Name = "digitalReadoutHS2SCUB";
		((SingleInstrumentBase)digitalReadoutHS2SCUB).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "EGR Valve Actuator Functional Check";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)(object)seekTimeListView, 5);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(digitalReadoutEngineState, "digitalReadoutEngineState");
		digitalReadoutEngineState.FontGroup = "egriaedigitals";
		((SingleInstrumentBase)digitalReadoutEngineState).FreezeValue = false;
		digitalReadoutEngineState.Gradient.Initialize((ValueState)0, 7);
		digitalReadoutEngineState.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutEngineState.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(4, 3.0, (ValueState)1);
		digitalReadoutEngineState.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(7, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutEngineState).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutEngineState).Name = "digitalReadoutEngineState";
		((SingleInstrumentBase)digitalReadoutEngineState).UnitAlignment = StringAlignment.Near;
		barInstrumentCoolantTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
		barInstrumentCoolantTemp.FontGroup = null;
		((SingleInstrumentBase)barInstrumentCoolantTemp).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentCoolantTemp).Gradient.Initialize((ValueState)2, 1, "°C");
		((AxisSingleInstrumentBase)barInstrumentCoolantTemp).Gradient.Modify(1, 50.0, (ValueState)1);
		((SingleInstrumentBase)barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature");
		((Control)(object)barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
		((SingleInstrumentBase)barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = labelCanStart;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmarkCanStart;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		panel1.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
