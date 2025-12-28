using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel;

public class UserPanel : CustomPanel
{
	private const string PtfConfPTransGroup = "VCD_PID_222_ptconf_p_Trans";

	private const string PtfConfPTransEmotNum = "ptconf_p_Trans_EmotNum_u8";

	private Channel eCpcChannel;

	private bool isEmot3Num = false;

	private bool oilPump1Started = false;

	private bool oilPump2Started = false;

	private Parameter emotNumParameter = null;

	private SelectablePanel selectablePanel1;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonClose;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentOilPump1;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentOilPump1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentOilPump2;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentOilPump2;

	private Button buttonStartOilPump2;

	private System.Windows.Forms.Label labelStatusLabelOilPump2;

	private System.Windows.Forms.Label labelStatus2;

	private Checkmark checkmarkOilPump2;

	private System.Windows.Forms.Label label1;

	private SeekTimeListView seekTimeListView1;

	private TableLayoutPanel tableLayoutPanel4;

	private Label labelOilPumpHeader1;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private ScalingLabel scalingLabelOilPump2;

	private SharedProcedureSelection sharedProcedureSelectionOilPump1;

	private Button buttonStartOilPump1;

	private Checkmark checkmarkOilPump1;

	private System.Windows.Forms.Label labelStatus1;

	private System.Windows.Forms.Label labelStatusLabelOilPump1;

	private Label labelOilPumpHeader2;

	private SharedProcedureSelection sharedProcedureSelectionOilPump2;

	private ScalingLabel scalingLabelOilPump1;

