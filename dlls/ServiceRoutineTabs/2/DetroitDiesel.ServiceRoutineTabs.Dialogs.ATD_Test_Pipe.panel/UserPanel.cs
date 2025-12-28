using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel;

public class UserPanel : CustomPanel
{
	private const int MaxDuration = 120;

	private const int MillisecondsPerMinute = 60000;

	private const int DefaultDuration = 30;

	private static readonly Regex DurationValidation = new Regex("\\d{1,3}", RegexOptions.Compiled);

	private RunServiceButton runServiceButtonStart;

	private TextBox textboxResults;

	private Button buttonClose;

	private TextBox textBoxDuration;

	private TableLayoutPanel tableMainLayout;

	private TableLayoutPanel tableDuration;

	private System.Windows.Forms.Label labelDuration;

	private System.Windows.Forms.Label labelMinutes;

	private RunServiceButton runServiceButtonStop;

	public UserPanel()
	{
		InitializeComponent();
		textBoxDuration.KeyPress += OnDurationKeyPress;
		textBoxDuration.TextChanged += OnDurationTextChanged;
		((RunSharedProcedureButtonBase)runServiceButtonStart).ButtonEnabledChanged += OnStartButtonEnabledChanged;
		((RunSharedProcedureButtonBase)runServiceButtonStart).Starting += OnStartStarting;
		runServiceButtonStart.ServiceComplete += OnStartFinished;
		((RunSharedProcedureButtonBase)runServiceButtonStop).Starting += OnStopStarting;
		runServiceButtonStop.ServiceComplete += OnStopFinished;
	}

	protected override void OnLoad(EventArgs e)
	{
		ClearResults();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		textBoxDuration.Text = 30.ToString();
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateUserInterface();
	}

	private void OnDurationKeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar != '\b' && !DurationValidation.IsMatch(e.KeyChar.ToString()))
		{
			e.Handled = true;
		}
	}

	private void OnDurationTextChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private bool ValidateAndSetDuration()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (!((RunSharedProcedureButtonBase)runServiceButtonStart).IsBusy)
		{
			string text = textBoxDuration.Text;
			if (!string.IsNullOrEmpty(text) && DurationValidation.IsMatch(text) && int.TryParse(text, out var result) && 0 <= result && result <= 120)
			{
				ServiceCall serviceCall = runServiceButtonStart.ServiceCall;
				if (!((ServiceCall)(ref serviceCall)).IsEmpty)
				{
					RunServiceButton obj = runServiceButtonStart;
					serviceCall = runServiceButtonStart.ServiceCall;
					string ecu = ((ServiceCall)(ref serviceCall)).Ecu;
					serviceCall = runServiceButtonStart.ServiceCall;
					obj.ServiceCall = new ServiceCall(ecu, ((ServiceCall)(ref serviceCall)).Qualifier, (IEnumerable<string>)new string[1] { (result * 60000).ToString() });
					return true;
				}
			}
		}
		return false;
	}

	private void OnStartButtonEnabledChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		((Control)(object)runServiceButtonStart).Enabled = ValidateAndSetDuration();
		textBoxDuration.Enabled = ((RunSharedProcedureButtonBase)runServiceButtonStart).ButtonEnabled;
	}

	private void OnStartStarting(object sender, CancelEventArgs e)
	{
		ReportResult(Resources.Message_ExecutingStart);
	}

	private void OnStartFinished(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			ReportResult(Resources.Message_ATDTestPipeStartSuccessfullyExecuted);
		}
		else
		{
			ReportResult(Resources.Message_ATDTestPipeStartFailedExecution + ((ResultEventArgs)(object)e).Exception.Message);
		}
	}

	private void OnStopStarting(object sender, CancelEventArgs e)
	{
		ReportResult(Resources.Message_ExecutingStop);
	}

	private void OnStopFinished(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			ReportResult(Resources.Message_ATDTestPipeStopSuccessfullyExecuted);
		}
		else
		{
			ReportResult(Resources.Message_ATDTestPipeStopFailedExecution + ((ResultEventArgs)(object)e).Exception.Message);
		}
	}

	private void ClearResults()
	{
		if (textboxResults != null)
		{
			textboxResults.Text = string.Empty;
		}
	}

	private void ReportResult(string text)
	{
		if (textboxResults != null)
		{
			StringBuilder stringBuilder = new StringBuilder(textboxResults.Text);
			stringBuilder.Append(text);
			stringBuilder.Append("\r\n");
			textboxResults.Text = stringBuilder.ToString();
			textboxResults.SelectionStart = textboxResults.TextLength;
			textboxResults.SelectionLength = 0;
			textboxResults.ScrollToCaret();
		}
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableDuration = new TableLayoutPanel();
		tableMainLayout = new TableLayoutPanel();
		textBoxDuration = new TextBox();
		labelDuration = new System.Windows.Forms.Label();
		labelMinutes = new System.Windows.Forms.Label();
		textboxResults = new TextBox();
		buttonClose = new Button();
		runServiceButtonStart = new RunServiceButton();
		runServiceButtonStop = new RunServiceButton();
		((Control)(object)tableMainLayout).SuspendLayout();
		((Control)(object)tableDuration).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableMainLayout, "tableMainLayout");
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)tableDuration, 1, 0);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(textboxResults, 0, 2);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(buttonClose, 1, 3);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)runServiceButtonStart, 0, 0);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)runServiceButtonStop, 0, 1);
		((Control)(object)tableMainLayout).Name = "tableMainLayout";
		componentResourceManager.ApplyResources(tableDuration, "tableDuration");
		((TableLayoutPanel)(object)tableDuration).Controls.Add(textBoxDuration, 1, 1);
		((TableLayoutPanel)(object)tableDuration).Controls.Add(labelDuration, 0, 1);
		((TableLayoutPanel)(object)tableDuration).Controls.Add(labelMinutes, 2, 1);
		((Control)(object)tableDuration).Name = "tableDuration";
		componentResourceManager.ApplyResources(textBoxDuration, "textBoxDuration");
		textBoxDuration.Name = "textBoxDuration";
		componentResourceManager.ApplyResources(labelDuration, "labelDuration");
		labelDuration.Name = "labelDuration";
		labelDuration.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelMinutes, "labelMinutes");
		labelMinutes.Name = "labelMinutes";
		labelMinutes.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)textboxResults, 2);
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(runServiceButtonStart, "runServiceButtonStart");
		((Control)(object)runServiceButtonStart).Name = "runServiceButtonStart";
		runServiceButtonStart.ServiceCall = new ServiceCall("MCM", "RT_SR017_ATD_Test_Sensor_Package_Start", (IEnumerable<string>)new string[1] { "20000" });
		componentResourceManager.ApplyResources(runServiceButtonStop, "runServiceButtonStop");
		((Control)(object)runServiceButtonStop).Name = "runServiceButtonStop";
		runServiceButtonStop.ServiceCall = new ServiceCall("MCM", "RT_SR017_ATD_Test_Sensor_Package_Stop");
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_ATDTestPipe");
		((Control)this).Controls.Add((Control)(object)tableMainLayout);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableMainLayout).ResumeLayout(performLayout: false);
		((Control)(object)tableMainLayout).PerformLayout();
		((Control)(object)tableDuration).ResumeLayout(performLayout: false);
		((Control)(object)tableDuration).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
