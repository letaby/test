using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel;

public class UserPanel : CustomPanel
{
	private const string WifiSsidPrefixQualifier = "WIFI_SSID_prefix";

	private string PadCharacter = char.ConvertFromUtf32(255);

	private List<Control> parameterControls = new List<Control>();

	private Channel ctp;

	private bool writingParameters = false;

	private bool dataChanged = false;

	private int parametersRead = 0;

	private Dictionary<string, string> eventInfos = new Dictionary<string, string>();

	private TableLayoutPanel tableLayoutPanelMain;

	private ScalingLabel scalingLabelStatus;

	private Checkmark checkmarkStatus;

	private Button buttonClose;

	private Button buttonSave;

	private TextBox textBoxSSID;

	private ComboBox comboBoxWifiMode;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label4;

	private TextBox textBoxWPA2KEY;

	private ComboBox comboBoxIpConfiguration;

	private string[] ParameterQualifiers => parameterControls.Select((Control pc) => (string)pc.Tag).ToArray();

	private bool CtpReady => ctp != null && ctp.CommunicationsState == CommunicationsState.Online;

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	public UserPanel()
	{
		InitializeComponent();
		foreach (Control control in ((TableLayoutPanel)(object)tableLayoutPanelMain).Controls)
		{
			if (!string.IsNullOrEmpty((string)control.Tag))
			{
				parameterControls.Add(control);
			}
		}
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && writingParameters)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			SetCTP(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetCTP(((CustomPanel)this).GetChannel("CTP01T"));
	}

	private void SetCTP(Channel ctp)
	{
		if (this.ctp != ctp)
		{
			parametersRead = 0;
			if (this.ctp != null)
			{
				this.ctp.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
				this.ctp.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersWriteCompleteEvent;
				this.ctp.CommunicationsStateUpdateEvent -= ctp_CommunicationsStateUpdateEvent;
			}
			this.ctp = ctp;
			if (this.ctp != null)
			{
				this.ctp.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
				this.ctp.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersWriteCompleteEvent;
				this.ctp.CommunicationsStateUpdateEvent += ctp_CommunicationsStateUpdateEvent;
				ReadParameters();
			}
		}
		UpdateUserInterface();
	}

	private void ctp_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void ReadParameters()
	{
		parametersRead = 0;
		string[] parameterQualifiers = ParameterQualifiers;
		foreach (string qualifier in parameterQualifiers)
		{
			Parameter parameter = ctp.Parameters[qualifier];
			if (parameter != null)
			{
				ctp.Parameters.ReadGroup(parameter.GroupQualifier, fromCache: true, synchronous: false);
			}
		}
	}

