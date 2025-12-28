// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Turbo_Actuator.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Turbo_Actuator.panel;

public class UserPanel : CustomPanel
{
  private const string softwareVersionQualifier = "CO_SoftwareVersion";
  private const string firstSoftwareVersion = "7.6.8.0";
  private const string firstSoftwareVersionWithHysteresis = "7.8.2.0";
  private const string sra5StatusCodeInstrumentQualifier = "DT_AS052_SRA5_Status_Code";
  private const string preInstallationStartQualifier = "RT_SR061_Pre_install_Routine_Start_ActuatorStatus";
  private const int preInstallationStartInputValue1ChoiceRawValue = 5;
  private const int preInstallationStartInputValue2Value = 90;
  private const int preInstallationStartOutputValueForSuccess = 1;
  private const int preInstallationStartOutputValueForFailure = 2;
  private const string preInstallationStatusQualifier = "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult";
  private const string preInstallationStopQualifier = "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber";
  private const int preInstallationStopInputValue1ChoiceRawValue = 5;
  private const string selfCalibrationStartQualifier = "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus";
  private const int selfCalibrationStartInputValue1ChoiceRawValue = 5;
  private const int selfCalibrationStartOutputValueForSuccess = 1;
  private const int selfCalibrationStartOutputValueForFailure = 2;
  private const string selfCalibrationStatusQualifier = "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus";
  private const int selfCalibrationStatusInputValue1ChoiceRawValue = 5;
  private const string selfCalibrationStopQualifier = "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber";
  private const int selfCalibrationStopInputValue1ChoiceRawValue = 5;
  private const string hysteresisTestStartQualifier = "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus";
  private const int hysteresisTestStartInputValue1ChoiceRawValue = 5;
  private const int hysteresisTestStartOutputValueForSuccess = 1;
  private const int hysteresisTestStartOutputValueForFailure = 2;
  private const string hysteresisTestDataQualifier = "RT_SR063_Hysteres_Test_Routine_Request_Results_Data";
  private const int hysteresisTestDataInputValue1ChoiceRawValue = 5;
  private const string hysteresisTestStopQualifier = "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber";
  private const int hysteresisTestStopInputValue1ChoiceRawValue = 5;
  private const int UIStateCount = 14;
  private Dictionary<string, CacheInfo> snapshot;
  private bool forcePreInstallationStop = false;
  private bool forceSelfCalibrationStop = false;
  private int hysteresisTestDataCacheTimeRestore;
  private bool flagDisplaySra5StatusCode = true;
  private bool forceHysteresisTestStop = false;
  private UserPanel.HysteresisTest hysteresisTest = new UserPanel.HysteresisTest();
  private UserPanel.UIState[] stateInfo = new UserPanel.UIState[14];
  private Channel mcmChannel = (Channel) null;
  private UserPanel.CurrentOperation currentOperation;
  private int stepNumber;
  private UserPanel.OperationPhase operationPhase;
  private UserPanel.OperationCallState operationCallState;
  private Timer operationTimer;
  private bool hysteresisOnly = false;
  private bool hysteresisSuccessful = false;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonStart;
  private Button buttonNext;
  private Button buttonPrevious;
  private Button buttonStop;
  private Button buttonHysteresisTest;
  private Button buttonPreInstallation;
  private Button buttonSelfCalibration;
  private PictureBox picWait;
  private PictureBox picHysteresis;
  private PictureBox picGrease;
  private PictureBox picBlank;
  private PictureBox picError;
  private PictureBox picMount;
  private PictureBox picNotMounted;
  private PictureBox picOk;
  private PictureBox picAlignHoles;
  private Panel panel3;
  private TableLayoutPanel tableLayoutPanel2;
  private TableLayoutPanel tableLayoutPanel3;
  private TextBox textboxInstructions;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label lblInstructions;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label lblStatus;
  private Button closeButton;
  private TextBox textboxStatus;

  public UserPanel() => this.InitializeComponent();

  private bool Connected
  {
    get
    {
      return this.mcmChannel != null && this.mcmChannel.CommunicationsState != CommunicationsState.Offline;
    }
  }

  private UserPanel.SoftwareComparisonResult CompareSoftwareVersions(
    string currentVersion,
    string referenceVersion)
  {
    int startIndex1 = 0;
    int startIndex2 = 0;
    bool flag1 = false;
    bool flag2 = false;
    while (!flag1 && !flag2)
    {
      int num1 = currentVersion.IndexOf('.', startIndex1);
      int num2 = referenceVersion.IndexOf('.', startIndex2);
      if (num1 < 0)
      {
        num1 = currentVersion.Length;
        flag1 = true;
      }
      if (num2 < 0)
      {
        num2 = referenceVersion.Length;
        flag2 = true;
      }
      int int32_1 = Convert.ToInt32(currentVersion.Substring(startIndex1, num1 - startIndex1));
      int int32_2 = Convert.ToInt32(referenceVersion.Substring(startIndex2, num2 - startIndex2));
      if (int32_1 < int32_2)
        return UserPanel.SoftwareComparisonResult.Older;
      if (int32_1 > int32_2)
        return UserPanel.SoftwareComparisonResult.Newer;
      startIndex1 = num1 + 1;
      startIndex2 = num2 + 1;
    }
    if (!flag1)
      return UserPanel.SoftwareComparisonResult.Newer;
    return flag2 ? UserPanel.SoftwareComparisonResult.Identical : UserPanel.SoftwareComparisonResult.Older;
  }

  private bool ValidEngineType
  {
    get
    {
      IEnumerable<EquipmentType> source = EquipmentType.ConnectedEquipmentTypes("Engine");
      if (!CollectionExtensions.Exactly<EquipmentType>(source, 1))
        return false;
      EquipmentType equipmentType = source.First<EquipmentType>();
      return ((EquipmentType) ref equipmentType).Name == "S60";
    }
  }

  private bool EcuFullySupported
  {
    get
    {
      bool ecuFullySupported = false;
      EcuInfo ecuInfo = this.mcmChannel.EcuInfos["CO_SoftwareVersion"];
      if (ecuInfo != null)
      {
        string empty = string.Empty;
        if (this.CompareSoftwareVersions(ecuInfo.Value, "7.8.2.0") != UserPanel.SoftwareComparisonResult.Older)
        {
          Service service1 = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus");
          Service service2 = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
          Service service3 = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber");
          ecuFullySupported = this.EcuPartiallySupported && service1 != (Service) null && service2 != (Service) null && service3 != (Service) null;
        }
      }
      return ecuFullySupported;
    }
  }

