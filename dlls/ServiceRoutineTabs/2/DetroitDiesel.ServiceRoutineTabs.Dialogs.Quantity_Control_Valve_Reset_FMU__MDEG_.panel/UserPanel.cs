using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Quantity_Control_Valve_Reset_FMU__MDEG_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanelWholePanel;

	private Button buttonClose;

	private System.Windows.Forms.Label labelDirections;

	private SeekTimeListView seekTimeListViewLog;

	private RunServiceButton runServiceButtonResetFMU;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		labelDirections.Text = Resources.Label_ClickButtonToResetFMU;
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void runServiceButtonResetFMU_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			LogText(Resources.Message_SuccessfulExecution);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(Resources.Message_FailedExecution);
		if (((ResultEventArgs)(object)e).Exception != null && !string.IsNullOrEmpty(((ResultEventArgs)(object)e).Exception.Message))
		{
			stringBuilder.Append(Resources.Message_Error);
			stringBuilder.Append(((ResultEventArgs)(object)e).Exception.Message);
		}
		LogText(stringBuilder.ToString());
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewLog.RequiredUserLabelPrefix, text);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		seekTimeListViewLog = new SeekTimeListView();
		buttonClose = new Button();
		labelDirections = new System.Windows.Forms.Label();
		runServiceButtonResetFMU = new RunServiceButton();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonClose, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelDirections, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)runServiceButtonResetFMU, 0, 1);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "QCV_Reset_FMU";
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		seekTimeListViewLog.TimeFormat = "HH:mm:ss";
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelDirections, "labelDirections");
		labelDirections.Name = "labelDirections";
		labelDirections.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonResetFMU, "runServiceButtonResetFMU");
		((Control)(object)runServiceButtonResetFMU).Name = "runServiceButtonResetFMU";
		runServiceButtonResetFMU.ServiceCall = new ServiceCall("MCM21T", "RT_SR014_SET_EOL_Default_Values_Start", (IEnumerable<string>)new string[1] { "E2P_Logical_Block_Number=25" });
		runServiceButtonResetFMU.ServiceComplete += runServiceButtonResetFMU_ServiceComplete;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
