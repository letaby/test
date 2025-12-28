using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__NGC_.panel;

public class UserPanel : CustomPanel
{
	private const string RadarEcuName = "RDF02T";

	private const string verticalPosition = "VertPos";

	private SharedProcedureBase selectedProcedure;

	private WarningManager warningManager;

	private bool closing = false;

	private Channel rdf02tChannel;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private System.Windows.Forms.Label labelStatus;

	private Button buttonStartStop;

	private TableLayoutPanel tableLayoutPanel1;

	private BarInstrument barInstrumentProcedureProgress;

	private SharedProcedureSelection sharedProcedureSelection1;

	private DialInstrument dialInstrumentVehicleSpeed;

	private SeekTimeListView seekTimeListView1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVertPos;

	private Button buttonHiddenStart;

	private Checkmark checkmark1;

	private Channel Rdf02tChannel
	{
		get
		{
			return rdf02tChannel;
		}
		set
		{
			if (rdf02tChannel != value)
			{
				warningManager.Reset();
				if (rdf02tChannel != null)
				{
					rdf02tChannel.Parameters.ParametersReadCompleteEvent -= Rdf02t_ParametersReadCompleteEvent;
				}
				rdf02tChannel = value;
				if (rdf02tChannel != null)
				{
					rdf02tChannel.Parameters.ParametersReadCompleteEvent += Rdf02t_ParametersReadCompleteEvent;
				}
			}
		}
	}

	private bool CanClose => !sharedProcedureSelection1.AnyProcedureInProgress;

	public UserPanel()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.WarningManagerJobName, seekTimeListView1.RequiredUserLabelPrefix);
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		SubscribeToEvents(sharedProcedureSelection1.SelectedProcedure);
		((CustomPanel)this).OnChannelsChanged();
		ReadParameters();
	}

	public override void OnChannelsChanged()
	{
		if (!closing)
		{
			Rdf02tChannel = ((CustomPanel)this).GetChannel("RDF02T");
		}
		((CustomPanel)this).OnChannelsChanged();
	}

	private void ReadParameters()
	{
		if (Rdf02tChannel != null && Rdf02tChannel.CommunicationsState == CommunicationsState.Online && Rdf02tChannel.Parameters["VertPos"] != null && !Rdf02tChannel.Parameters["VertPos"].HasBeenReadFromEcu)
		{
			Rdf02tChannel.Parameters.ReadGroup(Rdf02tChannel.Parameters["VertPos"].GroupQualifier, fromCache: true, synchronous: false);
		}
	}

	private void Rdf02t_ParametersReadCompleteEvent(object sender, ResultEventArgs result)
	{
		ReadParameters();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			closing = true;
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		}
	}

	private void SubscribeToEvents(SharedProcedureBase procedure)
	{
		if (procedure != selectedProcedure)
		{
			if (selectedProcedure != null)
			{
				selectedProcedure.StartComplete -= OnProcedureStart;
				selectedProcedure.StopComplete -= OnProcedureStop;
			}
			selectedProcedure = procedure;
			if (selectedProcedure != null)
			{
				selectedProcedure.StartComplete += OnProcedureStart;
				selectedProcedure.StopComplete += OnProcedureStop;
			}
		}
	}

	private void OnProcedureStart(object sender, PassFailResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState)3);
			((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState)1);
			((Control)(object)dialInstrumentVehicleSpeed).Invalidate();
		}
	}

	private void OnProcedureStop(object sender, PassFailResultEventArgs e)
	{
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState)3);
		((Control)(object)dialInstrumentVehicleSpeed).Invalidate();
	}

	private void buttonStartStop_Click(object sender, EventArgs e)
	{
		if (sharedProcedureSelection1.SelectedProcedure.CanStart)
		{
			if (warningManager.RequestContinue())
			{
				sharedProcedureIntegrationComponent1.StartStopButton.PerformClick();
			}
		}
		else
		{
			sharedProcedureIntegrationComponent1.StartStopButton.PerformClick();
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Cancelled);
		}
	}

	private void buttonHiddenStart_EnabledChanged(object sender, EventArgs e)
	{
		buttonStartStop.Enabled = buttonHiddenStart.Enabled;
	}

	private void buttonHiddenStart_TextChanged(object sender, EventArgs e)
	{
		buttonStartStop.Text = buttonHiddenStart.Text;
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Expected O, but got Unknown
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		dialInstrumentVehicleSpeed = new DialInstrument();
		labelStatus = new System.Windows.Forms.Label();
		checkmark1 = new Checkmark();
		barInstrumentProcedureProgress = new BarInstrument();
		buttonStartStop = new Button();
		seekTimeListView1 = new SeekTimeListView();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVertPos = new DigitalReadoutInstrument();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		buttonHiddenStart = new Button();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrumentVehicleSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentProcedureProgress, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStartStop, 5, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVertPos, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 4, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonHiddenStart, 3, 4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		dialInstrumentVehicleSpeed.AngleRange = 270.0;
		dialInstrumentVehicleSpeed.AngleStart = 135.0;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)dialInstrumentVehicleSpeed, 2);
		componentResourceManager.ApplyResources(dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
		dialInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState)0, 3, "km/h");
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(1, -1.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(2, 1.0, (ValueState)3);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(3, 40.0, (ValueState)3);
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 90.0, "mph");
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatus, 2);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrumentProcedureProgress, 2);
		componentResourceManager.ApplyResources(barInstrumentProcedureProgress, "barInstrumentProcedureProgress");
		barInstrumentProcedureProgress.FontGroup = null;
		((SingleInstrumentBase)barInstrumentProcedureProgress).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.Initialize((ValueState)0, 2, "%");
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.Modify(2, 101.0, (ValueState)0);
		((SingleInstrumentBase)barInstrumentProcedureProgress).Instrument = new Qualifier((QualifierTypes)1, "RDF02T", "DT_Service_Justage_Progress_service_justage_progress");
		((Control)(object)barInstrumentProcedureProgress).Name = "barInstrumentProcedureProgress";
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
		((SingleInstrumentBase)barInstrumentProcedureProgress).ShowValueReadout = false;
		((SingleInstrumentBase)barInstrumentProcedureProgress).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseCompatibleTextRendering = true;
		buttonStartStop.UseVisualStyleBackColor = true;
		buttonStartStop.Click += buttonStartStop_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 4);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "Detroit Assurance Driving Radar Alignment NGC";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView1, 4);
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss.f";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentVertPos, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVertPos, "digitalReadoutInstrumentVertPos");
		digitalReadoutInstrumentVertPos.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVertPos).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentVertPos).Instrument = new Qualifier((QualifierTypes)4, "RDF02T", "VertPos");
		((Control)(object)digitalReadoutInstrumentVertPos).Name = "digitalReadoutInstrumentVertPos";
		((SingleInstrumentBase)digitalReadoutInstrumentVertPos).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DrivingRadarAlignment_NGC" });
		componentResourceManager.ApplyResources(buttonHiddenStart, "buttonHiddenStart");
		buttonHiddenStart.Name = "buttonHiddenStart";
		buttonHiddenStart.UseCompatibleTextRendering = true;
		buttonHiddenStart.UseVisualStyleBackColor = true;
		buttonHiddenStart.EnabledChanged += buttonHiddenStart_EnabledChanged;
		buttonHiddenStart.TextChanged += buttonHiddenStart_TextChanged;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonHiddenStart;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