  private bool EcuPartiallySupported
  {
    get
    {
      bool partiallySupported = false;
      EcuInfo ecuInfo = this.mcmChannel.EcuInfos["CO_SoftwareVersion"];
      if (ecuInfo != null && this.CompareSoftwareVersions(ecuInfo.Value, "7.6.8.0") != UserPanel.SoftwareComparisonResult.Older)
      {
        Service service1 = this.GetService("MCM", "RT_SR061_Pre_install_Routine_Start_ActuatorStatus");
        Service service2 = this.GetService("MCM", "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult");
        Service service3 = this.GetService("MCM", "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber");
        Service service4 = this.GetService("MCM", "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus");
        Service service5 = this.GetService("MCM", "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus");
        Service service6 = this.GetService("MCM", "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber");
        partiallySupported = service1 != (Service) null && service2 != (Service) null && service3 != (Service) null && service4 != (Service) null && service5 != (Service) null && service6 != (Service) null;
      }
      return partiallySupported;
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.buttonPreInstallation.Click += new EventHandler(this.OnPreInstallation_Click);
    this.buttonSelfCalibration.Click += new EventHandler(this.OnSelfCalibration_Click);
    this.buttonHysteresisTest.Click += new EventHandler(this.OnHysteresisTest_Click);
    this.buttonNext.Click += new EventHandler(this.OnNext_Click);
    this.buttonPrevious.Click += new EventHandler(this.OnPrevious_Click);
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.stateInfo[0] = new UserPanel.UIState(UserPanel.CurrentOperation.Disconnected, 0, false, false, false, false, false, false, false, false, Resources.Message_MCMNotConnected, this.picError);
    this.stateInfo[1] = new UserPanel.UIState(UserPanel.CurrentOperation.ConnectedIdle, 0, false, false, false, false, false, false, false, false, Resources.Message_MCMConnected, (PictureBox) null);
    this.stateInfo[2] = new UserPanel.UIState(UserPanel.CurrentOperation.PreInstallation, 1, false, true, true, true, true, true, false, true, $"{Resources.Message_EnsureTheOutputGearIsUnimpeded}\r\n\r\n{Resources.ClickNextToProceed}", this.picNotMounted);
    this.stateInfo[3] = new UserPanel.UIState(UserPanel.CurrentOperation.PreInstallation, 2, true, true, true, true, false, true, true, true, Resources.Message_ClickStartToMoveTheGearIntoInitialPositionOrPreviousToGoBack, this.picBlank);
    this.stateInfo[4] = new UserPanel.UIState(UserPanel.CurrentOperation.PreInstallation, 3, false, true, true, true, false, true, false, true, $"{Resources.Message_PreInstallationRunning}\r\n\r\n{Resources.ClickStopToAbortOperation}", this.picWait);
    this.stateInfo[5] = new UserPanel.UIState(UserPanel.CurrentOperation.SelfCalibration, 1, false, true, true, true, true, true, false, true, $"{Resources.Message_ImportantPreInstallationMustBePerformedBeforeRunningASelfCalibration}\r\n\r\n{Resources.CleanAnyDebrisFromTheSectorGearAndApplyTheCorrectGrease}\r\n\r\n{Resources.ClickNextToProceed}", this.picGrease);
    this.stateInfo[6] = new UserPanel.UIState(UserPanel.CurrentOperation.SelfCalibration, 2, false, true, true, true, true, true, true, true, $"{Resources.Message_PositionTurboSectorGearToCorrespondWithTheActuatorOutputGearByAligningTheReferenceHoles}\r\n{Resources.RemoveThePinBeforeMounting}\r\n\r\n{Resources.ClickNextToProceedOrPreviousToGoBack}", this.picAlignHoles);
    this.stateInfo[7] = new UserPanel.UIState(UserPanel.CurrentOperation.SelfCalibration, 3, false, true, true, true, true, true, true, true, $"{Resources.Message_MountTheActuatorSecurelyOntoTheTurboMakingSureTheNozzleIsInTheRecommendedPosition}\r\n\r\n{Resources.ClickNextToProceedOrPreviousToGoBack}", this.picMount);
    this.stateInfo[8] = new UserPanel.UIState(UserPanel.CurrentOperation.SelfCalibration, 4, true, true, true, true, false, true, true, true, Resources.Message_ClickStartToExecuteTheSelfCalibrationProcessOrPreviousToGoBack, this.picBlank);
    this.stateInfo[9] = new UserPanel.UIState(UserPanel.CurrentOperation.SelfCalibration, 5, false, true, true, true, false, true, false, true, $"{Resources.Message_SelfCalibrationRunning}\r\n\r\n{Resources.ClickStopToAbortOperation}", this.picWait);
    this.stateInfo[10] = new UserPanel.UIState(UserPanel.CurrentOperation.HysteresisTest, 1, true, true, true, true, false, true, false, true, Resources.Message_ClickStartToCheckForFreeNozzleMovementOrPreviousToGoBack, this.picBlank);
    this.stateInfo[11] = new UserPanel.UIState(UserPanel.CurrentOperation.HysteresisTest, 2, false, true, true, true, false, true, false, true, $"{Resources.Message_HysteresisTestRunning}\r\n\r\n{Resources.ClickStopToAbortOperation}", this.picWait);
    this.stateInfo[12] = new UserPanel.UIState(UserPanel.CurrentOperation.ConnectedDisabled, 0, false, false, false, false, false, false, false, false, Resources.Message_MCMConnectedPanelDisabled, (PictureBox) null);
    this.stateInfo[13] = new UserPanel.UIState(UserPanel.CurrentOperation.ConnectedNoHysteresis, 0, false, false, false, false, false, false, false, false, Resources.Message_MCMConnectedHysteresisTestDisabled, (PictureBox) null);
    this.ClearUserInterfaceState();
    if (((Control) this).Tag != null)
    {
      this.hysteresisOnly = true;
      this.StartHysteresisTest();
    }
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.hysteresisSuccessful,
      (object) this.textboxStatus.Text
    };
  }

