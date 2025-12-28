// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Device_Status.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Device_Status.panel;

public class UserPanel : CustomPanel
{
  private static string[] VinMasters = new string[3]
  {
    "CPC",
    "ICUC",
    "CGW"
  };
  private static string VinReadServiceQualifier = "CO_VIN";
  private static string EcuSerialNumber = "CO_EcuSerialNumber";
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonClose;
  private Button buttonGetStatus;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label label2;
  private System.Windows.Forms.Label labelStatus;
  private System.Windows.Forms.Label label4;
  private TextBox textBoxVin;
  private System.Windows.Forms.Label label5;
  private ComboBox comboBoxRequestType;
  private ComboBox comboBoxDeviceType;
  private System.Windows.Forms.Label label6;
  private System.Windows.Forms.Label labelOperationStatus;
  private TextBox textBoxDeviceId;

  public UserPanel()
  {
    this.InitializeComponent();
    this.comboBoxRequestType.SelectedIndex = 0;
  }

  private void UpdateUserInterface(bool isBusy)
  {
    this.buttonGetStatus.Enabled = !isBusy;
    this.comboBoxDeviceType.Enabled = !isBusy;
  }

  private void PopulateVin()
  {
    foreach (string vinMaster in UserPanel.VinMasters)
    {
      string ecuName = vinMaster;
      Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Name.StartsWith(ecuName)));
      if (channel != null && channel.EcuInfos[UserPanel.VinReadServiceQualifier] != null)
      {
        this.textBoxVin.Text = channel.EcuInfos[UserPanel.VinReadServiceQualifier].Value;
        break;
      }
    }
  }

  private void PopulateDeviceId()
  {
    Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Name.StartsWith(this.comboBoxDeviceType.Text)));
    if (channel == null || channel.EcuInfos[UserPanel.EcuSerialNumber] == null)
      return;
    this.textBoxDeviceId.Text = channel.EcuInfos[UserPanel.EcuSerialNumber].Value;
  }

  public virtual void OnChannelsChanged()
  {
    this.PopulateDeviceTypeCombo();
    this.PopulateVin();
  }

  private void PopulateDeviceTypeCombo()
  {
    this.comboBoxDeviceType.Items.Clear();
    foreach (Channel channel in SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((Func<Channel, bool>) (ecu => !ecu.IsRollCall)))
      this.comboBoxDeviceType.Items.Add((object) channel.Ecu.Name);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void GetDeviceStatus()
  {
    ServerClient.GlobalInstance.StatusReturned += new EventHandler<StatusReturnedEventArgs>(this.serverInstance_StatusReturned);
    this.labelStatus.Text = string.Empty;
    this.labelOperationStatus.Text = string.Empty;
    if (this.textBoxVin.Text != string.Empty && this.comboBoxRequestType.Text != string.Empty && this.comboBoxDeviceType.Text != string.Empty && this.textBoxDeviceId.Text != string.Empty)
    {
      this.UpdateUserInterface(true);
      ServerClient.GlobalInstance.GetStatus((IEnumerable<StatusItem>) new List<StatusItem>()
      {
        new StatusItem(this.textBoxVin.Text, this.comboBoxRequestType.Text, this.comboBoxDeviceType.Text, this.textBoxDeviceId.Text, string.Empty)
      });
    }
    else
    {
      int num = (int) MessageBox.Show(Resources.Message_AllFieldsMustBeFilledIn);
    }
  }

  private void buttonGetStatus_Click(object sender, EventArgs e) => this.GetDeviceStatus();

  private void serverInstance_StatusReturned(object sender, StatusReturnedEventArgs e)
  {
    ServerClient.GlobalInstance.StatusReturned -= new EventHandler<StatusReturnedEventArgs>(this.serverInstance_StatusReturned);
    if (e.Succeeded)
    {
      List<StatusItem> statusItems = ServerDataManager.GlobalInstance.StatusResponse.StatusItems as List<StatusItem>;
      if (statusItems.Count > 0)
        this.labelStatus.Text = statusItems[0].Status;
    }
    else
      this.labelOperationStatus.Text = e.ErrorMessage;
    this.UpdateUserInterface(false);
  }

  protected void comboBoxDeviceType_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.PopulateDeviceId();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.comboBoxDeviceType = new ComboBox();
    this.label6 = new System.Windows.Forms.Label();
    this.label5 = new System.Windows.Forms.Label();
    this.textBoxVin = new TextBox();
    this.label4 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.buttonGetStatus = new Button();
    this.textBoxDeviceId = new TextBox();
    this.label1 = new System.Windows.Forms.Label();
    this.labelStatus = new System.Windows.Forms.Label();
    this.comboBoxRequestType = new ComboBox();
    this.labelOperationStatus = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.comboBoxDeviceType, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label6, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label5, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxVin, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label4, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonGetStatus, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxDeviceId, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.comboBoxRequestType, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelOperationStatus, 0, 5);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.comboBoxDeviceType, 2);
    this.comboBoxDeviceType.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxDeviceType.FormattingEnabled = true;
    componentResourceManager.ApplyResources((object) this.comboBoxDeviceType, "comboBoxDeviceType");
    this.comboBoxDeviceType.Name = "comboBoxDeviceType";
    this.comboBoxDeviceType.Sorted = true;
    this.comboBoxDeviceType.SelectedIndexChanged += new EventHandler(this.comboBoxDeviceType_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxVin, 2);
    componentResourceManager.ApplyResources((object) this.textBoxVin, "textBoxVin");
    this.textBoxVin.Name = "textBoxVin";
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    this.label4.Name = "label4";
    this.label4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonGetStatus, "buttonGetStatus");
    this.buttonGetStatus.Name = "buttonGetStatus";
    this.buttonGetStatus.UseCompatibleTextRendering = true;
    this.buttonGetStatus.UseVisualStyleBackColor = true;
    this.buttonGetStatus.Click += new EventHandler(this.buttonGetStatus_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBoxDeviceId, 2);
    componentResourceManager.ApplyResources((object) this.textBoxDeviceId, "textBoxDeviceId");
    this.textBoxDeviceId.Name = "textBoxDeviceId";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.comboBoxRequestType, 2);
    this.comboBoxRequestType.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxRequestType.FormattingEnabled = true;
    this.comboBoxRequestType.Items.AddRange(new object[1]
    {
      (object) componentResourceManager.GetString("comboBoxRequestType.Items")
    });
    componentResourceManager.ApplyResources((object) this.comboBoxRequestType, "comboBoxRequestType");
    this.comboBoxRequestType.Name = "comboBoxRequestType";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelOperationStatus, 3);
    componentResourceManager.ApplyResources((object) this.labelOperationStatus, "labelOperationStatus");
    this.labelOperationStatus.ForeColor = SystemColors.ControlText;
    this.labelOperationStatus.Name = "labelOperationStatus";
    this.labelOperationStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
