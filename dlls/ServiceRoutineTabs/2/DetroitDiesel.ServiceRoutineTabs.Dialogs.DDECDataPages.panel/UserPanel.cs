using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.DataHub;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DDECDataPages.panel;

public class UserPanel : CustomPanel
{
	private enum Action
	{
		none,
		extraction,
		resetData,
		clearPassword,
		setPassword,
		waitingForCPCOnline
	}

	private const string RequiredUserLabelPrefix = "DDecDataPages";

	private Action action = Action.none;

	private string VINEcuInfo = "CO_VIN";

	private Channel cpc = null;

	private string vin = null;

	private Button buttonExtract;

	private ProgressBar progressBar;

	private Checkmark checkmarkDataPagesEnabled;

	private Button buttonResetTrip;

	private Button buttonResetAll;

	private System.Windows.Forms.Label labelDataPageEnabled;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private TextBox textBoxStatus;

	private Button buttonClearPassword;

	private Button buttonSetPassword;

	private Button buttonClose;

	private bool CanClose
	{
		get
		{
			if (action != Action.none)
			{
				return false;
			}
			return true;
		}
	}

	private bool Busy => ExtractionManager.Busy || action != Action.none;

	private bool Online => cpc != null && (cpc.CommunicationsState == CommunicationsState.Online || cpc.CommunicationsState == CommunicationsState.ByteMessage || cpc.CommunicationsState == CommunicationsState.ExecuteService);

