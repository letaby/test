using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration.panel;

public class UserPanel : CustomPanel
{
	private const string RecalibrationServiceQualifier = "RT_SR065_Forced_Auto_Cal_EGR_Delta_P_Sensor_Start_status";

	private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private Channel mcm = null;

	private Service recalibrationService = null;

	private Instrument engineSpeed = null;

	private Button buttonBegin;

	private Label labelMCMStatus;

	private Label labelEngineStatus;

	private Label label2;

	private TableLayoutPanel tableLayoutPanel4;

	private Button buttonClose;

	private Checkmark McmCheck;

	private Checkmark EngineSpeedCheck;

	private Checkmark beginCheck;

	private TextBox textboxResults;

	private bool Working => recalibrationService != null;

	private bool Online => mcm != null && mcm.CommunicationsState == CommunicationsState.Online;

	private bool CanBegin => !Working && Online && McmCheck.Checked && EngineSpeedCheck.Checked;

	public UserPanel()
	{
		InitializeComponent();
		buttonBegin.Click += OnClickBegin;
	}

	protected override void OnLoad(EventArgs e)
	{
		ClearResults();
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			SetMCM(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM"));
		UpdateUserInterface();
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm == mcm)
		{
			return;
		}
		ResetData();
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
		}
		if (engineSpeed != null)
		{
			engineSpeed.InstrumentUpdateEvent -= OnEngineSpeedUpdate;
			engineSpeed = null;
		}
		this.mcm = mcm;
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			engineSpeed = this.mcm.Instruments["DT_AS010_Engine_Speed"];
			if (engineSpeed != null)
			{
				engineSpeed.InstrumentUpdateEvent += OnEngineSpeedUpdate;
			}
		}
	}

	private void UpdateMCMStatus()
	{
		bool flag = false;
		string text = Resources.Message_MCMIsNotConnected;
		if (mcm != null)
		{
			if (mcm.CommunicationsState == CommunicationsState.Online)
			{
				text = Resources.Message_MCMIsConnected;
				flag = true;
			}
			else
			{
				text = Resources.Message_MCMIsBusy;
			}
		}
		((Control)(object)labelMCMStatus).Text = text;
		McmCheck.Checked = flag;
	}

	private void UpdateEngineStatus()
	{
		bool flag = false;
		string text = Resources.Message_EngineSpeedCannotBeDetected;
		if (engineSpeed != null && engineSpeed.InstrumentValues.Current != null)
		{
			double num = Convert.ToDouble(engineSpeed.InstrumentValues.Current.Value);
			if (!double.IsNaN(num))
			{
				if (num == 0.0)
				{
					text = Resources.Message_EngineIsNotRunning;
					flag = true;
				}
				else
				{
					text = Resources.Message_EngineIsRunning;
				}
			}
		}
		((Control)(object)labelEngineStatus).Text = text;
		EngineSpeedCheck.Checked = flag;
	}

	private void UpdateButtonStatus()
	{
		bool canBegin = CanBegin;
		buttonBegin.Enabled = canBegin;
		beginCheck.Checked = canBegin;
	}

	private void UpdateUserInterface()
	{
		UpdateMCMStatus();
		UpdateEngineStatus();
		UpdateButtonStatus();
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnClickBegin(object sender, EventArgs e)
	{
		if (CanBegin && MessageBox.Show((IWin32Window)this, Resources.Message_PerformEGRDPSensorRecalibration, ApplicationInformation.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			ClearResults();
			recalibrationService = mcm.Services["RT_SR065_Forced_Auto_Cal_EGR_Delta_P_Sensor_Start_status"];
			if (recalibrationService != null)
			{
				UpdateUserInterface();
				recalibrationService.ServiceCompleteEvent += OnServiceComplete;
				recalibrationService.Execute(synchronous: false);
			}
			else
			{
				ReportResult(Resources.Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated);
			}
		}
	}

	private void ResetData()
	{
		ClearResults();
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		recalibrationService.ServiceCompleteEvent -= OnServiceComplete;
		if (e.Succeeded)
		{
			ServiceOutputValue serviceOutputValue = recalibrationService.OutputValues[0];
			ReportResult(string.Format(Resources.MessageFormat_RecalibrationExecuted0, serviceOutputValue.Value.ToString()));
		}
		else
		{
			ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredDuringRecalibration0, e.Exception.Message));
		}
		recalibrationService = null;
		UpdateUserInterface();
	}

	private void ClearResults()
	{
		if (textboxResults != null)
		{
			textboxResults.Text = "";
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel4 = new TableLayoutPanel();
		labelEngineStatus = new Label();
		textboxResults = new TextBox();
		labelMCMStatus = new Label();
		buttonBegin = new Button();
		label2 = new Label();
		buttonClose = new Button();
		McmCheck = new Checkmark();
		EngineSpeedCheck = new Checkmark();
		beginCheck = new Checkmark();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)labelEngineStatus, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(textboxResults, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)labelMCMStatus, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(buttonBegin, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)label2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(buttonClose, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)McmCheck, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)EngineSpeedCheck, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)beginCheck, 0, 3);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		labelEngineStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelEngineStatus, "labelEngineStatus");
		((Control)(object)labelEngineStatus).Name = "labelEngineStatus";
		labelEngineStatus.Orientation = (TextOrientation)1;
		labelEngineStatus.ShowBorder = false;
		labelEngineStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		labelMCMStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelMCMStatus, "labelMCMStatus");
		((Control)(object)labelMCMStatus).Name = "labelMCMStatus";
		labelMCMStatus.Orientation = (TextOrientation)1;
		labelMCMStatus.ShowBorder = false;
		labelMCMStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(buttonBegin, "buttonBegin");
		buttonBegin.Name = "buttonBegin";
		buttonBegin.UseCompatibleTextRendering = true;
		buttonBegin.UseVisualStyleBackColor = true;
		label2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label2, "label2");
		((TableLayoutPanel)(object)tableLayoutPanel4).SetColumnSpan((Control)(object)label2, 2);
		((Control)(object)label2).Name = "label2";
		label2.Orientation = (TextOrientation)1;
		label2.UseSystemColors = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(McmCheck, "McmCheck");
		((Control)(object)McmCheck).Name = "McmCheck";
		componentResourceManager.ApplyResources(EngineSpeedCheck, "EngineSpeedCheck");
		((Control)(object)EngineSpeedCheck).Name = "EngineSpeedCheck";
		componentResourceManager.ApplyResources(beginCheck, "beginCheck");
		((Control)(object)beginCheck).Name = "beginCheck";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_EGRDPSensorRecalibration");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel4);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
