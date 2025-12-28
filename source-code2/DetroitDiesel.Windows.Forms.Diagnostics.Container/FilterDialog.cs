using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
		get
		{
			return filterList;
		}
		set
		{
			if (value != filterList)
			{
				filterList = new Dictionary<FilterTypes, bool>(value);
			}
			UpdateFilterListbox();
		}
	}

	public FilterDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		Text = Resources.FilterDialog_Title;
		labelUserMessage.Text = Resources.FilterDialog_UserText;
		selectAllButton.Text = Resources.FilterDialog_SelectAll;
		selectNoneButton.Text = Resources.FilterDialog_SelectNone;
	}

	private void UpdateFilterListbox()
	{
		lock (filterList)
		{
			List<string> validFilterDisplayNames = filterList.Keys.Select((FilterTypes filter) => FilterTypeExtensions.ToDisplayString(filter)).ToList();
			List<string> list = (from string filterName in checkedListBoxFilters.Items
				where !validFilterDisplayNames.Contains(filterName)
				select filterName).ToList();
			list.ForEach(delegate(string x)
			{
				checkedListBoxFilters.Items.Remove(x);
			});
			filterList.Where((KeyValuePair<FilterTypes, bool> x) => !checkedListBoxFilters.Items.Contains(FilterTypeExtensions.ToDisplayString(x.Key))).ToList().ForEach(delegate(KeyValuePair<FilterTypes, bool> x)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				checkedListBoxFilters.Items.Add(FilterTypeExtensions.ToDisplayString(x.Key), x.Value);
			});
			UpdateSelectAllVisibility();
		}
	}

	private void UpdateFilterListFromUI()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		lock (filterList)
		{
			filterList.Clear();
			foreach (string item in checkedListBoxFilters.Items)
			{
				bool value = checkedListBoxFilters.CheckedItems.Contains(item);
				filterList.Add(FilterTypeExtensions.FromDisplayString(item), value);
			}
		}
	}

	private void UpdateSelectAllVisibility()
	{
		Button button = selectAllButton;
		bool visible = (selectNoneButton.Visible = checkedListBoxFilters.Items.Count > 0);
		button.Visible = visible;
	}

	private void checkedListBoxFilters_SelectedValueChanged(object sender, EventArgs e)
	{
		UpdateFilterListFromUI();
	}

	private void FilterDialog_HelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_ViewFilter"));
	}

	private void selectAllButton_Click(object sender, EventArgs e)
	{
		SetAllFilters(@checked: true);
	}

	private void selectNoneButton_Click(object sender, EventArgs e)
	{
		SetAllFilters(@checked: false);
	}

	private void SetAllFilters(bool @checked)
	{
		for (int i = 0; i < checkedListBoxFilters.Items.Count; i++)
		{
			checkedListBoxFilters.SetItemChecked(i, @checked);
		}
		UpdateFilterListFromUI();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.buttonOK = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.labelUserMessage = new System.Windows.Forms.Label();
		this.checkedListBoxFilters = new System.Windows.Forms.CheckedListBox();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.selectAllButton = new System.Windows.Forms.Button();
		this.selectNoneButton = new System.Windows.Forms.Button();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(200, 262);
		this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 1;
		this.buttonOK.Text = "OK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(288, 262);
		this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 3, 10, 8);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 2;
		this.buttonCancel.Text = "Cancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.labelUserMessage.AutoSize = true;
		this.tableLayoutPanel1.SetColumnSpan(this.labelUserMessage, 3);
		this.labelUserMessage.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelUserMessage.Location = new System.Drawing.Point(10, 8);
		this.labelUserMessage.Margin = new System.Windows.Forms.Padding(10, 8, 10, 3);
		this.labelUserMessage.Name = "labelUserMessage";
		this.labelUserMessage.Size = new System.Drawing.Size(353, 16);
		this.labelUserMessage.TabIndex = 3;
		this.labelUserMessage.Text = "Please select the types of panels that you wish to be visible in the tool.";
		this.labelUserMessage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
		this.checkedListBoxFilters.CheckOnClick = true;
		this.tableLayoutPanel1.SetColumnSpan(this.checkedListBoxFilters, 2);
		this.checkedListBoxFilters.Dock = System.Windows.Forms.DockStyle.Fill;
		this.checkedListBoxFilters.FormattingEnabled = true;
		this.checkedListBoxFilters.IntegralHeight = false;
		this.checkedListBoxFilters.Location = new System.Drawing.Point(10, 30);
		this.checkedListBoxFilters.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
		this.checkedListBoxFilters.Name = "checkedListBoxFilters";
		this.tableLayoutPanel1.SetRowSpan(this.checkedListBoxFilters, 2);
		this.checkedListBoxFilters.Size = new System.Drawing.Size(265, 225);
		this.checkedListBoxFilters.TabIndex = 5;
		this.checkedListBoxFilters.ThreeDCheckBoxes = true;
		this.checkedListBoxFilters.SelectedIndexChanged += new System.EventHandler(checkedListBoxFilters_SelectedValueChanged);
		this.tableLayoutPanel1.ColumnCount = 3;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95f));
		this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 3);
		this.tableLayoutPanel1.Controls.Add(this.checkedListBoxFilters, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.buttonOK, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.labelUserMessage, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.selectAllButton, 2, 1);
		this.tableLayoutPanel1.Controls.Add(this.selectNoneButton, 2, 2);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 4;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(373, 293);
		this.tableLayoutPanel1.TabIndex = 6;
		this.selectAllButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.selectAllButton.Location = new System.Drawing.Point(288, 30);
		this.selectAllButton.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
		this.selectAllButton.Name = "selectAllButton";
		this.selectAllButton.Size = new System.Drawing.Size(75, 23);
		this.selectAllButton.TabIndex = 7;
		this.selectAllButton.Text = "Select All";
		this.selectAllButton.UseVisualStyleBackColor = true;
		this.selectAllButton.Click += new System.EventHandler(selectAllButton_Click);
		this.selectNoneButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.selectNoneButton.Location = new System.Drawing.Point(288, 59);
		this.selectNoneButton.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
		this.selectNoneButton.Name = "selectNoneButton";
		this.selectNoneButton.Size = new System.Drawing.Size(75, 23);
		this.selectNoneButton.TabIndex = 8;
		this.selectNoneButton.Text = "Select None";
		this.selectNoneButton.UseVisualStyleBackColor = true;
		this.selectNoneButton.Click += new System.EventHandler(selectNoneButton_Click);
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(373, 293);
		base.Controls.Add(this.tableLayoutPanel1);
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(380, 200);
		base.Name = "FilterDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "View Filters";
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(FilterDialog_HelpButtonClicked);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
