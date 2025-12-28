// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY20_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY20_.panel;

public class UserPanel : CustomPanel
{
  private const string DpfAshVolumeSet = "RT_SR08B_DPF_ash_volume_ratio_update_Start";
  private const string DpfAshVolumeRead = "RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM";
  private const string ACMAshVolumeRatioUpdateStart = "RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction";
  private const string QualifierOdometer = "CO_Odometer";
  private const string QualifierEngineHours = "DT_AS045_Engine_Operating_Hours";
  private static readonly UserPanel.ValidationInformation HeavyDutySerialNumberValidation = new UserPanel.ValidationInformation(AtsType.TwoBox, new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));
  private static readonly UserPanel.ValidationInformation MediumDutySerialNumberValidation = new UserPanel.ValidationInformation(AtsType.OneBoxOneFilter, new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));
  private Dictionary<string, UserPanel.ValidationInformation> serialNumberValidations = new Dictionary<string, UserPanel.ValidationInformation>();
  private UserPanel.ValidationInformation connectedValidationInformation;
  private Channel acm;
  private Channel mcm;
  private static readonly Qualifier ATDTypeParameter = new Qualifier((QualifierTypes) 4, "ACM301T", "ATD_Hardware_Type");
  private ParameterDataItem atdType;
  private static readonly Qualifier AshRatioInstrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS109_Ash_Filter_Full_Volume");
  private InstrumentDataItem ashRatioInstrument;
  private string targetVIN;
  private bool adrReturnValue = false;
  private static readonly Regex AnySerialNumberValidation = new Regex("[R\\d]", RegexOptions.Compiled);
  private static readonly Regex MdegFullScopeSerialNumberValidation = new Regex("[A-Z0-9]", RegexOptions.Compiled);
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private object oldValue;
  private string valueToWrite;
  private string dpfSN1;
  private string instrumentRatioAtStart;
  private DateTime timeAtStart;
  private Timer verificationTimeoutTimer = new Timer();
  private static readonly TimeSpan VerificationTimeoutPeriod = new TimeSpan(0, 0, 10);
  private Service currentService;
  private Service lastRunService;
  private ScalingLabel titleLabel;
  private DigitalReadoutInstrument cpcReadout;
  private DigitalReadoutInstrument acmReadout;
  private System.Windows.Forms.Label labelMCM;
  private System.Windows.Forms.Label labelACM;
  private System.Windows.Forms.Label labelTaskQuestion;
  private RadioButton radioCleanRemanFilter;
  private RadioButton radioNewFilter;
  private RadioButton radioACMReplace;
  private TableLayoutPanel tableLayoutDPFSerialNumber;
  private TextBox textBoxDPFSerialNumber1;
  private System.Windows.Forms.Label labelProgress;
  private TextBox textBoxProgress;
  private Button buttonPerformAction;
  private System.Windows.Forms.Label labelSNErrorMessage1;
  private System.Windows.Forms.Label labelWarning;
  private Button buttonClose;
  private FlowLayoutPanel flowLayoutPanel1;
  private System.Windows.Forms.Label labelLicenseMessage;
  private System.Windows.Forms.Label labelDPFSerialNumberHeader;
  private TableLayoutPanel tableLayoutPanel;

  public UserPanel()
  {
    this.InitializeComponent();
    this.serialNumberValidations.Add("DD13", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD15", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD16", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD5", UserPanel.MediumDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD8", UserPanel.MediumDutySerialNumberValidation);
    this.radioCleanRemanFilter.Checked = true;
    this.radioCleanRemanFilter.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioNewFilter.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioACMReplace.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.textBoxDPFSerialNumber1.TextChanged += new EventHandler(this.OnDPFSerialNumberChanged);
    this.textBoxDPFSerialNumber1.KeyPress += new KeyPressEventHandler(this.OnDPFSerialNumberKeyPress);
    this.buttonPerformAction.Click += new EventHandler(this.OnPerformAction);
    this.verificationTimeoutTimer.Interval = (int) (UserPanel.VerificationTimeoutPeriod.TotalMilliseconds / 2.0);
    this.verificationTimeoutTimer.Tick += (EventHandler) ((sender, args) =>
    {
      if (this.CurrentStage != UserPanel.Stage.WaitingToConfirmChange && this.CurrentStage != UserPanel.Stage.WaitingForMCM21TInputToUpdate)
        return;
      this.PerformCurrentStage();
    });
    this.verificationTimeoutTimer.Enabled = false;
    SapiManager.GlobalInstance.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannels();
    this.UpdateConnectedEquipmentType();
    this.UpdateUserInterface();
  }

  private void UpdateChannels()
  {
    if (!(this.SetMCM(this.GetChannel("MCM21T")) | this.SetACM(this.GetChannel("ACM301T"))))
      return;
    this.UpdateWarningMessage();
    this.textBoxDPFSerialNumber1.Text = string.Empty;
  }

  private void CleanUpChannels()
  {
    this.SetMCM((Channel) null);
    this.SetACM((Channel) null);
    this.UpdateWarningMessage();
  }

  private void UpdateWarningMessage()
  {
    bool flag = false;
    if (this.IsLicenseValid)
    {
      if (this.acm != null && UserPanel.HasUnsentChanges(this.acm))
        flag = true;
      this.labelLicenseMessage.Visible = false;
    }
    else
      this.labelLicenseMessage.Visible = true;
    this.labelWarning.Visible = flag;
  }

  private static bool HasUnsentChanges(Channel channel)
  {
    bool flag = false;
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) channel.Parameters)
    {
      if (!object.Equals(parameter.Value, parameter.OriginalValue))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private bool SetMCM(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      this.StopWork(UserPanel.Reason.Disconnected);
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.targetVIN = string.Empty;
      }
      this.mcm = mcm;
      flag = true;
      if (this.mcm != null)
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    return flag;
  }

  private bool SetACM(Channel acm)
  {
    bool flag = false;
    if (this.acm != acm)
    {
      this.StopWork(UserPanel.Reason.Disconnected);
      if (this.acm != null)
      {
        this.acm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        if (this.ashRatioInstrument != null)
        {
          ((DataItem) this.ashRatioInstrument).UpdateEvent -= new EventHandler<ResultEventArgs>(this.OnAshRatioUpdate);
          this.ashRatioInstrument = (InstrumentDataItem) null;
        }
        this.targetVIN = string.Empty;
      }
      this.acm = acm;
      flag = true;
      if (this.acm != null)
      {
        this.acm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.atdType = DataItem.Create(UserPanel.ATDTypeParameter, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) as ParameterDataItem;
        this.ashRatioInstrument = DataItem.Create(UserPanel.AshRatioInstrument, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) as InstrumentDataItem;
        if (this.ashRatioInstrument != null)
        {
          Service service = this.acm.Services["RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"];
          if (service != (Service) null && service.InputValues[0].Units != ((DataItem) this.ashRatioInstrument).Units)
          {
            this.ashRatioInstrument = (InstrumentDataItem) null;
            int num = (int) MessageBox.Show(Resources.Message_AshVolumeRatioUpdateRoutineUnitsDoNotMatchInstrumentUnitsInvalidCBF);
          }
          else
            ((DataItem) this.ashRatioInstrument).UpdateEvent += new EventHandler<ResultEventArgs>(this.OnAshRatioUpdate);
        }
        this.ReadAccumulators(false);
      }
    }
    return flag;
  }

  private void OnAshRatioUpdate(object sender, ResultEventArgs args)
  {
    if (this.CurrentStage != UserPanel.Stage.WaitingToConfirmChange)
      return;
    this.PerformCurrentStage();
  }

  private void ReadAccumulators(bool synchronous)
  {
    if (this.acm == null)
      return;
    ParameterCollection parameters1 = this.acm.Parameters;
    Qualifier atdTypeParameter1 = UserPanel.ATDTypeParameter;
    string name1 = ((Qualifier) ref atdTypeParameter1).Name;
    if (parameters1[name1] != null)
    {
      ParameterCollection parameters2 = this.acm.Parameters;
      ParameterCollection parameters3 = this.acm.Parameters;
      Qualifier atdTypeParameter2 = UserPanel.ATDTypeParameter;
      string name2 = ((Qualifier) ref atdTypeParameter2).Name;
      string groupQualifier = parameters3[name2].GroupQualifier;
      int num = synchronous ? 1 : 0;
      parameters2.ReadGroup(groupQualifier, false, num != 0);
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.UpdateChannels();
    this.UpdateAccessLevels();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.StopWork(UserPanel.Reason.Closing);
    this.CleanUpChannels();
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrReturnValue,
      (object) this.textBoxProgress.Text
    };
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnReasonChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnDPFSerialNumberChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnDPFSerialNumberKeyPress(object sender, KeyPressEventArgs e)
  {
    e.KeyChar = e.KeyChar.ToString().ToUpperInvariant()[0];
    if (e.KeyChar == '\b' || (this.MdegFullScope ? (!UserPanel.MdegFullScopeSerialNumberValidation.IsMatch(e.KeyChar.ToString()) ? 1 : 0) : (!UserPanel.AnySerialNumberValidation.IsMatch(e.KeyChar.ToString()) ? 1 : 0)) == 0)
      return;
    e.Handled = true;
  }

  private AtsType AtsType
  {
    get
    {
      if (this.acm != null && this.connectedValidationInformation != null)
      {
        if (this.atdType == null)
          return this.connectedValidationInformation.OneBoxType;
        Choice originalValue = this.atdType.OriginalValue as Choice;
        if (originalValue != (object) null)
        {
          switch (Convert.ToByte(originalValue.RawValue))
          {
            case 0:
              return this.connectedValidationInformation.OneBoxType;
            case 1:
              return AtsType.TwoBox;
            default:
              return AtsType.Unknown;
          }
        }
      }
      return AtsType.Unknown;
    }
  }

  private bool ValidateSerialNumber(string text, out string errorText)
  {
    bool flag = false;
    errorText = string.Empty;
    if (this.connectedValidationInformation == null)
      errorText = Resources.Message_UnsupportedEquipment;
    else
      flag = this.connectedValidationInformation.Regex.IsMatch(text);
    return flag;
  }

  private void OnPerformAction(object sender, EventArgs e) => this.DoWork();

  public bool Online => this.IsChannelOnline(this.mcm) && this.IsChannelOnline(this.acm);

  public bool IsChannelOnline(Channel channel)
  {
    return channel != null && channel.CommunicationsState == CommunicationsState.Online;
  }

  public bool Working => this.currentStage != UserPanel.Stage.Idle;

  public bool CanClose => !this.Working;

  public bool MdegFullScope
  {
    get
    {
      string engineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
      return string.IsNullOrEmpty(engineSerialNumber) || !engineSerialNumber.StartsWith("934912C");
    }
  }

  public bool CanSetAshRatio
  {
    get
    {
      bool canSetAshRatio = false;
      if (!this.Working && this.Online)
        canSetAshRatio = (!this.radioACMReplace.Checked ? this.mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Start"] != (Service) null && this.ValidSerialNumberProvided : this.mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM"] != (Service) null) && this.ashRatioInstrument != null && this.acm.Services["RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"] != (Service) null;
      return canSetAshRatio;
    }
  }

  public bool ValidSerialNumberProvided
  {
    get
    {
      switch (this.AtsType)
      {
        case AtsType.OneBoxOneFilter:
        case AtsType.TwoBox:
          return this.ValidateSerialNumber(this.textBoxDPFSerialNumber1.Text, out string _);
        default:
          return false;
      }
    }
  }

  private bool IsLicenseValid => LicenseManager.GlobalInstance.AccessLevel >= 1;

  private void UpdateAccessLevels()
  {
    this.UpdateUserInterface();
    this.UpdateWarningMessage();
  }

  private void UpdateUserInterface()
  {
    bool flag = this.Online && !this.Working && this.IsLicenseValid && this.AtsType != AtsType.Unknown && this.connectedValidationInformation != null;
    this.radioCleanRemanFilter.Enabled = this.radioNewFilter.Enabled = flag;
    this.buttonClose.Enabled = this.CanClose;
    this.labelTaskQuestion.Enabled = flag;
    this.buttonPerformAction.Enabled = this.CanSetAshRatio && flag;
    int num;
    switch (ApplicationInformation.ProductAccessLevel)
    {
      case 1:
        this.radioACMReplace.Visible = false;
        goto label_11;
      case 3:
        num = 0;
        break;
      default:
        num = ApplicationInformation.ProductAccessLevel != 2 ? 1 : 0;
        break;
    }
    if (num == 0)
    {
      this.radioACMReplace.Enabled = flag;
      if (this.radioACMReplace.Checked)
        ((Control) this.tableLayoutDPFSerialNumber).Visible = false;
      else if (!((Control) this.tableLayoutDPFSerialNumber).Visible)
        ((Control) this.tableLayoutDPFSerialNumber).Visible = true;
    }
label_11:
    if (!((Control) this.tableLayoutDPFSerialNumber).Visible)
      return;
    this.textBoxDPFSerialNumber1.ReadOnly = !flag;
    ((Control) this.tableLayoutDPFSerialNumber).Enabled = flag;
    this.ValidateDPFSerialBox(this.textBoxDPFSerialNumber1, this.labelSNErrorMessage1);
    switch (this.AtsType)
    {
      case AtsType.OneBoxOneFilter:
      case AtsType.TwoBox:
        this.labelDPFSerialNumberHeader.Text = Resources.Message_PleaseProvideTheSerialNumberForTheAftertreatmentSystemNowInstalledOnTheVehicle;
        break;
    }
  }

  private void ValidateDPFSerialBox(TextBox box, System.Windows.Forms.Label errorMessage)
  {
    if (box.ReadOnly)
    {
      box.BackColor = SystemColors.Control;
    }
    else
    {
      string errorText;
      if (this.ValidateSerialNumber(box.Text, out errorText))
      {
        errorMessage.Text = string.Empty;
        box.BackColor = Color.LightGreen;
      }
      else
      {
        errorMessage.Text = errorText;
        box.BackColor = Color.LightPink;
      }
    }
  }

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

  private void DoWork()
  {
    this.CurrentStage = UserPanel.Stage.GetConfirmation;
    this.PerformCurrentStage();
  }

  private void PerformCurrentStage()
  {
    switch (this.CurrentStage)
    {
      case UserPanel.Stage.GetConfirmation:
        this.targetVIN = SapiManager.GetVehicleIdentificationNumber(this.acm);
        string action;
        if (this.radioACMReplace.Checked)
        {
          this.dpfSN1 = string.Empty;
          action = Resources.Message_CopiedFromMCM21T;
        }
        else
        {
          this.dpfSN1 = this.textBoxDPFSerialNumber1.Text;
          action = Resources.Message_Reset;
        }
        if (ConfirmationDialog.Show(this.targetVIN, this.AtsType, this.dpfSN1, action))
        {
          this.ClearOutput();
          this.Report(Resources.Message_AshVolumeRatioModificationStarted);
          this.Report(Resources.Message_VIN + this.targetVIN);
          if (((Control) this.tableLayoutDPFSerialNumber).Visible)
            this.Report(Resources.Message_DPFSerialNumber + this.dpfSN1);
          this.Report(Resources.Message_AshVolumeRatio + action);
          this.CurrentStage = UserPanel.Stage.GetValue;
          this.PerformCurrentStage();
          break;
        }
        this.StopWork(UserPanel.Reason.Canceled);
        break;
      case UserPanel.Stage.GetValue:
        if (this.radioACMReplace.Checked)
        {
          this.CurrentStage = UserPanel.Stage.WaitingForMCM21TRead;
          this.ExecuteService(this.mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM"], false);
          break;
        }
        this.CurrentStage = UserPanel.Stage.WriteValue;
        this.valueToWrite = ((DataItem) this.ashRatioInstrument).ValueAsString((object) 0.0);
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitingForMCM21TRead:
        this.valueToWrite = ((DataItem) this.ashRatioInstrument).ValueAsString((object) Convert.ToDouble(this.lastRunService.OutputValues[0].Value));
        this.CurrentStage = UserPanel.Stage.WriteValue;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WriteValue:
        if (this.ashRatioInstrument == null)
          this.StopWork(UserPanel.Reason.Disconnected);
        else
          this.instrumentRatioAtStart = ((DataItem) this.ashRatioInstrument).ValueAsString(((DataItem) this.ashRatioInstrument).Value);
        if (this.SetACMAshRatio(this.valueToWrite))
        {
          this.CurrentStage = UserPanel.Stage.WaitingForWrite;
          break;
        }
        this.StopWork(UserPanel.Reason.FailedWrite);
        break;
      case UserPanel.Stage.WaitingForWrite:
        this.CurrentStage = UserPanel.Stage.WaitingToConfirmChange;
        this.timeAtStart = DateTime.UtcNow;
        this.verificationTimeoutTimer.Enabled = true;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitingToConfirmChange:
        if (this.ashRatioInstrument == null)
        {
          this.StopWork(UserPanel.Reason.Disconnected);
          break;
        }
        if (!(DateTime.UtcNow - this.timeAtStart >= UserPanel.VerificationTimeoutPeriod))
          break;
        this.Report(((DataItem) this.ashRatioInstrument).ValueAsString(((DataItem) this.ashRatioInstrument).Value) != this.instrumentRatioAtStart ? Resources.Message_RatioUpdated : Resources.Message_RatioNotUpdated);
        this.CurrentStage = UserPanel.Stage.ResetMCM21T;
        this.verificationTimeoutTimer.Enabled = false;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ResetMCM21T:
        if (this.radioACMReplace.Checked)
        {
          this.CurrentStage = UserPanel.Stage.Finish;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForMCM21TReset;
        Service service = this.mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Start"];
        service.InputValues[0].Value = (object) 0;
        this.ExecuteService(service, false);
        break;
      case UserPanel.Stage.WaitingForMCM21TReset:
        this.CurrentStage = UserPanel.Stage.CommitChangesToMCM21T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.CommitChangesToMCM21T:
        this.CommitToMCM21TPermanentMemory();
        this.CurrentStage = UserPanel.Stage.WaitingForMCM21TCommit;
        break;
      case UserPanel.Stage.WaitingForMCM21TCommit:
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.Finish:
        this.StopWork(UserPanel.Reason.Success);
        break;
    }
  }

  private void CommitToMCM21TPermanentMemory()
  {
    if (this.mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
    {
      this.Report(Resources.Message_CommittingChanges);
      this.mcm.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnCommitCompleteEvent);
      this.mcm.Services.Execute(this.mcm.Ecu.Properties["CommitToPermanentMemoryService"], false);
    }
    else
    {
      this.Report(Resources.Message_NoCommitServiceAvailable);
      this.StopWork(UserPanel.Reason.FailedCommit);
    }
  }

  private void OnCommitCompleteEvent(object sender, ResultEventArgs e)
  {
    this.mcm.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnCommitCompleteEvent);
    if (e.Succeeded)
      this.PerformCurrentStage();
    else
      this.StopWork(UserPanel.Reason.FailedCommit);
  }

  private void StopWork(UserPanel.Reason reason)
  {
    this.verificationTimeoutTimer.Enabled = false;
    if (this.CurrentStage != UserPanel.Stage.Stopping && this.CurrentStage != UserPanel.Stage.Idle)
    {
      UserPanel.Stage currentStage = this.CurrentStage;
      this.CurrentStage = UserPanel.Stage.Stopping;
      if (reason == UserPanel.Reason.Success)
      {
        this.AddStationLogEntry(this.dpfSN1);
        this.Report(Resources.Message_TheProcedureCompletedSuccessfully);
        this.adrReturnValue = true;
      }
      else
      {
        this.adrReturnValue = false;
        this.Report(Resources.Message_TheProcedureFailedToComplete);
        switch (reason - 1)
        {
          case UserPanel.Reason.Success:
            if (currentStage == UserPanel.Stage.ResetMCM21T)
            {
              this.Report(Resources.Message_FailedToObtainServiceToResetMCM21T);
              break;
            }
            if (currentStage == UserPanel.Stage.GetValue)
            {
              this.Report(Resources.Message_FailedToObtainServiceForRetrievingMCM21TValue);
              break;
            }
            break;
          case UserPanel.Reason.FailedServiceExecute:
            if (currentStage == UserPanel.Stage.WaitingForMCM21TRead)
            {
              this.Report(Resources.Message_FailedToExecuteReadOfAshAccumulationDistanceFromMCM21T);
              break;
            }
            this.Report(Resources.Message_FailedToExecuteResetOfAshAccumulationDistanceInMCM21T);
            break;
          case UserPanel.Reason.FailedService:
            this.Report(Resources.Message_FailedToWriteTheAshMileageAccumulator);
            break;
          case UserPanel.Reason.FailedWrite:
            this.Report(Resources.Message_FailedToCommitTheChangesToTheMCM21TYouMayNeedToRepeatThisProcedure);
            break;
          case UserPanel.Reason.Closing:
            this.Report(Resources.Message_OneOrMoreDevicesDisconnected);
            this.textBoxDPFSerialNumber1.Text = string.Empty;
            break;
          case UserPanel.Reason.Disconnected:
            this.Report(Resources.Message_TheUserCanceledTheOperation);
            break;
        }
      }
      this.ClearCurrentService(false);
      this.valueToWrite = string.Empty;
      this.oldValue = (object) null;
      this.CurrentStage = UserPanel.Stage.Idle;
      this.ReadAccumulators(false);
    }
    this.UpdateWarningMessage();
  }

  private void AddStationLogEntry(string serialNumber1)
  {
    string empty = string.Empty;
    string str1;
    if (this.radioCleanRemanFilter.Checked)
      str1 = Resources.Message_ReasonCleanRemanFilter;
    else if (this.radioNewFilter.Checked)
    {
      str1 = Resources.Message_ReasonNewFilter;
    }
    else
    {
      if (!this.radioACMReplace.Checked)
        throw new InvalidOperationException();
      str1 = Resources.Message_ReasonACMReplacement;
    }
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    dictionary1["reasontext"] = str1;
    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
    int int32;
    if (this.oldValue != null)
    {
      Dictionary<string, string> dictionary3 = dictionary2;
      int32 = Convert.ToInt32(((DataItem) this.ashRatioInstrument).ValueAsDouble(this.oldValue));
      string str2 = int32.ToString();
      dictionary3["Old_AshRatio"] = str2;
    }
    Dictionary<string, string> dictionary4 = dictionary2;
    int32 = Convert.ToInt32(((DataItem) this.ashRatioInstrument).ValueAsDouble(((DataItem) this.ashRatioInstrument).Value));
    string str3 = int32.ToString();
    dictionary4["Current_AshRatio"] = str3;
    dictionary2["DPF_Serial_Number1"] = string.IsNullOrEmpty(serialNumber1) ? Resources.Message_NA : serialNumber1;
    dictionary2["CO_Odometer"] = this.ReadEcuInfoData(this.GetChannel("CPC04T", (CustomPanel.ChannelLookupOptions) 5), "CO_Odometer");
    dictionary2["DT_AS045_Engine_Operating_Hours"] = this.ReadInstrumentValue(this.mcm, "DT_AS045_Engine_Operating_Hours");
    ServerDataManager.UpdateEventsFile(this.acm, (IDictionary<string, string>) dictionary1, (IDictionary<string, string>) dictionary2, "DPFAshAccumulation", string.Empty, this.targetVIN, "OK", "DESCRIPTION", string.Empty);
  }

  private string ReadInstrumentValue(Channel channel, string qualifier)
  {
    string str = string.Empty;
    if (channel != null)
    {
      Instrument instrument = channel.Instruments[qualifier];
      if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
        str = instrument.InstrumentValues.Current.Value.ToString().Trim();
    }
    return str;
  }

  private string ReadEcuInfoData(Channel channel, string qualifier)
  {
    string str = string.Empty;
    if (channel != null)
    {
      EcuInfo ecuInfo = channel.EcuInfos[qualifier] ?? channel.EcuInfos.GetItemContaining(qualifier);
      if (ecuInfo != null)
        str = ecuInfo.Value.ToString().Trim();
    }
    return str.Trim();
  }

  private void ExecuteService(Service service, bool synchronous)
  {
    if (service == (Service) null)
    {
      this.StopWork(UserPanel.Reason.FailedServiceExecute);
    }
    else
    {
      if (!synchronous)
      {
        this.currentService = service;
        this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      }
      this.Report($"Executing {this.currentService.Name}...");
      service.Execute(synchronous);
    }
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.ClearCurrentService(true);
    if (this.CheckCompleteResult(e, Resources.Message_ServiceExecuted, Resources.Message_ServiceError))
      this.PerformCurrentStage();
    else
      this.StopWork(UserPanel.Reason.FailedService);
  }

  private bool CheckCompleteResult(ResultEventArgs e, string successText, string errorText)
  {
    bool flag = false;
    StringBuilder stringBuilder = new StringBuilder("    ");
    if (e.Succeeded)
    {
      flag = true;
      stringBuilder.Append(successText);
      if (e.Exception != null)
        stringBuilder.AppendFormat(" ({0})", (object) e.Exception.Message);
    }
    else
    {
      stringBuilder.Append(errorText);
      if (e.Exception != null)
        stringBuilder.AppendFormat(": {0}", (object) e.Exception.Message);
      else
        stringBuilder.Append(Resources.Message_Unknown);
    }
    this.Report(stringBuilder.ToString());
    return flag;
  }

  private void ClearCurrentService(bool saveLastRun)
  {
    if (!(this.currentService != (Service) null))
      return;
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.lastRunService = !saveLastRun ? (Service) null : this.currentService;
    this.currentService = (Service) null;
  }

  private bool SetACMAshRatio(string value)
  {
    bool flag = false;
    this.CurrentStage = UserPanel.Stage.WaitingForWrite;
    Service service = this.acm.Services["RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"];
    if (service != (Service) null)
    {
      this.oldValue = ((DataItem) this.ashRatioInstrument).Value;
      Cursor.Current = Cursors.WaitCursor;
      this.Report("Updating ash volume ratio...");
      service.InputValues[0].Value = (object) ((DataItem) this.ashRatioInstrument).UnscaledValueAsDouble((object) value);
      this.ExecuteService(service, false);
      flag = true;
    }
    else
      this.Report(Resources.Message_FailedToObtainACMAshDistanceAccumulator);
    return flag;
  }

  private void ClearOutput() => this.textBoxProgress.Text = string.Empty;

  private void Report(string text)
  {
    if (this.textBoxProgress != null)
    {
      TextBox textBoxProgress = this.textBoxProgress;
      textBoxProgress.Text = $"{textBoxProgress.Text}{text}\r\n";
      this.textBoxProgress.SelectionStart = this.textBoxProgress.TextLength;
      this.textBoxProgress.SelectionLength = 0;
      this.textBoxProgress.ScrollToCaret();
    }
    this.AddStatusMessage(text);
  }

  private void UpdateConnectedEquipmentType()
  {
    EquipmentType equipmentType = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault<EquipmentType>((Func<EquipmentType, bool>) (et =>
    {
      ElectronicsFamily family = ((EquipmentType) ref et).Family;
      return ((ElectronicsFamily) ref family).Category == "Engine";
    }));
    if (!EquipmentType.op_Inequality(equipmentType, EquipmentType.Empty) || this.serialNumberValidations.TryGetValue(((EquipmentType) ref equipmentType).Name, out this.connectedValidationInformation))
      return;
    this.connectedValidationInformation = (UserPanel.ValidationInformation) null;
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    if (!(e.Category == "Engine"))
      return;
    this.UpdateConnectedEquipmentType();
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutDPFSerialNumber = new TableLayoutPanel();
    this.labelDPFSerialNumberHeader = new System.Windows.Forms.Label();
    this.textBoxDPFSerialNumber1 = new TextBox();
    this.labelSNErrorMessage1 = new System.Windows.Forms.Label();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.labelLicenseMessage = new System.Windows.Forms.Label();
    this.labelWarning = new System.Windows.Forms.Label();
    this.titleLabel = new ScalingLabel();
    this.cpcReadout = new DigitalReadoutInstrument();
    this.acmReadout = new DigitalReadoutInstrument();
    this.labelMCM = new System.Windows.Forms.Label();
    this.labelACM = new System.Windows.Forms.Label();
    this.labelTaskQuestion = new System.Windows.Forms.Label();
    this.radioCleanRemanFilter = new RadioButton();
    this.radioNewFilter = new RadioButton();
    this.radioACMReplace = new RadioButton();
    this.buttonPerformAction = new Button();
    this.labelProgress = new System.Windows.Forms.Label();
    this.textBoxProgress = new TextBox();
    this.buttonClose = new Button();
    ((Control) this.tableLayoutDPFSerialNumber).SuspendLayout();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    this.flowLayoutPanel1.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutDPFSerialNumber, "tableLayoutDPFSerialNumber");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutDPFSerialNumber, 2);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.labelDPFSerialNumberHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.textBoxDPFSerialNumber1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.labelSNErrorMessage1, 2, 1);
    ((Control) this.tableLayoutDPFSerialNumber).Name = "tableLayoutDPFSerialNumber";
    componentResourceManager.ApplyResources((object) this.labelDPFSerialNumberHeader, "labelDPFSerialNumberHeader");
    this.labelDPFSerialNumberHeader.BackColor = SystemColors.ControlDark;
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).SetColumnSpan((Control) this.labelDPFSerialNumberHeader, 3);
    this.labelDPFSerialNumberHeader.Name = "labelDPFSerialNumberHeader";
    this.labelDPFSerialNumberHeader.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxDPFSerialNumber1, "textBoxDPFSerialNumber1");
    this.textBoxDPFSerialNumber1.Name = "textBoxDPFSerialNumber1";
    componentResourceManager.ApplyResources((object) this.labelSNErrorMessage1, "labelSNErrorMessage1");
    this.labelSNErrorMessage1.ForeColor = Color.Red;
    this.labelSNErrorMessage1.Name = "labelSNErrorMessage1";
    this.labelSNErrorMessage1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.flowLayoutPanel1, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.titleLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.cpcReadout, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.acmReadout, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelMCM, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelACM, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelTaskQuestion, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioCleanRemanFilter, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioNewFilter, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioACMReplace, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutDPFSerialNumber, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonPerformAction, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelProgress, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.textBoxProgress, 0, 10);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonClose, 1, 11);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    this.flowLayoutPanel1.Controls.Add((Control) this.labelLicenseMessage);
    this.flowLayoutPanel1.Controls.Add((Control) this.labelWarning);
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelLicenseMessage, "labelLicenseMessage");
    this.labelLicenseMessage.BackColor = SystemColors.Control;
    this.labelLicenseMessage.ForeColor = Color.Red;
    this.labelLicenseMessage.Name = "labelLicenseMessage";
    this.labelLicenseMessage.UseCompatibleTextRendering = true;
    this.labelLicenseMessage.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.labelWarning, "labelWarning");
    this.labelWarning.BackColor = SystemColors.Control;
    this.labelWarning.ForeColor = Color.Red;
    this.labelWarning.Name = "labelWarning";
    this.labelWarning.UseCompatibleTextRendering = true;
    this.labelWarning.UseMnemonic = false;
    this.titleLabel.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.titleLabel, 2);
    componentResourceManager.ApplyResources((object) this.titleLabel, "titleLabel");
    this.titleLabel.FontGroup = (string) null;
    this.titleLabel.LineAlignment = StringAlignment.Center;
    ((Control) this.titleLabel).Name = "titleLabel";
    ((Control) this.titleLabel).TabStop = false;
    componentResourceManager.ApplyResources((object) this.cpcReadout, "cpcReadout");
    this.cpcReadout.FontGroup = (string) null;
    ((SingleInstrumentBase) this.cpcReadout).FreezeValue = false;
    ((SingleInstrumentBase) this.cpcReadout).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS237_DPF_Ash_volume_from_ACM");
    ((Control) this.cpcReadout).Name = "cpcReadout";
    ((Control) this.cpcReadout).TabStop = false;
    ((SingleInstrumentBase) this.cpcReadout).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.acmReadout, "acmReadout");
    this.acmReadout.FontGroup = (string) null;
    ((SingleInstrumentBase) this.acmReadout).FreezeValue = false;
    ((SingleInstrumentBase) this.acmReadout).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS109_Ash_Filter_Full_Volume");
    ((Control) this.acmReadout).Name = "acmReadout";
    ((SingleInstrumentBase) this.acmReadout).UnitAlignment = StringAlignment.Near;
    this.labelMCM.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.labelMCM, "labelMCM");
    this.labelMCM.BackColor = SystemColors.ControlDark;
    this.labelMCM.CausesValidation = false;
    this.labelMCM.ForeColor = SystemColors.ControlText;
    this.labelMCM.Name = "labelMCM";
    this.labelMCM.UseCompatibleTextRendering = true;
    this.labelACM.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.labelACM, "labelACM");
    this.labelACM.BackColor = SystemColors.ControlDark;
    this.labelACM.CausesValidation = false;
    this.labelACM.ForeColor = SystemColors.ControlText;
    this.labelACM.Name = "labelACM";
    this.labelACM.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelTaskQuestion, "labelTaskQuestion");
    this.labelTaskQuestion.BackColor = SystemColors.ControlDark;
    this.labelTaskQuestion.CausesValidation = false;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.labelTaskQuestion, 2);
    this.labelTaskQuestion.Name = "labelTaskQuestion";
    this.labelTaskQuestion.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.radioCleanRemanFilter, "radioCleanRemanFilter");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.radioCleanRemanFilter, 2);
    this.radioCleanRemanFilter.Name = "radioCleanRemanFilter";
    this.radioCleanRemanFilter.TabStop = true;
    this.radioCleanRemanFilter.UseCompatibleTextRendering = true;
    this.radioCleanRemanFilter.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.radioNewFilter, "radioNewFilter");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.radioNewFilter, 2);
    this.radioNewFilter.Name = "radioNewFilter";
    this.radioNewFilter.TabStop = true;
    this.radioNewFilter.UseCompatibleTextRendering = true;
    this.radioNewFilter.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.radioACMReplace, "radioACMReplace");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.radioACMReplace, 2);
    this.radioACMReplace.Name = "radioACMReplace";
    this.radioACMReplace.TabStop = true;
    this.radioACMReplace.UseCompatibleTextRendering = true;
    this.radioACMReplace.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonPerformAction, "buttonPerformAction");
    this.buttonPerformAction.Name = "buttonPerformAction";
    this.buttonPerformAction.UseCompatibleTextRendering = true;
    this.buttonPerformAction.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelProgress, "labelProgress");
    this.labelProgress.BackColor = SystemColors.ControlDark;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.labelProgress, 2);
    this.labelProgress.Name = "labelProgress";
    this.labelProgress.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.textBoxProgress, 2);
    componentResourceManager.ApplyResources((object) this.textBoxProgress, "textBoxProgress");
    this.textBoxProgress.Name = "textBoxProgress";
    this.textBoxProgress.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DPFAshAccumulator_EPA10");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutDPFSerialNumber).ResumeLayout(false);
    ((Control) this.tableLayoutDPFSerialNumber).PerformLayout();
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private class ValidationInformation
  {
    public readonly AtsType OneBoxType;
    public readonly Regex Regex;

    public ValidationInformation(AtsType oneBoxType, Regex regex)
    {
      this.OneBoxType = oneBoxType;
      this.Regex = regex;
    }
  }

  private enum Stage
  {
    Idle = 0,
    GetConfirmation = 1,
    _Start = 1,
    GetValue = 2,
    WaitingForMCM21TRead = 3,
    WriteValue = 4,
    WaitingForWrite = 5,
    WaitingToConfirmChange = 6,
    WaitingForMCM21TInputToUpdate = 7,
    ResetMCM21T = 8,
    WaitingForMCM21TReset = 9,
    CommitChangesToMCM21T = 10, // 0x0000000A
    WaitingForMCM21TCommit = 11, // 0x0000000B
    Finish = 12, // 0x0000000C
    Stopping = 13, // 0x0000000D
  }

  private enum Reason
  {
    Success,
    FailedServiceExecute,
    FailedService,
    FailedWrite,
    FailedCommit,
    Closing,
    Disconnected,
    Canceled,
  }
}
