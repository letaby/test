using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel;

public class UserPanel : CustomPanel
{
	private enum MoveDirection
	{
		MoveUp = 9,
		MoveDown
	}

	private enum ServiceMode
	{
		Normal = 0,
		Calibration = 12
	}

	private enum InstrumentState
	{
		NotActive,
		Active,
		Error,
		SNA
	}

	private Channel xmc02t;

	private Channel hsv;

	private static int MoveTimeOut = 2000;

	private bool movingFront = false;

	private bool movingRear = false;

	private bool serviceRunning = false;

	private bool popupShown = false;

	private Timer timer = new Timer();

	private MoveDirection movingDirection;

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

	private Label label11;

	private ScalingLabel scalingLabelFrontAxleStatus;

	private TableLayoutPanel tableLayoutPanelRearStatus;

	private Label label10;

	private ScalingLabel scalingLabelRearAxleStatus;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private bool MovingAny
	{
		get
		{
			return movingFront || movingRear || serviceRunning;
		}
		set
		{
			movingFront = (movingRear = value);
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		UpdateUI();
		timer.Interval = 1000;
		timer.Tick += timer_Tick;
		timer.Start();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (xmc02t != null)
		{
			xmc02t.Services.ServiceCompleteEvent -= channel_ServiceCompleteEvent;
		}
		MoveService(moveFront: false, moveRear: false, MoveDirection.MoveDown, ServiceMode.Normal);
		SetChannelXmc02t(null);
		SetChannelHsv(null);
	}

	private void Log(string message)
	{
		((CustomPanel)this).LabelLog("HSV", message);
	}

	public override void OnChannelsChanged()
	{
		SetChannelXmc02t(((CustomPanel)this).GetChannel(Xmc02tName, (ChannelLookupOptions)3));
		SetChannelHsv(((CustomPanel)this).GetChannel(HsvName, (ChannelLookupOptions)3));
		UpdateUI();
	}

	private void FaultCodes_FaultCodesUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Invalid comparison between Unknown and I4
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Invalid comparison between Unknown and I4
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Invalid comparison between Unknown and I4
		UpdateDisplayedFaults();
		Button button = buttonMoveRearUp;
		Button button2 = buttonMoveFrontUp;
		string text = (buttonMoveBothUp.Text = (MovingAny ? Resources.Message_Stop : Resources.Message_Up));
		text = (button2.Text = text);
		button.Text = text;
		Button button3 = buttonMoveRearDown;
		Button button4 = buttonMoveFrontDown;
		text = (buttonMoveBothDown.Text = (MovingAny ? Resources.Message_Stop : Resources.Message_Down));
		text = (button4.Text = text);
		button3.Text = text;
		ValueState val = UpdateScalingLabelStatus(scalingLabelFrontAxleStatus, FrontAxleLiftingInstrument, FrontAxleLoweringInstrument);
		bool flag = (int)scalingLabelFrontAxleStatus.RepresentedState == 2 || (int)scalingLabelFrontAxleStatus.RepresentedState == 1;
		ValueState val2 = UpdateScalingLabelStatus(scalingLabelRearAxleStatus, RearAxleLiftingInstrument, RearAxleLoweringInstrument);
		bool flag2 = (int)scalingLabelRearAxleStatus.RepresentedState == 2 || (int)scalingLabelRearAxleStatus.RepresentedState == 1;
		RunServiceButton obj = runServiceButtonFrontGoRaised;
		RunServiceButton obj2 = runServiceButtonFrontGoStandard;
		RunServiceButton obj3 = runServiceButtonFrontGoAero;
		bool flag3 = (((Control)(object)runServiceButtonFrontGoLowered).Enabled = flag && !MovingAny);
		flag3 = (((Control)(object)obj3).Enabled = flag3);
		flag3 = (((Control)(object)obj2).Enabled = flag3);
		((Control)(object)obj).Enabled = flag3;
		RunServiceButton obj4 = runServiceButtonRearGoRaised;
		RunServiceButton obj5 = runServiceButtonRearGoStandard;
		RunServiceButton obj6 = runServiceButtonRearGoAero;
		flag3 = (((Control)(object)runServiceButtonRearGoLowered).Enabled = flag2 && !MovingAny);
		flag3 = (((Control)(object)obj6).Enabled = flag3);
		flag3 = (((Control)(object)obj5).Enabled = flag3);
		((Control)(object)obj4).Enabled = flag3;
		RunServiceButton obj7 = runServiceButtonGoRaised;
		RunServiceButton obj8 = runServiceButtonGoStandard;
		RunServiceButton obj9 = runServiceButtonGoAero;
		RunServiceButton obj10 = runServiceButtonGoLowered;
		flag3 = (((Control)(object)runServiceButtonFrontGoLowered).Enabled = flag2 && flag && !MovingAny);
		flag3 = (((Control)(object)obj10).Enabled = flag3);
		flag3 = (((Control)(object)obj9).Enabled = flag3);
		flag3 = (((Control)(object)obj8).Enabled = flag3);
		((Control)(object)obj7).Enabled = flag3;
		RunServiceButton obj11 = runServiceButtonSaveRaised;
		RunServiceButton obj12 = runServiceButtonSaveStandard;
		RunServiceButton obj13 = runServiceButtonSaveAero;
		flag3 = (((Control)(object)runServiceButtonSaveLowered).Enabled = flag2 && !MovingAny);
		flag3 = (((Control)(object)obj13).Enabled = flag3);
		flag3 = (((Control)(object)obj12).Enabled = flag3);
		((Control)(object)obj11).Enabled = flag3;
		Button button5 = buttonMoveFrontUp;
		flag3 = (buttonMoveFrontDown.Enabled = flag && xmc02t != null && xmc02t.Online && !serviceRunning);
		button5.Enabled = flag3;
		Button button6 = buttonMoveRearUp;
		flag3 = (buttonMoveRearDown.Enabled = flag2 && xmc02t != null && xmc02t.Online && !serviceRunning);
		button6.Enabled = flag3;
		Button button7 = buttonMoveBothUp;
		flag3 = (buttonMoveBothDown.Enabled = flag && flag2 && xmc02t != null && xmc02t.Online && !serviceRunning);
		button7.Enabled = flag3;
	}

