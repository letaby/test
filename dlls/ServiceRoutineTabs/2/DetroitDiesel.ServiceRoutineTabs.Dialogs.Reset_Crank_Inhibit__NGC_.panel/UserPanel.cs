using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel;

public class UserPanel : CustomPanel
{
	private Channel ssam;

	private Channel ctp;

	private TableLayoutPanel tableMain;

	private Label labelResetStatus;

	private Checkmark checkmarkEcusOnline;

	private SeekTimeListView seekTimeListViewOutput;

	private TableLayoutPanel tableTransControls;

	private RunServicesButton runServicesButton;

	private DigitalReadoutInstrument digitalReadoutInstrumentFlashInProgress;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrumentCrankInhibit;

	private bool CanReset => (int)digitalReadoutInstrumentCrankInhibit.RepresentedState != 3 && (int)digitalReadoutInstrumentFlashInProgress.RepresentedState != 3 && Online;

	private bool Online => ssam != null && ssam.Online && ctp != null && ctp.Online;

	public UserPanel()
	{
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		SetSSAM(((CustomPanel)this).GetChannel("SSAM02T"));
		SetCTP(((CustomPanel)this).GetChannel("CTP01T"));
	}

	private void SetSSAM(Channel ssam)
	{
		if (this.ssam != ssam)
		{
			this.ssam = ssam;
			UpdateUserInterface();
		}
	}

	private void SetCTP(Channel ctp)
	{
		if (this.ctp != ctp)
		{
			this.ctp = ctp;
			UpdateUserInterface();
		}
	}

