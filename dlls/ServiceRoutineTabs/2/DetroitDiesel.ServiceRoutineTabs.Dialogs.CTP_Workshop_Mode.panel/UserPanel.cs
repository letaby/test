using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrumentCtpWMMode;

	private Checkmark checkmarkReady;

	private RunServiceButton runServiceButtonWMOff;

	private Label labelStatus;

	public UserPanel()
	{
		InitializeComponent();
		UpdateUI();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		digitalReadoutInstrumentCtpWMMode.RepresentedStateChanged -= digitalReadoutInstrumentCtpWMMode_RepresentedStateChanged;
	}

	private void UpdateUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		if (((SingleInstrumentBase)digitalReadoutInstrumentCtpWMMode).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentCtpWMMode).DataItem.Value != null && (int)digitalReadoutInstrumentCtpWMMode.RepresentedState != 0)
		{
			Checkmark obj = checkmarkReady;
			bool flag = (((Control)(object)runServiceButtonWMOff).Enabled = (int)digitalReadoutInstrumentCtpWMMode.RepresentedState == 2);
			obj.Checked = flag;
			labelStatus.Text = (((int)digitalReadoutInstrumentCtpWMMode.RepresentedState == 2) ? Resources.Message_Ready : Resources.Message_WorkshopModeIsOff);
		}
		else
		{
			Checkmark obj2 = checkmarkReady;
			bool flag = (((Control)(object)runServiceButtonWMOff).Enabled = false);
			obj2.Checked = flag;
			labelStatus.Text = Resources.Message_UnableToRunRoutine;
		}
	}

	private void runServiceButtonWMOff_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		MessageBox.Show(Resources.Message_CTPWorkshopModeHasBeenTurnedOffDiagnosticLinkNeedsToBeClosedAndTheVehicleInterfaceAdaptorNeedsToBeDisconnectedFromTheDiagnosticPort, Resources.Message_WorkshopModeOff, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
	}

	private void digitalReadoutInstrumentCtpWMMode_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		checkmarkReady = new Checkmark();
		labelStatus = new Label();
		buttonClose = new Button();
		digitalReadoutInstrumentCtpWMMode = new DigitalReadoutInstrument();
		runServiceButtonWMOff = new RunServiceButton();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkReady, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCtpWMMode, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonWMOff, 2, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmarkReady, "checkmarkReady");
		((Control)(object)checkmarkReady).Name = "checkmarkReady";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatus, 3);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		digitalReadoutInstrumentCtpWMMode.Alignment = StringAlignment.Center;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentCtpWMMode, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCtpWMMode, "digitalReadoutInstrumentCtpWMMode");
		digitalReadoutInstrumentCtpWMMode.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCtpWMMode).FreezeValue = false;
		digitalReadoutInstrumentCtpWMMode.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentCtpWMMode.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentCtpWMMode.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentCtpWMMode.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentCtpWMMode.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentCtpWMMode.Gradient.Modify(2, 1.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentCtpWMMode).Instrument = new Qualifier((QualifierTypes)1, "CTP01T", "DT_STO_Workshop_Mode_Workshop_Mode");
		((Control)(object)digitalReadoutInstrumentCtpWMMode).Name = "digitalReadoutInstrumentCtpWMMode";
		((SingleInstrumentBase)digitalReadoutInstrumentCtpWMMode).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCtpWMMode).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentCtpWMMode.RepresentedStateChanged += digitalReadoutInstrumentCtpWMMode_RepresentedStateChanged;
		componentResourceManager.ApplyResources(runServiceButtonWMOff, "runServiceButtonWMOff");
		((Control)(object)runServiceButtonWMOff).Name = "runServiceButtonWMOff";
		runServiceButtonWMOff.ServiceCall = new ServiceCall("CTP01T", "DL_Workshop_Mode", (IEnumerable<string>)new string[1] { "Workshop_Mode=0" });
		runServiceButtonWMOff.ServiceComplete += runServiceButtonWMOff_ServiceComplete;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
