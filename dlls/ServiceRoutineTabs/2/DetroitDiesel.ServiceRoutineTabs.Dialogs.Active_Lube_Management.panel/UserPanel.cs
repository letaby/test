using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel;

public class UserPanel : CustomPanel
{
	private const string ALMGroup = "VCD_ACS_Active_Lubrication_Management";

	private const string ALMActivationTemp = "PACS_ALM_activation_temp";

	private const string ALMLookupV00M4 = "PACS_ALM_Lookup_v00_M_4";

	private const string ALMLookupV00M5 = "PACS_ALM_Lookup_v00_M_5";

	private const string ALMActivationTempShortName = "Activation Temp";

	private const string ALMLookupV00M4ShortName = "Lookup M4";

	private const string ALMLookupV00M5ShortName = "Lookup M5";

	private const string ALMAvl = "PACS_ALM_avl";

	private const string ALMPresent = "PACS_ALM_present";

	private const int TestModeAlmActivation = 5;

	private const int TestModeAlmLookup4 = 20;

	private const int TestModeAlmLookup5 = 20;

	private const int DefaultAlmActivation = 40;

	private const int DefaultAlmLookup4 = 255;

	private const int DefaultAlmLookup5 = 255;

	private Channel sSam = null;

	private Parameter almActivationParameter = null;

	private Parameter almLookup4Parameter = null;

	private Parameter almLookup5Parameter = null;

	private Parameter almAvlParameter = null;

	private int almActivationPreTestValue = 0;

	private int almLookup4PreTestValue = 0;

	private int almLookup5PreTestValue = 0;

	private Button buttonBegin;

	private Label labelSSAMStatus;

	private TableLayoutPanel tableLayoutPanel4;

	private Button buttonClose;

	private Checkmark SSAMCheck;

	private SeekTimeListView seekTimeListView;

	private DigitalReadoutInstrument digitalReadoutInstrumentALMFunctionStat;

	private DigitalReadoutInstrument digitalReadoutInstrumentALMValve;

	private TableLayoutPanel tableLayoutPanelBottom;

	private TimerControl timerControl;

	private bool Online => sSam != null && sSam.CommunicationsState == CommunicationsState.Online;

	private bool HasNeededParameters => almActivationParameter != null && almLookup4Parameter != null && almLookup5Parameter != null && almAvlParameter != null;

	private bool CanBegin => EnablePanel && Online && HasNeededParameters && SSAMCheck.Checked && !InProgress;

	private bool CanClose => !InProgress || sSam == null;

	private bool InProgress { get; set; }

	private bool EnablePanel { get; set; }

