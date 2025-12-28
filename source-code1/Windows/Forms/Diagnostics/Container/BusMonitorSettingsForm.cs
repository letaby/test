// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.BusMonitorSettingsForm
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Settings;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class BusMonitorSettingsForm : Form
{
  private IContainer components;
  private ComboBox comboBoxCan1;
  private TableLayoutPanel tableLayoutPanel;
  private ComboBox comboBoxCan2;
  private ComboBox comboBoxEthernet;
  private Button buttonOK;
  private Label label4;
  private CheckBox checkBoxCan1;
  private CheckBox checkBoxCan2;
  private CheckBox checkBoxEthernet;

  public BusMonitorSettingsForm()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
  }

  private static void AddResource(
    ComboBox comboBox,
    CheckBox checkBox,
    string displayString,
    string settingString)
  {
    int num = comboBox.Items.Add((object) displayString);
    if (!(displayString == settingString))
      return;
    comboBox.SelectedIndex = num;
    checkBox.Checked = true;
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    string settingString1 = SettingsManager.GlobalInstance.GetValue<StringSetting>("CAN1", "BusMonitorWindow", new StringSetting()).Value;
    string settingString2 = SettingsManager.GlobalInstance.GetValue<StringSetting>("CAN2", "BusMonitorWindow", new StringSetting()).Value;
    string settingString3 = SettingsManager.GlobalInstance.GetValue<StringSetting>("ETHERNET", "BusMonitorWindow", new StringSetting()).Value;
    foreach (ConnectionResource monitoringResource in (ReadOnlyCollection<ConnectionResource>) BusMonitorCollection.GetAvailableMonitoringResources())
    {
      string displayString = SapiExtensions.ToDisplayString(monitoringResource);
      switch (SapiExtensions.GetPhysicalInterfaceLink(monitoringResource))
      {
        case "CAN1":
          BusMonitorSettingsForm.AddResource(this.comboBoxCan1, this.checkBoxCan1, displayString, settingString1);
          continue;
        case "CAN2":
          BusMonitorSettingsForm.AddResource(this.comboBoxCan2, this.checkBoxCan2, displayString, settingString2);
          continue;
        default:
          continue;
      }
    }
    foreach (NetworkInterface networkInterface in ((IEnumerable<NetworkInterface>) NetworkInterface.GetAllNetworkInterfaces()).Where<NetworkInterface>((Func<NetworkInterface, bool>) (ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)))
      BusMonitorSettingsForm.AddResource(this.comboBoxEthernet, this.checkBoxEthernet, networkInterface.Description, settingString3);
    if (this.comboBoxCan1.Items.Count > 0)
      this.checkBoxCan1.Enabled = true;
    if (this.comboBoxCan2.Items.Count > 0)
      this.checkBoxCan2.Enabled = true;
    if (this.comboBoxEthernet.Items.Count <= 0)
      return;
    this.checkBoxEthernet.Enabled = true;
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    base.OnClosing(e);
    if (this.DialogResult != DialogResult.OK)
      return;
    string str1 = this.checkBoxCan1.Checked ? this.comboBoxCan1.Text : string.Empty;
    string str2 = this.checkBoxCan2.Checked ? this.comboBoxCan2.Text : string.Empty;
    string str3 = this.checkBoxEthernet.Checked ? this.comboBoxEthernet.Text : string.Empty;
    SettingsManager.GlobalInstance.SetValue<StringSetting>("CAN1", "BusMonitorWindow", new StringSetting(str1), false);
    SettingsManager.GlobalInstance.SetValue<StringSetting>("CAN2", "BusMonitorWindow", new StringSetting(str2), false);
    SettingsManager.GlobalInstance.SetValue<StringSetting>("ETHERNET", "BusMonitorWindow", new StringSetting(str3), false);
  }

  private void checkBoxCan1_CheckedChanged(object sender, EventArgs e)
  {
    this.comboBoxCan1.Enabled = this.checkBoxCan1.Checked;
  }

  private void checkBoxCan2_CheckedChanged(object sender, EventArgs e)
  {
    this.comboBoxCan2.Enabled = this.checkBoxCan2.Checked;
  }

  private void checkBoxEthernet_CheckedChanged(object sender, EventArgs e)
  {
    this.comboBoxEthernet.Enabled = this.checkBoxEthernet.Checked;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.comboBoxCan1 = new ComboBox();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.comboBoxCan2 = new ComboBox();
    this.comboBoxEthernet = new ComboBox();
    this.checkBoxCan1 = new CheckBox();
    this.checkBoxCan2 = new CheckBox();
    this.checkBoxEthernet = new CheckBox();
    this.buttonOK = new Button();
    this.label4 = new Label();
    this.tableLayoutPanel.SuspendLayout();
    this.SuspendLayout();
    this.comboBoxCan1.Dock = DockStyle.Fill;
    this.comboBoxCan1.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxCan1.Enabled = false;
    this.comboBoxCan1.FormattingEnabled = true;
    this.comboBoxCan1.Location = new Point(114, 3);
    this.comboBoxCan1.Name = "comboBoxCan1";
    this.comboBoxCan1.Size = new Size(412, 24);
    this.comboBoxCan1.TabIndex = 3;
    this.tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
    this.tableLayoutPanel.ColumnCount = 2;
    this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
    this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxCan1, 1, 1);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxCan2, 1, 2);
    this.tableLayoutPanel.Controls.Add((Control) this.comboBoxEthernet, 1, 3);
    this.tableLayoutPanel.Controls.Add((Control) this.checkBoxCan1, 0, 1);
    this.tableLayoutPanel.Controls.Add((Control) this.checkBoxCan2, 0, 2);
    this.tableLayoutPanel.Controls.Add((Control) this.checkBoxEthernet, 0, 3);
    this.tableLayoutPanel.Location = new Point(12, 55);
    this.tableLayoutPanel.Name = "tableLayoutPanel";
    this.tableLayoutPanel.RowCount = 5;
    this.tableLayoutPanel.RowStyles.Add(new RowStyle());
    this.tableLayoutPanel.RowStyles.Add(new RowStyle());
    this.tableLayoutPanel.RowStyles.Add(new RowStyle());
    this.tableLayoutPanel.RowStyles.Add(new RowStyle());
    this.tableLayoutPanel.RowStyles.Add(new RowStyle());
    this.tableLayoutPanel.Size = new Size(529, 147);
    this.tableLayoutPanel.TabIndex = 7;
    this.comboBoxCan2.Dock = DockStyle.Fill;
    this.comboBoxCan2.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxCan2.Enabled = false;
    this.comboBoxCan2.FormattingEnabled = true;
    this.comboBoxCan2.Location = new Point(114, 33);
    this.comboBoxCan2.Name = "comboBoxCan2";
    this.comboBoxCan2.Size = new Size(412, 24);
    this.comboBoxCan2.TabIndex = 4;
    this.comboBoxEthernet.Dock = DockStyle.Fill;
    this.comboBoxEthernet.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxEthernet.Enabled = false;
    this.comboBoxEthernet.FormattingEnabled = true;
    this.comboBoxEthernet.Location = new Point(114, 63 /*0x3F*/);
    this.comboBoxEthernet.Name = "comboBoxEthernet";
    this.comboBoxEthernet.Size = new Size(412, 24);
    this.comboBoxEthernet.TabIndex = 5;
    this.checkBoxCan1.AutoSize = true;
    this.checkBoxCan1.Dock = DockStyle.Fill;
    this.checkBoxCan1.Enabled = false;
    this.checkBoxCan1.Location = new Point(3, 3);
    this.checkBoxCan1.Name = "checkBoxCan1";
    this.checkBoxCan1.Size = new Size(105, 24);
    this.checkBoxCan1.TabIndex = 7;
    this.checkBoxCan1.Text = "CAN1";
    this.checkBoxCan1.UseVisualStyleBackColor = true;
    this.checkBoxCan1.CheckedChanged += new EventHandler(this.checkBoxCan1_CheckedChanged);
    this.checkBoxCan2.AutoSize = true;
    this.checkBoxCan2.Dock = DockStyle.Fill;
    this.checkBoxCan2.Enabled = false;
    this.checkBoxCan2.Location = new Point(3, 33);
    this.checkBoxCan2.Name = "checkBoxCan2";
    this.checkBoxCan2.Size = new Size(105, 24);
    this.checkBoxCan2.TabIndex = 8;
    this.checkBoxCan2.Text = "CAN2";
    this.checkBoxCan2.UseVisualStyleBackColor = true;
    this.checkBoxCan2.CheckedChanged += new EventHandler(this.checkBoxCan2_CheckedChanged);
    this.checkBoxEthernet.AutoSize = true;
    this.checkBoxEthernet.Dock = DockStyle.Fill;
    this.checkBoxEthernet.Enabled = false;
    this.checkBoxEthernet.Location = new Point(3, 63 /*0x3F*/);
    this.checkBoxEthernet.Name = "checkBoxEthernet";
    this.checkBoxEthernet.Size = new Size(105, 24);
    this.checkBoxEthernet.TabIndex = 9;
    this.checkBoxEthernet.Text = "ETHERNET";
    this.checkBoxEthernet.UseVisualStyleBackColor = true;
    this.checkBoxEthernet.CheckedChanged += new EventHandler(this.checkBoxEthernet_CheckedChanged);
    this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.buttonOK.DialogResult = DialogResult.OK;
    this.buttonOK.Location = new Point(415, 212);
    this.buttonOK.Name = "buttonOK";
    this.buttonOK.Size = new Size(126, 31 /*0x1F*/);
    this.buttonOK.TabIndex = 8;
    this.buttonOK.Text = "OK";
    this.buttonOK.UseVisualStyleBackColor = true;
    this.label4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    this.label4.Location = new Point(9, 9);
    this.label4.Name = "label4";
    this.label4.Size = new Size(532, 43);
    this.label4.TabIndex = 6;
    this.label4.Text = "Use the following specific connection resources for the specified physical interface links.";
    this.AutoScaleDimensions = new SizeF(8f, 16f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = SystemColors.Window;
    this.ClientSize = new Size(553, (int) byte.MaxValue);
    this.Controls.Add((Control) this.tableLayoutPanel);
    this.Controls.Add((Control) this.buttonOK);
    this.Controls.Add((Control) this.label4);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.MinimumSize = new Size(570, 300);
    this.Name = nameof (BusMonitorSettingsForm);
    this.ShowIcon = false;
    this.Text = "Bus Monitor Settings";
    this.tableLayoutPanel.ResumeLayout(false);
    this.tableLayoutPanel.PerformLayout();
    this.ResumeLayout(false);
  }
}
