// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY20_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY20_.panel;

public class UserPanel : CustomPanel
{
  private static readonly UserPanel.ValidationInformation HeavyDutySerialNumberValidation = new UserPanel.ValidationInformation(new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));
  private static readonly UserPanel.ValidationInformation MediumDutySerialNumberValidation = new UserPanel.ValidationInformation(new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));
  private Dictionary<string, UserPanel.ValidationInformation> serialNumberValidations = new Dictionary<string, UserPanel.ValidationInformation>();
  private int MaxLengthForRefText = 29;
  private Channel acm;
  private Channel mcm;
  private static readonly IList<string> acmScrAccumulatorQualifiers = (IList<string>) new List<string>((IEnumerable<string>) new string[5]
  {
    "Time_Above_SCR_Inlet_Temp_1_Hour",
    "Time_Above_SCR_Inlet_Temp_1_Min",
    "Time_Above_SCR_Inlet_Temp_1_Sec",
    "Time_Above_SCR_Inlet_Temp_2",
    "Time_Above_SCR_Outlet_Temp"
  }).AsReadOnly();
  private List<ParameterDataItem> accumulators = new List<ParameterDataItem>();
  private string targetVIN;
  private string targetESN;
  private static readonly Qualifier ATDTypeParameter = new Qualifier((QualifierTypes) 4, "ACM301T", "ATD_Hardware_Type");
  private ParameterDataItem atdType;
  private bool adrReturnValue = false;
  private static readonly Regex AnySerialNumberValidation = new Regex("[R\\d]", RegexOptions.Compiled);
  private static readonly Regex MdegFullScopeSerialNumberValidation = new Regex("[A-Z0-9]", RegexOptions.Compiled);
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private ScalingLabel titleLabel;
  private TableLayoutPanel tableLayoutSerialNumber;
  private System.Windows.Forms.Label labelSerialNumberHeader;
  private TextBox textBoxSerialNumber1;
  private System.Windows.Forms.Label labelProgress;
  private TextBox textBoxProgress;
  private Button buttonPerformAction;
  private System.Windows.Forms.Label labelSNErrorMessage1;
  private System.Windows.Forms.Label labelWarning;
  private Button buttonClose;
  private FlowLayoutPanel flowLayoutPanel1;
  private System.Windows.Forms.Label labelLicenseMessage;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private TableLayoutPanel tableLayoutPanel;

  public UserPanel()
  {
    this.InitializeComponent();
    this.serialNumberValidations.Add("DD13", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD15", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD16", UserPanel.HeavyDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD5", UserPanel.MediumDutySerialNumberValidation);
    this.serialNumberValidations.Add("DD8", UserPanel.MediumDutySerialNumberValidation);
    this.textBoxSerialNumber1.TextChanged += new EventHandler(this.OnSerialNumberChanged);
    this.textBoxSerialNumber1.KeyPress += new KeyPressEventHandler(this.OnSerialNumberKeyPress);
    this.buttonPerformAction.Click += new EventHandler(this.OnPerformAction);
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannels();
    this.UpdateUserInterface();
  }

  private void UpdateChannels()
  {
    if (!this.SetACM(this.GetChannel("ACM301T")) || !this.SetMCM2(this.GetChannel("MCM21T")))
      return;
    this.UpdateWarningMessage();
    this.textBoxSerialNumber1.Text = string.Empty;
  }

  private void CleanUpChannels()
  {
    this.SetACM((Channel) null);
    this.SetMCM2((Channel) null);
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
      if ((!UserPanel.acmScrAccumulatorQualifiers.Contains(parameter.Name) || channel.Ecu.Name != "ACM301T") && !object.Equals(parameter.Value, parameter.OriginalValue))
      {
        flag = true;
        break;
      }
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
        this.accumulators.Clear();
        this.atdType = (ParameterDataItem) null;
        this.targetVIN = string.Empty;
        this.targetESN = string.Empty;
      }
      this.acm = acm;
      flag = true;
      if (this.acm != null)
      {
        this.atdType = DataItem.Create(UserPanel.ATDTypeParameter, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) as ParameterDataItem;
        this.acm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.ReadAccumulators(false);
      }
    }
    return flag;
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
      parameters2.ReadGroup(groupQualifier, true, num != 0);
    }
    foreach (string accumulatorQualifier in (IEnumerable<string>) UserPanel.acmScrAccumulatorQualifiers)
    {
      if (DataItem.Create(new Qualifier((QualifierTypes) 4, "ACM301T", accumulatorQualifier), (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) is ParameterDataItem parameterDataItem)
        this.accumulators.Add(parameterDataItem);
    }
    if (this.acm.Parameters[UserPanel.acmScrAccumulatorQualifiers[0]] != null)
      this.acm.Parameters.ReadGroup(this.acm.Parameters[UserPanel.acmScrAccumulatorQualifiers[0]].GroupQualifier, true, synchronous);
  }

  private bool SetMCM2(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      this.StopWork(UserPanel.Reason.Disconnected);
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.targetVIN = string.Empty;
        this.targetESN = string.Empty;
      }
      this.mcm = mcm;
      flag = true;
      if (this.mcm != null)
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    return flag;
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

  private void OnSerialNumberChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnSerialNumberKeyPress(object sender, KeyPressEventArgs e)
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
      if (this.acm != null)
      {
        if (this.atdType == null)
          return AtsType.OneBox;
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

  public bool MdegFullScope
  {
    get
    {
      string engineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
      return string.IsNullOrEmpty(engineSerialNumber) || !engineSerialNumber.StartsWith("934912C");
    }
  }

  private bool ValidateSerialNumber(string text, out string errorText)
  {
    bool flag = false;
    errorText = string.Empty;
    UserPanel.ValidationInformation validationInformation = this.GetValidationInformation();
    if (validationInformation == null)
      errorText = Resources.Message_UnsupportedEquipment;
    else
      flag = validationInformation.Regex.IsMatch(text);
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
    UserPanel.ValidationInformation validationInformation = (UserPanel.ValidationInformation) null;
    EquipmentType equipmentType = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault<EquipmentType>((Func<EquipmentType, bool>) (et =>
    {
      ElectronicsFamily family = ((EquipmentType) ref et).Family;
      return ((ElectronicsFamily) ref family).Category == "Engine";
    }));
    if (EquipmentType.op_Inequality(equipmentType, EquipmentType.Empty))
      this.serialNumberValidations.TryGetValue(((EquipmentType) ref equipmentType).Name, out validationInformation);
    return validationInformation;
  }

  private void OnPerformAction(object sender, EventArgs e) => this.DoWork();

  public bool Online => this.IsChannelOnline(this.acm) && this.IsChannelOnline(this.mcm);

  public bool IsChannelOnline(Channel channel)
  {
    return channel != null && channel.CommunicationsState == CommunicationsState.Online;
  }

  public bool Working => this.currentStage != UserPanel.Stage.Idle;

  public bool CanClose => !this.Working;

  public bool CanResetAccumulators
  {
    get
    {
      if (this.Working || !this.Online || !this.ValidSerialNumberProvided || this.accumulators.Count != UserPanel.acmScrAccumulatorQualifiers.Count)
        return false;
      bool resetAccumulators = true;
      foreach (string accumulatorQualifier in (IEnumerable<string>) UserPanel.acmScrAccumulatorQualifiers)
        resetAccumulators &= this.acm.Parameters[accumulatorQualifier] != null;
      return resetAccumulators;
    }
  }

  public bool ValidSerialNumberProvided
  {
    get => this.ValidateSerialNumber(this.textBoxSerialNumber1.Text, out string _);
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
    ((Control) this.tableLayoutSerialNumber).Enabled = flag;
    this.textBoxSerialNumber1.ReadOnly = !flag;
    this.buttonPerformAction.Enabled = this.CanResetAccumulators && flag;
    this.buttonClose.Enabled = this.CanClose;
    this.ValidateSCRSerialBox(this.textBoxSerialNumber1, this.labelSNErrorMessage1);
    this.labelSerialNumberHeader.Text = Resources.Message_PleaseProvideTheSerialNumberForTheNewSCRUnit;
  }

  private void ValidateSCRSerialBox(TextBox box, System.Windows.Forms.Label errorMessage)
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
        this.targetESN = SapiManager.GetEngineSerialNumber(this.mcm);
        this.targetVIN = SapiManager.GetVehicleIdentificationNumber(this.mcm);
        string text = this.textBoxSerialNumber1.Text;
        if (ConfirmationDialog.Show(this.targetESN, this.targetVIN, text))
        {
          this.ClearOutput();
          this.Report(Resources.Message_SCRAccumulatorsResetStarted);
          this.Report(Resources.Message_VIN + this.targetVIN);
          this.Report(Resources.Message_ESN + this.targetESN);
          this.Report(Resources.Message_SerialNumber + text);
          this.CurrentStage = UserPanel.Stage.ResetValues;
          this.PerformCurrentStage();
          break;
        }
        this.StopWork(UserPanel.Reason.Canceled);
        break;
      case UserPanel.Stage.ResetValues:
        this.Report(Resources.Message_WritingNewValue);
        if (this.ResetAccumulators())
        {
          this.CurrentStage = UserPanel.Stage.WaitingForReset;
          break;
        }
        this.StopWork(UserPanel.Reason.FailedWrite);
        break;
      case UserPanel.Stage.WaitingForReset:
        this.CurrentStage = UserPanel.Stage.CommitChanges;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.CommitChanges:
        this.CurrentStage = UserPanel.Stage.WaitingForCommit;
        this.CommitToPermanentMemory();
        break;
      case UserPanel.Stage.WaitingForCommit:
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.Finish:
        this.StopWork(UserPanel.Reason.Success);
        break;
    }
  }

  private void CommitToPermanentMemory()
  {
    if (this.acm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
    {
      this.Report(Resources.Message_CommittingChanges);
      this.acm.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnCommitCompleteEvent);
      this.acm.Services.Execute(this.acm.Ecu.Properties["CommitToPermanentMemoryService"], false);
    }
    else
    {
      this.Report(Resources.Message_NoCommitServiceAvailable);
      this.StopWork(UserPanel.Reason.FailedCommit);
    }
  }

  private void OnCommitCompleteEvent(object sender, ResultEventArgs e)
  {
    this.acm.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnCommitCompleteEvent);
    if (e.Succeeded)
      this.PerformCurrentStage();
    else
      this.StopWork(UserPanel.Reason.FailedCommit);
  }

  private void StopWork(UserPanel.Reason reason)
  {
    if (this.CurrentStage != UserPanel.Stage.Stopping && this.CurrentStage != UserPanel.Stage.Idle)
    {
      UserPanel.Stage currentStage = this.CurrentStage;
      this.CurrentStage = UserPanel.Stage.Stopping;
      if (reason == UserPanel.Reason.Success)
      {
        this.AddStationLogEntry(this.textBoxSerialNumber1.Text);
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
            this.Report(Resources.Message_FailedToWriteTheAccumulators);
            break;
          case UserPanel.Reason.FailedWrite:
            this.Report(Resources.Message_FailedToCommitTheChangesToTheACMYouMayNeedToRepeatThisProcedure);
            break;
          case UserPanel.Reason.Closing:
            this.Report(Resources.Message_OneOrMoreDevicesDisconnected);
            this.textBoxSerialNumber1.Text = string.Empty;
            break;
          case UserPanel.Reason.Disconnected:
            this.Report(Resources.Message_TheUserCanceledTheOperation);
            break;
        }
      }
      this.CurrentStage = UserPanel.Stage.Idle;
    }
    this.UpdateWarningMessage();
  }

  private void AddStationLogEntry(string serialNumber)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(serialNumber);
    stringBuilder.Append(";");
    switch (this.AtsType)
    {
      case AtsType.OneBox:
        stringBuilder.Append("a");
        break;
      case AtsType.TwoBox:
        stringBuilder.Append("b");
        break;
    }
    if (stringBuilder.Length > this.MaxLengthForRefText)
      throw new InvalidOperationException("String too long for server event.");
    ServerDataManager.UpdateEventsFile(this.acm, "SCRReset", this.targetESN, this.targetVIN, "OK", "DESCRIPTION", stringBuilder.ToString());
  }

  private bool ResetAccumulators()
  {
    bool flag = false;
    if (this.accumulators.Count == UserPanel.acmScrAccumulatorQualifiers.Count)
    {
      Cursor.Current = Cursors.WaitCursor;
      foreach (DataItem accumulator in this.accumulators)
        accumulator.WriteValue((object) 0);
      this.acm.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
      this.acm.Parameters.Write(false);
      flag = true;
    }
    else
      this.Report(Resources.Message_FailedToObtainMCMAshDistanceAccumulator);
    return flag;
  }

  private void OnParametersWriteComplete(object sender, ResultEventArgs e)
  {
    ParameterCollection parameterCollection = (ParameterCollection) sender;
    parameterCollection.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
    bool flag = false;
    if (e.Succeeded)
    {
      foreach (string accumulatorQualifier in (IEnumerable<string>) UserPanel.acmScrAccumulatorQualifiers)
      {
        Parameter parameter = parameterCollection[accumulatorQualifier];
        flag |= !this.CheckParameterWriteStatus(parameter);
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

  private bool CheckParameterWriteStatus(Parameter parameter)
  {
    if (parameter.Exception != null)
    {
      if (parameter.Exception is CaesarException exception && exception.ErrorNumber == 6602L)
      {
        this.Report($"{Resources.Message_WhileResettingTheAccumulatorsTheFollowingWarningWasReported}\r\n{parameter.Name}: {exception.Message}");
      }
      else
      {
        this.Report($"{Resources.Message_WhileResettingTheAccumulatorsTheFollowingErrorWasReported}\r\n{parameter.Name}: {parameter.Exception.Message}");
        return false;
      }
    }
    return true;
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
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.labelLicenseMessage = new System.Windows.Forms.Label();
    this.labelWarning = new System.Windows.Forms.Label();
    this.titleLabel = new ScalingLabel();
    this.tableLayoutSerialNumber = new TableLayoutPanel();
    this.labelSerialNumberHeader = new System.Windows.Forms.Label();
    this.textBoxSerialNumber1 = new TextBox();
    this.labelSNErrorMessage1 = new System.Windows.Forms.Label();
    this.buttonPerformAction = new Button();
    this.labelProgress = new System.Windows.Forms.Label();
    this.textBoxProgress = new TextBox();
    this.buttonClose = new Button();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    this.flowLayoutPanel1.SuspendLayout();
    ((Control) this.tableLayoutSerialNumber).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.flowLayoutPanel1, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.titleLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutSerialNumber, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonPerformAction, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelProgress, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.textBoxProgress, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonClose, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutPanel1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrument5, 1, 2);
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
    componentResourceManager.ApplyResources((object) this.tableLayoutSerialNumber, "tableLayoutSerialNumber");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutSerialNumber, 2);
    ((TableLayoutPanel) this.tableLayoutSerialNumber).Controls.Add((Control) this.labelSerialNumberHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutSerialNumber).Controls.Add((Control) this.textBoxSerialNumber1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutSerialNumber).Controls.Add((Control) this.labelSNErrorMessage1, 2, 1);
    ((Control) this.tableLayoutSerialNumber).Name = "tableLayoutSerialNumber";
    componentResourceManager.ApplyResources((object) this.labelSerialNumberHeader, "labelSerialNumberHeader");
    this.labelSerialNumberHeader.BackColor = SystemColors.ControlDark;
    ((TableLayoutPanel) this.tableLayoutSerialNumber).SetColumnSpan((Control) this.labelSerialNumberHeader, 3);
    this.labelSerialNumberHeader.Name = "labelSerialNumberHeader";
    this.labelSerialNumberHeader.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxSerialNumber1, "textBoxSerialNumber1");
    this.textBoxSerialNumber1.Name = "textBoxSerialNumber1";
    componentResourceManager.ApplyResources((object) this.labelSNErrorMessage1, "labelSNErrorMessage1");
    this.labelSNErrorMessage1.ForeColor = Color.Red;
    this.labelSNErrorMessage1.Name = "labelSNErrorMessage1";
    this.labelSNErrorMessage1.UseCompatibleTextRendering = true;
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
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 2, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "Time_Above_SCR_Inlet_Temp_1_Hour");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "Time_Above_SCR_Inlet_Temp_1_Min");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "Time_Above_SCR_Inlet_Temp_1_Sec");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "Time_Above_SCR_Inlet_Temp_2");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "Time_Above_SCR_Outlet_Temp");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SCRReplacement");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    ((Control) this.tableLayoutSerialNumber).ResumeLayout(false);
    ((Control) this.tableLayoutSerialNumber).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private class ValidationInformation
  {
    public readonly Regex Regex;

    public ValidationInformation(Regex regex) => this.Regex = regex;
  }

  private enum Stage
  {
    Idle = 0,
    GetConfirmation = 1,
    _Start = 1,
    ResetValues = 2,
    WaitingForReset = 3,
    CommitChanges = 4,
    WaitingForCommit = 5,
    Finish = 6,
    Stopping = 7,
  }

  private enum Reason
  {
    Success,
    FailedWrite,
    FailedCommit,
    Closing,
    Disconnected,
    Canceled,
  }
}
