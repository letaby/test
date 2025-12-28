using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code__MY13_.panel;

public class UserPanel : CustomPanel
{
	private enum Stage
	{
		Idle,
		ReadParameters,
		Reset,
		ReadResetParameters,
		WriteParameters,
		Stopping,
		Finish,
		Unknown
	}

	private enum Reason
	{
		Succeeded,
		FailedParametersRead,
		FailedParametersWrite,
		FailedServiceExecute,
		FailedService,
		FailedUnlock,
		Closing,
		Disconnected,
		UnknownStage,
		NoParametersChanged
	}

	private const string ResetEEPROMServiceList = "RT_Reload_Original_CPC_Factory_Settings_Start_Routine_Status;FN_KeyOffOnReset";

	private const string EEPROMChecksumDTC = "740202";

	private const string CommonVIN = "CO_VIN";

	private Channel cpc;

	private Stage currentStage = Stage.Idle;

	private Dictionary<string, object> parameters = new Dictionary<string, object>();

	private System.Windows.Forms.Label label1;

	private Checkmark checkmarkDTC;

	private System.Windows.Forms.Label labelDTC;

	private TextBox textBoxOutput;

	private Button buttonClose;

	private Button buttonStart;

	private bool CanClose => !Working;

	private bool Online => cpc != null && cpc.Online;

