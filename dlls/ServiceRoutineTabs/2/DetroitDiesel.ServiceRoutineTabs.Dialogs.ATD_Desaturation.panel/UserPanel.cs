using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Net;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel;

public class UserPanel : CustomPanel
{
	private const string Cpc2PlusName = "CPC02T";

	private const string Cpc4Name = "CPC04T";

	private const string Cpc3Name = "CPC302T";

	private const string Cpc5Name = "CPC501T";

	private const string Cpc502Name = "CPC502T";

	private const string Acm3Name = "ACM301T";

	private const int ColDocInlet = 0;

	private const int ColDocOutlet = 1;

	private const int ColDpfOutlet = 2;

	private const int ColScrInlet = 3;

	private const int ColScrOutlet = 4;

	private const float WidthHidden = 0f;

	private const float Width4Cols = 25f;

	private const float Width5Cols = 20f;

	private const int ReAlertTimer = 5000;

	private static SubjectCollection NGCProcedure = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DisableHcDoserParkedRegen_NGC" });

	private static SubjectCollection GHG14Procedure = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DisableHcDoserParkedRegen_MY13" });

	private static SubjectCollection CPC5Procedure = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DisableHcDoserParkedRegen_CPC5" });

	private static SubjectCollection X45Procedure = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DisableHcDoserParkedRegen_45X" });

	private static SubjectCollection EPA10Procedure = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DisableHcDoserParkedRegen_EPA10" });

	private Timer warningTimer = new Timer();

	private bool warningShowing = false;

	private bool instructionsShown = false;

	private Dictionary<string, double> maxTemperatures = new Dictionary<string, double>();

	private SharedProcedureBase selectedProcedure;

	private TableLayoutPanel tableLayoutPanel1;

	private DialInstrument dialInstrument1;

	private SeekTimeListView seekTimeListView1;

	private Button buttonStart;

	private BarInstrument barInstrumentDOCIntletTemperature;

	private BarInstrument barInstrumentDOCOutletTemperature;

	private BarInstrument barInstrumentDPFOutletTemperature;

	private BarInstrument barInstrumentSCRInletTemperature;

	private BarInstrument barInstrumentSCROutletTemperature;

	private TableLayoutPanel tableLayoutPanel2;

	private System.Windows.Forms.Label labelStatus;

	private Checkmark checkmark;

	private SharedProcedureSelection sharedProcedureSelection;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private bool InFaultState => (int)barInstrumentDOCIntletTemperature.RepresentedState == 3 || (int)barInstrumentDOCOutletTemperature.RepresentedState == 3 || (int)barInstrumentDPFOutletTemperature.RepresentedState == 3 || (int)barInstrumentSCRInletTemperature.RepresentedState == 3 || (int)barInstrumentSCROutletTemperature.RepresentedState == 3;

