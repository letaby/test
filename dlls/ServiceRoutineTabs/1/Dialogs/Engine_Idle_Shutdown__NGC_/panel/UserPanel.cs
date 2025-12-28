// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Help;
using DetroitDiesel.Security;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__NGC_.panel;

public class UserPanel : CustomPanel
{
  private readonly string CpcName = "CPC302T";
  private readonly string EngineIdleShutdownParameterQualifier = "ess_p_EngConf_TesterPresentShtdDelay_u8";
  private Channel cpc = (Channel) null;
  private Parameter engineIdleShutdownParameter = (Parameter) null;
  private TableLayoutPanel tableMainLayout;
  private ScalingLabel labelEngineIdleShutdownParameterStatus;
  private Button preventButton;
  private Button allowButton;
  private Button buttonClose;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelPreventEngineIdleShutdownInstructions;
  private TableLayoutPanel tableLayoutPanelCannotCloseMessage;
  private System.Windows.Forms.Label labelPanelStatus;
  private PictureBox pictureBoxStatus;
  private System.Windows.Forms.Label label1;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.allowButton.Click += new EventHandler(this.OnAllowButtonClick);
    this.preventButton.Click += new EventHandler(this.OnPreventButtonClick);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!e.Cancel)
      e.Cancel = !this.CanClose;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.allowButton.Click -= new EventHandler(this.OnAllowButtonClick);
    this.preventButton.Click -= new EventHandler(this.OnPreventButtonClick);
    this.SetCpc((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetCpc(this.GetChannel(this.CpcName, (CustomPanel.ChannelLookupOptions) 7));
  }

  private void SetCpc(Channel cpc)
  {
    if (this.cpc == cpc)
      return;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.cpc.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
      this.cpc.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
      if (this.engineIdleShutdownParameter != null)
        this.engineIdleShutdownParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.engineIdleShutdownParameter_ParameterUpdateEvent);
      this.engineIdleShutdownParameter = (Parameter) null;
    }
    this.cpc = cpc;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.cpc.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
      this.cpc.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
      this.engineIdleShutdownParameter = this.cpc.Parameters[this.EngineIdleShutdownParameterQualifier];
      if (this.engineIdleShutdownParameter != null)
      {
        this.engineIdleShutdownParameter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.engineIdleShutdownParameter_ParameterUpdateEvent);
        this.ReadEngineIdleShutdownParameter();
      }
    }
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.allowButton.Enabled = this.CanWriteEngineIdleParameter && this.EngineIdleShutdownParameterValue != UserPanel.IdleShutdownStatuses.disabled;
    this.preventButton.Enabled = this.CanWriteEngineIdleParameter && this.EngineIdleShutdownParameterValue != UserPanel.IdleShutdownStatuses.enabled;
    this.buttonClose.Enabled = this.CanClose;
  }

  private void UpdateStatus(string status, bool showIcon)
  {
    this.labelPanelStatus.Text = status;
    if (showIcon)
      this.pictureBoxStatus.Show();
    else
      this.pictureBoxStatus.Hide();
    this.labelPanelStatus.Update();
  }

  private void UpdateEngineIdleShutdownParameterStatus(string engineIdleShutdownStatus)
  {
    ((Control) this.labelEngineIdleShutdownParameterStatus).Text = engineIdleShutdownStatus;
    ((Control) this.labelEngineIdleShutdownParameterStatus).Update();
  }

  private void UpdateEngineIdleShutdownParameterStatus(UserPanel.IdleShutdownStatuses status)
  {
    switch (status)
    {
      case UserPanel.IdleShutdownStatuses.disabled:
        this.UpdateEngineIdleShutdownParameterStatus(Resources.Message_StatusNotActive);
        break;
      case UserPanel.IdleShutdownStatuses.enabled:
        this.UpdateEngineIdleShutdownParameterStatus(Resources.Message_StatusActive);
        break;
      case UserPanel.IdleShutdownStatuses.other:
        this.UpdateEngineIdleShutdownParameterStatus(Resources.Message_StatusOther);
        break;
    }
  }

  private bool Unlock(Channel channel)
  {
    bool flag1 = true;
    if (PasswordManager.HasPasswords(channel))
    {
      PasswordManager passwordManager = PasswordManager.Create(channel);
      if (passwordManager != null && passwordManager.Valid)
      {
        try
        {
          this.AddStatusMessage(Resources.Message_AcquiringDeviceLockStatus);
          bool[] flagArray = passwordManager.AcquireRelevantListStatus((ProgressBar) null);
          bool flag2 = false;
          foreach (bool flag3 in flagArray)
          {
            if (flag3)
            {
              flag2 = true;
              break;
            }
          }
          if (flag2)
          {
            this.AddStatusMessage(Resources.Message_DeviceIsLocked);
            if (((Form) new PasswordEntryDialog(channel, flagArray, passwordManager)).ShowDialog() != DialogResult.OK)
            {
              flag1 = false;
              this.AddStatusMessage(Resources.Message_DeviceWasNotUnlockedByUser);
              this.UpdateStatus(Resources.Message_DeviceWasNotUnlockedByUser, true);
            }
            else
              this.AddStatusMessage(Resources.Message_DeviceWasUnlocked);
          }
          else
            this.AddStatusMessage(Resources.Message_DeviceIsUnlocked);
        }
        catch (CaesarException ex)
        {
          flag1 = false;
          this.AddStatusMessage(Resources.Message_ErrorWhileUnlockingDevice);
          this.UpdateStatus(Resources.Message_DeviceWasNotUnlockedByUser, true);
        }
      }
    }
    return flag1;
  }

  private bool HaveReadEngineIdleShutdownParameter
  {
    get
    {
      return this.engineIdleShutdownParameter != null && this.engineIdleShutdownParameter.HasBeenReadFromEcu;
    }
  }

  private UserPanel.IdleShutdownStatuses EngineIdleShutdownParameterValue
  {
    get
    {
      UserPanel.IdleShutdownStatuses shutdownParameterValue = UserPanel.IdleShutdownStatuses.other;
      if (this.HaveReadEngineIdleShutdownParameter && this.engineIdleShutdownParameter.Value != null)
      {
        if (this.engineIdleShutdownParameter.Value == (object) this.engineIdleShutdownParameter.Choices.GetItemFromRawValue((object) 0))
          shutdownParameterValue = UserPanel.IdleShutdownStatuses.disabled;
        else if (this.engineIdleShutdownParameter.Value == (object) this.engineIdleShutdownParameter.Choices.GetItemFromRawValue((object) 1))
          shutdownParameterValue = UserPanel.IdleShutdownStatuses.enabled;
      }
      return shutdownParameterValue;
    }
  }

  private bool CanWriteEngineIdleParameter
  {
    get => this.Online && this.engineIdleShutdownParameter != null;
  }

  private bool Online
  {
    get => this.cpc != null && this.cpc.CommunicationsState == CommunicationsState.Online;
  }

  private bool CanClose
  {
    get
    {
      return this.cpc == null || !this.cpc.Online || this.cpc.CommunicationsState != CommunicationsState.WriteParameters && this.EngineIdleShutdownParameterValue != UserPanel.IdleShutdownStatuses.disabled;
    }
  }

  private void ReadEngineIdleShutdownParameter()
  {
    if (this.cpc == null)
      return;
    this.UpdateStatus(Resources.Message_ReadingParameter, false);
    this.cpc.Parameters.ReadGroup(this.engineIdleShutdownParameter.GroupQualifier, false, false);
  }

  private void WriteIdleStatusParameter(UserPanel.IdleShutdownStatuses status)
  {
    if (!this.CanWriteEngineIdleParameter || !this.Unlock(this.cpc))
      return;
    this.engineIdleShutdownParameter.Value = (object) this.engineIdleShutdownParameter.Choices[(int) status];
    this.UpdateStatus(Resources.Message_WritingParameter, false);
    this.cpc.Parameters.Write(false);
  }

  private void OnPreventButtonClick(object sender, EventArgs e)
  {
    this.WriteIdleStatusParameter(UserPanel.IdleShutdownStatuses.enabled);
  }

  private void OnAllowButtonClick(object sender, EventArgs e)
  {
    this.WriteIdleStatusParameter(UserPanel.IdleShutdownStatuses.disabled);
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateEngineIdleShutdownParameterStatus(this.EngineIdleShutdownParameterValue);
    if (!this.cpc.Online)
      this.UpdateStatus(Resources.Message_StatusOffline, true);
    else if (this.cpc.CommunicationsState == CommunicationsState.ReadParameters)
      this.UpdateStatus(Resources.Message_ReadingParameter, false);
    else if (this.cpc.CommunicationsState == CommunicationsState.WriteParameters)
      this.UpdateStatus(Resources.Message_WritingParameter, false);
    else if (this.cpc.CommunicationsState == CommunicationsState.Online)
    {
      if (!this.HaveReadEngineIdleShutdownParameter)
        this.ReadEngineIdleShutdownParameter();
      else if (this.EngineIdleShutdownParameterValue == UserPanel.IdleShutdownStatuses.disabled)
        this.UpdateStatus(Resources.Message_CannotClose, true);
      else
        this.UpdateStatus(string.Empty, false);
    }
    this.UpdateUserInterface();
  }

  private void engineIdleShutdownParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateEngineIdleShutdownParameterStatus(this.EngineIdleShutdownParameterValue);
    this.UpdateUserInterface();
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
      this.UpdateEngineIdleShutdownParameterStatus(this.EngineIdleShutdownParameterValue);
    else
      this.UpdateStatus(Resources.Message_ErrorReadingParameter, true);
  }

  private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
      this.UpdateEngineIdleShutdownParameterStatus(this.EngineIdleShutdownParameterValue);
    else
      this.UpdateStatus(Resources.Message_ErrorWritingParameter, true);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelCannotCloseMessage = new TableLayoutPanel();
    this.labelPanelStatus = new System.Windows.Forms.Label();
    this.pictureBoxStatus = new PictureBox();
    this.tableMainLayout = new TableLayoutPanel();
    this.labelPreventEngineIdleShutdownInstructions = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonClose = new Button();
    this.label1 = new System.Windows.Forms.Label();
    this.labelEngineIdleShutdownParameterStatus = new ScalingLabel();
    this.preventButton = new Button();
    this.allowButton = new Button();
    ((Control) this.tableLayoutPanelCannotCloseMessage).SuspendLayout();
    ((ISupportInitialize) this.pictureBoxStatus).BeginInit();
    ((Control) this.tableMainLayout).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((Control) this.tableLayoutPanelCannotCloseMessage).BackColor = SystemColors.Info;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelCannotCloseMessage, "tableLayoutPanelCannotCloseMessage");
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.tableLayoutPanelCannotCloseMessage, 3);
    ((TableLayoutPanel) this.tableLayoutPanelCannotCloseMessage).Controls.Add((Control) this.labelPanelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelCannotCloseMessage).Controls.Add((Control) this.pictureBoxStatus, 0, 0);
    ((Control) this.tableLayoutPanelCannotCloseMessage).Name = "tableLayoutPanelCannotCloseMessage";
    this.labelPanelStatus.BackColor = SystemColors.Info;
    componentResourceManager.ApplyResources((object) this.labelPanelStatus, "labelPanelStatus");
    this.labelPanelStatus.Name = "labelPanelStatus";
    this.labelPanelStatus.UseCompatibleTextRendering = true;
    this.pictureBoxStatus.BackColor = SystemColors.Info;
    componentResourceManager.ApplyResources((object) this.pictureBoxStatus, "pictureBoxStatus");
    this.pictureBoxStatus.Name = "pictureBoxStatus";
    this.pictureBoxStatus.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableMainLayout, "tableMainLayout");
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.labelPreventEngineIdleShutdownInstructions, 0, 1);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.buttonClose, 2, 4);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.labelEngineIdleShutdownParameterStatus, 0, 2);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.preventButton, 1, 4);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.allowButton, 0, 4);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.tableLayoutPanelCannotCloseMessage, 0, 3);
    ((Control) this.tableMainLayout).Name = "tableMainLayout";
    this.labelPreventEngineIdleShutdownInstructions.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.labelPreventEngineIdleShutdownInstructions, 3);
    componentResourceManager.ApplyResources((object) this.labelPreventEngineIdleShutdownInstructions, "labelPreventEngineIdleShutdownInstructions");
    ((Control) this.labelPreventEngineIdleShutdownInstructions).Name = "labelPreventEngineIdleShutdownInstructions";
    this.labelPreventEngineIdleShutdownInstructions.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.label1.BackColor = Color.White;
    this.label1.BorderStyle = BorderStyle.Fixed3D;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.label1, 3);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.ForeColor = Color.Black;
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    this.labelEngineIdleShutdownParameterStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.labelEngineIdleShutdownParameterStatus, 3);
    componentResourceManager.ApplyResources((object) this.labelEngineIdleShutdownParameterStatus, "labelEngineIdleShutdownParameterStatus");
    this.labelEngineIdleShutdownParameterStatus.FontGroup = "";
    this.labelEngineIdleShutdownParameterStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.labelEngineIdleShutdownParameterStatus).Name = "labelEngineIdleShutdownParameterStatus";
    componentResourceManager.ApplyResources((object) this.preventButton, "preventButton");
    this.preventButton.Name = "preventButton";
    this.preventButton.UseCompatibleTextRendering = true;
    this.preventButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.allowButton, "allowButton");
    this.allowButton.Name = "allowButton";
    this.allowButton.UseCompatibleTextRendering = true;
    this.allowButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_EngineIdleShutdown");
    ((Control) this).Controls.Add((Control) this.tableMainLayout);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelCannotCloseMessage).ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxStatus).EndInit();
    ((Control) this.tableMainLayout).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum IdleShutdownStatuses
  {
    disabled,
    enabled,
    other,
  }
}
