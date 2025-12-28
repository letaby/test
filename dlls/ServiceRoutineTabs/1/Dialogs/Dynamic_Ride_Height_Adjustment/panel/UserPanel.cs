// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel;

public class UserPanel : CustomPanel
{
  private Channel xmc02t;
  private Channel hsv;
  private static int MoveTimeOut = 2000;
  private bool movingFront = false;
  private bool movingRear = false;
  private bool serviceRunning = false;
  private bool popupShown = false;
  private Timer timer = new Timer();
  private UserPanel.MoveDirection movingDirection;
  private DateTime startedMoving;
  private static string FrontAxleLiftingInstrument = "DT_1739";
  private static string RearAxleLiftingInstrument = "DT_1756";
  private static string FrontAxleLoweringInstrument = "DT_1740";
  private static string RearAxleLoweringInstrument = "DT_1755";
  private static string HsvName = "HSV";
  private static string MoveServiceQualifier = "IOC_RHC_OutputCtrl_Control(DiagRqData_OC_NomLvlRqFAx_Enbl={0},Nominal_Level_Request_Front_Axle={2},DiagRqData_OC_NomLvlRqRAx={2},DiagRqData_OC_NomLvlRqRAx_Enbl={1},DiagRqData_OC_LvlCtrlMd_Rq={3},Level_Control_Mode_Request=1)";
  private static string Xmc02tName = "XMC02T";
  private TableLayoutPanel tableLayoutPanelControls;
  private RunServiceButton runServiceButtonFrontGoRaised;
  private RunServiceButton runServiceButtonGoLowered;
  private RunServiceButton runServiceButtonGoAero;
  private RunServiceButton runServiceButtonGoStandard;
  private RunServiceButton runServiceButtonGoRaised;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label label2;
  private System.Windows.Forms.Label label3;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private TableLayoutPanel tableLayoutPanelMain;
  private ListViewEx listViewExFaults;
  private ColumnHeader columnHeaderChannel;
  private ColumnHeader columnHeaderName;
  private ColumnHeader columnHeaderNumber;
  private ColumnHeader columnHeaderMode;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrumentIgnitionSwitch;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private RunServiceButton runServiceButtonRearGoRaised;
  private RunServiceButton runServiceButtonFrontGoStandard;
  private RunServiceButton runServiceButtonRearGoStandard;
  private RunServiceButton runServiceButtonFrontGoAero;
  private RunServiceButton runServiceButtonRearGoAero;
  private RunServiceButton runServiceButtonFrontGoLowered;
  private RunServiceButton runServiceButtonRearGoLowered;
  private System.Windows.Forms.Label label5;
  private RunServiceButton runServiceButtonSaveLowered;
  private RunServiceButton runServiceButtonSaveRaised;
  private RunServiceButton runServiceButtonSaveAero;
  private System.Windows.Forms.Label label6;
  private RunServiceButton runServiceButtonSaveStandard;
  private Panel panelMoveTo;
  private TableLayoutPanel tableLayoutPanelMoveTo;
  private System.Windows.Forms.Label label7;
  private Panel panelSave;
  private TableLayoutPanel tableLayoutPanelSave;
  private Panel panelJog;
  private TableLayoutPanel tableLayoutPanelJog;
  private System.Windows.Forms.Label label8;
  private Button buttonMoveFrontUp;
  private Button buttonMoveRearUp;
  private Button buttonMoveFrontDown;
  private Button buttonMoveRearDown;
  private Button buttonClose;
  private Panel panelFaults;
  private TableLayoutPanel tableLayoutPanelFaults;
  private System.Windows.Forms.Label label9;
  private Button buttonMoveBothUp;
  private Button buttonMoveBothDown;
  private DigitalReadoutInstrument digitalReadoutInstrumentAirPressure;
  private TableLayoutPanel tableLayoutPanelFrontStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label11;
  private ScalingLabel scalingLabelFrontAxleStatus;
  private TableLayoutPanel tableLayoutPanelRearStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label10;
  private ScalingLabel scalingLabelRearAxleStatus;
  private DigitalReadoutInstrument digitalReadoutInstrument3;

