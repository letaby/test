using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel;

public class UserPanel : CustomPanel
{
	private const string ReleaseTransportSecurityQualifier = "DJ_Release_transport_security_for_TCM";

	private const string AntiTheftSecurityQualifier = "DJ_SecurityAccess_AntiTheftInit";

	private const string RoutineSecurityQualifier = "DJ_SecurityAccess_RoutineIO_AS";

	private Channel tcm;

	private Service ReleaseTransportSecurity;

	private Service AntiTheftSecurity;

	private Service RoutineSecurity;

	private bool isServiceRunning = false;

	private Button btnReleaseLock;

	private Checkmark checkmarkTcmOnline;

	private Label labelTcmStatus;

	private TableLayoutPanel tableMain;

	private SeekTimeListView seekTimeListViewOutput;

	private TableLayoutPanel tableTransControls;

	private bool Online => tcm != null && tcm.Online;

	public UserPanel()
	{
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		SetTcm(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
	}

	private void SetTcm(Channel tcm)
	{
		if (this.tcm != tcm)
		{
			if (this.tcm != null)
			{
				this.tcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				ReleaseTransportSecurity = null;
				AntiTheftSecurity = null;
				RoutineSecurity = null;
			}
			this.tcm = tcm;
			if (this.tcm != null)
			{
				this.tcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				ReleaseTransportSecurity = this.tcm.Services["DJ_Release_transport_security_for_TCM"];
				AntiTheftSecurity = ((this.tcm.Ecu.Name == "TCM05T") ? this.tcm.Services["DJ_SecurityAccess_AntiTheftInit"] : null);
				RoutineSecurity = ((this.tcm.Ecu.Name == "TCM05T") ? this.tcm.Services["DJ_SecurityAccess_RoutineIO_AS"] : null);
			}
			UpdateUserInterface();
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		checkmarkTcmOnline.Checked = Online;
		if (isServiceRunning)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_LockIsBeingReleased;
			btnReleaseLock.Enabled = false;
		}
		else if (!Online)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_TheLockCannotBeReleasedBecauseTheTCMIsOffline;
			btnReleaseLock.Enabled = false;
		}
		else
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_Ready;
			btnReleaseLock.Enabled = true;
		}
	}

	private void btnReleaseLock_Click(object sender, EventArgs e)
	{
		isServiceRunning = true;
		StartReleaseTransportSecurity();
	}

	private void StartReleaseTransportSecurity()
	{
		if (Online && AntiTheftSecurity != null)
		{
			AntiTheftSecurity.ServiceCompleteEvent += AntiTheftSecurity_ServiceCompleteEvent;
			AntiTheftSecurity.Execute(synchronous: false);
		}
		else if (Online && ReleaseTransportSecurity != null)
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

	private void AntiTheftSecurity_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		AntiTheftSecurity.ServiceCompleteEvent -= AntiTheftSecurity_ServiceCompleteEvent;
		if (Online && ReleaseTransportSecurity != null)
		{
			ReleaseTransportSecurity.ServiceCompleteEvent += ReleaseTransportSecurity_ServiceCompleteEvent;
			AddLogLabel(Resources.Message_ReleasingTransportSecurity);
			ReleaseTransportSecurity.Execute(synchronous: false);
		}
		else
		{
			isServiceRunning = false;
			UpdateUserInterface();
		}
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
			AddLogLabel(string.Format(Resources.MessageFormat_UnableToReleaseTransportSecurityError0, e.Exception.Message));
		}
		if (Online && RoutineSecurity != null)
		{
			RoutineSecurity.ServiceCompleteEvent += RoutineSecurity_ServiceServiceComplete;
			RoutineSecurity.Execute(synchronous: false);
		}
		else
		{
			isServiceRunning = false;
			UpdateUserInterface();
		}
	}

	private void RoutineSecurity_ServiceServiceComplete(object sender, ResultEventArgs e)
	{
		RoutineSecurity.ServiceCompleteEvent -= RoutineSecurity_ServiceServiceComplete;
		isServiceRunning = false;
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableTransControls = new TableLayoutPanel();
		btnReleaseLock = new Button();
		tableMain = new TableLayoutPanel();
		labelTcmStatus = new Label();
		checkmarkTcmOnline = new Checkmark();
		seekTimeListViewOutput = new SeekTimeListView();
		((Control)(object)tableTransControls).SuspendLayout();
		((Control)(object)tableMain).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableTransControls, "tableTransControls");
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)tableTransControls, 2);
		((TableLayoutPanel)(object)tableTransControls).Controls.Add(btnReleaseLock, 1, 0);
		((Control)(object)tableTransControls).Name = "tableTransControls";
		componentResourceManager.ApplyResources(btnReleaseLock, "btnReleaseLock");
		btnReleaseLock.Name = "btnReleaseLock";
		btnReleaseLock.UseCompatibleTextRendering = true;
		btnReleaseLock.UseVisualStyleBackColor = true;
		btnReleaseLock.Click += btnReleaseLock_Click;
		componentResourceManager.ApplyResources(tableMain, "tableMain");
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)labelTcmStatus, 1, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)checkmarkTcmOnline, 0, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)seekTimeListViewOutput, 0, 0);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)tableTransControls, 0, 3);
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
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_TCM_Release_Lock");
		((Control)this).Controls.Add((Control)(object)tableMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableTransControls).ResumeLayout(performLayout: false);
		((Control)(object)tableMain).ResumeLayout(performLayout: false);
		((Control)(object)tableMain).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
