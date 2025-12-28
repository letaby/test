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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel1;

	private BarInstrument barInstrument3;

	private BarInstrument barInstrument4;

	private BarInstrument barInstrument5;

	private BarInstrument barInstrument6;

	private BarInstrument barInstrument7;

	private BarInstrument barInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private BarInstrument barInstrument9;

	private BarInstrument barInstrument1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private SharedProcedureCreatorComponent DocFacePlugUncloggingEPA10;

	private Button button1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private Label LabelDocFacePlugUnclogging;

	private DialInstrument dialInstrument1;

	private BarInstrument barInstrument2;

	private SeekTimeListView DocFacePlugUncloggingResults;

	private Checkmark checkmark1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument dpfRegenSwitchStatus;

	private Label commandLabel;

	private DigitalReadoutInstrument digitalReadoutInstrumentResults;

	private DigitalReadoutInstrument ActualDpfZone;

	public UserPanel()
	{
		InitializeComponent();
		ActualDpfZone.RepresentedStateChanged += ActualDpfZone_RepresentedStateChanged;
		dpfRegenSwitchStatus.RepresentedStateChanged += dpfRegenSwitchStatus_RepresentedStateChanged;
		sharedProcedureIntegrationComponent1.StartStopButton.Click += StartStopButton_Click;
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
	}

	private void StartStopButton_Click(object sender, EventArgs e)
	{
		commandLabel.Text = "";
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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
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
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff3: Expected O, but got Unknown
		//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_122f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1270: Unknown result type (might be due to invalid IL or missing references)
		//IL_12af: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d9: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		tableLayoutPanel1 = new TableLayoutPanel();
		dialInstrument1 = new DialInstrument();
		barInstrument9 = new BarInstrument();
		barInstrument1 = new BarInstrument();
		barInstrument2 = new BarInstrument();
		barInstrument3 = new BarInstrument();
		barInstrument4 = new BarInstrument();
		barInstrument5 = new BarInstrument();
		barInstrument6 = new BarInstrument();
		barInstrument7 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		button1 = new Button();
		LabelDocFacePlugUnclogging = new Label();
		DocFacePlugUncloggingResults = new SeekTimeListView();
		ActualDpfZone = new DigitalReadoutInstrument();
		dpfRegenSwitchStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
		checkmark1 = new Checkmark();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		commandLabel = new Label();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		DocFacePlugUncloggingEPA10 = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument9, 7, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument1, 8, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument2, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument3, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument4, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument5, 5, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument6, 6, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument7, 7, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 8, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button1, 8, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(LabelDocFacePlugUnclogging, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DocFacePlugUncloggingResults, 3, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)ActualDpfZone, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dpfRegenSwitchStatus, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 5, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 5, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentResults, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(commandLabel, 1, 7);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		dialInstrument1.AngleRange = 135.0;
		dialInstrument1.AngleStart = -180.0;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)dialInstrument1, 4);
		componentResourceManager.ApplyResources(dialInstrument1, "dialInstrument1");
		dialInstrument1.FontGroup = "engineSpeed";
		((SingleInstrumentBase)dialInstrument1).FreezeValue = false;
		((SingleInstrumentBase)dialInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)dialInstrument1).Name = "dialInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)dialInstrument1, 3);
		((SingleInstrumentBase)dialInstrument1).UnitAlignment = StringAlignment.Near;
		barInstrument9.BarOrientation = (ControlOrientation)1;
		componentResourceManager.ApplyResources(barInstrument9, "barInstrument9");
		barInstrument9.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument9).FreezeValue = false;
		((SingleInstrumentBase)barInstrument9).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS033_Throttle_Valve_Commanded_Value");
		((Control)(object)barInstrument9).Name = "barInstrument9";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument9, 3);
		((SingleInstrumentBase)barInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument9).UnitAlignment = StringAlignment.Near;
		barInstrument1.BarOrientation = (ControlOrientation)1;
		componentResourceManager.ApplyResources(barInstrument1, "barInstrument1");
		barInstrument1.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument1).FreezeValue = false;
		((SingleInstrumentBase)barInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)barInstrument1).Name = "barInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument1, 3);
		((SingleInstrumentBase)barInstrument1).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument1).UnitAlignment = StringAlignment.Near;
		barInstrument2.BarOrientation = (ControlOrientation)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument2, 2);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS005_DOC_Inlet_Pressure");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((SingleInstrumentBase)barInstrument2).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument2).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		barInstrument3.BarOrientation = (ControlOrientation)1;
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((SingleInstrumentBase)barInstrument3).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument3).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		barInstrument4.BarOrientation = (ControlOrientation)1;
		barInstrument4.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument4, "barInstrument4");
		barInstrument4.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument4).FreezeValue = false;
		((SingleInstrumentBase)barInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)barInstrument4).Name = "barInstrument4";
		((SingleInstrumentBase)barInstrument4).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument4).UnitAlignment = StringAlignment.Near;
		barInstrument5.BarOrientation = (ControlOrientation)1;
		barInstrument5.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument5, "barInstrument5");
		barInstrument5.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument5).FreezeValue = false;
		((SingleInstrumentBase)barInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)barInstrument5).Name = "barInstrument5";
		((SingleInstrumentBase)barInstrument5).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument5).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument5).UnitAlignment = StringAlignment.Near;
		barInstrument6.BarOrientation = (ControlOrientation)1;
		barInstrument6.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument6, "barInstrument6");
		barInstrument6.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument6).FreezeValue = false;
		((SingleInstrumentBase)barInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)barInstrument6).Name = "barInstrument6";
		((SingleInstrumentBase)barInstrument6).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument6).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument6).UnitAlignment = StringAlignment.Near;
		barInstrument7.BarOrientation = (ControlOrientation)1;
		barInstrument7.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument7, "barInstrument7");
		barInstrument7.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument7).FreezeValue = false;
		((SingleInstrumentBase)barInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature");
		((Control)(object)barInstrument7).Name = "barInstrument7";
		((SingleInstrumentBase)barInstrument7).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument7).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument7).UnitAlignment = StringAlignment.Near;
		barInstrument8.BarOrientation = (ControlOrientation)1;
		barInstrument8.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "Temp";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((SingleInstrumentBase)barInstrument8).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument8).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)button1, 2);
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)LabelDocFacePlugUnclogging, 6);
		componentResourceManager.ApplyResources(LabelDocFacePlugUnclogging, "LabelDocFacePlugUnclogging");
		LabelDocFacePlugUnclogging.Name = "LabelDocFacePlugUnclogging";
		LabelDocFacePlugUnclogging.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)DocFacePlugUncloggingResults, 6);
		componentResourceManager.ApplyResources(DocFacePlugUncloggingResults, "DocFacePlugUncloggingResults");
		DocFacePlugUncloggingResults.FilterUserLabels = true;
		((Control)(object)DocFacePlugUncloggingResults).Name = "DocFacePlugUncloggingResults";
		DocFacePlugUncloggingResults.RequiredUserLabelPrefix = "DOC Face Plug Cleaning EPA10";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)DocFacePlugUncloggingResults, 3);
		DocFacePlugUncloggingResults.SelectedTime = null;
		DocFacePlugUncloggingResults.ShowChannelLabels = false;
		DocFacePlugUncloggingResults.ShowCommunicationsState = false;
		DocFacePlugUncloggingResults.ShowControlPanel = false;
		DocFacePlugUncloggingResults.ShowDeviceColumn = false;
		DocFacePlugUncloggingResults.TimeFormat = "HH:mm:ss";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ActualDpfZone, 3);
		componentResourceManager.ApplyResources(ActualDpfZone, "ActualDpfZone");
		ActualDpfZone.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)ActualDpfZone).FreezeValue = false;
		ActualDpfZone.Gradient.Initialize((ValueState)2, 2);
		ActualDpfZone.Gradient.Modify(1, 2.0, (ValueState)1);
		ActualDpfZone.Gradient.Modify(2, 3.0, (ValueState)2);
		((SingleInstrumentBase)ActualDpfZone).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS065_Actual_DPF_zone");
		((Control)(object)ActualDpfZone).Name = "ActualDpfZone";
		((SingleInstrumentBase)ActualDpfZone).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)dpfRegenSwitchStatus, 3);
		componentResourceManager.ApplyResources(dpfRegenSwitchStatus, "dpfRegenSwitchStatus");
		dpfRegenSwitchStatus.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)dpfRegenSwitchStatus).FreezeValue = false;
		dpfRegenSwitchStatus.Gradient.Initialize((ValueState)0, 4);
		dpfRegenSwitchStatus.Gradient.Modify(1, 0.0, (ValueState)2);
		dpfRegenSwitchStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		dpfRegenSwitchStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		dpfRegenSwitchStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes)1, "CPC02T", "DT_DSL_DPF_Regen_Switch_Status");
		((Control)(object)dpfRegenSwitchStatus).Name = "dpfRegenSwitchStatus";
		((SingleInstrumentBase)dpfRegenSwitchStatus).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument3, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS069_Jake_Brake_1_PWM13");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS094_Actual_Torque_Load");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "DigitalReadOut";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentResults, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
		digitalReadoutInstrumentResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentResults).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes)0, (string)null, (string)null);
		((Control)(object)digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
		((SingleInstrumentBase)digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)checkmark1, 2);
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DocFacePlugUncloggingEPA10" });
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)commandLabel, 6);
		componentResourceManager.ApplyResources(commandLabel, "commandLabel");
		commandLabel.Name = "commandLabel";
		commandLabel.UseCompatibleTextRendering = true;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = LabelDocFacePlugUnclogging;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = button1;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		DocFacePlugUncloggingEPA10.Suspend();
		DocFacePlugUncloggingEPA10.MonitorCall = new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status");
		DocFacePlugUncloggingEPA10.MonitorGradient.Initialize((ValueState)0, 6);
		DocFacePlugUncloggingEPA10.MonitorGradient.Modify(1, 0.0, (ValueState)3);
		DocFacePlugUncloggingEPA10.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		DocFacePlugUncloggingEPA10.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		DocFacePlugUncloggingEPA10.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		DocFacePlugUncloggingEPA10.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		DocFacePlugUncloggingEPA10.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		componentResourceManager.ApplyResources(DocFacePlugUncloggingEPA10, "DocFacePlugUncloggingEPA10");
		DocFacePlugUncloggingEPA10.Qualifier = "SP_DocFacePlugUncloggingEPA10";
		DocFacePlugUncloggingEPA10.StartCall = new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Start");
		val.Gradient.Initialize((ValueState)0, 4);
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)3);
		val.Gradient.Modify(4, 3.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
		val2.Gradient.Initialize((ValueState)3, 1, "rpm");
		val2.Gradient.Modify(1, 550.0, (ValueState)1);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		DocFacePlugUncloggingEPA10.StartConditions.Add(val);
		DocFacePlugUncloggingEPA10.StartConditions.Add(val2);
		DocFacePlugUncloggingEPA10.StopCall = new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Stop");
		DocFacePlugUncloggingEPA10.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DOCFacePlugCleaning");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
