// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.FilterDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class FilterDialog : Form
{
  private Dictionary<FilterTypes, bool> filterList = new Dictionary<FilterTypes, bool>();
  private IContainer components;
  private Button buttonOK;
  private Button buttonCancel;
  private Label labelUserMessage;
  private CheckedListBox checkedListBoxFilters;
  private TableLayoutPanel tableLayoutPanel1;
  private Button selectAllButton;
  private Button selectNoneButton;

  public Dictionary<FilterTypes, bool> FilterList
  {
    set
    {
      if (value != this.filterList)
        this.filterList = new Dictionary<FilterTypes, bool>((IDictionary<FilterTypes, bool>) value);
      this.UpdateFilterListbox();
    }
    get => this.filterList;
  }

  public FilterDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.Text = Resources.FilterDialog_Title;
    this.labelUserMessage.Text = Resources.FilterDialog_UserText;
    this.selectAllButton.Text = Resources.FilterDialog_SelectAll;
    this.selectNoneButton.Text = Resources.FilterDialog_SelectNone;
  }

  private void UpdateFilterListbox()
  {
    lock (this.filterList)
    {
      List<string> validFilterDisplayNames = this.filterList.Keys.Select<FilterTypes, string>((Func<FilterTypes, string>) (filter => FilterTypeExtensions.ToDisplayString(filter))).ToList<string>();
      this.checkedListBoxFilters.Items.Cast<string>().Where<string>((Func<string, bool>) (filterName => !validFilterDisplayNames.Contains(filterName))).ToList<string>().ForEach((Action<string>) (x => this.checkedListBoxFilters.Items.Remove((object) x)));
      this.filterList.Where<KeyValuePair<FilterTypes, bool>>((Func<KeyValuePair<FilterTypes, bool>, bool>) (x => !this.checkedListBoxFilters.Items.Contains((object) FilterTypeExtensions.ToDisplayString(x.Key)))).ToList<KeyValuePair<FilterTypes, bool>>().ForEach((Action<KeyValuePair<FilterTypes, bool>>) (x => this.checkedListBoxFilters.Items.Add((object) FilterTypeExtensions.ToDisplayString(x.Key), x.Value)));
      this.UpdateSelectAllVisibility();
    }
  }

  private void UpdateFilterListFromUI()
  {
    lock (this.filterList)
    {
      this.filterList.Clear();
      foreach (string str in (ListBox.ObjectCollection) this.checkedListBoxFilters.Items)
      {
        bool flag = this.checkedListBoxFilters.CheckedItems.Contains((object) str);
        this.filterList.Add(FilterTypeExtensions.FromDisplayString(str), flag);
      }
    }
  }

  private void UpdateSelectAllVisibility()
  {
    this.selectAllButton.Visible = this.selectNoneButton.Visible = this.checkedListBoxFilters.Items.Count > 0;
  }

  private void checkedListBoxFilters_SelectedValueChanged(object sender, EventArgs e)
  {
    this.UpdateFilterListFromUI();
  }

  private void FilterDialog_HelpButtonClicked(object sender, CancelEventArgs e)
  {
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_ViewFilter"));
  }

  private void selectAllButton_Click(object sender, EventArgs e) => this.SetAllFilters(true);

  private void selectNoneButton_Click(object sender, EventArgs e) => this.SetAllFilters(false);

  private void SetAllFilters(bool @checked)
  {
    for (int index = 0; index < this.checkedListBoxFilters.Items.Count; ++index)
      this.checkedListBoxFilters.SetItemChecked(index, @checked);
    this.UpdateFilterListFromUI();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.buttonOK = new Button();
    this.buttonCancel = new Button();
    this.labelUserMessage = new Label();
    this.checkedListBoxFilters = new CheckedListBox();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.selectAllButton = new Button();
    this.selectNoneButton = new Button();
    this.tableLayoutPanel1.SuspendLayout();
    this.SuspendLayout();
    this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.buttonOK.DialogResult = DialogResult.OK;
    this.buttonOK.Location = new Point(200, 262);
    this.buttonOK.Margin = new Padding(3, 3, 3, 8);
    this.buttonOK.Name = "buttonOK";
    this.buttonOK.Size = new Size(75, 23);
    this.buttonOK.TabIndex = 1;
    this.buttonOK.Text = "OK";
    this.buttonOK.UseVisualStyleBackColor = true;
    this.buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Location = new Point(288, 262);
    this.buttonCancel.Margin = new Padding(3, 3, 10, 8);
    this.buttonCancel.Name = "buttonCancel";
    this.buttonCancel.Size = new Size(75, 23);
    this.buttonCancel.TabIndex = 2;
    this.buttonCancel.Text = "Cancel";
    this.buttonCancel.UseVisualStyleBackColor = true;
    this.labelUserMessage.AutoSize = true;
    this.tableLayoutPanel1.SetColumnSpan((Control) this.labelUserMessage, 3);
    this.labelUserMessage.Dock = DockStyle.Fill;
    this.labelUserMessage.Location = new Point(10, 8);
    this.labelUserMessage.Margin = new Padding(10, 8, 10, 3);
    this.labelUserMessage.Name = "labelUserMessage";
    this.labelUserMessage.Size = new Size(353, 16 /*0x10*/);
    this.labelUserMessage.TabIndex = 3;
    this.labelUserMessage.Text = "Please select the types of panels that you wish to be visible in the tool.";
    this.labelUserMessage.TextAlign = ContentAlignment.BottomLeft;
    this.checkedListBoxFilters.CheckOnClick = true;
    this.tableLayoutPanel1.SetColumnSpan((Control) this.checkedListBoxFilters, 2);
    this.checkedListBoxFilters.Dock = DockStyle.Fill;
    this.checkedListBoxFilters.FormattingEnabled = true;
    this.checkedListBoxFilters.IntegralHeight = false;
    this.checkedListBoxFilters.Location = new Point(10, 30);
    this.checkedListBoxFilters.Margin = new Padding(10, 3, 3, 3);
    this.checkedListBoxFilters.Name = "checkedListBoxFilters";
    this.tableLayoutPanel1.SetRowSpan((Control) this.checkedListBoxFilters, 2);
    this.checkedListBoxFilters.Size = new Size(265, 225);
    this.checkedListBoxFilters.TabIndex = 5;
    this.checkedListBoxFilters.ThreeDCheckBoxes = true;
    this.checkedListBoxFilters.SelectedIndexChanged += new EventHandler(this.checkedListBoxFilters_SelectedValueChanged);
    this.tableLayoutPanel1.ColumnCount = 3;
    this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
    this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95f));
    this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95f));
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonCancel, 2, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.checkedListBoxFilters, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.buttonOK, 1, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.labelUserMessage, 0, 0);
    this.tableLayoutPanel1.Controls.Add((Control) this.selectAllButton, 2, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.selectNoneButton, 2, 2);
    this.tableLayoutPanel1.Dock = DockStyle.Fill;
    this.tableLayoutPanel1.Location = new Point(0, 0);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    this.tableLayoutPanel1.RowCount = 4;
    this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 27f));
    this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
    this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
    this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35f));
    this.tableLayoutPanel1.Size = new Size(373, 293);
    this.tableLayoutPanel1.TabIndex = 6;
    this.selectAllButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.selectAllButton.Location = new Point(288, 30);
    this.selectAllButton.Margin = new Padding(3, 3, 10, 3);
    this.selectAllButton.Name = "selectAllButton";
    this.selectAllButton.Size = new Size(75, 23);
    this.selectAllButton.TabIndex = 7;
    this.selectAllButton.Text = "Select All";
    this.selectAllButton.UseVisualStyleBackColor = true;
    this.selectAllButton.Click += new EventHandler(this.selectAllButton_Click);
    this.selectNoneButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.selectNoneButton.Location = new Point(288, 59);
    this.selectNoneButton.Margin = new Padding(3, 3, 10, 3);
    this.selectNoneButton.Name = "selectNoneButton";
    this.selectNoneButton.Size = new Size(75, 23);
    this.selectNoneButton.TabIndex = 8;
    this.selectNoneButton.Text = "Select None";
    this.selectNoneButton.UseVisualStyleBackColor = true;
    this.selectNoneButton.Click += new EventHandler(this.selectNoneButton_Click);
    this.AcceptButton = (IButtonControl) this.buttonOK;
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.ClientSize = new Size(373, 293);
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.MinimumSize = new Size(380, 200);
    this.Name = nameof (FilterDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = "View Filters";
    this.HelpButtonClicked += new CancelEventHandler(this.FilterDialog_HelpButtonClicked);
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.ResumeLayout(false);
  }
}
