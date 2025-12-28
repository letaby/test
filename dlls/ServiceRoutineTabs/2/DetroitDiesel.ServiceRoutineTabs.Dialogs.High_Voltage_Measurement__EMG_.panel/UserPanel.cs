using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.High_Voltage_Measurement__EMG_.panel;

public class UserPanel : CustomPanel
{
	private const string NumberofStringsQualifier = "ptconf_p_Veh_BatNumOfStrings_u8";

	private static int MaxBatteryCount = 9;

	private Channel ecpc01tChannel = null;

	private Parameter numberofStringsParameter = null;

	private int previousBatteryCount = 0;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentVoltages;

	private TableLayoutPanel tableLayoutPanelMain;

	private TableLayoutPanel tableLayoutPanelContent;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS1;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS2;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS3;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS4;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS5;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS6;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS7;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS8;

	private DigitalReadoutInstrument digitalReadoutInstrumentBMS9;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTIInverter1;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTIInverter2;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTIInverter3;

	private DigitalReadoutInstrument digitalReadoutInstrumentDCLConverter;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTC3;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTC1;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTC2;

	private DigitalReadoutInstrument digitalReadoutInstrumentEAC;

	private DigitalReadoutInstrument digitalReadoutInstrumentERC;

	private TableLayoutPanel tableLayoutPanelStatusComponentVoltages;

	private SharedProcedureSelection sharedProcedureSelectionMeasurement;

	private Button buttonStartStopHVMeasurement;

	private System.Windows.Forms.Label labelVoltagesRoutine;

	private Checkmark checkmarkVoltagesRoutine;

	private TableLayoutPanel tableLayoutPanelTop;

	private PictureBox pictureBoxWarningIcon;

	private WebBrowser webBrowserWarning;

	private DigitalReadoutInstrument digitalReadoutInstrumentPTC4;

	private System.Windows.Forms.Label label1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentVoltages;

	private bool EcpcOnline => ecpc01tChannel != null && (ecpc01tChannel.CommunicationsState == CommunicationsState.Online || ecpc01tChannel.CommunicationsState == CommunicationsState.LogFilePlayback);

	private int BatteryCount
	{
		get
		{
			int result = 9;
			if (ecpc01tChannel != null && numberofStringsParameter != null && numberofStringsParameter.HasBeenReadFromEcu && numberofStringsParameter.Value != null)
			{
				int.TryParse(numberofStringsParameter.Value.ToString(), out result);
			}
			if (result <= 3)
			{
				result = 4;
			}
			if (result > MaxBatteryCount)
			{
				result = MaxBatteryCount;
			}
			return result;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		previousBatteryCount = BatteryCount;
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		string text = "html { height:100%; display: table; } ";
		text += "body { margin: 0px; padding: 0px; display: table-cell; vertical-align: middle; } ";
		text += ".scaled { font-size: calc(0.33vw + 12.0vh); font-family: Segoe UI; padding: 0px; margin: 0px; }  ";
		text += ".bold { font-weight: bold; }";
		text += ".red { color: red; }";
		string format = "<html><style>{0}</style><body><span class='scaled bold red'>{1}</span><span class='scaled bold'>{2}</span><br><span class='scaled'>{3}</span><span class='scaled bold'>{4}</span></body><span class='scaled'>).</span></html>";
		webBrowserWarning.DocumentText = string.Format(CultureInfo.InvariantCulture, format, text, Resources.RedWarning, Resources.BlackWarning, Resources.WarningText, Resources.ReferenceChecklist);
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelectionMeasurement.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= this_ParentFormClosing;
		}
	}

	public override void OnChannelsChanged()
	{
		SetECPC(((CustomPanel)this).GetChannel("ECPC01T", (ChannelLookupOptions)3));
		UpdateUI();
	}