  private bool MovingAny
  {
    get => this.movingFront || this.movingRear || this.serviceRunning;
    set => this.movingFront = this.movingRear = value;
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.UpdateUI();
    this.timer.Interval = 1000;
    this.timer.Tick += new EventHandler(this.timer_Tick);
    this.timer.Start();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.xmc02t != null)
      this.xmc02t.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
    this.MoveService(false, false, UserPanel.MoveDirection.MoveDown, UserPanel.ServiceMode.Normal);
    this.SetChannelXmc02t((Channel) null);
    this.SetChannelHsv((Channel) null);
  }

  private void Log(string message) => this.LabelLog("HSV", message);

  public virtual void OnChannelsChanged()
  {
    this.SetChannelXmc02t(this.GetChannel(UserPanel.Xmc02tName, (CustomPanel.ChannelLookupOptions) 3));
    this.SetChannelHsv(this.GetChannel(UserPanel.HsvName, (CustomPanel.ChannelLookupOptions) 3));
    this.UpdateUI();
  }

  private void FaultCodes_FaultCodesUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    this.UpdateDisplayedFaults();
    Button buttonMoveRearUp = this.buttonMoveRearUp;
    Button buttonMoveFrontUp = this.buttonMoveFrontUp;
    string str1;
    this.buttonMoveBothUp.Text = str1 = this.MovingAny ? Resources.Message_Stop : Resources.Message_Up;
    string str2;
    string str3 = str2 = str1;
    buttonMoveFrontUp.Text = str2;
    string str4 = str3;
    buttonMoveRearUp.Text = str4;
    Button buttonMoveRearDown = this.buttonMoveRearDown;
    Button buttonMoveFrontDown = this.buttonMoveFrontDown;
    string str5;
    this.buttonMoveBothDown.Text = str5 = this.MovingAny ? Resources.Message_Stop : Resources.Message_Down;
    string str6;
    string str7 = str6 = str5;
    buttonMoveFrontDown.Text = str6;
    string str8 = str7;
    buttonMoveRearDown.Text = str8;
    this.UpdateScalingLabelStatus(this.scalingLabelFrontAxleStatus, UserPanel.FrontAxleLiftingInstrument, UserPanel.FrontAxleLoweringInstrument);
    bool flag1 = this.scalingLabelFrontAxleStatus.RepresentedState == 2 || this.scalingLabelFrontAxleStatus.RepresentedState == 1;
    this.UpdateScalingLabelStatus(this.scalingLabelRearAxleStatus, UserPanel.RearAxleLiftingInstrument, UserPanel.RearAxleLoweringInstrument);
    bool flag2 = this.scalingLabelRearAxleStatus.RepresentedState == 2 || this.scalingLabelRearAxleStatus.RepresentedState == 1;
    RunServiceButton buttonFrontGoRaised = this.runServiceButtonFrontGoRaised;
    RunServiceButton buttonFrontGoStandard = this.runServiceButtonFrontGoStandard;
    RunServiceButton buttonFrontGoAero = this.runServiceButtonFrontGoAero;
    bool flag3;
    ((Control) this.runServiceButtonFrontGoLowered).Enabled = flag3 = flag1 && !this.MovingAny;
    int num1;
    bool flag4 = (num1 = flag3 ? 1 : 0) != 0;
    ((Control) buttonFrontGoAero).Enabled = num1 != 0;
    int num2;
    bool flag5 = (num2 = flag4 ? 1 : 0) != 0;
    ((Control) buttonFrontGoStandard).Enabled = num2 != 0;
    int num3 = flag5 ? 1 : 0;
    ((Control) buttonFrontGoRaised).Enabled = num3 != 0;
    RunServiceButton buttonRearGoRaised = this.runServiceButtonRearGoRaised;
    RunServiceButton buttonRearGoStandard = this.runServiceButtonRearGoStandard;
    RunServiceButton buttonRearGoAero = this.runServiceButtonRearGoAero;
    bool flag6;
    ((Control) this.runServiceButtonRearGoLowered).Enabled = flag6 = flag2 && !this.MovingAny;
    int num4;
    bool flag7 = (num4 = flag6 ? 1 : 0) != 0;
    ((Control) buttonRearGoAero).Enabled = num4 != 0;
    int num5;
    bool flag8 = (num5 = flag7 ? 1 : 0) != 0;
    ((Control) buttonRearGoStandard).Enabled = num5 != 0;
    int num6 = flag8 ? 1 : 0;
    ((Control) buttonRearGoRaised).Enabled = num6 != 0;
    RunServiceButton serviceButtonGoRaised = this.runServiceButtonGoRaised;
    RunServiceButton buttonGoStandard = this.runServiceButtonGoStandard;
    RunServiceButton serviceButtonGoAero = this.runServiceButtonGoAero;
    RunServiceButton serviceButtonGoLowered = this.runServiceButtonGoLowered;
    bool flag9;
    ((Control) this.runServiceButtonFrontGoLowered).Enabled = flag9 = flag2 && flag1 && !this.MovingAny;
    int num7;
    bool flag10 = (num7 = flag9 ? 1 : 0) != 0;
    ((Control) serviceButtonGoLowered).Enabled = num7 != 0;
    int num8;
    bool flag11 = (num8 = flag10 ? 1 : 0) != 0;
    ((Control) serviceButtonGoAero).Enabled = num8 != 0;
    int num9;
    bool flag12 = (num9 = flag11 ? 1 : 0) != 0;
    ((Control) buttonGoStandard).Enabled = num9 != 0;
    int num10 = flag12 ? 1 : 0;
    ((Control) serviceButtonGoRaised).Enabled = num10 != 0;
    RunServiceButton buttonSaveRaised = this.runServiceButtonSaveRaised;
    RunServiceButton buttonSaveStandard = this.runServiceButtonSaveStandard;
    RunServiceButton serviceButtonSaveAero = this.runServiceButtonSaveAero;
    bool flag13;
    ((Control) this.runServiceButtonSaveLowered).Enabled = flag13 = flag2 && !this.MovingAny;
    int num11;
    bool flag14 = (num11 = flag13 ? 1 : 0) != 0;
    ((Control) serviceButtonSaveAero).Enabled = num11 != 0;
    int num12;
    bool flag15 = (num12 = flag14 ? 1 : 0) != 0;
    ((Control) buttonSaveStandard).Enabled = num12 != 0;
    int num13 = flag15 ? 1 : 0;
    ((Control) buttonSaveRaised).Enabled = num13 != 0;
    this.buttonMoveFrontUp.Enabled = this.buttonMoveFrontDown.Enabled = flag1 && this.xmc02t != null && this.xmc02t.Online && !this.serviceRunning;
    this.buttonMoveRearUp.Enabled = this.buttonMoveRearDown.Enabled = flag2 && this.xmc02t != null && this.xmc02t.Online && !this.serviceRunning;
    this.buttonMoveBothUp.Enabled = this.buttonMoveBothDown.Enabled = flag1 && flag2 && this.xmc02t != null && this.xmc02t.Online && !this.serviceRunning;
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
      ((Control) scalingLabel).Text = Resources.Message_Raising;
      scalingLabel.RepresentedState = (ValueState) 2;
    }
    else if (choice2.Index == 1)
    {
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
    if (this.xmc02t != channel)
    {
      if (this.xmc02t != null)
      {
        this.xmc02t.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
        this.xmc02t.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
      }
      this.xmc02t = channel;
      if (this.xmc02t != null)
      {
        if (!this.popupShown && this.xmc02t.Online)
        {
          this.popupShown = true;
          int num = (int) MessageBox.Show(Resources.Message_WARNING, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }
        channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
        channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
      }
    }
    this.UpdateUI();
  }

  private void SetChannelHsv(Channel channel)
  {
    if (this.hsv != channel)
    {
      if (this.hsv != null)
      {
        this.hsv.Instruments[UserPanel.FrontAxleLiftingInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        this.hsv.Instruments[UserPanel.RearAxleLiftingInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        this.hsv.Instruments[UserPanel.FrontAxleLoweringInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        this.hsv.Instruments[UserPanel.RearAxleLoweringInstrument].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        this.hsv.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
      }
      this.hsv = channel;
      if (this.hsv != null)
      {
        channel.Instruments[UserPanel.FrontAxleLiftingInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        channel.Instruments[UserPanel.RearAxleLiftingInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        channel.Instruments[UserPanel.FrontAxleLoweringInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        channel.Instruments[UserPanel.RearAxleLoweringInstrument].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent);
        channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
      }
    }
    this.UpdateUI();
  }

  private void OnInstrumentUpdateEvent(object sender, ResultEventArgs e) => this.UpdateUI();

  private void UpdateDisplayedFaults()
  {
    this.listViewExFaults.BeginUpdate();
    ((ListView) this.listViewExFaults).Items.Clear();
    this.AddFaults(this.xmc02t, this.listViewExFaults);
    this.AddFaults(this.hsv, this.listViewExFaults);
    this.listViewExFaults.EndUpdate();
  }

  private void AddFaults(Channel channel, ListViewEx listViewEx)
  {
    if (channel == null)
      return;
    foreach (FaultCode faultCode in channel.FaultCodes.Where<FaultCode>((Func<FaultCode, bool>) (fc1 => fc1.FaultCodeIncidents.Count > 0 && fc1.FaultCodeIncidents.Current != null && fc1.FaultCodeIncidents.Current.Active == ActiveStatus.Active)))
    {
      ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(new string[4]
      {
        channel.Ecu.Name,
        faultCode.Text,
        faultCode.Number,
        faultCode.Mode
      });
      ((ListView) listViewEx).Items.Add((ListViewItem) listViewExGroupItem);
    }
  }

  private void channel_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (sender as Service != (Service) null)
    {
      if (this.MovingAny && ((DateTime.Now - this.startedMoving).TotalMilliseconds > (double) UserPanel.MoveTimeOut || !this.MoveService(this.movingFront, this.movingRear, this.movingDirection, UserPanel.ServiceMode.Normal)))
        this.MovingAny = false;
    }
    else
      this.MovingAny = false;
    this.UpdateUI();
  }

  private bool MoveService(
    bool moveFront,
    bool moveRear,
    UserPanel.MoveDirection direction,
    UserPanel.ServiceMode mode)
  {
    bool flag = true;
    string str = string.Format(UserPanel.MoveServiceQualifier, (object) (moveFront ? 1 : 0), (object) (moveRear ? 1 : 0), (object) (int) direction, (object) (int) mode);
    this.Log(str);
    if (!this.RunService(this.xmc02t, str))
    {
      flag = false;
      this.Log(string.Format(Resources.MessageFormat_ServiceCannotBeStarted0, (object) str));
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

  private void buttonMove_Click(object sender, EventArgs e)
  {
    if (!this.MovingAny && this.xmc02t != null && this.xmc02t.CommunicationsState == CommunicationsState.Online)
    {
      string[] strArray = ((string) ((Control) sender).Tag).ToLower().Replace(" ", "").Split(',');
      this.movingFront = strArray[0].Equals("front") || strArray[0].Equals("both");
      this.movingRear = strArray[0].Equals("rear") || strArray[0].Equals("both");
      this.movingDirection = strArray[1].Equals("down") ? UserPanel.MoveDirection.MoveDown : UserPanel.MoveDirection.MoveUp;
      this.startedMoving = DateTime.Now;
      if (!this.MoveService(this.movingFront, this.movingRear, this.movingDirection, UserPanel.ServiceMode.Normal))
        this.MovingAny = false;
    }
    else
      this.MovingAny = false;
    this.UpdateUI();
  }

  private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    this.serviceRunning = false;
    this.UpdateUI();
  }

  private void timer_Tick(object sender, EventArgs e) => this.UpdateUI();

  private void runServiceButton_Starting(object sender, CancelEventArgs e)
  {
    this.serviceRunning = true;
    this.UpdateUI();
  }

  private void runServiceButton_Stopped(object sender, PassFailResultEventArgs e)
  {
    this.serviceRunning = false;
    this.UpdateUI();
  }

  private void runServiceButton_Stopping(object sender, CancelEventArgs e)
  {
    this.serviceRunning = false;
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelControls = new TableLayoutPanel();
    this.label5 = new System.Windows.Forms.Label();
    this.label1 = new System.Windows.Forms.Label();
    this.label3 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.panelMoveTo = new Panel();
    this.tableLayoutPanelMoveTo = new TableLayoutPanel();
    this.label7 = new System.Windows.Forms.Label();
    this.runServiceButtonFrontGoRaised = new RunServiceButton();
    this.runServiceButtonFrontGoStandard = new RunServiceButton();
    this.runServiceButtonFrontGoAero = new RunServiceButton();
    this.runServiceButtonFrontGoLowered = new RunServiceButton();
    this.runServiceButtonRearGoRaised = new RunServiceButton();
    this.runServiceButtonRearGoStandard = new RunServiceButton();
    this.runServiceButtonRearGoLowered = new RunServiceButton();
    this.runServiceButtonRearGoAero = new RunServiceButton();
    this.runServiceButtonGoLowered = new RunServiceButton();
    this.runServiceButtonGoAero = new RunServiceButton();
    this.runServiceButtonGoStandard = new RunServiceButton();
    this.runServiceButtonGoRaised = new RunServiceButton();
    this.panelJog = new Panel();
    this.tableLayoutPanelJog = new TableLayoutPanel();
    this.buttonMoveBothUp = new Button();
    this.label8 = new System.Windows.Forms.Label();
    this.buttonMoveFrontUp = new Button();
    this.buttonMoveRearUp = new Button();
    this.buttonMoveFrontDown = new Button();
    this.buttonMoveRearDown = new Button();
    this.buttonMoveBothDown = new Button();
    this.panelSave = new Panel();
    this.tableLayoutPanelSave = new TableLayoutPanel();
    this.runServiceButtonSaveRaised = new RunServiceButton();
    this.runServiceButtonSaveStandard = new RunServiceButton();
    this.runServiceButtonSaveAero = new RunServiceButton();
    this.runServiceButtonSaveLowered = new RunServiceButton();
    this.label6 = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.tableLayoutPanelFrontStatus = new TableLayoutPanel();
    this.label11 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelFrontAxleStatus = new ScalingLabel();
    this.tableLayoutPanelRearStatus = new TableLayoutPanel();
    this.label10 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelRearAxleStatus = new ScalingLabel();
    this.digitalReadoutInstrumentAirPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentIgnitionSwitch = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.panelFaults = new Panel();
    this.tableLayoutPanelFaults = new TableLayoutPanel();
    this.label9 = new System.Windows.Forms.Label();
    this.listViewExFaults = new ListViewEx();
    this.columnHeaderChannel = new ColumnHeader();
    this.columnHeaderName = new ColumnHeader();
    this.columnHeaderNumber = new ColumnHeader();
    this.columnHeaderMode = new ColumnHeader();
    ((Control) this.tableLayoutPanelControls).SuspendLayout();
    this.panelMoveTo.SuspendLayout();
    ((Control) this.tableLayoutPanelMoveTo).SuspendLayout();
    this.panelJog.SuspendLayout();
    ((Control) this.tableLayoutPanelJog).SuspendLayout();
    this.panelSave.SuspendLayout();
    ((Control) this.tableLayoutPanelSave).SuspendLayout();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanelFrontStatus).SuspendLayout();
    ((Control) this.tableLayoutPanelRearStatus).SuspendLayout();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    this.panelFaults.SuspendLayout();
    ((Control) this.tableLayoutPanelFaults).SuspendLayout();
    ((ISupportInitialize) this.listViewExFaults).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelControls, "tableLayoutPanelControls");
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.label5, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.label1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.label3, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.label2, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.panelMoveTo, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.panelJog, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.panelSave, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.buttonClose, 7, 5);
    ((Control) this.tableLayoutPanelControls).Name = "tableLayoutPanelControls";
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    this.label3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.panelMoveTo, "panelMoveTo");
    this.panelMoveTo.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelControls).SetColumnSpan((Control) this.panelMoveTo, 4);
    this.panelMoveTo.Controls.Add((Control) this.tableLayoutPanelMoveTo);
    this.panelMoveTo.Name = "panelMoveTo";
    ((TableLayoutPanel) this.tableLayoutPanelControls).SetRowSpan((Control) this.panelMoveTo, 4);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMoveTo, "tableLayoutPanelMoveTo");
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.label7, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonFrontGoRaised, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonFrontGoStandard, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonFrontGoAero, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonFrontGoLowered, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonRearGoRaised, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonRearGoStandard, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonRearGoLowered, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonRearGoAero, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonGoLowered, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonGoAero, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonGoStandard, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).Controls.Add((Control) this.runServiceButtonGoRaised, 0, 3);
    ((Control) this.tableLayoutPanelMoveTo).Name = "tableLayoutPanelMoveTo";
    componentResourceManager.ApplyResources((object) this.label7, "label7");
    ((TableLayoutPanel) this.tableLayoutPanelMoveTo).SetColumnSpan((Control) this.label7, 4);
    this.label7.Name = "label7";
    this.label7.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonFrontGoRaised, "runServiceButtonFrontGoRaised");
    ((Control) this.runServiceButtonFrontGoRaised).Name = "runServiceButtonFrontGoRaised";
    this.runServiceButtonFrontGoRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=6",
      "DiagRqData_OC_NomLvlRqRAx=0",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonFrontGoRaised.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoRaised).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoRaised).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoRaised).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonFrontGoStandard, "runServiceButtonFrontGoStandard");
    ((Control) this.runServiceButtonFrontGoStandard).Name = "runServiceButtonFrontGoStandard";
    this.runServiceButtonFrontGoStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=1",
      "DiagRqData_OC_NomLvlRqRAx=1",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonFrontGoStandard.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoStandard).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoStandard).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoStandard).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonFrontGoAero, "runServiceButtonFrontGoAero");
    ((Control) this.runServiceButtonFrontGoAero).Name = "runServiceButtonFrontGoAero";
    this.runServiceButtonFrontGoAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=2",
      "DiagRqData_OC_NomLvlRqRAx=2",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonFrontGoAero.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoAero).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoAero).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoAero).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonFrontGoLowered, "runServiceButtonFrontGoLowered");
    ((Control) this.runServiceButtonFrontGoLowered).Name = "runServiceButtonFrontGoLowered";
    this.runServiceButtonFrontGoLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=7",
      "DiagRqData_OC_NomLvlRqRAx=7",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonFrontGoLowered.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoLowered).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoLowered).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonFrontGoLowered).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonRearGoRaised, "runServiceButtonRearGoRaised");
    ((Control) this.runServiceButtonRearGoRaised).Name = "runServiceButtonRearGoRaised";
    this.runServiceButtonRearGoRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=6",
      "DiagRqData_OC_NomLvlRqRAx=6",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonRearGoRaised.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoRaised).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoRaised).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoRaised).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonRearGoStandard, "runServiceButtonRearGoStandard");
    ((Control) this.runServiceButtonRearGoStandard).Name = "runServiceButtonRearGoStandard";
    this.runServiceButtonRearGoStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=1",
      "DiagRqData_OC_NomLvlRqRAx=1",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonRearGoStandard.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoStandard).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoStandard).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoStandard).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonRearGoLowered, "runServiceButtonRearGoLowered");
    ((Control) this.runServiceButtonRearGoLowered).Name = "runServiceButtonRearGoLowered";
    this.runServiceButtonRearGoLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=7",
      "DiagRqData_OC_NomLvlRqRAx=7",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonRearGoLowered.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoLowered).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoLowered).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoLowered).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonRearGoAero, "runServiceButtonRearGoAero");
    ((Control) this.runServiceButtonRearGoAero).Name = "runServiceButtonRearGoAero";
    this.runServiceButtonRearGoAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=2",
      "DiagRqData_OC_NomLvlRqRAx=2",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonRearGoAero.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoAero).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoAero).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonRearGoAero).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonGoLowered, "runServiceButtonGoLowered");
    ((Control) this.runServiceButtonGoLowered).Name = "runServiceButtonGoLowered";
    this.runServiceButtonGoLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=7",
      "DiagRqData_OC_NomLvlRqRAx=7",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonGoLowered.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoLowered).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoLowered).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoLowered).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonGoAero, "runServiceButtonGoAero");
    ((Control) this.runServiceButtonGoAero).Name = "runServiceButtonGoAero";
    this.runServiceButtonGoAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=2",
      "DiagRqData_OC_NomLvlRqRAx=2",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonGoAero.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoAero).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoAero).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoAero).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonGoStandard, "runServiceButtonGoStandard");
    ((Control) this.runServiceButtonGoStandard).Name = "runServiceButtonGoStandard";
    this.runServiceButtonGoStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=1",
      "DiagRqData_OC_NomLvlRqRAx=1",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonGoStandard.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoStandard).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoStandard).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoStandard).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    componentResourceManager.ApplyResources((object) this.runServiceButtonGoRaised, "runServiceButtonGoRaised");
    ((Control) this.runServiceButtonGoRaised).Name = "runServiceButtonGoRaised";
    this.runServiceButtonGoRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
      "Nominal_Level_Request_Front_Axle=6",
      "DiagRqData_OC_NomLvlRqRAx=6",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=1"
    });
    this.runServiceButtonGoRaised.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoRaised).Starting += new EventHandler<CancelEventArgs>(this.runServiceButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoRaised).Stopping += new EventHandler<CancelEventArgs>(this.runServiceButton_Stopping);
    ((RunSharedProcedureButtonBase) this.runServiceButtonGoRaised).Stopped += new EventHandler<PassFailResultEventArgs>(this.runServiceButton_Stopped);
    this.panelJog.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelControls).SetColumnSpan((Control) this.panelJog, 2);
    this.panelJog.Controls.Add((Control) this.tableLayoutPanelJog);
    componentResourceManager.ApplyResources((object) this.panelJog, "panelJog");
    this.panelJog.Name = "panelJog";
    ((TableLayoutPanel) this.tableLayoutPanelControls).SetRowSpan((Control) this.panelJog, 4);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelJog, "tableLayoutPanelJog");
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.buttonMoveBothUp, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.label8, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.buttonMoveFrontUp, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.buttonMoveRearUp, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.buttonMoveFrontDown, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.buttonMoveRearDown, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelJog).Controls.Add((Control) this.buttonMoveBothDown, 1, 3);
    ((Control) this.tableLayoutPanelJog).Name = "tableLayoutPanelJog";
    componentResourceManager.ApplyResources((object) this.buttonMoveBothUp, "buttonMoveBothUp");
    this.buttonMoveBothUp.Name = "buttonMoveBothUp";
    this.buttonMoveBothUp.Tag = (object) "both,up";
    this.buttonMoveBothUp.UseCompatibleTextRendering = true;
    this.buttonMoveBothUp.UseVisualStyleBackColor = true;
    this.buttonMoveBothUp.Click += new EventHandler(this.buttonMove_Click);
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    ((TableLayoutPanel) this.tableLayoutPanelJog).SetColumnSpan((Control) this.label8, 2);
    this.label8.Name = "label8";
    this.label8.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonMoveFrontUp, "buttonMoveFrontUp");
    this.buttonMoveFrontUp.Name = "buttonMoveFrontUp";
    this.buttonMoveFrontUp.Tag = (object) "front,up";
    this.buttonMoveFrontUp.UseCompatibleTextRendering = true;
    this.buttonMoveFrontUp.UseVisualStyleBackColor = true;
    this.buttonMoveFrontUp.Click += new EventHandler(this.buttonMove_Click);
    componentResourceManager.ApplyResources((object) this.buttonMoveRearUp, "buttonMoveRearUp");
    this.buttonMoveRearUp.Name = "buttonMoveRearUp";
    this.buttonMoveRearUp.Tag = (object) "rear,up";
    this.buttonMoveRearUp.UseCompatibleTextRendering = true;
    this.buttonMoveRearUp.UseVisualStyleBackColor = true;
    this.buttonMoveRearUp.Click += new EventHandler(this.buttonMove_Click);
    componentResourceManager.ApplyResources((object) this.buttonMoveFrontDown, "buttonMoveFrontDown");
    this.buttonMoveFrontDown.Name = "buttonMoveFrontDown";
    this.buttonMoveFrontDown.Tag = (object) "front,down";
    this.buttonMoveFrontDown.UseCompatibleTextRendering = true;
    this.buttonMoveFrontDown.UseVisualStyleBackColor = true;
    this.buttonMoveFrontDown.Click += new EventHandler(this.buttonMove_Click);
    componentResourceManager.ApplyResources((object) this.buttonMoveRearDown, "buttonMoveRearDown");
    this.buttonMoveRearDown.Name = "buttonMoveRearDown";
    this.buttonMoveRearDown.Tag = (object) "rear,down";
    this.buttonMoveRearDown.UseCompatibleTextRendering = true;
    this.buttonMoveRearDown.UseVisualStyleBackColor = true;
    this.buttonMoveRearDown.Click += new EventHandler(this.buttonMove_Click);
    componentResourceManager.ApplyResources((object) this.buttonMoveBothDown, "buttonMoveBothDown");
    this.buttonMoveBothDown.Name = "buttonMoveBothDown";
    this.buttonMoveBothDown.Tag = (object) "both,down";
    this.buttonMoveBothDown.UseCompatibleTextRendering = true;
    this.buttonMoveBothDown.UseVisualStyleBackColor = true;
    this.buttonMoveBothDown.Click += new EventHandler(this.buttonMove_Click);
    this.panelSave.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelControls).SetColumnSpan((Control) this.panelSave, 6);
    this.panelSave.Controls.Add((Control) this.tableLayoutPanelSave);
    componentResourceManager.ApplyResources((object) this.panelSave, "panelSave");
    this.panelSave.Name = "panelSave";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelSave, "tableLayoutPanelSave");
    ((TableLayoutPanel) this.tableLayoutPanelSave).Controls.Add((Control) this.runServiceButtonSaveRaised, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelSave).Controls.Add((Control) this.runServiceButtonSaveStandard, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelSave).Controls.Add((Control) this.runServiceButtonSaveAero, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelSave).Controls.Add((Control) this.runServiceButtonSaveLowered, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanelSave).Controls.Add((Control) this.label6, 0, 0);
    ((Control) this.tableLayoutPanelSave).Name = "tableLayoutPanelSave";
    componentResourceManager.ApplyResources((object) this.runServiceButtonSaveRaised, "runServiceButtonSaveRaised");
    ((Control) this.runServiceButtonSaveRaised).Name = "runServiceButtonSaveRaised";
    this.runServiceButtonSaveRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=6",
      "DiagRqData_OC_NomLvlRqRAx=6",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonSaveStandard, "runServiceButtonSaveStandard");
    ((Control) this.runServiceButtonSaveStandard).Name = "runServiceButtonSaveStandard";
    this.runServiceButtonSaveStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=1",
      "DiagRqData_OC_NomLvlRqRAx=1",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonSaveAero, "runServiceButtonSaveAero");
    ((Control) this.runServiceButtonSaveAero).Name = "runServiceButtonSaveAero";
    this.runServiceButtonSaveAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=2",
      "DiagRqData_OC_NomLvlRqRAx=2",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonSaveLowered, "runServiceButtonSaveLowered");
    ((Control) this.runServiceButtonSaveLowered).Name = "runServiceButtonSaveLowered";
    this.runServiceButtonSaveLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
    {
      "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
      "Nominal_Level_Request_Front_Axle=7",
      "DiagRqData_OC_NomLvlRqRAx=7",
      "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
      "DiagRqData_OC_LvlCtrlMd_Rq=0",
      "Level_Control_Mode_Request=0"
    });
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    ((TableLayoutPanel) this.tableLayoutPanelSave).SetColumnSpan((Control) this.label6, 2);
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanelFrontStatus, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanelRearStatus, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentAirPressure, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument5, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentIgnitionSwitch, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 1);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFrontStatus, "tableLayoutPanelFrontStatus");
    ((TableLayoutPanel) this.tableLayoutPanelFrontStatus).Controls.Add((Control) this.label11, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFrontStatus).Controls.Add((Control) this.scalingLabelFrontAxleStatus, 0, 1);
    ((Control) this.tableLayoutPanelFrontStatus).Name = "tableLayoutPanelFrontStatus";
    this.label11.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label11, "label11");
    ((Control) this.label11).Name = "label11";
    this.label11.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelFrontAxleStatus.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelFrontAxleStatus, "scalingLabelFrontAxleStatus");
    this.scalingLabelFrontAxleStatus.FontGroup = (string) null;
    this.scalingLabelFrontAxleStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFrontAxleStatus).Name = "scalingLabelFrontAxleStatus";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelRearStatus, "tableLayoutPanelRearStatus");
    ((TableLayoutPanel) this.tableLayoutPanelRearStatus).Controls.Add((Control) this.label10, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelRearStatus).Controls.Add((Control) this.scalingLabelRearAxleStatus, 0, 1);
    ((Control) this.tableLayoutPanelRearStatus).Name = "tableLayoutPanelRearStatus";
    this.label10.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label10, "label10");
    ((Control) this.label10).Name = "label10";
    this.label10.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelRearAxleStatus.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelRearAxleStatus, "scalingLabelRearAxleStatus");
    this.scalingLabelRearAxleStatus.FontGroup = (string) null;
    this.scalingLabelRearAxleStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelRearAxleStatus).Name = "scalingLabelRearAxleStatus";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAirPressure, "digitalReadoutInstrumentAirPressure");
    this.digitalReadoutInstrumentAirPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirPressure).FreezeValue = false;
    this.digitalReadoutInstrumentAirPressure.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentAirPressure.Gradient.Modify(1, 90.0, (ValueState) 2);
    this.digitalReadoutInstrumentAirPressure.Gradient.Modify(2, 520.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirPressure).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU");
    ((Control) this.digitalReadoutInstrumentAirPressure).Name = "digitalReadoutInstrumentAirPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrument5.Gradient.Initialize((ValueState) 3, 3);
    this.digitalReadoutInstrument5.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument5.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument5.Gradient.Modify(3, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentIgnitionSwitch, "digitalReadoutInstrumentIgnitionSwitch");
    this.digitalReadoutInstrumentIgnitionSwitch.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).FreezeValue = false;
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Initialize((ValueState) 3, 3);
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(3, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ignitionStatus");
    ((Control) this.digitalReadoutInstrumentIgnitionSwitch).Name = "digitalReadoutInstrumentIgnitionSwitch";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).UnitAlignment = StringAlignment.Near;
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
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelControls, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelFaults, 0, 1);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    this.panelFaults.BorderStyle = BorderStyle.FixedSingle;
    this.panelFaults.Controls.Add((Control) this.tableLayoutPanelFaults);
    componentResourceManager.ApplyResources((object) this.panelFaults, "panelFaults");
    this.panelFaults.Name = "panelFaults";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFaults, "tableLayoutPanelFaults");
    ((TableLayoutPanel) this.tableLayoutPanelFaults).Controls.Add((Control) this.label9, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFaults).Controls.Add((Control) this.listViewExFaults, 0, 1);
    ((Control) this.tableLayoutPanelFaults).Name = "tableLayoutPanelFaults";
    componentResourceManager.ApplyResources((object) this.label9, "label9");
    this.label9.Name = "label9";
    this.label9.UseCompatibleTextRendering = true;
    this.listViewExFaults.CanDelete = false;
    ((ListView) this.listViewExFaults).Columns.AddRange(new ColumnHeader[4]
    {
      this.columnHeaderChannel,
      this.columnHeaderName,
      this.columnHeaderNumber,
      this.columnHeaderMode
    });
    componentResourceManager.ApplyResources((object) this.listViewExFaults, "listViewExFaults");
    this.listViewExFaults.EditableColumn = -1;
    ((Control) this.listViewExFaults).Name = "listViewExFaults";
    this.listViewExFaults.Sorting = SortOrder.Ascending;
    ((ListView) this.listViewExFaults).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeaderChannel, "columnHeaderChannel");
    componentResourceManager.ApplyResources((object) this.columnHeaderName, "columnHeaderName");
    componentResourceManager.ApplyResources((object) this.columnHeaderNumber, "columnHeaderNumber");
    componentResourceManager.ApplyResources((object) this.columnHeaderMode, "columnHeaderMode");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Aerodynamic_Ride_Height_Test");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanelControls).PerformLayout();
    this.panelMoveTo.ResumeLayout(false);
    ((Control) this.tableLayoutPanelMoveTo).ResumeLayout(false);
    this.panelJog.ResumeLayout(false);
    ((Control) this.tableLayoutPanelJog).ResumeLayout(false);
    this.panelSave.ResumeLayout(false);
    ((Control) this.tableLayoutPanelSave).ResumeLayout(false);
    ((Control) this.tableLayoutPanelSave).PerformLayout();
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFrontStatus).ResumeLayout(false);
    ((Control) this.tableLayoutPanelRearStatus).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    this.panelFaults.ResumeLayout(false);
    ((Control) this.tableLayoutPanelFaults).ResumeLayout(false);
    ((ISupportInitialize) this.listViewExFaults).EndInit();
    ((Control) this).ResumeLayout(false);
  }

  private enum MoveDirection
  {
    MoveUp = 9,
    MoveDown = 10, // 0x0000000A
  }

  private enum ServiceMode
  {
    Normal = 0,
    Calibration = 12, // 0x0000000C
  }

  private enum InstrumentState
  {
    NotActive,
    Active,
    Error,
    SNA,
  }
}