	private void UpdateUserInterface()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I4
		checkmarkEcusOnline.Checked = Online;
		((Control)(object)runServicesButton).Enabled = CanReset;
		if (((RunSharedProcedureButtonBase)runServicesButton).InProgress)
		{
			((Control)(object)labelResetStatus).Text = Resources.Message_DisablingCrankInhibit;
		}
		else if (!Online)
		{
			((Control)(object)labelResetStatus).Text = Resources.Message_EcusMustBeConnectedToDisableCrankInhibit;
		}
		else if ((int)digitalReadoutInstrumentCrankInhibit.RepresentedState == 3)
		{
			((Control)(object)labelResetStatus).Text = Resources.Message_CrankingIsNotInhibited;
		}
		else if ((int)digitalReadoutInstrumentFlashInProgress.RepresentedState == 3)
		{
			((Control)(object)labelResetStatus).Text = Resources.Message_FlashingOverTheAirIsCurrentlyInProgress;
		}
		else
		{
			((Control)(object)labelResetStatus).Text = Resources.Message_Ready;
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
			AddLogLabel(Resources.Message_CrankInhibitWasDisabled);
		}
		else
		{
			AddLogLabel(Resources.Message_UnableToDisableCrankInhibit);
		}
		UpdateUserInterface();
	}

	private void runServicesButton_Starting(object sender, CancelEventArgs e)
	{
		AddLogLabel(Resources.Message_DisablingCrankInhibit);
		UpdateUserInterface();
	}

	private void runServicesButton_ButtonEnabledChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void runServicesButton_Started(object sender, PassFailResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrument2_RepresentedStateChanged(object sender, EventArgs e)
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
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableMain = new TableLayoutPanel();
		labelResetStatus = new Label();
		checkmarkEcusOnline = new Checkmark();
		seekTimeListViewOutput = new SeekTimeListView();
		tableTransControls = new TableLayoutPanel();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		runServicesButton = new RunServicesButton();
		digitalReadoutInstrumentFlashInProgress = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCrankInhibit = new DigitalReadoutInstrument();
		((Control)(object)tableMain).SuspendLayout();
		((Control)(object)tableTransControls).SuspendLayout();
		((ISupportInitialize)runServicesButton).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableMain, "tableMain");
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)labelResetStatus, 1, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)checkmarkEcusOnline, 0, 1);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)seekTimeListViewOutput, 0, 0);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)tableTransControls, 0, 3);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)digitalReadoutInstrumentFlashInProgress, 2, 2);
		((TableLayoutPanel)(object)tableMain).Controls.Add((Control)(object)digitalReadoutInstrumentCrankInhibit, 0, 2);
		((Control)(object)tableMain).Name = "tableMain";
		labelResetStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)labelResetStatus, 2);
		componentResourceManager.ApplyResources(labelResetStatus, "labelResetStatus");
		((Control)(object)labelResetStatus).Name = "labelResetStatus";
		labelResetStatus.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(checkmarkEcusOnline, "checkmarkEcusOnline");
		((Control)(object)checkmarkEcusOnline).Name = "checkmarkEcusOnline";
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)seekTimeListViewOutput, 3);
		componentResourceManager.ApplyResources(seekTimeListViewOutput, "seekTimeListViewOutput");
		seekTimeListViewOutput.FilterUserLabels = true;
		((Control)(object)seekTimeListViewOutput).Name = "seekTimeListViewOutput";
		seekTimeListViewOutput.RequiredUserLabelPrefix = "CrankInhibitReset";
		seekTimeListViewOutput.SelectedTime = null;
		seekTimeListViewOutput.ShowChannelLabels = false;
		seekTimeListViewOutput.ShowCommunicationsState = false;
		seekTimeListViewOutput.ShowControlPanel = false;
		seekTimeListViewOutput.ShowDeviceColumn = false;
		seekTimeListViewOutput.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(tableTransControls, "tableTransControls");
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)tableTransControls, 3);
		((TableLayoutPanel)(object)tableTransControls).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 0);
		((TableLayoutPanel)(object)tableTransControls).Controls.Add((Control)(object)runServicesButton, 1, 0);
		((Control)(object)tableTransControls).Name = "tableTransControls";
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)64, "CTP01T", "RT_Reset_Crank_inhibition_Start_status");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runServicesButton, "runServicesButton");
		((Control)(object)runServicesButton).Name = "runServicesButton";
		runServicesButton.ServiceCalls.Add(new ServiceCall("CTP01T", "RT_Reset_Crank_inhibition_Start_status", (IEnumerable<string>)new string[1] { "InhibitionStatus=0" }));
		runServicesButton.Complete += runServicesButton_Complete;
		((RunSharedProcedureButtonBase)runServicesButton).Starting += runServicesButton_Starting;
		((RunSharedProcedureButtonBase)runServicesButton).Started += runServicesButton_Started;
		((RunSharedProcedureButtonBase)runServicesButton).ButtonEnabledChanged += runServicesButton_ButtonEnabledChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFlashInProgress, "digitalReadoutInstrumentFlashInProgress");
		digitalReadoutInstrumentFlashInProgress.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentFlashInProgress).FreezeValue = false;
		digitalReadoutInstrumentFlashInProgress.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentFlashInProgress.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentFlashInProgress.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentFlashInProgress.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentFlashInProgress.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentFlashInProgress).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_FOTA_InProcess");
		((Control)(object)digitalReadoutInstrumentFlashInProgress).Name = "digitalReadoutInstrumentFlashInProgress";
		((SingleInstrumentBase)digitalReadoutInstrumentFlashInProgress).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentFlashInProgress.RepresentedStateChanged += digitalReadoutInstrument1_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableMain).SetColumnSpan((Control)(object)digitalReadoutInstrumentCrankInhibit, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCrankInhibit, "digitalReadoutInstrumentCrankInhibit");
		digitalReadoutInstrumentCrankInhibit.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCrankInhibit).FreezeValue = false;
		digitalReadoutInstrumentCrankInhibit.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentCrankInhibit.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentCrankInhibit.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentCrankInhibit.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentCrankInhibit.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentCrankInhibit).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_CrankIntrlService_Cmd");
		((Control)(object)digitalReadoutInstrumentCrankInhibit).Name = "digitalReadoutInstrumentCrankInhibit";
		((SingleInstrumentBase)digitalReadoutInstrumentCrankInhibit).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentCrankInhibit.RepresentedStateChanged += digitalReadoutInstrument2_RepresentedStateChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableMain).ResumeLayout(performLayout: false);
		((Control)(object)tableMain).PerformLayout();
		((Control)(object)tableTransControls).ResumeLayout(performLayout: false);
		((ISupportInitialize)runServicesButton).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
