using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Security;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Rating__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string CruisePowerParameterName = "Cruise_Power";

	private const string Rating0BrakePowerQualifier = "DT_STO_ID_Rated_brake_power_for_rat_0_Rated_brake_power_for_rat_0";

	private const string Rating0EngineSpeedQualifier = "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0";

	private const string Rating1BrakePowerQualifier = "DT_STO_ID_Rated_brake_power_for_rat_1_Rated_brake_power_for_rat_1";

	private const string Rating1EngineSpeedQualifier = "DT_STO_ID_Rated_engine_speed_for_rat_1_Rated_engine_speed_for_rat_1";

	private const string MaxEngineTorqueQualifier = "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque";

	private const string MaxTorqueSpeedQualifier = "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed";

	private const string FuelmapDescriptionQualifier = "CO_FuelmapDescription";

	private const string RatingCodeQualifier = "CO_RatingCode";

	private const string CertificationNumberQualifier = "CO_CertificationNumber";

	private const double MinimumRatingThreshold = 50.0;

	private Channel mcm = null;

	private Channel cpc2 = null;

	private Parameter cruisePowerParameter;

	private Button buttonHighPower;

	private Button buttonLowPower;

	private Button buttonCruisePower;

	private Button buttonWriteRating;

	private Label labelSelection;

	private Label label5;

	private Label labelCruisePowerDescription;

	private Label label3;

	private Label labelLowPowerDescription;

	private Label label2;

	private Label labelHighPowerDescription;

	private Label label1;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonClose;

	private System.Windows.Forms.Label labelWarning;

	private TextBox textboxResults;

	private bool HaveReadCruisePower => cruisePowerParameter != null && cruisePowerParameter.HasBeenReadFromEcu;

	private bool Busy => Online && cpc2.CommunicationsState != CommunicationsState.Online;

	private bool CanRead => Online && !Busy && !HaveReadCruisePower;

	private bool CanWrite => Online && !Busy && HaveReadCruisePower && !object.Equals(cruisePowerParameter.Value, cruisePowerParameter.OriginalValue);

	private bool CanClose => !Busy;

	private bool Online => cpc2 != null && cpc2.Online;

	private bool HaveReadDescriptions => mcm != null && mcm.Online && mcm.CommunicationsState != CommunicationsState.ReadEcuInfo && mcm.CommunicationsState != CommunicationsState.OnlineButNotInitialized;

	public UserPanel()
	{
		InitializeComponent();
		buttonHighPower.Click += OnHighPowerClick;
		buttonLowPower.Click += OnLowPowerClick;
		buttonCruisePower.Click += OnCruisePowerClick;
		buttonWriteRating.Click += OnWriteRatingClick;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateChannels();
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			CleanUpChannels();
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
		UpdateUserInterface();
	}

	private void UpdateChannels()
	{
		if (SetCPC2(((CustomPanel)this).GetChannel("CPC04T")) | SetMCM(((CustomPanel)this).GetChannel("MCM21T")))
		{
			UpdateWarningMessage();
		}
	}

	private void CleanUpChannels()
	{
		SetCPC2(null);
		SetMCM(null);
		UpdateWarningMessage();
	}

	private bool SetMCM(Channel mcm)
	{
		bool result = false;
		if (this.mcm != mcm)
		{
			result = true;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
		return result;
	}

	private bool SetCPC2(Channel cpc2)
	{
		bool result = false;
		if (this.cpc2 != cpc2)
		{
			result = true;
			if (this.cpc2 != null)
			{
				this.cpc2.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				if (cruisePowerParameter != null)
				{
					cruisePowerParameter.ParameterUpdateEvent -= OnCruisePowerParameterUpdate;
				}
				this.cpc2.Parameters.ParametersWriteCompleteEvent -= OnParametersWriteComplete;
				this.cpc2.Parameters.ParametersReadCompleteEvent -= OnParametersReadComplete;
				cruisePowerParameter = null;
			}
			this.cpc2 = cpc2;
			if (this.cpc2 != null)
			{
				this.cpc2.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				cruisePowerParameter = this.cpc2.Parameters["Cruise_Power"];
				cruisePowerParameter.ParameterUpdateEvent += OnCruisePowerParameterUpdate;
				this.cpc2.Parameters.ParametersWriteCompleteEvent += OnParametersWriteComplete;
				this.cpc2.Parameters.ParametersReadCompleteEvent += OnParametersReadComplete;
				if (CanRead)
				{
					ReadRating();
				}
			}
		}
		return result;
	}

	private void UpdateWarningMessage()
	{
		bool visible = false;
		if (cpc2 != null && HasUnsentChanges(cpc2))
		{
			visible = true;
		}
		if (mcm != null && HasUnsentChanges(mcm))
		{
			visible = true;
		}
		labelWarning.Visible = visible;
	}

	private static bool HasUnsentChanges(Channel channel)
	{
		bool result = false;
		foreach (Parameter parameter in channel.Parameters)
		{
			if ((parameter.Qualifier != "Cruise_Power" || channel.Ecu.Name != "CPC04T") && !object.Equals(parameter.Value, parameter.OriginalValue))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private string EcuInfoValue(Channel channel, string Qualifier)
	{
		string result = string.Empty;
		EcuInfo ecuInfo = channel.EcuInfos[Qualifier];
		if (ecuInfo == null)
		{
			ecuInfo = channel.EcuInfos.GetItemContaining(Qualifier);
		}
		if (ecuInfo != null && !string.IsNullOrEmpty(ecuInfo.Value))
		{
			bool flag = false;
			Conversion conversion = Converter.GlobalInstance.GetConversion(ecuInfo.Units);
			if (conversion != null && Conversion.CanConvert((object)ecuInfo.Value))
			{
				result = $"{Math.Round(conversion.Convert((object)ecuInfo.Value))} {conversion.OutputUnit}";
				flag = true;
			}
			if (!flag)
			{
				result = $"{ecuInfo.Value.ToString()} {ecuInfo.Units}";
			}
		}
		return result;
	}

	private bool IsValidPowerRating(string powerRating)
	{
		bool result = false;
		Conversion conversion = Converter.GlobalInstance.GetConversion("KW");
		double num = 50.0;
		if (conversion != null)
		{
			num = conversion.Convert(num);
		}
		if (double.TryParse(powerRating.Remove(powerRating.IndexOf(" ")), out var result2))
		{
			result = result2 > num;
		}
		return result;
	}

	private void UpdateUserInterface()
	{
		bool flag = HaveReadCruisePower && cruisePowerParameter.Choices.Count > 0 && !Busy;
		bool flag2 = HaveReadCruisePower && cruisePowerParameter.Choices.Count > 1 && !Busy;
		bool flag3 = HaveReadCruisePower && cruisePowerParameter.Choices.Count > 2 && !Busy;
		bool canWrite = CanWrite;
		((Control)(object)labelHighPowerDescription).Text = "";
		((Control)(object)labelLowPowerDescription).Text = "";
		((Control)(object)labelCruisePowerDescription).Text = "";
		((Control)(object)labelSelection).Text = "";
		if (HaveReadCruisePower)
		{
			Choice choice = cruisePowerParameter.Value as Choice;
			switch (Convert.ToInt32(choice.RawValue))
			{
			case 0:
				flag = false;
				break;
			case 1:
				flag2 = false;
				break;
			case 2:
				flag3 = false;
				break;
			}
			((Control)(object)labelSelection).Text = choice.ToString();
		}
		if (HaveReadDescriptions)
		{
			string text = EcuInfoValue(mcm, "DT_STO_ID_Rated_brake_power_for_rat_0_Rated_brake_power_for_rat_0");
			string text2 = EcuInfoValue(mcm, "DT_STO_ID_Rated_brake_power_for_rat_1_Rated_brake_power_for_rat_1");
			flag = flag && !Busy && IsValidPowerRating(text);
			flag2 = flag2 && !Busy && IsValidPowerRating(text2);
			flag3 = flag3 && !Busy && IsValidPowerRating(text) && IsValidPowerRating(text2);
			((Control)(object)labelHighPowerDescription).Text = string.Format(Resources.MessageFormat_Power01Torque231, text, EcuInfoValue(mcm, "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0"), EcuInfoValue(mcm, "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque"), EcuInfoValue(mcm, "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed"));
			((Control)(object)labelLowPowerDescription).Text = string.Format(Resources.MessageFormat_Power01Torque23, text2, EcuInfoValue(mcm, "DT_STO_ID_Rated_engine_speed_for_rat_1_Rated_engine_speed_for_rat_1"), EcuInfoValue(mcm, "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque"), EcuInfoValue(mcm, "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed"));
			((Control)(object)labelCruisePowerDescription).Text = string.Format(Resources.MessageFormat_Power012Torque34, text2, text, EcuInfoValue(mcm, "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0"), EcuInfoValue(mcm, "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque"), EcuInfoValue(mcm, "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed"));
		}
		buttonClose.Enabled = CanClose;
		buttonHighPower.Enabled = flag;
		buttonLowPower.Enabled = flag2;
		buttonCruisePower.Enabled = flag3;
		buttonWriteRating.Enabled = canWrite;
	}

	private void ClearResults()
	{
		textboxResults.Text = string.Empty;
	}

	private void ReportResult(string text)
	{
		textboxResults.Text = textboxResults.Text + text + "\r\n";
		textboxResults.SelectionStart = textboxResults.TextLength;
		textboxResults.SelectionLength = 0;
		textboxResults.ScrollToCaret();
		((CustomPanel)this).AddStatusMessage(text);
	}

	private bool Unlock(Channel channel)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		bool result = true;
		if (PasswordManager.HasPasswords(channel))
		{
			PasswordManager val = PasswordManager.Create(channel);
			if (val != null && val.Valid)
			{
				try
				{
					ReportResult(Resources.Message_AcquiringDeviceLockStatus);
					bool[] array = val.AcquireRelevantListStatus((ProgressBar)null);
					bool flag = false;
					bool[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i])
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						ReportResult(Resources.Message_DeviceIsLocked);
						PasswordEntryDialog val2 = new PasswordEntryDialog(channel, array, val);
						if (((Form)(object)val2).ShowDialog() != DialogResult.OK)
						{
							result = false;
							ReportResult(Resources.Message_DeviceWasNotUnlockedByUser);
						}
						else
						{
							ReportResult(Resources.Message_DeviceWasUnlocked);
						}
					}
					else
					{
						ReportResult(Resources.Message_DeviceIsUnlocked);
					}
				}
				catch (CaesarException)
				{
					result = false;
					ReportResult(Resources.Message_ErrorWhileUnlockingDevice);
				}
			}
		}
		return result;
	}

	private void UpdateEcuInfo(string qualifier)
	{
		mcm.EcuInfos[qualifier]?.Read(synchronous: false);
	}

	private void OnParametersReadComplete(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_SettingSuccessfullyRead);
		}
		else
		{
			ReportResult(Resources.Message_ErrorWhileReadingSetting);
		}
	}

	private void OnParametersWriteComplete(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_SettingSuccessfullySent1);
			if (mcm != null && mcm.CommunicationsState == CommunicationsState.Online)
			{
				UpdateEcuInfo("CO_FuelmapDescription");
				UpdateEcuInfo("CO_RatingCode");
				UpdateEcuInfo("CO_CertificationNumber");
			}
		}
		else
		{
			ReportResult(Resources.Message_ErrorWhileSendingSetting);
		}
	}

	private void ReadRating()
	{
		if (CanRead)
		{
			ClearResults();
			ReportResult(Resources.Message_ReadingSettingFromCPC4);
			cpc2.Parameters.ReadGroup(cruisePowerParameter.GroupQualifier, fromCache: true, synchronous: false);
		}
	}

	private void OnWriteRatingClick(object sender, EventArgs e)
	{
		if (CanWrite)
		{
			ClearResults();
			if (Unlock(cpc2))
			{
				ReportResult(Resources.Message_SendingSettingToCPC4);
				cpc2.Parameters.Write(synchronous: false);
			}
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (CanRead && !HaveReadCruisePower)
		{
			ReadRating();
		}
		else
		{
			UpdateUserInterface();
		}
	}

	private void OnCruisePowerParameterUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnHighPowerClick(object sender, EventArgs e)
	{
		if (cruisePowerParameter != null)
		{
			cruisePowerParameter.Value = cruisePowerParameter.Choices[0];
			UpdateUserInterface();
		}
	}

	private void OnLowPowerClick(object sender, EventArgs e)
	{
		if (cruisePowerParameter != null)
		{
			cruisePowerParameter.Value = cruisePowerParameter.Choices[1];
			UpdateUserInterface();
		}
	}

	private void OnCruisePowerClick(object sender, EventArgs e)
	{
		if (cruisePowerParameter != null)
		{
			cruisePowerParameter.Value = cruisePowerParameter.Choices[2];
			UpdateUserInterface();
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonWriteRating = new Button();
		labelSelection = new Label();
		textboxResults = new TextBox();
		label1 = new Label();
		label5 = new Label();
		labelCruisePowerDescription = new Label();
		labelHighPowerDescription = new Label();
		label2 = new Label();
		labelLowPowerDescription = new Label();
		label3 = new Label();
		buttonClose = new Button();
		labelWarning = new System.Windows.Forms.Label();
		buttonHighPower = new Button();
		buttonLowPower = new Button();
		buttonCruisePower = new Button();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonWriteRating, 2, 8);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelSelection, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxResults, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label5, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelCruisePowerDescription, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelHighPowerDescription, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelLowPowerDescription, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label3, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 2, 10);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelWarning, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonHighPower, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonLowPower, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonCruisePower, 2, 4);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonWriteRating, "buttonWriteRating");
		buttonWriteRating.Name = "buttonWriteRating";
		buttonWriteRating.UseCompatibleTextRendering = true;
		buttonWriteRating.UseVisualStyleBackColor = true;
		labelSelection.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(labelSelection, "labelSelection");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)labelSelection, 2);
		((Control)(object)labelSelection).Name = "labelSelection";
		labelSelection.Orientation = (TextOrientation)1;
		labelSelection.UseSystemColors = true;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)textboxResults, 3);
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		label1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)(object)label1).Name = "label1";
		label1.Orientation = (TextOrientation)1;
		label1.UseSystemColors = true;
		label5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)(object)label5).Name = "label5";
		label5.Orientation = (TextOrientation)1;
		label5.UseSystemColors = true;
		labelCruisePowerDescription.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)labelCruisePowerDescription, 3);
		componentResourceManager.ApplyResources(labelCruisePowerDescription, "labelCruisePowerDescription");
		((Control)(object)labelCruisePowerDescription).Name = "labelCruisePowerDescription";
		labelCruisePowerDescription.Orientation = (TextOrientation)1;
		labelCruisePowerDescription.UseSystemColors = true;
		labelHighPowerDescription.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)labelHighPowerDescription, 3);
		componentResourceManager.ApplyResources(labelHighPowerDescription, "labelHighPowerDescription");
		((Control)(object)labelHighPowerDescription).Name = "labelHighPowerDescription";
		labelHighPowerDescription.Orientation = (TextOrientation)1;
		labelHighPowerDescription.UseSystemColors = true;
		label2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)(object)label2).Name = "label2";
		label2.Orientation = (TextOrientation)1;
		label2.UseSystemColors = true;
		labelLowPowerDescription.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)labelLowPowerDescription, 3);
		componentResourceManager.ApplyResources(labelLowPowerDescription, "labelLowPowerDescription");
		((Control)(object)labelLowPowerDescription).Name = "labelLowPowerDescription";
		labelLowPowerDescription.Orientation = (TextOrientation)1;
		labelLowPowerDescription.UseSystemColors = true;
		label3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)(object)label3).Name = "label3";
		label3.Orientation = (TextOrientation)1;
		label3.UseSystemColors = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		labelWarning.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)labelWarning, 3);
		componentResourceManager.ApplyResources(labelWarning, "labelWarning");
		labelWarning.ForeColor = Color.Red;
		labelWarning.Name = "labelWarning";
		labelWarning.UseCompatibleTextRendering = true;
		labelWarning.UseMnemonic = false;
		componentResourceManager.ApplyResources(buttonHighPower, "buttonHighPower");
		buttonHighPower.Name = "buttonHighPower";
		buttonHighPower.UseCompatibleTextRendering = true;
		buttonHighPower.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonLowPower, "buttonLowPower");
		buttonLowPower.Name = "buttonLowPower";
		buttonLowPower.UseCompatibleTextRendering = true;
		buttonLowPower.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonCruisePower, "buttonCruisePower");
		buttonCruisePower.Name = "buttonCruisePower";
		buttonCruisePower.UseCompatibleTextRendering = true;
		buttonCruisePower.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_Rating");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
