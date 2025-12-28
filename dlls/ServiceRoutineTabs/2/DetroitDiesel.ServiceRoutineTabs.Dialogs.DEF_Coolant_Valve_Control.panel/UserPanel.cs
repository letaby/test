using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control.panel;

public class UserPanel : CustomPanel
{
	private const int durationDefault = 30;

	private const int durationMinimum = 0;

	private const int durationMaximum = 300;

	private Timer durationTimer;

	private Channel acm;

	private TableLayoutPanel tableLayoutPanelBase;

	private BarInstrument barInstrumentCoolantTemperature;

	private RunServiceButton runServiceButtonCoolantValveOpenStart;

	private RunServiceButton runServiceButtonCoolantValveOpenStop;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantValve;

	private TableLayoutPanel tableLayoutPanel1;

	private DecimalTextBox decimalTextBoxDuration;

	private System.Windows.Forms.Label labelDuration;

	private System.Windows.Forms.Label labelSeconds;

	private TextBox textBoxOutput;

	private BarInstrument barInstrumentDefTankTemperature;

	private bool CoolantValveOpen => acm != null && acm.Online && (int)digitalReadoutInstrumentCoolantValve.RepresentedState == 1;

	private bool CanStart => Online && !((RunSharedProcedureButtonBase)runServiceButtonCoolantValveOpenStart).IsBusy && !durationTimer.Enabled;

	private bool CanStop => Online && !((RunSharedProcedureButtonBase)runServiceButtonCoolantValveOpenStop).IsBusy && durationTimer.Enabled;

	private bool CanClose => !durationTimer.Enabled;

	private bool Online => acm != null && acm.Online;

