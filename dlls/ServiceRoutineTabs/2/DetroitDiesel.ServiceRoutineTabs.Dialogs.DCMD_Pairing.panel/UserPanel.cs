using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel;

public class UserPanel : CustomPanel
{
	private enum KeyfobInputBoxesState
	{
		InvalidKeyfob,
		DuplicateKeyfob,
		NoChange,
		ChangesOk
	}

	private enum ProcessState
	{
		NotRunning,
		RemoteSC,
		SendKeyfobIDs,
		HardReset,
		ReloadEcuinfo
	}

	private const string EmptyKeyfob = "00000000";

	private const string RemoteScServiceName = "DL_ID_Remote_SC";

	private const string RemoteScInputParameterRscByteName = "RSC_Byte_";

	private const string KeyfobIDsServiceName = "DL_ID_Keyfob_IDs";

	private const string HardResetServiceName = "FN_HardReset_physical";

	private ProcessState state = ProcessState.NotRunning;

	private Channel dcmd02t;

	private string[] KeyfobIDEcuInfoNames = new string[5] { "DT_STO_ID_Keyfob_IDs_KeyID_1", "DT_STO_ID_Keyfob_IDs_KeyID_2", "DT_STO_ID_Keyfob_IDs_KeyID_3", "DT_STO_ID_Keyfob_IDs_KeyID_4", "DT_STO_ID_Keyfob_IDs_KeyID_5" };

	private List<TextBox> keyfobNumberInputs = new List<TextBox>();

	private bool setFailed = false;

	private bool setSuccess = false;

	private CommunicationsState previousCommunicationsState = CommunicationsState.Offline;