	private void SetECPC(Channel ecpc01t)
	{
		if (ecpc01tChannel == ecpc01t)
		{
			return;
		}
		if (ecpc01tChannel != null)
		{
			ecpc01tChannel.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			numberofStringsParameter = null;
		}
		ecpc01tChannel = ecpc01t;
		if (ecpc01tChannel != null)
		{
			ecpc01tChannel.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			numberofStringsParameter = ecpc01tChannel.Parameters["ptconf_p_Veh_BatNumOfStrings_u8"];
			if (EcpcOnline)
			{
				ReadInitialParameters();
			}
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (EcpcOnline)
		{
			ReadInitialParameters();
		}
		UpdateUI();
	}

	private void ReadInitialParameters()
	{
		if (EcpcOnline && ecpc01tChannel.CommunicationsState == CommunicationsState.Online && ecpc01tChannel.Parameters != null && numberofStringsParameter != null && !numberofStringsParameter.HasBeenReadFromEcu)
		{
			ecpc01tChannel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersInitialReadCompleteEvent;
			ecpc01tChannel.Parameters.ReadGroup(numberofStringsParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
		UpdateUI();
	}

	private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
	{
		ecpc01tChannel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersInitialReadCompleteEvent;
		UpdateUI();
	}

	private void UpdateUI()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		if (BatteryCount == previousBatteryCount)
		{
			return;
		}
		((Control)(object)tableLayoutPanelContent).SuspendLayout();
		if (digitalReadoutInstrumentPTIInverter3 != null)
		{
			((Control)(object)digitalReadoutInstrumentPTIInverter3).Visible = BatteryCount >= 7;
		}
		for (int i = 0; i < MaxBatteryCount; i++)
		{
			DigitalReadoutInstrument val = (DigitalReadoutInstrument)((TableLayoutPanel)(object)tableLayoutPanelContent).Controls[$"digitalReadoutInstrumentBMS{i + 1}"];
			if (val != null)
			{
				((Control)(object)val).Visible = i < BatteryCount;
			}
		}
		((Control)(object)tableLayoutPanelContent).ResumeLayout();
		previousBatteryCount = BatteryCount;
	}

	private void sharedProcedureCreatorComponentVoltages_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		e.Service.CombinedService.Execute(synchronous: false);
	}

	private void sharedProcedureCreatorComponentVoltages_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		digitalReadoutInstrumentBMS1.ShowScalingValue = true;
		digitalReadoutInstrumentBMS2.ShowScalingValue = true;
		digitalReadoutInstrumentBMS3.ShowScalingValue = true;
		digitalReadoutInstrumentBMS4.ShowScalingValue = true;
		digitalReadoutInstrumentBMS5.ShowScalingValue = true;
		digitalReadoutInstrumentBMS6.ShowScalingValue = true;
		digitalReadoutInstrumentBMS7.ShowScalingValue = true;
		digitalReadoutInstrumentBMS8.ShowScalingValue = true;
		digitalReadoutInstrumentBMS9.ShowScalingValue = true;
		digitalReadoutInstrumentPTIInverter1.ShowScalingValue = true;
		digitalReadoutInstrumentPTIInverter2.ShowScalingValue = true;
		digitalReadoutInstrumentPTIInverter3.ShowScalingValue = true;
		digitalReadoutInstrumentDCLConverter.ShowScalingValue = true;
		digitalReadoutInstrumentPTC1.ShowScalingValue = true;
		digitalReadoutInstrumentPTC2.ShowScalingValue = true;
		digitalReadoutInstrumentPTC3.ShowScalingValue = true;
		digitalReadoutInstrumentPTC4.ShowScalingValue = true;
		digitalReadoutInstrumentEAC.ShowScalingValue = true;
		digitalReadoutInstrumentERC.ShowScalingValue = true;
	}

