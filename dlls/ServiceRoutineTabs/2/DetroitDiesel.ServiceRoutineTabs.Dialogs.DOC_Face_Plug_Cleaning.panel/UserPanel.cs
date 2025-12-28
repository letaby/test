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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning.panel;

public class UserPanel : CustomPanel
{
	private const string CpcName = "CPC04T";

	private const string SsamName = "SSAM02T";

	private const string AcmName = "ACM21T";

	private const string DpfRegenSwitchStatusMy13Qualifier = "DT_DSL_DPF_Regen_Switch_Status";

	private const string DpfRegenSwitchStatusNGCQualifier = "RT_MSC_GetSwState_Start_Switch_033";

	private const string DefPressureMdegQualifier = "DT_AS014_DEF_Pressure";

	private const string DefPressureHdepQualifier = "DT_AS110_ADS_DEF_Pressure_2";

	private TableLayoutPanel tableLayoutPanel1;

	private BarInstrument barInstrument3;

	private BarInstrument barInstrument4;

	private BarInstrument barInstrument5;

	private BarInstrument barInstrument6;

	private BarInstrument barInstrument7;

	private BarInstrument barInstrument8;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private Button button1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private System.Windows.Forms.Label LabelDocFacePlugUnclogging;

	private BarInstrument barInstrument2;

	private SeekTimeListView DocFacePlugUncloggingResults;

	private Checkmark checkmark1;

	private DigitalReadoutInstrument dpfRegenSwitchStatus;

	private System.Windows.Forms.Label commandLabel;

	private DigitalReadoutInstrument digitalReadoutInstrumentResults;

	private BarInstrument barInstrument1;

	private BarInstrument barInstrument9;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private BarInstrument barInstrumentDefPressure;

	private TableLayoutPanel tableLayoutPanel2;

	private SharedProcedureCreatorComponent DocFacePlugUnclogging;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument ActualDpfZone;

	public static bool IsMediumDuty { get; set; }

	public UserPanel()
	{
		InitializeComponent();
		ActualDpfZone.RepresentedStateChanged += ActualDpfZone_RepresentedStateChanged;
		dpfRegenSwitchStatus.RepresentedStateChanged += dpfRegenSwitchStatus_RepresentedStateChanged;
		sharedProcedureIntegrationComponent1.StartStopButton.Click += StartStopButton_Click;
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((CustomPanel)this).OnChannelsChanged();
	}

	private void StartStopButton_Click(object sender, EventArgs e)
	{
		commandLabel.Text = "";
	}