	public UserPanel()
	{
		InitializeComponent();
		InProgress = false;
		EnablePanel = true;
		buttonBegin.Click += OnClickBegin;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			SetSSAM(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetSSAM(((CustomPanel)this).GetChannel("SSAM02T"));
		UpdateUserInterface();
	}

	private void SetSSAM(Channel sSam)
	{
		if (this.sSam == sSam)
		{
			return;
		}
		if (this.sSam != null)
		{
			this.sSam.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			if (timerControl.IsTimerRunning && sSam == null)
			{
				StopTimer();
			}
		}
		this.sSam = sSam;
		if (this.sSam != null)
		{
			this.sSam.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			almActivationParameter = this.sSam.Parameters["PACS_ALM_activation_temp"];
			almLookup4Parameter = this.sSam.Parameters["PACS_ALM_Lookup_v00_M_4"];
			almLookup5Parameter = this.sSam.Parameters["PACS_ALM_Lookup_v00_M_5"];
			almAvlParameter = this.sSam.Parameters["PACS_ALM_avl"] ?? this.sSam.Parameters["PACS_ALM_present"];
			if (HasNeededParameters && this.sSam.CommunicationsState == CommunicationsState.Online)
			{
				ReadInitialParameters();
			}
		}
	}

	private void UpdateSSAMStatus()
	{
		bool flag = false;
		string text = Resources.Message_SSAM02TIsNotConnected;
		if (sSam != null)
		{
			if (HasNeededParameters)
			{
				if (sSam.CommunicationsState == CommunicationsState.Online)
				{
					text = Resources.Message_SSAM02TIsConnected;
					flag = true;
				}
				else
				{
					text = Resources.Message_SSAM02TIsBusy;
				}
			}
			else
			{
				text = Resources.Message_ThisSSAMDoesNotHaveTheNeededParameters;
			}
		}
		((Control)(object)labelSSAMStatus).Text = text;
		SSAMCheck.Checked = flag;
	}

	private void UpdateButtonStatus()
	{
		buttonBegin.Enabled = CanBegin;
		buttonClose.Enabled = CanClose;
	}

	private void UpdateUserInterface()
	{
		UpdateSSAMStatus();
		UpdateButtonStatus();
		((Control)(object)digitalReadoutInstrumentALMValve).Enabled = EnablePanel;
		((Control)(object)digitalReadoutInstrumentALMFunctionStat).Enabled = EnablePanel;
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (sSam.CommunicationsState == CommunicationsState.Online && !almAvlParameter.HasBeenReadFromEcu)
		{
			ReadInitialParameters();
		}
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnClickBegin(object sender, EventArgs e)
	{
		if (CanBegin)
		{
			ReadALMParameters();
		}
	}

	private void ReadInitialParameters()
	{
		if (sSam != null && sSam.Parameters != null)
		{
			sSam.Parameters.ParametersReadCompleteEvent += Parameters_ParametersInitialReadCompleteEvent;
			InProgress = true;
			UpdateUserInterface();
			sSam.Parameters.ReadGroup("VCD_ACS_Active_Lubrication_Management", fromCache: false, synchronous: false);
		}
	}

	private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
	{
		sSam.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersInitialReadCompleteEvent;
		if (!e.Succeeded)
		{
			return;
		}
		int num = 0;
		num = ((!(almAvlParameter.Type == typeof(Choice))) ? Convert.ToInt32(almAvlParameter.Value) : Convert.ToInt32((almAvlParameter.Value as Choice).RawValue));
		if (num == 1)
		{
			EnablePanel = true;
			AddParametersToLog(Resources.Message_CurrentAlmValues);
			int num2 = Convert.ToInt32(almActivationParameter.Value);
			int num3 = Convert.ToInt32(almLookup4Parameter.Value);
			int num4 = Convert.ToInt32(almLookup5Parameter.Value);
			if (num2 == 5 && num3 == 20 && num4 == 20)
			{
				AddLogLabel(Resources.Message_SettingDefaultValues);
				almActivationParameter.Value = 40;
				almLookup4Parameter.Value = 255;
				almLookup5Parameter.Value = 255;
				sSam.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersDefaultBeginWriteCompleteEvent;
				sSam.Parameters.Write(synchronous: false);
			}
			else
			{
				InProgress = false;
			}
		}
		else
		{
			EnablePanel = false;
			InProgress = false;
			AddLogLabel(Resources.Message_ActiveLubeManagementNotEnabled);
		}
		UpdateUserInterface();
	}

	private void Parameters_ParametersDefaultBeginWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		sSam.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersDefaultBeginWriteCompleteEvent;
		AddParametersToLog(Resources.Message_ParametersSetToDefaultValues);
		InProgress = false;
		UpdateUserInterface();
	}

	private void ReadALMParameters()
	{
		if (sSam != null && sSam.Parameters != null)
		{
			sSam.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
			InProgress = true;
			UpdateUserInterface();
			sSam.Parameters.ReadGroup("VCD_ACS_Active_Lubrication_Management", fromCache: false, synchronous: false);
		}
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		sSam.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
		if (e.Succeeded)
		{
			almActivationPreTestValue = Convert.ToInt32(almActivationParameter.Value);
			almLookup4PreTestValue = Convert.ToInt32(almLookup4Parameter.Value);
			almLookup5PreTestValue = Convert.ToInt32(almLookup5Parameter.Value);
			AddLogLabel(Resources.Message_EnteringTestMode);
			almActivationParameter.Value = 5;
			almLookup4Parameter.Value = 20;
			almLookup5Parameter.Value = 20;
			sSam.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersBeginTestWriteCompleteEvent;
			sSam.Parameters.Write(synchronous: false);
		}
	}

	private void Parameters_ParametersBeginTestWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		sSam.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersBeginTestWriteCompleteEvent;
		AddParametersToLog(Resources.Message_RemainInTestMode);
		timerControl.Start();
	}

