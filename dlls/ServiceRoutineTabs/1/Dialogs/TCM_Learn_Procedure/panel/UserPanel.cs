// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel.UserPanel
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
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel;

public class UserPanel : CustomPanel
{
  private Channel tcmChannel = (Channel) null;
  private Checkmark startCheckmark;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private System.Windows.Forms.Label results;
  private TableLayoutPanel tableLayoutPanel2;
  private Button Start;
  private SharedProcedureSelection sharedProcedureSelection;
  private ComboBox comboBoxType;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineStartReq;
  private DigitalReadoutInstrument digitalReadoutInstrumentIgnitionReq;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.comboBoxType.SelectedIndex = 1;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelection.AnyProcedureInProgress)
    {
      e.Cancel = true;
    }
    else
    {
      this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.OnParentFormClosing);
      this.SetTcm((Channel) null);
      if (((SingleInstrumentBase) this.digitalReadoutInstrument5).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrument5).DataItem.Value != null)
        ((Control) this).Tag = (object) new object[2]
        {
          (object) (((Choice) ((SingleInstrumentBase) this.digitalReadoutInstrument5).DataItem.Value).Index == 0),
          (object) ((SingleInstrumentBase) this.digitalReadoutInstrument5).DataItem.Value.ToString()
        };
      else
        ((Control) this).Tag = (object) new object[2]
        {
          (object) false,
          (object) Resources.Message_ProcedureWasNotRun
        };
    }
  }

  public virtual void OnChannelsChanged()
  {
    this.SetTcm(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7));
  }

  private void SetTcm(Channel tcm)
  {
    if (this.tcmChannel == tcm)
      return;
    this.tcmChannel = tcm;
    if (tcm != null)
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
          Qualifier qualifier = new Qualifier(type, name1, name2);
          singleInstrumentBase2.Instrument = qualifier;
        }
      }
    }
  }

  private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
  {
    string str = this.sharedProcedureSelection.SharedProcedureQualifiers.ToString();
    if (str.Contains("("))
      str = str.Substring(0, str.IndexOf("("));
    try
    {
      str += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) (this.comboBoxType.SelectedIndex + 1));
    }
    catch (FormatException ex)
    {
      int num = (int) MessageBox.Show(Resources.Message_PleaseEnterANumericValue, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, (MessageBoxOptions) 0);
    }
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      str
    });
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrumentIgnitionReq = new DigitalReadoutInstrument();
    this.startCheckmark = new Checkmark();
    this.seekTimeListView1 = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.results = new System.Windows.Forms.Label();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.Start = new Button();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.comboBoxType = new ComboBox();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineStartReq = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentIgnitionReq, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.startCheckmark, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.results, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentEngineStartReq, 0, 4);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentIgnitionReq, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentIgnitionReq, "digitalReadoutInstrumentIgnitionReq");
    this.digitalReadoutInstrumentIgnitionReq.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionReq).FreezeValue = false;
    this.digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentIgnitionReq.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentIgnitionReq.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.digitalReadoutInstrumentIgnitionReq.Gradient.Modify(2, 1.0, (ValueState) 4);
    this.digitalReadoutInstrumentIgnitionReq.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionReq).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "IgnitionStatusRequest");
    ((Control) this.digitalReadoutInstrumentIgnitionReq).Name = "digitalReadoutInstrumentIgnitionReq";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionReq).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionReq).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionReq).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.startCheckmark, "startCheckmark");
    ((Control) this.startCheckmark).Name = "startCheckmark";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "TCM Learn";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "TCMLearn_instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 1, 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "TCMLearn_statuses";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_231B_Status_Einlernen_Kupplung_Status_Einlernen_Kupplung");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "TCMLearn_instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 3, 1, "psi");
    this.digitalReadoutInstrument2.Gradient.Modify(1, 90.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.results, "results");
    this.results.Name = "results";
    this.results.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.Start, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.comboBoxType, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.Start, "Start");
    this.Start.Name = "Start";
    this.Start.UseCompatibleTextRendering = true;
    this.Start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_TCM_Learn"
    });
    componentResourceManager.ApplyResources((object) this.comboBoxType, "comboBoxType");
    this.comboBoxType.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxType.FormattingEnabled = true;
    this.comboBoxType.Items.AddRange(new object[2]
    {
      (object) componentResourceManager.GetString("comboBoxType.Items"),
      (object) componentResourceManager.GetString("comboBoxType.Items1")
    });
    this.comboBoxType.Name = "comboBoxType";
    this.comboBoxType.SelectedIndexChanged += new EventHandler(this.comboBoxType_SelectedIndexChanged);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "TCMLearn_statuses";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2111_Status_Einlernen_Getriebe_stGbLrn");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument5, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "TCMLearn_statuses";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "TCM01T", "RT_0400_Einlernvorgang_Service_Request_Results_Fehler_Lernvorgang");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentEngineStartReq, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineStartReq, "digitalReadoutInstrumentEngineStartReq");
    this.digitalReadoutInstrumentEngineStartReq.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStartReq).FreezeValue = false;
    this.digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentEngineStartReq.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentEngineStartReq.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineStartReq.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentEngineStartReq.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineStartReq.Gradient.Modify(4, (double) byte.MaxValue, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStartReq).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2112_Anforderung_zum_Motorstart_waehrend_des_Einlernvorgangs_Anforderung_Motorstart");
    ((Control) this.digitalReadoutInstrumentEngineStartReq).Name = "digitalReadoutInstrumentEngineStartReq";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStartReq).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStartReq).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStartReq).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.results;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.startCheckmark;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.Start;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_TransmissionLearnProcedure");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
