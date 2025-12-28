using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EDAC_Oil_Life_Reset__EMG_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel;

	private RunServiceButton runServiceButtonResetOilChange;

	private System.Windows.Forms.Label label;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void runServiceButtonResetOilChange_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		ControlHelpers.ShowMessageBox(((ResultEventArgs)(object)e).Succeeded ? Resources.Message_TheResetOperationSucceeded : ((Resources.Message_TheResetOperationFailed + ((ResultEventArgs)(object)e).Exception.Message) ?? string.Empty), MessageBoxButtons.OK, ((ResultEventArgs)(object)e).Succeeded ? MessageBoxIcon.Asterisk : MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (((RunSharedProcedureButtonBase)runServiceButtonResetOilChange).InProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel && runServiceButtonResetOilChange != null)
		{
			runServiceButtonResetOilChange.ServiceComplete -= runServiceButtonResetOilChange_ServiceComplete;
			((CustomPanel)this).ParentFormClosing -= UserPanel_ParentFormClosing;
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		runServiceButtonResetOilChange = new RunServiceButton();
		label = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)runServiceButtonResetOilChange, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(label, 0, 0);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(runServiceButtonResetOilChange, "runServiceButtonResetOilChange");
		((Control)(object)runServiceButtonResetOilChange).Name = "runServiceButtonResetOilChange";
		runServiceButtonResetOilChange.ServiceCall = new ServiceCall("EAPU03T", "RT_Reset_oil_change_period_Start");
		runServiceButtonResetOilChange.ServiceComplete += runServiceButtonResetOilChange_ServiceComplete;
		componentResourceManager.ApplyResources(label, "label");
		label.Name = "label";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_EDAC_Oil_Life_Reset");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
