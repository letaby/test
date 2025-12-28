using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Battery_Management__EMG_.panel;

public class UserPanel : CustomPanel
{
	private const string NumberofStringsQualifier = "ptconf_p_Veh_BatNumOfStrings_u8";

	private const string HighVoltageLockQualifier = "DL_High_Voltage_Lock";

	private const string HighVoltageLockedQualifier = "DT_STO_High_Voltage_Lock_High_Voltage_Lock";

	private static int MaxBatteryCount = 9;

	private Channel ecpc01tChannel = null;

	private Parameter numberofStringsParameter = null;

	private int previousBatteryCount = 0;

	private readonly string[] BmsEcus = new string[9] { "BMS01T", "BMS201T", "BMS301T", "BMS401T", "BMS501T", "BMS601T", "BMS701T", "BMS801T", "BMS901T" };

	private Channel[] Bms = new Channel[9];

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock1;

	private RunServiceButton runServiceButtonBMS1Lock;

	private RunServiceButton runServiceButtonBMS1Unlock;

	private System.Windows.Forms.Label labelBatteryManagement;

	private System.Windows.Forms.Label labelTransportLockBitStatus;

	private System.Windows.Forms.Label labelTransportLockBitControl;

	private RunServiceButton runServiceButtonBMS3Lock;

	private RunServiceButton runServiceButtonBMS2Lock;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock9;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock8;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock7;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock6;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock5;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock4;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock3;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock2;

	private RunServiceButton runServiceButtonBMS9Lock;

	private RunServiceButton runServiceButtonBMS8Lock;

	private RunServiceButton runServiceButtonBMS7Lock;

	private RunServiceButton runServiceButtonBMS6Lock;

	private RunServiceButton runServiceButtonBMS5Lock;

	private RunServiceButton runServiceButtonBMS4Lock;

	private RunServiceButton runServiceButtonBMS9Unlock;

	private RunServiceButton runServiceButtonBMS8Unlock;

	private RunServiceButton runServiceButtonBMS7Unlock;

	private RunServiceButton runServiceButtonBMS6Unlock;

	private RunServiceButton runServiceButtonBMS5Unlock;

	private RunServiceButton runServiceButtonBMS4Unlock;

	private RunServiceButton runServiceButtonBMS3Unlock;

	private RunServiceButton runServiceButtonBMS2Unlock;

	private TableLayoutPanel tableLayoutPanel4;

	private DigitalReadoutInstrument digitalReadoutInstrumentCharging;

	private System.Windows.Forms.Label label1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private System.Windows.Forms.Label labelInterlockWarning;

	private System.Windows.Forms.Label label39;

	private TableLayoutPanel tableLayoutPanel1;

	private bool EcpcOnline => ecpc01tChannel != null && (ecpc01tChannel.CommunicationsState == CommunicationsState.Online || ecpc01tChannel.CommunicationsState == CommunicationsState.LogFilePlayback);

	private int BatteryCount
	{
		get
		{
			int result = 9;
			if (EcpcOnline && numberofStringsParameter != null && numberofStringsParameter.HasBeenReadFromEcu && numberofStringsParameter.Value != null)
			{
				int.TryParse(numberofStringsParameter.Value.ToString(), out result);
			}
			if (result <= 3)
			{
				result = 4;
			}
			if (result > MaxBatteryCount)
			{
				result = MaxBatteryCount;
			}
			return result;
		}
	}

	private bool VehicleCheckOk => (int)digitalReadoutInstrumentParkBrake.RepresentedState != 3 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentCharging.RepresentedState == 1;

	public UserPanel()
	{
		InitializeComponent();
		previousBatteryCount = BatteryCount;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += PreconditionRepresentedStateChanged;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += PreconditionRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock1.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock2.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock3.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock4.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock5.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock6.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock7.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock8.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		digitalReadoutInstrumentTransportLock9.RepresentedStateChanged += TransportLockRepresentedStateChanged;
		UpdateUserInterface();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		for (int i = 0; i < BmsEcus.Count(); i++)
		{
			SetBms(i, ((CustomPanel)this).GetChannel(BmsEcus[i]));
		}
		SetECPC(((CustomPanel)this).GetChannel("ECPC01T", (ChannelLookupOptions)3));
		UpdateUserInterface();
	}

