using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel;

public class UserPanel : CustomPanel
{
	private enum Stage
	{
		Idle,
		WriteVins,
		WaitForIgnitionOffDisconnection,
		WaitForIgnitionOnReconnection,
		Stopping,
		Finish,
		Unknown
	}

	private enum Reason
	{
		Succeeded,
		Closing,
		Disconnected,
		UnknownStage,
		NoVinsChanged,
		NoVinMaster
	}

	private const string VINMasterProperty = "VINMaster";

	private const string VINParameter = "VIN";

	private const string VINEcuInfo = "CO_VIN";

	private Stage currentStage = Stage.Idle;

	private Dictionary<string, object> parameters = new Dictionary<string, object>();

	private List<Channel> channelsToWorkWith = new List<Channel>();

	private List<Channel> channelsToWriteVINsFor = new List<Channel>();

	private List<Channel> channelsToWaitForReconnect = new List<Channel>();

	private Channel vinMasterChannel = null;

	private bool duplicateVinMasterError = false;

	private List<Ecu> manualConnectEcus = new List<Ecu>();

	private bool? wasAutoConnecting;

	private Checkmark checkmarkFaultPresent;

	private System.Windows.Forms.Label labelFault;

	private TextBox textBoxOutput;

	private Button buttonClose;

	private ListViewEx listViewVins;

	private ColumnHeader columnEcu;

	private ColumnHeader columnVin;

	private Button buttonStart;

	private bool CanClose => !Working || currentStage == Stage.WaitForIgnitionOnReconnection;

	private bool Online
	{
		get
		{
			foreach (Channel item in channelsToWorkWith)
			{
				if (item.Online)
				{
					return true;
				}
			}
			return false;
		}
	}