	private void UpdateUserInterface()
	{
		if (CtpReady)
		{
			((Control)(object)scalingLabelStatus).Text = Resources.Message_Ready;
			checkmarkStatus.CheckState = CheckState.Checked;
		}
		else if (writingParameters)
		{
			((Control)(object)scalingLabelStatus).Text = Resources.Message_WritingParameters;
			checkmarkStatus.CheckState = CheckState.Indeterminate;
		}
		else if (!CtpReady && ctp != null)
		{
			((Control)(object)scalingLabelStatus).Text = string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_CTPBusy0, ctp.CommunicationsState.ToString());
			checkmarkStatus.CheckState = CheckState.Unchecked;
		}
		else
		{
			((Control)(object)scalingLabelStatus).Text = Resources.Message_CTPBusy;
			checkmarkStatus.CheckState = CheckState.Unchecked;
		}
		buttonSave.Enabled = CtpReady && !writingParameters && dataChanged && parametersRead == parameterControls.Count;
		buttonClose.Enabled = ctp == null || !writingParameters;
	}

	private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		writingParameters = false;
		if (e.Succeeded)
		{
			dataChanged = false;
		}
		else
		{
			((Control)(object)scalingLabelStatus).Text = Resources.Message_ErrorWritingParameters;
		}
		AddStationLogEntry();
		UpdateUserInterface();
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		parametersRead++;
		string[] parameterQualifiers = ParameterQualifiers;
		foreach (string parameterQualifier in parameterQualifiers)
		{
			ParameterToControl(parameterQualifier);
		}
		dataChanged = false;
		UpdateUserInterface();
	}

	private void ParameterToControl(string parameterQualifier)
	{
		Control control = parameterControls.FirstOrDefault((Control c) => (string)c.Tag == parameterQualifier);
		Parameter parameter = ((ctp != null) ? ctp.Parameters[parameterQualifier] : null);
		if (parameter == null || parameter.Value == null || control == null)
		{
			return;
		}
		if (control.GetType() == typeof(ComboBox))
		{
			((ComboBox)control).Items.Clear();
			((ComboBox)control).Items.AddRange(parameter.Choices.Select((Choice p) => p.Name).ToArray());
			((ComboBox)control).SelectedIndex = ((Choice)parameter.Value).Index;
		}
		else if (control.GetType() == typeof(TextBox))
		{
			control.Text = parameter.Value.ToString().Replace(PadCharacter, string.Empty).TrimStart(' ');
		}
	}

	private void ControlToParameter(string parameterQualifier)
	{
		Control control = parameterControls.FirstOrDefault((Control c) => (string)c.Tag == parameterQualifier);
		Parameter parameter = ((ctp != null) ? ctp.Parameters[parameterQualifier] : null);
		if (parameter != null && control != null)
		{
			if (control.GetType() == typeof(ComboBox))
			{
				parameter.Value = parameter.Choices[((ComboBox)control).SelectedIndex];
				eventInfos.Add(parameter.Qualifier.Replace("_", string.Empty).Split('.').Last(), parameter.Value.ToString());
			}
			else if (control.GetType() == typeof(TextBox))
			{
				parameter.Value = control.Text.PadRight(32, PadCharacter[0]);
				eventInfos.Add(parameter.Qualifier.Replace("_", string.Empty).Split('.').Last(), control.Text);
			}
		}
	}

	private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		dataChanged = true;
		UpdateUserInterface();
	}

	private void textBox_TextChanged(object sender, EventArgs e)
	{
		dataChanged = true;
		UpdateUserInterface();
	}

	private void buttonSave_Click(object sender, EventArgs e)
	{
		if (CtpReady)
		{
			eventInfos.Clear();
			string[] parameterQualifiers = ParameterQualifiers;
			foreach (string parameterQualifier in parameterQualifiers)
			{
				ControlToParameter(parameterQualifier);
			}
			Parameter parameter = ((ctp != null) ? ctp.Parameters["WIFI_SSID_prefix"] : null);
			if (parameter != null)
			{
				parameter.Value = parameter.Value.ToString().PadRight(32, PadCharacter[0]);
			}
			writingParameters = true;
			ctp.Parameters.Write(synchronous: false);
		}
	}

	private void AddStationLogEntry()
	{
		eventInfos["reasontext"] = "ReasonCTP3GSundown";
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["CTP_Serial_Number"] = ReadEcuInfoData("CO_EcuSerialNumber");
		ServerDataManager.UpdateEventsFile(ctp, (IDictionary<string, string>)eventInfos, (IDictionary<string, string>)dictionary, "CTP3GSundown", string.Empty, ReadEcuInfoData("CO_VIN"), "OK", "DESCRIPTION", string.Empty);
	}

	private string ReadEcuInfoData(string qualifier)
	{
		string text = string.Empty;
		if (ctp != null)
		{
			EcuInfo ecuInfo = ctp.EcuInfos[qualifier];
			if (ecuInfo == null)
			{
				ecuInfo = ctp.EcuInfos.GetItemContaining(qualifier);
			}
			if (ecuInfo != null)
			{
				text = ecuInfo.Value.ToString().Trim();
			}
		}
		return text.Trim();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		scalingLabelStatus = new ScalingLabel();
		checkmarkStatus = new Checkmark();
		buttonClose = new Button();
		buttonSave = new Button();
		comboBoxIpConfiguration = new ComboBox();
		label1 = new System.Windows.Forms.Label();
		comboBoxWifiMode = new ComboBox();
		label2 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		textBoxSSID = new TextBox();
		textBoxWPA2KEY = new TextBox();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)scalingLabelStatus, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)checkmarkStatus, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 5, 6);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonSave, 4, 6);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(comboBoxIpConfiguration, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(comboBoxWifiMode, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label3, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label4, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxSSID, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxWPA2KEY, 2, 3);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		scalingLabelStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)scalingLabelStatus, 5);
		componentResourceManager.ApplyResources(scalingLabelStatus, "scalingLabelStatus");
		scalingLabelStatus.FontGroup = null;
		scalingLabelStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelStatus).Name = "scalingLabelStatus";
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		checkmarkStatus.IndeterminateImage = (Image)componentResourceManager.GetObject("checkmarkStatus.IndeterminateImage");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonSave, "buttonSave");
		buttonSave.Name = "buttonSave";
		buttonSave.UseCompatibleTextRendering = true;
		buttonSave.UseVisualStyleBackColor = true;
		buttonSave.Click += buttonSave_Click;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)comboBoxIpConfiguration, 4);
		componentResourceManager.ApplyResources(comboBoxIpConfiguration, "comboBoxIpConfiguration");
		comboBoxIpConfiguration.FormattingEnabled = true;
		comboBoxIpConfiguration.Name = "comboBoxIpConfiguration";
		comboBoxIpConfiguration.Tag = "VCD_PID_WIFI_BACKEND_MODE.IP_Configuration";
		comboBoxIpConfiguration.SelectedIndexChanged += comboBox_SelectedIndexChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)comboBoxWifiMode, 4);
		componentResourceManager.ApplyResources(comboBoxWifiMode, "comboBoxWifiMode");
		comboBoxWifiMode.FormattingEnabled = true;
		comboBoxWifiMode.Name = "comboBoxWifiMode";
		comboBoxWifiMode.Tag = "Wifi_mode";
		comboBoxWifiMode.SelectedIndexChanged += comboBox_SelectedIndexChanged;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		componentResourceManager.ApplyResources(label4, "label4");
		label4.Name = "label4";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)textBoxSSID, 4);
		componentResourceManager.ApplyResources(textBoxSSID, "textBoxSSID");
		textBoxSSID.Name = "textBoxSSID";
		textBoxSSID.Tag = "SSID";
		textBoxSSID.TextChanged += textBox_TextChanged;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)textBoxWPA2KEY, 4);
		componentResourceManager.ApplyResources(textBoxWPA2KEY, "textBoxWPA2KEY");
		textBoxWPA2KEY.Name = "textBoxWPA2KEY";
		textBoxWPA2KEY.Tag = "WPA2KEY";
		textBoxWPA2KEY.TextChanged += textBox_TextChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_3G_Sundown_Service_Campaign");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
