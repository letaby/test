using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Device_Status.panel;

public class UserPanel : CustomPanel
{
	private static string[] VinMasters = new string[3] { "CPC", "ICUC", "CGW" };

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
		InitializeComponent();
		comboBoxRequestType.SelectedIndex = 0;
	}

	private void UpdateUserInterface(bool isBusy)
	{
		buttonGetStatus.Enabled = !isBusy;
		comboBoxDeviceType.Enabled = !isBusy;
	}

	private void PopulateVin()
	{
		Channel channel = null;
		string[] vinMasters = VinMasters;
		foreach (string ecuName in vinMasters)
		{
			ChannelBaseCollection activeChannels = SapiManager.GlobalInstance.ActiveChannels;
			Func<Channel, bool> predicate = (Channel c) => c.Ecu.Name.StartsWith(ecuName);
			channel = activeChannels.FirstOrDefault(predicate);
			if (channel != null && channel.EcuInfos[VinReadServiceQualifier] != null)
			{
				textBoxVin.Text = channel.EcuInfos[VinReadServiceQualifier].Value;
				break;
			}
		}
	}

	private void PopulateDeviceId()
	{
		Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name.StartsWith(comboBoxDeviceType.Text));
		if (channel != null && channel.EcuInfos[EcuSerialNumber] != null)
		{
			textBoxDeviceId.Text = channel.EcuInfos[EcuSerialNumber].Value;
		}
	}

	public override void OnChannelsChanged()
	{
		PopulateDeviceTypeCombo();
		PopulateVin();
	}

	private void PopulateDeviceTypeCombo()
	{
		comboBoxDeviceType.Items.Clear();
		IEnumerable<Channel> enumerable = SapiManager.GlobalInstance.ActiveChannels.Where((Channel ecu) => !ecu.IsRollCall);
		foreach (Channel item in enumerable)
		{
			comboBoxDeviceType.Items.Add(item.Ecu.Name);
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void GetDeviceStatus()
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		ServerClient.GlobalInstance.StatusReturned += serverInstance_StatusReturned;
		labelStatus.Text = string.Empty;
		labelOperationStatus.Text = string.Empty;
		if (textBoxVin.Text != string.Empty && comboBoxRequestType.Text != string.Empty && comboBoxDeviceType.Text != string.Empty && textBoxDeviceId.Text != string.Empty)
		{
			UpdateUserInterface(isBusy: true);
			List<StatusItem> list = new List<StatusItem>();
			list.Add(new StatusItem(textBoxVin.Text, comboBoxRequestType.Text, comboBoxDeviceType.Text, textBoxDeviceId.Text, string.Empty));
			ServerClient.GlobalInstance.GetStatus((IEnumerable<StatusItem>)list);
		}
		else
		{
			MessageBox.Show(Resources.Message_AllFieldsMustBeFilledIn);
		}
	}

	private void buttonGetStatus_Click(object sender, EventArgs e)
	{
		GetDeviceStatus();
	}

	private void serverInstance_StatusReturned(object sender, StatusReturnedEventArgs e)
	{
		ServerClient.GlobalInstance.StatusReturned -= serverInstance_StatusReturned;
		if (e.Succeeded)
		{
			StatusResponse statusResponse = ServerDataManager.GlobalInstance.StatusResponse;
			List<StatusItem> list = statusResponse.StatusItems as List<StatusItem>;
			if (list.Count > 0)
			{
				labelStatus.Text = list[0].Status;
			}
		}
		else
		{
			labelOperationStatus.Text = e.ErrorMessage;
		}
		UpdateUserInterface(isBusy: false);
	}

	protected void comboBoxDeviceType_SelectedIndexChanged(object sender, EventArgs e)
	{
		PopulateDeviceId();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		comboBoxDeviceType = new ComboBox();
		label6 = new System.Windows.Forms.Label();
		label5 = new System.Windows.Forms.Label();
		textBoxVin = new TextBox();
		label4 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		buttonClose = new Button();
		buttonGetStatus = new Button();
		textBoxDeviceId = new TextBox();
		label1 = new System.Windows.Forms.Label();
		labelStatus = new System.Windows.Forms.Label();
		comboBoxRequestType = new ComboBox();
		labelOperationStatus = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(comboBoxDeviceType, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label6, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label5, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxVin, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label4, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonGetStatus, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxDeviceId, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(comboBoxRequestType, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelOperationStatus, 0, 5);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)comboBoxDeviceType, 2);
		comboBoxDeviceType.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxDeviceType.FormattingEnabled = true;
		componentResourceManager.ApplyResources(comboBoxDeviceType, "comboBoxDeviceType");
		comboBoxDeviceType.Name = "comboBoxDeviceType";
		comboBoxDeviceType.Sorted = true;
		comboBoxDeviceType.SelectedIndexChanged += comboBoxDeviceType_SelectedIndexChanged;
		componentResourceManager.ApplyResources(label6, "label6");
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxVin, 2);
		componentResourceManager.ApplyResources(textBoxVin, "textBoxVin");
		textBoxVin.Name = "textBoxVin";
		componentResourceManager.ApplyResources(label4, "label4");
		label4.Name = "label4";
		label4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonGetStatus, "buttonGetStatus");
		buttonGetStatus.Name = "buttonGetStatus";
		buttonGetStatus.UseCompatibleTextRendering = true;
		buttonGetStatus.UseVisualStyleBackColor = true;
		buttonGetStatus.Click += buttonGetStatus_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxDeviceId, 2);
		componentResourceManager.ApplyResources(textBoxDeviceId, "textBoxDeviceId");
		textBoxDeviceId.Name = "textBoxDeviceId";
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatus, 2);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)comboBoxRequestType, 2);
		comboBoxRequestType.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxRequestType.FormattingEnabled = true;
		comboBoxRequestType.Items.AddRange(new object[1] { componentResourceManager.GetString("comboBoxRequestType.Items") });
		componentResourceManager.ApplyResources(comboBoxRequestType, "comboBoxRequestType");
		comboBoxRequestType.Name = "comboBoxRequestType";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelOperationStatus, 3);
		componentResourceManager.ApplyResources(labelOperationStatus, "labelOperationStatus");
		labelOperationStatus.ForeColor = SystemColors.ControlText;
		labelOperationStatus.Name = "labelOperationStatus";
		labelOperationStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
