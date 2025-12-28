// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Clutch_Learn_Values_Reset.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clutch_Learn_Values_Reset.panel;

public class UserPanel : CustomPanel
{
  private Channel tcm = (Channel) null;
  private StringBuilder message = new StringBuilder();
  private bool canStart = false;
  private bool isBusy = false;
  private bool procedureHasBeenRun = false;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrumentNoShifting;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineStandstill;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleStandStill;
  private DigitalReadoutInstrument digitalReadoutInstrumentClutchMinimum;
  private Checkmark checkmarkStatus;
  private TableLayoutPanel tableLayoutPanel2;
  private TableLayoutPanel tableLayoutPanel3;
  private CheckBox checkBoxClutchWasReplaced;
  private DigitalReadoutInstrument digitalReadoutInstrumentNoLearningProcedure;
  private DigitalReadoutInstrument digitalReadoutInstrumentNeutral;
  private TextBox textBoxStartConditionStatus;
  private TableLayoutPanel tableLayoutPanel4;
  private DigitalReadoutInstrument digitalReadoutInstrumentClutchMaximum;
  private DigitalReadoutInstrument digitalReadoutInstrumentClutchFacingWearActualValue;
  private DigitalReadoutInstrument digitalReadoutInstrumentClutchFacingRemainingThickness;
  private RunServicesButton runServiceButtonStart;
  private SeekTimeListView seekTimeListView1;

