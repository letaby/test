using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private const string EngineSpeedInstrumentQualifier = "DT_AS001_Engine_Speed";

	private Channel acm = null;

	private Instrument engineSpeed = null;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private BarInstrument barInstrument1;

	private TableLayoutPanel tableLayoutPanel2;

	private System.Windows.Forms.Label labelDuration;

	private System.Windows.Forms.Label labelDurationUnits;

	private DecimalTextBox durationTextBox;

	private Button buttonClose;

	private TableLayoutPanel tableLayoutPanel3;

	private RunServiceButton runServiceButtonStart;

	private RunServiceButton runServiceButtonStop;

	private TableLayoutPanel tableLayoutPanel4;

	private Label labelEngineStatus;

	private Checkmark engineSpeedCheck;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private System.Windows.Forms.Label label1;

	private BarInstrument barInstrument2;

	private bool CanEditDuration
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			int result;
			if (!((RunSharedProcedureButtonBase)runServiceButtonStart).IsBusy)
			{
				ServiceCall serviceCall = runServiceButtonStart.ServiceCall;
				if (((ServiceCall)(ref serviceCall)).IsServiceAvailable)
				{
					result = (engineSpeedCheck.Checked ? 1 : 0);
					goto IL_0031;
				}
			}
			result = 0;
			goto IL_0031;
			IL_0031:
			return (byte)result != 0;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		durationTextBox.ValueChanged += OnDurationChanged;
		((RunSharedProcedureButtonBase)runServiceButtonStart).ButtonEnabledChanged += OnStartButtonEnabledChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		UpdateUserInterface();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			SetACM(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetACM(((CustomPanel)this).GetChannel("ACM02T"));
		UpdateUserInterface();
	}

	private void SetACM(Channel acm)
	{
		if (this.acm != acm && this.acm != null)
		{
			this.acm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
		}
		if (engineSpeed != null)
		{
			engineSpeed.InstrumentUpdateEvent -= OnEngineSpeedUpdate;
			engineSpeed = null;
		}
		this.acm = acm;
		if (this.acm != null)
		{
			this.acm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			engineSpeed = this.acm.Instruments["DT_AS001_Engine_Speed"];
			if (engineSpeed != null)
			{
				engineSpeed.InstrumentUpdateEvent += OnEngineSpeedUpdate;
			}
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnStartButtonEnabledChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		UpdateEngineStatus();
		if (!engineSpeedCheck.Checked)
		{
			((Control)(object)runServiceButtonStop).Enabled = false;
		}
		((Control)(object)runServiceButtonStart).Enabled = ValidateAndSetDuration() && engineSpeedCheck.Checked;
		((Control)(object)durationTextBox).Enabled = CanEditDuration;
	}

	private void UpdateEngineStatus()
	{
		bool flag = false;
		string text = Resources.Message_EngineSpeedCannotBeDetectedCheckCannotStart;
		if (engineSpeed != null && engineSpeed.InstrumentValues.Current != null)
		{
			double num = Convert.ToDouble(engineSpeed.InstrumentValues.Current.Value);
			if (!double.IsNaN(num))
			{
				if (num == 0.0)
				{
					text = Resources.Message_EngineIsNotRunningCheckCanStart;
					flag = true;
				}
				else
				{
					text = Resources.Message_EngineIsRunningCheckCannotStart0;
				}
			}
		}
		((Control)(object)labelEngineStatus).Text = text;
		engineSpeedCheck.Checked = flag;
	}

	private bool ValidateAndSetDuration()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!((RunSharedProcedureButtonBase)runServiceButtonStart).IsBusy)
		{
			double value = durationTextBox.Value;
			if (!double.IsInfinity(value) && !double.IsNaN(value))
			{
				ServiceCall serviceCall = runServiceButtonStart.ServiceCall;
				if (!((ServiceCall)(ref serviceCall)).IsEmpty)
				{
					RunServiceButton obj = runServiceButtonStart;
					serviceCall = runServiceButtonStart.ServiceCall;
					string ecu = ((ServiceCall)(ref serviceCall)).Ecu;
					serviceCall = runServiceButtonStart.ServiceCall;
					obj.ServiceCall = new ServiceCall(ecu, ((ServiceCall)(ref serviceCall)).Qualifier, (IEnumerable<string>)new string[1] { value.ToString() });
					return true;
				}
			}
		}
		return false;
	}

	private void OnDurationChanged(object sender, EventArgs e)
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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		barInstrument1 = new BarInstrument();
		barInstrument2 = new BarInstrument();
		buttonClose = new Button();
		labelDuration = new System.Windows.Forms.Label();
		labelDurationUnits = new System.Windows.Forms.Label();
		durationTextBox = new DecimalTextBox();
		runServiceButtonStart = new RunServiceButton();
		runServiceButtonStop = new RunServiceButton();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		labelEngineStatus = new Label();
		engineSpeedCheck = new Checkmark();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)tableLayoutPanel1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)labelEngineStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)engineSpeedCheck, 0, 0);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel4).SetColumnSpan((Control)(object)tableLayoutPanel1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS003_Enable_DEF_pump");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument1, 3);
		componentResourceManager.ApplyResources(barInstrument1, "barInstrument1");
		barInstrument1.FontGroup = null;
		((SingleInstrumentBase)barInstrument1).FreezeValue = false;
		((SingleInstrumentBase)barInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
		((Control)(object)barInstrument1).Name = "barInstrument1";
		((SingleInstrumentBase)barInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument2, 3);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = null;
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel3, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)runServiceButtonStart, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)runServiceButtonStop, 2, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(labelDuration, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(labelDurationUnits, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)durationTextBox, 1, 1);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(labelDuration, "labelDuration");
		labelDuration.Name = "labelDuration";
		labelDuration.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelDurationUnits, "labelDurationUnits");
		labelDurationUnits.Name = "labelDurationUnits";
		labelDurationUnits.UseCompatibleTextRendering = true;
		((Control)(object)durationTextBox).Cursor = Cursors.IBeam;
		componentResourceManager.ApplyResources(durationTextBox, "durationTextBox");
		durationTextBox.MaximumValue = 60.0;
		durationTextBox.MinimumValue = 0.0;
		((Control)(object)durationTextBox).Name = "durationTextBox";
		durationTextBox.Precision = 0;
		durationTextBox.Value = 60.0;
		componentResourceManager.ApplyResources(runServiceButtonStart, "runServiceButtonStart");
		((Control)(object)runServiceButtonStart).Name = "runServiceButtonStart";
		runServiceButtonStart.ServiceCall = new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Start", (IEnumerable<string>)new string[1] { "10" });
		componentResourceManager.ApplyResources(runServiceButtonStop, "runServiceButtonStop");
		((Control)(object)runServiceButtonStop).Name = "runServiceButtonStop";
		runServiceButtonStop.ServiceCall = new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Stop");
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label1, 3);
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		labelEngineStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelEngineStatus, "labelEngineStatus");
		((Control)(object)labelEngineStatus).Name = "labelEngineStatus";
		labelEngineStatus.Orientation = (TextOrientation)1;
		labelEngineStatus.ShowBorder = false;
		labelEngineStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(engineSpeedCheck, "engineSpeedCheck");
		((Control)(object)engineSpeedCheck).Name = "engineSpeedCheck";
		engineSpeedCheck.SizeMode = PictureBoxSizeMode.AutoSize;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRAirPressureSystemCheck");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel4);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
