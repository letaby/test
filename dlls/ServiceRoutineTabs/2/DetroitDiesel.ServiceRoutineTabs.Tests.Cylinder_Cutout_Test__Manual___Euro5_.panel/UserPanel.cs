using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___Euro5_.panel;

public class UserPanel : CustomPanel
{
	private const int MaxIdleSpeed = 1000;

	private const int IdleSpeedStep = 100;

	private Button[] offButtons;

	private Button[] onButtons;

	private string CylinderCutoffStartRoutine;

	private string AllCylindersOnRoutine;

	private ServiceCall idleSpeedSetRoutine;

	private ServiceCall idleSpeedResetRoutine;

	private int CylinderCount = -1;

	private static readonly Regex ValidIdleSpeedCharacters = new Regex("[0123456789]", RegexOptions.Compiled);

	private int MinIdleSpeed = 600;

	private Channel mr2 = null;

	private byte cylinderState = byte.MaxValue;

	private ChartInstrument chartInstrument;

	private DigitalReadoutInstrument DigitalReadoutInstrument3;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private Label label2;

	private Label label1;

	private Button allOnButton;

	private Label label8;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private Button decButton;

	private Button incButton;

	private TextBox idleSpeedText;

	private TextBox textBoxResults;

	private Button cylinder1Off;

	private Button cylinder2Off;

	private Button cylinder3Off;

	private Button cylinder4Off;

	private Button cylinder5Off;

	private Button cylinder6Off;

	private TableLayoutPanel tableLayoutPanel3;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private Button cylinder6On;

	private Button cylinder1On;

	private Button cylinder2On;

	private Button cylinder3On;

	private Button cylinder4On;

	private Button cylinder5On;

	private Button buttonIdleSpeedReset;

	private bool Online => mr2 != null && mr2.CommunicationsState == CommunicationsState.Online;

	private bool ValidIdleSpeed
	{
		get
		{
			bool result = false;
			int idleSpeed = IdleSpeed;
			if (idleSpeed >= MinIdleSpeed && idleSpeed <= 1000)
			{
				result = true;
			}
			return result;
		}
	}

	private int IdleSpeed
	{
		get
		{
			int result = -1;
			string text = idleSpeedText.Text;
			if (ValidIdleSpeedCharacters.IsMatch(text))
			{
				int.TryParse(text, out result);
			}
			return result;
		}
		set
		{
			if (value < MinIdleSpeed)
			{
				value = MinIdleSpeed;
			}
			else if (value > 1000)
			{
				value = 1000;
			}
			if (value != IdleSpeed)
			{
				idleSpeedText.Text = value.ToString();
				UpdateIdleSpeedUI();
			}
		}
	}

	private bool CanDecrement
	{
		get
		{
			int idleSpeed = IdleSpeed;
			return Online && (idleSpeed > MinIdleSpeed || idleSpeed == -1);
		}
	}

	private bool CanIncrement
	{
		get
		{
			int idleSpeed = IdleSpeed;
			return Online && (idleSpeed == -1 || idleSpeed < 1000);
		}
	}

	private bool CanApply => Online && ValidIdleSpeed;

	private bool CanReset => Online;

