using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__Euro4_.panel;

public class UserPanel : CustomPanel
{
	private const string MCMName = "MCM";

	private const string InstrumentTotalTimeEngineOnQualifier = "DT_ASL007_Engine_Operating_Hours";

	private const string InstrumentFuelTotalTimeWIFLightActiveQualifier = "DT_ASL007_Engine_Operating_Hours";

	private const string E2pLWaterRaisedEngHours = "e2p_l_water_raised_eng_hours";

	private const string E2pLWaterRaisedEngStarts = "e2p_l_water_raised_eng_starts";

	private Channel channel;

	private Qualifier[] ambientQualifiers;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;

	private Button buttonResetValues;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private TableLayoutPanel tableLayoutPanelHeader;

	private DigitalReadoutInstrument digitalReadoutInstrumentEnigneLoad;

	private ChartInstrument chartInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelDensity;

	private DigitalReadoutInstrument digitalReadoutIgnitionCycle;

	private DigitalReadoutInstrument digitalReadoutEngineHours;

	private TextBox textBoxOutput;

	private DigitalReadoutInstrument digitalReadoutWaterLifetimeCountInstrument;

	private DigitalReadoutInstrument digitalReadoutWaterLifetimeCountParameter;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

	private bool Online => channel != null && channel.CommunicationsState == CommunicationsState.Online;

	private bool RunningLogFile => channel != null && channel.LogFile != null;

	public UserPanel()
	{
		InitializeComponent();
		InitQualifiers();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		buttonResetValues.Click += OnButtonResetValuesClick;
	}

