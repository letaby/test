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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__Euro5_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private TableLayoutPanel tableLayoutPanel;

	private Button buttonStart;

	private Checkmark statusCheckmark;

	private SeekTimeListView seekTimeListView;

	private Label status;

	private SharedProcedureSelection sharedProcedureSelection;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private DigitalReadoutInstrument digitalReadoutInstrumentADSPrimingRequest;

	private DigitalReadoutInstrument digitalReadoutInstrumentEnableAds;

	private DigitalReadoutInstrument digitalReadoutInstrumentActualQuantity;

	private DigitalReadoutInstrument digitalReadoutInstrumentDosingQuantity;

	private DigitalReadoutInstrument digitalReadoutInstrumentAirlessDosingUreaPressure;

	private DigitalReadoutInstrument digitalReadoutInstrumentResults;

	private DigitalReadoutInstrument digitalReadoutInstrumentAdBluePumpSpeed;

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
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		if (sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (e.Cancel)
		{
			return;
		}
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		string text = Resources.Message_NoResultsAvailable;
		bool flag = false;
		if (sharedProcedureSelection.SelectedProcedure != null)
		{
			flag = (int)sharedProcedureSelection.SelectedProcedure.Result == 1;
			if (((SingleInstrumentBase)digitalReadoutInstrumentResults).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentResults).DataItem.Value != null)
			{
				text = ((SingleInstrumentBase)digitalReadoutInstrumentResults).DataItem.Value.ToString();
			}
		}
		((Control)this).Tag = new object[2] { flag, text };
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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0912: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b75: Expected O, but got Unknown
		//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelInstruments = new TableLayoutPanel();
		digitalReadoutInstrumentADSPrimingRequest = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEnableAds = new DigitalReadoutInstrument();
		digitalReadoutInstrumentActualQuantity = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDosingQuantity = new DigitalReadoutInstrument();
		digitalReadoutInstrumentAirlessDosingUreaPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentAdBluePumpSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
		tableLayoutPanel = new TableLayoutPanel();
		buttonStart = new Button();
		statusCheckmark = new Checkmark();
		seekTimeListView = new SeekTimeListView();
		status = new Label();
		sharedProcedureSelection = new SharedProcedureSelection();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutPanelInstruments, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentADSPrimingRequest, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentEnableAds, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentActualQuantity, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentDosingQuantity, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentAirlessDosingUreaPressure, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentAdBluePumpSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentResults, 0, 3);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentADSPrimingRequest, "digitalReadoutInstrumentADSPrimingRequest");
		digitalReadoutInstrumentADSPrimingRequest.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).FreezeValue = false;
		digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentADSPrimingRequest.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_ADS_Priming_Request");
		((Control)(object)digitalReadoutInstrumentADSPrimingRequest).Name = "digitalReadoutInstrumentADSPrimingRequest";
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEnableAds, "digitalReadoutInstrumentEnableAds");
		digitalReadoutInstrumentEnableAds.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).FreezeValue = false;
		digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentEnableAds.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentEnableAds.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentEnableAds.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentEnableAds.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentEnableAds.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_ADS_Pressure_dosing_enable_UPS");
		((Control)(object)digitalReadoutInstrumentEnableAds).Name = "digitalReadoutInstrumentEnableAds";
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentActualQuantity, "digitalReadoutInstrumentActualQuantity");
		digitalReadoutInstrumentActualQuantity.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Actual_DEF_Dosing_Quantity");
		((Control)(object)digitalReadoutInstrumentActualQuantity).Name = "digitalReadoutInstrumentActualQuantity";
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDosingQuantity, "digitalReadoutInstrumentDosingQuantity");
		digitalReadoutInstrumentDosingQuantity.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).FreezeValue = false;
		digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
		digitalReadoutInstrumentDosingQuantity.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentDosingQuantity.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentDosingQuantity.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentDosingQuantity.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentDosingQuantity.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_ADS_Status_DEF_pump");
		((Control)(object)digitalReadoutInstrumentDosingQuantity).Name = "digitalReadoutInstrumentDosingQuantity";
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAirlessDosingUreaPressure, "digitalReadoutInstrumentAirlessDosingUreaPressure");
		digitalReadoutInstrumentAirlessDosingUreaPressure.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_DEF_Pressure");
		((Control)(object)digitalReadoutInstrumentAirlessDosingUreaPressure).Name = "digitalReadoutInstrumentAirlessDosingUreaPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAdBluePumpSpeed, "digitalReadoutInstrumentAdBluePumpSpeed");
		digitalReadoutInstrumentAdBluePumpSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAdBluePumpSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAdBluePumpSpeed).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Urea_Pump_Speed");
		((Control)(object)digitalReadoutInstrumentAdBluePumpSpeed).Name = "digitalReadoutInstrumentAdBluePumpSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentAdBluePumpSpeed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)(object)digitalReadoutInstrumentResults, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
		digitalReadoutInstrumentResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentResults).FreezeValue = false;
		digitalReadoutInstrumentResults.Gradient.Initialize((ValueState)0, 8);
		digitalReadoutInstrumentResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(4, 11.0, (ValueState)3);
		digitalReadoutInstrumentResults.Gradient.Modify(5, 12.0, (ValueState)1);
		digitalReadoutInstrumentResults.Gradient.Modify(6, 13.0, (ValueState)3);
		digitalReadoutInstrumentResults.Gradient.Modify(7, 14.0, (ValueState)3);
		digitalReadoutInstrumentResults.Gradient.Modify(8, 15.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes)64, "MR201T", "RT_SR029D_EDU_Diagnosis_Routine_Request_Results_Urea_Quantity_Check");
		((Control)(object)digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
		((SingleInstrumentBase)digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonStart, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)statusCheckmark, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(status, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)sharedProcedureSelection, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 1);
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
		componentResourceManager.ApplyResources(status, "status");
		status.Name = "status";
		status.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DEFQuantityTest_MR2" });
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
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
