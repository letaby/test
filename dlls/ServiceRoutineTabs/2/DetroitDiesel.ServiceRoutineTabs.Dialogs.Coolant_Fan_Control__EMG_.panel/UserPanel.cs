using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Coolant_Fan_Control__EMG_.panel;

public class UserPanel : CustomPanel
{
	private const string FanCtrlRequestResultsQualifier = "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan";

	private const string CpcName = "ECPC01T";

	private TableLayoutPanel tableLayoutPanel1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private TableLayoutPanel tableLayoutPanelFooter;

	private Checkmark checkmarkReady;

	private System.Windows.Forms.Label labelStatus;

	private SharedProcedureSelection sharedProcedureSelectionCoolantFanControl;

	private Button buttonClose;

	private Button buttonStartStop;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private System.Windows.Forms.Label labelStatus2;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponent1;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void sharedProcedureCreatorComponent1_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		ServiceCall val = default(ServiceCall);
		((ServiceCall)(ref val))._002Ector("ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan");
		bool flag = 1 == 0;
		Service service = ((ServiceCall)(ref val)).GetService();
		if (service != null)
		{
			service.Execute(synchronous: false);
		}
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Expected O, but got Unknown
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		tableLayoutPanelFooter = new TableLayoutPanel();
		labelStatus2 = new System.Windows.Forms.Label();
		checkmarkReady = new Checkmark();
		labelStatus = new System.Windows.Forms.Label();
		sharedProcedureSelectionCoolantFanControl = new SharedProcedureSelection();
		buttonClose = new Button();
		buttonStartStop = new Button();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponent1 = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelFooter).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelFooter, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "CoolantControls";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS252_DrvCircInTemp_u16_DrvCircInTemp_u16");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "CoolantControls";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "CoolantControls";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).ShowValueReadout = false;
		((Control)(object)digitalReadoutInstrument3).TabStop = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "CoolantControls";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)3, 2, "mph");
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((Control)(object)digitalReadoutInstrument1).TabStop = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelFooter, "tableLayoutPanelFooter");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelFooter, 3);
		((TableLayoutPanel)(object)tableLayoutPanelFooter).Controls.Add(labelStatus2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFooter).Controls.Add((Control)(object)checkmarkReady, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFooter).Controls.Add(labelStatus, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFooter).Controls.Add((Control)(object)sharedProcedureSelectionCoolantFanControl, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFooter).Controls.Add(buttonClose, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFooter).Controls.Add(buttonStartStop, 4, 0);
		((Control)(object)tableLayoutPanelFooter).Name = "tableLayoutPanelFooter";
		componentResourceManager.ApplyResources(labelStatus2, "labelStatus2");
		labelStatus2.Name = "labelStatus2";
		labelStatus2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkReady, "checkmarkReady");
		((Control)(object)checkmarkReady).Name = "checkmarkReady";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelectionCoolantFanControl, "sharedProcedureSelectionCoolantFanControl");
		((Control)(object)sharedProcedureSelectionCoolantFanControl).Name = "sharedProcedureSelectionCoolantFanControl";
		sharedProcedureSelectionCoolantFanControl.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_CoolantFanControl" });
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "CoolantControls";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelectionCoolantFanControl;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmarkReady;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStartStop;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		sharedProcedureCreatorComponent1.Suspend();
		sharedProcedureCreatorComponent1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponent1, "sharedProcedureCreatorComponent1");
		sharedProcedureCreatorComponent1.Qualifier = "SP_CoolantFanControl";
		sharedProcedureCreatorComponent1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Start", (IEnumerable<string>)new string[3] { "Requested_duty_cycle_for_Brake_Resistor_1=0", "Requested_duty_cycle_for_Brake_Resistor_2=0", "Requested_duty_cycle_for_Edrive_Fan=50" });
		val.Gradient.Initialize((ValueState)3, 2, "mph");
		val.Gradient.Modify(1, 0.0, (ValueState)1);
		val.Gradient.Modify(2, 1.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		val2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		val2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		val2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		val2.Gradient.Initialize((ValueState)0, 2);
		val2.Gradient.Modify(1, 0.0, (ValueState)3);
		val2.Gradient.Modify(2, 1.0, (ValueState)1);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		sharedProcedureCreatorComponent1.StartConditions.Add(val);
		sharedProcedureCreatorComponent1.StartConditions.Add(val2);
		sharedProcedureCreatorComponent1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Stop", (IEnumerable<string>)new string[3] { "OTF_ETHM_FanCtrl_FanBrakeResistor1=0", "OTF_ETHM_FanCtrl_FanBrakeResistor2=0", "OTF_ETHM_FanCtrl_eDriveFan=0" });
		sharedProcedureCreatorComponent1.StopServiceComplete += sharedProcedureCreatorComponent1_StopServiceComplete;
		sharedProcedureCreatorComponent1.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Cooling_Fan_Control");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFooter).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
