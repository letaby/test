// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel.UserPanel
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
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel;

public class UserPanel : CustomPanel
{
  private Channel tcm = (Channel) null;
  private TableLayoutPanel tableLayoutPanel1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrumentCountershaftMaxSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DialInstrument dialInstrument1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private System.Windows.Forms.Label label1;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmark1;
  private TableLayoutPanel tableLayoutPanel3;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private TableLayoutPanel tableLayoutPanelOutput;
  private TableLayoutPanel tableLayoutPanelOutputMessage;
  private Checkmark checkmarkResults;
  private SeekTimeListView seekTimeListView1;
  private ScalingLabel scalingLabelResult;
  private DigitalReadoutInstrument digitalReadoutInstrumentCountershaftSpeed;
  private Button button1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    if (this.sharedProcedureSelection1.SelectedProcedure != null)
    {
      this.sharedProcedureSelection1.SelectedProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StartComplete);
      this.sharedProcedureSelection1.SelectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StopComplete);
    }
    this.DisplayResult(false);
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelection1.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    if (this.sharedProcedureSelection1.SelectedProcedure != null)
    {
      this.sharedProcedureSelection1.SelectedProcedure.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StartComplete);
      this.sharedProcedureSelection1.SelectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StopComplete);
    }
  }

  public virtual void OnChannelsChanged()
  {
    base.OnChannelsChanged();
    this.SetTcm(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7));
  }

  private void SetTcm(Channel tcm)
  {
    if (this.tcm == tcm)
      return;
    this.tcm = tcm;
    if (this.tcm != null)
    {
      foreach (SingleInstrumentBase singleInstrumentBase1 in CustomPanel.GetControlsOfType(((Control) this).Controls, typeof (SingleInstrumentBase)))
      {
        Qualifier instrument = singleInstrumentBase1.Instrument;
        Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier) ref instrument).Ecu);
        if (ecuByName != null && ecuByName.Identifier == tcm.Ecu.Identifier && ecuByName.Name != tcm.Ecu.Name)
        {
          SingleInstrumentBase singleInstrumentBase2 = singleInstrumentBase1;
          instrument = singleInstrumentBase1.Instrument;
          QualifierTypes type = ((Qualifier) ref instrument).Type;
          string name1 = tcm.Ecu.Name;
          instrument = singleInstrumentBase1.Instrument;
          string name2 = ((Qualifier) ref instrument).Name;
          Qualifier qualifier1 = new Qualifier(type, name1, name2);
          singleInstrumentBase2.Instrument = qualifier1;
          if (singleInstrumentBase1.DataItem == null)
          {
            QualifierTypes qualifierTypes1 = (QualifierTypes) 0;
            instrument = singleInstrumentBase1.Instrument;
            if (((Qualifier) ref instrument).Type == 1)
            {
              qualifierTypes1 = (QualifierTypes) 8;
            }
            else
            {
              instrument = singleInstrumentBase1.Instrument;
              if (((Qualifier) ref instrument).Type == 8)
                qualifierTypes1 = (QualifierTypes) 1;
            }
            SingleInstrumentBase singleInstrumentBase3 = singleInstrumentBase1;
            QualifierTypes qualifierTypes2 = qualifierTypes1;
            string name3 = tcm.Ecu.Name;
            instrument = singleInstrumentBase1.Instrument;
            string name4 = ((Qualifier) ref instrument).Name;
            Qualifier qualifier2 = new Qualifier(qualifierTypes2, name3, name4);
            singleInstrumentBase3.Instrument = qualifier2;
          }
        }
      }
      this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
      {
        "SP_CountershaftBrakeTest_" + this.tcm.Ecu.Name
      });
    }
  }

  private void SelectedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    this.DisplayResult(false);
  }

  private string GetInstrumentValue(SingleInstrumentBase instrument)
  {
    return instrument != null && instrument.DataItem != null && instrument.DataItem.Value != null ? $"{instrument.DataItem.ValueAsString(instrument.DataItem.Value)} {instrument.DataItem.Units}" : (string) null;
  }

  private void SelectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    if (this.seekTimeListView1.Text.EndsWith("Timeout\r\n", StringComparison.OrdinalIgnoreCase))
    {
      ((Control) this.scalingLabelResult).Text = Resources.MessageTimeout;
      this.checkmarkResults.CheckState = CheckState.Indeterminate;
      this.scalingLabelResult.RepresentedState = (ValueState) 2;
    }
    else
    {
      string instrumentValue = this.GetInstrumentValue((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftMaxSpeed);
      ((Control) this.scalingLabelResult).Text = $"{e.Result.ToString()}: {instrumentValue}" ?? Resources.MessageCounterShaftBrakeValueUnavailable;
      this.checkmarkResults.CheckState = e.Result == 1 ? CheckState.Checked : CheckState.Unchecked;
      this.scalingLabelResult.RepresentedState = e.Result == 1 ? (ValueState) 1 : (ValueState) 3;
    }
    this.DisplayResult(true);
  }

  private void DisplayResult(bool display)
  {
    ((Control) this.tableLayoutPanelOutputMessage).Visible = display;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.dialInstrument1 = new DialInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.button1 = new Button();
    this.checkmark1 = new Checkmark();
    this.label1 = new System.Windows.Forms.Label();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCountershaftMaxSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.tableLayoutPanelOutput = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanelOutputMessage = new TableLayoutPanel();
    this.scalingLabelResult = new ScalingLabel();
    this.checkmarkResults = new Checkmark();
    this.digitalReadoutInstrumentCountershaftSpeed = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanelOutput).SuspendLayout();
    ((Control) this.tableLayoutPanelOutputMessage).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCountershaftMaxSpeed, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelOutput, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCountershaftSpeed, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.dialInstrument1, "dialInstrument1");
    this.dialInstrument1.FontGroup = "dial";
    ((SingleInstrumentBase) this.dialInstrument1).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrument1).Gradient.Initialize((ValueState) 3, 4, "rpm");
    ((AxisSingleInstrumentBase) this.dialInstrument1).Gradient.Modify(1, 550.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrument1).Gradient.Modify(2, 650.0, (ValueState) 2);
    ((AxisSingleInstrumentBase) this.dialInstrument1).Gradient.Modify(3, 1900.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrument1).Gradient.Modify(4, 2000.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    ((Control) this.dialInstrument1).Name = "dialInstrument1";
    ((AxisSingleInstrumentBase) this.dialInstrument1).PreferredAxisRange = new AxisRange(500.0, 2000.0, "");
    ((SingleInstrumentBase) this.dialInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.button1, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label1, 2, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[2]
    {
      "SP_CountershaftBrakeTest_TCM01T",
      "SP_CountershaftBrakeTest_TCM05T"
    });
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument5, 2, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 1, 2);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 95.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd08_Istgang_Istgang");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel3).SetColumnSpan((Control) this.digitalReadoutInstrument4, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    this.digitalReadoutInstrument4.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument4.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrument4.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    this.digitalReadoutInstrument5.Gradient.Initialize((ValueState) 3, 1, "psi");
    this.digitalReadoutInstrument5.Gradient.Modify(1, 90.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCountershaftMaxSpeed, "digitalReadoutInstrumentCountershaftMaxSpeed");
    this.digitalReadoutInstrumentCountershaftMaxSpeed.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftMaxSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentCountershaftMaxSpeed.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentCountershaftMaxSpeed.Gradient.Modify(1, -10000.0, (ValueState) 1);
    this.digitalReadoutInstrumentCountershaftMaxSpeed.Gradient.Modify(2, -3000.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftMaxSpeed).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_231A_Maximaler_Gradient_Vorgelegewellen_Drehzahl_max_Gradient_Vorgelegewellen_Drehzahl");
    ((Control) this.digitalReadoutInstrumentCountershaftMaxSpeed).Name = "digitalReadoutInstrumentCountershaftMaxSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftMaxSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    this.digitalReadoutInstrument6.Gradient.Initialize((ValueState) 3, 8);
    this.digitalReadoutInstrument6.Gradient.Modify(1, -1.0, (ValueState) 3);
    this.digitalReadoutInstrument6.Gradient.Modify(2, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument6.Gradient.Modify(3, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument6.Gradient.Modify(4, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument6.Gradient.Modify(5, 3.0, (ValueState) 1);
    this.digitalReadoutInstrument6.Gradient.Modify(6, 4.0, (ValueState) 3);
    this.digitalReadoutInstrument6.Gradient.Modify(7, 5.0, (ValueState) 3);
    this.digitalReadoutInstrument6.Gradient.Modify(8, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelOutput, "tableLayoutPanelOutput");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelOutput, 2);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).Controls.Add((Control) this.seekTimeListView1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).Controls.Add((Control) this.tableLayoutPanelOutputMessage, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelOutput).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelOutput).Name = "tableLayoutPanelOutput";
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "CountershaftBrakeTest";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelOutputMessage, "tableLayoutPanelOutputMessage");
    ((TableLayoutPanel) this.tableLayoutPanelOutputMessage).Controls.Add((Control) this.scalingLabelResult, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelOutputMessage).Controls.Add((Control) this.checkmarkResults, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelOutputMessage).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelOutputMessage).Name = "tableLayoutPanelOutputMessage";
    this.scalingLabelResult.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelResult, "scalingLabelResult");
    this.scalingLabelResult.FontGroup = (string) null;
    this.scalingLabelResult.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelResult).Name = "scalingLabelResult";
    componentResourceManager.ApplyResources((object) this.checkmarkResults, "checkmarkResults");
    ((Control) this.checkmarkResults).Name = "checkmarkResults";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCountershaftSpeed, "digitalReadoutInstrumentCountershaftSpeed");
    this.digitalReadoutInstrumentCountershaftSpeed.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftSpeed).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle");
    ((Control) this.digitalReadoutInstrumentCountershaftSpeed).Name = "digitalReadoutInstrumentCountershaftSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCountershaftSpeed).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.label1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.button1;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_CountershaftBrakeTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanelOutput).ResumeLayout(false);
    ((Control) this.tableLayoutPanelOutputMessage).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