	private void sharedProcedureCreatorComponentVoltages_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		digitalReadoutInstrumentBMS1.ShowScalingValue = false;
		digitalReadoutInstrumentBMS2.ShowScalingValue = false;
		digitalReadoutInstrumentBMS3.ShowScalingValue = false;
		digitalReadoutInstrumentBMS4.ShowScalingValue = false;
		digitalReadoutInstrumentBMS5.ShowScalingValue = false;
		digitalReadoutInstrumentBMS6.ShowScalingValue = false;
		digitalReadoutInstrumentBMS7.ShowScalingValue = false;
		digitalReadoutInstrumentBMS8.ShowScalingValue = false;
		digitalReadoutInstrumentBMS9.ShowScalingValue = false;
		digitalReadoutInstrumentPTIInverter1.ShowScalingValue = false;
		digitalReadoutInstrumentPTIInverter2.ShowScalingValue = false;
		digitalReadoutInstrumentPTIInverter3.ShowScalingValue = false;
		digitalReadoutInstrumentDCLConverter.ShowScalingValue = false;
		digitalReadoutInstrumentPTC1.ShowScalingValue = false;
		digitalReadoutInstrumentPTC2.ShowScalingValue = false;
		digitalReadoutInstrumentPTC3.ShowScalingValue = false;
		digitalReadoutInstrumentPTC4.ShowScalingValue = false;
		digitalReadoutInstrumentEAC.ShowScalingValue = false;
		digitalReadoutInstrumentERC.ShowScalingValue = false;
	}

	private void InitializeComponent()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Expected O, but got Unknown
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe1: Unknown result type (might be due to invalid IL or missing references)
		//IL_108a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1133: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285: Unknown result type (might be due to invalid IL or missing references)
		//IL_1332: Unknown result type (might be due to invalid IL or missing references)
		//IL_157a: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		sharedProcedureCreatorComponentVoltages = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentVoltages = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureSelectionMeasurement = new SharedProcedureSelection();
		labelVoltagesRoutine = new System.Windows.Forms.Label();
		checkmarkVoltagesRoutine = new Checkmark();
		buttonStartStopHVMeasurement = new Button();
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanelContent = new TableLayoutPanel();
		digitalReadoutInstrumentPTC4 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS4 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS6 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS7 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS8 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBMS9 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPTIInverter1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPTIInverter2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPTIInverter3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDCLConverter = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPTC3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPTC1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPTC2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEAC = new DigitalReadoutInstrument();
		digitalReadoutInstrumentERC = new DigitalReadoutInstrument();
		tableLayoutPanelStatusComponentVoltages = new TableLayoutPanel();
		label1 = new System.Windows.Forms.Label();
		tableLayoutPanelTop = new TableLayoutPanel();
		pictureBoxWarningIcon = new PictureBox();
		webBrowserWarning = new WebBrowser();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelContent).SuspendLayout();
		((Control)(object)tableLayoutPanelStatusComponentVoltages).SuspendLayout();
		((Control)(object)tableLayoutPanelTop).SuspendLayout();
		((ISupportInitialize)pictureBoxWarningIcon).BeginInit();
		((Control)this).SuspendLayout();
		sharedProcedureCreatorComponentVoltages.Suspend();
		sharedProcedureCreatorComponentVoltages.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Request_Results_VoltageReadoutStat");
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Initialize((ValueState)0, 6);
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(1, 0.0, (ValueState)0);
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(5, 4.0, (ValueState)0);
		sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(6, 15.0, (ValueState)0);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentVoltages, "sharedProcedureCreatorComponentVoltages");
		sharedProcedureCreatorComponentVoltages.Qualifier = "SP_OTF_Readout";
		sharedProcedureCreatorComponentVoltages.StartCall = new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Start");
		sharedProcedureCreatorComponentVoltages.StopCall = new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Stop");
		sharedProcedureCreatorComponentVoltages.StartServiceComplete += sharedProcedureCreatorComponentVoltages_StartServiceComplete;
		sharedProcedureCreatorComponentVoltages.StopServiceComplete += sharedProcedureCreatorComponentVoltages_StopServiceComplete;
		sharedProcedureCreatorComponentVoltages.MonitorServiceComplete += sharedProcedureCreatorComponentVoltages_MonitorServiceComplete;
		sharedProcedureCreatorComponentVoltages.Resume();
		sharedProcedureIntegrationComponentVoltages.ProceduresDropDown = sharedProcedureSelectionMeasurement;
		sharedProcedureIntegrationComponentVoltages.ProcedureStatusMessageTarget = labelVoltagesRoutine;
		sharedProcedureIntegrationComponentVoltages.ProcedureStatusStateTarget = checkmarkVoltagesRoutine;
		sharedProcedureIntegrationComponentVoltages.ResultsTarget = null;
		sharedProcedureIntegrationComponentVoltages.StartStopButton = buttonStartStopHVMeasurement;
		sharedProcedureIntegrationComponentVoltages.StopAllButton = null;
		componentResourceManager.ApplyResources(sharedProcedureSelectionMeasurement, "sharedProcedureSelectionMeasurement");
		((Control)(object)sharedProcedureSelectionMeasurement).Name = "sharedProcedureSelectionMeasurement";
		sharedProcedureSelectionMeasurement.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_OTF_Readout" });
		componentResourceManager.ApplyResources(labelVoltagesRoutine, "labelVoltagesRoutine");
		labelVoltagesRoutine.Name = "labelVoltagesRoutine";
		labelVoltagesRoutine.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkVoltagesRoutine, "checkmarkVoltagesRoutine");
		((Control)(object)checkmarkVoltagesRoutine).Name = "checkmarkVoltagesRoutine";
		componentResourceManager.ApplyResources(buttonStartStopHVMeasurement, "buttonStartStopHVMeasurement");
		buttonStartStopHVMeasurement.Name = "buttonStartStopHVMeasurement";
		buttonStartStopHVMeasurement.UseCompatibleTextRendering = true;
		buttonStartStopHVMeasurement.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelContent, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelStatusComponentVoltages, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelTop, 0, 0);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		((Control)(object)tableLayoutPanelContent).BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(tableLayoutPanelContent, "tableLayoutPanelContent");
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTC4, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS6, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS8, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentBMS9, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTIInverter1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTIInverter2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTIInverter3, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentDCLConverter, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTC3, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTC1, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentPTC2, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentEAC, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanelContent).Controls.Add((Control)(object)digitalReadoutInstrumentERC, 1, 9);
		((Control)(object)tableLayoutPanelContent).Name = "tableLayoutPanelContent";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTC4, "digitalReadoutInstrumentPTC4");
		digitalReadoutInstrumentPTC4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC4).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab2");
		((Control)(object)digitalReadoutInstrumentPTC4).Name = "digitalReadoutInstrumentPTC4";
		digitalReadoutInstrumentPTC4.ShowBorder = false;
		digitalReadoutInstrumentPTC4.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC4).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC4).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS1, "digitalReadoutInstrumentBMS1");
		digitalReadoutInstrumentBMS1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS1).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS01");
		((Control)(object)digitalReadoutInstrumentBMS1).Name = "digitalReadoutInstrumentBMS1";
		digitalReadoutInstrumentBMS1.ShowBorder = false;
		digitalReadoutInstrumentBMS1.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS1).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS1).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS2, "digitalReadoutInstrumentBMS2");
		digitalReadoutInstrumentBMS2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS2).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS02");
		((Control)(object)digitalReadoutInstrumentBMS2).Name = "digitalReadoutInstrumentBMS2";
		digitalReadoutInstrumentBMS2.ShowBorder = false;
		digitalReadoutInstrumentBMS2.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS2).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS2).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS2).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS3, "digitalReadoutInstrumentBMS3");
		digitalReadoutInstrumentBMS3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS3).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS03");
		((Control)(object)digitalReadoutInstrumentBMS3).Name = "digitalReadoutInstrumentBMS3";
		digitalReadoutInstrumentBMS3.ShowBorder = false;
		digitalReadoutInstrumentBMS3.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS3).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS3).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS3).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS4, "digitalReadoutInstrumentBMS4");
		digitalReadoutInstrumentBMS4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS4).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS04");
		((Control)(object)digitalReadoutInstrumentBMS4).Name = "digitalReadoutInstrumentBMS4";
		digitalReadoutInstrumentBMS4.ShowBorder = false;
		digitalReadoutInstrumentBMS4.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS4).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS4).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS5, "digitalReadoutInstrumentBMS5");
		digitalReadoutInstrumentBMS5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS5).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS05");
		((Control)(object)digitalReadoutInstrumentBMS5).Name = "digitalReadoutInstrumentBMS5";
		digitalReadoutInstrumentBMS5.ShowBorder = false;
		digitalReadoutInstrumentBMS5.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS5).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS5).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS5).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS6, "digitalReadoutInstrumentBMS6");
		digitalReadoutInstrumentBMS6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS6).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS06");
		((Control)(object)digitalReadoutInstrumentBMS6).Name = "digitalReadoutInstrumentBMS6";
		digitalReadoutInstrumentBMS6.ShowBorder = false;
		digitalReadoutInstrumentBMS6.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS6).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS6).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS6).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS7, "digitalReadoutInstrumentBMS7");
		digitalReadoutInstrumentBMS7.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS7).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS07");
		((Control)(object)digitalReadoutInstrumentBMS7).Name = "digitalReadoutInstrumentBMS7";
		digitalReadoutInstrumentBMS7.ShowBorder = false;
		digitalReadoutInstrumentBMS7.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS7).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS7).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS7).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS8, "digitalReadoutInstrumentBMS8");
		digitalReadoutInstrumentBMS8.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS8).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS08");
		((Control)(object)digitalReadoutInstrumentBMS8).Name = "digitalReadoutInstrumentBMS8";
		digitalReadoutInstrumentBMS8.ShowBorder = false;
		digitalReadoutInstrumentBMS8.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS8).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS8).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS8).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBMS9, "digitalReadoutInstrumentBMS9");
		digitalReadoutInstrumentBMS9.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS9).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS09");
		((Control)(object)digitalReadoutInstrumentBMS9).Name = "digitalReadoutInstrumentBMS9";
		digitalReadoutInstrumentBMS9.ShowBorder = false;
		digitalReadoutInstrumentBMS9.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS9).TitleLengthPercentOfControl = 36;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS9).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBMS9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTIInverter1, "digitalReadoutInstrumentPTIInverter1");
		digitalReadoutInstrumentPTIInverter1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter1).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti1ActDcVolt");
		((Control)(object)digitalReadoutInstrumentPTIInverter1).Name = "digitalReadoutInstrumentPTIInverter1";
		digitalReadoutInstrumentPTIInverter1.ShowBorder = false;
		digitalReadoutInstrumentPTIInverter1.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter1).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter1).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTIInverter2, "digitalReadoutInstrumentPTIInverter2");
		digitalReadoutInstrumentPTIInverter2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter2).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti2ActDcVolt");
		((Control)(object)digitalReadoutInstrumentPTIInverter2).Name = "digitalReadoutInstrumentPTIInverter2";
		digitalReadoutInstrumentPTIInverter2.ShowBorder = false;
		digitalReadoutInstrumentPTIInverter2.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter2).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter2).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter2).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTIInverter3, "digitalReadoutInstrumentPTIInverter3");
		digitalReadoutInstrumentPTIInverter3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter3).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti3ActDcVolt_3");
		((Control)(object)digitalReadoutInstrumentPTIInverter3).Name = "digitalReadoutInstrumentPTIInverter3";
		digitalReadoutInstrumentPTIInverter3.ShowBorder = false;
		digitalReadoutInstrumentPTIInverter3.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter3).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter3).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter3).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTIInverter3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDCLConverter, "digitalReadoutInstrumentDCLConverter");
		digitalReadoutInstrumentDCLConverter.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDCLConverter).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDCLConverter).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalDCL");
		((Control)(object)digitalReadoutInstrumentDCLConverter).Name = "digitalReadoutInstrumentDCLConverter";
		digitalReadoutInstrumentDCLConverter.ShowBorder = false;
		digitalReadoutInstrumentDCLConverter.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDCLConverter).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentDCLConverter).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentDCLConverter).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentDCLConverter).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTC3, "digitalReadoutInstrumentPTC3");
		digitalReadoutInstrumentPTC3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC3).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab1");
		((Control)(object)digitalReadoutInstrumentPTC3).Name = "digitalReadoutInstrumentPTC3";
		digitalReadoutInstrumentPTC3.ShowBorder = false;
		digitalReadoutInstrumentPTC3.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC3).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC3).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC3).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTC1, "digitalReadoutInstrumentPTC1");
		digitalReadoutInstrumentPTC1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC1).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt1HvVoltage");
		((Control)(object)digitalReadoutInstrumentPTC1).Name = "digitalReadoutInstrumentPTC1";
		digitalReadoutInstrumentPTC1.ShowBorder = false;
		digitalReadoutInstrumentPTC1.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC1).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC1).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPTC2, "digitalReadoutInstrumentPTC2");
		digitalReadoutInstrumentPTC2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC2).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt2HvVoltage");
		((Control)(object)digitalReadoutInstrumentPTC2).Name = "digitalReadoutInstrumentPTC2";
		digitalReadoutInstrumentPTC2.ShowBorder = false;
		digitalReadoutInstrumentPTC2.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC2).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC2).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC2).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentPTC2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEAC, "digitalReadoutInstrumentEAC");
		digitalReadoutInstrumentEAC.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEAC).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEAC).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalEComp");
		((Control)(object)digitalReadoutInstrumentEAC).Name = "digitalReadoutInstrumentEAC";
		digitalReadoutInstrumentEAC.ShowBorder = false;
		digitalReadoutInstrumentEAC.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEAC).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentEAC).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentEAC).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentEAC).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentERC, "digitalReadoutInstrumentERC");
		digitalReadoutInstrumentERC.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrumentERC).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentERC).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_ErcHvVolt");
		((Control)(object)digitalReadoutInstrumentERC).Name = "digitalReadoutInstrumentERC";
		digitalReadoutInstrumentERC.ShowBorder = false;
		digitalReadoutInstrumentERC.ShowScalingValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentERC).TitleLengthPercentOfControl = 40;
		((SingleInstrumentBase)digitalReadoutInstrumentERC).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentERC).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentERC).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelStatusComponentVoltages, "tableLayoutPanelStatusComponentVoltages");
		((Control)(object)tableLayoutPanelStatusComponentVoltages).BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelStatusComponentVoltages, 2);
		((TableLayoutPanel)(object)tableLayoutPanelStatusComponentVoltages).Controls.Add((Control)(object)sharedProcedureSelectionMeasurement, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusComponentVoltages).Controls.Add(buttonStartStopHVMeasurement, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusComponentVoltages).Controls.Add(labelVoltagesRoutine, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusComponentVoltages).Controls.Add((Control)(object)checkmarkVoltagesRoutine, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusComponentVoltages).Controls.Add(label1, 3, 0);
		((Control)(object)tableLayoutPanelStatusComponentVoltages).Name = "tableLayoutPanelStatusComponentVoltages";
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		componentResourceManager.ApplyResources(tableLayoutPanelTop, "tableLayoutPanelTop");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelTop, 2);
		((TableLayoutPanel)(object)tableLayoutPanelTop).Controls.Add(pictureBoxWarningIcon, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTop).Controls.Add(webBrowserWarning, 1, 0);
		((Control)(object)tableLayoutPanelTop).Name = "tableLayoutPanelTop";
		pictureBoxWarningIcon.BackColor = Color.White;
		componentResourceManager.ApplyResources(pictureBoxWarningIcon, "pictureBoxWarningIcon");
		pictureBoxWarningIcon.Name = "pictureBoxWarningIcon";
		pictureBoxWarningIcon.TabStop = false;
		componentResourceManager.ApplyResources(webBrowserWarning, "webBrowserWarning");
		webBrowserWarning.Name = "webBrowserWarning";
		webBrowserWarning.Url = new Uri("about: blank", UriKind.Absolute);
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_High_Voltage_Measurement");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelContent).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelStatusComponentVoltages).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelStatusComponentVoltages).PerformLayout();
		((Control)(object)tableLayoutPanelTop).ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxWarningIcon).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