	public UserPanel()
	{
		InitializeComponent();
		durationTimer = new Timer();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	private void OnRunServiceButtonCoolantValveOpenStartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			CoolantValveOpenStart();
		}
		else
		{
			WriteMessage(string.Format(Resources.MessageFormat_TheValveFailedToOpen0, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
		}
	}

	private void OnRunServiceButtonCoolantValveOpenStopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		StopTimer();
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			UpdateServiceStoppedMessage();
		}
		else
		{
			WriteMessage(string.Format(Resources.MessageFormat_TheValveFailedToClose0, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty));
		}
	}

	private void OnDigitalReadoutInstrumentCoolantValveRepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		((CustomPanel)this).OnChannelsChanged();
		SetAcm(((CustomPanel)this).GetChannel("ACM02T"));
	}

	private void SetAcm(Channel acm)
	{
		if (this.acm == acm)
		{
			return;
		}
		if (this.acm != null)
		{
			this.acm.CommunicationsStateUpdateEvent -= OnAcmCommunicationsStateUpdateEvent;
			runServiceButtonCoolantValveOpenStart.ServiceComplete -= OnRunServiceButtonCoolantValveOpenStartServiceComplete;
			runServiceButtonCoolantValveOpenStop.ServiceComplete -= OnRunServiceButtonCoolantValveOpenStopServiceComplete;
			digitalReadoutInstrumentCoolantValve.RepresentedStateChanged -= OnDigitalReadoutInstrumentCoolantValveRepresentedStateChanged;
			if (durationTimer.Enabled && acm == null)
			{
				StopTimer();
				WriteMessage(Resources.Message_DisconnectionDetectedWhileProcedureRunningDEFCoolantValveMayStillBeOpen);
			}
		}
		this.acm = acm;
		if (this.acm != null)
		{
			this.acm.CommunicationsStateUpdateEvent += OnAcmCommunicationsStateUpdateEvent;
			runServiceButtonCoolantValveOpenStart.ServiceComplete += OnRunServiceButtonCoolantValveOpenStartServiceComplete;
			runServiceButtonCoolantValveOpenStop.ServiceComplete += OnRunServiceButtonCoolantValveOpenStopServiceComplete;
			digitalReadoutInstrumentCoolantValve.RepresentedStateChanged += OnDigitalReadoutInstrumentCoolantValveRepresentedStateChanged;
		}
		UpdateUserInterface();
	}

	private void OnAcmCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (CanClose)
		{
			SetAcm(null);
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void OnDurationTimerTick(object sender, EventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		StopTimer();
		if (Online)
		{
			ServiceCollection services = acm.Services;
			ServiceCall serviceCall = runServiceButtonCoolantValveOpenStop.ServiceCall;
			Service service = services[((ServiceCall)(ref serviceCall)).Qualifier];
			if (service != null)
			{
				service.ServiceCompleteEvent += OnStopServiceServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
		}
	}

	private void StopTimer()
	{
		durationTimer.Stop();
		durationTimer.Tick -= OnDurationTimerTick;
	}

	private void OnStopServiceServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= OnStopServiceServiceCompleteEvent;
			UpdateServiceStoppedMessage();
		}
	}

	private void UpdateServiceStoppedMessage()
	{
		WriteMessage(Resources.Message_TheDEFCoolantValveHasBeenSetToClosed);
		UpdateUserInterface();
	}

	private void CoolantValveOpenStart()
	{
		int num = 30;
		if (!string.IsNullOrEmpty(((Control)(object)decimalTextBoxDuration).Text) && decimalTextBoxDuration.Value > 0.0 && decimalTextBoxDuration.Value <= 300.0)
		{
			num = Convert.ToInt32(decimalTextBoxDuration.Value);
		}
		else
		{
			((Control)(object)decimalTextBoxDuration).Text = num.ToString();
		}
		durationTimer.Interval = num * 1000;
		durationTimer.Start();
		durationTimer.Tick += OnDurationTimerTick;
		ClearMessages();
		WriteMessage(string.Format(Resources.MessageFormat_TheDEFCoolantValveHasBeenSetToOpenFor0Seconds, num));
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		((Control)(object)runServiceButtonCoolantValveOpenStop).Enabled = CanStop;
		((Control)(object)runServiceButtonCoolantValveOpenStart).Enabled = CanStart;
		buttonClose.Enabled = CanClose;
	}

	private void ClearMessages()
	{
		textBoxOutput.Text = string.Empty;
	}

	private void WriteMessage(string message)
	{
		textBoxOutput.AppendText(message);
		textBoxOutput.AppendText("\r\n");
		((CustomPanel)this).AddStatusMessage(message);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelBase = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		decimalTextBoxDuration = new DecimalTextBox();
		labelDuration = new System.Windows.Forms.Label();
		labelSeconds = new System.Windows.Forms.Label();
		barInstrumentCoolantTemperature = new BarInstrument();
		barInstrumentDefTankTemperature = new BarInstrument();
		runServiceButtonCoolantValveOpenStart = new RunServiceButton();
		runServiceButtonCoolantValveOpenStop = new RunServiceButton();
		buttonClose = new Button();
		digitalReadoutInstrumentCoolantValve = new DigitalReadoutInstrument();
		textBoxOutput = new TextBox();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelBase).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)(object)tableLayoutPanel1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)decimalTextBoxDuration, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelDuration, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelSeconds, 2, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(decimalTextBoxDuration, "decimalTextBoxDuration");
		decimalTextBoxDuration.MaximumValue = 300.0;
		decimalTextBoxDuration.MinimumValue = 0.0;
		((Control)(object)decimalTextBoxDuration).Name = "decimalTextBoxDuration";
		decimalTextBoxDuration.Precision = 0;
		decimalTextBoxDuration.Value = 30.0;
		componentResourceManager.ApplyResources(labelDuration, "labelDuration");
		labelDuration.Name = "labelDuration";
		labelDuration.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelSeconds, "labelSeconds");
		labelSeconds.Name = "labelSeconds";
		labelSeconds.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelBase, "tableLayoutPanelBase");
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)barInstrumentCoolantTemperature, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)barInstrumentDefTankTemperature, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)runServiceButtonCoolantValveOpenStart, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)runServiceButtonCoolantValveOpenStop, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add(buttonClose, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)digitalReadoutInstrumentCoolantValve, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)tableLayoutPanel1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add(textBoxOutput, 0, 1);
		((Control)(object)tableLayoutPanelBase).Name = "tableLayoutPanelBase";
		barInstrumentCoolantTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentCoolantTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentCoolantTemperature, "barInstrumentCoolantTemperature");
		barInstrumentCoolantTemperature.FontGroup = "mainInstruments";
		((SingleInstrumentBase)barInstrumentCoolantTemperature).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)barInstrumentCoolantTemperature).Name = "barInstrumentCoolantTemperature";
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetRowSpan((Control)(object)barInstrumentCoolantTemperature, 3);
		((SingleInstrumentBase)barInstrumentCoolantTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentCoolantTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
		barInstrumentDefTankTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentDefTankTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentDefTankTemperature, "barInstrumentDefTankTemperature");
		barInstrumentDefTankTemperature.FontGroup = "mainInstruments";
		((SingleInstrumentBase)barInstrumentDefTankTemperature).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentDefTankTemperature).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS022_DEF_tank_Temperature");
		((Control)(object)barInstrumentDefTankTemperature).Name = "barInstrumentDefTankTemperature";
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetRowSpan((Control)(object)barInstrumentDefTankTemperature, 3);
		((SingleInstrumentBase)barInstrumentDefTankTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentDefTankTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentDefTankTemperature).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runServiceButtonCoolantValveOpenStart, "runServiceButtonCoolantValveOpenStart");
		((Control)(object)runServiceButtonCoolantValveOpenStart).Name = "runServiceButtonCoolantValveOpenStart";
		runServiceButtonCoolantValveOpenStart.ServiceCall = new ServiceCall("ACM02T", "RT_DSR_Coolant_Valve_Open_Start");
		componentResourceManager.ApplyResources(runServiceButtonCoolantValveOpenStop, "runServiceButtonCoolantValveOpenStop");
		((Control)(object)runServiceButtonCoolantValveOpenStop).Name = "runServiceButtonCoolantValveOpenStop";
		runServiceButtonCoolantValveOpenStop.ServiceCall = new ServiceCall("ACM02T", "RT_DSR_Coolant_Valve_Open_Stop");
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)(object)digitalReadoutInstrumentCoolantValve, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCoolantValve, "digitalReadoutInstrumentCoolantValve");
		digitalReadoutInstrumentCoolantValve.FontGroup = "mainInstruments";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantValve).FreezeValue = false;
		digitalReadoutInstrumentCoolantValve.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentCoolantValve.Gradient.Modify(1, 0.0, (ValueState)2);
		digitalReadoutInstrumentCoolantValve.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentCoolantValve.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentCoolantValve.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantValve).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS005_Coolant_Valve");
		((Control)(object)digitalReadoutInstrumentCoolantValve).Name = "digitalReadoutInstrumentCoolantValve";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantValve).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)textBoxOutput, 2);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DEFCoolantValveControl");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelBase);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanelBase).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelBase).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