  public virtual void OnChannelsChanged()
  {
    Channel channel = this.GetChannel("MCM");
    if (this.mcmChannel == channel)
      return;
    if (this.mcmChannel != null)
      this.mcmChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.mcmChannel = channel;
    if (this.mcmChannel != null)
      this.mcmChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.UpdateUserInterface();
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (this.currentOperation != UserPanel.CurrentOperation.ConnectedDisabled)
      return;
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.ClearStatus();
    if (this.Connected)
    {
      if (!this.ValidEngineType)
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedDisabled;
        this.stepNumber = 0;
        this.AppendStatus(Resources.Message_MCMConnectedButPanelWasNotReleasedForThisEngineType, UserPanel.PictureStatus.NoChange);
      }
      else if (this.EcuFullySupported)
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedIdle;
        this.stepNumber = 0;
        this.ClearStatus();
        this.hysteresisTestDataCacheTimeRestore = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data").CacheTimeout;
        this.AppendStatus(Resources.Message_MCMConnected1, UserPanel.PictureStatus.ShowBlank);
      }
      else if (this.EcuPartiallySupported)
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedNoHysteresis;
        this.stepNumber = 0;
        this.AppendStatus(Resources.Message_MCMConnectedButHysteresisTestIsDisabled, UserPanel.PictureStatus.ShowBlank);
      }
      else
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedDisabled;
        this.stepNumber = 0;
        this.AppendStatus(Resources.Message_MCMConnectedButPanelIsDisabledForThisSofwareVersion, UserPanel.PictureStatus.NoChange);
      }
    }
    else
    {
      this.currentOperation = UserPanel.CurrentOperation.Disconnected;
      this.stepNumber = 0;
      this.AppendStatus(Resources.Message_MCMDisconnected, UserPanel.PictureStatus.NoChange);
    }
    this.UpdateControls();
  }

  private UserPanel.UIState RetrieveUIStateObject()
  {
    for (int index = 0; index < 14; ++index)
    {
      UserPanel.UIState uiState = this.stateInfo[index];
      if (uiState.CurrentOperation == this.currentOperation && uiState.StepNumber == this.stepNumber)
        return uiState;
    }
    string.Format(Resources.MessageFormat_UIStateIsNull01, (object) this.currentOperation.ToString(), (object) this.stepNumber.ToString());
    return (UserPanel.UIState) null;
  }

  private void ClearUserInterfaceState()
  {
    this.buttonStart.Click -= new EventHandler(this.OnPreInstallationStart_Click);
    this.buttonStop.Click -= new EventHandler(this.OnPreInstallationStop_Click);
    this.buttonStart.Click -= new EventHandler(this.OnSelfCalibrationStart_Click);
    this.buttonStop.Click -= new EventHandler(this.OnSelfCalibrationStop_Click);
    this.buttonStart.Click -= new EventHandler(this.OnHysteresisTestStart_Click);
    this.buttonStop.Click -= new EventHandler(this.OnHysteresisTestStop_Click);
    if (this.Connected)
    {
      if (!this.ValidEngineType)
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedDisabled;
        this.stepNumber = 0;
      }
      else if (this.EcuFullySupported)
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedIdle;
        this.stepNumber = 0;
      }
      else if (this.EcuPartiallySupported)
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedNoHysteresis;
        this.stepNumber = 0;
      }
      else
      {
        this.currentOperation = UserPanel.CurrentOperation.ConnectedDisabled;
        this.stepNumber = 0;
      }
    }
    else
    {
      this.currentOperation = UserPanel.CurrentOperation.Disconnected;
      this.stepNumber = 0;
    }
    this.UpdateControls();
  }

  private void UpdateControls()
  {
    UserPanel.UIState uiState = this.RetrieveUIStateObject();
    switch (this.currentOperation)
    {
      case UserPanel.CurrentOperation.ConnectedNoHysteresis:
        this.buttonPreInstallation.Enabled = true;
        this.buttonSelfCalibration.Enabled = true;
        this.buttonHysteresisTest.Enabled = false;
        break;
      case UserPanel.CurrentOperation.ConnectedIdle:
        this.buttonPreInstallation.Enabled = true;
        this.buttonSelfCalibration.Enabled = true;
        this.buttonHysteresisTest.Enabled = true;
        break;
      default:
        this.buttonPreInstallation.Enabled = false;
        this.buttonSelfCalibration.Enabled = false;
        this.buttonHysteresisTest.Enabled = false;
        break;
    }
    this.buttonPreInstallation.Enabled = !this.hysteresisOnly && this.buttonPreInstallation.Enabled;
    this.buttonSelfCalibration.Enabled = !this.hysteresisOnly && this.buttonSelfCalibration.Enabled;
    if (uiState == null)
      return;
    this.buttonStart.Visible = uiState.StartButtonVisible;
    this.buttonStart.Enabled = uiState.StartButtonActive;
    this.buttonStop.Visible = uiState.StopButtonVisible;
    this.buttonStop.Enabled = uiState.StopButtonActive;
    this.buttonNext.Visible = uiState.NextButtonVisible;
    this.buttonNext.Enabled = uiState.NextButtonActive;
    this.buttonPrevious.Visible = uiState.PreviousButtonVisible;
    this.buttonPrevious.Enabled = uiState.PreviousButtonActive;
    this.textboxInstructions.Text = uiState.Instruction;
    this.ShowPicture(uiState.Picture);
  }

  private void AppendStatus(string message, UserPanel.PictureStatus pictureStatus)
  {
    TextBox textboxStatus = this.textboxStatus;
    textboxStatus.Text = textboxStatus.Text + message + Environment.NewLine;
    switch (pictureStatus)
    {
      case UserPanel.PictureStatus.ShowSuccess:
        this.ShowPicture(this.picOk);
        break;
      case UserPanel.PictureStatus.ShowError:
        this.ShowPicture(this.picError);
        break;
      case UserPanel.PictureStatus.ShowBlank:
        this.ShowPicture(this.picBlank);
        break;
    }
    this.textboxStatus.SelectionStart = this.textboxStatus.Text.Length - 1;
    this.textboxStatus.SelectionLength = 0;
    this.textboxStatus.ScrollToCaret();
  }

  private void ClearStatus() => this.textboxStatus.Text = string.Empty;

  private void ShowPicture(PictureBox pictureBox)
  {
    if (pictureBox == null)
      return;
    this.HideAllPictures();
    pictureBox.Visible = true;
  }

  private void HideAllPictures()
  {
    this.picBlank.Visible = false;
    this.picWait.Visible = false;
    this.picOk.Visible = false;
    this.picError.Visible = false;
    this.picAlignHoles.Visible = false;
    this.picGrease.Visible = false;
    this.picHysteresis.Visible = false;
    this.picMount.Visible = false;
    this.picNotMounted.Visible = false;
  }

  private void OnNext_Click(object sender, EventArgs e)
  {
    ++this.stepNumber;
    this.UpdateControls();
  }

  private void OnPrevious_Click(object sender, EventArgs e)
  {
    --this.stepNumber;
    this.UpdateControls();
  }

  private void OnPreInstallation_Click(object sender, EventArgs e)
  {
    this.currentOperation = UserPanel.CurrentOperation.PreInstallation;
    this.stepNumber = 1;
    this.ClearStatus();
    this.AppendStatus(Resources.Message_PreInstallationOperation, UserPanel.PictureStatus.NoChange);
    this.buttonStart.Click += new EventHandler(this.OnPreInstallationStart_Click);
    this.buttonStop.Click += new EventHandler(this.OnPreInstallationStop_Click);
    this.UpdateControls();
  }

  private void OnSelfCalibration_Click(object sender, EventArgs e)
  {
    this.currentOperation = UserPanel.CurrentOperation.SelfCalibration;
    this.stepNumber = 1;
    this.ClearStatus();
    this.AppendStatus(Resources.Message_SelfCalibrationOperation, UserPanel.PictureStatus.NoChange);
    this.buttonStart.Click += new EventHandler(this.OnSelfCalibrationStart_Click);
    this.buttonStop.Click += new EventHandler(this.OnSelfCalibrationStop_Click);
    this.UpdateControls();
  }

  private void OnHysteresisTest_Click(object sender, EventArgs e) => this.StartHysteresisTest();

  private void StartHysteresisTest()
  {
    this.currentOperation = UserPanel.CurrentOperation.HysteresisTest;
    this.stepNumber = 1;
    this.ClearStatus();
    this.AppendStatus(Resources.Message_HysteresisTestOperation, UserPanel.PictureStatus.NoChange);
    this.buttonStart.Click += new EventHandler(this.OnHysteresisTestStart_Click);
    this.buttonStop.Click += new EventHandler(this.OnHysteresisTestStop_Click);
    this.UpdateControls();
  }

  private void operationTimer_Tick(object sender, EventArgs e)
  {
    if (!this.Connected || this.currentOperation == UserPanel.CurrentOperation.ConnectedIdle)
    {
      this.StopTimer();
    }
    else
    {
      if (this.operationCallState != UserPanel.OperationCallState.Ready)
        return;
      if (this.currentOperation == UserPanel.CurrentOperation.PreInstallation)
      {
        if (this.forcePreInstallationStop)
        {
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.ExecutePreInstallationStop();
        }
        else if (this.operationPhase == UserPanel.OperationPhase.Start)
          this.ExecutePreInstallationStart();
        else if (this.operationPhase == UserPanel.OperationPhase.Status)
          this.ExecutePreInstallationStatus();
        else if (this.operationPhase == UserPanel.OperationPhase.Stop)
          this.ExecutePreInstallationStop();
      }
      else if (this.currentOperation == UserPanel.CurrentOperation.SelfCalibration)
      {
        if (this.forceSelfCalibrationStop)
        {
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.ExecuteSelfCalibrationStop();
        }
        else if (this.operationPhase == UserPanel.OperationPhase.Start)
          this.ExecuteSelfCalibrationStart();
        else if (this.operationPhase == UserPanel.OperationPhase.Status)
          this.ExecuteSelfCalibrationStatus();
        else if (this.operationPhase == UserPanel.OperationPhase.Stop)
          this.ExecuteSelfCalibrationStop();
      }
      else if (this.currentOperation == UserPanel.CurrentOperation.HysteresisTest)
      {
        if (this.forceHysteresisTestStop)
        {
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.ExecuteHysteresisTestStop();
        }
        else if (this.operationPhase == UserPanel.OperationPhase.Start)
          this.ExecuteHysteresisTestStart();
        else if (this.operationPhase == UserPanel.OperationPhase.Data)
          this.ExecuteHysteresisTestData();
        else if (this.operationPhase == UserPanel.OperationPhase.Stop)
          this.ExecuteHysteresisTestStop();
      }
    }
  }

  private void StopTimer()
  {
    this.operationPhase = UserPanel.OperationPhase.None;
    this.operationCallState = UserPanel.OperationCallState.Ready;
    if (this.operationTimer == null)
      return;
    this.operationTimer.Stop();
    this.operationTimer = (Timer) null;
  }

  private void DisplaySra5StatusCode()
  {
    if (!this.Connected || !this.flagDisplaySra5StatusCode)
      return;
    int int32;
    do
    {
      int32 = Convert.ToInt32(this.GetInstrument("MCM", "DT_AS052_SRA5_Status_Code").InstrumentValues.Current.ToString());
    }
    while (int32 == 7);
    if (int32 != 0)
    {
      switch (int32)
      {
        case 0:
          this.AppendStatus(Resources.Message_SRA5StatusCode0NormalOperation, UserPanel.PictureStatus.NoChange);
          break;
        case 1:
          this.AppendStatus(Resources.Message_SRA5StatusCode1TemperatureInExcessOfFaultThreshold, UserPanel.PictureStatus.ShowError);
          break;
        case 2:
          this.AppendStatus(Resources.Message_SRA5StatusCode2TemperatureInExcessOfWarningThreshold, UserPanel.PictureStatus.ShowError);
          break;
        case 3:
          this.AppendStatus(Resources.Message_SRA5StatusCode3SlowToCloseErrorOrUnableToCloseError, UserPanel.PictureStatus.ShowError);
          break;
        case 4:
          this.AppendStatus(Resources.Message_SRA5StatusCode4RestrictionDetectedAtLearn, UserPanel.PictureStatus.ShowError);
          break;
        case 5:
          this.AppendStatus(Resources.Message_SRA5StatusCode5MotorDisabledOperationCannotContinueDueToDetectedFaultCondition, UserPanel.PictureStatus.ShowError);
          break;
        case 7:
          this.AppendStatus(Resources.Message_SRA5StatusCode7TheSRAIsRunningAnInternalTestSequence, UserPanel.PictureStatus.ShowError);
          break;
        case 8:
          this.AppendStatus(Resources.Message_SRA5StatusCode8NoValidCANCommandFor75MsecAndValidPreviouslyReceived, UserPanel.PictureStatus.ShowError);
          break;
        case 9:
          this.AppendStatus(Resources.Message_SRA5StatusCode9DetectedLearnSpanBetween0And100IsTooLarge, UserPanel.PictureStatus.ShowError);
          break;
        case 10:
          this.AppendStatus(Resources.Message_SRA5StatusCode10Either100StopIsTooHighOr0StopIsTooLowAndSpanTooLargeHasNotOccurred, UserPanel.PictureStatus.ShowError);
          break;
        case 11:
          this.AppendStatus(Resources.Message_SRA5StatusCode11NoValidCommandSourceHasBeenSeenByTheSRASincePoweringUpAndNoCommandSourceTimeHasPassed, UserPanel.PictureStatus.ShowError);
          break;
        case 15:
          this.AppendStatus(Resources.Message_SRA5StatusCode15IgnitionVoltageHasSustainedAtTooLowLevelForAnExcessivePeriod, UserPanel.PictureStatus.ShowError);
          break;
        case 17:
          this.AppendStatus(Resources.Message_SRA5StatusCode17CommandPWMSignalIsNotValid, UserPanel.PictureStatus.ShowError);
          break;
        case 22:
          this.AppendStatus(Resources.Message_SRA5StatusCode22ActuationHasTakenInternalActionToAvoidOverheatingAndDamagingItsMotorRequiredTorqueMayNotBeAvailableAtActuatorOutput, UserPanel.PictureStatus.ShowError);
          break;
        case 23:
          this.AppendStatus(Resources.Message_SRA5StatusCode23ReferenceNotDetectedDuringLearnDueToMechanicalSystemBindingOrInternalConditionTruePositionNotKnown, UserPanel.PictureStatus.ShowError);
          break;
        case 30:
          this.AppendStatus(Resources.Message_SRA5StatusCode30NoValidUARTCommandFor262MsecAndValidPreviouslyReceived, UserPanel.PictureStatus.ShowError);
          break;
        default:
          this.AppendStatus(string.Format(Resources.MessageFormat_SRA5StatusCode0UnknownValue, (object) int32), UserPanel.PictureStatus.NoChange);
          break;
      }
    }
  }

  private void SetupInstruments()
  {
    if (!this.Connected)
      return;
    this.snapshot = InstrumentCacheManager.GenerateSnapshot(this.mcmChannel);
    InstrumentCacheManager.UnmarkAllInstruments(this.mcmChannel);
    InstrumentCacheManager.MarkInstrument(this.mcmChannel, "DT_AS052_SRA5_Status_Code", (ushort) 10);
    this.mcmChannel.FaultCodes.AutoRead = false;
  }

  private void RestoreInstruments()
  {
    if (!this.Connected || this.snapshot == null)
      return;
    InstrumentCacheManager.ApplySnapshot(this.mcmChannel, this.snapshot);
    this.mcmChannel.FaultCodes.AutoRead = true;
  }

  private void SetupHysteresisServiceRoutines()
  {
    if (!this.Connected)
      return;
    this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data").CacheTimeout = 0;
  }

  private void RestoreHysteresisServiceRoutines()
  {
    if (!this.Connected)
      return;
    this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data").CacheTimeout = this.hysteresisTestDataCacheTimeRestore;
  }

  private void OnPreInstallationStart_Click(object sender, EventArgs e)
  {
    ++this.stepNumber;
    this.UpdateControls();
    this.AppendStatus(Resources.Message_PerformingPreInstallation, UserPanel.PictureStatus.NoChange);
    this.operationPhase = UserPanel.OperationPhase.Start;
    this.operationCallState = UserPanel.OperationCallState.Ready;
    this.forcePreInstallationStop = false;
    this.operationTimer = new Timer();
    this.operationTimer.Interval = 1000;
    this.operationTimer.Tick += new EventHandler(this.operationTimer_Tick);
    this.operationTimer.Start();
  }

  private void ExecutePreInstallationStart()
  {
    if (!this.Connected)
      return;
    Service service = this.GetService("MCM", "RT_SR061_Pre_install_Routine_Start_ActuatorStatus");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.PreInstallationStart_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    service.InputValues[1].Value = (object) 90;
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void PreInstallationStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    bool flag = false;
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.PreInstallationStart_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      if (Convert.ToInt32(choice.RawValue) == 1)
      {
        this.AppendStatus(Resources.Message_PreInstallationStartedSuccessfully, UserPanel.PictureStatus.NoChange);
        this.operationPhase = UserPanel.OperationPhase.Status;
        this.operationCallState = UserPanel.OperationCallState.Ready;
        flag = true;
      }
      else if (Convert.ToInt32(choice.RawValue) == 2)
        this.AppendStatus(Resources.Message_ErrorPreInstallationStartProcessFailed, UserPanel.PictureStatus.ShowError);
      else
        this.AppendStatus(string.Format(Resources.MessageFormat_ErrorPreInstallationStartProcessFailedWithOutputValue0, (object) choice), UserPanel.PictureStatus.ShowError);
    }
    else
      this.AppendStatus(Resources.Message_ErrorPreInstallationStartServiceCallFailed, UserPanel.PictureStatus.ShowError);
    if (flag)
      return;
    this.StopTimer();
    this.ClearUserInterfaceState();
  }

  private void ExecutePreInstallationStatus()
  {
    if (!this.Connected)
      return;
    Service service = this.GetService("MCM", "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.PreInstallationStatus_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private UserPanel.ActuatorResultStatus GetResultStatus(ServiceOutputValue output)
  {
    Choice choice = output.Value as Choice;
    if (choice != (object) null)
    {
      switch (Convert.ToInt32(choice.RawValue))
      {
        case 1:
          return UserPanel.ActuatorResultStatus.Done;
        case 2:
        case 3:
        case 4:
          return UserPanel.ActuatorResultStatus.InProgress;
        case 5:
        case 6:
        case 7:
        case 10:
          return UserPanel.ActuatorResultStatus.Aborted;
        case 8:
          return UserPanel.ActuatorResultStatus.NotStarted;
        case 9:
          return UserPanel.ActuatorResultStatus.NoCommunication;
      }
    }
    return UserPanel.ActuatorResultStatus.Unknown;
  }

  private void PreInstallationStatus_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.PreInstallationStatus_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.flagDisplaySra5StatusCode = true;
      string message = service.OutputValues[0].Value.ToString();
      switch (this.GetResultStatus(service.OutputValues[0]))
      {
        case UserPanel.ActuatorResultStatus.InProgress:
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(message, UserPanel.PictureStatus.NoChange);
          break;
        case UserPanel.ActuatorResultStatus.Done:
          this.flagDisplaySra5StatusCode = false;
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(Resources.Message_PreInstallationCompletedSuccessfully, UserPanel.PictureStatus.ShowSuccess);
          break;
        case UserPanel.ActuatorResultStatus.Aborted:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(string.Format(Resources.MessageFormat_ErrorPreInstallationStatusAbortedWithTheFollowingMessage0, (object) message), UserPanel.PictureStatus.ShowError);
          break;
        case UserPanel.ActuatorResultStatus.NotStarted:
          this.AppendStatus(Resources.Message_ErrorPreInstallationStatusIndicatesServiceRoutineNotStarted, UserPanel.PictureStatus.ShowError);
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          break;
        case UserPanel.ActuatorResultStatus.NoCommunication:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(Resources.Message_ErrorPreInstallationStatusIndicatesNoCommunicationWithSRA, UserPanel.PictureStatus.ShowError);
          break;
        default:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(string.Format(Resources.MessageFormat_ErrorPreInstallationStatusEncounteredAnUnknownStatusValue0, (object) message), UserPanel.PictureStatus.ShowError);
          break;
      }
    }
    else
      this.AppendStatus(Resources.Message_ErrorPreInstallationStatusServiceCallFailed, UserPanel.PictureStatus.ShowError);
  }

  private void ExecutePreInstallationStop()
  {
    if (this.operationPhase == UserPanel.OperationPhase.Stop && this.operationCallState == UserPanel.OperationCallState.Executing || !this.Connected)
      return;
    this.DisplaySra5StatusCode();
    Service service = this.GetService("MCM", "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.PreInstallationStop_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void PreInstallationStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.PreInstallationStop_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      if (Convert.ToInt32(choice.RawValue) == 5)
        this.AppendStatus(Resources.Message_PreInstallationStoppedSuccessfully, UserPanel.PictureStatus.NoChange);
      else
        this.AppendStatus(string.Format(Resources.MessageFormat_ErrorPreInstallationStopProcessFailedWithOutputValue0, (object) choice), UserPanel.PictureStatus.ShowError);
    }
    else
      this.AppendStatus(Resources.Message_ErrorPreInstallationStopServiceCallFailed, UserPanel.PictureStatus.ShowError);
    this.StopTimer();
    this.ClearUserInterfaceState();
  }

  private void OnPreInstallationStop_Click(object sender, EventArgs e)
  {
    this.buttonStart.Click -= new EventHandler(this.OnPreInstallationStart_Click);
    this.buttonStop.Click -= new EventHandler(this.OnPreInstallationStop_Click);
    this.buttonStop.Enabled = false;
    this.AppendStatus(Resources.Message_PreInstallationStopRequestedByUser, UserPanel.PictureStatus.NoChange);
    if (this.operationTimer == null)
    {
      this.ShowPicture(this.picBlank);
      this.StopTimer();
      this.ClearUserInterfaceState();
    }
    else
      this.forcePreInstallationStop = true;
  }

  private void OnSelfCalibrationStart_Click(object sender, EventArgs e)
  {
    ++this.stepNumber;
    this.UpdateControls();
    this.AppendStatus(Resources.Message_PerformingSelfCalibration, UserPanel.PictureStatus.NoChange);
    this.operationPhase = UserPanel.OperationPhase.Start;
    this.operationCallState = UserPanel.OperationCallState.Ready;
    this.forceSelfCalibrationStop = false;
    this.operationTimer = new Timer();
    this.operationTimer.Interval = 1000;
    this.operationTimer.Tick += new EventHandler(this.operationTimer_Tick);
    this.operationTimer.Start();
  }

  private void ExecuteSelfCalibrationStart()
  {
    if (!this.Connected)
      return;
    Service service = this.GetService("MCM", "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SelfCalibrationStart_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void SelfCalibrationStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    bool flag = false;
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SelfCalibrationStart_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      if (Convert.ToInt32(choice.RawValue) == 1)
      {
        this.AppendStatus(Resources.Message_SelfCalibrationStartedSuccessfully, UserPanel.PictureStatus.NoChange);
        this.operationPhase = UserPanel.OperationPhase.Status;
        this.operationCallState = UserPanel.OperationCallState.Ready;
        flag = true;
      }
      else if (Convert.ToInt32(choice.RawValue) == 2)
        this.AppendStatus(Resources.Message_ErrorSelfCalibrationStartProcessFailed, UserPanel.PictureStatus.ShowError);
      else
        this.AppendStatus(string.Format(Resources.MessageFormat_ErrorSelfCalibrationStartProcessFailedWithOutputValue0, (object) choice), UserPanel.PictureStatus.ShowError);
    }
    else
      this.AppendStatus(Resources.Message_ErrorSelfCalibrationStartServiceCallFailed, UserPanel.PictureStatus.ShowError);
    if (flag)
      return;
    this.StopTimer();
    this.ClearUserInterfaceState();
  }

  private void ExecuteSelfCalibrationStatus()
  {
    if (!this.Connected)
      return;
    Service service = this.GetService("MCM", "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SelfCalibrationStatus_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void SelfCalibrationStatus_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SelfCalibrationStatus_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.flagDisplaySra5StatusCode = true;
      string message = service.OutputValues[0].Value.ToString();
      switch (this.GetResultStatus(service.OutputValues[0]))
      {
        case UserPanel.ActuatorResultStatus.InProgress:
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(message, UserPanel.PictureStatus.NoChange);
          break;
        case UserPanel.ActuatorResultStatus.Done:
          this.flagDisplaySra5StatusCode = false;
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(Resources.Message_SelfCalibrationCompletedSuccessfully, UserPanel.PictureStatus.ShowSuccess);
          break;
        case UserPanel.ActuatorResultStatus.Aborted:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(string.Format(Resources.MessageFormat_ErrorSelfCalibrationStatusAbortedWithTheFollowingMessage0, (object) message), UserPanel.PictureStatus.ShowError);
          break;
        case UserPanel.ActuatorResultStatus.NotStarted:
          this.AppendStatus(Resources.Message_ErrorSelfCalibrationStatusIndicatesServiceRoutineNotStarted, UserPanel.PictureStatus.ShowError);
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          break;
        case UserPanel.ActuatorResultStatus.NoCommunication:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(Resources.Message_ErrorSelfCalibrationStatusIndicatesNoCommunicationWithSRA, UserPanel.PictureStatus.ShowError);
          break;
        default:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(string.Format(Resources.MessageFormat_ErrorSelfCalibrationStatusEncounteredAnUnknownStatusValue0, (object) message), UserPanel.PictureStatus.ShowError);
          break;
      }
    }
    else
      this.AppendStatus(Resources.Message_ErrorSelfCalibrationStatusServiceCallFailed, UserPanel.PictureStatus.ShowError);
  }

  private void ExecuteSelfCalibrationStop()
  {
    if (this.operationPhase == UserPanel.OperationPhase.Stop && this.operationCallState == UserPanel.OperationCallState.Executing || !this.Connected)
      return;
    this.DisplaySra5StatusCode();
    Service service = this.GetService("MCM", "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SelfCalibrationStop_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void SelfCalibrationStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SelfCalibrationStop_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      if (Convert.ToInt32(choice.RawValue) == 5)
        this.AppendStatus(Resources.Message_SelfCalibrationStoppedSuccessfully, UserPanel.PictureStatus.NoChange);
      else
        this.AppendStatus(string.Format(Resources.MessageFormat_ErrorSelfCalibrationStopProcessFailedWithOutputValue0, (object) choice), UserPanel.PictureStatus.ShowError);
    }
    else
      this.AppendStatus(Resources.Message_ErrorSelfCalibrationStopServiceCallFailed, UserPanel.PictureStatus.ShowError);
    this.StopTimer();
    this.ClearUserInterfaceState();
  }

  private void OnSelfCalibrationStop_Click(object sender, EventArgs e)
  {
    this.buttonStart.Click -= new EventHandler(this.OnSelfCalibrationStart_Click);
    this.buttonStop.Click -= new EventHandler(this.OnSelfCalibrationStop_Click);
    this.buttonStop.Enabled = false;
    this.AppendStatus(Resources.Message_SelfCalibrationStopRequestedByUser, UserPanel.PictureStatus.NoChange);
    if (this.operationTimer == null)
    {
      this.ShowPicture(this.picBlank);
      this.StopTimer();
      this.ClearUserInterfaceState();
    }
    else
      this.forceSelfCalibrationStop = true;
  }

  private void OnHysteresisTestStart_Click(object sender, EventArgs e)
  {
    ++this.stepNumber;
    this.UpdateControls();
    this.AppendStatus(Resources.Message_PerformingHysteresisTest, UserPanel.PictureStatus.NoChange);
    this.operationPhase = UserPanel.OperationPhase.Start;
    this.operationCallState = UserPanel.OperationCallState.Ready;
    this.forceHysteresisTestStop = false;
    this.hysteresisSuccessful = false;
    this.operationTimer = new Timer();
    this.operationTimer.Interval = 1;
    this.operationTimer.Tick += new EventHandler(this.operationTimer_Tick);
    this.operationTimer.Start();
  }

  private void ExecuteHysteresisTestStart()
  {
    if (!this.Connected)
      return;
    Service service = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.HysteresisTestStart_ServiceCompleteEvent);
    this.SetupInstruments();
    this.SetupHysteresisServiceRoutines();
    this.hysteresisTest.Reset();
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void HysteresisTestStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    bool flag = false;
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.HysteresisTestStart_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      if (Convert.ToInt32(choice.RawValue) == 1)
      {
        this.AppendStatus(Resources.Message_HysteresisTestStartedSuccessfully, UserPanel.PictureStatus.NoChange);
        this.operationPhase = UserPanel.OperationPhase.Data;
        this.operationCallState = UserPanel.OperationCallState.Ready;
        flag = true;
      }
      else if (Convert.ToInt32(choice.RawValue) == 2)
        this.AppendStatus(Resources.Message_ErrorHysteresisTestStartProcessFailed, UserPanel.PictureStatus.ShowError);
      else
        this.AppendStatus(string.Format(Resources.MessageFormat_ErrorHysteresisTestStartProcessFailedWithOutputValue0, (object) choice), UserPanel.PictureStatus.ShowError);
    }
    else
      this.AppendStatus(Resources.Message_ErrorHysteresisTestStartServiceCallFailed, UserPanel.PictureStatus.ShowError);
    if (flag)
      return;
    this.RestoreInstruments();
    this.RestoreHysteresisServiceRoutines();
    this.StopTimer();
    this.ClearUserInterfaceState();
  }

  private void ExecuteHysteresisTestData()
  {
    if (!this.Connected)
      return;
    Service service = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.HysteresisTestData_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void HysteresisTestData_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.HysteresisTestData_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.flagDisplaySra5StatusCode = true;
      string str = service.OutputValues[0].Value.ToString();
      byte num1 = Convert.ToByte(str.Substring(2, 2));
      byte num2 = num1;
      bool flag1 = false;
      switch (num1)
      {
        case 1:
          this.flagDisplaySra5StatusCode = false;
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(Resources.Message_HysteresisTestCompletedSuccessfully, UserPanel.PictureStatus.ShowSuccess);
          this.hysteresisSuccessful = true;
          bool flag2 = this.hysteresisTest.DetermineStrokeRanges();
          this.AppendStatus(string.Format(Resources.MessageFormat_HysteresisTestDataIndexes01234, (object) this.hysteresisTest.OutstrokeBeginIndex, (object) this.hysteresisTest.OutstrokeEndIndex, (object) this.hysteresisTest.BackstrokeBeginIndex, (object) this.hysteresisTest.BackstrokeEndIndex, flag2 ? (object) Resources.Message_Passed : (object) Resources.Message_Failed3), UserPanel.PictureStatus.NoChange);
          if (flag2)
          {
            flag2 = this.hysteresisTest.CalculateAverageOutstrokeEffort();
            this.AppendStatus(string.Format(Resources.MessageFormat_HysteresisTestOutstrokeAverageEffort01, (object) this.hysteresisTest.OutstrokeAverageEffort.ToString("0.00"), flag2 ? (object) Resources.Message_Passed3 : (object) Resources.Message_Failed6), UserPanel.PictureStatus.NoChange);
          }
          if (flag2)
          {
            flag2 = this.hysteresisTest.CalculateAverageBackstrokeEffort();
            this.AppendStatus(string.Format(Resources.MessageFormat_HysteresisTestBackstrokeAverageEffort01, (object) this.hysteresisTest.BackstrokeAverageEffort.ToString("0.00"), flag2 ? (object) Resources.Message_Passed2 : (object) Resources.Message_Failed5), UserPanel.PictureStatus.NoChange);
          }
          if (flag2)
          {
            flag2 = this.hysteresisTest.TestPositionDeviationThresholds();
            this.AppendStatus(string.Format(Resources.MessageFormat_HysteresisTestPercentPositionDeviationCheck0, flag2 ? (object) Resources.Message_Passed1 : (object) Resources.Message_Failed4), UserPanel.PictureStatus.NoChange);
          }
          if (flag2)
          {
            flag1 = true;
            this.AppendStatus(Resources.Message_HysteresisTestPassed, UserPanel.PictureStatus.ShowSuccess);
            break;
          }
          num2 = (byte) this.hysteresisTest.ErrorType;
          this.AppendStatus(this.hysteresisTest.ErrorType != UserPanel.HysteresisErrorType.PositionDeviationThreshold ? Resources.Message_HysteresisTestFailed : string.Format(Resources.MessageFormat_HysteresisTestFailedOnThe0AtTargetPosition1Index2, this.hysteresisTest.ErrorMotorDirection == UserPanel.HysteresisMotorDirection.Outstroke ? (object) Resources.Message_Outstroke : (object) Resources.Message_Backstroke, (object) this.hysteresisTest.ErrorTargetPosition, (object) this.hysteresisTest.ErrorItemIndex), UserPanel.PictureStatus.ShowError);
          break;
        case 2:
        case 3:
          this.hysteresisTest.NewDataItem();
          this.hysteresisTest.SetCurrentPosition(Convert.ToUInt16(str.Substring(4, 4), 16 /*0x10*/));
          this.hysteresisTest.SetTargetPosition(Convert.ToUInt16(str.Substring(8, 4), 16 /*0x10*/));
          this.hysteresisTest.SetMotorEffort(Convert.ToByte(str.Substring(12, 2), 16 /*0x10*/));
          this.hysteresisTest.MarkAsComplete();
          this.operationPhase = UserPanel.OperationPhase.Data;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          break;
        case 5:
        case 6:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(string.Format(Resources.MessageFormat_ErrorHysteresisTestDataAbortedCode0AbortedTimeout, (object) num1), UserPanel.PictureStatus.ShowError);
          break;
        case 8:
          this.AppendStatus(Resources.Message_ErrorHysteresisTestDataIndicatesServiceRoutineNotStarted, UserPanel.PictureStatus.ShowError);
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          break;
        case 9:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(Resources.Message_ErrorHysteresisTestStatusIndicatesNoCommunicationWithSRA, UserPanel.PictureStatus.ShowError);
          break;
        default:
          this.operationPhase = UserPanel.OperationPhase.Stop;
          this.operationCallState = UserPanel.OperationCallState.Ready;
          this.AppendStatus(string.Format(Resources.MessageFormat_ErrorHysteresisTestStatusEncounteredAnUnknownStatusValue0, (object) num1), UserPanel.PictureStatus.ShowError);
          break;
      }
      if (this.operationPhase != UserPanel.OperationPhase.Stop)
        return;
      new ServiceCode(SapiManager.GetEngineSerialNumber(service.Channel), (byte) 3, num2, flag1, DateTime.Now).ShowMessage(Resources.Message_TurboActuatorHysteresisTestEPA07);
    }
    else
      this.AppendStatus(Resources.Message_ErrorHysteresisTestDataServiceCallFailed, UserPanel.PictureStatus.ShowError);
  }

  private void ExecuteHysteresisTestStop()
  {
    if (this.operationPhase == UserPanel.OperationPhase.Stop && this.operationCallState == UserPanel.OperationCallState.Executing || !this.Connected)
      return;
    this.DisplaySra5StatusCode();
    Service service = this.GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber");
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.HysteresisTestStop_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.operationCallState = UserPanel.OperationCallState.Executing;
    service.Execute(false);
  }

  private void HysteresisTestStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.HysteresisTestStop_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      if (Convert.ToInt32(choice.RawValue) == 5)
        this.AppendStatus(Resources.Message_HysteresisTestStoppedSuccessfully, UserPanel.PictureStatus.NoChange);
      else
        this.AppendStatus(string.Format(Resources.MessageFormat_ErrorHysteresisTestStopProcessFailedWithOutputValue0, (object) choice), UserPanel.PictureStatus.ShowError);
    }
    else
      this.AppendStatus(Resources.Message_ErrorHysteresisTestStopServiceCallFailed, UserPanel.PictureStatus.ShowError);
    this.RestoreInstruments();
    this.RestoreHysteresisServiceRoutines();
    this.StopTimer();
    this.ClearUserInterfaceState();
  }

  private void OnHysteresisTestStop_Click(object sender, EventArgs e)
  {
    this.buttonStart.Click -= new EventHandler(this.OnHysteresisTestStart_Click);
    this.buttonStop.Click -= new EventHandler(this.OnHysteresisTestStop_Click);
    this.buttonStop.Enabled = false;
    this.AppendStatus(Resources.Message_HysteresisTestStopRequestedByUser, UserPanel.PictureStatus.NoChange);
    if (this.operationTimer == null)
    {
      this.RestoreInstruments();
      this.RestoreHysteresisServiceRoutines();
      this.ShowPicture(this.picBlank);
      this.StopTimer();
      this.ClearUserInterfaceState();
    }
    else
      this.forceHysteresisTestStop = true;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.panel3 = new Panel();
    this.buttonStart = new Button();
    this.buttonNext = new Button();
    this.buttonPrevious = new Button();
    this.buttonHysteresisTest = new Button();
    this.buttonPreInstallation = new Button();
    this.buttonSelfCalibration = new Button();
    this.closeButton = new Button();
    this.buttonStop = new Button();
    this.picWait = new PictureBox();
    this.picAlignHoles = new PictureBox();
    this.picHysteresis = new PictureBox();
    this.picError = new PictureBox();
    this.picGrease = new PictureBox();
    this.picMount = new PictureBox();
    this.picBlank = new PictureBox();
    this.picNotMounted = new PictureBox();
    this.picOk = new PictureBox();
    this.textboxInstructions = new TextBox();
    this.lblInstructions = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.lblStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.textboxStatus = new TextBox();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    this.panel3.SuspendLayout();
    ((ISupportInitialize) this.picWait).BeginInit();
    ((ISupportInitialize) this.picAlignHoles).BeginInit();
    ((ISupportInitialize) this.picHysteresis).BeginInit();
    ((ISupportInitialize) this.picError).BeginInit();
    ((ISupportInitialize) this.picGrease).BeginInit();
    ((ISupportInitialize) this.picMount).BeginInit();
    ((ISupportInitialize) this.picBlank).BeginInit();
    ((ISupportInitialize) this.picNotMounted).BeginInit();
    ((ISupportInitialize) this.picOk).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 7, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonNext, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonPrevious, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonHysteresisTest, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonPreInstallation, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSelfCalibration, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.closeButton, 8, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStop, 4, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonNext, "buttonNext");
    this.buttonNext.Name = "buttonNext";
    this.buttonNext.UseCompatibleTextRendering = true;
    this.buttonNext.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonPrevious, "buttonPrevious");
    this.buttonPrevious.Name = "buttonPrevious";
    this.buttonPrevious.UseCompatibleTextRendering = true;
    this.buttonPrevious.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonHysteresisTest, "buttonHysteresisTest");
    this.buttonHysteresisTest.Name = "buttonHysteresisTest";
    this.buttonHysteresisTest.UseCompatibleTextRendering = true;
    this.buttonHysteresisTest.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonPreInstallation, "buttonPreInstallation");
    this.buttonPreInstallation.Name = "buttonPreInstallation";
    this.buttonPreInstallation.UseCompatibleTextRendering = true;
    this.buttonPreInstallation.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonSelfCalibration, "buttonSelfCalibration");
    this.buttonSelfCalibration.Name = "buttonSelfCalibration";
    this.buttonSelfCalibration.UseCompatibleTextRendering = true;
    this.buttonSelfCalibration.UseVisualStyleBackColor = true;
    this.closeButton.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.closeButton, "closeButton");
    this.closeButton.Name = "closeButton";
    this.closeButton.UseCompatibleTextRendering = true;
    this.closeButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.lblInstructions, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.lblStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxStatus, 0, 3);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.panel3, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.textboxInstructions, 1, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.panel3, "panel3");
    this.panel3.Controls.Add((Control) this.picWait);
    this.panel3.Controls.Add((Control) this.picAlignHoles);
    this.panel3.Controls.Add((Control) this.picHysteresis);
    this.panel3.Controls.Add((Control) this.picError);
    this.panel3.Controls.Add((Control) this.picGrease);
    this.panel3.Controls.Add((Control) this.picMount);
    this.panel3.Controls.Add((Control) this.picBlank);
    this.panel3.Controls.Add((Control) this.picNotMounted);
    this.panel3.Controls.Add((Control) this.picOk);
    this.panel3.Name = "panel3";
    componentResourceManager.ApplyResources((object) this.picWait, "picWait");
    this.picWait.Name = "picWait";
    this.picWait.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picAlignHoles, "picAlignHoles");
    this.picAlignHoles.Name = "picAlignHoles";
    this.picAlignHoles.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picHysteresis, "picHysteresis");
    this.picHysteresis.Name = "picHysteresis";
    this.picHysteresis.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picError, "picError");
    this.picError.Name = "picError";
    this.picError.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picGrease, "picGrease");
    this.picGrease.Name = "picGrease";
    this.picGrease.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picMount, "picMount");
    this.picMount.Name = "picMount";
    this.picMount.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picBlank, "picBlank");
    this.picBlank.Name = "picBlank";
    this.picBlank.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picNotMounted, "picNotMounted");
    this.picNotMounted.Name = "picNotMounted";
    this.picNotMounted.TabStop = false;
    componentResourceManager.ApplyResources((object) this.picOk, "picOk");
    this.picOk.Name = "picOk";
    this.picOk.TabStop = false;
    componentResourceManager.ApplyResources((object) this.textboxInstructions, "textboxInstructions");
    this.textboxInstructions.Name = "textboxInstructions";
    this.textboxInstructions.ReadOnly = true;
    this.lblInstructions.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.lblInstructions, "lblInstructions");
    ((Control) this.lblInstructions).Name = "lblInstructions";
    this.lblInstructions.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.lblStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.lblStatus, "lblStatus");
    ((Control) this.lblStatus).Name = "lblStatus";
    this.lblStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.textboxStatus, "textboxStatus");
    this.textboxStatus.Name = "textboxStatus";
    this.textboxStatus.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_TurboActuator");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).PerformLayout();
    this.panel3.ResumeLayout(false);
    this.panel3.PerformLayout();
    ((ISupportInitialize) this.picWait).EndInit();
    ((ISupportInitialize) this.picAlignHoles).EndInit();
    ((ISupportInitialize) this.picHysteresis).EndInit();
    ((ISupportInitialize) this.picError).EndInit();
    ((ISupportInitialize) this.picGrease).EndInit();
    ((ISupportInitialize) this.picMount).EndInit();
    ((ISupportInitialize) this.picBlank).EndInit();
    ((ISupportInitialize) this.picNotMounted).EndInit();
    ((ISupportInitialize) this.picOk).EndInit();
    ((Control) this).ResumeLayout(false);
  }

  public enum SoftwareComparisonResult
  {
    Older,
    Newer,
    Identical,
  }

  public enum CurrentOperation
  {
    Disconnected,
    ConnectedDisabled,
    ConnectedNoHysteresis,
    ConnectedIdle,
    PreInstallation,
    SelfCalibration,
    HysteresisTest,
  }

  public enum OperationPhase
  {
    None,
    Start,
    Status,
    Data,
    Stop,
  }

  public enum OperationCallState
  {
    Ready,
    Executing,
  }

  public enum PictureStatus
  {
    NoChange,
    ShowSuccess,
    ShowError,
    ShowBlank,
  }

  public class UIState
  {
    private UserPanel.CurrentOperation currentOperation;
    private int stepNumber;
    private bool startButtonActive;
    private bool startButtonVisible;
    private bool stopButtonActive;
    private bool stopButtonVisible;
    private bool nextButtonActive;
    private bool nextButtonVisible;
    private bool previousButtonActive;
    private bool previousButtonVisible;
    private string instruction;
    private PictureBox picture;

    public UserPanel.CurrentOperation CurrentOperation => this.currentOperation;

    public int StepNumber => this.stepNumber;

    public bool StartButtonActive => this.startButtonActive;

    public bool StartButtonVisible => this.startButtonVisible;

    public bool StopButtonActive => this.stopButtonActive;

    public bool StopButtonVisible => this.stopButtonVisible;

    public bool NextButtonActive => this.nextButtonActive;

    public bool NextButtonVisible => this.nextButtonVisible;

    public bool PreviousButtonActive => this.previousButtonActive;

    public bool PreviousButtonVisible => this.previousButtonVisible;

    public string Instruction => this.instruction;

    public PictureBox Picture => this.picture;

    public UIState(
      UserPanel.CurrentOperation currentOperation,
      int stepNumber,
      bool startButtonActive,
      bool startButtonVisible,
      bool stopButtonActive,
      bool stopButtonVisible,
      bool nextButtonActive,
      bool nextButtonVisible,
      bool previousButtonActive,
      bool previousButtonVisible,
      string instruction,
      PictureBox picture)
    {
      this.currentOperation = currentOperation;
      this.stepNumber = stepNumber;
      this.startButtonActive = startButtonActive;
      this.startButtonVisible = startButtonVisible;
      this.stopButtonActive = stopButtonActive;
      this.stopButtonVisible = stopButtonVisible;
      this.nextButtonActive = nextButtonActive;
      this.nextButtonVisible = nextButtonVisible;
      this.previousButtonActive = previousButtonActive;
      this.previousButtonVisible = previousButtonVisible;
      this.instruction = instruction;
      this.picture = picture;
    }
  }

  public enum HysteresisMotorDirection
  {
    None,
    Outstroke,
    Backstroke,
  }

  public enum HysteresisErrorType
  {
    None = 100, // 0x00000064
    InconsistentData = 101, // 0x00000065
    AverageMotorEffortThreshold = 102, // 0x00000066
    PositionDeviationThreshold = 103, // 0x00000067
  }

  public class HysteresisDataItem
  {
    public DateTime timestamp;
    public int targetPosition;
    public int currentPosition;
    public int motorEffort;
    public bool complete;
  }

  public class HysteresisTest
  {
    private const int motorEffortLowerBound = 50;
    private const int motorEffortUpperBound = 950;
    private const double motorEffortMaxThreshold = 40.0;
    private const double positionDeviationPercentageThreshold = 2.5;
    private ArrayList dataList = new ArrayList();
    private UserPanel.HysteresisDataItem currentDataItem;
    private int outstrokeBeginIndex;
    private int outstrokeEndIndex;
    private int backstrokeBeginIndex;
    private int backstrokeEndIndex;
    private double outstrokeAverageEffort;
    private double backstrokeAverageEffort;
    private UserPanel.HysteresisErrorType errorType;
    private UserPanel.HysteresisMotorDirection errorMotorDirection;
    private int errorTargetPosition;
    private int errorItemIndex;

    public UserPanel.HysteresisDataItem CurrentDataItem => this.currentDataItem;

    public UserPanel.HysteresisErrorType ErrorType => this.errorType;

    public UserPanel.HysteresisMotorDirection ErrorMotorDirection => this.errorMotorDirection;

    public int ErrorTargetPosition => this.errorTargetPosition;

    public int ErrorItemIndex => this.errorItemIndex;

    public int OutstrokeBeginIndex => this.outstrokeBeginIndex;

    public int OutstrokeEndIndex => this.outstrokeEndIndex;

    public int BackstrokeBeginIndex => this.backstrokeBeginIndex;

    public int BackstrokeEndIndex => this.backstrokeEndIndex;

    public double OutstrokeAverageEffort => this.outstrokeAverageEffort;

    public double BackstrokeAverageEffort => this.backstrokeAverageEffort;

    public void NewDataItem()
    {
      this.currentDataItem = new UserPanel.HysteresisDataItem();
      this.currentDataItem.timestamp = DateTime.Now;
      this.dataList.Add((object) this.currentDataItem);
    }

    public void SetTargetPosition(ushort targetPosition)
    {
      this.currentDataItem.targetPosition = (int) targetPosition;
    }

    public void SetCurrentPosition(ushort currentPosition)
    {
      this.currentDataItem.currentPosition = (int) currentPosition;
    }

    public void SetMotorEffort(byte motorEffort)
    {
      this.currentDataItem.motorEffort = (int) motorEffort * 2 - 248;
    }

    public void MarkAsComplete() => this.currentDataItem.complete = true;

    public void Reset()
    {
      this.dataList.Clear();
      this.dataList.TrimToSize();
      this.currentDataItem = (UserPanel.HysteresisDataItem) null;
      this.outstrokeAverageEffort = 0.0;
      this.backstrokeAverageEffort = 0.0;
      this.outstrokeBeginIndex = -1;
      this.outstrokeEndIndex = -1;
      this.backstrokeBeginIndex = -1;
      this.backstrokeEndIndex = -1;
      this.errorType = UserPanel.HysteresisErrorType.None;
      this.errorMotorDirection = UserPanel.HysteresisMotorDirection.None;
      this.errorTargetPosition = 0;
      this.errorItemIndex = 0;
    }

    public bool DetermineStrokeRanges()
    {
      int index = 0;
      this.outstrokeBeginIndex = -1;
      this.outstrokeEndIndex = -1;
      this.backstrokeBeginIndex = -1;
      this.backstrokeEndIndex = -1;
      for (; index < this.dataList.Count; ++index)
      {
        if ((this.dataList[index] as UserPanel.HysteresisDataItem).targetPosition >= 50)
        {
          this.outstrokeBeginIndex = index;
          ++index;
          break;
        }
      }
      for (; index < this.dataList.Count; ++index)
      {
        if ((this.dataList[index] as UserPanel.HysteresisDataItem).targetPosition <= 950)
          continue;
        this.outstrokeEndIndex = index - 1;
        break;
      }
      for (; index < this.dataList.Count; ++index)
      {
        if ((this.dataList[index] as UserPanel.HysteresisDataItem).targetPosition > 950)
          continue;
        this.backstrokeBeginIndex = index;
        ++index;
        break;
      }
      for (; index < this.dataList.Count; ++index)
      {
        if ((this.dataList[index] as UserPanel.HysteresisDataItem).targetPosition >= 50)
          continue;
        this.backstrokeEndIndex = index - 1;
        break;
      }
      if (this.outstrokeBeginIndex < 0 || this.outstrokeEndIndex < 0)
      {
        this.errorType = UserPanel.HysteresisErrorType.InconsistentData;
        this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Outstroke;
        this.errorTargetPosition = 0;
        this.errorItemIndex = 0;
        return false;
      }
      if (this.backstrokeBeginIndex < 0 || this.backstrokeEndIndex < 0)
      {
        this.errorType = UserPanel.HysteresisErrorType.InconsistentData;
        this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Backstroke;
        this.errorTargetPosition = 0;
        this.errorItemIndex = 0;
        return false;
      }
      if (this.outstrokeBeginIndex > this.outstrokeEndIndex)
      {
        this.errorType = UserPanel.HysteresisErrorType.InconsistentData;
        this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Outstroke;
        this.errorTargetPosition = 0;
        this.errorItemIndex = 0;
        return false;
      }
      if (this.backstrokeBeginIndex <= this.backstrokeEndIndex)
        return true;
      this.errorType = UserPanel.HysteresisErrorType.InconsistentData;
      this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Backstroke;
      this.errorTargetPosition = 0;
      this.errorItemIndex = 0;
      return false;
    }

    public bool CalculateAverageOutstrokeEffort()
    {
      double num1 = 0.0;
      int num2 = this.outstrokeEndIndex - this.outstrokeBeginIndex + 1;
      this.outstrokeAverageEffort = 0.0;
      if (this.outstrokeBeginIndex < 0 || this.outstrokeEndIndex < 0 || num2 <= 0)
      {
        this.errorType = UserPanel.HysteresisErrorType.InconsistentData;
        this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Outstroke;
        this.errorTargetPosition = 0;
        this.errorItemIndex = 0;
        return false;
      }
      for (int outstrokeBeginIndex = this.outstrokeBeginIndex; outstrokeBeginIndex <= this.outstrokeEndIndex; ++outstrokeBeginIndex)
      {
        UserPanel.HysteresisDataItem data = this.dataList[outstrokeBeginIndex] as UserPanel.HysteresisDataItem;
        num1 += (double) data.motorEffort;
      }
      this.outstrokeAverageEffort = num1 / (double) num2;
      if (Math.Abs(this.outstrokeAverageEffort) <= 40.0)
        return true;
      this.errorType = UserPanel.HysteresisErrorType.AverageMotorEffortThreshold;
      this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Outstroke;
      this.errorTargetPosition = 0;
      this.errorItemIndex = 0;
      return false;
    }

    public bool CalculateAverageBackstrokeEffort()
    {
      double num1 = 0.0;
      int num2 = this.backstrokeEndIndex - this.backstrokeBeginIndex + 1;
      this.backstrokeAverageEffort = 0.0;
      if (this.backstrokeBeginIndex < 0 || this.backstrokeEndIndex < 0 || num2 <= 0)
      {
        this.errorType = UserPanel.HysteresisErrorType.InconsistentData;
        this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Backstroke;
        this.errorTargetPosition = 0;
        this.errorItemIndex = 0;
        return false;
      }
      for (int backstrokeBeginIndex = this.backstrokeBeginIndex; backstrokeBeginIndex <= this.backstrokeEndIndex; ++backstrokeBeginIndex)
      {
        UserPanel.HysteresisDataItem data = this.dataList[backstrokeBeginIndex] as UserPanel.HysteresisDataItem;
        num1 += (double) data.motorEffort;
      }
      this.backstrokeAverageEffort = num1 / (double) num2;
      if (Math.Abs(this.backstrokeAverageEffort) <= 40.0)
        return true;
      this.errorType = UserPanel.HysteresisErrorType.AverageMotorEffortThreshold;
      this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Backstroke;
      this.errorTargetPosition = 0;
      this.errorItemIndex = 0;
      return false;
    }

    public bool TestPositionDeviationThresholds()
    {
      for (int outstrokeBeginIndex = this.outstrokeBeginIndex; outstrokeBeginIndex < this.outstrokeEndIndex - 1; ++outstrokeBeginIndex)
      {
        if ((double) (this.dataList[outstrokeBeginIndex] as UserPanel.HysteresisDataItem).motorEffort > this.outstrokeAverageEffort && (double) (this.dataList[outstrokeBeginIndex + 2] as UserPanel.HysteresisDataItem).motorEffort > this.outstrokeAverageEffort)
        {
          UserPanel.HysteresisDataItem data = this.dataList[outstrokeBeginIndex + 1] as UserPanel.HysteresisDataItem;
          if ((double) data.motorEffort > this.outstrokeAverageEffort && (double) (Math.Abs((data.targetPosition - data.currentPosition) / data.targetPosition) * 100) > 2.5)
          {
            this.errorType = UserPanel.HysteresisErrorType.PositionDeviationThreshold;
            this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Outstroke;
            this.errorTargetPosition = data.targetPosition;
            this.errorItemIndex = outstrokeBeginIndex + 1;
            return false;
          }
        }
      }
      for (int backstrokeBeginIndex = this.backstrokeBeginIndex; backstrokeBeginIndex < this.backstrokeEndIndex - 1; ++backstrokeBeginIndex)
      {
        if ((double) (this.dataList[backstrokeBeginIndex] as UserPanel.HysteresisDataItem).motorEffort > this.backstrokeAverageEffort && (double) (this.dataList[backstrokeBeginIndex + 2] as UserPanel.HysteresisDataItem).motorEffort > this.backstrokeAverageEffort)
        {
          UserPanel.HysteresisDataItem data = this.dataList[backstrokeBeginIndex + 1] as UserPanel.HysteresisDataItem;
          if ((double) data.motorEffort > this.backstrokeAverageEffort && (double) (Math.Abs((data.targetPosition - data.currentPosition) / data.targetPosition) * 100) > 2.5)
          {
            this.errorType = UserPanel.HysteresisErrorType.PositionDeviationThreshold;
            this.errorMotorDirection = UserPanel.HysteresisMotorDirection.Backstroke;
            this.errorTargetPosition = data.targetPosition;
            this.errorItemIndex = backstrokeBeginIndex + 1;
            return false;
          }
        }
      }
      return true;
    }
  }

  private enum ActuatorResultStatus
  {
    Unknown,
    InProgress,
    Done,
    Aborted,
    NotStarted,
    NoCommunication,
  }
}
