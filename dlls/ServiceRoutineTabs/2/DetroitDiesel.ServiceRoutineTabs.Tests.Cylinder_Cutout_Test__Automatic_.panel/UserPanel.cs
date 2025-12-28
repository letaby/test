using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		Start,
		ChangeIdleSpeed,
		WaitForIdleSpeed,
		Init,
		WaitToGetBaseline,
		CylinderOff,
		WaitWhileCylinderOff,
		CylinderOn,
		End
	}

	private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private const string ActualTorqueQualifier = "DT_AS003_Actual_Torque";

	private const double RequiredEngineSpeed = 1000.0;

	private static string instructionText = Resources.Message_ToProceed;

	private static string DpfZoneTestConditionNotMet = Resources.Message_TheVehicleNeedsToBeInZone0PerformAParkedRegenerationAndTryAgain;

	private static string DpfZoneTestConditionMet = Resources.Message_TheDPFZoneIsZero;

	private readonly ReadOnlyCollection<CylinderGroup> cylinderGroups;

	private Channel channel;

	private Instrument dPfZoneInstrument;

	private Dictionary<string, CacheInfo> snapshot;

	private DateTime initialTimeBeforeCylindercut;

	private State state = State.Start;

	private Timer timer = new Timer();

	private bool adrReturnValue = true;

	private int currentGroupIndex = -1;

	private static readonly TimeSpan AverageSampleSpan = new TimeSpan(0, 0, 3);

	private ScalingLabel ScalingLabel5;

	private ScalingLabel ScalingLabel4;

	private ScalingLabel ScalingLabel3;

	private ScalingLabel ScalingLabel2;

	private ScalingLabel ScalingLabel1;

	private ChartInstrument ChartInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument3;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument4;

	private TableLayoutPanel tableLayoutPanelBase;

	private TabControl tabControl2;

	private TabPage tabPage3;

	private TableLayoutPanel tableLayoutPanel2;

	private TabPage tabPage4;

	private Button buttonCancel;

	private Button buttonExecute;

	private Label label1;

	private Label label8;

	private Label label9;

	private Label label10;

	private Label label11;

	private Label label12;

	private Label labelCheckmarkDpfZoneZero;

	private Checkmark checkmarkDpfZone;

	private Label unitsLabel1and6;

	private Label unitsLabel12and3;

	private Label unitsLabel45and6;

	private Label unitsLabel2and5;

	private Label unitsLabel3and4;

	private TextBox textboxResults;

	private DateTime EndOfInitialIdleTimeBeforeCylinderCut => initialTimeBeforeCylindercut + new TimeSpan(0, 0, 5);

	public override bool CanProvideHtml => state == State.Start && textboxResults.TextLength > 0;

	private bool InDpfZoneZero
	{
		get
		{
			bool result = false;
			if (dPfZoneInstrument != null && dPfZoneInstrument.InstrumentValues != null && dPfZoneInstrument.InstrumentValues.Current != null && dPfZoneInstrument.InstrumentValues.Current.Value == dPfZoneInstrument.Choices.GetItemFromRawValue(0))
			{
				result = true;
			}
			return result;
		}
	}

	public UserPanel()
	{
		WarningManager.SetJobName(Resources.Message_CylinderCutoutTest);
		InitializeComponent();
		timer.Interval = 1000;
		timer.Tick += Advance;
		buttonExecute.Click += OnExecuteButtonClick;
		buttonCancel.Click += OnCancelButtonClick;
		cylinderGroups = new List<CylinderGroup>(new CylinderGroup[5]
		{
			new CylinderGroup(new int[2] { 1, 6 }, ScalingLabel1, unitsLabel1and6),
			new CylinderGroup(new int[2] { 2, 5 }, ScalingLabel2, unitsLabel2and5),
			new CylinderGroup(new int[2] { 3, 4 }, ScalingLabel3, unitsLabel3and4),
			new CylinderGroup(new int[3] { 1, 2, 3 }, ScalingLabel4, unitsLabel12and3),
			new CylinderGroup(new int[3] { 4, 5, 6 }, ScalingLabel5, unitsLabel45and6)
		}).AsReadOnly();
	}

	public override string ToHtml()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<table>");
		stringBuilder.Append("<tr><td class=\"group\">" + Resources.Message_TorqueLostWhileCuttingCylinders + "</td><td class=\"standard\"></td></tr>");
		foreach (CylinderGroup cylinderGroup in cylinderGroups)
		{
			stringBuilder.AppendFormat(arg1: cylinderGroup.Result.HasValue ? cylinderGroup.ResultAsString : "&lt;No Value&gt;", format: "<tr><td class=\"standard\">" + Resources.MessageFormat_Cylinder0 + "</td><td class=\"standard\">{1}</td></tr>", arg0: cylinderGroup.GetCuttingStatusText());
		}
		stringBuilder.Append("<tr><td class=\"group\">" + Resources.Message_History + "</td><td class=\"standard\"></td></tr>");
		string[] lines = textboxResults.Lines;
		foreach (string arg in lines)
		{
			stringBuilder.AppendFormat("<tr><td class=\"standard\">{0}</td><td class=\"standard\"></td></tr>", arg);
		}
		stringBuilder.Append("</table>");
		return stringBuilder.ToString();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Tag = new object[2] { adrReturnValue, textboxResults.Text };
	}

	private void OnExecuteButtonClick(object sender, EventArgs e)
	{
		if (WarningManager.RequestContinue() && DialogResult.OK == MessageBox.Show(instructionText, ApplicationInformation.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2))
		{
			timer.Start();
			Advance();
		}
	}

	private void OnCancelButtonClick(object sender, EventArgs e)
	{
		state = State.End;
		WriteLine(Resources.Message_CancelingTest);
		Advance();
		WriteLine(Resources.Message_TheTestWasCancelledByTheUser);
	}

	private void Advance(object sender, EventArgs e)
	{
		Advance();
	}

	private void Advance()
	{
		bool flag = false;
		switch (state)
		{
		case State.Start:
			currentGroupIndex = -1;
			flag = true;
			ClearResults();
			SetupInstruments();
			state = State.ChangeIdleSpeed;
			break;
		case State.ChangeIdleSpeed:
			flag = SetIdleStart(1000.0);
			if (flag)
			{
				state = State.WaitForIdleSpeed;
			}
			break;
		case State.WaitForIdleSpeed:
		{
			double num = 0.0;
			Instrument instrument = null;
			if (channel == null)
			{
				break;
			}
			instrument = channel.Instruments["DT_AS010_Engine_Speed"];
			if (!(instrument != null) || instrument.InstrumentValues.Current == null || instrument.InstrumentValues.Current.Value == null)
			{
				break;
			}
			try
			{
				num = Convert.ToDouble(instrument.InstrumentValues.Current.Value);
				flag = true;
				if (num >= 1000.0)
				{
					state = State.Init;
				}
			}
			catch (InvalidCastException)
			{
			}
			break;
		}
		case State.Init:
			initialTimeBeforeCylindercut = DateTime.Now;
			WriteLine(Resources.Message_Idle);
			state = State.WaitToGetBaseline;
			flag = true;
			break;
		case State.WaitToGetBaseline:
		{
			flag = true;
			DateTime now = DateTime.Now;
			if (currentGroupIndex >= 0)
			{
				if (now >= cylinderGroups[currentGroupIndex].EndOfWaitTimeAfterGroupWasTurnedOn)
				{
					state = State.CylinderOff;
				}
			}
			else if (now >= EndOfInitialIdleTimeBeforeCylinderCut)
			{
				state = State.CylinderOff;
			}
			break;
		}
		case State.CylinderOff:
			currentGroupIndex++;
			if (currentGroupIndex < cylinderGroups.Count)
			{
				WriteLine(string.Format(Resources.MessageFormat_CuttingCylinders0, cylinderGroups[currentGroupIndex].GetCuttingStatusText()));
				flag = SwitchCylindersOff(cylinderGroups[currentGroupIndex]);
				cylinderGroups[currentGroupIndex].TimeGroupWasTurnedOff = DateTime.Now;
				state = State.WaitWhileCylinderOff;
			}
			else
			{
				flag = false;
			}
			break;
		case State.WaitWhileCylinderOff:
			flag = true;
			if (DateTime.Now >= cylinderGroups[currentGroupIndex].EndOfWaitTimeAfterGroupWasTurnedOff)
			{
				state = State.CylinderOn;
			}
			break;
		case State.CylinderOn:
			WriteLine(Resources.Message_Idle);
			flag = SwitchAllCylindersOn();
			cylinderGroups[currentGroupIndex].TimeGroupWasTurnedOn = DateTime.Now;
			if (currentGroupIndex == cylinderGroups.Count - 1)
			{
				state = State.End;
			}
			else
			{
				state = State.WaitToGetBaseline;
			}
			break;
		case State.End:
			flag = SwitchAllCylindersOn();
			state = State.Start;
			currentGroupIndex = -1;
			timer.Stop();
			SetIdleStop();
			DisplayResults();
			RestoreInstruments();
			WriteLine(Resources.Message_CylinderCutoutTestCompleted);
			break;
		}
		if (!flag)
		{
			adrReturnValue = false;
			if (state != State.Start)
			{
				WriteLine(Resources.Message_AnErrorOccurredTheTestWillTerminateEarly);
				if (state != State.ChangeIdleSpeed || state != State.WaitForIdleSpeed)
				{
					flag = SwitchAllCylindersOn();
				}
				state = State.End;
			}
		}
		UpdateButtonState();
	}

	private bool SwitchCylindersOff(CylinderGroup group)
	{
		bool result = false;
		if (channel != null)
		{
			try
			{
				int num = channel.Services.Execute(group.GetServiceExecuteList(), synchronous: true);
				result = num == group.Cylinders.Count;
			}
			catch (CaesarException ex)
			{
				WriteLine(ex.Message);
			}
		}
		return result;
	}

	private bool SwitchAllCylindersOn()
	{
		bool result = false;
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR004_Engine_Cylinder_Cut_Off_Stop");
		if (service != null)
		{
			result = CustomPanel.ExecuteService(service);
		}
		return result;
	}

	public override void OnChannelsChanged()
	{
		WarningManager.Reset();
		SetChannel(((CustomPanel)this).GetChannel("MCM"));
		UpdateButtonState();
	}

	private void OnDpfZoneInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateButtonState();
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel == channel)
		{
			return;
		}
		if (this.channel != null)
		{
			this.channel.CommunicationsStateUpdateEvent -= OnChannelCommunicationsStateUpdateEvent;
			if (dPfZoneInstrument != null)
			{
				dPfZoneInstrument.InstrumentUpdateEvent -= OnDpfZoneInstrumentUpdateEvent;
			}
		}
		this.channel = channel;
		if (this.channel != null)
		{
			this.channel.CommunicationsStateUpdateEvent += OnChannelCommunicationsStateUpdateEvent;
			dPfZoneInstrument = this.channel.Instruments["DT_AS072_DPF_Zone"];
			if (dPfZoneInstrument != null)
			{
				dPfZoneInstrument.InstrumentUpdateEvent += OnDpfZoneInstrumentUpdateEvent;
			}
		}
	}

	private void OnChannelCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		bool enabled = false;
		bool enabled2 = false;
		checkmarkDpfZone.Checked = InDpfZoneZero;
		((Control)(object)labelCheckmarkDpfZoneZero).Text = (checkmarkDpfZone.Checked ? DpfZoneTestConditionMet : DpfZoneTestConditionNotMet);
		if (channel != null && channel.Instruments["DT_AS010_Engine_Speed"] != null)
		{
			if (state == State.Start)
			{
				if (checkmarkDpfZone.Checked)
				{
					enabled = true;
				}
			}
			else
			{
				enabled2 = true;
			}
		}
		else
		{
			state = State.Start;
			timer.Stop();
		}
		buttonExecute.Enabled = enabled;
		buttonCancel.Enabled = enabled2;
	}

	private void ClearResults()
	{
		foreach (CylinderGroup cylinderGroup in cylinderGroups)
		{
			cylinderGroup.SetResult(null, string.Empty);
		}
		textboxResults.Text = string.Empty;
	}

	private void DisplayResults()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		Instrument instrument = ((CustomPanel)this).GetInstrument("MCM", "DT_AS003_Actual_Torque");
		if (!(instrument != null))
		{
			return;
		}
		List<double?> list = new List<double?>();
		SpatialAverageCalculator val = new SpatialAverageCalculator(instrument);
		DateTime timeGroupWasTurnedOn = initialTimeBeforeCylindercut;
		foreach (CylinderGroup cylinderGroup in cylinderGroups)
		{
			double average = val.GetAverage(timeGroupWasTurnedOn + AverageSampleSpan, cylinderGroup.TimeGroupWasTurnedOff);
			double average2 = val.GetAverage(cylinderGroup.TimeGroupWasTurnedOff + AverageSampleSpan, cylinderGroup.TimeGroupWasTurnedOn);
			list.Add(average2 - average);
			timeGroupWasTurnedOn = cylinderGroup.TimeGroupWasTurnedOn;
		}
		for (int i = 0; i < cylinderGroups.Count; i++)
		{
			cylinderGroups[i].SetResult(list[i], instrument.Units);
		}
	}

	private bool SetIdleStart(double idle)
	{
		bool result = false;
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR015_Idle_Speed_Modification_Start");
		if (service != null)
		{
			WriteLine(Resources.Message_SettingRPM);
			service.InputValues[0].Value = idle;
			result = CustomPanel.ExecuteService(service);
		}
		return result;
	}

	private bool SetIdleStop()
	{
		bool result = false;
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR015_Idle_Speed_Modification_Stop");
		if (service != null)
		{
			result = CustomPanel.ExecuteService(service);
		}
		return result;
	}

	private void SetupInstruments()
	{
		if (channel != null)
		{
			snapshot = InstrumentCacheManager.GenerateSnapshot(channel);
			InstrumentCacheManager.UnmarkAllInstruments(channel);
			InstrumentCacheManager.MarkInstrument(channel, "DT_AS003_Actual_Torque", (ushort)10);
			InstrumentCacheManager.MarkInstrument(channel, "DT_AS010_Engine_Speed", (ushort)10);
			InstrumentCacheManager.MarkInstrument(channel, "DT_AS013_Coolant_Temperature", (ushort)10);
			InstrumentCacheManager.MarkInstrument(channel, "DT_AS032_EGR_Actual_Valve_Position", (ushort)10);
			InstrumentCacheManager.MarkInstrument(channel, "DT_AS034_Throttle_Valve_Actual_Position", (ushort)10);
			InstrumentCacheManager.MarkInstrument(channel, "DT_AS117_Percentage_of_current_VGT_position", (ushort)10);
		}
	}

	private void RestoreInstruments()
	{
		if (channel != null)
		{
			InstrumentCacheManager.ApplySnapshot(channel, snapshot);
		}
	}

	private void WriteLine(string text)
	{
		if (textboxResults != null)
		{
			StringBuilder stringBuilder = new StringBuilder(textboxResults.Text);
			stringBuilder.Append(text);
			stringBuilder.Append("\r\n");
			textboxResults.Text = stringBuilder.ToString();
			textboxResults.SelectionStart = textboxResults.TextLength;
			textboxResults.SelectionLength = 0;
			textboxResults.ScrollToCaret();
		}
		((CustomPanel)this).AddStatusMessage(text);
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
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
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelBase = new TableLayoutPanel();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
		tabControl2 = new TabControl();
		tabPage4 = new TabPage();
		ChartInstrument1 = new ChartInstrument();
		tabPage3 = new TabPage();
		tableLayoutPanel2 = new TableLayoutPanel();
		ScalingLabel3 = new ScalingLabel();
		ScalingLabel2 = new ScalingLabel();
		ScalingLabel1 = new ScalingLabel();
		ScalingLabel4 = new ScalingLabel();
		ScalingLabel5 = new ScalingLabel();
		label1 = new Label();
		label8 = new Label();
		label9 = new Label();
		label10 = new Label();
		label11 = new Label();
		label12 = new Label();
		unitsLabel1and6 = new Label();
		unitsLabel12and3 = new Label();
		unitsLabel45and6 = new Label();
		unitsLabel2and5 = new Label();
		unitsLabel3and4 = new Label();
		textboxResults = new TextBox();
		buttonCancel = new Button();
		buttonExecute = new Button();
		labelCheckmarkDpfZoneZero = new Label();
		checkmarkDpfZone = new Checkmark();
		((Control)(object)tableLayoutPanelBase).SuspendLayout();
		tabControl2.SuspendLayout();
		tabPage4.SuspendLayout();
		tabPage3.SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelBase, "tableLayoutPanelBase");
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)DigitalReadoutInstrument4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)DigitalReadoutInstrument1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)DigitalReadoutInstrument2, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)DigitalReadoutInstrument3, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add(tabControl2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add(textboxResults, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add(buttonCancel, 4, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add(buttonExecute, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)labelCheckmarkDpfZoneZero, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBase).Controls.Add((Control)(object)checkmarkDpfZone, 0, 3);
		((Control)(object)tableLayoutPanelBase).Name = "tableLayoutPanelBase";
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)(object)DigitalReadoutInstrument4, 2);
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
		DigitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS003_Actual_Torque");
		((Control)(object)DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
		((SingleInstrumentBase)DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tabControl2, "tabControl2");
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)tabControl2, 5);
		tabControl2.Controls.Add(tabPage4);
		tabControl2.Controls.Add(tabPage3);
		tabControl2.Name = "tabControl2";
		tabControl2.SelectedIndex = 0;
		tabPage4.Controls.Add((Control)(object)ChartInstrument1);
		componentResourceManager.ApplyResources(tabPage4, "tabPage4");
		tabPage4.Name = "tabPage4";
		tabPage4.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(ChartInstrument1, "ChartInstrument1");
		((Collection<Qualifier>)(object)ChartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM", "DT_AS003_Actual_Torque"));
		((Control)(object)ChartInstrument1).Name = "ChartInstrument1";
		ChartInstrument1.SelectedTime = null;
		ChartInstrument1.ShowEvents = false;
		ChartInstrument1.ShowLegend = false;
		tabPage3.Controls.Add((Control)(object)tableLayoutPanel2);
		componentResourceManager.ApplyResources(tabPage3, "tabPage3");
		tabPage3.Name = "tabPage3";
		tabPage3.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)ScalingLabel3, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)ScalingLabel2, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)ScalingLabel1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)ScalingLabel4, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)ScalingLabel5, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label8, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label9, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label10, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label11, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)label12, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)unitsLabel1and6, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)unitsLabel12and3, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)unitsLabel45and6, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)unitsLabel2and5, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)unitsLabel3and4, 5, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		ScalingLabel3.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)ScalingLabel3, 2);
		componentResourceManager.ApplyResources(ScalingLabel3, "ScalingLabel3");
		ScalingLabel3.FontGroup = "AutomaticCOResults";
		ScalingLabel3.LineAlignment = StringAlignment.Center;
		((Control)(object)ScalingLabel3).Name = "ScalingLabel3";
		ScalingLabel2.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)ScalingLabel2, 2);
		componentResourceManager.ApplyResources(ScalingLabel2, "ScalingLabel2");
		ScalingLabel2.FontGroup = "AutomaticCOResults";
		ScalingLabel2.LineAlignment = StringAlignment.Center;
		((Control)(object)ScalingLabel2).Name = "ScalingLabel2";
		ScalingLabel1.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)ScalingLabel1, 2);
		componentResourceManager.ApplyResources(ScalingLabel1, "ScalingLabel1");
		ScalingLabel1.FontGroup = "AutomaticCOResults";
		ScalingLabel1.LineAlignment = StringAlignment.Center;
		((Control)(object)ScalingLabel1).Name = "ScalingLabel1";
		ScalingLabel4.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)ScalingLabel4, 2);
		componentResourceManager.ApplyResources(ScalingLabel4, "ScalingLabel4");
		ScalingLabel4.FontGroup = "AutomaticCOResults";
		ScalingLabel4.LineAlignment = StringAlignment.Center;
		((Control)(object)ScalingLabel4).Name = "ScalingLabel4";
		ScalingLabel5.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)ScalingLabel5, 2);
		componentResourceManager.ApplyResources(ScalingLabel5, "ScalingLabel5");
		ScalingLabel5.FontGroup = "AutomaticCOResults";
		ScalingLabel5.LineAlignment = StringAlignment.Center;
		((Control)(object)ScalingLabel5).Name = "ScalingLabel5";
		label1.Alignment = StringAlignment.Center;
		componentResourceManager.ApplyResources(label1, "label1");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)label1, 6);
		((Control)(object)label1).Name = "label1";
		label1.Orientation = (TextOrientation)1;
		label8.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)(object)label8).Name = "label8";
		label8.Orientation = (TextOrientation)1;
		label9.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)(object)label9).Name = "label9";
		label9.Orientation = (TextOrientation)1;
		label10.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)(object)label10).Name = "label10";
		label10.Orientation = (TextOrientation)1;
		label11.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)(object)label11).Name = "label11";
		label11.Orientation = (TextOrientation)1;
		label12.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label12, "label12");
		((Control)(object)label12).Name = "label12";
		label12.Orientation = (TextOrientation)1;
		unitsLabel1and6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(unitsLabel1and6, "unitsLabel1and6");
		((Control)(object)unitsLabel1and6).Name = "unitsLabel1and6";
		unitsLabel1and6.Orientation = (TextOrientation)1;
		unitsLabel12and3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(unitsLabel12and3, "unitsLabel12and3");
		((Control)(object)unitsLabel12and3).Name = "unitsLabel12and3";
		unitsLabel12and3.Orientation = (TextOrientation)1;
		unitsLabel45and6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(unitsLabel45and6, "unitsLabel45and6");
		((Control)(object)unitsLabel45and6).Name = "unitsLabel45and6";
		unitsLabel45and6.Orientation = (TextOrientation)1;
		unitsLabel2and5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(unitsLabel2and5, "unitsLabel2and5");
		((Control)(object)unitsLabel2and5).Name = "unitsLabel2and5";
		unitsLabel2and5.Orientation = (TextOrientation)1;
		unitsLabel3and4.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(unitsLabel3and4, "unitsLabel3and4");
		((Control)(object)unitsLabel3and4).Name = "unitsLabel3and4";
		unitsLabel3and4.Orientation = (TextOrientation)1;
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)textboxResults, 5);
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		componentResourceManager.ApplyResources(buttonCancel, "buttonCancel");
		buttonCancel.Name = "buttonCancel";
		buttonCancel.UseCompatibleTextRendering = true;
		buttonCancel.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonExecute, "buttonExecute");
		buttonExecute.Name = "buttonExecute";
		buttonExecute.UseCompatibleTextRendering = true;
		buttonExecute.UseVisualStyleBackColor = true;
		labelCheckmarkDpfZoneZero.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelBase).SetColumnSpan((Control)(object)labelCheckmarkDpfZoneZero, 2);
		componentResourceManager.ApplyResources(labelCheckmarkDpfZoneZero, "labelCheckmarkDpfZoneZero");
		((Control)(object)labelCheckmarkDpfZoneZero).Name = "labelCheckmarkDpfZoneZero";
		labelCheckmarkDpfZoneZero.Orientation = (TextOrientation)1;
		labelCheckmarkDpfZoneZero.UseSystemColors = true;
		componentResourceManager.ApplyResources(checkmarkDpfZone, "checkmarkDpfZone");
		((Control)(object)checkmarkDpfZone).Name = "checkmarkDpfZone";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CylinderCutoutTestAuto");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelBase);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelBase).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelBase).PerformLayout();
		tabControl2.ResumeLayout(performLayout: false);
		tabPage4.ResumeLayout(performLayout: false);
		tabPage3.ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
