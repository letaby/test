using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Replacement__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string SetClutchTypeQualifier = "RT_0530_Kupplungsaktuatortyp_setzen_Service_Start_aktueller_Kupplungsaktuatortyp";

	private const string SetTransmissionType = "RT_0410_Getriebetyp_setzen_und_Wegwerte_loeschen_Service_Start_Getriebetyp";

	private const string ReleaseTransportSecurityQualifier = "DJ_Release_transport_security_for_TCM";

	private const int CPCA_SAE1 = 0;

	private Channel tcm;

	private Service SetClutchActuator;

	private Service transTypeService;

	private Service ReleaseTransportSecurity;

	private bool waitingForIgnitionOff;

	private bool waitingForIgnitionOn;

	private bool? wasAutoConnecting;

	private List<Channel> channelsToWorkWith = new List<Channel>();

	private List<Channel> channelsToWaitForReconnect = new List<Channel>();

	private List<Ecu> manualConnectEcus = new List<Ecu>();

	private bool isServiceRunning = false;

	private static string warningMessage = Resources.Message_ByExecutingThisRoutineTheTransmissionSLearnedValuesWillBeResetUntilTheseValuesAreRelearnedShiftQualityMayNotBeOptimalDoYouWishToContinue;

	private WarningManager warningMgr;

	private List<string> transNotSetFaultCodes = new List<string> { "52F3EE", "25F3EE", "23F3EE" };

	private Button btnSetTransmissionType;

	private Checkmark checkmarkTcmOnline;

	private Label labelTcmStatus;

	private TableLayoutPanel tableMain;

	private SeekTimeListView seekTimeListViewOutput;

	private TableLayoutPanel tableTransControls;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransmissionType;

	private ComboBox comboTransType;

	private Service SetTransType
	{
		get
		{
			return transTypeService;
		}
		set
		{
			transTypeService = value;
			if (SetTransType != null)
			{
				comboTransType.DataSource = SetTransType.InputValues[0].Choices;
			}
			else
			{
				comboTransType.DataSource = null;
			}
		}
	}

	private bool Online => tcm != null && tcm.Online;

	public UserPanel()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
		UpdateUserInterface();
		warningMgr = new WarningManager(warningMessage, (string)null, seekTimeListViewOutput.RequiredUserLabelPrefix);
	}

	public override void OnChannelsChanged()
	{
		SetTcm(((CustomPanel)this).GetChannel("TCM01T"));
	}

	private void SetTcm(Channel tcm)
	{
		if (this.tcm != tcm)
		{
			if (this.tcm != null)
			{
				this.tcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				SetClutchActuator = null;
				ReleaseTransportSecurity = null;
				SetTransType = null;
			}
			this.tcm = tcm;
			if (this.tcm != null)
			{
				this.tcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				SetClutchActuator = this.tcm.Services["RT_0530_Kupplungsaktuatortyp_setzen_Service_Start_aktueller_Kupplungsaktuatortyp"];
				ReleaseTransportSecurity = this.tcm.Services["DJ_Release_transport_security_for_TCM"];
				SetTransType = this.tcm.Services["RT_0410_Getriebetyp_setzen_und_Wegwerte_loeschen_Service_Start_Getriebetyp"];
			}
			UpdateUserInterface();
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (waitingForIgnitionOff)
		{
			if (tcm == null || tcm.CommunicationsState == CommunicationsState.Offline)
			{
				waitingForIgnitionOff = false;
				checkmarkTcmOnline.Checked = true;
				((Control)(object)labelTcmStatus).Text = Resources.Message_IgnitionIsOffPleaseWait;
				waitingForIgnitionOn = true;
				SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect();
				((Control)(object)this).OnEnabledChanged(new EventArgs());
			}
		}
		else if (waitingForIgnitionOn && tcm != null && tcm.CommunicationsState == CommunicationsState.Online)
		{
			RestoreAutoConnectState();
			((Control)(object)this).OnEnabledChanged(new EventArgs());
		}
		UpdateUserInterface();
	}

	private void TurnOffAutoConnect()
	{
		wasAutoConnecting = SapiManager.GlobalInstance.Sapi.Channels.AutoConnecting;
		Cursor.Current = Cursors.WaitCursor;
		SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
		Cursor.Current = Cursors.Default;
		foreach (Channel item in channelsToWorkWith)
		{
			channelsToWaitForReconnect.Add(item);
			if (!item.Ecu.MarkedForAutoConnect)
			{
				manualConnectEcus.Add(item.Ecu);
				item.Ecu.MarkedForAutoConnect = true;
			}
		}
		waitingForIgnitionOff = true;
		UpdateUserInterface();
	}

	private void RestoreAutoConnectState()
	{
		waitingForIgnitionOn = false;
		foreach (Ecu manualConnectEcu in manualConnectEcus)
		{
			manualConnectEcu.MarkedForAutoConnect = false;
		}
		manualConnectEcus.Clear();
		if (wasAutoConnecting.HasValue)
		{
			Cursor.Current = Cursors.WaitCursor;
			SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
			if (wasAutoConnecting.Value)
			{
				SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect(1);
			}
			wasAutoConnecting = null;
			Cursor.Current = Cursors.Default;
		}
		UpdateUserInterface();
	}

	private void DisplayIgnitionMessage()
	{
		if (waitingForIgnitionOff)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_PleaseTurnIgnitionOffAndWait;
			checkmarkTcmOnline.Checked = false;
		}
		else if (waitingForIgnitionOn)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_PleaseTurnIgnitionOnAndWait;
			checkmarkTcmOnline.Checked = false;
		}
	}

	private void UpdateUserInterface()
	{
		checkmarkTcmOnline.Checked = Online;
		if (isServiceRunning)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_TransmissionIsBeingSet;
			btnSetTransmissionType.Enabled = false;
		}
		else if (!Online)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_TheTransmissionTypeCannotBeSetBecauseTheTCMIsOffline;
			btnSetTransmissionType.Enabled = false;
		}
		else
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_TheTransmissionTypeCanBeSet;
			btnSetTransmissionType.Enabled = true;
		}
		DisplayIgnitionMessage();
	}

	private bool CheckFaultsAndWarn()
	{
		if (!Online)
		{
			return false;
		}
		IEnumerable<string> second = from x in tcm.FaultCodes.GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent)
			select x.Code;
		return transNotSetFaultCodes.Intersect(second).Count() > 0 || warningMgr.RequestContinue();
	}

	private void btnSetTransmissionType_Click(object sender, EventArgs e)
	{
		if (CheckFaultsAndWarn())
		{
			isServiceRunning = true;
			StartSetClutchActuator();
		}
	}

	private void StartSetClutchActuator()
	{
		if (Online && SetClutchActuator != null)
		{
			SetClutchActuator.InputValues[0].Value = SetClutchActuator.InputValues[0].Choices.GetItemFromRawValue(0);
			SetClutchActuator.ServiceCompleteEvent += SetClutchActuator_ServiceCompleteEvent;
			AddLogLabel(Resources.Message_SettingClutchActuatorType);
			SetClutchActuator.Execute(synchronous: false);
		}
		else
		{
			AddLogLabel(Resources.Message_CannotSetClutchActuatorTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound);
			isServiceRunning = false;
		}
		UpdateUserInterface();
	}

	private void StartSetTransmissionType()
	{
		if (Online && SetTransType != null)
		{
			SetTransType.InputValues[0].Value = comboTransType.SelectedValue as Choice;
			SetTransType.ServiceCompleteEvent += SetTransType_ServiceCompleteEvent;
			AddLogLabel(Resources.Message_SettingTransmissionType);
			SetTransType.Execute(synchronous: false);
		}
		else
		{
			AddLogLabel(Resources.Message_CannotSetTransmissionTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound);
			isServiceRunning = false;
		}
		UpdateUserInterface();
	}

	private void StartReleaseTransportSecurity()
	{
		if (Online && ReleaseTransportSecurity != null)
		{
			ReleaseTransportSecurity.ServiceCompleteEvent += ReleaseTransportSecurity_ServiceCompleteEvent;
			AddLogLabel(Resources.Message_ReleasingTransportSecurity);
			ReleaseTransportSecurity.Execute(synchronous: false);
		}
		else
		{
			AddLogLabel(Resources.Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound);
			isServiceRunning = false;
		}
		UpdateUserInterface();
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (isServiceRunning && e.CloseReason == CloseReason.UserClosing)
		{
			e.Cancel = true;
		}
	}

	private void SetClutchActuator_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		SetClutchActuator.ServiceCompleteEvent -= SetClutchActuator_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			AddLogLabel(Resources.Message_SuccessfullySetTheClutchActuator);
			StartSetTransmissionType();
		}
		else
		{
			AddLogLabel(string.Format(Resources.MessageFormat_UnableToSetTheClutchActuatorError0, e.Exception.Message));
			isServiceRunning = false;
		}
		UpdateUserInterface();
	}

	private void SetTransType_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		SetTransType.ServiceCompleteEvent -= SetTransType_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			AddLogLabel(string.Format(Resources.MessageFormat_SuccessfullySetTheTransmissionTypeTo0, comboTransType.SelectedItem.ToString()));
			StartReleaseTransportSecurity();
		}
		else
		{
			AddLogLabel(string.Format(Resources.MessageFormat_UnableToSetTheTransmissionTypeError0, e.Exception.Message));
			isServiceRunning = false;
		}
		UpdateUserInterface();
	}

	private void ReleaseTransportSecurity_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		ReleaseTransportSecurity.ServiceCompleteEvent -= ReleaseTransportSecurity_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			AddLogLabel(Resources.Message_SuccessfullyReleasedTransportSecurity);
		}
		else
		{
			AddLogLabel(string.Format(Resources.MessageFormat_UnableToReleaseTransportSecurityError, e.Exception.Message));
		}
		isServiceRunning = false;
		TurnOffAutoConnect();
		UpdateUserInterface();
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewOutput.RequiredUserLabelPrefix, text);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableTransControls = new TableLayoutPanel();
		tableMain = new TableLayoutPanel();
		comboTransType = new ComboBox();
		btnSetTransmissionType = new Button();
		labelTcmStatus = new Label();
		checkmarkTcmOnline = new Checkmark();
		seekTimeListViewOutput = new SeekTimeListView();
		digitalReadoutInstrumentTransmissionType = new DigitalReadoutInstrument();
		((Control)(object)tableTransControls).SuspendLayout();
		((Control)(object)tableMain).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableTransControls, "tableTransControls");
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)tableTransControls, 2);
		((TableLayoutPanel)(object)tableTransControls).Controls.Add(comboTransType, 0, 0);
		((TableLayoutPanel)(object)tableTransControls).Controls.Add(btnSetTransmissionType, 1, 0);
		((Control)(object)tableTransControls).Name = "tableTransControls";
		componentResourceManager.ApplyResources(comboTransType, "comboTransType");
		comboTransType.DropDownStyle = ComboBoxStyle.DropDownList;
		comboTransType.FormattingEnabled = true;
		comboTransType.Name = "comboTransType";
		componentResourceManager.ApplyResources(btnSetTransmissionType, "btnSetTransmissionType");
		btnSetTransmissionType.Name = "btnSetTransmissionType";
		btnSetTransmissionType.UseCompatibleTextRendering = true;
		btnSetTransmissionType.UseVisualStyleBackColor = true;
		btnSetTransmissionType.Click += btnSetTransmissionType_Click;
		componentResourceManager.ApplyResources(tableMain, "tableMain");
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)labelTcmStatus, 1, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)checkmarkTcmOnline, 0, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)seekTimeListViewOutput, 0, 0);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)tableTransControls, 0, 3);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)digitalReadoutInstrumentTransmissionType, 0, 2);
		((Control)(object)tableMain).Name = "tableMain";
		labelTcmStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelTcmStatus, "labelTcmStatus");
		((Control)(object)labelTcmStatus).Name = "labelTcmStatus";
		labelTcmStatus.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(checkmarkTcmOnline, "checkmarkTcmOnline");
		((Control)(object)checkmarkTcmOnline).Name = "checkmarkTcmOnline";
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)seekTimeListViewOutput, 2);
		componentResourceManager.ApplyResources(seekTimeListViewOutput, "seekTimeListViewOutput");
		seekTimeListViewOutput.FilterUserLabels = true;
		((Control)(object)seekTimeListViewOutput).Name = "seekTimeListViewOutput";
		seekTimeListViewOutput.RequiredUserLabelPrefix = "tcmReplacementMy13";
		seekTimeListViewOutput.SelectedTime = null;
		seekTimeListViewOutput.ShowChannelLabels = false;
		seekTimeListViewOutput.ShowCommunicationsState = false;
		seekTimeListViewOutput.ShowControlPanel = false;
		seekTimeListViewOutput.ShowDeviceColumn = false;
		seekTimeListViewOutput.TimeFormat = "HH:mm:ss.f";
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)digitalReadoutInstrumentTransmissionType, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransmissionType, "digitalReadoutInstrumentTransmissionType");
		digitalReadoutInstrumentTransmissionType.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionType).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionType).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "CO_TransType");
		((Control)(object)digitalReadoutInstrumentTransmissionType).Name = "digitalReadoutInstrumentTransmissionType";
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionType).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_TCMReplacement");
		((Control)this).Controls.Add((Control)(object)tableMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableTransControls).ResumeLayout(performLayout: false);
		((Control)(object)tableTransControls).PerformLayout();
		((Control)(object)tableMain).ResumeLayout(performLayout: false);
		((Control)(object)tableMain).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
