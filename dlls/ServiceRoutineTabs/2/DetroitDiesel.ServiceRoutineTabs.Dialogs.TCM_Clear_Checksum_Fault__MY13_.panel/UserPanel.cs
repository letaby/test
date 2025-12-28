using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clear_Checksum_Fault__MY13_.panel;

public class UserPanel : CustomPanel
{
	private TableLayoutPanel tableLayoutPanel1;

	private RunServiceButton runServiceButton1;

	private Button button1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		digitalReadoutInstrument1.RepresentedStateChanged += digitalReadoutInstrument1_RepresentedStateChanged;
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
		UpdateUserInterface();
	}

	private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		((Control)(object)runServiceButton1).Enabled = (int)digitalReadoutInstrument1.RepresentedState == 3;
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		digitalReadoutInstrument1.RepresentedStateChanged -= digitalReadoutInstrument1_RepresentedStateChanged;
		((CustomPanel)this).ParentFormClosing -= this_ParentFormClosing;
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		runServiceButton1 = new RunServiceButton();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		button1 = new Button();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(runServiceButton1, "runServiceButton1");
		((Control)(object)runServiceButton1).Name = "runServiceButton1";
		runServiceButton1.ServiceCall = new ServiceCall("TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start");
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButton1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button1, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "TCM CCF Font";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)32, "TCM01T", "18F3EE");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		button1.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "TCM CCF Font";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)32, "TCM01T", "00F1EE");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
