using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inducement_system_activation.panel;

public class UserPanel : CustomPanel
{
	private const string AcmInducementSystemNotActivatedQualifier = "96100E";

	private const string InducementActivationStartQualifier = "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start";

	private const string Acm21TName = "ACM21T";

	private Channel acm = null;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private RunServiceButton runServiceButtonMCM;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label label2;

	private RunServiceButton runServiceButtonACM;

	public UserPanel()
	{
		InitializeComponent();
		((Control)(object)runServiceButtonACM).Enabled = false;
		((Control)(object)runServiceButtonACM).Enabled = false;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
	}

	public override void OnChannelsChanged()
	{
		SetACM(((CustomPanel)this).GetChannel("ACM21T", (ChannelLookupOptions)7));
	}

	private void SetACM(Channel acm)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (this.acm != acm)
		{
			this.acm = acm;
			if (this.acm != null)
			{
				((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)32, this.acm.Ecu.Name, "96100E");
				runServiceButtonACM.ServiceCall = new ServiceCall(this.acm.Ecu.Name, "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start");
			}
			if (acm == null || acm.Services["RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start"] == null)
			{
				((Control)(object)runServiceButtonACM).Enabled = false;
			}
		}
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
	}

	private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)digitalReadoutInstrument1.RepresentedState == 1)
		{
			((Control)(object)runServiceButtonACM).Enabled = false;
		}
		else
		{
			((Control)(object)runServiceButtonACM).Enabled = true;
		}
	}

	private void digitalReadoutInstrument2_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)digitalReadoutInstrument2.RepresentedState == 1)
		{
			((Control)(object)runServiceButtonMCM).Enabled = false;
		}
		else
		{
			((Control)(object)runServiceButtonMCM).Enabled = true;
		}
	}

	private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			e.Service.Channel.FaultCodes.Reset(synchronous: false);
		}
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
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_1133: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		runServiceButtonACM = new RunServiceButton();
		runServiceButtonMCM = new RunServiceButton();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonACM, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonMCM, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(runServiceButtonACM, "runServiceButtonACM");
		((Control)(object)runServiceButtonACM).Name = "runServiceButtonACM";
		runServiceButtonACM.ServiceCall = new ServiceCall("ACM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start");
		runServiceButtonACM.ServiceComplete += runServiceButton_ServiceComplete;
		componentResourceManager.ApplyResources(runServiceButtonMCM, "runServiceButtonMCM");
		((Control)(object)runServiceButtonMCM).Name = "runServiceButtonMCM";
		runServiceButtonMCM.ServiceCall = new ServiceCall("MCM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start");
		runServiceButtonMCM.ServiceComplete += runServiceButton_ServiceComplete;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)0, 64);
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(3, 4.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(4, 5.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(5, 8.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(6, 9.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(7, 12.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(8, 13.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(9, 32.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(10, 33.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(11, 36.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(12, 37.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(13, 40.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(14, 41.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(15, 44.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(16, 45.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(17, 128.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(18, 129.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(19, 132.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(20, 133.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(21, 136.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(22, 137.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(23, 140.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(24, 141.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(25, 160.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(26, 161.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(27, 164.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(28, 165.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(29, 168.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(30, 169.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(31, 172.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(32, 173.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(33, 256.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(34, 257.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(35, 260.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(36, 261.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(37, 264.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(38, 265.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(39, 268.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(40, 269.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(41, 288.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(42, 289.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(43, 292.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(44, 293.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(45, 296.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(46, 297.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(47, 300.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(48, 301.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(49, 384.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(50, 385.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(51, 388.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(52, 389.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(53, 392.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(54, 393.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(55, 396.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(56, 397.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(57, 416.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(58, 417.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(59, 420.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(60, 421.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(61, 424.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(62, 425.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(63, 428.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(64, 429.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)32, "ACM21T", "96100E");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrument1.RepresentedStateChanged += digitalReadoutInstrument1_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "base";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)0, 64);
		digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(3, 4.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(4, 5.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(5, 8.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(6, 9.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(7, 12.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(8, 13.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(9, 32.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(10, 33.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(11, 36.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(12, 37.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(13, 40.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(14, 41.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(15, 44.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(16, 45.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(17, 128.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(18, 129.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(19, 132.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(20, 133.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(21, 136.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(22, 137.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(23, 140.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(24, 141.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(25, 160.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(26, 161.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(27, 164.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(28, 165.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(29, 168.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(30, 169.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(31, 172.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(32, 173.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(33, 256.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(34, 257.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(35, 260.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(36, 261.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(37, 264.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(38, 265.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(39, 268.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(40, 269.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(41, 288.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(42, 289.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(43, 292.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(44, 293.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(45, 296.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(46, 297.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(47, 300.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(48, 301.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(49, 384.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(50, 385.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(51, 388.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(52, 389.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(53, 392.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(54, 393.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(55, 396.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(56, 397.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(57, 416.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(58, 417.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(59, 420.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(60, 421.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(61, 424.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(62, 425.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(63, 428.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(64, 429.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)32, "MCM21T", "7E140E");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrument2.RepresentedStateChanged += digitalReadoutInstrument2_RepresentedStateChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
