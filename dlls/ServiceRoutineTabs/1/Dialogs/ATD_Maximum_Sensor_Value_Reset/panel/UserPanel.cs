// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Maximum_Sensor_Value_Reset.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Maximum_Sensor_Value_Reset.panel;

public class UserPanel : CustomPanel
{
  private const string ResetService = "RT_SR014_SET_EOL_Default_Values_Start";
  private Channel mcm = (Channel) null;
  private Service resetService = (Service) null;
  private Button buttonBegin;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelMCMStatus;
  private TableLayoutPanel tableLayoutPanel4;
  private Button buttonClose;
  private Checkmark mcmConnectedCheck;
  private Checkmark canBeginCheck;
  private TextBox textboxResults;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonBegin.Click += new EventHandler(this.OnClickBegin);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.ClearResults();
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetMCM((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return;
    this.ResetData();
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.mcm = mcm;
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
  }

  private bool Working => this.resetService != (Service) null;

  private bool Online
  {
    get => this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool CanBegin => !this.Working && this.Online && this.mcmConnectedCheck.Checked;

  private void UpdateMCMStatus()
  {
    bool flag = false;
    string str = Resources.Message_PleaseConnectTheMCM;
    if (this.mcm != null)
    {
      if (this.mcm.CommunicationsState == CommunicationsState.Online)
      {
        str = Resources.Message_MCMIsConnected;
        flag = true;
      }
      else
        str = Resources.Message_MCMIsBusy;
    }
    ((Control) this.labelMCMStatus).Text = str;
    this.mcmConnectedCheck.Checked = flag;
  }

  private void UpdateButtonStatus()
  {
    bool canBegin = this.CanBegin;
    this.buttonBegin.Enabled = canBegin;
    this.canBeginCheck.Checked = canBegin;
  }

  private void UpdateUserInterface()
  {
    this.UpdateMCMStatus();
    this.UpdateButtonStatus();
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnClickBegin(object sender, EventArgs e)
  {
    if (!this.CanBegin || MessageBox.Show((IWin32Window) this, Resources.Message_PerformSensorValueReset, ApplicationInformation.ProductName, MessageBoxButtons.YesNo) != DialogResult.Yes)
      return;
    this.ClearResults();
    this.resetService = this.mcm.Services["RT_SR014_SET_EOL_Default_Values_Start"];
    if (this.resetService != (Service) null)
    {
      this.UpdateUserInterface();
      this.ReportResult($"{Resources.Message_Executing}{this.resetService.Name}...");
      this.resetService.InputValues[0].Value = (object) this.resetService.InputValues[0].Choices.GetItemFromRawValue((object) 6);
      this.resetService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      this.resetService.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_UnableToAcquireTheResetServiceATDMaximumSensorValuesCannotBeReset);
  }

  private void ResetData() => this.ClearResults();

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.resetService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (e.Succeeded)
    {
      if (this.resetService.OutputValues.Count > 0)
        this.ReportResult(string.Format(Resources.MessageFormat_ResetExecuted0, (object) this.resetService.OutputValues[0].Value.ToString()));
      else
        this.ReportResult(Resources.Message_ResetExecutedSuccessfully);
    }
    else
      this.ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredDuringTheReset0, (object) e.Exception.Message));
    this.resetService = (Service) null;
    this.UpdateUserInterface();
  }

  private void ClearResults()
  {
    if (this.textboxResults == null)
      return;
    this.textboxResults.Text = string.Empty;
  }

  private void ReportResult(string text)
  {
    if (this.textboxResults != null)
    {
      StringBuilder stringBuilder = new StringBuilder(this.textboxResults.Text);
      stringBuilder.Append(text);
      stringBuilder.Append("\r\n");
      this.textboxResults.Text = stringBuilder.ToString();
      this.textboxResults.SelectionStart = this.textboxResults.TextLength;
      this.textboxResults.SelectionLength = 0;
      this.textboxResults.ScrollToCaret();
    }
    this.AddStatusMessage(text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.textboxResults = new TextBox();
    this.labelMCMStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonBegin = new Button();
    this.buttonClose = new Button();
    this.mcmConnectedCheck = new Checkmark();
    this.canBeginCheck = new Checkmark();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.textboxResults, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelMCMStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.buttonBegin, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.buttonClose, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.mcmConnectedCheck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.canBeginCheck, 0, 1);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    this.labelMCMStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelMCMStatus, "labelMCMStatus");
    ((Control) this.labelMCMStatus).Name = "labelMCMStatus";
    this.labelMCMStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelMCMStatus.ShowBorder = false;
    this.labelMCMStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonBegin, "buttonBegin");
    this.buttonBegin.Name = "buttonBegin";
    this.buttonBegin.UseCompatibleTextRendering = true;
    this.buttonBegin.UseVisualStyleBackColor = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.mcmConnectedCheck, "mcmConnectedCheck");
    ((Control) this.mcmConnectedCheck).Name = "mcmConnectedCheck";
    this.mcmConnectedCheck.SizeMode = PictureBoxSizeMode.AutoSize;
    componentResourceManager.ApplyResources((object) this.canBeginCheck, "canBeginCheck");
    ((Control) this.canBeginCheck).Name = "canBeginCheck";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_ATDMaximumSensorValueReset");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel4);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
