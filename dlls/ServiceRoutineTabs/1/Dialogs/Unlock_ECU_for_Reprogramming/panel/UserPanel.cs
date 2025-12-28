// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_ECU_for_Reprogramming.panel.UserPanel
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
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_ECU_for_Reprogramming.panel;

public class UserPanel : CustomPanel
{
  private const string checksum = "XXX";
  private const string readLockConfigurationService = "DJ_Read_Z_Status_and_Fuelmap_Status";
  private const string readVeDocInputsService = "DJ_Read_AUT64_VeDoc_Input";
  private const string unlockEcuForReprogrammingService = "RT_SR089_X_Routine_improved_Start_Status";
  private const string defaultCalculationType = "X7";
  private Channel selectedChannel;
  private Service currentService;
  private List<string> ecusToUnlock = new List<string>()
  {
    "ACM21T",
    "ACM301T",
    "MCM21T"
  };
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
  private ComboBox CalculationTypeComboBox;
  private TextBox Aut64Key1TextBox;
  private TextBox Aut64Key2TextBox;
  private TextBox Aut64Key3TextBox;
  private TextBox Aut64Key4TextBox;
  private Button RefreshInputsButton;
  private ComboBox EcuComboBox;
  private System.Windows.Forms.Label label7;
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
    this.UpdateChannels();
    this.PopulateComboBoxes();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    if (this.currentState == UserPanel.State.Closing)
      return;
    this.UpdateChannels();
  }

  private void UpdateChannels()
  {
    this.EcuComboBox.Items.Clear();
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      if (this.ecusToUnlock.Contains(channel.ToString()) && channel.Online)
      {
        this.EcuComboBox.Items.Add((object) channel);
        if (channel == this.selectedChannel)
          this.EcuComboBox.SelectedItem = (object) channel;
      }
    }
    if (this.EcuComboBox.Items.Count > 0)
    {
      if (this.EcuComboBox.SelectedItem == null)
        this.EcuComboBox.SelectedItem = this.EcuComboBox.Items[0];
      this.SetChannel((Channel) this.EcuComboBox.SelectedItem);
    }
    else
      this.SetChannel((Channel) null);
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
      this.ReadLockConfiguration();
  }

  private void PopulateComboBoxes()
  {
    this.CalculationTypeComboBox.Items.Clear();
    if (this.selectedChannel != null)
    {
      Service service = this.selectedChannel.Services["RT_SR089_X_Routine_improved_Start_Status"];
      if (!(service != (Service) null) || service.InputValues.Count <= 0)
        return;
      foreach (Choice choice in (ReadOnlyCollection<Choice>) service.InputValues[0].Choices)
        this.CalculationTypeComboBox.Items.Add((object) choice.Name);
      this.CalculationTypeComboBox.SelectedText = "X7";
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
        this.EcuComboBox.Enabled = false;
        this.CalculationTypeComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InitializingWait);
        break;
      case UserPanel.State.ReadingInputs:
        this.EcuComboBox.Enabled = false;
        this.CalculationTypeComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingInputsWait);
        break;
      case UserPanel.State.WaitingForKey:
        this.EcuComboBox.Enabled = true;
        this.CalculationTypeComboBox.Enabled = true;
        this.RefreshInputsButton.Enabled = true;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = true;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_PleaseTypeInVeDocSKey);
        break;
      case UserPanel.State.ReadyToUnlock:
        this.EcuComboBox.Enabled = true;
        this.CalculationTypeComboBox.Enabled = true;
        this.RefreshInputsButton.Enabled = true;
        this.UnlockButton.Enabled = true;
        this.CloseButton.Enabled = true;
        break;
      case UserPanel.State.Unlocking:
        this.EcuComboBox.Enabled = false;
        this.CalculationTypeComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockingWait0);
        break;
      case UserPanel.State.ReadingLockConfiguration:
        this.EcuComboBox.Enabled = false;
        this.CalculationTypeComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingLockConfigurationWait);
        break;
      case UserPanel.State.Done:
        this.EcuComboBox.Enabled = true;
        this.CalculationTypeComboBox.Enabled = true;
        this.RefreshInputsButton.Enabled = true;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = true;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Done);
        break;
      case UserPanel.State.Closing:
        this.EcuComboBox.Enabled = false;
        this.CalculationTypeComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = false;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Closing);
        break;
      default:
        this.EcuComboBox.Enabled = false;
        this.CalculationTypeComboBox.Enabled = false;
        this.RefreshInputsButton.Enabled = false;
        this.UnlockButton.Enabled = false;
        this.CloseButton.Enabled = true;
        this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Unknown);
        break;
    }
  }

  private void EcuComboBox_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.SetChannel((Channel) this.EcuComboBox.SelectedItem);
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
    this.currentService = this.selectedChannel.Services["DJ_Read_AUT64_VeDoc_Input"];
    if (this.currentService != (Service) null)
    {
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnReadVeDocInputsComplete);
      this.currentService.Execute(false);
    }
  }

  private void OnReadVeDocInputsComplete(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnReadVeDocInputsComplete);
    this.ClearInputs();
    this.ClearKeys();
    if (e.Succeeded && this.currentService.OutputValues != null && this.currentService.OutputValues.Count > 3)
    {
      string str1 = this.currentService.OutputValues[0].Value.ToString();
      string str2 = this.currentService.OutputValues[1].Value.ToString();
      string str3 = this.currentService.OutputValues[2].Value.ToString();
      string str4 = this.currentService.OutputValues[3].Value.ToString();
      if (str1.Length >= 4 && str3.Length >= 10 && str4.Length >= 8)
      {
        this.RandomNumber1TextBox.Text = str1.Substring(0, 2);
        this.RandomNumber2TextBox.Text = str1.Substring(2, 2);
        this.NKeysTextBox.Text = str2;
        this.TspCode1TextBox.Text = str3.Substring(0, 2);
        this.TspCode2TextBox.Text = str3.Substring(2, 2);
        this.TspCode3TextBox.Text = str3.Substring(4, 2);
        this.TspCode4TextBox.Text = str3.Substring(6, 2);
        this.TspCode5TextBox.Text = str3.Substring(8, 2);
        this.IdCode1TextBox.Text = str4.Substring(0, 2);
        this.IdCode2TextBox.Text = str4.Substring(2, 2);
        this.IdCode3TextBox.Text = str4.Substring(4, 2);
        this.IdCode4TextBox.Text = str4.Substring(6, 2);
        this.ChecksumTextBox.Text = "XXX";
        this.SetState(UserPanel.State.WaitingForKey);
        this.Aut64Key1TextBox.Focus();
        return;
      }
    }
    this.ClearInputs();
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailureReadingVeDocInputs);
    this.SetState(UserPanel.State.Done);
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

  private void UnlockButton_Click(object sender, EventArgs e) => this.UnlockEcuForReprogramming();

  private void UnlockEcuForReprogramming()
  {
    if (this.selectedChannel == null)
      return;
    this.SetState(UserPanel.State.Unlocking);
    this.currentService = this.selectedChannel.Services["RT_SR089_X_Routine_improved_Start_Status"];
    if (this.currentService != (Service) null)
    {
      this.currentService.InputValues[0].Value = (object) this.CalculationTypeComboBox.Text;
      this.currentService.InputValues[1].Value = (object) this.Aut64Key1TextBox.Text;
      this.currentService.InputValues[2].Value = (object) this.Aut64Key2TextBox.Text;
      this.currentService.InputValues[3].Value = (object) this.Aut64Key3TextBox.Text;
      this.currentService.InputValues[4].Value = (object) this.Aut64Key4TextBox.Text;
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnUnlockComplete);
      this.currentService.Execute(false);
    }
  }

  private void OnUnlockComplete(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnUnlockComplete);
    if (e.Succeeded)
    {
      this.SetState(UserPanel.State.Done);
    }
    else
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorUnlocking + e.Exception.Message);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void ReadLockConfiguration()
  {
    if (this.selectedChannel == null)
      return;
    this.SetState(UserPanel.State.ReadingLockConfiguration);
    this.LockConfigurationTextBox.Text = "";
    this.currentService = this.selectedChannel.Services["DJ_Read_Z_Status_and_Fuelmap_Status"];
    if (this.currentService != (Service) null)
    {
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnReadLockConfigurationComplete);
      this.currentService.Execute(false);
    }
  }

  private void OnReadLockConfigurationComplete(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnReadLockConfigurationComplete);
    string empty = string.Empty;
    if (e.Succeeded && this.currentService != (Service) null && this.currentService.OutputValues != null && this.currentService.OutputValues.Count > 0)
    {
      this.LockConfigurationTextBox.Text = this.currentService.OutputValues[0].Value.ToString();
      this.ReadVeDocInputs();
    }
    else
    {
      this.LockConfigurationTextBox.Text = e.Exception.Message;
      this.ClearInputs();
      this.ClearKeys();
      this.SetState(UserPanel.State.Unknown);
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.CalculationTypeComboBox = new ComboBox();
    this.EcuComboBox = new ComboBox();
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
    this.label7 = new System.Windows.Forms.Label();
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
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CalculationTypeComboBox, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.EcuComboBox, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label6, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key1TextBox, 2, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key2TextBox, 3, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key4TextBox, 5, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label5, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RandomNumber1TextBox, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RandomNumber2TextBox, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.NKeysTextBox, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label3, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode1TextBox, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode2TextBox, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode3TextBox, 4, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode4TextBox, 5, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.TspCode5TextBox, 6, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode1TextBox, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode2TextBox, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode3TextBox, 4, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.IdCode4TextBox, 5, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label7, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label8, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.LockConfigurationTextBox, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CloseButton, 5, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label9, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ChecksumTextBox, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.Aut64Key3TextBox, 4, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.UnlockButton, 1, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.RefreshInputsButton, 3, 10);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.CalculationTypeComboBox, "CalculationTypeComboBox");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.CalculationTypeComboBox, 2);
    this.CalculationTypeComboBox.FormattingEnabled = true;
    this.CalculationTypeComboBox.Name = "CalculationTypeComboBox";
    this.CalculationTypeComboBox.Tag = (object) "";
    componentResourceManager.ApplyResources((object) this.EcuComboBox, "EcuComboBox");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.EcuComboBox, 2);
    this.EcuComboBox.FormattingEnabled = true;
    this.EcuComboBox.Name = "EcuComboBox";
    this.EcuComboBox.SelectedIndexChanged += new EventHandler(this.EcuComboBox_SelectedIndexChanged);
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
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label7, 2);
    componentResourceManager.ApplyResources((object) this.label7, "label7");
    this.label7.Name = "label7";
    this.label7.UseCompatibleTextRendering = true;
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
    Done,
    Closing,
  }
}
