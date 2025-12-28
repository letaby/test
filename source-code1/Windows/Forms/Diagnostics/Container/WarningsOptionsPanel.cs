// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.WarningsOptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class WarningsOptionsPanel : OptionsPanel
{
  private IContainer components;
  private Button buttonEnableAll;
  private Button buttonDisableAll;
  private ListView listView;
  private ColumnHeader columnHeaderWarning;
  private FlowLayoutPanel flowLayoutPanel1;

  public WarningsOptionsPanel()
  {
    this.InitializeComponent();
    this.HeaderImage = (Image) Resources.option_warnings;
    this.AddItem("WarningCloseConnections", Resources.DialogsOptionsPanel_WarningCloseConnections);
    this.AddItem("WarningCloseLogFile", Resources.DialogsOptionsPanel_WarningCloseLogFile);
    this.AddItem("WarningLanguageChangeNeedsRestart", Resources.DialogsOptionsPanel_WarningChangeLanguage);
    this.AddItem("WarningMinimumRequiredRam", Resources.DialogsOptionsPanel_WarningMinimumRequiredRam);
    this.AddItem("WarningRollCallDisabled", Resources.DialogsOptionasPanel_WarningRollCallDisabled);
    this.AddItem("WarningAutoBaudRateUnavailable", Resources.DialogsOptionasPanel_WarningAutoBaudRateUnavailable);
    this.AddItem("WarningTroubleshootingUnavailable", Resources.DialogsOptionsPanel_WarningTroubleshootingUnavailable);
  }

  private void AddItem(string setting, string settingDisplayName)
  {
    this.listView.Items.Add(settingDisplayName).Name = setting;
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    foreach (ListViewItem listViewItem in this.listView.Items)
      listViewItem.Checked = WarningMessageBox.GetWarningShown(listViewItem.Name);
  }

  private void OnClickEnableAll(object sender, EventArgs e)
  {
    foreach (ListViewItem listViewItem in this.listView.Items)
      listViewItem.Checked = true;
  }

  private void OnClickDisableAll(object sender, EventArgs e)
  {
    foreach (ListViewItem listViewItem in this.listView.Items)
      listViewItem.Checked = false;
  }

  public override bool ApplySettings()
  {
    bool flag = true;
    if (this.IsDirty)
    {
      foreach (ListViewItem listViewItem in this.listView.Items)
        WarningMessageBox.SetWarningShown(listViewItem.Name, listViewItem.Checked);
      flag = base.ApplySettings();
    }
    return flag;
  }

  private void OnItemChecked(object sender, ItemCheckedEventArgs e) => this.MarkDirty();

  protected override void OnSizeChanged(EventArgs e)
  {
    base.OnSizeChanged(e);
    if (this.listView == null)
      return;
    this.columnHeaderWarning.Width = this.listView.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (WarningsOptionsPanel));
    this.buttonEnableAll = new Button();
    this.buttonDisableAll = new Button();
    this.listView = new ListView();
    this.columnHeaderWarning = new ColumnHeader();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.flowLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.buttonEnableAll, "buttonEnableAll");
    this.buttonEnableAll.Name = "buttonEnableAll";
    this.buttonEnableAll.UseVisualStyleBackColor = true;
    this.buttonEnableAll.Click += new EventHandler(this.OnClickEnableAll);
    componentResourceManager.ApplyResources((object) this.buttonDisableAll, "buttonDisableAll");
    this.buttonDisableAll.Name = "buttonDisableAll";
    this.buttonDisableAll.UseVisualStyleBackColor = true;
    this.buttonDisableAll.Click += new EventHandler(this.OnClickDisableAll);
    this.listView.CheckBoxes = true;
    this.listView.Columns.AddRange(new ColumnHeader[1]
    {
      this.columnHeaderWarning
    });
    this.listView.FullRowSelect = true;
    this.listView.HeaderStyle = ColumnHeaderStyle.None;
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.Name = "listView";
    this.listView.ShowGroups = false;
    this.listView.ShowItemToolTips = true;
    this.listView.Sorting = SortOrder.Ascending;
    this.listView.UseCompatibleStateImageBehavior = false;
    this.listView.View = View.Details;
    this.listView.ItemChecked += new ItemCheckedEventHandler(this.OnItemChecked);
    componentResourceManager.ApplyResources((object) this.columnHeaderWarning, "columnHeaderWarning");
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    this.flowLayoutPanel1.Controls.Add((Control) this.buttonEnableAll);
    this.flowLayoutPanel1.Controls.Add((Control) this.buttonDisableAll);
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Controls.Add((Control) this.flowLayoutPanel1);
    this.Controls.Add((Control) this.listView);
    this.Name = nameof (WarningsOptionsPanel);
    this.Controls.SetChildIndex((Control) this.listView, 0);
    this.Controls.SetChildIndex((Control) this.flowLayoutPanel1, 0);
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