	private bool VehicleCheckStatus => (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentParkingBrake.RepresentedState == 1;

	private bool CanTurnCylinderOnOrOff => Online && VehicleCheckStatus;

	private bool CanTurnAllCylindersOn => Online;

	public UserPanel()
	{
		WarningManager.SetJobName(Resources.Message_ThisChange);
		InitializeComponent();
		offButtons = new Button[6] { cylinder1Off, cylinder2Off, cylinder3Off, cylinder4Off, cylinder5Off, cylinder6Off };
		onButtons = new Button[6] { cylinder1On, cylinder2On, cylinder3On, cylinder4On, cylinder5On, cylinder6On };
		Button[] array = offButtons;
		foreach (Button button in array)
		{
			button.Click += button_CylinderOffClick;
		}
		array = onButtons;
		foreach (Button button in array)
		{
			button.Click += button_CylinderOnClick;
		}
		allOnButton.Click += OnAllCylindersOnClick;
		idleSpeedText.KeyPress += OnIdleSpeedKeyPress;
		idleSpeedText.TextChanged += OnIdleSpeedChanged;
		incButton.Click += OnIncrementClick;
		decButton.Click += OnDecrementClick;
		buttonIdleSpeedReset.Click += OnResetClick;
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	public override void OnChannelsChanged()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		SetMr2(((CustomPanel)this).GetChannel("MR201T"));
		UpdateUserInterface();
		idleSpeedSetRoutine = SapiManager.GetSetEngineSpeedService(SapiManager.GlobalInstance.ActiveChannels, new string[0]);
		idleSpeedResetRoutine = SapiManager.GetCancelSetEngineSpeedService(SapiManager.GlobalInstance.ActiveChannels);
	}

	private void SetMr2(Channel mr2)
	{
		if (this.mr2 != mr2)
		{
			WarningManager.Reset();
			if (this.mr2 != null)
			{
				this.mr2.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			if (this.mr2 == null && mr2 != null)
			{
				ClearResults();
			}
			this.mr2 = mr2;
			if (this.mr2 != null)
			{
				this.mr2.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				UpdateConnectedEquipmentType();
			}
		}
	}

	private void UpdateUserInterface()
	{
		foreach (Button item in offButtons.Union(onButtons))
		{
			item.Enabled = CanTurnCylinderOnOrOff;
		}
		allOnButton.Enabled = CanTurnAllCylindersOn;
		UpdateIdleSpeedUI();
	}

	private void UpdateIdleSpeedUI()
	{
		idleSpeedText.Enabled = Online;
		incButton.Enabled = CanIncrement;
		decButton.Enabled = CanDecrement;
		buttonIdleSpeedReset.Enabled = CanReset;
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnDecrementClick(object sender, EventArgs e)
	{
		IdleSpeed -= 100;
		SetIdleSpeed();
	}

	private void OnIncrementClick(object sender, EventArgs e)
	{
		IdleSpeed += 100;
		SetIdleSpeed();
	}

	private void OnIdleSpeedKeyPress(object sender, KeyPressEventArgs e)
	{
		if (!ValidIdleSpeedCharacters.IsMatch(e.KeyChar.ToString()) && e.KeyChar != '\b')
		{
			e.Handled = true;
		}
	}

	private void OnIdleSpeedChanged(object sender, EventArgs e)
	{
		UpdateIdleSpeedUI();
	}

	private void button_CylinderOffClick(object sender, EventArgs e)
	{
		Button button = sender as Button;
		SwitchCylinder(Convert.ToInt32(button.Tag, CultureInfo.InvariantCulture), on: false);
	}

	private void button_CylinderOnClick(object sender, EventArgs e)
	{
		Button button = sender as Button;
		SwitchCylinder(Convert.ToInt32(button.Tag, CultureInfo.InvariantCulture), on: true);
	}

	private void OnAllCylindersOnClick(object sender, EventArgs e)
	{
		SwitchAllCylindersOn();
	}

	private void OnResetClick(object sender, EventArgs e)
	{
		ResetIdleSpeed();
	}

	private void SwitchCylinder(int number, bool on)
	{
		if (!CanTurnCylinderOnOrOff || !WarningManager.RequestContinue())
		{
			return;
		}
		Service service = mr2.Services[CylinderCutoffStartRoutine];
		if (service != null)
		{
			service.ServiceCompleteEvent += OnServiceCompleteEvent;
			int num = 1 << number - 1;
			if (on)
			{
				cylinderState |= (byte)num;
			}
			else
			{
				cylinderState &= (byte)(~num);
			}
			service.InputValues[0].Value = new Dump(new byte[1] { cylinderState });
			ReportResult(string.Format(CultureInfo.CurrentCulture, on ? Resources.MessageFormat_EnablingCylinder0 : Resources.MessageFormat_CuttingCylinder0, number), withNewLine: false);
			service.Execute(synchronous: false);
		}
		else
		{
			ReportResult(Resources.Message_ErrorCouldNotFindService, withNewLine: true);
		}
	}

	private void SwitchAllCylindersOn()
	{
		if (CanTurnAllCylindersOn && WarningManager.RequestContinue())
		{
			Service service = mr2.Services[AllCylindersOnRoutine];
			if (service != null)
			{
				cylinderState = byte.MaxValue;
				service.ServiceCompleteEvent += OnServiceCompleteEvent;
				ReportResult(Resources.Message_EnablingAllCylinders, withNewLine: false);
				service.Execute(synchronous: false);
			}
			else
			{
				ReportResult(Resources.Message_ErrorCouldNotFindService, withNewLine: true);
			}
		}
	}

	private void SetIdleSpeed()
	{
		if (CanApply && WarningManager.RequestContinue())
		{
			Service service = ((ServiceCall)(ref idleSpeedSetRoutine)).GetService();
			if (service != null)
			{
				service.ServiceCompleteEvent += OnServiceCompleteEvent;
				service.InputValues[0].Value = (double)IdleSpeed;
				ReportResult(string.Format(Resources.MessageFormat_SettingIdleSpeedTo0Rpm, IdleSpeed), withNewLine: false);
				service.Execute(synchronous: false);
			}
			else
			{
				ReportResult(Resources.Message_ErrorCouldNotFindService, withNewLine: true);
			}
		}
	}

	private void ResetIdleSpeed()
	{
		if (CanReset && WarningManager.RequestContinue())
		{
			Service service = ((ServiceCall)(ref idleSpeedResetRoutine)).GetService();
			if (service != null)
			{
				service.ServiceCompleteEvent += OnServiceCompleteEvent;
				ReportResult(Resources.Message_ResettingIdleSpeed, withNewLine: false);
				service.Execute(synchronous: false);
				IdleSpeed = MinIdleSpeed;
			}
			else
			{
				ReportResult(Resources.Message_ErrorCouldNotFindService, withNewLine: true);
			}
		}
	}

	private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		string empty = string.Empty;
		empty = ((!e.Succeeded) ? string.Format(Resources.MessageFormat_Error0, e.Exception.Message) : ((e.Exception == null) ? Resources.Message_Done : string.Format(Resources.MessageFormat_Done0, e.Exception.Message)));
		ReportResult(empty, withNewLine: true);
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnServiceCompleteEvent;
	}

	private void ClearResults()
	{
		if (textBoxResults != null)
		{
			textBoxResults.Text = string.Empty;
		}
	}

	private void ReportResult(string text, bool withNewLine)
	{
		textBoxResults.Text = textBoxResults.Text + text + (withNewLine ? "\r\n" : string.Empty);
		textBoxResults.SelectionStart = textBoxResults.TextLength;
		textBoxResults.SelectionLength = 0;
		textBoxResults.ScrollToCaret();
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category == "Engine")
		{
			UpdateConnectedEquipmentType();
		}
	}

	private bool GetEngineDetails(string equipmentName)
	{
		bool flag = false;
		switch (equipmentName.ToUpper())
		{
		case "MBE900":
			flag = true;
			ReportResult(string.Format(Resources.MessageFormat_EngineRecognized, equipmentName), withNewLine: true);
			CylinderCutoffStartRoutine = "RT_SR0200_Single_Cylinder_Cutoff_Start";
			AllCylindersOnRoutine = "RT_SR0200_Single_Cylinder_Cutoff_Stop";
			CylinderCount = 6;
			MinIdleSpeed = 900;
			break;
		case "MBE4000":
			flag = true;
			ReportResult(string.Format(Resources.MessageFormat_EngineRecognized, equipmentName), withNewLine: true);
			CylinderCutoffStartRoutine = "RT_SR0200_Single_Cylinder_Cutoff_Start";
			AllCylindersOnRoutine = "RT_SR0200_Single_Cylinder_Cutoff_Stop";
			CylinderCount = 6;
			MinIdleSpeed = 600;
			break;
		default:
			flag = false;
			CylinderCutoffStartRoutine = string.Empty;
			AllCylindersOnRoutine = string.Empty;
			CylinderCount = 0;
			MinIdleSpeed = 0;
			ReportResult(string.Format(Resources.Message_Engine_Type_Not_Recognized, equipmentName), withNewLine: true);
			break;
		}
		return flag;
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Name == "MBE";
		});
		if (val != EquipmentType.Empty)
		{
			int cylinderCount = CylinderCount;
			if (GetEngineDetails(((EquipmentType)(ref val)).Name))
			{
				if (cylinderCount != CylinderCount)
				{
					Label obj = label7;
					bool visible = (cylinder5Off.Visible = CylinderCount == 6);
					((Control)(object)obj).Visible = visible;
					Label obj2 = label8;
					visible = (cylinder6Off.Visible = CylinderCount == 6);
					((Control)(object)obj2).Visible = visible;
				}
				idleSpeedText.Text = MinIdleSpeed.ToString();
			}
			else
			{
				ReportResult(string.Format(Resources.Message_Engine_Type_Not_Recognized, ((EquipmentType)(ref val)).Name), withNewLine: true);
			}
		}
		else
		{
			ReportResult(string.Format(Resources.Message_Engine_Type_Not_Recognized, "MBE engine family"), withNewLine: true);
		}
	}

	private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentParkingBrake_RepresentedStateChanged(object sender, EventArgs e)
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
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
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1029: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_113b: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
		chartInstrument = new ChartInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		cylinder6On = new Button();
		label8 = new Label();
		label2 = new Label();
		label1 = new Label();
		allOnButton = new Button();
		label3 = new Label();
		label4 = new Label();
		label5 = new Label();
		label6 = new Label();
		label7 = new Label();
		cylinder1Off = new Button();
		cylinder2Off = new Button();
		cylinder3Off = new Button();
		cylinder4Off = new Button();
		cylinder5Off = new Button();
		cylinder6Off = new Button();
		decButton = new Button();
		incButton = new Button();
		idleSpeedText = new TextBox();
		textBoxResults = new TextBox();
		buttonIdleSpeedReset = new Button();
		cylinder1On = new Button();
		cylinder2On = new Button();
		cylinder3On = new Button();
		cylinder4On = new Button();
		cylinder5On = new Button();
		tableLayoutPanel3 = new TableLayoutPanel();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument3, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)chartInstrument, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Coolant_Temperature");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Engine_Speed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
		DigitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Actual_torque_via_CAN");
		((Control)(object)DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
		((SingleInstrumentBase)DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)chartInstrument, 4);
		componentResourceManager.ApplyResources(chartInstrument, "chartInstrument");
		((Collection<Qualifier>)(object)chartInstrument.Instruments).Add(new Qualifier((QualifierTypes)1, "CPC04T", "DT_ASL_Actual_Torque"));
		((Control)(object)chartInstrument).Name = "chartInstrument";
		chartInstrument.SelectedTime = null;
		chartInstrument.ShowButtonPanel = false;
		chartInstrument.ShowEvents = false;
		chartInstrument.ShowLegend = false;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder6On, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label8, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label2, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(allOnButton, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label6, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder1Off, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder2Off, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder3Off, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder4Off, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder5Off, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder6Off, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(decButton, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(incButton, 5, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(idleSpeedText, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textBoxResults, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonIdleSpeedReset, 6, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder1On, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder2On, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder3On, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder4On, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder5On, 1, 6);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(cylinder6On, "cylinder6On");
		cylinder6On.Name = "cylinder6On";
		cylinder6On.Tag = "6";
		cylinder6On.UseCompatibleTextRendering = true;
		cylinder6On.UseVisualStyleBackColor = true;
		label8.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)(object)label8).Name = "label8";
		label8.Orientation = (TextOrientation)1;
		label2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label2, "label2");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)label2, 6);
		((Control)(object)label2).Name = "label2";
		label2.Orientation = (TextOrientation)1;
		label1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)label1, 3);
		((Control)(object)label1).Name = "label1";
		label1.Orientation = (TextOrientation)1;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)allOnButton, 3);
		componentResourceManager.ApplyResources(allOnButton, "allOnButton");
		allOnButton.Name = "allOnButton";
		allOnButton.UseCompatibleTextRendering = true;
		allOnButton.UseVisualStyleBackColor = true;
		label3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)(object)label3).Name = "label3";
		label3.Orientation = (TextOrientation)1;
		label4.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)(object)label4).Name = "label4";
		label4.Orientation = (TextOrientation)1;
		label5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)(object)label5).Name = "label5";
		label5.Orientation = (TextOrientation)1;
		label6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)(object)label6).Name = "label6";
		label6.Orientation = (TextOrientation)1;
		label7.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)(object)label7).Name = "label7";
		label7.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(cylinder1Off, "cylinder1Off");
		cylinder1Off.Name = "cylinder1Off";
		cylinder1Off.Tag = "1";
		cylinder1Off.UseCompatibleTextRendering = true;
		cylinder1Off.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder2Off, "cylinder2Off");
		cylinder2Off.Name = "cylinder2Off";
		cylinder2Off.Tag = "2";
		cylinder2Off.UseCompatibleTextRendering = true;
		cylinder2Off.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder3Off, "cylinder3Off");
		cylinder3Off.Name = "cylinder3Off";
		cylinder3Off.Tag = "3";
		cylinder3Off.UseCompatibleTextRendering = true;
		cylinder3Off.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder4Off, "cylinder4Off");
		cylinder4Off.Name = "cylinder4Off";
		cylinder4Off.Tag = "4";
		cylinder4Off.UseCompatibleTextRendering = true;
		cylinder4Off.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder5Off, "cylinder5Off");
		cylinder5Off.Name = "cylinder5Off";
		cylinder5Off.Tag = "5";
		cylinder5Off.UseCompatibleTextRendering = true;
		cylinder5Off.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder6Off, "cylinder6Off");
		cylinder6Off.Name = "cylinder6Off";
		cylinder6Off.Tag = "6";
		cylinder6Off.UseCompatibleTextRendering = true;
		cylinder6Off.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(decButton, "decButton");
		decButton.Name = "decButton";
		decButton.UseCompatibleTextRendering = true;
		decButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(incButton, "incButton");
		incButton.Name = "incButton";
		incButton.UseCompatibleTextRendering = true;
		incButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(idleSpeedText, "idleSpeedText");
		idleSpeedText.Name = "idleSpeedText";
		idleSpeedText.ReadOnly = true;
		idleSpeedText.ShortcutsEnabled = false;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)textBoxResults, 6);
		componentResourceManager.ApplyResources(textBoxResults, "textBoxResults");
		textBoxResults.Name = "textBoxResults";
		textBoxResults.ReadOnly = true;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetRowSpan((Control)textBoxResults, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)buttonIdleSpeedReset, 2);
		componentResourceManager.ApplyResources(buttonIdleSpeedReset, "buttonIdleSpeedReset");
		buttonIdleSpeedReset.Name = "buttonIdleSpeedReset";
		buttonIdleSpeedReset.UseCompatibleTextRendering = true;
		buttonIdleSpeedReset.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder1On, "cylinder1On");
		cylinder1On.Name = "cylinder1On";
		cylinder1On.Tag = "1";
		cylinder1On.UseCompatibleTextRendering = true;
		cylinder1On.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder2On, "cylinder2On");
		cylinder2On.Name = "cylinder2On";
		cylinder2On.Tag = "2";
		cylinder2On.UseCompatibleTextRendering = true;
		cylinder2On.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder3On, "cylinder3On");
		cylinder3On.Name = "cylinder3On";
		cylinder3On.Tag = "3";
		cylinder3On.UseCompatibleTextRendering = true;
		cylinder3On.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder4On, "cylinder4On");
		cylinder4On.Name = "cylinder4On";
		cylinder4On.Tag = "4";
		cylinder4On.UseCompatibleTextRendering = true;
		cylinder4On.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cylinder5On, "cylinder5On");
		cylinder5On.Name = "cylinder5On";
		cylinder5On.Tag = "5";
		cylinder5On.UseCompatibleTextRendering = true;
		cylinder5On.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = "Vehicle Check Status";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)2, 4);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Parking_Brake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += digitalReadoutInstrumentParkingBrake_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = "Vehicle Check Status";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Vehicle_speed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CylinderCutoutTestManual");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
