// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentHVActiveDischarge;
  private SharedProcedureSelection sharedProcedureSelection;
  private Label labelStatus;
  private Checkmark checkmarkStatus;
  private Button buttonStartStop;
  private TableLayoutPanel tableLayoutPanelMain;
  private TableLayoutPanel tableLayoutPanelStatusIndicators;
  private DigitalReadoutInstrument digitalReadoutInstrumentHVReady;
  private TableLayoutPanel tableLayoutPanelTop;
  private PictureBox pictureBoxWarningIcon;
  private WebBrowser webBrowserWarning;
  private DigitalReadoutInstrument digitalReadoutInstrumentHVActiveDischargeRequestResults;
  private DigitalReadoutInstrument digitalReadoutInstrumentGlobalHVIL;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentHVActiveDischarge;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.webBrowserWarning.DocumentText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<html><style>{0}</style><body><span class='scaled bold red'>{1}</span><span class='scaled bold'>{2}</span><br><span class='scaled'>{3}</span><span class='scaled bold'>{4}</span></body><span class='scaled'>).</span></html>", (object) ("html { height:100%; display: table; } " + "body { margin: 0px; padding: 0px; display: table-cell; vertical-align: middle; } " + ".scaled { font-size: calc(0.62vw + 8.0vh); font-family: Segoe UI; padding: 0px; margin: 0px; }  " + ".bold { font-weight: bold; }" + ".red { color: red; }"), (object) Resources.RedWarning, (object) Resources.BlackWarning, (object) Resources.WarningText, (object) Resources.ReferenceChecklist);
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelection.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.this_ParentFormClosing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition = new DataItemCondition();
    this.sharedProcedureCreatorComponentHVActiveDischarge = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentHVActiveDischarge = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.labelStatus = new Label();
    this.checkmarkStatus = new Checkmark();
    this.buttonStartStop = new Button();
    this.tableLayoutPanelStatusIndicators = new TableLayoutPanel();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanelTop = new TableLayoutPanel();
    this.pictureBoxWarningIcon = new PictureBox();
    this.webBrowserWarning = new WebBrowser();
    this.digitalReadoutInstrumentHVReady = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHVActiveDischargeRequestResults = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentGlobalHVIL = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanelStatusIndicators).SuspendLayout();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelTop).SuspendLayout();
    ((ISupportInitialize) this.pictureBoxWarningIcon).BeginInit();
    ((Control) this).SuspendLayout();
    this.sharedProcedureCreatorComponentHVActiveDischarge.Suspend();
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status");
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Initialize((ValueState) 0, 6);
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(1, 0.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(5, 4.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(6, 15.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentHVActiveDischarge, "sharedProcedureCreatorComponentHVActiveDischarge");
    this.sharedProcedureCreatorComponentHVActiveDischarge.Qualifier = "SP_OTF_HV_ActiveDischarge";
    this.sharedProcedureCreatorComponentHVActiveDischarge.StartCall = new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Start");
    dataItemCondition.Gradient.Initialize((ValueState) 3, 4);
    dataItemCondition.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition.Gradient.Modify(3, 2.0, (ValueState) 0);
    dataItemCondition.Gradient.Modify(4, 3.0, (ValueState) 0);
    dataItemCondition.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready");
    this.sharedProcedureCreatorComponentHVActiveDischarge.StartConditions.Add(dataItemCondition);
    this.sharedProcedureCreatorComponentHVActiveDischarge.StopCall = new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Stop");
    this.sharedProcedureCreatorComponentHVActiveDischarge.Resume();
    this.sharedProcedureIntegrationComponentHVActiveDischarge.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponentHVActiveDischarge.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponentHVActiveDischarge.ProcedureStatusStateTarget = this.checkmarkStatus;
    this.sharedProcedureIntegrationComponentHVActiveDischarge.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentHVActiveDischarge.StartStopButton = this.buttonStartStop;
    this.sharedProcedureIntegrationComponentHVActiveDischarge.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_OTF_HV_ActiveDischarge"
    });
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelStatusIndicators, "tableLayoutPanelStatusIndicators");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelStatusIndicators, 2);
    ((TableLayoutPanel) this.tableLayoutPanelStatusIndicators).Controls.Add((Control) this.sharedProcedureSelection, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusIndicators).Controls.Add((Control) this.buttonStartStop, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusIndicators).Controls.Add((Control) this.labelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusIndicators).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((Control) this.tableLayoutPanelStatusIndicators).Name = "tableLayoutPanelStatusIndicators";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelStatusIndicators, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelTop, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentHVReady, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentHVActiveDischargeRequestResults, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentGlobalHVIL, 0, 2);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTop, "tableLayoutPanelTop");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelTop, 2);
    ((TableLayoutPanel) this.tableLayoutPanelTop).Controls.Add((Control) this.pictureBoxWarningIcon, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTop).Controls.Add((Control) this.webBrowserWarning, 1, 0);
    ((Control) this.tableLayoutPanelTop).Name = "tableLayoutPanelTop";
    this.pictureBoxWarningIcon.BackColor = Color.White;
    componentResourceManager.ApplyResources((object) this.pictureBoxWarningIcon, "pictureBoxWarningIcon");
    this.pictureBoxWarningIcon.Name = "pictureBoxWarningIcon";
    this.pictureBoxWarningIcon.TabStop = false;
    componentResourceManager.ApplyResources((object) this.webBrowserWarning, "webBrowserWarning");
    this.webBrowserWarning.Name = "webBrowserWarning";
    this.webBrowserWarning.Url = new Uri("about: blank", UriKind.Absolute);
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutInstrumentHVReady, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHVReady, "digitalReadoutInstrumentHVReady");
    this.digitalReadoutInstrumentHVReady.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).FreezeValue = false;
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentHVReady.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready");
    ((Control) this.digitalReadoutInstrumentHVReady).Name = "digitalReadoutInstrumentHVReady";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutInstrumentHVActiveDischargeRequestResults, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHVActiveDischargeRequestResults, "digitalReadoutInstrumentHVActiveDischargeRequestResults");
    this.digitalReadoutInstrumentHVActiveDischargeRequestResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVActiveDischargeRequestResults).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVActiveDischargeRequestResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status");
    ((Control) this.digitalReadoutInstrumentHVActiveDischargeRequestResults).Name = "digitalReadoutInstrumentHVActiveDischargeRequestResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVActiveDischargeRequestResults).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutInstrumentGlobalHVIL, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentGlobalHVIL, "digitalReadoutInstrumentGlobalHVIL");
    this.digitalReadoutInstrumentGlobalHVIL.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGlobalHVIL).FreezeValue = false;
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.Initialize((ValueState) 0, 5);
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGlobalHVIL).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS219_globalhvil_globalhvil");
    ((Control) this.digitalReadoutInstrumentGlobalHVIL).Name = "digitalReadoutInstrumentGlobalHVIL";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGlobalHVIL).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGlobalHVIL).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelStatusIndicators).ResumeLayout(false);
    ((Control) this.tableLayoutPanelStatusIndicators).PerformLayout();
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelTop).ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxWarningIcon).EndInit();
    ((Control) this).ResumeLayout(false);
  }
}
