using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Synchronize_ACM.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		Unknown,
		Starting,
		ServerUnlock,
		WaitingForServerConnection,
		StartMarriage,
		Marrying,
		StopMarriage,
		ReadyToClearFaults,
		WaitingForFaultsToClear,
		FaultsCleared,
		Success,
		Done
	}

	private const string UnlockSharedProcedureFormat = "SP_SecurityUnlock_ACM21T_UnlockXN";

	private const string StartMarriageProcedure = "RT_Start_ECU_Marriage_Routine_Start";

	private const string MonitorMarriageProcedure = "RT_Start_ECU_Marriage_Routine_Request_Results_ECU_Marriage_Routine_Status_Byte";

	private const string StopMarriageProcedure = "RT_Start_ECU_Marriage_Routine_Stop";

	private const string DeleteFaults = "RT_SR087_Delete_Non_Erasable_FC_Start";

	private const string VinQualifier = "CO_VIN";

	private State currentState;

	private Timer monitorMarriageTimer = new Timer();

	private Channel mcm21t;

	private Channel acm;

	private bool hasBeenProgrammed = false;

	private TableLayoutPanel tableLayoutPanelMain;

	private DigitalReadoutInstrument digitalReadoutInstrumentAcmFault;

	private Panel panelSyncAcm;

	private TableLayoutPanel tableLayoutPanelSyncAcm;

	private SeekTimeListView seekTimeListView1;

	private Button buttonSynchronize;

	private Checkmark checkmarkSync;

	private System.Windows.Forms.Label labelSyncStatus;

	private Button buttonClose;

	private bool RequiresUnlock => (int)digitalReadoutInstrumentAcmFault.RepresentedState != 1;

	public UserPanel()
	{
		InitializeComponent();
		monitorMarriageTimer.Interval = 1000;
		monitorMarriageTimer.Tick += marriageTimer_Tick;
		currentState = State.Unknown;
		((CustomPanel)this).OnChannelsChanged();
		string acmVin = ((acm != null) ? acm.EcuInfos["CO_VIN"].Value : string.Empty);
		string mcmVin = ((acm != null) ? mcm21t.EcuInfos["CO_VIN"].Value : string.Empty);
		string today = DateTime.Now.Date.ToString("MM/dd/yyyy");
		hasBeenProgrammed = File.ReadAllLines(Directories.StationLogFile).Any((string line) => line.Contains("Replace") && (((line.Contains("ACM21T") || line.Contains("ACM301T")) && line.Contains(acmVin)) || (line.Contains("MCM21T") && line.Contains(mcmVin))) && line.Contains(today));
		UpdateUI();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = currentState != State.Unknown && currentState != State.Done;
		if (!e.Cancel)
		{
			monitorMarriageTimer.Tick -= marriageTimer_Tick;
			monitorMarriageTimer.Dispose();
		}
	}

	private void UpdateUI()
	{
		Button button = buttonSynchronize;
		bool enabled = (checkmarkSync.Checked = ReadyToStart());
		button.Enabled = enabled;
	}

	private bool ReadyToStart()
	{
		if (currentState != State.Unknown && currentState != State.Done)
		{
			labelSyncStatus.Text = Resources.Message_Running;
			return false;
		}
		if (mcm21t == null)
		{
			labelSyncStatus.Text = Resources.Message_MCMOffline;
			return false;
		}
		if (acm == null)
		{
			labelSyncStatus.Text = Resources.Message_ACMOffline;
			return false;
		}
		if (mcm21t.EcuInfos["CO_VIN"].Value != acm.EcuInfos["CO_VIN"].Value)
		{
			labelSyncStatus.Text = Resources.Message_VINsNotSynchronized;
			return false;
		}
		if (!hasBeenProgrammed)
		{
			labelSyncStatus.Text = Resources.Message_UseDiagnosticLinkToProgramTheDevice;
			return false;
		}
		labelSyncStatus.Text = Resources.Message_Ready;
		return true;
	}

	private bool SetChannelMcm(Channel channel)
	{
		if (mcm21t != channel)
		{
			currentState = State.Unknown;
			mcm21t = channel;
		}
		return mcm21t != null;
	}

	private bool SetChannelAcm(Channel channel)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (acm != channel)
		{
			currentState = State.Unknown;
			acm = channel;
			if (acm != null)
			{
				((SingleInstrumentBase)digitalReadoutInstrumentAcmFault).Instrument = new Qualifier((QualifierTypes)32, acm.Ecu.Name, "ED000D");
			}
		}
		return acm != null;
	}

	private bool RunService(Channel channel, string serviceQualifier, ServiceCompleteEventHandler serviceCompleteEventHandler)
	{
		if (channel != null && channel.Online)
		{
			Service service = channel.Services[serviceQualifier];
			if (service != null)
			{
				if (serviceCompleteEventHandler != null)
				{
					service.ServiceCompleteEvent += serviceCompleteEventHandler;
				}
				service.Execute(synchronous: false);
				return true;
			}
		}
		UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ServiceCouldNotBeStarted01, channel.Ecu.Name, serviceQualifier));
		return false;
	}

	public override void OnChannelsChanged()
	{
		SetChannelMcm(((CustomPanel)this).GetChannel("MCM21T", (ChannelLookupOptions)1));
		if (!SetChannelAcm(((CustomPanel)this).GetChannel("ACM21T", (ChannelLookupOptions)1)))
		{
			SetChannelAcm(((CustomPanel)this).GetChannel("ACM301T", (ChannelLookupOptions)1));
		}
		UpdateUI();
	}

	private void buttonSynchronize_Click(object sender, EventArgs e)
	{
		UpdateUI();
		if (RequiresUnlock)
		{
			SetState(State.Starting);
		}
		else
		{
			SetState(State.StartMarriage);
		}
	}

	private void GoMachine()
	{
		switch (currentState)
		{
		case State.Starting:
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Unlocking0, acm.Ecu.Name));
			currentState = State.ServerUnlock;
			PerformServerUnlock();
			break;
		case State.StartMarriage:
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_StartingMarriage0, acm.Ecu.Name));
			StartMarriage();
			break;
		case State.Marrying:
			MonitorMarriage();
			break;
		case State.StopMarriage:
			StopMarriage();
			break;
		case State.ReadyToClearFaults:
			UpdateStatus(Resources.Message_ClearingCodes);
			PerformFaultCodeClear();
			break;
		case State.FaultsCleared:
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0CodesCleared, acm.Ecu.Name));
			SetState(State.Success);
			break;
		case State.Success:
			UpdateStatus(Resources.Message_TurnIgnitionOffThenTurnIgnitionOn);
			if (RequiresUnlock)
			{
				UpdateStatus(Resources.Message_OnceThisIsDoneFaultCodeShouldBeCleared);
			}
			SetState(State.Done);
			break;
		}
		UpdateUI();
	}

	private void UpdateStatus(string message)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, message);
	}

	private void SetState(State newState)
	{
		if (newState != currentState)
		{
			currentState = newState;
			GoMachine();
		}
	}

	private void PerformServerUnlock()
	{
		SharedProcedureBase val = SharedProcedureBase.AvailableProcedures["SP_SecurityUnlock_ACM21T_UnlockXN"];
		if (val != null)
		{
			if (val.CanStart)
			{
				SetState(State.WaitingForServerConnection);
				val.StartComplete += unlockSharedProcedure_StartComplete;
				val.Start();
				return;
			}
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedureWasFoundButItCouldNotBeStarted0, val.Name));
		}
		else
		{
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedureWasNotFound0, "SP_SecurityUnlock_ACM21T_UnlockXN"));
		}
		SetState(State.Done);
	}

	private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StartComplete -= unlockSharedProcedure_StartComplete;
		if (((ResultEventArgs)(object)e).Succeeded && (int)e.Result == 1)
		{
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0UnlockViaTheServerWasInitiatedUsingProcedure1, acm.Ecu.Name, val.Name));
			val.StopComplete += unlockSharedProcedure_StopComplete;
		}
		else
		{
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedure0FailedAtStart1, val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
			SetState(State.Done);
		}
	}

	private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StopComplete -= unlockSharedProcedure_StopComplete;
		if (!((ResultEventArgs)(object)e).Succeeded || (int)e.Result == 0)
		{
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedure0Failed1, val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
			SetState(State.Done);
		}
		else
		{
			UpdateStatus(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0WasUnlockedViaTheServerUsingProcedure1, acm.Ecu.Name, val.Name));
			SetState(State.StartMarriage);
		}
	}

	private void StartMarriage()
	{
		if (!RunService(acm, "RT_Start_ECU_Marriage_Routine_Start", StartMarriageServiceComplete))
		{
			SetState(State.Done);
		}
	}

	private void StartMarriageServiceComplete(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= StartMarriageServiceComplete;
		if (e.Succeeded)
		{
			SetState(State.Marrying);
			return;
		}
		UpdateStatus(Resources.Message_MarriageCouldNotBeStarted);
		SetState(State.Done);
	}

	private void MonitorMarriage()
	{
		if (!RunService(acm, "RT_Start_ECU_Marriage_Routine_Request_Results_ECU_Marriage_Routine_Status_Byte", MonitorMarriageServiceComplete))
		{
			monitorMarriageTimer.Enabled = false;
			SetState(State.Done);
		}
	}

	private void MonitorMarriageServiceComplete(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= MonitorMarriageServiceComplete;
		Choice choice;
		if (e.Succeeded && service.OutputValues != null && service.OutputValues[0] != null && (choice = (Choice)service.OutputValues[0].Value) != null)
		{
			switch (choice.Index)
			{
			case 1:
				SetState(State.StopMarriage);
				break;
			case 2:
				monitorMarriageTimer.Enabled = true;
				break;
			default:
				UpdateStatus(Resources.Message_MarriageFailed);
				SetState(State.Done);
				break;
			}
		}
		else
		{
			UpdateStatus(Resources.Message_MarriageCouldNotBeMonitored);
			SetState(State.Done);
		}
	}

	private void marriageTimer_Tick(object sender, EventArgs e)
	{
		monitorMarriageTimer.Enabled = false;
		MonitorMarriage();
	}

	private void StopMarriage()
	{
		if (!RunService(acm, "RT_Start_ECU_Marriage_Routine_Stop", StopMarriageServiceComplete))
		{
			SetState(State.Done);
		}
	}

	private void StopMarriageServiceComplete(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= StopMarriageServiceComplete;
		if (e.Succeeded)
		{
			if (RequiresUnlock)
			{
				SetState(State.ReadyToClearFaults);
			}
			else
			{
				SetState(State.Success);
			}
		}
		else
		{
			UpdateStatus(Resources.Message_MarriageCouldNotBeStopped);
			SetState(State.Done);
		}
	}

	private void PerformFaultCodeClear()
	{
		if (RunService(acm, "RT_SR087_Delete_Non_Erasable_FC_Start", DeleteNonErasableFCServiceComplete))
		{
			SetState(State.WaitingForFaultsToClear);
		}
		else
		{
			SetState(State.Done);
		}
	}

	private void DeleteNonErasableFCServiceComplete(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= DeleteNonErasableFCServiceComplete;
		if (e.Succeeded)
		{
			SetState(State.FaultsCleared);
			return;
		}
		UpdateStatus(Resources.Message_FaultsCouldNotBeCleared);
		SetState(State.Done);
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_11ec: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		panelSyncAcm = new Panel();
		tableLayoutPanelSyncAcm = new TableLayoutPanel();
		digitalReadoutInstrumentAcmFault = new DigitalReadoutInstrument();
		checkmarkSync = new Checkmark();
		labelSyncStatus = new System.Windows.Forms.Label();
		buttonSynchronize = new Button();
		seekTimeListView1 = new SeekTimeListView();
		buttonClose = new Button();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		panelSyncAcm.SuspendLayout();
		((Control)(object)tableLayoutPanelSyncAcm).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelSyncAcm, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListView1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 1, 3);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		panelSyncAcm.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)panelSyncAcm, 2);
		panelSyncAcm.Controls.Add((Control)(object)tableLayoutPanelSyncAcm);
		componentResourceManager.ApplyResources(panelSyncAcm, "panelSyncAcm");
		panelSyncAcm.Name = "panelSyncAcm";
		componentResourceManager.ApplyResources(tableLayoutPanelSyncAcm, "tableLayoutPanelSyncAcm");
		((TableLayoutPanel)(object)tableLayoutPanelSyncAcm).Controls.Add((Control)(object)digitalReadoutInstrumentAcmFault, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelSyncAcm).Controls.Add((Control)(object)checkmarkSync, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelSyncAcm).Controls.Add(labelSyncStatus, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelSyncAcm).Controls.Add(buttonSynchronize, 3, 1);
		((Control)(object)tableLayoutPanelSyncAcm).Name = "tableLayoutPanelSyncAcm";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAcmFault, "digitalReadoutInstrumentAcmFault");
		((TableLayoutPanel)(object)tableLayoutPanelSyncAcm).SetColumnSpan((Control)(object)digitalReadoutInstrumentAcmFault, 4);
		digitalReadoutInstrumentAcmFault.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAcmFault).FreezeValue = false;
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText38"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText39"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText40"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText41"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText42"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText43"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText44"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText45"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText46"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText47"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText48"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText49"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText50"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText51"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText52"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText53"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText54"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText55"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText56"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText57"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText58"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText59"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText60"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText61"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText62"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText63"));
		digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText64"));
		digitalReadoutInstrumentAcmFault.Gradient.Initialize((ValueState)2, 64);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(3, 4.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(4, 5.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(5, 8.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(6, 9.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(7, 12.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(8, 13.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(9, 32.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(10, 33.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(11, 36.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(12, 37.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(13, 40.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(14, 41.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(15, 44.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(16, 45.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(17, 128.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(18, 129.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(19, 132.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(20, 133.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(21, 136.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(22, 137.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(23, 140.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(24, 141.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(25, 160.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(26, 161.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(27, 164.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(28, 165.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(29, 168.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(30, 169.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(31, 172.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(32, 173.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(33, 256.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(34, 257.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(35, 260.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(36, 261.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(37, 264.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(38, 265.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(39, 268.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(40, 269.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(41, 288.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(42, 289.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(43, 292.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(44, 293.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(45, 296.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(46, 297.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(47, 300.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(48, 301.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(49, 384.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(50, 385.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(51, 388.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(52, 389.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(53, 392.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(54, 393.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(55, 396.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(56, 397.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(57, 416.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(58, 417.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(59, 420.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(60, 421.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(61, 424.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(62, 425.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(63, 428.0, (ValueState)2);
		digitalReadoutInstrumentAcmFault.Gradient.Modify(64, 429.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentAcmFault).Instrument = new Qualifier((QualifierTypes)32, "ACM21T", "ED000D");
		((Control)(object)digitalReadoutInstrumentAcmFault).Name = "digitalReadoutInstrumentAcmFault";
		((SingleInstrumentBase)digitalReadoutInstrumentAcmFault).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAcmFault).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAcmFault).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentAcmFault.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(checkmarkSync, "checkmarkSync");
		((Control)(object)checkmarkSync).Name = "checkmarkSync";
		componentResourceManager.ApplyResources(labelSyncStatus, "labelSyncStatus");
		((TableLayoutPanel)(object)tableLayoutPanelSyncAcm).SetColumnSpan((Control)labelSyncStatus, 2);
		labelSyncStatus.Name = "labelSyncStatus";
		labelSyncStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonSynchronize, "buttonSynchronize");
		buttonSynchronize.Name = "buttonSynchronize";
		buttonSynchronize.UseCompatibleTextRendering = true;
		buttonSynchronize.UseVisualStyleBackColor = true;
		buttonSynchronize.Click += buttonSynchronize_Click;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)seekTimeListView1, 2);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "Synchronize ACM";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		panelSyncAcm.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelSyncAcm).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelSyncAcm).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
