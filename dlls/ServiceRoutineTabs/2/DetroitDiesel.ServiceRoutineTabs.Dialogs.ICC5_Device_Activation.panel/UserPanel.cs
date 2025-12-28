using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICC5_Device_Activation.panel;

public class UserPanel : CustomPanel
{
	private Checkmark checkmark;

	private Label label;

	private SharedProcedureSelection sharedProcedureSelection;

	private Button button;

	private SeekTimeListView seekTimeListView1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private TableLayoutPanel tableLayoutPanel;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		checkmark = new Checkmark();
		label = new Label();
		sharedProcedureSelection = new SharedProcedureSelection();
		button = new Button();
		seekTimeListView1 = new SeekTimeListView();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)checkmark, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(label, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)sharedProcedureSelection, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(button, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView1, 0, 0);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(checkmark, "checkmark");
		((Control)(object)checkmark).Name = "checkmark";
		componentResourceManager.ApplyResources(label, "label");
		label.Name = "label";
		label.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_ICC5_DeviceActivation" });
		componentResourceManager.ApplyResources(button, "button");
		button.Name = "button";
		button.UseCompatibleTextRendering = true;
		button.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)seekTimeListView1, 4);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "ICC5 Device Activation";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss.fff";
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = label;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmark;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = button;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