	private void timerControl_TimerCountdownCompleted(object sender, EventArgs e)
	{
		if (Online)
		{
			StopTimer();
			AddLogLabel(Resources.Message_ResettingParameters);
			almActivationParameter.Value = almActivationPreTestValue;
			almLookup4Parameter.Value = almLookup4PreTestValue;
			almLookup5Parameter.Value = almLookup5PreTestValue;
			sSam.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersEndTestWriteCompleteEvent;
			sSam.Parameters.Write(synchronous: false);
		}
	}

	private void StopTimer()
	{
		timerControl.Stop();
	}

	private void Parameters_ParametersEndTestWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		sSam.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersEndTestWriteCompleteEvent;
		AddParametersToLog(Resources.Message_ParametersReset);
		AddLogLabel(Resources.Message_Finished);
		InProgress = false;
		UpdateUserInterface();
	}

	private void AddParametersToLog(string text)
	{
		string text2 = string.Format("{0} {1} {2}, {3} {4}, {5} {6}", text, "Activation Temp", almActivationParameter.Value, "Lookup M4", almLookup4Parameter.Value, "Lookup M5", almLookup5Parameter.Value);
		AddLogLabel(text2);
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
		}
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentALMFunctionStat = new DigitalReadoutInstrument();
		timerControl = new TimerControl();
		seekTimeListView = new SeekTimeListView();
		digitalReadoutInstrumentALMValve = new DigitalReadoutInstrument();
		tableLayoutPanelBottom = new TableLayoutPanel();
		SSAMCheck = new Checkmark();
		buttonClose = new Button();
		buttonBegin = new Button();
		labelSSAMStatus = new Label();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)(object)tableLayoutPanelBottom).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentALMFunctionStat, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)timerControl, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)seekTimeListView, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentALMValve, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)tableLayoutPanelBottom, 0, 3);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentALMFunctionStat, "digitalReadoutInstrumentALMFunctionStat");
		digitalReadoutInstrumentALMFunctionStat.FontGroup = "DigitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentALMFunctionStat).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentALMFunctionStat).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ALMFunction_Stat");
		((Control)(object)digitalReadoutInstrumentALMFunctionStat).Name = "digitalReadoutInstrumentALMFunctionStat";
		((SingleInstrumentBase)digitalReadoutInstrumentALMFunctionStat).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(timerControl, "timerControl");
		timerControl.Duration = TimeSpan.Parse("00:00:30");
		timerControl.FontGroup = null;
		((Control)(object)timerControl).Name = "timerControl";
		((TableLayoutPanel)(object)tableLayoutPanel4).SetRowSpan((Control)(object)timerControl, 2);
		timerControl.TimerCountdownCompletedDisplayMessage = " ";
		timerControl.TimerCountdownCompleted += timerControl_TimerCountdownCompleted;
		((TableLayoutPanel)(object)tableLayoutPanel4).SetColumnSpan((Control)(object)seekTimeListView, 2);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Active Lube Management";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentALMValve, "digitalReadoutInstrumentALMValve");
		digitalReadoutInstrumentALMValve.FontGroup = "DigitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentALMValve).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentALMValve).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ActvLubMgntValve");
		((Control)(object)digitalReadoutInstrumentALMValve).Name = "digitalReadoutInstrumentALMValve";
		((SingleInstrumentBase)digitalReadoutInstrumentALMValve).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelBottom, "tableLayoutPanelBottom");
		((TableLayoutPanel)(object)tableLayoutPanel4).SetColumnSpan((Control)(object)tableLayoutPanelBottom, 2);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)SSAMCheck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add(buttonClose, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add(buttonBegin, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)labelSSAMStatus, 1, 0);
		((Control)(object)tableLayoutPanelBottom).Name = "tableLayoutPanelBottom";
		componentResourceManager.ApplyResources(SSAMCheck, "SSAMCheck");
		((Control)(object)SSAMCheck).Name = "SSAMCheck";
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonBegin, "buttonBegin");
		buttonBegin.Name = "buttonBegin";
		buttonBegin.UseCompatibleTextRendering = true;
		buttonBegin.UseVisualStyleBackColor = true;
		labelSSAMStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelSSAMStatus, "labelSSAMStatus");
		((Control)(object)labelSSAMStatus).Name = "labelSSAMStatus";
		labelSSAMStatus.Orientation = (TextOrientation)1;
		labelSSAMStatus.ShowBorder = false;
		labelSSAMStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel4);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)(object)tableLayoutPanelBottom).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelBottom).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
