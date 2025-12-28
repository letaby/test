using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Security;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__NGC_.panel;

public class UserPanel : CustomPanel
{
	private enum IdleShutdownStatuses
	{
		disabled,
		enabled,
		other
	}

	private readonly string CpcName = "CPC302T";

	private readonly string EngineIdleShutdownParameterQualifier = "ess_p_EngConf_TesterPresentShtdDelay_u8";

	private Channel cpc = null;

	private Parameter engineIdleShutdownParameter = null;

	private TableLayoutPanel tableMainLayout;

	private ScalingLabel labelEngineIdleShutdownParameterStatus;

	private Button preventButton;

	private Button allowButton;

	private Button buttonClose;

	private Label labelPreventEngineIdleShutdownInstructions;

	private TableLayoutPanel tableLayoutPanelCannotCloseMessage;

	private System.Windows.Forms.Label labelPanelStatus;

	private PictureBox pictureBoxStatus;

	private System.Windows.Forms.Label label1;

	private bool HaveReadEngineIdleShutdownParameter => engineIdleShutdownParameter != null && engineIdleShutdownParameter.HasBeenReadFromEcu;

	private IdleShutdownStatuses EngineIdleShutdownParameterValue
	{
		get
		{
			IdleShutdownStatuses result = IdleShutdownStatuses.other;
			if (HaveReadEngineIdleShutdownParameter && engineIdleShutdownParameter.Value != null)
			{
				if (engineIdleShutdownParameter.Value == engineIdleShutdownParameter.Choices.GetItemFromRawValue(0))
				{
					result = IdleShutdownStatuses.disabled;
				}
				else if (engineIdleShutdownParameter.Value == engineIdleShutdownParameter.Choices.GetItemFromRawValue(1))
				{
					result = IdleShutdownStatuses.enabled;
				}
			}
			return result;
		}
	}

	private bool CanWriteEngineIdleParameter => Online && engineIdleShutdownParameter != null;

	private bool Online => cpc != null && cpc.CommunicationsState == CommunicationsState.Online;

