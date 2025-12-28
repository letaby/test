// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Calibration.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Calibration.panel;

public class UserPanel : CustomPanel
{
  private Channel xmc02t;
  private Channel hsv;
  private static string FrontAxleLiftingInstrument = "DT_1739";
  private static string RearAxleLiftingInstrument = "DT_1756";
  private static string FrontAxleLoweringInstrument = "DT_1740";
  private static string RearAxleLoweringInstrument = "DT_1755";
  private static string HsvName = "HSV";
  private static string Xmc02tName = "XMC02T";
  private static string MoveServiceQualifier = "IOC_RHC_OutputCtrl_Control(DiagRqData_OC_NomLvlRqFAx_Enbl={0},Nominal_Level_Request_Front_Axle={2},DiagRqData_OC_NomLvlRqRAx={2},DiagRqData_OC_NomLvlRqRAx_Enbl={1},DiagRqData_OC_LvlCtrlMd_Rq={3},Level_Control_Mode_Request=1)";
  private static string HsvHighestLvlQualifier = "RT_HIGHESTLVL";
  private static string HsvNomLvl1Qualifier = "RT_NOMLVL1";
  private static string HsvNomLvl2Qualifier = "RT_NOMLVL2";
  private static string HsvLowestLvlQualifier = "RT_LOESTLVL";
  private Timer timer;
  private Timer lastStepFinishedTimer;
  private DateTime lastMoved;
  private Dictionary<UserPanel.Level, string> hsvServices = new Dictionary<UserPanel.Level, string>();
  private Timer hsvCalibrateTimer;
  private string hsvServiceQualifier = string.Empty;
  private readonly UserPanel.ProcessState[] userStates = new UserPanel.ProcessState[8]
  {
    UserPanel.ProcessState.NotRunning,
    UserPanel.ProcessState.Starting,
    UserPanel.ProcessState.Step1,
    UserPanel.ProcessState.Step2,
    UserPanel.ProcessState.Step3,
    UserPanel.ProcessState.Step4,
    UserPanel.ProcessState.Step5,
    UserPanel.ProcessState.Complete
  };
  private bool popupShown = false;
  private bool aborted = false;
  private bool working = false;
  private bool lastStepFinished = false;
  private UserPanel.ProcessState state;
  private bool hasFrontAxel = true;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private TableLayoutPanel tableLayoutPanelMain;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrumentAirPressure;
  private CheckBox checkBoxTruckOnStand;
  private CheckBox checkBoxFullSetOfBlocks;
  private CheckBox checkBoxNoOneAround;
  private TableLayoutPanel tableLayoutPanel1;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label3;
  private ScalingLabel scalingLabelRearAxleStatus;
  private TableLayoutPanel tableLayoutPanel5;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label20;
  private ScalingLabel scalingLabelFrontAxleStatus;
  private TextBox textBoxInstructions;
  private System.Windows.Forms.Label label1;
  private TableLayoutPanel tableLayoutPanelControls;
  private Button buttonClose;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmarkStatus;
  private System.Windows.Forms.Label labelStatus;
  private Button buttonStartNext;
  private Button buttonAbort;
  private TabControl tabControl;
  private TabPage tabPageAxles;
  private PictureBox pictureBoxAxles;
  private TabPage tabPageFrontAxle;
  private PictureBox pictureBoxFrontAxle;
  private TabPage tabPageCenterAxle;
  private PictureBox pictureBoxCenterAxle;
  private DigitalReadoutInstrument digitalReadoutInstrument3;

  private bool HasFrontAxel => this.hasFrontAxel;

