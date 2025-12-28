// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
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
    this.InitializeComponent();
    this.textBoxDuration.KeyPress += new KeyPressEventHandler(this.OnDurationKeyPress);
    this.textBoxDuration.TextChanged += new EventHandler(this.OnDurationTextChanged);
    ((RunSharedProcedureButtonBase) this.runServiceButtonStart).ButtonEnabledChanged += new EventHandler(this.OnStartButtonEnabledChanged);
    ((RunSharedProcedureButtonBase) this.runServiceButtonStart).Starting += new EventHandler<CancelEventArgs>(this.OnStartStarting);
    this.runServiceButtonStart.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.OnStartFinished);
    ((RunSharedProcedureButtonBase) this.runServiceButtonStop).Starting += new EventHandler<CancelEventArgs>(this.OnStopStarting);
    this.runServiceButtonStop.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.OnStopFinished);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.ClearResults();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.textBoxDuration.Text = 30.ToString();
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  public virtual void OnChannelsChanged() => this.UpdateUserInterface();

  private void OnDurationKeyPress(object sender, KeyPressEventArgs e)
  {
    if (e.KeyChar == '\b' || UserPanel.DurationValidation.IsMatch(e.KeyChar.ToString()))
      return;
    e.Handled = true;
  }

  private void OnDurationTextChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private bool ValidateAndSetDuration()
  {
    if (!((RunSharedProcedureButtonBase) this.runServiceButtonStart).IsBusy)
    {
      string text = this.textBoxDuration.Text;
      int result;
      if (!string.IsNullOrEmpty(text) && UserPanel.DurationValidation.IsMatch(text) && int.TryParse(text, out result) && 0 <= result && result <= 120)
      {
        ServiceCall serviceCall1 = this.runServiceButtonStart.ServiceCall;
        if (!((ServiceCall) ref serviceCall1).IsEmpty)
        {
          RunServiceButton serviceButtonStart = this.runServiceButtonStart;
          ServiceCall serviceCall2 = this.runServiceButtonStart.ServiceCall;
          string ecu = ((ServiceCall) ref serviceCall2).Ecu;
          serviceCall2 = this.runServiceButtonStart.ServiceCall;
          string qualifier = ((ServiceCall) ref serviceCall2).Qualifier;
          string[] strArray = new string[1]
          {
            (result * 60000).ToString()
          };
          ServiceCall serviceCall3 = new ServiceCall(ecu, qualifier, (IEnumerable<string>) strArray);
          serviceButtonStart.ServiceCall = serviceCall3;
          return true;
        }
      }
    }
    return false;
  }

  private void OnStartButtonEnabledChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    ((Control) this.runServiceButtonStart).Enabled = this.ValidateAndSetDuration();
    this.textBoxDuration.Enabled = ((RunSharedProcedureButtonBase) this.runServiceButtonStart).ButtonEnabled;
  }

  private void OnStartStarting(object sender, CancelEventArgs e)
  {
    this.ReportResult(Resources.Message_ExecutingStart);
  }

  private void OnStartFinished(object sender, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.ReportResult(Resources.Message_ATDTestPipeStartSuccessfullyExecuted);
    else
      this.ReportResult(Resources.Message_ATDTestPipeStartFailedExecution + ((ResultEventArgs) e).Exception.Message);
  }

  private void OnStopStarting(object sender, CancelEventArgs e)
  {
    this.ReportResult(Resources.Message_ExecutingStop);
  }

  private void OnStopFinished(object sender, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.ReportResult(Resources.Message_ATDTestPipeStopSuccessfullyExecuted);
    else
      this.ReportResult(Resources.Message_ATDTestPipeStopFailedExecution + ((ResultEventArgs) e).Exception.Message);
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
    this.tableDuration = new TableLayoutPanel();
    this.tableMainLayout = new TableLayoutPanel();
    this.textBoxDuration = new TextBox();
    this.labelDuration = new System.Windows.Forms.Label();
    this.labelMinutes = new System.Windows.Forms.Label();
    this.textboxResults = new TextBox();
    this.buttonClose = new Button();
    this.runServiceButtonStart = new RunServiceButton();
    this.runServiceButtonStop = new RunServiceButton();
    ((Control) this.tableMainLayout).SuspendLayout();
    ((Control) this.tableDuration).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableMainLayout, "tableMainLayout");
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.tableDuration, 1, 0);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.textboxResults, 0, 2);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.buttonClose, 1, 3);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.runServiceButtonStart, 0, 0);
    ((TableLayoutPanel) this.tableMainLayout).Controls.Add((Control) this.runServiceButtonStop, 0, 1);
    ((Control) this.tableMainLayout).Name = "tableMainLayout";
    componentResourceManager.ApplyResources((object) this.tableDuration, "tableDuration");
    ((TableLayoutPanel) this.tableDuration).Controls.Add((Control) this.textBoxDuration, 1, 1);
    ((TableLayoutPanel) this.tableDuration).Controls.Add((Control) this.labelDuration, 0, 1);
    ((TableLayoutPanel) this.tableDuration).Controls.Add((Control) this.labelMinutes, 2, 1);
    ((Control) this.tableDuration).Name = "tableDuration";
    componentResourceManager.ApplyResources((object) this.textBoxDuration, "textBoxDuration");
    this.textBoxDuration.Name = "textBoxDuration";
    componentResourceManager.ApplyResources((object) this.labelDuration, "labelDuration");
    this.labelDuration.Name = "labelDuration";
    this.labelDuration.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelMinutes, "labelMinutes");
    this.labelMinutes.Name = "labelMinutes";
    this.labelMinutes.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableMainLayout).SetColumnSpan((Control) this.textboxResults, 2);
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonStart, "runServiceButtonStart");
    ((Control) this.runServiceButtonStart).Name = "runServiceButtonStart";
    this.runServiceButtonStart.ServiceCall = new ServiceCall("MCM", "RT_SR017_ATD_Test_Sensor_Package_Start", (IEnumerable<string>) new string[1]
    {
      "20000"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonStop, "runServiceButtonStop");
    ((Control) this.runServiceButtonStop).Name = "runServiceButtonStop";
    this.runServiceButtonStop.ServiceCall = new ServiceCall("MCM", "RT_SR017_ATD_Test_Sensor_Package_Stop");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_ATDTestPipe");
    ((Control) this).Controls.Add((Control) this.tableMainLayout);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableMainLayout).ResumeLayout(false);
    ((Control) this.tableMainLayout).PerformLayout();
    ((Control) this.tableDuration).ResumeLayout(false);
    ((Control) this.tableDuration).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