	private ValueState UpdateScalingLabelStatus(ScalingLabel scalingLabel, string liftingQualifier, string loweringQualifier)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		Choice choice = ReadChoiceValue(hsv, liftingQualifier);
		Choice choice2 = ReadChoiceValue(hsv, loweringQualifier);
		if (choice == null || choice.Index == 2 || choice.Index == 3 || choice2 == null || choice2.Index == 2 || choice2.Index == 3)
		{
			((Control)(object)scalingLabel).Text = Resources.Message_SNA;
			scalingLabel.RepresentedState = (ValueState)3;
		}
		else if (choice.Index == 1)
		{
			((Control)(object)scalingLabel).Text = Resources.Message_Raising;
			scalingLabel.RepresentedState = (ValueState)2;
		}
		else if (choice2.Index == 1)
		{
			((Control)(object)scalingLabel).Text = Resources.Message_Lowering;
			scalingLabel.RepresentedState = (ValueState)2;
		}
		else
		{
			((Control)(object)scalingLabel).Text = Resources.Message_Stationary;
			scalingLabel.RepresentedState = (ValueState)1;
		}
		return scalingLabel.RepresentedState;
	}

	private Choice ReadChoiceValue(Channel channel, string qualifier)
	{
		if (channel != null && channel.Instruments != null && channel.Instruments[qualifier] != null && channel.Instruments[qualifier].InstrumentValues != null && channel.Instruments[qualifier].InstrumentValues.Current != null)
		{
			return (Choice)channel.Instruments[qualifier].InstrumentValues.Current.Value;
		}
		return null;
	}

	private void SetChannelXmc02t(Channel channel)
	{
		if (xmc02t != channel)
		{
			if (xmc02t != null)
			{
				xmc02t.Services.ServiceCompleteEvent -= channel_ServiceCompleteEvent;
				xmc02t.FaultCodes.FaultCodesUpdateEvent -= FaultCodes_FaultCodesUpdateEvent;
			}
			xmc02t = channel;
			if (xmc02t != null)
			{
				if (!popupShown && xmc02t.Online)
				{
					popupShown = true;
					MessageBox.Show(Resources.Message_WARNING, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				}
				channel.FaultCodes.FaultCodesUpdateEvent += FaultCodes_FaultCodesUpdateEvent;
				channel.Services.ServiceCompleteEvent += channel_ServiceCompleteEvent;
			}
		}
		UpdateUI();
	}

	private void SetChannelHsv(Channel channel)
	{
		if (hsv != channel)
		{
			if (hsv != null)
			{
				hsv.Instruments[FrontAxleLiftingInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.Instruments[RearAxleLiftingInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.Instruments[FrontAxleLoweringInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.Instruments[RearAxleLoweringInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.FaultCodes.FaultCodesUpdateEvent -= FaultCodes_FaultCodesUpdateEvent;
			}
			hsv = channel;
			if (hsv != null)
			{
				channel.Instruments[FrontAxleLiftingInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.Instruments[RearAxleLiftingInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.Instruments[FrontAxleLoweringInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.Instruments[RearAxleLoweringInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.FaultCodes.FaultCodesUpdateEvent += FaultCodes_FaultCodesUpdateEvent;
			}
		}
		UpdateUI();
	}

	private void OnInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void UpdateDisplayedFaults()
	{
		listViewExFaults.BeginUpdate();
		((ListView)(object)listViewExFaults).Items.Clear();
		AddFaults(xmc02t, listViewExFaults);
		AddFaults(hsv, listViewExFaults);
		listViewExFaults.EndUpdate();
	}

	private void AddFaults(Channel channel, ListViewEx listViewEx)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		if (channel == null)
		{
			return;
		}
		foreach (FaultCode item in channel.FaultCodes.Where((FaultCode fc1) => fc1.FaultCodeIncidents.Count > 0 && fc1.FaultCodeIncidents.Current != null && fc1.FaultCodeIncidents.Current.Active == ActiveStatus.Active))
		{
			ListViewExGroupItem value = new ListViewExGroupItem(new string[4]
			{
				channel.Ecu.Name,
				item.Text,
				item.Number,
				item.Mode
			});
			((ListView)(object)listViewEx).Items.Add((ListViewItem)(object)value);
		}
	}

	private void channel_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			if (MovingAny && ((DateTime.Now - startedMoving).TotalMilliseconds > (double)MoveTimeOut || !MoveService(movingFront, movingRear, movingDirection, ServiceMode.Normal)))
			{
				MovingAny = false;
			}
		}
		else
		{
			MovingAny = false;
		}
		UpdateUI();
	}

	private bool MoveService(bool moveFront, bool moveRear, MoveDirection direction, ServiceMode mode)
	{
		bool result = true;
		string text = string.Format(MoveServiceQualifier, moveFront ? 1 : 0, moveRear ? 1 : 0, (int)direction, (int)mode);
		Log(text);
		if (!RunService(xmc02t, text))
		{
			result = false;
			Log(string.Format(Resources.MessageFormat_ServiceCannotBeStarted0, text));
		}
		return result;
	}

	private bool RunService(Channel channel, string serviceQualifier)
	{
		if (channel != null && channel.Online)
		{
			Service service = channel.Services[serviceQualifier];
			if (service != null)
			{
				service.InputValues.ParseValues(serviceQualifier);
				service.Execute(synchronous: false);
				return true;
			}
		}
		return false;
	}

	private void buttonMove_Click(object sender, EventArgs e)
	{
		if (!MovingAny && xmc02t != null && xmc02t.CommunicationsState == CommunicationsState.Online)
		{
			string[] array = ((string)((Button)sender).Tag).ToLower().Replace(" ", "").Split(',');
			movingFront = array[0].Equals("front") || array[0].Equals("both");
			movingRear = array[0].Equals("rear") || array[0].Equals("both");
			movingDirection = (array[1].Equals("down") ? MoveDirection.MoveDown : MoveDirection.MoveUp);
			startedMoving = DateTime.Now;
			if (!MoveService(movingFront, movingRear, movingDirection, ServiceMode.Normal))
			{
				MovingAny = false;
			}
		}
		else
		{
			MovingAny = false;
		}
		UpdateUI();
	}

	private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		serviceRunning = false;
		UpdateUI();
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void runServiceButton_Starting(object sender, CancelEventArgs e)
	{
		serviceRunning = true;
		UpdateUI();
	}

	private void runServiceButton_Stopped(object sender, PassFailResultEventArgs e)
	{
		serviceRunning = false;
		UpdateUI();
	}

	private void runServiceButton_Stopping(object sender, CancelEventArgs e)
	{
		serviceRunning = false;
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fde: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_166a: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1756: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f96: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ffc: Unknown result type (might be due to invalid IL or missing references)
		//IL_224e: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelControls = new TableLayoutPanel();
		label5 = new System.Windows.Forms.Label();
		label1 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		panelMoveTo = new Panel();
		tableLayoutPanelMoveTo = new TableLayoutPanel();
		label7 = new System.Windows.Forms.Label();
		runServiceButtonFrontGoRaised = new RunServiceButton();
		runServiceButtonFrontGoStandard = new RunServiceButton();
		runServiceButtonFrontGoAero = new RunServiceButton();
		runServiceButtonFrontGoLowered = new RunServiceButton();
		runServiceButtonRearGoRaised = new RunServiceButton();
		runServiceButtonRearGoStandard = new RunServiceButton();
		runServiceButtonRearGoLowered = new RunServiceButton();
		runServiceButtonRearGoAero = new RunServiceButton();
		runServiceButtonGoLowered = new RunServiceButton();
		runServiceButtonGoAero = new RunServiceButton();
		runServiceButtonGoStandard = new RunServiceButton();
		runServiceButtonGoRaised = new RunServiceButton();
		panelJog = new Panel();
		tableLayoutPanelJog = new TableLayoutPanel();
		buttonMoveBothUp = new Button();
		label8 = new System.Windows.Forms.Label();
		buttonMoveFrontUp = new Button();
		buttonMoveRearUp = new Button();
		buttonMoveFrontDown = new Button();
		buttonMoveRearDown = new Button();
		buttonMoveBothDown = new Button();
		panelSave = new Panel();
		tableLayoutPanelSave = new TableLayoutPanel();
		runServiceButtonSaveRaised = new RunServiceButton();
		runServiceButtonSaveStandard = new RunServiceButton();
		runServiceButtonSaveAero = new RunServiceButton();
		runServiceButtonSaveLowered = new RunServiceButton();
		label6 = new System.Windows.Forms.Label();
		buttonClose = new Button();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		tableLayoutPanelFrontStatus = new TableLayoutPanel();
		label11 = new Label();
		scalingLabelFrontAxleStatus = new ScalingLabel();
		tableLayoutPanelRearStatus = new TableLayoutPanel();
		label10 = new Label();
		scalingLabelRearAxleStatus = new ScalingLabel();
		digitalReadoutInstrumentAirPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentIgnitionSwitch = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		tableLayoutPanelMain = new TableLayoutPanel();
		panelFaults = new Panel();
		tableLayoutPanelFaults = new TableLayoutPanel();
		label9 = new System.Windows.Forms.Label();
		listViewExFaults = new ListViewEx();
		columnHeaderChannel = new ColumnHeader();
		columnHeaderName = new ColumnHeader();
		columnHeaderNumber = new ColumnHeader();
		columnHeaderMode = new ColumnHeader();
		((Control)(object)tableLayoutPanelControls).SuspendLayout();
		panelMoveTo.SuspendLayout();
		((Control)(object)tableLayoutPanelMoveTo).SuspendLayout();
		panelJog.SuspendLayout();
		((Control)(object)tableLayoutPanelJog).SuspendLayout();
		panelSave.SuspendLayout();
		((Control)(object)tableLayoutPanelSave).SuspendLayout();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanelFrontStatus).SuspendLayout();
		((Control)(object)tableLayoutPanelRearStatus).SuspendLayout();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		panelFaults.SuspendLayout();
		((Control)(object)tableLayoutPanelFaults).SuspendLayout();
		((ISupportInitialize)listViewExFaults).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelControls, "tableLayoutPanelControls");
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(label5, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(label1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(label3, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(label2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(panelMoveTo, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(panelJog, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(panelSave, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(buttonClose, 7, 5);
		((Control)(object)tableLayoutPanelControls).Name = "tableLayoutPanelControls";
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		label3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(panelMoveTo, "panelMoveTo");
		panelMoveTo.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelControls).SetColumnSpan((Control)panelMoveTo, 4);
		panelMoveTo.Controls.Add((Control)(object)tableLayoutPanelMoveTo);
		panelMoveTo.Name = "panelMoveTo";
		((TableLayoutPanel)(object)tableLayoutPanelControls).SetRowSpan((Control)panelMoveTo, 4);
		componentResourceManager.ApplyResources(tableLayoutPanelMoveTo, "tableLayoutPanelMoveTo");
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add(label7, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonFrontGoRaised, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonFrontGoStandard, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonFrontGoAero, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonFrontGoLowered, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonRearGoRaised, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonRearGoStandard, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonRearGoLowered, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonRearGoAero, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonGoLowered, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonGoAero, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonGoStandard, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).Controls.Add((Control)(object)runServiceButtonGoRaised, 0, 3);
		((Control)(object)tableLayoutPanelMoveTo).Name = "tableLayoutPanelMoveTo";
		componentResourceManager.ApplyResources(label7, "label7");
		((TableLayoutPanel)(object)tableLayoutPanelMoveTo).SetColumnSpan((Control)label7, 4);
		label7.Name = "label7";
		label7.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonFrontGoRaised, "runServiceButtonFrontGoRaised");
		((Control)(object)runServiceButtonFrontGoRaised).Name = "runServiceButtonFrontGoRaised";
		runServiceButtonFrontGoRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=0", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonFrontGoRaised.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoRaised).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoRaised).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoRaised).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonFrontGoStandard, "runServiceButtonFrontGoStandard");
		((Control)(object)runServiceButtonFrontGoStandard).Name = "runServiceButtonFrontGoStandard";
		runServiceButtonFrontGoStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonFrontGoStandard.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoStandard).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoStandard).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoStandard).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonFrontGoAero, "runServiceButtonFrontGoAero");
		((Control)(object)runServiceButtonFrontGoAero).Name = "runServiceButtonFrontGoAero";
		runServiceButtonFrontGoAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonFrontGoAero.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoAero).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoAero).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoAero).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonFrontGoLowered, "runServiceButtonFrontGoLowered");
		((Control)(object)runServiceButtonFrontGoLowered).Name = "runServiceButtonFrontGoLowered";
		runServiceButtonFrontGoLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonFrontGoLowered.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoLowered).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoLowered).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonFrontGoLowered).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonRearGoRaised, "runServiceButtonRearGoRaised");
		((Control)(object)runServiceButtonRearGoRaised).Name = "runServiceButtonRearGoRaised";
		runServiceButtonRearGoRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=6", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonRearGoRaised.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoRaised).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoRaised).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoRaised).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonRearGoStandard, "runServiceButtonRearGoStandard");
		((Control)(object)runServiceButtonRearGoStandard).Name = "runServiceButtonRearGoStandard";
		runServiceButtonRearGoStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonRearGoStandard.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoStandard).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoStandard).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoStandard).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonRearGoLowered, "runServiceButtonRearGoLowered");
		((Control)(object)runServiceButtonRearGoLowered).Name = "runServiceButtonRearGoLowered";
		runServiceButtonRearGoLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonRearGoLowered.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoLowered).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoLowered).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoLowered).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonRearGoAero, "runServiceButtonRearGoAero");
		((Control)(object)runServiceButtonRearGoAero).Name = "runServiceButtonRearGoAero";
		runServiceButtonRearGoAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonRearGoAero.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoAero).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoAero).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonRearGoAero).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonGoLowered, "runServiceButtonGoLowered");
		((Control)(object)runServiceButtonGoLowered).Name = "runServiceButtonGoLowered";
		runServiceButtonGoLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonGoLowered.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonGoLowered).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonGoLowered).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonGoLowered).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonGoAero, "runServiceButtonGoAero");
		((Control)(object)runServiceButtonGoAero).Name = "runServiceButtonGoAero";
		runServiceButtonGoAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonGoAero.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonGoAero).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonGoAero).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonGoAero).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonGoStandard, "runServiceButtonGoStandard");
		((Control)(object)runServiceButtonGoStandard).Name = "runServiceButtonGoStandard";
		runServiceButtonGoStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonGoStandard.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonGoStandard).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonGoStandard).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonGoStandard).Stopped += runServiceButton_Stopped;
		componentResourceManager.ApplyResources(runServiceButtonGoRaised, "runServiceButtonGoRaised");
		((Control)(object)runServiceButtonGoRaised).Name = "runServiceButtonGoRaised";
		runServiceButtonGoRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=6", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" });
		runServiceButtonGoRaised.ServiceComplete += runServiceButton_ServiceComplete;
		((RunSharedProcedureButtonBase)runServiceButtonGoRaised).Starting += runServiceButton_Starting;
		((RunSharedProcedureButtonBase)runServiceButtonGoRaised).Stopping += runServiceButton_Stopping;
		((RunSharedProcedureButtonBase)runServiceButtonGoRaised).Stopped += runServiceButton_Stopped;
		panelJog.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelControls).SetColumnSpan((Control)panelJog, 2);
		panelJog.Controls.Add((Control)(object)tableLayoutPanelJog);
		componentResourceManager.ApplyResources(panelJog, "panelJog");
		panelJog.Name = "panelJog";
		((TableLayoutPanel)(object)tableLayoutPanelControls).SetRowSpan((Control)panelJog, 4);
		componentResourceManager.ApplyResources(tableLayoutPanelJog, "tableLayoutPanelJog");
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(buttonMoveBothUp, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(label8, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(buttonMoveFrontUp, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(buttonMoveRearUp, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(buttonMoveFrontDown, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(buttonMoveRearDown, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelJog).Controls.Add(buttonMoveBothDown, 1, 3);
		((Control)(object)tableLayoutPanelJog).Name = "tableLayoutPanelJog";
		componentResourceManager.ApplyResources(buttonMoveBothUp, "buttonMoveBothUp");
		buttonMoveBothUp.Name = "buttonMoveBothUp";
		buttonMoveBothUp.Tag = "both,up";
		buttonMoveBothUp.UseCompatibleTextRendering = true;
		buttonMoveBothUp.UseVisualStyleBackColor = true;
		buttonMoveBothUp.Click += buttonMove_Click;
		componentResourceManager.ApplyResources(label8, "label8");
		((TableLayoutPanel)(object)tableLayoutPanelJog).SetColumnSpan((Control)label8, 2);
		label8.Name = "label8";
		label8.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonMoveFrontUp, "buttonMoveFrontUp");
		buttonMoveFrontUp.Name = "buttonMoveFrontUp";
		buttonMoveFrontUp.Tag = "front,up";
		buttonMoveFrontUp.UseCompatibleTextRendering = true;
		buttonMoveFrontUp.UseVisualStyleBackColor = true;
		buttonMoveFrontUp.Click += buttonMove_Click;
		componentResourceManager.ApplyResources(buttonMoveRearUp, "buttonMoveRearUp");
		buttonMoveRearUp.Name = "buttonMoveRearUp";
		buttonMoveRearUp.Tag = "rear,up";
		buttonMoveRearUp.UseCompatibleTextRendering = true;
		buttonMoveRearUp.UseVisualStyleBackColor = true;
		buttonMoveRearUp.Click += buttonMove_Click;
		componentResourceManager.ApplyResources(buttonMoveFrontDown, "buttonMoveFrontDown");
		buttonMoveFrontDown.Name = "buttonMoveFrontDown";
		buttonMoveFrontDown.Tag = "front,down";
		buttonMoveFrontDown.UseCompatibleTextRendering = true;
		buttonMoveFrontDown.UseVisualStyleBackColor = true;
		buttonMoveFrontDown.Click += buttonMove_Click;
		componentResourceManager.ApplyResources(buttonMoveRearDown, "buttonMoveRearDown");
		buttonMoveRearDown.Name = "buttonMoveRearDown";
		buttonMoveRearDown.Tag = "rear,down";
		buttonMoveRearDown.UseCompatibleTextRendering = true;
		buttonMoveRearDown.UseVisualStyleBackColor = true;
		buttonMoveRearDown.Click += buttonMove_Click;
		componentResourceManager.ApplyResources(buttonMoveBothDown, "buttonMoveBothDown");
		buttonMoveBothDown.Name = "buttonMoveBothDown";
		buttonMoveBothDown.Tag = "both,down";
		buttonMoveBothDown.UseCompatibleTextRendering = true;
		buttonMoveBothDown.UseVisualStyleBackColor = true;
		buttonMoveBothDown.Click += buttonMove_Click;
		panelSave.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelControls).SetColumnSpan((Control)panelSave, 6);
		panelSave.Controls.Add((Control)(object)tableLayoutPanelSave);
		componentResourceManager.ApplyResources(panelSave, "panelSave");
		panelSave.Name = "panelSave";
		componentResourceManager.ApplyResources(tableLayoutPanelSave, "tableLayoutPanelSave");
		((TableLayoutPanel)(object)tableLayoutPanelSave).Controls.Add((Control)(object)runServiceButtonSaveRaised, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelSave).Controls.Add((Control)(object)runServiceButtonSaveStandard, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelSave).Controls.Add((Control)(object)runServiceButtonSaveAero, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelSave).Controls.Add((Control)(object)runServiceButtonSaveLowered, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanelSave).Controls.Add(label6, 0, 0);
		((Control)(object)tableLayoutPanelSave).Name = "tableLayoutPanelSave";
		componentResourceManager.ApplyResources(runServiceButtonSaveRaised, "runServiceButtonSaveRaised");
		((Control)(object)runServiceButtonSaveRaised).Name = "runServiceButtonSaveRaised";
		runServiceButtonSaveRaised.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=6", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" });
		componentResourceManager.ApplyResources(runServiceButtonSaveStandard, "runServiceButtonSaveStandard");
		((Control)(object)runServiceButtonSaveStandard).Name = "runServiceButtonSaveStandard";
		runServiceButtonSaveStandard.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" });
		componentResourceManager.ApplyResources(runServiceButtonSaveAero, "runServiceButtonSaveAero");
		((Control)(object)runServiceButtonSaveAero).Name = "runServiceButtonSaveAero";
		runServiceButtonSaveAero.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" });
		componentResourceManager.ApplyResources(runServiceButtonSaveLowered, "runServiceButtonSaveLowered");
		((Control)(object)runServiceButtonSaveLowered).Name = "runServiceButtonSaveLowered";
		runServiceButtonSaveLowered.ServiceCall = new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" });
		componentResourceManager.ApplyResources(label6, "label6");
		((TableLayoutPanel)(object)tableLayoutPanelSave).SetColumnSpan((Control)label6, 2);
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanelFrontStatus, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanelRearStatus, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentAirPressure, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument5, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentIgnitionSwitch, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 1);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(tableLayoutPanelFrontStatus, "tableLayoutPanelFrontStatus");
		((TableLayoutPanel)(object)tableLayoutPanelFrontStatus).Controls.Add((Control)(object)label11, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFrontStatus).Controls.Add((Control)(object)scalingLabelFrontAxleStatus, 0, 1);
		((Control)(object)tableLayoutPanelFrontStatus).Name = "tableLayoutPanelFrontStatus";
		label11.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)(object)label11).Name = "label11";
		label11.Orientation = (TextOrientation)1;
		scalingLabelFrontAxleStatus.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelFrontAxleStatus, "scalingLabelFrontAxleStatus");
		scalingLabelFrontAxleStatus.FontGroup = null;
		scalingLabelFrontAxleStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFrontAxleStatus).Name = "scalingLabelFrontAxleStatus";
		componentResourceManager.ApplyResources(tableLayoutPanelRearStatus, "tableLayoutPanelRearStatus");
		((TableLayoutPanel)(object)tableLayoutPanelRearStatus).Controls.Add((Control)(object)label10, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelRearStatus).Controls.Add((Control)(object)scalingLabelRearAxleStatus, 0, 1);
		((Control)(object)tableLayoutPanelRearStatus).Name = "tableLayoutPanelRearStatus";
		label10.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)(object)label10).Name = "label10";
		label10.Orientation = (TextOrientation)1;
		scalingLabelRearAxleStatus.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelRearAxleStatus, "scalingLabelRearAxleStatus");
		scalingLabelRearAxleStatus.FontGroup = null;
		scalingLabelRearAxleStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelRearAxleStatus).Name = "scalingLabelRearAxleStatus";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAirPressure, "digitalReadoutInstrumentAirPressure");
		digitalReadoutInstrumentAirPressure.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAirPressure).FreezeValue = false;
		digitalReadoutInstrumentAirPressure.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentAirPressure.Gradient.Modify(1, 90.0, (ValueState)2);
		digitalReadoutInstrumentAirPressure.Gradient.Modify(2, 520.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentAirPressure).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU");
		((Control)(object)digitalReadoutInstrumentAirPressure).Name = "digitalReadoutInstrumentAirPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentAirPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrument5.Gradient.Initialize((ValueState)3, 3);
		digitalReadoutInstrument5.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument5.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument5.Gradient.Modify(3, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentIgnitionSwitch, "digitalReadoutInstrumentIgnitionSwitch");
		digitalReadoutInstrumentIgnitionSwitch.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).FreezeValue = false;
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Initialize((ValueState)3, 3);
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(3, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus");
		((Control)(object)digitalReadoutInstrumentIgnitionSwitch).Name = "digitalReadoutInstrumentIgnitionSwitch";
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionSwitch).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1724");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1722");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1721");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1723");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelControls, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelFaults, 0, 1);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		panelFaults.BorderStyle = BorderStyle.FixedSingle;
		panelFaults.Controls.Add((Control)(object)tableLayoutPanelFaults);
		componentResourceManager.ApplyResources(panelFaults, "panelFaults");
		panelFaults.Name = "panelFaults";
		componentResourceManager.ApplyResources(tableLayoutPanelFaults, "tableLayoutPanelFaults");
		((TableLayoutPanel)(object)tableLayoutPanelFaults).Controls.Add(label9, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFaults).Controls.Add((Control)(object)listViewExFaults, 0, 1);
		((Control)(object)tableLayoutPanelFaults).Name = "tableLayoutPanelFaults";
		componentResourceManager.ApplyResources(label9, "label9");
		label9.Name = "label9";
		label9.UseCompatibleTextRendering = true;
		listViewExFaults.CanDelete = false;
		((ListView)(object)listViewExFaults).Columns.AddRange(new ColumnHeader[4] { columnHeaderChannel, columnHeaderName, columnHeaderNumber, columnHeaderMode });
		componentResourceManager.ApplyResources(listViewExFaults, "listViewExFaults");
		listViewExFaults.EditableColumn = -1;
		((Control)(object)listViewExFaults).Name = "listViewExFaults";
		listViewExFaults.Sorting = SortOrder.Ascending;
		((ListView)(object)listViewExFaults).UseCompatibleStateImageBehavior = false;
		componentResourceManager.ApplyResources(columnHeaderChannel, "columnHeaderChannel");
		componentResourceManager.ApplyResources(columnHeaderName, "columnHeaderName");
		componentResourceManager.ApplyResources(columnHeaderNumber, "columnHeaderNumber");
		componentResourceManager.ApplyResources(columnHeaderMode, "columnHeaderMode");
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Aerodynamic_Ride_Height_Test");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelControls).PerformLayout();
		panelMoveTo.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMoveTo).ResumeLayout(performLayout: false);
		panelJog.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelJog).ResumeLayout(performLayout: false);
		panelSave.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelSave).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelSave).PerformLayout();
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFrontStatus).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelRearStatus).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		panelFaults.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFaults).ResumeLayout(performLayout: false);
		((ISupportInitialize)listViewExFaults).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