	private static bool IsMediumDutyEquipment(EquipmentType equipment)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (equipment != EquipmentType.Empty)
		{
			switch (((EquipmentType)(ref equipment)).Name.ToUpperInvariant())
			{
			case "DD5":
			case "DD8":
			case "MDEG 4-CYLINDER TIER4":
			case "MDEG 6-CYLINDER TIER4":
				return true;
			}
		}
		return false;
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EquipmentType equipment = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
		});
		UpdateDEFPressureInstrument(IsMediumDutyEquipment(equipment));
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
		{
			UpdateConnectedEquipmentType();
		}
	}

	private void ActualDpfZone_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		if ((int)ActualDpfZone.RepresentedState == 1 && (int)sharedProcedureIntegrationComponent1.ProceduresDropDown.SelectedProcedure.State == 2)
		{
			commandLabel.Text = Resources.Message_PressAndHoldTheRegenSwitchOnTheDashForFiveSecondsToStartTheRegen;
		}
	}

	private void dpfRegenSwitchStatus_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		if ((int)dpfRegenSwitchStatus.RepresentedState == 1 && (int)sharedProcedureIntegrationComponent1.ProceduresDropDown.SelectedProcedure.State == 2)
		{
			commandLabel.Text = "";
		}
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		if (sharedProcedureSelection1.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (e.Cancel)
		{
			return;
		}
		((ContainerControl)this).ParentForm.FormClosing -= this_ParentFormClosing;
		string text = Resources.Message_NotAvailable;
		bool flag = false;
		if (sharedProcedureSelection1.SelectedProcedure != null)
		{
			flag = (int)sharedProcedureSelection1.SelectedProcedure.Result == 1;
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
		UpdateDpfRegenSwitchInstrument();
	}

	private void UpdateDpfRegenSwitchInstrument()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Channel channel = ((CustomPanel)this).GetChannel("SSAM02T");
		if (channel != null)
		{
			((SingleInstrumentBase)dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "RT_MSC_GetSwState_Start_Switch_033");
		}
		else
		{
			((SingleInstrumentBase)dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status");
		}
		((Control)(object)dpfRegenSwitchStatus).Refresh();
	}

	private void UpdateDEFPressureInstrument(bool isMediumDuty)
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

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Expected O, but got Unknown
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_100d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10be: Unknown result type (might be due to invalid IL or missing references)
		//IL_116f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1220: Unknown result type (might be due to invalid IL or missing references)
		//IL_1331: Unknown result type (might be due to invalid IL or missing references)
		//IL_142a: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_152b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1555: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		checkmark1 = new Checkmark();
		LabelDocFacePlugUnclogging = new System.Windows.Forms.Label();
		commandLabel = new System.Windows.Forms.Label();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DocFacePlugUncloggingResults = new SeekTimeListView();
		digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
		dpfRegenSwitchStatus = new DigitalReadoutInstrument();
		ActualDpfZone = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		barInstrumentDefPressure = new BarInstrument();
		barInstrument2 = new BarInstrument();
		barInstrument1 = new BarInstrument();
		barInstrument9 = new BarInstrument();
		barInstrument3 = new BarInstrument();
		barInstrument5 = new BarInstrument();
		barInstrument6 = new BarInstrument();
		barInstrument7 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		barInstrument4 = new BarInstrument();
		button1 = new Button();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		DocFacePlugUnclogging = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DocFacePlugUncloggingResults, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentResults, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dpfRegenSwitchStatus, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)ActualDpfZone, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentDefPressure, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument2, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument1, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument9, 6, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument3, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument5, 7, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument6, 6, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument7, 5, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument4, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button1, 7, 8);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection1, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(LabelDocFacePlugUnclogging, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(commandLabel, 1, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DocFacePlugUnclogging" });
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel2).SetRowSpan((Control)(object)checkmark1, 2);
		componentResourceManager.ApplyResources(LabelDocFacePlugUnclogging, "LabelDocFacePlugUnclogging");
		LabelDocFacePlugUnclogging.Name = "LabelDocFacePlugUnclogging";
		LabelDocFacePlugUnclogging.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(commandLabel, "commandLabel");
		commandLabel.Name = "commandLabel";
		commandLabel.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument6, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS143_ADS_Pump_Speed");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument7, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "engineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS094_Actual_Torque_Load");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)DocFacePlugUncloggingResults, 6);
		componentResourceManager.ApplyResources(DocFacePlugUncloggingResults, "DocFacePlugUncloggingResults");
		DocFacePlugUncloggingResults.FilterUserLabels = true;
		((Control)(object)DocFacePlugUncloggingResults).Name = "DocFacePlugUncloggingResults";
		DocFacePlugUncloggingResults.RequiredUserLabelPrefix = "DOC Face Plug Cleaning";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)DocFacePlugUncloggingResults, 3);
		DocFacePlugUncloggingResults.SelectedTime = null;
		DocFacePlugUncloggingResults.ShowChannelLabels = false;
		DocFacePlugUncloggingResults.ShowCommunicationsState = false;
		DocFacePlugUncloggingResults.ShowControlPanel = false;
		DocFacePlugUncloggingResults.ShowDeviceColumn = false;
		DocFacePlugUncloggingResults.TimeFormat = "HH:mm:ss";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentResults, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
		digitalReadoutInstrumentResults.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrumentResults).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status");
		((Control)(object)digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
		((SingleInstrumentBase)digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)dpfRegenSwitchStatus, 2);
		componentResourceManager.ApplyResources(dpfRegenSwitchStatus, "dpfRegenSwitchStatus");
		dpfRegenSwitchStatus.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)dpfRegenSwitchStatus).FreezeValue = false;
		dpfRegenSwitchStatus.Gradient.Initialize((ValueState)0, 4);
		dpfRegenSwitchStatus.Gradient.Modify(1, 0.0, (ValueState)2);
		dpfRegenSwitchStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		dpfRegenSwitchStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		dpfRegenSwitchStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status");
		((Control)(object)dpfRegenSwitchStatus).Name = "dpfRegenSwitchStatus";
		((SingleInstrumentBase)dpfRegenSwitchStatus).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ActualDpfZone, 2);
		componentResourceManager.ApplyResources(ActualDpfZone, "ActualDpfZone");
		ActualDpfZone.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)ActualDpfZone).FreezeValue = false;
		ActualDpfZone.Gradient.Initialize((ValueState)2, 2);
		ActualDpfZone.Gradient.Modify(1, 2.0, (ValueState)1);
		ActualDpfZone.Gradient.Modify(2, 3.0, (ValueState)2);
		((SingleInstrumentBase)ActualDpfZone).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS065_Actual_DPF_zone");
		((Control)(object)ActualDpfZone).Name = "ActualDpfZone";
		((SingleInstrumentBase)ActualDpfZone).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument3, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS069_Jake_Brake_1_PWM13");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument4, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS036_SCR_Inlet_NOx_Sensor");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument5, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS035_SCR_Outlet_NOx_Sensor");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		barInstrumentDefPressure.BarOrientation = (ControlOrientation)1;
		componentResourceManager.ApplyResources(barInstrumentDefPressure, "barInstrumentDefPressure");
		barInstrumentDefPressure.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrumentDefPressure).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
		((Control)(object)barInstrumentDefPressure).Name = "barInstrumentDefPressure";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrumentDefPressure, 3);
		((SingleInstrumentBase)barInstrumentDefPressure).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentDefPressure).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentDefPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument2, 2);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument1, 2);
		componentResourceManager.ApplyResources(barInstrument1, "barInstrument1");
		barInstrument1.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument1).FreezeValue = false;
		((SingleInstrumentBase)barInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)barInstrument1).Name = "barInstrument1";
		((SingleInstrumentBase)barInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument9, 2);
		componentResourceManager.ApplyResources(barInstrument9, "barInstrument9");
		barInstrument9.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument9).FreezeValue = false;
		((SingleInstrumentBase)barInstrument9).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value");
		((Control)(object)barInstrument9).Name = "barInstrument9";
		((SingleInstrumentBase)barInstrument9).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument3, 2);
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		barInstrument5.BarOrientation = (ControlOrientation)1;
		barInstrument5.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument5, "barInstrument5");
		barInstrument5.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument5).FreezeValue = false;
		((SingleInstrumentBase)barInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)barInstrument5).Name = "barInstrument5";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument5, 3);
		((SingleInstrumentBase)barInstrument5).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument5).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument5).UnitAlignment = StringAlignment.Near;
		barInstrument6.BarOrientation = (ControlOrientation)1;
		barInstrument6.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument6, "barInstrument6");
		barInstrument6.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument6).FreezeValue = false;
		((SingleInstrumentBase)barInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)barInstrument6).Name = "barInstrument6";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument6, 3);
		((SingleInstrumentBase)barInstrument6).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument6).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument6).UnitAlignment = StringAlignment.Near;
		barInstrument7.BarOrientation = (ControlOrientation)1;
		barInstrument7.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument7, "barInstrument7");
		barInstrument7.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument7).FreezeValue = false;
		((SingleInstrumentBase)barInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS018_SCR_Inlet_Temperature");
		((Control)(object)barInstrument7).Name = "barInstrument7";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument7, 3);
		((SingleInstrumentBase)barInstrument7).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument7).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument7).UnitAlignment = StringAlignment.Near;
		barInstrument8.BarOrientation = (ControlOrientation)1;
		barInstrument8.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS019_SCR_Outlet_Temperature");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument8, 3);
		((SingleInstrumentBase)barInstrument8).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument8).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		barInstrument4.BarOrientation = (ControlOrientation)1;
		barInstrument4.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument4, "barInstrument4");
		barInstrument4.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument4).FreezeValue = false;
		((SingleInstrumentBase)barInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)barInstrument4).Name = "barInstrument4";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument4, 3);
		((SingleInstrumentBase)barInstrument4).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = LabelDocFacePlugUnclogging;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = button1;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		DocFacePlugUnclogging.Suspend();
		DocFacePlugUnclogging.MonitorCall = new ServiceCall("ACM21T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status");
		DocFacePlugUnclogging.MonitorGradient.Initialize((ValueState)0, 6);
		DocFacePlugUnclogging.MonitorGradient.Modify(1, 0.0, (ValueState)3);
		DocFacePlugUnclogging.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		DocFacePlugUnclogging.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		DocFacePlugUnclogging.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		DocFacePlugUnclogging.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		DocFacePlugUnclogging.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		componentResourceManager.ApplyResources(DocFacePlugUnclogging, "DocFacePlugUnclogging");
		DocFacePlugUnclogging.Qualifier = "SP_DocFacePlugUnclogging";
		DocFacePlugUnclogging.StartCall = new ServiceCall("ACM21T", "RT_DOC_Face_Plug_Unclogging_Start");
		val.Gradient.Initialize((ValueState)0, 4);
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)3);
		val.Gradient.Modify(4, 3.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		val2.Gradient.Initialize((ValueState)3, 1, "rpm");
		val2.Gradient.Modify(1, 550.0, (ValueState)1);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		DocFacePlugUnclogging.StartConditions.Add(val);
		DocFacePlugUnclogging.StartConditions.Add(val2);
		DocFacePlugUnclogging.StopCall = new ServiceCall("ACM21T", "RT_DOC_Face_Plug_Unclogging_Stop");
		DocFacePlugUnclogging.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DOCFacePlugCleaning");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
