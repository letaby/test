using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Maximum_Sensor_Value_Reset.panel;

public class UserPanel : CustomPanel
{
	private const string ResetService = "RT_SR014_SET_EOL_Default_Values_Start";

	private Channel mcm = null;

	private Service resetService = null;

	private Button buttonBegin;

	private Label labelMCMStatus;

	private TableLayoutPanel tableLayoutPanel4;

	private Button buttonClose;

	private Checkmark mcmConnectedCheck;

	private Checkmark canBeginCheck;

	private TextBox textboxResults;

	private bool Working => resetService != null;

	private bool Online => mcm != null && mcm.CommunicationsState == CommunicationsState.Online;

	private bool CanBegin => !Working && Online && mcmConnectedCheck.Checked;

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
		if (this.mcm != mcm)
		{
			ResetData();
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
	}

	private void UpdateMCMStatus()
	{
		bool flag = false;
		string text = Resources.Message_PleaseConnectTheMCM;
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
		mcmConnectedCheck.Checked = flag;
	}

	private void UpdateButtonStatus()
	{
		bool canBegin = CanBegin;
		buttonBegin.Enabled = canBegin;
		canBeginCheck.Checked = canBegin;
	}

	private void UpdateUserInterface()
	{
		UpdateMCMStatus();
		UpdateButtonStatus();
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnClickBegin(object sender, EventArgs e)
	{
		if (CanBegin && MessageBox.Show((IWin32Window)this, Resources.Message_PerformSensorValueReset, ApplicationInformation.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			ClearResults();
			resetService = mcm.Services["RT_SR014_SET_EOL_Default_Values_Start"];
			if (resetService != null)
			{
				UpdateUserInterface();
				ReportResult(Resources.Message_Executing + resetService.Name + "...");
				resetService.InputValues[0].Value = resetService.InputValues[0].Choices.GetItemFromRawValue(6);
				resetService.ServiceCompleteEvent += OnServiceComplete;
				resetService.Execute(synchronous: false);
			}
			else
			{
				ReportResult(Resources.Message_UnableToAcquireTheResetServiceATDMaximumSensorValuesCannotBeReset);
			}
		}
	}

	private void ResetData()
	{
		ClearResults();
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		resetService.ServiceCompleteEvent -= OnServiceComplete;
		if (e.Succeeded)
		{
			if (resetService.OutputValues.Count > 0)
			{
				ServiceOutputValue serviceOutputValue = resetService.OutputValues[0];
				ReportResult(string.Format(Resources.MessageFormat_ResetExecuted0, serviceOutputValue.Value.ToString()));
			}
			else
			{
				ReportResult(Resources.Message_ResetExecutedSuccessfully);
			}
		}
		else
		{
			ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredDuringTheReset0, e.Exception.Message));
		}
		resetService = null;
		UpdateUserInterface();
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel4 = new TableLayoutPanel();
		textboxResults = new TextBox();
		labelMCMStatus = new Label();
		buttonBegin = new Button();
		buttonClose = new Button();
		mcmConnectedCheck = new Checkmark();
		canBeginCheck = new Checkmark();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(textboxResults, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)labelMCMStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(buttonBegin, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(buttonClose, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)mcmConnectedCheck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)canBeginCheck, 0, 1);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
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
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(mcmConnectedCheck, "mcmConnectedCheck");
		((Control)(object)mcmConnectedCheck).Name = "mcmConnectedCheck";
		mcmConnectedCheck.SizeMode = PictureBoxSizeMode.AutoSize;
		componentResourceManager.ApplyResources(canBeginCheck, "canBeginCheck");
		((Control)(object)canBeginCheck).Name = "canBeginCheck";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_ATDMaximumSensorValueReset");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel4);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
