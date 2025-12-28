using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Start_Reset__EMG_.panel;

public class UserPanel : CustomPanel
{
	private static string OmcsStartServiceQualifier = "IOC_IOC_HV_OMCS(1)";

	private static List<int> ValidStartValues = new List<int> { 0, 1 };

	private static string OmcsStopServiceQualifier = "IOC_IOC_HV_OMCS_Return_Control";

	private Channel eCpcChannel;

	private bool routineRunning = false;

	private bool routineHasRun = false;

	private Timer waitTimer;

	private SelectablePanel selectablePanel1;

	private TableLayoutPanel tableLayoutPanelMain;

	private System.Windows.Forms.Label labelStatusLabelStatusText;

	private System.Windows.Forms.Label label5;

	private Checkmark checkmarkStartReset;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label4;

	private Button buttonStart;

	private Button buttonClose;

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public UserPanel()
	{
		InitializeComponent();
		waitTimer = new Timer();
		waitTimer.Interval = 6000;
		waitTimer.Tick += waitTimer_Tick;
		UpdateUI();
	}

	public override void OnChannelsChanged()
	{
		SetECPC01TChannel("ECPC01T");
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = routineRunning;
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			waitTimer.Tick -= waitTimer_Tick;
			waitTimer.Dispose();
		}
	}

	private void SetECPC01TChannel(string ecuName)
	{
		Channel channel = ((CustomPanel)this).GetChannel(ecuName, (ChannelLookupOptions)3);
		if (eCpcChannel != channel)
		{
			eCpcChannel = channel;
			routineRunning = (routineHasRun = false);
		}
		UpdateUI();
	}

	private void UpdateUI()
	{
		buttonStart.Enabled = eCpcChannel != null && eCpcChannel.Online && !routineRunning;
		buttonClose.Enabled = !routineRunning;
		if (!routineHasRun && !routineRunning)
		{
			if (eCpcChannel != null && eCpcChannel.Online)
			{
				labelStatusLabelStatusText.Text = Resources.Message_ReadyToRun;
				checkmarkStartReset.CheckState = CheckState.Checked;
			}
			else
			{
				labelStatusLabelStatusText.Text = Resources.Message_NotReady;
				checkmarkStartReset.CheckState = CheckState.Unchecked;
			}
		}
	}

	private void LogMessage(string text, bool updateStatusText, CheckState checkState)
	{
		((CustomPanel)this).AddStatusMessage(text);
		((CustomPanel)this).LabelLog("EmgStartReset", text);
		if (updateStatusText)
		{
			labelStatusLabelStatusText.Text = text;
			checkmarkStartReset.CheckState = checkState;
		}
	}

	private bool RunService(Channel channel, string serviceQualifier, ServiceCompleteEventHandler serviceCompleteEventHandler)
	{
		if (channel != null && channel.Online)
		{
			Service service = channel.Services[serviceQualifier];
			if (service != null)
			{
				if (serviceCompleteEventHandler != null)
				{
					service.ServiceCompleteEvent += serviceCompleteEventHandler;
				}
				service.InputValues.ParseValues(serviceQualifier);
				service.Execute(synchronous: false);
				return true;
			}
		}
		return false;
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		routineHasRun = true;
		routineRunning = RunService(eCpcChannel, OmcsStartServiceQualifier, Start_ServiceCompleteEvent);
		LogMessage(routineRunning ? Resources.Message_Running : Resources.Message_CouldNotStart, updateStatusText: true, routineRunning ? CheckState.Indeterminate : CheckState.Unchecked);
		UpdateUI();
	}

	private int ConvertChoiceValueObjectToRawValue(ServiceOutputValue value)
	{
		Choice choice = value.Value as Choice;
		if (choice != null)
		{
			try
			{
				return Convert.ToInt32(choice.RawValue);
			}
			catch (InvalidCastException ex)
			{
				LogMessage(ex.Message, updateStatusText: false, CheckState.Checked);
			}
		}
		return -1;
	}

	private void Start_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		int item = -1;
		if (service != null)
		{
			item = ConvertChoiceValueObjectToRawValue(service.OutputValues[0]);
			service.ServiceCompleteEvent -= Start_ServiceCompleteEvent;
		}
		if (e.Succeeded && ValidStartValues.Contains(item))
		{
			waitTimer.Start();
		}
		else
		{
			routineRunning = false;
			string text = Resources.Message_Unknown;
			if (service.OutputValues.Count > 0 && service.OutputValues[0].Value != null)
			{
				text = service.OutputValues[0].Value.ToString();
			}
			text = ((e.Exception != null) ? e.Exception.Message : text);
			LogMessage(string.Format(Resources.MessageFormat_Failed0, text), updateStatusText: true, CheckState.Unchecked);
		}
		UpdateUI();
	}

	private void waitTimer_Tick(object sender, EventArgs e)
	{
		waitTimer.Stop();
		routineRunning = RunService(eCpcChannel, OmcsStopServiceQualifier, Stop_ServiceCompleteEvent);
		LogMessage(routineRunning ? Resources.Message_ReturningControl : Resources.Message_CouldNotReturnControl, updateStatusText: true, routineRunning ? CheckState.Checked : CheckState.Unchecked);
		UpdateUI();
	}

	private void Stop_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= Stop_ServiceCompleteEvent;
		}
		routineRunning = false;
		if (e.Succeeded)
		{
			LogMessage(Resources.Message_ResetComplete, updateStatusText: true, CheckState.Checked);
		}
		else
		{
			string text = ((e.Exception != null) ? e.Exception.Message : Resources.Message_Unknown1);
			LogMessage(string.Format(Resources.MessageFormat_CouldNotReturnControl0, e.Exception.Message), updateStatusText: true, CheckState.Unchecked);
		}
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		selectablePanel1 = new SelectablePanel();
		tableLayoutPanelMain = new TableLayoutPanel();
		labelStatusLabelStatusText = new System.Windows.Forms.Label();
		label5 = new System.Windows.Forms.Label();
		checkmarkStartReset = new Checkmark();
		label3 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		buttonStart = new Button();
		buttonClose = new Button();
		((Control)(object)selectablePanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)(object)selectablePanel1).Controls.Add((Control)(object)tableLayoutPanelMain);
		componentResourceManager.ApplyResources(selectablePanel1, "selectablePanel1");
		((Control)(object)selectablePanel1).Name = "selectablePanel1";
		((Panel)(object)selectablePanel1).TabStop = true;
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelStatusLabelStatusText, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label5, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)checkmarkStartReset, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label4, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonStart, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 4, 2);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(labelStatusLabelStatusText, "labelStatusLabelStatusText");
		labelStatusLabelStatusText.Name = "labelStatusLabelStatusText";
		labelStatusLabelStatusText.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkStartReset, "checkmarkStartReset");
		((Control)(object)checkmarkStartReset).Name = "checkmarkStartReset";
		componentResourceManager.ApplyResources(label3, "label3");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)label3, 5);
		label3.Name = "label3";
		componentResourceManager.ApplyResources(label4, "label4");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)label4, 5);
		label4.Name = "label4";
		label4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_OMCS_Reset");
		((Control)this).Controls.Add((Control)(object)selectablePanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)selectablePanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
