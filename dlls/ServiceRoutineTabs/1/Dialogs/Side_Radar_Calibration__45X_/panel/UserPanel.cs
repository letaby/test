// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
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
  private Channel channelSrrr = (Channel) null;
  private Channel channelSrrl = (Channel) null;
  private Channel channelSrrfr = (Channel) null;
  private Channel channelSrrfl = (Channel) null;
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
    this.InitializeComponent();
    this.webBrowserMessage.DocumentText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<html><style>{0}</style><body><span class='scaled'>{1}</span></body></html>", (object) ("html { height:100%; display: table; } " + "body { margin: 0px; padding: 5px; display: table-cell; vertical-align: middle; } " + ".scaled { font-size: calc(0.3vw + 9vh); font-family: Segoe UI; padding: 0px; margin: 0px; }  "), (object) Resources.Message_MessageText);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelectionSrrr.AnyProcedureInProgress || this.sharedProcedureSelectionSrrfr.AnyProcedureInProgress || this.sharedProcedureSelectionSrrl.AnyProcedureInProgress || this.sharedProcedureSelectionSrrfl.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.channelSrrr = (Channel) null;
    this.channelSrrl = (Channel) null;
    this.channelSrrfr = (Channel) null;
    this.channelSrrfl = (Channel) null;
  }

  public virtual void OnChannelsChanged()
  {
    this.SetSrrrChannel(this.GetChannel(UserPanel.ChannelSrrrName, (CustomPanel.ChannelLookupOptions) 3));
    this.SetSrrlChannel(this.GetChannel(UserPanel.ChannelSrrlName, (CustomPanel.ChannelLookupOptions) 3));
    this.SetSrrfrChannel(this.GetChannel(UserPanel.ChannelSrrfrName, (CustomPanel.ChannelLookupOptions) 3));
    this.SetSrrflChannel(this.GetChannel(UserPanel.ChannelSrrflName, (CustomPanel.ChannelLookupOptions) 3));
    this.UpdateCalibrateButton();
  }

  private void SetSrrrChannel(Channel channel)
  {
    if (this.channelSrrr == channel)
      return;
    this.channelSrrr = channel;
  }

  private void SetSrrlChannel(Channel channel)
  {
    if (this.channelSrrl == channel)
      return;
    this.channelSrrl = channel;
  }

  private void SetSrrfrChannel(Channel channel)
  {
    if (this.channelSrrfr == channel)
      return;
    this.channelSrrfr = channel;
  }

  private void SetSrrflChannel(Channel channel)
  {
    if (this.channelSrrfl == channel)
      return;
    this.channelSrrfl = channel;
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListViewLog.RequiredUserLabelPrefix, text);
  }

  private void sharedProcedureCreatorComponent_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (!(sender is SharedProcedureBase sharedProcedureBase))
      return;
    if (sharedProcedureBase.Name.Equals(UserPanel.SrrrSpName))
      this.LogText($"{UserPanel.ChannelSrrrName}: {Resources.Message_DynamicCalibrationSDAStarted}");
    else if (sharedProcedureBase.Name.Equals(UserPanel.SrrlSpName))
      this.LogText($"{UserPanel.ChannelSrrlName}: {Resources.Message_DynamicCalibrationSDAStarted}");
    else if (sharedProcedureBase.Name.Equals(UserPanel.SrrfrSpName))
      this.LogText($"{UserPanel.ChannelSrrfrName}: {Resources.Message_DynamicCalibrationSDAStarted}");
    else if (sharedProcedureBase.Name.Equals(UserPanel.SrrflSpName))
      this.LogText($"{UserPanel.ChannelSrrflName}: {Resources.Message_DynamicCalibrationSDAStarted}");
  }

  private void ResetFaultCodes(Channel channel)
  {
    if (channel == null)
      return;
    this.LogText($"{channel.Ecu.Name}: {Resources.Message_ResettingFaultCodes}");
    channel.FaultCodes.Reset(false);
  }

  private void StopServiceComplete(
    SharedProcedureBase procedure,
    string channelName,
    Channel channel)
  {
    this.LogText($"{channelName}: {Resources.Message_DynamicCalibrationSDAStopped}");
    if (procedure.Result != 1)
      return;
    this.ResetFaultCodes(channel);
  }

  private void sharedProcedureCreatorComponent_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (sender is SharedProcedureBase procedure)
    {
      if (procedure.Name.Equals(UserPanel.SrrrSpName))
        this.StopServiceComplete(procedure, UserPanel.ChannelSrrrName, this.channelSrrr);
      else if (procedure.Name.Equals(UserPanel.SrrlSpName))
        this.StopServiceComplete(procedure, UserPanel.ChannelSrrlName, this.channelSrrl);
      else if (procedure.Name.Equals(UserPanel.SrrfrSpName))
        this.StopServiceComplete(procedure, UserPanel.ChannelSrrfrName, this.channelSrrfr);
      else if (procedure.Name.Equals(UserPanel.SrrflSpName))
        this.StopServiceComplete(procedure, UserPanel.ChannelSrrflName, this.channelSrrfl);
    }
    this.UpdateCalibrateButton();
  }

  private void UpdateCalibrateButton()
  {
    this.buttonCalibrateStartStop.Enabled = this.buttonStartStopSrrr.Enabled || this.buttonStartStopSrrl.Enabled || this.buttonStartStopSrrfr.Enabled || this.buttonStartStopSrrl.Enabled;
  }

  private void buttonStartStop_EnabledChanged(object sender, EventArgs e)
  {
    this.UpdateCalibrateButton();
  }

  private void service_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.service_ServiceCompleteEvent);
    if (!e.Succeeded)
      return;
    string str = string.Empty;
    if (service.Qualifier == UserPanel.DynamicCalibrationStatus)
      str = "Status";
    else if (service.Qualifier == UserPanel.DynamicCalibrationProgress)
      str = "Progress";
    else if (service.Qualifier == UserPanel.DynamicCalibrationOutOfProfileCause)
      str = "OutOfProfileCause";
    this.LogText(string.Format(Resources.MessageFormat_StatusMessage, (object) service.Channel.Ecu.Name, (object) str, (object) service.OutputValues[0].Value.ToString()));
  }

  private void ExecuteService(Channel channel, string serviceName)
  {
    if (channel == null)
      return;
    Service service = channel.Services[serviceName];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.service_ServiceCompleteEvent);
      service.Execute(false);
    }
  }

  private void MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      if (((MonitorResultEventArgs) e).Action != 0)
        return;
      try
      {
        this.ExecuteService(e.Service.Channel, UserPanel.DynamicCalibrationStatus);
        this.ExecuteService(e.Service.Channel, UserPanel.DynamicCalibrationProgress);
        this.ExecuteService(e.Service.Channel, UserPanel.DynamicCalibrationOutOfProfileCause);
      }
      catch (CaesarException ex)
      {
        ((MonitorResultEventArgs) e).Action = (MonitorAction) 1;
      }
    }
    else
      ((MonitorResultEventArgs) e).Action = (MonitorAction) 1;
  }

  private void sharedProcedureCreatorComponent_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    this.MonitorServiceComplete(sender, e);
  }

  private void buttonCalibrateStartStop_Click(object sender, EventArgs e)
  {
    if (this.buttonStartStopSrrr.Enabled)
      this.buttonStartStopSrrr.PerformClick();
    if (this.buttonStartStopSrrl.Enabled)
      this.buttonStartStopSrrl.PerformClick();
    if (this.buttonStartStopSrrfr.Enabled)
      this.buttonStartStopSrrfr.PerformClick();
    if (this.buttonStartStopSrrfl.Enabled)
      this.buttonStartStopSrrfl.PerformClick();
    if (this.buttonCalibrateStartStop.Text == UserPanel.ButtonCalibrate)
      this.buttonCalibrateStartStop.Text = UserPanel.ButtonStop;
    else
      this.buttonCalibrateStartStop.Text = UserPanel.ButtonCalibrate;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    DataItemCondition dataItemCondition4 = new DataItemCondition();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.barInstrumentProcedureProgressSRRR = new BarInstrument();
    this.barInstrumentProcedureProgressSRRL = new BarInstrument();
    this.barInstrumentProcedureProgressSRRFR = new BarInstrument();
    this.barInstrumentProcedureProgressSRRFL = new BarInstrument();
    this.buttonStartStopSrrfl = new Button();
    this.buttonStartStopSrrfr = new Button();
    this.buttonStartStopSrrr = new Button();
    this.buttonStartStopSrrl = new Button();
    this.checkmarkStatusSrrr = new Checkmark();
    this.labelStatusSrrr = new System.Windows.Forms.Label();
    this.labelStatusSrrl = new System.Windows.Forms.Label();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.labelStatusSrrfr = new System.Windows.Forms.Label();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.labelStatusSrrfl = new System.Windows.Forms.Label();
    this.checkmarkStatusSrrfl = new Checkmark();
    this.digitalReadoutInstrumentRequestResultsStatus = new DigitalReadoutInstrument();
    this.tableLayoutPanelButtons = new TableLayoutPanel();
    this.sharedProcedureSelectionSrrr = new SharedProcedureSelection();
    this.sharedProcedureSelectionSrrfr = new SharedProcedureSelection();
    this.sharedProcedureSelectionSrrfl = new SharedProcedureSelection();
    this.buttonCalibrateStartStop = new Button();
    this.sharedProcedureSelectionSrrl = new SharedProcedureSelection();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.checkmarkStatusSrrfr = new Checkmark();
    this.checkmarkStatusSrrl = new Checkmark();
    this.tableLayoutPanelHeader = new TableLayoutPanel();
    this.dialInstrumentVehicleSpeed = new DialInstrument();
    this.webBrowserMessage = new WebBrowser();
    this.sharedProcedureCreatorComponentSrrr = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentSrrr = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentSrrl = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentSrrl = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentSrrfr = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentSrrfr = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentSrrfl = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentSrrfl = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this.tableLayoutPanelButtons).SuspendLayout();
    ((Control) this.tableLayoutPanelHeader).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentProcedureProgressSRRR, 5, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentProcedureProgressSRRL, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentProcedureProgressSRRFR, 5, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentProcedureProgressSRRFL, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonStartStopSrrfl, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonStartStopSrrfr, 8, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonStartStopSrrr, 8, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonStartStopSrrl, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.checkmarkStatusSrrr, 5, 7);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelStatusSrrr, 6, 7);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelStatusSrrl, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument3, 5, 5);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelStatusSrrfr, 6, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument2, 5, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelStatusSrrfl, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.checkmarkStatusSrrfl, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrumentRequestResultsStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelButtons, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.checkmarkStatusSrrfr, 5, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.checkmarkStatusSrrl, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentProcedureProgressSRRR, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentProcedureProgressSRRR, "barInstrumentProcedureProgressSRRR");
    this.barInstrumentProcedureProgressSRRR.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Gradient.Initialize((ValueState) 0, 2, "%");
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Gradient.Modify(2, 101.0, (ValueState) 0);
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
    ((Control) this.barInstrumentProcedureProgressSRRR).Name = "barInstrumentProcedureProgressSRRR";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).ShowValueReadout = false;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRR).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentProcedureProgressSRRL, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentProcedureProgressSRRL, "barInstrumentProcedureProgressSRRL");
    this.barInstrumentProcedureProgressSRRL.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Gradient.Initialize((ValueState) 0, 2, "%");
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Gradient.Modify(2, 101.0, (ValueState) 0);
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
    ((Control) this.barInstrumentProcedureProgressSRRL).Name = "barInstrumentProcedureProgressSRRL";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).ShowValueReadout = false;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRL).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentProcedureProgressSRRFR, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentProcedureProgressSRRFR, "barInstrumentProcedureProgressSRRFR");
    this.barInstrumentProcedureProgressSRRFR.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Gradient.Initialize((ValueState) 0, 2, "%");
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Gradient.Modify(2, 101.0, (ValueState) 0);
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
    ((Control) this.barInstrumentProcedureProgressSRRFR).Name = "barInstrumentProcedureProgressSRRFR";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).ShowValueReadout = false;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFR).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentProcedureProgressSRRFL, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentProcedureProgressSRRFL, "barInstrumentProcedureProgressSRRFL");
    this.barInstrumentProcedureProgressSRRFL.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Gradient.Initialize((ValueState) 0, 2, "%");
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Gradient.Modify(2, 101.0, (ValueState) 0);
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress");
    ((Control) this.barInstrumentProcedureProgressSRRFL).Name = "barInstrumentProcedureProgressSRRFL";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).ShowValueReadout = false;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgressSRRFL).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStartStopSrrfl, "buttonStartStopSrrfl");
    this.buttonStartStopSrrfl.Name = "buttonStartStopSrrfl";
    this.buttonStartStopSrrfl.UseCompatibleTextRendering = true;
    this.buttonStartStopSrrfl.UseVisualStyleBackColor = true;
    this.buttonStartStopSrrfl.EnabledChanged += new EventHandler(this.buttonStartStop_EnabledChanged);
    componentResourceManager.ApplyResources((object) this.buttonStartStopSrrfr, "buttonStartStopSrrfr");
    this.buttonStartStopSrrfr.Name = "buttonStartStopSrrfr";
    this.buttonStartStopSrrfr.UseCompatibleTextRendering = true;
    this.buttonStartStopSrrfr.UseVisualStyleBackColor = true;
    this.buttonStartStopSrrfr.EnabledChanged += new EventHandler(this.buttonStartStop_EnabledChanged);
    componentResourceManager.ApplyResources((object) this.buttonStartStopSrrr, "buttonStartStopSrrr");
    this.buttonStartStopSrrr.Name = "buttonStartStopSrrr";
    this.buttonStartStopSrrr.UseCompatibleTextRendering = true;
    this.buttonStartStopSrrr.UseVisualStyleBackColor = true;
    this.buttonStartStopSrrr.EnabledChanged += new EventHandler(this.buttonStartStop_EnabledChanged);
    componentResourceManager.ApplyResources((object) this.buttonStartStopSrrl, "buttonStartStopSrrl");
    this.buttonStartStopSrrl.Name = "buttonStartStopSrrl";
    this.buttonStartStopSrrl.UseCompatibleTextRendering = true;
    this.buttonStartStopSrrl.UseVisualStyleBackColor = true;
    this.buttonStartStopSrrl.EnabledChanged += new EventHandler(this.buttonStartStop_EnabledChanged);
    componentResourceManager.ApplyResources((object) this.checkmarkStatusSrrr, "checkmarkStatusSrrr");
    ((Control) this.checkmarkStatusSrrr).Name = "checkmarkStatusSrrr";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.labelStatusSrrr, 2);
    componentResourceManager.ApplyResources((object) this.labelStatusSrrr, "labelStatusSrrr");
    this.labelStatusSrrr.Name = "labelStatusSrrr";
    this.labelStatusSrrr.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.labelStatusSrrl, 2);
    componentResourceManager.ApplyResources((object) this.labelStatusSrrl, "labelStatusSrrl");
    this.labelStatusSrrl.Name = "labelStatusSrrl";
    this.labelStatusSrrl.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrument3, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "SRRRInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
    this.digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 0, 5);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrument3.Gradient.Modify(5, 4.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrument1, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "SRRRInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 0, 5);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrument1.Gradient.Modify(5, 4.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.labelStatusSrrfr, 2);
    componentResourceManager.ApplyResources((object) this.labelStatusSrrfr, "labelStatusSrrfr");
    this.labelStatusSrrfr.Name = "labelStatusSrrfr";
    this.labelStatusSrrfr.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrument2, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "SRRRInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 0, 5);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument2.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrument2.Gradient.Modify(5, 4.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.labelStatusSrrfl, 2);
    componentResourceManager.ApplyResources((object) this.labelStatusSrrfl, "labelStatusSrrfl");
    this.labelStatusSrrfl.Name = "labelStatusSrrfl";
    this.labelStatusSrrfl.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkStatusSrrfl, "checkmarkStatusSrrfl");
    ((Control) this.checkmarkStatusSrrfl).Name = "checkmarkStatusSrrfl";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrumentRequestResultsStatus, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRequestResultsStatus, "digitalReadoutInstrumentRequestResultsStatus");
    this.digitalReadoutInstrumentRequestResultsStatus.FontGroup = "SRRRInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).FreezeValue = false;
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Initialize((ValueState) 0, 5);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(5, 4.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus");
    ((Control) this.digitalReadoutInstrumentRequestResultsStatus).Name = "digitalReadoutInstrumentRequestResultsStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelButtons, "tableLayoutPanelButtons");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanelButtons, 8);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.sharedProcedureSelectionSrrr, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.sharedProcedureSelectionSrrfr, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.sharedProcedureSelectionSrrfl, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.buttonCalibrateStartStop, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.sharedProcedureSelectionSrrl, 3, 0);
    ((Control) this.tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionSrrr, "sharedProcedureSelectionSrrr");
    ((Control) this.sharedProcedureSelectionSrrr).Name = "sharedProcedureSelectionSrrr";
    this.sharedProcedureSelectionSrrr.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_SRRRCalibration"
    });
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionSrrfr, "sharedProcedureSelectionSrrfr");
    ((Control) this.sharedProcedureSelectionSrrfr).Name = "sharedProcedureSelectionSrrfr";
    this.sharedProcedureSelectionSrrfr.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_SRRFRCalibration"
    });
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionSrrfl, "sharedProcedureSelectionSrrfl");
    ((Control) this.sharedProcedureSelectionSrrfl).Name = "sharedProcedureSelectionSrrfl";
    this.sharedProcedureSelectionSrrfl.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_SRRFLCalibration"
    });
    componentResourceManager.ApplyResources((object) this.buttonCalibrateStartStop, "buttonCalibrateStartStop");
    this.buttonCalibrateStartStop.Name = "buttonCalibrateStartStop";
    this.buttonCalibrateStartStop.UseVisualStyleBackColor = true;
    this.buttonCalibrateStartStop.Click += new EventHandler(this.buttonCalibrateStartStop_Click);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionSrrl, "sharedProcedureSelectionSrrl");
    ((Control) this.sharedProcedureSelectionSrrl).Name = "sharedProcedureSelectionSrrl";
    this.sharedProcedureSelectionSrrl.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_SRRLCalibration"
    });
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.seekTimeListViewLog, 9);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "SRRRCalibration";
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    this.seekTimeListViewLog.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.checkmarkStatusSrrfr, "checkmarkStatusSrrfr");
    ((Control) this.checkmarkStatusSrrfr).Name = "checkmarkStatusSrrfr";
    componentResourceManager.ApplyResources((object) this.checkmarkStatusSrrl, "checkmarkStatusSrrl");
    ((Control) this.checkmarkStatusSrrl).Name = "checkmarkStatusSrrl";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelHeader, "tableLayoutPanelHeader");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanelHeader, 9);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.dialInstrumentVehicleSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.webBrowserMessage, 1, 0);
    ((Control) this.tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
    this.dialInstrumentVehicleSpeed.AngleRange = 180.0;
    this.dialInstrumentVehicleSpeed.AngleStart = 180.0;
    componentResourceManager.ApplyResources((object) this.dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
    this.dialInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState) 3, 2, "km/h");
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(1, 30.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(2, 50.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 45.0, "mph");
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.webBrowserMessage, "webBrowserMessage");
    this.webBrowserMessage.Name = "webBrowserMessage";
    this.webBrowserMessage.Url = new Uri("about: blank", UriKind.Absolute);
    this.sharedProcedureCreatorComponentSrrr.Suspend();
    this.sharedProcedureCreatorComponentSrrr.MonitorCall = new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
    this.sharedProcedureCreatorComponentSrrr.MonitorInterval = 2500;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentSrrr, "sharedProcedureCreatorComponentSrrr");
    this.sharedProcedureCreatorComponentSrrr.Qualifier = "SP_SRRRCalibration";
    this.sharedProcedureCreatorComponentSrrr.StartCall = new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Start");
    dataItemCondition1.Gradient.Initialize((ValueState) 3, 1, "km/h");
    dataItemCondition1.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    this.sharedProcedureCreatorComponentSrrr.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentSrrr.StopCall = new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Stop");
    this.sharedProcedureCreatorComponentSrrr.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StartServiceComplete);
    this.sharedProcedureCreatorComponentSrrr.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StopServiceComplete);
    this.sharedProcedureCreatorComponentSrrr.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentSrrr.Resume();
    this.sharedProcedureIntegrationComponentSrrr.ProceduresDropDown = this.sharedProcedureSelectionSrrr;
    this.sharedProcedureIntegrationComponentSrrr.ProcedureStatusMessageTarget = this.labelStatusSrrr;
    this.sharedProcedureIntegrationComponentSrrr.ProcedureStatusStateTarget = this.checkmarkStatusSrrr;
    this.sharedProcedureIntegrationComponentSrrr.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentSrrr.StartStopButton = this.buttonStartStopSrrr;
    this.sharedProcedureIntegrationComponentSrrr.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentSrrl.Suspend();
    this.sharedProcedureCreatorComponentSrrl.MonitorCall = new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
    this.sharedProcedureCreatorComponentSrrl.MonitorInterval = 2500;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentSrrl, "sharedProcedureCreatorComponentSrrl");
    this.sharedProcedureCreatorComponentSrrl.Qualifier = "SP_SRRLCalibration";
    this.sharedProcedureCreatorComponentSrrl.StartCall = new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Start");
    dataItemCondition2.Gradient.Initialize((ValueState) 3, 1, "km/h");
    dataItemCondition2.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    this.sharedProcedureCreatorComponentSrrl.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentSrrl.StopCall = new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Stop");
    this.sharedProcedureCreatorComponentSrrl.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StartServiceComplete);
    this.sharedProcedureCreatorComponentSrrl.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StopServiceComplete);
    this.sharedProcedureCreatorComponentSrrl.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentSrrl.Resume();
    this.sharedProcedureIntegrationComponentSrrl.ProceduresDropDown = this.sharedProcedureSelectionSrrl;
    this.sharedProcedureIntegrationComponentSrrl.ProcedureStatusMessageTarget = this.labelStatusSrrl;
    this.sharedProcedureIntegrationComponentSrrl.ProcedureStatusStateTarget = this.checkmarkStatusSrrl;
    this.sharedProcedureIntegrationComponentSrrl.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentSrrl.StartStopButton = this.buttonStartStopSrrl;
    this.sharedProcedureIntegrationComponentSrrl.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentSrrfr.Suspend();
    this.sharedProcedureCreatorComponentSrrfr.MonitorCall = new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
    this.sharedProcedureCreatorComponentSrrfr.MonitorInterval = 2500;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentSrrfr, "sharedProcedureCreatorComponentSrrfr");
    this.sharedProcedureCreatorComponentSrrfr.Qualifier = "SP_SRRFRCalibration";
    this.sharedProcedureCreatorComponentSrrfr.StartCall = new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Start");
    dataItemCondition3.Gradient.Initialize((ValueState) 3, 1, "km/h");
    dataItemCondition3.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition3.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    this.sharedProcedureCreatorComponentSrrfr.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentSrrfr.StopCall = new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Stop");
    this.sharedProcedureCreatorComponentSrrfr.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StartServiceComplete);
    this.sharedProcedureCreatorComponentSrrfr.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StopServiceComplete);
    this.sharedProcedureCreatorComponentSrrfr.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentSrrfr.Resume();
    this.sharedProcedureIntegrationComponentSrrfr.ProceduresDropDown = this.sharedProcedureSelectionSrrfr;
    this.sharedProcedureIntegrationComponentSrrfr.ProcedureStatusMessageTarget = this.labelStatusSrrfr;
    this.sharedProcedureIntegrationComponentSrrfr.ProcedureStatusStateTarget = this.checkmarkStatusSrrfr;
    this.sharedProcedureIntegrationComponentSrrfr.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentSrrfr.StartStopButton = this.buttonStartStopSrrfr;
    this.sharedProcedureIntegrationComponentSrrfr.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentSrrfl.Suspend();
    this.sharedProcedureCreatorComponentSrrfl.MonitorCall = new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo");
    this.sharedProcedureCreatorComponentSrrfl.MonitorInterval = 2500;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentSrrfl, "sharedProcedureCreatorComponentSrrfl");
    this.sharedProcedureCreatorComponentSrrfl.Qualifier = "SP_SRRFLCalibration";
    this.sharedProcedureCreatorComponentSrrfl.StartCall = new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Start");
    dataItemCondition4.Gradient.Initialize((ValueState) 3, 1, "km/h");
    dataItemCondition4.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition4.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    this.sharedProcedureCreatorComponentSrrfl.StartConditions.Add(dataItemCondition4);
    this.sharedProcedureCreatorComponentSrrfl.StopCall = new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Stop");
    this.sharedProcedureCreatorComponentSrrfl.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StartServiceComplete);
    this.sharedProcedureCreatorComponentSrrfl.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StopServiceComplete);
    this.sharedProcedureCreatorComponentSrrfl.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentSrrfl.Resume();
    this.sharedProcedureIntegrationComponentSrrfl.ProceduresDropDown = this.sharedProcedureSelectionSrrfl;
    this.sharedProcedureIntegrationComponentSrrfl.ProcedureStatusMessageTarget = this.labelStatusSrrfl;
    this.sharedProcedureIntegrationComponentSrrfl.ProcedureStatusStateTarget = this.checkmarkStatusSrrfl;
    this.sharedProcedureIntegrationComponentSrrfl.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentSrrfl.StartStopButton = this.buttonStartStopSrrfl;
    this.sharedProcedureIntegrationComponentSrrfl.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelButtons).ResumeLayout(false);
    ((Control) this.tableLayoutPanelHeader).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
