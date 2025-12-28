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
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC2_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
	private const string cameraHeight = "camera_height";

	private Channel channel;

	private Service calibrateRequestResultsProgressService;

	private string calibrateCommitServiceList;

	private WarningManager warningManager;

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

	private Button buttonHiddenStart;

	private DigitalReadoutInstrument digitalReadoutInstrumentCameraHeight;

	private System.Windows.Forms.Label labelTitle;

	public UserPanel()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.WarningManagerJobName, seekTimeListViewLog.RequiredUserLabelPrefix);
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("MPC02T"));
	}

	private void SetChannel(Channel mpcChannel)
	{
		if (channel == mpcChannel)
		{
			return;
		}
		warningManager.Reset();
		if (channel != null)
		{
			calibrateRequestResultsProgressService = null;
			calibrateCommitServiceList = null;
		}
		channel = mpcChannel;
		if (channel != null)
		{
			calibrateRequestResultsProgressService = channel.Services["RT_iOCAL_Routine_Control_Request_Results_Progress"];
			calibrateCommitServiceList = channel.Services.GetDereferencedServiceList("CommitToPermanentMemoryService");
			if (channel.CommunicationsState == CommunicationsState.Online && channel.Parameters["camera_height"] != null && !channel.Parameters["camera_height"].HasBeenReadFromEcu)
			{
				channel.Parameters.ReadGroup(channel.Parameters["camera_height"].GroupQualifier, fromCache: true, synchronous: false);
			}
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
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		if (val == null)
		{
			return;
		}
		if ((int)val.Result == 1)
		{
			if (channel != null && calibrateCommitServiceList != null)
			{
				LogText(Resources.Message_ResettingFaultCodes);
				channel.FaultCodes.Reset(synchronous: false);
				LogText(Resources.Message_CommittingCalibration);
				channel.Services.Execute(calibrateCommitServiceList, synchronous: false);
				LogText(Resources.Messagre_CalibrationComplete);
			}
			else
			{
				LogText(Resources.Message_CalibrationNotComplete);
			}
		}
		else
		{
			LogText(Resources.Message_CalibrationNotComplete);
		}
	}

	private void buttonStartStop_Click(object sender, EventArgs e)
	{
		if (sharedProcedureSelection.SelectedProcedure.CanStart)
		{
			if (warningManager.RequestContinue())
			{
				sharedProcedureIntegrationComponent.StartStopButton.PerformClick();
			}
		}
		else
		{
			sharedProcedureIntegrationComponent.StartStopButton.PerformClick();
			LogText(Resources.Message_Cancelled);
		}
	}

	private void buttonHiddenStart_EnabledChanged(object sender, EventArgs e)
	{
		buttonStartStop.Enabled = buttonHiddenStart.Enabled;
	}

	private void buttonHiddenStart_TextChanged(object sender, EventArgs e)
	{
		string a = null;
		if (sender is Button)
		{
			a = (sender as Button).Text;
		}
		if (string.Equals(a, "&Start", StringComparison.OrdinalIgnoreCase))
		{
			buttonStartStop.Text = Resources.Button_StartTitle;
		}
		else
		{
			buttonStartStop.Text = buttonHiddenStart.Text;
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
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Expected O, but got Unknown
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_097d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		tableLayoutPanelButtons = new TableLayoutPanel();
		buttonHiddenStart = new Button();
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
		digitalReadoutInstrumentCameraHeight = new DigitalReadoutInstrument();
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
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(buttonHiddenStart, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(labelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(buttonStartStop, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)sharedProcedureSelection, 3, 0);
		((Control)(object)tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
		componentResourceManager.ApplyResources(buttonHiddenStart, "buttonHiddenStart");
		buttonHiddenStart.Name = "buttonHiddenStart";
		buttonHiddenStart.UseCompatibleTextRendering = true;
		buttonHiddenStart.UseVisualStyleBackColor = true;
		buttonHiddenStart.EnabledChanged += buttonHiddenStart_EnabledChanged;
		buttonHiddenStart.TextChanged += buttonHiddenStart_TextChanged;
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseCompatibleTextRendering = true;
		buttonStartStop.UseVisualStyleBackColor = true;
		buttonStartStop.Click += buttonStartStop_Click;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_MPC2Calibration" });
		componentResourceManager.ApplyResources(progressBarResultsProgress, "progressBarResultsProgress");
		progressBarResultsProgress.Name = "progressBarResultsProgress";
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "MPC2-Calibration";
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
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentRequestResultsStatus, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentCameraHeight, 0, 0);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRequestResultsStatus, "digitalReadoutInstrumentRequestResultsStatus");
		digitalReadoutInstrumentRequestResultsStatus.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).FreezeValue = false;
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Initialize((ValueState)0, 8);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(8, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).Instrument = new Qualifier((QualifierTypes)64, "MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status");
		((Control)(object)digitalReadoutInstrumentRequestResultsStatus).Name = "digitalReadoutInstrumentRequestResultsStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCameraHeight, "digitalReadoutInstrumentCameraHeight");
		digitalReadoutInstrumentCameraHeight.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).FreezeValue = false;
		digitalReadoutInstrumentCameraHeight.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentCameraHeight.Gradient.Modify(1, 1.01, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).Instrument = new Qualifier((QualifierTypes)4, "MPC02T", "camera_height");
		((Control)(object)digitalReadoutInstrumentCameraHeight).Name = "digitalReadoutInstrumentCameraHeight";
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).UnitAlignment = StringAlignment.Near;
		sharedProcedureCreatorComponent.Suspend();
		sharedProcedureCreatorComponent.MonitorCall = new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status");
		sharedProcedureCreatorComponent.MonitorGradient.Initialize((ValueState)0, 8);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(1, 0.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(7, 6.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorGradient.Modify(8, 255.0, (ValueState)3);
		sharedProcedureCreatorComponent.MonitorInterval = 2500;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponent, "sharedProcedureCreatorComponent");
		sharedProcedureCreatorComponent.Qualifier = "SP_MPC2Calibration";
		sharedProcedureCreatorComponent.StartCall = new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Start");
		val.Gradient.Initialize((ValueState)3, 1);
		val.Gradient.Modify(1, 1.0, (ValueState)1);
		val.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		sharedProcedureCreatorComponent.StartConditions.Add(val);
		sharedProcedureCreatorComponent.StopCall = new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Stop");
		sharedProcedureCreatorComponent.StopServiceComplete += sharedProcedureCreatorComponent_StopServiceComplete;
		sharedProcedureCreatorComponent.MonitorServiceComplete += sharedProcedureCreatorComponent_MonitorServiceComplete;
		sharedProcedureCreatorComponent.Resume();
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = buttonHiddenStart;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_MPC2_Calibration");
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
