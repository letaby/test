// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes.panel;

public class UserPanel : CustomPanel
{
  private const string readVeDocInputsQualifier = "DJ_Read_AUT64_VeDoc_Input";
  private const string unlockSeedQualifier = "DNU_Config_3_Seed_Request";
  private const string unlockKeyQualifier = "DJ_ZKB_Config_3_WriteKeyFingerprint";
  private const string clearNonErasableFaultCodesQualifier = "RT_SR087_Delete_Non_Erasable_FC_Start";
  private const string readEcuIdCodeQualifier = "DJ_Get_ECU_Serial_Number";
  private const string unlockMarkerQualifier = "DT_STO_Read_look_up_table_1_sys_look_up_table1_1m";
  private const string defaultCalculationType = "XN";
  private const string unlockSharedProcedureFormat = "SP_SecurityUnlock_{0}_Unlock{1}";
  private bool useManualUnlock;
  private byte unlockMarkerValue;
  private List<string> ecusToUnlock = new List<string>()
  {
    "ACM21T",
    "ACM301T",
    "MCM21T"
  };
  private byte[] unlockMarkerSeedKeyStyleValues = new byte[2]
  {
    (byte) 18,
    (byte) 19
  };
  private Channel selectedChannel;
  private Service clearFaultCodeService;
  private Service readInputService;
  private Service readIdCodeService;
  private Service unlockService;
  private UserPanel.CalculationTypes connectedEcuCalculationType = UserPanel.CalculationTypes.Unknown;
  private Dictionary<string, string> YKeyField = (Dictionary<string, string>) null;
  private Dictionary<string, string> VKeyField = (Dictionary<string, string>) null;
  private UserPanel.State currentState;
  private TableLayoutPanel tableLayoutPanelManualInputs;
  private Button buttonClose;
  private Button buttonRefreshInputs;
  private System.Windows.Forms.Label labelECU;
  private ComboBox comboBoxECU;
  private System.Windows.Forms.Label labelRandomNumber;
  private TextBox textBoxRN1;
  private TextBox textBoxRN2;
  private TextBox textBoxIDCode1;
  private TextBox textBoxIDCode2;
  private TextBox textBoxIDCode3;
  private TextBox textBoxIDCode4;
  private TextBox textBoxCalculationType;
  private TextBox textBoxAuth64Key1;
  private TextBox textBoxAuth64Key2;
  private TextBox textBoxAuth64Key3;
  private TextBox textBoxAuth64Key4;
  private System.Windows.Forms.Label labelIDCode;
  private System.Windows.Forms.Label labelCalculationType;
  private TextBox textBoxToolId;
  private System.Windows.Forms.Label label2;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonClearNonEraseableFaultCodes;
  private Button buttonSwitchUnlockMode;
  private SeekTimeListView seekTimeListView1;
  private System.Windows.Forms.Label labelUnlockMarker;
  private TextBox textBoxUnlockMarker;
  private System.Windows.Forms.Label label1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.YKeyField = new Dictionary<string, string>();
    this.YKeyField["0"] = "53C1";
    this.YKeyField["1"] = "43C3";
    this.YKeyField["2"] = "55B5";
    this.YKeyField["3"] = "5451";
    this.YKeyField["4"] = "3717";
    this.YKeyField["5"] = "27AD";
    this.YKeyField["6"] = "11A5";
    this.YKeyField["7"] = "713A";
    this.YKeyField["8"] = "3327";
    this.YKeyField["9"] = "5B31";
    this.YKeyField["A"] = "B157";
    this.YKeyField["B"] = "BA75";
    this.YKeyField["C"] = "2A57";
    this.YKeyField["D"] = "3154";
    this.YKeyField["E"] = "C5A7";
    this.YKeyField["F"] = "A7D3";
    this.VKeyField = new Dictionary<string, string>();
    this.VKeyField["0"] = "51A3C521";
    this.VKeyField["1"] = "41B3C923";
    this.VKeyField["2"] = "55C5B435";
    this.VKeyField["3"] = "5AB45131";
    this.VKeyField["4"] = "35371AA7";
    this.VKeyField["5"] = "2377AAD3";
    this.VKeyField["6"] = "1A31A755";
    this.VKeyField["7"] = "772135AA";
    this.VKeyField["8"] = "37232177";
    this.VKeyField["9"] = "5735B31A";
    this.VKeyField["A"] = "B4515737";
    this.VKeyField["B"] = "B31A3751";
    this.VKeyField["C"] = "21AA7757";
    this.VKeyField["D"] = "31D355B4";
    this.VKeyField["E"] = "C555A7B3";
    this.VKeyField["F"] = "AAA7D3C5";
    this.useManualUnlock = false;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.SetState(UserPanel.State.Initializing);
    this.UpdateChannels();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.ResizeForm(false);
  }

  private void InitializeForConnectedEcu()
  {
    if (this.selectedChannel == null)
      return;
    switch (this.connectedEcuCalculationType)
    {
      case UserPanel.CalculationTypes.Unknown:
        this.clearFaultCodeService = (Service) null;
        this.readInputService = (Service) null;
        this.readIdCodeService = (Service) null;
        this.unlockService = (Service) null;
        this.SetState(UserPanel.State.Unknown);
        break;
      case UserPanel.CalculationTypes.XN:
        this.clearFaultCodeService = this.selectedChannel.Services["RT_SR087_Delete_Non_Erasable_FC_Start"];
        this.readInputService = this.selectedChannel.Services["DJ_Read_AUT64_VeDoc_Input"];
        this.unlockService = (Service) null;
        this.textBoxAuth64Key1.Visible = true;
        this.textBoxAuth64Key2.Visible = true;
        this.textBoxAuth64Key3.Visible = true;
        this.textBoxAuth64Key4.Visible = true;
        break;
      case UserPanel.CalculationTypes.SeedKey:
        this.clearFaultCodeService = this.selectedChannel.Services["RT_SR087_Delete_Non_Erasable_FC_Start"];
        this.readInputService = this.selectedChannel.Services["DNU_Config_3_Seed_Request"];
        this.readIdCodeService = this.selectedChannel.Services["DJ_Get_ECU_Serial_Number"];
        this.unlockService = this.selectedChannel.Services["DJ_ZKB_Config_3_WriteKeyFingerprint"];
        this.textBoxAuth64Key1.Visible = true;
        this.textBoxAuth64Key2.Visible = false;
        this.textBoxAuth64Key3.Visible = false;
        this.textBoxAuth64Key4.Visible = false;
        break;
    }
    this.textBoxUnlockMarker.Text = this.unlockMarkerValue.ToString();
    this.textBoxCalculationType.Text = this.connectedEcuCalculationType.ToString();
    this.textBoxToolId.Text = ApplicationInformation.ComputerId.ToString();
  }

  private void DetermineCalculationType()
  {
    if (this.selectedChannel != null)
    {
      EcuInfo ecuInfo = this.selectedChannel.EcuInfos["DT_STO_Read_look_up_table_1_sys_look_up_table1_1m"];
      ecuInfo.Read(true);
      if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
      {
        if (ecuInfo.EcuInfoValues.Current.Value is Dump dump && dump.Data.Count<byte>() >= 3)
        {
          foreach (byte seedKeyStyleValue in this.unlockMarkerSeedKeyStyleValues)
          {
            if ((int) dump.Data[2] == (int) seedKeyStyleValue)
            {
              this.unlockMarkerValue = dump.Data[2];
              this.connectedEcuCalculationType = UserPanel.CalculationTypes.SeedKey;
              return;
            }
          }
        }
        this.connectedEcuCalculationType = UserPanel.CalculationTypes.XN;
        return;
      }
    }
    this.connectedEcuCalculationType = UserPanel.CalculationTypes.Unknown;
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    switch (this.currentState)
    {
      case UserPanel.State.Initializing:
      case UserPanel.State.ReadingInputs:
      case UserPanel.State.ClearingFaultCodes:
        e.Cancel = true;
        break;
    }
    if (e.Cancel)
      return;
    this.SetState(UserPanel.State.Closing);
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetChannel((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    if (this.currentState == UserPanel.State.Closing || this.currentState == UserPanel.State.ReInitializing)
      return;
    this.UpdateChannels();
  }

  private void OnCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    if (this.currentState == UserPanel.State.ReInitializing || this.connectedEcuCalculationType != UserPanel.CalculationTypes.Unknown || e.CommunicationsState != CommunicationsState.Online)
      return;
    this.DetermineCalculationType();
    if ((this.connectedEcuCalculationType == UserPanel.CalculationTypes.SeedKey || this.currentState != UserPanel.State.ReInitializing) && this.useManualUnlock)
      this.ReadInputs();
    else if (this.connectedEcuCalculationType != UserPanel.CalculationTypes.Unknown)
      this.SetState(UserPanel.State.ReadyToClearFaults);
    else
      this.SetState(UserPanel.State.Initializing);
  }

  private void UpdateChannels()
  {
    this.comboBoxECU.Items.Clear();
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      if (this.ecusToUnlock.Contains(channel.ToString()) && channel.Online)
      {
        this.comboBoxECU.Items.Add((object) channel);
        if (channel == this.selectedChannel)
          this.comboBoxECU.SelectedItem = (object) channel;
      }
    }
    if (this.comboBoxECU.Items.Count > 0)
    {
      if (this.comboBoxECU.SelectedItem == null)
        this.comboBoxECU.SelectedItem = this.comboBoxECU.Items[0];
      this.SetChannel((Channel) this.comboBoxECU.SelectedItem);
    }
    else
      this.SetChannel((Channel) null);
  }

  private void SetChannel(Channel channel)
  {
    if (channel != this.selectedChannel)
    {
      if (channel == null && this.currentState != UserPanel.State.Closing)
      {
        this.SetState(UserPanel.State.Unknown);
        this.ClearInputs();
        this.ClearKeys();
      }
      if (this.selectedChannel != null)
        this.selectedChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdateEvent);
      this.selectedChannel = channel;
      if (this.currentState != UserPanel.State.Closing && this.currentState != UserPanel.State.ClearingFaultCodes && this.currentState != UserPanel.State.ReadingInputs && this.selectedChannel != null)
      {
        if (this.currentState != UserPanel.State.ReInitializing)
          this.DetermineCalculationType();
        this.selectedChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdateEvent);
        this.InitializeForConnectedEcu();
        if ((this.connectedEcuCalculationType == UserPanel.CalculationTypes.SeedKey || this.currentState != UserPanel.State.ReInitializing) && this.useManualUnlock)
          this.ReadInputs();
        else if (this.connectedEcuCalculationType != UserPanel.CalculationTypes.Unknown)
          this.SetState(UserPanel.State.ReadyToClearFaults);
        else
          this.SetState(UserPanel.State.Initializing);
      }
    }
    if (this.selectedChannel != null)
      return;
    this.SetState(UserPanel.State.Unknown);
  }

  private void SetState(UserPanel.State newState)
  {
    if (newState == this.currentState)
      return;
    this.currentState = newState;
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    if (this.useManualUnlock)
    {
      this.buttonRefreshInputs.Visible = true;
      ((Control) this.tableLayoutPanelManualInputs).Visible = true;
      this.buttonSwitchUnlockMode.Text = Resources.Message_SwitchToServerUnlock;
    }
    else
    {
      this.buttonRefreshInputs.Visible = false;
      ((Control) this.tableLayoutPanelManualInputs).Visible = false;
      this.buttonSwitchUnlockMode.Text = Resources.Message_SwitchToManualUnlock;
    }
    switch (this.currentState)
    {
      case UserPanel.State.Initializing:
        this.comboBoxECU.Enabled = false;
        this.buttonRefreshInputs.Enabled = false;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = false;
        this.buttonSwitchUnlockMode.Enabled = false;
        this.UpdateStatus(Resources.Message_InitializingWait);
        break;
      case UserPanel.State.ReInitializing:
        this.comboBoxECU.Enabled = false;
        this.buttonRefreshInputs.Enabled = false;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = false;
        this.buttonSwitchUnlockMode.Enabled = false;
        break;
      case UserPanel.State.ReadingInputs:
        this.comboBoxECU.Enabled = false;
        this.buttonRefreshInputs.Enabled = false;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = false;
        this.buttonSwitchUnlockMode.Enabled = false;
        this.UpdateStatus(Resources.Message_ReadingInputsWait);
        break;
      case UserPanel.State.WaitingForKey:
        this.comboBoxECU.Enabled = true;
        this.buttonRefreshInputs.Enabled = true;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = true;
        this.buttonSwitchUnlockMode.Enabled = true;
        this.UpdateStatus(Resources.Message_PleaseTypeInUnlockKey);
        break;
      case UserPanel.State.ReadyToClearFaults:
        this.comboBoxECU.Enabled = true;
        this.buttonRefreshInputs.Enabled = true;
        this.buttonClearNonEraseableFaultCodes.Enabled = true;
        this.buttonClose.Enabled = true;
        this.buttonSwitchUnlockMode.Enabled = true;
        this.UpdateStatus(Resources.Message_ReadyToClearFaults);
        break;
      case UserPanel.State.ClearingFaultCodes:
        this.comboBoxECU.Enabled = false;
        this.buttonRefreshInputs.Enabled = false;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = false;
        this.buttonSwitchUnlockMode.Enabled = false;
        this.UpdateStatus(Resources.Message_ClearingFaultCodesWait);
        break;
      case UserPanel.State.Done:
        this.comboBoxECU.Enabled = true;
        this.buttonRefreshInputs.Enabled = true;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = true;
        this.buttonSwitchUnlockMode.Enabled = true;
        this.UpdateStatus(Resources.Message_Done);
        break;
      case UserPanel.State.Closing:
        this.comboBoxECU.Enabled = false;
        this.buttonRefreshInputs.Enabled = false;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = false;
        this.buttonSwitchUnlockMode.Enabled = false;
        this.UpdateStatus(Resources.Message_Closing);
        break;
      default:
        this.comboBoxECU.Enabled = false;
        this.buttonRefreshInputs.Enabled = false;
        this.buttonClearNonEraseableFaultCodes.Enabled = false;
        this.buttonClose.Enabled = true;
        this.buttonSwitchUnlockMode.Enabled = true;
        this.UpdateStatus(Resources.Message_Unknown);
        break;
    }
  }

  private void UpdateStatus(string message)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.StatusFormat, (object) message));
  }

  private void comboBoxECU_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.SetChannel((Channel) this.comboBoxECU.SelectedItem);
  }

  private void buttonRefreshInputs_Click(object sender, EventArgs e)
  {
    this.ClearInputs();
    this.ReadInputs();
  }

  private void ReadInputs()
  {
    this.ClearInputs();
    this.ClearKeys();
    switch (this.connectedEcuCalculationType)
    {
      case UserPanel.CalculationTypes.XN:
        this.ReadSeedValues();
        break;
      case UserPanel.CalculationTypes.SeedKey:
        this.ReadEcuIdCode();
        break;
    }
  }

  private void ReadEcuIdCode()
  {
    if (this.selectedChannel == null || !(this.readIdCodeService != (Service) null))
      return;
    this.SetState(UserPanel.State.ReadingInputs);
    this.readIdCodeService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnReadIdCodeValuesComplete);
    this.readIdCodeService.Execute(false);
  }

  private void OnReadIdCodeValuesComplete(object sender, ResultEventArgs e)
  {
    this.readIdCodeService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnReadIdCodeValuesComplete);
    if (e.Succeeded && this.readIdCodeService.OutputValues != null)
    {
      string str = this.readIdCodeService.OutputValues[0].Value.ToString();
      if (str.Length < 8)
        return;
      this.textBoxIDCode1.Text = str.Substring(0, 2);
      this.textBoxIDCode2.Text = str.Substring(2, 2);
      this.textBoxIDCode3.Text = str.Substring(4, 2);
      this.textBoxIDCode4.Text = str.Substring(6, 2);
      this.ReadSeedValues();
    }
    else
    {
      this.ClearInputs();
      this.UpdateStatus(Resources.Message_FailureReadingInputs);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void ReadSeedValues()
  {
    if (this.selectedChannel == null || !(this.readInputService != (Service) null))
      return;
    if (this.connectedEcuCalculationType == UserPanel.CalculationTypes.XN)
      this.selectedChannel.Instruments.AutoRead = false;
    this.SetState(UserPanel.State.ReadingInputs);
    this.readInputService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnReadSeedValuesComplete);
    this.readInputService.Execute(false);
  }

  private void OnReadSeedValuesComplete(object sender, ResultEventArgs e)
  {
    this.readInputService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnReadSeedValuesComplete);
    if (e.Succeeded && this.readInputService.OutputValues != null)
    {
      switch (this.connectedEcuCalculationType)
      {
        case UserPanel.CalculationTypes.XN:
          string str1 = this.readInputService.OutputValues[0].Value.ToString();
          string str2 = this.readInputService.OutputValues[3].Value.ToString();
          if (str1.Length < 4 || str2.Length < 8)
            return;
          this.textBoxRN1.Text = str1.Substring(0, 2);
          this.textBoxRN2.Text = str1.Substring(2, 2);
          this.textBoxIDCode1.Text = str2.Substring(0, 2);
          this.textBoxIDCode2.Text = str2.Substring(2, 2);
          this.textBoxIDCode3.Text = str2.Substring(4, 2);
          this.textBoxIDCode4.Text = str2.Substring(6, 2);
          this.SetState(UserPanel.State.ReInitializing);
          this.textBoxAuth64Key1.Focus();
          this.ResetECUConnection();
          return;
        case UserPanel.CalculationTypes.SeedKey:
          string str3 = this.readInputService.OutputValues[0].Value.ToString();
          if (str3.Length < 4)
            return;
          this.textBoxRN1.Text = str3.Substring(0, 2);
          this.textBoxRN2.Text = str3.Substring(2, 2);
          if (str3.Equals("0000", StringComparison.OrdinalIgnoreCase))
          {
            this.textBoxAuth64Key1.Text = "0000";
            this.buttonClearNonEraseableFaultCodes.Focus();
            this.SetState(UserPanel.State.ReadyToClearFaults);
          }
          else
          {
            this.textBoxAuth64Key1.Focus();
            this.SetState(UserPanel.State.WaitingForKey);
          }
          return;
      }
    }
    this.ClearInputs();
    this.UpdateStatus(Resources.Message_FailureReadingInputs);
    this.SetState(UserPanel.State.Done);
    this.ResetECUConnection();
  }

  private void ResetECUConnection()
  {
    this.selectedChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.selectedChannel_CommunicationsStateUpdateEvent);
    SapiManager.GlobalInstance.CloseConnection(this.selectedChannel);
  }

  private void selectedChannel_CommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    if (this.selectedChannel == null)
      return;
    if (e.CommunicationsState == CommunicationsState.Offline)
    {
      this.selectedChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.selectedChannel_CommunicationsStateUpdateEvent);
      SapiManager.GlobalInstance.ChannelInitializingEvent += new EventHandler<ChannelInitializingEventArgs>(this.GlobalInstance_ChannelInitializingEvent);
      SapiManager.GlobalInstance.OpenConnection(this.selectedChannel.ConnectionResource);
    }
    else if (e.CommunicationsState == CommunicationsState.Online && this.currentState != UserPanel.State.WaitingForKey)
    {
      this.SetState(UserPanel.State.WaitingForKey);
      this.selectedChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.selectedChannel_CommunicationsStateUpdateEvent);
      this.UpdateChannels();
    }
  }

  private void GlobalInstance_ChannelInitializingEvent(
    object sender,
    ChannelInitializingEventArgs e)
  {
    Channel channel = e.Channel;
    if (channel == null || this.currentState != UserPanel.State.ReInitializing || string.CompareOrdinal(this.selectedChannel.Ecu.Name, channel.Ecu.Name) != 0)
      return;
    this.SetChannel(channel);
    this.selectedChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.selectedChannel_CommunicationsStateUpdateEvent);
    SapiManager.GlobalInstance.ChannelInitializingEvent -= new EventHandler<ChannelInitializingEventArgs>(this.GlobalInstance_ChannelInitializingEvent);
  }

  private void textBoxAuth64Key_KeyPress(object sender, KeyPressEventArgs e)
  {
    switch (this.connectedEcuCalculationType)
    {
      case UserPanel.CalculationTypes.XN:
        if (!char.IsLetter(e.KeyChar) && !char.IsSymbol(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
          break;
        e.Handled = true;
        break;
      case UserPanel.CalculationTypes.SeedKey:
        if (!char.IsSymbol(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
          break;
        e.Handled = true;
        break;
    }
  }

  private void textBoxAuth64Key1_TextChanged(object sender, EventArgs e)
  {
    switch (this.connectedEcuCalculationType)
    {
      case UserPanel.CalculationTypes.XN:
        if (!this.checkAut64KeyTextBox(this.textBoxAuth64Key1) || this.textBoxAuth64Key1.Text.Length != 3)
          break;
        this.textBoxAuth64Key2.Focus();
        this.textBoxAuth64Key2.SelectAll();
        break;
      case UserPanel.CalculationTypes.SeedKey:
        if (this.textBoxAuth64Key1.Text.Length != 4)
          break;
        this.SetState(UserPanel.State.ReadyToClearFaults);
        this.buttonClearNonEraseableFaultCodes.Focus();
        break;
    }
  }

  private void textBoxAuth64Key2_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.textBoxAuth64Key2) || this.textBoxAuth64Key2.Text.Length != 3)
      return;
    this.textBoxAuth64Key3.Focus();
    this.textBoxAuth64Key3.SelectAll();
  }

  private void textBoxAuth64Key3_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.textBoxAuth64Key3) || this.textBoxAuth64Key3.Text.Length != 3)
      return;
    this.textBoxAuth64Key4.Focus();
    this.textBoxAuth64Key4.SelectAll();
  }

  private void textBoxAuth64Key4_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.textBoxAuth64Key4) || this.textBoxAuth64Key4.Text.Length != 3)
      return;
    this.buttonClearNonEraseableFaultCodes.Focus();
  }

  private bool checkAut64KeyTextBox(TextBox textBox)
  {
    if (!this.checkKeyTextBox(textBox))
    {
      this.SetState(UserPanel.State.WaitingForKey);
      textBox.Focus();
      return false;
    }
    if (this.checkAllKeys())
      this.SetState(UserPanel.State.ReadyToClearFaults);
    else
      this.SetState(UserPanel.State.WaitingForKey);
    return true;
  }

  private bool checkAllKeys()
  {
    if (!this.checkKeyTextBox(this.textBoxAuth64Key1) || !this.checkKeyTextBox(this.textBoxAuth64Key2) || !this.checkKeyTextBox(this.textBoxAuth64Key3) || !this.checkKeyTextBox(this.textBoxAuth64Key4))
      return false;
    if (this.ValidateVeDocKey())
      return true;
    this.UpdateStatus(Resources.Message_UnlockKeyIsIncorrect);
    return false;
  }

  private bool checkKeyTextBox(TextBox textBox)
  {
    if (textBox.Text.Length < 1)
      return false;
    if (textBox.Text.Length > 3)
    {
      this.UpdateStatus(Resources.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range);
      textBox.SelectAll();
      return false;
    }
    bool flag = byte.TryParse(textBox.Text, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out byte _);
    if (!flag)
    {
      this.UpdateStatus(Resources.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range);
      textBox.SelectAll();
    }
    return flag;
  }

  private void ClearInputs()
  {
    this.textBoxIDCode1.Text = "";
    this.textBoxIDCode2.Text = "";
    this.textBoxIDCode3.Text = "";
    this.textBoxIDCode4.Text = "";
    this.textBoxRN1.Text = "";
    this.textBoxRN2.Text = "";
  }

  private void ClearKeys()
  {
    this.textBoxAuth64Key1.Text = "";
    this.textBoxAuth64Key2.Text = "";
    this.textBoxAuth64Key3.Text = "";
    this.textBoxAuth64Key4.Text = "";
    this.textBoxAuth64Key1.Focus();
  }

  private void buttonClearNonEraseableFaultCodes_Click(object sender, EventArgs e)
  {
    this.SetState(UserPanel.State.ClearingFaultCodes);
    if (this.useManualUnlock)
    {
      switch (this.connectedEcuCalculationType)
      {
        case UserPanel.CalculationTypes.XN:
          this.PerformFaultCodeClear();
          break;
        case UserPanel.CalculationTypes.SeedKey:
          this.PerformUnlockForFaultCodesClear();
          break;
      }
    }
    else
      this.PerformServerUnlockAndClear();
  }

  private void PerformServerUnlockAndClear()
  {
    string str = $"SP_SecurityUnlock_{this.selectedChannel.Ecu.Name}_Unlock{this.connectedEcuCalculationType.ToString()}";
    SharedProcedureBase availableProcedure = SharedProcedureBase.AvailableProcedures[str];
    if (availableProcedure != null)
    {
      if (availableProcedure.CanStart)
      {
        availableProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
        availableProcedure.Start();
        return;
      }
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", (object) availableProcedure.Name));
    }
    else
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", (object) str));
    this.SetState(UserPanel.State.Done);
  }

  private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result == 1)
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} unlock via the server was initiated using procedure {1}", (object) this.selectedChannel.Ecu.Name, (object) sharedProcedureBase.Name));
      sharedProcedureBase.StopComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    }
    else
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
      this.SetState(UserPanel.State.Done);
    }
  }

  private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    if (!((ResultEventArgs) e).Succeeded || e.Result == 0)
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
      if (((ResultEventArgs) e).Succeeded)
        this.SetState(UserPanel.State.ReadyToClearFaults);
      else
        this.SetState(UserPanel.State.Done);
    }
    else
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} was unlocked via the server using procedure {1}", (object) this.selectedChannel.Ecu.Name, (object) sharedProcedureBase.Name));
      this.PerformFaultCodeClear();
    }
  }

  private void PerformUnlockForFaultCodesClear()
  {
    if (!(this.unlockService != (Service) null))
      return;
    this.unlockService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.unlockService_OnUnlockServiceServiceComplete);
    this.unlockService.InputValues[0].Value = (object) this.textBoxAuth64Key1.Text;
    this.unlockService.Execute(false);
  }

  private void unlockService_OnUnlockServiceServiceComplete(object sender, ResultEventArgs e)
  {
    this.unlockService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.unlockService_OnUnlockServiceServiceComplete);
    if (e.Succeeded)
    {
      this.PerformFaultCodeClear();
    }
    else
    {
      this.ClearInputs();
      this.ClearKeys();
      this.ReadInputs();
      this.SetState(UserPanel.State.ReadingInputs);
    }
  }

  private void PerformFaultCodeClear()
  {
    if (!(this.clearFaultCodeService != (Service) null))
      return;
    this.clearFaultCodeService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.currentService_OnClearNonErasableFaultCodesServiceComplete);
    try
    {
      this.clearFaultCodeService.Execute(true);
    }
    catch (Exception ex)
    {
      this.UpdateStatus(ex.Message);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void currentService_OnClearNonErasableFaultCodesServiceComplete(
    object sender,
    ResultEventArgs e)
  {
    this.clearFaultCodeService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.currentService_OnClearNonErasableFaultCodesServiceComplete);
    this.ClearKeys();
    this.SetState(UserPanel.State.Done);
  }

  private bool ValidateVeDocKey()
  {
    string str = this.YKeyField[this.textBoxRN1.Text.Substring(1, 1)];
    string hexString1 = this.VKeyField[this.textBoxRN2.Text.Substring(0, 1)];
    string hexString2 = $"{this.textBoxIDCode1.Text}{this.textBoxIDCode2.Text}{this.textBoxIDCode3.Text}{this.textBoxIDCode4.Text}";
    string hexString3 = $"{this.textBoxRN1.Text}{this.textBoxRN2.Text}{str}";
    byte[] numArray = this.XOR(this.ConvertToByteArray(hexString2), this.ConvertToByteArray(hexString3));
    byte[] byteArray = this.ConvertToByteArray(hexString1);
    if (BitConverter.IsLittleEndian)
    {
      Array.Reverse((Array) numArray);
      Array.Reverse((Array) byteArray);
    }
    string[] stringArray = this.ConvertToStringArray(this.SwapFinalResult((BitConverter.ToUInt32(numArray, 0) + BitConverter.ToUInt32(byteArray, 0)).ToString("X")));
    return this.CompareValues(this.textBoxAuth64Key1.Text, stringArray[0]) && this.CompareValues(this.textBoxAuth64Key2.Text, stringArray[1]) && this.CompareValues(this.textBoxAuth64Key3.Text, stringArray[2]) && this.CompareValues(this.textBoxAuth64Key4.Text, stringArray[3]);
  }

  private bool CompareValues(string textBoxValue, string calculatedResult)
  {
    int result1;
    int result2;
    return int.TryParse(textBoxValue, out result1) && int.TryParse(calculatedResult, out result2) && result1 == result2;
  }

  private byte[] ConvertToByteArray(string hexString)
  {
    byte[] byteArray = new byte[hexString.Length / 2];
    for (int index = 0; index < byteArray.Length; ++index)
    {
      string s = hexString.Substring(index * 2, 2);
      byteArray[index] = byte.Parse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
    }
    return byteArray;
  }

  private byte[] XOR(byte[] num1, byte[] num2)
  {
    byte[] numArray = new byte[num1.Length];
    for (int index = 0; index < num1.Length; ++index)
      numArray[index] = (byte) ((uint) num1[index] ^ (uint) num2[index]);
    return numArray;
  }

  private string SwapFinalResult(string s)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < s.Length; ++index)
    {
      if (index == 0 || index == s.Length - 2)
      {
        stringBuilder.Append(s[index + 1]);
        stringBuilder.Append(s[index]);
        ++index;
      }
      else
        stringBuilder.Append(s[index]);
    }
    return stringBuilder.ToString();
  }

  private string[] ConvertToStringArray(string s)
  {
    string[] stringArray = BitConverter.ToString(this.ConvertToByteArray(s)).Split('-');
    int index1 = 0;
    foreach (string str1 in stringArray)
    {
      string str2 = Convert.ToUInt32(str1, 16 /*0x10*/).ToString();
      if (str2.Length < 3)
      {
        int length = str2.Length;
        for (int index2 = 0; index2 < 3 - length; ++index2)
          str2 = $"0{str2}";
      }
      stringArray[index1] = str2;
      ++index1;
    }
    return stringArray;
  }

  private void ResizeForm(bool largeSize)
  {
    if (largeSize)
    {
      ((TableLayoutPanel) this.tableLayoutPanel2).RowStyles[1].SizeType = SizeType.AutoSize;
    }
    else
    {
      ((TableLayoutPanel) this.tableLayoutPanel2).RowStyles[1].SizeType = SizeType.Absolute;
      ((TableLayoutPanel) this.tableLayoutPanel2).RowStyles[1].Height = 0.0f;
    }
  }

  private void buttonSwitchUnlockMode_Click(object sender, EventArgs e)
  {
    this.useManualUnlock = !this.useManualUnlock;
    if (this.useManualUnlock)
    {
      this.SetState(UserPanel.State.Initializing);
      this.ResizeForm(true);
      this.UpdateUI();
      this.ReadInputs();
    }
    else
    {
      this.SetState(UserPanel.State.ReadyToClearFaults);
      this.ResizeForm(false);
      this.UpdateUI();
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelManualInputs = new TableLayoutPanel();
    this.label2 = new System.Windows.Forms.Label();
    this.labelRandomNumber = new System.Windows.Forms.Label();
    this.labelIDCode = new System.Windows.Forms.Label();
    this.labelCalculationType = new System.Windows.Forms.Label();
    this.label1 = new System.Windows.Forms.Label();
    this.textBoxToolId = new TextBox();
    this.textBoxAuth64Key1 = new TextBox();
    this.textBoxAuth64Key2 = new TextBox();
    this.textBoxAuth64Key3 = new TextBox();
    this.textBoxAuth64Key4 = new TextBox();
    this.textBoxIDCode1 = new TextBox();
    this.textBoxCalculationType = new TextBox();
    this.textBoxRN1 = new TextBox();
    this.textBoxRN2 = new TextBox();
    this.textBoxIDCode2 = new TextBox();
    this.textBoxIDCode3 = new TextBox();
    this.textBoxIDCode4 = new TextBox();
    this.labelECU = new System.Windows.Forms.Label();
    this.comboBoxECU = new ComboBox();
    this.buttonClose = new Button();
    this.buttonRefreshInputs = new Button();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonSwitchUnlockMode = new Button();
    this.buttonClearNonEraseableFaultCodes = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.labelUnlockMarker = new System.Windows.Forms.Label();
    this.textBoxUnlockMarker = new TextBox();
    ((Control) this.tableLayoutPanelManualInputs).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelManualInputs, "tableLayoutPanelManualInputs");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.tableLayoutPanelManualInputs, 5);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.label2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.labelRandomNumber, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.labelIDCode, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.labelCalculationType, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.label1, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxToolId, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxAuth64Key1, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxAuth64Key2, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxAuth64Key3, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxAuth64Key4, 4, 5);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxIDCode1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxCalculationType, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxRN1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxRN2, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxIDCode2, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxIDCode3, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxIDCode4, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.labelUnlockMarker, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).Controls.Add((Control) this.textBoxUnlockMarker, 1, 3);
    ((Control) this.tableLayoutPanelManualInputs).Name = "tableLayoutPanelManualInputs";
    ((Panel) this.tableLayoutPanelManualInputs).TabStop = true;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelRandomNumber, "labelRandomNumber");
    this.labelRandomNumber.Name = "labelRandomNumber";
    this.labelRandomNumber.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelIDCode, "labelIDCode");
    this.labelIDCode.Name = "labelIDCode";
    this.labelIDCode.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelCalculationType, "labelCalculationType");
    this.labelCalculationType.Name = "labelCalculationType";
    this.labelCalculationType.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxToolId, "textBoxToolId");
    ((TableLayoutPanel) this.tableLayoutPanelManualInputs).SetColumnSpan((Control) this.textBoxToolId, 3);
    this.textBoxToolId.Name = "textBoxToolId";
    this.textBoxToolId.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxAuth64Key1, "textBoxAuth64Key1");
    this.textBoxAuth64Key1.Name = "textBoxAuth64Key1";
    this.textBoxAuth64Key1.TextChanged += new EventHandler(this.textBoxAuth64Key1_TextChanged);
    this.textBoxAuth64Key1.KeyPress += new KeyPressEventHandler(this.textBoxAuth64Key_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxAuth64Key2, "textBoxAuth64Key2");
    this.textBoxAuth64Key2.Name = "textBoxAuth64Key2";
    this.textBoxAuth64Key2.TextChanged += new EventHandler(this.textBoxAuth64Key2_TextChanged);
    this.textBoxAuth64Key2.KeyPress += new KeyPressEventHandler(this.textBoxAuth64Key_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxAuth64Key3, "textBoxAuth64Key3");
    this.textBoxAuth64Key3.Name = "textBoxAuth64Key3";
    this.textBoxAuth64Key3.TextChanged += new EventHandler(this.textBoxAuth64Key3_TextChanged);
    this.textBoxAuth64Key3.KeyPress += new KeyPressEventHandler(this.textBoxAuth64Key_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxAuth64Key4, "textBoxAuth64Key4");
    this.textBoxAuth64Key4.Name = "textBoxAuth64Key4";
    this.textBoxAuth64Key4.TextChanged += new EventHandler(this.textBoxAuth64Key4_TextChanged);
    this.textBoxAuth64Key4.KeyPress += new KeyPressEventHandler(this.textBoxAuth64Key_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxIDCode1, "textBoxIDCode1");
    this.textBoxIDCode1.Name = "textBoxIDCode1";
    this.textBoxIDCode1.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxCalculationType, "textBoxCalculationType");
    this.textBoxCalculationType.Name = "textBoxCalculationType";
    this.textBoxCalculationType.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxRN1, "textBoxRN1");
    this.textBoxRN1.Name = "textBoxRN1";
    this.textBoxRN1.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxRN2, "textBoxRN2");
    this.textBoxRN2.Name = "textBoxRN2";
    this.textBoxRN2.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxIDCode2, "textBoxIDCode2");
    this.textBoxIDCode2.Name = "textBoxIDCode2";
    this.textBoxIDCode2.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxIDCode3, "textBoxIDCode3");
    this.textBoxIDCode3.Name = "textBoxIDCode3";
    this.textBoxIDCode3.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxIDCode4, "textBoxIDCode4");
    this.textBoxIDCode4.Name = "textBoxIDCode4";
    this.textBoxIDCode4.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.labelECU, "labelECU");
    this.labelECU.Name = "labelECU";
    this.labelECU.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.comboBoxECU, "comboBoxECU");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.comboBoxECU, 3);
    this.comboBoxECU.FormattingEnabled = true;
    this.comboBoxECU.Name = "comboBoxECU";
    this.comboBoxECU.SelectedIndexChanged += new EventHandler(this.comboBoxECU_SelectedIndexChanged);
    this.buttonClose.DialogResult = DialogResult.Cancel;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonRefreshInputs, "buttonRefreshInputs");
    this.buttonRefreshInputs.Name = "buttonRefreshInputs";
    this.buttonRefreshInputs.UseCompatibleTextRendering = true;
    this.buttonRefreshInputs.UseVisualStyleBackColor = true;
    this.buttonRefreshInputs.Click += new EventHandler(this.buttonRefreshInputs_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 4, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelECU, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonSwitchUnlockMode, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanelManualInputs, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonRefreshInputs, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClearNonEraseableFaultCodes, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.comboBoxECU, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.seekTimeListView1, 0, 2);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonSwitchUnlockMode, "buttonSwitchUnlockMode");
    this.buttonSwitchUnlockMode.Name = "buttonSwitchUnlockMode";
    this.buttonSwitchUnlockMode.UseCompatibleTextRendering = true;
    this.buttonSwitchUnlockMode.UseVisualStyleBackColor = true;
    this.buttonSwitchUnlockMode.Click += new EventHandler(this.buttonSwitchUnlockMode_Click);
    componentResourceManager.ApplyResources((object) this.buttonClearNonEraseableFaultCodes, "buttonClearNonEraseableFaultCodes");
    this.buttonClearNonEraseableFaultCodes.Name = "buttonClearNonEraseableFaultCodes";
    this.buttonClearNonEraseableFaultCodes.UseCompatibleTextRendering = true;
    this.buttonClearNonEraseableFaultCodes.UseVisualStyleBackColor = true;
    this.buttonClearNonEraseableFaultCodes.Click += new EventHandler(this.buttonClearNonEraseableFaultCodes_Click);
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.seekTimeListView1, 5);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "ClearNonErasableMR2";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.labelUnlockMarker, "labelUnlockMarker");
    this.labelUnlockMarker.Name = "labelUnlockMarker";
    this.labelUnlockMarker.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxUnlockMarker, "textBoxUnlockMarker");
    this.textBoxUnlockMarker.Name = "textBoxUnlockMarker";
    this.textBoxUnlockMarker.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelManualInputs).ResumeLayout(false);
    ((Control) this.tableLayoutPanelManualInputs).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    Unknown,
    Initializing,
    ReInitializing,
    ReadingInputs,
    WaitingForKey,
    WaitingForServerConnection,
    ReadyToClearFaults,
    ClearingFaultCodes,
    Done,
    Closing,
  }

  private enum CalculationTypes
  {
    Unknown,
    XN,
    SeedKey,
  }
}