	private string[] remoteScInputValues = new string[16]
	{
		"44", "54", "4E", "41", "4B", "45", "59", "46", "4F", "42",
		"53", "43", "44", "41", "54", "41"
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

	private bool ProcedureCanStart => dcmd02t != null && dcmd02t.CommunicationsState == CommunicationsState.Online && state == ProcessState.NotRunning && dcmd02t.Services["DL_ID_Keyfob_IDs"] != null;

	private bool ProcedureIsRunningOrHasRun => state != ProcessState.NotRunning || setFailed || setSuccess;

	public UserPanel()
	{
		InitializeComponent();
		keyfobNumberInputs.Add(textBoxKeyfob1);
		keyfobNumberInputs.Add(textBoxKeyfob2);
		keyfobNumberInputs.Add(textBoxKeyfob3);
		keyfobNumberInputs.Add(textBoxKeyfob4);
		keyfobNumberInputs.Add(textBoxKeyfob5);
	}

	private void LogMessage(string message)
	{
		if (labelSetMessage.Text != message)
		{
			labelSetMessage.Text = message;
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, message);
		}
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = dcmd02t != null && state != ProcessState.NotRunning;
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("DCMD02T", (ChannelLookupOptions)1));
	}

	private void SetChannel(Channel dcmd02t)
	{
		if (this.dcmd02t != dcmd02t)
		{
			previousCommunicationsState = CommunicationsState.Offline;
			if (this.dcmd02t != null)
			{
				this.dcmd02t.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.dcmd02t = dcmd02t;
			if (this.dcmd02t != null)
			{
				this.dcmd02t.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				previousCommunicationsState = this.dcmd02t.CommunicationsState;
			}
			state = ProcessState.NotRunning;
			PopoulateDefaultKeyfobs();
			setFailed = (setSuccess = false);
		}
		UpdateUI();
	}

	private void PopoulateDefaultKeyfobs()
	{
		for (int i = 0; i < keyfobNumberInputs.Count; i++)
		{
			keyfobNumberInputs[i].Text = GetEcuInofKeyfobValue(i);
		}
	}

	private string GetEcuInofKeyfobValue(int index)
	{
		return (dcmd02t == null || dcmd02t.EcuInfos[KeyfobIDEcuInfoNames[index]] == null) ? string.Empty : dcmd02t.EcuInfos[KeyfobIDEcuInfoNames[index]].Value;
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (previousCommunicationsState != CommunicationsState.ExecuteService && previousCommunicationsState != CommunicationsState.Online)
		{
			PopoulateDefaultKeyfobs();
		}
		previousCommunicationsState = ((dcmd02t != null) ? dcmd02t.CommunicationsState : CommunicationsState.Offline);
		UpdateUI();
	}

	private KeyfobInputBoxesState UpdateInputBoxes()
	{
		KeyfobInputBoxesState keyfobInputBoxesState = KeyfobInputBoxesState.NoChange;
		List<string> list = new List<string>();
		for (int i = 0; i < keyfobNumberInputs.Count; i++)
		{
			int result = 0;
			if (keyfobNumberInputs[i].Text.Length != 8 || !int.TryParse(keyfobNumberInputs[i].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
			{
				keyfobNumberInputs[i].BackColor = Color.LightPink;
				keyfobInputBoxesState = KeyfobInputBoxesState.InvalidKeyfob;
			}
			else if (!keyfobNumberInputs[i].Text.Equals("00000000", StringComparison.InvariantCultureIgnoreCase) && list.Contains(keyfobNumberInputs[i].Text))
			{
				keyfobNumberInputs[i].BackColor = Color.LightPink;
				keyfobInputBoxesState = KeyfobInputBoxesState.DuplicateKeyfob;
			}
			else if (keyfobNumberInputs[i].Text == GetEcuInofKeyfobValue(i))
			{
				keyfobNumberInputs[i].BackColor = SystemColors.Window;
			}
			else
			{
				keyfobNumberInputs[i].BackColor = Color.PaleGreen;
				if (keyfobInputBoxesState == KeyfobInputBoxesState.NoChange)
				{
					keyfobInputBoxesState = KeyfobInputBoxesState.ChangesOk;
				}
			}
			list.Add(keyfobNumberInputs[i].Text);
		}
		return keyfobInputBoxesState;
	}

	private void UpdateUI()
	{
		KeyfobInputBoxesState keyfobInputBoxesState = UpdateInputBoxes();
		buttonWriteValues.Enabled = ProcedureCanStart && keyfobInputBoxesState == KeyfobInputBoxesState.ChangesOk;
		if ((!ProcedureIsRunningOrHasRun && keyfobInputBoxesState == KeyfobInputBoxesState.ChangesOk) || dcmd02t == null)
		{
			LogMessage(buttonWriteValues.Enabled ? Resources.Message_TheProcedureCanStart : Resources.Message_PreconditionsNotMet);
			checkmarkCanStart.CheckState = (buttonWriteValues.Enabled ? CheckState.Checked : CheckState.Unchecked);
		}
		else if (!ProcedureIsRunningOrHasRun && keyfobInputBoxesState == KeyfobInputBoxesState.DuplicateKeyfob)
		{
			labelSetMessage.Text = Resources.Message_DuplicateKeyfob;
			checkmarkCanStart.CheckState = CheckState.Unchecked;
		}
		else if (!ProcedureIsRunningOrHasRun && keyfobInputBoxesState == KeyfobInputBoxesState.InvalidKeyfob)
		{
			labelSetMessage.Text = Resources.Message_InvalidKeyfob;
			checkmarkCanStart.CheckState = CheckState.Unchecked;
		}
		else if (!ProcedureIsRunningOrHasRun && keyfobInputBoxesState == KeyfobInputBoxesState.NoChange)
		{
			labelSetMessage.Text = Resources.Message_NoKeyfobsChanged;
			checkmarkCanStart.CheckState = CheckState.Unchecked;
		}
		else if (state == ProcessState.NotRunning)
		{
			checkmarkCanStart.CheckState = ((!setFailed) ? CheckState.Checked : CheckState.Unchecked);
		}
		buttonClose.Enabled = dcmd02t == null || state == ProcessState.NotRunning;
		buttonReadValues.Enabled = ProcedureCanStart;
		TextBox textBox = textBoxKeyfob1;
		TextBox textBox2 = textBoxKeyfob2;
		TextBox textBox3 = textBoxKeyfob3;
		TextBox textBox4 = textBoxKeyfob4;
		bool flag = (textBoxKeyfob5.Enabled = ProcedureCanStart);
		flag = (textBox4.Enabled = flag);
		flag = (textBox3.Enabled = flag);
		flag = (textBox2.Enabled = flag);
		textBox.Enabled = flag;
		Button button = buttonClear1;
		Button button2 = buttonClear2;
		Button button3 = buttonClear3;
		Button button4 = buttonClear4;
		flag = (buttonClear5.Enabled = ProcedureCanStart);
		flag = (button4.Enabled = flag);
		flag = (button3.Enabled = flag);
		flag = (button2.Enabled = flag);
		button.Enabled = flag;
	}

	private void buttonWriteValues_Click(object sender, EventArgs e)
	{
		state = ProcessState.RemoteSC;
		setSuccess = false;
		setFailed = false;
		GoMachine();
	}

	private void GoMachine()
	{
		switch (state)
		{
		case ProcessState.RemoteSC:
			SetRemoteSc();
			break;
		case ProcessState.SendKeyfobIDs:
			SendKeyfobIDs();
			break;
		case ProcessState.HardReset:
			ResetUnit();
			break;
		case ProcessState.ReloadEcuinfo:
			if (dcmd02t != null)
			{
				dcmd02t.EcuInfos.EcuInfosReadCompleteEvent += EcuInfos_EcuInfosReadCompleteEvent;
				dcmd02t.EcuInfos.Read(synchronous: false);
			}
			else
			{
				LogMessage(Resources.Message_CannotReloadEcuInfo);
				setFailed = true;
				state = ProcessState.NotRunning;
			}
			break;
		}
	}

	private void SetRemoteSc()
	{
		Service service;
		if (dcmd02t == null || (service = dcmd02t.Services["DL_ID_Remote_SC"]) == null)
		{
			state = ProcessState.NotRunning;
			setFailed = true;
			LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CanNotStart0, "DL_ID_Remote_SC"));
		}
		else
		{
			for (int i = 0; i < remoteScInputValues.Length; i++)
			{
				service.InputValues[string.Format(CultureInfo.InvariantCulture, "{0}{1}", "RSC_Byte_", i)].Value = remoteScInputValues[i];
			}
			service.ServiceCompleteEvent += setRemoteSc_ServiceCompleteEvent;
			LogMessage(Resources.Message_WritingValues);
			service.Execute(synchronous: false);
		}
		UpdateUI();
	}

	private void setRemoteSc_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		UpdateUI();
		state = ProcessState.NotRunning;
		if (service != null)
		{
			service.ServiceCompleteEvent -= setRemoteSc_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				state = ProcessState.SendKeyfobIDs;
				GoMachine();
				return;
			}
		}
		setFailed = true;
		LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0Failed, "DL_ID_Remote_SC"));
	}

	private void SendKeyfobIDs()
	{
		Service service;
		if (dcmd02t != null && (service = dcmd02t.Services["DL_ID_Keyfob_IDs"]) != null)
		{
			for (int i = 0; i < keyfobNumberInputs.Count; i++)
			{
				service.InputValues[i].Value = keyfobNumberInputs[i].Text;
			}
			service.ServiceCompleteEvent += sendKeyfobIDs_ServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
		else
		{
			state = ProcessState.NotRunning;
			setFailed = true;
			LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CanNotStart0, "DL_ID_Keyfob_IDs"));
			UpdateUI();
		}
	}

	private void sendKeyfobIDs_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		UpdateUI();
		state = ProcessState.NotRunning;
		if (service != null)
		{
			service.ServiceCompleteEvent -= sendKeyfobIDs_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				state = ProcessState.HardReset;
				LogMessage(Resources.Message_ResettingUnit);
				GoMachine();
				return;
			}
		}
		setFailed = true;
		LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0Failed, "DL_ID_Keyfob_IDs"));
	}

	private void ResetUnit()
	{
		Service service;
		if (dcmd02t != null && (service = dcmd02t.Services["FN_HardReset_physical"]) != null)
		{
			service.ServiceCompleteEvent += hardReset_ServiceCompleteEvent;
			service.Execute(synchronous: false);
			return;
		}
		state = ProcessState.NotRunning;
		setFailed = true;
		LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CanNotStart0, "FN_HardReset_physical"));
		UpdateUI();
	}

	private void hardReset_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		UpdateUI();
		if (service != null)
		{
			service.ServiceCompleteEvent -= hardReset_ServiceCompleteEvent;
			if (dcmd02t != null)
			{
				LogMessage(Resources.Message_ReadingValues);
				state = ProcessState.ReloadEcuinfo;
				GoMachine();
				return;
			}
		}
		setFailed = true;
		state = ProcessState.NotRunning;
		LogMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0Failed, "FN_HardReset_physical"));
	}

	private void EcuInfos_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
	{
		dcmd02t.EcuInfos.EcuInfosReadCompleteEvent -= EcuInfos_EcuInfosReadCompleteEvent;
		state = ProcessState.NotRunning;
		PopoulateDefaultKeyfobs();
		setFailed = false;
		setSuccess = true;
		LogMessage(Resources.Message_ValuesWritten);
		UpdateUI();
	}

	private void buttonReadValues_Click(object sender, EventArgs e)
	{
		PopoulateDefaultKeyfobs();
		LogMessage(Resources.Message_ValuesRead);
	}

	private void textBoxKeyfob_KeyPress(object sender, KeyPressEventArgs e)
	{
		e.KeyChar = char.ToUpperInvariant(e.KeyChar);
		if (!char.IsControl(e.KeyChar) && !Regex.IsMatch(e.KeyChar.ToString(), "[0-9A-F]"))
		{
			e.Handled = true;
		}
		setSuccess = (setFailed = false);
	}

	private void buttonClear_Click(object sender, EventArgs e)
	{
		if (sender is Button button)
		{
			keyfobNumberInputs[Convert.ToInt32(button.Tag)].Text = "00000000";
			setSuccess = (setFailed = false);
		}
		UpdateUI();
	}

	private void textBoxKeyfob_TextChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		labelFobNumber = new System.Windows.Forms.Label();
		textBoxKeyfob1 = new TextBox();
		labelCurrentPairedFob = new System.Windows.Forms.Label();
		textBoxKeyfob2 = new TextBox();
		textBoxKeyfob3 = new TextBox();
		textBoxKeyfob4 = new TextBox();
		textBoxKeyfob5 = new TextBox();
		labelFobNumber1 = new System.Windows.Forms.Label();
		labelFobNumber2 = new System.Windows.Forms.Label();
		labelFobNumber3 = new System.Windows.Forms.Label();
		labelFobNumber4 = new System.Windows.Forms.Label();
		labelFobNumber5 = new System.Windows.Forms.Label();
		buttonClear1 = new Button();
		buttonClear2 = new Button();
		buttonClear3 = new Button();
		buttonClear4 = new Button();
		buttonClear5 = new Button();
		buttonReadValues = new Button();
		tableLayoutPanel1 = new TableLayoutPanel();
		buttonWriteValues = new Button();
		checkmarkCanStart = new Checkmark();
		labelSetMessage = new System.Windows.Forms.Label();
		seekTimeListView = new SeekTimeListView();
		buttonClose = new Button();
		digitalReadoutInstrumentRKEUnlock = new DigitalReadoutInstrument();
		digitalReadoutInstrumentRKEButton3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentRKELock = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelFobNumber, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxKeyfob1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelCurrentPairedFob, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxKeyfob2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxKeyfob3, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxKeyfob4, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxKeyfob5, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelFobNumber1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelFobNumber2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelFobNumber3, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelFobNumber4, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelFobNumber5, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClear1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClear2, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClear3, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClear4, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClear5, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonReadValues, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanel1, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListView, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 3, 8);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentRKEUnlock, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentRKEButton3, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentRKELock, 3, 4);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(labelFobNumber, "labelFobNumber");
		labelFobNumber.Name = "labelFobNumber";
		labelFobNumber.UseCompatibleTextRendering = true;
		textBoxKeyfob1.BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(textBoxKeyfob1, "textBoxKeyfob1");
		textBoxKeyfob1.Name = "textBoxKeyfob1";
		textBoxKeyfob1.Tag = "0";
		textBoxKeyfob1.TextChanged += textBoxKeyfob_TextChanged;
		textBoxKeyfob1.KeyPress += textBoxKeyfob_KeyPress;
		componentResourceManager.ApplyResources(labelCurrentPairedFob, "labelCurrentPairedFob");
		labelCurrentPairedFob.Name = "labelCurrentPairedFob";
		labelCurrentPairedFob.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxKeyfob2, "textBoxKeyfob2");
		textBoxKeyfob2.Name = "textBoxKeyfob2";
		textBoxKeyfob2.TextChanged += textBoxKeyfob_TextChanged;
		textBoxKeyfob2.KeyPress += textBoxKeyfob_KeyPress;
		componentResourceManager.ApplyResources(textBoxKeyfob3, "textBoxKeyfob3");
		textBoxKeyfob3.Name = "textBoxKeyfob3";
		textBoxKeyfob3.TextChanged += textBoxKeyfob_TextChanged;
		textBoxKeyfob3.KeyPress += textBoxKeyfob_KeyPress;
		componentResourceManager.ApplyResources(textBoxKeyfob4, "textBoxKeyfob4");
		textBoxKeyfob4.Name = "textBoxKeyfob4";
		textBoxKeyfob4.TextChanged += textBoxKeyfob_TextChanged;
		textBoxKeyfob4.KeyPress += textBoxKeyfob_KeyPress;
		componentResourceManager.ApplyResources(textBoxKeyfob5, "textBoxKeyfob5");
		textBoxKeyfob5.Name = "textBoxKeyfob5";
		textBoxKeyfob5.TextChanged += textBoxKeyfob_TextChanged;
		textBoxKeyfob5.KeyPress += textBoxKeyfob_KeyPress;
		componentResourceManager.ApplyResources(labelFobNumber1, "labelFobNumber1");
		labelFobNumber1.Name = "labelFobNumber1";
		labelFobNumber1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelFobNumber2, "labelFobNumber2");
		labelFobNumber2.Name = "labelFobNumber2";
		labelFobNumber2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelFobNumber3, "labelFobNumber3");
		labelFobNumber3.Name = "labelFobNumber3";
		labelFobNumber3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelFobNumber4, "labelFobNumber4");
		labelFobNumber4.Name = "labelFobNumber4";
		labelFobNumber4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelFobNumber5, "labelFobNumber5");
		labelFobNumber5.Name = "labelFobNumber5";
		labelFobNumber5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonClear1, "buttonClear1");
		buttonClear1.Name = "buttonClear1";
		buttonClear1.Tag = "0";
		buttonClear1.UseCompatibleTextRendering = true;
		buttonClear1.UseVisualStyleBackColor = true;
		buttonClear1.Click += buttonClear_Click;
		componentResourceManager.ApplyResources(buttonClear2, "buttonClear2");
		buttonClear2.Name = "buttonClear2";
		buttonClear2.Tag = "1";
		buttonClear2.UseCompatibleTextRendering = true;
		buttonClear2.UseVisualStyleBackColor = true;
		buttonClear2.Click += buttonClear_Click;
		componentResourceManager.ApplyResources(buttonClear3, "buttonClear3");
		buttonClear3.Name = "buttonClear3";
		buttonClear3.Tag = "2";
		buttonClear3.UseCompatibleTextRendering = true;
		buttonClear3.UseVisualStyleBackColor = true;
		buttonClear3.Click += buttonClear_Click;
		componentResourceManager.ApplyResources(buttonClear4, "buttonClear4");
		buttonClear4.Name = "buttonClear4";
		buttonClear4.Tag = "3";
		buttonClear4.UseCompatibleTextRendering = true;
		buttonClear4.UseVisualStyleBackColor = true;
		buttonClear4.Click += buttonClear_Click;
		componentResourceManager.ApplyResources(buttonClear5, "buttonClear5");
		buttonClear5.Name = "buttonClear5";
		buttonClear5.Tag = "4";
		buttonClear5.UseCompatibleTextRendering = true;
		buttonClear5.UseVisualStyleBackColor = true;
		buttonClear5.Click += buttonClear_Click;
		componentResourceManager.ApplyResources(buttonReadValues, "buttonReadValues");
		buttonReadValues.Name = "buttonReadValues";
		buttonReadValues.UseCompatibleTextRendering = true;
		buttonReadValues.UseVisualStyleBackColor = true;
		buttonReadValues.Click += buttonReadValues_Click;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanel1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonWriteValues, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkCanStart, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelSetMessage, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(buttonWriteValues, "buttonWriteValues");
		buttonWriteValues.Name = "buttonWriteValues";
		buttonWriteValues.UseCompatibleTextRendering = true;
		buttonWriteValues.UseVisualStyleBackColor = true;
		buttonWriteValues.Click += buttonWriteValues_Click;
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(labelSetMessage, "labelSetMessage");
		labelSetMessage.Name = "labelSetMessage";
		labelSetMessage.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)seekTimeListView, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "DCMD Keyfob Pairing";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRKEUnlock, "digitalReadoutInstrumentRKEUnlock");
		digitalReadoutInstrumentRKEUnlock.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentRKEUnlock).FreezeValue = false;
		digitalReadoutInstrumentRKEUnlock.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentRKEUnlock.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentRKEUnlock.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentRKEUnlock).Instrument = new Qualifier((QualifierTypes)1, "DCMD02T", "DT_RKE_Button_3_IN_RKE_Button3");
		((Control)(object)digitalReadoutInstrumentRKEUnlock).Name = "digitalReadoutInstrumentRKEUnlock";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)digitalReadoutInstrumentRKEUnlock, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentRKEUnlock).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRKEButton3, "digitalReadoutInstrumentRKEButton3");
		digitalReadoutInstrumentRKEButton3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentRKEButton3).FreezeValue = false;
		digitalReadoutInstrumentRKEButton3.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentRKEButton3.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentRKEButton3.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentRKEButton3).Instrument = new Qualifier((QualifierTypes)1, "DCMD02T", "DT_RKE_Button_1_IN_RKE_Button1");
		((Control)(object)digitalReadoutInstrumentRKEButton3).Name = "digitalReadoutInstrumentRKEButton3";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)digitalReadoutInstrumentRKEButton3, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentRKEButton3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRKELock, "digitalReadoutInstrumentRKELock");
		digitalReadoutInstrumentRKELock.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentRKELock).FreezeValue = false;
		digitalReadoutInstrumentRKELock.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentRKELock.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentRKELock.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentRKELock).Instrument = new Qualifier((QualifierTypes)1, "DCMD02T", "DT_RKE_Button_2_IN_RKE_Button2");
		((Control)(object)digitalReadoutInstrumentRKELock).Name = "digitalReadoutInstrumentRKELock";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)digitalReadoutInstrumentRKELock, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentRKELock).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
