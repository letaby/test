// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel;

public class UserPanel : CustomPanel
{
  private const string defaultValue = "0";
  private bool canClose = true;
  private Channel tcm;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument rangeActuatorPosition;
  private Checkmark checkmark1;
  private System.Windows.Forms.Label messageTarget;
  private SharedProcedureSelection sharedProcedureSelection1;
  private ComboBox activationComboBoxMode;
  private DigitalReadoutInstrument splitActuatorPosition;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private ComboBox splitRangeEnterBox;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private Button startButton;
  private DecimalTextBox gearEnterBox;
  private Panel panel1;
  private DigitalReadoutInstrument gearEngagedDigitalReadout;
  private Button buttonReturnControl;
  private TableLayoutPanel tableLayoutPanel2;

  public UserPanel()
  {
    this.InitializeComponent();
    ((Control) this.gearEnterBox).TextChanged += new EventHandler(this.gearEnterBox_TextChanged);
    this.splitRangeEnterBox.TextChanged += new EventHandler(this.splitRangeEnterBox_TextChanged);
    this.sharedProcedureSelection1.StatusReport += new EventHandler<StatusReportEventArgs>(this.sharedProcedureSelection1_StatusReport);
  }

  private void sharedProcedureSelection1_StatusReport(object sender, StatusReportEventArgs e)
  {
    SharedProcedureSelection procedureSelection = sender as SharedProcedureSelection;
    if (procedureSelection.SelectedProcedure.CanStart)
    {
      if (procedureSelection.SelectedProcedure.Result == 1)
      {
        this.canClose = false;
        if (this.activationComboBoxMode.SelectedIndex == 0 && string.Compare(((Control) this.gearEnterBox).Text, "0", StringComparison.OrdinalIgnoreCase) == 0)
          this.canClose = true;
      }
    }
    else
      this.canClose = true;
    this.buttonReturnControl.Enabled = !this.canClose;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    this.activationComboBoxMode.SelectedIndex = 0;
    ((Control) this.gearEnterBox).Text = "0";
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
        "SP_TCM_Gear_Split_Range_Select_" + this.tcm.Ecu.Name
      });
    }
  }

  private void SetGearSplitRangeQualifierValue()
  {
    string str = this.sharedProcedureSelection1.SharedProcedureQualifiers.ToString();
    if (str.Contains("("))
      str = str.Substring(0, str.IndexOf("("));
    int num1 = 0;
    int num2 = 2;
    int num3 = 2;
    switch (this.activationComboBoxMode.SelectedIndex)
    {
      case 0:
        num1 = (int) this.gearEnterBox.Value;
        if (num1 == 0)
        {
          num3 = (int) this.CurrentRange;
          break;
        }
        break;
      case 1:
        num2 = this.splitRangeEnterBox.SelectedIndex;
        num3 = (int) this.CurrentRange;
        break;
      case 2:
        num3 = this.splitRangeEnterBox.SelectedIndex;
        break;
      default:
        throw new IndexOutOfRangeException();
    }
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1},{2})", (object) num1, (object) num2, (object) num3)
    });
  }

  private UserPanel.RangeValue CurrentRange
  {
    get
    {
      UserPanel.RangeValue currentRange = UserPanel.RangeValue.Other;
      if (((SingleInstrumentBase) this.rangeActuatorPosition).DataItem != null && ((SingleInstrumentBase) this.rangeActuatorPosition).DataItem.Value is InstrumentValue instrumentValue)
      {
        Choice choice = instrumentValue.Value as Choice;
        if (choice != (object) null)
        {
          switch (choice.RawValue.ToString())
          {
            case "0":
              currentRange = UserPanel.RangeValue.Low;
              break;
            case "2":
              currentRange = UserPanel.RangeValue.High;
              break;
            default:
              currentRange = UserPanel.RangeValue.Other;
              break;
          }
        }
      }
      return currentRange;
    }
  }

  private void activationComboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.SetActivationComboBoxModeControl();
  }

  private void SetActivationComboBoxModeControl()
  {
    switch (this.activationComboBoxMode.SelectedIndex)
    {
      case 0:
        ((Control) this.gearEnterBox).Visible = true;
        this.splitRangeEnterBox.Visible = false;
        ((Control) this.gearEnterBox).Text = "0";
        break;
      case 1:
        ((Control) this.gearEnterBox).Visible = false;
        this.splitRangeEnterBox.Visible = true;
        this.splitRangeEnterBox.Items.Clear();
        this.splitRangeEnterBox.Items.AddRange(new object[2]
        {
          (object) UserPanel.SplitValue.Low,
          (object) UserPanel.SplitValue.High
        });
        this.splitRangeEnterBox.SelectedIndex = 0;
        break;
      case 2:
        ((Control) this.gearEnterBox).Visible = false;
        this.splitRangeEnterBox.Visible = true;
        this.splitRangeEnterBox.Items.Clear();
        this.splitRangeEnterBox.Items.AddRange(new object[2]
        {
          (object) UserPanel.RangeValue.Low,
          (object) UserPanel.RangeValue.High
        });
        this.splitRangeEnterBox.SelectedIndex = 0;
        break;
      default:
        throw new IndexOutOfRangeException();
    }
    this.SetGearSplitRangeQualifierValue();
  }

  private void gearEnterBox_TextChanged(object sender, EventArgs e)
  {
    this.SetGearSplitRangeQualifierValue();
  }

  private void splitRangeEnterBox_TextChanged(object sender, EventArgs e)
  {
    this.SetGearSplitRangeQualifierValue();
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelection1.AnyProcedureInProgress)
      e.Cancel = true;
    else if (!this.canClose)
    {
      e.Cancel = true;
      int num = (int) MessageBox.Show(Resources.Message_CanNotExitUntilControlIsReturnedToTheVehicle, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.None);
    }
    if (e.Cancel)
      return;
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    ((Control) this.gearEnterBox).TextChanged -= new EventHandler(this.gearEnterBox_TextChanged);
    this.splitRangeEnterBox.TextChanged -= new EventHandler(this.splitRangeEnterBox_TextChanged);
    this.sharedProcedureSelection1.StatusReport -= new EventHandler<StatusReportEventArgs>(this.sharedProcedureSelection1_StatusReport);
  }

  private void buttonReturnControl_Click(object sender, EventArgs e)
  {
    this.activationComboBoxMode.SelectedIndex = 0;
    this.gearEnterBox.Value = 0.0;
    this.SetGearSplitRangeQualifierValue();
    this.sharedProcedureSelection1.StartSelectedProcedure();
  }

  private void gearEnterBox_Leave(object sender, EventArgs e)
  {
    if (!string.IsNullOrEmpty(((Control) this.gearEnterBox).Text))
      return;
    ((Control) this.gearEnterBox).Text = "0";
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmark1 = new Checkmark();
    this.messageTarget = new System.Windows.Forms.Label();
    this.startButton = new Button();
    this.activationComboBoxMode = new ComboBox();
    this.panel1 = new Panel();
    this.gearEnterBox = new DecimalTextBox();
    this.splitRangeEnterBox = new ComboBox();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.buttonReturnControl = new Button();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.splitActuatorPosition = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.rangeActuatorPosition = new DigitalReadoutInstrument();
    this.gearEngagedDigitalReadout = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    this.panel1.SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.messageTarget, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.startButton, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.activationComboBoxMode, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.panel1, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonReturnControl, 5, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.messageTarget, 4);
    componentResourceManager.ApplyResources((object) this.messageTarget, "messageTarget");
    this.messageTarget.Name = "messageTarget";
    this.messageTarget.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.startButton, "startButton");
    this.startButton.Name = "startButton";
    this.startButton.UseCompatibleTextRendering = true;
    this.startButton.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.activationComboBoxMode, 2);
    componentResourceManager.ApplyResources((object) this.activationComboBoxMode, "activationComboBoxMode");
    this.activationComboBoxMode.DropDownStyle = ComboBoxStyle.DropDownList;
    this.activationComboBoxMode.FormattingEnabled = true;
    this.activationComboBoxMode.Items.AddRange(new object[3]
    {
      (object) componentResourceManager.GetString("activationComboBoxMode.Items"),
      (object) componentResourceManager.GetString("activationComboBoxMode.Items1"),
      (object) componentResourceManager.GetString("activationComboBoxMode.Items2")
    });
    this.activationComboBoxMode.Name = "activationComboBoxMode";
    this.activationComboBoxMode.SelectedIndexChanged += new EventHandler(this.activationComboBoxMode_SelectedIndexChanged);
    this.panel1.Controls.Add((Control) this.gearEnterBox);
    this.panel1.Controls.Add((Control) this.splitRangeEnterBox);
    componentResourceManager.ApplyResources((object) this.panel1, "panel1");
    this.panel1.Name = "panel1";
    componentResourceManager.ApplyResources((object) this.gearEnterBox, "gearEnterBox");
    this.gearEnterBox.MaximumValue = 12.0;
    this.gearEnterBox.MinimumValue = 0.0;
    ((Control) this.gearEnterBox).Name = "gearEnterBox";
    this.gearEnterBox.Precision = new int?(0);
    this.gearEnterBox.Value = 0.0;
    ((Control) this.gearEnterBox).Leave += new EventHandler(this.gearEnterBox_Leave);
    componentResourceManager.ApplyResources((object) this.splitRangeEnterBox, "splitRangeEnterBox");
    this.splitRangeEnterBox.DropDownStyle = ComboBoxStyle.DropDownList;
    this.splitRangeEnterBox.FormattingEnabled = true;
    this.splitRangeEnterBox.Name = "splitRangeEnterBox";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[2]
    {
      "SP_TCM_Gear_Split_Range_Select_TCM01T",
      "SP_TCM_Gear_Split_Range_Select_TCM05T"
    });
    componentResourceManager.ApplyResources((object) this.buttonReturnControl, "buttonReturnControl");
    this.buttonReturnControl.Name = "buttonReturnControl";
    this.buttonReturnControl.UseCompatibleTextRendering = true;
    this.buttonReturnControl.UseVisualStyleBackColor = true;
    this.buttonReturnControl.Click += new EventHandler(this.buttonReturnControl_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.splitActuatorPosition, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.rangeActuatorPosition, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.gearEngagedDigitalReadout, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "Gear Split Range Select Activation";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.splitActuatorPosition, "splitActuatorPosition");
    this.splitActuatorPosition.FontGroup = "";
    ((SingleInstrumentBase) this.splitActuatorPosition).FreezeValue = false;
    this.splitActuatorPosition.Gradient.Initialize((ValueState) 0, 5);
    this.splitActuatorPosition.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.splitActuatorPosition.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.splitActuatorPosition.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.splitActuatorPosition.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.splitActuatorPosition.Gradient.Modify(5, (double) byte.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.splitActuatorPosition).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2306_Aktuator_Stellung_Split_Aktuator_Stellung_Split");
    ((Control) this.splitActuatorPosition).Name = "splitActuatorPosition";
    ((SingleInstrumentBase) this.splitActuatorPosition).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 8, "TCM01T", "CO_TransType");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 655.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    this.digitalReadoutInstrument4.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument4.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrument4.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.rangeActuatorPosition, 2);
    componentResourceManager.ApplyResources((object) this.rangeActuatorPosition, "rangeActuatorPosition");
    this.rangeActuatorPosition.FontGroup = "";
    ((SingleInstrumentBase) this.rangeActuatorPosition).FreezeValue = false;
    this.rangeActuatorPosition.Gradient.Initialize((ValueState) 0, 5);
    this.rangeActuatorPosition.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.rangeActuatorPosition.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.rangeActuatorPosition.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.rangeActuatorPosition.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.rangeActuatorPosition.Gradient.Modify(5, (double) byte.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.rangeActuatorPosition).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2309_Aktuator_Stellung_Range_Aktuator_Stellung_Range");
    ((Control) this.rangeActuatorPosition).Name = "rangeActuatorPosition";
    ((SingleInstrumentBase) this.rangeActuatorPosition).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.gearEngagedDigitalReadout, "gearEngagedDigitalReadout");
    this.gearEngagedDigitalReadout.FontGroup = "";
    ((SingleInstrumentBase) this.gearEngagedDigitalReadout).FreezeValue = false;
    this.gearEngagedDigitalReadout.Gradient.Initialize((ValueState) 1, 1);
    this.gearEngagedDigitalReadout.Gradient.Modify(1, 1.0, (ValueState) 0);
    ((SingleInstrumentBase) this.gearEngagedDigitalReadout).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd08_Istgang_Istgang");
    ((Control) this.gearEngagedDigitalReadout).Name = "gearEngagedDigitalReadout";
    ((SingleInstrumentBase) this.gearEngagedDigitalReadout).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.messageTarget;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.startButton;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_TransmissionGearSplitRangeActivation");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    this.panel1.ResumeLayout(false);
    this.panel1.PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  public enum ActivationMode
  {
    Gear,
    Split,
    Range,
  }

  public enum SplitValue
  {
    Low,
    High,
  }

  public enum RangeValue
  {
    Low,
    High,
    Other,
  }
}
