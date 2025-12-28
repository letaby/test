using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes__MR2_.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		Unknown,
		Initializing,
		ReInitializing,
		ReadingInputs,
		WaitingForKey,
		WaitingForServerConnection,
		ReadyToClearFaults,
		ClearingFaultCodes,
		Done,
		Closing
	}

	private enum CalculationTypes
	{
		Unknown,
		XN
	}

	private const string ecuSerialNumberQualifier = "DJ_Get_ECU_Serial_Number";

	private const string unlockSeedQualifier = "DT_STO_ID_AUT64_Challenge_challenge";

	private const string securityQualifier = "RT_SR0903_EGR_Function_lock_Start_State_Byte";

	private const int disableSecurityInput = 2;

	private const int enableSecurityInput = 0;

	private const string ecuToUnlock = "MR201T";

	private const string defaultCalculationType = "XN";

	private const string unlockSharedProcedureFormat = "SP_SecurityUnlock_{0}_UnlockXN";

	private bool useManualUnlock;

	private Channel channel;

	private Service readEcuSerialNumberService;

	private Service securityService;

	private Dictionary<string, string> YKeyField = null;

	private Dictionary<string, string> VKeyField = null;

	private State currentState;

	private TableLayoutPanel tableLayoutPanelManualInputs;

	private Button buttonClose;

	private Button buttonRefreshInputs;

	private System.Windows.Forms.Label labelRandomNumber;

	private TextBox textBoxRN1;

	private TextBox textBoxRN2;

	private TextBox textBoxIDCode1;

	private TextBox textBoxIDCode2;

	private TextBox textBoxIDCode3;

	private TextBox textBoxIDCode4;

	private TextBox textBoxCalculationType;

	private TextBox textBoxAuth64Key1;

	private TextBox textBoxAuth64Key2;

	private TextBox textBoxAuth64Key3;

	private TextBox textBoxAuth64Key4;

	private System.Windows.Forms.Label labelIDCode;

	private System.Windows.Forms.Label labelCalculationType;

	private TextBox textBoxToolId;

	private System.Windows.Forms.Label label2;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonClearNonEraseableFaultCodes;

	private Button buttonSwitchUnlockMode;

	private SeekTimeListView seekTimeListView1;

	private System.Windows.Forms.Label label1;

	public UserPanel()
	{
		InitializeComponent();
		YKeyField = new Dictionary<string, string>();
		YKeyField["0"] = "53C1";
		YKeyField["1"] = "43C3";
		YKeyField["2"] = "55B5";
		YKeyField["3"] = "5451";
		YKeyField["4"] = "3717";
		YKeyField["5"] = "27AD";
		YKeyField["6"] = "11A5";
		YKeyField["7"] = "713A";
		YKeyField["8"] = "3327";
		YKeyField["9"] = "5B31";
		YKeyField["A"] = "B157";
		YKeyField["B"] = "BA75";
		YKeyField["C"] = "2A57";
		YKeyField["D"] = "3154";
		YKeyField["E"] = "C5A7";
		YKeyField["F"] = "A7D3";
		VKeyField = new Dictionary<string, string>();
		VKeyField["0"] = "51A3C521";
		VKeyField["1"] = "41B3C923";
		VKeyField["2"] = "55C5B435";
		VKeyField["3"] = "5AB45131";
		VKeyField["4"] = "35371AA7";
		VKeyField["5"] = "2377AAD3";
		VKeyField["6"] = "1A31A755";
		VKeyField["7"] = "772135AA";
		VKeyField["8"] = "37232177";
		VKeyField["9"] = "5735B31A";
		VKeyField["A"] = "B4515737";
		VKeyField["B"] = "B31A3751";
		VKeyField["C"] = "21AA7757";
		VKeyField["D"] = "31D355B4";
		VKeyField["E"] = "C555A7B3";
		VKeyField["F"] = "AAA7D3C5";
		useManualUnlock = false;
	}

	protected override void OnLoad(EventArgs e)
	{
		SetState(State.Initializing);
		SetChannel(((CustomPanel)this).GetChannel("MR201T"));
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
		ResizeForm(largeSize: false);
	}

	private void InitializeForConnectedEcu()
	{
		if (channel != null)
		{
			readEcuSerialNumberService = channel.Services["DJ_Get_ECU_Serial_Number"];
			securityService = channel.Services["RT_SR0903_EGR_Function_lock_Start_State_Byte"];
			textBoxAuth64Key1.Visible = true;
			textBoxAuth64Key2.Visible = true;
			textBoxAuth64Key3.Visible = true;
			textBoxAuth64Key4.Visible = true;
			textBoxCalculationType.Text = "XN";
			textBoxToolId.Text = ApplicationInformation.ComputerId.ToString();
		}
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		switch (currentState)
		{
		case State.Initializing:
		case State.ReadingInputs:
		case State.ClearingFaultCodes:
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

	public override void OnChannelsChanged()
	{
		if (currentState != State.Closing && currentState != State.ReInitializing)
		{
			SetChannel(((CustomPanel)this).GetChannel("MR201T"));
		}
	}

	private void SetChannel(Channel channel)
	{
		if (channel != this.channel)
		{
			if (channel == null && currentState != State.Closing)
			{
				SetState(State.Unknown);
				ClearInputs();
				ClearKeys();
			}
			this.channel = channel;
			if (currentState != State.Closing && currentState != State.ClearingFaultCodes && currentState != State.ReadingInputs && this.channel != null)
			{
				InitializeForConnectedEcu();
				if (currentState != State.ReInitializing && useManualUnlock)
				{
					ReadInputs();
				}
				else
				{
					SetState(State.ReadyToClearFaults);
				}
			}
		}
		if (this.channel == null)
		{
			SetState(State.Unknown);
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
		if (useManualUnlock)
		{
			buttonRefreshInputs.Visible = true;
			((Control)(object)tableLayoutPanelManualInputs).Visible = true;
			buttonSwitchUnlockMode.Text = Resources.Message_SwitchToServerUnlock;
		}
		else
		{
			buttonRefreshInputs.Visible = false;
			((Control)(object)tableLayoutPanelManualInputs).Visible = false;
			buttonSwitchUnlockMode.Text = Resources.Message_SwitchToManualUnlock;
		}
		switch (currentState)
		{
		default:
			buttonRefreshInputs.Enabled = false;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = true;
			buttonSwitchUnlockMode.Enabled = true;
			UpdateStatus(Resources.Message_Unknown);
			break;
		case State.Initializing:
			buttonRefreshInputs.Enabled = false;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = false;
			buttonSwitchUnlockMode.Enabled = false;
			UpdateStatus(Resources.Message_InitializingWait);
			break;
		case State.ReInitializing:
			buttonRefreshInputs.Enabled = false;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = false;
			buttonSwitchUnlockMode.Enabled = false;
			break;
		case State.ReadingInputs:
			buttonRefreshInputs.Enabled = false;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = false;
			buttonSwitchUnlockMode.Enabled = false;
			UpdateStatus(Resources.Message_ReadingInputsWait);
			break;
		case State.WaitingForKey:
			buttonRefreshInputs.Enabled = true;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = true;
			buttonSwitchUnlockMode.Enabled = true;
			UpdateStatus(Resources.Message_PleaseTypeInUnlockKey);
			break;
		case State.ReadyToClearFaults:
			buttonRefreshInputs.Enabled = true;
			buttonClearNonEraseableFaultCodes.Enabled = true;
			buttonClose.Enabled = true;
			buttonSwitchUnlockMode.Enabled = true;
			UpdateStatus(Resources.Message_ReadyToClearFaults);
			break;
		case State.ClearingFaultCodes:
			buttonRefreshInputs.Enabled = false;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = false;
			buttonSwitchUnlockMode.Enabled = false;
			UpdateStatus(Resources.Message_ClearingFaultCodesWait);
			break;
		case State.Done:
			buttonRefreshInputs.Enabled = true;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = true;
			buttonSwitchUnlockMode.Enabled = true;
			UpdateStatus(Resources.Message_Done);
			break;
		case State.Closing:
			buttonRefreshInputs.Enabled = false;
			buttonClearNonEraseableFaultCodes.Enabled = false;
			buttonClose.Enabled = false;
			buttonSwitchUnlockMode.Enabled = false;
			UpdateStatus(Resources.Message_Closing);
			break;
		}
	}

	private void UpdateStatus(string message)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.CurrentCulture, Resources.StatusFormat, message));
	}

	private void buttonRefreshInputs_Click(object sender, EventArgs e)
	{
		ClearInputs();
		ReadInputs();
	}

	private void ReadInputs()
	{
		if (channel != null && readEcuSerialNumberService != null)
		{
			SetState(State.ReadingInputs);
			readEcuSerialNumberService.ServiceCompleteEvent += OnReadInputsComplete;
			readEcuSerialNumberService.Execute(synchronous: false);
		}
	}

	private void OnReadInputsComplete(object sender, ResultEventArgs e)
	{
		readEcuSerialNumberService.ServiceCompleteEvent -= OnReadInputsComplete;
		if (e.Succeeded && readEcuSerialNumberService.OutputValues != null)
		{
			string text = readEcuSerialNumberService.OutputValues[0].Value.ToString();
			string ecuInfoData = GetEcuInfoData(channel, "DT_STO_ID_AUT64_Challenge_challenge");
			if (text.Length >= 8 && ecuInfoData.Length >= 4)
			{
				textBoxIDCode1.Text = text.Substring(0, 2);
				textBoxIDCode2.Text = text.Substring(2, 2);
				textBoxIDCode3.Text = text.Substring(4, 2);
				textBoxIDCode4.Text = text.Substring(6, 2);
				textBoxRN1.Text = ecuInfoData.Substring(0, 2);
				textBoxRN2.Text = ecuInfoData.Substring(2, 2);
				textBoxAuth64Key1.Focus();
				SetState(State.WaitingForKey);
				return;
			}
		}
		ClearInputs();
		UpdateStatus(Resources.Message_FailureReadingInputs);
		SetState(State.Done);
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

	private void textBoxAuth64Key_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (char.IsLetter(e.KeyChar) || char.IsSymbol(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || char.IsPunctuation(e.KeyChar))
		{
			e.Handled = true;
		}
	}

	private void textBoxAuth64Key1_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(textBoxAuth64Key1) && textBoxAuth64Key1.Text.Length == 3)
		{
			textBoxAuth64Key2.Focus();
			textBoxAuth64Key2.SelectAll();
		}
	}

	private void textBoxAuth64Key2_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(textBoxAuth64Key2) && textBoxAuth64Key2.Text.Length == 3)
		{
			textBoxAuth64Key3.Focus();
			textBoxAuth64Key3.SelectAll();
		}
	}

	private void textBoxAuth64Key3_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(textBoxAuth64Key3) && textBoxAuth64Key3.Text.Length == 3)
		{
			textBoxAuth64Key4.Focus();
			textBoxAuth64Key4.SelectAll();
		}
	}

	private void textBoxAuth64Key4_TextChanged(object sender, EventArgs e)
	{
		if (checkAut64KeyTextBox(textBoxAuth64Key4) && textBoxAuth64Key4.Text.Length == 3)
		{
			buttonClearNonEraseableFaultCodes.Focus();
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
			SetState(State.ReadyToClearFaults);
		}
		else
		{
			SetState(State.WaitingForKey);
		}
		return true;
	}

	private bool checkAllKeys()
	{
		if (!checkKeyTextBox(textBoxAuth64Key1))
		{
			return false;
		}
		if (!checkKeyTextBox(textBoxAuth64Key2))
		{
			return false;
		}
		if (!checkKeyTextBox(textBoxAuth64Key3))
		{
			return false;
		}
		if (!checkKeyTextBox(textBoxAuth64Key4))
		{
			return false;
		}
		if (!ValidateVeDocKey())
		{
			UpdateStatus(Resources.Message_UnlockKeyIsIncorrect);
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
			UpdateStatus(Resources.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range);
			textBox.SelectAll();
			return false;
		}
		byte result;
		bool flag = byte.TryParse(textBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
		if (!flag)
		{
			UpdateStatus(Resources.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range);
			textBox.SelectAll();
		}
		return flag;
	}

	private void ClearInputs()
	{
		textBoxIDCode1.Text = "";
		textBoxIDCode2.Text = "";
		textBoxIDCode3.Text = "";
		textBoxIDCode4.Text = "";
		textBoxRN1.Text = "";
		textBoxRN2.Text = "";
	}

	private void ClearKeys()
	{
		textBoxAuth64Key1.Text = "";
		textBoxAuth64Key2.Text = "";
		textBoxAuth64Key3.Text = "";
		textBoxAuth64Key4.Text = "";
		textBoxAuth64Key1.Focus();
	}

	private void buttonClearNonEraseableFaultCodes_Click(object sender, EventArgs e)
	{
		SetState(State.ClearingFaultCodes);
		if (useManualUnlock)
		{
			PerformUnlockForFaultCodesClear();
		}
		else
		{
			PerformServerUnlockAndClear();
		}
	}

	private void PerformServerUnlockAndClear()
	{
		string text = $"SP_SecurityUnlock_{channel.Ecu.Name}_UnlockXN";
		SharedProcedureBase val = SharedProcedureBase.AvailableProcedures[text];
		if (val != null)
		{
			if (val.CanStart)
			{
				val.StartComplete += unlockSharedProcedure_StartComplete;
				val.Start();
				return;
			}
			UpdateStatus(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", val.Name));
		}
		else
		{
			UpdateStatus(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", text));
		}
		SetState(State.Done);
	}

	private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StartComplete -= unlockSharedProcedure_StartComplete;
		if (((ResultEventArgs)(object)e).Succeeded && (int)e.Result == 1)
		{
			UpdateStatus(string.Format(CultureInfo.InvariantCulture, "{0} unlock via the server was initiated using procedure {1}", channel.Ecu.Name, val.Name));
			val.StopComplete += unlockSharedProcedure_StopComplete;
		}
		else
		{
			UpdateStatus(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
			SetState(State.Done);
		}
	}

	private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StopComplete -= unlockSharedProcedure_StopComplete;
		if (!((ResultEventArgs)(object)e).Succeeded || (int)e.Result == 0)
		{
			UpdateStatus(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
			if (((ResultEventArgs)(object)e).Succeeded)
			{
				SetState(State.ReadyToClearFaults);
			}
			else
			{
				SetState(State.Done);
			}
		}
		else
		{
			UpdateStatus(string.Format(CultureInfo.InvariantCulture, "{0} was unlocked via the server using procedure {1}", channel.Ecu.Name, val.Name));
			PerformUnlockForFaultCodesClear();
		}
	}

	private void PerformUnlockForFaultCodesClear()
	{
		if (securityService != null)
		{
			securityService.ServiceCompleteEvent += unlockService_OnUnlockServiceServiceComplete;
			securityService.InputValues[0].Value = securityService.InputValues[0].Choices.GetItemFromRawValue(2);
			securityService.Execute(synchronous: false);
		}
	}

	private void unlockService_OnUnlockServiceServiceComplete(object sender, ResultEventArgs e)
	{
		securityService.ServiceCompleteEvent -= unlockService_OnUnlockServiceServiceComplete;
		if (e.Succeeded)
		{
			PerformFaultCodeClear();
			return;
		}
		ClearInputs();
		ClearKeys();
		ReadInputs();
		SetState(State.ReadingInputs);
	}

	private void PerformFaultCodeClear()
	{
		channel.FaultCodes.FaultCodesUpdateEvent += OnFaultCodesUpdateEventComplete;
		channel.FaultCodes.Reset(synchronous: false);
	}

	private void OnFaultCodesUpdateEventComplete(object sender, ResultEventArgs e)
	{
		channel.FaultCodes.FaultCodesUpdateEvent -= OnFaultCodesUpdateEventComplete;
		PerformLock();
	}

	private void PerformLock()
	{
		if (securityService != null)
		{
			securityService.ServiceCompleteEvent += lockService_OnLockServiceServiceComplete;
			securityService.InputValues[0].Value = securityService.InputValues[0].Choices.GetItemFromRawValue(0);
			securityService.Execute(synchronous: false);
		}
	}

	private void lockService_OnLockServiceServiceComplete(object sender, ResultEventArgs e)
	{
		securityService.ServiceCompleteEvent -= lockService_OnLockServiceServiceComplete;
		ClearKeys();
		SetState(State.Done);
	}

	private bool ValidateVeDocKey()
	{
		string key = textBoxRN1.Text.Substring(1, 1);
		string arg = YKeyField[key];
		string key2 = textBoxRN2.Text.Substring(0, 1);
		string hexString = VKeyField[key2];
		string hexString2 = $"{textBoxIDCode1.Text}{textBoxIDCode2.Text}{textBoxIDCode3.Text}{textBoxIDCode4.Text}";
		string hexString3 = $"{textBoxRN1.Text}{textBoxRN2.Text}{arg}";
		byte[] num = ConvertToByteArray(hexString2);
		byte[] num2 = ConvertToByteArray(hexString3);
		byte[] array = XOR(num, num2);
		byte[] array2 = ConvertToByteArray(hexString);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(array);
			Array.Reverse(array2);
		}
		uint num3 = BitConverter.ToUInt32(array, 0);
		uint num4 = BitConverter.ToUInt32(array2, 0);
		string s = (num3 + num4).ToString("X");
		string s2 = SwapFinalResult(s);
		string[] array3 = ConvertToStringArray(s2);
		if (!CompareValues(textBoxAuth64Key1.Text, array3[0]))
		{
			return false;
		}
		if (!CompareValues(textBoxAuth64Key2.Text, array3[1]))
		{
			return false;
		}
		if (!CompareValues(textBoxAuth64Key3.Text, array3[2]))
		{
			return false;
		}
		if (!CompareValues(textBoxAuth64Key4.Text, array3[3]))
		{
			return false;
		}
		return true;
	}

	private bool CompareValues(string textBoxValue, string calculatedResult)
	{
		if (!int.TryParse(textBoxValue, out var result))
		{
			return false;
		}
		if (!int.TryParse(calculatedResult, out var result2))
		{
			return false;
		}
		return result == result2;
	}

	private byte[] ConvertToByteArray(string hexString)
	{
		byte[] array = new byte[hexString.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			string s = hexString.Substring(i * 2, 2);
			array[i] = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}
		return array;
	}

	private byte[] XOR(byte[] num1, byte[] num2)
	{
		byte[] array = new byte[num1.Length];
		for (int i = 0; i < num1.Length; i++)
		{
			array[i] = (byte)(num1[i] ^ num2[i]);
		}
		return array;
	}

	private string SwapFinalResult(string s)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < s.Length; i++)
		{
			if (i == 0 || i == s.Length - 2)
			{
				stringBuilder.Append(s[i + 1]);
				stringBuilder.Append(s[i]);
				i++;
			}
			else
			{
				stringBuilder.Append(s[i]);
			}
		}
		return stringBuilder.ToString();
	}

	private string[] ConvertToStringArray(string s)
	{
		byte[] array = ConvertToByteArray(s);
		string text = BitConverter.ToString(array);
		string[] array2 = text.Split('-');
		int num = 0;
		string[] array3 = array2;
		foreach (string value in array3)
		{
			string text2 = Convert.ToUInt32(value, 16).ToString();
			if (text2.Length < 3)
			{
				int length = text2.Length;
				for (int j = 0; j < 3 - length; j++)
				{
					text2 = $"0{text2}";
				}
			}
			array2[num] = text2;
			num++;
		}
		return array2;
	}

	private void ResizeForm(bool largeSize)
	{
		if (largeSize)
		{
			((TableLayoutPanel)(object)tableLayoutPanel2).RowStyles[0].SizeType = SizeType.AutoSize;
			return;
		}
		((TableLayoutPanel)(object)tableLayoutPanel2).RowStyles[0].SizeType = SizeType.Absolute;
		((TableLayoutPanel)(object)tableLayoutPanel2).RowStyles[0].Height = 0f;
	}

	private void buttonSwitchUnlockMode_Click(object sender, EventArgs e)
	{
		useManualUnlock = !useManualUnlock;
		if (useManualUnlock)
		{
			SetState(State.Initializing);
			ResizeForm(largeSize: true);
			UpdateUI();
			ReadInputs();
		}
		else
		{
			SetState(State.ReadyToClearFaults);
			ResizeForm(largeSize: false);
			UpdateUI();
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelManualInputs = new TableLayoutPanel();
		label2 = new System.Windows.Forms.Label();
		labelRandomNumber = new System.Windows.Forms.Label();
		labelIDCode = new System.Windows.Forms.Label();
		labelCalculationType = new System.Windows.Forms.Label();
		label1 = new System.Windows.Forms.Label();
		textBoxToolId = new TextBox();
		textBoxAuth64Key1 = new TextBox();
		textBoxAuth64Key2 = new TextBox();
		textBoxAuth64Key3 = new TextBox();
		textBoxAuth64Key4 = new TextBox();
		textBoxIDCode1 = new TextBox();
		textBoxCalculationType = new TextBox();
		textBoxRN1 = new TextBox();
		textBoxRN2 = new TextBox();
		textBoxIDCode2 = new TextBox();
		textBoxIDCode3 = new TextBox();
		textBoxIDCode4 = new TextBox();
		buttonClose = new Button();
		buttonRefreshInputs = new Button();
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonSwitchUnlockMode = new Button();
		buttonClearNonEraseableFaultCodes = new Button();
		seekTimeListView1 = new SeekTimeListView();
		((Control)(object)tableLayoutPanelManualInputs).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelManualInputs, "tableLayoutPanelManualInputs");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)tableLayoutPanelManualInputs, 5);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(label2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(labelRandomNumber, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(labelIDCode, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(labelCalculationType, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(label1, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxToolId, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxAuth64Key1, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxAuth64Key2, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxAuth64Key3, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxAuth64Key4, 4, 5);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxIDCode1, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxCalculationType, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxRN1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxRN2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxIDCode2, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxIDCode3, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).Controls.Add(textBoxIDCode4, 4, 2);
		((Control)(object)tableLayoutPanelManualInputs).Name = "tableLayoutPanelManualInputs";
		((Panel)(object)tableLayoutPanelManualInputs).TabStop = true;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelRandomNumber, "labelRandomNumber");
		labelRandomNumber.Name = "labelRandomNumber";
		labelRandomNumber.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelIDCode, "labelIDCode");
		labelIDCode.Name = "labelIDCode";
		labelIDCode.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelCalculationType, "labelCalculationType");
		labelCalculationType.Name = "labelCalculationType";
		labelCalculationType.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxToolId, "textBoxToolId");
		((TableLayoutPanel)(object)tableLayoutPanelManualInputs).SetColumnSpan((Control)textBoxToolId, 3);
		textBoxToolId.Name = "textBoxToolId";
		textBoxToolId.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxAuth64Key1, "textBoxAuth64Key1");
		textBoxAuth64Key1.Name = "textBoxAuth64Key1";
		textBoxAuth64Key1.TextChanged += textBoxAuth64Key1_TextChanged;
		textBoxAuth64Key1.KeyPress += textBoxAuth64Key_KeyPress;
		componentResourceManager.ApplyResources(textBoxAuth64Key2, "textBoxAuth64Key2");
		textBoxAuth64Key2.Name = "textBoxAuth64Key2";
		textBoxAuth64Key2.TextChanged += textBoxAuth64Key2_TextChanged;
		textBoxAuth64Key2.KeyPress += textBoxAuth64Key_KeyPress;
		componentResourceManager.ApplyResources(textBoxAuth64Key3, "textBoxAuth64Key3");
		textBoxAuth64Key3.Name = "textBoxAuth64Key3";
		textBoxAuth64Key3.TextChanged += textBoxAuth64Key3_TextChanged;
		textBoxAuth64Key3.KeyPress += textBoxAuth64Key_KeyPress;
		componentResourceManager.ApplyResources(textBoxAuth64Key4, "textBoxAuth64Key4");
		textBoxAuth64Key4.Name = "textBoxAuth64Key4";
		textBoxAuth64Key4.TextChanged += textBoxAuth64Key4_TextChanged;
		textBoxAuth64Key4.KeyPress += textBoxAuth64Key_KeyPress;
		componentResourceManager.ApplyResources(textBoxIDCode1, "textBoxIDCode1");
		textBoxIDCode1.Name = "textBoxIDCode1";
		textBoxIDCode1.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxCalculationType, "textBoxCalculationType");
		textBoxCalculationType.Name = "textBoxCalculationType";
		textBoxCalculationType.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxRN1, "textBoxRN1");
		textBoxRN1.Name = "textBoxRN1";
		textBoxRN1.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxRN2, "textBoxRN2");
		textBoxRN2.Name = "textBoxRN2";
		textBoxRN2.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxIDCode2, "textBoxIDCode2");
		textBoxIDCode2.Name = "textBoxIDCode2";
		textBoxIDCode2.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxIDCode3, "textBoxIDCode3");
		textBoxIDCode3.Name = "textBoxIDCode3";
		textBoxIDCode3.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxIDCode4, "textBoxIDCode4");
		textBoxIDCode4.Name = "textBoxIDCode4";
		textBoxIDCode4.ReadOnly = true;
		buttonClose.DialogResult = DialogResult.Cancel;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonRefreshInputs, "buttonRefreshInputs");
		buttonRefreshInputs.Name = "buttonRefreshInputs";
		buttonRefreshInputs.UseCompatibleTextRendering = true;
		buttonRefreshInputs.UseVisualStyleBackColor = true;
		buttonRefreshInputs.Click += buttonRefreshInputs_Click;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonSwitchUnlockMode, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanelManualInputs, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonRefreshInputs, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClearNonEraseableFaultCodes, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)seekTimeListView1, 0, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonSwitchUnlockMode, "buttonSwitchUnlockMode");
		buttonSwitchUnlockMode.Name = "buttonSwitchUnlockMode";
		buttonSwitchUnlockMode.UseCompatibleTextRendering = true;
		buttonSwitchUnlockMode.UseVisualStyleBackColor = true;
		buttonSwitchUnlockMode.Click += buttonSwitchUnlockMode_Click;
		componentResourceManager.ApplyResources(buttonClearNonEraseableFaultCodes, "buttonClearNonEraseableFaultCodes");
		buttonClearNonEraseableFaultCodes.Name = "buttonClearNonEraseableFaultCodes";
		buttonClearNonEraseableFaultCodes.UseCompatibleTextRendering = true;
		buttonClearNonEraseableFaultCodes.UseVisualStyleBackColor = true;
		buttonClearNonEraseableFaultCodes.Click += buttonClearNonEraseableFaultCodes_Click;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)seekTimeListView1, 5);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "ClearNonErasableMR2";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelManualInputs).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelManualInputs).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
