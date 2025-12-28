using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel;

public class UserPanel : CustomPanel
{
	private static string DynamicCalibrationStatus = "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus";

	private static string DynamicCalibrationProgress = "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress";

	private static string DynamicCalibrationOutOfProfileCause = "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationOutOfProfileCause";

	private static string ChannelSrrrName = "SRRR02T";

	private static string ChannelSrrlName = "SRRL02T";

	private static string ChannelSrrfrName = "SRRFR02T";

	private static string ChannelSrrflName = "SRRFL02T";

	private static string SrrrSpName = "SRRR-Calibration";

	private static string SrrlSpName = "SRRL-Calibration";

	private static string SrrfrSpName = "SRRFR-Calibration";

	private static string SrrflSpName = "SRRFL-Calibration";

	private static string ButtonCalibrate = "Calibrate";

	private static string ButtonStop = "Stop";

	private Channel channelSrrr = null;

	private Channel channelSrrl = null;

	private Channel channelSrrfr = null;

	private Channel channelSrrfl = null;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private TableLayoutPanel tableLayoutPanelButtons;

	private SeekTimeListView seekTimeListViewLog;

	private Checkmark checkmarkStatusSrrfl;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentSrrr;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentSrrr;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentSrrl;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentSrrl;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentSrrfr;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentSrrfr;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentSrrfl;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentSrrfl;

	private SharedProcedureSelection sharedProcedureSelectionSrrl;

	private Checkmark checkmarkStatusSrrr;

	private System.Windows.Forms.Label labelStatusSrrr;

	private System.Windows.Forms.Label labelStatusSrrl;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private System.Windows.Forms.Label labelStatusSrrfr;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private System.Windows.Forms.Label labelStatusSrrfl;

	private DigitalReadoutInstrument digitalReadoutInstrumentRequestResultsStatus;

	private SharedProcedureSelection sharedProcedureSelectionSrrr;

	private SharedProcedureSelection sharedProcedureSelectionSrrfr;

	private SharedProcedureSelection sharedProcedureSelectionSrrfl;

	private Checkmark checkmarkStatusSrrfr;

	private Checkmark checkmarkStatusSrrl;

	private Button buttonStartStopSrrfl;

	private Button buttonStartStopSrrfr;

	private Button buttonStartStopSrrr;

	private Button buttonStartStopSrrl;

	private TableLayoutPanel tableLayoutPanelHeader;

	private WebBrowser webBrowserMessage;

	private BarInstrument barInstrumentProcedureProgressSRRR;

	private BarInstrument barInstrumentProcedureProgressSRRL;

	private BarInstrument barInstrumentProcedureProgressSRRFR;

	private BarInstrument barInstrumentProcedureProgressSRRFL;

	private DialInstrument dialInstrumentVehicleSpeed;

	private Button buttonCalibrateStartStop;