	private void InitQualifiers()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		ambientQualifiers = (Qualifier[])(object)new Qualifier[5]
		{
			new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
			new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_AS087_Actual_Fuel_Mass"),
			new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
			new Qualifier((QualifierTypes)1, "virtual", "coolantTemp")
		};
		((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)ambientQualifiers);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			SetChannel(null);
		}
	}

	private Channel GetActiveChannel(string name)
	{
		foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
		{
			if (activeChannel.Ecu.Name == name)
			{
				return activeChannel;
			}
		}
		return null;
	}

	private void ReadParameters()
	{
		ReadParameter(channel.Parameters["e2p_l_water_raised_eng_hours"]);
		ReadParameter(channel.Parameters["e2p_l_water_raised_eng_starts"]);
	}

	private void ReadParameter(Parameter parameter)
	{
		if (parameter != null && parameter.Channel.Online)
		{
			string groupQualifier = parameter.GroupQualifier;
			parameter.Channel.Parameters.ReadGroup(groupQualifier, fromCache: false, synchronous: false);
		}
	}

	public override void OnChannelsChanged()
	{
		SetChannel(GetActiveChannel("MCM"));
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel != channel)
		{
			if (this.channel != null)
			{
				this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			this.channel = channel;
			if (this.channel != null)
			{
				this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				ReadParameters();
			}
			UpdateUserInterface();
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnButtonResetValuesClick(object sender, EventArgs e)
	{
		WaterInFuelReset();
	}

	private void WaterInFuelReset()
	{
		if (channel != null)
		{
			Service service = channel.Services["RT_SR0AB_Reset_Water_in_Fuel_Values_Start"];
			if (service != null)
			{
				service.ServiceCompleteEvent += OnResetServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
			else
			{
				DisplayMessage(Resources.Message_ErrorCouldNotFindTheServiceRoutineRT_SR0AB_Reset_Water_In_Fuel_Values_Start);
			}
		}
	}

	private void OnResetServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (channel != null)
		{
			Service service = sender as Service;
			if (service != null)
			{
				service.ServiceCompleteEvent -= OnResetServiceCompleteEvent;
				DisplayMessage(Resources.Message_WaterInFuelRegistersHaveBeenReset0);
			}
		}
		ReadParameters();
	}

	private void UpdateUserInterface()
	{
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		buttonResetValues.Enabled = Online;
	}

	private void ClearMessages()
	{
		textBoxOutput.Clear();
	}

	private void DisplayMessage(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(textBoxOutput.Text);
		stringBuilder.AppendLine(text);
		textBoxOutput.Text = stringBuilder.ToString();
		textBoxOutput.SelectionLength = 0;
		textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
		textBoxOutput.ScrollToCaret();
		((CustomPanel)this).AddStatusMessage(text);
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
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelHeader = new TableLayoutPanel();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEnigneLoad = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelDensity = new DigitalReadoutInstrument();
		digitalReadoutIgnitionCycle = new DigitalReadoutInstrument();
		digitalReadoutEngineHours = new DigitalReadoutInstrument();
		chartInstrument1 = new ChartInstrument();
		buttonResetValues = new Button();
		textBoxOutput = new TextBox();
		digitalReadoutWaterLifetimeCountInstrument = new DigitalReadoutInstrument();
		digitalReadoutWaterLifetimeCountParameter = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanelHeader).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutIgnitionCycle, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutEngineHours, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)chartInstrument1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonResetValues, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(textBoxOutput, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutWaterLifetimeCountInstrument, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutWaterLifetimeCountParameter, 0, 4);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(tableLayoutPanelHeader, "tableLayoutPanelHeader");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanelHeader, 5);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentEnigneLoad, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentCoolantTemperature, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentFuelTemperature, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentFuelDensity, 2, 0);
		((Control)(object)tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEnigneLoad, "digitalReadoutInstrumentEnigneLoad");
		digitalReadoutInstrumentEnigneLoad.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentEnigneLoad).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEnigneLoad).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineTorque");
		((Control)(object)digitalReadoutInstrumentEnigneLoad).Name = "digitalReadoutInstrumentEnigneLoad";
		((SingleInstrumentBase)digitalReadoutInstrumentEnigneLoad).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
		digitalReadoutInstrumentCoolantTemperature.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
		digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState)0, 0, "°C");
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelTemperature, "digitalReadoutInstrumentFuelTemperature");
		digitalReadoutInstrumentFuelTemperature.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).FreezeValue = false;
		digitalReadoutInstrumentFuelTemperature.Gradient.Initialize((ValueState)0, 0, "°C");
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "fuelTemp");
		((Control)(object)digitalReadoutInstrumentFuelTemperature).Name = "digitalReadoutInstrumentFuelTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelDensity, "digitalReadoutInstrumentFuelDensity");
		digitalReadoutInstrumentFuelDensity.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelDensity).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentFuelDensity).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS087_Actual_Fuel_Mass");
		((Control)(object)digitalReadoutInstrumentFuelDensity).Name = "digitalReadoutInstrumentFuelDensity";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelDensity).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutIgnitionCycle, 2);
		componentResourceManager.ApplyResources(digitalReadoutIgnitionCycle, "digitalReadoutIgnitionCycle");
		digitalReadoutIgnitionCycle.FontGroup = "tableLayoutPanelWholePanel";
		((SingleInstrumentBase)digitalReadoutIgnitionCycle).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutIgnitionCycle).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS047_Ignition_Cycle_Counter");
		((Control)(object)digitalReadoutIgnitionCycle).Name = "digitalReadoutIgnitionCycle";
		((SingleInstrumentBase)digitalReadoutIgnitionCycle).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutEngineHours, 2);
		componentResourceManager.ApplyResources(digitalReadoutEngineHours, "digitalReadoutEngineHours");
		digitalReadoutEngineHours.FontGroup = "tableLayoutPanelWholePanel";
		((SingleInstrumentBase)digitalReadoutEngineHours).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutEngineHours).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS045_Engine_Operating_Hours");
		((Control)(object)digitalReadoutEngineHours).Name = "digitalReadoutEngineHours";
		((SingleInstrumentBase)digitalReadoutEngineHours).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)chartInstrument1, 3);
		componentResourceManager.ApplyResources(chartInstrument1, "chartInstrument1");
		((Control)(object)chartInstrument1).Name = "chartInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)(object)chartInstrument1, 7);
		chartInstrument1.SelectedTime = null;
		chartInstrument1.ShowEvents = false;
		componentResourceManager.ApplyResources(buttonResetValues, "buttonResetValues");
		buttonResetValues.Name = "buttonResetValues";
		buttonResetValues.UseCompatibleTextRendering = true;
		buttonResetValues.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)textBoxOutput, 2);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)textBoxOutput, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutWaterLifetimeCountInstrument, 2);
		componentResourceManager.ApplyResources(digitalReadoutWaterLifetimeCountInstrument, "digitalReadoutWaterLifetimeCountInstrument");
		digitalReadoutWaterLifetimeCountInstrument.FontGroup = "tableLayoutPanelWholePanel";
		((SingleInstrumentBase)digitalReadoutWaterLifetimeCountInstrument).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutWaterLifetimeCountInstrument).Instrument = new Qualifier((QualifierTypes)4, "MCM", "e2p_l_water_raised_eng_hours");
		((Control)(object)digitalReadoutWaterLifetimeCountInstrument).Name = "digitalReadoutWaterLifetimeCountInstrument";
		((SingleInstrumentBase)digitalReadoutWaterLifetimeCountInstrument).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutWaterLifetimeCountParameter, 2);
		componentResourceManager.ApplyResources(digitalReadoutWaterLifetimeCountParameter, "digitalReadoutWaterLifetimeCountParameter");
		digitalReadoutWaterLifetimeCountParameter.FontGroup = "tableLayoutPanelWholePanel";
		((SingleInstrumentBase)digitalReadoutWaterLifetimeCountParameter).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutWaterLifetimeCountParameter).Instrument = new Qualifier((QualifierTypes)4, "MCM", "e2p_l_water_raised_eng_starts");
		((Control)(object)digitalReadoutWaterLifetimeCountParameter).Name = "digitalReadoutWaterLifetimeCountParameter";
		((SingleInstrumentBase)digitalReadoutWaterLifetimeCountParameter).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_FIS_Water_in_Fuel");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).PerformLayout();
		((Control)(object)tableLayoutPanelHeader).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
