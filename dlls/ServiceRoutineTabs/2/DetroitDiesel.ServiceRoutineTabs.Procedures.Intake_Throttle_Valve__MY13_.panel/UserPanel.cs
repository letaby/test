using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string ITVStartRoutine = "RT_SR068_Control_IAT_Start_status";

	private const string ITVStopRoutine = "RT_SR068_Control_IAT_Stop";

	private const string EngineSpeedInstrumentName = "DT_AS010_Engine_Speed";

	private const int MinCommandedValue = 0;

	private const int MaxCommandedValue = 100;

	private const int PositionStep = 10;

	private static readonly Regex ValidCommandedValueCharacters = new Regex("[0123456789]", RegexOptions.Compiled);

	private string ErrorServiceNotFound = Resources.Message_TheMCM21TConnectedIsNotSupported;

	private Channel mcm = null;

	private Instrument engineSpeedInstrument = null;

	private bool manipulating = false;

	private DigitalReadoutInstrument vehicleCheckInstrument;

	private DigitalReadoutInstrument itvActualInstrument;

	private ScalingLabel titleLabel;

	private Label positionHeader;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private TextBox textboxResults;

	private Button buttonStart;

	private Button buttonStop;

	private Button buttonCommandedValueOpen;

	private Button buttonDecrement;

	private TextBox textboxCommandedValue;

	private Button buttonIncrement;

	private Button buttonCommandedValueClose;

	private Label labelCylindersHeader;

	private bool Online => mcm != null && mcm.CommunicationsState == CommunicationsState.Online;

	private bool ValidCommandedValue
	{
		get
		{
			bool result = false;
			int commandedValue = CommandedValue;
			if (commandedValue >= 0 && commandedValue <= 100)
			{
				result = true;
			}
			return result;
		}
	}

	private int CommandedValue
	{
		get
		{
			int result = -1;
			string text = textboxCommandedValue.Text;
			if (ValidCommandedValueCharacters.IsMatch(text))
			{
				int.TryParse(text, out result);
			}
			return result;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			else if (value > 100)
			{
				value = 100;
			}
			if (value != CommandedValue)
			{
				textboxCommandedValue.Text = value.ToString();
				UpdateCommandedValueUI();
				CommandValue();
			}
		}
	}

	private bool CanDecrement
	{
		get
		{
			int commandedValue = CommandedValue;
			return CanManipulate && (commandedValue > 0 || commandedValue == -1);
		}
	}

	private bool CanIncrement
	{
		get
		{
			int commandedValue = CommandedValue;
			return CanManipulate && (commandedValue == -1 || commandedValue < 100);
		}
	}

	private bool CanStart => Online && ValidCommandedValue && ValidEngineSpeed && !manipulating;

	private bool ValidEngineSpeed => Online && EngineSpeed == 0;

	private int EngineSpeed
	{
		get
		{
			if (engineSpeedInstrument != null && engineSpeedInstrument.InstrumentValues.Current != null && int.TryParse(engineSpeedInstrument.InstrumentValues.Current.Value.ToString(), out var result))
			{
				return result;
			}
			return -1;
		}
	}

	private bool CanStop => Online && manipulating;

	private bool CanManipulate => Online && manipulating;

	private bool ServicesExist => mcm != null && mcm.Services["RT_SR068_Control_IAT_Start_status"] != null && mcm.Services["RT_SR068_Control_IAT_Stop"] != null;

	public UserPanel()
	{
		InitializeComponent();
		buttonStart.Click += OnStartClick;
		buttonStop.Click += OnStopClick;
		textboxCommandedValue.KeyPress += OnCommandedValueKeyPress;
		textboxCommandedValue.TextChanged += OnCommandedValueChanged;
		buttonIncrement.Click += OnIncrementClick;
		buttonDecrement.Click += OnDecrementClick;
		buttonCommandedValueClose.Click += OnCommandedValueCloseClick;
		buttonCommandedValueOpen.Click += OnCommandedValueOpenClick;
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM21T"));
		UpdateUserInterface();
	}

	private bool SetMCM(Channel mcm)
	{
		bool result = false;
		if (this.mcm != mcm)
		{
			result = true;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				if (engineSpeedInstrument != null)
				{
					engineSpeedInstrument.InstrumentUpdateEvent -= OnEngineSpeedUpdate;
					engineSpeedInstrument = null;
				}
				manipulating = false;
			}
			if (this.mcm == null && mcm != null)
			{
				ClearResults();
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				engineSpeedInstrument = ((CustomPanel)this).GetInstrument("MCM21T", "DT_AS010_Engine_Speed");
				if (engineSpeedInstrument != null)
				{
					engineSpeedInstrument.InstrumentUpdateEvent += OnEngineSpeedUpdate;
				}
			}
		}
		return result;
	}

	private void UpdateUserInterface()
	{
		buttonStart.Enabled = CanStart;
		buttonStop.Enabled = CanStop;
		UpdateCommandedValueUI();
	}

	private void UpdateCommandedValueUI()
	{
		bool canManipulate = CanManipulate;
		buttonIncrement.Enabled = CanIncrement;
		buttonDecrement.Enabled = CanDecrement;
		buttonCommandedValueClose.Enabled = canManipulate && CommandedValue != 100;
		buttonCommandedValueOpen.Enabled = canManipulate && CommandedValue != 0;
		textboxCommandedValue.Enabled = canManipulate;
	}

	private void CommandValue()
	{
		if (CanManipulate && ValidCommandedValue)
		{
			Service service = ((CustomPanel)this).GetService("MCM21T", "RT_SR068_Control_IAT_Start_status");
			if (service != null)
			{
				service.InputValues[0].Value = CommandedValue;
				service.ServiceCompleteEvent += OnServiceCompleteEvent;
				ReportResult(Resources.Message_ManipulatingIntakeThrottleValveTo + CommandedValue + "% : ", withNewLine: false);
				service.Execute(synchronous: false);
			}
			else
			{
				ReportResult(ErrorServiceNotFound, withNewLine: true);
			}
		}
	}

	private void StopManipulation()
	{
		if (CanStop)
		{
			Service service = ((CustomPanel)this).GetService("MCM21T", "RT_SR068_Control_IAT_Stop");
			if (service != null)
			{
				service.ServiceCompleteEvent += OnServiceCompleteEvent;
				ReportResult(Resources.Message_StoppingIntakeThrottleValveManipulation, withNewLine: false);
				service.Execute(synchronous: false);
				manipulating = false;
				UpdateUserInterface();
			}
			else
			{
				ReportResult(ErrorServiceNotFound, withNewLine: true);
			}
		}
	}

	private void ClearResults()
	{
		textboxResults.Text = string.Empty;
	}

	private void ReportResult(string text, bool withNewLine)
	{
		textboxResults.Text = textboxResults.Text + text + (withNewLine ? "\r\n" : string.Empty);
		textboxResults.SelectionStart = textboxResults.TextLength;
		textboxResults.SelectionLength = 0;
		textboxResults.ScrollToCaret();
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		if (EngineSpeed != 0)
		{
			if (manipulating)
			{
				ReportResult(Resources.Message_EngineStartedWhileIntakeThrottleValveManipulationInProgressStoppingNow, withNewLine: true);
				StopManipulation();
			}
		}
		else
		{
			UpdateUserInterface();
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

	private void OnDecrementClick(object sender, EventArgs e)
	{
		CommandedValue -= 10;
	}

	private void OnIncrementClick(object sender, EventArgs e)
	{
		CommandedValue += 10;
	}

	private void OnCommandedValueKeyPress(object sender, KeyPressEventArgs e)
	{
		if (!ValidCommandedValueCharacters.IsMatch(e.KeyChar.ToString()) && e.KeyChar != '\b')
		{
			e.Handled = true;
		}
		if (e.KeyChar == '\r' && ValidCommandedValue)
		{
			CommandValue();
		}
	}

	private void OnCommandedValueChanged(object sender, EventArgs e)
	{
		UpdateCommandedValueUI();
	}

	private void OnStartClick(object sender, EventArgs e)
	{
		manipulating = true;
		UpdateUserInterface();
		CommandValue();
	}

	private void OnStopClick(object sender, EventArgs e)
	{
		StopManipulation();
	}

	private void OnCommandedValueOpenClick(object sender, EventArgs e)
	{
		CommandedValue = 0;
	}

	private void OnCommandedValueCloseClick(object sender, EventArgs e)
	{
		CommandedValue = 100;
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
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		positionHeader = new Label();
		labelCylindersHeader = new Label();
		buttonStart = new Button();
		buttonStop = new Button();
		buttonCommandedValueOpen = new Button();
		buttonDecrement = new Button();
		textboxCommandedValue = new TextBox();
		buttonIncrement = new Button();
		buttonCommandedValueClose = new Button();
		textboxResults = new TextBox();
		titleLabel = new ScalingLabel();
		itvActualInstrument = new DigitalReadoutInstrument();
		vehicleCheckInstrument = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)positionHeader, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelCylindersHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStart, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStop, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonCommandedValueOpen, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonDecrement, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxCommandedValue, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonIncrement, 5, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonCommandedValueClose, 6, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxResults, 0, 2);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		positionHeader.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)positionHeader, 6);
		componentResourceManager.ApplyResources(positionHeader, "positionHeader");
		((Control)(object)positionHeader).Name = "positionHeader";
		positionHeader.Orientation = (TextOrientation)1;
		labelCylindersHeader.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelCylindersHeader, "labelCylindersHeader");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)labelCylindersHeader, 2);
		((Control)(object)labelCylindersHeader).Name = "labelCylindersHeader";
		labelCylindersHeader.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonCommandedValueOpen, "buttonCommandedValueOpen");
		buttonCommandedValueOpen.Name = "buttonCommandedValueOpen";
		buttonCommandedValueOpen.UseCompatibleTextRendering = true;
		buttonCommandedValueOpen.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonDecrement, "buttonDecrement");
		buttonDecrement.Name = "buttonDecrement";
		buttonDecrement.UseCompatibleTextRendering = true;
		buttonDecrement.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(textboxCommandedValue, "textboxCommandedValue");
		textboxCommandedValue.Name = "textboxCommandedValue";
		textboxCommandedValue.ShortcutsEnabled = false;
		componentResourceManager.ApplyResources(buttonIncrement, "buttonIncrement");
		buttonIncrement.Name = "buttonIncrement";
		buttonIncrement.UseCompatibleTextRendering = true;
		buttonIncrement.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonCommandedValueClose, "buttonCommandedValueClose");
		buttonCommandedValueClose.Name = "buttonCommandedValueClose";
		buttonCommandedValueClose.UseCompatibleTextRendering = true;
		buttonCommandedValueClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)textboxResults, 8);
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)titleLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)itvActualInstrument, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)vehicleCheckInstrument, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		titleLabel.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)titleLabel, 2);
		componentResourceManager.ApplyResources(titleLabel, "titleLabel");
		titleLabel.FontGroup = null;
		titleLabel.LineAlignment = StringAlignment.Center;
		((Control)(object)titleLabel).Name = "titleLabel";
		titleLabel.ShowBorder = false;
		componentResourceManager.ApplyResources(itvActualInstrument, "itvActualInstrument");
		itvActualInstrument.FontGroup = null;
		((SingleInstrumentBase)itvActualInstrument).FreezeValue = false;
		((SingleInstrumentBase)itvActualInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)itvActualInstrument).Name = "itvActualInstrument";
		((SingleInstrumentBase)itvActualInstrument).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(vehicleCheckInstrument, "vehicleCheckInstrument");
		vehicleCheckInstrument.FontGroup = null;
		((SingleInstrumentBase)vehicleCheckInstrument).FreezeValue = false;
		((SingleInstrumentBase)vehicleCheckInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)vehicleCheckInstrument).Name = "vehicleCheckInstrument";
		((SingleInstrumentBase)vehicleCheckInstrument).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_IntakeThrottleValve");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
