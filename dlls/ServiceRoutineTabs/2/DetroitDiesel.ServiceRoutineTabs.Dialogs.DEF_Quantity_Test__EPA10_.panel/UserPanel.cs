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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private TableLayoutPanel tableLayoutPanel;

	private Button buttonStart;

	private Checkmark statusCheckmark;

	private SeekTimeListView seekTimeListView;

	private Label labelNote;

	private Label status;

	private SharedProcedureSelection sharedProcedureSelection;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private BarInstrument barInstrumentDEFPressure;

	private BarInstrument barInstrumentDEFAirPressure;

	private DigitalReadoutInstrument digitalReadoutInstrumentActualQuantity;

	private DigitalReadoutInstrument digitalReadoutInstrumentDosingQuantity;

	private TableLayoutPanel tableLayoutPanel3;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		if (sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			string text = Resources.Message_NoResultsAvailable;
			bool flag = false;
			if (sharedProcedureSelection.SelectedProcedure != null)
			{
				flag = (int)sharedProcedureSelection.SelectedProcedure.Result == 1;
				text = (flag ? Resources.Message_DEFDosingQuantityCheckCompleted : Resources.Message_DEFDosingQuantityCheckFailedOrTerminatedWithUnknownResult);
			}
			((Control)this).Tag = new object[2] { flag, text };
		}
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
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
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Expected O, but got Unknown
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanel = new TableLayoutPanel();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		tableLayoutPanel3 = new TableLayoutPanel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		barInstrumentDEFPressure = new BarInstrument();
		barInstrumentDEFAirPressure = new BarInstrument();
		digitalReadoutInstrumentActualQuantity = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDosingQuantity = new DigitalReadoutInstrument();
		buttonStart = new Button();
		statusCheckmark = new Checkmark();
		seekTimeListView = new SeekTimeListView();
		labelNote = new Label();
		status = new Label();
		sharedProcedureSelection = new SharedProcedureSelection();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrument3, 2, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS003_Enable_DEF_pump");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutPanelInstruments, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)barInstrumentDEFPressure, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)barInstrumentDEFAirPressure, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentActualQuantity, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentDosingQuantity, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanel3, 0, 0);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)(object)barInstrumentDEFPressure, 2);
		componentResourceManager.ApplyResources(barInstrumentDEFPressure, "barInstrumentDEFPressure");
		barInstrumentDEFPressure.FontGroup = null;
		((SingleInstrumentBase)barInstrumentDEFPressure).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDEFPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure");
		((Control)(object)barInstrumentDEFPressure).Name = "barInstrumentDEFPressure";
		((SingleInstrumentBase)barInstrumentDEFPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)(object)barInstrumentDEFAirPressure, 2);
		componentResourceManager.ApplyResources(barInstrumentDEFAirPressure, "barInstrumentDEFAirPressure");
		barInstrumentDEFAirPressure.FontGroup = null;
		((SingleInstrumentBase)barInstrumentDEFAirPressure).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDEFAirPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
		((Control)(object)barInstrumentDEFAirPressure).Name = "barInstrumentDEFAirPressure";
		((SingleInstrumentBase)barInstrumentDEFAirPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentActualQuantity, "digitalReadoutInstrumentActualQuantity");
		digitalReadoutInstrumentActualQuantity.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity");
		((Control)(object)digitalReadoutInstrumentActualQuantity).Name = "digitalReadoutInstrumentActualQuantity";
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDosingQuantity, "digitalReadoutInstrumentDosingQuantity");
		digitalReadoutInstrumentDosingQuantity.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity");
		((Control)(object)digitalReadoutInstrumentDosingQuantity).Name = "digitalReadoutInstrumentDosingQuantity";
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonStart, 4, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)statusCheckmark, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)labelNote, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(status, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)sharedProcedureSelection, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 2);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(statusCheckmark, "statusCheckmark");
		((Control)(object)statusCheckmark).Name = "statusCheckmark";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)seekTimeListView, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "DEF Quantity Test";
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
		componentResourceManager.ApplyResources(status, "status");
		status.Name = "status";
		status.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DEFQuantityTest_EPA10" });
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = status;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = statusCheckmark;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DEFQuantityTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
