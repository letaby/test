using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.GPRS_Configuration.panel;

public class UserPanel : CustomPanel
{
	private enum PartStatus
	{
		Pass,
		HardwareFail,
		SoftwareFail,
		HardwareUnknown,
		SoftwareUnknown
	}

	private enum GprsStatus
	{
		Pass,
		Fail,
		Unknown,
		IgnoreGprs
	}

	private const string GprsConfigValue = "1F8B080000000000000073767367656060B8C9758D914B994B3625B33839BF2CB5A852AFB4582F393F29512F2531333727B508C8C915B837935988D9D0DC408AD9D8D0408937B120CF502FA5242F112469A5CC2597985E9E999759A2575094A297925A52949F59929C9F97979A5C02D6FEC7CE499E4B0AA8089782BF761EAC010D4F3922181318B28430151511E3C22A547701000CC0CF68E1000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

	private const string GprsConfigReadQualifier = "DT_STO_Gprs_Config_1_gprsConfig";

	private const string GprsConfigWriteQualifier = "DL_Gprs_Config_1";

	private const string AntennaDiagnosticGroupQualifier = "VCD_PID_Antenna_diagnostic";

	private const string HardResetQualifier = "FN_HardReset";

	private const string HardwarePartNumberQualifier = "CO_HardwarePartNumber";

	private const string SoftwarePartNumberQualifier = "CO_SoftwarePartNumber";

	private readonly List<string> AffectedHardwarePartNumbers = new List<string> { "66-05466-001", "66-10777-001", "66-13928-001", "66-13931-001" };

	private readonly List<string> AffectedSoftwarePartNumbers = new List<string> { "A0014487260-001", "A0014485460-001" };

	private Channel channel;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView1;

	private Checkmark checkmarkAffectedPartNumber;

	private Checkmark checkmarkGprsConfigCorrect;

	private Button buttonFixGprs;

	private Label labelAffectedPartNumber;

	private Button buttonClose;

	private Label labelGprsConfigCorrect;

	private bool Busy { get; set; }

	public bool Result { get; private set; }

	public string ResultMessage { get; private set; }

	private PartStatus IsAffectedPartNumber
	{
		get
		{
			if (channel != null && channel.EcuInfos["CO_HardwarePartNumber"] != null && channel.EcuInfos["CO_HardwarePartNumber"].Value != null)
			{
				if (channel.EcuInfos["CO_SoftwarePartNumber"] != null && channel.EcuInfos["CO_SoftwarePartNumber"].Value != null)
				{
					Part currentHardwarePart = new Part(channel.EcuInfos["CO_HardwarePartNumber"].Value);
					if (AffectedHardwarePartNumbers.Where((string x) => PartExtensions.IsEqual(currentHardwarePart, x)).Any())
					{
						Part currentSoftwarePart = new Part(channel.EcuInfos["CO_SoftwarePartNumber"].Value);
						if (AffectedSoftwarePartNumbers.Where((string x) => PartExtensions.IsEqual(currentSoftwarePart, x)).Any())
						{
							return PartStatus.Pass;
						}
						return PartStatus.SoftwareFail;
					}
					return PartStatus.HardwareFail;
				}
				return PartStatus.SoftwareUnknown;
			}
			return PartStatus.HardwareUnknown;
		}
	}

