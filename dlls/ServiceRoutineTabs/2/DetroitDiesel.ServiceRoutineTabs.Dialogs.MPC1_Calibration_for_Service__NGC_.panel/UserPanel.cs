using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__NGC_.panel;

public class UserPanel : CustomPanel
{
	private enum ProcessState
	{
		NotRunning,
		Unlock,
		ConfigStatic,
		Reset,
		RequestResult,
		Complete
	}

	private Channel channel;

	private Service resultService;

	private Service staticConfigService;

	private Service resetService;

	private Service unlockService;

	private ProcessState state;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView;

	private DigitalReadoutInstrument digitalReadoutInstrumentResult;

	private DigitalReadoutInstrument digitalReadoutInstrumentOverallStatus;

	private DigitalReadoutInstrument digitalReadoutInstrumentOnlineStatus;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrumentStaticStatus;

	private ScalingLabel scalingLabelTitle;

	private DigitalReadoutInstrument digitalReadoutInstrumentLDWFunction;

	private DigitalReadoutInstrument digitalReadoutInstrumentCameraHeight;

	private Button buttonCalibrate;

	public bool Working => state != ProcessState.NotRunning && state != ProcessState.Complete;

	public bool Online => channel != null && channel.Online;

	public bool HaveReadParameters => channel != null && channel.Parameters.HaveBeenReadFromEcu;

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("MPC01T"));
	}

	private void SetChannel(Channel mpcChannel)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (channel != null)
		{
			channel.Services.ServiceCompleteEvent -= Advance;
			channel.CommunicationsStateUpdateEvent -= channel_CommunicationsStateUpdateEvent;
			resultService = null;
			staticConfigService = null;
			resetService = null;
			unlockService = null;
		}
		channel = mpcChannel;
		if (channel != null)
		{
			channel.Services.ServiceCompleteEvent += Advance;
			channel.CommunicationsStateUpdateEvent += channel_CommunicationsStateUpdateEvent;
			if (!channel.Parameters.HaveBeenReadFromEcu)
			{
				channel.Parameters.Read(synchronous: false);
			}
			ServiceCollection services = channel.Services;
			Qualifier instrument = ((SingleInstrumentBase)digitalReadoutInstrumentResult).Instrument;
			resultService = services[((Qualifier)(ref instrument)).Name];
			if (resultService != null)
			{
				resultService.Execute(synchronous: false);
			}
			staticConfigService = channel.Services["DL_Static_Camera_Calibration_Data"];
			resetService = channel.Services["FN_HardReset"];
			unlockService = channel.Services["DJ_SecurityAccess_Config_Dev"];
		}
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		buttonCalibrate.Enabled = HaveReadParameters && !Working;
	}

	private void GoMachine()
	{
		state++;
		switch (state)
		{
		case ProcessState.Unlock:
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, Resources.Message_UnlockingDevice);
			unlockService.Execute(synchronous: false);
			break;
		case ProcessState.ConfigStatic:
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, Resources.Message_ConfiguringStaticCalibration);
			staticConfigService.InputValues[0].Value = 0f;
			staticConfigService.InputValues[1].Value = 0f;
			staticConfigService.InputValues[2].Value = 0f;
			staticConfigService.Execute(synchronous: false);
			break;
		case ProcessState.Reset:
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, Resources.Message_ResettingDevice);
			resetService.Execute(synchronous: false);
			break;
		case ProcessState.RequestResult:
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, Resources.Message_VerifyingResult);
			resultService.Execute(synchronous: false);
			break;
		case ProcessState.Complete:
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, Resources.Message_Complete);
			UpdateUserInterface();
			break;
		}
	}

	private void Advance(object sender, ResultEventArgs e)
	{
		if (!Working)
		{
			return;
		}
		if (e.Succeeded)
		{
			if (Online)
			{
				GoMachine();
			}
			else
			{
				Abort(Resources.Message_EcuDisconnectedBeforeCompletion);
			}
		}
		else
		{
			Abort(e.Exception.Message);
		}
	}

	private void Abort(string reason)
	{
		state = ProcessState.Complete;
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, reason);
		if (Online)
		{
			resultService.Execute(synchronous: false);
		}
		else
		{
			UpdateUserInterface();
		}
	}

	private void buttonCalibrate_Click(object sender, EventArgs e)
	{
		state = ProcessState.NotRunning;
		GoMachine();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (Working)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			SetChannel(null);
		}
	}

	private void channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrumentOverallStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrumentOnlineStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentResult = new DigitalReadoutInstrument();
		digitalReadoutInstrumentStaticStatus = new DigitalReadoutInstrument();
		scalingLabelTitle = new ScalingLabel();
		digitalReadoutInstrumentLDWFunction = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		buttonCalibrate = new Button();
		digitalReadoutInstrumentCameraHeight = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentOverallStatus, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentOnlineStatus, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentResult, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentStaticStatus, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)scalingLabelTitle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentLDWFunction, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonCalibrate, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCameraHeight, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentOverallStatus, "digitalReadoutInstrumentOverallStatus");
		digitalReadoutInstrumentOverallStatus.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentOverallStatus).FreezeValue = false;
		digitalReadoutInstrumentOverallStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentOverallStatus.Gradient.Modify(1, 0.0, (ValueState)4);
		digitalReadoutInstrumentOverallStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentOverallStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentOverallStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentOverallStatus).Instrument = new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc01_Camera_Calibration_Overall_Camera_Calibration_Status");
		((Control)(object)digitalReadoutInstrumentOverallStatus).Name = "digitalReadoutInstrumentOverallStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentOverallStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentOnlineStatus, "digitalReadoutInstrumentOnlineStatus");
		digitalReadoutInstrumentOnlineStatus.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentOnlineStatus).FreezeValue = false;
		digitalReadoutInstrumentOnlineStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentOnlineStatus.Gradient.Modify(1, 0.0, (ValueState)4);
		digitalReadoutInstrumentOnlineStatus.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentOnlineStatus.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentOnlineStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentOnlineStatus).Instrument = new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc01_Camera_Calibration_Online_Camera_Calibration_Status");
		((Control)(object)digitalReadoutInstrumentOnlineStatus).Name = "digitalReadoutInstrumentOnlineStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentOnlineStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)32, "MPC01T", "00FBED");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentResult, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentResult, "digitalReadoutInstrumentResult");
		digitalReadoutInstrumentResult.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentResult).FreezeValue = false;
		digitalReadoutInstrumentResult.Gradient.Initialize((ValueState)0, 10);
		digitalReadoutInstrumentResult.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentResult.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentResult.Gradient.Modify(3, 2.0, (ValueState)4);
		digitalReadoutInstrumentResult.Gradient.Modify(4, 3.0, (ValueState)4);
		digitalReadoutInstrumentResult.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentResult.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentResult.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentResult.Gradient.Modify(8, 7.0, (ValueState)3);
		digitalReadoutInstrumentResult.Gradient.Modify(9, 8.0, (ValueState)3);
		digitalReadoutInstrumentResult.Gradient.Modify(10, 15.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentResult).Instrument = new Qualifier((QualifierTypes)64, "MPC01T", "RT_End_of_Line_Calibration_RequestResults_Static_Camera_Calibration_Result");
		((Control)(object)digitalReadoutInstrumentResult).Name = "digitalReadoutInstrumentResult";
		((SingleInstrumentBase)digitalReadoutInstrumentResult).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentStaticStatus, "digitalReadoutInstrumentStaticStatus");
		digitalReadoutInstrumentStaticStatus.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentStaticStatus).FreezeValue = false;
		digitalReadoutInstrumentStaticStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentStaticStatus.Gradient.Modify(1, 0.0, (ValueState)4);
		digitalReadoutInstrumentStaticStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentStaticStatus.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentStaticStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentStaticStatus).Instrument = new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc01_Camera_Calibration_Static_Camera_Calibration_Status");
		((Control)(object)digitalReadoutInstrumentStaticStatus).Name = "digitalReadoutInstrumentStaticStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentStaticStatus).UnitAlignment = StringAlignment.Near;
		scalingLabelTitle.Alignment = StringAlignment.Center;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)scalingLabelTitle, 4);
		componentResourceManager.ApplyResources(scalingLabelTitle, "scalingLabelTitle");
		scalingLabelTitle.FontGroup = null;
		scalingLabelTitle.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelTitle).Name = "scalingLabelTitle";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentLDWFunction, "digitalReadoutInstrumentLDWFunction");
		digitalReadoutInstrumentLDWFunction.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentLDWFunction).FreezeValue = false;
		digitalReadoutInstrumentLDWFunction.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentLDWFunction.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentLDWFunction.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentLDWFunction.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentLDWFunction.Gradient.Modify(4, 3.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentLDWFunction).Instrument = new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc02_LDW_Function_Data_LDW_Function_State");
		((Control)(object)digitalReadoutInstrumentLDWFunction).Name = "digitalReadoutInstrumentLDWFunction";
		((SingleInstrumentBase)digitalReadoutInstrumentLDWFunction).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 3);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "MPC1Calibration";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 3);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(buttonCalibrate, "buttonCalibrate");
		buttonCalibrate.Name = "buttonCalibrate";
		buttonCalibrate.UseCompatibleTextRendering = true;
		buttonCalibrate.UseVisualStyleBackColor = true;
		buttonCalibrate.Click += buttonCalibrate_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentCameraHeight, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCameraHeight, "digitalReadoutInstrumentCameraHeight");
		digitalReadoutInstrumentCameraHeight.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).Instrument = new Qualifier((QualifierTypes)4, "MPC01T", "camera_height");
		((Control)(object)digitalReadoutInstrumentCameraHeight).Name = "digitalReadoutInstrumentCameraHeight";
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_MPC1_Camera_Alignment");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
