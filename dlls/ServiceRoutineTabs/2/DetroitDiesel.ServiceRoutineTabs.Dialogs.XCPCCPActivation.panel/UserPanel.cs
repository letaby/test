using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

public class UserPanel : CustomPanel
{
	private const string Checksum = "XXX";

	private const string CalculationType = "XE";

	private const int UnlockMinimumAccessLevel = 3;

	private const string unlockSharedProcedureFormat = "SP_SecurityUnlock_{0}_Unlock{1}";

	private Channel selectedChannel;

	private bool unlockSupported;

	private bool busy;

	private bool closing;

	private List<string> ecusToUnlock = new List<string> { "ACM21T", "ACM301T", "MCM21T" };

	private bool useManualUnlock;

	private readonly Dictionary<string, VedocReadInputDataQualifiers> VeDocInputResponseQualifiers = new Dictionary<string, VedocReadInputDataQualifiers>
	{
		{
			"ACM21T",
			VedocReadInputDataQualifiers.Create("ZUFALLSZAHL", "ID_CODE", "ANZAHL_SCHLUESSEL", "TRANSPONDER_CODE")
		},
		{
			"ACM301T",
			VedocReadInputDataQualifiers.Create("ZUFALLSZAHL", "ID_CODE", "ANZAHL_SCHLUESSEL", "TRANSPONDER_CODE")
		},
		{
			"MCM21T",
			VedocReadInputDataQualifiers.Create("ZUFALLSZAHL", "ID_CODE", "ANZAHL_SCHLUESSEL", "TRANSPONDER_CODE")
		}
	};

	private readonly Dictionary<string, string> ReadActivationModeQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration" },
		{ "ACM301T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration" },
		{ "MCM21T", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_Function_supported_by_calibration" }
	};

	private readonly Dictionary<string, string> ReadActivationStatusQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU" },
		{ "ACM301T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU" },
		{ "MCM21T", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_State_of_measurement_CAN_in_ROM_ECU" }
	};