	private GprsStatus CurrentGprsConfigCorrect
	{
		get
		{
			if (IsAffectedPartNumber == PartStatus.Pass)
			{
				if (channel != null && channel.EcuInfos["DT_STO_Gprs_Config_1_gprsConfig"] != null && channel.EcuInfos["DT_STO_Gprs_Config_1_gprsConfig"].Value != null)
				{
					if (channel.EcuInfos["DT_STO_Gprs_Config_1_gprsConfig"].Value.Equals("1F8B080000000000000073767367656060B8C9758D914B994B3625B33839BF2CB5A852AFB4582F393F29512F2531333727B508C8C915B837935988D9D0DC408AD9D8D0408937B120CF502FA5242F112469A5CC2597985E9E999759A2575094A297925A52949F59929C9F97979A5C02D6FEC7CE499E4B0AA8089782BF761EAC010D4F3922181318B28430151511E3C22A547701000CC0CF68E1000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", StringComparison.OrdinalIgnoreCase))
					{
						return GprsStatus.Pass;
					}
					return GprsStatus.Fail;
				}
				return GprsStatus.Unknown;
			}
			return GprsStatus.IgnoreGprs;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetCTP(((CustomPanel)this).GetChannel("CTP01T", (ChannelLookupOptions)3));
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		ResetResults();
		SetCTP(((CustomPanel)this).GetChannel("CTP01T", (ChannelLookupOptions)3));
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = Busy;
		if (!e.Cancel)
		{
			((Control)this).Tag = new object[2] { Result, ResultMessage };
			buttonClose.DialogResult = (Result ? DialogResult.Yes : DialogResult.No);
			if (((ContainerControl)this).ParentForm != null)
			{
				((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			}
		}
	}

	private void SetCTP(Channel ctp)
	{
		if (channel != ctp)
		{
			ResetResults();
			if (Busy)
			{
				ResultMessage = Resources.Message_CTP01TDeviceChangedDuringProcess;
				AddLogLabel(Resources.Message_CTP01TDeviceChangedDuringProcess);
				Busy = false;
			}
			if (channel != null)
			{
				channel.EcuInfos.EcuInfosReadCompleteEvent -= EcuInfos_EcuInfosReadCompleteEvent;
			}
			channel = ctp;
			if (channel != null)
			{
				channel.EcuInfos.EcuInfosReadCompleteEvent += EcuInfos_EcuInfosReadCompleteEvent;
			}
		}
		UpdateUserInterface();
	}

	private void ResetResults()
	{
		Result = false;
		ResultMessage = Resources.Message_None;
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
	}

	private void UpdateUserInterface()
	{
		switch (IsAffectedPartNumber)
		{
		case PartStatus.Pass:
			checkmarkAffectedPartNumber.CheckState = CheckState.Checked;
			((Control)(object)labelAffectedPartNumber).Text = Resources.Message_ConnectedCTP01TIsAllowed;
			break;
		case PartStatus.HardwareFail:
			checkmarkAffectedPartNumber.CheckState = CheckState.Unchecked;
			ResultMessage = Resources.Message_ConnectedCTP01THardwareIsNotSupported;
			((Control)(object)labelAffectedPartNumber).Text = Resources.Message_ConnectedCTP01THardwareIsNotSupported;
			break;
		case PartStatus.SoftwareFail:
			checkmarkAffectedPartNumber.CheckState = CheckState.Unchecked;
			ResultMessage = Resources.Message_ConnectedCTP01TSoftwareIsNotSupported;
			((Control)(object)labelAffectedPartNumber).Text = Resources.Message_ConnectedCTP01TSoftwareIsNotSupported;
			break;
		case PartStatus.HardwareUnknown:
			checkmarkAffectedPartNumber.CheckState = CheckState.Indeterminate;
			ResultMessage = Resources.Message_CannotConfirmConnectedCTP01THardwareVersion;
			((Control)(object)labelAffectedPartNumber).Text = Resources.Message_CannotConfirmConnectedCTP01THardwareVersion;
			break;
		case PartStatus.SoftwareUnknown:
			checkmarkAffectedPartNumber.CheckState = CheckState.Indeterminate;
			ResultMessage = Resources.Message_CannotConfirmConnectedCTP01TSoftwareVersion;
			((Control)(object)labelAffectedPartNumber).Text = Resources.Message_CannotConfirmConnectedCTP01TSoftwareVersion;
			break;
		}
		switch (CurrentGprsConfigCorrect)
		{
		case GprsStatus.Pass:
			checkmarkGprsConfigCorrect.CheckState = CheckState.Checked;
			((Control)(object)labelGprsConfigCorrect).Text = Resources.Message_GPRSConfigurationIsCORRECT;
			break;
		case GprsStatus.Fail:
			checkmarkGprsConfigCorrect.CheckState = CheckState.Unchecked;
			((Control)(object)labelGprsConfigCorrect).Text = Resources.Message_GPRSConfigurationIsNOTCORRECT;
			break;
		case GprsStatus.Unknown:
			checkmarkGprsConfigCorrect.CheckState = CheckState.Indeterminate;
			((Control)(object)labelGprsConfigCorrect).Text = Resources.Message_GPRSConfigurationCanNotBeRead;
			break;
		case GprsStatus.IgnoreGprs:
			checkmarkGprsConfigCorrect.CheckState = CheckState.Indeterminate;
			((Control)(object)labelGprsConfigCorrect).Text = Resources.Message_GPRSNotApplicableToCurrentUnit;
			break;
		}
		buttonFixGprs.Enabled = channel != null && channel.Online && checkmarkAffectedPartNumber.CheckState == CheckState.Checked && checkmarkGprsConfigCorrect.CheckState == CheckState.Unchecked && !Busy;
	}

	private void SendCorrectGprsConfig()
	{
		Service service = channel.Services["DL_Gprs_Config_1"];
		if (service != null)
		{
			AddLogLabel(Resources.Message_WritingGPRSConfigurationValue);
			Busy = true;
			UpdateUserInterface();
			service.ServiceCompleteEvent += gprsWriteService_ServiceCompleteEvent;
			service.InputValues[0].Value = "1F8B080000000000000073767367656060B8C9758D914B994B3625B33839BF2CB5A852AFB4582F393F29512F2531333727B508C8C915B837935988D9D0DC408AD9D8D0408937B120CF502FA5242F112469A5CC2597985E9E999759A2575094A297925A52949F59929C9F97979A5C02D6FEC7CE499E4B0AA8089782BF761EAC010D4F3922181318B28430151511E3C22A547701000CC0CF68E1000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
			service.Execute(synchronous: false);
		}
		else
		{
			AddLogLabel(Resources.Message_CouldNotFindGPRSWriteService);
		}
	}

	private void gprsWriteService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= gprsWriteService_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				AddLogLabel(Resources.Message_GPRSConfigurationWasSuccessfullyUpdated);
				EnableAntennaDiagnostics();
			}
			else
			{
				Result = false;
				ResultMessage = Resources.Message_FailedToUpdateGPRSConfiguration;
				AddLogLabel(Resources.Message_FailedToUpdateGPRSConfiguration);
				Busy = false;
			}
		}
		else
		{
			AddLogLabel(Resources.Message_UnknownErrorWhenExecutingGPRSWrite);
			Busy = false;
		}
		UpdateUserInterface();
	}

