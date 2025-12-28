// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel.UserPanel
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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string J1939Name = "J1939-255";
  private const string ECPC01TName = "ECPC01T";
  private const string ETCM01TName = "ETCM01T";
  private const string BMS01TName = "BMS01T";
  private const string BMS201TName = "BMS201T";
  private const string BMS301TName = "BMS301T";
  private const string BMS401TName = "BMS401T";
  private const string BMS501TName = "BMS501T";
  private const string BMS601TName = "BMS601T";
  private const string BMS701TName = "BMS701T";
  private const string BMS801TName = "BMS801T";
  private const string BMS901TName = "BMS901T";
  private const string PTI101TName = "PTI101T";
  private const string PTI201TName = "PTI201T";
  private const string PTI301TName = "PTI301T";
  private const string DCL101TName = "DCL101T";
  private const string EAPU03TName = "EAPU03T";
  private const string DCB01TName = "DCB01T";
  private const string DCB02TName = "DCB02T";
  private const string VINEcuInfo = "CO_VIN";
  private const string HardResetService = "FN_HardReset";
  private const string VehicleIdentificationNumberService = "DL_ID_VIN_Current";
  private Precondition vehicleChargingPrecondition;
  private static readonly Regex ValidESNCharacters = new Regex("(\\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
  private bool setVIN = false;
  private Channel ecpc01t;
  private Channel etcm01t;
  private Channel bms01t;
  private Channel bms201t;
  private Channel bms301t;
  private Channel bms401t;
  private Channel bms501t;
  private Channel bms601t;
  private Channel bms701t;
  private Channel bms801t;
  private Channel bms901t;
  private Channel pti101t;
  private Channel pti201t;
  private Channel pti301t;
  private Channel dcl101t;
  private Channel eapu03t;
  private Channel dcb01t;
  private Channel dcb02t;
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private Service currentService;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonClose;
  private System.Windows.Forms.Label labelVIN;
  private Button buttonSetVIN;
  private TextBox textBoxVIN;
  private System.Windows.Forms.Label labelChargingStatus;
  private TextBox textBoxOutput;

  public UserPanel()
  {
    this.InitializeComponent();
    this.textBoxVIN.TextChanged += new EventHandler(this.OnTextChanged);
    this.buttonSetVIN.Click += new EventHandler(this.OnSetVINClick);
    this.vehicleChargingPrecondition = PreconditionManager.GlobalInstance.Preconditions.FirstOrDefault<Precondition>((Func<Precondition, bool>) (p => p.PreconditionType == 1));
    this.vehicleChargingPrecondition.StateChanged += new EventHandler(this.vehicleChargingPrecondition_StateChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    this.SetECPC01T(this.GetChannel("ECPC01T"));
    this.SetETCM01T(this.GetChannel("ETCM01T"));
    this.SetBMS01T(this.GetChannel("BMS01T"));
    this.SetBMS201T(this.GetChannel("BMS201T"));
    this.SetBMS301T(this.GetChannel("BMS301T"));
    this.SetBMS401T(this.GetChannel("BMS401T"));
    this.SetBMS501T(this.GetChannel("BMS501T"));
    this.SetBMS601T(this.GetChannel("BMS601T"));
    this.SetBMS701T(this.GetChannel("BMS701T"));
    this.SetBMS801T(this.GetChannel("BMS801T"));
    this.SetBMS901T(this.GetChannel("BMS901T"));
    this.SetPTI101T(this.GetChannel("PTI101T"));
    this.SetPTI201T(this.GetChannel("PTI201T"));
    this.SetPTI301T(this.GetChannel("PTI301T"));
    this.SetDCL101T(this.GetChannel("DCL101T"));
    this.SetEAPU03T(this.GetChannel("EAPU03T"));
    this.SetDCB01T(this.GetChannel("DCB01T"));
    this.SetDCB02T(this.GetChannel("DCB02T"));
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      string identificationNumber = SapiManager.GetVehicleIdentificationNumber(channel);
      if (!string.IsNullOrEmpty(identificationNumber) && Utility.ValidateVehicleIdentificationNumber(identificationNumber))
        this.Vehicle = identificationNumber;
      if (this.Vehicle.Length > 0)
        break;
    }
    this.UpdateUserInterface();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.vehicleChargingPrecondition.StateChanged -= new EventHandler(this.vehicleChargingPrecondition_StateChanged);
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.StopWork(UserPanel.Reason.Closing);
    this.SetECPC01T((Channel) null);
    this.SetETCM01T((Channel) null);
    this.SetBMS01T((Channel) null);
    this.SetBMS201T((Channel) null);
    this.SetBMS301T((Channel) null);
    this.SetBMS401T((Channel) null);
    this.SetBMS501T((Channel) null);
    this.SetBMS601T((Channel) null);
    this.SetBMS701T((Channel) null);
    this.SetBMS801T((Channel) null);
    this.SetBMS901T((Channel) null);
    this.SetPTI101T((Channel) null);
    this.SetPTI201T((Channel) null);
    this.SetPTI301T((Channel) null);
    this.SetDCL101T((Channel) null);
    this.SetEAPU03T((Channel) null);
    this.SetDCB01T((Channel) null);
    this.SetDCB02T((Channel) null);
  }

  public string Vehicle
  {
    get => this.textBoxVIN.Text;
    set => this.textBoxVIN.Text = value;
  }

  private bool SetECPC01T(Channel cpc)
  {
    bool flag = false;
    if (this.ecpc01t != cpc)
    {
      if (this.ecpc01t != null)
        this.ecpc01t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.ecpc01t = cpc;
      if (this.ecpc01t != null)
        this.ecpc01t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetETCM01T(Channel tcm)
  {
    bool flag = false;
    if (this.etcm01t != tcm)
    {
      if (this.etcm01t != null)
        this.etcm01t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.etcm01t = tcm;
      if (this.etcm01t != null)
        this.etcm01t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS01T(Channel bms)
  {
    bool flag = false;
    if (this.bms01t != bms)
    {
      if (this.bms01t != null)
        this.bms01t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms01t = bms;
      if (this.bms01t != null)
        this.bms01t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS201T(Channel bms)
  {
    bool flag = false;
    if (this.bms201t != bms)
    {
      if (this.bms201t != null)
        this.bms201t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms201t = bms;
      if (this.bms201t != null)
        this.bms201t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS301T(Channel bms)
  {
    bool flag = false;
    if (this.bms301t != bms)
    {
      if (this.bms301t != null)
        this.bms301t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms301t = bms;
      if (this.bms301t != null)
        this.bms301t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS401T(Channel bms)
  {
    bool flag = false;
    if (this.bms401t != bms)
    {
      if (this.bms401t != null)
        this.bms401t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms401t = bms;
      if (this.bms401t != null)
        this.bms401t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS501T(Channel bms)
  {
    bool flag = false;
    if (this.bms501t != bms)
    {
      if (this.bms501t != null)
        this.bms501t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms501t = bms;
      if (this.bms501t != null)
        this.bms501t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS601T(Channel bms)
  {
    bool flag = false;
    if (this.bms601t != bms)
    {
      if (this.bms601t != null)
        this.bms601t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms601t = bms;
      if (this.bms601t != null)
        this.bms601t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS701T(Channel bms)
  {
    bool flag = false;
    if (this.bms701t != bms)
    {
      if (this.bms701t != null)
        this.bms701t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms701t = bms;
      if (this.bms701t != null)
        this.bms701t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS801T(Channel bms)
  {
    bool flag = false;
    if (this.bms801t != bms)
    {
      if (this.bms801t != null)
        this.bms801t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms801t = bms;
      if (this.bms801t != null)
        this.bms801t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetBMS901T(Channel bms)
  {
    bool flag = false;
    if (this.bms901t != bms)
    {
      if (this.bms901t != null)
        this.bms901t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.bms901t = bms;
      if (this.bms901t != null)
        this.bms901t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetPTI101T(Channel pti)
  {
    bool flag = false;
    if (this.pti101t != pti)
    {
      if (this.pti101t != null)
        this.pti101t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.pti101t = pti;
      if (this.pti101t != null)
        this.pti101t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetPTI201T(Channel pti)
  {
    bool flag = false;
    if (this.pti201t != pti)
    {
      if (this.pti201t != null)
        this.pti201t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.pti201t = pti;
      if (this.pti201t != null)
        this.pti201t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetPTI301T(Channel pti)
  {
    bool flag = false;
    if (this.pti301t != pti)
    {
      if (this.pti301t != null)
        this.pti301t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.pti301t = pti;
      if (this.pti301t != null)
        this.pti301t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetDCL101T(Channel dcl)
  {
    bool flag = false;
    if (this.dcl101t != dcl)
    {
      if (this.dcl101t != null)
        this.dcl101t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.dcl101t = dcl;
      if (this.dcl101t != null)
        this.dcl101t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetEAPU03T(Channel eapu)
  {
    bool flag = false;
    if (this.eapu03t != eapu)
    {
      if (this.eapu03t != null)
        this.eapu03t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.eapu03t = eapu;
      if (this.eapu03t != null)
        this.eapu03t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetDCB01T(Channel dcb)
  {
    bool flag = false;
    if (this.dcb01t != dcb)
    {
      if (this.dcb01t != null)
        this.dcb01t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.dcb01t = dcb;
      if (this.dcb01t != null)
        this.dcb01t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private bool SetDCB02T(Channel dcb)
  {
    bool flag = false;
    if (this.dcb02t != dcb)
    {
      if (this.dcb02t != null)
        this.dcb02t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      flag = true;
      this.dcb02t = dcb;
      if (this.dcb02t != null)
        this.dcb02t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    return flag;
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnTextChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void UpdateUserInterface()
  {
    this.buttonClose.Enabled = this.CanClose;
    this.buttonSetVIN.Enabled = this.IsValidVIN && this.vehicleChargingPrecondition.State != 2;
    this.labelChargingStatus.Text = this.vehicleChargingPrecondition.State == 2 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheVINCannotBeSet0, (object) this.vehicleChargingPrecondition.Text) : string.Empty;
    this.textBoxVIN.ReadOnly = !this.CanSetVIN;
    if (this.textBoxVIN.ReadOnly)
      this.textBoxVIN.BackColor = SystemColors.Control;
    else if (this.IsVINValid(this.textBoxVIN.Text))
      this.textBoxVIN.BackColor = Color.PaleGreen;
    else
      this.textBoxVIN.BackColor = Color.LightPink;
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
      return this.ecpc01t != null && this.ecpc01t.CommunicationsState == CommunicationsState.Online || this.etcm01t != null && this.etcm01t.CommunicationsState == CommunicationsState.Online || this.bms01t != null && this.bms01t.CommunicationsState == CommunicationsState.Online || this.bms201t != null && this.bms201t.CommunicationsState == CommunicationsState.Online || this.bms301t != null && this.bms301t.CommunicationsState == CommunicationsState.Online || this.bms401t != null && this.bms401t.CommunicationsState == CommunicationsState.Online || this.bms501t != null && this.bms501t.CommunicationsState == CommunicationsState.Online || this.bms601t != null && this.bms601t.CommunicationsState == CommunicationsState.Online || this.bms701t != null && this.bms701t.CommunicationsState == CommunicationsState.Online || this.bms801t != null && this.bms801t.CommunicationsState == CommunicationsState.Online || this.bms901t != null && this.bms901t.CommunicationsState == CommunicationsState.Online || this.pti101t != null && this.pti101t.CommunicationsState == CommunicationsState.Online || this.pti201t != null && this.pti201t.CommunicationsState == CommunicationsState.Online || this.pti301t != null && this.pti301t.CommunicationsState == CommunicationsState.Online || this.dcl101t != null && this.dcl101t.CommunicationsState == CommunicationsState.Online || this.eapu03t != null && this.eapu03t.CommunicationsState == CommunicationsState.Online || this.dcb01t != null && this.dcb01t.CommunicationsState == CommunicationsState.Online || this.dcb02t != null && this.dcb02t.CommunicationsState == CommunicationsState.Online;
    }
  }

  private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

  private bool CanClose => !this.Working;

  private bool CanSetVIN => !this.Working && this.Online && this.IsValidLicense;

  private bool IsValidVIN => this.CanSetVIN && this.IsVINValid(this.textBoxVIN.Text);

  public bool IsVINValid(string text)
  {
    bool flag = false;
    if (!string.IsNullOrEmpty(text) && Utility.ValidateVehicleIdentificationNumber(text))
      flag = true;
    return flag;
  }

  private void ReportResult(string text)
  {
    this.LabelLog("SetVIN", text);
    this.textBoxOutput.AppendText(text + Environment.NewLine);
  }

  private void OnSetVINClick(object sender, EventArgs e)
  {
    if (!this.IsValidVIN)
      return;
    this.setVIN = true;
    this.StartWork();
  }

  private void StartWork()
  {
    if (this.setVIN)
      this.CurrentStage = UserPanel.Stage.SetVINForBMS01T;
    this.PerformCurrentStage();
  }

  private void PerformCurrentStage()
  {
    switch (this.CurrentStage)
    {
      case UserPanel.Stage.Idle:
        break;
      case UserPanel.Stage.SetVINForBMS01T:
        if (this.bms01t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS01T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS201T;
          this.PerformCurrentStage();
          break;
        }
        Service service1 = this.bms01t.Services["DL_ID_VIN_Current"];
        if (service1 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS201T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS01T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS01TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS01T", (object) text));
          service1.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service1);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS01TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS201T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS201T:
        if (this.bms201t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS201T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS301T;
          this.PerformCurrentStage();
          break;
        }
        Service service2 = this.bms201t.Services["DL_ID_VIN_Current"];
        if (service2 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS301T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS201T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS201TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS201T", (object) text));
          service2.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service2);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS201TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS301T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS301T:
        if (this.bms301t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS301T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS401T;
          this.PerformCurrentStage();
          break;
        }
        Service service3 = this.bms301t.Services["DL_ID_VIN_Current"];
        if (service3 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS401T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS301T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS301TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS301T", (object) text));
          service3.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service3);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS301TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS401T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS401T:
        if (this.bms401t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS401T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS501T;
          this.PerformCurrentStage();
          break;
        }
        Service service4 = this.bms401t.Services["DL_ID_VIN_Current"];
        if (service4 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS501T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS401T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS401TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS401T", (object) text));
          service4.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service4);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS401TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS501T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS501T:
        if (this.bms501t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS501T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS601T;
          this.PerformCurrentStage();
          break;
        }
        Service service5 = this.bms501t.Services["DL_ID_VIN_Current"];
        if (service5 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS601T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS501T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS501TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS501T", (object) text));
          service5.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service5);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS501TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS601T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS601T:
        if (this.bms601t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS601T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS701T;
          this.PerformCurrentStage();
          break;
        }
        Service service6 = this.bms601t.Services["DL_ID_VIN_Current"];
        if (service6 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS701T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS601T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS601TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS601T", (object) text));
          service6.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service6);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS601TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS701T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS701T:
        if (this.bms701t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS701T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS801T;
          this.PerformCurrentStage();
          break;
        }
        Service service7 = this.bms701t.Services["DL_ID_VIN_Current"];
        if (service7 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS801T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS701T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS701TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS701T", (object) text));
          service7.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service7);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS701TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS801T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS801T:
        if (this.bms801t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS801T"));
          this.CurrentStage = UserPanel.Stage.SetVINForBMS901T;
          this.PerformCurrentStage();
          break;
        }
        Service service8 = this.bms801t.Services["DL_ID_VIN_Current"];
        if (service8 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForBMS901T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS801T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS801TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS801T", (object) text));
          service8.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service8);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS801TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForBMS901T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForBMS901T:
        if (this.bms901t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "BMS901T"));
          this.CurrentStage = UserPanel.Stage.SetVINForPTI101T;
          this.PerformCurrentStage();
          break;
        }
        Service service9 = this.bms901t.Services["DL_ID_VIN_Current"];
        if (service9 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForPTI101T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "BMS901T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetBMS901TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "BMS901T", (object) text));
          service9.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service9);
        }
        break;
      case UserPanel.Stage.WaitingForSetBMS901TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForPTI101T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForPTI101T:
        if (this.pti101t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "PTI101T"));
          this.CurrentStage = UserPanel.Stage.SetVINForPTI201T;
          this.PerformCurrentStage();
          break;
        }
        Service service10 = this.pti101t.Services["DL_ID_VIN_Current"];
        if (service10 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForPTI201T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "PTI101T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetPTI101TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "PTI101T", (object) text));
          service10.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service10);
        }
        break;
      case UserPanel.Stage.WaitingForSetPTI101TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForPTI201T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForPTI201T:
        if (this.pti201t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "PTI201T"));
          this.CurrentStage = UserPanel.Stage.SetVINForPTI301T;
          this.PerformCurrentStage();
          break;
        }
        Service service11 = this.pti201t.Services["DL_ID_VIN_Current"];
        if (service11 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForPTI301T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "PTI201T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetPTI201TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "PTI201T", (object) text));
          service11.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service11);
        }
        break;
      case UserPanel.Stage.WaitingForSetPTI201TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForPTI301T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForPTI301T:
        if (this.pti301t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "PTI301T"));
          this.CurrentStage = UserPanel.Stage.SetVINForDCL101T;
          this.PerformCurrentStage();
          break;
        }
        Service service12 = this.pti301t.Services["DL_ID_VIN_Current"];
        if (service12 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForDCL101T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "PTI301T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetPTI301TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "PTI301T", (object) text));
          service12.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service12);
        }
        break;
      case UserPanel.Stage.WaitingForSetPTI301TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForDCL101T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForDCL101T:
        if (this.dcl101t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "DCL101T"));
          this.CurrentStage = UserPanel.Stage.SetVINForEAPU03T;
          this.PerformCurrentStage();
          break;
        }
        Service service13 = this.dcl101t.Services["DL_ID_VIN_Current"];
        if (service13 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForEAPU03T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "DCL101T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetDCL101TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "DCL101T", (object) text));
          service13.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service13);
        }
        break;
      case UserPanel.Stage.WaitingForSetDCL101TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForEAPU03T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForEAPU03T:
        if (this.eapu03t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "EAPU03T"));
          this.CurrentStage = UserPanel.Stage.SetVINForDCB01T;
          this.PerformCurrentStage();
          break;
        }
        Service service14 = this.eapu03t.Services["DL_ID_VIN_Current"];
        if (service14 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForDCB01T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "EAPU03T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetEAPU03TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "EAPU03T", (object) text));
          service14.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service14);
        }
        break;
      case UserPanel.Stage.WaitingForSetEAPU03TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForDCB01T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForDCB01T:
        if (this.dcb01t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "DCB01T"));
          this.CurrentStage = UserPanel.Stage.SetVINForDCB02T;
          this.PerformCurrentStage();
          break;
        }
        Service service15 = this.dcb01t.Services["DL_ID_VIN_Current"];
        if (service15 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForDCB02T;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "DCB01T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetDCB01TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "DCB01T", (object) text));
          service15.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service15);
        }
        break;
      case UserPanel.Stage.WaitingForSetDCB01TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForDCB02T;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForDCB02T:
        if (this.dcb02t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "DCB02T"));
          this.CurrentStage = UserPanel.Stage.SetVINForTCM;
          this.PerformCurrentStage();
          break;
        }
        Service service16 = this.dcb02t.Services["DL_ID_VIN_Current"];
        if (service16 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForTCM;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "DCB02T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetDCB02TVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "DCB02T", (object) text));
          service16.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service16);
        }
        break;
      case UserPanel.Stage.WaitingForSetDCB02TVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForTCM;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForTCM:
        if (this.etcm01t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "ETCM01T"));
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.PerformCurrentStage();
          break;
        }
        Service service17 = this.etcm01t.Services["DL_ID_VIN_Current"];
        if (service17 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.SetVINForCPC;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "ETCM01T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetTCMVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "ETCM01T", (object) text));
          service17.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service17);
        }
        break;
      case UserPanel.Stage.WaitingForSetTCMVIN:
        this.CurrentStage = UserPanel.Stage.SetVINForCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.SetVINForCPC:
        if (this.ecpc01t == null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, (object) "ECPC01T"));
          this.CurrentStage = UserPanel.Stage.Finish;
          this.PerformCurrentStage();
          break;
        }
        Service service18 = this.ecpc01t.Services["DL_ID_VIN_Current"];
        if (service18 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.Finish;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, (object) "ECPC01T"));
          this.PerformCurrentStage();
        }
        else
        {
          this.CurrentStage = UserPanel.Stage.WaitingForSetCPCVIN;
          string text = this.textBoxVIN.Text;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, (object) "ETCM01T", (object) text));
          service18.InputValues[0].Value = (object) text;
          this.ExecuteAsynchronousService(service18);
        }
        break;
      case UserPanel.Stage.WaitingForSetCPCVIN:
        this.CurrentStage = UserPanel.Stage.HardResetCPC;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.HardResetCPC:
        Service service19 = this.ecpc01t.Services["FN_HardReset"];
        if (service19 == (Service) null)
        {
          this.CurrentStage = UserPanel.Stage.Finish;
          this.ReportResult(Resources.Message_SkippingCPCResetAsTheServiceCannotBeFound);
          this.PerformCurrentStage();
          break;
        }
        this.CurrentStage = UserPanel.Stage.WaitingForHardResetCPC;
        this.ReportResult(Resources.Message_CommittingChangesToCPCViaHardReset);
        this.ExecuteAsynchronousService(service19);
        break;
      case UserPanel.Stage.WaitingForHardResetCPC:
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.Finish:
        if (this.ecpc01t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "ECPC01T"));
          this.ecpc01t.FaultCodes.Reset(false);
        }
        if (this.etcm01t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "ETCM01T"));
          this.etcm01t.FaultCodes.Reset(false);
        }
        if (this.bms01t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS01T"));
          this.bms01t.FaultCodes.Reset(false);
        }
        if (this.bms201t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS201T"));
          this.bms201t.FaultCodes.Reset(false);
        }
        if (this.bms301t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS301T"));
          this.bms301t.FaultCodes.Reset(false);
        }
        if (this.bms401t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS401T"));
          this.bms401t.FaultCodes.Reset(false);
        }
        if (this.bms501t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS501T"));
          this.bms501t.FaultCodes.Reset(false);
        }
        if (this.bms601t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS601T"));
          this.bms601t.FaultCodes.Reset(false);
        }
        if (this.bms701t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS701T"));
          this.bms701t.FaultCodes.Reset(false);
        }
        if (this.bms801t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS801T"));
          this.bms801t.FaultCodes.Reset(false);
        }
        if (this.bms901t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "BMS901T"));
          this.bms901t.FaultCodes.Reset(false);
        }
        if (this.pti101t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "PTI101T"));
          this.pti101t.FaultCodes.Reset(false);
        }
        if (this.pti201t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "PTI201T"));
          this.pti201t.FaultCodes.Reset(false);
        }
        if (this.pti301t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "PTI301T"));
          this.pti301t.FaultCodes.Reset(false);
        }
        if (this.dcl101t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "DCL101T"));
          this.dcl101t.FaultCodes.Reset(false);
        }
        if (this.eapu03t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "EAPU03T"));
          this.eapu03t.FaultCodes.Reset(false);
        }
        if (this.dcb01t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "DCB01T"));
          this.dcb01t.FaultCodes.Reset(false);
        }
        if (this.dcb02t != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "DCB02T"));
          this.dcb02t.FaultCodes.Reset(false);
        }
        Channel channel = this.GetChannel("J1939-255");
        if (channel != null)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, (object) "J1939-255"));
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
      if (this.setVIN)
      {
        this.VerifyResults(this.ecpc01t, "ECPC01T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.etcm01t, "ETCM01T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms01t, "BMS01T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms201t, "BMS201T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms301t, "BMS301T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms401t, "BMS401T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms501t, "BMS501T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms601t, "BMS601T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms701t, "BMS701T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms801t, "BMS801T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.bms901t, "BMS901T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.pti101t, "PTI101T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.pti201t, "PTI201T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.pti301t, "PTI301T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.dcl101t, "DCL101T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.eapu03t, "EAPU03T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.dcb01t, "DCB01T", "CO_VIN", "VIN", this.textBoxVIN.Text);
        this.VerifyResults(this.dcb02t, "DCB02T", "CO_VIN", "VIN", this.textBoxVIN.Text);
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

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.ClearCurrentService();
    if (this.CheckCompleteResult(e, Resources.Message_ServiceExecuted, Resources.Message_ServiceError))
    {
      this.PerformCurrentStage();
    }
    else
    {
      this.ReportResult(Resources.Message_FailedToExecuteService);
      this.PerformCurrentStage();
    }
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

  private void vehicleChargingPrecondition_StateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelVIN = new System.Windows.Forms.Label();
    this.textBoxVIN = new TextBox();
    this.buttonSetVIN = new Button();
    this.buttonClose = new Button();
    this.textBoxOutput = new TextBox();
    this.labelChargingStatus = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelVIN, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxVIN, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetVIN, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxOutput, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelChargingStatus, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelVIN, "labelVIN");
    this.labelVIN.Name = "labelVIN";
    this.labelVIN.UseCompatibleTextRendering = true;
    this.textBoxVIN.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textBoxVIN, "textBoxVIN");
    this.textBoxVIN.Name = "textBoxVIN";
    componentResourceManager.ApplyResources((object) this.buttonSetVIN, "buttonSetVIN");
    this.buttonSetVIN.Name = "buttonSetVIN";
    this.buttonSetVIN.UseCompatibleTextRendering = true;
    this.buttonSetVIN.UseVisualStyleBackColor = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxOutput, 3);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.labelChargingStatus, "labelChargingStatus");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelChargingStatus, 3);
    this.labelChargingStatus.ForeColor = Color.Red;
    this.labelChargingStatus.Name = "labelChargingStatus";
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
    SetVINForBMS01T = 1,
    _StartVIN = 1,
    WaitingForSetBMS01TVIN = 2,
    SetVINForBMS201T = 3,
    WaitingForSetBMS201TVIN = 4,
    SetVINForBMS301T = 5,
    WaitingForSetBMS301TVIN = 6,
    SetVINForBMS401T = 7,
    WaitingForSetBMS401TVIN = 8,
    SetVINForBMS501T = 9,
    WaitingForSetBMS501TVIN = 10, // 0x0000000A
    SetVINForBMS601T = 11, // 0x0000000B
    WaitingForSetBMS601TVIN = 12, // 0x0000000C
    SetVINForBMS701T = 13, // 0x0000000D
    WaitingForSetBMS701TVIN = 14, // 0x0000000E
    SetVINForBMS801T = 15, // 0x0000000F
    WaitingForSetBMS801TVIN = 16, // 0x00000010
    SetVINForBMS901T = 17, // 0x00000011
    WaitingForSetBMS901TVIN = 18, // 0x00000012
    SetVINForPTI101T = 19, // 0x00000013
    WaitingForSetPTI101TVIN = 20, // 0x00000014
    SetVINForPTI201T = 21, // 0x00000015
    WaitingForSetPTI201TVIN = 22, // 0x00000016
    SetVINForPTI301T = 23, // 0x00000017
    WaitingForSetPTI301TVIN = 24, // 0x00000018
    SetVINForDCL101T = 25, // 0x00000019
    WaitingForSetDCL101TVIN = 26, // 0x0000001A
    SetVINForEAPU03T = 27, // 0x0000001B
    WaitingForSetEAPU03TVIN = 28, // 0x0000001C
    SetVINForDCB01T = 29, // 0x0000001D
    WaitingForSetDCB01TVIN = 30, // 0x0000001E
    SetVINForDCB02T = 31, // 0x0000001F
    WaitingForSetDCB02TVIN = 32, // 0x00000020
    SetVINForTCM = 33, // 0x00000021
    WaitingForSetTCMVIN = 34, // 0x00000022
    SetVINForCPC = 35, // 0x00000023
    WaitingForSetCPCVIN = 36, // 0x00000024
    HardResetCPC = 37, // 0x00000025
    WaitingForHardResetCPC = 38, // 0x00000026
    Finish = 39, // 0x00000027
    Stopping = 40, // 0x00000028
  }

  private enum Reason
  {
    Succeeded,
    FailedServiceExecute,
    FailedService,
    Closing,
  }
}
