// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_MR2_for_Reprogramming.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_MR2_for_Reprogramming.panel;

public class UserPanel : CustomPanel
{
  private const string getEcuSerialNumber = "DJ_Get_ECU_Serial_Number";
  private const string readAut64Challenge = "DT_STO_ID_AUT64_Challenge_challenge";
  private const string readTransponderCode = "DT_STO_ID_Transponder_Code_TranspCode";
  private const string readNumberOfTransponderCode = "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s";
  private const string readFuelmapStatus = "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status";
  private const string securityAccessRoutine = "DJ_SecurityAccess_Routine_1";
  private const string unlockMr2ForReprogrammingService = "RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State";
  private const string defaultMR2CalculationType = "X7 - Unlock flash memory";
  private const string unlockXOption = "No X-Option";
  private const string checksum = "XXX";
  private const string MR201T = "MR201T";
  private Channel selectedChannel;
  private UserPanel.State currentState;
  private TableLayoutPanel tableLayoutPanel1;
  private Button UnlockButton;
  private TextBox IdCode1TextBox;
  private TextBox IdCode2TextBox;
  private TextBox IdCode3TextBox;
  private TextBox IdCode4TextBox;
  private TextBox TspCode1TextBox;
  private TextBox TspCode2TextBox;
  private TextBox TspCode3TextBox;
  private TextBox TspCode4TextBox;
  private TextBox TspCode5TextBox;
  private TextBox NKeysTextBox;
  private TextBox RandomNumber1TextBox;
  private TextBox RandomNumber2TextBox;
  private ComboBox XFactorComboBox;
  private TextBox Aut64Key1TextBox;
  private TextBox Aut64Key2TextBox;
  private TextBox Aut64Key3TextBox;
  private TextBox Aut64Key4TextBox;
  private Button RefreshInputsButton;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label label2;
  private System.Windows.Forms.Label label3;
  private System.Windows.Forms.Label label4;
  private System.Windows.Forms.Label label5;
  private System.Windows.Forms.Label label6;
  private SeekTimeListView seekTimeListView1;
  private System.Windows.Forms.Label label8;
  private TextBox LockConfigurationTextBox;
  private System.Windows.Forms.Label label9;
  private TextBox ChecksumTextBox;
  private Button CloseButton;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    this.SetState(UserPanel.State.Initializing);
    this.SetChannel(this.GetChannel("MR201T", (CustomPanel.ChannelLookupOptions) 5));
    this.PopulateComboBoxes();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    if (this.currentState == UserPanel.State.Closing)
      return;
    this.SetChannel(this.GetChannel("MR201T", (CustomPanel.ChannelLookupOptions) 5));
  }

  private void SetChannel(Channel channel)
  {
    if (channel == this.selectedChannel)
      return;
    if (channel == null && this.currentState != UserPanel.State.Closing)
    {
      this.SetState(UserPanel.State.Unknown);
      this.ClearInputs();
      this.ClearKeys();
    }
    this.selectedChannel = channel;
    if (this.selectedChannel != null)
    {
      this.PopulateComboBoxes();
      this.ReadVeDocInputs();
    }
  }

  private void PopulateComboBoxes()
  {
    this.XFactorComboBox.Items.Clear();
    if (this.selectedChannel != null)
    {
      Service service = this.selectedChannel.Services["RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State"];
      if (!(service != (Service) null) || service.InputValues.Count <= 0)
        return;
      foreach (Choice choice in (ReadOnlyCollection<Choice>) service.InputValues[0].Choices)
        this.XFactorComboBox.Items.Add((object) choice.Name);
      this.XFactorComboBox.SelectedItem = (object) "X7 - Unlock flash memory";
    }
    else
      this.SetState(UserPanel.State.Unknown);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    switch (this.currentState)
    {
      case UserPanel.State.Initializing:
      case UserPanel.State.ReadingInputs:
      case UserPanel.State.Unlocking:
      case UserPanel.State.ReadingLockConfiguration:
        e.Cancel = true;
        break;
    }
    if (e.Cancel)
      return;
    this.SetState(UserPanel.State.Closing);
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetChannel((Channel) null);
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
    switch (this.currentState)
    {
      case UserPanel.State.Initializing:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InitializingWait);
        break;
      case UserPanel.State.ReadingInputs:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingInputsWait);
        break;
      case UserPanel.State.WaitingForKey:
        this.XFactorComboBox.Enabled = true;
        this.RefreshInputsButton.Enabled = true;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = true;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_PleaseTypeInVeDocSKey);
        break;
      case UserPanel.State.ReadyToUnlock:
        this.XFactorComboBox.Enabled = true;
        this.RefreshInputsButton.Enabled = true;
        this.UnlockButton.Enabled = true;
        this.CloseButton.Enabled = true;
        break;
      case UserPanel.State.Unlocking:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockingWait0);
        break;
      case UserPanel.State.ReadingLockConfiguration:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingLockConfigurationWait);
        break;
      case UserPanel.State.RefreshEcuData:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_RefreshingEcuData);
        break;
      case UserPanel.State.Done:
        this.XFactorComboBox.Enabled = true;
        this.RefreshInputsButton.Enabled = true;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = true;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Done);
        break;
      case UserPanel.State.Closing:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Closing);
        break;
      default:
        this.XFactorComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = true;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Unknown);
        break;
    }
  }

  private void Aut64KeyTextBox_KeyPress(object sender, KeyPressEventArgs e)
  {
    if (!char.IsLetter(e.KeyChar) && !char.IsSymbol(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsPunctuation(e.KeyChar))
      return;
    e.Handled = true;
  }

  private void Aut64Key1TextBox_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.Aut64Key1TextBox) || this.Aut64Key1TextBox.Text.Length != 3)
      return;
    this.Aut64Key2TextBox.Focus();
    this.Aut64Key2TextBox.SelectAll();
  }

  private void Aut64Key2TextBox_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.Aut64Key2TextBox) || this.Aut64Key2TextBox.Text.Length != 3)
      return;
    this.Aut64Key3TextBox.Focus();
    this.Aut64Key3TextBox.SelectAll();
  }

  private void Aut64Key3TextBox_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.Aut64Key3TextBox) || this.Aut64Key3TextBox.Text.Length != 3)
      return;
    this.Aut64Key4TextBox.Focus();
    this.Aut64Key4TextBox.SelectAll();
  }

  private void Aut64Key4TextBox_TextChanged(object sender, EventArgs e)
  {
    if (!this.checkAut64KeyTextBox(this.Aut64Key4TextBox) || this.Aut64Key4TextBox.Text.Length != 3)
      return;
    this.UnlockButton.Focus();
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
      this.SetState(UserPanel.State.ReadyToUnlock);
    else
      this.SetState(UserPanel.State.WaitingForKey);
    return true;
  }

  private bool checkAllKeys()
  {
    return this.checkKeyTextBox(this.Aut64Key1TextBox) && this.checkKeyTextBox(this.Aut64Key2TextBox) && this.checkKeyTextBox(this.Aut64Key3TextBox) && this.checkKeyTextBox(this.Aut64Key4TextBox);
  }

  private bool checkKeyTextBox(TextBox textBox)
  {
    if (textBox.Text.Length < 1)
      return false;
    if (textBox.Text.Length > 3)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range);
      textBox.SelectAll();
      return false;
    }
    bool flag = byte.TryParse(textBox.Text, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out byte _);
    if (!flag)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range);
      textBox.SelectAll();
    }
    return flag;
  }

  private void RefreshInputsButton_Click(object sender, EventArgs e) => this.ReadVeDocInputs();

  private void ReadVeDocInputs()
  {
    if (this.selectedChannel == null)
      return;
    this.selectedChannel.Instruments.AutoRead = false;
    this.SetState(UserPanel.State.ReadingInputs);
    Service service = this.selectedChannel.Services["DJ_Get_ECU_Serial_Number"];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.getEcuSerialNumberService_ServiceCompleteEvent);
      service.Execute(false);
    }
  }

  private void getEcuSerialNumberService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.getEcuSerialNumberService_ServiceCompleteEvent);
    this.ClearInputs();
    this.ClearKeys();
    if (e.Succeeded && service.OutputValues != null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string str = service.OutputValues[0].Value.ToString();
      if (str.Length >= 8)
      {
        this.IdCode1TextBox.Text = str.Substring(0, 2);
        this.IdCode2TextBox.Text = str.Substring(2, 2);
        this.IdCode3TextBox.Text = str.Substring(4, 2);
        this.IdCode4TextBox.Text = str.Substring(6, 2);
      }
      this.SetMarkedEcuInfoItems();
      this.selectedChannel.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.selectedChannel_EcuInfosReadCompleteEvent);
      this.selectedChannel.EcuInfos.Read(false);
    }
    else
    {
      this.ClearInputs();
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailureReadingVeDocInputs);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void selectedChannel_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.selectedChannel.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.selectedChannel_EcuInfosReadCompleteEvent);
    if (e.Succeeded)
    {
      string ecuInfoData1 = this.GetEcuInfoData(this.selectedChannel, "DT_STO_ID_AUT64_Challenge_challenge");
      string ecuInfoData2 = this.GetEcuInfoData(this.selectedChannel, "DT_STO_ID_Transponder_Code_TranspCode");
      string ecuInfoData3 = this.GetEcuInfoData(this.selectedChannel, "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s");
      if (ecuInfoData1.Length >= 4 && ecuInfoData2.Length >= 10)
      {
        this.RandomNumber1TextBox.Text = ecuInfoData1.Substring(0, 2);
        this.RandomNumber2TextBox.Text = ecuInfoData1.Substring(2, 2);
        this.NKeysTextBox.Text = ecuInfoData3;
        this.TspCode1TextBox.Text = ecuInfoData2.Substring(0, 2);
        this.TspCode2TextBox.Text = ecuInfoData2.Substring(2, 2);
        this.TspCode3TextBox.Text = ecuInfoData2.Substring(4, 2);
        this.TspCode4TextBox.Text = ecuInfoData2.Substring(6, 2);
        this.TspCode5TextBox.Text = ecuInfoData2.Substring(8, 2);
        this.ChecksumTextBox.Text = "XXX";
        this.ReadLockConfiguration();
        this.SetState(UserPanel.State.WaitingForKey);
        this.Aut64Key1TextBox.Focus();
        return;
      }
    }
    this.ClearInputs();
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailureReadingVeDocInputs);
    this.SetState(UserPanel.State.Done);
  }

  private void SetMarkedEcuInfoItems()
  {
    IEnumerable<string> source = (IEnumerable<string>) new string[4]
    {
      "DT_STO_ID_AUT64_Challenge_challenge",
      "DT_STO_ID_Transponder_Code_TranspCode",
      "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s",
      "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status"
    };
    foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) this.selectedChannel.EcuInfos)
      ecuInfo.Marked = source.Contains<string>(ecuInfo.Qualifier);
  }

  private void ReadLockConfiguration()
  {
    if (this.selectedChannel == null)
      return;
    this.SetState(UserPanel.State.ReadingLockConfiguration);
    this.LockConfigurationTextBox.Text = this.GetEcuInfoData(this.selectedChannel, "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status");
  }

  private string GetEcuInfoData(Channel channel, string qualifier)
  {
    string empty = string.Empty;
    if (channel != null)
    {
      EcuInfo ecuInfo = channel.EcuInfos[qualifier];
      if (ecuInfo != null)
        empty = ecuInfo.Value.ToString();
    }
    return empty.Trim();
  }

  private void ClearInputs()
  {
    this.IdCode1TextBox.Text = "";
    this.IdCode2TextBox.Text = "";
    this.IdCode3TextBox.Text = "";
    this.IdCode4TextBox.Text = "";
    this.TspCode1TextBox.Text = "";
    this.TspCode2TextBox.Text = "";
    this.TspCode3TextBox.Text = "";
    this.TspCode4TextBox.Text = "";
    this.TspCode5TextBox.Text = "";
    this.NKeysTextBox.Text = "";
    this.RandomNumber1TextBox.Text = "";
    this.RandomNumber2TextBox.Text = "";
    this.ChecksumTextBox.Text = "";
  }

  private void ClearKeys()
  {
    this.Aut64Key1TextBox.Text = "";
    this.Aut64Key2TextBox.Text = "";
    this.Aut64Key3TextBox.Text = "";
    this.Aut64Key4TextBox.Text = "";
  }

  private void UnlockButton_Click(object sender, EventArgs e)
  {
    Service service = this.selectedChannel.Services["DJ_SecurityAccess_Routine_1"];
    if (!(service != (Service) null))
      return;
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.securityAccessService_ServiceCompleteEvent);
    service.Execute(false);
  }

  private void securityAccessService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.securityAccessService_ServiceCompleteEvent);
    if (e.Succeeded && service != (Service) null)
    {
      this.UnlockMR2ForReprogramming();
    }
    else
    {
      this.LockConfigurationTextBox.Text = e.Exception.Message;
      this.ClearInputs();
      this.ClearKeys();
      this.SetState(UserPanel.State.Unknown);
    }
  }

  private void UnlockMR2ForReprogramming()
  {
    if (this.selectedChannel == null)
      return;
    this.SetState(UserPanel.State.Unlocking);
    Service service = this.selectedChannel.Services["RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State"];
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) this.XFactorComboBox.Text;
      service.InputValues[1].Value = (object) "No X-Option";
      service.InputValues[2].Value = (object) this.Aut64Key1TextBox.Text;
      service.InputValues[3].Value = (object) this.Aut64Key2TextBox.Text;
      service.InputValues[4].Value = (object) this.Aut64Key3TextBox.Text;
      service.InputValues[5].Value = (object) this.Aut64Key4TextBox.Text;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.unlockMr2Service_ServiceCompleteEvent);
      service.Execute(false);
    }
  }

  private void unlockMr2Service_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.unlockMr2Service_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.SetMarkedEcuInfoItems();
      this.selectedChannel.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.selectedChannelEcuInfos_EcuInfosReadCompleteEvent);
      this.selectedChannel.EcuInfos.Read(false);
      this.SetState(UserPanel.State.RefreshEcuData);
    }
    else
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorUnlocking + e.Exception.Message);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void selectedChannelEcuInfos_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.selectedChannel.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.selectedChannelEcuInfos_EcuInfosReadCompleteEvent);
    if (e.Succeeded)
    {
      this.ReadLockConfiguration();
      this.SetState(UserPanel.State.Done);
    }
    else
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorUnlocking + e.Exception.Message);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.XFactorComboBox = new ComboBox();
    this.seekTimeListView1 = new SeekTimeListView();
    this.label6 = new System.Windows.Forms.Label();
    this.Aut64Key1TextBox = new TextBox();
    this.Aut64Key2TextBox = new TextBox();
    this.Aut64Key4TextBox = new TextBox();
    this.label5 = new System.Windows.Forms.Label();
    this.label4 = new System.Windows.Forms.Label();
    this.RandomNumber1TextBox = new TextBox();
    this.RandomNumber2TextBox = new TextBox();
    this.NKeysTextBox = new TextBox();
    this.label3 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.TspCode1TextBox = new TextBox();
    this.TspCode2TextBox = new TextBox();
    this.TspCode3TextBox = new TextBox();
    this.TspCode4TextBox = new TextBox();
    this.TspCode5TextBox = new TextBox();
    this.label1 = new System.Windows.Forms.Label();
    this.IdCode1TextBox = new TextBox();
    this.IdCode2TextBox = new TextBox();
    this.IdCode3TextBox = new TextBox();
    this.IdCode4TextBox = new TextBox();
    this.label8 = new System.Windows.Forms.Label();
    this.LockConfigurationTextBox = new TextBox();
    this.CloseButton = new Button();
    this.label9 = new System.Windows.Forms.Label();
    this.ChecksumTextBox = new TextBox();
    this.Aut64Key3TextBox = new TextBox();
    this.UnlockButton = new Button();
    this.RefreshInputsButton = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.XFactorComboBox, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label6, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key1TextBox, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key2TextBox, 3, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key4TextBox, 5, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label5, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RandomNumber1TextBox, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RandomNumber2TextBox, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.NKeysTextBox, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label3, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode1TextBox, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode2TextBox, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode3TextBox, 4, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode4TextBox, 5, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode5TextBox, 6, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode1TextBox, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode2TextBox, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode3TextBox, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode4TextBox, 5, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label8, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.LockConfigurationTextBox, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CloseButton, 5, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label9, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ChecksumTextBox, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key3TextBox, 4, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.UnlockButton, 1, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RefreshInputsButton, 3, 9);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.XFactorComboBox, "XFactorComboBox");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.XFactorComboBox, 4);
    this.XFactorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
    this.XFactorComboBox.FormattingEnabled = true;
    this.XFactorComboBox.Name = "XFactorComboBox";
    this.XFactorComboBox.Tag = (object) "";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 7);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    ((Control) this.seekTimeListView1).TabStop = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label6, 2);
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.Aut64Key1TextBox, "Aut64Key1TextBox");
    this.Aut64Key1TextBox.Name = "Aut64Key1TextBox";
    this.Aut64Key1TextBox.TextChanged += new EventHandler(this.Aut64Key1TextBox_TextChanged);
    this.Aut64Key1TextBox.KeyPress += new KeyPressEventHandler(this.Aut64KeyTextBox_KeyPress);
    componentResourceManager.ApplyResources((object) this.Aut64Key2TextBox, "Aut64Key2TextBox");
    this.Aut64Key2TextBox.Name = "Aut64Key2TextBox";
    this.Aut64Key2TextBox.TextChanged += new EventHandler(this.Aut64Key2TextBox_TextChanged);
    this.Aut64Key2TextBox.KeyPress += new KeyPressEventHandler(this.Aut64KeyTextBox_KeyPress);
    componentResourceManager.ApplyResources((object) this.Aut64Key4TextBox, "Aut64Key4TextBox");
    this.Aut64Key4TextBox.Name = "Aut64Key4TextBox";
    this.Aut64Key4TextBox.TextChanged += new EventHandler(this.Aut64Key4TextBox_TextChanged);
    this.Aut64Key4TextBox.KeyPress += new KeyPressEventHandler(this.Aut64KeyTextBox_KeyPress);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label5, 2);
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label4, 2);
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    this.label4.Name = "label4";
    this.label4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.RandomNumber1TextBox, "RandomNumber1TextBox");
    this.RandomNumber1TextBox.Name = "RandomNumber1TextBox";
    this.RandomNumber1TextBox.ReadOnly = true;
    this.RandomNumber1TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.RandomNumber2TextBox, "RandomNumber2TextBox");
    this.RandomNumber2TextBox.Name = "RandomNumber2TextBox";
    this.RandomNumber2TextBox.ReadOnly = true;
    this.RandomNumber2TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.NKeysTextBox, "NKeysTextBox");
    this.NKeysTextBox.Name = "NKeysTextBox";
    this.NKeysTextBox.ReadOnly = true;
    this.NKeysTextBox.TabStop = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label3, 2);
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    this.label3.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label2, 2);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.TspCode1TextBox, "TspCode1TextBox");
    this.TspCode1TextBox.Name = "TspCode1TextBox";
    this.TspCode1TextBox.ReadOnly = true;
    this.TspCode1TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.TspCode2TextBox, "TspCode2TextBox");
    this.TspCode2TextBox.Name = "TspCode2TextBox";
    this.TspCode2TextBox.ReadOnly = true;
    this.TspCode2TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.TspCode3TextBox, "TspCode3TextBox");
    this.TspCode3TextBox.Name = "TspCode3TextBox";
    this.TspCode3TextBox.ReadOnly = true;
    this.TspCode3TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.TspCode4TextBox, "TspCode4TextBox");
    this.TspCode4TextBox.Name = "TspCode4TextBox";
    this.TspCode4TextBox.ReadOnly = true;
    this.TspCode4TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.TspCode5TextBox, "TspCode5TextBox");
    this.TspCode5TextBox.Name = "TspCode5TextBox";
    this.TspCode5TextBox.ReadOnly = true;
    this.TspCode5TextBox.TabStop = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label1, 2);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.IdCode1TextBox, "IdCode1TextBox");
    this.IdCode1TextBox.Name = "IdCode1TextBox";
    this.IdCode1TextBox.ReadOnly = true;
    this.IdCode1TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.IdCode2TextBox, "IdCode2TextBox");
    this.IdCode2TextBox.Name = "IdCode2TextBox";
    this.IdCode2TextBox.ReadOnly = true;
    this.IdCode2TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.IdCode3TextBox, "IdCode3TextBox");
    this.IdCode3TextBox.Name = "IdCode3TextBox";
    this.IdCode3TextBox.ReadOnly = true;
    this.IdCode3TextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.IdCode4TextBox, "IdCode4TextBox");
    this.IdCode4TextBox.Name = "IdCode4TextBox";
    this.IdCode4TextBox.ReadOnly = true;
    this.IdCode4TextBox.TabStop = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label8, 2);
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    this.label8.Name = "label8";
    this.label8.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.LockConfigurationTextBox, "LockConfigurationTextBox");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.LockConfigurationTextBox, 5);
    this.LockConfigurationTextBox.Name = "LockConfigurationTextBox";
    this.LockConfigurationTextBox.ReadOnly = true;
    this.LockConfigurationTextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.CloseButton, "CloseButton");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.CloseButton, 2);
    this.CloseButton.DialogResult = DialogResult.Cancel;
    this.CloseButton.Name = "CloseButton";
    this.CloseButton.UseCompatibleTextRendering = true;
    this.CloseButton.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label9, 2);
    componentResourceManager.ApplyResources((object) this.label9, "label9");
    this.label9.Name = "label9";
    this.label9.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.ChecksumTextBox, "ChecksumTextBox");
    this.ChecksumTextBox.Name = "ChecksumTextBox";
    this.ChecksumTextBox.ReadOnly = true;
    this.ChecksumTextBox.TabStop = false;
    componentResourceManager.ApplyResources((object) this.Aut64Key3TextBox, "Aut64Key3TextBox");
    this.Aut64Key3TextBox.Name = "Aut64Key3TextBox";
    this.Aut64Key3TextBox.TextChanged += new EventHandler(this.Aut64Key3TextBox_TextChanged);
    this.Aut64Key3TextBox.KeyPress += new KeyPressEventHandler(this.Aut64KeyTextBox_KeyPress);
    componentResourceManager.ApplyResources((object) this.UnlockButton, "UnlockButton");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.UnlockButton, 2);
    this.UnlockButton.Name = "UnlockButton";
    this.UnlockButton.UseCompatibleTextRendering = true;
    this.UnlockButton.UseVisualStyleBackColor = true;
    this.UnlockButton.Click += new EventHandler(this.UnlockButton_Click);
    componentResourceManager.ApplyResources((object) this.RefreshInputsButton, "RefreshInputsButton");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.RefreshInputsButton, 2);
    this.RefreshInputsButton.Name = "RefreshInputsButton";
    this.RefreshInputsButton.UseCompatibleTextRendering = true;
    this.RefreshInputsButton.UseVisualStyleBackColor = true;
    this.RefreshInputsButton.Click += new EventHandler(this.RefreshInputsButton_Click);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Unlock_ECU_for_Reprogramming");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    Unknown,
    Initializing,
    ReadingInputs,
    WaitingForKey,
    ReadyToUnlock,
    Unlocking,
    ReadingLockConfiguration,
    RefreshEcuData,
    Done,
    Closing,
  }
}
