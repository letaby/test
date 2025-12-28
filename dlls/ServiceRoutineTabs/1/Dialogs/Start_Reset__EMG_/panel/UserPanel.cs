// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Start_Reset__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Start_Reset__EMG_.panel;

public class UserPanel : CustomPanel
{
  private static string OmcsStartServiceQualifier = "IOC_IOC_HV_OMCS(1)";
  private static List<int> ValidStartValues = new List<int>()
  {
    0,
    1
  };
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

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.waitTimer = new Timer();
    this.waitTimer.Interval = 6000;
    this.waitTimer.Tick += new EventHandler(this.waitTimer_Tick);
    this.UpdateUI();
  }

  public virtual void OnChannelsChanged() => this.SetECPC01TChannel("ECPC01T");

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.routineRunning;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.waitTimer.Tick -= new EventHandler(this.waitTimer_Tick);
    this.waitTimer.Dispose();
  }

  private void SetECPC01TChannel(string ecuName)
  {
    Channel channel = this.GetChannel(ecuName, (CustomPanel.ChannelLookupOptions) 3);
    if (this.eCpcChannel != channel)
    {
      this.eCpcChannel = channel;
      this.routineRunning = this.routineHasRun = false;
    }
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    this.buttonStart.Enabled = this.eCpcChannel != null && this.eCpcChannel.Online && !this.routineRunning;
    this.buttonClose.Enabled = !this.routineRunning;
    if (this.routineHasRun || this.routineRunning)
      return;
    if (this.eCpcChannel != null && this.eCpcChannel.Online)
    {
      this.labelStatusLabelStatusText.Text = Resources.Message_ReadyToRun;
      this.checkmarkStartReset.CheckState = CheckState.Checked;
    }
    else
    {
      this.labelStatusLabelStatusText.Text = Resources.Message_NotReady;
      this.checkmarkStartReset.CheckState = CheckState.Unchecked;
    }
  }

  private void LogMessage(string text, bool updateStatusText, CheckState checkState)
  {
    this.AddStatusMessage(text);
    this.LabelLog("EmgStartReset", text);
    if (!updateStatusText)
      return;
    this.labelStatusLabelStatusText.Text = text;
    this.checkmarkStartReset.CheckState = checkState;
  }

  private bool RunService(
    Channel channel,
    string serviceQualifier,
    ServiceCompleteEventHandler serviceCompleteEventHandler)
  {
    if (channel != null && channel.Online)
    {
      Service service = channel.Services[serviceQualifier];
      if (service != (Service) null)
      {
        if (serviceCompleteEventHandler != null)
          service.ServiceCompleteEvent += serviceCompleteEventHandler;
        service.InputValues.ParseValues(serviceQualifier);
        service.Execute(false);
        return true;
      }
    }
    return false;
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    this.routineHasRun = true;
    this.routineRunning = this.RunService(this.eCpcChannel, UserPanel.OmcsStartServiceQualifier, new ServiceCompleteEventHandler(this.Start_ServiceCompleteEvent));
    this.LogMessage(this.routineRunning ? Resources.Message_Running : Resources.Message_CouldNotStart, true, this.routineRunning ? CheckState.Indeterminate : CheckState.Unchecked);
    this.UpdateUI();
  }

  private int ConvertChoiceValueObjectToRawValue(ServiceOutputValue value)
  {
    Choice choice = value.Value as Choice;
    if (choice != (object) null)
    {
      try
      {
        return Convert.ToInt32(choice.RawValue);
      }
      catch (InvalidCastException ex)
      {
        this.LogMessage(ex.Message, false, CheckState.Checked);
      }
    }
    return -1;
  }

  private void Start_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    int num = -1;
    if (service != (Service) null)
    {
      num = this.ConvertChoiceValueObjectToRawValue(service.OutputValues[0]);
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Start_ServiceCompleteEvent);
    }
    if (e.Succeeded && UserPanel.ValidStartValues.Contains(num))
    {
      this.waitTimer.Start();
    }
    else
    {
      this.routineRunning = false;
      string messageUnknown = Resources.Message_Unknown;
      if (service.OutputValues.Count > 0 && service.OutputValues[0].Value != null)
        messageUnknown = service.OutputValues[0].Value.ToString();
      this.LogMessage(string.Format(Resources.MessageFormat_Failed0, e.Exception != null ? (object) e.Exception.Message : (object) messageUnknown), true, CheckState.Unchecked);
    }
    this.UpdateUI();
  }

  private void waitTimer_Tick(object sender, EventArgs e)
  {
    this.waitTimer.Stop();
    this.routineRunning = this.RunService(this.eCpcChannel, UserPanel.OmcsStopServiceQualifier, new ServiceCompleteEventHandler(this.Stop_ServiceCompleteEvent));
    this.LogMessage(this.routineRunning ? Resources.Message_ReturningControl : Resources.Message_CouldNotReturnControl, true, this.routineRunning ? CheckState.Checked : CheckState.Unchecked);
    this.UpdateUI();
  }

  private void Stop_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Stop_ServiceCompleteEvent);
    this.routineRunning = false;
    if (e.Succeeded)
    {
      this.LogMessage(Resources.Message_ResetComplete, true, CheckState.Checked);
    }
    else
    {
      string str = e.Exception != null ? e.Exception.Message : Resources.Message_Unknown1;
      this.LogMessage(string.Format(Resources.MessageFormat_CouldNotReturnControl0, (object) e.Exception.Message), true, CheckState.Unchecked);
    }
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.selectablePanel1 = new SelectablePanel();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.labelStatusLabelStatusText = new System.Windows.Forms.Label();
    this.label5 = new System.Windows.Forms.Label();
    this.checkmarkStartReset = new Checkmark();
    this.label3 = new System.Windows.Forms.Label();
    this.label4 = new System.Windows.Forms.Label();
    this.buttonStart = new Button();
    this.buttonClose = new Button();
    ((Control) this.selectablePanel1).SuspendLayout();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((Control) this.selectablePanel1).Controls.Add((Control) this.tableLayoutPanelMain);
    componentResourceManager.ApplyResources((object) this.selectablePanel1, "selectablePanel1");
    ((Control) this.selectablePanel1).Name = "selectablePanel1";
    ((Panel) this.selectablePanel1).TabStop = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelStatusLabelStatusText, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label5, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.checkmarkStartReset, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label4, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonStart, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 4, 2);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.labelStatusLabelStatusText, "labelStatusLabelStatusText");
    this.labelStatusLabelStatusText.Name = "labelStatusLabelStatusText";
    this.labelStatusLabelStatusText.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkStartReset, "checkmarkStartReset");
    ((Control) this.checkmarkStartReset).Name = "checkmarkStartReset";
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.label3, 5);
    this.label3.Name = "label3";
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.label4, 5);
    this.label4.Name = "label4";
    this.label4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_OMCS_Reset");
    ((Control) this).Controls.Add((Control) this.selectablePanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.selectablePanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
