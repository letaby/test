// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation.panel;

public class UserPanel : CustomPanel
{
  private const string ResetMileageService = "RT_DPF_Ash_Mileage_Reset_Start";
  private const string ReadMileageService = "RT_DPF_Ash_Mileage_Read_Request_Results_Status";
  private const string ResetMaximaService = "RT_SR014_SET_EOL_Default_Values_Start";
  private const string QualifierOdometer = "CO_Odometer";
  private const string QualifierEngineHours = "DT_AS045_Engine_Operating_Hours";
  private const double ReducedDpfAshMileageValueBy = 241402.0;
  private static readonly UserPanel.ValidationInformation HeavyDutySerialNumberValidation = new UserPanel.ValidationInformation(new Regex("S[HL]M\\d{6}|124\\d{11}", RegexOptions.Compiled), Resources.Message_SerialNumberShouldBeOfTheFormatSHMxxxxxxSLMxxxxxxOr124XxxxxxxxxxxAustralia);
  private static readonly UserPanel.ValidationInformation MediumDutySerialNumberValidation = new UserPanel.ValidationInformation(new Regex("SM[LM]\\d{6}", RegexOptions.Compiled), Resources.Message_SerialNumberShouldBeOfTheFormatSMLxxxxxxOrSMMxxxxxx);
  private Dictionary<string, UserPanel.ValidationInformation> serialNumberValidations = new Dictionary<string, UserPanel.ValidationInformation>();
  private Channel mcm;
  private Channel cpc2;
  private static readonly Qualifier AshMileageParameter = new Qualifier((QualifierTypes) 4, "MCM", "e2p_dpf_ash_last_clean_dist");
  private ParameterDataItem ashMileage;
  private static readonly Qualifier DistanceTillAshFullInstrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS078_Distance_till_Ash_Full");
  private InstrumentDataItem distanceTillAshFull;
  private string targetVIN;
  private string targetESN;
  private bool adrReturnValue = false;
  private static readonly Regex AnySerialNumberValidation = new Regex("[SHLM\\d]", RegexOptions.Compiled);
  private static readonly Regex DistanceValidation = new Regex("\\d{1,6}", RegexOptions.Compiled);
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private object oldDistanceValue;
  private object newDistanceValue;
  private string distanceValueToWrite;
  private string dpfSN;
  private Service currentService;
  private Service lastRunService;
  private RadioButton radioMCMReplace;
  private TableLayoutPanel tableLayoutPanel;
  private ScalingLabel titleLabel;
  private DigitalReadoutInstrument cpcReadout;
  private System.Windows.Forms.Label labelCPC2;
  private System.Windows.Forms.Label labelMCM;
  private System.Windows.Forms.Label labelTaskQuestion;
  private RadioButton radioCleanRemanFilter;
  private RadioButton radioNewFilter;
  private RadioButton radioMileageReset;
  private TableLayoutPanel tableLayoutDPFSerialNumber;
  private System.Windows.Forms.Label labelDPFSerialNumberHeader;
  private System.Windows.Forms.Label labelDPFSerialNumber;
  private TextBox textBoxDPFSerialNumber;
  private System.Windows.Forms.Label labelSNErrorMessage;
  private TableLayoutPanel tableLayoutAshDistance;
  private System.Windows.Forms.Label labelAshDistanceAndTime;
  private RadioButton radioDistanceCopiedFromCPC2;
  private RadioButton radioDistanceProvidedByTech;
  private System.Windows.Forms.Label labelTechDistanceUnits;
  private TextBox textBoxAshDistance;
  private System.Windows.Forms.Label labelTechDistance;
  private TableLayoutPanel tableLayoutPanelMCM;
  private DigitalReadoutInstrument readoutMCMDistance;
  private Button buttonClose;
  private TextBox textBoxProgress;
  private System.Windows.Forms.Label labelProgress;
  private FlowLayoutPanel flowLayoutPanel1;
  private System.Windows.Forms.Label labelWarning;
  private System.Windows.Forms.Label labelLicenseMessage;
  private Button buttonPerformAction;

