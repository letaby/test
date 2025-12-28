using System;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
		InitializeComponent();
		base.HeaderImage = Resources.option_warnings;
		AddItem("WarningCloseConnections", Resources.DialogsOptionsPanel_WarningCloseConnections);
		AddItem("WarningCloseLogFile", Resources.DialogsOptionsPanel_WarningCloseLogFile);
		AddItem("WarningLanguageChangeNeedsRestart", Resources.DialogsOptionsPanel_WarningChangeLanguage);
		AddItem("WarningMinimumRequiredRam", Resources.DialogsOptionsPanel_WarningMinimumRequiredRam);
		AddItem("WarningRollCallDisabled", Resources.DialogsOptionasPanel_WarningRollCallDisabled);
		AddItem("WarningAutoBaudRateUnavailable", Resources.DialogsOptionasPanel_WarningAutoBaudRateUnavailable);
		AddItem("WarningTroubleshootingUnavailable", Resources.DialogsOptionsPanel_WarningTroubleshootingUnavailable);
	}

	private void AddItem(string setting, string settingDisplayName)
	{
		ListViewItem listViewItem = listView.Items.Add(settingDisplayName);
		listViewItem.Name = setting;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		foreach (ListViewItem item in listView.Items)
		{
			item.Checked = WarningMessageBox.GetWarningShown(item.Name);
		}
	}

	private void OnClickEnableAll(object sender, EventArgs e)
	{
		foreach (ListViewItem item in listView.Items)
		{
			item.Checked = true;
		}
	}

	private void OnClickDisableAll(object sender, EventArgs e)
	{
		foreach (ListViewItem item in listView.Items)
		{
			item.Checked = false;
		}
	}

	public override bool ApplySettings()
	{
		bool result = true;
		if (base.IsDirty)
		{
			foreach (ListViewItem item in listView.Items)
			{
				WarningMessageBox.SetWarningShown(item.Name, item.Checked);
			}
			result = base.ApplySettings();
		}
		return result;
	}

	private void OnItemChecked(object sender, ItemCheckedEventArgs e)
	{
		MarkDirty();
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		if (listView != null)
		{
			int num = listView.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4;
			columnHeaderWarning.Width = num;
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.WarningsOptionsPanel));
		this.buttonEnableAll = new System.Windows.Forms.Button();
		this.buttonDisableAll = new System.Windows.Forms.Button();
		this.listView = new System.Windows.Forms.ListView();
		this.columnHeaderWarning = new System.Windows.Forms.ColumnHeader();
		this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
		this.flowLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.buttonEnableAll, "buttonEnableAll");
		this.buttonEnableAll.Name = "buttonEnableAll";
		this.buttonEnableAll.UseVisualStyleBackColor = true;
		this.buttonEnableAll.Click += new System.EventHandler(OnClickEnableAll);
		resources.ApplyResources(this.buttonDisableAll, "buttonDisableAll");
		this.buttonDisableAll.Name = "buttonDisableAll";
		this.buttonDisableAll.UseVisualStyleBackColor = true;
		this.buttonDisableAll.Click += new System.EventHandler(OnClickDisableAll);
		this.listView.CheckBoxes = true;
		this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeaderWarning });
		this.listView.FullRowSelect = true;
		this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
		resources.ApplyResources(this.listView, "listView");
		this.listView.Name = "listView";
		this.listView.ShowGroups = false;
		this.listView.ShowItemToolTips = true;
		this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.listView.UseCompatibleStateImageBehavior = false;
		this.listView.View = System.Windows.Forms.View.Details;
		this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(OnItemChecked);
		resources.ApplyResources(this.columnHeaderWarning, "columnHeaderWarning");
		resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
		this.flowLayoutPanel1.Controls.Add(this.buttonEnableAll);
		this.flowLayoutPanel1.Controls.Add(this.buttonDisableAll);
		this.flowLayoutPanel1.Name = "flowLayoutPanel1";
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.flowLayoutPanel1);
		base.Controls.Add(this.listView);
		base.Name = "WarningsOptionsPanel";
		base.Controls.SetChildIndex(this.listView, 0);
		base.Controls.SetChildIndex(this.flowLayoutPanel1, 0);
		this.flowLayoutPanel1.ResumeLayout(false);
		this.flowLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