	private bool FaultIsPresent
	{
		get
		{
			string masterVIN = MasterVIN;
			if (!string.IsNullOrEmpty(masterVIN))
			{
				foreach (Channel item in channelsToWorkWith)
				{
					string vehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(item);
					if (!masterVIN.Equals(vehicleIdentificationNumber))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
	}

	private bool AnEcuIsBusy
	{
		get
		{
			foreach (Channel item in channelsToWorkWith)
			{
				if (item.Online && item.CommunicationsState != CommunicationsState.Online)
				{
					return true;
				}
			}
			return false;
		}
	}

	private bool CanStart => !Working && Online && FaultIsPresent && !AnEcuIsBusy && !string.IsNullOrEmpty(MasterVIN);

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

	private string MasterVIN => (vinMasterChannel != null) ? SapiManager.GetVehicleIdentificationNumber(vinMasterChannel) : null;

	public UserPanel()
	{
		InitializeComponent();
		buttonStart.Click += OnStartClick;
		currentStage = Stage.Idle;
		ConnectionManager.GlobalInstance.ConnectionChanged += ConnectionManager_ConnectionChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((CustomPanel)this).OnChannelsChanged();
	}

	private void CheckVinMaster(Channel channel)
	{
		if (!channel.Ecu.Properties.ContainsKey("VINMaster"))
		{
			return;
		}
		bool result = false;
		if (bool.TryParse(channel.Ecu.Properties["VINMaster"], out result) && result)
		{
			if (vinMasterChannel == null)
			{
				vinMasterChannel = channel;
				duplicateVinMasterError = false;
			}
			else
			{
				vinMasterChannel = null;
				duplicateVinMasterError = true;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Invalid comparison between Unknown and I4
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Invalid comparison between Unknown and I4
		foreach (Channel item in channelsToWorkWith)
		{
			item.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		vinMasterChannel = null;
		channelsToWorkWith.Clear();
		foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
		{
			if (channel.Parameters["VIN"] != null && channel.EcuInfos["CO_VIN"] != null)
			{
				channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				channelsToWorkWith.Add(channel);
				if (channel.CommunicationsState == CommunicationsState.Online)
				{
					channel.EcuInfos["CO_VIN"].Read(synchronous: false);
				}
				CheckVinMaster(channel);
			}
		}
		switch (CurrentStage)
		{
		case Stage.WaitForIgnitionOffDisconnection:
			if (channelsToWorkWith.Count == 0 || (int)ConnectionManager.GlobalInstance.IgnitionStatus == 1)
			{
				CurrentStage = Stage.WaitForIgnitionOnReconnection;
				PerformCurrentStage();
			}
			break;
		case Stage.WaitForIgnitionOnReconnection:
		{
			int num = 0;
			foreach (Channel item2 in channelsToWaitForReconnect)
			{
				foreach (Channel channel2 in SapiManager.GlobalInstance.Sapi.Channels)
				{
					if (channel2.Ecu.Name.Equals(item2.Ecu.Name))
					{
						num++;
						break;
					}
				}
			}
			if (num == channelsToWaitForReconnect.Count && (int)ConnectionManager.GlobalInstance.IgnitionStatus == 0)
			{
				channelsToWaitForReconnect.Clear();
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
			}
			break;
		}
		default:
			if (Working && channelsToWorkWith.Count == 0)
			{
				StopWork(Reason.Disconnected);
			}
			else
			{
				UpdateUserInterface();
			}
			break;
		}
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (e.Cancel)
		{
			return;
		}
		StopWork(Reason.Closing);
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		foreach (Channel item in channelsToWorkWith)
		{
			item.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		ConnectionManager.GlobalInstance.ConnectionChanged -= ConnectionManager_ConnectionChanged;
	}

	private void OnStartClick(object sender, EventArgs e)
	{
		ClearOutput();
		CurrentStage = Stage.WriteVins;
		PerformCurrentStage();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnParametersWriteComplete(object sender, ResultEventArgs e)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		parameterCollection.ParametersWriteCompleteEvent -= OnParametersWriteComplete;
		channelsToWriteVINsFor.Remove(parameterCollection.Channel);
		if (e.Succeeded)
		{
			ReportResult(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_SuccessfullyWroteVINFor0, parameterCollection.Channel.Ecu.Name));
		}
		else
		{
			ReportResult(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_FailedToWriteVINFor0, parameterCollection.Channel.Ecu.Name));
		}
		if (channelsToWriteVINsFor.Count == 0)
		{
			CurrentStage = Stage.WaitForIgnitionOffDisconnection;
			PerformCurrentStage();
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
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		if (CurrentStage == Stage.Stopping)
		{
			return;
		}
		buttonStart.Enabled = CanStart;
		buttonClose.Enabled = CanClose;
		((ListView)(object)listViewVins).Items.Clear();
		if (Online)
		{
			foreach (Channel item in channelsToWorkWith)
			{
				ListViewExGroupItem val = new ListViewExGroupItem(item.Ecu.Name);
				((ListViewItem)(object)val).SubItems.Add(SapiManager.GetVehicleIdentificationNumber(item));
				if (item == vinMasterChannel)
				{
					((ListViewItem)(object)val).Font = new Font(((ListViewItem)(object)val).Font.FontFamily, ((ListViewItem)(object)val).Font.Size, FontStyle.Bold);
				}
				((ListView)(object)listViewVins).Items.Add((ListViewItem)(object)val);
			}
		}
		if (!Working)
		{
			if (Online)
			{
				checkmarkFaultPresent.Checked = !FaultIsPresent;
				if (FaultIsPresent)
				{
					if (vinMasterChannel != null)
					{
						if (!string.IsNullOrEmpty(MasterVIN))
						{
							labelFault.Text = string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_TheVINsInThisVehicleAreNotSynchronizedClickStartToCopyTheVINFrom0ToTheOtherDevices, vinMasterChannel.Ecu.Name);
						}
						else
						{
							labelFault.Text = Resources.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseTheVINMasterDeviceDoesNotHaveAVIN;
						}
					}
					else if (duplicateVinMasterError)
					{
						labelFault.Text = Resources.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseMultipleVINMasterDevicesAreDefinedAndConnected;
					}
					else
					{
						labelFault.Text = Resources.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseNoVINMasterDeviceIsDefinedOrConnected;
					}
				}
				else if (channelsToWorkWith.Count < 2)
				{
					labelFault.Text = Resources.Message_ConnectAtLeastTwoDevicesToDetermineVINSynchronization;
				}
				else
				{
					labelFault.Text = Resources.Message_TheVINsInThisVehicleAreSynchronizedNoActionIsNecessaryIfTheVINIsIncorrectYouWillNeedToReprogramUsingServerData;
				}
			}
			else
			{
				checkmarkFaultPresent.Checked = false;
				labelFault.Text = Resources.Message_ThereAreNoDevicesOnline;
			}
			return;
		}
		checkmarkFaultPresent.Checked = false;
		switch (CurrentStage)
		{
		case Stage.WaitForIgnitionOnReconnection:
			if (channelsToWorkWith.Count == 0 || (int)ConnectionManager.GlobalInstance.IgnitionStatus == 1)
			{
				labelFault.Text = Resources.Message_PleaseTurnTheIgnitionOnAndWait;
			}
			else
			{
				labelFault.Text = Resources.Message_WaitingForRemainingDevicesToComeOnlinePleaseWait;
			}
			break;
		case Stage.WaitForIgnitionOffDisconnection:
			if (channelsToWorkWith.Count < channelsToWaitForReconnect.Count)
			{
				labelFault.Text = Resources.Message_WaitingForRemainingDevicesToShutdownPleaseWait;
			}
			else
			{
				labelFault.Text = Resources.Message_PleaseTurnTheIgnitionOffAndWait;
			}
			break;
		default:
			labelFault.Text = Resources.Message_ProcessingPleaseWait;
			break;
		}
	}

	private void WriteVins()
	{
		string masterVIN = MasterVIN;
		if (!string.IsNullOrEmpty(masterVIN))
		{
			foreach (Channel item in channelsToWorkWith)
			{
				string vehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(item);
				if (!masterVIN.Equals(vehicleIdentificationNumber))
				{
					ReportResult(string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_UpdatingVINFor0, item.Ecu.Name));
					item.Parameters["VIN"].Value = masterVIN;
					item.Parameters.ParametersWriteCompleteEvent += OnParametersWriteComplete;
					channelsToWriteVINsFor.Add(item);
					item.Parameters.Write(synchronous: false);
				}
			}
			if (channelsToWriteVINsFor.Count == 0)
			{
				StopWork(Reason.NoVinsChanged);
			}
		}
		else
		{
			StopWork(Reason.NoVinMaster);
		}
	}

	private void TurnOffAutoConnect()
	{
		wasAutoConnecting = SapiManager.GlobalInstance.Sapi.Channels.AutoConnecting;
		Cursor.Current = Cursors.WaitCursor;
		SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
		Cursor.Current = Cursors.Default;
		foreach (Channel item in channelsToWorkWith)
		{
			channelsToWaitForReconnect.Add(item);
			if (!item.Ecu.MarkedForAutoConnect)
			{
				manualConnectEcus.Add(item.Ecu);
				item.Ecu.MarkedForAutoConnect = true;
			}
		}
	}

	private void RestoreAutoConnectState()
	{
		foreach (Ecu manualConnectEcu in manualConnectEcus)
		{
			manualConnectEcu.MarkedForAutoConnect = false;
		}
		manualConnectEcus.Clear();
		if (wasAutoConnecting.HasValue)
		{
			Cursor.Current = Cursors.WaitCursor;
			SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
			if (wasAutoConnecting.Value)
			{
				SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect(1);
			}
			wasAutoConnecting = null;
			Cursor.Current = Cursors.Default;
		}
	}

	private void PerformCurrentStage()
	{
		if (Online || CurrentStage == Stage.WaitForIgnitionOffDisconnection || CurrentStage == Stage.WaitForIgnitionOnReconnection)
		{
			switch (CurrentStage)
			{
			case Stage.Idle:
				break;
			case Stage.WriteVins:
				WriteVins();
				break;
			case Stage.WaitForIgnitionOffDisconnection:
				ReportResult(Resources.Message_WaitingForDevicesToDisconnect);
				TurnOffAutoConnect();
				break;
			case Stage.WaitForIgnitionOnReconnection:
				ReportResult(Resources.Message_WaitingForDevicesToReconnect);
				SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect();
				break;
			case Stage.Finish:
				foreach (Channel item in SapiManager.GlobalInstance.ActiveChannels.Where((Channel x) => x.Ecu.Name.StartsWith("MCM") || x.Ecu.Name.StartsWith("ACM")))
				{
					item.FaultCodes.Reset(synchronous: false);
				}
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
		}
		else
		{
			if (reason != Reason.NoVinsChanged)
			{
				buttonStart.Text = Resources.Message_Retry;
			}
			ReportResult(Resources.Message_TheProcedureFailedToComplete);
			switch (reason)
			{
			case Reason.Disconnected:
				ReportResult(Resources.Message_ADeviceWasDisconnected);
				break;
			case Reason.UnknownStage:
				ReportResult(Resources.Message_UnknownStage);
				break;
			case Reason.NoVinsChanged:
				ReportResult(Resources.Message_ItWasNotNecessaryToChangeAnyVINs);
				break;
			case Reason.NoVinMaster:
				ReportResult(Resources.Message_NoVINWasFoundInTheVINMasterDevice);
				break;
			case Reason.Closing:
				ReportResult(Resources.Message_TheOperationWasAborted);
				break;
			}
		}
		RestoreAutoConnectState();
		channelsToWaitForReconnect.Clear();
		channelsToWriteVINsFor.Clear();
		CurrentStage = Stage.Idle;
	}

	private void ConnectionManager_ConnectionChanged(object sender, IgnitionStatusEventArgs e)
	{
		((CustomPanel)this).OnChannelsChanged();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		checkmarkFaultPresent = new Checkmark();
		labelFault = new System.Windows.Forms.Label();
		textBoxOutput = new TextBox();
		buttonClose = new Button();
		buttonStart = new Button();
		listViewVins = new ListViewEx();
		columnEcu = new ColumnHeader();
		columnVin = new ColumnHeader();
		((ISupportInitialize)listViewVins).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(checkmarkFaultPresent, "checkmarkFaultPresent");
		((Control)(object)checkmarkFaultPresent).Name = "checkmarkFaultPresent";
		componentResourceManager.ApplyResources(labelFault, "labelFault");
		labelFault.Name = "labelFault";
		labelFault.UseCompatibleTextRendering = true;
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
		listViewVins.CanDelete = false;
		((ListView)(object)listViewVins).Columns.AddRange(new ColumnHeader[2] { columnEcu, columnVin });
		listViewVins.EditableColumn = -1;
		listViewVins.GridLines = true;
		componentResourceManager.ApplyResources(listViewVins, "listViewVins");
		((Control)(object)listViewVins).Name = "listViewVins";
		listViewVins.ShowGlyphs = (GlyphBehavior)1;
		listViewVins.ShowItemImages = (ImageBehavior)1;
		listViewVins.ShowStateImages = (ImageBehavior)1;
		((ListView)(object)listViewVins).UseCompatibleStateImageBehavior = false;
		componentResourceManager.ApplyResources(columnEcu, "columnEcu");
		componentResourceManager.ApplyResources(columnVin, "columnVin");
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CheckVINSynchronization");
		((Control)this).Controls.Add((Control)(object)listViewVins);
		((Control)this).Controls.Add(buttonStart);
		((Control)this).Controls.Add(buttonClose);
		((Control)this).Controls.Add(textBoxOutput);
		((Control)this).Controls.Add(labelFault);
		((Control)this).Controls.Add((Control)(object)checkmarkFaultPresent);
		((Control)this).Name = "UserPanel";
		((ISupportInitialize)listViewVins).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
