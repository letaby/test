using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		None,
		ExecutingDesiredClutchValue,
		RequestingDesiredClutchValueStatus,
		WaitingForDesiredClutchValueStatus,
		RecordSampleClutchPositionStatus,
		StoppingDesiredClutchValue,
		Fault
	}

	private const string DesiredClutchValueStartService = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start";

	private const string DesiredClutchValueRequestService = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status";

	private const string DesiredClutchValueStopService = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop";

	private const string ActualClutchPositionInstrumentQualifier = "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung";

	private const string PressureInstrumentQualifier = "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck";

	private State state = State.None;

	private State nextState = State.None;

	private Channel tcm = null;

	private Channel mcm = null;

	private Instrument actualClutchPositionInstrument = null;

	private Instrument pressureInstrument = null;

	private int intialClutchPosition;

	private int finalClutchPosition;

	private int thresholdLeakPosition = 10;

	private int serviceReturnCode;

	private string faultMessage;

	private Timer waitTimer;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView;

	private DigitalReadoutInstrument digitalReadoutVehicleCheckStatus;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmarkCanStart;

	private Button buttonStart;

	private Label labelCanStart;

	private Button buttonStop;

	private DigitalReadoutInstrument digitalReadoutEngineState;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private bool TestRunning => state != State.None;

	private bool Online => tcm != null && tcm.Online && mcm != null && mcm.Online;

	private bool Busy => Online && mcm.CommunicationsState != CommunicationsState.Online && tcm.CommunicationsState != CommunicationsState.Online;

	private bool CanStart => !TestRunning && Online && EngineStateOk && PressureOk && VehicleCheckStatusOk;

	private bool CanStop => TestRunning;

	private bool EngineStateOk => (int)digitalReadoutEngineState.RepresentedState != 3;

	private bool VehicleCheckStatusOk => (int)digitalReadoutVehicleCheckStatus.RepresentedState == 1;

	private bool PressureOk
	{
		get
		{
			if (pressureInstrument != null && pressureInstrument.InstrumentValues.Current != null)
			{
				try
				{
					double num = Convert.ToDouble(pressureInstrument.InstrumentValues.Current.Value, CultureInfo.InvariantCulture);
					if (num > 620.528)
					{
						return true;
					}
				}
				catch (FormatException)
				{
					return false;
				}
			}
			return false;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		waitTimer = new Timer();
		buttonStart.Click += OnStartButtonClick;
		buttonStop.Click += OnStopButtonClick;
		digitalReadoutVehicleCheckStatus.RepresentedStateChanged += OnPreconditionStateChanged;
		digitalReadoutEngineState.RepresentedStateChanged += OnPreconditionStateChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			digitalReadoutVehicleCheckStatus.RepresentedStateChanged -= OnPreconditionStateChanged;
			digitalReadoutEngineState.RepresentedStateChanged -= OnPreconditionStateChanged;
			SetTCM(null);
			SetMCM(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetTCM(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
		SetMCM(((CustomPanel)this).GetChannel("MCM21T"));
		UpdateUserInterface();
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm != mcm && this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		this.mcm = mcm;
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
		}
	}

	private void SetTCM(Channel tcm)
	{
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Invalid comparison between Unknown and I4
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Invalid comparison between Unknown and I4
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		if (this.tcm != tcm && this.tcm != null)
		{
			this.tcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		if (pressureInstrument != null)
		{
			pressureInstrument.InstrumentUpdateEvent -= OnPressureInstrumentValueChanged;
			pressureInstrument = null;
		}
		this.tcm = tcm;
		if (this.tcm == null)
		{
			return;
		}
		this.tcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
		pressureInstrument = this.tcm.Instruments["DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"];
		actualClutchPositionInstrument = this.tcm.Instruments["DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung"];
		if (pressureInstrument != null)
		{
			pressureInstrument.InstrumentUpdateEvent += OnPressureInstrumentValueChanged;
		}
		Ecu ecu = null;
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			ecu = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecu == null || !(ecu.Identifier == tcm.Ecu.Identifier) || !(ecu.Name != tcm.Ecu.Name))
			{
				continue;
			}
			instrument = val.Instrument;
			QualifierTypes type = ((Qualifier)(ref instrument)).Type;
			string name = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			if (val.DataItem != null)
			{
				continue;
			}
			QualifierTypes val2 = (QualifierTypes)0;
			instrument = val.Instrument;
			if ((int)((Qualifier)(ref instrument)).Type == 1)
			{
				val2 = (QualifierTypes)8;
			}
			else
			{
				instrument = val.Instrument;
				if ((int)((Qualifier)(ref instrument)).Type == 8)
				{
					val2 = (QualifierTypes)1;
				}
			}
			QualifierTypes val3 = val2;
			string name2 = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(val3, name2, ((Qualifier)(ref instrument)).Name);
		}
	}

	private void OnStartButtonClick(object sender, EventArgs e)
	{
		nextState = State.ExecutingDesiredClutchValue;
		Advance();
	}

	private void OnStopButtonClick(object sender, EventArgs e)
	{
		nextState = State.StoppingDesiredClutchValue;
		Advance();
	}

	private void OnIntialClutchSampleRecordTimerTick(object sender, EventArgs e)
	{
		waitTimer.Tick -= OnIntialClutchSampleRecordTimerTick;
		waitTimer.Stop();
		intialClutchPosition = GetActualClutchPosition(actualClutchPositionInstrument);
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_IntialClutchPostionObserved0, intialClutchPosition));
		Advance();
	}

	private void OnFinalClutchSampleRecordTimerTick(object sender, EventArgs e)
	{
		waitTimer.Tick -= OnFinalClutchSampleRecordTimerTick;
		waitTimer.Stop();
		finalClutchPosition = GetActualClutchPosition(actualClutchPositionInstrument);
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_FinalClutchPostionObserved0, finalClutchPosition));
		if (finalClutchPosition - intialClutchPosition > thresholdLeakPosition)
		{
			AddLogLabel(Resources.Message_LeakFoundTestFailed);
		}
		else
		{
			AddLogLabel(Resources.Message_NoLeakFoundTestPassed);
		}
		Advance();
	}

	private void OnRequestClutchValueStatusTick(object sender, EventArgs e)
	{
		waitTimer.Tick -= OnRequestClutchValueStatusTick;
		waitTimer.Stop();
		Advance();
	}

	private void UpdateUserInterface()
	{
		buttonStart.Enabled = CanStart;
		buttonStop.Enabled = CanStop;
		checkmarkCanStart.CheckState = ((CanStart || TestRunning) ? CheckState.Checked : CheckState.Unchecked);
		string text = Resources.Message_TestCanStart;
		if (!buttonStart.Enabled)
		{
			if (TestRunning)
			{
				text = Resources.Message_TestInProgress;
			}
			else if (Busy)
			{
				text = Resources.Message_CannotStartTheTestAsTheDeviceIsBusy;
			}
			else if (!Online)
			{
				text = Resources.Message_CannotStartTheTestAsTheDeviceIsNotOnline;
			}
			else if (!EngineStateOk)
			{
				text = Resources.Message_CannotStartTheTestAsTheEngineIsRunningStopTheEngine;
			}
			else if (!VehicleCheckStatusOk)
			{
				text = Resources.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionIsInNeutral;
			}
			else if (!PressureOk)
			{
				text = Resources.Message_CannotStartTheTestAsAirSupplyPressureIsBeGreaterThan90Psi;
			}
		}
		((Control)(object)labelCanStart).Text = text;
	}

	private void Advance()
	{
		state = nextState;
		switch (state)
		{
		case State.ExecutingDesiredClutchValue:
			AddLogLabel(Resources.Message_StartingClutchApplyLeakTest);
			nextState = State.RequestingDesiredClutchValueStatus;
			AddLogLabel(Resources.Message_RequestingDesiredValueRequirementClutchStartService);
			if (!ExecuteService("RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start", new object[1] { 100 }))
			{
				faultMessage = Resources.Message_ErrorExecutingDesiredValueRequirementClutchStartService;
				nextState = State.Fault;
				Advance();
			}
			break;
		case State.RequestingDesiredClutchValueStatus:
			nextState = State.WaitingForDesiredClutchValueStatus;
			if (!ExecuteService("RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", null))
			{
				faultMessage = Resources.Message_ErrorExecutingDesiredValueRequirementClutchRequestService;
				nextState = State.Fault;
				Advance();
			}
			break;
		case State.WaitingForDesiredClutchValueStatus:
			if (serviceReturnCode == 0)
			{
				waitTimer.Interval = 5000;
				waitTimer.Tick += OnIntialClutchSampleRecordTimerTick;
				waitTimer.Start();
				nextState = State.RecordSampleClutchPositionStatus;
			}
			else if (serviceReturnCode == 1)
			{
				nextState = State.RequestingDesiredClutchValueStatus;
				waitTimer.Interval = 1000;
				waitTimer.Tick += OnRequestClutchValueStatusTick;
				waitTimer.Start();
			}
			else
			{
				nextState = State.StoppingDesiredClutchValue;
				Advance();
			}
			break;
		case State.RecordSampleClutchPositionStatus:
			AddLogLabel(Resources.Message_WaitingToRecordSampleClutchPositionValue);
			waitTimer.Interval = 60000;
			waitTimer.Tick += OnFinalClutchSampleRecordTimerTick;
			waitTimer.Start();
			nextState = State.StoppingDesiredClutchValue;
			break;
		case State.StoppingDesiredClutchValue:
			nextState = State.None;
			AddLogLabel(Resources.Message_RequestingDesiredValueRequirementClutchStopService);
			ExecuteService("RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop", null, ignoreResults: true);
			Advance();
			AddLogLabel(Resources.Message_CompletedClutchApplyLeakTest);
			break;
		case State.Fault:
			AddLogLabel(faultMessage);
			nextState = State.None;
			Advance();
			break;
		default:
			throw new InvalidOperationException("Unknown state");
		case State.None:
			break;
		}
		UpdateUserInterface();
	}

	private bool ExecuteService(string serviceQualifier, object[] args)
	{
		return ExecuteService(serviceQualifier, args, ignoreResults: false);
	}

	private bool ExecuteService(string serviceQualifier, object[] args, bool ignoreResults)
	{
		bool result = false;
		if (tcm != null)
		{
			Service service = tcm.Services[serviceQualifier];
			if (service != null)
			{
				if (args != null)
				{
					for (int i = 0; i < args.Length; i++)
					{
						service.InputValues[i].Value = args[i];
					}
				}
				if (!ignoreResults)
				{
					service.ServiceCompleteEvent += OnServiceCompleteEvent;
				}
				try
				{
					service.Execute(ignoreResults);
					result = true;
				}
				catch (InvalidOperationException)
				{
				}
				catch (ArgumentException)
				{
				}
				catch (CaesarException)
				{
				}
			}
		}
		return result;
	}

	private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnServiceCompleteEvent;
		if (!e.Succeeded)
		{
			nextState = State.Fault;
			if (e.Exception != null)
			{
				faultMessage = e.Exception.Message;
			}
			else
			{
				faultMessage = string.Empty;
			}
		}
		else if (service.OutputValues.Count > 0)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (choice != null)
			{
				serviceReturnCode = Convert.ToInt32(choice.RawValue);
			}
			else
			{
				serviceReturnCode = -1;
			}
		}
		Advance();
	}

	private int GetActualClutchPosition(Instrument instrument)
	{
		int result = 0;
		if (instrument != null && instrument.InstrumentValues.Current != null)
		{
			result = Convert.ToInt32(instrument.InstrumentValues.Current.Value, CultureInfo.InvariantCulture);
		}
		return result;
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnPressureInstrumentValueChanged(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnPreconditionStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
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
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutEngineState = new DigitalReadoutInstrument();
		digitalReadoutVehicleCheckStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmarkCanStart = new Checkmark();
		buttonStart = new Button();
		labelCanStart = new Label();
		buttonStop = new Button();
		seekTimeListView = new SeekTimeListView();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutEngineState, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutVehicleCheckStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutEngineState, "digitalReadoutEngineState");
		digitalReadoutEngineState.FontGroup = "InstrumentValue";
		((SingleInstrumentBase)digitalReadoutEngineState).FreezeValue = false;
		digitalReadoutEngineState.Gradient.Initialize((ValueState)0, 8);
		digitalReadoutEngineState.Gradient.Modify(1, -1.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(2, 0.0, (ValueState)1);
		digitalReadoutEngineState.Gradient.Modify(3, 1.0, (ValueState)0);
		digitalReadoutEngineState.Gradient.Modify(4, 2.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(5, 3.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(6, 4.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(7, 5.0, (ValueState)3);
		digitalReadoutEngineState.Gradient.Modify(8, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutEngineState).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutEngineState).Name = "digitalReadoutEngineState";
		((SingleInstrumentBase)digitalReadoutEngineState).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutVehicleCheckStatus, "digitalReadoutVehicleCheckStatus");
		digitalReadoutVehicleCheckStatus.FontGroup = "InstrumentValue";
		((SingleInstrumentBase)digitalReadoutVehicleCheckStatus).FreezeValue = false;
		digitalReadoutVehicleCheckStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutVehicleCheckStatus).Name = "digitalReadoutVehicleCheckStatus";
		((SingleInstrumentBase)digitalReadoutVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "InstrumentValue";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)3, 1, "psi");
		digitalReadoutInstrument2.Gradient.Modify(1, 90.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "InstrumentValue";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmarkCanStart, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStart, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelCanStart, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStop, 3, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		labelCanStart.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelCanStart, "labelCanStart");
		((Control)(object)labelCanStart).Name = "labelCanStart";
		labelCanStart.Orientation = (TextOrientation)1;
		labelCanStart.ShowBorder = false;
		labelCanStart.UseSystemColors = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "TCM Clutch Apply Leak Test";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss:fff";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_ClutchApplyLeakTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
