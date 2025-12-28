using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_USB_stick_Flashing.panel;

public class UserPanel : CustomPanel
{
	private const string ReprogrammingFlashSeedServiceQualifier = "DJ_Reprogramming_Flash_Seed";

	private const string SoftwareUpdateViaUsbStartStatusServiceQualifier = "RT_Shutdown_Software_Update_via_USB_Start_RoutineStartStatus";

	private const string EcuMaxFunctionalQualifier = "SES_Programming_P2_CAN_ECU_max_physical";

	private const int ValidStartStatusService = 0;

	private const string ResultsServiceQualifier = "RT_Shutdown_Software_Update_via_USB_Request_Results_ShutdownUpdateStatus";

	private const int ShutdownUpdateComplete = 5;

	private const string HardResetQualifier = "FN_HardReset";

	private const string HardwarePartNumberQualifier = "CO_HardwarePartNumber";

	private const string SoftwarePartNumberQualifier = "CO_SoftwarePartNumber";

	private readonly int[] ValidShutdownUpdateStatus = new int[5] { 0, 1, 3, 4, 5 };

	private readonly List<string> ValidHardwarePartNumbers = new List<string> { "66-10777-001", "66-13928-001", "66-13931-001", "66-19901-003", "66-19901-001", "66-19901-501", "66-19901-503", "66-05466-001", "66-13931-501" };

	private Channel ctp;

	private ProcessState state = ProcessState.NotRunning;

	private Timer monitoringTimer;

	private string origionalSoftwarePartNumber = string.Empty;

	private string currentSoftwarePartNumber = string.Empty;

	private string currentHardwarePartNumber = string.Empty;

	private TableLayoutPanel tableLayoutPanelMain;

	private ScalingLabel scalingLabelStatus;

	private Checkmark checkmarkStatus;

	private ProgressBar progressBarMarquee;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private SeekTimeListView seekTimeListView;

	private ScalingLabel scalingLabelConnect;

	private Checkmark checkmarkConnect;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private Button buttonStart;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private System.Windows.Forms.Label labelWarning;

	public bool CtpBusy => ctp != null && ctp.CommunicationsState != CommunicationsState.Online;

	public bool ProcessRunning => state != ProcessState.NotRunning && state != ProcessState.Complete;

	public bool CanFlash => !ProcessRunning && ValidHardware;

	public bool ValidHardware => ValidHardwarePartNumbers.Contains(CurrentHardwarePartNumber);

	public string CurrentSoftwarePartNumber
	{
		get
		{
			return string.IsNullOrEmpty(currentSoftwarePartNumber) ? null : currentSoftwarePartNumber;
		}
		set
		{
			if (value != null)
			{
				currentSoftwarePartNumber = value;
			}
		}
	}

	public string CurrentHardwarePartNumber
	{
		get
		{
			return string.IsNullOrEmpty(currentHardwarePartNumber) ? null : currentHardwarePartNumber;
		}
		set
		{
			if (value != null)
			{
				currentHardwarePartNumber = value;
			}
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	public UserPanel()
	{
		InitializeComponent();
		progressBarMarquee.Hide();
		monitoringTimer = new Timer();
		monitoringTimer.Interval = 5000;
		monitoringTimer.Tick += timer_Tick;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && ProcessRunning)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			SetCTP(null);
			monitoringTimer.Stop();
			monitoringTimer.Tick -= timer_Tick;
			monitoringTimer.Dispose();
		}
	}

	private void AddLogLabel(string text, bool updateStatus)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
			if (updateStatus)
			{
				((Control)(object)scalingLabelStatus).Text = text;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		SetCTP(((CustomPanel)this).GetChannel("CTP01T", (ChannelLookupOptions)3));
	}

