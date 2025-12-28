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

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string CylinderCutStartStopRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder";

	private const string AllCylindersOnRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Stop";

	private const string SetTmModeQualifier = "RT_SR09A_Force_TMC_to_TMx_Mode_Start";

	private const int MaxIdleSpeed = 1000;

	private const int IdleSpeedStep = 100;

	private Button[] onButtons;

	private Button[] offButtons;

	private ServiceCall idleSpeedSetRoutine;

	private ServiceCall idleSpeedResetRoutine;

	private Service setTmMode = null;

	private bool setTm5Mode = true;

	private bool stopTm5Mode = false;

	private int CylinderCount = 6;

	private static readonly Regex ValidIdleSpeedCharacters = new Regex("[0123456789]", RegexOptions.Compiled);

	private Channel mcm = null;

	private ChartInstrument chartInstrument;

	private DigitalReadoutInstrument DigitalReadoutInstrument3;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument4;

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

	private Button cylinder1On;

	private Button cylinder2On;

	private Button cylinder3On;

	private Button cylinder4On;

	private Button cylinder5On;

	private Button cylinder6On;

	private Button cylinder1Off;

	private Button cylinder2Off;

	private Button cylinder3Off;

	private Button cylinder4Off;

	private Button cylinder5Off;

	private Button cylinder6Off;

	private Button decButton;

	private Button incButton;

	private TextBox idleSpeedText;

	private TextBox textBoxResults;

	private TableLayoutPanel tableLayoutPanelTMMode;

	private Button buttonSetTM5Mode;

	private Button buttonStopTM5Mode;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private Button buttonIdleSpeedReset;

	private int MinIdleSpeed
	{
		get
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
				return ((ElectronicsFamily)(ref family)).Category == "Engine";
			});
			return ((EquipmentType)(ref val)).Name switch
			{
				"DD5" => 800, 
				"DD8" => 700, 
				_ => 600, 
			};
		}
	}

	private bool Online => mcm != null && mcm.CommunicationsState == CommunicationsState.Online;

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

	private bool CanTurnCylinderOnOrOff => Online;

	private bool CanTurnAllCylindersOn => Online;

	private bool CanSetTmMode => Online && setTmMode != null;

	private bool CanSetTm5Mode => Online && setTmMode != null && setTm5Mode;

	private bool CanStopTm5Mode => Online && setTmMode != null && stopTm5Mode;

	public UserPanel()
	{
		WarningManager.SetJobName(Resources.Message_ThisChange);
		InitializeComponent();
		onButtons = new Button[6] { cylinder1On, cylinder2On, cylinder3On, cylinder4On, cylinder5On, cylinder6On };
		offButtons = new Button[6] { cylinder1Off, cylinder2Off, cylinder3Off, cylinder4Off, cylinder5Off, cylinder6Off };
		cylinder1On.Click += OnCylinderOnClick;
		cylinder2On.Click += OnCylinderOnClick;
		cylinder3On.Click += OnCylinderOnClick;
		cylinder4On.Click += OnCylinderOnClick;
		cylinder5On.Click += OnCylinderOnClick;
		cylinder6On.Click += OnCylinderOnClick;
		cylinder1Off.Click += OnCylinderOffClick;
		cylinder2Off.Click += OnCylinderOffClick;
		cylinder3Off.Click += OnCylinderOffClick;
		cylinder4Off.Click += OnCylinderOffClick;
		cylinder5Off.Click += OnCylinderOffClick;
		cylinder6Off.Click += OnCylinderOffClick;
		allOnButton.Click += OnAllCylindersOnClick;
		idleSpeedText.KeyPress += OnIdleSpeedKeyPress;
		idleSpeedText.TextChanged += OnIdleSpeedChanged;
		incButton.Click += OnIncrementClick;
		decButton.Click += OnDecrementClick;
		buttonIdleSpeedReset.Click += OnResetClick;
		buttonSetTM5Mode.Click += OnSetTM5ModeClick;
		buttonStopTM5Mode.Click += OnStopTM5ModeClick;
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	public override void OnChannelsChanged()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		SetMCM(((CustomPanel)this).GetChannel("MCM21T"));
		UpdateUserInterface();
		idleSpeedSetRoutine = SapiManager.GetSetEngineSpeedService(SapiManager.GlobalInstance.ActiveChannels, new string[0]);
		idleSpeedResetRoutine = SapiManager.GetCancelSetEngineSpeedService(SapiManager.GlobalInstance.ActiveChannels);
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm != mcm)
		{
			WarningManager.Reset();
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			if (this.mcm == null && mcm != null)
			{
				ClearResults();
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				UpdateConnectedEquipmentType();
				setTmMode = this.mcm.Services["RT_SR09A_Force_TMC_to_TMx_Mode_Start"];
			}
		}
	}

	private void UpdateUserInterface()
	{
		if (setTmMode == null)
		{
			((Control)(object)tableLayoutPanelTMMode).Visible = false;
		}
		else
		{
			buttonSetTM5Mode.Enabled = CanSetTm5Mode;
			buttonStopTM5Mode.Enabled = CanStopTm5Mode;
		}
		bool canTurnCylinderOnOrOff = CanTurnCylinderOnOrOff;
		Button[] array = onButtons;
		foreach (Button button in array)
		{
			button.Enabled = canTurnCylinderOnOrOff;
		}
		array = offButtons;
		foreach (Button button2 in array)
		{
			button2.Enabled = canTurnCylinderOnOrOff;
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

	private void OnCylinderOnClick(object sender, EventArgs e)
	{
		Button button = sender as Button;
		SwitchCylinder(Convert.ToInt32(button.Tag, CultureInfo.InvariantCulture), on: true);
	}

	private void OnCylinderOffClick(object sender, EventArgs e)
	{
		Button button = sender as Button;
		SwitchCylinder(Convert.ToInt32(button.Tag, CultureInfo.InvariantCulture), on: false);
	}

	private void OnAllCylindersOnClick(object sender, EventArgs e)
	{
		SwitchAllCylindersOn();
	}

	private void OnResetClick(object sender, EventArgs e)
	{
		ResetIdleSpeed();
	}

	private void OnSetTM5ModeClick(object sender, EventArgs e)
	{
		SetTmModeFunction(5);
	}

	private void OnStopTM5ModeClick(object sender, EventArgs e)
	{
		SetTmModeFunction(0);
	}

	private void SwitchCylinder(int number, bool on)
	{
		if (!CanTurnCylinderOnOrOff || !WarningManager.RequestContinue())
		{
			return;
		}
		Service service = mcm.Services["RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder"];
		if (service != null)
		{
			service.ServiceCompleteEvent += OnServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(number);
			service.InputValues[1].Value = service.InputValues[1].Choices.GetItemFromRawValue(on ? 1 : 0);
			if (on)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_EnablingCylinder0, number), withNewLine: false);
			}
			else
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CuttingCylinder0, number), withNewLine: false);
			}
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
			Service service = mcm.Services["RT_SR004_Engine_Cylinder_Cut_Off_Stop"];
			if (service != null)
			{
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
				int idleSpeed = IdleSpeed;
				service.ServiceCompleteEvent += OnServiceCompleteEvent;
				service.InputValues[0].Value = (double)idleSpeed;
				ReportResult(string.Format(Resources.MessageFormat_SettingIdleSpeedTo0Rpm, idleSpeed), withNewLine: false);
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

	private void SetTmModeFunction(int mode)
	{
		if (!CanSetTmMode || !WarningManager.RequestContinue())
		{
			return;
		}
		if (setTmMode != null)
		{
			setTmMode.ServiceCompleteEvent += OnServiceCompleteEvent;
			setTmMode.InputValues[0].Value = setTmMode.InputValues[0].Choices.GetItemFromRawValue(mode);
			if (mode == 5)
			{
				ReportResult(Resources.MessgeFormat_SettingTmModeActive, withNewLine: false);
				stopTm5Mode = true;
				setTm5Mode = false;
			}
			else
			{
				ReportResult(Resources.MessageFormat_StoppingTmMode, withNewLine: false);
				setTm5Mode = true;
				stopTm5Mode = false;
			}
			setTmMode.Execute(synchronous: false);
		}
		else
		{
			ReportResult(Resources.Message_ErrorCouldNotFindService, withNewLine: true);
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

	private int GetCylinderCount(string equipment)
	{
		switch (equipment)
		{
		case "DD5":
		case "MDEG 4-Cylinder Tier4":
			return 4;
		default:
			return 6;
		}
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category == "Engine";
		});
		if (val != EquipmentType.Empty)
		{
			int cylinderCount = GetCylinderCount(((EquipmentType)(ref val)).Name);
			if (cylinderCount != CylinderCount)
			{
				CylinderCount = cylinderCount;
				Label obj = label7;
				Button button = cylinder5Off;
				bool flag = (cylinder5On.Visible = CylinderCount == 6);
				flag = (button.Visible = flag);
				((Control)(object)obj).Visible = flag;
				Label obj2 = label8;
				Button button2 = cylinder6Off;
				flag = (cylinder6On.Visible = CylinderCount == 6);
				flag = (button2.Visible = flag);
				((Control)(object)obj2).Visible = flag;
			}
			idleSpeedText.Text = MinIdleSpeed.ToString();
		}
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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
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
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1067: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
		chartInstrument = new ChartInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		label8 = new Label();
		label2 = new Label();
		label1 = new Label();
		allOnButton = new Button();
		label3 = new Label();
		label4 = new Label();
		label5 = new Label();
		label6 = new Label();
		label7 = new Label();
		cylinder1On = new Button();
		cylinder2On = new Button();
		cylinder3On = new Button();
		cylinder4On = new Button();
		cylinder5On = new Button();
		cylinder6On = new Button();
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
		tableLayoutPanelTMMode = new TableLayoutPanel();
		buttonSetTM5Mode = new Button();
		buttonStopTM5Mode = new Button();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanelTMMode).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument3, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)chartInstrument, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelTMMode, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 4, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
		DigitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS003_Actual_Torque");
		((Control)(object)DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
		((SingleInstrumentBase)DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)chartInstrument, 5);
		componentResourceManager.ApplyResources(chartInstrument, "chartInstrument");
		((Collection<Qualifier>)(object)chartInstrument.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS003_Actual_Torque"));
		((Control)(object)chartInstrument).Name = "chartInstrument";
		chartInstrument.SelectedTime = null;
		chartInstrument.ShowButtonPanel = false;
		chartInstrument.ShowEvents = false;
		chartInstrument.ShowLegend = false;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label8, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label2, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(allOnButton, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label6, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder1On, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder2On, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder3On, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder4On, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder5On, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(cylinder6On, 1, 7);
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
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
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
		componentResourceManager.ApplyResources(cylinder6On, "cylinder6On");
		cylinder6On.Name = "cylinder6On";
		cylinder6On.Tag = "6";
		cylinder6On.UseCompatibleTextRendering = true;
		cylinder6On.UseVisualStyleBackColor = true;
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
		componentResourceManager.ApplyResources(tableLayoutPanelTMMode, "tableLayoutPanelTMMode");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelTMMode, 5);
		((TableLayoutPanel)(object)tableLayoutPanelTMMode).Controls.Add(buttonSetTM5Mode, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTMMode).Controls.Add(buttonStopTM5Mode, 2, 0);
		((Control)(object)tableLayoutPanelTMMode).Name = "tableLayoutPanelTMMode";
		componentResourceManager.ApplyResources(buttonSetTM5Mode, "buttonSetTM5Mode");
		buttonSetTM5Mode.Name = "buttonSetTM5Mode";
		buttonSetTM5Mode.UseCompatibleTextRendering = true;
		buttonSetTM5Mode.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStopTM5Mode, "buttonStopTM5Mode");
		buttonStopTM5Mode.Name = "buttonStopTM5Mode";
		buttonStopTM5Mode.UseCompatibleTextRendering = true;
		buttonStopTM5Mode.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS190_Thermomanagement_status");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CylinderCutoutTestManual");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanelTMMode).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