	private void EnableAntennaDiagnostics()
	{
		int num = 0;
		foreach (Parameter item in channel.Parameters.Where((Parameter p) => p.GroupQualifier.Equals("VCD_PID_Antenna_diagnostic", StringComparison.OrdinalIgnoreCase)))
		{
			num++;
			item.Value = item.Choices.GetItemFromRawValue(1);
			item.Marked = true;
		}
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.Message_EnablingAntennaDiagnosticsFormat, num));
		channel.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersWriteCompleteEvent;
		channel.Parameters.Write(synchronous: false);
	}

	private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		channel.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersWriteCompleteEvent;
		if (e.Succeeded)
		{
			Result = true;
			ResultMessage = Resources.Message_ProcessCompletedSuccessfully;
			AddLogLabel(Resources.Message_AntennaDiagnosticsHaveBeenEnabled);
			AddLogLabel(Resources.Message_ProcessCompletedSuccessfully);
		}
		else
		{
			Result = false;
			ResultMessage = Resources.Message_AntennaDiagnosticsEnableFailedToWrite;
			AddLogLabel(Resources.Message_AntennaDiagnosticsEnableFailedToWrite);
		}
		channel.EcuInfos.Read(synchronous: false);
		Busy = false;
	}

	private void EcuInfos_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void buttonFixGprs_Click(object sender, EventArgs e)
	{
		ResetResults();
		SendCorrectGprsConfig();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		checkmarkAffectedPartNumber = new Checkmark();
		checkmarkGprsConfigCorrect = new Checkmark();
		buttonFixGprs = new Button();
		labelAffectedPartNumber = new Label();
		labelGprsConfigCorrect = new Label();
		buttonClose = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkAffectedPartNumber, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkGprsConfigCorrect, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonFixGprs, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelAffectedPartNumber, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelGprsConfigCorrect, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 2, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "GPRSConfiguration";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(checkmarkAffectedPartNumber, "checkmarkAffectedPartNumber");
		((Control)(object)checkmarkAffectedPartNumber).Name = "checkmarkAffectedPartNumber";
		componentResourceManager.ApplyResources(checkmarkGprsConfigCorrect, "checkmarkGprsConfigCorrect");
		((Control)(object)checkmarkGprsConfigCorrect).Name = "checkmarkGprsConfigCorrect";
		componentResourceManager.ApplyResources(buttonFixGprs, "buttonFixGprs");
		buttonFixGprs.Name = "buttonFixGprs";
		buttonFixGprs.UseCompatibleTextRendering = true;
		buttonFixGprs.UseVisualStyleBackColor = true;
		buttonFixGprs.Click += buttonFixGprs_Click;
		labelAffectedPartNumber.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelAffectedPartNumber, 2);
		componentResourceManager.ApplyResources(labelAffectedPartNumber, "labelAffectedPartNumber");
		((Control)(object)labelAffectedPartNumber).Name = "labelAffectedPartNumber";
		labelAffectedPartNumber.Orientation = (TextOrientation)1;
		labelGprsConfigCorrect.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelGprsConfigCorrect, 2);
		componentResourceManager.ApplyResources(labelGprsConfigCorrect, "labelGprsConfigCorrect");
		((Control)(object)labelGprsConfigCorrect).Name = "labelGprsConfigCorrect";
		labelGprsConfigCorrect.Orientation = (TextOrientation)1;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