  public UserPanel()
  {
    this.InitializeComponent();
    this.serialNumberValidations.Add("S60", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("MBE900", UserPanel.MediumDutySerialNumberValidation);
    this.serialNumberValidations.Add("MBE4000", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD13", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD15", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD15EURO4", UserPanel.HeavyDutySerialNumberValidation);
    this.radioDistanceCopiedFromCPC2.Checked = true;
    this.radioCleanRemanFilter.Checked = true;
    this.radioCleanRemanFilter.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioNewFilter.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioMCMReplace.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioMileageReset.CheckedChanged += new EventHandler(this.OnReasonChanged);
    this.radioDistanceCopiedFromCPC2.CheckedChanged += new EventHandler(this.OnSourceChanged);
    this.radioDistanceProvidedByTech.CheckedChanged += new EventHandler(this.OnSourceChanged);
    this.textBoxAshDistance.TextChanged += new EventHandler(this.OnAshDistanceChanged);
    this.textBoxAshDistance.KeyPress += new KeyPressEventHandler(this.OnAshDistanceKeyPress);
    this.textBoxDPFSerialNumber.TextChanged += new EventHandler(this.OnDPFSerialNumberChanged);
    this.textBoxDPFSerialNumber.KeyPress += new KeyPressEventHandler(this.OnDPFSerialNumberKeyPress);
    this.buttonPerformAction.Click += new EventHandler(this.OnPerformAction);
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannels();
    this.UpdateUserInterface();
  }

  private void UpdateChannels()
  {
    if (!(this.SetCPC2(this.GetChannel("CPC2")) | this.SetMCM(this.GetChannel("MCM"))))
      return;
    this.UpdateWarningMessage();
  }

  private void CleanUpChannels()
  {
    this.SetCPC2((Channel) null);
    this.SetMCM((Channel) null);
    this.UpdateWarningMessage();
  }

  private void UpdateWarningMessage()
  {
    bool flag = false;
    if (this.IsLicenseValid)
    {
      if (this.mcm != null && UserPanel.HasUnsentChanges(this.mcm))
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
      string qualifier = parameter.Qualifier;
      Qualifier mileageParameter = UserPanel.AshMileageParameter;
      string name1 = ((Qualifier) ref mileageParameter).Name;
      int num;
      if (!(qualifier != name1))
      {
        string name2 = channel.Ecu.Name;
        mileageParameter = UserPanel.AshMileageParameter;
        string ecu = ((Qualifier) ref mileageParameter).Ecu;
        num = !(name2 != ecu) ? 1 : 0;
      }
      else
        num = 0;
      if (num == 0 && !object.Equals(parameter.Value, parameter.OriginalValue))
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
        this.targetESN = string.Empty;
      }
      this.cpc2 = cpc2;
      flag = true;
      if (this.cpc2 != null)
        this.cpc2.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
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
        this.ashMileage = (ParameterDataItem) null;
        if (this.distanceTillAshFull != null)
        {
          ((DataItem) this.distanceTillAshFull).UpdateEvent -= new EventHandler<ResultEventArgs>(this.OnDistanceTillAshFullUpdate);
          this.distanceTillAshFull = (InstrumentDataItem) null;
        }
        this.targetVIN = string.Empty;
        this.targetESN = string.Empty;
      }
      this.mcm = mcm;
      flag = true;
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.ashMileage = DataItem.Create(UserPanel.AshMileageParameter, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) as ParameterDataItem;
        this.distanceTillAshFull = DataItem.Create(UserPanel.DistanceTillAshFullInstrument, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) as InstrumentDataItem;
        if (this.distanceTillAshFull != null)
          ((DataItem) this.distanceTillAshFull).UpdateEvent += new EventHandler<ResultEventArgs>(this.OnDistanceTillAshFullUpdate);
        this.ReadAccumulators(false);
      }
    }
    return flag;
  }

  private void OnDistanceTillAshFullUpdate(object sender, ResultEventArgs e)
  {
    this.UpdateAccessLevels();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    SapiManager.GlobalInstance.AccessLevelsChanged += new EventHandler(this.OnAccessLevelsChanged);
    this.UpdateChannels();
    this.UpdateAccessLevels();
  }

  private void OnAccessLevelsChanged(object sender, EventArgs e) => this.UpdateAccessLevels();

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    SapiManager.GlobalInstance.AccessLevelsChanged -= new EventHandler(this.OnAccessLevelsChanged);
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

  private void OnSourceChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnDPFSerialNumberChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnDPFSerialNumberKeyPress(object sender, KeyPressEventArgs e)
  {
    e.KeyChar = e.KeyChar.ToString().ToUpperInvariant()[0];
    if (e.KeyChar == '\b' || UserPanel.AnySerialNumberValidation.IsMatch(e.KeyChar.ToString()))
      return;
    e.Handled = true;
  }

  private void OnAshDistanceChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnAshDistanceKeyPress(object sender, KeyPressEventArgs e)
  {
    if (e.KeyChar == '\b' || UserPanel.DistanceValidation.IsMatch(e.KeyChar.ToString()))
      return;
    e.Handled = true;
  }

  private bool ValidateDistance(string text)
  {
    double result;
    if (!string.IsNullOrEmpty(text) && this.HaveDistanceMaxValue && UserPanel.DistanceValidation.IsMatch(text) && double.TryParse(text, out result))
    {
      double num = ((DataItem) this.ashMileage).ValueAsDouble(this.ashMileage.OriginalValue) + ((DataItem) this.distanceTillAshFull).ValueAsDouble(((DataItem) this.distanceTillAshFull).Value);
      if (result <= num)
        return true;
    }
    return false;
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

  private bool Online => this.IsChannelOnline(this.cpc2) && this.IsChannelOnline(this.mcm);

  private bool IsChannelOnline(Channel channel)
  {
    return channel != null && channel.CommunicationsState == CommunicationsState.Online;
  }

  private bool Working => this.currentStage != UserPanel.Stage.Idle;

  private bool CanClose => !this.Working;

  private UserPanel.SupportedAccumulatorManualChange ShowAshAccumulators
  {
    get
    {
      UserPanel.SupportedAccumulatorManualChange showAshAccumulators = UserPanel.SupportedAccumulatorManualChange.None;
      if (this.radioMCMReplace.Checked && this.AllowTechAccumulatorProvision)
        showAshAccumulators |= UserPanel.SupportedAccumulatorManualChange.Distance;
      return showAshAccumulators;
    }
  }

  private bool CanSetAshAccumulationDistance
  {
    get
    {
      bool accumulationDistance = false;
      if (!this.Working && this.Online)
      {
        int num;
        if (!this.radioMCMReplace.Checked ? (!this.radioMileageReset.Checked ? this.cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"] != (Service) null && this.ashMileage != null && this.ValidSerialNumberProvided : this.cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"] != (Service) null && this.ashMileage != null && this.ValidSerialNumberProvided && this.CanPerformMileageReset) : (!this.radioDistanceCopiedFromCPC2.Checked ? this.ValidDistanceProvided && this.cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"] != (Service) null && this.ashMileage != null : this.cpc2.Services["RT_DPF_Ash_Mileage_Read_Request_Results_Status"] != (Service) null && this.ashMileage != null))
        {
          ParameterCollection parameters = this.mcm.Parameters;
          Qualifier mileageParameter = UserPanel.AshMileageParameter;
          string name = ((Qualifier) ref mileageParameter).Name;
          num = parameters[name] != null ? 1 : 0;
        }
        else
          num = 0;
        accumulationDistance = num != 0;
      }
      return accumulationDistance;
    }
  }

  private bool ValidDistanceProvided => this.ValidateDistance(this.textBoxAshDistance.Text);

  private bool ValidSerialNumberProvided
  {
    get => this.ValidateSerialNumber(this.textBoxDPFSerialNumber.Text, out string _);
  }

  private bool HaveDistanceMaxValue
  {
    get
    {
      return this.ashMileage != null && this.distanceTillAshFull != null && this.ashMileage.OriginalValue != null && ((DataItem) this.distanceTillAshFull).Value != null;
    }
  }

  private bool AllowTechAccumulatorProvision
  {
    get
    {
      switch (LicenseManager.GlobalInstance.AccessLevel)
      {
        case 2:
          if (this.ashMileage != null && this.ashMileage.OriginalValue != null)
            return ((DataItem) this.ashMileage).ValueAsDouble(this.ashMileage.OriginalValue) == 0.0;
          break;
        case 3:
          return true;
      }
      return false;
    }
  }

  private bool IsLicenseValid => LicenseManager.GlobalInstance.AccessLevel >= 1;

  private bool SupportsMileageReset
  {
    get
    {
      string engineTypeName = this.GetEngineTypeName();
      return !string.IsNullOrEmpty(engineTypeName) && engineTypeName != "MBE900";
    }
  }

  private bool CanPerformMileageReset
  {
    get
    {
      return this.ashMileage != null && this.ashMileage.OriginalValue != null && Math.Round(Convert.ToDouble(this.ashMileage.OriginalValue)) >= 241402.0;
    }
  }

  private void UpdateAccessLevels()
  {
    if (this.AllowTechAccumulatorProvision && !((Control) this.tableLayoutAshDistance).Visible)
      this.UpdateUserInterface();
    else if (!this.AllowTechAccumulatorProvision && ((Control) this.tableLayoutAshDistance).Visible)
    {
      this.radioDistanceCopiedFromCPC2.Checked = true;
      this.UpdateUserInterface();
    }
    this.UpdateWarningMessage();
  }

  private void UpdateAccumulatorEntryUserInterface()
  {
    this.labelTechDistanceUnits.Text = ((SingleInstrumentBase) this.readoutMCMDistance).Unit;
    this.textBoxAshDistance.ReadOnly = !this.radioDistanceProvidedByTech.Checked || !this.HaveDistanceMaxValue;
    ((Control) this.tableLayoutAshDistance).Visible = this.ShowAshAccumulators != UserPanel.SupportedAccumulatorManualChange.None;
    this.labelAshDistanceAndTime.Text = Resources.Message_PleaseIndicateTheNewAshAccumulationDistance;
  }

  private void UpdateUserInterface()
  {
    bool flag = this.Online && !this.Working && this.IsLicenseValid && this.GetValidationInformation() != null;
    this.radioCleanRemanFilter.Enabled = this.radioNewFilter.Enabled = flag;
    this.radioMileageReset.Enabled = flag && this.SupportsMileageReset && this.CanPerformMileageReset;
    this.buttonClose.Enabled = this.CanClose;
    this.labelTaskQuestion.Enabled = flag;
    this.buttonPerformAction.Enabled = this.CanSetAshAccumulationDistance && flag;
    int num;
    switch (ApplicationInformation.ProductAccessLevel)
    {
      case 1:
        this.radioMCMReplace.Visible = false;
        ((Control) this.tableLayoutAshDistance).Visible = false;
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
      this.radioMCMReplace.Enabled = flag;
      ((Control) this.tableLayoutAshDistance).Enabled = flag;
      if (this.radioMCMReplace.Checked)
        ((Control) this.tableLayoutDPFSerialNumber).Visible = false;
      else if (!((Control) this.tableLayoutDPFSerialNumber).Visible)
        ((Control) this.tableLayoutDPFSerialNumber).Visible = true;
    }
label_11:
    this.UpdateAccumulatorEntryUserInterface();
    if (!((Control) this.tableLayoutDPFSerialNumber).Visible)
      return;
    ((Control) this.tableLayoutDPFSerialNumber).Enabled = flag;
    this.textBoxDPFSerialNumber.ReadOnly = !flag;
    if (this.textBoxDPFSerialNumber.ReadOnly)
    {
      this.textBoxDPFSerialNumber.BackColor = SystemColors.Control;
    }
    else
    {
      string errorText;
      if (this.ValidateSerialNumber(this.textBoxDPFSerialNumber.Text, out errorText))
      {
        this.labelSNErrorMessage.Visible = false;
        this.textBoxDPFSerialNumber.BackColor = Color.LightGreen;
      }
      else
      {
        this.labelSNErrorMessage.Text = errorText;
        this.labelSNErrorMessage.Visible = true;
        this.textBoxDPFSerialNumber.BackColor = Color.LightPink;
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
        this.targetESN = SapiManager.GetEngineSerialNumber(this.mcm);
        this.targetVIN = SapiManager.GetVehicleIdentificationNumber(this.mcm);
        string action;
        if (this.radioMCMReplace.Checked)
        {
          this.dpfSN = string.Empty;
          action = !this.radioDistanceCopiedFromCPC2.Checked ? $"{Resources.Message_ChangeDistanceTo}{this.textBoxAshDistance.Text} {this.labelTechDistanceUnits.Text}" : Resources.Message_SynchronizedFromCPC2;
        }
        else
        {
          action = !this.radioMileageReset.Checked ? Resources.Message_Reset : string.Format(Resources.MessageFormat_DPFAshMileageExtendedBy01, (object) Math.Round(((DataItem) this.ashMileage).ValueAsDouble((object) 241402.0)).ToString(), (object) ((DataItem) this.ashMileage).Units);
          this.dpfSN = this.textBoxDPFSerialNumber.Text;
        }
        if (UserPanel.ConfirmationDialog.Show(this.targetESN, this.targetVIN, this.dpfSN, action))
        {
          this.ClearOutput();
          this.Report(Resources.Message_AshAccumulatorModificationsStarted);
          this.Report(Resources.Message_VIN + this.targetVIN);
          this.Report(Resources.Message_ESN + this.targetESN);
          if (((Control) this.tableLayoutDPFSerialNumber).Visible)
            this.Report(Resources.Message_DPFSerialNumber + this.dpfSN);
          this.Report(Resources.Message_AshAccumulators0 + action);
          this.CurrentStage = UserPanel.Stage.GetValue;
          this.PerformCurrentStage();
          break;
        }
        this.StopWork(UserPanel.Reason.Canceled);
        break;
      case UserPanel.Stage.GetValue:
        if (this.radioMCMReplace.Checked)
        {
          if (this.radioDistanceCopiedFromCPC2.Checked)
          {
            this.CurrentStage = UserPanel.Stage.WaitingForCPC2Read;
            this.ExecuteService(this.cpc2.Services["RT_DPF_Ash_Mileage_Read_Request_Results_Status"], false);
            break;
          }
          this.CurrentStage = UserPanel.Stage.WriteValue;
          this.distanceValueToWrite = this.textBoxAshDistance.Text;
          this.PerformCurrentStage();
          break;
        }
        this.distanceValueToWrite = !this.radioMileageReset.Checked ? ((DataItem) this.ashMileage).ValueAsString((object) 0.0) : (Math.Round(((DataItem) this.ashMileage).ValueAsDouble(this.ashMileage.OriginalValue)) - Math.Round(((DataItem) this.ashMileage).ValueAsDouble((object) 241402.0))).ToString();
        this.CurrentStage = UserPanel.Stage.WriteValue;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitingForCPC2Read:
        this.distanceValueToWrite = ((DataItem) this.ashMileage).ValueAsString((object) Convert.ToDouble(this.lastRunService.OutputValues[0].Value));
        this.CurrentStage = UserPanel.Stage.WriteValue;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WriteValue:
        this.Report(Resources.Message_WritingNewValues);
        if (this.SetMCMAshAccumulators(this.distanceValueToWrite))
        {
          this.CurrentStage = UserPanel.Stage.ResetCPC2;
          break;
        }
        this.StopWork(UserPanel.Reason.FailedWrite);
        break;
      case UserPanel.Stage.ResetCPC2:
        this.CurrentStage = UserPanel.Stage.WaitingForCPC2Reset;
        this.ExecuteService(this.cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"], false);
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
        this.CurrentStage = UserPanel.Stage.ResetDPFMaxima;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ResetDPFMaxima:
        this.CurrentStage = UserPanel.Stage.WaitingForDPFMaximaReset;
        Service service = this.mcm.Services["RT_SR014_SET_EOL_Default_Values_Start"];
        if (service == (Service) null)
        {
          this.Report(Resources.Message_NoResetServiceAvailable);
          this.PerformCurrentStage();
          break;
        }
        this.Report(Resources.Message_ResettingDPFMaximumTemperatures);
        service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 6);
        this.ExecuteService(service, false);
        break;
      case UserPanel.Stage.WaitingForDPFMaximaReset:
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

  private void UpdateServiceLog(char type, object oldValue, object currentValue)
  {
    string str1 = string.Empty;
    if (this.radioCleanRemanFilter.Checked)
      str1 = Resources.Message_ReasonCleanRemanFilter;
    else if (this.radioNewFilter.Checked)
      str1 = Resources.Message_ReasonNewFilter;
    else if (this.radioMCMReplace.Checked)
      str1 = Resources.Message_ReasonMCMReplacement;
    else if (this.radioMileageReset.Checked)
      str1 = Resources.Message_ReasonMileageResetFromFilterMeasurement;
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    dictionary1["reasontext"] = str1;
    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
    dictionary2["DPF_Serial_Number1"] = string.IsNullOrEmpty(this.dpfSN) ? Resources.Message_NA : this.dpfSN;
    string str2 = type == 'd' ? "Distance" : "Time";
    int int32;
    if (oldValue != null)
    {
      Dictionary<string, string> dictionary3 = dictionary2;
      string key = "Old_" + str2;
      int32 = Convert.ToInt32(oldValue);
      string str3 = int32.ToString();
      dictionary3[key] = str3;
    }
    Dictionary<string, string> dictionary4 = dictionary2;
    string key1 = "Current_" + str2;
    int32 = Convert.ToInt32(currentValue);
    string str4 = int32.ToString();
    dictionary4[key1] = str4;
    dictionary2["CO_Odometer"] = this.ReadEcuInfoData(this.cpc2, "CO_Odometer");
    dictionary2["DT_AS045_Engine_Operating_Hours"] = this.ReadInstrumentValue(this.mcm, "DT_AS045_Engine_Operating_Hours");
    ServerDataManager.UpdateEventsFile(this.mcm, (IDictionary<string, string>) dictionary1, (IDictionary<string, string>) dictionary2, "DPFAshAccumulation", this.targetESN, this.targetVIN, "OK", "DESCRIPTION", string.Empty);
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

  private void StopWork(UserPanel.Reason reason)
  {
    if (this.CurrentStage != UserPanel.Stage.Stopping && this.CurrentStage != UserPanel.Stage.Idle)
    {
      UserPanel.Stage currentStage = this.CurrentStage;
      this.CurrentStage = UserPanel.Stage.Stopping;
      if (reason == UserPanel.Reason.Success)
      {
        this.UpdateServiceLog('d', this.oldDistanceValue, this.newDistanceValue);
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
            switch (currentStage)
            {
              case UserPanel.Stage.WaitingForCPC2Read:
                this.Report(Resources.Message_FailedToExecuteReadOfAshAccumulationDistanceFromCPC2);
                break;
              case UserPanel.Stage.WaitingForCPC2Reset:
                this.Report(Resources.Message_FailedToExecuteResetOfAshAccumulationDistanceInCPC2);
                break;
              case UserPanel.Stage.WaitingForDPFMaximaReset:
                this.Report(Resources.Message_FailedToExecuteResetOfDPFTemperatureAccumulatorsInMCM);
                break;
            }
            break;
          case UserPanel.Reason.FailedService:
            this.Report(Resources.Message_FailedToWriteTheAshAccumulators);
            break;
          case UserPanel.Reason.FailedWrite:
            this.Report(Resources.Message_FailedToCommitTheChangesToTheCPC2YouMayNeedToRepeatThisProcedure);
            break;
          case UserPanel.Reason.Closing:
            this.Report(Resources.Message_OneOrMoreDevicesDisconnected);
            this.textBoxDPFSerialNumber.Text = this.textBoxAshDistance.Text = string.Empty;
            break;
          case UserPanel.Reason.Disconnected:
            this.Report(Resources.Message_TheUserCanceledTheOperation);
            break;
        }
      }
      this.ClearCurrentService(false);
      this.distanceValueToWrite = string.Empty;
      this.oldDistanceValue = (object) null;
      this.newDistanceValue = (object) null;
      this.CurrentStage = UserPanel.Stage.Idle;
      this.ReadAccumulators(false);
    }
    this.UpdateWarningMessage();
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

  private void ReadAccumulators(bool synchronous)
  {
    if (this.mcm == null)
      return;
    ParameterCollection parameters1 = this.mcm.Parameters;
    Qualifier mileageParameter1 = UserPanel.AshMileageParameter;
    string name1 = ((Qualifier) ref mileageParameter1).Name;
    if (parameters1[name1] != null)
    {
      ParameterCollection parameters2 = this.mcm.Parameters;
      ParameterCollection parameters3 = this.mcm.Parameters;
      Qualifier mileageParameter2 = UserPanel.AshMileageParameter;
      string name2 = ((Qualifier) ref mileageParameter2).Name;
      string groupQualifier = parameters3[name2].GroupQualifier;
      int num = synchronous ? 1 : 0;
      parameters2.ReadGroup(groupQualifier, true, num != 0);
    }
  }

  private bool SetMCMAshAccumulators(string valueDistance)
  {
    bool flag = false;
    if (this.ashMileage != null)
    {
      Cursor.Current = Cursors.WaitCursor;
      this.oldDistanceValue = this.ashMileage.OriginalValue;
      ((DataItem) this.ashMileage).WriteValue((object) valueDistance);
      this.newDistanceValue = ((DataItem) this.ashMileage).Value;
      this.mcm.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
      this.mcm.Parameters.Write(false);
      flag = true;
    }
    else
      this.Report(Resources.Message_FailedToObtainMCMAshDistanceAccumulator);
    return flag;
  }

  private void OnParametersWriteComplete(object sender, ResultEventArgs e)
  {
    ParameterCollection parameterCollection1 = (ParameterCollection) sender;
    parameterCollection1.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
    bool flag = false;
    if (e.Succeeded)
    {
      ParameterCollection parameterCollection2 = parameterCollection1;
      Qualifier mileageParameter = UserPanel.AshMileageParameter;
      string name = ((Qualifier) ref mileageParameter).Name;
      Parameter parameter = parameterCollection2[name];
      if (parameter.Exception != null)
      {
        if (parameter.Exception is CaesarException exception && exception.ErrorNumber == 6602L)
        {
          this.Report($"{Resources.Message_WhileWritingTheNewAshAccumulationDistanceTheFollowingWarningWasReportedRN}\r\n{parameter.Name}: {exception.Message}");
        }
        else
        {
          flag = true;
          this.Report($"{Resources.Message_WhileWritingTheNewAshAccumulationDistanceTheFollowingErrorWasReportedRN}\r\n{parameter.Name}: {parameter.Exception.Message}");
        }
      }
    }
    else
    {
      this.Report(string.Format($"{Resources.MessageFormat_WhileWritingTheNewAshAccumulationDistanceTheFollowingErrorOccurred}\r\n\r\n\"{{0}}\"\r\n\r\n{Resources.ParametersHaveNotBeenVerifiedAndMayNotHaveBeenWritten}", (object) e.Exception.Message));
      flag = true;
    }
    Cursor.Current = Cursors.Default;
    if (flag)
      this.StopWork(UserPanel.Reason.FailedWrite);
    else
      this.PerformCurrentStage();
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
    Application.DoEvents();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.radioMCMReplace = new RadioButton();
    this.titleLabel = new ScalingLabel();
    this.cpcReadout = new DigitalReadoutInstrument();
    this.labelCPC2 = new System.Windows.Forms.Label();
    this.labelMCM = new System.Windows.Forms.Label();
    this.labelTaskQuestion = new System.Windows.Forms.Label();
    this.radioCleanRemanFilter = new RadioButton();
    this.radioNewFilter = new RadioButton();
    this.radioMileageReset = new RadioButton();
    this.tableLayoutDPFSerialNumber = new TableLayoutPanel();
    this.labelDPFSerialNumberHeader = new System.Windows.Forms.Label();
    this.labelDPFSerialNumber = new System.Windows.Forms.Label();
    this.textBoxDPFSerialNumber = new TextBox();
    this.labelSNErrorMessage = new System.Windows.Forms.Label();
    this.tableLayoutAshDistance = new TableLayoutPanel();
    this.labelAshDistanceAndTime = new System.Windows.Forms.Label();
    this.radioDistanceCopiedFromCPC2 = new RadioButton();
    this.radioDistanceProvidedByTech = new RadioButton();
    this.labelTechDistanceUnits = new System.Windows.Forms.Label();
    this.textBoxAshDistance = new TextBox();
    this.labelTechDistance = new System.Windows.Forms.Label();
    this.tableLayoutPanelMCM = new TableLayoutPanel();
    this.readoutMCMDistance = new DigitalReadoutInstrument();
    this.buttonClose = new Button();
    this.textBoxProgress = new TextBox();
    this.labelProgress = new System.Windows.Forms.Label();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.labelWarning = new System.Windows.Forms.Label();
    this.labelLicenseMessage = new System.Windows.Forms.Label();
    this.buttonPerformAction = new Button();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this.tableLayoutDPFSerialNumber).SuspendLayout();
    ((Control) this.tableLayoutAshDistance).SuspendLayout();
    ((Control) this.tableLayoutPanelMCM).SuspendLayout();
    this.flowLayoutPanel1.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioMCMReplace, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.titleLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.cpcReadout, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelCPC2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelMCM, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelTaskQuestion, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioCleanRemanFilter, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioNewFilter, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.radioMileageReset, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutDPFSerialNumber, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutAshDistance, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutPanelMCM, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonClose, 1, 14);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.textBoxProgress, 0, 13);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelProgress, 0, 12);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.flowLayoutPanel1, 0, 11);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonPerformAction, 0, 10);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.radioMCMReplace, "radioMCMReplace");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.radioMCMReplace, 2);
    this.radioMCMReplace.Name = "radioMCMReplace";
    this.radioMCMReplace.TabStop = true;
    this.radioMCMReplace.UseCompatibleTextRendering = true;
    this.radioMCMReplace.UseVisualStyleBackColor = true;
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
    ((SingleInstrumentBase) this.cpcReadout).Instrument = new Qualifier((QualifierTypes) 1, "CPC2", "DT_AS056_DPF_Ash_Content_Mileage");
    ((Control) this.cpcReadout).Name = "cpcReadout";
    ((Control) this.cpcReadout).TabStop = false;
    ((SingleInstrumentBase) this.cpcReadout).UnitAlignment = StringAlignment.Near;
    this.labelCPC2.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.labelCPC2, "labelCPC2");
    this.labelCPC2.BackColor = SystemColors.ControlDark;
    this.labelCPC2.CausesValidation = false;
    this.labelCPC2.ForeColor = SystemColors.ControlText;
    this.labelCPC2.Name = "labelCPC2";
    this.labelCPC2.UseCompatibleTextRendering = true;
    this.labelMCM.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.labelMCM, "labelMCM");
    this.labelMCM.BackColor = SystemColors.ControlDark;
    this.labelMCM.CausesValidation = false;
    this.labelMCM.ForeColor = SystemColors.ControlText;
    this.labelMCM.Name = "labelMCM";
    this.labelMCM.UseCompatibleTextRendering = true;
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
    componentResourceManager.ApplyResources((object) this.radioMileageReset, "radioMileageReset");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.radioMileageReset, 2);
    this.radioMileageReset.Name = "radioMileageReset";
    this.radioMileageReset.TabStop = true;
    this.radioMileageReset.UseCompatibleTextRendering = true;
    this.radioMileageReset.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutDPFSerialNumber, "tableLayoutDPFSerialNumber");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutDPFSerialNumber, 2);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.labelDPFSerialNumberHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.labelDPFSerialNumber, 0, 1);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.textBoxDPFSerialNumber, 1, 1);
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).Controls.Add((Control) this.labelSNErrorMessage, 2, 1);
    ((Control) this.tableLayoutDPFSerialNumber).Name = "tableLayoutDPFSerialNumber";
    componentResourceManager.ApplyResources((object) this.labelDPFSerialNumberHeader, "labelDPFSerialNumberHeader");
    this.labelDPFSerialNumberHeader.BackColor = SystemColors.ControlDark;
    ((TableLayoutPanel) this.tableLayoutDPFSerialNumber).SetColumnSpan((Control) this.labelDPFSerialNumberHeader, 3);
    this.labelDPFSerialNumberHeader.Name = "labelDPFSerialNumberHeader";
    this.labelDPFSerialNumberHeader.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelDPFSerialNumber, "labelDPFSerialNumber");
    this.labelDPFSerialNumber.Name = "labelDPFSerialNumber";
    this.labelDPFSerialNumber.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxDPFSerialNumber, "textBoxDPFSerialNumber");
    this.textBoxDPFSerialNumber.Name = "textBoxDPFSerialNumber";
    componentResourceManager.ApplyResources((object) this.labelSNErrorMessage, "labelSNErrorMessage");
    this.labelSNErrorMessage.ForeColor = Color.Red;
    this.labelSNErrorMessage.Name = "labelSNErrorMessage";
    this.labelSNErrorMessage.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutAshDistance, "tableLayoutAshDistance");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutAshDistance, 2);
    ((TableLayoutPanel) this.tableLayoutAshDistance).Controls.Add((Control) this.labelAshDistanceAndTime, 0, 0);
    ((TableLayoutPanel) this.tableLayoutAshDistance).Controls.Add((Control) this.radioDistanceCopiedFromCPC2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutAshDistance).Controls.Add((Control) this.radioDistanceProvidedByTech, 0, 2);
    ((TableLayoutPanel) this.tableLayoutAshDistance).Controls.Add((Control) this.labelTechDistanceUnits, 2, 3);
    ((TableLayoutPanel) this.tableLayoutAshDistance).Controls.Add((Control) this.textBoxAshDistance, 1, 3);
    ((TableLayoutPanel) this.tableLayoutAshDistance).Controls.Add((Control) this.labelTechDistance, 0, 3);
    ((Control) this.tableLayoutAshDistance).Name = "tableLayoutAshDistance";
    componentResourceManager.ApplyResources((object) this.labelAshDistanceAndTime, "labelAshDistanceAndTime");
    this.labelAshDistanceAndTime.BackColor = SystemColors.ControlDark;
    ((TableLayoutPanel) this.tableLayoutAshDistance).SetColumnSpan((Control) this.labelAshDistanceAndTime, 4);
    this.labelAshDistanceAndTime.Name = "labelAshDistanceAndTime";
    this.labelAshDistanceAndTime.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.radioDistanceCopiedFromCPC2, "radioDistanceCopiedFromCPC2");
    ((TableLayoutPanel) this.tableLayoutAshDistance).SetColumnSpan((Control) this.radioDistanceCopiedFromCPC2, 4);
    this.radioDistanceCopiedFromCPC2.Name = "radioDistanceCopiedFromCPC2";
    this.radioDistanceCopiedFromCPC2.TabStop = true;
    this.radioDistanceCopiedFromCPC2.UseCompatibleTextRendering = true;
    this.radioDistanceCopiedFromCPC2.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.radioDistanceProvidedByTech, "radioDistanceProvidedByTech");
    ((TableLayoutPanel) this.tableLayoutAshDistance).SetColumnSpan((Control) this.radioDistanceProvidedByTech, 4);
    this.radioDistanceProvidedByTech.Name = "radioDistanceProvidedByTech";
    this.radioDistanceProvidedByTech.TabStop = true;
    this.radioDistanceProvidedByTech.UseCompatibleTextRendering = true;
    this.radioDistanceProvidedByTech.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelTechDistanceUnits, "labelTechDistanceUnits");
    this.labelTechDistanceUnits.Name = "labelTechDistanceUnits";
    this.labelTechDistanceUnits.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxAshDistance, "textBoxAshDistance");
    this.textBoxAshDistance.Name = "textBoxAshDistance";
    componentResourceManager.ApplyResources((object) this.labelTechDistance, "labelTechDistance");
    this.labelTechDistance.Name = "labelTechDistance";
    this.labelTechDistance.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMCM, "tableLayoutPanelMCM");
    ((TableLayoutPanel) this.tableLayoutPanelMCM).Controls.Add((Control) this.readoutMCMDistance, 0, 0);
    ((Control) this.tableLayoutPanelMCM).Name = "tableLayoutPanelMCM";
    componentResourceManager.ApplyResources((object) this.readoutMCMDistance, "readoutMCMDistance");
    this.readoutMCMDistance.FontGroup = (string) null;
    ((SingleInstrumentBase) this.readoutMCMDistance).FreezeValue = false;
    ((SingleInstrumentBase) this.readoutMCMDistance).Instrument = new Qualifier((QualifierTypes) 4, "MCM", "e2p_dpf_ash_last_clean_dist");
    ((Control) this.readoutMCMDistance).Name = "readoutMCMDistance";
    ((SingleInstrumentBase) this.readoutMCMDistance).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.textBoxProgress, 2);
    componentResourceManager.ApplyResources((object) this.textBoxProgress, "textBoxProgress");
    this.textBoxProgress.Name = "textBoxProgress";
    this.textBoxProgress.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.labelProgress, "labelProgress");
    this.labelProgress.BackColor = SystemColors.ControlDark;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.labelProgress, 2);
    this.labelProgress.Name = "labelProgress";
    this.labelProgress.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.flowLayoutPanel1, 2);
    this.flowLayoutPanel1.Controls.Add((Control) this.labelWarning);
    this.flowLayoutPanel1.Controls.Add((Control) this.labelLicenseMessage);
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelWarning, "labelWarning");
    this.labelWarning.BackColor = SystemColors.Control;
    this.labelWarning.ForeColor = Color.Red;
    this.labelWarning.Name = "labelWarning";
    this.labelWarning.UseCompatibleTextRendering = true;
    this.labelWarning.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.labelLicenseMessage, "labelLicenseMessage");
    this.labelLicenseMessage.BackColor = SystemColors.Control;
    this.labelLicenseMessage.ForeColor = Color.Red;
    this.labelLicenseMessage.Name = "labelLicenseMessage";
    this.labelLicenseMessage.UseCompatibleTextRendering = true;
    this.labelLicenseMessage.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.buttonPerformAction, "buttonPerformAction");
    this.buttonPerformAction.Name = "buttonPerformAction";
    this.buttonPerformAction.UseCompatibleTextRendering = true;
    this.buttonPerformAction.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DPFAshAccumulator_EPA07");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    ((Control) this.tableLayoutDPFSerialNumber).ResumeLayout(false);
    ((Control) this.tableLayoutDPFSerialNumber).PerformLayout();
    ((Control) this.tableLayoutAshDistance).ResumeLayout(false);
    ((Control) this.tableLayoutAshDistance).PerformLayout();
    ((Control) this.tableLayoutPanelMCM).ResumeLayout(false);
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum SupportedAccumulatorManualChange
  {
    None,
    Distance,
  }

  private enum Stage
  {
    Idle = 0,
    GetConfirmation = 1,
    _Start = 1,
    GetValue = 2,
    WaitingForCPC2Read = 3,
    WriteValue = 4,
    ResetCPC2 = 5,
    WaitingForCPC2Reset = 6,
    CommitChangesToCPC2 = 7,
    WaitingForCPC2Commit = 8,
    ResetDPFMaxima = 9,
    WaitingForDPFMaximaReset = 10, // 0x0000000A
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

  private static class Log
  {
    public static void AddEvent(string eventText)
    {
      if (SapiManager.GlobalInstance == null || SapiManager.GlobalInstance.Sapi == null || string.IsNullOrEmpty(eventText))
        return;
      SapiManager.GlobalInstance.Sapi.LogFiles.LabelLog(eventText.Trim());
    }
  }

  private static class ConfirmationDialog
  {
    private static string FormatString = $"{Resources.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheToolRN}\r\n{Resources.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarrantyRN}\r\n{Resources.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLikeRN}\r\n{Resources.Message_ToProceedWithTheRequestedChangeToTheDPFAshAccumulationDistanceRN}\r\n\r\n{Resources.MessageFormat_VIN1RN}\r\n{Resources.MessageFormat_ESN0RN}\r\nDPF SN: {{2}}\r\n{Resources.MessageFormat_NewAshMileage3RN}\r\n\r\n{Resources.Message_IsThisInformationCorrectAndDoYouWantToContinue}";
    private static string LogEntryForConfirmation = $"{Resources.Message_DPFAshAccumulationChangeRequested}DPFSN:{{0}},{Resources.MessageFormat_Mileage1}";

    public static bool Show(string esn, string vin, string dpfsn, string action)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string text;
      if (string.IsNullOrEmpty(dpfsn))
        text = string.Format(UserPanel.ConfirmationDialog.FormatString.Replace("DPF SN: {2}\r\n", ""), (object) esn, (object) vin, (object) dpfsn, (object) action);
      else
        text = string.Format(UserPanel.ConfirmationDialog.FormatString, (object) esn, (object) vin, (object) dpfsn, (object) action);
      bool flag;
      if (DialogResult.Yes == MessageBox.Show(text, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
      {
        UserPanel.Log.AddEvent(!string.IsNullOrEmpty(dpfsn) ? string.Format(UserPanel.ConfirmationDialog.LogEntryForConfirmation, (object) dpfsn, (object) action) : string.Format(UserPanel.ConfirmationDialog.LogEntryForConfirmation.Replace("DPFSN:{0},", ""), (object) dpfsn, (object) action));
        flag = true;
      }
      else
        flag = false;
      return flag;
    }
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
}
