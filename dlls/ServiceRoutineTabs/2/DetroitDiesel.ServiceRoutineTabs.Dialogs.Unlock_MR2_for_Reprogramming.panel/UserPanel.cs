using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_MR2_for_Reprogramming.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		Unknown,
		Initializing,
		ReadingInputs,
		WaitingForKey,
		ReadyToUnlock,
		Unlocking,
		ReadingLockConfiguration,
		RefreshEcuData,
		Done,
		Closing
	}

	private const string getEcuSerialNumber = "DJ_Get_ECU_Serial_Number";

	private const string readAut64Challenge = "DT_STO_ID_AUT64_Challenge_challenge";

	private const string readTransponderCode = "DT_STO_ID_Transponder_Code_TranspCode";

	private const string readNumberOfTransponderCode = "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s";

	private const string readFuelmapStatus = "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status";

	private const string securityAccessRoutine = "DJ_SecurityAccess_Routine_1";

	private const string unlockMr2ForReprogrammingService = "RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State";

	private const string defaultMR2CalculationType = "X7 - Unlock flash memory";

	private const string unlockXOption = "No X-Option";

	private const string checksum = "XXX";

	private const string MR201T = "MR201T";

	private Channel selectedChannel;

	private State currentState;

	private TableLayoutPanel tableLayoutPanel1;

	private Button UnlockButton;

	private TextBox IdCode1TextBox;

	private TextBox IdCode2TextBox;

	private TextBox IdCode3TextBox;

	private TextBox IdCode4TextBox;

	private TextBox TspCode1TextBox;

	private TextBox TspCode2TextBox;

	private TextBox TspCode3TextBox;

	private TextBox TspCode4TextBox;

	private TextBox TspCode5TextBox;

	private TextBox NKeysTextBox;

	private TextBox RandomNumber1TextBox;

	private TextBox RandomNumber2TextBox;

	private ComboBox XFactorComboBox;

	private TextBox Aut64Key1TextBox;

	private TextBox Aut64Key2TextBox;

	private TextBox Aut64Key3TextBox;

	private TextBox Aut64Key4TextBox;

	private Button RefreshInputsButton;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label4;

	private System.Windows.Forms.Label label5;

	private System.Windows.Forms.Label label6;

	private SeekTimeListView seekTimeListView1;

	private System.Windows.Forms.Label label8;

	private TextBox LockConfigurationTextBox;

	private System.Windows.Forms.Label label9;

	private TextBox ChecksumTextBox;

	private Button CloseButton;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		SetState(State.Initializing);
		SetChannel(((CustomPanel)this).GetChannel("MR201T", (ChannelLookupOptions)5));
		PopulateComboBoxes();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		if (currentState != State.Closing)
		{
			SetChannel(((CustomPanel)this).GetChannel("MR201T", (ChannelLookupOptions)5));
		}
	}

	private void SetChannel(Channel channel)
	{
		if (channel != selectedChannel)
		{
			if (channel == null && currentState != State.Closing)
			{
				SetState(State.Unknown);
				ClearInputs();
				ClearKeys();
			}
			selectedChannel = channel;
			if (selectedChannel != null)
			{
				PopulateComboBoxes();
				ReadVeDocInputs();
			}
		}
	}

	private void PopulateComboBoxes()
	{
		XFactorComboBox.Items.Clear();
		if (selectedChannel != null)
		{
			Service service = selectedChannel.Services["RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State"];
			if (!(service != null) || service.InputValues.Count <= 0)
			{
				return;
			}
			ChoiceCollection choices = service.InputValues[0].Choices;
			foreach (Choice item in choices)
			{
				XFactorComboBox.Items.Add(item.Name);
			}
			XFactorComboBox.SelectedItem = "X7 - Unlock flash memory";
		}
		else
		{
			SetState(State.Unknown);
		}
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		switch (currentState)
		{
		case State.Initializing:
		case State.ReadingInputs:
		case State.Unlocking:
		case State.ReadingLockConfiguration:
			e.Cancel = true;
			break;
		}
		if (!e.Cancel)
		{
			SetState(State.Closing);
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			SetChannel(null);
		}
	}

	private void SetState(State newState)
	{
		if (newState != currentState)
		{
			currentState = newState;
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		switch (currentState)
		{
		default:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = true;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Unknown);
			break;
		case State.Initializing:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InitializingWait);
			break;
		case State.ReadingInputs:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingInputsWait);
			break;
		case State.WaitingForKey:
			XFactorComboBox.Enabled = true;
			RefreshInputsButton.Enabled = true;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = true;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_PleaseTypeInVeDocSKey);
			break;
		case State.ReadyToUnlock:
			XFactorComboBox.Enabled = true;
			RefreshInputsButton.Enabled = true;
			UnlockButton.Enabled = true;
			CloseButton.Enabled = true;
			break;
		case State.Unlocking:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockingWait0);
			break;
		case State.ReadingLockConfiguration:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingLockConfigurationWait);
			break;
		case State.RefreshEcuData:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_RefreshingEcuData);
			break;
		case State.Done:
			XFactorComboBox.Enabled = true;
			RefreshInputsButton.Enabled = true;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = true;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Done);
			break;
		case State.Closing:
			XFactorComboBox.Enabled = false;
			RefreshInputsButton.Enabled = false;
			UnlockButton.Enabled = false;
			CloseButton.Enabled = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Closing);
			break;
		}
	}

	private void Aut64KeyTextBox_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (char.IsLetter(e.KeyChar) || char.IsSymbol(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || char.IsPunctuation(e.KeyChar))
		{
			e.Handled = true;
		}
	}

	private void Aut64Key1TextBox_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(Aut64Key1TextBox) && Aut64Key1TextBox.Text.Length == 3)
		{
			Aut64Key2TextBox.Focus();
			Aut64Key2TextBox.SelectAll();
		}
	}

	private void Aut64Key2TextBox_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(Aut64Key2TextBox) && Aut64Key2TextBox.Text.Length == 3)
		{
			Aut64Key3TextBox.Focus();
			Aut64Key3TextBox.SelectAll();
		}
	}

	private void Aut64Key3TextBox_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(Aut64Key3TextBox) && Aut64Key3TextBox.Text.Length == 3)
		{
			Aut64Key4TextBox.Focus();
			Aut64Key4TextBox.SelectAll();
		}
	}

	private void Aut64Key4TextBox_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(Aut64Key4TextBox) && Aut64Key4TextBox.Text.Length == 3)
		{
			UnlockButton.Focus();
		}
	}

	private bool checkAut64KeyTextBox(TextBox textBox)
	{
		if (!checkKeyTextBox(textBox))
		{
			SetState(State.WaitingForKey);
			textBox.Focus();
			return false;
		}
		if (checkAllKeys())
		{
			SetState(State.ReadyToUnlock);
		}
		else
		{
			SetState(State.WaitingForKey);
		}
		return true;
	}

	private bool checkAllKeys()
	{
		if (!checkKeyTextBox(Aut64Key1TextBox))
		{
			return false;
		}
		if (!checkKeyTextBox(Aut64Key2TextBox))
		{
			return false;
		}
		if (!checkKeyTextBox(Aut64Key3TextBox))
		{
			return false;
		}
		if (!checkKeyTextBox(Aut64Key4TextBox))
		{
			return false;
		}
		return true;
	}

	private bool checkKeyTextBox(TextBox textBox)
	{
		if (textBox.Text.Length < 1)
		{
			return false;
		}
		if (textBox.Text.Length > 3)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range);
			textBox.SelectAll();
			return false;
		}
		byte result;
		bool flag = byte.TryParse(textBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
		if (!flag)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range);
			textBox.SelectAll();
		}
		return flag;
	}

	private void RefreshInputsButton_Click(object sender, EventArgs e)
	{
		ReadVeDocInputs();
	}

	private void ReadVeDocInputs()
	{
		if (selectedChannel != null)
		{
			selectedChannel.Instruments.AutoRead = false;
			SetState(State.ReadingInputs);
			Service service = selectedChannel.Services["DJ_Get_ECU_Serial_Number"];
			if (service != null)
			{
				service.ServiceCompleteEvent += getEcuSerialNumberService_ServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
		}
	}

	private void getEcuSerialNumberService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= getEcuSerialNumberService_ServiceCompleteEvent;
		ClearInputs();
		ClearKeys();
		if (e.Succeeded && service.OutputValues != null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = service.OutputValues[0].Value.ToString();
			if (text.Length >= 8)
			{
				IdCode1TextBox.Text = text.Substring(0, 2);
				IdCode2TextBox.Text = text.Substring(2, 2);
				IdCode3TextBox.Text = text.Substring(4, 2);
				IdCode4TextBox.Text = text.Substring(6, 2);
			}
			SetMarkedEcuInfoItems();
			selectedChannel.EcuInfos.EcuInfosReadCompleteEvent += selectedChannel_EcuInfosReadCompleteEvent;
			selectedChannel.EcuInfos.Read(synchronous: false);
		}
		else
		{
			ClearInputs();
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailureReadingVeDocInputs);
			SetState(State.Done);
		}
	}

	private void selectedChannel_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
	{
		selectedChannel.EcuInfos.EcuInfosReadCompleteEvent -= selectedChannel_EcuInfosReadCompleteEvent;
		if (e.Succeeded)
		{
			string ecuInfoData = GetEcuInfoData(selectedChannel, "DT_STO_ID_AUT64_Challenge_challenge");
			string ecuInfoData2 = GetEcuInfoData(selectedChannel, "DT_STO_ID_Transponder_Code_TranspCode");
			string ecuInfoData3 = GetEcuInfoData(selectedChannel, "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s");
			if (ecuInfoData.Length >= 4 && ecuInfoData2.Length >= 10)
			{
				RandomNumber1TextBox.Text = ecuInfoData.Substring(0, 2);
				RandomNumber2TextBox.Text = ecuInfoData.Substring(2, 2);
				NKeysTextBox.Text = ecuInfoData3;
				TspCode1TextBox.Text = ecuInfoData2.Substring(0, 2);
				TspCode2TextBox.Text = ecuInfoData2.Substring(2, 2);
				TspCode3TextBox.Text = ecuInfoData2.Substring(4, 2);
				TspCode4TextBox.Text = ecuInfoData2.Substring(6, 2);
				TspCode5TextBox.Text = ecuInfoData2.Substring(8, 2);
				ChecksumTextBox.Text = "XXX";
				ReadLockConfiguration();
				SetState(State.WaitingForKey);
				Aut64Key1TextBox.Focus();
				return;
			}
		}
		ClearInputs();
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailureReadingVeDocInputs);
		SetState(State.Done);
	}

	private void SetMarkedEcuInfoItems()
	{
		IEnumerable<string> source = new string[4] { "DT_STO_ID_AUT64_Challenge_challenge", "DT_STO_ID_Transponder_Code_TranspCode", "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s", "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status" };
		foreach (EcuInfo ecuInfo in selectedChannel.EcuInfos)
		{
			ecuInfo.Marked = source.Contains(ecuInfo.Qualifier);
		}
	}

	private void ReadLockConfiguration()
	{
		if (selectedChannel != null)
		{
			SetState(State.ReadingLockConfiguration);
			LockConfigurationTextBox.Text = GetEcuInfoData(selectedChannel, "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status");
		}
	}

	private string GetEcuInfoData(Channel channel, string qualifier)
	{
		string text = string.Empty;
		if (channel != null)
		{
			EcuInfo ecuInfo = channel.EcuInfos[qualifier];
			if (ecuInfo != null)
			{
				text = ecuInfo.Value.ToString();
			}
		}
		return text.Trim();
	}

	private void ClearInputs()
	{
		IdCode1TextBox.Text = "";
		IdCode2TextBox.Text = "";
		IdCode3TextBox.Text = "";
		IdCode4TextBox.Text = "";
		TspCode1TextBox.Text = "";
		TspCode2TextBox.Text = "";
		TspCode3TextBox.Text = "";
		TspCode4TextBox.Text = "";
		TspCode5TextBox.Text = "";
		NKeysTextBox.Text = "";
		RandomNumber1TextBox.Text = "";
		RandomNumber2TextBox.Text = "";
		ChecksumTextBox.Text = "";
	}

	private void ClearKeys()
	{
		Aut64Key1TextBox.Text = "";
		Aut64Key2TextBox.Text = "";
		Aut64Key3TextBox.Text = "";
		Aut64Key4TextBox.Text = "";
	}

	private void UnlockButton_Click(object sender, EventArgs e)
	{
		Service service = selectedChannel.Services["DJ_SecurityAccess_Routine_1"];
		if (service != null)
		{
			service.ServiceCompleteEvent += securityAccessService_ServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
	}

	private void securityAccessService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= securityAccessService_ServiceCompleteEvent;
		if (e.Succeeded && service != null)
		{
			UnlockMR2ForReprogramming();
			return;
		}
		LockConfigurationTextBox.Text = e.Exception.Message;
		ClearInputs();
		ClearKeys();
		SetState(State.Unknown);
	}

	private void UnlockMR2ForReprogramming()
	{
		if (selectedChannel != null)
		{
			SetState(State.Unlocking);
			Service service = selectedChannel.Services["RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State"];
			if (service != null)
			{
				service.InputValues[0].Value = XFactorComboBox.Text;
				service.InputValues[1].Value = "No X-Option";
				service.InputValues[2].Value = Aut64Key1TextBox.Text;
				service.InputValues[3].Value = Aut64Key2TextBox.Text;
				service.InputValues[4].Value = Aut64Key3TextBox.Text;
				service.InputValues[5].Value = Aut64Key4TextBox.Text;
				service.ServiceCompleteEvent += unlockMr2Service_ServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
		}
	}

	private void unlockMr2Service_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= unlockMr2Service_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			SetMarkedEcuInfoItems();
			selectedChannel.EcuInfos.EcuInfosReadCompleteEvent += selectedChannelEcuInfos_EcuInfosReadCompleteEvent;
			selectedChannel.EcuInfos.Read(synchronous: false);
			SetState(State.RefreshEcuData);
		}
		else
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorUnlocking + e.Exception.Message);
			SetState(State.Done);
		}
	}

	private void selectedChannelEcuInfos_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
	{
		selectedChannel.EcuInfos.EcuInfosReadCompleteEvent -= selectedChannelEcuInfos_EcuInfosReadCompleteEvent;
		if (e.Succeeded)
		{
			ReadLockConfiguration();
			SetState(State.Done);
		}
		else
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ErrorUnlocking + e.Exception.Message);
			SetState(State.Done);
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		XFactorComboBox = new ComboBox();
		seekTimeListView1 = new SeekTimeListView();
		label6 = new System.Windows.Forms.Label();
		Aut64Key1TextBox = new TextBox();
		Aut64Key2TextBox = new TextBox();
		Aut64Key4TextBox = new TextBox();
		label5 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		RandomNumber1TextBox = new TextBox();
		RandomNumber2TextBox = new TextBox();
		NKeysTextBox = new TextBox();
		label3 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		TspCode1TextBox = new TextBox();
		TspCode2TextBox = new TextBox();
		TspCode3TextBox = new TextBox();
		TspCode4TextBox = new TextBox();
		TspCode5TextBox = new TextBox();
		label1 = new System.Windows.Forms.Label();
		IdCode1TextBox = new TextBox();
		IdCode2TextBox = new TextBox();
		IdCode3TextBox = new TextBox();
		IdCode4TextBox = new TextBox();
		label8 = new System.Windows.Forms.Label();
		LockConfigurationTextBox = new TextBox();
		CloseButton = new Button();
		label9 = new System.Windows.Forms.Label();
		ChecksumTextBox = new TextBox();
		Aut64Key3TextBox = new TextBox();
		UnlockButton = new Button();
		RefreshInputsButton = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(XFactorComboBox, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label6, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(Aut64Key1TextBox, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(Aut64Key2TextBox, 3, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(Aut64Key4TextBox, 5, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label5, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(RandomNumber1TextBox, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(RandomNumber2TextBox, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(NKeysTextBox, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label3, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(TspCode1TextBox, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(TspCode2TextBox, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(TspCode3TextBox, 4, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(TspCode4TextBox, 5, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(TspCode5TextBox, 6, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(IdCode1TextBox, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(IdCode2TextBox, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(IdCode3TextBox, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(IdCode4TextBox, 5, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label8, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(LockConfigurationTextBox, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(CloseButton, 5, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label9, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(ChecksumTextBox, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(Aut64Key3TextBox, 4, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(UnlockButton, 1, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(RefreshInputsButton, 3, 9);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(XFactorComboBox, "XFactorComboBox");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)XFactorComboBox, 4);
		XFactorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		XFactorComboBox.FormattingEnabled = true;
		XFactorComboBox.Name = "XFactorComboBox";
		XFactorComboBox.Tag = "";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 7);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		((Control)(object)seekTimeListView1).TabStop = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label6, 2);
		componentResourceManager.ApplyResources(label6, "label6");
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(Aut64Key1TextBox, "Aut64Key1TextBox");
		Aut64Key1TextBox.Name = "Aut64Key1TextBox";
		Aut64Key1TextBox.TextChanged += Aut64Key1TextBox_TextChanged;
		Aut64Key1TextBox.KeyPress += Aut64KeyTextBox_KeyPress;
		componentResourceManager.ApplyResources(Aut64Key2TextBox, "Aut64Key2TextBox");
		Aut64Key2TextBox.Name = "Aut64Key2TextBox";
		Aut64Key2TextBox.TextChanged += Aut64Key2TextBox_TextChanged;
		Aut64Key2TextBox.KeyPress += Aut64KeyTextBox_KeyPress;
		componentResourceManager.ApplyResources(Aut64Key4TextBox, "Aut64Key4TextBox");
		Aut64Key4TextBox.Name = "Aut64Key4TextBox";
		Aut64Key4TextBox.TextChanged += Aut64Key4TextBox_TextChanged;
		Aut64Key4TextBox.KeyPress += Aut64KeyTextBox_KeyPress;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label5, 2);
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label4, 2);
		componentResourceManager.ApplyResources(label4, "label4");
		label4.Name = "label4";
		label4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(RandomNumber1TextBox, "RandomNumber1TextBox");
		RandomNumber1TextBox.Name = "RandomNumber1TextBox";
		RandomNumber1TextBox.ReadOnly = true;
		RandomNumber1TextBox.TabStop = false;
		componentResourceManager.ApplyResources(RandomNumber2TextBox, "RandomNumber2TextBox");
		RandomNumber2TextBox.Name = "RandomNumber2TextBox";
		RandomNumber2TextBox.ReadOnly = true;
		RandomNumber2TextBox.TabStop = false;
		componentResourceManager.ApplyResources(NKeysTextBox, "NKeysTextBox");
		NKeysTextBox.Name = "NKeysTextBox";
		NKeysTextBox.ReadOnly = true;
		NKeysTextBox.TabStop = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label3, 2);
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		label3.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label2, 2);
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(TspCode1TextBox, "TspCode1TextBox");
		TspCode1TextBox.Name = "TspCode1TextBox";
		TspCode1TextBox.ReadOnly = true;
		TspCode1TextBox.TabStop = false;
		componentResourceManager.ApplyResources(TspCode2TextBox, "TspCode2TextBox");
		TspCode2TextBox.Name = "TspCode2TextBox";
		TspCode2TextBox.ReadOnly = true;
		TspCode2TextBox.TabStop = false;
		componentResourceManager.ApplyResources(TspCode3TextBox, "TspCode3TextBox");
		TspCode3TextBox.Name = "TspCode3TextBox";
		TspCode3TextBox.ReadOnly = true;
		TspCode3TextBox.TabStop = false;
		componentResourceManager.ApplyResources(TspCode4TextBox, "TspCode4TextBox");
		TspCode4TextBox.Name = "TspCode4TextBox";
		TspCode4TextBox.ReadOnly = true;
		TspCode4TextBox.TabStop = false;
		componentResourceManager.ApplyResources(TspCode5TextBox, "TspCode5TextBox");
		TspCode5TextBox.Name = "TspCode5TextBox";
		TspCode5TextBox.ReadOnly = true;
		TspCode5TextBox.TabStop = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label1, 2);
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(IdCode1TextBox, "IdCode1TextBox");
		IdCode1TextBox.Name = "IdCode1TextBox";
		IdCode1TextBox.ReadOnly = true;
		IdCode1TextBox.TabStop = false;
		componentResourceManager.ApplyResources(IdCode2TextBox, "IdCode2TextBox");
		IdCode2TextBox.Name = "IdCode2TextBox";
		IdCode2TextBox.ReadOnly = true;
		IdCode2TextBox.TabStop = false;
		componentResourceManager.ApplyResources(IdCode3TextBox, "IdCode3TextBox");
		IdCode3TextBox.Name = "IdCode3TextBox";
		IdCode3TextBox.ReadOnly = true;
		IdCode3TextBox.TabStop = false;
		componentResourceManager.ApplyResources(IdCode4TextBox, "IdCode4TextBox");
		IdCode4TextBox.Name = "IdCode4TextBox";
		IdCode4TextBox.ReadOnly = true;
		IdCode4TextBox.TabStop = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label8, 2);
		componentResourceManager.ApplyResources(label8, "label8");
		label8.Name = "label8";
		label8.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(LockConfigurationTextBox, "LockConfigurationTextBox");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)LockConfigurationTextBox, 5);
		LockConfigurationTextBox.Name = "LockConfigurationTextBox";
		LockConfigurationTextBox.ReadOnly = true;
		LockConfigurationTextBox.TabStop = false;
		componentResourceManager.ApplyResources(CloseButton, "CloseButton");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)CloseButton, 2);
		CloseButton.DialogResult = DialogResult.Cancel;
		CloseButton.Name = "CloseButton";
		CloseButton.UseCompatibleTextRendering = true;
		CloseButton.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label9, 2);
		componentResourceManager.ApplyResources(label9, "label9");
		label9.Name = "label9";
		label9.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(ChecksumTextBox, "ChecksumTextBox");
		ChecksumTextBox.Name = "ChecksumTextBox";
		ChecksumTextBox.ReadOnly = true;
		ChecksumTextBox.TabStop = false;
		componentResourceManager.ApplyResources(Aut64Key3TextBox, "Aut64Key3TextBox");
		Aut64Key3TextBox.Name = "Aut64Key3TextBox";
		Aut64Key3TextBox.TextChanged += Aut64Key3TextBox_TextChanged;
		Aut64Key3TextBox.KeyPress += Aut64KeyTextBox_KeyPress;
		componentResourceManager.ApplyResources(UnlockButton, "UnlockButton");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)UnlockButton, 2);
		UnlockButton.Name = "UnlockButton";
		UnlockButton.UseCompatibleTextRendering = true;
		UnlockButton.UseVisualStyleBackColor = true;
		UnlockButton.Click += UnlockButton_Click;
		componentResourceManager.ApplyResources(RefreshInputsButton, "RefreshInputsButton");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)RefreshInputsButton, 2);
		RefreshInputsButton.Name = "RefreshInputsButton";
		RefreshInputsButton.UseCompatibleTextRendering = true;
		RefreshInputsButton.UseVisualStyleBackColor = true;
		RefreshInputsButton.Click += RefreshInputsButton_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Unlock_ECU_for_Reprogramming");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
