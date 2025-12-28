using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Left_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
	private Channel channel;

	private Service calibrateRequestResultsProgressService;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private TableLayoutPanel tableLayoutPanelButtons;

	private ProgressBar progressBarResultsProgress;

	private SeekTimeListView seekTimeListViewLog;

	private TextBox textBoxInstructions;

	private DigitalReadoutInstrument digitalReadoutInstrumentRequestResultsStatus;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private Checkmark checkmarkStatus;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponent;

	private System.Windows.Forms.Label labelStatus;

	private Button buttonStartStop;

	private SharedProcedureSelection sharedProcedureSelection;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private System.Windows.Forms.Label labelTitle;

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("SRRL01T"));
	}

	private void SetChannel(Channel mpcChannel)
	{
		if (channel != null)
		{
			calibrateRequestResultsProgressService = null;
		}
		channel = mpcChannel;
		if (channel != null)
		{
			calibrateRequestResultsProgressService = channel.Services["RT_Service_Alignment_Azimuth_Request_Results_Routine_Progress"];
		}
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewLog.RequiredUserLabelPrefix, text);
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			SetChannel(null);
		}
	}

	private void sharedProcedureCreatorComponent_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)((MonitorResultEventArgs)e).Action != 0 || !(calibrateRequestResultsProgressService != null))
		{
			return;
		}
		try
		{
			calibrateRequestResultsProgressService.Execute(synchronous: true);
			if (!(calibrateRequestResultsProgressService.OutputValues[0].Value is string))
			{
				progressBarResultsProgress.Value = Convert.ToInt32(calibrateRequestResultsProgressService.OutputValues[0].Value);
			}
		}
		catch (CaesarException)
		{
		}
	}

	private void sharedProcedureCreatorComponent_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		if (val != null && (int)val.Result == 1 && channel != null)
		{
			LogText(Resources.Message_ResettingFaultCodes);
			channel.FaultCodes.Reset(synchronous: false);
		}
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Expected O, but got Unknown
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		tableLayoutPanelButtons = new TableLayoutPanel();
		checkmarkStatus = new Checkmark();
		labelStatus = new System.Windows.Forms.Label();
		buttonStartStop = new Button();
		sharedProcedureSelection = new SharedProcedureSelection();
		progressBarResultsProgress = new ProgressBar();
		seekTimeListViewLog = new SeekTimeListView();
		labelTitle = new System.Windows.Forms.Label();
		textBoxInstructions = new TextBox();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		digitalReadoutInstrumentRequestResultsStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		sharedProcedureCreatorComponent = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanelButtons).SuspendLayout();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelButtons, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(progressBarResultsProgress, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelTitle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(textBoxInstructions, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(tableLayoutPanelButtons, "tableLayoutPanelButtons");
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(labelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(buttonStartStop, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)sharedProcedureSelection, 2, 0);
		((Control)(object)tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseCompatibleTextRendering = true;
		buttonStartStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_SRRLCalibration" });
		componentResourceManager.ApplyResources(progressBarResultsProgress, "progressBarResultsProgress");
		progressBarResultsProgress.Name = "progressBarResultsProgress";
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "SRRL-Calibration";
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		seekTimeListViewLog.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(labelTitle, "labelTitle");
		labelTitle.Name = "labelTitle";
		labelTitle.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxInstructions, "textBoxInstructions");
		textBoxInstructions.Name = "textBoxInstructions";
		textBoxInstructions.ReadOnly = true;
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentRequestResultsStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 0);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRequestResultsStatus, "digitalReadoutInstrumentRequestResultsStatus");
		digitalReadoutInstrumentRequestResultsStatus.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).FreezeValue = false;
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Initialize((ValueState)0, 11);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(8, 7.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(9, 8.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(10, 9.0, (ValueState)0);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(11, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).Instrument = new Qualifier((QualifierTypes)64, "SRRL01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status");
		((Control)(object)digitalReadoutInstrumentRequestResultsStatus).Name = "digitalReadoutInstrumentRequestResultsStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		sharedProcedureCreatorComponent.Suspend();
		sharedProcedureCreatorComponent.MonitorCall = new ServiceCall("SRRL01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status");
		sharedProcedureCreatorComponent.MonitorGradient.Initialize((ValueState)0, 11);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(1, 0.0, (ValueState)1);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(7, 6.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(8, 7.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(9, 8.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(10, 9.0, (ValueState)0);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(11, 255.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorInterval = 2500;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponent, "sharedProcedureCreatorComponent");
		sharedProcedureCreatorComponent.Qualifier = "SP_SRRLCalibration";
		sharedProcedureCreatorComponent.StartCall = new ServiceCall("SRRL01T", "RT_Service_Alignment_Azimuth_Start");
		val.Gradient.Initialize((ValueState)3, 1);
		val.Gradient.Modify(1, 1.0, (ValueState)1);
		val.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		sharedProcedureCreatorComponent.StartConditions.Add(val);
		sharedProcedureCreatorComponent.StopCall = new ServiceCall("SRRL01T", "RT_Service_Alignment_Azimuth_Stop");
		sharedProcedureCreatorComponent.StopServiceComplete += sharedProcedureCreatorComponent_StopServiceComplete;
		sharedProcedureCreatorComponent.MonitorServiceComplete += sharedProcedureCreatorComponent_MonitorServiceComplete;
		sharedProcedureCreatorComponent.Resume();
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStartStop;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).PerformLayout();
		((Control)(object)tableLayoutPanelButtons).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
