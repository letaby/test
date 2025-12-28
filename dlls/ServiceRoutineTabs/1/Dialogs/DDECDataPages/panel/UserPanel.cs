// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DDECDataPages.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.DataHub;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DDECDataPages.panel;

public class UserPanel : CustomPanel
{
  private const string RequiredUserLabelPrefix = "DDecDataPages";
  private UserPanel.Action action = UserPanel.Action.none;
  private string VINEcuInfo = "CO_VIN";
  private Channel cpc = (Channel) null;
  private string vin = (string) null;
  private Button buttonExtract;
  private ProgressBar progressBar;
  private Checkmark checkmarkDataPagesEnabled;
  private Button buttonResetTrip;
  private Button buttonResetAll;
  private System.Windows.Forms.Label labelDataPageEnabled;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private TextBox textBoxStatus;
  private Button buttonClearPassword;
  private Button buttonSetPassword;
  private Button buttonClose;

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged()
  {
    this.SetCPC(SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Name.StartsWith("CPC"))));
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.SetCPC((Channel) null);
  }

  private void buttonClose_Click(object sender, EventArgs e)
  {
    ((ContainerControl) this).ParentForm.Close();
  }

  private void buttonExtract_Click(object sender, EventArgs e)
  {
    this.LogText(Resources.Message_StartingExtraction);
    this.action = UserPanel.Action.extraction;
    this.progressBar.Value = 0;
    this.progressBar.Visible = true;
    ExtractionManager.GlobalInstance.ExtractionCompleteEvent += new EventHandler<ExtractionCompleteEventArgs>(this.ExtractionManager_ExtractionCompleteEvent);
    ExtractionManager.GlobalInstance.ExtractionProgressEvent += new EventHandler<ExtractionProgressEventArgs>(this.ExtractionManager_ExtractionProgressEvent);
    ExtractionManager.GlobalInstance.DoExtraction(string.Empty);
    this.UpdateUserInterface();
  }

  private void buttonSetPassword_Click(object sender, EventArgs e)
  {
    this.LogText(Resources.Message_SettingPassword);
    this.action = UserPanel.Action.setPassword;
    this.UpdateUserInterface();
    ExtractionManager.GlobalInstance.SetDataPagesPasswordCompleteEvent += new EventHandler<ChangeDataPagePasswordRequestEventArgs>(this.ExtractionManager_SetDataPagesPasswordCompleteEvent);
    ExtractionManager.GlobalInstance.SetDataPagesPassword();
  }

  private void buttonClearPassword_Click(object sender, EventArgs e)
  {
    this.LogText(Resources.Message_ClearingPassword);
    this.action = UserPanel.Action.clearPassword;
    this.UpdateUserInterface();
    ExtractionManager.GlobalInstance.ClearDataPagesPasswordCompleteEvent += new EventHandler<ResultEventArgs>(this.ExtractionManager_ClearDataPagesPasswordCompleteEvent);
    ExtractionManager.GlobalInstance.ClearDataPagesPassword();
  }

  private void buttonResetTrip_Click(object sender, EventArgs e)
  {
    this.action = UserPanel.Action.resetData;
    this.LogText(Resources.Message_ResettingTrip);
    this.UpdateUserInterface();
    this.PerformDataPageReset(false);
  }

  private void buttonResetAll_Click(object sender, EventArgs e)
  {
    this.action = UserPanel.Action.resetData;
    this.LogText(Resources.Message_ResettingAllTrips);
    this.UpdateUserInterface();
    this.PerformDataPageReset(true);
  }

  private void ExtractionManager_ExtractionCompleteEvent(
    object sender,
    ExtractionCompleteEventArgs extractionCompleteEventArgs)
  {
    ExtractionManager.GlobalInstance.ExtractionCompleteEvent -= new EventHandler<ExtractionCompleteEventArgs>(this.ExtractionManager_ExtractionCompleteEvent);
    ExtractionManager.GlobalInstance.ExtractionProgressEvent -= new EventHandler<ExtractionProgressEventArgs>(this.ExtractionManager_ExtractionProgressEvent);
    this.LogText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}.", extractionCompleteEventArgs.Succeeded ? (object) Resources.Message_SucessExtraction : (object) Resources.Message_ExtractionFailed, !string.IsNullOrEmpty(this.vin) ? (object) this.vin : (object) Resources.Message_WithNoVIN));
    this.progressBar.Visible = false;
    this.action = UserPanel.Action.none;
    this.UpdateUserInterface();
  }

  private void ExtractionManager_ExtractionProgressEvent(
    object sender,
    ExtractionProgressEventArgs extractionProgressEventArgs)
  {
    this.progressBar.Value = (int) extractionProgressEventArgs.Percent;
    this.UpdateUserInterface();
  }

  private void ExtractionManager_ClearDataPagesCompleteEvent(object sender, ResultEventArgs e)
  {
    ExtractionManager.GlobalInstance.ClearDataPagesCompleteEvent -= new EventHandler<ResultEventArgs>(this.ExtractionManager_ClearDataPagesCompleteEvent);
    this.LogText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}.", e.Succeeded ? (object) Resources.Message_TripsReset : (object) Resources.Message_ResetFailed, !string.IsNullOrEmpty(this.vin) ? (object) this.vin : (object) Resources.Message_WithNoVIN));
    this.action = UserPanel.Action.none;
    this.UpdateUserInterface();
  }

  private void ExtractionManager_SetDataPagesPasswordCompleteEvent(
    object sender,
    ChangeDataPagePasswordRequestEventArgs e)
  {
    ExtractionManager.GlobalInstance.SetDataPagesPasswordCompleteEvent -= new EventHandler<ChangeDataPagePasswordRequestEventArgs>(this.ExtractionManager_SetDataPagesPasswordCompleteEvent);
    this.LogText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}.", e.Result == ChangePasswordResult.Success ? (object) Resources.Message_PasswordSet : (e.Result == ChangePasswordResult.Cancel ? (object) Resources.Message_SetPasswordCancel : (object) Resources.Message_SetPasswordFail), !string.IsNullOrEmpty(this.vin) ? (object) this.vin : (object) Resources.Message_WithNoVIN));
    if (e.Result == ChangePasswordResult.Success)
    {
      this.LogText(Resources.Message_CPCReset);
      this.action = UserPanel.Action.waitingForCPCOnline;
      this.cpc.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.commitChangesService_ServiceCompleteEvent);
      this.cpc.Services.Execute(this.cpc.Ecu.Properties["CommitToPermanentMemoryService"], false);
    }
    else
      this.action = UserPanel.Action.none;
    this.UpdateUserInterface();
  }

  private void ExtractionManager_ClearDataPagesPasswordCompleteEvent(
    object sender,
    ResultEventArgs e)
  {
    ExtractionManager.GlobalInstance.ClearDataPagesPasswordCompleteEvent -= new EventHandler<ResultEventArgs>(this.ExtractionManager_ClearDataPagesPasswordCompleteEvent);
    this.LogText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}.", e.Succeeded ? (object) Resources.Message_PasswordCleared : (object) Resources.Message_ClearPasswordFail, !string.IsNullOrEmpty(this.vin) ? (object) this.vin : (object) Resources.Message_WithNoVIN));
    if (e.Succeeded)
    {
      this.LogText(Resources.Message_CPCReset);
      this.action = UserPanel.Action.waitingForCPCOnline;
      this.cpc.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.commitChangesService_ServiceCompleteEvent);
      this.cpc.Services.Execute(this.cpc.Ecu.Properties["CommitToPermanentMemoryService"], false);
    }
    else
      this.action = UserPanel.Action.none;
    this.UpdateUserInterface();
  }

  private void commitChangesService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.cpc.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.commitChangesService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.LogText(Resources.Message_ChangesCommited);
    else
      this.LogText(Resources.Message_CommitServiceFailed);
    this.action = UserPanel.Action.none;
    this.UpdateUserInterface();
  }

  private bool CanClose => this.action == UserPanel.Action.none;

  private bool Busy => ExtractionManager.Busy || this.action != UserPanel.Action.none;

  private bool Online
  {
    get
    {
      return this.cpc != null && (this.cpc.CommunicationsState == CommunicationsState.Online || this.cpc.CommunicationsState == CommunicationsState.ByteMessage || this.cpc.CommunicationsState == CommunicationsState.ExecuteService);
    }
  }

  private bool CanPerformAction
  {
    get => ExtractionManager.GlobalInstance.DataPagesEnabled && !this.Busy && this.Online;
  }

  private void GetVin()
  {
    if (!this.Online || this.cpc.EcuInfos[this.VINEcuInfo] == null)
      return;
    string empty = string.Empty;
    this.cpc.EcuInfos[this.VINEcuInfo].Read(true);
    string a = this.cpc.EcuInfos[this.VINEcuInfo].Value;
    if (!string.Equals(a, this.vin, StringComparison.OrdinalIgnoreCase))
    {
      this.vin = a;
      if (!string.IsNullOrEmpty(this.vin))
        this.LogText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Vin, (object) this.vin));
    }
  }

  private void SetCPC(Channel cpc)
  {
    if (this.cpc == cpc)
      return;
    if (this.cpc != null)
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.vin = (string) null;
    this.cpc = cpc;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      if (SapiManager.SupportsDdecDataPages(this.cpc))
        ExtractionManager.GlobalInstance.SetChannel(this.cpc);
      this.GetVin();
    }
    this.UpdateUserInterface();
  }

  private void StopActionInProgress()
  {
    switch (this.action)
    {
      case UserPanel.Action.extraction:
        ExtractionManager.GlobalInstance.ExtractionCompleteEvent -= new EventHandler<ExtractionCompleteEventArgs>(this.ExtractionManager_ExtractionCompleteEvent);
        ExtractionManager.GlobalInstance.ExtractionProgressEvent -= new EventHandler<ExtractionProgressEventArgs>(this.ExtractionManager_ExtractionProgressEvent);
        break;
      case UserPanel.Action.clearPassword:
        ExtractionManager.GlobalInstance.ClearDataPagesPasswordCompleteEvent -= new EventHandler<ResultEventArgs>(this.ExtractionManager_ClearDataPagesPasswordCompleteEvent);
        break;
      case UserPanel.Action.setPassword:
        ExtractionManager.GlobalInstance.SetDataPagesPasswordCompleteEvent -= new EventHandler<ChangeDataPagePasswordRequestEventArgs>(this.ExtractionManager_SetDataPagesPasswordCompleteEvent);
        break;
      case UserPanel.Action.waitingForCPCOnline:
        this.cpc.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.commitChangesService_ServiceCompleteEvent);
        break;
      default:
        ExtractionManager.GlobalInstance.ClearDataPagesCompleteEvent -= new EventHandler<ResultEventArgs>(this.ExtractionManager_ClearDataPagesCompleteEvent);
        break;
    }
    this.progressBar.Visible = false;
    this.LogText(Resources.Message_OperationtionCpcOffline);
    this.LogText($"{(this.action == UserPanel.Action.extraction ? (object) Resources.Message_ExtractionFailed : (object) Resources.Message_ResetFailed)}{(!string.IsNullOrEmpty(this.vin) ? (object) this.vin : (object) Resources.Message_WithNoVIN)}");
    this.action = UserPanel.Action.none;
    this.UpdateUserInterface();
  }

  private void PerformDataPageReset(bool resetAll)
  {
    ExtractionManager.GlobalInstance.ClearDataPagesCompleteEvent += new EventHandler<ResultEventArgs>(this.ExtractionManager_ClearDataPagesCompleteEvent);
    if (resetAll)
      ExtractionManager.GlobalInstance.ResetAllData();
    else
      ExtractionManager.GlobalInstance.ResetTripData();
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (this.Online && string.IsNullOrEmpty(this.vin))
      this.GetVin();
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    if (ExtractionManager.GlobalInstance.SupportDetailsRead && this.Online)
    {
      if (ExtractionManager.GlobalInstance.DataPagesEnabled)
      {
        this.labelDataPageEnabled.Text = this.Busy ? Resources.Message_Working : Resources.Message_DataPagesEnabled;
        this.checkmarkDataPagesEnabled.Checked = true;
      }
      else
      {
        this.labelDataPageEnabled.Text = Resources.Message_DataPagesDisabled;
        this.checkmarkDataPagesEnabled.Checked = false;
      }
      this.buttonExtract.Enabled = this.CanPerformAction;
      this.buttonResetAll.Enabled = this.CanPerformAction;
      this.buttonResetTrip.Enabled = this.CanPerformAction;
      this.buttonSetPassword.Enabled = this.CanPerformAction && ExtractionManager.GlobalInstance.DataPagesEnabled;
      this.buttonClearPassword.Enabled = this.CanPerformAction && ExtractionManager.GlobalInstance.DataPagesEnabled;
    }
    else if (this.Online && SapiManager.SupportsDdecDataPages(this.cpc))
    {
      ExtractionManager.GlobalInstance.SetChannel(this.cpc);
      this.labelDataPageEnabled.Text = Resources.Message_ReadingSupportDetails;
      this.checkmarkDataPagesEnabled.Checked = false;
      this.buttonExtract.Enabled = false;
      this.buttonResetAll.Enabled = false;
      this.buttonResetTrip.Enabled = false;
      this.buttonSetPassword.Enabled = false;
      this.buttonClearPassword.Enabled = false;
    }
    else
    {
      if (this.action == UserPanel.Action.waitingForCPCOnline)
      {
        this.LogText(Resources.Message_WaitingForCPCOnline);
        this.checkmarkDataPagesEnabled.Checked = true;
      }
      else if (this.action != UserPanel.Action.none)
      {
        this.StopActionInProgress();
        this.checkmarkDataPagesEnabled.Checked = false;
      }
      else if (this.checkmarkDataPagesEnabled.Checked)
      {
        this.LogText(Resources.Message_LostConnection);
        this.checkmarkDataPagesEnabled.Checked = false;
      }
      this.labelDataPageEnabled.Text = Resources.Message_NotConnected;
      this.buttonExtract.Enabled = false;
      this.buttonResetAll.Enabled = false;
      this.buttonResetTrip.Enabled = false;
      this.buttonSetPassword.Enabled = false;
      this.buttonClearPassword.Enabled = false;
    }
    this.buttonClose.Enabled = this.CanClose;
  }

  private void LogText(string text)
  {
    this.LabelLog("DDecDataPages", text);
    StringBuilder stringBuilder = new StringBuilder(this.textBoxStatus.Text);
    stringBuilder.AppendLine(text);
    this.textBoxStatus.Text = stringBuilder.ToString();
    this.textBoxStatus.SelectionStart = this.textBoxStatus.TextLength;
    this.textBoxStatus.SelectionLength = 0;
    this.textBoxStatus.ScrollToCaret();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmarkDataPagesEnabled = new Checkmark();
    this.labelDataPageEnabled = new System.Windows.Forms.Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.buttonClearPassword = new Button();
    this.buttonSetPassword = new Button();
    this.buttonClose = new Button();
    this.buttonResetAll = new Button();
    this.progressBar = new ProgressBar();
    this.buttonExtract = new Button();
    this.buttonResetTrip = new Button();
    this.textBoxStatus = new TextBox();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmarkDataPagesEnabled, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelDataPageEnabled, 1, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmarkDataPagesEnabled, "checkmarkDataPagesEnabled");
    ((Control) this.checkmarkDataPagesEnabled).Name = "checkmarkDataPagesEnabled";
    componentResourceManager.ApplyResources((object) this.labelDataPageEnabled, "labelDataPageEnabled");
    this.labelDataPageEnabled.Name = "labelDataPageEnabled";
    this.labelDataPageEnabled.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClearPassword, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetPassword, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonResetAll, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.progressBar, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonExtract, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonResetTrip, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxStatus, 0, 3);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.buttonClearPassword, "buttonClearPassword");
    this.buttonClearPassword.Name = "buttonClearPassword";
    this.buttonClearPassword.UseCompatibleTextRendering = true;
    this.buttonClearPassword.UseVisualStyleBackColor = true;
    this.buttonClearPassword.Click += new EventHandler(this.buttonClearPassword_Click);
    componentResourceManager.ApplyResources((object) this.buttonSetPassword, "buttonSetPassword");
    this.buttonSetPassword.Name = "buttonSetPassword";
    this.buttonSetPassword.UseCompatibleTextRendering = true;
    this.buttonSetPassword.UseVisualStyleBackColor = true;
    this.buttonSetPassword.Click += new EventHandler(this.buttonSetPassword_Click);
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
    componentResourceManager.ApplyResources((object) this.buttonResetAll, "buttonResetAll");
    this.buttonResetAll.Name = "buttonResetAll";
    this.buttonResetAll.UseCompatibleTextRendering = true;
    this.buttonResetAll.UseVisualStyleBackColor = true;
    this.buttonResetAll.Click += new EventHandler(this.buttonResetAll_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.progressBar, 3);
    componentResourceManager.ApplyResources((object) this.progressBar, "progressBar");
    this.progressBar.Name = "progressBar";
    componentResourceManager.ApplyResources((object) this.buttonExtract, "buttonExtract");
    this.buttonExtract.Name = "buttonExtract";
    this.buttonExtract.UseCompatibleTextRendering = true;
    this.buttonExtract.UseVisualStyleBackColor = true;
    this.buttonExtract.Click += new EventHandler(this.buttonExtract_Click);
    componentResourceManager.ApplyResources((object) this.buttonResetTrip, "buttonResetTrip");
    this.buttonResetTrip.Name = "buttonResetTrip";
    this.buttonResetTrip.UseCompatibleTextRendering = true;
    this.buttonResetTrip.UseVisualStyleBackColor = true;
    this.buttonResetTrip.Click += new EventHandler(this.buttonResetTrip_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxStatus, 3);
    componentResourceManager.ApplyResources((object) this.textBoxStatus, "textBoxStatus");
    this.textBoxStatus.Name = "textBoxStatus";
    this.textBoxStatus.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_DDEC_Data_Pages");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum Action
  {
    none,
    extraction,
    resetData,
    clearPassword,
    setPassword,
    waitingForCPCOnline,
  }
}
