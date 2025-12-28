using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Replacement__MY16_.panel;

public class UserPanel : CustomPanel
{
	private RunServiceButton runServiceButton;

	private TableLayoutPanel tableLayoutPanel1;

	private System.Windows.Forms.Label label1;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Exception != null)
		{
			MessageBox.Show((IWin32Window)this, ((ResultEventArgs)(object)e).Exception.Message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			MessageBox.Show((IWin32Window)this, Resources.Message_TheRoutineWasSuccessful, ApplicationInformation.ProductName, MessageBoxButtons.OK);
		}
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		runServiceButton = new RunServiceButton();
		label1 = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButton, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(runServiceButton, "runServiceButton");
		((Control)(object)runServiceButton).Name = "runServiceButton";
		runServiceButton.ServiceCall = new ServiceCall("MCM21T", "RT_SR0C6_Reset_EGR_values_Start", (IEnumerable<string>)new string[1] { "Reset_Type=255" });
		runServiceButton.ServiceComplete += runServiceButton_ServiceComplete;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label1, 2);
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