	private bool CanPerformAction
	{
		get
		{
			if (ExtractionManager.GlobalInstance.DataPagesEnabled && !Busy && Online)
			{
				return true;
			}
			return false;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetCPC(SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name.StartsWith("CPC")));
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			SetCPC(null);
		}
	}

	private void buttonClose_Click(object sender, EventArgs e)
	{
		((ContainerControl)this).ParentForm.Close();
	}

	private void buttonExtract_Click(object sender, EventArgs e)
	{
		LogText(Resources.Message_StartingExtraction);
		action = Action.extraction;
		progressBar.Value = 0;
		progressBar.Visible = true;
		ExtractionManager.GlobalInstance.ExtractionCompleteEvent += ExtractionManager_ExtractionCompleteEvent;
		ExtractionManager.GlobalInstance.ExtractionProgressEvent += ExtractionManager_ExtractionProgressEvent;
		ExtractionManager.GlobalInstance.DoExtraction(string.Empty);
		UpdateUserInterface();
	}

	private void buttonSetPassword_Click(object sender, EventArgs e)
	{
		LogText(Resources.Message_SettingPassword);
		action = Action.setPassword;
		UpdateUserInterface();
		ExtractionManager.GlobalInstance.SetDataPagesPasswordCompleteEvent += ExtractionManager_SetDataPagesPasswordCompleteEvent;
		ExtractionManager.GlobalInstance.SetDataPagesPassword();
	}

	private void buttonClearPassword_Click(object sender, EventArgs e)
	{
		LogText(Resources.Message_ClearingPassword);
		action = Action.clearPassword;
		UpdateUserInterface();
		ExtractionManager.GlobalInstance.ClearDataPagesPasswordCompleteEvent += ExtractionManager_ClearDataPagesPasswordCompleteEvent;
		ExtractionManager.GlobalInstance.ClearDataPagesPassword();
	}

	private void buttonResetTrip_Click(object sender, EventArgs e)
	{
		action = Action.resetData;
		LogText(Resources.Message_ResettingTrip);
		UpdateUserInterface();
		PerformDataPageReset(resetAll: false);
	}

	private void buttonResetAll_Click(object sender, EventArgs e)
	{
		action = Action.resetData;
		LogText(Resources.Message_ResettingAllTrips);
		UpdateUserInterface();
		PerformDataPageReset(resetAll: true);
	}

	private void ExtractionManager_ExtractionCompleteEvent(object sender, ExtractionCompleteEventArgs extractionCompleteEventArgs)
	{
		ExtractionManager.GlobalInstance.ExtractionCompleteEvent -= ExtractionManager_ExtractionCompleteEvent;
		ExtractionManager.GlobalInstance.ExtractionProgressEvent -= ExtractionManager_ExtractionProgressEvent;
		string arg = (extractionCompleteEventArgs.Succeeded ? Resources.Message_SucessExtraction : Resources.Message_ExtractionFailed);
		LogText(string.Format(CultureInfo.CurrentCulture, "{0} {1}.", arg, (!string.IsNullOrEmpty(vin)) ? vin : Resources.Message_WithNoVIN));
		progressBar.Visible = false;
		action = Action.none;
		UpdateUserInterface();
	}

	private void ExtractionManager_ExtractionProgressEvent(object sender, ExtractionProgressEventArgs extractionProgressEventArgs)
	{
		progressBar.Value = (int)extractionProgressEventArgs.Percent;
		UpdateUserInterface();
	}

	private void ExtractionManager_ClearDataPagesCompleteEvent(object sender, ResultEventArgs e)
	{
		ExtractionManager.GlobalInstance.ClearDataPagesCompleteEvent -= ExtractionManager_ClearDataPagesCompleteEvent;
		string arg = (e.Succeeded ? Resources.Message_TripsReset : Resources.Message_ResetFailed);
		LogText(string.Format(CultureInfo.CurrentCulture, "{0} {1}.", arg, (!string.IsNullOrEmpty(vin)) ? vin : Resources.Message_WithNoVIN));
		action = Action.none;
		UpdateUserInterface();
	}

	private void ExtractionManager_SetDataPagesPasswordCompleteEvent(object sender, ChangeDataPagePasswordRequestEventArgs e)
	{
		ExtractionManager.GlobalInstance.SetDataPagesPasswordCompleteEvent -= ExtractionManager_SetDataPagesPasswordCompleteEvent;
		string arg = ((e.Result == ChangePasswordResult.Success) ? Resources.Message_PasswordSet : ((e.Result == ChangePasswordResult.Cancel) ? Resources.Message_SetPasswordCancel : Resources.Message_SetPasswordFail));
		LogText(string.Format(CultureInfo.CurrentCulture, "{0} {1}.", arg, (!string.IsNullOrEmpty(vin)) ? vin : Resources.Message_WithNoVIN));
		if (e.Result == ChangePasswordResult.Success)
		{
			LogText(Resources.Message_CPCReset);
			action = Action.waitingForCPCOnline;
			cpc.Services.ServiceCompleteEvent += commitChangesService_ServiceCompleteEvent;
			cpc.Services.Execute(cpc.Ecu.Properties["CommitToPermanentMemoryService"], synchronous: false);
		}
		else
		{
			action = Action.none;
		}
		UpdateUserInterface();
	}

	private void ExtractionManager_ClearDataPagesPasswordCompleteEvent(object sender, ResultEventArgs e)
	{
		ExtractionManager.GlobalInstance.ClearDataPagesPasswordCompleteEvent -= ExtractionManager_ClearDataPagesPasswordCompleteEvent;
		string arg = (e.Succeeded ? Resources.Message_PasswordCleared : Resources.Message_ClearPasswordFail);
		LogText(string.Format(CultureInfo.CurrentCulture, "{0} {1}.", arg, (!string.IsNullOrEmpty(vin)) ? vin : Resources.Message_WithNoVIN));
		if (e.Succeeded)
		{
			LogText(Resources.Message_CPCReset);
			action = Action.waitingForCPCOnline;
			cpc.Services.ServiceCompleteEvent += commitChangesService_ServiceCompleteEvent;
			cpc.Services.Execute(cpc.Ecu.Properties["CommitToPermanentMemoryService"], synchronous: false);
		}
		else
		{
			action = Action.none;
		}
		UpdateUserInterface();
	}

	private void commitChangesService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		cpc.Services.ServiceCompleteEvent -= commitChangesService_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			LogText(Resources.Message_ChangesCommited);
		}
		else
		{
			LogText(Resources.Message_CommitServiceFailed);
		}
		action = Action.none;
		UpdateUserInterface();
	}

	private void GetVin()
	{
		if (!Online || cpc.EcuInfos[VINEcuInfo] == null)
		{
			return;
		}
		string empty = string.Empty;
		cpc.EcuInfos[VINEcuInfo].Read(synchronous: true);
		empty = cpc.EcuInfos[VINEcuInfo].Value;
		if (!string.Equals(empty, vin, StringComparison.OrdinalIgnoreCase))
		{
			vin = empty;
			if (!string.IsNullOrEmpty(vin))
			{
				LogText(string.Format(CultureInfo.CurrentCulture, Resources.Message_Vin, vin));
			}
		}
	}

	private void SetCPC(Channel cpc)
	{
		if (this.cpc == cpc)
		{
			return;
		}
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
		}
		vin = null;
		this.cpc = cpc;
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			if (SapiManager.SupportsDdecDataPages(this.cpc))
			{
				ExtractionManager.GlobalInstance.SetChannel(this.cpc);
			}
			GetVin();
		}
		UpdateUserInterface();
	}

	private void StopActionInProgress()
	{
		switch (action)
		{
		case Action.extraction:
			ExtractionManager.GlobalInstance.ExtractionCompleteEvent -= ExtractionManager_ExtractionCompleteEvent;
			ExtractionManager.GlobalInstance.ExtractionProgressEvent -= ExtractionManager_ExtractionProgressEvent;
			break;
		case Action.setPassword:
			ExtractionManager.GlobalInstance.SetDataPagesPasswordCompleteEvent -= ExtractionManager_SetDataPagesPasswordCompleteEvent;
			break;
		case Action.clearPassword:
			ExtractionManager.GlobalInstance.ClearDataPagesPasswordCompleteEvent -= ExtractionManager_ClearDataPagesPasswordCompleteEvent;
			break;
		case Action.waitingForCPCOnline:
			cpc.Services.ServiceCompleteEvent -= commitChangesService_ServiceCompleteEvent;
			break;
		default:
			ExtractionManager.GlobalInstance.ClearDataPagesCompleteEvent -= ExtractionManager_ClearDataPagesCompleteEvent;
			break;
		}
		progressBar.Visible = false;
		LogText(Resources.Message_OperationtionCpcOffline);
		string arg = ((action == Action.extraction) ? Resources.Message_ExtractionFailed : Resources.Message_ResetFailed);
		string text = $"{arg}{((!string.IsNullOrEmpty(vin)) ? vin : Resources.Message_WithNoVIN)}";
		LogText(text);
		action = Action.none;
		UpdateUserInterface();
	}

	private void PerformDataPageReset(bool resetAll)
	{
		ExtractionManager.GlobalInstance.ClearDataPagesCompleteEvent += ExtractionManager_ClearDataPagesCompleteEvent;
		if (resetAll)
		{
			ExtractionManager.GlobalInstance.ResetAllData();
		}
		else
		{
			ExtractionManager.GlobalInstance.ResetTripData();
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (Online && string.IsNullOrEmpty(vin))
		{
			GetVin();
		}
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		if (ExtractionManager.GlobalInstance.SupportDetailsRead && Online)
		{
			if (ExtractionManager.GlobalInstance.DataPagesEnabled)
			{
				labelDataPageEnabled.Text = (Busy ? Resources.Message_Working : Resources.Message_DataPagesEnabled);
				checkmarkDataPagesEnabled.Checked = true;
			}
			else
			{
				labelDataPageEnabled.Text = Resources.Message_DataPagesDisabled;
				checkmarkDataPagesEnabled.Checked = false;
			}
			buttonExtract.Enabled = CanPerformAction;
			buttonResetAll.Enabled = CanPerformAction;
			buttonResetTrip.Enabled = CanPerformAction;
			buttonSetPassword.Enabled = CanPerformAction && ExtractionManager.GlobalInstance.DataPagesEnabled;
			buttonClearPassword.Enabled = CanPerformAction && ExtractionManager.GlobalInstance.DataPagesEnabled;
		}
		else if (Online && SapiManager.SupportsDdecDataPages(cpc))
		{
			ExtractionManager.GlobalInstance.SetChannel(cpc);
			labelDataPageEnabled.Text = Resources.Message_ReadingSupportDetails;
			checkmarkDataPagesEnabled.Checked = false;
			buttonExtract.Enabled = false;
			buttonResetAll.Enabled = false;
			buttonResetTrip.Enabled = false;
			buttonSetPassword.Enabled = false;
			buttonClearPassword.Enabled = false;
		}
		else
		{
			if (action == Action.waitingForCPCOnline)
			{
				LogText(Resources.Message_WaitingForCPCOnline);
				checkmarkDataPagesEnabled.Checked = true;
			}
			else if (action != Action.none)
			{
				StopActionInProgress();
				checkmarkDataPagesEnabled.Checked = false;
			}
			else if (checkmarkDataPagesEnabled.Checked)
			{
				LogText(Resources.Message_LostConnection);
				checkmarkDataPagesEnabled.Checked = false;
			}
			labelDataPageEnabled.Text = Resources.Message_NotConnected;
			buttonExtract.Enabled = false;
			buttonResetAll.Enabled = false;
			buttonResetTrip.Enabled = false;
			buttonSetPassword.Enabled = false;
			buttonClearPassword.Enabled = false;
		}
		buttonClose.Enabled = CanClose;
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog("DDecDataPages", text);
		StringBuilder stringBuilder = new StringBuilder(textBoxStatus.Text);
		stringBuilder.AppendLine(text);
		textBoxStatus.Text = stringBuilder.ToString();
		textBoxStatus.SelectionStart = textBoxStatus.TextLength;
		textBoxStatus.SelectionLength = 0;
		textBoxStatus.ScrollToCaret();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmarkDataPagesEnabled = new Checkmark();
		labelDataPageEnabled = new System.Windows.Forms.Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		buttonClearPassword = new Button();
		buttonSetPassword = new Button();
		buttonClose = new Button();
		buttonResetAll = new Button();
		progressBar = new ProgressBar();
		buttonExtract = new Button();
		buttonResetTrip = new Button();
		textBoxStatus = new TextBox();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmarkDataPagesEnabled, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelDataPageEnabled, 1, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmarkDataPagesEnabled, "checkmarkDataPagesEnabled");
		((Control)(object)checkmarkDataPagesEnabled).Name = "checkmarkDataPagesEnabled";
		componentResourceManager.ApplyResources(labelDataPageEnabled, "labelDataPageEnabled");
		labelDataPageEnabled.Name = "labelDataPageEnabled";
		labelDataPageEnabled.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClearPassword, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetPassword, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonResetAll, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(progressBar, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonExtract, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonResetTrip, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxStatus, 0, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(buttonClearPassword, "buttonClearPassword");
		buttonClearPassword.Name = "buttonClearPassword";
		buttonClearPassword.UseCompatibleTextRendering = true;
		buttonClearPassword.UseVisualStyleBackColor = true;
		buttonClearPassword.Click += buttonClearPassword_Click;
		componentResourceManager.ApplyResources(buttonSetPassword, "buttonSetPassword");
		buttonSetPassword.Name = "buttonSetPassword";
		buttonSetPassword.UseCompatibleTextRendering = true;
		buttonSetPassword.UseVisualStyleBackColor = true;
		buttonSetPassword.Click += buttonSetPassword_Click;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		buttonClose.Click += buttonClose_Click;
		componentResourceManager.ApplyResources(buttonResetAll, "buttonResetAll");
		buttonResetAll.Name = "buttonResetAll";
		buttonResetAll.UseCompatibleTextRendering = true;
		buttonResetAll.UseVisualStyleBackColor = true;
		buttonResetAll.Click += buttonResetAll_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)progressBar, 3);
		componentResourceManager.ApplyResources(progressBar, "progressBar");
		progressBar.Name = "progressBar";
		componentResourceManager.ApplyResources(buttonExtract, "buttonExtract");
		buttonExtract.Name = "buttonExtract";
		buttonExtract.UseCompatibleTextRendering = true;
		buttonExtract.UseVisualStyleBackColor = true;
		buttonExtract.Click += buttonExtract_Click;
		componentResourceManager.ApplyResources(buttonResetTrip, "buttonResetTrip");
		buttonResetTrip.Name = "buttonResetTrip";
		buttonResetTrip.UseCompatibleTextRendering = true;
		buttonResetTrip.UseVisualStyleBackColor = true;
		buttonResetTrip.Click += buttonResetTrip_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxStatus, 3);
		componentResourceManager.ApplyResources(textBoxStatus, "textBoxStatus");
		textBoxStatus.Name = "textBoxStatus";
		textBoxStatus.ReadOnly = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_DDEC_Data_Pages");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
