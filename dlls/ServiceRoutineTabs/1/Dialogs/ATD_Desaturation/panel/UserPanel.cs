// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Net;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel;

public class UserPanel : CustomPanel
{
  private const string Cpc2PlusName = "CPC02T";
  private const string Cpc4Name = "CPC04T";
  private const string Cpc3Name = "CPC302T";
  private const string Cpc5Name = "CPC501T";
  private const string Cpc502Name = "CPC502T";
  private const string Acm3Name = "ACM301T";
  private const int ColDocInlet = 0;
  private const int ColDocOutlet = 1;
  private const int ColDpfOutlet = 2;
  private const int ColScrInlet = 3;
  private const int ColScrOutlet = 4;
  private const float WidthHidden = 0.0f;
  private const float Width4Cols = 25f;
  private const float Width5Cols = 20f;
  private const int ReAlertTimer = 5000;
  private static SubjectCollection NGCProcedure = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_DisableHcDoserParkedRegen_NGC"
  });
  private static SubjectCollection GHG14Procedure = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_DisableHcDoserParkedRegen_MY13"
  });
  private static SubjectCollection CPC5Procedure = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_DisableHcDoserParkedRegen_CPC5"
  });
  private static SubjectCollection X45Procedure = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_DisableHcDoserParkedRegen_45X"
  });
  private static SubjectCollection EPA10Procedure = new SubjectCollection((IEnumerable<string>) new string[1]
  {
    "SP_DisableHcDoserParkedRegen_EPA10"
  });
  private Timer warningTimer = new Timer();
  private bool warningShowing = false;
  private bool instructionsShown = false;
  private Dictionary<string, double> maxTemperatures = new Dictionary<string, double>();
  private SharedProcedureBase selectedProcedure;
  private TableLayoutPanel tableLayoutPanel1;
  private DialInstrument dialInstrument1;
  private SeekTimeListView seekTimeListView1;
  private Button buttonStart;
  private BarInstrument barInstrumentDOCIntletTemperature;
  private BarInstrument barInstrumentDOCOutletTemperature;
  private BarInstrument barInstrumentDPFOutletTemperature;
  private BarInstrument barInstrumentSCRInletTemperature;
  private BarInstrument barInstrumentSCROutletTemperature;
  private TableLayoutPanel tableLayoutPanel2;
  private System.Windows.Forms.Label labelStatus;
  private Checkmark checkmark;
  private SharedProcedureSelection sharedProcedureSelection;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningTimer.Enabled = false;
    this.warningTimer.Interval = 5000;
    this.warningTimer.Tick += new EventHandler(this.warningTimer_Tick);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ProcedureStillRunningCannotCloseDialog);
      e.Cancel = true;
    }
    else if (this.InFaultState)
    {
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_TemperaturesAreTooHighCannotCloseDialog);
      e.Cancel = true;
    }
    if (e.Cancel)
      return;
    this.warningTimer.Enabled = false;
    this.warningTimer.Dispose();
    if (this.selectedProcedure != null)
    {
      this.selectedProcedure.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.selectedProcedure_StartComplete);
      this.selectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.selectedProcedure_StopComplete);
      this.selectedProcedure = (SharedProcedureBase) null;
    }
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateProcedure();
    this.UpdateDisplay();
  }

  private bool InFaultState
  {
    get
    {
      return this.barInstrumentDOCIntletTemperature.RepresentedState == 3 || this.barInstrumentDOCOutletTemperature.RepresentedState == 3 || this.barInstrumentDPFOutletTemperature.RepresentedState == 3 || this.barInstrumentSCRInletTemperature.RepresentedState == 3 || this.barInstrumentSCROutletTemperature.RepresentedState == 3;
    }
  }

  private void UpdateDisplay()
  {
    Channel channel = this.GetChannel("ACM301T", (CustomPanel.ChannelLookupOptions) 7);
    if (channel == null)
      return;
    bool flag = channel.Ecu.Name.Equals("ACM301T", StringComparison.InvariantCulture);
    float num1 = flag ? 25f : 20f;
    float num2 = flag ? 0.0f : 20f;
    ((Control) this.barInstrumentSCRInletTemperature).Visible = !flag;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles[0].Width = num1;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles[1].Width = num1;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles[2].Width = num1;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles[3].Width = num2;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles[4].Width = num1;
  }

  private void UpdateProcedure()
  {
    Channel channel = this.GetChannel("CPC302T", (CustomPanel.ChannelLookupOptions) 7);
    if (this.selectedProcedure != null)
    {
      this.selectedProcedure.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.selectedProcedure_StartComplete);
      this.selectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.selectedProcedure_StopComplete);
      this.selectedProcedure = (SharedProcedureBase) null;
    }
    if (channel == null)
      return;
    switch (channel.Ecu.Name)
    {
      case "CPC02T":
        this.sharedProcedureSelection.SharedProcedureQualifiers = UserPanel.EPA10Procedure;
        break;
      case "CPC04T":
        this.sharedProcedureSelection.SharedProcedureQualifiers = UserPanel.GHG14Procedure;
        break;
      case "CPC302T":
        this.sharedProcedureSelection.SharedProcedureQualifiers = UserPanel.NGCProcedure;
        break;
      case "CPC501T":
        this.sharedProcedureSelection.SharedProcedureQualifiers = UserPanel.CPC5Procedure;
        break;
      case "CPC502T":
        this.sharedProcedureSelection.SharedProcedureQualifiers = UserPanel.X45Procedure;
        break;
      default:
        this.sharedProcedureSelection.SharedProcedureQualifiers = UserPanel.NGCProcedure;
        break;
    }
    this.selectedProcedure = this.sharedProcedureSelection.SelectedProcedure;
    this.selectedProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.selectedProcedure_StartComplete);
    this.selectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.selectedProcedure_StopComplete);
  }

  private void ShowWarning()
  {
    if (this.warningShowing || this.warningTimer.Enabled)
      return;
    this.warningTimer.Enabled = false;
    this.warningShowing = true;
    int num = (int) MessageBox.Show(string.Format(Resources.Message_WarningText, (object) Environment.NewLine), Resources.Message_WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_HighTemperatureWarningDialogClosed);
    this.warningShowing = false;
    this.warningTimer.Enabled = this.InFaultState;
  }

  private void AddEventEntry()
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string key in this.maxTemperatures.Keys)
      dictionary[key] = this.maxTemperatures[key].ToString("0.##");
    Channel channel = this.GetChannel("ACM301T", (CustomPanel.ChannelLookupOptions) 7);
    ServerDataManager.UpdateEventsFile(channel, (IDictionary<string, string>) dictionary, "ATDDesaturation", string.Empty, SapiManager.GetVehicleIdentificationNumber(channel), "OK", string.Empty, string.Empty, false);
  }

  private void barInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (!this.InFaultState)
      return;
    this.ShowWarning();
  }

  private void warningTimer_Tick(object sender, EventArgs e)
  {
    this.warningTimer.Enabled = false;
    if (!this.InFaultState)
      return;
    this.ShowWarning();
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    if (this.instructionsShown)
      return;
    this.instructionsShown = true;
    int num = (int) MessageBox.Show(Resources.Message_InstructionText, Resources.Message_InstructionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InstructionsAcknowledged);
  }

  private void selectedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    this.maxTemperatures.Clear();
  }

  private void selectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    this.AddEventEntry();
  }

  private void barInstrument_DataChanged(object sender, EventArgs e)
  {
    if (!(sender is SingleInstrumentBase singleInstrumentBase) || singleInstrumentBase.DataItem == null || singleInstrumentBase.DataItem.Value == null)
      return;
    double num1 = singleInstrumentBase.DataItem.ValueAsDouble(singleInstrumentBase.DataItem.Value);
    Dictionary<string, double> maxTemperatures1 = this.maxTemperatures;
    Qualifier qualifier = singleInstrumentBase.DataItem.Qualifier;
    string name1 = ((Qualifier) ref qualifier).Name;
    if (maxTemperatures1.ContainsKey(name1))
    {
      Dictionary<string, double> maxTemperatures2 = this.maxTemperatures;
      qualifier = singleInstrumentBase.DataItem.Qualifier;
      string name2 = ((Qualifier) ref qualifier).Name;
      if (maxTemperatures2[name2] < num1)
      {
        Dictionary<string, double> maxTemperatures3 = this.maxTemperatures;
        qualifier = singleInstrumentBase.DataItem.Qualifier;
        string name3 = ((Qualifier) ref qualifier).Name;
        double num2 = num1;
        maxTemperatures3[name3] = num2;
      }
    }
    else
    {
      Dictionary<string, double> maxTemperatures4 = this.maxTemperatures;
      qualifier = singleInstrumentBase.DataItem.Qualifier;
      string name4 = ((Qualifier) ref qualifier).Name;
      double num3 = num1;
      maxTemperatures4.Add(name4, num3);
    }
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.dialInstrument1 = new DialInstrument();
    this.seekTimeListView1 = new SeekTimeListView();
    this.buttonStart = new Button();
    this.barInstrumentDOCIntletTemperature = new BarInstrument();
    this.barInstrumentDOCOutletTemperature = new BarInstrument();
    this.barInstrumentDPFOutletTemperature = new BarInstrument();
    this.barInstrumentSCRInletTemperature = new BarInstrument();
    this.barInstrumentSCROutletTemperature = new BarInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.labelStatus = new System.Windows.Forms.Label();
    this.checkmark = new Checkmark();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentDOCIntletTemperature, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentDOCOutletTemperature, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentDPFOutletTemperature, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentSCRInletTemperature, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentSCROutletTemperature, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.dialInstrument1.AngleRange = 220.0;
    this.dialInstrument1.AngleStart = -200.0;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dialInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.dialInstrument1, "dialInstrument1");
    this.dialInstrument1.FontGroup = "readouts";
    ((SingleInstrumentBase) this.dialInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.dialInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.dialInstrument1).Name = "dialInstrument1";
    ((AxisSingleInstrumentBase) this.dialInstrument1).PreferredAxisRange = new AxisRange(0.0, 2500.0, "rpm");
    ((SingleInstrumentBase) this.dialInstrument1).ShowValueReadout = false;
    ((SingleInstrumentBase) this.dialInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "ATDDesaturation";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    this.barInstrumentDOCIntletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentDOCIntletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentDOCIntletTemperature, "barInstrumentDOCIntletTemperature");
    this.barInstrumentDOCIntletTemperature.FontGroup = "thermometers";
    ((SingleInstrumentBase) this.barInstrumentDOCIntletTemperature).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentDOCIntletTemperature).Gradient.Initialize((ValueState) 1, 2, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentDOCIntletTemperature).Gradient.Modify(1, 200.0, (ValueState) 2);
    ((AxisSingleInstrumentBase) this.barInstrumentDOCIntletTemperature).Gradient.Modify(2, 500.0, (ValueState) 3);
    ((SingleInstrumentBase) this.barInstrumentDOCIntletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "DOCInletTemperature");
    ((Control) this.barInstrumentDOCIntletTemperature).Name = "barInstrumentDOCIntletTemperature";
    ((AxisSingleInstrumentBase) this.barInstrumentDOCIntletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
    ((SingleInstrumentBase) this.barInstrumentDOCIntletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentDOCIntletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentDOCIntletTemperature).UnitAlignment = StringAlignment.Near;
    this.barInstrumentDOCIntletTemperature.RepresentedStateChanged += new EventHandler(this.barInstrument_RepresentedStateChanged);
    ((SingleInstrumentBase) this.barInstrumentDOCIntletTemperature).DataChanged += new EventHandler(this.barInstrument_DataChanged);
    this.barInstrumentDOCOutletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentDOCOutletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentDOCOutletTemperature, "barInstrumentDOCOutletTemperature");
    this.barInstrumentDOCOutletTemperature.FontGroup = "thermometers";
    ((SingleInstrumentBase) this.barInstrumentDOCOutletTemperature).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentDOCOutletTemperature).Gradient.Initialize((ValueState) 1, 2, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentDOCOutletTemperature).Gradient.Modify(1, 200.0, (ValueState) 2);
    ((AxisSingleInstrumentBase) this.barInstrumentDOCOutletTemperature).Gradient.Modify(2, 500.0, (ValueState) 3);
    ((SingleInstrumentBase) this.barInstrumentDOCOutletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "DOCOutletTemperature");
    ((Control) this.barInstrumentDOCOutletTemperature).Name = "barInstrumentDOCOutletTemperature";
    ((AxisSingleInstrumentBase) this.barInstrumentDOCOutletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
    ((SingleInstrumentBase) this.barInstrumentDOCOutletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentDOCOutletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentDOCOutletTemperature).UnitAlignment = StringAlignment.Near;
    this.barInstrumentDOCOutletTemperature.RepresentedStateChanged += new EventHandler(this.barInstrument_RepresentedStateChanged);
    ((SingleInstrumentBase) this.barInstrumentDOCOutletTemperature).DataChanged += new EventHandler(this.barInstrument_DataChanged);
    this.barInstrumentDPFOutletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentDPFOutletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentDPFOutletTemperature, "barInstrumentDPFOutletTemperature");
    this.barInstrumentDPFOutletTemperature.FontGroup = "thermometers";
    ((SingleInstrumentBase) this.barInstrumentDPFOutletTemperature).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentDPFOutletTemperature).Gradient.Initialize((ValueState) 1, 2, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentDPFOutletTemperature).Gradient.Modify(1, 200.0, (ValueState) 2);
    ((AxisSingleInstrumentBase) this.barInstrumentDPFOutletTemperature).Gradient.Modify(2, 500.0, (ValueState) 3);
    ((SingleInstrumentBase) this.barInstrumentDPFOutletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "DPFOutletTemperature");
    ((Control) this.barInstrumentDPFOutletTemperature).Name = "barInstrumentDPFOutletTemperature";
    ((AxisSingleInstrumentBase) this.barInstrumentDPFOutletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
    ((SingleInstrumentBase) this.barInstrumentDPFOutletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentDPFOutletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentDPFOutletTemperature).UnitAlignment = StringAlignment.Near;
    this.barInstrumentDPFOutletTemperature.RepresentedStateChanged += new EventHandler(this.barInstrument_RepresentedStateChanged);
    ((SingleInstrumentBase) this.barInstrumentDPFOutletTemperature).DataChanged += new EventHandler(this.barInstrument_DataChanged);
    this.barInstrumentSCRInletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentSCRInletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentSCRInletTemperature, "barInstrumentSCRInletTemperature");
    this.barInstrumentSCRInletTemperature.FontGroup = "thermometers";
    ((SingleInstrumentBase) this.barInstrumentSCRInletTemperature).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentSCRInletTemperature).Gradient.Initialize((ValueState) 1, 2, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentSCRInletTemperature).Gradient.Modify(1, 200.0, (ValueState) 2);
    ((AxisSingleInstrumentBase) this.barInstrumentSCRInletTemperature).Gradient.Modify(2, 500.0, (ValueState) 3);
    ((SingleInstrumentBase) this.barInstrumentSCRInletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "SCRInletTemperature");
    ((Control) this.barInstrumentSCRInletTemperature).Name = "barInstrumentSCRInletTemperature";
    ((AxisSingleInstrumentBase) this.barInstrumentSCRInletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
    ((SingleInstrumentBase) this.barInstrumentSCRInletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentSCRInletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentSCRInletTemperature).UnitAlignment = StringAlignment.Near;
    this.barInstrumentSCRInletTemperature.RepresentedStateChanged += new EventHandler(this.barInstrument_RepresentedStateChanged);
    ((SingleInstrumentBase) this.barInstrumentSCRInletTemperature).DataChanged += new EventHandler(this.barInstrument_DataChanged);
    this.barInstrumentSCROutletTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentSCROutletTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentSCROutletTemperature, "barInstrumentSCROutletTemperature");
    this.barInstrumentSCROutletTemperature.FontGroup = "thermometers";
    ((SingleInstrumentBase) this.barInstrumentSCROutletTemperature).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentSCROutletTemperature).Gradient.Initialize((ValueState) 1, 2, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentSCROutletTemperature).Gradient.Modify(1, 200.0, (ValueState) 2);
    ((AxisSingleInstrumentBase) this.barInstrumentSCROutletTemperature).Gradient.Modify(2, 500.0, (ValueState) 3);
    ((SingleInstrumentBase) this.barInstrumentSCROutletTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "SCROutletTemperature");
    ((Control) this.barInstrumentSCROutletTemperature).Name = "barInstrumentSCROutletTemperature";
    ((AxisSingleInstrumentBase) this.barInstrumentSCROutletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
    ((SingleInstrumentBase) this.barInstrumentSCROutletTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentSCROutletTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentSCROutletTemperature).UnitAlignment = StringAlignment.Near;
    this.barInstrumentSCROutletTemperature.RepresentedStateChanged += new EventHandler(this.barInstrument_RepresentedStateChanged);
    ((SingleInstrumentBase) this.barInstrumentSCROutletTemperature).DataChanged += new EventHandler(this.barInstrument_DataChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection, 2, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark, "checkmark");
    ((Control) this.checkmark).Name = "checkmark";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DisableHcDoserParkedRegen_NGC"
    });
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmark;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
