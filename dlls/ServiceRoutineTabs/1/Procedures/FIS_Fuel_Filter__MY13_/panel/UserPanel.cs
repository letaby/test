// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string MCMName = "MCM21T";
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
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFuelFilterReplacementRequired;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFuelFilterServiceWarning;
  private TextBox textBoxOutput;
  private TableLayoutPanel tableLayoutPanelFuelFilterReplacementFaultInstrument;
  private TableLayoutPanel tableLayoutPanelFuelServiceWarning;
  private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

  public UserPanel()
  {
    this.InitializeComponent();
    this.InitQualifiers();
    this.faultInstruments.Add(new FaultInstrument(this.labelFuelFilterReplacementRequired, this.scalingLabelFuelFilterFail, ((Control) this.scalingLabelFuelFilterFail).Tag.ToString()));
    this.faultInstruments.Add(new FaultInstrument(this.labelFuelFilterServiceWarning, this.scalingLabelFuelFilterService, ((Control) this.scalingLabelFuelFilterService).Tag.ToString()));
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.buttonResetValues.Click += new EventHandler(this.OnButtonResetValuesClick);
  }

  private void InitQualifiers()
  {
    this.ambientQualifiers = new Qualifier[5]
    {
      new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
      new Qualifier((QualifierTypes) 1, "virtual", "engineTorque"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
      new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
      new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp")
    };
    this.actualValuesQualifiers = new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS125_Fuel_Filter_State"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure")
    };
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.ambientQualifiers);
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.actualValuesQualifiers);
    this.checkBoxAmbientData.CheckStateChanged += new EventHandler(this.OnCheckBoxCheckStateChanged);
    this.checkBoxActualValues.CheckStateChanged += new EventHandler(this.OnCheckBoxCheckStateChanged);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.SetChannel((Channel) null);
  }

  private Channel GetActiveChannel(string name)
  {
    foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
    {
      if (activeChannel.Ecu.Name == name)
        return activeChannel;
    }
    return (Channel) null;
  }

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetActiveChannel("MCM21T"));

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      foreach (FaultInstrument faultInstrument in this.faultInstruments)
      {
        if (this.channel.FaultCodes[faultInstrument.faultCodeId] != null)
          this.channel.FaultCodes[faultInstrument.faultCodeId].FaultCodeIncidents.FaultCodeIncidentUpdateEvent -= new EventHandler(faultInstrument.OnFaultCodeIncidentUpdateHandler);
      }
      this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged);
    }
    this.channel = channel;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      foreach (FaultInstrument faultInstrument in this.faultInstruments)
      {
        FaultCode faultCode = this.channel.FaultCodes[faultInstrument.faultCodeId];
        faultInstrument.DisplayFaultTitle(faultCode);
        if (faultCode != null)
        {
          faultInstrument.DisplayFaultText(faultCode.FaultCodeIncidents);
          faultCode.FaultCodeIncidents.FaultCodeIncidentUpdateEvent += new EventHandler(faultInstrument.OnFaultCodeIncidentUpdateHandler);
        }
        else
          faultInstrument.DisplayFaultText((FaultCodeIncidentCollection) null);
      }
      this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged);
    }
    this.UpdateUserInterface();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private bool Online
  {
    get => this.channel != null && this.channel.CommunicationsState == CommunicationsState.Online;
  }

  private bool RunningLogFile => this.channel != null && this.channel.LogFile != null;

  private void OnCheckBoxCheckStateChanged(object sender, EventArgs e)
  {
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Clear();
    if (this.checkBoxAmbientData.Checked)
      ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.ambientQualifiers);
    if (!this.checkBoxActualValues.Checked)
      return;
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.actualValuesQualifiers);
  }

  private void OnButtonResetValuesClick(object sender, EventArgs e) => this.FuelFilterReset();

  private void FuelFilterReset()
  {
    if (this.channel == null)
      return;
    Service service = this.channel.Services["RT_SR082_Fuel_Filter_Reset_Start_Status"];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnFuelFilterResetServiceCompleteEvent);
      service.Execute(false);
    }
  }

  private void OnFuelFilterResetServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.channel == null)
      return;
    Service service = sender as Service;
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnFuelFilterResetServiceCompleteEvent);
      ServiceOutputValue outputValue = service.OutputValues[0];
      this.DisplayMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, $"{Resources.MessageFormat_ValuesReset}\r\n{Resources.FuelFilterCalculationIs0Active}", outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 1) ? (object) string.Empty : (object) Resources.Message_Not));
    }
  }

  private void UpdateUserInterface() => this.UpdateButtonState();

  private bool CanResetValues
  {
    get => this.Online && this.digitalReadoutInstrumentEngineSpeed.RepresentedState == 1;
  }

  private void UpdateButtonState() => this.buttonResetValues.Enabled = this.CanResetValues;

  private void ClearMessages() => this.textBoxOutput.Clear();

  private void DisplayMessage(string text)
  {
    StringBuilder stringBuilder = new StringBuilder(this.textBoxOutput.Text);
    stringBuilder.AppendLine(text);
    this.textBoxOutput.Text = stringBuilder.ToString();
    this.textBoxOutput.SelectionLength = 0;
    this.textBoxOutput.SelectionStart = this.textBoxOutput.Text.Length;
    this.textBoxOutput.ScrollToCaret();
    this.AddStatusMessage(text);
  }

  private void digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateButtonState();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelFuelServiceWarning = new TableLayoutPanel();
    this.labelFuelFilterServiceWarning = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelFuelFilterService = new ScalingLabel();
    this.tableLayoutPanelFuelFilterReplacementFaultInstrument = new TableLayoutPanel();
    this.scalingLabelFuelFilterFail = new ScalingLabel();
    this.labelFuelFilterReplacementRequired = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.tableLayoutPanelHeader = new TableLayoutPanel();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEnigneLoad = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelDensity = new DigitalReadoutInstrument();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.digitalReadoutInstrumentPFuelFilterAct = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelFilterState = new DigitalReadoutInstrument();
    this.textBoxOutput = new TextBox();
    this.chartInstrument1 = new ChartInstrument();
    this.buttonResetValues = new Button();
    this.checkBoxAmbientData = new CheckBox();
    this.checkBoxActualValues = new CheckBox();
    ((Control) this.tableLayoutPanelFuelServiceWarning).SuspendLayout();
    ((Control) this.tableLayoutPanelFuelFilterReplacementFaultInstrument).SuspendLayout();
    ((Control) this.tableLayoutPanelHeader).SuspendLayout();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFuelServiceWarning, "tableLayoutPanelFuelServiceWarning");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanelFuelServiceWarning, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFuelServiceWarning).Controls.Add((Control) this.labelFuelFilterServiceWarning, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFuelServiceWarning).Controls.Add((Control) this.scalingLabelFuelFilterService, 0, 1);
    ((Control) this.tableLayoutPanelFuelServiceWarning).Name = "tableLayoutPanelFuelServiceWarning";
    this.labelFuelFilterServiceWarning.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFuelFilterServiceWarning, "labelFuelFilterServiceWarning");
    ((Control) this.labelFuelFilterServiceWarning).Name = "labelFuelFilterServiceWarning";
    this.labelFuelFilterServiceWarning.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    ((Control) this.labelFuelFilterServiceWarning).Tag = (object) "5E000F";
    this.scalingLabelFuelFilterService.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelFuelFilterService, "scalingLabelFuelFilterService");
    this.scalingLabelFuelFilterService.FontGroup = "tableLayoutPanelWholePanel";
    this.scalingLabelFuelFilterService.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFuelFilterService).Name = "scalingLabelFuelFilterService";
    ((Control) this.scalingLabelFuelFilterService).Tag = (object) "5E000F";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFuelFilterReplacementFaultInstrument, "tableLayoutPanelFuelFilterReplacementFaultInstrument");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanelFuelFilterReplacementFaultInstrument, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFuelFilterReplacementFaultInstrument).Controls.Add((Control) this.scalingLabelFuelFilterFail, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFuelFilterReplacementFaultInstrument).Controls.Add((Control) this.labelFuelFilterReplacementRequired, 0, 0);
    ((Control) this.tableLayoutPanelFuelFilterReplacementFaultInstrument).Name = "tableLayoutPanelFuelFilterReplacementFaultInstrument";
    this.scalingLabelFuelFilterFail.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelFuelFilterFail, "scalingLabelFuelFilterFail");
    this.scalingLabelFuelFilterFail.FontGroup = "tableLayoutPanelWholePanel";
    this.scalingLabelFuelFilterFail.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFuelFilterFail).Name = "scalingLabelFuelFilterFail";
    ((Control) this.scalingLabelFuelFilterFail).Tag = (object) "5E0010";
    this.labelFuelFilterReplacementRequired.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFuelFilterReplacementRequired, "labelFuelFilterReplacementRequired");
    ((Control) this.labelFuelFilterReplacementRequired).Name = "labelFuelFilterReplacementRequired";
    this.labelFuelFilterReplacementRequired.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    ((Control) this.labelFuelFilterReplacementRequired).Tag = (object) "5E0010";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelHeader, "tableLayoutPanelHeader");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanelHeader, 5);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentEnigneLoad, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentCoolantTemperature, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentFuelTemperature, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentFuelDensity, 2, 0);
    ((Control) this.tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEnigneLoad, "digitalReadoutInstrumentEnigneLoad");
    this.digitalReadoutInstrumentEnigneLoad.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnigneLoad).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnigneLoad).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineTorque");
    ((Control) this.digitalReadoutInstrumentEnigneLoad).Name = "digitalReadoutInstrumentEnigneLoad";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnigneLoad).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
    this.digitalReadoutInstrumentCoolantTemperature.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState) 0, 0, "°C");
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelTemperature, "digitalReadoutInstrumentFuelTemperature");
    this.digitalReadoutInstrumentFuelTemperature.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentFuelTemperature.Gradient.Initialize((ValueState) 0, 0, "°C");
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp");
    ((Control) this.digitalReadoutInstrumentFuelTemperature).Name = "digitalReadoutInstrumentFuelTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelDensity, "digitalReadoutInstrumentFuelDensity");
    this.digitalReadoutInstrumentFuelDensity.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelDensity).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelDensity).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass");
    ((Control) this.digitalReadoutInstrumentFuelDensity).Name = "digitalReadoutInstrumentFuelDensity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelDensity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrumentPFuelFilterAct, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrumentFuelFilterState, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.textBoxOutput, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelFuelFilterReplacementFaultInstrument, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.chartInstrument1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelFuelServiceWarning, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonResetValues, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.checkBoxAmbientData, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.checkBoxActualValues, 3, 6);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrumentPFuelFilterAct, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPFuelFilterAct, "digitalReadoutInstrumentPFuelFilterAct");
    this.digitalReadoutInstrumentPFuelFilterAct.FontGroup = "tableLayoutPanelWholePanel";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPFuelFilterAct).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPFuelFilterAct).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure");
    ((Control) this.digitalReadoutInstrumentPFuelFilterAct).Name = "digitalReadoutInstrumentPFuelFilterAct";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPFuelFilterAct).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrumentFuelFilterState, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelFilterState, "digitalReadoutInstrumentFuelFilterState");
    this.digitalReadoutInstrumentFuelFilterState.FontGroup = "tableLayoutPanelWholePanel";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelFilterState).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelFilterState).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS125_Fuel_Filter_State");
    ((Control) this.digitalReadoutInstrumentFuelFilterState).Name = "digitalReadoutInstrumentFuelFilterState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelFilterState).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.textBoxOutput, 2);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.chartInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.chartInstrument1, "chartInstrument1");
    ((Control) this.chartInstrument1).Name = "chartInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetRowSpan((Control) this.chartInstrument1, 5);
    this.chartInstrument1.SelectedTime = new DateTime?();
    this.chartInstrument1.ShowButtonPanel = false;
    this.chartInstrument1.ShowEvents = false;
    componentResourceManager.ApplyResources((object) this.buttonResetValues, "buttonResetValues");
    this.buttonResetValues.Name = "buttonResetValues";
    this.buttonResetValues.UseCompatibleTextRendering = true;
    this.buttonResetValues.UseVisualStyleBackColor = true;
    this.checkBoxAmbientData.Checked = true;
    this.checkBoxAmbientData.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkBoxAmbientData, "checkBoxAmbientData");
    this.checkBoxAmbientData.Name = "checkBoxAmbientData";
    this.checkBoxAmbientData.UseCompatibleTextRendering = true;
    this.checkBoxAmbientData.UseVisualStyleBackColor = true;
    this.checkBoxActualValues.Checked = true;
    this.checkBoxActualValues.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkBoxActualValues, "checkBoxActualValues");
    this.checkBoxActualValues.Name = "checkBoxActualValues";
    this.checkBoxActualValues.UseCompatibleTextRendering = true;
    this.checkBoxActualValues.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_FIS_Fuel_Filter");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelFuelServiceWarning).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFuelFilterReplacementFaultInstrument).ResumeLayout(false);
    ((Control) this.tableLayoutPanelHeader).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
