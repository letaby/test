// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Leak_Detection.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Settings;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Leak_Detection.panel;

public class UserPanel : CustomPanel
{
  private const string SR076Start = "RT_SR076_Desired_Rail_Pressure_Start_Status";
  private const string SR07BStart = "RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start";
  private const string DSServiceCall = "RT_SR076_Desired_Rail_Pressure_Start_Status(870);RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start()";
  private const string ParameterLeakCounter = "HP_Leak_Counter";
  private const string ParameterLeakLearnedValue = "HP_Leak_Learned_Value";
  private const string ParameterLeakLearnedCounter = "HP_Leak_Learned_Counter";
  private const string InstrumentCoolantTemperature = "DT_AS067_Coolant_Temperatures_2";
  private const string InstrumentFuelTemperature = "DT_AS014_Fuel_Temperature";
  private const string setEolDefaultService = "RT_SR014_SET_EOL_Default_Values_Start";
  private const byte resetRpgLeak = 24;
  private const int idleSpeed = 1000;
  private Channel channel;
  private Timer timer = new Timer();
  private DateTime startIdleTime;
  private bool timerRunning;
  private Parameter parameterLeakLearnedCounter;
  private Instrument instrumentCoolantTemperature;
  private Instrument instrumentFuelTemperature;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private TableLayoutPanel tableLayoutPanel3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label1;
  private ScalingLabel scalingLabel1;
  private DigitalReadoutInstrument digitalReadoutInstrument8;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument7;
  private DigitalReadoutInstrument digitalReadoutInstrument9;
  private Button buttonDSService;
  private Button buttonResetLearntData;
  private Button buttonResetErrorCounter;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonClose;
  private Button readAccumulatorsButton;
  private Button startTimerButton;
  private Button stopTimerButton;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonDSService.Click += new EventHandler(this.OnDSServiceClick);
    this.buttonResetLearntData.Click += new EventHandler(this.OnResetLearntDataClick);
    this.buttonResetErrorCounter.Click += new EventHandler(this.OnResetErrorCounterClick);
    this.readAccumulatorsButton.Click += new EventHandler(this.OnReadAccumulatorsClick);
    this.startTimerButton.Click += new EventHandler(this.OnStartTimerClick);
    this.stopTimerButton.Click += new EventHandler(this.OnStopTimerClick);
    this.timer.Tick += new EventHandler(this.OnTimer);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.SetChannel((Channel) null);
    if (this.timerRunning)
      this.StopTimer();
  }

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetChannel("MCM"));

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
    {
      if (this.parameterLeakLearnedCounter != null)
      {
        this.parameterLeakLearnedCounter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.OnLeakLearnedCounterUpdateEvent);
        this.parameterLeakLearnedCounter = (Parameter) null;
      }
      if (this.instrumentCoolantTemperature != (Instrument) null)
      {
        this.instrumentCoolantTemperature.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnCoolantTemperatureUpdateEvent);
        this.instrumentCoolantTemperature = (Instrument) null;
      }
      if (this.instrumentFuelTemperature != (Instrument) null)
      {
        this.instrumentFuelTemperature.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnFuelTemperatureUpdateEvent);
        this.instrumentFuelTemperature = (Instrument) null;
      }
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    this.channel = channel;
    if (this.channel != null)
    {
      this.parameterLeakLearnedCounter = this.channel.Parameters["HP_Leak_Learned_Counter"];
      if (this.parameterLeakLearnedCounter != null)
        this.parameterLeakLearnedCounter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.OnLeakLearnedCounterUpdateEvent);
      this.instrumentCoolantTemperature = this.channel.Instruments["DT_AS067_Coolant_Temperatures_2"];
      if (this.instrumentCoolantTemperature != (Instrument) null)
        this.instrumentCoolantTemperature.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnCoolantTemperatureUpdateEvent);
      this.instrumentFuelTemperature = this.channel.Instruments["DT_AS014_Fuel_Temperature"];
      if (this.instrumentFuelTemperature != (Instrument) null)
        this.instrumentFuelTemperature.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnFuelTemperatureUpdateEvent);
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.ReadParameters();
    }
  }

  private void OnCoolantTemperatureUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateButtonState();
  }

  private void OnFuelTemperatureUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateButtonState();
  }

  private void OnLeakLearnedCounterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateButtonState();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateButtonState();
  }

  private void UpdateButtonState()
  {
    bool online = this.Online;
    this.buttonDSService.Enabled = this.CanRunDSService;
    this.buttonResetLearntData.Enabled = this.CanResetLearnedData;
    this.buttonResetErrorCounter.Enabled = online;
    this.readAccumulatorsButton.Enabled = online;
    this.startTimerButton.Enabled = online && !this.timerRunning;
    this.stopTimerButton.Enabled = online && this.timerRunning;
  }

  private bool Online
  {
    get => this.channel != null && this.channel.CommunicationsState == CommunicationsState.Online;
  }

  private bool CanResetLearnedData => LicenseManager.GlobalInstance.AccessLevel > 1 && this.Online;

  private bool CanRunDSService
  {
    get
    {
      if (this.Online && this.parameterLeakLearnedCounter != null && this.instrumentFuelTemperature != (Instrument) null && this.instrumentCoolantTemperature != (Instrument) null && this.channel.Services["RT_SR076_Desired_Rail_Pressure_Start_Status"] != (Service) null && this.channel.Services["RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start"] != (Service) null && this.parameterLeakLearnedCounter.OriginalValue != null && this.instrumentFuelTemperature.InstrumentValues.Current != null && this.instrumentCoolantTemperature.InstrumentValues.Current != null && this.instrumentFuelTemperature.InstrumentValues.Current.Value != null && this.instrumentCoolantTemperature.InstrumentValues.Current.Value != null && UserPanel.ObjectToDouble(this.parameterLeakLearnedCounter.OriginalValue, string.Empty) >= 10.0)
      {
        Conversion conversion1 = Converter.GlobalInstance.GetConversion(this.instrumentCoolantTemperature.Units, "degC");
        Conversion conversion2 = Converter.GlobalInstance.GetConversion(this.instrumentFuelTemperature.Units, "degC");
        if (conversion1 != null && conversion2 != null && 50.0 < conversion1.Convert(this.instrumentCoolantTemperature.InstrumentValues.Current.Value) && 10.0 < conversion2.Convert(this.instrumentFuelTemperature.InstrumentValues.Current.Value))
          return true;
      }
      return false;
    }
  }

  private static double ObjectToDouble(object value, string units)
  {
    double num = double.NaN;
    if (value != null)
    {
      Choice choice = value as Choice;
      if (choice != (object) null)
      {
        num = Convert.ToDouble(choice.RawValue);
      }
      else
      {
        try
        {
          num = Convert.ToDouble(value);
          Conversion conversion = Converter.GlobalInstance.GetConversion(units);
          if (conversion != null)
            num = conversion.Convert(num);
        }
        catch (InvalidCastException ex)
        {
          num = double.NaN;
        }
        catch (FormatException ex)
        {
          num = double.NaN;
        }
      }
    }
    return num;
  }

  private void OnDSServiceClick(object sender, EventArgs e)
  {
    if (!this.CanRunDSService)
      return;
    this.channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.channel.Services.Execute("RT_SR076_Desired_Rail_Pressure_Start_Status(870);RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start()", false);
  }

  private void ReadParameters()
  {
    this.ReadParameter(this.channel.Parameters["HP_Leak_Counter"]);
    this.ReadParameter(this.channel.Parameters["HP_Leak_Learned_Value"]);
    this.ReadParameter(this.channel.Parameters["HP_Leak_Learned_Counter"]);
  }

  private void ReadParameter(Parameter parameter)
  {
    if (parameter == null || !parameter.Channel.Online)
      return;
    string groupQualifier = parameter.GroupQualifier;
    parameter.Channel.Parameters.ReadGroup(groupQualifier, false, false);
  }

  private void OnResetLearntDataClick(object sender, EventArgs e)
  {
    if (!this.CanResetLearnedData || DialogResult.Yes != MessageBox.Show(Resources.Message_AreYouSureYouWantToResetTheLearnedData, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
      return;
    Service service = this.channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) (byte) 24);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      service.Execute(false);
    }
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    if (this.channel != null)
    {
      Service service = sender as Service;
      if (service != (Service) null)
        service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
      else
        this.channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    }
    this.ReadParameters();
  }

  private void OnResetErrorCounterClick(object sender, EventArgs e)
  {
    if (this.channel == null || DialogResult.Yes != MessageBox.Show(Resources.Message_AreYouSure, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
      return;
    Parameter parameter = this.channel.Parameters["HP_Leak_Counter"];
    parameter.Value = (object) 4;
    parameter.Channel.Parameters.Write(false);
  }

  private void OnReadAccumulatorsClick(object sender, EventArgs e) => this.ReadParameters();

  private void OnStartTimerClick(object sender, EventArgs e) => this.StartTimer();

  private void OnStopTimerClick(object sender, EventArgs e)
  {
    this.scalingLabel1.RepresentedState = (ValueState) 3;
    this.timer.Stop();
    this.StopTimer();
  }

  private void StartTimer()
  {
    this.timerRunning = true;
    this.startIdleTime = DateTime.Now;
    this.timer.Interval = 100;
    this.timer.Start();
    this.UpdateButtonState();
  }

  private void StopTimer()
  {
    this.timerRunning = false;
    this.timer.Stop();
    this.UpdateButtonState();
  }

  private void OnTimer(object sender, EventArgs e)
  {
    TimeSpan timeSpan = DateTime.Now - this.startIdleTime;
    ((Control) this.scalingLabel1).Text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds / 100:D1}";
    if (timeSpan > new TimeSpan(0, 10, 0))
      this.scalingLabel1.RepresentedState = (ValueState) 3;
    else if (timeSpan > new TimeSpan(0, 5, 0))
      this.scalingLabel1.RepresentedState = (ValueState) 1;
    else
      this.scalingLabel1.RepresentedState = (ValueState) 2;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.label1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabel1 = new ScalingLabel();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument9 = new DigitalReadoutInstrument();
    this.buttonResetLearntData = new Button();
    this.buttonResetErrorCounter = new Button();
    this.buttonClose = new Button();
    this.readAccumulatorsButton = new Button();
    this.startTimerButton = new Button();
    this.stopTimerButton = new Button();
    this.buttonDSService = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 4, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument8, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument7, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument9, 0, 6);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 4, "MCM", "HP_Leak_Counter");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument1, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 4, "MCM", "HP_Leak_Learned_Value");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument2, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 4, "MCM", "HP_Leak_Learned_Counter");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument3, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument4, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS114_RPG_COMPENSATION");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument4, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument5, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS115_HP_Leak_Actual_Value");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument5, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.scalingLabel1, 0, 1);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel3, 2);
    this.label1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((Control) this.label1).Name = "label1";
    this.label1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabel1.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel1, "scalingLabel1");
    this.scalingLabel1.FontGroup = (string) null;
    this.scalingLabel1.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel1).Name = "scalingLabel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS010_Engine_Speed");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument8, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    this.digitalReadoutInstrument6.Gradient.Initialize((ValueState) 0, 3, "°C");
    this.digitalReadoutInstrument6.Gradient.Modify(1, 50.0, (ValueState) 2);
    this.digitalReadoutInstrument6.Gradient.Modify(2, 70.0, (ValueState) 1);
    this.digitalReadoutInstrument6.Gradient.Modify(3, 100.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS067_Coolant_Temperatures_2");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument6, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    this.digitalReadoutInstrument7.Gradient.Initialize((ValueState) 0, 3, "°C");
    this.digitalReadoutInstrument7.Gradient.Modify(1, 10.0, (ValueState) 2);
    this.digitalReadoutInstrument7.Gradient.Modify(2, 35.0, (ValueState) 1);
    this.digitalReadoutInstrument7.Gradient.Modify(3, 89.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS014_Fuel_Temperature");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument7, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument9, "digitalReadoutInstrument9");
    this.digitalReadoutInstrument9.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).FreezeValue = false;
    this.digitalReadoutInstrument9.Gradient.Initialize((ValueState) 0, 7);
    this.digitalReadoutInstrument9.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrument9.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrument9.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument9.Gradient.Modify(4, 3.0, (ValueState) 2);
    this.digitalReadoutInstrument9.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrument9.Gradient.Modify(6, 5.0, (ValueState) 0);
    this.digitalReadoutInstrument9.Gradient.Modify(7, 6.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrument9, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonResetLearntData, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonResetErrorCounter, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 7, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.readAccumulatorsButton, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.startTimerButton, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.stopTimerButton, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonDSService, 3, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonResetLearntData, "buttonResetLearntData");
    this.buttonResetLearntData.Name = "buttonResetLearntData";
    this.buttonResetLearntData.UseCompatibleTextRendering = true;
    this.buttonResetLearntData.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonResetErrorCounter, "buttonResetErrorCounter");
    this.buttonResetErrorCounter.Name = "buttonResetErrorCounter";
    this.buttonResetErrorCounter.UseCompatibleTextRendering = true;
    this.buttonResetErrorCounter.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.readAccumulatorsButton, "readAccumulatorsButton");
    this.readAccumulatorsButton.Name = "readAccumulatorsButton";
    this.readAccumulatorsButton.UseCompatibleTextRendering = true;
    this.readAccumulatorsButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.startTimerButton, "startTimerButton");
    this.startTimerButton.Name = "startTimerButton";
    this.startTimerButton.UseCompatibleTextRendering = true;
    this.startTimerButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.stopTimerButton, "stopTimerButton");
    this.stopTimerButton.Name = "stopTimerButton";
    this.stopTimerButton.UseCompatibleTextRendering = true;
    this.stopTimerButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonDSService, "buttonDSService");
    this.buttonDSService.Name = "buttonDSService";
    this.buttonDSService.UseCompatibleTextRendering = true;
    this.buttonDSService.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_LeakDetection");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
