using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data_45X_.panel;

public class UserPanel : CustomPanel
{
	private SeekTimeListView seekTimeListView1;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private RunServiceButton runServiceButtonStart;

	private Checkmark checkmark1;

	private System.Windows.Forms.Label label1;

	private Button button2;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (((RunSharedProcedureButtonBase)runServiceButtonStart).InProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((CustomPanel)this).ParentFormClosing -= UserPanel_ParentFormClosing;
		}
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
	}

	private void runServiceButtonStart_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			LogText(Resources.Message_CompleteSuccess);
			checkmark1.CheckState = CheckState.Checked;
			label1.Text = Resources.Message_CompleteSuccess;
			return;
		}
		checkmark1.CheckState = CheckState.Unchecked;
		label1.Text = Resources.Message_Error;
		LogText(Resources.Message_Error);
		if (((ResultEventArgs)(object)e).Exception != null)
		{
			LogText(((ResultEventArgs)(object)e).Exception.Message);
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanel1 = new TableLayoutPanel();
		checkmark1 = new Checkmark();
		label1 = new System.Windows.Forms.Label();
		button2 = new Button();
		runServiceButtonStart = new RunServiceButton();
		tableLayoutPanel2 = new TableLayoutPanel();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "MaintenanceSystemTransfer45X";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button2, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonStart, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		checkmark1.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		button2.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(button2, "button2");
		button2.Name = "button2";
		button2.UseCompatibleTextRendering = true;
		button2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(runServiceButtonStart, "runServiceButtonStart");
		((Control)(object)runServiceButtonStart).Name = "runServiceButtonStart";
		runServiceButtonStart.ServiceCall = new ServiceCall("CGW05T", "RT_Transfer_data_from_the_mirror_memory_Start");
		runServiceButtonStart.ServiceComplete += runServiceButtonStart_ServiceComplete;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)seekTimeListView1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel1, 0, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