	private bool EcpcOnline => eCpcChannel != null && eCpcChannel.CommunicationsState == CommunicationsState.Online;

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
		oilPump1Started = false;
		oilPump2Started = false;
		UpdateUI();
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetECPC01TChannel(((CustomPanel)this).GetChannel("ECPC01T", (ChannelLookupOptions)3));
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = oilPump1Started || oilPump2Started;
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			if (eCpcChannel != null)
			{
				eCpcChannel.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
		}
	}

	private void SetECPC01TChannel(Channel eCpc)
	{
		if (eCpcChannel != eCpc)
		{
			if (eCpcChannel != null)
			{
				eCpcChannel.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			eCpcChannel = eCpc;
			if (eCpcChannel != null)
			{
				oilPump1Started = false;
				oilPump2Started = false;
				eCpcChannel.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				emotNumParameter = eCpcChannel.Parameters["ptconf_p_Trans_EmotNum_u8"];
				if (EcpcOnline)
				{
					ReadInitialParameters();
				}
			}
		}
		UpdateUI();
	}

	private void ReadInitialParameters()
	{
		if (eCpcChannel.CommunicationsState == CommunicationsState.Online && eCpcChannel.Parameters != null && !emotNumParameter.HasBeenReadFromEcu)
		{
			eCpcChannel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersInitialReadCompleteEvent;
			eCpcChannel.Parameters.ReadGroup(emotNumParameter.GroupQualifier, fromCache: true, synchronous: false);
		}
	}

	private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
	{
		eCpcChannel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersInitialReadCompleteEvent;
		UpdateUI();
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		ReadInitialParameters();
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
		}
	}

	private void UpdateUI()
	{
		int result = 1;
		if (emotNumParameter != null && emotNumParameter.Value != null)
		{
			result = (int.TryParse(emotNumParameter.Value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : 3);
		}
		isEmot3Num = result == 3;
		sharedProcedureIntegrationComponentOilPump2.ProceduresDropDown = (isEmot3Num ? sharedProcedureSelectionOilPump2 : null);
		((Control)(object)labelOilPumpHeader2).Visible = isEmot3Num;
		((Control)(object)scalingLabelOilPump2).Visible = isEmot3Num;
		((Control)(object)checkmarkOilPump2).Visible = isEmot3Num;
		labelStatus2.Visible = isEmot3Num;
		labelStatusLabelOilPump2.Visible = isEmot3Num;
		buttonStartOilPump2.Visible = isEmot3Num;
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[9].Height = (isEmot3Num ? 25f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[10].Height = (isEmot3Num ? 20f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[11].Height = (isEmot3Num ? 20f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[12].Height = (isEmot3Num ? 20f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[13].Height = (isEmot3Num ? 20f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[14].Height = (isEmot3Num ? 20f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[15].Height = (isEmot3Num ? 39f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[16].Height = (isEmot3Num ? 8f : 0f);
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[17].Height = (isEmot3Num ? 32f : 0f);
		buttonClose.Enabled = ((!isEmot3Num) ? (!oilPump1Started) : (!oilPump1Started && !oilPump2Started));
	}

	private void sharedProcedureCreatorComponentOilPump1_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			oilPump1Started = true;
			((Control)(object)scalingLabelOilPump1).Text = "0.0";
			AddLogLabel(Resources.Message_OilPump1_Started);
		}
		else
		{
			AddLogLabel(Resources.Message_OilPump1_FailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentOilPump1_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		oilPump1Started = false;
		((Control)(object)scalingLabelOilPump1).Text = "0.0";
		AddLogLabel(Resources.Message_OilPump1_Stopped);
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentOilPump1_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		if (e.Service.OutputValues.Count() > 0)
		{
			((Control)(object)scalingLabelOilPump1).Text = e.Service.OutputValues[0].Value.ToString();
		}
	}

	private void sharedProcedureCreatorComponentOilPump2_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			oilPump2Started = true;
			((Control)(object)scalingLabelOilPump2).Text = "0.0";
			AddLogLabel(Resources.Message_OilPump2_Started);
		}
		else
		{
			AddLogLabel(Resources.Message_OilPump2_FailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentOilPump2_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		oilPump2Started = false;
		((Control)(object)scalingLabelOilPump2).Text = "0.0";
		AddLogLabel(Resources.Message_OilPump2_Stopped);
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentOilPump2_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		if (e.Service.OutputValues.Count() > 0)
		{
			((Control)(object)scalingLabelOilPump2).Text = e.Service.OutputValues[0].Value.ToString();
		}
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Expected O, but got Unknown
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb2: Expected O, but got Unknown
		//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1022: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1105: Unknown result type (might be due to invalid IL or missing references)
		//IL_1177: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		DataItemCondition val4 = new DataItemCondition();
		selectablePanel1 = new SelectablePanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		sharedProcedureSelectionOilPump1 = new SharedProcedureSelection();
		buttonStartOilPump2 = new Button();
		buttonStartOilPump1 = new Button();
		label1 = new System.Windows.Forms.Label();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		labelOilPumpHeader1 = new Label();
		scalingLabelOilPump1 = new ScalingLabel();
		checkmarkOilPump1 = new Checkmark();
		labelStatus1 = new System.Windows.Forms.Label();
		labelStatusLabelOilPump1 = new System.Windows.Forms.Label();
		labelOilPumpHeader2 = new Label();
		scalingLabelOilPump2 = new ScalingLabel();
		checkmarkOilPump2 = new Checkmark();
		labelStatus2 = new System.Windows.Forms.Label();
		labelStatusLabelOilPump2 = new System.Windows.Forms.Label();
		sharedProcedureSelectionOilPump2 = new SharedProcedureSelection();
		buttonClose = new Button();
		sharedProcedureIntegrationComponentOilPump1 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentOilPump1 = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentOilPump2 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentOilPump2 = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)selectablePanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)(object)selectablePanel1).Controls.Add((Control)(object)tableLayoutPanel1);
		componentResourceManager.ApplyResources(selectablePanel1, "selectablePanel1");
		((Control)(object)selectablePanel1).Name = "selectablePanel1";
		((Panel)(object)selectablePanel1).TabStop = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelectionOilPump1, 5, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStartOilPump2, 5, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStartOilPump1, 5, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 3, 18);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel4, 0, 18);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelOilPumpHeader1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)scalingLabelOilPump1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkOilPump1, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus1, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatusLabelOilPump1, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelOilPumpHeader2, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)scalingLabelOilPump2, 0, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkOilPump2, 0, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus2, 1, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatusLabelOilPump2, 2, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelectionOilPump2, 5, 17);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 5, 20);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(sharedProcedureSelectionOilPump1, "sharedProcedureSelectionOilPump1");
		((Control)(object)sharedProcedureSelectionOilPump1).Name = "sharedProcedureSelectionOilPump1";
		sharedProcedureSelectionOilPump1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "ETHM_OilPumpCtr" });
		componentResourceManager.ApplyResources(buttonStartOilPump2, "buttonStartOilPump2");
		buttonStartOilPump2.Name = "buttonStartOilPump2";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)buttonStartOilPump2, 2);
		buttonStartOilPump2.UseCompatibleTextRendering = true;
		buttonStartOilPump2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStartOilPump1, "buttonStartOilPump1");
		buttonStartOilPump1.Name = "buttonStartOilPump1";
		buttonStartOilPump1.UseCompatibleTextRendering = true;
		buttonStartOilPump1.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(label1, "label1");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label1, 6);
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "eTransmissionOilPumpControls";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView1, 2);
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel4, 3);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 5);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel4, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((TableLayoutPanel)(object)tableLayoutPanel4).SetRowSpan((Control)(object)digitalReadoutInstrumentParkBrake, 5);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
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
		((TableLayoutPanel)(object)tableLayoutPanel4).SetRowSpan((Control)(object)digitalReadoutInstrumentVehicleSpeed, 5);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		labelOilPumpHeader1.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelOilPumpHeader1, 6);
		componentResourceManager.ApplyResources(labelOilPumpHeader1, "labelOilPumpHeader1");
		((Control)(object)labelOilPumpHeader1).Name = "labelOilPumpHeader1";
		labelOilPumpHeader1.Orientation = (TextOrientation)1;
		scalingLabelOilPump1.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)scalingLabelOilPump1, 6);
		componentResourceManager.ApplyResources(scalingLabelOilPump1, "scalingLabelOilPump1");
		scalingLabelOilPump1.FontGroup = null;
		scalingLabelOilPump1.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelOilPump1).Name = "scalingLabelOilPump1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)scalingLabelOilPump1, 5);
		componentResourceManager.ApplyResources(checkmarkOilPump1, "checkmarkOilPump1");
		((Control)(object)checkmarkOilPump1).Name = "checkmarkOilPump1";
		componentResourceManager.ApplyResources(labelStatus1, "labelStatus1");
		labelStatus1.Name = "labelStatus1";
		labelStatus1.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatusLabelOilPump1, 3);
		componentResourceManager.ApplyResources(labelStatusLabelOilPump1, "labelStatusLabelOilPump1");
		labelStatusLabelOilPump1.Name = "labelStatusLabelOilPump1";
		labelStatusLabelOilPump1.UseCompatibleTextRendering = true;
		labelOilPumpHeader2.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelOilPumpHeader2, 6);
		componentResourceManager.ApplyResources(labelOilPumpHeader2, "labelOilPumpHeader2");
		((Control)(object)labelOilPumpHeader2).Name = "labelOilPumpHeader2";
		labelOilPumpHeader2.Orientation = (TextOrientation)1;
		scalingLabelOilPump2.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)scalingLabelOilPump2, 6);
		componentResourceManager.ApplyResources(scalingLabelOilPump2, "scalingLabelOilPump2");
		scalingLabelOilPump2.FontGroup = null;
		scalingLabelOilPump2.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelOilPump2).Name = "scalingLabelOilPump2";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)scalingLabelOilPump2, 5);
		componentResourceManager.ApplyResources(checkmarkOilPump2, "checkmarkOilPump2");
		((Control)(object)checkmarkOilPump2).Name = "checkmarkOilPump2";
		componentResourceManager.ApplyResources(labelStatus2, "labelStatus2");
		labelStatus2.Name = "labelStatus2";
		labelStatus2.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatusLabelOilPump2, 3);
		componentResourceManager.ApplyResources(labelStatusLabelOilPump2, "labelStatusLabelOilPump2");
		labelStatusLabelOilPump2.Name = "labelStatusLabelOilPump2";
		labelStatusLabelOilPump2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelectionOilPump2, "sharedProcedureSelectionOilPump2");
		((Control)(object)sharedProcedureSelectionOilPump2).Name = "sharedProcedureSelectionOilPump2";
		sharedProcedureSelectionOilPump2.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "ETHM_OilPumpCtrl2" });
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		sharedProcedureIntegrationComponentOilPump1.ProceduresDropDown = sharedProcedureSelectionOilPump1;
		sharedProcedureIntegrationComponentOilPump1.ProcedureStatusMessageTarget = labelStatusLabelOilPump1;
		sharedProcedureIntegrationComponentOilPump1.ProcedureStatusStateTarget = checkmarkOilPump1;
		sharedProcedureIntegrationComponentOilPump1.ResultsTarget = null;
		sharedProcedureIntegrationComponentOilPump1.StartStopButton = buttonStartOilPump1;
		sharedProcedureIntegrationComponentOilPump1.StopAllButton = null;
		sharedProcedureCreatorComponentOilPump1.Suspend();
		sharedProcedureCreatorComponentOilPump1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump1_Control_results_resp");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentOilPump1, "sharedProcedureCreatorComponentOilPump1");
		sharedProcedureCreatorComponentOilPump1.Qualifier = "ETHM_OilPumpCtr";
		sharedProcedureCreatorComponentOilPump1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump1_Control_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control=100", "ETHM_Oil_Pump2_Control=0" });
		val.Gradient.Initialize((ValueState)3, 5, "mph");
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)3);
		val.Gradient.Modify(4, 3.0, (ValueState)3);
		val.Gradient.Modify(5, 2147483647.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
		val2.Gradient.Initialize((ValueState)0, 4);
		val2.Gradient.Modify(1, 0.0, (ValueState)3);
		val2.Gradient.Modify(2, 1.0, (ValueState)1);
		val2.Gradient.Modify(3, 2.0, (ValueState)3);
		val2.Gradient.Modify(4, 3.0, (ValueState)3);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		sharedProcedureCreatorComponentOilPump1.StartConditions.Add(val);
		sharedProcedureCreatorComponentOilPump1.StartConditions.Add(val2);
		sharedProcedureCreatorComponentOilPump1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump1_Control_stop_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control_stop=0", "ETHM_Oil_Pump2_Control_stop=0" });
		sharedProcedureCreatorComponentOilPump1.StartServiceComplete += sharedProcedureCreatorComponentOilPump1_StartServiceComplete;
		sharedProcedureCreatorComponentOilPump1.StopServiceComplete += sharedProcedureCreatorComponentOilPump1_StopServiceComplete;
		sharedProcedureCreatorComponentOilPump1.MonitorServiceComplete += sharedProcedureCreatorComponentOilPump1_MonitorServiceComplete;
		sharedProcedureCreatorComponentOilPump1.Resume();
		sharedProcedureIntegrationComponentOilPump2.ProceduresDropDown = sharedProcedureSelectionOilPump2;
		sharedProcedureIntegrationComponentOilPump2.ProcedureStatusMessageTarget = labelStatusLabelOilPump2;
		sharedProcedureIntegrationComponentOilPump2.ProcedureStatusStateTarget = checkmarkOilPump2;
		sharedProcedureIntegrationComponentOilPump2.ResultsTarget = null;
		sharedProcedureIntegrationComponentOilPump2.StartStopButton = buttonStartOilPump2;
		sharedProcedureIntegrationComponentOilPump2.StopAllButton = null;
		sharedProcedureCreatorComponentOilPump2.Suspend();
		sharedProcedureCreatorComponentOilPump2.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump2_Control_results_resp");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentOilPump2, "sharedProcedureCreatorComponentOilPump2");
		sharedProcedureCreatorComponentOilPump2.Qualifier = "ETHM_OilPumpCtrl2";
		sharedProcedureCreatorComponentOilPump2.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump2_Control_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control=0", "ETHM_Oil_Pump2_Control=100" });
		val3.Gradient.Initialize((ValueState)3, 5, "mph");
		val3.Gradient.Modify(1, 0.0, (ValueState)3);
		val3.Gradient.Modify(2, 1.0, (ValueState)1);
		val3.Gradient.Modify(3, 2.0, (ValueState)3);
		val3.Gradient.Modify(4, 3.0, (ValueState)3);
		val3.Gradient.Modify(5, 2147483647.0, (ValueState)3);
		val3.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
		val4.Gradient.Initialize((ValueState)0, 4);
		val4.Gradient.Modify(1, 0.0, (ValueState)3);
		val4.Gradient.Modify(2, 1.0, (ValueState)1);
		val4.Gradient.Modify(3, 2.0, (ValueState)3);
		val4.Gradient.Modify(4, 3.0, (ValueState)3);
		val4.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		sharedProcedureCreatorComponentOilPump2.StartConditions.Add(val3);
		sharedProcedureCreatorComponentOilPump2.StartConditions.Add(val4);
		sharedProcedureCreatorComponentOilPump2.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump2_Control_stop_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control_stop=0", "ETHM_Oil_Pump2_Control_stop=0" });
		sharedProcedureCreatorComponentOilPump2.StartServiceComplete += sharedProcedureCreatorComponentOilPump2_StartServiceComplete;
		sharedProcedureCreatorComponentOilPump2.StopServiceComplete += sharedProcedureCreatorComponentOilPump2_StopServiceComplete;
		sharedProcedureCreatorComponentOilPump2.MonitorServiceComplete += sharedProcedureCreatorComponentOilPump2_MonitorServiceComplete;
		sharedProcedureCreatorComponentOilPump2.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Transmission_Oil_Pump_Controls");
		((Control)this).Controls.Add((Control)(object)selectablePanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)selectablePanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
