using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset.panel;

public class UserPanel : CustomPanel
{
	private Channel vrdu;

	private Checkmark checkmarkTcmOnline;

	private Label labelTcmStatus;

	private TableLayoutPanel tableMain;

	private SeekTimeListView seekTimeListViewOutput;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private RunServicesButton runServicesButton;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private TableLayoutPanel tableTransControls;

	private bool Online => vrdu != null && vrdu.Online;

	public UserPanel()
	{
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		SetTcm(((CustomPanel)this).GetChannel("VRDU01T"));
	}

	private void SetTcm(Channel vrdu)
	{
		if (this.vrdu != vrdu)
		{
			this.vrdu = vrdu;
			UpdateUserInterface();
		}
	}

	private void UpdateUserInterface()
	{
		checkmarkTcmOnline.Checked = Online;
		if (((RunSharedProcedureButtonBase)runServicesButton).InProgress)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_ResettingTheFault;
		}
		else if (!Online)
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_TheFaultCannotBeResetBecauseTheVRDUIsOffline;
		}
		else
		{
			((Control)(object)labelTcmStatus).Text = Resources.Message_Ready;
		}
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (((RunSharedProcedureButtonBase)runServicesButton).InProgress && e.CloseReason == CloseReason.UserClosing)
		{
			e.Cancel = true;
		}
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewOutput.RequiredUserLabelPrefix, text);
	}

	private void runServicesButton_Complete(object sender, PassFailResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(string.Format(Resources.MessageFormat_TheFaultHasBeenReset0, ((SingleInstrumentBase)digitalReadoutInstrument1).DataItem.Value));
		}
		else
		{
			AddLogLabel(string.Format(Resources.MessageFormat_UnableToResetError0, ((ResultEventArgs)(object)e).Exception.Message));
		}
		UpdateUserInterface();
	}

	private void runServicesButton_Starting(object sender, CancelEventArgs e)
	{
		AddLogLabel(Resources.Message_ResettingTheFault);
		UpdateUserInterface();
	}

	private void runServicesButton_ButtonEnabledChanged(object sender, EventArgs e)
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
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableMain = new TableLayoutPanel();
		tableTransControls = new TableLayoutPanel();
		runServicesButton = new RunServicesButton();
		labelTcmStatus = new Label();
		checkmarkTcmOnline = new Checkmark();
		seekTimeListViewOutput = new SeekTimeListView();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		((Control)(object)tableTransControls).SuspendLayout();
		((ISupportInitialize)runServicesButton).BeginInit();
		((Control)(object)tableMain).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableTransControls, "tableTransControls");
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)tableTransControls, 3);
		((TableLayoutPanel)(object)tableTransControls).Controls.Add((Control)(object)runServicesButton, 1, 0);
		((Control)(object)tableTransControls).Name = "tableTransControls";
		componentResourceManager.ApplyResources(runServicesButton, "runServicesButton");
		((Control)(object)runServicesButton).Name = "runServicesButton";
		runServicesButton.ServiceCalls.Add(new ServiceCall("VRDU01T", "DJ_SecurityAccess_Routine"));
		runServicesButton.ServiceCalls.Add(new ServiceCall("VRDU01T", "RT_Delete_permanent_errors_Start"));
		runServicesButton.ServiceCalls.Add(new ServiceCall("VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results"));
		runServicesButton.Complete += runServicesButton_Complete;
		((RunSharedProcedureButtonBase)runServicesButton).Starting += runServicesButton_Starting;
		((RunSharedProcedureButtonBase)runServicesButton).ButtonEnabledChanged += runServicesButton_ButtonEnabledChanged;
		componentResourceManager.ApplyResources(tableMain, "tableMain");
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)labelTcmStatus, 1, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)checkmarkTcmOnline, 0, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)seekTimeListViewOutput, 0, 0);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)tableTransControls, 0, 3);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)digitalReadoutInstrument1, 2, 2);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 2);
		((Control)(object)tableMain).Name = "tableMain";
		labelTcmStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)labelTcmStatus, 2);
		componentResourceManager.ApplyResources(labelTcmStatus, "labelTcmStatus");
		((Control)(object)labelTcmStatus).Name = "labelTcmStatus";
		labelTcmStatus.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(checkmarkTcmOnline, "checkmarkTcmOnline");
		((Control)(object)checkmarkTcmOnline).Name = "checkmarkTcmOnline";
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)seekTimeListViewOutput, 3);
		componentResourceManager.ApplyResources(seekTimeListViewOutput, "seekTimeListViewOutput");
		seekTimeListViewOutput.FilterUserLabels = true;
		((Control)(object)seekTimeListViewOutput).Name = "seekTimeListViewOutput";
		seekTimeListViewOutput.RequiredUserLabelPrefix = "vrduABAMisuseReset";
		seekTimeListViewOutput.SelectedTime = null;
		seekTimeListViewOutput.ShowChannelLabels = false;
		seekTimeListViewOutput.ShowCommunicationsState = false;
		seekTimeListViewOutput.ShowControlPanel = false;
		seekTimeListViewOutput.ShowDeviceColumn = false;
		seekTimeListViewOutput.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)64, "VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)32, "VRDU01T", "02FBFF");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableTransControls).ResumeLayout(performLayout: false);
		((ISupportInitialize)runServicesButton).EndInit();
		((Control)(object)tableMain).ResumeLayout(performLayout: false);
		((Control)(object)tableMain).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
