// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private const string ResetRatioService = "RT_DPF_Ash_Volume_Reset_Start";
  private const string ReadRatioService = "RT_DPF_Ash_Volume_Read_Request_Results_Status";
  private const string ACMAshVolumeRatioUpdateStart = "RT_Ash_Volume_Ratio_Update_Start";
  private const string QualifierOdometer = "CO_Odometer";
  private const string QualifierEngineHours = "DT_AS045_Engine_Operating_Hours";
  private static readonly UserPanel.ValidationInformation HeavyDutySerialNumberValidation = new UserPanel.ValidationInformation(new Regex("(R124\\d{11}|124R\\d{11}|124\\d{11})", RegexOptions.Compiled), Resources.Message_SerialNumberShouldBeOfTheFormat124Xxxxxxxxxxx);
  private Dictionary<string, UserPanel.ValidationInformation> serialNumberValidations = new Dictionary<string, UserPanel.ValidationInformation>();
  private Channel acm;
  private Channel cpc2;
  private static readonly Qualifier ATDTypeParameter = new Qualifier((QualifierTypes) 4, "ACM02T", "ATD_Hardware_Type");
  private ParameterDataItem atdType;
  private static readonly Qualifier AshRatioInstrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS109_Ash_Filter_Full_Volume");
  private InstrumentDataItem ashRatioInstrument;
  private string targetVIN;
  private bool adrReturnValue = false;
  private static readonly Regex AnySerialNumberValidation = new Regex("[R\\d]", RegexOptions.Compiled);
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private object oldValue;
  private string valueToWrite;
  private string dpfSN1;
  private string dpfSN2;
  private string instrumentRatioAtStart;
  private DateTime timeAtStart;
  private Timer verificationTimeoutTimer = new Timer();
  private static readonly TimeSpan VerificationTimeoutPeriod = new TimeSpan(0, 0, 10);
  private Service currentService;
  private Service lastRunService;
  private ScalingLabel titleLabel;
  private DigitalReadoutInstrument cpcReadout;
  private DigitalReadoutInstrument acmReadout;
  private System.Windows.Forms.Label labelCPC2;
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
  private TextBox textBoxDPFSerialNumber2;
  private System.Windows.Forms.Label labelSNErrorMessage2;
  private TableLayoutPanel tableLayoutPanel;

  public UserPanel()
  {
    this.InitializeComponent();
    this.serialNumberValidations.Add("DD13", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD15", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD16", UserPanel.HeavyDutySerialNumberValidation);
    this.radioCleanRemanFilter.Checked = true;
    this.radioCleanRemanFilter.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioNewFilter.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioACMReplace.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.textBoxDPFSerialNumber1.TextChanged += new EventHandler(this.OnDPFSerialNumberChanged);
    this.textBoxDPFSerialNumber1.KeyPress += new KeyPressEventHandler(this.OnDPFSerialNumberKeyPress);
    this.textBoxDPFSerialNumber2.TextChanged += new EventHandler(this.OnDPFSerialNumberChanged);
    this.textBoxDPFSerialNumber2.KeyPress += new KeyPressEventHandler(this.OnDPFSerialNumberKeyPress);
    this.buttonPerformAction.Click += new EventHandler(this.OnPerformAction);
    this.verificationTimeoutTimer.Interval = (int) (UserPanel.VerificationTimeoutPeriod.TotalMilliseconds / 2.0);
    this.verificationTimeoutTimer.Tick += (EventHandler) ((sender, args) =>
    {
      if (this.CurrentStage != UserPanel.Stage.WaitingToConfirmChange)
        return;
      this.PerformCurrentStage();
    });
    this.verificationTimeoutTimer.Enabled = false;
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannels();
    this.UpdateUserInterface();
  }

  private void UpdateChannels()
  {
    if (!(this.SetCPC2(this.GetChannel("CPC02T")) | this.SetACM(this.GetChannel("ACM02T"))))
      return;
    this.UpdateWarningMessage();
    this.textBoxDPFSerialNumber1.Text = string.Empty;
    this.textBoxDPFSerialNumber2.Text = string.Empty;
  }

  private void CleanUpChannels()
  {
    this.SetCPC2((Channel) null);
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

  private bool SetCPC2(Channel cpc2)
  {
    bool flag = false;
    if (this.cpc2 != cpc2)
    {
      this.StopWork(UserPanel.Reason.Disconnected);
      if (this.cpc2 != null)
      {
        this.cpc2.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.targetVIN = string.Empty;
      }
      this.cpc2 = cpc2;
      flag = true;
      if (this.cpc2 != null)
        this.cpc2.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
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
          Service service = this.acm.Services["RT_Ash_Volume_Ratio_Update_Start"];
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
    if (e.KeyChar == '\b' || UserPanel.AnySerialNumberValidation.IsMatch(e.KeyChar.ToString()))
      return;
    e.Handled = true;
  }

  private AtsType AtsType
  {
    get
    {
      if (this.acm != null && this.atdType != null)
      {
        Choice originalValue = this.atdType.OriginalValue as Choice;
        if (originalValue != (object) null)
        {
          switch (Convert.ToByte(originalValue.RawValue))
          {
            case 0:
              return AtsType.OneBox;
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
    UserPanel.ValidationInformation validationInformation = this.GetValidationInformation();
    if (validationInformation == null)
      errorText = Resources.Message_UnsupportedEquipment;
    else if (validationInformation.Regex.IsMatch(text))
    {
      flag = true;
    }
    else
    {
      errorText = validationInformation.ErrorMessage;
      flag = false;
    }
    return flag;
  }

  private string GetEngineTypeName()
  {
    IEnumerable<EquipmentType> source = EquipmentType.ConnectedEquipmentTypes("Engine");
    if (!CollectionExtensions.Exactly<EquipmentType>(source, 1))
      return (string) null;
    EquipmentType equipmentType = source.First<EquipmentType>();
    return ((EquipmentType) ref equipmentType).Name;
  }

  private UserPanel.ValidationInformation GetValidationInformation()
  {
    string engineTypeName = this.GetEngineTypeName();
    UserPanel.ValidationInformation validationInformation;
    return string.IsNullOrEmpty(engineTypeName) || !this.serialNumberValidations.TryGetValue(engineTypeName, out validationInformation) ? (UserPanel.ValidationInformation) null : validationInformation;
  }

  private void OnPerformAction(object sender, EventArgs e) => this.DoWork();

  public bool Online => this.IsChannelOnline(this.cpc2) && this.IsChannelOnline(this.acm);

  public bool IsChannelOnline(Channel channel)
  {
    return channel != null && channel.CommunicationsState == CommunicationsState.Online;
  }

  public bool Working => this.currentStage != UserPanel.Stage.Idle;

  public bool CanClose => !this.Working;

  public bool CanSetAshRatio
  {
    get
    {
      bool canSetAshRatio = false;
      if (!this.Working && this.Online)
        canSetAshRatio = (!this.radioACMReplace.Checked ? this.cpc2.Services["RT_DPF_Ash_Volume_Reset_Start"] != (Service) null && this.ValidSerialNumberProvided : this.cpc2.Services["RT_DPF_Ash_Volume_Read_Request_Results_Status"] != (Service) null) && this.ashRatioInstrument != null && this.acm.Services["RT_Ash_Volume_Ratio_Update_Start"] != (Service) null;
      return canSetAshRatio;
    }
  }

  public bool ValidSerialNumberProvided
  {
    get
    {
      switch (this.AtsType)
      {
        case AtsType.OneBox:
          string errorText;
          return this.ValidateSerialNumber(this.textBoxDPFSerialNumber1.Text, out errorText) && this.ValidateSerialNumber(this.textBoxDPFSerialNumber2.Text, out errorText);
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
    bool flag = this.Online && !this.Working && this.IsLicenseValid && this.AtsType != AtsType.Unknown && this.GetValidationInformation() != null;
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
    this.textBoxDPFSerialNumber2.ReadOnly = !flag;
    ((Control) this.tableLayoutDPFSerialNumber).Enabled = flag;
    this.ValidateDPFSerialBox(this.textBoxDPFSerialNumber1, this.labelSNErrorMessage1);
    switch (this.AtsType)
    {
      case AtsType.OneBox:
        this.textBoxDPFSerialNumber2.Visible = this.labelSNErrorMessage2.Visible = true;
        this.ValidateDPFSerialBox(this.textBoxDPFSerialNumber2, this.labelSNErrorMessage2);
        this.labelDPFSerialNumberHeader.Text = Resources.Message_PleaseProvideTheTwoSerialNumbersForTheAftertreatmentSystemNowInstalledOnTheVehicle;
        break;
      case AtsType.TwoBox:
        this.textBoxDPFSerialNumber2.Visible = this.labelSNErrorMessage2.Visible = false;
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
          this.dpfSN1 = this.dpfSN2 = string.Empty;
          action = Resources.Message_CopiedFromCPC2;
        }
        else
        {
          this.dpfSN1 = this.textBoxDPFSerialNumber1.Text;
          this.dpfSN2 = this.textBoxDPFSerialNumber2.Text;
          action = Resources.Message_Reset;
        }
        if (ConfirmationDialog.Show(this.targetVIN, this.AtsType, this.dpfSN1, this.dpfSN2, action))
        {
          this.ClearOutput();
          this.Report(Resources.Message_AshVolumeRatioModificationStarted);
          this.Report(Resources.Message_VIN + this.targetVIN);
          if (((Control) this.tableLayoutDPFSerialNumber).Visible)
            this.Report(Resources.Message_DPFSerialNumbers + this.dpfSN1 + (this.AtsType == AtsType.OneBox ? ", " + this.dpfSN2 : string.Empty));
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
          this.CurrentStage = UserPanel.Stage.WaitingForCPC2Read;
          this.ExecuteService(this.cpc2.Services["RT_DPF_Ash_Volume_Read_Request_Results_Status"], false);
          break;
        }
        this.CurrentStage = UserPanel.Stage.WriteValue;
        this.valueToWrite = ((DataItem) this.ashRatioInstrument).ValueAsString((object) 0.0);
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitingForCPC2Read:
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
        if (((DataItem) this.ashRatioInstrument).ValueAsString(((DataItem) this.ashRatioInstrument).Value) != this.instrumentRatioAtStart)
        {
          this.Report(Resources.Message_RatioUpdated);
          this.CurrentStage = UserPanel.Stage.ResetCPC2;
          this.verificationTimeoutTimer.Enabled = false;
          this.PerformCurrentStage();
          break;
        }
        if (!(DateTime.UtcNow - this.timeAtStart >= UserPanel.VerificationTimeoutPeriod))
          break;
        this.Report(Resources.Message_RatioNotUpdated);
        this.CurrentStage = UserPanel.Stage.ResetCPC2;
        this.verificationTimeoutTimer.Enabled = false;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ResetCPC2:
        if (this.radioACMReplace.Checked)
        {
          this.CurrentStage = UserPanel.Stage.Finish;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForCPC2Reset;
        this.ExecuteService(this.cpc2.Services["RT_DPF_Ash_Volume_Reset_Start"], false);
        break;
      case UserPanel.Stage.WaitingForCPC2Reset:
        this.CurrentStage = UserPanel.Stage.CommitChangesToCPC2;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.CommitChangesToCPC2:
        this.CommitToCPC2PermanentMemory();
        this.CurrentStage = UserPanel.Stage.WaitingForCPC2Commit;
        break;
      case UserPanel.Stage.WaitingForCPC2Commit:
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.Finish:
        this.StopWork(UserPanel.Reason.Success);
        break;
    }
  }

  private void CommitToCPC2PermanentMemory()
  {
    if (this.cpc2.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
    {
      this.Report(Resources.Message_CommittingChanges);
      this.cpc2.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnCommitCompleteEvent);
      this.cpc2.Services.Execute(this.cpc2.Ecu.Properties["CommitToPermanentMemoryService"], false);
    }
    else
    {
      this.Report(Resources.Message_NoCommitServiceAvailable);
      this.StopWork(UserPanel.Reason.FailedCommit);
    }
  }

  private void OnCommitCompleteEvent(object sender, ResultEventArgs e)
  {
    this.cpc2.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnCommitCompleteEvent);
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
        this.AddStationLogEntry(this.dpfSN1, this.dpfSN2);
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
            if (currentStage == UserPanel.Stage.ResetCPC2)
            {
              this.Report(Resources.Message_FailedToObtainServiceToResetCPC2);
              break;
            }
            if (currentStage == UserPanel.Stage.GetValue)
            {
              this.Report(Resources.Message_FailedToObtainServiceForRetrievingCPC2Value);
              break;
            }
            break;
          case UserPanel.Reason.FailedServiceExecute:
            if (currentStage == UserPanel.Stage.WaitingForCPC2Read)
            {
              this.Report(Resources.Message_FailedToExecuteReadOfAshAccumulationDistanceFromCPC2);
              break;
            }
            this.Report(Resources.Message_FailedToExecuteResetOfAshAccumulationDistanceInCPC2);
            break;
          case UserPanel.Reason.FailedService:
            this.Report(Resources.Message_FailedToWriteTheAshMileageAccumulator);
            break;
          case UserPanel.Reason.FailedWrite:
            this.Report(Resources.Message_FailedToCommitTheChangesToTheCPC2YouMayNeedToRepeatThisProcedure);
            break;
          case UserPanel.Reason.Closing:
            this.Report(Resources.Message_OneOrMoreDevicesDisconnected);
            this.textBoxDPFSerialNumber1.Text = string.Empty;
            this.textBoxDPFSerialNumber2.Text = string.Empty;
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

  private void AddStationLogEntry(string serialNumber1, string serialNumber2)
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
    dictionary2["DPF_Serial_Number2"] = string.IsNullOrEmpty(serialNumber2) ? Resources.Message_NA : serialNumber2;
    dictionary2["CO_Odometer"] = this.ReadEcuInfoData(this.cpc2, "CO_Odometer");
    dictionary2["DT_AS045_Engine_Operating_Hours"] = this.ReadInstrumentValue(this.GetChannel("MCM02T"), "DT_AS045_Engine_Operating_Hours");
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
      this.Report($"{Resources.Message_Executing}{this.currentService.Name}...");
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
    Service service = this.acm.Services["RT_Ash_Volume_Ratio_Update_Start"];
    if (service != (Service) null)
    {
      this.oldValue = ((DataItem) this.ashRatioInstrument).Value;
      Cursor.Current = Cursors.WaitCursor;
      this.Report(Resources.Message_UpdatingAshVolumeRatio);
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

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutDPFSerialNumber = new TableLayoutPanel();
    this.labelDPFSerialNumberHeader = new System.Windows.Forms.Label();
    this.textBoxDPFSerialNumber1 = new TextBox();
    this.labelSNErrorMessage1 = new System.Windows.Forms.Label();
    this.textBoxDPFSerialNumber2 = new TextBox();
    this.labelSNErrorMessage2 = new System.Windows.Forms.Label();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.labelLicenseMessage = new System.Windows.Forms.Label();
    this.labelWarning = new System.Windows.Forms.Label();
    this.titleLabel = new ScalingLabel();
    this.cpcReadout = new DigitalReadoutInstrument();
    this.acmReadout = new DigitalReadoutInstrument();
    this.labelCPC2 = new System.Windows.Forms.Label();
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
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.textBoxDPFSerialNumber2, 1, 2);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.labelSNErrorMessage2, 2, 2);
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
    componentResourceManager.ApplyResources((object) this.textBoxDPFSerialNumber2, "textBoxDPFSerialNumber2");
    this.textBoxDPFSerialNumber2.Name = "textBoxDPFSerialNumber2";
    componentResourceManager.ApplyResources((object) this.labelSNErrorMessage2, "labelSNErrorMessage2");
    this.labelSNErrorMessage2.ForeColor = Color.Red;
    this.labelSNErrorMessage2.Name = "labelSNErrorMessage2";
    this.labelSNErrorMessage2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.flowLayoutPanel1, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.titleLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.cpcReadout, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.acmReadout, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelCPC2, 0, 1);
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
    ((SingleInstrumentBase) this.cpcReadout).Instrument = new Qualifier((QualifierTypes) 1, "CPC02T", "DT_AS052_DPF_Ash_Volume");
    ((Control) this.cpcReadout).Name = "cpcReadout";
    ((Control) this.cpcReadout).TabStop = false;
    ((SingleInstrumentBase) this.cpcReadout).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.acmReadout, "acmReadout");
    this.acmReadout.FontGroup = (string) null;
    ((SingleInstrumentBase) this.acmReadout).FreezeValue = false;
    ((SingleInstrumentBase) this.acmReadout).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS109_Ash_Filter_Full_Volume");
    ((Control) this.acmReadout).Name = "acmReadout";
    ((SingleInstrumentBase) this.acmReadout).UnitAlignment = StringAlignment.Near;
    this.labelCPC2.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.labelCPC2, "labelCPC2");
    this.labelCPC2.BackColor = SystemColors.ControlDark;
    this.labelCPC2.CausesValidation = false;
    this.labelCPC2.ForeColor = SystemColors.ControlText;
    this.labelCPC2.Name = "labelCPC2";
    this.labelCPC2.UseCompatibleTextRendering = true;
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
    public readonly Regex Regex;
    public readonly string ErrorMessage;

    public ValidationInformation(Regex regex, string errorMessage)
    {
      this.Regex = regex;
      this.ErrorMessage = errorMessage;
    }
  }

  private enum Stage
  {
    Idle = 0,
    GetConfirmation = 1,
    _Start = 1,
    GetValue = 2,
    WaitingForCPC2Read = 3,
    WriteValue = 4,
    WaitingForWrite = 5,
    WaitingToConfirmChange = 6,
    ResetCPC2 = 7,
    WaitingForCPC2Reset = 8,
    CommitChangesToCPC2 = 9,
    WaitingForCPC2Commit = 10, // 0x0000000A
    Finish = 11, // 0x0000000B
    Stopping = 12, // 0x0000000C
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
