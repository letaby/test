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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string AcmName = "ACM21T";

	private const string MeduimDutyEngineDefPressureQualifier = "DT_AS014_DEF_Pressure";

	private const string HeavyDutyEngineDefPressureQualifier = "DT_AS110_ADS_DEF_Pressure_2";

	private TableLayoutPanel tableLayoutPanel;

	private Button buttonStart;

	private SeekTimeListView seekTimeListView;

	private Panel panel1;

	private Checkmark checkmarkCanStart;

	private Label labelCanStart;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentADSPumpSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentADSPrimingRequest;

	private DigitalReadoutInstrument digitalReadoutInstrumentADSDosingValveState;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentResults;

	private BarInstrument barInstrumentDefPressure;

	private SharedProcedureSelection sharedProcedureSelection;

	public UserPanel()
	{
		InitializeComponent();
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
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
		((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		SapiManager.GlobalInstance.EquipmentTypeChanged -= GlobalInstance_EquipmentTypeChanged;
		string text = Resources.Message_NoResultAvailable;
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
		UpdateConnectedEquipmentType();
	}

	private void UpdateInstruments(bool isMediumDuty)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (isMediumDuty)
		{
			((SingleInstrumentBase)barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS014_DEF_Pressure");
		}
		else
		{
			((SingleInstrumentBase)barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
		}
		((Control)(object)barInstrumentDefPressure).Refresh();
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
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
		});
		if (val != EquipmentType.Empty)
		{
			UpdateInstruments(IsMediumDuty(((EquipmentType)(ref val)).Name));
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
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
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Expected O, but got Unknown
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0855: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		panel1 = new Panel();
		sharedProcedureSelection = new SharedProcedureSelection();
		labelCanStart = new Label();
		checkmarkCanStart = new Checkmark();
		buttonStart = new Button();
		digitalReadoutInstrumentADSPrimingRequest = new DigitalReadoutInstrument();
		digitalReadoutInstrumentADSDosingValveState = new DigitalReadoutInstrument();
		digitalReadoutInstrumentADSPumpSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
		barInstrumentDefPressure = new BarInstrument();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel).SuspendLayout();
		panel1.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(panel1, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentADSPrimingRequest, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentADSDosingValveState, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentADSPumpSpeed, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentResults, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)barInstrumentDefPressure, 0, 3);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "SCR ADS Self-check";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)(object)seekTimeListView, 7);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.f";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)panel1, 2);
		panel1.Controls.Add((Control)(object)sharedProcedureSelection);
		panel1.Controls.Add(labelCanStart);
		panel1.Controls.Add((Control)(object)checkmarkCanStart);
		panel1.Controls.Add(buttonStart);
		componentResourceManager.ApplyResources(panel1, "panel1");
		panel1.Name = "panel1";
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_SCR_ADS_Self_Check_MY13" });
		componentResourceManager.ApplyResources(labelCanStart, "labelCanStart");
		labelCanStart.Name = "labelCanStart";
		labelCanStart.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentADSPrimingRequest, "digitalReadoutInstrumentADSPrimingRequest");
		digitalReadoutInstrumentADSPrimingRequest.FontGroup = "SCRADSDigitals";
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS079_ADS_priming_request");
		((Control)(object)digitalReadoutInstrumentADSPrimingRequest).Name = "digitalReadoutInstrumentADSPrimingRequest";
		((SingleInstrumentBase)digitalReadoutInstrumentADSPrimingRequest).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentADSDosingValveState, "digitalReadoutInstrumentADSDosingValveState");
		digitalReadoutInstrumentADSDosingValveState.FontGroup = "SCRADSDigitals";
		((SingleInstrumentBase)digitalReadoutInstrumentADSDosingValveState).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentADSDosingValveState).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS011_ADS_dosing_valve_state");
		((Control)(object)digitalReadoutInstrumentADSDosingValveState).Name = "digitalReadoutInstrumentADSDosingValveState";
		((SingleInstrumentBase)digitalReadoutInstrumentADSDosingValveState).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentADSPumpSpeed, "digitalReadoutInstrumentADSPumpSpeed");
		digitalReadoutInstrumentADSPumpSpeed.FontGroup = "SCRADSDigitals";
		((SingleInstrumentBase)digitalReadoutInstrumentADSPumpSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentADSPumpSpeed).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS143_ADS_Pump_Speed");
		((Control)(object)digitalReadoutInstrumentADSPumpSpeed).Name = "digitalReadoutInstrumentADSPumpSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentADSPumpSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "SCRADSDigitals";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS001_Engine_Speed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS077_Vehicle_speed_from_ISP100ms");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
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
		((SingleInstrumentBase)digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SCR_ADS_SelfCheck_Routine_Request_Results_status_of_service_function");
		((Control)(object)digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
		((SingleInstrumentBase)digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrumentDefPressure, "barInstrumentDefPressure");
		barInstrumentDefPressure.FontGroup = "SCRADSBars";
		((SingleInstrumentBase)barInstrumentDefPressure).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS014_DEF_Pressure");
		((Control)(object)barInstrumentDefPressure).Name = "barInstrumentDefPressure";
		((SingleInstrumentBase)barInstrumentDefPressure).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = labelCanStart;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmarkCanStart;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRADSSelf-Check");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		panel1.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