	public UserPanel()
	{
		InitializeComponent();
		string text = "html { height:100%; display: table; } ";
		text += "body { margin: 0px; padding: 5px; display: table-cell; vertical-align: middle; } ";
		text += ".scaled { font-size: calc(0.3vw + 9vh); font-family: Segoe UI; padding: 0px; margin: 0px; }  ";
		string format = "<html><style>{0}</style><body><span class='scaled'>{1}</span></body></html>";
		webBrowserMessage.DocumentText = string.Format(CultureInfo.InvariantCulture, format, text, Resources.Message_MessageText);
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelectionSrrr.AnyProcedureInProgress || sharedProcedureSelectionSrrfr.AnyProcedureInProgress || sharedProcedureSelectionSrrl.AnyProcedureInProgress || sharedProcedureSelectionSrrfl.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			channelSrrr = null;
			channelSrrl = null;
			channelSrrfr = null;
			channelSrrfl = null;
		}
	}

	public override void OnChannelsChanged()
	{
		SetSrrrChannel(((CustomPanel)this).GetChannel(ChannelSrrrName, (ChannelLookupOptions)3));
		SetSrrlChannel(((CustomPanel)this).GetChannel(ChannelSrrlName, (ChannelLookupOptions)3));
		SetSrrfrChannel(((CustomPanel)this).GetChannel(ChannelSrrfrName, (ChannelLookupOptions)3));
		SetSrrflChannel(((CustomPanel)this).GetChannel(ChannelSrrflName, (ChannelLookupOptions)3));
		UpdateCalibrateButton();
	}

	private void SetSrrrChannel(Channel channel)
	{
		if (channelSrrr != channel)
		{
			channelSrrr = channel;
		}
	}

	private void SetSrrlChannel(Channel channel)
	{
		if (channelSrrl != channel)
		{
			channelSrrl = channel;
		}
	}

	private void SetSrrfrChannel(Channel channel)
	{
		if (channelSrrfr != channel)
		{
			channelSrrfr = channel;
		}
	}

	private void SetSrrflChannel(Channel channel)
	{
		if (channelSrrfl != channel)
		{
			channelSrrfl = channel;
		}
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewLog.RequiredUserLabelPrefix, text);
	}

	private void sharedProcedureCreatorComponent_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		if (val != null)
		{
			if (val.Name.Equals(SrrrSpName))
			{
				LogText($"{ChannelSrrrName}: {Resources.Message_DynamicCalibrationSDAStarted}");
			}
			else if (val.Name.Equals(SrrlSpName))
			{
				LogText($"{ChannelSrrlName}: {Resources.Message_DynamicCalibrationSDAStarted}");
			}
			else if (val.Name.Equals(SrrfrSpName))
			{
				LogText($"{ChannelSrrfrName}: {Resources.Message_DynamicCalibrationSDAStarted}");
			}
			else if (val.Name.Equals(SrrflSpName))
			{
				LogText($"{ChannelSrrflName}: {Resources.Message_DynamicCalibrationSDAStarted}");
			}
		}
	}

	private void ResetFaultCodes(Channel channel)
	{
		if (channel != null)
		{
			LogText($"{channel.Ecu.Name}: {Resources.Message_ResettingFaultCodes}");
			channel.FaultCodes.Reset(synchronous: false);
		}
	}

	private void StopServiceComplete(SharedProcedureBase procedure, string channelName, Channel channel)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		LogText($"{channelName}: {Resources.Message_DynamicCalibrationSDAStopped}");
		if ((int)procedure.Result == 1)
		{
			ResetFaultCodes(channel);
		}
	}

	private void sharedProcedureCreatorComponent_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		if (val != null)
		{
			if (val.Name.Equals(SrrrSpName))
			{
				StopServiceComplete(val, ChannelSrrrName, channelSrrr);
			}
			else if (val.Name.Equals(SrrlSpName))
			{
				StopServiceComplete(val, ChannelSrrlName, channelSrrl);
			}
			else if (val.Name.Equals(SrrfrSpName))
			{
				StopServiceComplete(val, ChannelSrrfrName, channelSrrfr);
			}
			else if (val.Name.Equals(SrrflSpName))
			{
				StopServiceComplete(val, ChannelSrrflName, channelSrrfl);
			}
		}
		UpdateCalibrateButton();
	}

	private void UpdateCalibrateButton()
	{
		buttonCalibrateStartStop.Enabled = buttonStartStopSrrr.Enabled || buttonStartStopSrrl.Enabled || buttonStartStopSrrfr.Enabled || buttonStartStopSrrl.Enabled;
	}

	private void buttonStartStop_EnabledChanged(object sender, EventArgs e)
	{
		UpdateCalibrateButton();
	}

	private void service_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= service_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			string arg = string.Empty;
			if (service.Qualifier == DynamicCalibrationStatus)
			{
				arg = "Status";
			}
			else if (service.Qualifier == DynamicCalibrationProgress)
			{
				arg = "Progress";
			}
			else if (service.Qualifier == DynamicCalibrationOutOfProfileCause)
			{
				arg = "OutOfProfileCause";
			}
			LogText(string.Format(Resources.MessageFormat_StatusMessage, service.Channel.Ecu.Name, arg, service.OutputValues[0].Value.ToString()));
		}
	}

	private void ExecuteService(Channel channel, string serviceName)
	{
		if (channel != null)
		{
			Service service = channel.Services[serviceName];
			if (service != null)
			{
				service.ServiceCompleteEvent += service_ServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
		}
	}

	private void MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			if ((int)((MonitorResultEventArgs)e).Action == 0)
			{
				try
				{
					ExecuteService(e.Service.Channel, DynamicCalibrationStatus);
					ExecuteService(e.Service.Channel, DynamicCalibrationProgress);
					ExecuteService(e.Service.Channel, DynamicCalibrationOutOfProfileCause);
				}
				catch (CaesarException)
				{
					((MonitorResultEventArgs)e).Action = (MonitorAction)1;
				}
			}
		}
		else
		{
			((MonitorResultEventArgs)e).Action = (MonitorAction)1;
		}
	}

	private void sharedProcedureCreatorComponent_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		MonitorServiceComplete(sender, e);
	}

	private void buttonCalibrateStartStop_Click(object sender, EventArgs e)
	{
		if (buttonStartStopSrrr.Enabled)
		{
			buttonStartStopSrrr.PerformClick();
		}
		if (buttonStartStopSrrl.Enabled)
		{
			buttonStartStopSrrl.PerformClick();
		}
		if (buttonStartStopSrrfr.Enabled)
		{
			buttonStartStopSrrfr.PerformClick();
		}
		if (buttonStartStopSrrfl.Enabled)
		{
			buttonStartStopSrrfl.PerformClick();
		}
		if (buttonCalibrateStartStop.Text == ButtonCalibrate)
		{
			buttonCalibrateStartStop.Text = ButtonStop;
		}
		else
		{
			buttonCalibrateStartStop.Text = ButtonCalibrate;
		}
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1010: Unknown result type (might be due to invalid IL or missing references)
		//IL_1243: Unknown result type (might be due to invalid IL or missing references)
		//IL_1499: Unknown result type (might be due to invalid IL or missing references)
		//IL_15be: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c8: Expected O, but got Unknown
		//IL_1605: Unknown result type (might be due to invalid IL or missing references)
		//IL_160f: Expected O, but got Unknown
		//IL_164c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1656: Expected O, but got Unknown
		//IL_16db: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e5: Expected O, but got Unknown
		//IL_18fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1935: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d26: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d94: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f32: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		DataItemCondition val4 = new DataItemCondition();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		barInstrumentProcedureProgressSRRR = new BarInstrument();
		barInstrumentProcedureProgressSRRL = new BarInstrument();
		barInstrumentProcedureProgressSRRFR = new BarInstrument();
		barInstrumentProcedureProgressSRRFL = new BarInstrument();
		buttonStartStopSrrfl = new Button();
		buttonStartStopSrrfr = new Button();
		buttonStartStopSrrr = new Button();
		buttonStartStopSrrl = new Button();
		checkmarkStatusSrrr = new Checkmark();
		labelStatusSrrr = new System.Windows.Forms.Label();
		labelStatusSrrl = new System.Windows.Forms.Label();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		labelStatusSrrfr = new System.Windows.Forms.Label();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		labelStatusSrrfl = new System.Windows.Forms.Label();
		checkmarkStatusSrrfl = new Checkmark();
		digitalReadoutInstrumentRequestResultsStatus = new DigitalReadoutInstrument();
		tableLayoutPanelButtons = new TableLayoutPanel();
		sharedProcedureSelectionSrrr = new SharedProcedureSelection();
		sharedProcedureSelectionSrrfr = new SharedProcedureSelection();
		sharedProcedureSelectionSrrfl = new SharedProcedureSelection();
		buttonCalibrateStartStop = new Button();
		sharedProcedureSelectionSrrl = new SharedProcedureSelection();
		seekTimeListViewLog = new SeekTimeListView();
		checkmarkStatusSrrfr = new Checkmark();
		checkmarkStatusSrrl = new Checkmark();
		tableLayoutPanelHeader = new TableLayoutPanel();
		dialInstrumentVehicleSpeed = new DialInstrument();
		webBrowserMessage = new WebBrowser();
		sharedProcedureCreatorComponentSrrr = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentSrrr = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentSrrl = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentSrrl = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentSrrfr = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentSrrfr = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentSrrfl = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentSrrfl = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanelButtons).SuspendLayout();
		((Control)(object)tableLayoutPanelHeader).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentProcedureProgressSRRR, 5, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentProcedureProgressSRRL, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentProcedureProgressSRRFR, 5, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)barInstrumentProcedureProgressSRRFL, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonStartStopSrrfl, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonStartStopSrrfr, 8, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonStartStopSrrr, 8, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonStartStopSrrl, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)checkmarkStatusSrrr, 5, 7);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelStatusSrrr, 6, 7);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelStatusSrrl, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument3, 5, 5);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelStatusSrrfr, 6, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument2, 5, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelStatusSrrfl, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)checkmarkStatusSrrfl, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrumentRequestResultsStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelButtons, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)checkmarkStatusSrrfr, 5, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)checkmarkStatusSrrl, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentProcedureProgressSRRR, 3);
		componentResourceManager.ApplyResources(barInstrumentProcedureProgressSRRR, "barInstrumentProcedureProgressSRRR");
		barInstrumentProcedureProgressSRRR.FontGroup = null;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRR).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).Gradient.Initialize((ValueState)0, 2, "%");
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).Gradient.Modify(2, 101.0, (ValueState)0);
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRR).Instrument = new Qualifier((QualifierTypes)64, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
		((Control)(object)barInstrumentProcedureProgressSRRR).Name = "barInstrumentProcedureProgressSRRR";
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRR).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRR).ShowValueReadout = false;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRR).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRR).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentProcedureProgressSRRL, 3);
		componentResourceManager.ApplyResources(barInstrumentProcedureProgressSRRL, "barInstrumentProcedureProgressSRRL");
		barInstrumentProcedureProgressSRRL.FontGroup = null;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRL).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).Gradient.Initialize((ValueState)0, 2, "%");
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).Gradient.Modify(2, 101.0, (ValueState)0);
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRL).Instrument = new Qualifier((QualifierTypes)64, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
		((Control)(object)barInstrumentProcedureProgressSRRL).Name = "barInstrumentProcedureProgressSRRL";
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRL).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRL).ShowValueReadout = false;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRL).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRL).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentProcedureProgressSRRFR, 3);
		componentResourceManager.ApplyResources(barInstrumentProcedureProgressSRRFR, "barInstrumentProcedureProgressSRRFR");
		barInstrumentProcedureProgressSRRFR.FontGroup = null;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFR).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Gradient.Initialize((ValueState)0, 2, "%");
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Gradient.Modify(2, 101.0, (ValueState)0);
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFR).Instrument = new Qualifier((QualifierTypes)64, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
		((Control)(object)barInstrumentProcedureProgressSRRFR).Name = "barInstrumentProcedureProgressSRRFR";
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFR).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFR).ShowValueReadout = false;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFR).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFR).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)barInstrumentProcedureProgressSRRFL, 3);
		componentResourceManager.ApplyResources(barInstrumentProcedureProgressSRRFL, "barInstrumentProcedureProgressSRRFL");
		barInstrumentProcedureProgressSRRFL.FontGroup = null;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFL).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Gradient.Initialize((ValueState)0, 2, "%");
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Gradient.Modify(2, 101.0, (ValueState)0);
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFL).Instrument = new Qualifier((QualifierTypes)64, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
		((Control)(object)barInstrumentProcedureProgressSRRFL).Name = "barInstrumentProcedureProgressSRRFL";
		((AxisSingleInstrumentBase)barInstrumentProcedureProgressSRRFL).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFL).ShowValueReadout = false;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFL).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrumentProcedureProgressSRRFL).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonStartStopSrrfl, "buttonStartStopSrrfl");
		buttonStartStopSrrfl.Name = "buttonStartStopSrrfl";
		buttonStartStopSrrfl.UseCompatibleTextRendering = true;
		buttonStartStopSrrfl.UseVisualStyleBackColor = true;
		buttonStartStopSrrfl.EnabledChanged += buttonStartStop_EnabledChanged;
		componentResourceManager.ApplyResources(buttonStartStopSrrfr, "buttonStartStopSrrfr");
		buttonStartStopSrrfr.Name = "buttonStartStopSrrfr";
		buttonStartStopSrrfr.UseCompatibleTextRendering = true;
		buttonStartStopSrrfr.UseVisualStyleBackColor = true;
		buttonStartStopSrrfr.EnabledChanged += buttonStartStop_EnabledChanged;
		componentResourceManager.ApplyResources(buttonStartStopSrrr, "buttonStartStopSrrr");
		buttonStartStopSrrr.Name = "buttonStartStopSrrr";
		buttonStartStopSrrr.UseCompatibleTextRendering = true;
		buttonStartStopSrrr.UseVisualStyleBackColor = true;
		buttonStartStopSrrr.EnabledChanged += buttonStartStop_EnabledChanged;
		componentResourceManager.ApplyResources(buttonStartStopSrrl, "buttonStartStopSrrl");
		buttonStartStopSrrl.Name = "buttonStartStopSrrl";
		buttonStartStopSrrl.UseCompatibleTextRendering = true;
		buttonStartStopSrrl.UseVisualStyleBackColor = true;
		buttonStartStopSrrl.EnabledChanged += buttonStartStop_EnabledChanged;
		componentResourceManager.ApplyResources(checkmarkStatusSrrr, "checkmarkStatusSrrr");
		((Control)(object)checkmarkStatusSrrr).Name = "checkmarkStatusSrrr";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)labelStatusSrrr, 2);
		componentResourceManager.ApplyResources(labelStatusSrrr, "labelStatusSrrr");
		labelStatusSrrr.Name = "labelStatusSrrr";
		labelStatusSrrr.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)labelStatusSrrl, 2);
		componentResourceManager.ApplyResources(labelStatusSrrl, "labelStatusSrrl");
		labelStatusSrrl.Name = "labelStatusSrrl";
		labelStatusSrrl.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutInstrument3, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "SRRRInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrument3.Gradient.Modify(5, 4.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)64, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "SRRRInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrument1.Gradient.Modify(5, 4.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)64, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)labelStatusSrrfr, 2);
		componentResourceManager.ApplyResources(labelStatusSrrfr, "labelStatusSrrfr");
		labelStatusSrrfr.Name = "labelStatusSrrfr";
		labelStatusSrrfr.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "SRRRInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument2.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrument2.Gradient.Modify(5, 4.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)64, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)labelStatusSrrfl, 2);
		componentResourceManager.ApplyResources(labelStatusSrrfl, "labelStatusSrrfl");
		labelStatusSrrfl.Name = "labelStatusSrrfl";
		labelStatusSrrfl.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkStatusSrrfl, "checkmarkStatusSrrfl");
		((Control)(object)checkmarkStatusSrrfl).Name = "checkmarkStatusSrrfl";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentRequestResultsStatus, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRequestResultsStatus, "digitalReadoutInstrumentRequestResultsStatus");
		digitalReadoutInstrumentRequestResultsStatus.FontGroup = "SRRRInstruments";
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).FreezeValue = false;
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(5, 4.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).Instrument = new Qualifier((QualifierTypes)64, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
		((Control)(object)digitalReadoutInstrumentRequestResultsStatus).Name = "digitalReadoutInstrumentRequestResultsStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentRequestResultsStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelButtons, "tableLayoutPanelButtons");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanelButtons, 8);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)sharedProcedureSelectionSrrr, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)sharedProcedureSelectionSrrfr, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)sharedProcedureSelectionSrrfl, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(buttonCalibrateStartStop, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)sharedProcedureSelectionSrrl, 3, 0);
		((Control)(object)tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
		componentResourceManager.ApplyResources(sharedProcedureSelectionSrrr, "sharedProcedureSelectionSrrr");
		((Control)(object)sharedProcedureSelectionSrrr).Name = "sharedProcedureSelectionSrrr";
		sharedProcedureSelectionSrrr.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_SRRRCalibration" });
		componentResourceManager.ApplyResources(sharedProcedureSelectionSrrfr, "sharedProcedureSelectionSrrfr");
		((Control)(object)sharedProcedureSelectionSrrfr).Name = "sharedProcedureSelectionSrrfr";
		sharedProcedureSelectionSrrfr.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_SRRFRCalibration" });
		componentResourceManager.ApplyResources(sharedProcedureSelectionSrrfl, "sharedProcedureSelectionSrrfl");
		((Control)(object)sharedProcedureSelectionSrrfl).Name = "sharedProcedureSelectionSrrfl";
		sharedProcedureSelectionSrrfl.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_SRRFLCalibration" });
		componentResourceManager.ApplyResources(buttonCalibrateStartStop, "buttonCalibrateStartStop");
		buttonCalibrateStartStop.Name = "buttonCalibrateStartStop";
		buttonCalibrateStartStop.UseVisualStyleBackColor = true;
		buttonCalibrateStartStop.Click += buttonCalibrateStartStop_Click;
		componentResourceManager.ApplyResources(sharedProcedureSelectionSrrl, "sharedProcedureSelectionSrrl");
		((Control)(object)sharedProcedureSelectionSrrl).Name = "sharedProcedureSelectionSrrl";
		sharedProcedureSelectionSrrl.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_SRRLCalibration" });
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)seekTimeListViewLog, 9);
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "SRRRCalibration";
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		seekTimeListViewLog.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(checkmarkStatusSrrfr, "checkmarkStatusSrrfr");
		((Control)(object)checkmarkStatusSrrfr).Name = "checkmarkStatusSrrfr";
		componentResourceManager.ApplyResources(checkmarkStatusSrrl, "checkmarkStatusSrrl");
		((Control)(object)checkmarkStatusSrrl).Name = "checkmarkStatusSrrl";
		componentResourceManager.ApplyResources(tableLayoutPanelHeader, "tableLayoutPanelHeader");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanelHeader, 9);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)dialInstrumentVehicleSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add(webBrowserMessage, 1, 0);
		((Control)(object)tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
		dialInstrumentVehicleSpeed.AngleRange = 180.0;
		dialInstrumentVehicleSpeed.AngleStart = 180.0;
		componentResourceManager.ApplyResources(dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
		dialInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState)3, 2, "km/h");
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(1, 30.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(2, 50.0, (ValueState)3);
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 45.0, "mph");
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(webBrowserMessage, "webBrowserMessage");
		webBrowserMessage.Name = "webBrowserMessage";
		webBrowserMessage.Url = new Uri("about: blank", UriKind.Absolute);
		sharedProcedureCreatorComponentSrrr.Suspend();
		sharedProcedureCreatorComponentSrrr.MonitorCall = new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
		sharedProcedureCreatorComponentSrrr.MonitorInterval = 2500;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentSrrr, "sharedProcedureCreatorComponentSrrr");
		sharedProcedureCreatorComponentSrrr.Qualifier = "SP_SRRRCalibration";
		sharedProcedureCreatorComponentSrrr.StartCall = new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Start");
		val.Gradient.Initialize((ValueState)3, 1, "km/h");
		val.Gradient.Modify(1, 1.0, (ValueState)1);
		val.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		sharedProcedureCreatorComponentSrrr.StartConditions.Add(val);
		sharedProcedureCreatorComponentSrrr.StopCall = new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Stop");
		sharedProcedureCreatorComponentSrrr.StartServiceComplete += sharedProcedureCreatorComponent_StartServiceComplete;
		sharedProcedureCreatorComponentSrrr.StopServiceComplete += sharedProcedureCreatorComponent_StopServiceComplete;
		sharedProcedureCreatorComponentSrrr.MonitorServiceComplete += sharedProcedureCreatorComponent_MonitorServiceComplete;
		sharedProcedureCreatorComponentSrrr.Resume();
		sharedProcedureIntegrationComponentSrrr.ProceduresDropDown = sharedProcedureSelectionSrrr;
		sharedProcedureIntegrationComponentSrrr.ProcedureStatusMessageTarget = labelStatusSrrr;
		sharedProcedureIntegrationComponentSrrr.ProcedureStatusStateTarget = checkmarkStatusSrrr;
		sharedProcedureIntegrationComponentSrrr.ResultsTarget = null;
		sharedProcedureIntegrationComponentSrrr.StartStopButton = buttonStartStopSrrr;
		sharedProcedureIntegrationComponentSrrr.StopAllButton = null;
		sharedProcedureCreatorComponentSrrl.Suspend();
		sharedProcedureCreatorComponentSrrl.MonitorCall = new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
		sharedProcedureCreatorComponentSrrl.MonitorInterval = 2500;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentSrrl, "sharedProcedureCreatorComponentSrrl");
		sharedProcedureCreatorComponentSrrl.Qualifier = "SP_SRRLCalibration";
		sharedProcedureCreatorComponentSrrl.StartCall = new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Start");
		val2.Gradient.Initialize((ValueState)3, 1, "km/h");
		val2.Gradient.Modify(1, 1.0, (ValueState)1);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		sharedProcedureCreatorComponentSrrl.StartConditions.Add(val2);
		sharedProcedureCreatorComponentSrrl.StopCall = new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Stop");
		sharedProcedureCreatorComponentSrrl.StartServiceComplete += sharedProcedureCreatorComponent_StartServiceComplete;
		sharedProcedureCreatorComponentSrrl.StopServiceComplete += sharedProcedureCreatorComponent_StopServiceComplete;
		sharedProcedureCreatorComponentSrrl.MonitorServiceComplete += sharedProcedureCreatorComponent_MonitorServiceComplete;
		sharedProcedureCreatorComponentSrrl.Resume();
		sharedProcedureIntegrationComponentSrrl.ProceduresDropDown = sharedProcedureSelectionSrrl;
		sharedProcedureIntegrationComponentSrrl.ProcedureStatusMessageTarget = labelStatusSrrl;
		sharedProcedureIntegrationComponentSrrl.ProcedureStatusStateTarget = checkmarkStatusSrrl;
		sharedProcedureIntegrationComponentSrrl.ResultsTarget = null;
		sharedProcedureIntegrationComponentSrrl.StartStopButton = buttonStartStopSrrl;
		sharedProcedureIntegrationComponentSrrl.StopAllButton = null;
		sharedProcedureCreatorComponentSrrfr.Suspend();
		sharedProcedureCreatorComponentSrrfr.MonitorCall = new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
		sharedProcedureCreatorComponentSrrfr.MonitorInterval = 2500;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentSrrfr, "sharedProcedureCreatorComponentSrrfr");
		sharedProcedureCreatorComponentSrrfr.Qualifier = "SP_SRRFRCalibration";
		sharedProcedureCreatorComponentSrrfr.StartCall = new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Start");
		val3.Gradient.Initialize((ValueState)3, 1, "km/h");
		val3.Gradient.Modify(1, 1.0, (ValueState)1);
		val3.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		sharedProcedureCreatorComponentSrrfr.StartConditions.Add(val3);
		sharedProcedureCreatorComponentSrrfr.StopCall = new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Stop");
		sharedProcedureCreatorComponentSrrfr.StartServiceComplete += sharedProcedureCreatorComponent_StartServiceComplete;
		sharedProcedureCreatorComponentSrrfr.StopServiceComplete += sharedProcedureCreatorComponent_StopServiceComplete;
		sharedProcedureCreatorComponentSrrfr.MonitorServiceComplete += sharedProcedureCreatorComponent_MonitorServiceComplete;
		sharedProcedureCreatorComponentSrrfr.Resume();
		sharedProcedureIntegrationComponentSrrfr.ProceduresDropDown = sharedProcedureSelectionSrrfr;
		sharedProcedureIntegrationComponentSrrfr.ProcedureStatusMessageTarget = labelStatusSrrfr;
		sharedProcedureIntegrationComponentSrrfr.ProcedureStatusStateTarget = checkmarkStatusSrrfr;
		sharedProcedureIntegrationComponentSrrfr.ResultsTarget = null;
		sharedProcedureIntegrationComponentSrrfr.StartStopButton = buttonStartStopSrrfr;
		sharedProcedureIntegrationComponentSrrfr.StopAllButton = null;
		sharedProcedureCreatorComponentSrrfl.Suspend();
		sharedProcedureCreatorComponentSrrfl.MonitorCall = new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
		sharedProcedureCreatorComponentSrrfl.MonitorInterval = 2500;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentSrrfl, "sharedProcedureCreatorComponentSrrfl");
		sharedProcedureCreatorComponentSrrfl.Qualifier = "SP_SRRFLCalibration";
		sharedProcedureCreatorComponentSrrfl.StartCall = new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Start");
		val4.Gradient.Initialize((ValueState)3, 1, "km/h");
		val4.Gradient.Modify(1, 1.0, (ValueState)1);
		val4.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		sharedProcedureCreatorComponentSrrfl.StartConditions.Add(val4);
		sharedProcedureCreatorComponentSrrfl.StopCall = new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Stop");
		sharedProcedureCreatorComponentSrrfl.StartServiceComplete += sharedProcedureCreatorComponent_StartServiceComplete;
		sharedProcedureCreatorComponentSrrfl.StopServiceComplete += sharedProcedureCreatorComponent_StopServiceComplete;
		sharedProcedureCreatorComponentSrrfl.MonitorServiceComplete += sharedProcedureCreatorComponent_MonitorServiceComplete;
		sharedProcedureCreatorComponentSrrfl.Resume();
		sharedProcedureIntegrationComponentSrrfl.ProceduresDropDown = sharedProcedureSelectionSrrfl;
		sharedProcedureIntegrationComponentSrrfl.ProcedureStatusMessageTarget = labelStatusSrrfl;
		sharedProcedureIntegrationComponentSrrfl.ProcedureStatusStateTarget = checkmarkStatusSrrfl;
		sharedProcedureIntegrationComponentSrrfl.ResultsTarget = null;
		sharedProcedureIntegrationComponentSrrfl.StartStopButton = buttonStartStopSrrfl;
		sharedProcedureIntegrationComponentSrrfl.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelButtons).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelHeader).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
