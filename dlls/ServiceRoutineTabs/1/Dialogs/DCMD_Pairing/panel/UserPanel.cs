// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel;

public class UserPanel : CustomPanel
{
  private const string EmptyKeyfob = "00000000";
  private const string RemoteScServiceName = "DL_ID_Remote_SC";
  private const string RemoteScInputParameterRscByteName = "RSC_Byte_";
  private const string KeyfobIDsServiceName = "DL_ID_Keyfob_IDs";
  private const string HardResetServiceName = "FN_HardReset_physical";
  private UserPanel.ProcessState state = UserPanel.ProcessState.NotRunning;
  private Channel dcmd02t;
  private string[] KeyfobIDEcuInfoNames = new string[5]
  {
    "DT_STO_ID_Keyfob_IDs_KeyID_1",
    "DT_STO_ID_Keyfob_IDs_KeyID_2",
    "DT_STO_ID_Keyfob_IDs_KeyID_3",
    "DT_STO_ID_Keyfob_IDs_KeyID_4",
    "DT_STO_ID_Keyfob_IDs_KeyID_5"
  };
  private List<TextBox> keyfobNumberInputs = new List<TextBox>();
  private bool setFailed = false;
  private bool setSuccess = false;
  private CommunicationsState previousCommunicationsState = CommunicationsState.Offline;
  private string[] remoteScInputValues = new string[16 /*0x10*/]
  {
    "44",
    "54",
    "4E",
    "41",
    "4B",
    "45",
    "59",
    "46",
    "4F",
    "42",
    "53",
    "43",
    "44",
    "41",
    "54",
    "41"
  };
  private TableLayoutPanel tableLayoutPanelMain;
  private TextBox textBoxKeyfob1;
  private System.Windows.Forms.Label labelCurrentPairedFob;
  private TextBox textBoxKeyfob2;
  private TextBox textBoxKeyfob3;
  private TextBox textBoxKeyfob4;
  private TextBox textBoxKeyfob5;
  private System.Windows.Forms.Label labelFobNumber;
  private System.Windows.Forms.Label labelFobNumber1;
  private System.Windows.Forms.Label labelFobNumber2;
  private System.Windows.Forms.Label labelFobNumber3;
  private System.Windows.Forms.Label labelFobNumber4;
  private System.Windows.Forms.Label labelFobNumber5;
  private Button buttonReadValues;
  private Button buttonClear1;
  private Button buttonClear2;
  private Button buttonClear3;
  private Button buttonClear4;
  private Button buttonClear5;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonWriteValues;
  private Checkmark checkmarkCanStart;
  private System.Windows.Forms.Label labelSetMessage;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutInstrumentRKEUnlock;
  private DigitalReadoutInstrument digitalReadoutInstrumentRKEButton3;
  private DigitalReadoutInstrument digitalReadoutInstrumentRKELock;
  private Button buttonClose;

  private bool ProcedureCanStart
  {
    get
    {
      return this.dcmd02t != null && this.dcmd02t.CommunicationsState == CommunicationsState.Online && this.state == UserPanel.ProcessState.NotRunning && this.dcmd02t.Services["DL_ID_Keyfob_IDs"] != (Service) null;
    }
  }

  private bool ProcedureIsRunningOrHasRun
  {
    get => this.state != UserPanel.ProcessState.NotRunning || this.setFailed || this.setSuccess;
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.keyfobNumberInputs.Add(this.textBoxKeyfob1);
    this.keyfobNumberInputs.Add(this.textBoxKeyfob2);
    this.keyfobNumberInputs.Add(this.textBoxKeyfob3);
    this.keyfobNumberInputs.Add(this.textBoxKeyfob4);
    this.keyfobNumberInputs.Add(this.textBoxKeyfob5);
  }

