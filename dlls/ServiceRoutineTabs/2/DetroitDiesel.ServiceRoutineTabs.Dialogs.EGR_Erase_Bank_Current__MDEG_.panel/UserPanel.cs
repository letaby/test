using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Erase_Bank_Current__MDEG_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanelWholePanel;

	private Button buttonClose;

	private System.Windows.Forms.Label labelDirections;

	private SeekTimeListView seekTimeListViewLog;

	private RunServiceButton runServiceButtonEraseBank;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		labelDirections.Text = Resources.Label_ClickButtonToEraseBank;
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

	private void runServiceButtonEraseBank_ServiceComplete(object sender, SingleServiceResultEventArgs e)
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
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		seekTimeListViewLog = new SeekTimeListView();
		buttonClose = new Button();
		labelDirections = new System.Windows.Forms.Label();
		runServiceButtonEraseBank = new RunServiceButton();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonClose, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelDirections, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)runServiceButtonEraseBank, 0, 1);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "EGR_EraseBank";
		seekTimeListViewLog.FilterUserLabels = true;
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
		componentResourceManager.ApplyResources(runServiceButtonEraseBank, "runServiceButtonEraseBank");
		((Control)(object)runServiceButtonEraseBank).Name = "runServiceButtonEraseBank";
		runServiceButtonEraseBank.ServiceCall = new ServiceCall("MCM21T", "RT_SR0C8_Erase_bank_current_Start");
		runServiceButtonEraseBank.ServiceComplete += runServiceButtonEraseBank_ServiceComplete;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
