// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN_OEM__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN_OEM__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string McmName = "MCM21T";
  private const string CpcName = "CPC04T";
  private const string AcmName = "ACM21T";
  private const string ESNEcuInfo = "CO_ESN";
  private const string VINEcuInfo = "CO_VIN";
  private const string EngineSerialNumberService = "DL_ID_Write_Engine_Serial_Number";
  private const string KeyOffOnResetService = "FN_KeyOffOnReset";
  private const string VehicleIdentificationNumberService = "DL_ID_Write_VIN_Current";
  private const string VINServiceForCPC = "DL_ID_VIN_Current";
  private const string ParameterWriteInitializeServiceName = "ParameterWriteInitializeService";
  private static readonly Regex ValidESNCharacters = new Regex("[\\da-zA-Z]");
  private static readonly Regex ValidESNRegex = new Regex("\\A[\\da-zA-Z]{14}\\z");
  private static readonly Regex VinNameRegex = new Regex("\\bVIN\\b");
  private static readonly Regex ValidVinCharacters = new Regex("[\\da-zA-Z-[iIoOqQ]]");
  private Channel mcm;
  private EcuInfo ecuInfoESN = (EcuInfo) null;
  private Channel cpc;
  private Channel acm;
  private bool haveUpdatedESN = false;
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private bool parametersHaveBeenRead = false;
  private List<Channel> parametersBeingRead = new List<Channel>();
  private Service currentService;
  private TableLayoutPanel tableLayoutPanel1;
  private System.Windows.Forms.Label labelVIN;
  private TextBox textBoxESN;
  private Button buttonSynchronize;
  private TextBox textBoxVIN;
  private Button buttonClose;
  private SeekTimeListView seekTimeListView;
  private System.Windows.Forms.Label labelESN;

  public UserPanel()
  {
    this.InitializeComponent();
    this.textBoxESN.TextChanged += new EventHandler(this.OnTextChanged);
    this.textBoxESN.KeyPress += new KeyPressEventHandler(this.textBoxESN_KeyPress);
    this.textBoxVIN.TextChanged += new EventHandler(this.OnTextChanged);
    this.textBoxVIN.KeyPress += new KeyPressEventHandler(this.textBoxVIN_KeyPress);
    this.buttonSynchronize.Click += new EventHandler(this.buttonSynchronize_Click);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM21T"));
    this.SetCPC(this.GetChannel("CPC04T"));
    this.SetACM(this.GetChannel("ACM21T"));
    if (string.IsNullOrEmpty(this.VinIdentifier))
      this.VinIdentifier = CollectionExtensions.FirstOrDefault<string>(SapiManager.GlobalInstance.Sapi.Channels.Select(channel =>
      {
        var data = new
        {
          channel = channel,
          id = SapiManager.GetVehicleIdentificationNumber(channel)
        };
        return data;
      }).Where(_param0 => VehicleIdentification.IsValidVin(_param0.id)).Select(_param0 => _param0.id), this.VinIdentifier);
    this.UpdateUserInterface();
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.StopWork(UserPanel.Reason.Closing);
    this.SetMCM((Channel) null);
    this.SetCPC((Channel) null);
    this.SetACM((Channel) null);
  }

  public string VinIdentifier
  {
    get => this.textBoxVIN.Text;
    set => this.textBoxVIN.Text = value;
  }

  private bool SetMCM(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      flag = true;
      if (this.CurrentStage != UserPanel.Stage.WaitingForBackOnlineCPC)
        this.StopWork(UserPanel.Reason.Disconnected);
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        if (this.ecuInfoESN != null)
        {
          this.ecuInfoESN.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.OnESNUpdate);
          this.ecuInfoESN = (EcuInfo) null;
        }
      }
      this.mcm = mcm;
      this.haveUpdatedESN = false;
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.ecuInfoESN = this.mcm.EcuInfos["CO_ESN"];
        if (this.ecuInfoESN != null)
          this.ecuInfoESN.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.OnESNUpdate);
      }
    }
    return flag;
  }

  private bool SetCPC(Channel cpc)
  {
    bool flag = false;
    if (this.cpc != cpc)
    {
      if (this.cpc != null)
        this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.cpc = cpc;
      if (this.cpc != null)
      {
        this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.VinIdentifier = SapiManager.GetVehicleIdentificationNumber(this.cpc);
      }
    }
    return flag;
  }

  private bool SetACM(Channel acm)
  {
    bool flag = false;
    if (this.acm != acm)
    {
      flag = true;
      this.acm = acm;
    }
    return flag;
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (sender == this.cpc && e.CommunicationsState == CommunicationsState.Online && this.CurrentStage == UserPanel.Stage.WaitingForBackOnlineCPC)
      this.PerformCurrentStage();
    this.UpdateUserInterface();
  }

  private void OnTextChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void textBoxESN_KeyPress(object sender, KeyPressEventArgs e)
  {
    if (char.IsControl(e.KeyChar) || UserPanel.ValidESNCharacters.IsMatch(e.KeyChar.ToString()))
      return;
    e.Handled = true;
  }

  private void textBoxVIN_KeyPress(object sender, KeyPressEventArgs e)
  {
    if (char.IsControl(e.KeyChar) || UserPanel.ValidVinCharacters.IsMatch(e.KeyChar.ToString()))
      return;
    e.Handled = true;
  }

  private void OnESNUpdate(object sender, ResultEventArgs e) => this.UpdateESN(true);

  private void UpdateESN(bool forceUpdate)
  {
    if (!forceUpdate && (this.textBoxESN.TextLength != 0 || this.haveUpdatedESN))
      return;
    if (this.ecuInfoESN != null && this.ecuInfoESN.Value != null)
    {
      this.textBoxESN.Text = this.ecuInfoESN.Value;
      this.textBoxESN.SelectAll();
      this.haveUpdatedESN = true;
    }
    else
    {
      this.textBoxESN.Text = string.Empty;
      if (this.ecuInfoESN != null)
        this.ecuInfoESN.Read(false);
    }
  }

  private void UpdateUserInterface()
  {
    this.UpdateESN(false);
    this.buttonClose.Enabled = this.CanClose;
    this.buttonSynchronize.Enabled = this.IsValidEsn && this.IsValidVIN && !this.Working;
    this.textBoxESN.ReadOnly = !this.CanEditEsn;
    this.textBoxVIN.ReadOnly = !this.CanSetVIN;
    if (this.textBoxESN.ReadOnly)
      this.textBoxESN.BackColor = SystemColors.Control;
    else
      this.textBoxESN.BackColor = this.IsValidEsn ? Color.PaleGreen : Color.LightPink;
    if (this.textBoxVIN.ReadOnly)
      this.textBoxVIN.BackColor = SystemColors.Control;
    else
      this.textBoxVIN.BackColor = this.IsValidVIN ? Color.PaleGreen : Color.LightPink;
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

  private bool Working => this.currentStage != UserPanel.Stage.Idle;

  private bool Online
  {
    get
    {
      return this.cpc != null && this.cpc.CommunicationsState == CommunicationsState.Online || this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online || this.acm != null && this.acm.CommunicationsState == CommunicationsState.Online;
    }
  }

  private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

  private bool CanClose => !this.Working;

  private bool CanSetESN
  {
    get
    {
      return !this.Working && this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online && this.mcm.Services["DL_ID_Write_Engine_Serial_Number"] != (Service) null && this.IsValidLicense;
    }
  }

  private bool IsValidEsn => this.CanSetESN && UserPanel.IsSerialNumberValid(this.textBoxESN.Text);

  private bool CanEditEsn
  {
    get => this.CanSetESN && this.ecuInfoESN != null && this.ecuInfoESN.Value != null;
  }

  private bool CanSetVIN => !this.Working && this.Online && this.IsValidLicense;

  private bool IsValidVIN
  {
    get => this.CanSetVIN && VehicleIdentification.IsValidVin(this.textBoxVIN.Text);
  }

  private static bool IsSerialNumberValid(string text)
  {
    return text != null && UserPanel.ValidESNRegex.IsMatch(text);
  }

  private void ReportResult(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void buttonSynchronize_Click(object sender, EventArgs e)
  {
    if (!this.IsValidEsn || !this.IsValidVIN)
      return;
    this.StartWork();
  }

  private void StartWork()
  {
    this.CurrentStage = UserPanel.Stage.SetEsn;
    this.PerformCurrentStage();
  }

  private void PerformCurrentStage()
  {
    switch (this.CurrentStage)
    {
      case UserPanel.Stage.Idle:
        break;
      case UserPanel.Stage.SetEsn:
        this.CurrentStage = UserPanel.Stage.WaitingForSetEsn;
        Service service1 = this.mcm.Services["DL_ID_Write_Engine_Serial_Number"];
        if (service1 == (Service) null)
        {
          this.StopWork(UserPanel.Reason.FailedServiceExecute);
          break;
        }
        string text = this.textBoxESN.Text;
        this.ReportResult($"{Resources.Message_SettingESNTo}{text}.");
        service1.InputValues[0].Value = (object) text;
        this.ExecuteAsynchronousService(service1);
        break;
      case UserPanel.Stage.WaitingForSetEsn:
        this.CurrentStage = UserPanel.Stage.CommitMCMForESN;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.CommitMCMForESN:
        if (this.mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService") && this.mcm.Services[this.mcm.Ecu.Properties["CommitToPermanentMemoryService"]] != (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.WaitingForCommitMCMForESN;
          this.ReportResult(Resources.Message_WritingChangesToPermanentMemory);
          this.ExecuteAsynchronousService(this.mcm.Services[this.mcm.Ecu.Properties["CommitToPermanentMemoryService"]]);
          break;
        }
        this.ReportResult(Resources.Message_SkippingMCM21TCommitProcessAsTheServiceCouldNotBeFound);
        this.CurrentStage = UserPanel.Stage.KeyOffOnCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitingForCommitMCMForESN:
        this.CurrentStage = UserPanel.Stage.KeyOffOnCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.KeyOffOnCPC:
        if (this.cpc == null)
        {
          this.ReportResult(Resources.Message_SkippingCPC04TCommitProcessAsTheCPC04TIsNotConnected);
          this.CurrentStage = UserPanel.Stage.Finish;
          this.PerformCurrentStage();
          break;
        }
        Service service2 = this.cpc.Services["FN_KeyOffOnReset"];
        if (service2 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.Finish;
          this.ReportResult(Resources.Message_SkippingCPC04TResetAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForKeyOffOnCPC;
          this.ReportResult(Resources.Message_SynchronizingESNToCPC04TViaKeyOffOnReset);
          this.ExecuteAsynchronousService(service2);
        }
        break;
      case UserPanel.Stage.WaitingForKeyOffOnCPC:
        this.CurrentStage = UserPanel.Stage.WaitingForBackOnlineCPC;
        this.ReportResult(Resources.Message_WaitingForTheCPC04TToComeBackOnline);
        break;
      case UserPanel.Stage.WaitingForBackOnlineCPC:
        this.CurrentStage = UserPanel.Stage.SetVINForMCM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForMCM:
        if (this.mcm == null)
        {
          this.ReportResult(Resources.Message_SkippingMCM21TSetVINProcessAsTheMCM21TIsNotConnected);
          this.CurrentStage = UserPanel.Stage.SetVINForACM;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.ExecuteMCMVINService;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ExecuteMCMVINService:
        this.CurrentStage = UserPanel.Stage.WaitingForSetMCMVIN;
        Service service3 = this.mcm.Services["DL_ID_Write_VIN_Current"];
        if (service3 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForACM;
          this.ReportResult(Resources.Message_SkippingSettingOfMCM21TVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForSetMCMVIN;
        this.ReportResult($"{Resources.Message_SettingVINTo}{this.VinIdentifier}.");
        service3.InputValues[0].Value = (object) this.VinIdentifier;
        this.ExecuteAsynchronousService(service3);
        break;
      case UserPanel.Stage.WaitingForSetMCMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForACM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForACM:
        if (this.acm == null)
        {
          this.ReportResult(Resources.Message_SkippingACM21TSetVINProcessAsTheACM21TIsNotConnected);
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.ExecuteACMVINService;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ExecuteACMVINService:
        this.CurrentStage = UserPanel.Stage.WaitingForSetACMVIN;
        Service service4 = this.acm.Services["DL_ID_Write_VIN_Current"];
        if (service4 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.ReportResult(Resources.Message_SkippingSettingOfACM21TVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForSetACMVIN;
        this.ReportResult($"{Resources.Message_SettingVINTo1}{this.VinIdentifier}.");
        service4.InputValues[0].Value = (object) this.VinIdentifier;
        this.ExecuteAsynchronousService(service4);
        break;
      case UserPanel.Stage.WaitingForSetACMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForCPC:
        if (this.cpc == null)
        {
          this.ReportResult(Resources.Message_SkippingCPC04TSetVINProcessAsTheCPC04TIsNotConnected);
          this.CurrentStage = UserPanel.Stage.Finish;
          this.PerformCurrentStage();
          break;
        }
        Service service5 = this.cpc.Services["DL_ID_VIN_Current"];
        if (service5 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.Finish;
          this.ReportResult(Resources.Message_SkippingSettingOfCPC04TVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetCPCVIN;
          this.ReportResult($"{Resources.Message_SettingVINTo2}{this.VinIdentifier}.");
          service5.InputValues[0].Value = (object) this.VinIdentifier;
          this.ExecuteAsynchronousService(service5);
        }
        break;
      case UserPanel.Stage.WaitingForSetCPCVIN:
        this.CurrentStage = UserPanel.Stage.ReadParameters;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ReadParameters:
        this.CurrentStage = UserPanel.Stage.WaitingForReadParameters;
        if (this.acm != null)
        {
          this.parametersBeingRead.Add(this.acm);
          this.acm.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
          this.acm.Parameters.Read(false);
        }
        if (this.cpc != null)
        {
          this.parametersBeingRead.Add(this.cpc);
          this.cpc.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
          this.cpc.Parameters.Read(false);
        }
        if (this.mcm == null)
          break;
        this.parametersBeingRead.Add(this.mcm);
        this.mcm.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.mcm.Parameters.Read(false);
        break;
      case UserPanel.Stage.WaitingForReadParameters:
        this.ReportResult(Resources.Message_ParametersReadSuccessfully);
        this.parametersHaveBeenRead = true;
        this.CurrentStage = UserPanel.Stage.ConnectToServer;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ConnectToServer:
        if (!this.parametersHaveBeenRead)
          break;
        this.CurrentStage = UserPanel.Stage.WaitingForConnectToServer;
        this.ReportResult(Resources.Message_ConnectToServer);
        ServerClient.GlobalInstance.Complete += new EventHandler<ClientConnectionCompleteEventArgs>(this.ServerClient_ServerClientCompleteEvent);
        Collection<UnitInformation> collection = new Collection<UnitInformation>();
        ServerDataManager.GlobalInstance.GetUploadUnits(collection, (ServerDataManager.UploadType) 0);
        ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, collection);
        break;
      case UserPanel.Stage.WaitingForConnectToServer:
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.Finish:
        if (this.mcm != null)
          this.mcm.FaultCodes.Reset(false);
        if (this.acm != null)
          this.acm.FaultCodes.Reset(false);
        this.StopWork(UserPanel.Reason.Succeeded);
        break;
      case UserPanel.Stage.Stopping:
        break;
      default:
        throw new InvalidOperationException("Unknown stage.");
    }
  }

  private void ServerClient_ServerClientCompleteEvent(
    object sender,
    ClientConnectionCompleteEventArgs e)
  {
    this.PerformCurrentStage();
  }

  private void StopWork(UserPanel.Reason reason)
  {
    if (this.CurrentStage == UserPanel.Stage.Stopping || this.CurrentStage == UserPanel.Stage.Idle)
      return;
    UserPanel.Stage currentStage = this.CurrentStage;
    this.CurrentStage = UserPanel.Stage.Stopping;
    if (reason == UserPanel.Reason.Succeeded)
    {
      if (currentStage != UserPanel.Stage.Finish)
        throw new InvalidOperationException();
      this.VerifyResults(this.mcm, "MCM21T", "CO_ESN", "ESN", this.textBoxESN.Text);
      this.VerifyResults(this.cpc, "CPC04T", "CO_ESN", "ESN", this.textBoxESN.Text);
      this.VerifyResults(this.acm, "ACM21T", "CO_ESN", "ESN", this.textBoxESN.Text);
      this.VerifyResults(this.mcm, "MCM21T", "CO_VIN", "VIN", this.textBoxVIN.Text);
      this.VerifyResults(this.cpc, "CPC04T", "CO_VIN", "VIN", this.textBoxVIN.Text);
      this.VerifyResults(this.acm, "ACM21T", "CO_VIN", "VIN", this.textBoxVIN.Text);
    }
    else
    {
      this.ReportResult(Resources.Message_TheProcedureFailedToComplete);
      switch (reason - 1)
      {
        case UserPanel.Reason.Succeeded:
          this.ReportResult(Resources.Message_FailedToObtainService);
          break;
        case UserPanel.Reason.FailedServiceExecute:
          this.ReportResult(Resources.Message_FailedToExecuteService);
          break;
        case UserPanel.Reason.Closing:
          this.ReportResult(Resources.Message_TheMCM21TWasDisconnected);
          break;
      }
    }
    this.ClearCurrentService();
    this.CurrentStage = UserPanel.Stage.Idle;
  }

  private bool VerifyResults(
    Channel channel,
    string channelName,
    string function,
    string functionName,
    string textBoxText)
  {
    bool flag = false;
    if (channel == null || channel.EcuInfos[function] == null)
    {
      this.ReportResult($"{Resources.Message_The}{channelName} {functionName}{Resources.Message_CannotBeVerified}");
    }
    else
    {
      EcuInfo ecuInfo = channel.EcuInfos[function];
      try
      {
        ecuInfo.Read(true);
      }
      catch (CaesarException ex)
      {
        this.ReportResult($"{Resources.Message_FailedToReadECUInfoCannotVerifyThe}{channelName} {functionName}{Resources.Message_Error}{ex.Message}");
      }
      catch (InvalidOperationException ex)
      {
        this.ReportResult(Resources.Message_FailedToReadECUInfoThe + channelName + Resources.Message_IsUnavailableError + ex.Message);
      }
      flag = string.Compare(ecuInfo.Value, textBoxText, true) == 0;
      if (flag)
        this.ReportResult($"{Resources.Message_The1}{channelName} {functionName}{Resources.Message_HasSuccessfullyBeenSetTo}{textBoxText}.");
      else
        this.ReportResult($"{Resources.Message_The2}{channelName} {functionName}{Resources.Message_HasNotBeenChangedAndHasAValueOf}{ecuInfo.Value}.");
    }
    return flag;
  }

  private void ExecuteAsynchronousService(Service service)
  {
    if (this.currentService != (Service) null)
      throw new InvalidOperationException("Must wait for current service to finish before continuing.");
    if (service == (Service) null)
    {
      this.StopWork(UserPanel.Reason.FailedServiceExecute);
    }
    else
    {
      this.currentService = service;
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      this.ReportResult($"{Resources.Message_Executing}{UserPanel.VinNameRegex.Replace(service.Name, "VIN")}…");
      this.currentService.Execute(false);
    }
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.ClearCurrentService();
    if (this.CheckCompleteResult(e, "Service executed", "Service error"))
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
        stringBuilder.Append(": Unknown");
    }
    this.ReportResult(stringBuilder.ToString());
    return flag;
  }

  private void ClearCurrentService()
  {
    if (!(this.currentService != (Service) null))
      return;
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.currentService = (Service) null;
  }

  private void buttonConnectToServer_Click(object sender, EventArgs e)
  {
    this.CurrentStage = UserPanel.Stage.ConnectToServer;
    this.PerformCurrentStage();
  }

  private void buttonReadParameters_Click(object sender, EventArgs e)
  {
    this.CurrentStage = UserPanel.Stage.ReadParameters;
    this.PerformCurrentStage();
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    ParameterCollection parameterCollection = sender as ParameterCollection;
    parameterCollection.Channel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
    if (parameterCollection != null)
      this.parametersBeingRead.Remove(parameterCollection.Channel);
    if (e.Succeeded)
    {
      ServerDataManager.GlobalInstance.AutoSaveSettings(parameterCollection.Channel, (ServerDataManager.AutoSaveDestination) 1, "ECUREAD");
      ServerDataManager.GlobalInstance.AutoSaveSettings(parameterCollection.Channel, (ServerDataManager.AutoSaveDestination) 0, "ECUUPDATE");
      if (this.parametersBeingRead.Count != 0)
        return;
      this.PerformCurrentStage();
    }
    else
      this.StopWork(UserPanel.Reason.FailedParameterRead);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelVIN = new System.Windows.Forms.Label();
    this.labelESN = new System.Windows.Forms.Label();
    this.textBoxESN = new TextBox();
    this.textBoxVIN = new TextBox();
    this.buttonSynchronize = new Button();
    this.buttonClose = new Button();
    this.seekTimeListView = new SeekTimeListView();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelVIN, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelESN, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxESN, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxVIN, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSynchronize, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelVIN, "labelVIN");
    this.labelVIN.Name = "labelVIN";
    this.labelVIN.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelESN, "labelESN");
    this.labelESN.Name = "labelESN";
    this.labelESN.UseCompatibleTextRendering = true;
    this.textBoxESN.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textBoxESN, "textBoxESN");
    this.textBoxESN.Name = "textBoxESN";
    this.textBoxVIN.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textBoxVIN, "textBoxVIN");
    this.textBoxVIN.Name = "textBoxVIN";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.buttonSynchronize, 2);
    componentResourceManager.ApplyResources((object) this.buttonSynchronize, "buttonSynchronize");
    this.buttonSynchronize.Name = "buttonSynchronize";
    this.buttonSynchronize.UseCompatibleTextRendering = true;
    this.buttonSynchronize.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "SetEsnVinOemMy13";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Set_Engine_Serial_Number_Product_Identification_Number");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum Stage
  {
    Idle = 0,
    SetEsn = 1,
    _StartESN = 1,
    WaitingForSetEsn = 2,
    CommitMCMForESN = 3,
    WaitingForCommitMCMForESN = 4,
    KeyOffOnCPC = 5,
    WaitingForKeyOffOnCPC = 6,
    WaitingForBackOnlineCPC = 7,
    SetVINForMCM = 8,
    _StartVIN = 8,
    ExecuteMCMVINService = 9,
    WaitingForSetMCMVIN = 10, // 0x0000000A
    SetVINForACM = 11, // 0x0000000B
    ExecuteACMVINService = 12, // 0x0000000C
    WaitingForSetACMVIN = 13, // 0x0000000D
    SetVINForCPC = 14, // 0x0000000E
    WaitingForSetCPCVIN = 15, // 0x0000000F
    ReadParameters = 16, // 0x00000010
    WaitingForReadParameters = 17, // 0x00000011
    ConnectToServer = 18, // 0x00000012
    WaitingForConnectToServer = 19, // 0x00000013
    Finish = 20, // 0x00000014
    Stopping = 21, // 0x00000015
  }

  private enum Reason
  {
    Succeeded,
    FailedServiceExecute,
    FailedService,
    FailedParameterRead,
    Closing,
    Disconnected,
  }
}
