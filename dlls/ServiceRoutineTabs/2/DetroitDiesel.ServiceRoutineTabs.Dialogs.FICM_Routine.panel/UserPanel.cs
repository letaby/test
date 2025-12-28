using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel;

public class UserPanel : CustomPanel
{
	private bool adrResult = false;

	private string adrMessage = Resources.Message_Test_Not_Run;

	private TableLayoutPanel tableLayoutPanelMain;

	private TableLayoutPanel tableLayoutPanelBottom;

	private Checkmark checkmarkStatus;

	private Button buttonStartStop;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheck;

	private SeekTimeListView seekTimeListView;

	private Label labelStatus;

	private SharedProcedureSelection sharedProcedureSelection1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private TextBox textBoxWarning;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public UserPanel()
	{
		InitializeComponent();
		sharedProcedureSelection1.SelectedProcedure.StopComplete += SelectedProcedure_StopComplete;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			sharedProcedureSelection1.SelectedProcedure.StopComplete -= SelectedProcedure_StopComplete;
			((Control)this).Tag = new object[2] { adrResult, adrMessage };
		}
	}

	private void LogText(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
		}
	}

	private void SelectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if ((int)sharedProcedureSelection1.SelectedProcedure.Result == 1)
		{
			adrMessage = Resources.Message_Success;
			adrResult = true;
		}
		else
		{
			adrMessage = Resources.Message_Stopped;
			adrResult = false;
		}
		LogText(adrMessage);
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Expected O, but got Unknown
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanelBottom = new TableLayoutPanel();
		checkmarkStatus = new Checkmark();
		buttonStartStop = new Button();
		labelStatus = new Label();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleCheck = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		textBoxWarning = new TextBox();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelBottom).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelBottom, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleCheck, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListView, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxWarning, 0, 0);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanelBottom, "tableLayoutPanelBottom");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelBottom, 2);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add(buttonStartStop, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add(labelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)sharedProcedureSelection1, 2, 0);
		((Control)(object)tableLayoutPanelBottom).Name = "tableLayoutPanelBottom";
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseCompatibleTextRendering = true;
		buttonStartStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[2] { "SP_Powertrain_Repair_Validation_Routine1", "SP_FuelInjectorCleaningMachine_Routine" });
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "DigitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)0);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(2, 1191.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleCheck, "digitalReadoutInstrumentVehicleCheck");
		digitalReadoutInstrumentVehicleCheck.FontGroup = "DigitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheck).FreezeValue = false;
		digitalReadoutInstrumentVehicleCheck.Gradient.Initialize((ValueState)3, 3);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(3, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheck).Instrument = new Qualifier((QualifierTypes)1, "virtual", "VehicleCheckStatus");
		((Control)(object)digitalReadoutInstrumentVehicleCheck).Name = "digitalReadoutInstrumentVehicleCheck";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheck).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "FICM";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)seekTimeListView, 4);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = "DigitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = "DigitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStartStop;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)textBoxWarning, 2);
		componentResourceManager.ApplyResources(textBoxWarning, "textBoxWarning");
		textBoxWarning.Name = "textBoxWarning";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Fuel_Injector_Cleaning_Machine_Routine");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelBottom).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelBottom).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