	private void SetCTP(Channel ctp)
	{
		if (this.ctp != ctp)
		{
			if (this.ctp != null)
			{
				this.ctp.EcuInfos.EcuInfoUpdateEvent -= EcuInfos_EcuInfoUpdateEvent;
				this.ctp.CommunicationsStateUpdateEvent -= ctp_CommunicationsStateUpdateEvent;
				if (ProcessRunning)
				{
					Abort("CTP Disconnected");
				}
				state = ProcessState.NotRunning;
			}
			this.ctp = ctp;
			if (this.ctp != null)
			{
				state = ProcessState.NotRunning;
				this.ctp.EcuInfos.EcuInfoUpdateEvent += EcuInfos_EcuInfoUpdateEvent;
				this.ctp.CommunicationsStateUpdateEvent += ctp_CommunicationsStateUpdateEvent;
				UpdatePartNumbers();
			}
		}
		UpdateUserInterface();
	}

	private void EcuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdatePartNumbers();
		UpdateUserInterface();
	}

	private void UpdatePartNumbers()
	{
		CurrentSoftwarePartNumber = ReadEcuInfoData("CO_SoftwarePartNumber");
		CurrentHardwarePartNumber = ReadEcuInfoData("CO_HardwarePartNumber");
	}

	private void UpdateUserInterface()
	{
		if (ctp != null)
		{
			((Control)(object)scalingLabelConnect).Text = Resources.Message_ConnectedToCTP;
			checkmarkConnect.CheckState = CheckState.Checked;
			if (state == ProcessState.NotRunning)
			{
				if (CtpBusy)
				{
					checkmarkStatus.CheckState = CheckState.Indeterminate;
					((Control)(object)scalingLabelStatus).Text = Resources.Message_CTPBusy;
				}
				else if (!ValidHardware)
				{
					checkmarkStatus.CheckState = CheckState.Unchecked;
					((Control)(object)scalingLabelStatus).Text = Resources.Message_InvalidHardwarePartNumber;
				}
				else if (CanFlash)
				{
					checkmarkStatus.CheckState = CheckState.Checked;
					((Control)(object)scalingLabelStatus).Text = Resources.Message_ReadyToStart;
				}
			}
		}
		else
		{
			((Control)(object)scalingLabelConnect).Text = Resources.Message_NotConnectedToCTP;
			checkmarkConnect.CheckState = CheckState.Unchecked;
		}
		buttonStart.Enabled = ctp != null && !ProcessRunning && !CtpBusy && CanFlash;
		buttonClose.Enabled = !ProcessRunning;
	}

	private void GoMachine()
	{
		switch (state)
		{
		case ProcessState.StartHardReset1:
			origionalSoftwarePartNumber = CurrentSoftwarePartNumber;
			checkmarkStatus.CheckState = CheckState.Indeterminate;
			progressBarMarquee.Show();
			AddLogLabel(Resources.Message_ResettingCTPBeforeFlashing, updateStatus: true);
			ExecuteService("FN_HardReset", hardResetService_ServiceCompleteEvent);
			break;
		case ProcessState.WaitingforResetToFinish1:
			AddLogLabel(Resources.Message_WaitingForCTPToReset, updateStatus: true);
			break;
		case ProcessState.SetMaxFunctional:
			AddLogLabel("Set Max Functional", updateStatus: true);
			ExecuteService("SES_Programming_P2_CAN_ECU_max_physical", Services_MaxFunctionalServiceCompleteEvent);
			break;
		case ProcessState.ReprogrammingFlashSeed:
			AddLogLabel(Resources.Message_Unlocking, updateStatus: true);
			ExecuteService("DJ_Reprogramming_Flash_Seed", Services_FlashSeedServiceCompleteEvent);
			break;
		case ProcessState.StartReprogramming:
			AddLogLabel(Resources.Message_Reprogramming, updateStatus: true);
			ExecuteService("RT_Shutdown_Software_Update_via_USB_Start_RoutineStartStatus", Services_StartReprogrammingServiceCompleteEvent);
			break;
		case ProcessState.MonitorReprogramming:
			AddLogLabel(Resources.Message_CopyingData, updateStatus: true);
			monitoringTimer.Start();
			break;
		case ProcessState.StartHardReset2:
			AddLogLabel(Resources.Message_ResettingCTPAfterFlashing, updateStatus: true);
			ExecuteService("FN_HardReset", hardResetService_ServiceCompleteEvent);
			break;
		case ProcessState.WaitingforResetToFinish2:
			AddLogLabel(Resources.Message_WaitingForCTPToReset1, updateStatus: true);
			break;
		case ProcessState.Complete:
			progressBarMarquee.Hide();
			checkmarkStatus.CheckState = CheckState.Checked;
			AddStationLogEntry();
			AddLogLabel(Resources.Message_FlashingComplete, updateStatus: true);
			UpdateUserInterface();
			break;
		}
		UpdateUserInterface();
	}

	private void AddStationLogEntry()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["reasontext"] = "ReasonCTPUSBStickFlashing";
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2["Old_SW_PartNumber"] = origionalSoftwarePartNumber;
		dictionary2["New_SW_PartNumber"] = CurrentSoftwarePartNumber;
		ServerDataManager.UpdateEventsFile(ctp, (IDictionary<string, string>)dictionary, (IDictionary<string, string>)dictionary2, "CTPUSBStickFlashing", string.Empty, ReadEcuInfoData("CO_VIN"), "OK", "DESCRIPTION", string.Empty);
	}

	private string ReadEcuInfoData(string qualifier)
	{
		string text = string.Empty;
		if (ctp != null)
		{
			EcuInfo ecuInfo = ctp.EcuInfos[qualifier];
			if (ecuInfo == null)
			{
				ecuInfo = ctp.EcuInfos.GetItemContaining(qualifier);
			}
			if (ecuInfo != null)
			{
				text = ecuInfo.Value.ToString().Trim();
			}
		}
		return text.Trim();
	}

	private void ExecuteService(string serviceQualifier, ServiceCompleteEventHandler serviceCompleteEvent)
	{
		Service service = ctp.Services[serviceQualifier];
		if (service != null)
		{
			if (serviceCompleteEvent != null)
			{
				service.ServiceCompleteEvent += serviceCompleteEvent;
			}
			service.Execute(synchronous: false);
		}
		else
		{
			Abort(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_ServiceDoesNotExist0, serviceQualifier));
		}
	}

	private static void RemoveServiceComplete(Service service, ServiceCompleteEventHandler serviceCompleteEventHandler)
	{
		if (service != null)
		{
			service.ServiceCompleteEvent -= serviceCompleteEventHandler;
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (ctp != null && state == ProcessState.MonitorReprogramming && !CtpBusy)
		{
			ExecuteService("RT_Shutdown_Software_Update_via_USB_Request_Results_ShutdownUpdateStatus", Services_MonitoringServiceCompleteEvent);
		}
	}

	private void Services_MaxFunctionalServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		RemoveServiceComplete(sender as Service, Services_MaxFunctionalServiceCompleteEvent);
		if (e.Succeeded)
		{
			state++;
			GoMachine();
		}
		else
		{
			Abort(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_MaxFunctionalServiceFailed0, e.Exception.Message));
		}
	}

	private void Services_FlashSeedServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		RemoveServiceComplete(sender as Service, Services_FlashSeedServiceCompleteEvent);
		state++;
		GoMachine();
	}

	private void Services_StartReprogrammingServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		RemoveServiceComplete(sender as Service, Services_StartReprogrammingServiceCompleteEvent);
		if (e.Succeeded)
		{
			Service service = sender as Service;
			if (service != null && (int)((Choice)service.OutputValues[0].Value).RawValue == 0)
			{
				state++;
				GoMachine();
			}
			else
			{
				Abort(string.Format(CultureInfo.InvariantCulture, Resources.Message_CouldNotStartReprogrammingCheckUSBDrive));
			}
		}
		else
		{
			Abort(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_CouldNotStartReprogramming0, e.Exception.Message));
		}
	}

	private void Services_MonitoringServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		RemoveServiceComplete(service, Services_MonitoringServiceCompleteEvent);
		if (e.Succeeded)
		{
			Choice choice = (Choice)service.OutputValues[0].Value;
			AddLogLabel(choice.ToString(), updateStatus: false);
			int num = (int)choice.RawValue;
			if (ValidShutdownUpdateStatus.Contains(num))
			{
				if (5 == num)
				{
					monitoringTimer.Stop();
					state++;
					GoMachine();
				}
			}
			else
			{
				Abort(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_FlashFailed0, service.OutputValues[0].Value.ToString()));
			}
		}
		else
		{
			Abort(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_FlashFailed01, e.Exception.Message));
		}
	}

	private void Abort(string reason)
	{
		progressBarMarquee.Hide();
		state = ProcessState.NotRunning;
		AddLogLabel(Resources.Message_Failed + reason, updateStatus: true);
		checkmarkStatus.CheckState = CheckState.Unchecked;
		UpdatePartNumbers();
		UpdateUserInterface();
	}

	private void hardResetService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		RemoveServiceComplete(sender as Service, hardResetService_ServiceCompleteEvent);
		if (e.Succeeded)
		{
			state++;
			GoMachine();
		}
		else
		{
			Abort(Resources.Message_HardResetFailed);
		}
	}

	private void ctp_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdatePartNumbers();
		UpdateUserInterface();
		if (e.CommunicationsState == CommunicationsState.Online && (state == ProcessState.WaitingforResetToFinish1 || state == ProcessState.WaitingforResetToFinish2))
		{
			state++;
			GoMachine();
		}
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		state = ProcessState.StartHardReset1;
		GoMachine();
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		scalingLabelStatus = new ScalingLabel();
		checkmarkStatus = new Checkmark();
		scalingLabelConnect = new ScalingLabel();
		buttonClose = new Button();
		seekTimeListView = new SeekTimeListView();
		checkmarkConnect = new Checkmark();
		progressBarMarquee = new ProgressBar();
		buttonStart = new Button();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		labelWarning = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)scalingLabelStatus, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)checkmarkStatus, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)scalingLabelConnect, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 4, 7);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListView, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)checkmarkConnect, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(progressBarMarquee, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonStart, 3, 7);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrument1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrument3, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelWarning, 0, 5);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		scalingLabelStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)scalingLabelStatus, 4);
		componentResourceManager.ApplyResources(scalingLabelStatus, "scalingLabelStatus");
		scalingLabelStatus.FontGroup = null;
		scalingLabelStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelStatus).Name = "scalingLabelStatus";
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		checkmarkStatus.IndeterminateImage = (Image)componentResourceManager.GetObject("checkmarkStatus.IndeterminateImage");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		scalingLabelConnect.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)scalingLabelConnect, 4);
		componentResourceManager.ApplyResources(scalingLabelConnect, "scalingLabelConnect");
		scalingLabelConnect.FontGroup = null;
		scalingLabelConnect.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelConnect).Name = "scalingLabelConnect";
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)seekTimeListView, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "CTPUSBFlashing";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(checkmarkConnect, "checkmarkConnect");
		((Control)(object)checkmarkConnect).Name = "checkmarkConnect";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)progressBarMarquee, 5);
		componentResourceManager.ApplyResources(progressBarMarquee, "progressBarMarquee");
		progressBarMarquee.Maximum = 1000;
		progressBarMarquee.Name = "progressBarMarquee";
		progressBarMarquee.Step = 1;
		progressBarMarquee.Style = ProgressBarStyle.Marquee;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)0, 8);
		digitalReadoutInstrument2.Gradient.Modify(1, 6605466001.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(2, 6605466002.0, (ValueState)0);
		digitalReadoutInstrument2.Gradient.Modify(3, 6610777001.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(4, 6610777002.0, (ValueState)0);
		digitalReadoutInstrument2.Gradient.Modify(5, 6613928001.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(6, 6613928002.0, (ValueState)0);
		digitalReadoutInstrument2.Gradient.Modify(7, 6613931001.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(8, 6613931002.0, (ValueState)0);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)8, "CTP01T", "CO_HardwarePartNumber");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument1.Gradient.Modify(1, 14485460001.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(2, 14485460002.0, (ValueState)0);
		digitalReadoutInstrument1.Gradient.Modify(3, 14487260001.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(4, 14487260002.0, (ValueState)0);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)8, "CTP01T", "CO_SoftwarePartNumber");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutInstrument3, 3);
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)8, "CTP01T", "DT_STO_ID_FBS_Sw_Version_fbsVersion");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)labelWarning, 5);
		componentResourceManager.ApplyResources(labelWarning, "labelWarning");
		labelWarning.Name = "labelWarning";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_CTP_USB_Stick_Flash");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