	private readonly Dictionary<string, string> ReadVeDocInputQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "DJ_Read_AUT64_VeDoc_Input_In_Application" },
		{ "ACM301T", "DJ_Read_AUT64_VeDoc_Input_In_Application" },
		{ "MCM21T", "DJ_Read_AUT64_VeDoc_Input_for_UDS" }
	};

	private readonly Dictionary<string, string> ReadVeDocInputIdCodeOverrideQualifier = new Dictionary<string, string> { { "MCM21T", "DT_STO_ID_Read_Curent_ECU_ID_ECU_ID_Current" } };

	private readonly Dictionary<string, string> UnlockAut64SecurityQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2" },
		{ "ACM301T", "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2" },
		{ "MCM21T", "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2" }
	};

	private readonly Dictionary<string, string> EnableXCPCCPQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start" },
		{ "ACM301T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start" },
		{ "MCM21T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start" }
	};

	private readonly Dictionary<string, string> DisableXCPCCPQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start" },
		{ "ACM301T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start" },
		{ "MCM21T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_Start" }
	};

	private readonly Dictionary<string, string> ResetEcuQualifier = new Dictionary<string, string>
	{
		{ "ACM21T", "FN_KeyOffOnReset" },
		{ "ACM301T", "FN_KeyOffOnReset" },
		{ "MCM21T", "FN_KeyOffOnReset" }
	};

	private Service readVeDocInputService = null;

	private Service disableXCPCCPService = null;

	private bool unlockingToEnableXCPCCP;

	private Service unlockVeDocService = null;

	private Service enableXCPCCPService = null;

	private string commitServiceList = string.Empty;

	private Service resetEcuService = null;

	private TableLayoutPanel tableLayoutPanel1;

	private System.Windows.Forms.Label label8;

	private TextBox textBoxXFactor;

	private TextBox textBoxChallengeCode;

	private TextBox textBoxNumberOfCode;

	private TextBox textBoxTransponderCode;

	private TextBox textBoxEcuIdCode;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label IDCodeLabel;

	private System.Windows.Forms.Label TranspoderCodeLabel;

	private System.Windows.Forms.Label NumberOfKeysLabel;

	private System.Windows.Forms.Label RandomNumberLabel;

	private System.Windows.Forms.Label XFactorValueLabel;

	private System.Windows.Forms.Label UnlockKeyLabel;

	private TextBox textBoxActivationMode;

	private ComboBox comboBoxEcuToUnlock;

	private FlowLayoutPanel flowLayoutPanel1;

	private TextBox textBoxKey1;

	private TextBox textBoxKey2;

	private TextBox textBoxKey3;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonRefresh;

	private Button buttonEnable;

	private Button buttonDisable;

	private Button buttonClose;

	private SeekTimeListView seekTimeListView1;

	private TextBox textBoxChecksum;

	private System.Windows.Forms.Label ChecksumLabel;

	private Button buttonSwitchUnlockMode;

	private TextBox textBoxKey4;

	private bool EnableButtons => unlockSupported && !busy && !closing;

	private bool UnlockInputsValid => textBoxKey1.Text.Length == 3 && IsNumeric(textBoxKey1.Text) && textBoxKey2.Text.Length == 3 && IsNumeric(textBoxKey2.Text) && textBoxKey3.Text.Length == 3 && IsNumeric(textBoxKey3.Text) && textBoxKey4.Text.Length == 3 && IsNumeric(textBoxKey4.Text);

	private bool CanEnableXCPCCP => 3 <= ApplicationInformation.ProductAccessLevel;

	public UserPanel()
	{
		InitializeComponent();
		useManualUnlock = false;
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		if (!closing)
		{
			UpdateChannels();
		}
	}

	private void UpdateChannels()
	{
		comboBoxEcuToUnlock.Items.Clear();
		foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
		{
			if (ecusToUnlock.Contains(channel.ToString()) && channel.Online)
			{
				comboBoxEcuToUnlock.Items.Add(channel);
				if (channel == selectedChannel)
				{
					comboBoxEcuToUnlock.SelectedItem = channel;
				}
			}
		}
		if (comboBoxEcuToUnlock.Items.Count > 0)
		{
			if (comboBoxEcuToUnlock.SelectedItem == null)
			{
				comboBoxEcuToUnlock.SelectedItem = comboBoxEcuToUnlock.Items[0];
			}
			SetChannel((Channel)comboBoxEcuToUnlock.SelectedItem);
		}
		else
		{
			SetChannel(null);
		}
	}

	private void SetChannel(Channel channel)
	{
		if (channel != selectedChannel)
		{
			if (channel == null)
			{
				ClearInputs();
				ClearKeys();
			}
			selectedChannel = channel;
			ClearInputs();
			ClearKeys();
			ReadLockConfiguration();
			if (unlockSupported && useManualUnlock)
			{
				ReadInputs();
			}
		}
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			closing = true;
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			SetChannel(null);
		}
	}

	private static bool IsNumeric(string input)
	{
		double result = 0.0;
		if (!string.IsNullOrEmpty(input) && double.TryParse(input, out result))
		{
			return true;
		}
		return false;
	}

	private void UpdateUserInterface()
	{
		if (!busy && !closing)
		{
			buttonSwitchUnlockMode.Enabled = true;
		}
		if (useManualUnlock)
		{
			buttonEnable.Enabled = EnableButtons && UnlockInputsValid && CanEnableXCPCCP;
			buttonRefresh.Enabled = EnableButtons && CanEnableXCPCCP;
			buttonSwitchUnlockMode.Text = Resources.Message_SwitchToServerUnlock;
			textBoxKey1.Enabled = EnableButtons && CanEnableXCPCCP;
			textBoxKey2.Enabled = EnableButtons && CanEnableXCPCCP;
			textBoxKey3.Enabled = EnableButtons && CanEnableXCPCCP;
			textBoxKey4.Enabled = EnableButtons && CanEnableXCPCCP;
			ShowManualInputs(visible: true);
		}
		else
		{
			buttonEnable.Enabled = EnableButtons && CanEnableXCPCCP;
			buttonRefresh.Enabled = false;
			buttonSwitchUnlockMode.Text = Resources.Message_SwitchToManualUnlock;
			ShowManualInputs(visible: false);
		}
		buttonDisable.Enabled = EnableButtons;
	}

	private void ClearInputs()
	{
		textBoxActivationMode.Text = string.Empty;
		textBoxEcuIdCode.Text = string.Empty;
		textBoxTransponderCode.Text = string.Empty;
		textBoxNumberOfCode.Text = string.Empty;
		textBoxChallengeCode.Text = string.Empty;
		textBoxXFactor.Text = string.Empty;
		textBoxChecksum.Text = string.Empty;
	}

	private void ClearKeys()
	{
		textBoxKey1.Text = string.Empty;
		textBoxKey2.Text = string.Empty;
		textBoxKey3.Text = string.Empty;
		textBoxKey4.Text = string.Empty;
	}

	private void ShowManualInputs(bool visible)
	{
		RandomNumberLabel.Visible = visible;
		IDCodeLabel.Visible = visible;
		XFactorValueLabel.Visible = visible;
		NumberOfKeysLabel.Visible = visible;
		TranspoderCodeLabel.Visible = visible;
		ChecksumLabel.Visible = visible;
		UnlockKeyLabel.Visible = visible;
		textBoxEcuIdCode.Visible = visible;
		textBoxTransponderCode.Visible = visible;
		textBoxNumberOfCode.Visible = visible;
		textBoxChallengeCode.Visible = visible;
		textBoxXFactor.Visible = visible;
		textBoxChecksum.Visible = visible;
		textBoxKey1.Visible = visible;
		textBoxKey2.Visible = visible;
		textBoxKey3.Visible = visible;
		textBoxKey4.Visible = visible;
	}

	private string EcuInfoCurrentValue(Channel channel, string qualifier, bool forceRead)
	{
		string result = string.Empty;
		EcuInfo ecuInfo = channel.EcuInfos[qualifier];
		if (ecuInfo != null)
		{
			if (forceRead)
			{
				ecuInfo.Read(synchronous: true);
			}
			if (ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
			{
				Choice choice = ecuInfo.EcuInfoValues.Current.Value as Choice;
				result = ((!(choice != null)) ? ecuInfo.EcuInfoValues.Current.Value.ToString() : choice.ToString());
			}
		}
		return result;
	}

	private int EcuInfoRawValue(EcuInfo ecuInfo)
	{
		int? num = null;
		if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
		{
			Choice choice = ecuInfo.EcuInfoValues.Current.Value as Choice;
			num = ((!(choice != null)) ? (ecuInfo.EcuInfoValues.Current.Value as int?) : (choice.RawValue as int?));
		}
		return (!num.HasValue) ? int.MinValue : num.Value;
	}

	private void ReadModeQualifier(Channel channel, string qualifier)
	{
		EcuInfo ecuInfo = channel.EcuInfos[qualifier];
		if (ecuInfo != null)
		{
			ecuInfo.EcuInfoUpdateEvent += OnModeQualifierReadUpdate;
			ecuInfo.Read(synchronous: false);
		}
		else
		{
			textBoxActivationMode.Text = Resources.Message_NotSupportedOrRequired;
		}
	}

	protected void OnModeQualifierReadUpdate(object sender, ResultEventArgs e)
	{
		EcuInfo ecuInfo = sender as EcuInfo;
		ecuInfo.EcuInfoUpdateEvent -= OnModeQualifierReadUpdate;
		if (EcuInfoRawValue(ecuInfo) != 2)
		{
			textBoxActivationMode.Text = Resources.Message_NotSupportedOrRequired;
		}
		UpdateUserInterface();
	}

	private void ReadStatusQualifier(Channel channel, string qualifier)
	{
		EcuInfo ecuInfo = channel.EcuInfos[qualifier];
		if (ecuInfo != null)
		{
			ecuInfo.EcuInfoUpdateEvent += ecuInfo_EcuInfoUpdateEvent;
			ecuInfo.Read(synchronous: false);
		}
	}

	protected void ecuInfo_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
	{
		EcuInfo ecuInfo = sender as EcuInfo;
		ecuInfo.EcuInfoUpdateEvent -= ecuInfo_EcuInfoUpdateEvent;
		if (EcuInfoRawValue(ecuInfo) == 1)
		{
			textBoxActivationMode.Text = Resources.Message_XCPCCPActivated;
		}
		else
		{
			textBoxActivationMode.Text = Resources.Message_VeDocUnlockRequiredForActivation;
		}
		UpdateUserInterface();
	}

	private string GetSelectedChannelQualifier(Dictionary<string, string> dictionary)
	{
		if (selectedChannel != null && dictionary.ContainsKey(selectedChannel.Ecu.Name))
		{
			return dictionary[selectedChannel.Ecu.Name];
		}
		return string.Empty;
	}

	private void ReadLockConfiguration()
	{
		unlockSupported = false;
		string selectedChannelQualifier = GetSelectedChannelQualifier(ReadActivationModeQualifier);
		string selectedChannelQualifier2 = GetSelectedChannelQualifier(ReadActivationStatusQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier) && !string.IsNullOrEmpty(selectedChannelQualifier2))
		{
			ReadModeQualifier(selectedChannel, selectedChannelQualifier);
			if (CanEnableXCPCCP)
			{
				unlockSupported = true;
				ReadStatusQualifier(selectedChannel, selectedChannelQualifier2);
			}
			else
			{
				textBoxActivationMode.Text = Resources.Message_VeDocUnlockRequiredForActivationButToolIsNotPermittedToPerformAction;
				unlockSupported = true;
			}
		}
		UpdateUserInterface();
	}

	private void ReadInputs()
	{
		string selectedChannelQualifier = GetSelectedChannelQualifier(ReadVeDocInputQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier) && CanEnableXCPCCP)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ReadingUnlockInputDetails);
			readVeDocInputService = selectedChannel.Services[selectedChannelQualifier];
			if (readVeDocInputService != null)
			{
				busy = true;
				readVeDocInputService.ServiceCompleteEvent += readVeDocInputService_ServiceCompleteEvent;
				readVeDocInputService.Execute(synchronous: false);
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
			}
		}
		UpdateUserInterface();
	}

	private void readVeDocInputService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (readVeDocInputService == sender as Service)
		{
			busy = false;
			readVeDocInputService.ServiceCompleteEvent -= readVeDocInputService_ServiceCompleteEvent;
			if (e.Succeeded && selectedChannel != null)
			{
				VedocReadInputDataQualifiers vedocReadInputDataQualifiers = VeDocInputResponseQualifiers[selectedChannel.Ecu.Name];
				if (readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.ChallengeCodeQualifier] != null && readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.ChallengeCodeQualifier].Value != null)
				{
					textBoxChallengeCode.Text = readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.ChallengeCodeQualifier].Value.ToString();
				}
				else
				{
					textBoxChallengeCode.Text = Resources.Message_NullValue;
				}
				if (!OverrideIdCode())
				{
					if (readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.IdCodeQualifier] != null && readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.IdCodeQualifier].Value != null)
					{
						textBoxEcuIdCode.Text = readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.IdCodeQualifier].Value.ToString();
					}
					else
					{
						textBoxEcuIdCode.Text = Resources.Message_NullValue;
					}
				}
				if (readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.NumberOfCodesQualifier] != null && readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.NumberOfCodesQualifier].Value != null)
				{
					textBoxNumberOfCode.Text = readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.NumberOfCodesQualifier].Value.ToString();
				}
				else
				{
					textBoxNumberOfCode.Text = Resources.Message_NullValue;
				}
				if (readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.TransponderCodeQualifier] != null && readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.TransponderCodeQualifier].Value != null)
				{
					textBoxTransponderCode.Text = readVeDocInputService.OutputValues[vedocReadInputDataQualifiers.TransponderCodeQualifier].Value.ToString();
				}
				else
				{
					textBoxTransponderCode.Text = Resources.Message_NullValue;
				}
				readVeDocInputService = null;
				textBoxChecksum.Text = "XXX";
				textBoxXFactor.Text = "XE";
				textBoxKey1.Focus();
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockInputDetailsReadSuccessfully);
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailedToReadUnlockInputDetails);
			}
		}
		UpdateUserInterface();
	}

	private bool OverrideIdCode()
	{
		string selectedChannelQualifier = GetSelectedChannelQualifier(ReadVeDocInputIdCodeOverrideQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier))
		{
			textBoxEcuIdCode.Text = EcuInfoCurrentValue(selectedChannel, selectedChannelQualifier, forceRead: false);
			return true;
		}
		return false;
	}

	private void DisableXCPCCP()
	{
		string selectedChannelQualifier = GetSelectedChannelQualifier(DisableXCPCCPQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier))
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_DisableXCPCCP);
			disableXCPCCPService = selectedChannel.Services[selectedChannelQualifier];
			if (disableXCPCCPService != null)
			{
				busy = true;
				disableXCPCCPService.ServiceCompleteEvent += disableXCPCCPService_ServiceCompleteEvent;
				disableXCPCCPService.Execute(synchronous: false);
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
			}
		}
		UpdateUserInterface();
	}

	private void disableXCPCCPService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (disableXCPCCPService == sender as Service)
		{
			disableXCPCCPService.ServiceCompleteEvent -= disableXCPCCPService_ServiceCompleteEvent;
			disableXCPCCPService = null;
			if (e.Succeeded)
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_XCPCCPHasBeenDisabled);
				CommitChanges();
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailedToDisableXCPCCP);
			}
		}
		UpdateUserInterface();
	}

	private void UnlockVeDocToEnableXCPCCP()
	{
		string selectedChannelQualifier = GetSelectedChannelQualifier(UnlockAut64SecurityQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier))
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_AttemptingToUnlockController);
			unlockVeDocService = selectedChannel.Services[selectedChannelQualifier];
			if (unlockVeDocService != null)
			{
				busy = true;
				unlockingToEnableXCPCCP = true;
				unlockVeDocService.ServiceCompleteEvent += unlockVeDocService_ServiceCompleteEvent;
				unlockVeDocService.InputValues[0].Value = "XE";
				unlockVeDocService.InputValues[1].Value = textBoxKey1.Text;
				unlockVeDocService.InputValues[2].Value = textBoxKey2.Text;
				unlockVeDocService.InputValues[3].Value = textBoxKey3.Text;
				unlockVeDocService.InputValues[4].Value = textBoxKey4.Text;
				unlockVeDocService.Execute(synchronous: false);
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
			}
		}
		UpdateUserInterface();
	}

	private void unlockVeDocService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (unlockVeDocService == sender as Service)
		{
			busy = false;
			unlockVeDocService.ServiceCompleteEvent -= unlockVeDocService_ServiceCompleteEvent;
			if (e.Succeeded && unlockingToEnableXCPCCP && unlockVeDocService.OutputValues != null && unlockVeDocService.OutputValues[0].Value != null)
			{
				Choice choice = unlockVeDocService.OutputValues[0].Value as Choice;
				if (choice != null)
				{
					int? num = choice.RawValue as int?;
					if (num.GetValueOrDefault() == 0 && num.HasValue)
					{
						unlockVeDocService = null;
						unlockingToEnableXCPCCP = false;
						((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_UnlockSuccessful);
						EnableXCPCCP();
						return;
					}
				}
			}
			unlockVeDocService = null;
		}
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_VeDocUnlockHasFailedCheckInputsAndTryAgain);
		UpdateUserInterface();
	}

	private void EnableXCPCCP()
	{
		string selectedChannelQualifier = GetSelectedChannelQualifier(EnableXCPCCPQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier) && CanEnableXCPCCP)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_EnablingXCPCCP);
			enableXCPCCPService = selectedChannel.Services[selectedChannelQualifier];
			if (enableXCPCCPService != null)
			{
				busy = true;
				enableXCPCCPService.ServiceCompleteEvent += enableXCPCCPService_ServiceCompleteEvent;
				enableXCPCCPService.Execute(synchronous: false);
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
			}
		}
		UpdateUserInterface();
	}

	private void enableXCPCCPService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (enableXCPCCPService == sender as Service)
		{
			enableXCPCCPService.ServiceCompleteEvent -= enableXCPCCPService_ServiceCompleteEvent;
			enableXCPCCPService = null;
			if (e.Succeeded)
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_XCPCCPHasBeenEnabled);
				CommitChanges();
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_FailedToEnableXCPCCP);
			}
		}
		UpdateUserInterface();
	}

	private void CommitChanges()
	{
		commitServiceList = selectedChannel.Ecu.Properties["CommitToPermanentMemoryService"];
		if (!string.IsNullOrEmpty(commitServiceList))
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_CommitingChanges);
			busy = true;
			selectedChannel.Services.ServiceCompleteEvent += commitChangesService_ServiceCompleteEvent;
			selectedChannel.Services.Execute(commitServiceList, synchronous: false);
		}
		else
		{
			ResetEcu();
		}
		UpdateUserInterface();
	}

	private void commitChangesService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (commitServiceList == sender as string)
		{
			selectedChannel.Services.ServiceCompleteEvent -= commitChangesService_ServiceCompleteEvent;
			commitServiceList = string.Empty;
			if (e.Succeeded)
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ChangesCommited);
				ResetEcu();
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_CommitServiceFailed);
			}
		}
		UpdateUserInterface();
	}

	private void ResetEcu()
	{
		string selectedChannelQualifier = GetSelectedChannelQualifier(ResetEcuQualifier);
		if (!string.IsNullOrEmpty(selectedChannelQualifier) && CanEnableXCPCCP)
		{
			Application.DoEvents();
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_EcuIsReseting);
			resetEcuService = selectedChannel.Services[selectedChannelQualifier];
			if (resetEcuService != null)
			{
				busy = true;
				selectedChannel.CommunicationsStateUpdateEvent += selectedChannel_CommunicationsStateUpdateEvent;
				resetEcuService.Execute(synchronous: false);
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ServiceNotSupportedByCurrentlyConnectSoftware);
			}
		}
		UpdateUserInterface();
	}

	private void selectedChannel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		if ((e.CommunicationsState == CommunicationsState.Online || e.CommunicationsState == CommunicationsState.Offline) && resetEcuService != null)
		{
			selectedChannel.CommunicationsStateUpdateEvent -= selectedChannel_CommunicationsStateUpdateEvent;
			resetEcuService = null;
			busy = false;
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_OperationCompleted);
			if (selectedChannel != null)
			{
				Application.DoEvents();
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_CycleTheIgnitionToCompleteTheProcess);
			}
		}
	}

	private void PerformServerUnlockAndXcpEnable()
	{
		string text = string.Format("SP_SecurityUnlock_{0}_Unlock{1}", selectedChannel.Ecu.Name, "XE");
		SharedProcedureBase val = SharedProcedureBase.AvailableProcedures[text];
		if (val != null)
		{
			if (val.CanStart)
			{
				val.StartComplete += unlockSharedProcedure_StartComplete;
				val.Start();
			}
			else
			{
				((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", val.Name));
			}
		}
		else
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", text));
		}
	}

	private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StartComplete -= unlockSharedProcedure_StartComplete;
		if (((ResultEventArgs)(object)e).Succeeded && (int)e.Result == 1)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, "{0} unlock via the server was initiated using procedure {1}", selectedChannel.Ecu.Name, val.Name));
			val.StopComplete += unlockSharedProcedure_StopComplete;
		}
		else
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
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
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
			return;
		}
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, "{0} was unlocked via the server using procedure {1}", selectedChannel.Ecu.Name, val.Name));
		EnableXCPCCP();
	}

	private void buttonRefresh_Click(object sender, EventArgs e)
	{
		ReadInputs();
	}

	private void buttonEnable_Click(object sender, EventArgs e)
	{
		if (useManualUnlock)
		{
			UnlockVeDocToEnableXCPCCP();
		}
		else
		{
			PerformServerUnlockAndXcpEnable();
		}
	}

	private void buttonDisable_Click(object sender, EventArgs e)
	{
		DisableXCPCCP();
	}

	private void comboBoxEcuToUnlock_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetChannel((Channel)comboBoxEcuToUnlock.SelectedItem);
	}

	private void textBoxKey1_TextChanged(object sender, EventArgs e)
	{
		if (textBoxKey1.Text.Length == 3)
		{
			textBoxKey2.Focus();
			textBoxKey2.SelectAll();
		}
		UpdateUserInterface();
	}

	private void textBoxKey2_TextChanged(object sender, EventArgs e)
	{
		if (textBoxKey2.Text.Length == 3)
		{
			textBoxKey3.Focus();
			textBoxKey3.SelectAll();
		}
		UpdateUserInterface();
	}

	private void textBoxKey3_TextChanged(object sender, EventArgs e)
	{
		if (textBoxKey3.Text.Length == 3)
		{
			textBoxKey4.Focus();
			textBoxKey4.SelectAll();
		}
		UpdateUserInterface();
	}

	private void textBoxKey4_TextChanged(object sender, EventArgs e)
	{
		if (textBoxKey4.Text.Length == 3)
		{
			buttonEnable.Focus();
		}
		UpdateUserInterface();
	}

	private void buttonSwitchUnlockMode_Click(object sender, EventArgs e)
	{
		useManualUnlock = !useManualUnlock;
		ReadLockConfiguration();
		if (useManualUnlock)
		{
			UpdateUserInterface();
			ReadInputs();
		}
		else
		{
			UpdateUserInterface();
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		textBoxChecksum = new TextBox();
		ChecksumLabel = new System.Windows.Forms.Label();
		label8 = new System.Windows.Forms.Label();
		label1 = new System.Windows.Forms.Label();
		UnlockKeyLabel = new System.Windows.Forms.Label();
		textBoxActivationMode = new TextBox();
		comboBoxEcuToUnlock = new ComboBox();
		flowLayoutPanel1 = new FlowLayoutPanel();
		textBoxKey1 = new TextBox();
		textBoxKey2 = new TextBox();
		textBoxKey3 = new TextBox();
		textBoxKey4 = new TextBox();
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonSwitchUnlockMode = new Button();
		buttonRefresh = new Button();
		buttonEnable = new Button();
		buttonDisable = new Button();
		buttonClose = new Button();
		seekTimeListView1 = new SeekTimeListView();
		RandomNumberLabel = new System.Windows.Forms.Label();
		textBoxChallengeCode = new TextBox();
		IDCodeLabel = new System.Windows.Forms.Label();
		textBoxEcuIdCode = new TextBox();
		XFactorValueLabel = new System.Windows.Forms.Label();
		textBoxXFactor = new TextBox();
		NumberOfKeysLabel = new System.Windows.Forms.Label();
		TranspoderCodeLabel = new System.Windows.Forms.Label();
		textBoxNumberOfCode = new TextBox();
		textBoxTransponderCode = new TextBox();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		flowLayoutPanel1.SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxChecksum, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(ChecksumLabel, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label8, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(UnlockKeyLabel, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxActivationMode, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(comboBoxEcuToUnlock, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(flowLayoutPanel1, 1, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(RandomNumberLabel, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxChallengeCode, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(IDCodeLabel, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxEcuIdCode, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(XFactorValueLabel, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxXFactor, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(NumberOfKeysLabel, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(TranspoderCodeLabel, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxNumberOfCode, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxTransponderCode, 1, 6);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(textBoxChecksum, "textBoxChecksum");
		textBoxChecksum.Name = "textBoxChecksum";
		textBoxChecksum.ReadOnly = true;
		componentResourceManager.ApplyResources(ChecksumLabel, "ChecksumLabel");
		ChecksumLabel.Name = "ChecksumLabel";
		ChecksumLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label8, "label8");
		label8.Name = "label8";
		label8.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(UnlockKeyLabel, "UnlockKeyLabel");
		UnlockKeyLabel.Name = "UnlockKeyLabel";
		UnlockKeyLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxActivationMode, "textBoxActivationMode");
		textBoxActivationMode.Name = "textBoxActivationMode";
		textBoxActivationMode.ReadOnly = true;
		componentResourceManager.ApplyResources(comboBoxEcuToUnlock, "comboBoxEcuToUnlock");
		comboBoxEcuToUnlock.FormattingEnabled = true;
		comboBoxEcuToUnlock.Name = "comboBoxEcuToUnlock";
		comboBoxEcuToUnlock.SelectedIndexChanged += comboBoxEcuToUnlock_SelectedIndexChanged;
		flowLayoutPanel1.Controls.Add(textBoxKey1);
		flowLayoutPanel1.Controls.Add(textBoxKey2);
		flowLayoutPanel1.Controls.Add(textBoxKey3);
		flowLayoutPanel1.Controls.Add(textBoxKey4);
		componentResourceManager.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
		flowLayoutPanel1.Name = "flowLayoutPanel1";
		componentResourceManager.ApplyResources(textBoxKey1, "textBoxKey1");
		textBoxKey1.Name = "textBoxKey1";
		textBoxKey1.TextChanged += textBoxKey1_TextChanged;
		componentResourceManager.ApplyResources(textBoxKey2, "textBoxKey2");
		textBoxKey2.Name = "textBoxKey2";
		textBoxKey2.TextChanged += textBoxKey2_TextChanged;
		componentResourceManager.ApplyResources(textBoxKey3, "textBoxKey3");
		textBoxKey3.Name = "textBoxKey3";
		textBoxKey3.TextChanged += textBoxKey3_TextChanged;
		componentResourceManager.ApplyResources(textBoxKey4, "textBoxKey4");
		textBoxKey4.Name = "textBoxKey4";
		textBoxKey4.TextChanged += textBoxKey4_TextChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonSwitchUnlockMode, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonRefresh, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonEnable, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonDisable, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 4, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonSwitchUnlockMode, "buttonSwitchUnlockMode");
		buttonSwitchUnlockMode.Name = "buttonSwitchUnlockMode";
		buttonSwitchUnlockMode.UseCompatibleTextRendering = true;
		buttonSwitchUnlockMode.UseVisualStyleBackColor = true;
		buttonSwitchUnlockMode.Click += buttonSwitchUnlockMode_Click;
		componentResourceManager.ApplyResources(buttonRefresh, "buttonRefresh");
		buttonRefresh.Name = "buttonRefresh";
		buttonRefresh.UseCompatibleTextRendering = true;
		buttonRefresh.UseVisualStyleBackColor = true;
		buttonRefresh.Click += buttonRefresh_Click;
		componentResourceManager.ApplyResources(buttonEnable, "buttonEnable");
		buttonEnable.Name = "buttonEnable";
		buttonEnable.UseCompatibleTextRendering = true;
		buttonEnable.UseVisualStyleBackColor = true;
		buttonEnable.Click += buttonEnable_Click;
		componentResourceManager.ApplyResources(buttonDisable, "buttonDisable");
		buttonDisable.Name = "buttonDisable";
		buttonDisable.UseCompatibleTextRendering = true;
		buttonDisable.UseVisualStyleBackColor = true;
		buttonDisable.Click += buttonDisable_Click;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 2);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "XCPCCPACTPAN";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		componentResourceManager.ApplyResources(RandomNumberLabel, "RandomNumberLabel");
		RandomNumberLabel.Name = "RandomNumberLabel";
		RandomNumberLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxChallengeCode, "textBoxChallengeCode");
		textBoxChallengeCode.Name = "textBoxChallengeCode";
		textBoxChallengeCode.ReadOnly = true;
		componentResourceManager.ApplyResources(IDCodeLabel, "IDCodeLabel");
		IDCodeLabel.Name = "IDCodeLabel";
		IDCodeLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxEcuIdCode, "textBoxEcuIdCode");
		textBoxEcuIdCode.Name = "textBoxEcuIdCode";
		textBoxEcuIdCode.ReadOnly = true;
		componentResourceManager.ApplyResources(XFactorValueLabel, "XFactorValueLabel");
		XFactorValueLabel.Name = "XFactorValueLabel";
		XFactorValueLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxXFactor, "textBoxXFactor");
		textBoxXFactor.Name = "textBoxXFactor";
		textBoxXFactor.ReadOnly = true;
		componentResourceManager.ApplyResources(NumberOfKeysLabel, "NumberOfKeysLabel");
		NumberOfKeysLabel.Name = "NumberOfKeysLabel";
		NumberOfKeysLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(TranspoderCodeLabel, "TranspoderCodeLabel");
		TranspoderCodeLabel.Name = "TranspoderCodeLabel";
		TranspoderCodeLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxNumberOfCode, "textBoxNumberOfCode");
		textBoxNumberOfCode.Name = "textBoxNumberOfCode";
		textBoxNumberOfCode.ReadOnly = true;
		componentResourceManager.ApplyResources(textBoxTransponderCode, "textBoxTransponderCode");
		textBoxTransponderCode.Name = "textBoxTransponderCode";
		textBoxTransponderCode.ReadOnly = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		flowLayoutPanel1.ResumeLayout(performLayout: false);
		flowLayoutPanel1.PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
