// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel.UserPanel
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
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

public class UserPanel : CustomPanel
{
  private const string Checksum = "XXX";
  private const string CalculationType = "XE";
  private const int UnlockMinimumAccessLevel = 3;
  private const string unlockSharedProcedureFormat = "SP_SecurityUnlock_{0}_Unlock{1}";
  private Channel selectedChannel;
  private bool unlockSupported;
  private bool busy;
  private bool closing;
  private List<string> ecusToUnlock = new List<string>()
  {
    "ACM21T",
    "ACM301T",
    "MCM21T"
  };
  private bool useManualUnlock;
  private readonly Dictionary<string, VedocReadInputDataQualifiers> VeDocInputResponseQualifiers = new Dictionary<string, VedocReadInputDataQualifiers>()
  {
    {
      "ACM21T",
      VedocReadInputDataQualifiers.Create("ZUFALLSZAHL", "ID_CODE", "ANZAHL_SCHLUESSEL", "TRANSPONDER_CODE")
    },
    {
      "ACM301T",
      VedocReadInputDataQualifiers.Create("ZUFALLSZAHL", "ID_CODE", "ANZAHL_SCHLUESSEL", "TRANSPONDER_CODE")
    },
    {
      "MCM21T",
      VedocReadInputDataQualifiers.Create("ZUFALLSZAHL", "ID_CODE", "ANZAHL_SCHLUESSEL", "TRANSPONDER_CODE")
    }
  };
  private readonly Dictionary<string, string> ReadActivationModeQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration"
    },
    {
      "ACM301T",
      "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration"
    },
    {
      "MCM21T",
      "DT_STO_ID_Read0139_xcp_ccp_activation_mode_Function_supported_by_calibration"
    }
  };
  private readonly Dictionary<string, string> ReadActivationStatusQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU"
    },
    {
      "ACM301T",
      "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU"
    },
    {
      "MCM21T",
      "DT_STO_ID_Read0139_xcp_ccp_activation_mode_State_of_measurement_CAN_in_ROM_ECU"
    }
  };
  private readonly Dictionary<string, string> ReadVeDocInputQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "DJ_Read_AUT64_VeDoc_Input_In_Application"
    },
    {
      "ACM301T",
      "DJ_Read_AUT64_VeDoc_Input_In_Application"
    },
    {
      "MCM21T",
      "DJ_Read_AUT64_VeDoc_Input_for_UDS"
    }
  };
  private readonly Dictionary<string, string> ReadVeDocInputIdCodeOverrideQualifier = new Dictionary<string, string>()
  {
    {
      "MCM21T",
      "DT_STO_ID_Read_Curent_ECU_ID_ECU_ID_Current"
    }
  };
  private readonly Dictionary<string, string> UnlockAut64SecurityQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2"
    },
    {
      "ACM301T",
      "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2"
    },
    {
      "MCM21T",
      "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2"
    }
  };
  private readonly Dictionary<string, string> EnableXCPCCPQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"
    },
    {
      "ACM301T",
      "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"
    },
    {
      "MCM21T",
      "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"
    }
  };
  private readonly Dictionary<string, string> DisableXCPCCPQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start"
    },
    {
      "ACM301T",
      "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start"
    },
    {
      "MCM21T",
      "RT_SR0506_Deactivation_of_XCP_CCP_communication_Start"
    }
  };
  private readonly Dictionary<string, string> ResetEcuQualifier = new Dictionary<string, string>()
  {
    {
      "ACM21T",
      "FN_KeyOffOnReset"
    },
    {
      "ACM301T",
      "FN_KeyOffOnReset"
    },
    {
      "MCM21T",
      "FN_KeyOffOnReset"
    }
  };
  private Service readVeDocInputService = (Service) null;
  private Service disableXCPCCPService = (Service) null;
  private bool unlockingToEnableXCPCCP;
  private Service unlockVeDocService = (Service) null;
  private Service enableXCPCCPService = (Service) null;
  private string commitServiceList = string.Empty;
  private Service resetEcuService = (Service) null;
  private TableLayoutPanel tableLayoutPanel1;
  private System.Windows.Forms.Label label8;
  private TextBox textBoxXFactor;
  private TextBox textBoxChallengeCode;
  private TextBox textBoxNumberOfCode;
  private TextBox textBoxTransponderCode;
  private TextBox textBoxEcuIdCode;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label IDCodeLabel;
  private System.Windows.Forms.Label TranspoderCodeLabel;
  private System.Windows.Forms.Label NumberOfKeysLabel;
  private System.Windows.Forms.Label RandomNumberLabel;
  private System.Windows.Forms.Label XFactorValueLabel;
  private System.Windows.Forms.Label UnlockKeyLabel;
  private TextBox textBoxActivationMode;
  private ComboBox comboBoxEcuToUnlock;
  private FlowLayoutPanel flowLayoutPanel1;
  private TextBox textBoxKey1;
  private TextBox textBoxKey2;
  private TextBox textBoxKey3;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonRefresh;
  private Button buttonEnable;
  private Button buttonDisable;
  private Button buttonClose;
  private SeekTimeListView seekTimeListView1;
  private TextBox textBoxChecksum;
  private System.Windows.Forms.Label ChecksumLabel;
  private Button buttonSwitchUnlockMode;
  private TextBox textBoxKey4;

  public UserPanel()
  {
    this.InitializeComponent();
    this.useManualUnlock = false;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    if (this.closing)
      return;
    this.UpdateChannels();
  }

  private void UpdateChannels()
  {
    this.comboBoxEcuToUnlock.Items.Clear();
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      if (this.ecusToUnlock.Contains(channel.ToString()) && channel.Online)
      {
        this.comboBoxEcuToUnlock.Items.Add((object) channel);
        if (channel == this.selectedChannel)
          this.comboBoxEcuToUnlock.SelectedItem = (object) channel;
      }
    }
    if (this.comboBoxEcuToUnlock.Items.Count > 0)
    {
      if (this.comboBoxEcuToUnlock.SelectedItem == null)
        this.comboBoxEcuToUnlock.SelectedItem = this.comboBoxEcuToUnlock.Items[0];
      this.SetChannel((Channel) this.comboBoxEcuToUnlock.SelectedItem);
    }
    else
      this.SetChannel((Channel) null);
  }

  private void SetChannel(Channel channel)
  {
    if (channel == this.selectedChannel)
      return;
    if (channel == null)
    {
      this.ClearInputs();
      this.ClearKeys();
    }
    this.selectedChannel = channel;
    this.ClearInputs();
    this.ClearKeys();
    this.ReadLockConfiguration();
    if (this.unlockSupported && this.useManualUnlock)
      this.ReadInputs();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    this.closing = true;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetChannel((Channel) null);
  }

  private bool EnableButtons => this.unlockSupported && !this.busy && !this.closing;

  private bool UnlockInputsValid
  {
    get
    {
      return this.textBoxKey1.Text.Length == 3 && UserPanel.IsNumeric(this.textBoxKey1.Text) && this.textBoxKey2.Text.Length == 3 && UserPanel.IsNumeric(this.textBoxKey2.Text) && this.textBoxKey3.Text.Length == 3 && UserPanel.IsNumeric(this.textBoxKey3.Text) && this.textBoxKey4.Text.Length == 3 && UserPanel.IsNumeric(this.textBoxKey4.Text);
    }
  }

  private bool CanEnableXCPCCP => 3 <= ApplicationInformation.ProductAccessLevel;

  private static bool IsNumeric(string input)
  {
    double result = 0.0;
    return !string.IsNullOrEmpty(input) && double.TryParse(input, out result);
  }

  private void UpdateUserInterface()
  {
    if (!this.busy && !this.closing)
      this.buttonSwitchUnlockMode.Enabled = true;
    if (this.useManualUnlock)
    {
      this.buttonEnable.Enabled = this.EnableButtons && this.UnlockInputsValid && this.CanEnableXCPCCP;
      this.buttonRefresh.Enabled = this.EnableButtons && this.CanEnableXCPCCP;
      this.buttonSwitchUnlockMode.Text = Resources.Message_SwitchToServerUnlock;
      this.textBoxKey1.Enabled = this.EnableButtons && this.CanEnableXCPCCP;
      this.textBoxKey2.Enabled = this.EnableButtons && this.CanEnableXCPCCP;
      this.textBoxKey3.Enabled = this.EnableButtons && this.CanEnableXCPCCP;
      this.textBoxKey4.Enabled = this.EnableButtons && this.CanEnableXCPCCP;
      this.ShowManualInputs(true);
    }
    else
    {
      this.buttonEnable.Enabled = this.EnableButtons && this.CanEnableXCPCCP;
      this.buttonRefresh.Enabled = false;
      this.buttonSwitchUnlockMode.Text = Resources.Message_SwitchToManualUnlock;
      this.ShowManualInputs(false);
    }
    this.buttonDisable.Enabled = this.EnableButtons;
  }

  private void ClearInputs()
  {
    this.textBoxActivationMode.Text = string.Empty;
    this.textBoxEcuIdCode.Text = string.Empty;
    this.textBoxTransponderCode.Text = string.Empty;
    this.textBoxNumberOfCode.Text = string.Empty;
    this.textBoxChallengeCode.Text = string.Empty;
    this.textBoxXFactor.Text = string.Empty;
    this.textBoxChecksum.Text = string.Empty;
  }

  private void ClearKeys()
  {
    this.textBoxKey1.Text = string.Empty;
    this.textBoxKey2.Text = string.Empty;
    this.textBoxKey3.Text = string.Empty;
    this.textBoxKey4.Text = string.Empty;
  }

  private void ShowManualInputs(bool visible)
  {
    this.RandomNumberLabel.Visible = visible;
    this.IDCodeLabel.Visible = visible;
    this.XFactorValueLabel.Visible = visible;
    this.NumberOfKeysLabel.Visible = visible;
    this.TranspoderCodeLabel.Visible = visible;
    this.ChecksumLabel.Visible = visible;
    this.UnlockKeyLabel.Visible = visible;
    this.textBoxEcuIdCode.Visible = visible;
    this.textBoxTransponderCode.Visible = visible;
    this.textBoxNumberOfCode.Visible = visible;
    this.textBoxChallengeCode.Visible = visible;
    this.textBoxXFactor.Visible = visible;
    this.textBoxChecksum.Visible = visible;
    this.textBoxKey1.Visible = visible;
    this.textBoxKey2.Visible = visible;
    this.textBoxKey3.Visible = visible;
    this.textBoxKey4.Visible = visible;
  }

  private string EcuInfoCurrentValue(Channel channel, string qualifier, bool forceRead)
  {
    string str = string.Empty;
    EcuInfo ecuInfo = channel.EcuInfos[qualifier];
    if (ecuInfo != null)
    {
      if (forceRead)
        ecuInfo.Read(true);
      if (ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
      {
        Choice choice = ecuInfo.EcuInfoValues.Current.Value as Choice;
        str = !(choice != (object) null) ? ecuInfo.EcuInfoValues.Current.Value.ToString() : choice.ToString();
      }
    }
    return str;
  }

  private int EcuInfoRawValue(EcuInfo ecuInfo)
  {
    int? nullable = new int?();
    if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
    {
      Choice choice = ecuInfo.EcuInfoValues.Current.Value as Choice;
      nullable = !(choice != (object) null) ? ecuInfo.EcuInfoValues.Current.Value as int? : choice.RawValue as int?;
    }
    return !nullable.HasValue ? int.MinValue : nullable.Value;
  }

  private void ReadModeQualifier(Channel channel, string qualifier)
  {
    EcuInfo ecuInfo = channel.EcuInfos[qualifier];
    if (ecuInfo != null)
    {
      ecuInfo.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.OnModeQualifierReadUpdate);
      ecuInfo.Read(false);
    }
    else
      this.textBoxActivationMode.Text = Resources.Message_NotSupportedOrRequired;
  }

  protected void OnModeQualifierReadUpdate(object sender, ResultEventArgs e)
  {
    EcuInfo ecuInfo = sender as EcuInfo;
    ecuInfo.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.OnModeQualifierReadUpdate);
    if (this.EcuInfoRawValue(ecuInfo) != 2)
      this.textBoxActivationMode.Text = Resources.Message_NotSupportedOrRequired;
    this.UpdateUserInterface();
  }

  private void ReadStatusQualifier(Channel channel, string qualifier)
  {
    EcuInfo ecuInfo = channel.EcuInfos[qualifier];
    if (ecuInfo == null)
      return;
    ecuInfo.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.ecuInfo_EcuInfoUpdateEvent);
    ecuInfo.Read(false);
  }

  protected void ecuInfo_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
  {
    EcuInfo ecuInfo = sender as EcuInfo;
    ecuInfo.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.ecuInfo_EcuInfoUpdateEvent);
    if (this.EcuInfoRawValue(ecuInfo) == 1)
      this.textBoxActivationMode.Text = Resources.Message_XCPCCPActivated;
    else
      this.textBoxActivationMode.Text = Resources.Message_VeDocUnlockRequiredForActivation;
    this.UpdateUserInterface();
  }

  private string GetSelectedChannelQualifier(Dictionary<string, string> dictionary)
  {
    return this.selectedChannel != null && dictionary.ContainsKey(this.selectedChannel.Ecu.Name) ? dictionary[this.selectedChannel.Ecu.Name] : string.Empty;
  }

  private void ReadLockConfiguration()
  {
    this.unlockSupported = false;
    string channelQualifier1 = this.GetSelectedChannelQualifier(this.ReadActivationModeQualifier);
    string channelQualifier2 = this.GetSelectedChannelQualifier(this.ReadActivationStatusQualifier);
    if (!string.IsNullOrEmpty(channelQualifier1) && !string.IsNullOrEmpty(channelQualifier2))
    {
      this.ReadModeQualifier(this.selectedChannel, channelQualifier1);
      if (this.CanEnableXCPCCP)
      {
        this.unlockSupported = true;
        this.ReadStatusQualifier(this.selectedChannel, channelQualifier2);
      }
      else
      {
        this.textBoxActivationMode.Text = Resources.Message_VeDocUnlockRequiredForActivationButToolIsNotPermittedToPerformAction;
        this.unlockSupported = true;
      }
    }
    this.UpdateUserInterface();
  }

  private void ReadInputs()
  {
    string channelQualifier = this.GetSelectedChannelQualifier(this.ReadVeDocInputQualifier);
    if (!string.IsNullOrEmpty(channelQualifier) && this.CanEnableXCPCCP)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingUnlockInputDetails);
      this.readVeDocInputService = this.selectedChannel.Services[channelQualifier];
      if (this.readVeDocInputService != (Service) null)
      {
        this.busy = true;
        this.readVeDocInputService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.readVeDocInputService_ServiceCompleteEvent);
        this.readVeDocInputService.Execute(false);
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
    }
    this.UpdateUserInterface();
  }

  private void readVeDocInputService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.readVeDocInputService == sender as Service)
    {
      this.busy = false;
      this.readVeDocInputService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.readVeDocInputService_ServiceCompleteEvent);
      if (e.Succeeded && this.selectedChannel != null)
      {
        VedocReadInputDataQualifiers responseQualifier = this.VeDocInputResponseQualifiers[this.selectedChannel.Ecu.Name];
        if (this.readVeDocInputService.OutputValues[responseQualifier.ChallengeCodeQualifier] != null && this.readVeDocInputService.OutputValues[responseQualifier.ChallengeCodeQualifier].Value != null)
          this.textBoxChallengeCode.Text = this.readVeDocInputService.OutputValues[responseQualifier.ChallengeCodeQualifier].Value.ToString();
        else
          this.textBoxChallengeCode.Text = Resources.Message_NullValue;
        if (!this.OverrideIdCode())
        {
          if (this.readVeDocInputService.OutputValues[responseQualifier.IdCodeQualifier] != null && this.readVeDocInputService.OutputValues[responseQualifier.IdCodeQualifier].Value != null)
            this.textBoxEcuIdCode.Text = this.readVeDocInputService.OutputValues[responseQualifier.IdCodeQualifier].Value.ToString();
          else
            this.textBoxEcuIdCode.Text = Resources.Message_NullValue;
        }
        if (this.readVeDocInputService.OutputValues[responseQualifier.NumberOfCodesQualifier] != null && this.readVeDocInputService.OutputValues[responseQualifier.NumberOfCodesQualifier].Value != null)
          this.textBoxNumberOfCode.Text = this.readVeDocInputService.OutputValues[responseQualifier.NumberOfCodesQualifier].Value.ToString();
        else
          this.textBoxNumberOfCode.Text = Resources.Message_NullValue;
        if (this.readVeDocInputService.OutputValues[responseQualifier.TransponderCodeQualifier] != null && this.readVeDocInputService.OutputValues[responseQualifier.TransponderCodeQualifier].Value != null)
          this.textBoxTransponderCode.Text = this.readVeDocInputService.OutputValues[responseQualifier.TransponderCodeQualifier].Value.ToString();
        else
          this.textBoxTransponderCode.Text = Resources.Message_NullValue;
        this.readVeDocInputService = (Service) null;
        this.textBoxChecksum.Text = "XXX";
        this.textBoxXFactor.Text = "XE";
        this.textBoxKey1.Focus();
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockInputDetailsReadSuccessfully);
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailedToReadUnlockInputDetails);
    }
    this.UpdateUserInterface();
  }

  private bool OverrideIdCode()
  {
    string channelQualifier = this.GetSelectedChannelQualifier(this.ReadVeDocInputIdCodeOverrideQualifier);
    if (string.IsNullOrEmpty(channelQualifier))
      return false;
    this.textBoxEcuIdCode.Text = this.EcuInfoCurrentValue(this.selectedChannel, channelQualifier, false);
    return true;
  }

  private void DisableXCPCCP()
  {
    string channelQualifier = this.GetSelectedChannelQualifier(this.DisableXCPCCPQualifier);
    if (!string.IsNullOrEmpty(channelQualifier))
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_DisableXCPCCP);
      this.disableXCPCCPService = this.selectedChannel.Services[channelQualifier];
      if (this.disableXCPCCPService != (Service) null)
      {
        this.busy = true;
        this.disableXCPCCPService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.disableXCPCCPService_ServiceCompleteEvent);
        this.disableXCPCCPService.Execute(false);
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
    }
    this.UpdateUserInterface();
  }

  private void disableXCPCCPService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.disableXCPCCPService == sender as Service)
    {
      this.disableXCPCCPService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.disableXCPCCPService_ServiceCompleteEvent);
      this.disableXCPCCPService = (Service) null;
      if (e.Succeeded)
      {
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_XCPCCPHasBeenDisabled);
        this.CommitChanges();
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailedToDisableXCPCCP);
    }
    this.UpdateUserInterface();
  }

  private void UnlockVeDocToEnableXCPCCP()
  {
    string channelQualifier = this.GetSelectedChannelQualifier(this.UnlockAut64SecurityQualifier);
    if (!string.IsNullOrEmpty(channelQualifier))
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_AttemptingToUnlockController);
      this.unlockVeDocService = this.selectedChannel.Services[channelQualifier];
      if (this.unlockVeDocService != (Service) null)
      {
        this.busy = true;
        this.unlockingToEnableXCPCCP = true;
        this.unlockVeDocService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.unlockVeDocService_ServiceCompleteEvent);
        this.unlockVeDocService.InputValues[0].Value = (object) "XE";
        this.unlockVeDocService.InputValues[1].Value = (object) this.textBoxKey1.Text;
        this.unlockVeDocService.InputValues[2].Value = (object) this.textBoxKey2.Text;
        this.unlockVeDocService.InputValues[3].Value = (object) this.textBoxKey3.Text;
        this.unlockVeDocService.InputValues[4].Value = (object) this.textBoxKey4.Text;
        this.unlockVeDocService.Execute(false);
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
    }
    this.UpdateUserInterface();
  }

  private void unlockVeDocService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.unlockVeDocService == sender as Service)
    {
      this.busy = false;
      this.unlockVeDocService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.unlockVeDocService_ServiceCompleteEvent);
      if (e.Succeeded && this.unlockingToEnableXCPCCP && this.unlockVeDocService.OutputValues != null && this.unlockVeDocService.OutputValues[0].Value != null)
      {
        Choice choice = this.unlockVeDocService.OutputValues[0].Value as Choice;
        int num;
        if (choice != (object) null)
        {
          int? rawValue = choice.RawValue as int?;
          num = (rawValue.GetValueOrDefault() != 0 ? 0 : (rawValue.HasValue ? 1 : 0)) == 0 ? 1 : 0;
        }
        else
          num = 1;
        if (num == 0)
        {
          this.unlockVeDocService = (Service) null;
          this.unlockingToEnableXCPCCP = false;
          this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockSuccessful);
          this.EnableXCPCCP();
          return;
        }
      }
      this.unlockVeDocService = (Service) null;
    }
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_VeDocUnlockHasFailedCheckInputsAndTryAgain);
    this.UpdateUserInterface();
  }

  private void EnableXCPCCP()
  {
    string channelQualifier = this.GetSelectedChannelQualifier(this.EnableXCPCCPQualifier);
    if (!string.IsNullOrEmpty(channelQualifier) && this.CanEnableXCPCCP)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_EnablingXCPCCP);
      this.enableXCPCCPService = this.selectedChannel.Services[channelQualifier];
      if (this.enableXCPCCPService != (Service) null)
      {
        this.busy = true;
        this.enableXCPCCPService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.enableXCPCCPService_ServiceCompleteEvent);
        this.enableXCPCCPService.Execute(false);
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
    }
    this.UpdateUserInterface();
  }

  private void enableXCPCCPService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.enableXCPCCPService == sender as Service)
    {
      this.enableXCPCCPService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.enableXCPCCPService_ServiceCompleteEvent);
      this.enableXCPCCPService = (Service) null;
      if (e.Succeeded)
      {
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_XCPCCPHasBeenEnabled);
        this.CommitChanges();
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailedToEnableXCPCCP);
    }
    this.UpdateUserInterface();
  }

  private void CommitChanges()
  {
    this.commitServiceList = this.selectedChannel.Ecu.Properties["CommitToPermanentMemoryService"];
    if (!string.IsNullOrEmpty(this.commitServiceList))
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_CommitingChanges);
      this.busy = true;
      this.selectedChannel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.commitChangesService_ServiceCompleteEvent);
      this.selectedChannel.Services.Execute(this.commitServiceList, false);
    }
    else
      this.ResetEcu();
    this.UpdateUserInterface();
  }

  private void commitChangesService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.commitServiceList == sender as string)
    {
      this.selectedChannel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.commitChangesService_ServiceCompleteEvent);
      this.commitServiceList = string.Empty;
      if (e.Succeeded)
      {
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ChangesCommited);
        this.ResetEcu();
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_CommitServiceFailed);
    }
    this.UpdateUserInterface();
  }

  private void ResetEcu()
  {
    string channelQualifier = this.GetSelectedChannelQualifier(this.ResetEcuQualifier);
    if (!string.IsNullOrEmpty(channelQualifier) && this.CanEnableXCPCCP)
    {
      Application.DoEvents();
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_EcuIsReseting);
      this.resetEcuService = this.selectedChannel.Services[channelQualifier];
      if (this.resetEcuService != (Service) null)
      {
        this.busy = true;
        this.selectedChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.selectedChannel_CommunicationsStateUpdateEvent);
        this.resetEcuService.Execute(false);
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
    }
    this.UpdateUserInterface();
  }

  private void selectedChannel_CommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    if (e.CommunicationsState != CommunicationsState.Online && e.CommunicationsState != CommunicationsState.Offline || !(this.resetEcuService != (Service) null))
      return;
    this.selectedChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.selectedChannel_CommunicationsStateUpdateEvent);
    this.resetEcuService = (Service) null;
    this.busy = false;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_OperationCompleted);
    if (this.selectedChannel != null)
    {
      Application.DoEvents();
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_CycleTheIgnitionToCompleteTheProcess);
    }
  }

  private void PerformServerUnlockAndXcpEnable()
  {
    string str = $"SP_SecurityUnlock_{this.selectedChannel.Ecu.Name}_Unlock{"XE"}";
    SharedProcedureBase availableProcedure = SharedProcedureBase.AvailableProcedures[str];
    if (availableProcedure != null)
    {
      if (availableProcedure.CanStart)
      {
        availableProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
        availableProcedure.Start();
      }
      else
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", (object) availableProcedure.Name));
    }
    else
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", (object) str));
  }

  private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result == 1)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} unlock via the server was initiated using procedure {1}", (object) this.selectedChannel.Ecu.Name, (object) sharedProcedureBase.Name));
      sharedProcedureBase.StopComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    }
    else
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
  }

  private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    if (!((ResultEventArgs) e).Succeeded || e.Result == 0)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
    }
    else
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} was unlocked via the server using procedure {1}", (object) this.selectedChannel.Ecu.Name, (object) sharedProcedureBase.Name));
      this.EnableXCPCCP();
    }
  }

  private void buttonRefresh_Click(object sender, EventArgs e) => this.ReadInputs();

  private void buttonEnable_Click(object sender, EventArgs e)
  {
    if (this.useManualUnlock)
      this.UnlockVeDocToEnableXCPCCP();
    else
      this.PerformServerUnlockAndXcpEnable();
  }

  private void buttonDisable_Click(object sender, EventArgs e) => this.DisableXCPCCP();

  private void comboBoxEcuToUnlock_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.SetChannel((Channel) this.comboBoxEcuToUnlock.SelectedItem);
  }

  private void textBoxKey1_TextChanged(object sender, EventArgs e)
  {
    if (this.textBoxKey1.Text.Length == 3)
    {
      this.textBoxKey2.Focus();
      this.textBoxKey2.SelectAll();
    }
    this.UpdateUserInterface();
  }

  private void textBoxKey2_TextChanged(object sender, EventArgs e)
  {
    if (this.textBoxKey2.Text.Length == 3)
    {
      this.textBoxKey3.Focus();
      this.textBoxKey3.SelectAll();
    }
    this.UpdateUserInterface();
  }

  private void textBoxKey3_TextChanged(object sender, EventArgs e)
  {
    if (this.textBoxKey3.Text.Length == 3)
    {
      this.textBoxKey4.Focus();
      this.textBoxKey4.SelectAll();
    }
    this.UpdateUserInterface();
  }

  private void textBoxKey4_TextChanged(object sender, EventArgs e)
  {
    if (this.textBoxKey4.Text.Length == 3)
      this.buttonEnable.Focus();
    this.UpdateUserInterface();
  }

  private void buttonSwitchUnlockMode_Click(object sender, EventArgs e)
  {
    this.useManualUnlock = !this.useManualUnlock;
    this.ReadLockConfiguration();
    if (this.useManualUnlock)
    {
      this.UpdateUserInterface();
      this.ReadInputs();
    }
    else
      this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.textBoxChecksum = new TextBox();
    this.ChecksumLabel = new System.Windows.Forms.Label();
    this.label8 = new System.Windows.Forms.Label();
    this.label1 = new System.Windows.Forms.Label();
    this.UnlockKeyLabel = new System.Windows.Forms.Label();
    this.textBoxActivationMode = new TextBox();
    this.comboBoxEcuToUnlock = new ComboBox();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.textBoxKey1 = new TextBox();
    this.textBoxKey2 = new TextBox();
    this.textBoxKey3 = new TextBox();
    this.textBoxKey4 = new TextBox();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonSwitchUnlockMode = new Button();
    this.buttonRefresh = new Button();
    this.buttonEnable = new Button();
    this.buttonDisable = new Button();
    this.buttonClose = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.RandomNumberLabel = new System.Windows.Forms.Label();
    this.textBoxChallengeCode = new TextBox();
    this.IDCodeLabel = new System.Windows.Forms.Label();
    this.textBoxEcuIdCode = new TextBox();
    this.XFactorValueLabel = new System.Windows.Forms.Label();
    this.textBoxXFactor = new TextBox();
    this.NumberOfKeysLabel = new System.Windows.Forms.Label();
    this.TranspoderCodeLabel = new System.Windows.Forms.Label();
    this.textBoxNumberOfCode = new TextBox();
    this.textBoxTransponderCode = new TextBox();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    this.flowLayoutPanel1.SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxChecksum, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ChecksumLabel, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label8, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.UnlockKeyLabel, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxActivationMode, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.comboBoxEcuToUnlock, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.flowLayoutPanel1, 1, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RandomNumberLabel, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxChallengeCode, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IDCodeLabel, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxEcuIdCode, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.XFactorValueLabel, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxXFactor, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.NumberOfKeysLabel, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TranspoderCodeLabel, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxNumberOfCode, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxTransponderCode, 1, 6);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.textBoxChecksum, "textBoxChecksum");
    this.textBoxChecksum.Name = "textBoxChecksum";
    this.textBoxChecksum.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.ChecksumLabel, "ChecksumLabel");
    this.ChecksumLabel.Name = "ChecksumLabel";
    this.ChecksumLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    this.label8.Name = "label8";
    this.label8.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.UnlockKeyLabel, "UnlockKeyLabel");
    this.UnlockKeyLabel.Name = "UnlockKeyLabel";
    this.UnlockKeyLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxActivationMode, "textBoxActivationMode");
    this.textBoxActivationMode.Name = "textBoxActivationMode";
    this.textBoxActivationMode.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.comboBoxEcuToUnlock, "comboBoxEcuToUnlock");
    this.comboBoxEcuToUnlock.FormattingEnabled = true;
    this.comboBoxEcuToUnlock.Name = "comboBoxEcuToUnlock";
    this.comboBoxEcuToUnlock.SelectedIndexChanged += new EventHandler(this.comboBoxEcuToUnlock_SelectedIndexChanged);
    this.flowLayoutPanel1.Controls.Add((Control) this.textBoxKey1);
    this.flowLayoutPanel1.Controls.Add((Control) this.textBoxKey2);
    this.flowLayoutPanel1.Controls.Add((Control) this.textBoxKey3);
    this.flowLayoutPanel1.Controls.Add((Control) this.textBoxKey4);
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.textBoxKey1, "textBoxKey1");
    this.textBoxKey1.Name = "textBoxKey1";
    this.textBoxKey1.TextChanged += new EventHandler(this.textBoxKey1_TextChanged);
    componentResourceManager.ApplyResources((object) this.textBoxKey2, "textBoxKey2");
    this.textBoxKey2.Name = "textBoxKey2";
    this.textBoxKey2.TextChanged += new EventHandler(this.textBoxKey2_TextChanged);
    componentResourceManager.ApplyResources((object) this.textBoxKey3, "textBoxKey3");
    this.textBoxKey3.Name = "textBoxKey3";
    this.textBoxKey3.TextChanged += new EventHandler(this.textBoxKey3_TextChanged);
    componentResourceManager.ApplyResources((object) this.textBoxKey4, "textBoxKey4");
    this.textBoxKey4.Name = "textBoxKey4";
    this.textBoxKey4.TextChanged += new EventHandler(this.textBoxKey4_TextChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonSwitchUnlockMode, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonRefresh, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonEnable, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonDisable, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 4, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonSwitchUnlockMode, "buttonSwitchUnlockMode");
    this.buttonSwitchUnlockMode.Name = "buttonSwitchUnlockMode";
    this.buttonSwitchUnlockMode.UseCompatibleTextRendering = true;
    this.buttonSwitchUnlockMode.UseVisualStyleBackColor = true;
    this.buttonSwitchUnlockMode.Click += new EventHandler(this.buttonSwitchUnlockMode_Click);
    componentResourceManager.ApplyResources((object) this.buttonRefresh, "buttonRefresh");
    this.buttonRefresh.Name = "buttonRefresh";
    this.buttonRefresh.UseCompatibleTextRendering = true;
    this.buttonRefresh.UseVisualStyleBackColor = true;
    this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
    componentResourceManager.ApplyResources((object) this.buttonEnable, "buttonEnable");
    this.buttonEnable.Name = "buttonEnable";
    this.buttonEnable.UseCompatibleTextRendering = true;
    this.buttonEnable.UseVisualStyleBackColor = true;
    this.buttonEnable.Click += new EventHandler(this.buttonEnable_Click);
    componentResourceManager.ApplyResources((object) this.buttonDisable, "buttonDisable");
    this.buttonDisable.Name = "buttonDisable";
    this.buttonDisable.UseCompatibleTextRendering = true;
    this.buttonDisable.UseVisualStyleBackColor = true;
    this.buttonDisable.Click += new EventHandler(this.buttonDisable_Click);
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "XCPCCPACTPAN";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    componentResourceManager.ApplyResources((object) this.RandomNumberLabel, "RandomNumberLabel");
    this.RandomNumberLabel.Name = "RandomNumberLabel";
    this.RandomNumberLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxChallengeCode, "textBoxChallengeCode");
    this.textBoxChallengeCode.Name = "textBoxChallengeCode";
    this.textBoxChallengeCode.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.IDCodeLabel, "IDCodeLabel");
    this.IDCodeLabel.Name = "IDCodeLabel";
    this.IDCodeLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxEcuIdCode, "textBoxEcuIdCode");
    this.textBoxEcuIdCode.Name = "textBoxEcuIdCode";
    this.textBoxEcuIdCode.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.XFactorValueLabel, "XFactorValueLabel");
    this.XFactorValueLabel.Name = "XFactorValueLabel";
    this.XFactorValueLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxXFactor, "textBoxXFactor");
    this.textBoxXFactor.Name = "textBoxXFactor";
    this.textBoxXFactor.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.NumberOfKeysLabel, "NumberOfKeysLabel");
    this.NumberOfKeysLabel.Name = "NumberOfKeysLabel";
    this.NumberOfKeysLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.TranspoderCodeLabel, "TranspoderCodeLabel");
    this.TranspoderCodeLabel.Name = "TranspoderCodeLabel";
    this.TranspoderCodeLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxNumberOfCode, "textBoxNumberOfCode");
    this.textBoxNumberOfCode.Name = "textBoxNumberOfCode";
    this.textBoxNumberOfCode.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textBoxTransponderCode, "textBoxTransponderCode");
    this.textBoxTransponderCode.Name = "textBoxTransponderCode";
    this.textBoxTransponderCode.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
