using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel;

public class UserPanel : CustomPanel
{
	private readonly string[] professionalProcedures = new string[1] { "SP_ICUC01T_AutoConfig" };

	private readonly string[] engineeringProcedures = new string[3] { "SP_ICUC01T_AutoConfig", "SP_ICUC01T_AutoConfig_PID20", "SP_ICUC01T_AutoConfig_PID25" };

	private TableLayoutPanel tableLayoutPanel;

	private SeekTimeListView seekTimeListView;

	private SharedProcedureSelection sharedProcedureSelection;

	private Button button;

	private System.Windows.Forms.Label labelStatus;

	private Checkmark checkmark;

	private System.Windows.Forms.Label labelInstructions;

	private DigitalReadoutInstrument digitalReadoutInstrumentAutoConfigFault;

	private System.Windows.Forms.Label label1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	public UserPanel()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		InitializeComponent();
		seekTimeListView.RequiredUserLabelPrefix = sharedProcedureSelection.SelectedProcedure.Name;
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)(ApplicationInformation.IsProductName("Engineering") ? engineeringProcedures : professionalProcedures));
		UpdateInstruments();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateInstruments();
	}

	private void UpdateInstruments()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Channel channel = SapiManager.GlobalInstance.Sapi.Channels.FirstOrDefault((Channel c) => c.Ecu.Identifier == "UDS-23");
		string text = ((channel != null) ? channel.Ecu.Name : "ICUC01T");
		labelInstructions.Text = Resources.Message_ICUCNotPerformed;
		label1.Text = Resources.Message_ICUCSMF;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)32, text, "0DFBFF");
		((SingleInstrumentBase)digitalReadoutInstrumentAutoConfigFault).Instrument = new Qualifier((QualifierTypes)32, text, "0FFBFF");
	}

	private void sharedProcedureSelection_StatusReport(object sender, StatusReportEventArgs e)
	{
		((Control)(object)sharedProcedureSelection).Enabled = !sharedProcedureSelection.AnyProcedureInProgress;
	}

	private void sharedProcedureSelection_SelectionChanged(object sender, EventArgs e)
	{
		seekTimeListView.RequiredUserLabelPrefix = sharedProcedureSelection.SelectedProcedure.Name;
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Expected O, but got Unknown
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		label1 = new System.Windows.Forms.Label();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		button = new Button();
		labelStatus = new System.Windows.Forms.Label();
		checkmark = new Checkmark();
		labelInstructions = new System.Windows.Forms.Label();
		digitalReadoutInstrumentAutoConfigFault = new DigitalReadoutInstrument();
		sharedProcedureSelection = new SharedProcedureSelection();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(label1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrument1, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(button, 4, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelStatus, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)checkmark, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelInstructions, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentAutoConfigFault, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)sharedProcedureSelection, 3, 4);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)label1, 3);
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)32, "ICUC01T", "0DFBFF");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)seekTimeListView, 5);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "ICUCAutoConfig";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(button, "button");
		button.Name = "button";
		button.UseCompatibleTextRendering = true;
		button.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)labelStatus, 2);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)labelStatus, 3);
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark, "checkmark");
		((Control)(object)checkmark).Name = "checkmark";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)labelInstructions, 3);
		componentResourceManager.ApplyResources(labelInstructions, "labelInstructions");
		labelInstructions.Name = "labelInstructions";
		labelInstructions.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentAutoConfigFault, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAutoConfigFault, "digitalReadoutInstrumentAutoConfigFault");
		digitalReadoutInstrumentAutoConfigFault.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAutoConfigFault).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentAutoConfigFault).Instrument = new Qualifier((QualifierTypes)32, "ICUC01T", "0FFBFF");
		((Control)(object)digitalReadoutInstrumentAutoConfigFault).Name = "digitalReadoutInstrumentAutoConfigFault";
		((SingleInstrumentBase)digitalReadoutInstrumentAutoConfigFault).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[3] { "SP_ICUC01T_AutoConfig", "SP_ICUC01T_AutoConfig_PID20", "SP_ICUC01T_AutoConfig_PID25" });
		sharedProcedureSelection.StatusReport += sharedProcedureSelection_StatusReport;
		sharedProcedureSelection.SelectionChanged += sharedProcedureSelection_SelectionChanged;
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmark;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = button;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_ICUC_Automatic_Configuration");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
