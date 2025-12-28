// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Global_Radar_Dyno_Mode.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Global_Radar_Dyno_Mode.panel;

public class UserPanel : CustomPanel
{
  private readonly string GlobalRadarStatusQualifier = "RT_ABA_release_Request_Results_release_state";
  private readonly string DisplayMessageToEnable = Resources.Message_TheDetroitAssuranceAdvancedBrakeAssistFunctionIsCurrentlyDisabledToEnableDetroitAssuranceOnThisVehicleClickTheStartButton;
  private readonly string DisplayMessageToDisable = Resources.Message_TheDetroitAssuranceAdvancedBrakeAssistFunctionIsCurrentlyEnabledToDisableDetroitAssuranceOnThisVehicleClickTheStartButton;
  private SubjectCollection DisableGlobalRadar = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_GlobalRadarDisable"
  });
  private SubjectCollection EnableGlobalRadar = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_GlobalRadarEnable"
  });
  private SharedProcedureBase selectedProcedure;
  private Channel ecu;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private Checkmark checkmark1;
  private Button button1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label DisplayMessageLabel;
  private SeekTimeListView seekTimeListView1;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.SubscribeToEvents(this.sharedProcedureSelection1.SelectedProcedure);
    this.SetECU(this.GetChannel("VRDU01T"));
  }

  public virtual void OnChannelsChanged()
  {
    Channel ecu = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Name == "VRDU01T"));
    if (this.ecu == ecu)
      return;
    this.SetECU(ecu);
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    if (this.selectedProcedure != null)
      this.selectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.OnStopComplete);
    if (this.ecu != null)
      this.ecu.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
  }

  private void OnStopComplete(object sender, PassFailResultEventArgs e)
  {
    if (e.Result != 1)
      return;
    this.RequestGlobalRadarStatus();
  }

  private void OnCheckStatusComplete(object sender, ResultEventArgs e)
  {
    Service service = (Service) sender;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnCheckStatusComplete);
    if (service.OutputValues.Count <= 0)
      return;
    Choice itemFromRawValue = service.OutputValues[0].Value as Choice;
    byte rawValue = 3;
    if (itemFromRawValue != (object) null)
      rawValue = Convert.ToByte(itemFromRawValue.RawValue);
    else
      itemFromRawValue = service.OutputValues[0].Choices.GetItemFromRawValue((object) rawValue);
    if (itemFromRawValue != (object) null)
    {
      switch (rawValue)
      {
        case 1:
          this.sharedProcedureSelection1.SharedProcedureQualifiers = this.DisableGlobalRadar;
          break;
        case 2:
          this.sharedProcedureSelection1.SharedProcedureQualifiers = this.EnableGlobalRadar;
          break;
      }
      this.SubscribeToEvents(this.sharedProcedureSelection1.SelectedProcedure);
    }
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (!((Channel) sender).Ecu.Name.Equals("VRDU01T") || e.CommunicationsState != CommunicationsState.Online)
      return;
    this.RequestGlobalRadarStatus();
    this.ecu.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
  }

  private bool CanClose => !this.sharedProcedureSelection1.AnyProcedureInProgress;

  private bool Online
  {
    get => this.ecu != null && this.ecu.CommunicationsState == CommunicationsState.Online;
  }

  private Service GetStatusService
  {
    get => this.ecu == null ? (Service) null : this.ecu.Services[this.GlobalRadarStatusQualifier];
  }

  private void SetECU(Channel ecu)
  {
    if (this.ecu == ecu)
      return;
    this.ecu = ecu;
    if (this.ecu != null)
    {
      if (this.ecu.CommunicationsState == CommunicationsState.Online)
        this.RequestGlobalRadarStatus();
      else
        this.ecu.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
  }

  private void SubscribeToEvents(SharedProcedureBase procedure)
  {
    if (procedure == this.selectedProcedure)
      return;
    if (this.selectedProcedure != null)
      this.selectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.OnStopComplete);
    this.selectedProcedure = procedure;
    if (this.selectedProcedure != null)
      this.selectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.OnStopComplete);
  }

  private void RequestGlobalRadarStatus()
  {
    Service getStatusService = this.GetStatusService;
    if (!(getStatusService != (Service) null) || !this.Online)
      return;
    getStatusService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnCheckStatusComplete);
    getStatusService.Execute(false);
  }

  private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (this.digitalReadoutInstrument1.RepresentedState == 1)
    {
      this.DisplayMessageLabel.Text = this.DisplayMessageToDisable;
    }
    else
    {
      if (this.digitalReadoutInstrument1.RepresentedState != 2)
        return;
      this.DisplayMessageLabel.Text = this.DisplayMessageToEnable;
    }
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.checkmark1 = new Checkmark();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.seekTimeListView1 = new SeekTimeListView();
    this.button1 = new Button();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.label1 = new System.Windows.Forms.Label();
    this.DisplayMessageLabel = new System.Windows.Forms.Label();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button1, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DisplayMessageLabel, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(4, (double) byte.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "VRDU01T", "RT_ABA_release_Request_Results_release_state");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrument1.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument1_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "Global Radar";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_GlobalRadarDisable"
    });
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.DisplayMessageLabel, 4);
    componentResourceManager.ApplyResources((object) this.DisplayMessageLabel, "DisplayMessageLabel");
    this.DisplayMessageLabel.Name = "DisplayMessageLabel";
    this.DisplayMessageLabel.UseCompatibleTextRendering = true;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.label1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.button1;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Active_Brake_Assist_-_Enable_Disable");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
