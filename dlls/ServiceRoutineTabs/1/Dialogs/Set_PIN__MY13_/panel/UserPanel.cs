// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_PIN__MY13_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_PIN__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string McmName = "MCM21T";
  private const string AcmName = "ACM21T";
  private const string CpcName = "CPC04T";
  private const string ESNEcuInfo = "CO_ESN";
  private const string VINEcuInfo = "CO_VIN";
  private const string EngineSerialNumberService = "DL_ID_Write_Engine_Serial_Number";
  private const string KeyOffOnResetService = "FN_KeyOffOnReset";
  private const string VehicleIdentificationNumberService = "DL_ID_Write_VIN_Current";
  private const string VINServiceForCPC = "DL_ID_VIN_Current";
  private static readonly Regex ValidESNCharacters = new Regex("[\\da-zA-Z]");
  private static readonly Regex ValidESNRegex = new Regex("\\A[\\da-zA-Z]{14}\\z");
  private static readonly Regex VinNameRegex = new Regex("\\bVIN\\b");
  private static readonly Regex ValidPinCharacters = new Regex("[\\da-zA-Z-[iIoOqQ]]");
  private Channel mcm;
  private bool setESN = false;
  private bool setPIN = false;
  private EcuInfo ecuInfoESN = (EcuInfo) null;
  private Channel cpc;
  private Channel acm;
  private bool haveUpdatedESN = false;
  private bool alreadyAskedUser = false;
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private bool parametersHaveBeenRead = false;
  private bool PINHasBeenSynchronized = false;
  private bool UploadToServerAlso = false;
  private List<Channel> parametersBeingRead = new List<Channel>();
  private Service currentService;
  private Channel executeAsynchronousServicesChannel;
  private string currentServiceList;
  private TableLayoutPanel tableLayoutPanel1;
  private System.Windows.Forms.Label labelPIN;
  private TextBox textBoxESN;
  private Button buttonSetESN;
  private TextBox textBoxOutput;
  private Button buttonSetPIN;
  private TextBox textBoxPIN;
  private Button buttonReadParameters;
  private Button buttonConnectToServer;
  private System.Windows.Forms.Label labelReadParameters;
  private System.Windows.Forms.Label labelUploadToServer;
  private Button buttonClose;
  private System.Windows.Forms.Label labelReadParametersMessage;
  private System.Windows.Forms.Label labelUploadToServerMessage;
  private System.Windows.Forms.Label labelESN;

  public UserPanel()
  {
    this.InitializeComponent();
    this.textBoxESN.TextChanged += new EventHandler(this.OnTextChanged);
    this.textBoxESN.KeyPress += new KeyPressEventHandler(this.textBoxESN_KeyPress);
    this.textBoxPIN.TextChanged += new EventHandler(this.OnTextChanged);
    this.textBoxPIN.KeyPress += new KeyPressEventHandler(this.textBoxPIN_KeyPress);
    this.buttonSetESN.Click += new EventHandler(this.buttonSetESN_Click);
    this.buttonSetPIN.Click += new EventHandler(this.buttonSetPIN_Click);
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
    this.SetACM(this.GetChannel("ACM21T", (CustomPanel.ChannelLookupOptions) 5));
    if (string.IsNullOrEmpty(this.PinIdentifier))
      this.PinIdentifier = CollectionExtensions.FirstOrDefault<string>(SapiManager.GlobalInstance.Sapi.Channels.Select(channel =>
      {
        var data = new
        {
          channel = channel,
          id = SapiManager.GetVehicleIdentificationNumber(channel)
        };
        return data;
      }).Where(_param0 => VehicleIdentification.IsValidPin(_param0.id)).Select(_param0 => _param0.id), this.PinIdentifier);
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

  public string PinIdentifier
  {
    get => this.textBoxPIN.Text;
    set => this.textBoxPIN.Text = value;
  }

  private bool SetMCM(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      flag = true;
      if (this.CurrentStage != UserPanel.Stage.WaitingForBackOnlineCPC)
      {
        this.StopWork(UserPanel.Reason.Disconnected);
        this.ClearOutput();
      }
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
        this.PinIdentifier = SapiManager.GetVehicleIdentificationNumber(this.cpc);
      }
    }
    return flag;
  }

  private bool SetACM(Channel acm)
  {
    bool flag = false;
    if (this.acm != acm)
    {
      if (this.acm != null)
        this.acm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.acm = acm;
      if (this.acm != null)
        this.acm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
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

  private void textBoxPIN_KeyPress(object sender, KeyPressEventArgs e)
  {
    if (char.IsControl(e.KeyChar) || UserPanel.ValidPinCharacters.IsMatch(e.KeyChar.ToString()))
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
      this.CheckPINandESN();
    }
    else
    {
      this.textBoxESN.Text = string.Empty;
      if (this.ecuInfoESN != null)
        this.ecuInfoESN.Read(false);
    }
  }

  private void CheckPINandESN()
  {
    if (!this.IsValidEsn || this.IsValidPIN || this.alreadyAskedUser || !this.haveUpdatedESN)
      return;
    this.alreadyAskedUser = true;
    if (MessageBox.Show(Resources.Message_UseESNAsPIN, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      this.textBoxPIN.Text = this.textBoxESN.Text;
  }

  private void UpdateUserInterface()
  {
    this.UpdateESN(false);
    this.buttonReadParameters.Enabled = this.PINHasBeenSynchronized;
    this.buttonConnectToServer.Enabled = this.parametersHaveBeenRead;
    this.buttonClose.Enabled = this.CanClose;
    this.buttonSetESN.Enabled = this.IsValidEsn;
    this.buttonSetPIN.Enabled = this.IsValidPIN;
    this.textBoxESN.ReadOnly = !this.CanEditEsn;
    this.textBoxPIN.ReadOnly = !this.CanSetPIN;
    if (this.textBoxESN.ReadOnly)
      this.textBoxESN.BackColor = SystemColors.Control;
    else
      this.textBoxESN.BackColor = this.IsValidEsn ? Color.PaleGreen : Color.LightPink;
    if (this.textBoxPIN.ReadOnly)
    {
      this.textBoxPIN.BackColor = SystemColors.Control;
    }
    else
    {
      this.textBoxPIN.BackColor = this.IsValidPIN ? Color.PaleGreen : Color.LightPink;
      this.CheckPINandESN();
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

  private bool CanSetPIN => !this.Working && this.Online && this.IsValidLicense;

  private bool IsValidPIN
  {
    get => this.CanSetPIN && VehicleIdentification.IsValidPin(this.textBoxPIN.Text);
  }

  private static bool IsSerialNumberValid(string text)
  {
    return text != null && UserPanel.ValidESNRegex.IsMatch(text);
  }

  private void ClearOutput() => this.textBoxOutput.Text = string.Empty;

  private void ReportResult(string text)
  {
    this.textBoxOutput.Text = $"{this.textBoxOutput.Text}{text}\r\n";
    this.textBoxOutput.SelectionStart = this.textBoxOutput.TextLength;
    this.textBoxOutput.SelectionLength = 0;
    this.textBoxOutput.ScrollToCaret();
  }

  private void buttonSetESN_Click(object sender, EventArgs e)
  {
    if (!this.IsValidEsn)
      return;
    this.setESN = true;
    this.setPIN = false;
    this.StartWork();
  }

  private void buttonSetPIN_Click(object sender, EventArgs e)
  {
    if (!this.IsValidPIN)
      return;
    this.setESN = false;
    this.setPIN = true;
    this.StartWork();
  }

  private void StartWork()
  {
    if (this.setESN)
      this.CurrentStage = UserPanel.Stage.SetEsn;
    else if (this.setPIN)
      this.CurrentStage = UserPanel.Stage.SetVINForMCM;
    this.PerformCurrentStage();
  }

  private void PerformCurrentStage()
  {
    switch (this.CurrentStage)
    {
      case UserPanel.Stage.Idle:
        break;
      case UserPanel.Stage.SetEsn:
        this.ClearOutput();
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
        if (this.mcm != null && !SapiManager.GetBootModeStatus(this.mcm))
        {
          string dereferencedServiceList = this.mcm.Services.GetDereferencedServiceList("CommitToPermanentMemoryService");
          this.CurrentStage = UserPanel.Stage.WaitingForCommitMCMForESN;
          this.ReportResult(Resources.Message_WritingChangesToPermanentMemory);
          this.ExecuteAsynchronousServices(dereferencedServiceList, this.mcm);
          break;
        }
        this.ReportResult(Resources.Message_SkippingMCM21TSetPINProcessAsTheMCM21TIsNotConnectedOrInBootMode);
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
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForMCM:
        this.ClearOutput();
        if (this.mcm == null)
        {
          this.ReportResult(Resources.Message_SkippingMCM21TSetPINProcessAsTheMCM21TIsNotConnectedOrInBootMode);
          this.CurrentStage = UserPanel.Stage.SetVINForACM;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.UnlockMCMParameterWrite;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.UnlockMCMParameterWrite:
        string dereferencedServiceList1 = this.mcm.Services.GetDereferencedServiceList("ParameterWriteInitializeService");
        if (this.mcm != null && !SapiManager.GetBootModeStatus(this.mcm))
        {
          this.CurrentStage = UserPanel.Stage.ExecuteMCMVINService;
          this.ReportResult(string.Format(Resources.Message_Unlocking0ParameterWriteService, (object) this.mcm.Ecu.Name));
          this.ExecuteAsynchronousServices(dereferencedServiceList1, this.mcm);
          break;
        }
        this.ReportResult(Resources.Message_SkippingMCM21TSetPINProcessAsTheMCM21TIsNotConnectedOrInBootMode);
        this.CurrentStage = UserPanel.Stage.SetVINForACM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ExecuteMCMVINService:
        Service service3 = this.mcm.Services["DL_ID_Write_VIN_Current"];
        if (service3 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForACM;
          this.ReportResult(Resources.Message_SkippingSettingOfMCM21TPINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForSetMCMVIN;
        this.ReportResult(string.Format($"{Resources.Message_Setting1PINTo}{this.PinIdentifier}.", (object) this.mcm.Ecu.Name));
        service3.InputValues[0].Value = (object) this.PinIdentifier;
        this.ExecuteAsynchronousService(service3);
        break;
      case UserPanel.Stage.WaitingForSetMCMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForACM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForACM:
        if (this.acm == null)
        {
          this.ReportResult(string.Format(Resources.Message_SkippingACMSetPINProcessAsTheACMIsNotConnectedOrInBootMode, this.acm == null ? (object) "ACM" : (object) this.acm.Ecu.Name));
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.UnlockACMParameterWrite;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.UnlockACMParameterWrite:
        string dereferencedServiceList2 = this.acm.Services.GetDereferencedServiceList("ParameterWriteInitializeService");
        if (this.acm != null && !SapiManager.GetBootModeStatus(this.acm))
        {
          this.CurrentStage = UserPanel.Stage.ExecuteACMVINService;
          this.ReportResult(string.Format(Resources.Message_Unlocking0ParameterWriteService, (object) this.acm.Ecu.Name));
          this.ExecuteAsynchronousServices(dereferencedServiceList2, this.acm);
          break;
        }
        this.ReportResult(string.Format(Resources.Message_SkippingACMSetPINProcessAsTheACMIsNotConnectedOrInBootMode, this.acm == null ? (object) "ACM21T" : (object) this.acm.Ecu.Name));
        this.CurrentStage = UserPanel.Stage.SetVINForCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ExecuteACMVINService:
        this.CurrentStage = UserPanel.Stage.WaitingForSetACMVIN;
        Service service4 = this.acm.Services["DL_ID_Write_VIN_Current"];
        if (service4 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.ReportResult(string.Format(Resources.Message_SkippingSettingOfACMPINAsTheServiceCannotBeFound, this.acm == null ? (object) "ACM" : (object) this.acm.Ecu.Name));
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForSetACMVIN;
        this.ReportResult(string.Format($"{Resources.Message_Setting1PINTo}{this.PinIdentifier}.", (object) this.acm.Ecu.Name));
        service4.InputValues[0].Value = (object) this.PinIdentifier;
        this.ExecuteAsynchronousService(service4);
        break;
      case UserPanel.Stage.WaitingForSetACMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForCPC:
        this.CurrentStage = UserPanel.Stage.WaitingForSetCPCVIN;
        if (this.cpc == null)
        {
          this.ReportResult(string.Format(Resources.Message_SkippingCPC04TSetPINProcessAsTheCPC04TIsNotConnectedOrInBootMode, this.cpc == null ? (object) "CPC04T" : (object) this.cpc.Ecu.Name));
          this.PerformCurrentStage();
          break;
        }
        Service service5 = this.cpc.Services["DL_ID_VIN_Current"];
        if (service5 == (Service) null)
        {
          this.ReportResult(Resources.Message_SkippingSettingOfCPC04TPINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.ReportResult(string.Format($"{Resources.Message_Setting1PINTo}{this.PinIdentifier}.", (object) this.cpc.Ecu.Name));
          service5.InputValues[0].Value = (object) this.PinIdentifier;
          this.ExecuteAsynchronousService(service5);
        }
        break;
      case UserPanel.Stage.WaitingForSetCPCVIN:
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PINHasBeenSynchronized = this.VerifyResults(this.mcm, "MCM21T", "CO_VIN", "PIN", this.PinIdentifier);
        this.PINHasBeenSynchronized &= this.VerifyResults(this.cpc, "CPC04T", "CO_VIN", "PIN", this.PinIdentifier);
        this.PINHasBeenSynchronized &= this.VerifyResults(this.acm, "ACM21T", "CO_VIN", "PIN", this.PinIdentifier);
        if (this.PINHasBeenSynchronized && MessageBox.Show(Resources.Message_ReadParametersAndUpload, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
          this.CurrentStage = UserPanel.Stage.ReadParameters;
          this.UploadToServerAlso = true;
        }
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ReadParameters:
        this.CurrentStage = UserPanel.Stage.WaitingForReadParameters;
        this.parametersBeingRead.Add(this.acm);
        this.acm.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.acm.Parameters.Read(false);
        this.parametersBeingRead.Add(this.cpc);
        this.cpc.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.cpc.Parameters.Read(false);
        this.parametersBeingRead.Add(this.mcm);
        this.mcm.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.mcm.Parameters.Read(false);
        break;
      case UserPanel.Stage.WaitingForReadParameters:
        this.ReportResult(Resources.Message_ParametersReadSuccessfully);
        this.parametersHaveBeenRead = true;
        this.CurrentStage = this.UploadToServerAlso ? UserPanel.Stage.ConnectToServer : UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.ConnectToServer:
        if (!this.parametersHaveBeenRead)
          break;
        this.CurrentStage = UserPanel.Stage.WaitingForConnectToServer;
        this.ReportResult(Resources.Message_ConnectToServer);
        ServerClient.GlobalInstance.Complete += new EventHandler<ClientConnectionCompleteEventArgs>(this.ServerClient_ServerClientCompleteEvent);
        ServerClient.GlobalInstance.Go((Collection<UnitInformation>) null, (Collection<UnitInformation>) null);
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
      if (this.setESN)
      {
        this.VerifyResults(this.mcm, "MCM21T", "CO_ESN", "ESN", this.textBoxESN.Text);
        this.VerifyResults(this.cpc, "CPC04T", "CO_ESN", "ESN", this.textBoxESN.Text);
      }
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
    string ecuName,
    string function,
    string functionName,
    string textBoxText)
  {
    bool flag = false;
    string str = channel == null ? ecuName : channel.Ecu.Name;
    if (channel == null || channel.EcuInfos[function] == null)
    {
      this.ReportResult($"{Resources.Message_The}{str} {functionName}{Resources.Message_CannotBeVerified}");
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
        this.ReportResult($"{Resources.Message_FailedToReadECUInfoCannotVerifyThe}{str} {functionName}{Resources.Message_Error}{ex.Message}");
      }
      catch (InvalidOperationException ex)
      {
        this.ReportResult(Resources.Message_FailedToReadECUInfoThe + str + Resources.Message_IsUnavailableError + ex.Message);
      }
      flag = string.Compare(ecuInfo.Value, textBoxText, true) == 0;
      if (flag)
        this.ReportResult($"{Resources.Message_The1}{str} {functionName}{Resources.Message_HasSuccessfullyBeenSetTo}{textBoxText}.");
      else
        this.ReportResult($"{Resources.Message_The2}{str} {functionName}{Resources.Message_HasNotBeenChangedAndHasAValueOf}{ecuInfo.Value}.");
    }
    return flag;
  }

  private void ExecuteAsynchronousService(Service service)
  {
    this.currentService = service;
    this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.ReportResult($"{Resources.Message_Executing}{UserPanel.VinNameRegex.Replace(service.Name, "PIN")}…");
    this.currentService.Execute(false);
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.ClearCurrentService();
    if (this.CheckCompleteResult(e, "Service executed", "Service error"))
      this.PerformCurrentStage();
    else
      this.StopWork(UserPanel.Reason.FailedService);
  }

  private void ExecuteAsynchronousServices(string serviceList, Channel ch)
  {
    bool flag = false;
    this.executeAsynchronousServicesChannel = ch;
    if (!string.IsNullOrEmpty(this.currentServiceList))
      throw new InvalidOperationException("Must wait for current service to finish before continuing.");
    if (string.IsNullOrEmpty(serviceList))
    {
      flag = false;
    }
    else
    {
      this.currentServiceList = serviceList;
      this.currentServiceList = this.currentServiceList.TrimEnd(';');
      List<string> list = ((IEnumerable<string>) this.currentServiceList.Split(";".ToCharArray())).ToList<string>();
      this.ReportResult(Resources.Message_ExecutingServices);
      int num = this.executeAsynchronousServicesChannel.Services.Execute(this.currentServiceList, false);
      if (num != 0 && num == list.Count<string>())
        flag = true;
    }
    if (!flag)
    {
      if (this.CurrentStage == UserPanel.Stage.ExecuteMCMVINService)
      {
        this.ReportResult(string.Format(Resources.Message_SkippingSettingOf0PINAsTheServiceCannotBeFound, (object) this.executeAsynchronousServicesChannel.Ecu.Name));
        this.CurrentStage = UserPanel.Stage.SetVINForACM;
      }
      else if (this.CurrentStage == UserPanel.Stage.ExecuteACMVINService)
      {
        this.ReportResult(string.Format(Resources.Message_SkippingSettingOf0PINAsTheServiceCannotBeFound, (object) this.executeAsynchronousServicesChannel.Ecu.Name));
        this.CurrentStage = UserPanel.Stage.SetVINForCPC;
      }
      else if (this.CurrentStage == UserPanel.Stage.CommitMCMForESN)
      {
        this.ReportResult(Resources.Message_SkippingMCM21TCommitProcessAsTheServiceCouldNotBeFound);
        this.CurrentStage = UserPanel.Stage.KeyOffOnCPC;
      }
    }
    this.currentServiceList = (string) null;
    this.executeAsynchronousServicesChannel = (Channel) null;
    this.PerformCurrentStage();
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
    this.labelPIN = new System.Windows.Forms.Label();
    this.labelESN = new System.Windows.Forms.Label();
    this.textBoxESN = new TextBox();
    this.textBoxOutput = new TextBox();
    this.textBoxPIN = new TextBox();
    this.buttonSetPIN = new Button();
    this.buttonSetESN = new Button();
    this.buttonReadParameters = new Button();
    this.buttonConnectToServer = new Button();
    this.labelReadParameters = new System.Windows.Forms.Label();
    this.labelUploadToServer = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.labelReadParametersMessage = new System.Windows.Forms.Label();
    this.labelUploadToServerMessage = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelPIN, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelESN, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxESN, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxOutput, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxPIN, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetPIN, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetESN, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonReadParameters, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonConnectToServer, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelReadParameters, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelUploadToServer, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelReadParametersMessage, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelUploadToServerMessage, 1, 3);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelPIN, "labelPIN");
    this.labelPIN.Name = "labelPIN";
    this.labelPIN.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelESN, "labelESN");
    this.labelESN.Name = "labelESN";
    this.labelESN.UseCompatibleTextRendering = true;
    this.textBoxESN.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textBoxESN, "textBoxESN");
    this.textBoxESN.Name = "textBoxESN";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxOutput, 4);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    this.textBoxPIN.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textBoxPIN, "textBoxPIN");
    this.textBoxPIN.Name = "textBoxPIN";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.buttonSetPIN, 2);
    componentResourceManager.ApplyResources((object) this.buttonSetPIN, "buttonSetPIN");
    this.buttonSetPIN.Name = "buttonSetPIN";
    this.buttonSetPIN.UseCompatibleTextRendering = true;
    this.buttonSetPIN.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.buttonSetESN, 2);
    componentResourceManager.ApplyResources((object) this.buttonSetESN, "buttonSetESN");
    this.buttonSetESN.Name = "buttonSetESN";
    this.buttonSetESN.UseCompatibleTextRendering = true;
    this.buttonSetESN.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.buttonReadParameters, 2);
    componentResourceManager.ApplyResources((object) this.buttonReadParameters, "buttonReadParameters");
    this.buttonReadParameters.Name = "buttonReadParameters";
    this.buttonReadParameters.UseCompatibleTextRendering = true;
    this.buttonReadParameters.UseVisualStyleBackColor = true;
    this.buttonReadParameters.Click += new EventHandler(this.buttonReadParameters_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.buttonConnectToServer, 2);
    componentResourceManager.ApplyResources((object) this.buttonConnectToServer, "buttonConnectToServer");
    this.buttonConnectToServer.Name = "buttonConnectToServer";
    this.buttonConnectToServer.UseCompatibleTextRendering = true;
    this.buttonConnectToServer.UseVisualStyleBackColor = true;
    this.buttonConnectToServer.Click += new EventHandler(this.buttonConnectToServer_Click);
    componentResourceManager.ApplyResources((object) this.labelReadParameters, "labelReadParameters");
    this.labelReadParameters.Name = "labelReadParameters";
    this.labelReadParameters.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelUploadToServer, "labelUploadToServer");
    this.labelUploadToServer.Name = "labelUploadToServer";
    this.labelUploadToServer.UseCompatibleTextRendering = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelReadParametersMessage, "labelReadParametersMessage");
    this.labelReadParametersMessage.Name = "labelReadParametersMessage";
    this.labelReadParametersMessage.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelUploadToServerMessage, "labelUploadToServerMessage");
    this.labelUploadToServerMessage.Name = "labelUploadToServerMessage";
    this.labelUploadToServerMessage.UseCompatibleTextRendering = true;
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
    UnlockMCMParameterWrite = 9,
    ExecuteMCMVINService = 10, // 0x0000000A
    WaitingForSetMCMVIN = 11, // 0x0000000B
    SetVINForACM = 12, // 0x0000000C
    UnlockACMParameterWrite = 13, // 0x0000000D
    ExecuteACMVINService = 14, // 0x0000000E
    WaitingForSetACMVIN = 15, // 0x0000000F
    SetVINForCPC = 16, // 0x00000010
    WaitingForSetCPCVIN = 17, // 0x00000011
    ReadParameters = 18, // 0x00000012
    WaitingForReadParameters = 19, // 0x00000013
    ConnectToServer = 20, // 0x00000014
    WaitingForConnectToServer = 21, // 0x00000015
    Finish = 22, // 0x00000016
    Stopping = 23, // 0x00000017
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
