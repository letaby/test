using System;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel;

public class UserPanel : CustomPanel
{
	private const string CTP01T = "CTP01T";

	private const string ResetServiceQualifier = "ResetCalibrationsToDefaultService";

	private Channel ctp;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView;

	private Button buttonClose;

	private Button buttonFactoryReset;

	private Checkmark checkmarkReady;

	private System.Windows.Forms.Label labelStatus;

	private bool CanClose => ctp == null || !Busy;

	public bool CanStart => ctp != null && !Busy && ctp.Ecu.Properties.ContainsKey("ResetCalibrationsToDefaultService");

	public bool Busy { get; private set; }

	public bool RoutineComplete { get; private set; }

	public bool Result { get; private set; }

	public string ResultMessage { get; private set; }

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		Result = false;
		ResultMessage = Resources.Message_None;
		UpdateChannels();
		UpdateUserInterface();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = !CanClose;
		if (CanClose)
		{
			((Control)this).Tag = new object[2] { Result, ResultMessage };
			buttonClose.DialogResult = (Result ? DialogResult.Yes : DialogResult.No);
			if (((ContainerControl)this).ParentForm != null)
			{
				((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
	}

	private void UpdateChannels()
	{
		Channel channel = ((CustomPanel)this).GetChannel("CTP01T", (ChannelLookupOptions)5);
		if (ctp != channel)
		{
			ctp = channel;
			ResetResults();
		}
	}

	private void LogMessage(string message)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, message);
		((CustomPanel)this).AddStatusMessage(message);
	}

	private void ResetResults()
	{
		RoutineComplete = false;
		Result = false;
		ResultMessage = Resources.Message_None;
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		buttonClose.Enabled = CanClose;
		buttonFactoryReset.Enabled = CanStart;
		if (ctp == null)
		{
			labelStatus.Text = Resources.Message_CTPIsOffline;
			checkmarkReady.Checked = false;
			return;
		}
		if (Busy)
		{
			checkmarkReady.CheckState = CheckState.Indeterminate;
			labelStatus.Text = Resources.Message_PerformingFactoryResetRoutine;
			return;
		}
		if (RoutineComplete)
		{
			checkmarkReady.Checked = Result;
			labelStatus.Text = ResultMessage;
			return;
		}
		checkmarkReady.Checked = CanStart;
		if (CanStart)
		{
			labelStatus.Text = Resources.Message_Ready;
		}
		else
		{
			labelStatus.Text = Resources.Message_ErrorCannotStartRoutine;
		}
	}

	private void ExecuteReloadService()
	{
		ctp.Services.ServiceCompleteEvent += ReloadOriginalSupplier_ServiceCompleteEvent;
		ctp.Services.Execute(ctp.Ecu.Properties["ResetCalibrationsToDefaultService"], synchronous: false);
		LogMessage(Resources.Message_ReloadSupplierConfigurationServiceExecuted);
		UpdateUserInterface();
	}

	private void ReloadOriginalSupplier_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		ctp.Services.ServiceCompleteEvent -= ReloadOriginalSupplier_ServiceCompleteEvent;
		Result = e.Succeeded;
		if (e.Succeeded)
		{
			LogMessage(Resources.Message_ReloadSupplierConfigurationExecutedSuccessfully);
			ResultMessage = Resources.Message_FactoryResetRoutineCompletedSuccessfully;
		}
		else
		{
			LogMessage(Resources.Message_ReloadSupplierConfigurationFailed);
			ResultMessage = Resources.Message_FactoryResetRoutineFAILED;
		}
		Application.DoEvents();
		LogMessage(Resources.Message_Finished);
		RoutineComplete = true;
		Busy = false;
		UpdateUserInterface();
	}

	private void buttonFactoryReset_Click(object sender, EventArgs e)
	{
		Busy = true;
		LogMessage(Resources.Message_UserRequestedCTPFactoryReset);
		ResetResults();
		ExecuteReloadService();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		buttonClose = new Button();
		buttonFactoryReset = new Button();
		checkmarkReady = new Checkmark();
		labelStatus = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonFactoryReset, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkReady, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 5);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "CTPFactoryReset";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonFactoryReset, "buttonFactoryReset");
		buttonFactoryReset.Name = "buttonFactoryReset";
		buttonFactoryReset.UseCompatibleTextRendering = true;
		buttonFactoryReset.UseVisualStyleBackColor = true;
		buttonFactoryReset.Click += buttonFactoryReset_Click;
		componentResourceManager.ApplyResources(checkmarkReady, "checkmarkReady");
		((Control)(object)checkmarkReady).Name = "checkmarkReady";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
