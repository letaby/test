using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter.panel;

public class UserPanel : CustomPanel
{
	private const string MCMName = "MCM02T";

	private const string InstrumentCoolantTemperatureQualifier = "DT_AS067_Coolant_Temperatures_2";

	private const string InstrumentFuelTemperatureQualifier = "DT_AS014_Fuel_Temperature";

	private Channel channel;

	private Qualifier[] ambientQualifiers;

	private Qualifier[] actualValuesQualifiers;

	private List<FaultInstrument> faultInstruments = new List<FaultInstrument>(2);

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;

	private Button buttonResetValues;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private TableLayoutPanel tableLayoutPanelHeader;

	private DigitalReadoutInstrument digitalReadoutInstrumentEnigneLoad;

	private ChartInstrument chartInstrument1;

	private CheckBox checkBoxAmbientData;

	private CheckBox checkBoxActualValues;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelDensity;

	private DigitalReadoutInstrument digitalReadoutInstrumentPFuelFilterAct;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelFilterState;

	private ScalingLabel scalingLabelFuelFilterFail;

	private ScalingLabel scalingLabelFuelFilterService;

	private Label labelFuelFilterReplacementRequired;

	private Label labelFuelFilterServiceWarning;

	private TextBox textBoxOutput;

	private TableLayoutPanel tableLayoutPanelFuelFilterReplacementFaultInstrument;

	private TableLayoutPanel tableLayoutPanelFuelServiceWarning;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

	private bool Online => channel != null && channel.CommunicationsState == CommunicationsState.Online;

	private bool RunningLogFile => channel != null && channel.LogFile != null;

	private bool CanResetValues => Online && (int)digitalReadoutInstrumentEngineSpeed.RepresentedState == 1;

