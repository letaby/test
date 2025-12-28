// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code.panel;

public class UserPanel : CustomPanel
{
  private const string ResetEEPROMServiceList = "RT_Reload_Original_CPC_Factory_Settings_All_of_EEPROM_minus_SVDO_Block_Start_Routine_Status;FN_KeyOffOnReset";
  private const string EEPROMChecksumDTC = "740202";
  private const string CommonVIN = "CO_VIN";
  private Channel cpc2;
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private Dictionary<string, object> parameters = new Dictionary<string, object>();
  private System.Windows.Forms.Label label1;
  private Checkmark checkmarkDTC;
  private System.Windows.Forms.Label labelDTC;
  private TextBox textBoxOutput;
  private Button buttonClose;
  private Button buttonStart;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonStart.Click += new EventHandler(this.OnStartClick);
    this.currentStage = UserPanel.Stage.Idle;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.SetCPC2(this.GetChannel("CPC02T"));
  }

  public virtual void OnChannelsChanged() => this.SetCPC2(this.GetChannel("CPC02T"));

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.StopWork(UserPanel.Reason.Closing);
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.SetCPC2((Channel) null);
  }

  private void OnStartClick(object sender, EventArgs e)
  {
    this.ClearOutput();
    this.CurrentStage = this.parameters.Count <= 0 ? UserPanel.Stage.ReadParameters : UserPanel.Stage.Reset;
    this.PerformCurrentStage();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnFaultCodesUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void OnParametersReadComplete(object sender, ResultEventArgs e)
  {
    this.cpc2.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
    if (e.Succeeded)
    {
      UserPanel.Stage stage = UserPanel.Stage.Unknown;
      switch (this.CurrentStage)
      {
        case UserPanel.Stage.ReadParameters:
          this.parameters.Clear();
          foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) this.cpc2.Parameters)
            this.parameters.Add(parameter.Qualifier, parameter.Value);
          stage = UserPanel.Stage.Reset;
          break;
        case UserPanel.Stage.ReadResetParameters:
          stage = UserPanel.Stage.WriteParameters;
          break;
      }
      this.CurrentStage = stage;
      this.PerformCurrentStage();
    }
    else
      this.StopWork(UserPanel.Reason.FailedParametersRead);
  }

  private void OnParametersWriteComplete(object sender, ResultEventArgs e)
  {
    this.cpc2.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
    if (e.Succeeded)
    {
      this.CurrentStage = UserPanel.Stage.Finish;
      this.PerformCurrentStage();
    }
    else
      this.StopWork(UserPanel.Reason.FailedParametersRead);
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    this.cpc2.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (e.Succeeded)
    {
      this.CurrentStage = UserPanel.Stage.ReadResetParameters;
      this.PerformCurrentStage();
    }
    else
      this.StopWork(UserPanel.Reason.FailedServiceExecute);
  }

  private bool CanClose => !this.Working;

  private bool Online => this.cpc2 != null && this.cpc2.Online;

  private bool FaultIsPresent
  {
    get
    {
      if (this.Online && this.cpc2.FaultCodes.HaveBeenReadFromEcu)
      {
        FaultCode faultCode = this.cpc2.FaultCodes["740202"];
        if (faultCode != null)
        {
          FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
          if (current != null && current.Active == ActiveStatus.Active)
            return true;
        }
      }
      return false;
    }
  }

  private bool EcuIsBusy
  {
    get => this.Online && this.cpc2.CommunicationsState != CommunicationsState.Online;
  }

  private bool CanStart => !this.Working && this.Online && this.FaultIsPresent && !this.EcuIsBusy;

  private UserPanel.Stage CurrentStage
  {
    get => this.currentStage;
    set
    {
      if (this.currentStage == value)
        return;
      this.currentStage = value;
      this.UpdateUserInterface();
      Application.DoEvents();
    }
  }

  private bool Working => this.currentStage != UserPanel.Stage.Idle;

  private void ClearOutput() => this.textBoxOutput.Text = string.Empty;

  private void ReportResult(string text)
  {
    this.textBoxOutput.Text = $"{this.textBoxOutput.Text}{text}\r\n";
    this.textBoxOutput.SelectionStart = this.textBoxOutput.TextLength;
    this.textBoxOutput.SelectionLength = 0;
    this.textBoxOutput.ScrollToCaret();
  }

  private void UpdateUserInterface()
  {
    this.buttonStart.Enabled = this.CanStart;
    this.buttonClose.Enabled = this.CanClose;
    if (this.Online)
    {
      this.checkmarkDTC.Checked = !this.FaultIsPresent;
      if (!this.Working)
        this.labelDTC.Text = this.FaultIsPresent ? Resources.Message_TheEEPROMChecksumFailureFaultCodeIsPresentAndMustBeCleared : Resources.Message_TheEEPROMChecksumFailureFaultCodeIsNotPresentNoActionIsNecessary;
      else
        this.labelDTC.Text = Resources.Message_ProcessingPleaseWait;
    }
    else
    {
      this.checkmarkDTC.Checked = false;
      this.labelDTC.Text = Resources.Message_TheCPC2IsNotOnlineSoFaultCodesCouldNotBeRead;
    }
  }

  private void SetCPC2(Channel channel)
  {
    if (this.cpc2 == channel)
      return;
    if (this.cpc2 != null)
    {
      this.cpc2.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.cpc2.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.OnFaultCodesUpdate);
      this.StopWork(UserPanel.Reason.Disconnected);
    }
    this.cpc2 = channel;
    if (this.cpc2 != null)
    {
      this.cpc2.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.cpc2.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodesUpdate);
    }
  }

  private void PerformCurrentStage()
  {
    if (this.cpc2 != null && this.cpc2.Online)
    {
      StringDictionary stringDictionary = new StringDictionary();
      switch (this.CurrentStage)
      {
        case UserPanel.Stage.ReadParameters:
          this.ReportResult(Resources.Message_ReadingParameters);
          this.cpc2.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
          this.cpc2.Parameters.ReadAll(false);
          break;
        case UserPanel.Stage.Reset:
          this.ReportResult(Resources.Message_PreparingForEEPROMReset);
          if (!this.Unlock())
            break;
          this.ReportResult(Resources.Message_PerformingEEPROMReset);
          this.ExecuteService("RT_Reload_Original_CPC_Factory_Settings_All_of_EEPROM_minus_SVDO_Block_Start_Routine_Status;FN_KeyOffOnReset");
          break;
        case UserPanel.Stage.ReadResetParameters:
          this.ReportResult(Resources.Message_ReadingDefaultParametersAfterReset);
          this.cpc2.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
          this.cpc2.Parameters.ReadAll(false);
          break;
        case UserPanel.Stage.WriteParameters:
          this.ReportResult(Resources.Message_AssigningParameters);
          int num = this.AssignParameters();
          if (num > 0)
          {
            this.ReportResult(Resources.Message_UpdateSeed);
            this.cpc2.EcuInfos["CO_VIN"].Read(true);
            this.ReportResult(string.Format(Resources.MessageFormat_PreparingToWriteBack0Parameters, (object) num));
            if (!this.Unlock())
              break;
            this.ReportResult(string.Format(Resources.MessageFormat_Writing0Parameters, (object) num));
            this.cpc2.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
            this.cpc2.Parameters.Write(false);
            break;
          }
          this.StopWork(UserPanel.Reason.NoParametersChanged);
          break;
        case UserPanel.Stage.Finish:
          this.StopWork(UserPanel.Reason.Succeeded);
          break;
        case UserPanel.Stage.Unknown:
          this.StopWork(UserPanel.Reason.UnknownStage);
          break;
      }
    }
    else
      this.StopWork(UserPanel.Reason.Disconnected);
  }

  private int AssignParameters()
  {
    int num = 0;
    foreach (KeyValuePair<string, object> parameter1 in this.parameters)
    {
      Parameter parameter2 = this.cpc2.Parameters[parameter1.Key];
      if (parameter2 != null && parameter1.Value != null && !object.Equals(parameter2.Value, parameter1.Value))
      {
        parameter2.Value = parameter1.Value;
        ++num;
      }
    }
    return num;
  }

  private void ExecuteService(string qualifier)
  {
    this.cpc2.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (this.cpc2.Services.Execute("RT_Reload_Original_CPC_Factory_Settings_All_of_EEPROM_minus_SVDO_Block_Start_Routine_Status;FN_KeyOffOnReset", false) != 0)
      return;
    this.cpc2.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.StopWork(UserPanel.Reason.FailedService);
  }

  private bool Unlock()
  {
    try
    {
      this.cpc2.Extension.Invoke(nameof (Unlock), (object[]) null);
      return true;
    }
    catch (CaesarException ex)
    {
      this.StopWork(UserPanel.Reason.FailedUnlock);
      return false;
    }
  }

  private void StopWork(UserPanel.Reason reason)
  {
    if (this.CurrentStage == UserPanel.Stage.Stopping || this.CurrentStage == UserPanel.Stage.Idle)
      return;
    UserPanel.Stage currentStage = this.CurrentStage;
    this.CurrentStage = UserPanel.Stage.Stopping;
    switch (reason)
    {
      case UserPanel.Reason.Succeeded:
        if (currentStage != UserPanel.Stage.Finish)
          throw new InvalidOperationException();
        this.ReportResult(Resources.Message_Complete);
        this.buttonStart.Text = Resources.Message_Start;
        this.parameters.Clear();
        break;
      case UserPanel.Reason.NoParametersChanged:
        this.ReportResult(Resources.Message_TheProcedureFailedToComplete);
        switch (reason - 1)
        {
          case UserPanel.Reason.Succeeded:
            this.ReportResult(Resources.Message_FailedToReadExistingParameters);
            break;
          case UserPanel.Reason.FailedParametersWrite:
            this.ReportResult(Resources.Message_FailedToExecuteService);
            break;
          case UserPanel.Reason.FailedServiceExecute:
            this.ReportResult(Resources.Message_FailedToObtainService);
            break;
          case UserPanel.Reason.FailedService:
            this.ReportResult(Resources.Message_FailedToUnlock);
            break;
          case UserPanel.Reason.Closing:
            this.ReportResult(Resources.Message_TheCPC2WasDisconnected);
            break;
          case UserPanel.Reason.Disconnected:
            this.ReportResult(Resources.Message_UnknownStage);
            break;
          case UserPanel.Reason.UnknownStage:
            this.ReportResult(Resources.Message_NoParametersAreChangedFromDefaultUseProgramDeviceToRestoreServerConfiguration);
            break;
        }
        break;
      default:
        this.buttonStart.Text = Resources.Message_Retry;
        goto case UserPanel.Reason.NoParametersChanged;
    }
    this.CurrentStage = UserPanel.Stage.Idle;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.label1 = new System.Windows.Forms.Label();
    this.checkmarkDTC = new Checkmark();
    this.labelDTC = new System.Windows.Forms.Label();
    this.textBoxOutput = new TextBox();
    this.buttonClose = new Button();
    this.buttonStart = new Button();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkDTC, "checkmarkDTC");
    ((Control) this.checkmarkDTC).Name = "checkmarkDTC";
    componentResourceManager.ApplyResources((object) this.labelDTC, "labelDTC");
    this.labelDTC.Name = "labelDTC";
    this.labelDTC.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.Cancel;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.buttonStart);
    ((Control) this).Controls.Add((Control) this.buttonClose);
    ((Control) this).Controls.Add((Control) this.textBoxOutput);
    ((Control) this).Controls.Add((Control) this.labelDTC);
    ((Control) this).Controls.Add((Control) this.checkmarkDTC);
    ((Control) this).Controls.Add((Control) this.label1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }

  private enum Stage
  {
    Idle,
    ReadParameters,
    Reset,
    ReadResetParameters,
    WriteParameters,
    Stopping,
    Finish,
    Unknown,
  }

  private enum Reason
  {
    Succeeded,
    FailedParametersRead,
    FailedParametersWrite,
    FailedServiceExecute,
    FailedService,
    FailedUnlock,
    Closing,
    Disconnected,
    UnknownStage,
    NoParametersChanged,
  }
}