  private void LogMessage(string message)
  {
    if (!(this.labelSetMessage.Text != message))
      return;
    this.labelSetMessage.Text = message;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, message);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.dcmd02t != null && this.state != UserPanel.ProcessState.NotRunning;
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannel(this.GetChannel("DCMD02T", (CustomPanel.ChannelLookupOptions) 1));
  }

  private void SetChannel(Channel dcmd02t)
  {
    if (this.dcmd02t != dcmd02t)
    {
      this.previousCommunicationsState = CommunicationsState.Offline;
      if (this.dcmd02t != null)
        this.dcmd02t.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.dcmd02t = dcmd02t;
      if (this.dcmd02t != null)
      {
        this.dcmd02t.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.previousCommunicationsState = this.dcmd02t.CommunicationsState;
      }
      this.state = UserPanel.ProcessState.NotRunning;
      this.PopoulateDefaultKeyfobs();
      this.setFailed = this.setSuccess = false;
    }
    this.UpdateUI();
  }

  private void PopoulateDefaultKeyfobs()
  {
    for (int index = 0; index < this.keyfobNumberInputs.Count; ++index)
      this.keyfobNumberInputs[index].Text = this.GetEcuInofKeyfobValue(index);
  }

  private string GetEcuInofKeyfobValue(int index)
  {
    return this.dcmd02t == null || this.dcmd02t.EcuInfos[this.KeyfobIDEcuInfoNames[index]] == null ? string.Empty : this.dcmd02t.EcuInfos[this.KeyfobIDEcuInfoNames[index]].Value;
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (this.previousCommunicationsState != CommunicationsState.ExecuteService && this.previousCommunicationsState != CommunicationsState.Online)
      this.PopoulateDefaultKeyfobs();
    this.previousCommunicationsState = this.dcmd02t != null ? this.dcmd02t.CommunicationsState : CommunicationsState.Offline;
    this.UpdateUI();
  }

  private UserPanel.KeyfobInputBoxesState UpdateInputBoxes()
  {
    UserPanel.KeyfobInputBoxesState keyfobInputBoxesState = UserPanel.KeyfobInputBoxesState.NoChange;
    List<string> stringList = new List<string>();
    for (int index = 0; index < this.keyfobNumberInputs.Count; ++index)
    {
      int result = 0;
      if (this.keyfobNumberInputs[index].Text.Length != 8 || !int.TryParse(this.keyfobNumberInputs[index].Text, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        this.keyfobNumberInputs[index].BackColor = Color.LightPink;
        keyfobInputBoxesState = UserPanel.KeyfobInputBoxesState.InvalidKeyfob;
      }
      else if (!this.keyfobNumberInputs[index].Text.Equals("00000000", StringComparison.InvariantCultureIgnoreCase) && stringList.Contains(this.keyfobNumberInputs[index].Text))
      {
        this.keyfobNumberInputs[index].BackColor = Color.LightPink;
        keyfobInputBoxesState = UserPanel.KeyfobInputBoxesState.DuplicateKeyfob;
      }
      else if (this.keyfobNumberInputs[index].Text == this.GetEcuInofKeyfobValue(index))
      {
        this.keyfobNumberInputs[index].BackColor = SystemColors.Window;
      }
      else
      {
        this.keyfobNumberInputs[index].BackColor = Color.PaleGreen;
        if (keyfobInputBoxesState == UserPanel.KeyfobInputBoxesState.NoChange)
          keyfobInputBoxesState = UserPanel.KeyfobInputBoxesState.ChangesOk;
      }
      stringList.Add(this.keyfobNumberInputs[index].Text);
    }
    return keyfobInputBoxesState;
  }

  private void UpdateUI()
  {
    UserPanel.KeyfobInputBoxesState keyfobInputBoxesState = this.UpdateInputBoxes();
    this.buttonWriteValues.Enabled = this.ProcedureCanStart && keyfobInputBoxesState == UserPanel.KeyfobInputBoxesState.ChangesOk;
    if (!this.ProcedureIsRunningOrHasRun && keyfobInputBoxesState == UserPanel.KeyfobInputBoxesState.ChangesOk || this.dcmd02t == null)
    {
      this.LogMessage(this.buttonWriteValues.Enabled ? Resources.Message_TheProcedureCanStart : Resources.Message_PreconditionsNotMet);
      this.checkmarkCanStart.CheckState = this.buttonWriteValues.Enabled ? CheckState.Checked : CheckState.Unchecked;
    }
    else if (!this.ProcedureIsRunningOrHasRun && keyfobInputBoxesState == UserPanel.KeyfobInputBoxesState.DuplicateKeyfob)
    {
      this.labelSetMessage.Text = Resources.Message_DuplicateKeyfob;
      this.checkmarkCanStart.CheckState = CheckState.Unchecked;
    }
    else if (!this.ProcedureIsRunningOrHasRun && keyfobInputBoxesState == UserPanel.KeyfobInputBoxesState.InvalidKeyfob)
    {
      this.labelSetMessage.Text = Resources.Message_InvalidKeyfob;
      this.checkmarkCanStart.CheckState = CheckState.Unchecked;
    }
    else if (!this.ProcedureIsRunningOrHasRun && keyfobInputBoxesState == UserPanel.KeyfobInputBoxesState.NoChange)
    {
      this.labelSetMessage.Text = Resources.Message_NoKeyfobsChanged;
      this.checkmarkCanStart.CheckState = CheckState.Unchecked;
    }
    else if (this.state == UserPanel.ProcessState.NotRunning)
      this.checkmarkCanStart.CheckState = this.setFailed ? CheckState.Unchecked : CheckState.Checked;
    this.buttonClose.Enabled = this.dcmd02t == null || this.state == UserPanel.ProcessState.NotRunning;
    this.buttonReadValues.Enabled = this.ProcedureCanStart;
    this.textBoxKeyfob1.Enabled = this.textBoxKeyfob2.Enabled = this.textBoxKeyfob3.Enabled = this.textBoxKeyfob4.Enabled = this.textBoxKeyfob5.Enabled = this.ProcedureCanStart;
    this.buttonClear1.Enabled = this.buttonClear2.Enabled = this.buttonClear3.Enabled = this.buttonClear4.Enabled = this.buttonClear5.Enabled = this.ProcedureCanStart;
  }

  private void buttonWriteValues_Click(object sender, EventArgs e)
  {
    this.state = UserPanel.ProcessState.RemoteSC;
    this.setSuccess = false;
    this.setFailed = false;
    this.GoMachine();
  }

  private void GoMachine()
  {
    switch (this.state)
    {
      case UserPanel.ProcessState.RemoteSC:
        this.SetRemoteSc();
        break;
      case UserPanel.ProcessState.SendKeyfobIDs:
        this.SendKeyfobIDs();
        break;
      case UserPanel.ProcessState.HardReset:
        this.ResetUnit();
        break;
      case UserPanel.ProcessState.ReloadEcuinfo:
        if (this.dcmd02t != null)
        {
          this.dcmd02t.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.EcuInfos_EcuInfosReadCompleteEvent);
          this.dcmd02t.EcuInfos.Read(false);
          break;
        }
        this.LogMessage(Resources.Message_CannotReloadEcuInfo);
        this.setFailed = true;
        this.state = UserPanel.ProcessState.NotRunning;
        break;
    }
  }

  private void SetRemoteSc()
  {
    Service service;
    if (this.dcmd02t == null || (service = this.dcmd02t.Services["DL_ID_Remote_SC"]) == (Service) null)
    {
      this.state = UserPanel.ProcessState.NotRunning;
      this.setFailed = true;
      this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CanNotStart0, (object) "DL_ID_Remote_SC"));
    }
    else
    {
      for (int index = 0; index < this.remoteScInputValues.Length; ++index)
        service.InputValues[string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) "RSC_Byte_", (object) index)].Value = (object) this.remoteScInputValues[index];
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.setRemoteSc_ServiceCompleteEvent);
      this.LogMessage(Resources.Message_WritingValues);
      service.Execute(false);
    }
    this.UpdateUI();
  }

  private void setRemoteSc_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    this.UpdateUI();
    this.state = UserPanel.ProcessState.NotRunning;
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.setRemoteSc_ServiceCompleteEvent);
      if (e.Succeeded)
      {
        this.state = UserPanel.ProcessState.SendKeyfobIDs;
        this.GoMachine();
        return;
      }
    }
    this.setFailed = true;
    this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0Failed, (object) "DL_ID_Remote_SC"));
  }

  private void SendKeyfobIDs()
  {
    Service service;
    if (this.dcmd02t != null && (service = this.dcmd02t.Services["DL_ID_Keyfob_IDs"]) != (Service) null)
    {
      for (int index = 0; index < this.keyfobNumberInputs.Count; ++index)
        service.InputValues[index].Value = (object) this.keyfobNumberInputs[index].Text;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.sendKeyfobIDs_ServiceCompleteEvent);
      service.Execute(false);
    }
    else
    {
      this.state = UserPanel.ProcessState.NotRunning;
      this.setFailed = true;
      this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CanNotStart0, (object) "DL_ID_Keyfob_IDs"));
      this.UpdateUI();
    }
  }

  private void sendKeyfobIDs_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    this.UpdateUI();
    this.state = UserPanel.ProcessState.NotRunning;
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.sendKeyfobIDs_ServiceCompleteEvent);
      if (e.Succeeded)
      {
        this.state = UserPanel.ProcessState.HardReset;
        this.LogMessage(Resources.Message_ResettingUnit);
        this.GoMachine();
        return;
      }
    }
    this.setFailed = true;
    this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0Failed, (object) "DL_ID_Keyfob_IDs"));
  }

  private void ResetUnit()
  {
    Service service;
    if (this.dcmd02t != null && (service = this.dcmd02t.Services["FN_HardReset_physical"]) != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.hardReset_ServiceCompleteEvent);
      service.Execute(false);
    }
    else
    {
      this.state = UserPanel.ProcessState.NotRunning;
      this.setFailed = true;
      this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CanNotStart0, (object) "FN_HardReset_physical"));
      this.UpdateUI();
    }
  }

  private void hardReset_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    this.UpdateUI();
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.hardReset_ServiceCompleteEvent);
      if (this.dcmd02t != null)
      {
        this.LogMessage(Resources.Message_ReadingValues);
        this.state = UserPanel.ProcessState.ReloadEcuinfo;
        this.GoMachine();
        return;
      }
    }
    this.setFailed = true;
    this.state = UserPanel.ProcessState.NotRunning;
    this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0Failed, (object) "FN_HardReset_physical"));
  }

  private void EcuInfos_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.dcmd02t.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.EcuInfos_EcuInfosReadCompleteEvent);
    this.state = UserPanel.ProcessState.NotRunning;
    this.PopoulateDefaultKeyfobs();
    this.setFailed = false;
    this.setSuccess = true;
    this.LogMessage(Resources.Message_ValuesWritten);
    this.UpdateUI();
  }

  private void buttonReadValues_Click(object sender, EventArgs e)
  {
    this.PopoulateDefaultKeyfobs();
    this.LogMessage(Resources.Message_ValuesRead);
  }

  private void textBoxKeyfob_KeyPress(object sender, KeyPressEventArgs e)
  {
    e.KeyChar = char.ToUpperInvariant(e.KeyChar);
    if (!char.IsControl(e.KeyChar) && !Regex.IsMatch(e.KeyChar.ToString(), "[0-9A-F]"))
      e.Handled = true;
    this.setSuccess = this.setFailed = false;
  }

  private void buttonClear_Click(object sender, EventArgs e)
  {
    if (sender is Button button)
    {
      this.keyfobNumberInputs[Convert.ToInt32(button.Tag)].Text = "00000000";
      this.setSuccess = this.setFailed = false;
    }
    this.UpdateUI();
  }

  private void textBoxKeyfob_TextChanged(object sender, EventArgs e) => this.UpdateUI();

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.labelFobNumber = new System.Windows.Forms.Label();
    this.textBoxKeyfob1 = new TextBox();
    this.labelCurrentPairedFob = new System.Windows.Forms.Label();
    this.textBoxKeyfob2 = new TextBox();
    this.textBoxKeyfob3 = new TextBox();
    this.textBoxKeyfob4 = new TextBox();
    this.textBoxKeyfob5 = new TextBox();
    this.labelFobNumber1 = new System.Windows.Forms.Label();
    this.labelFobNumber2 = new System.Windows.Forms.Label();
    this.labelFobNumber3 = new System.Windows.Forms.Label();
    this.labelFobNumber4 = new System.Windows.Forms.Label();
    this.labelFobNumber5 = new System.Windows.Forms.Label();
    this.buttonClear1 = new Button();
    this.buttonClear2 = new Button();
    this.buttonClear3 = new Button();
    this.buttonClear4 = new Button();
    this.buttonClear5 = new Button();
    this.buttonReadValues = new Button();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.buttonWriteValues = new Button();
    this.checkmarkCanStart = new Checkmark();
    this.labelSetMessage = new System.Windows.Forms.Label();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonClose = new Button();
    this.digitalReadoutInstrumentRKEUnlock = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentRKEButton3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentRKELock = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelFobNumber, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxKeyfob1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelCurrentPairedFob, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxKeyfob2, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxKeyfob3, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxKeyfob4, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxKeyfob5, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelFobNumber1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelFobNumber2, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelFobNumber3, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelFobNumber4, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelFobNumber5, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClear1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClear2, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClear3, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClear4, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClear5, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonReadValues, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanel1, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListView, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 3, 8);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentRKEUnlock, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentRKEButton3, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentRKELock, 3, 4);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.labelFobNumber, "labelFobNumber");
    this.labelFobNumber.Name = "labelFobNumber";
    this.labelFobNumber.UseCompatibleTextRendering = true;
    this.textBoxKeyfob1.BackColor = SystemColors.Window;
    componentResourceManager.ApplyResources((object) this.textBoxKeyfob1, "textBoxKeyfob1");
    this.textBoxKeyfob1.Name = "textBoxKeyfob1";
    this.textBoxKeyfob1.Tag = (object) "0";
    this.textBoxKeyfob1.TextChanged += new EventHandler(this.textBoxKeyfob_TextChanged);
    this.textBoxKeyfob1.KeyPress += new KeyPressEventHandler(this.textBoxKeyfob_KeyPress);
    componentResourceManager.ApplyResources((object) this.labelCurrentPairedFob, "labelCurrentPairedFob");
    this.labelCurrentPairedFob.Name = "labelCurrentPairedFob";
    this.labelCurrentPairedFob.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxKeyfob2, "textBoxKeyfob2");
    this.textBoxKeyfob2.Name = "textBoxKeyfob2";
    this.textBoxKeyfob2.TextChanged += new EventHandler(this.textBoxKeyfob_TextChanged);
    this.textBoxKeyfob2.KeyPress += new KeyPressEventHandler(this.textBoxKeyfob_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxKeyfob3, "textBoxKeyfob3");
    this.textBoxKeyfob3.Name = "textBoxKeyfob3";
    this.textBoxKeyfob3.TextChanged += new EventHandler(this.textBoxKeyfob_TextChanged);
    this.textBoxKeyfob3.KeyPress += new KeyPressEventHandler(this.textBoxKeyfob_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxKeyfob4, "textBoxKeyfob4");
    this.textBoxKeyfob4.Name = "textBoxKeyfob4";
    this.textBoxKeyfob4.TextChanged += new EventHandler(this.textBoxKeyfob_TextChanged);
    this.textBoxKeyfob4.KeyPress += new KeyPressEventHandler(this.textBoxKeyfob_KeyPress);
    componentResourceManager.ApplyResources((object) this.textBoxKeyfob5, "textBoxKeyfob5");
    this.textBoxKeyfob5.Name = "textBoxKeyfob5";
    this.textBoxKeyfob5.TextChanged += new EventHandler(this.textBoxKeyfob_TextChanged);
    this.textBoxKeyfob5.KeyPress += new KeyPressEventHandler(this.textBoxKeyfob_KeyPress);
    componentResourceManager.ApplyResources((object) this.labelFobNumber1, "labelFobNumber1");
    this.labelFobNumber1.Name = "labelFobNumber1";
    this.labelFobNumber1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelFobNumber2, "labelFobNumber2");
    this.labelFobNumber2.Name = "labelFobNumber2";
    this.labelFobNumber2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelFobNumber3, "labelFobNumber3");
    this.labelFobNumber3.Name = "labelFobNumber3";
    this.labelFobNumber3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelFobNumber4, "labelFobNumber4");
    this.labelFobNumber4.Name = "labelFobNumber4";
    this.labelFobNumber4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelFobNumber5, "labelFobNumber5");
    this.labelFobNumber5.Name = "labelFobNumber5";
    this.labelFobNumber5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonClear1, "buttonClear1");
    this.buttonClear1.Name = "buttonClear1";
    this.buttonClear1.Tag = (object) "0";
    this.buttonClear1.UseCompatibleTextRendering = true;
    this.buttonClear1.UseVisualStyleBackColor = true;
    this.buttonClear1.Click += new EventHandler(this.buttonClear_Click);
    componentResourceManager.ApplyResources((object) this.buttonClear2, "buttonClear2");
    this.buttonClear2.Name = "buttonClear2";
    this.buttonClear2.Tag = (object) "1";
    this.buttonClear2.UseCompatibleTextRendering = true;
    this.buttonClear2.UseVisualStyleBackColor = true;
    this.buttonClear2.Click += new EventHandler(this.buttonClear_Click);
    componentResourceManager.ApplyResources((object) this.buttonClear3, "buttonClear3");
    this.buttonClear3.Name = "buttonClear3";
    this.buttonClear3.Tag = (object) "2";
    this.buttonClear3.UseCompatibleTextRendering = true;
    this.buttonClear3.UseVisualStyleBackColor = true;
    this.buttonClear3.Click += new EventHandler(this.buttonClear_Click);
    componentResourceManager.ApplyResources((object) this.buttonClear4, "buttonClear4");
    this.buttonClear4.Name = "buttonClear4";
    this.buttonClear4.Tag = (object) "3";
    this.buttonClear4.UseCompatibleTextRendering = true;
    this.buttonClear4.UseVisualStyleBackColor = true;
    this.buttonClear4.Click += new EventHandler(this.buttonClear_Click);
    componentResourceManager.ApplyResources((object) this.buttonClear5, "buttonClear5");
    this.buttonClear5.Name = "buttonClear5";
    this.buttonClear5.Tag = (object) "4";
    this.buttonClear5.UseCompatibleTextRendering = true;
    this.buttonClear5.UseVisualStyleBackColor = true;
    this.buttonClear5.Click += new EventHandler(this.buttonClear_Click);
    componentResourceManager.ApplyResources((object) this.buttonReadValues, "buttonReadValues");
    this.buttonReadValues.Name = "buttonReadValues";
    this.buttonReadValues.UseCompatibleTextRendering = true;
    this.buttonReadValues.UseVisualStyleBackColor = true;
    this.buttonReadValues.Click += new EventHandler(this.buttonReadValues_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanel1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonWriteValues, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkCanStart, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelSetMessage, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.buttonWriteValues, "buttonWriteValues");
    this.buttonWriteValues.Name = "buttonWriteValues";
    this.buttonWriteValues.UseCompatibleTextRendering = true;
    this.buttonWriteValues.UseVisualStyleBackColor = true;
    this.buttonWriteValues.Click += new EventHandler(this.buttonWriteValues_Click);
    componentResourceManager.ApplyResources((object) this.checkmarkCanStart, "checkmarkCanStart");
    ((Control) this.checkmarkCanStart).Name = "checkmarkCanStart";
    componentResourceManager.ApplyResources((object) this.labelSetMessage, "labelSetMessage");
    this.labelSetMessage.Name = "labelSetMessage";
    this.labelSetMessage.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.seekTimeListView, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "DCMD Keyfob Pairing";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRKEUnlock, "digitalReadoutInstrumentRKEUnlock");
    this.digitalReadoutInstrumentRKEUnlock.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKEUnlock).FreezeValue = false;
    this.digitalReadoutInstrumentRKEUnlock.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentRKEUnlock.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentRKEUnlock.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKEUnlock).Instrument = new Qualifier((QualifierTypes) 1, "DCMD02T", "DT_RKE_Button_3_IN_RKE_Button3");
    ((Control) this.digitalReadoutInstrumentRKEUnlock).Name = "digitalReadoutInstrumentRKEUnlock";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.digitalReadoutInstrumentRKEUnlock, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKEUnlock).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRKEButton3, "digitalReadoutInstrumentRKEButton3");
    this.digitalReadoutInstrumentRKEButton3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKEButton3).FreezeValue = false;
    this.digitalReadoutInstrumentRKEButton3.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentRKEButton3.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentRKEButton3.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKEButton3).Instrument = new Qualifier((QualifierTypes) 1, "DCMD02T", "DT_RKE_Button_1_IN_RKE_Button1");
    ((Control) this.digitalReadoutInstrumentRKEButton3).Name = "digitalReadoutInstrumentRKEButton3";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.digitalReadoutInstrumentRKEButton3, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKEButton3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRKELock, "digitalReadoutInstrumentRKELock");
    this.digitalReadoutInstrumentRKELock.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKELock).FreezeValue = false;
    this.digitalReadoutInstrumentRKELock.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentRKELock.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentRKELock.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKELock).Instrument = new Qualifier((QualifierTypes) 1, "DCMD02T", "DT_RKE_Button_2_IN_RKE_Button2");
    ((Control) this.digitalReadoutInstrumentRKELock).Name = "digitalReadoutInstrumentRKELock";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.digitalReadoutInstrumentRKELock, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRKELock).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum KeyfobInputBoxesState
  {
    InvalidKeyfob,
    DuplicateKeyfob,
    NoChange,
    ChangesOk,
  }

  private enum ProcessState
  {
    NotRunning,
    RemoteSC,
    SendKeyfobIDs,
    HardReset,
    ReloadEcuinfo,
  }
}
