using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Air_Dryer_Cartridge_Replacement__EMG_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel;

	private RunServiceButton runServiceButtonResetWetness;

	private System.Windows.Forms.Label label;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void runServiceButtonResetWetness_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		ControlHelpers.ShowMessageBox(((ResultEventArgs)(object)e).Succeeded ? Resources.Message_TheResetOperationSucceeded : ((Resources.Message_TheResetOperationFailed + ((ResultEventArgs)(object)e).Exception.Message) ?? string.Empty), MessageBoxButtons.OK, ((ResultEventArgs)(object)e).Succeeded ? MessageBoxIcon.Asterisk : MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		runServiceButtonResetWetness = new RunServiceButton();
		label = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)runServiceButtonResetWetness, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(label, 0, 0);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(runServiceButtonResetWetness, "runServiceButtonResetWetness");
		((Control)(object)runServiceButtonResetWetness).Name = "runServiceButtonResetWetness";
		runServiceButtonResetWetness.ServiceCall = new ServiceCall("EAPU03T", "RT_Reset_wetness_Start");
		runServiceButtonResetWetness.ServiceComplete += runServiceButtonResetWetness_ServiceComplete;
		componentResourceManager.ApplyResources(label, "label");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)label, 2);
		label.Name = "label";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
