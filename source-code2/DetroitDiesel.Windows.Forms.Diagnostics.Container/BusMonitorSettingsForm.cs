using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Settings;
using SapiLayer1;

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
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
	}

	private static void AddResource(ComboBox comboBox, CheckBox checkBox, string displayString, string settingString)
	{
		int selectedIndex = comboBox.Items.Add(displayString);
		if (displayString == settingString)
		{
			comboBox.SelectedIndex = selectedIndex;
			checkBox.Checked = true;
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		base.OnLoad(e);
		string value = SettingsManager.GlobalInstance.GetValue<StringSetting>("CAN1", "BusMonitorWindow", new StringSetting()).Value;
		string value2 = SettingsManager.GlobalInstance.GetValue<StringSetting>("CAN2", "BusMonitorWindow", new StringSetting()).Value;
		string value3 = SettingsManager.GlobalInstance.GetValue<StringSetting>("ETHERNET", "BusMonitorWindow", new StringSetting()).Value;
		ConnectionResourceCollection availableMonitoringResources = BusMonitorCollection.GetAvailableMonitoringResources();
		foreach (ConnectionResource item in (ReadOnlyCollection<ConnectionResource>)(object)availableMonitoringResources)
		{
			string displayString = SapiExtensions.ToDisplayString(item);
			string physicalInterfaceLink = SapiExtensions.GetPhysicalInterfaceLink(item);
			if (!(physicalInterfaceLink == "CAN1"))
			{
				if (physicalInterfaceLink == "CAN2")
				{
					AddResource(comboBoxCan2, checkBoxCan2, displayString, value2);
				}
			}
			else
			{
				AddResource(comboBoxCan1, checkBoxCan1, displayString, value);
			}
		}
		foreach (NetworkInterface item2 in from ni in NetworkInterface.GetAllNetworkInterfaces()
			where ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
			select ni)
		{
			AddResource(comboBoxEthernet, checkBoxEthernet, item2.Description, value3);
		}
		if (comboBoxCan1.Items.Count > 0)
		{
			checkBoxCan1.Enabled = true;
		}
		if (comboBoxCan2.Items.Count > 0)
		{
			checkBoxCan2.Enabled = true;
		}
		if (comboBoxEthernet.Items.Count > 0)
		{
			checkBoxEthernet.Enabled = true;
		}
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		base.OnClosing(e);
		if (base.DialogResult == DialogResult.OK)
		{
			string text = (checkBoxCan1.Checked ? comboBoxCan1.Text : string.Empty);
			string text2 = (checkBoxCan2.Checked ? comboBoxCan2.Text : string.Empty);
			string text3 = (checkBoxEthernet.Checked ? comboBoxEthernet.Text : string.Empty);
			SettingsManager.GlobalInstance.SetValue<StringSetting>("CAN1", "BusMonitorWindow", new StringSetting(text), false);
			SettingsManager.GlobalInstance.SetValue<StringSetting>("CAN2", "BusMonitorWindow", new StringSetting(text2), false);
			SettingsManager.GlobalInstance.SetValue<StringSetting>("ETHERNET", "BusMonitorWindow", new StringSetting(text3), false);
		}
	}

	private void checkBoxCan1_CheckedChanged(object sender, EventArgs e)
	{
		comboBoxCan1.Enabled = checkBoxCan1.Checked;
	}

	private void checkBoxCan2_CheckedChanged(object sender, EventArgs e)
	{
		comboBoxCan2.Enabled = checkBoxCan2.Checked;
	}

	private void checkBoxEthernet_CheckedChanged(object sender, EventArgs e)
	{
		comboBoxEthernet.Enabled = checkBoxEthernet.Checked;
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
		this.comboBoxCan1 = new System.Windows.Forms.ComboBox();
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.comboBoxCan2 = new System.Windows.Forms.ComboBox();
		this.comboBoxEthernet = new System.Windows.Forms.ComboBox();
		this.checkBoxCan1 = new System.Windows.Forms.CheckBox();
		this.checkBoxCan2 = new System.Windows.Forms.CheckBox();
		this.checkBoxEthernet = new System.Windows.Forms.CheckBox();
		this.buttonOK = new System.Windows.Forms.Button();
		this.label4 = new System.Windows.Forms.Label();
		this.tableLayoutPanel.SuspendLayout();
		base.SuspendLayout();
		this.comboBoxCan1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.comboBoxCan1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxCan1.Enabled = false;
		this.comboBoxCan1.FormattingEnabled = true;
		this.comboBoxCan1.Location = new System.Drawing.Point(114, 3);
		this.comboBoxCan1.Name = "comboBoxCan1";
		this.comboBoxCan1.Size = new System.Drawing.Size(412, 24);
		this.comboBoxCan1.TabIndex = 3;
		this.tableLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.tableLayoutPanel.ColumnCount = 2;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
		this.tableLayoutPanel.Controls.Add(this.comboBoxCan1, 1, 1);
		this.tableLayoutPanel.Controls.Add(this.comboBoxCan2, 1, 2);
		this.tableLayoutPanel.Controls.Add(this.comboBoxEthernet, 1, 3);
		this.tableLayoutPanel.Controls.Add(this.checkBoxCan1, 0, 1);
		this.tableLayoutPanel.Controls.Add(this.checkBoxCan2, 0, 2);
		this.tableLayoutPanel.Controls.Add(this.checkBoxEthernet, 0, 3);
		this.tableLayoutPanel.Location = new System.Drawing.Point(12, 55);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 5;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel.Size = new System.Drawing.Size(529, 147);
		this.tableLayoutPanel.TabIndex = 7;
		this.comboBoxCan2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.comboBoxCan2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxCan2.Enabled = false;
		this.comboBoxCan2.FormattingEnabled = true;
		this.comboBoxCan2.Location = new System.Drawing.Point(114, 33);
		this.comboBoxCan2.Name = "comboBoxCan2";
		this.comboBoxCan2.Size = new System.Drawing.Size(412, 24);
		this.comboBoxCan2.TabIndex = 4;
		this.comboBoxEthernet.Dock = System.Windows.Forms.DockStyle.Fill;
		this.comboBoxEthernet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxEthernet.Enabled = false;
		this.comboBoxEthernet.FormattingEnabled = true;
		this.comboBoxEthernet.Location = new System.Drawing.Point(114, 63);
		this.comboBoxEthernet.Name = "comboBoxEthernet";
		this.comboBoxEthernet.Size = new System.Drawing.Size(412, 24);
		this.comboBoxEthernet.TabIndex = 5;
		this.checkBoxCan1.AutoSize = true;
		this.checkBoxCan1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.checkBoxCan1.Enabled = false;
		this.checkBoxCan1.Location = new System.Drawing.Point(3, 3);
		this.checkBoxCan1.Name = "checkBoxCan1";
		this.checkBoxCan1.Size = new System.Drawing.Size(105, 24);
		this.checkBoxCan1.TabIndex = 7;
		this.checkBoxCan1.Text = "CAN1";
		this.checkBoxCan1.UseVisualStyleBackColor = true;
		this.checkBoxCan1.CheckedChanged += new System.EventHandler(checkBoxCan1_CheckedChanged);
		this.checkBoxCan2.AutoSize = true;
		this.checkBoxCan2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.checkBoxCan2.Enabled = false;
		this.checkBoxCan2.Location = new System.Drawing.Point(3, 33);
		this.checkBoxCan2.Name = "checkBoxCan2";
		this.checkBoxCan2.Size = new System.Drawing.Size(105, 24);
		this.checkBoxCan2.TabIndex = 8;
		this.checkBoxCan2.Text = "CAN2";
		this.checkBoxCan2.UseVisualStyleBackColor = true;
		this.checkBoxCan2.CheckedChanged += new System.EventHandler(checkBoxCan2_CheckedChanged);
		this.checkBoxEthernet.AutoSize = true;
		this.checkBoxEthernet.Dock = System.Windows.Forms.DockStyle.Fill;
		this.checkBoxEthernet.Enabled = false;
		this.checkBoxEthernet.Location = new System.Drawing.Point(3, 63);
		this.checkBoxEthernet.Name = "checkBoxEthernet";
		this.checkBoxEthernet.Size = new System.Drawing.Size(105, 24);
		this.checkBoxEthernet.TabIndex = 9;
		this.checkBoxEthernet.Text = "ETHERNET";
		this.checkBoxEthernet.UseVisualStyleBackColor = true;
		this.checkBoxEthernet.CheckedChanged += new System.EventHandler(checkBoxEthernet_CheckedChanged);
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(415, 212);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(126, 31);
		this.buttonOK.TabIndex = 8;
		this.buttonOK.Text = "OK";
		this.buttonOK.UseVisualStyleBackColor = true;
		this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label4.Location = new System.Drawing.Point(9, 9);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(532, 43);
		this.label4.TabIndex = 6;
		this.label4.Text = "Use the following specific connection resources for the specified physical interface links.";
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Window;
		base.ClientSize = new System.Drawing.Size(553, 255);
		base.Controls.Add(this.tableLayoutPanel);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.label4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(570, 300);
		base.Name = "BusMonitorSettingsForm";
		base.ShowIcon = false;
		this.Text = "Bus Monitor Settings";
		this.tableLayoutPanel.ResumeLayout(false);
		this.tableLayoutPanel.PerformLayout();
		base.ResumeLayout(false);
	}
}