  public UserPanel()
  {
    this.InitializeComponent();
    this.UpdateUI();
    this.timer = new Timer();
    this.timer.Interval = 1000;
    this.timer.Tick += new EventHandler(this.UpdateUI);
    this.timer.Start();
    this.lastMoved = DateTime.Now.AddSeconds(-5.0);
    this.lastStepFinishedTimer = new Timer();
    this.lastStepFinishedTimer.Interval = 10000;
    this.lastStepFinishedTimer.Tick += new EventHandler(this.LastStepFinished);
    this.hsvCalibrateTimer = new Timer();
    this.hsvCalibrateTimer.Interval = 100;
    this.hsvCalibrateTimer.Tick += new EventHandler(this.hsvCalibrateTimer_Tick);
    this.hsvServices.Add(UserPanel.Level.Normal1, UserPanel.HsvNomLvl1Qualifier);
    this.hsvServices.Add(UserPanel.Level.Normal2, UserPanel.HsvNomLvl2Qualifier);
    this.hsvServices.Add(UserPanel.Level.Upper, UserPanel.HsvHighestLvlQualifier);
    this.hsvServices.Add(UserPanel.Level.Lowest, UserPanel.HsvLowestLvlQualifier);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    this.timer.Stop();
    this.timer.Tick -= new EventHandler(this.UpdateUI);
    this.lastStepFinishedTimer.Stop();
    this.lastStepFinishedTimer.Tick -= new EventHandler(this.LastStepFinished);
    this.StopHsvService();
    this.hsvCalibrateTimer.Tick -= new EventHandler(this.hsvCalibrateTimer_Tick);
    if (this.working)
      this.Abort("Form Closed");
    if (this.xmc02t != null)
      this.xmc02t.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
    this.MoveService(false, false, UserPanel.Level.None, UserPanel.ServiceMode.Normal);
    this.SetChannelXmc02t((Channel) null);
    this.SetChannelHsv((Channel) null);
  }

  private void UpdateUI(object sender, EventArgs e) => this.UpdateUI();

  private void LastStepFinished(object sender, EventArgs e)
  {
    this.lastStepFinishedTimer.Stop();
    this.lastStepFinished = true;
    this.UpdateUI();
  }

  private void Log(string message) => this.LabelLog("HSVCalibrate", message);

  private void UpdateInstructions(string step, string message, string instructions)
  {
    this.textBoxInstructions.Text = step + Environment.NewLine + message + Environment.NewLine + instructions;
    this.labelStatus.Text = instructions;
    this.Log(message);
  }

  private void UpdateInstructionsWithSeparateMessage(
    string step,
    string message,
    string instructions)
  {
    this.textBoxInstructions.Text = step + Environment.NewLine + message;
    this.labelStatus.Text = instructions;
    this.Log(message);
  }

