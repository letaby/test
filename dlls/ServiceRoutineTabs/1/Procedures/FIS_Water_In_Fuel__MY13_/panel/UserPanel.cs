// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__MY13_.panel.UserPanel
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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string MCMName = "MCM21T";
  private const string InstrumentTotalTimeEngineOnQualifier = "DT_AS045_Engine_Operating_Hours";
  private const string InstrumentFuelTotalTimeWIFLightActiveQualifier = "DT_AS045_Engine_Operating_Hours";
  private const string E2pLWaterRaisedEngHours = "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours";
  private const string E2pLWaterRaisedEngStarts = "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_starts";
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
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

  public UserPanel()
  {
    this.InitializeComponent();
    this.InitQualifiers();
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
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.ambientQualifiers);
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

  private void ReadStoredData()
  {
    this.ReadEcuInfo(this.channel.EcuInfos["DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours"]);
    this.ReadEcuInfo(this.channel.EcuInfos["DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_starts"]);
  }

  private void ReadParameter(Parameter parameter)
  {
    if (parameter == null || !parameter.Channel.Online)
      return;
    string groupQualifier = parameter.GroupQualifier;
    parameter.Channel.Parameters.ReadGroup(groupQualifier, false, false);
  }

  private void ReadEcuInfo(EcuInfo ecuInfo)
  {
    if (ecuInfo == null || !ecuInfo.Channel.Online)
      return;
    ecuInfo.Channel.EcuInfos[ecuInfo.Qualifier].Read(false);
  }

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetActiveChannel("MCM21T"));

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.channel = channel;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.ReadStoredData();
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

  private void OnButtonResetValuesClick(object sender, EventArgs e) => this.WaterInFuelReset();

  private void WaterInFuelReset()
  {
    if (this.channel == null)
      return;
    Service service = this.channel.Services["RT_SR0AB_Reset_Water_in_Fuel_Values_Start"];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnResetServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.DisplayMessage(Resources.Message_ErrorCouldNotFindTheServiceRoutineRT_SR0AB_Reset_Water_In_Fuel_Values_Start0);
  }

  private void OnResetServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.channel != null)
    {
      Service service = sender as Service;
      if (service != (Service) null)
      {
        service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnResetServiceCompleteEvent);
        this.DisplayMessage(Resources.Message_WaterInFuelRegistersHaveBeenReset);
      }
    }
    this.ReadStoredData();
  }

  private void UpdateUserInterface() => this.UpdateButtonState();

  private void UpdateButtonState() => this.buttonResetValues.Enabled = this.Online;

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

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelHeader = new TableLayoutPanel();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEnigneLoad = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelDensity = new DigitalReadoutInstrument();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.digitalReadoutIgnitionCycle = new DigitalReadoutInstrument();
    this.digitalReadoutEngineHours = new DigitalReadoutInstrument();
    this.chartInstrument1 = new ChartInstrument();
    this.buttonResetValues = new Button();
    this.textBoxOutput = new TextBox();
    this.digitalReadoutWaterLifetimeCountInstrument = new DigitalReadoutInstrument();
    this.digitalReadoutWaterLifetimeCountParameter = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanelHeader).SuspendLayout();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this).SuspendLayout();
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
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutIgnitionCycle, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutEngineHours, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.chartInstrument1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonResetValues, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.textBoxOutput, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutWaterLifetimeCountInstrument, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutWaterLifetimeCountParameter, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 2);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.digitalReadoutIgnitionCycle, "digitalReadoutIgnitionCycle");
    this.digitalReadoutIgnitionCycle.FontGroup = "FIS_WIF_Values";
    ((SingleInstrumentBase) this.digitalReadoutIgnitionCycle).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutIgnitionCycle).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS047_Ignition_Cycle_Counter");
    ((Control) this.digitalReadoutIgnitionCycle).Name = "digitalReadoutIgnitionCycle";
    ((SingleInstrumentBase) this.digitalReadoutIgnitionCycle).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutEngineHours, "digitalReadoutEngineHours");
    this.digitalReadoutEngineHours.FontGroup = "FIS_WIF_Values";
    ((SingleInstrumentBase) this.digitalReadoutEngineHours).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutEngineHours).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS045_Engine_Operating_Hours");
    ((Control) this.digitalReadoutEngineHours).Name = "digitalReadoutEngineHours";
    ((SingleInstrumentBase) this.digitalReadoutEngineHours).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.chartInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.chartInstrument1, "chartInstrument1");
    ((Control) this.chartInstrument1).Name = "chartInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetRowSpan((Control) this.chartInstrument1, 6);
    this.chartInstrument1.SelectedTime = new DateTime?();
    this.chartInstrument1.ShowEvents = false;
    componentResourceManager.ApplyResources((object) this.buttonResetValues, "buttonResetValues");
    this.buttonResetValues.Name = "buttonResetValues";
    this.buttonResetValues.UseCompatibleTextRendering = true;
    this.buttonResetValues.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.textBoxOutput, 2);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutWaterLifetimeCountInstrument, "digitalReadoutWaterLifetimeCountInstrument");
    this.digitalReadoutWaterLifetimeCountInstrument.FontGroup = "FIS_WIF_Values";
    ((SingleInstrumentBase) this.digitalReadoutWaterLifetimeCountInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutWaterLifetimeCountInstrument).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours");
    ((Control) this.digitalReadoutWaterLifetimeCountInstrument).Name = "digitalReadoutWaterLifetimeCountInstrument";
    ((SingleInstrumentBase) this.digitalReadoutWaterLifetimeCountInstrument).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutWaterLifetimeCountParameter, "digitalReadoutWaterLifetimeCountParameter");
    this.digitalReadoutWaterLifetimeCountParameter.FontGroup = "FIS_WIF_Values";
    ((SingleInstrumentBase) this.digitalReadoutWaterLifetimeCountParameter).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutWaterLifetimeCountParameter).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC065_OP_Data_Oil_e2p_i_water_raised_eng_starts");
    ((Control) this.digitalReadoutWaterLifetimeCountParameter).Name = "digitalReadoutWaterLifetimeCountParameter";
    ((SingleInstrumentBase) this.digitalReadoutWaterLifetimeCountParameter).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "FIS_WIF_Faults";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM21T", "61000F");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "FIS_WIF_Faults";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM21T", "610010");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_FIS_Water_in_Fuel");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelHeader).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
