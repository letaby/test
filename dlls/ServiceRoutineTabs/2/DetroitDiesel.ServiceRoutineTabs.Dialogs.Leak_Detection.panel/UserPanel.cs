using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Settings;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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

	private Label label1;

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

	private bool Online => channel != null && channel.CommunicationsState == CommunicationsState.Online;

	private bool CanResetLearnedData => LicenseManager.GlobalInstance.AccessLevel > 1 && Online;

	private bool CanRunDSService
	{
		get
		{
			if (Online && parameterLeakLearnedCounter != null && instrumentFuelTemperature != null && instrumentCoolantTemperature != null && channel.Services["RT_SR076_Desired_Rail_Pressure_Start_Status"] != null && channel.Services["RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start"] != null && parameterLeakLearnedCounter.OriginalValue != null && instrumentFuelTemperature.InstrumentValues.Current != null && instrumentCoolantTemperature.InstrumentValues.Current != null && instrumentFuelTemperature.InstrumentValues.Current.Value != null && instrumentCoolantTemperature.InstrumentValues.Current.Value != null && ObjectToDouble(parameterLeakLearnedCounter.OriginalValue, string.Empty) >= 10.0)
			{
				Conversion conversion = Converter.GlobalInstance.GetConversion(instrumentCoolantTemperature.Units, "degC");
				Conversion conversion2 = Converter.GlobalInstance.GetConversion(instrumentFuelTemperature.Units, "degC");
				if (conversion != null && conversion2 != null && 50.0 < conversion.Convert(instrumentCoolantTemperature.InstrumentValues.Current.Value) && 10.0 < conversion2.Convert(instrumentFuelTemperature.InstrumentValues.Current.Value))
				{
					return true;
				}
			}
			return false;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		buttonDSService.Click += OnDSServiceClick;
		buttonResetLearntData.Click += OnResetLearntDataClick;
		buttonResetErrorCounter.Click += OnResetErrorCounterClick;
		readAccumulatorsButton.Click += OnReadAccumulatorsClick;
		startTimerButton.Click += OnStartTimerClick;
		stopTimerButton.Click += OnStopTimerClick;
		timer.Tick += OnTimer;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			SetChannel(null);
			if (timerRunning)
			{
				StopTimer();
			}
		}
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("MCM"));
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel == channel)
		{
			return;
		}
		if (this.channel != null)
		{
			if (parameterLeakLearnedCounter != null)
			{
				parameterLeakLearnedCounter.ParameterUpdateEvent -= OnLeakLearnedCounterUpdateEvent;
				parameterLeakLearnedCounter = null;
			}
			if (instrumentCoolantTemperature != null)
			{
				instrumentCoolantTemperature.InstrumentUpdateEvent -= OnCoolantTemperatureUpdateEvent;
				instrumentCoolantTemperature = null;
			}
			if (instrumentFuelTemperature != null)
			{
				instrumentFuelTemperature.InstrumentUpdateEvent -= OnFuelTemperatureUpdateEvent;
				instrumentFuelTemperature = null;
			}
			this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		this.channel = channel;
		if (this.channel != null)
		{
			parameterLeakLearnedCounter = this.channel.Parameters["HP_Leak_Learned_Counter"];
			if (parameterLeakLearnedCounter != null)
			{
				parameterLeakLearnedCounter.ParameterUpdateEvent += OnLeakLearnedCounterUpdateEvent;
			}
			instrumentCoolantTemperature = this.channel.Instruments["DT_AS067_Coolant_Temperatures_2"];
			if (instrumentCoolantTemperature != null)
			{
				instrumentCoolantTemperature.InstrumentUpdateEvent += OnCoolantTemperatureUpdateEvent;
			}
			instrumentFuelTemperature = this.channel.Instruments["DT_AS014_Fuel_Temperature"];
			if (instrumentFuelTemperature != null)
			{
				instrumentFuelTemperature.InstrumentUpdateEvent += OnFuelTemperatureUpdateEvent;
			}
			this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			ReadParameters();
		}
	}

	private void OnCoolantTemperatureUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateButtonState();
	}

	private void OnFuelTemperatureUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateButtonState();
	}

	private void OnLeakLearnedCounterUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateButtonState();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		bool online = Online;
		buttonDSService.Enabled = CanRunDSService;
		buttonResetLearntData.Enabled = CanResetLearnedData;
		buttonResetErrorCounter.Enabled = online;
		readAccumulatorsButton.Enabled = online;
		startTimerButton.Enabled = online && !timerRunning;
		stopTimerButton.Enabled = online && timerRunning;
	}

	private static double ObjectToDouble(object value, string units)
	{
		double num = double.NaN;
		if (value != null)
		{
			Choice choice = value as Choice;
			if (choice != null)
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
					{
						num = conversion.Convert(num);
					}
				}
				catch (InvalidCastException)
				{
					num = double.NaN;
				}
				catch (FormatException)
				{
					num = double.NaN;
				}
			}
		}
		return num;
	}

	private void OnDSServiceClick(object sender, EventArgs e)
	{
		if (CanRunDSService)
		{
			channel.Services.ServiceCompleteEvent += OnServiceComplete;
			channel.Services.Execute("RT_SR076_Desired_Rail_Pressure_Start_Status(870);RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start()", synchronous: false);
		}
	}

	private void ReadParameters()
	{
		ReadParameter(channel.Parameters["HP_Leak_Counter"]);
		ReadParameter(channel.Parameters["HP_Leak_Learned_Value"]);
		ReadParameter(channel.Parameters["HP_Leak_Learned_Counter"]);
	}

	private void ReadParameter(Parameter parameter)
	{
		if (parameter != null && parameter.Channel.Online)
		{
			string groupQualifier = parameter.GroupQualifier;
			parameter.Channel.Parameters.ReadGroup(groupQualifier, fromCache: false, synchronous: false);
		}
	}

	private void OnResetLearntDataClick(object sender, EventArgs e)
	{
		if (CanResetLearnedData && DialogResult.Yes == MessageBox.Show(Resources.Message_AreYouSureYouWantToResetTheLearnedData, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
		{
			Service service = channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
			if (service != null)
			{
				service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue((byte)24);
				service.ServiceCompleteEvent += OnServiceComplete;
				service.Execute(synchronous: false);
			}
		}
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		if (channel != null)
		{
			Service service = sender as Service;
			if (service != null)
			{
				service.ServiceCompleteEvent -= OnServiceComplete;
			}
			else
			{
				channel.Services.ServiceCompleteEvent -= OnServiceComplete;
			}
		}
		ReadParameters();
	}

	private void OnResetErrorCounterClick(object sender, EventArgs e)
	{
		if (channel != null && DialogResult.Yes == MessageBox.Show(Resources.Message_AreYouSure, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
		{
			Parameter parameter = channel.Parameters["HP_Leak_Counter"];
			parameter.Value = 4;
			parameter.Channel.Parameters.Write(synchronous: false);
		}
	}

	private void OnReadAccumulatorsClick(object sender, EventArgs e)
	{
		ReadParameters();
	}

	private void OnStartTimerClick(object sender, EventArgs e)
	{
		StartTimer();
	}

	private void OnStopTimerClick(object sender, EventArgs e)
	{
		scalingLabel1.RepresentedState = (ValueState)3;
		timer.Stop();
		StopTimer();
	}

	private void StartTimer()
	{
		timerRunning = true;
		startIdleTime = DateTime.Now;
		timer.Interval = 100;
		timer.Start();
		UpdateButtonState();
	}

	private void StopTimer()
	{
		timerRunning = false;
		timer.Stop();
		UpdateButtonState();
	}

	private void OnTimer(object sender, EventArgs e)
	{
		TimeSpan timeSpan = DateTime.Now - startIdleTime;
		((Control)(object)scalingLabel1).Text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds / 100:D1}";
		if (timeSpan > new TimeSpan(0, 10, 0))
		{
			scalingLabel1.RepresentedState = (ValueState)3;
		}
		else if (timeSpan > new TimeSpan(0, 5, 0))
		{
			scalingLabel1.RepresentedState = (ValueState)1;
		}
		else
		{
			scalingLabel1.RepresentedState = (ValueState)2;
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel3 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		label1 = new Label();
		scalingLabel1 = new ScalingLabel();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		buttonResetLearntData = new Button();
		buttonResetErrorCounter = new Button();
		buttonClose = new Button();
		readAccumulatorsButton = new Button();
		startTimerButton = new Button();
		stopTimerButton = new Button();
		buttonDSService = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 4, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 0, 6);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)4, "MCM", "HP_Leak_Counter");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument1, 5);
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)4, "MCM", "HP_Leak_Learned_Value");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument2, 5);
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument3, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)4, "MCM", "HP_Leak_Learned_Counter");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument3, 5);
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument4, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS114_RPG_COMPENSATION");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument4, 5);
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument5, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS115_HP_Leak_Actual_Value");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument5, 5);
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)scalingLabel1, 0, 1);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel3, 2);
		label1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)(object)label1).Name = "label1";
		label1.Orientation = (TextOrientation)1;
		scalingLabel1.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel1, "scalingLabel1");
		scalingLabel1.FontGroup = null;
		scalingLabel1.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel1).Name = "scalingLabel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument8, 2);
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		digitalReadoutInstrument6.Gradient.Initialize((ValueState)0, 3, "°C");
		digitalReadoutInstrument6.Gradient.Modify(1, 50.0, (ValueState)2);
		digitalReadoutInstrument6.Gradient.Modify(2, 70.0, (ValueState)1);
		digitalReadoutInstrument6.Gradient.Modify(3, 100.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS067_Coolant_Temperatures_2");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument6, 2);
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		digitalReadoutInstrument7.Gradient.Initialize((ValueState)0, 3, "°C");
		digitalReadoutInstrument7.Gradient.Modify(1, 10.0, (ValueState)2);
		digitalReadoutInstrument7.Gradient.Modify(2, 35.0, (ValueState)1);
		digitalReadoutInstrument7.Gradient.Modify(3, 89.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS014_Fuel_Temperature");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument7, 2);
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		digitalReadoutInstrument9.Gradient.Initialize((ValueState)0, 7);
		digitalReadoutInstrument9.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(4, 3.0, (ValueState)2);
		digitalReadoutInstrument9.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(6, 5.0, (ValueState)0);
		digitalReadoutInstrument9.Gradient.Modify(7, 6.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrument9, 2);
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonResetLearntData, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonResetErrorCounter, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 7, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(readAccumulatorsButton, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(startTimerButton, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(stopTimerButton, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonDSService, 3, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonResetLearntData, "buttonResetLearntData");
		buttonResetLearntData.Name = "buttonResetLearntData";
		buttonResetLearntData.UseCompatibleTextRendering = true;
		buttonResetLearntData.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonResetErrorCounter, "buttonResetErrorCounter");
		buttonResetErrorCounter.Name = "buttonResetErrorCounter";
		buttonResetErrorCounter.UseCompatibleTextRendering = true;
		buttonResetErrorCounter.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(readAccumulatorsButton, "readAccumulatorsButton");
		readAccumulatorsButton.Name = "readAccumulatorsButton";
		readAccumulatorsButton.UseCompatibleTextRendering = true;
		readAccumulatorsButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(startTimerButton, "startTimerButton");
		startTimerButton.Name = "startTimerButton";
		startTimerButton.UseCompatibleTextRendering = true;
		startTimerButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(stopTimerButton, "stopTimerButton");
		stopTimerButton.Name = "stopTimerButton";
		stopTimerButton.UseCompatibleTextRendering = true;
		stopTimerButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonDSService, "buttonDSService");
		buttonDSService.Name = "buttonDSService";
		buttonDSService.UseCompatibleTextRendering = true;
		buttonDSService.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_LeakDetection");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