	public UserPanel()
	{
		InitializeComponent();
		InitQualifiers();
		faultInstruments.Add(new FaultInstrument(labelFuelFilterReplacementRequired, scalingLabelFuelFilterFail, ((Control)(object)scalingLabelFuelFilterFail).Tag.ToString()));
		faultInstruments.Add(new FaultInstrument(labelFuelFilterServiceWarning, scalingLabelFuelFilterService, ((Control)(object)scalingLabelFuelFilterService).Tag.ToString()));
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
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		ambientQualifiers = (Qualifier[])(object)new Qualifier[5]
		{
			new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
			new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
			new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
			new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
			new Qualifier((QualifierTypes)1, "virtual", "coolantTemp")
		};
		actualValuesQualifiers = (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS125_Fuel_Filter_State"),
			new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure")
		};
		((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)ambientQualifiers);
		((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)actualValuesQualifiers);
		checkBoxAmbientData.CheckStateChanged += OnCheckBoxCheckStateChanged;
		checkBoxActualValues.CheckStateChanged += OnCheckBoxCheckStateChanged;
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

	public override void OnChannelsChanged()
	{
		SetChannel(GetActiveChannel("MCM02T"));
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel == channel)
		{
			return;
		}
		if (this.channel != null)
		{
			this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			foreach (FaultInstrument faultInstrument in faultInstruments)
			{
				if (this.channel.FaultCodes[faultInstrument.faultCodeId] != null)
				{
					this.channel.FaultCodes[faultInstrument.faultCodeId].FaultCodeIncidents.FaultCodeIncidentUpdateEvent -= faultInstrument.OnFaultCodeIncidentUpdateHandler;
				}
			}
			digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged -= digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged;
		}
		this.channel = channel;
		if (this.channel != null)
		{
			this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			foreach (FaultInstrument faultInstrument2 in faultInstruments)
			{
				FaultCode faultCode = this.channel.FaultCodes[faultInstrument2.faultCodeId];
				faultInstrument2.DisplayFaultTitle(faultCode);
				if (faultCode != null)
				{
					faultInstrument2.DisplayFaultText(faultCode.FaultCodeIncidents);
					faultCode.FaultCodeIncidents.FaultCodeIncidentUpdateEvent += faultInstrument2.OnFaultCodeIncidentUpdateHandler;
				}
				else
				{
					faultInstrument2.DisplayFaultText(null);
				}
			}
			digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged;
		}
		UpdateUserInterface();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnCheckBoxCheckStateChanged(object sender, EventArgs e)
	{
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Clear();
		if (checkBoxAmbientData.Checked)
		{
			((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)ambientQualifiers);
		}
		if (checkBoxActualValues.Checked)
		{
			((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)actualValuesQualifiers);
		}
	}

	private void OnButtonResetValuesClick(object sender, EventArgs e)
	{
		FuelFilterReset();
	}

	private void FuelFilterReset()
	{
		if (channel != null)
		{
			Service service = channel.Services["RT_SR082_Fuel_Filter_Reset_Start_Status"];
			if (service != null)
			{
				service.ServiceCompleteEvent += OnFuelFilterResetServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
		}
	}

	private void OnFuelFilterResetServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (channel != null)
		{
			Service service = sender as Service;
			if (service != null)
			{
				service.ServiceCompleteEvent -= OnFuelFilterResetServiceCompleteEvent;
				ServiceOutputValue serviceOutputValue = service.OutputValues[0];
				DisplayMessage(string.Format(arg0: ((serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(1)) ? true : false) ? string.Empty : Resources.Message_Not, provider: CultureInfo.CurrentCulture, format: Resources.MessageFormat_ValuesReset + "\r\n" + Resources.FuelFilterCalculationIs0Active0));
			}
		}
	}

	private void UpdateUserInterface()
	{
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		buttonResetValues.Enabled = CanResetValues;
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

	private void digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateButtonState();
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		tableLayoutPanelHeader = new TableLayoutPanel();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEnigneLoad = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelDensity = new DigitalReadoutInstrument();
		digitalReadoutInstrumentPFuelFilterAct = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelFilterState = new DigitalReadoutInstrument();
		textBoxOutput = new TextBox();
		tableLayoutPanelFuelFilterReplacementFaultInstrument = new TableLayoutPanel();
		scalingLabelFuelFilterFail = new ScalingLabel();
		labelFuelFilterReplacementRequired = new Label();
		chartInstrument1 = new ChartInstrument();
		tableLayoutPanelFuelServiceWarning = new TableLayoutPanel();
		labelFuelFilterServiceWarning = new Label();
		scalingLabelFuelFilterService = new ScalingLabel();
		buttonResetValues = new Button();
		checkBoxAmbientData = new CheckBox();
		checkBoxActualValues = new CheckBox();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanelHeader).SuspendLayout();
		((Control)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument).SuspendLayout();
		((Control)(object)tableLayoutPanelFuelServiceWarning).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrumentPFuelFilterAct, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrumentFuelFilterState, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(textBoxOutput, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)chartInstrument1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelFuelServiceWarning, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(buttonResetValues, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(checkBoxAmbientData, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(checkBoxActualValues, 3, 6);
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
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
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
		((SingleInstrumentBase)digitalReadoutInstrumentFuelDensity).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass");
		((Control)(object)digitalReadoutInstrumentFuelDensity).Name = "digitalReadoutInstrumentFuelDensity";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelDensity).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentPFuelFilterAct, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentPFuelFilterAct, "digitalReadoutInstrumentPFuelFilterAct");
		digitalReadoutInstrumentPFuelFilterAct.FontGroup = "tableLayoutPanelWholePanel";
		((SingleInstrumentBase)digitalReadoutInstrumentPFuelFilterAct).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentPFuelFilterAct).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure");
		((Control)(object)digitalReadoutInstrumentPFuelFilterAct).Name = "digitalReadoutInstrumentPFuelFilterAct";
		((SingleInstrumentBase)digitalReadoutInstrumentPFuelFilterAct).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentFuelFilterState, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelFilterState, "digitalReadoutInstrumentFuelFilterState");
		digitalReadoutInstrumentFuelFilterState.FontGroup = "tableLayoutPanelWholePanel";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelFilterState).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentFuelFilterState).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS125_Fuel_Filter_State");
		((Control)(object)digitalReadoutInstrumentFuelFilterState).Name = "digitalReadoutInstrumentFuelFilterState";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelFilterState).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)textBoxOutput, 2);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		componentResourceManager.ApplyResources(tableLayoutPanelFuelFilterReplacementFaultInstrument, "tableLayoutPanelFuelFilterReplacementFaultInstrument");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument).Controls.Add((Control)(object)scalingLabelFuelFilterFail, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument).Controls.Add((Control)(object)labelFuelFilterReplacementRequired, 0, 0);
		((Control)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument).Name = "tableLayoutPanelFuelFilterReplacementFaultInstrument";
		scalingLabelFuelFilterFail.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelFuelFilterFail, "scalingLabelFuelFilterFail");
		scalingLabelFuelFilterFail.FontGroup = "tableLayoutPanelWholePanel";
		scalingLabelFuelFilterFail.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFuelFilterFail).Name = "scalingLabelFuelFilterFail";
		((Control)(object)scalingLabelFuelFilterFail).Tag = "5E0010";
		labelFuelFilterReplacementRequired.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFuelFilterReplacementRequired, "labelFuelFilterReplacementRequired");
		((Control)(object)labelFuelFilterReplacementRequired).Name = "labelFuelFilterReplacementRequired";
		labelFuelFilterReplacementRequired.Orientation = (TextOrientation)1;
		((Control)(object)labelFuelFilterReplacementRequired).Tag = "5E0010";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)chartInstrument1, 3);
		componentResourceManager.ApplyResources(chartInstrument1, "chartInstrument1");
		((Control)(object)chartInstrument1).Name = "chartInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetRowSpan((Control)(object)chartInstrument1, 5);
		chartInstrument1.SelectedTime = null;
		chartInstrument1.ShowEvents = false;
		componentResourceManager.ApplyResources(tableLayoutPanelFuelServiceWarning, "tableLayoutPanelFuelServiceWarning");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanelFuelServiceWarning, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFuelServiceWarning).Controls.Add((Control)(object)labelFuelFilterServiceWarning, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFuelServiceWarning).Controls.Add((Control)(object)scalingLabelFuelFilterService, 0, 1);
		((Control)(object)tableLayoutPanelFuelServiceWarning).Name = "tableLayoutPanelFuelServiceWarning";
		labelFuelFilterServiceWarning.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFuelFilterServiceWarning, "labelFuelFilterServiceWarning");
		((Control)(object)labelFuelFilterServiceWarning).Name = "labelFuelFilterServiceWarning";
		labelFuelFilterServiceWarning.Orientation = (TextOrientation)1;
		((Control)(object)labelFuelFilterServiceWarning).Tag = "5E000F";
		scalingLabelFuelFilterService.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelFuelFilterService, "scalingLabelFuelFilterService");
		scalingLabelFuelFilterService.FontGroup = "tableLayoutPanelWholePanel";
		scalingLabelFuelFilterService.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFuelFilterService).Name = "scalingLabelFuelFilterService";
		((Control)(object)scalingLabelFuelFilterService).Tag = "5E000F";
		componentResourceManager.ApplyResources(buttonResetValues, "buttonResetValues");
		buttonResetValues.Name = "buttonResetValues";
		buttonResetValues.UseCompatibleTextRendering = true;
		buttonResetValues.UseVisualStyleBackColor = true;
		checkBoxAmbientData.Checked = true;
		checkBoxAmbientData.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkBoxAmbientData, "checkBoxAmbientData");
		checkBoxAmbientData.Name = "checkBoxAmbientData";
		checkBoxAmbientData.UseCompatibleTextRendering = true;
		checkBoxAmbientData.UseVisualStyleBackColor = true;
		checkBoxActualValues.Checked = true;
		checkBoxActualValues.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkBoxActualValues, "checkBoxActualValues");
		checkBoxActualValues.Name = "checkBoxActualValues";
		checkBoxActualValues.UseCompatibleTextRendering = true;
		checkBoxActualValues.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_FIS_Fuel_Filter");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).PerformLayout();
		((Control)(object)tableLayoutPanelHeader).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFuelFilterReplacementFaultInstrument).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFuelServiceWarning).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