	private bool FaultIsPresent
	{
		get
		{
			if (Online && cpc.FaultCodes.HaveBeenReadFromEcu)
			{
				FaultCode faultCode = cpc.FaultCodes["740202"];
				if (faultCode != null)
				{
					FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
					if (current != null && current.Active == ActiveStatus.Active)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private bool EcuIsBusy => Online && cpc.CommunicationsState != CommunicationsState.Online;

	private bool CanStart => !Working && Online && FaultIsPresent && !EcuIsBusy;

	private Stage CurrentStage
	{
		get
		{
			return currentStage;
		}
		set
		{
			if (currentStage != value)
			{
				currentStage = value;
				UpdateUserInterface();
				Application.DoEvents();
			}
		}
	}

	private bool Working => currentStage != Stage.Idle;

	public UserPanel()
	{
		InitializeComponent();
		buttonStart.Click += OnStartClick;
		currentStage = Stage.Idle;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		SetCPC(((CustomPanel)this).GetChannel("CPC04T"));
	}

	public override void OnChannelsChanged()
	{
		SetCPC(((CustomPanel)this).GetChannel("CPC04T"));
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			StopWork(Reason.Closing);
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			SetCPC(null);
		}
	}

	private void OnStartClick(object sender, EventArgs e)
	{
		ClearOutput();
		if (parameters.Count > 0)
		{
			CurrentStage = Stage.Reset;
		}
		else
		{
			CurrentStage = Stage.ReadParameters;
		}
		PerformCurrentStage();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnFaultCodesUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnParametersReadComplete(object sender, ResultEventArgs e)
	{
		cpc.Parameters.ParametersReadCompleteEvent -= OnParametersReadComplete;
		if (e.Succeeded)
		{
			Stage stage = Stage.Unknown;
			switch (CurrentStage)
			{
			case Stage.ReadParameters:
				parameters.Clear();
				foreach (Parameter parameter in cpc.Parameters)
				{
					parameters.Add(parameter.Qualifier, parameter.Value);
				}
				stage = Stage.Reset;
				break;
			case Stage.ReadResetParameters:
				stage = Stage.WriteParameters;
				break;
			}
			CurrentStage = stage;
			PerformCurrentStage();
		}
		else
		{
			StopWork(Reason.FailedParametersRead);
		}
	}

	private void OnParametersWriteComplete(object sender, ResultEventArgs e)
	{
		cpc.Parameters.ParametersWriteCompleteEvent -= OnParametersWriteComplete;
		if (e.Succeeded)
		{
			CurrentStage = Stage.Finish;
			PerformCurrentStage();
		}
		else
		{
			StopWork(Reason.FailedParametersRead);
		}
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		cpc.Services.ServiceCompleteEvent -= OnServiceComplete;
		if (e.Succeeded)
		{
			CurrentStage = Stage.ReadResetParameters;
			PerformCurrentStage();
		}
		else
		{
			StopWork(Reason.FailedServiceExecute);
		}
	}

	private void ClearOutput()
	{
		textBoxOutput.Text = string.Empty;
	}

	private void ReportResult(string text)
	{
		textBoxOutput.Text = textBoxOutput.Text + text + "\r\n";
		textBoxOutput.SelectionStart = textBoxOutput.TextLength;
		textBoxOutput.SelectionLength = 0;
		textBoxOutput.ScrollToCaret();
	}

	private void UpdateUserInterface()
	{
		buttonStart.Enabled = CanStart;
		buttonClose.Enabled = CanClose;
		if (Online)
		{
			checkmarkDTC.Checked = !FaultIsPresent;
			if (!Working)
			{
				labelDTC.Text = (FaultIsPresent ? Resources.Message_TheEEPROMChecksumFailureFaultCodeIsPresentAndMustBeCleared : Resources.Message_TheEEPROMChecksumFailureFaultCodeIsNotPresentNoActionIsNecessary);
			}
			else
			{
				labelDTC.Text = Resources.Message_ProcessingPleaseWait;
			}
		}
		else
		{
			checkmarkDTC.Checked = false;
			labelDTC.Text = Resources.Message_TheCPC04TIsNotOnlineSoFaultCodesCouldNotBeRead;
		}
	}

	private void SetCPC(Channel channel)
	{
		if (cpc != channel)
		{
			if (cpc != null)
			{
				cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				cpc.FaultCodes.FaultCodesUpdateEvent -= OnFaultCodesUpdate;
				StopWork(Reason.Disconnected);
			}
			cpc = channel;
			if (cpc != null)
			{
				cpc.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				cpc.FaultCodes.FaultCodesUpdateEvent += OnFaultCodesUpdate;
			}
		}
	}

	private void PerformCurrentStage()
	{
		if (cpc != null && cpc.Online)
		{
			StringDictionary stringDictionary = new StringDictionary();
			switch (CurrentStage)
			{
			case Stage.Idle:
				break;
			case Stage.ReadParameters:
				ReportResult(Resources.Message_ReadingParameters);
				cpc.Parameters.ParametersReadCompleteEvent += OnParametersReadComplete;
				cpc.Parameters.ReadAll(synchronous: false);
				break;
			case Stage.Reset:
				ReportResult(Resources.Message_PreparingForEEPROMReset);
				if (Unlock())
				{
					ReportResult(Resources.Message_PerformingEEPROMReset);
					ExecuteService("RT_Reload_Original_CPC_Factory_Settings_Start_Routine_Status;FN_KeyOffOnReset");
				}
				break;
			case Stage.ReadResetParameters:
				ReportResult(Resources.Message_ReadingDefaultParametersAfterReset);
				cpc.Parameters.ParametersReadCompleteEvent += OnParametersReadComplete;
				cpc.Parameters.ReadAll(synchronous: false);
				break;
			case Stage.WriteParameters:
			{
				ReportResult(Resources.Message_AssigningParameters);
				int num = AssignParameters();
				if (num > 0)
				{
					ReportResult(Resources.Message_UpdateSeed);
					cpc.EcuInfos["CO_VIN"].Read(synchronous: true);
					ReportResult(string.Format(Resources.MessageFormat_PreparingToWriteBack0Parameters, num));
					if (Unlock())
					{
						ReportResult(string.Format(Resources.MessageFormat_Writing0Parameters, num));
						cpc.Parameters.ParametersWriteCompleteEvent += OnParametersWriteComplete;
						cpc.Parameters.Write(synchronous: false);
					}
				}
				else
				{
					StopWork(Reason.NoParametersChanged);
				}
				break;
			}
			case Stage.Finish:
				StopWork(Reason.Succeeded);
				break;
			case Stage.Unknown:
				StopWork(Reason.UnknownStage);
				break;
			case Stage.Stopping:
				break;
			}
		}
		else
		{
			StopWork(Reason.Disconnected);
		}
	}

	private int AssignParameters()
	{
		int num = 0;
		foreach (KeyValuePair<string, object> parameter2 in parameters)
		{
			Parameter parameter = cpc.Parameters[parameter2.Key];
			if (parameter != null && parameter2.Value != null && !object.Equals(parameter.Value, parameter2.Value))
			{
				parameter.Value = parameter2.Value;
				num++;
			}
		}
		return num;
	}

	private void ExecuteService(string qualifier)
	{
		cpc.Services.ServiceCompleteEvent += OnServiceComplete;
		if (cpc.Services.Execute("RT_Reload_Original_CPC_Factory_Settings_Start_Routine_Status;FN_KeyOffOnReset", synchronous: false) == 0)
		{
			cpc.Services.ServiceCompleteEvent -= OnServiceComplete;
			StopWork(Reason.FailedService);
		}
	}

	private bool Unlock()
	{
		try
		{
			cpc.Extension.Invoke("Unlock", null);
			return true;
		}
		catch (CaesarException)
		{
			StopWork(Reason.FailedUnlock);
			return false;
		}
	}

	private void StopWork(Reason reason)
	{
		if (CurrentStage == Stage.Stopping || CurrentStage == Stage.Idle)
		{
			return;
		}
		Stage stage = CurrentStage;
		CurrentStage = Stage.Stopping;
		if (reason == Reason.Succeeded)
		{
			if (stage != Stage.Finish)
			{
				throw new InvalidOperationException();
			}
			ReportResult(Resources.Message_Complete);
			buttonStart.Text = Resources.Message_Start;
			parameters.Clear();
		}
		else
		{
			if (reason != Reason.NoParametersChanged)
			{
				buttonStart.Text = Resources.Message_Retry;
			}
			ReportResult(Resources.Message_TheProcedureFailedToComplete);
			switch (reason)
			{
			case Reason.Disconnected:
				ReportResult(Resources.Message_TheCPC04TWasDisconnected);
				break;
			case Reason.FailedServiceExecute:
				ReportResult(Resources.Message_FailedToExecuteService);
				break;
			case Reason.FailedParametersRead:
				ReportResult(Resources.Message_FailedToReadExistingParameters);
				break;
			case Reason.FailedService:
				ReportResult(Resources.Message_FailedToObtainService);
				break;
			case Reason.FailedUnlock:
				ReportResult(Resources.Message_FailedToUnlock);
				break;
			case Reason.UnknownStage:
				ReportResult(Resources.Message_UnknownStage);
				break;
			case Reason.NoParametersChanged:
				ReportResult(Resources.Message_NoParametersAreChangedFromDefaultUseProgramDeviceToRestoreServerConfiguration);
				break;
			}
		}
		CurrentStage = Stage.Idle;
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		label1 = new System.Windows.Forms.Label();
		checkmarkDTC = new Checkmark();
		labelDTC = new System.Windows.Forms.Label();
		textBoxOutput = new TextBox();
		buttonClose = new Button();
		buttonStart = new Button();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkDTC, "checkmarkDTC");
		((Control)(object)checkmarkDTC).Name = "checkmarkDTC";
		componentResourceManager.ApplyResources(labelDTC, "labelDTC");
		labelDTC.Name = "labelDTC";
		labelDTC.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.Cancel;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add(buttonStart);
		((Control)this).Controls.Add(buttonClose);
		((Control)this).Controls.Add(textBoxOutput);
		((Control)this).Controls.Add(labelDTC);
		((Control)this).Controls.Add((Control)(object)checkmarkDTC);
		((Control)this).Controls.Add(label1);
		((Control)this).Name = "UserPanel";
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