	public UserPanel()
	{
		InitializeComponent();
		warningTimer.Enabled = false;
		warningTimer.Interval = 5000;
		warningTimer.Tick += warningTimer_Tick;
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_ProcedureStillRunningCannotCloseDialog);
			e.Cancel = true;
		}
		else if (InFaultState)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_TemperaturesAreTooHighCannotCloseDialog);
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			warningTimer.Enabled = false;
			warningTimer.Dispose();
			if (selectedProcedure != null)
			{
				selectedProcedure.StartComplete -= selectedProcedure_StartComplete;
				selectedProcedure.StopComplete -= selectedProcedure_StopComplete;
				selectedProcedure = null;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateProcedure();
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		Channel channel = ((CustomPanel)this).GetChannel("ACM301T", (ChannelLookupOptions)7);
		if (channel != null)
		{
			bool flag = channel.Ecu.Name.Equals("ACM301T", StringComparison.InvariantCulture);
			float width = (flag ? 25f : 20f);
			float width2 = (flag ? 0f : 20f);
			((Control)(object)barInstrumentSCRInletTemperature).Visible = !flag;
			((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[0].Width = width;
			((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[1].Width = width;
			((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[2].Width = width;
			((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[3].Width = width2;
			((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[4].Width = width;
		}
	}

	private void UpdateProcedure()
	{
		Channel channel = ((CustomPanel)this).GetChannel("CPC302T", (ChannelLookupOptions)7);
		if (selectedProcedure != null)
		{
			selectedProcedure.StartComplete -= selectedProcedure_StartComplete;
			selectedProcedure.StopComplete -= selectedProcedure_StopComplete;
			selectedProcedure = null;
		}
		if (channel != null)
		{
			switch (channel.Ecu.Name)
			{
			case "CPC02T":
				sharedProcedureSelection.SharedProcedureQualifiers = EPA10Procedure;
				break;
			case "CPC04T":
				sharedProcedureSelection.SharedProcedureQualifiers = GHG14Procedure;
				break;
			case "CPC302T":
				sharedProcedureSelection.SharedProcedureQualifiers = NGCProcedure;
				break;
			case "CPC501T":
				sharedProcedureSelection.SharedProcedureQualifiers = CPC5Procedure;
				break;
			case "CPC502T":
				sharedProcedureSelection.SharedProcedureQualifiers = X45Procedure;
				break;
			default:
				sharedProcedureSelection.SharedProcedureQualifiers = NGCProcedure;
				break;
			}
			selectedProcedure = sharedProcedureSelection.SelectedProcedure;
			selectedProcedure.StartComplete += selectedProcedure_StartComplete;
			selectedProcedure.StopComplete += selectedProcedure_StopComplete;
		}
	}

	private void ShowWarning()
	{
		if (!warningShowing && !warningTimer.Enabled)
		{
			warningTimer.Enabled = false;
			warningShowing = true;
			MessageBox.Show(string.Format(Resources.Message_WarningText, Environment.NewLine), Resources.Message_WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_HighTemperatureWarningDialogClosed);
			warningShowing = false;
			if (InFaultState)
			{
				warningTimer.Enabled = true;
			}
			else
			{
				warningTimer.Enabled = false;
			}
		}
	}

	private void AddEventEntry()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string key in maxTemperatures.Keys)
		{
			dictionary[key] = maxTemperatures[key].ToString("0.##");
		}
		Channel channel = ((CustomPanel)this).GetChannel("ACM301T", (ChannelLookupOptions)7);
		ServerDataManager.UpdateEventsFile(channel, (IDictionary<string, string>)dictionary, "ATDDesaturation", string.Empty, SapiManager.GetVehicleIdentificationNumber(channel), "OK", string.Empty, string.Empty, false);
	}

	private void barInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (InFaultState)
		{
			ShowWarning();
		}
	}

	private void warningTimer_Tick(object sender, EventArgs e)
	{
		warningTimer.Enabled = false;
		if (InFaultState)
		{
			ShowWarning();
		}
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		if (!instructionsShown)
		{
			instructionsShown = true;
			MessageBox.Show(Resources.Message_InstructionText, Resources.Message_InstructionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_InstructionsAcknowledged);
		}
	}

	private void selectedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		maxTemperatures.Clear();
	}

	private void selectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		AddEventEntry();
	}

	private void barInstrument_DataChanged(object sender, EventArgs e)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		SingleInstrumentBase val = (SingleInstrumentBase)((sender is SingleInstrumentBase) ? sender : null);
		if (val == null || val.DataItem == null || val.DataItem.Value == null)
		{
			return;
		}
		double num = val.DataItem.ValueAsDouble(val.DataItem.Value);
		Dictionary<string, double> dictionary = maxTemperatures;
		Qualifier qualifier = val.DataItem.Qualifier;
		if (dictionary.ContainsKey(((Qualifier)(ref qualifier)).Name))
		{
			Dictionary<string, double> dictionary2 = maxTemperatures;
			qualifier = val.DataItem.Qualifier;
			if (dictionary2[((Qualifier)(ref qualifier)).Name] < num)
			{
				Dictionary<string, double> dictionary3 = maxTemperatures;
				qualifier = val.DataItem.Qualifier;
				dictionary3[((Qualifier)(ref qualifier)).Name] = num;
			}
		}
		else
		{
			Dictionary<string, double> dictionary4 = maxTemperatures;
			qualifier = val.DataItem.Qualifier;
			dictionary4.Add(((Qualifier)(ref qualifier)).Name, num);
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0970: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Expected O, but got Unknown
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		dialInstrument1 = new DialInstrument();
		seekTimeListView1 = new SeekTimeListView();
		buttonStart = new Button();
		barInstrumentDOCIntletTemperature = new BarInstrument();
		barInstrumentDOCOutletTemperature = new BarInstrument();
		barInstrumentDPFOutletTemperature = new BarInstrument();
		barInstrumentSCRInletTemperature = new BarInstrument();
		barInstrumentSCROutletTemperature = new BarInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		labelStatus = new System.Windows.Forms.Label();
		checkmark = new Checkmark();
		sharedProcedureSelection = new SharedProcedureSelection();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentDOCIntletTemperature, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentDOCOutletTemperature, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentDPFOutletTemperature, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentSCRInletTemperature, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentSCROutletTemperature, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		dialInstrument1.AngleRange = 220.0;
		dialInstrument1.AngleStart = -200.0;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)dialInstrument1, 2);
		componentResourceManager.ApplyResources(dialInstrument1, "dialInstrument1");
		dialInstrument1.FontGroup = "readouts";
		((SingleInstrumentBase)dialInstrument1).FreezeValue = false;
		((SingleInstrumentBase)dialInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)dialInstrument1).Name = "dialInstrument1";
		((AxisSingleInstrumentBase)dialInstrument1).PreferredAxisRange = new AxisRange(0.0, 2500.0, "rpm");
		((SingleInstrumentBase)dialInstrument1).ShowValueReadout = false;
		((SingleInstrumentBase)dialInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "ATDDesaturation";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		barInstrumentDOCIntletTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentDOCIntletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentDOCIntletTemperature, "barInstrumentDOCIntletTemperature");
		barInstrumentDOCIntletTemperature.FontGroup = "thermometers";
		((SingleInstrumentBase)barInstrumentDOCIntletTemperature).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentDOCIntletTemperature).Gradient.Initialize((ValueState)1, 2, "°C");
		((AxisSingleInstrumentBase)barInstrumentDOCIntletTemperature).Gradient.Modify(1, 200.0, (ValueState)2);
		((AxisSingleInstrumentBase)barInstrumentDOCIntletTemperature).Gradient.Modify(2, 500.0, (ValueState)3);
		((SingleInstrumentBase)barInstrumentDOCIntletTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "DOCInletTemperature");
		((Control)(object)barInstrumentDOCIntletTemperature).Name = "barInstrumentDOCIntletTemperature";
		((AxisSingleInstrumentBase)barInstrumentDOCIntletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
		((SingleInstrumentBase)barInstrumentDOCIntletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentDOCIntletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentDOCIntletTemperature).UnitAlignment = StringAlignment.Near;
		barInstrumentDOCIntletTemperature.RepresentedStateChanged += barInstrument_RepresentedStateChanged;
		((SingleInstrumentBase)barInstrumentDOCIntletTemperature).DataChanged += barInstrument_DataChanged;
		barInstrumentDOCOutletTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentDOCOutletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentDOCOutletTemperature, "barInstrumentDOCOutletTemperature");
		barInstrumentDOCOutletTemperature.FontGroup = "thermometers";
		((SingleInstrumentBase)barInstrumentDOCOutletTemperature).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentDOCOutletTemperature).Gradient.Initialize((ValueState)1, 2, "°C");
		((AxisSingleInstrumentBase)barInstrumentDOCOutletTemperature).Gradient.Modify(1, 200.0, (ValueState)2);
		((AxisSingleInstrumentBase)barInstrumentDOCOutletTemperature).Gradient.Modify(2, 500.0, (ValueState)3);
		((SingleInstrumentBase)barInstrumentDOCOutletTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "DOCOutletTemperature");
		((Control)(object)barInstrumentDOCOutletTemperature).Name = "barInstrumentDOCOutletTemperature";
		((AxisSingleInstrumentBase)barInstrumentDOCOutletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
		((SingleInstrumentBase)barInstrumentDOCOutletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentDOCOutletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentDOCOutletTemperature).UnitAlignment = StringAlignment.Near;
		barInstrumentDOCOutletTemperature.RepresentedStateChanged += barInstrument_RepresentedStateChanged;
		((SingleInstrumentBase)barInstrumentDOCOutletTemperature).DataChanged += barInstrument_DataChanged;
		barInstrumentDPFOutletTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentDPFOutletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentDPFOutletTemperature, "barInstrumentDPFOutletTemperature");
		barInstrumentDPFOutletTemperature.FontGroup = "thermometers";
		((SingleInstrumentBase)barInstrumentDPFOutletTemperature).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentDPFOutletTemperature).Gradient.Initialize((ValueState)1, 2, "°C");
		((AxisSingleInstrumentBase)barInstrumentDPFOutletTemperature).Gradient.Modify(1, 200.0, (ValueState)2);
		((AxisSingleInstrumentBase)barInstrumentDPFOutletTemperature).Gradient.Modify(2, 500.0, (ValueState)3);
		((SingleInstrumentBase)barInstrumentDPFOutletTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "DPFOutletTemperature");
		((Control)(object)barInstrumentDPFOutletTemperature).Name = "barInstrumentDPFOutletTemperature";
		((AxisSingleInstrumentBase)barInstrumentDPFOutletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
		((SingleInstrumentBase)barInstrumentDPFOutletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentDPFOutletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentDPFOutletTemperature).UnitAlignment = StringAlignment.Near;
		barInstrumentDPFOutletTemperature.RepresentedStateChanged += barInstrument_RepresentedStateChanged;
		((SingleInstrumentBase)barInstrumentDPFOutletTemperature).DataChanged += barInstrument_DataChanged;
		barInstrumentSCRInletTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentSCRInletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentSCRInletTemperature, "barInstrumentSCRInletTemperature");
		barInstrumentSCRInletTemperature.FontGroup = "thermometers";
		((SingleInstrumentBase)barInstrumentSCRInletTemperature).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentSCRInletTemperature).Gradient.Initialize((ValueState)1, 2, "°C");
		((AxisSingleInstrumentBase)barInstrumentSCRInletTemperature).Gradient.Modify(1, 200.0, (ValueState)2);
		((AxisSingleInstrumentBase)barInstrumentSCRInletTemperature).Gradient.Modify(2, 500.0, (ValueState)3);
		((SingleInstrumentBase)barInstrumentSCRInletTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "SCRInletTemperature");
		((Control)(object)barInstrumentSCRInletTemperature).Name = "barInstrumentSCRInletTemperature";
		((AxisSingleInstrumentBase)barInstrumentSCRInletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
		((SingleInstrumentBase)barInstrumentSCRInletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentSCRInletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentSCRInletTemperature).UnitAlignment = StringAlignment.Near;
		barInstrumentSCRInletTemperature.RepresentedStateChanged += barInstrument_RepresentedStateChanged;
		((SingleInstrumentBase)barInstrumentSCRInletTemperature).DataChanged += barInstrument_DataChanged;
		barInstrumentSCROutletTemperature.BarOrientation = (ControlOrientation)1;
		barInstrumentSCROutletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentSCROutletTemperature, "barInstrumentSCROutletTemperature");
		barInstrumentSCROutletTemperature.FontGroup = "thermometers";
		((SingleInstrumentBase)barInstrumentSCROutletTemperature).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentSCROutletTemperature).Gradient.Initialize((ValueState)1, 2, "°C");
		((AxisSingleInstrumentBase)barInstrumentSCROutletTemperature).Gradient.Modify(1, 200.0, (ValueState)2);
		((AxisSingleInstrumentBase)barInstrumentSCROutletTemperature).Gradient.Modify(2, 500.0, (ValueState)3);
		((SingleInstrumentBase)barInstrumentSCROutletTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "SCROutletTemperature");
		((Control)(object)barInstrumentSCROutletTemperature).Name = "barInstrumentSCROutletTemperature";
		((AxisSingleInstrumentBase)barInstrumentSCROutletTemperature).PreferredAxisRange = new AxisRange(0.0, 1025.0, "");
		((SingleInstrumentBase)barInstrumentSCROutletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentSCROutletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentSCROutletTemperature).UnitAlignment = StringAlignment.Near;
		barInstrumentSCROutletTemperature.RepresentedStateChanged += barInstrument_RepresentedStateChanged;
		((SingleInstrumentBase)barInstrumentSCROutletTemperature).DataChanged += barInstrument_DataChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection, 2, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark, "checkmark");
		((Control)(object)checkmark).Name = "checkmark";
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DisableHcDoserParkedRegen_NGC" });
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmark;
		sharedProcedureIntegrationComponent.ResultsTarget = null;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
