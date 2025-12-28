using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string AcmName = "ACM21T";

	private const string MeduimDutyEngineDefPressureQualifier = "DT_AS014_DEF_Pressure";

	private const string HeavyDutyEngineDefPressureQualifier = "DT_AS110_ADS_DEF_Pressure_2";

	private const string MediumDutyDEFQuantitySharedProcedureQualifier = "SP_DEFQuantityTest_MY13_MDEG";

	private const string HeavyDutyDEFQuantitySharedProcedureQualifier = "SP_DEFQuantityTest_MY13";

	private Channel channel;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private TableLayoutPanel tableLayoutPanel;

	private Button buttonStart;

	private Checkmark statusCheckmark;

	private SeekTimeListView seekTimeListView;

	private System.Windows.Forms.Label status;

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
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Invalid comparison between Unknown and I4
		if (sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (e.Cancel)
		{
			return;
		}
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		SapiManager.GlobalInstance.EquipmentTypeChanged -= GlobalInstance_EquipmentTypeChanged;
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

	public override void OnChannelsChanged()
	{
		Channel channel = ((CustomPanel)this).GetChannel("ACM21T");
		if (channel != this.channel)
		{
			this.channel = channel;
		}
		UpdateConnectedEquipmentType();
	}

	private void UpdateInstruments(bool isMediumDuty)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (channel != null)
		{
			if (isMediumDuty)
			{
				((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS014_DEF_Pressure");
			}
			else
			{
				((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
			}
			((Control)(object)digitalReadoutInstrumentAirlessDosingUreaPressure).Refresh();
		}
	}

	private void UpdateSharedProcedureSelection(bool isMediumDuty)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { isMediumDuty ? "SP_DEFQuantityTest_MY13_MDEG" : "SP_DEFQuantityTest_MY13" });
	}

	private bool IsMediumDuty(string equipment)
	{
		switch (equipment.ToUpperInvariant())
		{
		case "DD5":
		case "DD8":
		case "MDEG 4-CYLINDER TIER4":
		case "MDEG 6-CYLINDER TIER4":
			return true;
		default:
			return false;
		}
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
		});
		if (val != EquipmentType.Empty)
		{
			flag = IsMediumDuty(((EquipmentType)(ref val)).Name);
			UpdateInstruments(flag);
			UpdateSharedProcedureSelection(flag);
		}
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
		{
			UpdateConnectedEquipmentType();
		}
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
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
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Expected O, but got Unknown
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		tableLayoutPanel = new TableLayoutPanel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		digitalReadoutInstrumentADSPrimingRequest = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEnableAds = new DigitalReadoutInstrument();
		digitalReadoutInstrumentActualQuantity = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDosingQuantity = new DigitalReadoutInstrument();
		digitalReadoutInstrumentAirlessDosingUreaPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentAdBluePumpSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
		buttonStart = new Button();
		statusCheckmark = new Checkmark();
		seekTimeListView = new SeekTimeListView();
		status = new System.Windows.Forms.Label();
		sharedProcedureSelection = new SharedProcedureSelection();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonStart, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)statusCheckmark, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(status, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)sharedProcedureSelection, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 1);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
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
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS079_ADS_priming_request");
		((Control)(object)digitalReadoutInstrumentADSPrimingRequest).Name = "digitalReadoutInstrumentADSPrimingRequest";
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEnableAds, "digitalReadoutInstrumentEnableAds");
		digitalReadoutInstrumentEnableAds.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS002_Enable_ADS");
		((Control)(object)digitalReadoutInstrumentEnableAds).Name = "digitalReadoutInstrumentEnableAds";
		((SingleInstrumentBase)digitalReadoutInstrumentEnableAds).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentActualQuantity, "digitalReadoutInstrumentActualQuantity");
		digitalReadoutInstrumentActualQuantity.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr");
		((Control)(object)digitalReadoutInstrumentActualQuantity).Name = "digitalReadoutInstrumentActualQuantity";
		((SingleInstrumentBase)digitalReadoutInstrumentActualQuantity).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDosingQuantity, "digitalReadoutInstrumentDosingQuantity");
		digitalReadoutInstrumentDosingQuantity.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS104_ADS_Doser_PWM");
		((Control)(object)digitalReadoutInstrumentDosingQuantity).Name = "digitalReadoutInstrumentDosingQuantity";
		((SingleInstrumentBase)digitalReadoutInstrumentDosingQuantity).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAirlessDosingUreaPressure, "digitalReadoutInstrumentAirlessDosingUreaPressure");
		digitalReadoutInstrumentAirlessDosingUreaPressure.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
		((Control)(object)digitalReadoutInstrumentAirlessDosingUreaPressure).Name = "digitalReadoutInstrumentAirlessDosingUreaPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentAirlessDosingUreaPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAdBluePumpSpeed, "digitalReadoutInstrumentAdBluePumpSpeed");
		digitalReadoutInstrumentAdBluePumpSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAdBluePumpSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAdBluePumpSpeed).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS143_ADS_Pump_Speed");
		((Control)(object)digitalReadoutInstrumentAdBluePumpSpeed).Name = "digitalReadoutInstrumentAdBluePumpSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentAdBluePumpSpeed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)(object)digitalReadoutInstrumentResults, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
		digitalReadoutInstrumentResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentResults).FreezeValue = false;
		digitalReadoutInstrumentResults.Gradient.Initialize((ValueState)0, 16);
		digitalReadoutInstrumentResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(6, 5.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(7, 6.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(8, 7.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(9, 8.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(10, 9.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(11, 10.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(12, 11.0, (ValueState)0);
		digitalReadoutInstrumentResults.Gradient.Modify(13, 12.0, (ValueState)1);
		digitalReadoutInstrumentResults.Gradient.Modify(14, 13.0, (ValueState)3);
		digitalReadoutInstrumentResults.Gradient.Modify(15, 14.0, (ValueState)3);
		digitalReadoutInstrumentResults.Gradient.Modify(16, 15.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SCR_Dosing_Quantity_Check_Request_Results_status_of_service_function");
		((Control)(object)digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
		((SingleInstrumentBase)digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
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
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[2] { "SP_DEFQuantityTest_MY13", "SP_DEFQuantityTest_MY13_MDEG" });
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
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
