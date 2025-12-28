using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel;

public class UserPanel : CustomPanel
{
	private Channel dcb01t;

	private Channel dcb02t;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrumentCharging;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransmission;

	private System.Windows.Forms.Label label1;

	private DigitalReadoutInstrument digitalReadoutInstrumentDCB01T;

	private DigitalReadoutInstrument digitalReadoutInstrumentDCB02T;

	private SeekTimeListView seekTimeListView;

	private RunServiceButton runServiceButtonDCB01T;

	private RunServiceButton runServiceButtonDCB02T;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private bool VehicleCharging => (int)digitalReadoutInstrumentCharging.RepresentedState != 1;

	private bool VehicleCheckOk => (int)digitalReadoutInstrumentParkBrake.RepresentedState != 3 && (int)digitalReadoutInstrumentTransmission.RepresentedState == 1 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && !VehicleCharging;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void AddLabelLog(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	public override void OnChannelsChanged()
	{
		dcb01t = ((CustomPanel)this).GetChannel("DCB01T");
		dcb02t = ((CustomPanel)this).GetChannel("DCB02T");
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		DigitalReadoutInstrument obj = digitalReadoutInstrumentDCB02T;
		bool visible = (((Control)(object)runServiceButtonDCB02T).Visible = dcb02t != null);
		((Control)(object)obj).Visible = visible;
		((Control)(object)runServiceButtonDCB02T).Enabled = dcb02t != null && VehicleCheckOk;
		((Control)(object)runServiceButtonDCB01T).Enabled = VehicleCheckOk;
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void runServices_Complete(string dcbName, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLabelLog(string.Format(Resources.MessageFormat_0UnlockComplete, dcbName));
		}
		else
		{
			AddLabelLog(string.Format(Resources.MessageFormat_0UnlockFailed1, dcbName, (((ResultEventArgs)(object)e).Exception == null) ? string.Empty : (Resources.Message_Error + ((ResultEventArgs)(object)e).Exception.Message)));
		}
	}

	private void runServiceButtonDCB01T_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		runServices_Complete("DCB01T", e);
	}

	private void runServiceButtonDCB02T_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		runServices_Complete("DCB02T", e);
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
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		runServiceButtonDCB02T = new RunServiceButton();
		seekTimeListView = new SeekTimeListView();
		digitalReadoutInstrumentDCB01T = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDCB02T = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransmission = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		runServiceButtonDCB01T = new RunServiceButton();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonDCB02T, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentDCB01T, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentDCB02T, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCharging, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentTransmission, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonDCB01T, 2, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(runServiceButtonDCB02T, "runServiceButtonDCB02T");
		((Control)(object)runServiceButtonDCB02T).Name = "runServiceButtonDCB02T";
		runServiceButtonDCB02T.ServiceCall = new ServiceCall("DCB02T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "HV_Lock_Status=0" });
		runServiceButtonDCB02T.ServiceComplete += runServiceButtonDCB02T_ServiceComplete;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 2);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "DCB Unlock";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDCB01T, "digitalReadoutInstrumentDCB01T");
		digitalReadoutInstrumentDCB01T.FontGroup = "DCB Unlock Text";
		((SingleInstrumentBase)digitalReadoutInstrumentDCB01T).FreezeValue = false;
		digitalReadoutInstrumentDCB01T.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentDCB01T.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentDCB01T.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentDCB01T).Instrument = new Qualifier((QualifierTypes)8, "DCB01T", "DT_STO_High_Voltage_Lock_HV_Lock_Status");
		((Control)(object)digitalReadoutInstrumentDCB01T).Name = "digitalReadoutInstrumentDCB01T";
		((SingleInstrumentBase)digitalReadoutInstrumentDCB01T).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDCB01T).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDCB02T, "digitalReadoutInstrumentDCB02T");
		digitalReadoutInstrumentDCB02T.FontGroup = "DCB Unlock Text";
		((SingleInstrumentBase)digitalReadoutInstrumentDCB02T).FreezeValue = false;
		digitalReadoutInstrumentDCB02T.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentDCB02T.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentDCB02T.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentDCB02T).Instrument = new Qualifier((QualifierTypes)8, "DCB02T", "DT_STO_High_Voltage_Lock_HV_Lock_Status");
		((Control)(object)digitalReadoutInstrumentDCB02T).Name = "digitalReadoutInstrumentDCB02T";
		((SingleInstrumentBase)digitalReadoutInstrumentDCB02T).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDCB02T).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label1, 3);
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
		digitalReadoutInstrumentCharging.FontGroup = "DCB Unlock Text";
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).FreezeValue = false;
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition");
		((Control)(object)digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentCharging.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = "DCB Unlock Text";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransmission, "digitalReadoutInstrumentTransmission");
		digitalReadoutInstrumentTransmission.FontGroup = "DCB Unlock Text";
		((SingleInstrumentBase)digitalReadoutInstrumentTransmission).FreezeValue = false;
		digitalReadoutInstrumentTransmission.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentTransmission.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentTransmission.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentTransmission).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		((Control)(object)digitalReadoutInstrumentTransmission).Name = "digitalReadoutInstrumentTransmission";
		((SingleInstrumentBase)digitalReadoutInstrumentTransmission).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentTransmission.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = "DCB Unlock Text";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(runServiceButtonDCB01T, "runServiceButtonDCB01T");
		((Control)(object)runServiceButtonDCB01T).Name = "runServiceButtonDCB01T";
		runServiceButtonDCB01T.ServiceCall = new ServiceCall("DCB01T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "HV_Lock_Status=0" });
		runServiceButtonDCB01T.ServiceComplete += runServiceButtonDCB01T_ServiceComplete;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_DCB_Unlock");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