	private void SetECPC(Channel ecpc01t)
	{
		if (ecpc01tChannel == ecpc01t)
		{
			return;
		}
		if (ecpc01tChannel != null)
		{
			ecpc01tChannel.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			numberofStringsParameter = null;
		}
		ecpc01tChannel = ecpc01t;
		if (ecpc01tChannel != null)
		{
			ecpc01tChannel.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			numberofStringsParameter = ecpc01tChannel.Parameters["ptconf_p_Veh_BatNumOfStrings_u8"];
			if (EcpcOnline)
			{
				ReadInitialParameters();
			}
		}
	}

	private void SetBms(int bmsNum, Channel channel)
	{
		if (Bms[bmsNum] != channel)
		{
			if (Bms[bmsNum] != null)
			{
				Bms[bmsNum].Services["DL_High_Voltage_Lock"].ServiceCompleteEvent -= BmsServiceCompleteEvent;
			}
			Bms[bmsNum] = channel;
			if (Bms[bmsNum] != null)
			{
				Bms[bmsNum].Services["DL_High_Voltage_Lock"].ServiceCompleteEvent += BmsServiceCompleteEvent;
			}
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (EcpcOnline)
		{
			ReadInitialParameters();
		}
		UpdateUserInterface();
	}

	private void ReadInitialParameters()
	{
		if (EcpcOnline && ecpc01tChannel.CommunicationsState == CommunicationsState.Online && ecpc01tChannel.Parameters != null && numberofStringsParameter != null && !numberofStringsParameter.HasBeenReadFromEcu)
		{
			ecpc01tChannel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersInitialReadCompleteEvent;
			ecpc01tChannel.Parameters.ReadGroup(numberofStringsParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
		UpdateUserInterface();
	}

	private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
	{
		ecpc01tChannel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersInitialReadCompleteEvent;
		UpdateUserInterface();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			for (int i = 0; i < BmsEcus.Count(); i++)
			{
				SetBms(i, null);
			}
		}
	}

	private void UpdateUserInterface()
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Invalid comparison between Unknown and I4
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b9: Expected O, but got Unknown
		//IL_00c0: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Invalid comparison between Unknown and I4
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Invalid comparison between Unknown and I4
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Invalid comparison between Unknown and I4
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Invalid comparison between Unknown and I4
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Invalid comparison between Unknown and I4
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Invalid comparison between Unknown and I4
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Invalid comparison between Unknown and I4
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Invalid comparison between Unknown and I4
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Invalid comparison between Unknown and I4
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Invalid comparison between Unknown and I4
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Invalid comparison between Unknown and I4
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Invalid comparison between Unknown and I4
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Invalid comparison between Unknown and I4
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Invalid comparison between Unknown and I4
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Invalid comparison between Unknown and I4
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Invalid comparison between Unknown and I4
		if (BatteryCount != previousBatteryCount)
		{
			((Control)(object)tableLayoutPanel1).SuspendLayout();
			for (int i = 0; i < MaxBatteryCount; i++)
			{
				DigitalReadoutInstrument val = (DigitalReadoutInstrument)((TableLayoutPanel)(object)tableLayoutPanel1).Controls[$"digitalReadoutInstrumentTransportLock{i + 1}"];
				RunServiceButton val2 = (RunServiceButton)((TableLayoutPanel)(object)tableLayoutPanel1).Controls[$"runServiceButtonBMS{i + 1}Lock"];
				bool flag = (((Control)(RunServiceButton)((TableLayoutPanel)(object)tableLayoutPanel1).Controls[$"runServiceButtonBMS{i + 1}Unlock"]).Visible = i < BatteryCount);
				flag = (((Control)val2).Visible = flag);
				((Control)val).Visible = flag;
			}
			((Control)(object)tableLayoutPanel1).ResumeLayout();
			previousBatteryCount = BatteryCount;
		}
		labelInterlockWarning.Visible = !VehicleCheckOk;
		((Control)(object)runServiceButtonBMS1Lock).Enabled = (int)digitalReadoutInstrumentTransportLock1.RepresentedState == 1 && VehicleCheckOk && ((Control)(object)runServiceButtonBMS1Lock).Visible;
		((Control)(object)runServiceButtonBMS1Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock1.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS2Lock).Enabled = (int)digitalReadoutInstrumentTransportLock2.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS2Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock2.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS3Lock).Enabled = (int)digitalReadoutInstrumentTransportLock3.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS3Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock3.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS4Lock).Enabled = (int)digitalReadoutInstrumentTransportLock4.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS4Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock4.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS5Lock).Enabled = (int)digitalReadoutInstrumentTransportLock5.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS5Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock5.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS6Lock).Enabled = (int)digitalReadoutInstrumentTransportLock6.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS6Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock6.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS7Lock).Enabled = (int)digitalReadoutInstrumentTransportLock7.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS7Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock7.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS8Lock).Enabled = (int)digitalReadoutInstrumentTransportLock8.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS8Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock8.RepresentedState == 3 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS9Lock).Enabled = (int)digitalReadoutInstrumentTransportLock9.RepresentedState == 1 && VehicleCheckOk;
		((Control)(object)runServiceButtonBMS9Unlock).Enabled = (int)digitalReadoutInstrumentTransportLock9.RepresentedState == 3 && VehicleCheckOk;
	}

	private void PreconditionRepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void TransportLockRepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void BmsServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null && service.Channel.EcuInfos["DT_STO_High_Voltage_Lock_High_Voltage_Lock"] != null)
		{
			service.Channel.EcuInfos["DT_STO_High_Voltage_Lock_High_Voltage_Lock"].Read(synchronous: false);
		}
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
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
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
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
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_101d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1284: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_141e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1685: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_173f: Unknown result type (might be due to invalid IL or missing references)
		//IL_182d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		labelInterlockWarning = new System.Windows.Forms.Label();
		label39 = new System.Windows.Forms.Label();
		runServiceButtonBMS9Unlock = new RunServiceButton();
		runServiceButtonBMS8Unlock = new RunServiceButton();
		runServiceButtonBMS7Unlock = new RunServiceButton();
		runServiceButtonBMS6Unlock = new RunServiceButton();
		runServiceButtonBMS5Unlock = new RunServiceButton();
		runServiceButtonBMS4Unlock = new RunServiceButton();
		runServiceButtonBMS3Unlock = new RunServiceButton();
		runServiceButtonBMS2Unlock = new RunServiceButton();
		runServiceButtonBMS9Lock = new RunServiceButton();
		runServiceButtonBMS8Lock = new RunServiceButton();
		runServiceButtonBMS7Lock = new RunServiceButton();
		runServiceButtonBMS6Lock = new RunServiceButton();
		runServiceButtonBMS5Lock = new RunServiceButton();
		runServiceButtonBMS4Lock = new RunServiceButton();
		runServiceButtonBMS3Lock = new RunServiceButton();
		runServiceButtonBMS2Lock = new RunServiceButton();
		digitalReadoutInstrumentTransportLock9 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock8 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock7 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock6 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock4 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransportLock1 = new DigitalReadoutInstrument();
		runServiceButtonBMS1Lock = new RunServiceButton();
		runServiceButtonBMS1Unlock = new RunServiceButton();
		labelBatteryManagement = new System.Windows.Forms.Label();
		labelTransportLockBitStatus = new System.Windows.Forms.Label();
		labelTransportLockBitControl = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS9Unlock, 3, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS8Unlock, 3, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS7Unlock, 3, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS6Unlock, 3, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS5Unlock, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS4Unlock, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS3Unlock, 3, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS2Unlock, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS9Lock, 2, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS8Lock, 2, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS7Lock, 2, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS6Lock, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS5Lock, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS4Lock, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS3Lock, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS2Lock, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock9, 1, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock8, 1, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock7, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock6, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock5, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock4, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock3, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock2, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransportLock1, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS1Lock, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonBMS1Unlock, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelBatteryManagement, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelTransportLockBitStatus, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelTransportLockBitControl, 2, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentCharging, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(labelInterlockWarning, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label39, 0, 2);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel4, 10);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
		digitalReadoutInstrumentCharging.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).FreezeValue = false;
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition");
		((Control)(object)digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentCharging.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)3, 5, "mph");
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(5, 2147483647.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(labelInterlockWarning, "labelInterlockWarning");
		labelInterlockWarning.ForeColor = Color.Red;
		labelInterlockWarning.Name = "labelInterlockWarning";
		labelInterlockWarning.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label39, "label39");
		label39.Name = "label39";
		label39.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonBMS9Unlock, "runServiceButtonBMS9Unlock");
		((Control)(object)runServiceButtonBMS9Unlock).Name = "runServiceButtonBMS9Unlock";
		runServiceButtonBMS9Unlock.ServiceCall = new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS8Unlock, "runServiceButtonBMS8Unlock");
		((Control)(object)runServiceButtonBMS8Unlock).Name = "runServiceButtonBMS8Unlock";
		runServiceButtonBMS8Unlock.ServiceCall = new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS7Unlock, "runServiceButtonBMS7Unlock");
		((Control)(object)runServiceButtonBMS7Unlock).Name = "runServiceButtonBMS7Unlock";
		runServiceButtonBMS7Unlock.ServiceCall = new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS6Unlock, "runServiceButtonBMS6Unlock");
		((Control)(object)runServiceButtonBMS6Unlock).Name = "runServiceButtonBMS6Unlock";
		runServiceButtonBMS6Unlock.ServiceCall = new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS5Unlock, "runServiceButtonBMS5Unlock");
		((Control)(object)runServiceButtonBMS5Unlock).Name = "runServiceButtonBMS5Unlock";
		runServiceButtonBMS5Unlock.ServiceCall = new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS4Unlock, "runServiceButtonBMS4Unlock");
		((Control)(object)runServiceButtonBMS4Unlock).Name = "runServiceButtonBMS4Unlock";
		runServiceButtonBMS4Unlock.ServiceCall = new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS3Unlock, "runServiceButtonBMS3Unlock");
		((Control)(object)runServiceButtonBMS3Unlock).Name = "runServiceButtonBMS3Unlock";
		runServiceButtonBMS3Unlock.ServiceCall = new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS2Unlock, "runServiceButtonBMS2Unlock");
		((Control)(object)runServiceButtonBMS2Unlock).Name = "runServiceButtonBMS2Unlock";
		runServiceButtonBMS2Unlock.ServiceCall = new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		componentResourceManager.ApplyResources(runServiceButtonBMS9Lock, "runServiceButtonBMS9Lock");
		((Control)(object)runServiceButtonBMS9Lock).Name = "runServiceButtonBMS9Lock";
		runServiceButtonBMS9Lock.ServiceCall = new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS8Lock, "runServiceButtonBMS8Lock");
		((Control)(object)runServiceButtonBMS8Lock).Name = "runServiceButtonBMS8Lock";
		runServiceButtonBMS8Lock.ServiceCall = new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS7Lock, "runServiceButtonBMS7Lock");
		((Control)(object)runServiceButtonBMS7Lock).Name = "runServiceButtonBMS7Lock";
		runServiceButtonBMS7Lock.ServiceCall = new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS6Lock, "runServiceButtonBMS6Lock");
		((Control)(object)runServiceButtonBMS6Lock).Name = "runServiceButtonBMS6Lock";
		runServiceButtonBMS6Lock.ServiceCall = new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS5Lock, "runServiceButtonBMS5Lock");
		((Control)(object)runServiceButtonBMS5Lock).Name = "runServiceButtonBMS5Lock";
		runServiceButtonBMS5Lock.ServiceCall = new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS4Lock, "runServiceButtonBMS4Lock");
		((Control)(object)runServiceButtonBMS4Lock).Name = "runServiceButtonBMS4Lock";
		runServiceButtonBMS4Lock.ServiceCall = new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS3Lock, "runServiceButtonBMS3Lock");
		((Control)(object)runServiceButtonBMS3Lock).Name = "runServiceButtonBMS3Lock";
		runServiceButtonBMS3Lock.ServiceCall = new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS2Lock, "runServiceButtonBMS2Lock");
		((Control)(object)runServiceButtonBMS2Lock).Name = "runServiceButtonBMS2Lock";
		runServiceButtonBMS2Lock.ServiceCall = new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock9, "digitalReadoutInstrumentTransportLock9");
		digitalReadoutInstrumentTransportLock9.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock9).FreezeValue = false;
		digitalReadoutInstrumentTransportLock9.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock9.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock9.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock9.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock9).Instrument = new Qualifier((QualifierTypes)8, "BMS901T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock9).Name = "digitalReadoutInstrumentTransportLock9";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock8, "digitalReadoutInstrumentTransportLock8");
		digitalReadoutInstrumentTransportLock8.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock8).FreezeValue = false;
		digitalReadoutInstrumentTransportLock8.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock8.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock8.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock8.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock8).Instrument = new Qualifier((QualifierTypes)8, "BMS801T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock8).Name = "digitalReadoutInstrumentTransportLock8";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock7, "digitalReadoutInstrumentTransportLock7");
		digitalReadoutInstrumentTransportLock7.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock7).FreezeValue = false;
		digitalReadoutInstrumentTransportLock7.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock7.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock7.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock7.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock7).Instrument = new Qualifier((QualifierTypes)8, "BMS701T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock7).Name = "digitalReadoutInstrumentTransportLock7";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock6, "digitalReadoutInstrumentTransportLock6");
		digitalReadoutInstrumentTransportLock6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock6).FreezeValue = false;
		digitalReadoutInstrumentTransportLock6.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock6.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock6.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock6.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock6).Instrument = new Qualifier((QualifierTypes)8, "BMS601T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock6).Name = "digitalReadoutInstrumentTransportLock6";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock5, "digitalReadoutInstrumentTransportLock5");
		digitalReadoutInstrumentTransportLock5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock5).FreezeValue = false;
		digitalReadoutInstrumentTransportLock5.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock5.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock5.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock5.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock5).Instrument = new Qualifier((QualifierTypes)8, "BMS501T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock5).Name = "digitalReadoutInstrumentTransportLock5";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock4, "digitalReadoutInstrumentTransportLock4");
		digitalReadoutInstrumentTransportLock4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock4).FreezeValue = false;
		digitalReadoutInstrumentTransportLock4.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock4.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock4.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock4.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock4).Instrument = new Qualifier((QualifierTypes)8, "BMS401T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock4).Name = "digitalReadoutInstrumentTransportLock4";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock3, "digitalReadoutInstrumentTransportLock3");
		digitalReadoutInstrumentTransportLock3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock3).FreezeValue = false;
		digitalReadoutInstrumentTransportLock3.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock3.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock3.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock3.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock3).Instrument = new Qualifier((QualifierTypes)8, "BMS301T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock3).Name = "digitalReadoutInstrumentTransportLock3";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock2, "digitalReadoutInstrumentTransportLock2");
		digitalReadoutInstrumentTransportLock2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock2).FreezeValue = false;
		digitalReadoutInstrumentTransportLock2.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock2.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock2.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock2.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock2).Instrument = new Qualifier((QualifierTypes)8, "BMS201T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock2).Name = "digitalReadoutInstrumentTransportLock2";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransportLock1, "digitalReadoutInstrumentTransportLock1");
		digitalReadoutInstrumentTransportLock1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock1).FreezeValue = false;
		digitalReadoutInstrumentTransportLock1.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentTransportLock1.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransportLock1.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransportLock1.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock1).Instrument = new Qualifier((QualifierTypes)8, "BMS01T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
		((Control)(object)digitalReadoutInstrumentTransportLock1).Name = "digitalReadoutInstrumentTransportLock1";
		((SingleInstrumentBase)digitalReadoutInstrumentTransportLock1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runServiceButtonBMS1Lock, "runServiceButtonBMS1Lock");
		((Control)(object)runServiceButtonBMS1Lock).Name = "runServiceButtonBMS1Lock";
		runServiceButtonBMS1Lock.ServiceCall = new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" });
		componentResourceManager.ApplyResources(runServiceButtonBMS1Unlock, "runServiceButtonBMS1Unlock");
		((Control)(object)runServiceButtonBMS1Unlock).Name = "runServiceButtonBMS1Unlock";
		runServiceButtonBMS1Unlock.ServiceCall = new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" });
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelBatteryManagement, 4);
		componentResourceManager.ApplyResources(labelBatteryManagement, "labelBatteryManagement");
		labelBatteryManagement.Name = "labelBatteryManagement";
		labelBatteryManagement.UseCompatibleTextRendering = true;
		labelTransportLockBitStatus.BorderStyle = BorderStyle.FixedSingle;
		componentResourceManager.ApplyResources(labelTransportLockBitStatus, "labelTransportLockBitStatus");
		labelTransportLockBitStatus.Name = "labelTransportLockBitStatus";
		labelTransportLockBitStatus.UseCompatibleTextRendering = true;
		labelTransportLockBitControl.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelTransportLockBitControl, 2);
		componentResourceManager.ApplyResources(labelTransportLockBitControl, "labelTransportLockBitControl");
		labelTransportLockBitControl.Name = "labelTransportLockBitControl";
		labelTransportLockBitControl.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Battery_Management");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