	private bool CanClose => cpc == null || !cpc.Online || (cpc.CommunicationsState != CommunicationsState.WriteParameters && EngineIdleShutdownParameterValue != IdleShutdownStatuses.disabled);

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		allowButton.Click += OnAllowButtonClick;
		preventButton.Click += OnPreventButtonClick;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			e.Cancel = !CanClose;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			allowButton.Click -= OnAllowButtonClick;
			preventButton.Click -= OnPreventButtonClick;
			SetCpc(null);
		}
	}

	public override void OnChannelsChanged()
	{
		Channel channel = ((CustomPanel)this).GetChannel(CpcName, (ChannelLookupOptions)7);
		SetCpc(channel);
	}

	private void SetCpc(Channel cpc)
	{
		if (this.cpc == cpc)
		{
			return;
		}
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			this.cpc.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
			this.cpc.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersWriteCompleteEvent;
			if (engineIdleShutdownParameter != null)
			{
				engineIdleShutdownParameter.ParameterUpdateEvent -= engineIdleShutdownParameter_ParameterUpdateEvent;
			}
			engineIdleShutdownParameter = null;
		}
		this.cpc = cpc;
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			this.cpc.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
			this.cpc.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersWriteCompleteEvent;
			engineIdleShutdownParameter = this.cpc.Parameters[EngineIdleShutdownParameterQualifier];
			if (engineIdleShutdownParameter != null)
			{
				engineIdleShutdownParameter.ParameterUpdateEvent += engineIdleShutdownParameter_ParameterUpdateEvent;
				ReadEngineIdleShutdownParameter();
			}
		}
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		allowButton.Enabled = CanWriteEngineIdleParameter && EngineIdleShutdownParameterValue != IdleShutdownStatuses.disabled;
		preventButton.Enabled = CanWriteEngineIdleParameter && EngineIdleShutdownParameterValue != IdleShutdownStatuses.enabled;
		buttonClose.Enabled = CanClose;
	}

	private void UpdateStatus(string status, bool showIcon)
	{
		labelPanelStatus.Text = status;
		if (showIcon)
		{
			pictureBoxStatus.Show();
		}
		else
		{
			pictureBoxStatus.Hide();
		}
		labelPanelStatus.Update();
	}

	private void UpdateEngineIdleShutdownParameterStatus(string engineIdleShutdownStatus)
	{
		((Control)(object)labelEngineIdleShutdownParameterStatus).Text = engineIdleShutdownStatus;
		((Control)(object)labelEngineIdleShutdownParameterStatus).Update();
	}

	private void UpdateEngineIdleShutdownParameterStatus(IdleShutdownStatuses status)
	{
		switch (status)
		{
		case IdleShutdownStatuses.disabled:
			UpdateEngineIdleShutdownParameterStatus(Resources.Message_StatusNotActive);
			break;
		case IdleShutdownStatuses.enabled:
			UpdateEngineIdleShutdownParameterStatus(Resources.Message_StatusActive);
			break;
		case IdleShutdownStatuses.other:
			UpdateEngineIdleShutdownParameterStatus(Resources.Message_StatusOther);
			break;
		}
	}

	private bool Unlock(Channel channel)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		bool result = true;
		if (PasswordManager.HasPasswords(channel))
		{
			PasswordManager val = PasswordManager.Create(channel);
			if (val != null && val.Valid)
			{
				try
				{
					((CustomPanel)this).AddStatusMessage(Resources.Message_AcquiringDeviceLockStatus);
					bool[] array = val.AcquireRelevantListStatus((ProgressBar)null);
					bool flag = false;
					bool[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i])
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						((CustomPanel)this).AddStatusMessage(Resources.Message_DeviceIsLocked);
						PasswordEntryDialog val2 = new PasswordEntryDialog(channel, array, val);
						if (((Form)(object)val2).ShowDialog() != DialogResult.OK)
						{
							result = false;
							((CustomPanel)this).AddStatusMessage(Resources.Message_DeviceWasNotUnlockedByUser);
							UpdateStatus(Resources.Message_DeviceWasNotUnlockedByUser, showIcon: true);
						}
						else
						{
							((CustomPanel)this).AddStatusMessage(Resources.Message_DeviceWasUnlocked);
						}
					}
					else
					{
						((CustomPanel)this).AddStatusMessage(Resources.Message_DeviceIsUnlocked);
					}
				}
				catch (CaesarException)
				{
					result = false;
					((CustomPanel)this).AddStatusMessage(Resources.Message_ErrorWhileUnlockingDevice);
					UpdateStatus(Resources.Message_DeviceWasNotUnlockedByUser, showIcon: true);
				}
			}
		}
		return result;
	}

	private void ReadEngineIdleShutdownParameter()
	{
		if (cpc != null)
		{
			UpdateStatus(Resources.Message_ReadingParameter, showIcon: false);
			cpc.Parameters.ReadGroup(engineIdleShutdownParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
	}

	private void WriteIdleStatusParameter(IdleShutdownStatuses status)
	{
		if (CanWriteEngineIdleParameter && Unlock(cpc))
		{
			engineIdleShutdownParameter.Value = engineIdleShutdownParameter.Choices[(int)status];
			UpdateStatus(Resources.Message_WritingParameter, showIcon: false);
			cpc.Parameters.Write(synchronous: false);
		}
	}

	private void OnPreventButtonClick(object sender, EventArgs e)
	{
		WriteIdleStatusParameter(IdleShutdownStatuses.enabled);
	}

	private void OnAllowButtonClick(object sender, EventArgs e)
	{
		WriteIdleStatusParameter(IdleShutdownStatuses.disabled);
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateEngineIdleShutdownParameterStatus(EngineIdleShutdownParameterValue);
		if (!cpc.Online)
		{
			UpdateStatus(Resources.Message_StatusOffline, showIcon: true);
		}
		else if (cpc.CommunicationsState == CommunicationsState.ReadParameters)
		{
			UpdateStatus(Resources.Message_ReadingParameter, showIcon: false);
		}
		else if (cpc.CommunicationsState == CommunicationsState.WriteParameters)
		{
			UpdateStatus(Resources.Message_WritingParameter, showIcon: false);
		}
		else if (cpc.CommunicationsState == CommunicationsState.Online)
		{
			if (!HaveReadEngineIdleShutdownParameter)
			{
				ReadEngineIdleShutdownParameter();
			}
			else if (EngineIdleShutdownParameterValue == IdleShutdownStatuses.disabled)
			{
				UpdateStatus(Resources.Message_CannotClose, showIcon: true);
			}
			else
			{
				UpdateStatus(string.Empty, showIcon: false);
			}
		}
		UpdateUserInterface();
	}

	private void engineIdleShutdownParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateEngineIdleShutdownParameterStatus(EngineIdleShutdownParameterValue);
		UpdateUserInterface();
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			UpdateEngineIdleShutdownParameterStatus(EngineIdleShutdownParameterValue);
		}
		else
		{
			UpdateStatus(Resources.Message_ErrorReadingParameter, showIcon: true);
		}
	}

	private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			UpdateEngineIdleShutdownParameterStatus(EngineIdleShutdownParameterValue);
		}
		else
		{
			UpdateStatus(Resources.Message_ErrorWritingParameter, showIcon: true);
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelCannotCloseMessage = new TableLayoutPanel();
		labelPanelStatus = new System.Windows.Forms.Label();
		pictureBoxStatus = new PictureBox();
		tableMainLayout = new TableLayoutPanel();
		labelPreventEngineIdleShutdownInstructions = new Label();
		buttonClose = new Button();
		label1 = new System.Windows.Forms.Label();
		labelEngineIdleShutdownParameterStatus = new ScalingLabel();
		preventButton = new Button();
		allowButton = new Button();
		((Control)(object)tableLayoutPanelCannotCloseMessage).SuspendLayout();
		((ISupportInitialize)pictureBoxStatus).BeginInit();
		((Control)(object)tableMainLayout).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)(object)tableLayoutPanelCannotCloseMessage).BackColor = SystemColors.Info;
		componentResourceManager.ApplyResources(tableLayoutPanelCannotCloseMessage, "tableLayoutPanelCannotCloseMessage");
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)(object)tableLayoutPanelCannotCloseMessage, 3);
		((TableLayoutPanel)(object)tableLayoutPanelCannotCloseMessage).Controls.Add(labelPanelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelCannotCloseMessage).Controls.Add(pictureBoxStatus, 0, 0);
		((Control)(object)tableLayoutPanelCannotCloseMessage).Name = "tableLayoutPanelCannotCloseMessage";
		labelPanelStatus.BackColor = SystemColors.Info;
		componentResourceManager.ApplyResources(labelPanelStatus, "labelPanelStatus");
		labelPanelStatus.Name = "labelPanelStatus";
		labelPanelStatus.UseCompatibleTextRendering = true;
		pictureBoxStatus.BackColor = SystemColors.Info;
		componentResourceManager.ApplyResources(pictureBoxStatus, "pictureBoxStatus");
		pictureBoxStatus.Name = "pictureBoxStatus";
		pictureBoxStatus.TabStop = false;
		componentResourceManager.ApplyResources(tableMainLayout, "tableMainLayout");
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)labelPreventEngineIdleShutdownInstructions, 0, 1);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(buttonClose, 2, 4);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)labelEngineIdleShutdownParameterStatus, 0, 2);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(preventButton, 1, 4);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(allowButton, 0, 4);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)tableLayoutPanelCannotCloseMessage, 0, 3);
		((Control)(object)tableMainLayout).Name = "tableMainLayout";
		labelPreventEngineIdleShutdownInstructions.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)(object)labelPreventEngineIdleShutdownInstructions, 3);
		componentResourceManager.ApplyResources(labelPreventEngineIdleShutdownInstructions, "labelPreventEngineIdleShutdownInstructions");
		((Control)(object)labelPreventEngineIdleShutdownInstructions).Name = "labelPreventEngineIdleShutdownInstructions";
		labelPreventEngineIdleShutdownInstructions.Orientation = (TextOrientation)1;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		label1.BackColor = Color.White;
		label1.BorderStyle = BorderStyle.Fixed3D;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)label1, 3);
		componentResourceManager.ApplyResources(label1, "label1");
		label1.ForeColor = Color.Black;
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		labelEngineIdleShutdownParameterStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)(object)labelEngineIdleShutdownParameterStatus, 3);
		componentResourceManager.ApplyResources(labelEngineIdleShutdownParameterStatus, "labelEngineIdleShutdownParameterStatus");
		labelEngineIdleShutdownParameterStatus.FontGroup = "";
		labelEngineIdleShutdownParameterStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)labelEngineIdleShutdownParameterStatus).Name = "labelEngineIdleShutdownParameterStatus";
		componentResourceManager.ApplyResources(preventButton, "preventButton");
		preventButton.Name = "preventButton";
		preventButton.UseCompatibleTextRendering = true;
		preventButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(allowButton, "allowButton");
		allowButton.Name = "allowButton";
		allowButton.UseCompatibleTextRendering = true;
		allowButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_EngineIdleShutdown");
		((Control)this).Controls.Add((Control)(object)tableMainLayout);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelCannotCloseMessage).ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxStatus).EndInit();
		((Control)(object)tableMainLayout).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
