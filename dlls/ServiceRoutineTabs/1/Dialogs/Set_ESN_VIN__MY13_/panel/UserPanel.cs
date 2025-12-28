// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Adr;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string McmName = "MCM21T";
  private const string AcmName = "ACM21T";
  private const string CpcName = "CPC04T";
  private const string TcmName = "TCM01T";
  private const string ESNEcuInfo = "CO_ESN";
  private const string VINEcuInfo = "CO_VIN";
  private const string EngineSerialNumberService = "DL_ID_Write_Engine_Serial_Number";
  private const string AlternateEngineSerialNumberService = "DL_ID_Engine_Serial_Number";
  private const string KeyOffOnResetService = "FN_KeyOffOnReset";
  private const string HardResetService = "FN_HardReset";
  private const string VehicleIdentificationNumberService = "DL_ID_Write_VIN_Current";
  private const string AlternateVehicleIdentificationNumberService = "DL_ID_VIN_Current";
  private const string VINServiceForCPC = "WriteVINService";
  private static readonly Regex ValidESNCharacters = new Regex("(\\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
  private Channel mcm;
  private bool setESN = false;
  private bool setVIN = false;
  private EcuInfo ecuInfoESN = (EcuInfo) null;
  private Channel cpc;
  private Channel acm;
  private Channel tcm;
  private bool haveUpdatedESN = false;
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private Service currentService;
  private string currentServiceString;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonClose;
  private System.Windows.Forms.Label labelVIN;
  private TextBox textBoxESN;
  private Button buttonSetESN;
  private Button buttonSetVIN;
  private TextBox textBoxVIN;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private TextBox textBoxOutput;
  private System.Windows.Forms.Label labelESN;

  public UserPanel()
  {
    this.InitializeComponent();
    this.textBoxESN.TextChanged += new EventHandler(this.OnTextChanged);
    this.textBoxVIN.TextChanged += new EventHandler(this.OnTextChanged);
    this.buttonSetESN.Click += new EventHandler(this.OnSetESNClick);
    this.buttonSetVIN.Click += new EventHandler(this.OnSetVINClick);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    ConnectionManager.GlobalInstance.PropertyChanged += new PropertyChangedEventHandler(this.ConnectionManager_PropertyChanged);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM21T", (CustomPanel.ChannelLookupOptions) 5));
    this.SetCPC(this.GetChannel("CPC04T", (CustomPanel.ChannelLookupOptions) 5));
    this.SetACM(this.GetChannel("ACM21T", (CustomPanel.ChannelLookupOptions) 5));
    this.SetTCM(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 5));
    if (this.CurrentStage == UserPanel.Stage.WaitForIgnitionOnReconnection || this.CurrentStage == UserPanel.Stage.WaitForIgnitionOffDisconnection)
    {
      this.PerformCurrentStage();
    }
    else
    {
      foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
      {
        string identificationNumber = SapiManager.GetVehicleIdentificationNumber(channel);
        if (!string.IsNullOrEmpty(identificationNumber) && Utility.ValidateVehicleIdentificationNumber(identificationNumber))
          this.Vehicle = identificationNumber;
        if (this.Vehicle.Length > 0)
          break;
      }
    }
    this.UpdateUserInterface();
  }

  private void ConnectionManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if ((!(e.PropertyName == "IgnitionStatus") || this.CurrentStage != UserPanel.Stage.WaitForIgnitionOnReconnection) && this.CurrentStage != UserPanel.Stage.WaitForIgnitionOffDisconnection)
      return;
    this.PerformCurrentStage();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    ConnectionManager.GlobalInstance.PropertyChanged -= new PropertyChangedEventHandler(this.ConnectionManager_PropertyChanged);
    this.StopWork(UserPanel.Reason.Closing);
    this.SetMCM((Channel) null);
    this.SetCPC((Channel) null);
    this.SetACM((Channel) null);
    this.SetTCM((Channel) null);
  }

  public string Vehicle
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
      if (this.CurrentStage != UserPanel.Stage.WaitingForBackOnlineCPC && this.CurrentStage != UserPanel.Stage.WaitForIgnitionOffDisconnection && this.CurrentStage != UserPanel.Stage.WaitForIgnitionOnReconnection)
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
        ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 8, this.mcm.Ecu.Name, "CO_EquipmentType");
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
        this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
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

  private bool SetTCM(Channel tcm)
  {
    bool flag = false;
    if (this.tcm != tcm)
    {
      flag = true;
      this.tcm = tcm;
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
    ((Control) this.digitalReadoutInstrument1).Visible = this.mcm != null;
    this.buttonClose.Enabled = this.CanClose;
    this.buttonSetESN.Enabled = this.IsValidEsn;
    this.buttonSetVIN.Enabled = this.IsValidVIN;
    this.textBoxESN.ReadOnly = !this.CanEditEsn;
    this.textBoxVIN.ReadOnly = !this.CanSetVIN;
    if (this.textBoxESN.ReadOnly)
      this.textBoxESN.BackColor = SystemColors.Control;
    else if (this.IsSerialNumberValid(this.textBoxESN.Text))
      this.textBoxESN.BackColor = Color.PaleGreen;
    else
      this.textBoxESN.BackColor = Color.LightPink;
    if (this.textBoxVIN.ReadOnly)
      this.textBoxVIN.BackColor = SystemColors.Control;
    else if (this.IsVINValid(this.textBoxVIN.Text))
      this.textBoxVIN.BackColor = Color.PaleGreen;
    else
      this.textBoxVIN.BackColor = Color.LightPink;
  }

  private string GetEngineTypeName()
  {
    IEnumerable<EquipmentType> source = EquipmentType.ConnectedEquipmentTypes("Engine");
    if (!CollectionExtensions.Exactly<EquipmentType>(source, 1))
      return (string) null;
    EquipmentType equipmentType = source.First<EquipmentType>();
    return ((EquipmentType) ref equipmentType).Name;
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
      return this.cpc != null && this.cpc.CommunicationsState == CommunicationsState.Online || this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online || this.acm != null && this.acm.CommunicationsState == CommunicationsState.Online || this.tcm != null && this.tcm.CommunicationsState == CommunicationsState.Online;
    }
  }

  private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

  private bool CanClose => !this.Working;

  private bool CanSetESN
  {
    get
    {
      return !this.Working && this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online && (this.mcm.Services["DL_ID_Write_Engine_Serial_Number"] != (Service) null || this.mcm.Services["DL_ID_Engine_Serial_Number"] != (Service) null) && this.IsValidLicense;
    }
  }

  private bool IsValidEsn => this.CanSetESN && this.IsSerialNumberValid(this.textBoxESN.Text);

  private bool CanEditEsn
  {
    get => this.CanSetESN && this.ecuInfoESN != null && this.ecuInfoESN.Value != null;
  }

  private bool CanSetVIN => !this.Working && this.Online && this.IsValidLicense;

  private bool IsValidVIN => this.CanSetVIN && this.IsVINValid(this.textBoxVIN.Text);

  public bool IsVINValid(string text)
  {
    bool flag = false;
    if (!string.IsNullOrEmpty(text) && Utility.ValidateVehicleIdentificationNumber(text))
      flag = true;
    return flag;
  }

  public bool IsSerialNumberValid(string text)
  {
    bool flag = false;
    int num = this.GetEngineTypeName() == "S60" ? 10 : 14;
    if (!string.IsNullOrEmpty(text) && text.Length == num && UserPanel.ValidESNCharacters.IsMatch(text))
      flag = true;
    return flag;
  }

  private void ReportResult(string text)
  {
    this.LabelLog("SetESNVIN", text);
    this.textBoxOutput.AppendText(text + Environment.NewLine);
  }

  private void OnSetESNClick(object sender, EventArgs e)
  {
    if (!this.IsValidEsn)
      return;
    this.setESN = true;
    this.setVIN = false;
    this.StartWork();
  }

  private void OnSetVINClick(object sender, EventArgs e)
  {
    if (!this.IsValidVIN)
      return;
    this.setESN = false;
    this.setVIN = true;
    this.StartWork();
  }

  private void StartWork()
  {
    if (this.setESN)
      this.CurrentStage = UserPanel.Stage.SetEsn;
    else if (this.setVIN)
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
        this.CurrentStage = UserPanel.Stage.WaitingForSetEsn;
        Service service1 = this.mcm.Services["DL_ID_Write_Engine_Serial_Number"];
        if ((object) service1 == null)
          service1 = this.mcm.Services["DL_ID_Engine_Serial_Number"];
        Service service2 = service1;
        if (service2 == (Service) null)
        {
          this.StopWork(UserPanel.Reason.FailedServiceExecute);
          break;
        }
        string text1 = this.textBoxESN.Text;
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingESNTo0, (object) text1));
        service2.InputValues[0].Value = (object) text1;
        this.ExecuteAsynchronousService(service2);
        break;
      case UserPanel.Stage.WaitingForSetEsn:
        this.CurrentStage = UserPanel.Stage.CommitMCMForESN;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.CommitMCMForESN:
        if (this.mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
        {
          this.CurrentStage = UserPanel.Stage.WaitingForCommitMCMForESN;
          this.ReportResult(Resources.Message_WritingChangesToPermanentMemory);
          this.ExecuteAsynchronousService(this.mcm.Ecu.Properties["CommitToPermanentMemoryService"]);
          break;
        }
        if (this.mcm.Ecu.Name != "MR201T")
          this.ReportResult(Resources.Message_SkippingMCMCommitProcessAsTheServiceCouldNotBeFound);
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
          this.ReportResult(Resources.Message_SkippingCPCCommitProcessAsTheCPCIsNotConnected);
          this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
          this.PerformCurrentStage();
          break;
        }
        if (this.cpc.Ecu.Name != "CPC302T" && this.cpc.Ecu.Name != "CPC501T" && this.cpc.Ecu.Name != "CPC502T")
        {
          Service service3 = this.cpc.Services["FN_KeyOffOnReset"];
          if (service3 == (Service) null)
          {
            this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
            this.ReportResult(Resources.Message_SkippingCPCResetAsTheServiceCannotBeFound);
            this.PerformCurrentStage();
          }
          else
          {
            this.CurrentStage = UserPanel.Stage.WaitingForKeyOffOnCPC;
            this.ReportResult(Resources.Message_SynchronizingESNToCPCViaKeyOffOnReset);
            this.ExecuteAsynchronousService(service3);
          }
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
          this.PerformCurrentStage();
        }
        break;
      case UserPanel.Stage.WaitingForKeyOffOnCPC:
        this.CurrentStage = UserPanel.Stage.WaitingForBackOnlineCPC;
        this.ReportResult(Resources.Message_WaitingForTheCPCToComeBackOnline);
        break;
      case UserPanel.Stage.WaitingForBackOnlineCPC:
        this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForMCM:
        if (this.mcm == null)
        {
          this.ReportResult(Resources.Message_SkippingMCMSetVINProcessAsTheMCMIsNotConnected);
          this.CurrentStage = UserPanel.Stage.SetVINForACM;
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForSetMCMVIN;
        Service service4 = this.mcm.Services["DL_ID_Write_VIN_Current"];
        if ((object) service4 == null)
          service4 = this.mcm.Services["DL_ID_VIN_Current"];
        Service service5 = service4;
        if (service5 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForACM;
          this.ReportResult(Resources.Message_SkippingSettingOfMCMVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetMCMVIN;
          string text2 = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, (object) text2));
          service5.InputValues[0].Value = (object) text2;
          this.ExecuteAsynchronousService(service5);
        }
        break;
      case UserPanel.Stage.WaitingForSetMCMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForACM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForACM:
        if (this.acm == null)
        {
          this.ReportResult(Resources.Message_SkippingACMSetVINProcessAsTheACMIsNotConnected);
          this.CurrentStage = UserPanel.Stage.SetVINForTCM;
          this.PerformCurrentStage();
          break;
        }
        Service service6 = this.acm.Services["DL_ID_Write_VIN_Current"];
        if (service6 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForTCM;
          this.ReportResult(Resources.Message_SkippingSettingOfACMVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetACMVIN;
          string text3 = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, (object) text3));
          service6.InputValues[0].Value = (object) text3;
          this.ExecuteAsynchronousService(service6);
        }
        break;
      case UserPanel.Stage.WaitingForSetACMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForTCM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForTCM:
        if (this.tcm == null)
        {
          this.ReportResult(Resources.Message_SkippingTCMSetVINProcessAsTheTCMIsNotConnected);
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.PerformCurrentStage();
          break;
        }
        Service service7 = this.tcm.Services["DL_ID_VIN_Current"];
        if (service7 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.ReportResult(Resources.Message_SkippingSettingOfTCMVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetTCMVIN;
          string text4 = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, (object) text4));
          service7.InputValues[0].Value = (object) text4;
          this.ExecuteAsynchronousService(service7);
        }
        break;
      case UserPanel.Stage.WaitingForSetTCMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForCPC:
        if (this.cpc == null)
        {
          this.ReportResult(Resources.Message_SkippingCPCSetVINProcessAsTheCPCIsNotConnected);
          this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
          this.PerformCurrentStage();
          break;
        }
        Service service8 = this.cpc.Services[this.cpc.Ecu.Properties["WriteVINService"]];
        if (service8 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
          this.ReportResult(Resources.Message_SkippingSettingOfCPCVINAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetCPCVIN;
          string text5 = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, (object) text5));
          service8.InputValues[0].Value = (object) text5;
          this.ExecuteAsynchronousService(service8);
        }
        break;
      case UserPanel.Stage.WaitingForSetCPCVIN:
        this.CurrentStage = this.cpc.Ecu.Name == "CPC302T" || this.cpc.Ecu.Name == "CPC501T" || this.cpc.Ecu.Name == "CPC502T" ? UserPanel.Stage.HardResetCPC : UserPanel.Stage.TurnIgnitionOff;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.HardResetCPC:
        Service service9 = this.cpc.Services["FN_HardReset"];
        if (service9 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
          this.ReportResult(Resources.Message_SkippingCPCResetAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForHardResetCPC;
        this.ReportResult(Resources.Message_CommittingChangesToCPCViaHardReset);
        this.ExecuteAsynchronousService(service9);
        break;
      case UserPanel.Stage.WaitingForHardResetCPC:
        this.CurrentStage = UserPanel.Stage.TurnIgnitionOff;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.TurnIgnitionOff:
        if (this.mcm != null && this.mcm.Ecu.Name == "MR201T")
        {
          this.CurrentStage = UserPanel.Stage.WaitForIgnitionOffDisconnection;
          this.ReportResult(Resources.Message_TurnTheIgnitionOffToFinalizeChanges);
          break;
        }
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitForIgnitionOffDisconnection:
        if (ConnectionManager.GlobalInstance.IgnitionStatus != 1 || this.GetChannel("MR201T") != null)
          break;
        this.CurrentStage = UserPanel.Stage.WaitForIgnitionOnReconnection;
        this.ReportResult(Resources.Message_TurnTheIgnitionOnToVerifyTheChanges);
        break;
      case UserPanel.Stage.WaitForIgnitionOnReconnection:
        if (this.GetChannel("MR201T") == null || ConnectionManager.GlobalInstance.IgnitionStatus != 0)
          break;
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.Finish:
        if (this.mcm != null)
        {
          this.ReportResult(Resources.Message_ResettingMCMFaults);
          this.mcm.FaultCodes.Reset(false);
        }
        if (this.acm != null)
        {
          this.ReportResult(Resources.Message_ResettingACMFaults);
          this.acm.FaultCodes.Reset(false);
        }
        if (this.cpc != null)
        {
          this.ReportResult(Resources.Message_ResettingCPCFaults);
          this.cpc.FaultCodes.Reset(false);
        }
        Channel channel = this.GetChannel("J1939-255");
        if (channel != null)
        {
          this.ReportResult(Resources.Message_ResettingJ1939Faults);
          channel.FaultCodes.Reset(false);
        }
        this.StopWork(UserPanel.Reason.Succeeded);
        break;
      case UserPanel.Stage.Stopping:
        break;
      default:
        throw new InvalidOperationException("Unknown stage.");
    }
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
      this.ReportResult(Resources.Message_VerifyingResults);
      if (this.setESN)
      {
        this.VerifyResults(this.mcm, "MCM", "CO_ESN", "ESN", this.textBoxESN.Text);
        if (this.cpc != null && this.cpc.Ecu.Name != "CPC302T" && this.cpc.Ecu.Name != "CPC501T" && this.cpc.Ecu.Name != "CPC502T")
          this.VerifyResults(this.cpc, "CPC", "CO_ESN", "ESN", this.textBoxESN.Text);
      }
      else if (this.setVIN)
      {
        this.VerifyResults(this.mcm, "MCM", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.cpc, "CPC", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.acm, "ACM", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.tcm, "TCM", "CO_VIN", "VIN", this.textBoxVIN.Text);
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
          this.ReportResult(Resources.Message_TheMCMWasDisconnected);
          break;
      }
    }
    this.ClearCurrentService();
    this.CurrentStage = UserPanel.Stage.Idle;
  }

  private void VerifyResults(
    Channel channel,
    string channelName,
    string function,
    string functionName,
    string textBoxText)
  {
    if (channel == null || channel.EcuInfos[function] == null)
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_01CannotBeVerified, (object) channelName, (object) functionName));
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
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_FailedToReadECUInfoCannotVerifyThe01Error2, (object) channelName, (object) functionName, (object) ex.Message));
      }
      catch (InvalidOperationException ex)
      {
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_FailedToReadECUInfoThe0IsUnavailableError1, (object) channelName, (object) ex.Message));
      }
      if (string.Compare(ecuInfo.Value, textBoxText, true) == 0)
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_The01HasSuccessfullyBeenSetTo2, (object) channelName, (object) functionName, (object) textBoxText));
      else
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_The01HasNotBeenChangedAndHasAValueOf2, (object) channelName, (object) functionName, (object) ecuInfo.Value));
    }
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
      this.ReportResult($"{Resources.Message_Executing}{service.Name}…");
      this.currentService.Execute(false);
    }
  }

  private void ExecuteAsynchronousService(string serviceString)
  {
    if (!string.IsNullOrEmpty(this.currentServiceString))
      throw new InvalidOperationException("Must wait for current service to finish before continuing.");
    if (string.IsNullOrEmpty(serviceString))
    {
      this.StopWork(UserPanel.Reason.FailedServiceExecute);
    }
    else
    {
      this.currentServiceString = serviceString;
      this.mcm.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      this.ReportResult(Resources.Message_ExecutingServices);
      this.mcm.Services.Execute(serviceString, false);
    }
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.ClearCurrentService();
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
        stringBuilder.Append(": Unknown");
    }
    this.ReportResult(stringBuilder.ToString());
    return flag;
  }

  private void ClearCurrentService()
  {
    if (this.currentService != (Service) null)
    {
      this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
      this.currentService = (Service) null;
    }
    if (string.IsNullOrEmpty(this.currentServiceString))
      return;
    this.mcm.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.currentServiceString = (string) null;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelVIN = new System.Windows.Forms.Label();
    this.labelESN = new System.Windows.Forms.Label();
    this.textBoxESN = new TextBox();
    this.textBoxVIN = new TextBox();
    this.buttonSetVIN = new Button();
    this.buttonSetESN = new Button();
    this.buttonClose = new Button();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.textBoxOutput = new TextBox();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelVIN, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelESN, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxESN, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxVIN, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetVIN, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetESN, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxOutput, 0, 4);
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
    componentResourceManager.ApplyResources((object) this.buttonSetVIN, "buttonSetVIN");
    this.buttonSetVIN.Name = "buttonSetVIN";
    this.buttonSetVIN.UseCompatibleTextRendering = true;
    this.buttonSetVIN.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonSetESN, "buttonSetESN");
    this.buttonSetESN.Name = "buttonSetESN";
    this.buttonSetESN.UseCompatibleTextRendering = true;
    this.buttonSetESN.UseVisualStyleBackColor = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "CO_EquipmentType");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitleLengthPercentOfControl = 48 /*0x30*/;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxOutput, 3);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SetESN");
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
    WaitingForSetMCMVIN = 9,
    SetVINForACM = 10, // 0x0000000A
    WaitingForSetACMVIN = 11, // 0x0000000B
    SetVINForTCM = 12, // 0x0000000C
    WaitingForSetTCMVIN = 13, // 0x0000000D
    SetVINForCPC = 14, // 0x0000000E
    WaitingForSetCPCVIN = 15, // 0x0000000F
    HardResetCPC = 16, // 0x00000010
    WaitingForHardResetCPC = 17, // 0x00000011
    TurnIgnitionOff = 18, // 0x00000012
    WaitForIgnitionOffDisconnection = 19, // 0x00000013
    WaitForIgnitionOnReconnection = 20, // 0x00000014
    Finish = 21, // 0x00000015
    Stopping = 22, // 0x00000016
  }

  private enum Reason
  {
    Succeeded,
    FailedServiceExecute,
    FailedService,
    Closing,
    Disconnected,
  }
}