  private bool TcmOnline => this.tcm != null && this.tcm.Online;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!((RunSharedProcedureButtonBase) this.runServiceButtonStart).IsBusy)
      return;
    e.Cancel = true;
  }

  private void SetTcm(Channel tcm)
  {
    if (this.tcm == tcm)
      return;
    this.checkBoxClutchWasReplaced.Checked = false;
    this.tcm = tcm;
  }

  public virtual void OnChannelsChanged()
  {
    base.OnChannelsChanged();
    this.SetTcm(this.GetChannel("TCM05T"));
    this.UpdateUserInterface();
  }

  private void UpdateLog(string logMessage)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, logMessage);
  }

  private void UpdateCannotStartMessage(string condition)
  {
    if (this.canStart)
      this.message.AppendLine(this.procedureHasBeenRun ? Resources.MessageTheProcedureCannotBeRunAgain : Resources.MessageTheProcedureCannotStart);
    else
      this.message.AppendLine(",");
    this.message.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, " {0}", (object) condition);
    this.canStart = false;
  }

  private void UpdateUserInterface()
  {
    this.message.Clear();
    if (this.isBusy)
    {
      this.message.Append(Resources.MessageTheProcedureIsRunning);
    }
    else
    {
      this.canStart = this.TcmOnline;
      if (!this.canStart)
      {
        this.message.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) Resources.MessageTheProcedureCannotStart, (object) Resources.ConditionTcmOnline);
      }
      else
      {
        if (this.digitalReadoutInstrumentVehicleStandStill.RepresentedState != 1)
          this.UpdateCannotStartMessage(Resources.ConditionVehicleStandstill);
        if (this.digitalReadoutInstrumentParkBrake.RepresentedState != 1)
          this.UpdateCannotStartMessage(Resources.ConditionParkingBrakeSet);
        if (this.digitalReadoutInstrumentEngineStandstill.RepresentedState != 1)
          this.UpdateCannotStartMessage(Resources.ConditionEngineStopped);
        if (this.digitalReadoutInstrumentNoShifting.RepresentedState != 1)
          this.UpdateCannotStartMessage(Resources.ConditionNoShifting);
        if (this.digitalReadoutInstrumentNeutral.RepresentedState != 1)
          this.UpdateCannotStartMessage(Resources.ConditionTransmissionInNeutral);
        if (this.digitalReadoutInstrumentNoLearningProcedure.RepresentedState != 1)
          this.UpdateCannotStartMessage(Resources.ConditionNoLearningProcess);
        if (!this.checkBoxClutchWasReplaced.Checked)
          this.UpdateCannotStartMessage(Resources.ConditionClutchReplacedChecked);
        if (this.canStart)
          this.message.Append(this.procedureHasBeenRun ? Resources.MessageTheProcedureHasAlreadyBeenRun : Resources.MessageTheProcedureCanStart);
      }
    }
    this.message.Append(".");
    ((Control) this.runServiceButtonStart).Enabled = this.canStart && !this.isBusy;
    this.checkmarkStatus.Checked = this.canStart && !this.isBusy;
    this.textBoxStartConditionStatus.Text = this.message.ToString();
  }

  private void digitalReadoutInstrumentVehicleStandStill_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentEngineStandstill_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentNoShifting_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentNeutral_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentNoLearningProcedure_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentRepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void checkBoxClutchWasReplaced_MouseClick(object sender, MouseEventArgs e)
  {
    if (this.checkBoxClutchWasReplaced.Checked)
      this.UpdateLog(Resources.MessageClutchReplacedChecked);
    this.UpdateUserInterface();
  }

  private void UpdateLogFileWithNewClutchInstrumentValues()
  {
    this.UpdateLog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageNewClutchMinimumValue, ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMinimum).DataItem == null || ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMinimum).DataItem.Value == null ? (object) Resources.MessageValueNotAvailable : ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMinimum).DataItem.Value));
    this.UpdateLog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageNewClutchMaximumValue, ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMaximum).DataItem == null || ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMaximum).DataItem.Value == null ? (object) Resources.MessageValueNotAvailable : ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMaximum).DataItem.Value));
    this.UpdateLog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageNewClutchFacingWearActualValue, ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingWearActualValue).DataItem == null || ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingWearActualValue).DataItem.Value == null ? (object) Resources.MessageValueNotAvailable : ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingWearActualValue).DataItem.Value));
    this.UpdateLog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageNewClutchFacingRemainingThickness, ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingRemainingThickness).DataItem == null || ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingRemainingThickness).DataItem.Value == null ? (object) Resources.MessageValueNotAvailable : ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingRemainingThickness).DataItem.Value));
  }

  private void runServiceButtonStart_Complete(object sender, PassFailResultEventArgs e)
  {
    this.isBusy = false;
    this.procedureHasBeenRun = true;
    this.checkBoxClutchWasReplaced.Checked = false;
    if (((ResultEventArgs) e).Succeeded)
    {
      this.UpdateLog(Resources.MessageTheProcedureRanSuccessfully);
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}\r\n\r\n1. {1}\r\n2. {2}\r\n3. {3}\r\n4. {4}", (object) Resources.MessageRunTheTCMLearnProcedurePart1, (object) Resources.MessageRunTheTCMLearnProcedurePart2, (object) Resources.MessageRunTheTCMLearnProcedurePart3, (object) Resources.MessageRunTheTCMLearnProcedurePart4, (object) Resources.MessageRunTheTCMLearnProcedurePart5);
      int num = (int) MessageBox.Show(str, Resources.CaptionRunTheTCMLearnProcedure, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification, false);
      this.UpdateLog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageTcmOnline, this.tcm != null ? (object) this.tcm.CommunicationsState.ToString() : (object) Resources.MessageOffline));
      this.UpdateLogFileWithNewClutchInstrumentValues();
      this.UpdateLog(str);
    }
    else
    {
      this.UpdateLog(Resources.MessageTheProcedureFailed);
      if (((ResultEventArgs) e).Exception != null)
        this.UpdateLog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageErrorReturned, (object) ((ResultEventArgs) e).Exception.Message));
    }
    this.UpdateUserInterface();
  }

  private void runServiceButtonStart_Click(object sender, EventArgs e)
  {
    this.isBusy = true;
    this.UpdateLog(Resources.MessageTheProcedureStarted);
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmarkStatus = new Checkmark();
    this.textBoxStartConditionStatus = new TextBox();
    this.digitalReadoutInstrumentClutchMinimum = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentClutchMaximum = new DigitalReadoutInstrument();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.checkBoxClutchWasReplaced = new CheckBox();
    this.runServiceButtonStart = new RunServicesButton();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentNoShifting = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleStandStill = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineStandstill = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentNeutral = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentNoLearningProcedure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentClutchFacingWearActualValue = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentClutchFacingRemainingThickness = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((ISupportInitialize) this.runServiceButtonStart).BeginInit();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentClutchMinimum, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentClutchMaximum, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel4, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentClutchFacingWearActualValue, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentClutchFacingRemainingThickness, 1, 5);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "ClutchLearnValuesReset";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textBoxStartConditionStatus, 1, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    this.textBoxStartConditionStatus.BorderStyle = BorderStyle.None;
    componentResourceManager.ApplyResources((object) this.textBoxStartConditionStatus, "textBoxStartConditionStatus");
    this.textBoxStartConditionStatus.Name = "textBoxStartConditionStatus";
    this.textBoxStartConditionStatus.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentClutchMinimum, "digitalReadoutInstrumentClutchMinimum");
    this.digitalReadoutInstrumentClutchMinimum.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMinimum).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMinimum).Instrument = new Qualifier((QualifierTypes) 8, "TCM05T", "DT_STO_2316_Clutch_Minimum_learned_value_Clutch_Minimum_learned_value");
    ((Control) this.digitalReadoutInstrumentClutchMinimum).Name = "digitalReadoutInstrumentClutchMinimum";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMinimum).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentClutchMaximum, "digitalReadoutInstrumentClutchMaximum");
    this.digitalReadoutInstrumentClutchMaximum.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMaximum).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMaximum).Instrument = new Qualifier((QualifierTypes) 8, "TCM05T", "DT_STO_2317_Clutch_Maximum_learned_value_Clutch_Maximum_learned_value");
    ((Control) this.digitalReadoutInstrumentClutchMaximum).Name = "digitalReadoutInstrumentClutchMaximum";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchMaximum).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkBoxClutchWasReplaced, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.runServiceButtonStart, 1, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.checkBoxClutchWasReplaced, "checkBoxClutchWasReplaced");
    this.checkBoxClutchWasReplaced.Name = "checkBoxClutchWasReplaced";
    this.checkBoxClutchWasReplaced.UseVisualStyleBackColor = true;
    this.checkBoxClutchWasReplaced.MouseClick += new MouseEventHandler(this.checkBoxClutchWasReplaced_MouseClick);
    componentResourceManager.ApplyResources((object) this.runServiceButtonStart, "runServiceButtonStart");
    ((Control) this.runServiceButtonStart).Name = "runServiceButtonStart";
    this.runServiceButtonStart.ServiceCalls.Add(new ServiceCall("TCM05T", "RT_0528_Reset_Clutch_learned_values_Start", (IEnumerable<string>) new string[1]
    {
      "Reset_Clutch_learned_values=4"
    }));
    this.runServiceButtonStart.ServiceCalls.Add(new ServiceCall("TCM05T", "DL_B101_Clutch_replacement_Actual_clutch_facing_wear", (IEnumerable<string>) new string[1]
    {
      "Clutch_replacement_Actual_clutch_facing_wear=0"
    }));
    this.runServiceButtonStart.ServiceCalls.Add(new ServiceCall("TCM05T", "FN_HardReset"));
    this.runServiceButtonStart.Complete += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonStart_Complete);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentNoShifting, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentVehicleStandStill, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentEngineStandstill, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentNeutral, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentNoLearningProcedure, 2, 1);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel4, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNoShifting, "digitalReadoutInstrumentNoShifting");
    this.digitalReadoutInstrumentNoShifting.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNoShifting).FreezeValue = false;
    this.digitalReadoutInstrumentNoShifting.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentNoShifting.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentNoShifting.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNoShifting).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_No_gearshift_active");
    ((Control) this.digitalReadoutInstrumentNoShifting).Name = "digitalReadoutInstrumentNoShifting";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNoShifting).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentNoShifting.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentNoShifting_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleStandStill, "digitalReadoutInstrumentVehicleStandStill");
    this.digitalReadoutInstrumentVehicleStandStill.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStandStill).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleStandStill.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentVehicleStandStill.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleStandStill.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStandStill).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Vehicle_standstill");
    ((Control) this.digitalReadoutInstrumentVehicleStandStill).Name = "digitalReadoutInstrumentVehicleStandStill";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStandStill).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleStandStill.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleStandStill_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineStandstill, "digitalReadoutInstrumentEngineStandstill");
    this.digitalReadoutInstrumentEngineStandstill.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStandstill).FreezeValue = false;
    this.digitalReadoutInstrumentEngineStandstill.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentEngineStandstill.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineStandstill.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStandstill).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Engine_standstill");
    ((Control) this.digitalReadoutInstrumentEngineStandstill).Name = "digitalReadoutInstrumentEngineStandstill";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineStandstill).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineStandstill.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentEngineStandstill_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Park_brake_activated");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentParkBrake_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNeutral, "digitalReadoutInstrumentNeutral");
    this.digitalReadoutInstrumentNeutral.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNeutral).FreezeValue = false;
    this.digitalReadoutInstrumentNeutral.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentNeutral.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentNeutral.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNeutral).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Transmission_in_neutral");
    ((Control) this.digitalReadoutInstrumentNeutral).Name = "digitalReadoutInstrumentNeutral";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNeutral).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentNeutral.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentNeutral_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNoLearningProcedure, "digitalReadoutInstrumentNoLearningProcedure");
    this.digitalReadoutInstrumentNoLearningProcedure.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNoLearningProcedure).FreezeValue = false;
    this.digitalReadoutInstrumentNoLearningProcedure.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentNoLearningProcedure.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentNoLearningProcedure.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNoLearningProcedure).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_No_learn_procedure_active");
    ((Control) this.digitalReadoutInstrumentNoLearningProcedure).Name = "digitalReadoutInstrumentNoLearningProcedure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNoLearningProcedure).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentNoLearningProcedure.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentNoLearningProcedure_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentClutchFacingWearActualValue, "digitalReadoutInstrumentClutchFacingWearActualValue");
    this.digitalReadoutInstrumentClutchFacingWearActualValue.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingWearActualValue).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingWearActualValue).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_2651_Clutch_facing_data_Clutch_facing_wear_Actual_value");
    ((Control) this.digitalReadoutInstrumentClutchFacingWearActualValue).Name = "digitalReadoutInstrumentClutchFacingWearActualValue";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingWearActualValue).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentClutchFacingRemainingThickness, "digitalReadoutInstrumentClutchFacingRemainingThickness");
    this.digitalReadoutInstrumentClutchFacingRemainingThickness.FontGroup = "Main";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingRemainingThickness).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingRemainingThickness).Instrument = new Qualifier((QualifierTypes) 1, "TCM05T", "DT_2651_Clutch_facing_data_Clutch_facing_Remaining_thickness");
    ((Control) this.digitalReadoutInstrumentClutchFacingRemainingThickness).Name = "digitalReadoutInstrumentClutchFacingRemainingThickness";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentClutchFacingRemainingThickness).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((ISupportInitialize) this.runServiceButtonStart).EndInit();
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
