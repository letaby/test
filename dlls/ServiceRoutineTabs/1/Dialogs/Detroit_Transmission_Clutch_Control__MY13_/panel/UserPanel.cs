// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

public class UserPanel : CustomPanel
{
  private WarningManager warningManager;
  private Channel tcm;
  private OuterTest outerTest = (OuterTest) null;
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private BarInstrument barInstrumentClutchDesiredValue;
  private BarInstrument barInstrumentClutchDisplacement;
  private SeekTimeListView seekTimeListViewLog;
  private TableLayoutPanel tableLayoutPanelTestControls;
  private Checkmark checkmarkTestStatus;
  private System.Windows.Forms.Label scalingLabelTestStatus;
  private Button buttonStartTest;
  private TimerControl timerControl1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheckStatus;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private BarInstrument barInstrumentAirSupplyPressure;
  private BarInstrument barInstrumentCounterShaftSpeed;
  private BarInstrument barInstrumentActualEngineSpeed;
  private Checkmark checkmarkTestResult1;
  private Checkmark checkmarkTestResult2;
  private Checkmark checkmarkTestResult3;
  private ScalingLabel scalingLabelTestResult1;
  private ScalingLabel scalingLabelTestResult2;
  private ScalingLabel scalingLabelTestResult3;
  private TableLayoutPanel tableLayoutPanel3;
  private System.Windows.Forms.Label labelDirections;
  private Button buttonStopTest;
  private TableLayoutPanel tableLayoutPanelResults;
  private System.Windows.Forms.Label labelAdditionalInfo;
  private DigitalReadoutInstrument digitalReadoutInstrumentGearEngaged;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.OnParentFormClosing);
    this.warningManager = new WarningManager(string.Empty, (string) null, this.seekTimeListViewLog.RequiredUserLabelPrefix);
    this.outerTest = new OuterTest(new Action<string>(this.LogText), new Action<string>(this.DisplayDirections), new Action<int, TestResults, string>(this.UpdateTestResults), this.barInstrumentClutchDisplacement, this.barInstrumentCounterShaftSpeed, this.barInstrumentActualEngineSpeed, this.timerControl1);
    this.buttonStartTest.Click += new EventHandler(this.buttonStartTest_Click);
    this.buttonStopTest.Click += new EventHandler(this.buttonStopTest_Click);
    this.barInstrumentAirSupplyPressure.RepresentedStateChanged += new EventHandler(this.TestConditionChanged);
    this.barInstrumentActualEngineSpeed.RepresentedStateChanged += new EventHandler(this.TestConditionChanged);
    this.digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += new EventHandler(this.TestConditionChanged);
    this.digitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += new EventHandler(this.TestConditionChanged);
    this.digitalReadoutInstrumentGearEngaged.RepresentedStateChanged += new EventHandler(this.TestConditionChanged);
  }

  protected virtual void OnLoad(EventArgs e) => __nonvirtual (((UserControl) this).OnLoad(e));

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.OnParentFormClosing);
    this.SetTCM((Channel) null);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) false,
      (object) string.Empty
    };
  }

  private bool CanClose => this.outerTest == null || !this.outerTest.OuterTestIsRunning;

  public virtual void OnChannelsChanged()
  {
    this.SetTCM(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 5));
    if (this.tcm != null || this.outerTest == null || !this.outerTest.OuterTestIsRunning)
      return;
    this.StopTest();
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.SetTCM((Channel) null);
    this.outerTest.Dispose(disposing);
  }

  private void SetTCM(Channel tcm)
  {
    if (this.tcm == tcm)
      return;
    if (this.tcm != null)
      this.tcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.tcm = tcm;
    if (this.tcm != null)
    {
      this.tcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
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
    }
  }

  private void StopTest() => this.outerTest.StopTest();

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListViewLog.RequiredUserLabelPrefix, text);
  }

  private void DisplayDirections(string directions) => this.labelDirections.Text = directions;

  private void TestConditionChanged(object sender, EventArgs e) => this.UpdateControlState();

  public void UpdateTestResults(int testNumber, TestResults results, string errorString)
  {
    string str = string.Empty;
    CheckState checkState = CheckState.Indeterminate;
    switch (results)
    {
      case TestResults.Fail:
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNFailed, (object) testNumber);
        checkState = CheckState.Unchecked;
        break;
      case TestResults.Success:
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNPassed, (object) testNumber);
        checkState = CheckState.Checked;
        break;
      case TestResults.StopTest:
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNStopped, (object) testNumber);
        checkState = CheckState.Unchecked;
        break;
      case TestResults.Error:
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ErrorAnErrorOccurred, (object) testNumber, (object) errorString);
        break;
      case TestResults.NotRun:
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNNotRun, (object) testNumber);
        break;
    }
    Checkmark checkmark = (Checkmark) null;
    ScalingLabel scalingLabel = (ScalingLabel) null;
    switch (testNumber)
    {
      case 1:
        checkmark = this.checkmarkTestResult1;
        scalingLabel = this.scalingLabelTestResult1;
        break;
      case 2:
        checkmark = this.checkmarkTestResult2;
        scalingLabel = this.scalingLabelTestResult2;
        break;
      case 3:
        checkmark = this.checkmarkTestResult3;
        scalingLabel = this.scalingLabelTestResult3;
        break;
    }
    ((Control) scalingLabel).Text = str;
    checkmark.CheckState = checkState;
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateControlState();
  }

  private void UpdateControlState()
  {
    bool flag = true;
    StringBuilder stringBuilder = new StringBuilder();
    this.labelAdditionalInfo.Text = string.Empty;
    if (!this.outerTest.OuterTestIsRunning)
    {
      if (this.barInstrumentAirSupplyPressure.RepresentedState != 1)
      {
        flag = false;
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TransmissionAirSupplyPressureTooLow, (object) ((SingleInstrumentBase) this.barInstrumentAirSupplyPressure).Title));
      }
      if (this.digitalReadoutInstrumentGearEngaged.RepresentedState != 1)
      {
        flag = false;
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestStateTransmissionInGear, (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentGearEngaged).Title));
      }
      if (this.digitalReadoutInstrumentVehicleCheckStatus.RepresentedState != 1)
      {
        flag = false;
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.VehicleCheckStatusFalse, (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentGearEngaged).Title));
      }
      if (this.digitalReadoutInstrumentParkingBrake.RepresentedState != 1)
      {
        flag = false;
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestStatusParkingBrakeOff, (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Title));
      }
      if (this.barInstrumentActualEngineSpeed.RepresentedState != 1)
      {
        flag = false;
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestStatusEngineRunning, (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Title));
      }
      this.checkmarkTestStatus.Checked = flag;
      this.scalingLabelTestStatus.Text = flag ? Resources.TestStateTestCanStart : Resources.TestStateTestCannotStart;
      this.labelAdditionalInfo.Text = stringBuilder.ToString();
    }
    else
      this.scalingLabelTestStatus.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestStateTestNIsRunning, (object) this.outerTest.TestNumber);
    this.buttonStartTest.Enabled = flag && !this.outerTest.OuterTestIsRunning;
    this.buttonStopTest.Enabled = this.outerTest.OuterTestIsRunning;
  }

  private void buttonStartTest_Click(object sender, EventArgs e)
  {
    this.UpdateTestResults(1, TestResults.NotRun, string.Empty);
    this.UpdateTestResults(2, TestResults.NotRun, string.Empty);
    this.UpdateTestResults(3, TestResults.NotRun, string.Empty);
    this.outerTest.StartTest(this.tcm);
  }

  private void buttonStopTest_Click(object sender, EventArgs e) => this.StopTest();

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelTestControls = new TableLayoutPanel();
    this.buttonStartTest = new Button();
    this.checkmarkTestStatus = new Checkmark();
    this.buttonStopTest = new Button();
    this.scalingLabelTestStatus = new System.Windows.Forms.Label();
    this.timerControl1 = new TimerControl();
    this.labelAdditionalInfo = new System.Windows.Forms.Label();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.digitalReadoutInstrumentGearEngaged = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.tableLayoutPanelResults = new TableLayoutPanel();
    this.scalingLabelTestResult3 = new ScalingLabel();
    this.scalingLabelTestResult2 = new ScalingLabel();
    this.scalingLabelTestResult1 = new ScalingLabel();
    this.checkmarkTestResult3 = new Checkmark();
    this.checkmarkTestResult1 = new Checkmark();
    this.checkmarkTestResult2 = new Checkmark();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.barInstrumentClutchDesiredValue = new BarInstrument();
    this.barInstrumentClutchDisplacement = new BarInstrument();
    this.labelDirections = new System.Windows.Forms.Label();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.barInstrumentAirSupplyPressure = new BarInstrument();
    this.barInstrumentCounterShaftSpeed = new BarInstrument();
    this.barInstrumentActualEngineSpeed = new BarInstrument();
    ((Control) this.tableLayoutPanelTestControls).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanelResults).SuspendLayout();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTestControls, "tableLayoutPanelTestControls");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanelTestControls, 3);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.buttonStartTest, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.checkmarkTestStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.buttonStopTest, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.scalingLabelTestStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.timerControl1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.labelAdditionalInfo, 0, 1);
    ((Control) this.tableLayoutPanelTestControls).Name = "tableLayoutPanelTestControls";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetRowSpan((Control) this.tableLayoutPanelTestControls, 2);
    componentResourceManager.ApplyResources((object) this.buttonStartTest, "buttonStartTest");
    this.buttonStartTest.Name = "buttonStartTest";
    this.buttonStartTest.UseCompatibleTextRendering = true;
    this.buttonStartTest.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.checkmarkTestStatus, "checkmarkTestStatus");
    ((Control) this.checkmarkTestStatus).Name = "checkmarkTestStatus";
    componentResourceManager.ApplyResources((object) this.buttonStopTest, "buttonStopTest");
    this.buttonStopTest.Name = "buttonStopTest";
    this.buttonStopTest.UseCompatibleTextRendering = true;
    this.buttonStopTest.UseVisualStyleBackColor = true;
    this.scalingLabelTestStatus.BackColor = SystemColors.Info;
    componentResourceManager.ApplyResources((object) this.scalingLabelTestStatus, "scalingLabelTestStatus");
    this.scalingLabelTestStatus.Name = "scalingLabelTestStatus";
    this.scalingLabelTestStatus.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.timerControl1, 2);
    componentResourceManager.ApplyResources((object) this.timerControl1, "timerControl1");
    this.timerControl1.Duration = TimeSpan.Parse("00:01:00");
    this.timerControl1.FontGroup = (string) null;
    ((Control) this.timerControl1).Name = "timerControl1";
    this.timerControl1.TimerCountdownCompletedDisplayMessage = (string) null;
    this.labelAdditionalInfo.BackColor = SystemColors.Info;
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.labelAdditionalInfo, 2);
    componentResourceManager.ApplyResources((object) this.labelAdditionalInfo, "labelAdditionalInfo");
    this.labelAdditionalInfo.Name = "labelAdditionalInfo";
    this.labelAdditionalInfo.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentGearEngaged, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentVehicleCheckStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 0, 2);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetRowSpan((Control) this.tableLayoutPanel3, 5);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentGearEngaged, "digitalReadoutInstrumentGearEngaged");
    this.digitalReadoutInstrumentGearEngaged.FontGroup = "bars";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGearEngaged).FreezeValue = false;
    this.digitalReadoutInstrumentGearEngaged.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentGearEngaged.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentGearEngaged.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGearEngaged).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd08_Istgang_Istgang");
    ((Control) this.digitalReadoutInstrumentGearEngaged).Name = "digitalReadoutInstrumentGearEngaged";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentGearEngaged).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleCheckStatus, "digitalReadoutInstrumentVehicleCheckStatus");
    this.digitalReadoutInstrumentVehicleCheckStatus.FontGroup = "bars";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrumentVehicleCheckStatus).Name = "digitalReadoutInstrumentVehicleCheckStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelResults, "tableLayoutPanelResults");
    ((TableLayoutPanel) this.tableLayoutPanelResults).Controls.Add((Control) this.scalingLabelTestResult3, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelResults).Controls.Add((Control) this.scalingLabelTestResult2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelResults).Controls.Add((Control) this.scalingLabelTestResult1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelResults).Controls.Add((Control) this.checkmarkTestResult3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelResults).Controls.Add((Control) this.checkmarkTestResult1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelResults).Controls.Add((Control) this.checkmarkTestResult2, 0, 1);
    ((Control) this.tableLayoutPanelResults).Name = "tableLayoutPanelResults";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetRowSpan((Control) this.tableLayoutPanelResults, 2);
    this.scalingLabelTestResult3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelTestResult3, "scalingLabelTestResult3");
    this.scalingLabelTestResult3.FontGroup = "TestResults";
    this.scalingLabelTestResult3.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelTestResult3).Name = "scalingLabelTestResult3";
    this.scalingLabelTestResult2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelTestResult2, "scalingLabelTestResult2");
    this.scalingLabelTestResult2.FontGroup = "TestResults";
    this.scalingLabelTestResult2.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelTestResult2).Name = "scalingLabelTestResult2";
    this.scalingLabelTestResult1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelTestResult1, "scalingLabelTestResult1");
    this.scalingLabelTestResult1.FontGroup = "TestResults";
    this.scalingLabelTestResult1.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelTestResult1).Name = "scalingLabelTestResult1";
    this.checkmarkTestResult3.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkTestResult3, "checkmarkTestResult3");
    ((Control) this.checkmarkTestResult3).Name = "checkmarkTestResult3";
    this.checkmarkTestResult1.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkTestResult1, "checkmarkTestResult1");
    ((Control) this.checkmarkTestResult1).Name = "checkmarkTestResult1";
    this.checkmarkTestResult2.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkTestResult2, "checkmarkTestResult2");
    ((Control) this.checkmarkTestResult2).Name = "checkmarkTestResult2";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentClutchDesiredValue, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentClutchDisplacement, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelDirections, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelTestControls, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentAirSupplyPressure, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentCounterShaftSpeed, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.barInstrumentActualEngineSpeed, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanel3, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelResults, 3, 6);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentClutchDesiredValue, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentClutchDesiredValue, "barInstrumentClutchDesiredValue");
    this.barInstrumentClutchDesiredValue.FontGroup = "bars";
    ((SingleInstrumentBase) this.barInstrumentClutchDesiredValue).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentClutchDesiredValue).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2314_Kupplungssollwert_Kupplungssollwert");
    ((Control) this.barInstrumentClutchDesiredValue).Name = "barInstrumentClutchDesiredValue";
    ((SingleInstrumentBase) this.barInstrumentClutchDesiredValue).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentClutchDisplacement, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentClutchDisplacement, "barInstrumentClutchDisplacement");
    this.barInstrumentClutchDisplacement.FontGroup = "bars";
    ((SingleInstrumentBase) this.barInstrumentClutchDisplacement).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentClutchDisplacement).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung");
    ((Control) this.barInstrumentClutchDisplacement).Name = "barInstrumentClutchDisplacement";
    ((SingleInstrumentBase) this.barInstrumentClutchDisplacement).UnitAlignment = StringAlignment.Near;
    this.labelDirections.BackColor = SystemColors.Info;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.labelDirections, 4);
    componentResourceManager.ApplyResources((object) this.labelDirections, "labelDirections");
    this.labelDirections.Name = "labelDirections";
    this.labelDirections.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.seekTimeListViewLog, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "DetroitTransmissionClutchControl";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetRowSpan((Control) this.seekTimeListViewLog, 3);
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentAirSupplyPressure, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentAirSupplyPressure, "barInstrumentAirSupplyPressure");
    this.barInstrumentAirSupplyPressure.FontGroup = "bars";
    ((SingleInstrumentBase) this.barInstrumentAirSupplyPressure).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentAirSupplyPressure).Gradient.Initialize((ValueState) 3, 1, "psi");
    ((AxisSingleInstrumentBase) this.barInstrumentAirSupplyPressure).Gradient.Modify(1, 100.0, (ValueState) 1);
    ((SingleInstrumentBase) this.barInstrumentAirSupplyPressure).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
    ((Control) this.barInstrumentAirSupplyPressure).Name = "barInstrumentAirSupplyPressure";
    ((SingleInstrumentBase) this.barInstrumentAirSupplyPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentCounterShaftSpeed, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentCounterShaftSpeed, "barInstrumentCounterShaftSpeed");
    this.barInstrumentCounterShaftSpeed.FontGroup = "bars";
    ((SingleInstrumentBase) this.barInstrumentCounterShaftSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentCounterShaftSpeed).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle");
    ((Control) this.barInstrumentCounterShaftSpeed).Name = "barInstrumentCounterShaftSpeed";
    ((AxisSingleInstrumentBase) this.barInstrumentCounterShaftSpeed).PreferredAxisRange = new AxisRange(0.0, 100.0, (string) null);
    ((SingleInstrumentBase) this.barInstrumentCounterShaftSpeed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.barInstrumentActualEngineSpeed, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentActualEngineSpeed, "barInstrumentActualEngineSpeed");
    this.barInstrumentActualEngineSpeed.FontGroup = "bars";
    ((SingleInstrumentBase) this.barInstrumentActualEngineSpeed).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentActualEngineSpeed).Gradient.Initialize((ValueState) 0, 0, "rpm");
    ((SingleInstrumentBase) this.barInstrumentActualEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.barInstrumentActualEngineSpeed).Name = "barInstrumentActualEngineSpeed";
    ((AxisSingleInstrumentBase) this.barInstrumentActualEngineSpeed).PreferredAxisRange = new AxisRange(0.0, 2000.0, (string) null);
    ((SingleInstrumentBase) this.barInstrumentActualEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelTestControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanelResults).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
