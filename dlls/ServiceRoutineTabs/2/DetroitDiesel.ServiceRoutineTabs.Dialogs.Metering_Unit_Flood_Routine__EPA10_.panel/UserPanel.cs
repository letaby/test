using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Metering_Unit_Flood_Routine__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private SharedProcedureSelection sharedProcedureSelection;

	private Label status;

	private Checkmark statusCheckmark;

	private Button buttonStart;

	private TableLayoutPanel tableLayoutPanel;

	private BarInstrument barInstrumentDEFAirPressure;

	private BarInstrument barInstrumentDEFPressure;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private SeekTimeListView seekTimeListView;

	private Label labelNote;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress;
	}

	private void InitializeComponent()
	{
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
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Expected O, but got Unknown
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		status = new Label();
		buttonStart = new Button();
		barInstrumentDEFAirPressure = new BarInstrument();
		barInstrumentDEFPressure = new BarInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		labelNote = new Label();
		sharedProcedureSelection = new SharedProcedureSelection();
		statusCheckmark = new Checkmark();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(status, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonStart, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)barInstrumentDEFAirPressure, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)barInstrumentDEFPressure, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)labelNote, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)sharedProcedureSelection, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)statusCheckmark, 0, 5);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(status, "status");
		status.Name = "status";
		status.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)barInstrumentDEFAirPressure, 4);
		componentResourceManager.ApplyResources(barInstrumentDEFAirPressure, "barInstrumentDEFAirPressure");
		barInstrumentDEFAirPressure.FontGroup = null;
		((SingleInstrumentBase)barInstrumentDEFAirPressure).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDEFAirPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
		((Control)(object)barInstrumentDEFAirPressure).Name = "barInstrumentDEFAirPressure";
		((SingleInstrumentBase)barInstrumentDEFAirPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)barInstrumentDEFPressure, 4);
		componentResourceManager.ApplyResources(barInstrumentDEFPressure, "barInstrumentDEFPressure");
		barInstrumentDEFPressure.FontGroup = null;
		((SingleInstrumentBase)barInstrumentDEFPressure).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDEFPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure");
		((Control)(object)barInstrumentDEFPressure).Name = "barInstrumentDEFPressure";
		((SingleInstrumentBase)barInstrumentDEFPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)seekTimeListView, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Metering Unit Flood Routine";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.f";
		labelNote.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelNote, "labelNote");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)labelNote, 4);
		((Control)(object)labelNote).Name = "labelNote";
		labelNote.Orientation = (TextOrientation)1;
		labelNote.UseSystemColors = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DEFDoserPurgeRoutine_EPA10" });
		componentResourceManager.ApplyResources(statusCheckmark, "statusCheckmark");
		((Control)(object)statusCheckmark).Name = "statusCheckmark";
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = status;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = statusCheckmark;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_MeteringUnitFloodRoutine");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