  private void GoMachine()
  {
    if (!this.aborted)
    {
      this.working = true;
      ++this.state;
      switch (this.state)
      {
        case UserPanel.ProcessState.Starting:
          this.lastStepFinished = false;
          this.hasFrontAxel = this.scalingLabelFrontAxleStatus.RepresentedState == 1 || this.scalingLabelFrontAxleStatus.RepresentedState == 2;
          this.UpdateInstructions(Resources.Message_WARNING, Resources.Message_CalibrationProcedure, Resources.Message_ToBeginClickNext);
          if (!this.popupShown)
          {
            this.popupShown = true;
            int num = (int) MessageBox.Show(Resources.Message_WARNING1, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            break;
          }
          break;
        case UserPanel.ProcessState.StartingOnNext:
          this.MoveService(this.HasFrontAxel, true, UserPanel.Level.None, UserPanel.ServiceMode.Normal);
          break;
        case UserPanel.ProcessState.Step1:
          this.UpdateInstructions(Resources.Message_Step1, Resources.Message_RaiseTruckToMaximumHeightThenAutomaticallyDrop14, Resources.Message_ClickNextToRaiseTruck);
          break;
        case UserPanel.ProcessState.Step1OnNext:
          this.lastMoved = DateTime.Now;
          this.MoveService(this.HasFrontAxel, true, UserPanel.Level.Upper, UserPanel.ServiceMode.Calibration);
          break;
        case UserPanel.ProcessState.Step2:
          this.UpdateInstructions(Resources.Message_Step2, "", Resources.Message_TruckIsCurrentlyRaising);
          break;
        case UserPanel.ProcessState.Step2OnNext:
          this.lastMoved = DateTime.Now;
          this.MoveService(this.HasFrontAxel, true, UserPanel.Level.Normal1, UserPanel.ServiceMode.Calibration);
          break;
        case UserPanel.ProcessState.Step3:
          this.UpdateInstructions(Resources.Message_Step3, "", Resources.Message_TruckIsCurrentlyLowering);
          break;
        case UserPanel.ProcessState.Step3OnNext:
          this.lastMoved = DateTime.Now;
          this.MoveService(this.HasFrontAxel, true, UserPanel.Level.Normal2, UserPanel.ServiceMode.Calibration);
          break;
        case UserPanel.ProcessState.Step4:
          this.UpdateInstructions(Resources.Message_Step4, "", Resources.Message_TruckIsCurrentlyLowering);
          break;
        case UserPanel.ProcessState.Step4OnNext:
          this.lastMoved = DateTime.Now;
          this.MoveService(this.HasFrontAxel, true, UserPanel.Level.Lowest, UserPanel.ServiceMode.Calibration);
          break;
        case UserPanel.ProcessState.Step5:
          this.UpdateInstructionsWithSeparateMessage(Resources.Message_Step5, Resources.Message_TruckIsCurrentlyLoweringStep5Instructions, Resources.Message_TruckIsCurrentlyLowering);
          this.lastStepFinishedTimer.Start();
          break;
        case UserPanel.ProcessState.Step5OnNext:
          this.StopHsvService();
          this.lastMoved = DateTime.Now;
          this.MoveService(false, false, UserPanel.Level.None, UserPanel.ServiceMode.Normal);
          break;
        case UserPanel.ProcessState.Complete:
          this.StopHsvService();
          this.UpdateInstructions(Resources.Message_Step6, Resources.Message_HeightValuesHaveBeenStored, Resources.Message_CalibrationCompleted);
          this.working = false;
          break;
      }
    }
    this.UpdateUI();
  }

  private void channel_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.aborted)
      return;
    if (e.Succeeded)
    {
      if (this.xmc02t != null)
        this.GoMachine();
      else
        this.Abort(Resources.Message_EcuDisconnectedBeforeCompletion);
    }
    else
      this.Abort(e.Exception.Message);
  }

  private void Abort(string reason)
  {
    this.StopHsvService();
    this.state = UserPanel.ProcessState.Complete;
    this.aborted = true;
    this.working = false;
    this.UpdateInstructions(Resources.Message_Abort, reason, Resources.Message_Aborted);
    this.MoveService(true, true, UserPanel.Level.Normal1, UserPanel.ServiceMode.Normal);
    this.UpdateUI();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannelXmc02t(this.GetChannel(UserPanel.Xmc02tName, (CustomPanel.ChannelLookupOptions) 3));
    this.SetChannelHsv(this.GetChannel(UserPanel.HsvName, (CustomPanel.ChannelLookupOptions) 3));
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    this.checkBoxTruckOnStand.Enabled = this.checkBoxFullSetOfBlocks.Enabled = this.checkBoxNoOneAround.Enabled = !this.working;
    this.buttonStartNext.Text = this.working ? Resources.Message_Next : Resources.Message_StartCalibration;
    this.buttonAbort.Enabled = this.working;
    this.checkmarkStatus.Checked = this.state != UserPanel.ProcessState.Step5 ? (this.buttonStartNext.Enabled = this.working ? (DateTime.Now - this.lastMoved).Seconds >= 5 : this.CanStart()) : (this.buttonStartNext.Enabled = this.working ? this.lastStepFinished : this.CanStart());
    if (this.buttonStartNext.Enabled)
    {
      switch (this.state)
      {
        case UserPanel.ProcessState.Step2:
          this.UpdateInstructions(Resources.Message_Step2, Resources.Message_TruckHasBeenRaised, Resources.Message_OnceBlocksAreInPlaceClickNextTruckWillBeginToLower);
          break;
        case UserPanel.ProcessState.Step3:
          this.UpdateInstructions(Resources.Message_Step3, Resources.Message_TruckHasBeenLowered, Resources.Message_OnceAeroHeightGaugeBlocksAreInPlaceClickNextToLowerTheTruck);
          break;
        case UserPanel.ProcessState.Step4:
          this.UpdateInstructions(Resources.Message_Step4, Resources.Message_TruckHasBeenLoweredRemoveAeroGaugeBlocks, Resources.Message_OnceBlocksAreRemovedClickNextTruckWillBeginToLower);
          break;
        case UserPanel.ProcessState.Step5:
          this.UpdateInstructionsWithSeparateMessage(Resources.Message_Step5, Resources.Message_TruckIsCurrentlyLoweringStep5Instructions, Resources.Message_ClickNextToReturnToStandardHeight);
          break;
      }
    }
    this.UpdateScalingLabelStatus(this.scalingLabelFrontAxleStatus, UserPanel.FrontAxleLiftingInstrument, UserPanel.FrontAxleLoweringInstrument);
    this.UpdateScalingLabelStatus(this.scalingLabelRearAxleStatus, UserPanel.RearAxleLiftingInstrument, UserPanel.RearAxleLoweringInstrument);
  }

  private bool CanStart()
  {
    if (this.xmc02t == null || !this.xmc02t.Online)
    {
      this.labelStatus.Text = Resources.Message_XMC02TOffline;
      return false;
    }
    if (!this.checkBoxTruckOnStand.Checked)
    {
      this.labelStatus.Text = Resources.Message_ViewCalibrationBlockPhotos;
      return false;
    }
    if (!this.checkBoxFullSetOfBlocks.Checked)
    {
      this.labelStatus.Text = Resources.Message_ConfirmThatFullSetOfHeightGaugeBlocksAvailable;
      return false;
    }
    if (!this.checkBoxNoOneAround.Checked)
    {
      this.labelStatus.Text = Resources.Message_ConfirmThatNoOneIsWorkingOnOrNearTheTruck;
      return false;
    }
    if (this.digitalReadoutInstrumentAirPressure.RepresentedState != 1)
    {
      this.labelStatus.Text = Resources.Message_AirPressureInvalid;
      return false;
    }
    if (this.digitalReadoutInstrumentParkingBrake.RepresentedState != 1)
    {
      this.labelStatus.Text = Resources.Message_ParkingBrakeInvalid;
      return false;
    }
    if (this.digitalReadoutInstrumentEngineSpeed.RepresentedState != 1)
    {
      this.labelStatus.Text = Resources.Message_EngineSpeedInvalid;
      return false;
    }
    this.labelStatus.Text = Resources.Message_ReadyToStart;
    return true;
  }

  private ValueState UpdateScalingLabelStatus(
    ScalingLabel scalingLabel,
    string liftingQualifier,
    string loweringQualifier)
  {
    Choice choice1 = this.ReadChoiceValue(this.hsv, liftingQualifier);
    Choice choice2 = this.ReadChoiceValue(this.hsv, loweringQualifier);
    if (choice1 == (object) null || choice1.Index == 2 || choice1.Index == 3 || choice2 == (object) null || choice2.Index == 2 || choice2.Index == 3)
    {
      ((Control) scalingLabel).Text = Resources.Message_SNA;
      scalingLabel.RepresentedState = (ValueState) 3;
    }
    else if (choice1.Index == 1)
    {
      this.lastMoved = DateTime.Now;
      ((Control) scalingLabel).Text = Resources.Message_Raising;
      scalingLabel.RepresentedState = (ValueState) 2;
    }
    else if (choice2.Index == 1)
    {
      this.lastMoved = DateTime.Now;
      ((Control) scalingLabel).Text = Resources.Message_Lowering;
      scalingLabel.RepresentedState = (ValueState) 2;
    }
    else
    {
      ((Control) scalingLabel).Text = Resources.Message_Stationary;
      scalingLabel.RepresentedState = (ValueState) 1;
    }
    return scalingLabel.RepresentedState;
  }

  private Choice ReadChoiceValue(Channel channel, string qualifier)
  {
    return channel != null && channel.Instruments != null && channel.Instruments[qualifier] != (Instrument) null && channel.Instruments[qualifier].InstrumentValues != null && channel.Instruments[qualifier].InstrumentValues.Current != null ? (Choice) channel.Instruments[qualifier].InstrumentValues.Current.Value : (Choice) null;
  }

  private void SetChannelXmc02t(Channel channel)
  {
    if (this.xmc02t == channel)
      return;
    this.StopHsvService();
    if (this.xmc02t != null)
      this.xmc02t.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
    this.xmc02t = channel;
    if (this.xmc02t != null)
      channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
  }

  private void SetChannelHsv(Channel channel)
  {
    if (this.hsv == channel)
      return;
    this.StopHsvService();
    if (this.hsv != null)
    {
      this.hsv.Instruments[UserPanel.FrontAxleLiftingInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
      this.hsv.Instruments[UserPanel.RearAxleLiftingInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
      this.hsv.Instruments[UserPanel.FrontAxleLoweringInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
      this.hsv.Instruments[UserPanel.RearAxleLoweringInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
    }
    this.hsv = channel;
    if (this.hsv != null)
    {
      channel.Instruments[UserPanel.FrontAxleLiftingInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
      channel.Instruments[UserPanel.RearAxleLiftingInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
      channel.Instruments[UserPanel.FrontAxleLoweringInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
      channel.Instruments[UserPanel.RearAxleLoweringInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
    }
  }

  private void OnInstrumentUpdateEvent(object sender, ResultEventArgs e) => this.UpdateUI();

  private bool MoveService(
    bool moveFront,
    bool moveRear,
    UserPanel.Level level,
    UserPanel.ServiceMode mode)
  {
    bool flag = true;
    if (mode == UserPanel.ServiceMode.Calibration)
    {
      this.hsvServiceQualifier = this.hsvServices[level];
      this.Log(this.hsvServiceQualifier);
      this.hsvCalibrateTimer.Start();
      this.GoMachine();
    }
    else
    {
      this.StopHsvService();
      string str = string.Format(UserPanel.MoveServiceQualifier, (object) (moveFront ? 1 : 0), (object) (moveRear ? 1 : 0), (object) (int) level, (object) (int) mode);
      this.Log(str);
      if (!this.RunService(this.xmc02t, str) && !this.aborted)
      {
        flag = false;
        this.Abort(string.Format(Resources.MessageFormat_ServiceCannotBeStarted0, (object) str));
      }
    }
    return flag;
  }

  private bool RunService(Channel channel, string serviceQualifier)
  {
    if (channel != null && channel.Online)
    {
      Service service = channel.Services[serviceQualifier];
      if (service != (Service) null)
      {
        service.InputValues.ParseValues(serviceQualifier);
        service.Execute(false);
        return true;
      }
    }
    return false;
  }

  private void StopHsvService()
  {
    this.hsvCalibrateTimer.Stop();
    this.hsvServiceQualifier = string.Empty;
  }

  private void hsvCalibrateTimer_Tick(object sender, EventArgs e)
  {
    if (!string.IsNullOrEmpty(this.hsvServiceQualifier) && !this.aborted && this.RunService(this.hsv, this.hsvServiceQualifier))
      return;
    this.StopHsvService();
  }

  private void buttonStartNext_Click(object sender, EventArgs e)
  {
    if (((IEnumerable<UserPanel.ProcessState>) this.userStates).Contains<UserPanel.ProcessState>(this.state))
    {
      if (!this.working)
      {
        this.state = UserPanel.ProcessState.NotRunning;
        this.aborted = false;
      }
      this.GoMachine();
    }
    this.UpdateUI();
  }

  private void buttonAbort_Click(object sender, EventArgs e)
  {
    this.Abort(Resources.Message_UserAbortedTest);
  }

  private void checkBoxTruckOnStand_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxTruckOnStand.Checked)
      this.Log(Resources.Message_UserConfirmedTruckOnStand);
    this.UpdateUI();
  }

  private void checkBoxFullSetOfBlocks_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxFullSetOfBlocks.Checked)
      this.Log(Resources.Message_UserConfirmedFullSetOfBlocks);
    this.UpdateUI();
  }

  private void checkBoxNoOneAround_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxNoOneAround.Checked)
      this.Log(Resources.Message_UserConfirmedNoOneWorkingOnOrAroundTruck);
    this.UpdateUI();
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.label3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelRearAxleStatus = new ScalingLabel();
    this.tableLayoutPanel5 = new TableLayoutPanel();
    this.label20 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelFrontAxleStatus = new ScalingLabel();
    this.checkBoxNoOneAround = new CheckBox();
    this.digitalReadoutInstrumentAirPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.checkBoxTruckOnStand = new CheckBox();
    this.checkBoxFullSetOfBlocks = new CheckBox();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanelControls = new TableLayoutPanel();
    this.buttonClose = new Button();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmarkStatus = new Checkmark();
    this.labelStatus = new System.Windows.Forms.Label();
    this.buttonStartNext = new Button();
    this.buttonAbort = new Button();
    this.textBoxInstructions = new TextBox();
    this.tabControl = new TabControl();
    this.tabPageAxles = new TabPage();
    this.pictureBoxAxles = new PictureBox();
    this.tabPageFrontAxle = new TabPage();
    this.pictureBoxFrontAxle = new PictureBox();
    this.tabPageCenterAxle = new TabPage();
    this.pictureBoxCenterAxle = new PictureBox();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel5).SuspendLayout();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelControls).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    this.tabControl.SuspendLayout();
    this.tabPageAxles.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxAxles).BeginInit();
    this.tabPageFrontAxle.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxFrontAxle).BeginInit();
    this.tabPageCenterAxle.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxCenterAxle).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanel1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanel5, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.checkBoxNoOneAround, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentAirPressure, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.checkBoxTruckOnStand, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.checkBoxFullSetOfBlocks, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.label1, 0, 3);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label3, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.scalingLabelRearAxleStatus, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.label3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    ((Control) this.label3).Name = "label3";
    this.label3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelRearAxleStatus.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelRearAxleStatus, "scalingLabelRearAxleStatus");
    this.scalingLabelRearAxleStatus.FontGroup = (string) null;
    this.scalingLabelRearAxleStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelRearAxleStatus).Name = "scalingLabelRearAxleStatus";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel5, "tableLayoutPanel5");
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.label20, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.scalingLabelFrontAxleStatus, 0, 1);
    ((Control) this.tableLayoutPanel5).Name = "tableLayoutPanel5";
    this.label20.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label20, "label20");
    ((Control) this.label20).Name = "label20";
    this.label20.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelFrontAxleStatus.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelFrontAxleStatus, "scalingLabelFrontAxleStatus");
    this.scalingLabelFrontAxleStatus.FontGroup = (string) null;
    this.scalingLabelFrontAxleStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFrontAxleStatus).Name = "scalingLabelFrontAxleStatus";
    componentResourceManager.ApplyResources((object) this.checkBoxNoOneAround, "checkBoxNoOneAround");
    this.checkBoxNoOneAround.BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.checkBoxNoOneAround, 3);
    this.checkBoxNoOneAround.Name = "checkBoxNoOneAround";
    this.checkBoxNoOneAround.UseCompatibleTextRendering = true;
    this.checkBoxNoOneAround.UseVisualStyleBackColor = false;
    this.checkBoxNoOneAround.CheckedChanged += new EventHandler(this.checkBoxNoOneAround_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAirPressure, "digitalReadoutInstrumentAirPressure");
    this.digitalReadoutInstrumentAirPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirPressure).FreezeValue = false;
    this.digitalReadoutInstrumentAirPressure.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentAirPressure.Gradient.Modify(1, 193.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirPressure).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU");
    ((Control) this.digitalReadoutInstrumentAirPressure).Name = "digitalReadoutInstrumentAirPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirPressure).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentAirPressure.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "HSV", "DT_1724");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "HSV", "DT_1722");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "HSV", "DT_1721");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "HSV", "DT_1723");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.checkBoxTruckOnStand, "checkBoxTruckOnStand");
    this.checkBoxTruckOnStand.BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.checkBoxTruckOnStand, 3);
    this.checkBoxTruckOnStand.Name = "checkBoxTruckOnStand";
    this.checkBoxTruckOnStand.UseCompatibleTextRendering = true;
    this.checkBoxTruckOnStand.UseVisualStyleBackColor = false;
    this.checkBoxTruckOnStand.CheckedChanged += new EventHandler(this.checkBoxTruckOnStand_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxFullSetOfBlocks, "checkBoxFullSetOfBlocks");
    this.checkBoxFullSetOfBlocks.BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.checkBoxFullSetOfBlocks, 3);
    this.checkBoxFullSetOfBlocks.Name = "checkBoxFullSetOfBlocks";
    this.checkBoxFullSetOfBlocks.UseCompatibleTextRendering = true;
    this.checkBoxFullSetOfBlocks.UseVisualStyleBackColor = false;
    this.checkBoxFullSetOfBlocks.CheckedChanged += new EventHandler(this.checkBoxFullSetOfBlocks_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(2, 200.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 3, 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.label1, 3);
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelControls, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxInstructions, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tabControl, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelControls, "tableLayoutPanelControls");
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.buttonClose, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.tableLayoutPanel2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.buttonStartNext, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.buttonAbort, 4, 0);
    ((Control) this.tableLayoutPanelControls).Name = "tableLayoutPanelControls";
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelStatus, 1, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonStartNext, "buttonStartNext");
    this.buttonStartNext.Name = "buttonStartNext";
    this.buttonStartNext.UseCompatibleTextRendering = true;
    this.buttonStartNext.UseVisualStyleBackColor = true;
    this.buttonStartNext.Click += new EventHandler(this.buttonStartNext_Click);
    componentResourceManager.ApplyResources((object) this.buttonAbort, "buttonAbort");
    this.buttonAbort.Name = "buttonAbort";
    this.buttonAbort.UseCompatibleTextRendering = true;
    this.buttonAbort.UseVisualStyleBackColor = true;
    this.buttonAbort.Click += new EventHandler(this.buttonAbort_Click);
    componentResourceManager.ApplyResources((object) this.textBoxInstructions, "textBoxInstructions");
    this.textBoxInstructions.Name = "textBoxInstructions";
    this.textBoxInstructions.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.tabControl, "tabControl");
    this.tabControl.Controls.Add((Control) this.tabPageAxles);
    this.tabControl.Controls.Add((Control) this.tabPageFrontAxle);
    this.tabControl.Controls.Add((Control) this.tabPageCenterAxle);
    this.tabControl.Name = "tabControl";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.tabControl, 3);
    this.tabControl.SelectedIndex = 0;
    this.tabPageAxles.Controls.Add((Control) this.pictureBoxAxles);
    componentResourceManager.ApplyResources((object) this.tabPageAxles, "tabPageAxles");
    this.tabPageAxles.Name = "tabPageAxles";
    this.tabPageAxles.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.pictureBoxAxles, "pictureBoxAxles");
    this.pictureBoxAxles.Name = "pictureBoxAxles";
    this.pictureBoxAxles.TabStop = false;
    this.tabPageFrontAxle.Controls.Add((Control) this.pictureBoxFrontAxle);
    componentResourceManager.ApplyResources((object) this.tabPageFrontAxle, "tabPageFrontAxle");
    this.tabPageFrontAxle.Name = "tabPageFrontAxle";
    this.tabPageFrontAxle.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.pictureBoxFrontAxle, "pictureBoxFrontAxle");
    this.pictureBoxFrontAxle.Name = "pictureBoxFrontAxle";
    this.pictureBoxFrontAxle.TabStop = false;
    this.tabPageCenterAxle.Controls.Add((Control) this.pictureBoxCenterAxle);
    componentResourceManager.ApplyResources((object) this.tabPageCenterAxle, "tabPageCenterAxle");
    this.tabPageCenterAxle.Name = "tabPageCenterAxle";
    this.tabPageCenterAxle.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.pictureBoxCenterAxle, "pictureBoxCenterAxle");
    this.pictureBoxCenterAxle.Name = "pictureBoxCenterAxle";
    this.pictureBoxCenterAxle.TabStop = false;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Aerodynamic_Ride_Height_Calibration");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInstruments).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel5).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    this.tabControl.ResumeLayout(false);
    this.tabPageAxles.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxAxles).EndInit();
    this.tabPageFrontAxle.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxFrontAxle).EndInit();
    this.tabPageCenterAxle.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxCenterAxle).EndInit();
    ((Control) this).ResumeLayout(false);
  }

  private enum InstrumentState
  {
    NotActive,
    Active,
    Error,
    SNA,
  }

  private enum ServiceMode
  {
    Normal = 0,
    Calibration = 12, // 0x0000000C
  }

  private enum Level
  {
    None = 0,
    Normal1 = 1,
    Normal2 = 2,
    Upper = 6,
    Lowest = 7,
  }

  private enum ProcessState
  {
    NotRunning,
    Starting,
    StartingOnNext,
    Step1,
    Step1OnNext,
    Step2,
    Step2OnNext,
    Step3,
    Step3OnNext,
    Step4,
    Step4OnNext,
    Step5,
    Step5OnNext,
    Complete,
  }
}
