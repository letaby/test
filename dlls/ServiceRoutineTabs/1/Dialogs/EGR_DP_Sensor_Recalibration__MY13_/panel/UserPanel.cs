// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__MY13_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string RecalibrationServiceQualifier = "RT_SR065_Forced_Auto_Cal_EGR_Delta_P_Sensor_Start_status";
  private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";
  private Channel mcm = (Channel) null;
  private Service recalibrationService = (Service) null;
  private Instrument engineSpeed = (Instrument) null;
  private Button buttonBegin;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelMCMStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelEngineStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label2;
  private TableLayoutPanel tableLayoutPanel4;
  private Button buttonClose;
  private Checkmark McmCheck;
  private Checkmark EngineSpeedCheck;
  private Checkmark beginCheck;
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
    this.SetMCM(this.GetChannel("MCM21T"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return;
    this.ResetData();
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    if (this.engineSpeed != (Instrument) null)
    {
      this.engineSpeed.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      this.engineSpeed = (Instrument) null;
    }
    this.mcm = mcm;
    if (this.mcm != null)
    {
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.engineSpeed = this.mcm.Instruments["DT_AS010_Engine_Speed"];
      if (this.engineSpeed != (Instrument) null)
        this.engineSpeed.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
    }
  }

  private bool Working => this.recalibrationService != (Service) null;

  private bool Online
  {
    get => this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool CanBegin
  {
    get => !this.Working && this.Online && this.McmCheck.Checked && this.EngineSpeedCheck.Checked;
  }

  private void UpdateMCMStatus()
  {
    bool flag = false;
    string str = Resources.Message_MCM21TIsNotConnected;
    if (this.mcm != null)
    {
      if (this.mcm.CommunicationsState == CommunicationsState.Online)
      {
        str = Resources.Message_MCM21TIsConnected;
        flag = true;
      }
      else
        str = Resources.Message_MCM21TIsBusy;
    }
    ((Control) this.labelMCMStatus).Text = str;
    this.McmCheck.Checked = flag;
  }

  private void UpdateEngineStatus()
  {
    bool flag = false;
    string str = Resources.Message_EngineSpeedCannotBeDetected;
    if (this.engineSpeed != (Instrument) null && this.engineSpeed.InstrumentValues.Current != null)
    {
      double d = Convert.ToDouble(this.engineSpeed.InstrumentValues.Current.Value);
      if (!double.IsNaN(d))
      {
        if (d == 0.0)
        {
          str = Resources.Message_EngineIsNotRunning;
          flag = true;
        }
        else
          str = Resources.Message_EngineIsRunning;
      }
    }
    ((Control) this.labelEngineStatus).Text = str;
    this.EngineSpeedCheck.Checked = flag;
  }

  private void UpdateButtonStatus()
  {
    bool canBegin = this.CanBegin;
    this.buttonBegin.Enabled = canBegin;
    this.beginCheck.Checked = canBegin;
  }

  private void UpdateUserInterface()
  {
    this.UpdateMCMStatus();
    this.UpdateEngineStatus();
    this.UpdateButtonStatus();
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void OnClickBegin(object sender, EventArgs e)
  {
    if (!this.CanBegin || MessageBox.Show((IWin32Window) this, Resources.Message_PerformEGRDPSensorRecalibration, ApplicationInformation.ProductName, MessageBoxButtons.YesNo) != DialogResult.Yes)
      return;
    this.ClearResults();
    this.recalibrationService = this.mcm.Services["RT_SR065_Forced_Auto_Cal_EGR_Delta_P_Sensor_Start_status"];
    if (this.recalibrationService != (Service) null)
    {
      this.UpdateUserInterface();
      this.recalibrationService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      this.recalibrationService.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated);
  }

  private void ResetData() => this.ClearResults();

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.recalibrationService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (e.Succeeded)
      this.ReportResult(string.Format(Resources.MessageFormat_RecalibrationExecuted0, (object) this.recalibrationService.OutputValues[0].Value.ToString()));
    else
      this.ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredDuringRecalibration, (object) e.Exception.Message));
    this.recalibrationService = (Service) null;
    this.UpdateUserInterface();
  }

  private void ClearResults()
  {
    if (this.textboxResults == null)
      return;
    this.textboxResults.Text = "";
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
    this.labelEngineStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.textboxResults = new TextBox();
    this.labelMCMStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonBegin = new Button();
    this.label2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonClose = new Button();
    this.McmCheck = new Checkmark();
    this.EngineSpeedCheck = new Checkmark();
    this.beginCheck = new Checkmark();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelEngineStatus, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.textboxResults, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelMCMStatus, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.buttonBegin, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.buttonClose, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.McmCheck, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.EngineSpeedCheck, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.beginCheck, 0, 3);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    this.labelEngineStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelEngineStatus, "labelEngineStatus");
    ((Control) this.labelEngineStatus).Name = "labelEngineStatus";
    this.labelEngineStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelEngineStatus.ShowBorder = false;
    this.labelEngineStatus.UseSystemColors = true;
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
    this.label2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    ((TableLayoutPanel) this.tableLayoutPanel4).SetColumnSpan((Control) this.label2, 2);
    ((Control) this.label2).Name = "label2";
    this.label2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label2.UseSystemColors = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.McmCheck, "McmCheck");
    ((Control) this.McmCheck).Name = "McmCheck";
    componentResourceManager.ApplyResources((object) this.EngineSpeedCheck, "EngineSpeedCheck");
    ((Control) this.EngineSpeedCheck).Name = "EngineSpeedCheck";
    componentResourceManager.ApplyResources((object) this.beginCheck, "beginCheck");
    ((Control) this.beginCheck).Name = "beginCheck";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_EGRDPSensorRecalibration");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel4);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
