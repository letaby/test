using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Clutch_Overload_Monitoring.panel;

public class UserPanel : CustomPanel, IRefreshable
{
	private Channel tcm;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument9;

	private DigitalReadoutInstrument digitalReadoutInstrument10;

	private DigitalReadoutInstrument digitalReadoutInstrument11;

	private DigitalReadoutInstrument digitalReadoutInstrument12;

	private DigitalReadoutInstrument digitalReadoutInstrument13;

	private DigitalReadoutInstrument digitalReadoutInstrument14;

	private DigitalReadoutInstrument digitalReadoutInstrument15;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage1;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage2;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage3;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage4;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage5;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage6;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage7;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage8;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage9;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage10;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage11;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage12;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage13;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage14;

	private DigitalReadoutInstrument digitalReadoutInstrumentMileage15;

	private BarInstrument barInstrument1;

	private BarInstrument barInstrument2;

	private BarInstrument barInstrument3;

	private BarInstrument barInstrument4;

	private BarInstrument barInstrument5;

	private BarInstrument barInstrument6;

	private BarInstrument barInstrument7;

	private BarInstrument barInstrument8;

	private BarInstrument barInstrument9;

	private BarInstrument barInstrument10;

	private BarInstrument barInstrument11;

	private BarInstrument barInstrument12;

	private BarInstrument barInstrument13;

	private BarInstrument barInstrument14;

	private BarInstrument barInstrument15;

	private DigitalReadoutInstrument digitalReadoutInstrument31;

	private DigitalReadoutInstrument digitalReadoutInstrument32;

	private Label label16;

	private Label label15;

	private Label label14;

	private Label label13;

	private Label label12;

	private Label label11;

	private Label label10;

	private Label label9;

	private Label label8;

	private Label label7;

	private Label label6;

	private Label label5;

	private Label label4;

	private Label label3;

	private Label label1;

	private RunServiceButton runServiceButtonClear;

	private DigitalReadoutInstrument digitalReadoutInstrument33;

	public bool CanRefreshView => SapiManager.GlobalInstance.Online && tcm != null && tcm.CommunicationsState == CommunicationsState.Online;

	public UserPanel()
	{
		InitializeComponent();
		runServiceButtonClear.ServiceComplete += OnServiceComplete;
	}

	public override void OnChannelsChanged()
	{
		UpdateChannel();
	}

	private void UpdateChannel()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		Channel channel = ((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7);
		if (tcm == channel)
		{
			return;
		}
		tcm = channel;
		if (tcm == null)
		{
			return;
		}
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecuByName != null && ecuByName.Identifier == channel.Ecu.Identifier && ecuByName.Name != channel.Ecu.Name)
			{
				instrument = val.Instrument;
				QualifierTypes type = ((Qualifier)(ref instrument)).Type;
				string name = channel.Ecu.Name;
				instrument = val.Instrument;
				val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			}
		}
		if (tcm.Ecu.Name == "TCM01T")
		{
			runServiceButtonClear.ServiceCall = new ServiceCall("TCM01T", "RT_0471_KupplungUeberdrueckt_Zaehler_zuruecksetzen_Start");
		}
		else if (tcm.Ecu.Name == "TCM05T")
		{
			runServiceButtonClear.ServiceCall = new ServiceCall("TCM05T", "RT_0471_Reset_clutch_overload_event_counter_Start");
		}
	}

	private void OnServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			MessageBox.Show(e.Service.Name + Resources.Message_CompletedSuccessfully, ApplicationInformation.ProductName);
		}
		else
		{
			MessageBox.Show(e.Service.Name + Resources.Message_Failed + ((ResultEventArgs)(object)e).Exception.Message + "'", ApplicationInformation.ProductName);
		}
	}

	public void RefreshView()
	{
		if (SapiManager.GlobalInstance.Online && tcm != null && tcm.CommunicationsState == CommunicationsState.Online)
		{
			tcm.EcuInfos.Read(synchronous: false);
		}
	}

	private void duration_DataChanged(object sender, EventArgs e)
	{
		UpdateVisibility((SingleInstrumentBase)((sender is SingleInstrumentBase) ? sender : null));
	}

	private void temperature_DataChanged(object sender, EventArgs e)
	{
		UpdateVisibility((SingleInstrumentBase)((sender is SingleInstrumentBase) ? sender : null));
	}

	private void UpdateVisibility(SingleInstrumentBase updatedInstrument)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		if (updatedInstrument != null)
		{
			int row = ((TableLayoutPanel)(object)tableLayoutPanel1).GetRow((Control)(object)updatedInstrument);
			SingleInstrumentBase val = (SingleInstrumentBase)((TableLayoutPanel)(object)tableLayoutPanel1).GetControlFromPosition(2, row);
			SingleInstrumentBase val2 = (SingleInstrumentBase)((TableLayoutPanel)(object)tableLayoutPanel1).GetControlFromPosition(3, row);
			if (val.DataItem != null && val.DataItem.Value != null && val2.DataItem != null && val2.DataItem.Value != null)
			{
				((TableLayoutPanel)(object)tableLayoutPanel1).GetControlFromPosition(1, row).Visible = val.DataItem.Value.ToString() != "s.n.a." || val2.DataItem.Value.ToString() != "s.n.a.";
			}
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
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Expected O, but got Unknown
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Expected O, but got Unknown
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Expected O, but got Unknown
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Expected O, but got Unknown
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Expected O, but got Unknown
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_105d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_117b: Unknown result type (might be due to invalid IL or missing references)
		//IL_120a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1299: Unknown result type (might be due to invalid IL or missing references)
		//IL_1328: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1446: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1564: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1682: Unknown result type (might be due to invalid IL or missing references)
		//IL_1731: Unknown result type (might be due to invalid IL or missing references)
		//IL_176a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1808: Unknown result type (might be due to invalid IL or missing references)
		//IL_1841: Unknown result type (might be due to invalid IL or missing references)
		//IL_18df: Unknown result type (might be due to invalid IL or missing references)
		//IL_1918: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e22: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_206e: Unknown result type (might be due to invalid IL or missing references)
		//IL_20a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2145: Unknown result type (might be due to invalid IL or missing references)
		//IL_217e: Unknown result type (might be due to invalid IL or missing references)
		//IL_221c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2255: Unknown result type (might be due to invalid IL or missing references)
		//IL_22f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_232c: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_243a: Unknown result type (might be due to invalid IL or missing references)
		//IL_24bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2540: Unknown result type (might be due to invalid IL or missing references)
		//IL_25b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_262e: Unknown result type (might be due to invalid IL or missing references)
		//IL_26a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_271c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2793: Unknown result type (might be due to invalid IL or missing references)
		//IL_280a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2881: Unknown result type (might be due to invalid IL or missing references)
		//IL_28f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_296f: Unknown result type (might be due to invalid IL or missing references)
		//IL_29e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bc2: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		runServiceButtonClear = new RunServiceButton();
		label16 = new Label();
		label15 = new Label();
		label14 = new Label();
		label13 = new Label();
		label12 = new Label();
		label11 = new Label();
		label10 = new Label();
		label9 = new Label();
		label8 = new Label();
		label7 = new Label();
		label6 = new Label();
		label5 = new Label();
		label4 = new Label();
		label3 = new Label();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		digitalReadoutInstrument10 = new DigitalReadoutInstrument();
		digitalReadoutInstrument11 = new DigitalReadoutInstrument();
		digitalReadoutInstrument12 = new DigitalReadoutInstrument();
		digitalReadoutInstrument13 = new DigitalReadoutInstrument();
		digitalReadoutInstrument14 = new DigitalReadoutInstrument();
		digitalReadoutInstrument15 = new DigitalReadoutInstrument();
		barInstrument1 = new BarInstrument();
		barInstrument2 = new BarInstrument();
		barInstrument3 = new BarInstrument();
		barInstrument4 = new BarInstrument();
		barInstrument5 = new BarInstrument();
		barInstrument6 = new BarInstrument();
		barInstrument7 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		barInstrument9 = new BarInstrument();
		barInstrument10 = new BarInstrument();
		barInstrument11 = new BarInstrument();
		barInstrument12 = new BarInstrument();
		barInstrument13 = new BarInstrument();
		barInstrument14 = new BarInstrument();
		barInstrument15 = new BarInstrument();
		digitalReadoutInstrument31 = new DigitalReadoutInstrument();
		digitalReadoutInstrument33 = new DigitalReadoutInstrument();
		digitalReadoutInstrument32 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage2 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage4 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage6 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage7 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage8 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage9 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage10 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage11 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage12 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage13 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage14 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentMileage15 = new DigitalReadoutInstrument();
		label1 = new Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonClear, 4, 16);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label16, 0, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label15, 0, 14);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label14, 0, 13);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label13, 0, 12);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label12, 0, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label11, 0, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label10, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label9, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label8, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label6, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 2, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 2, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument10, 2, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument11, 2, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument12, 2, 12);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument13, 2, 13);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument14, 2, 14);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument15, 2, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument1, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument2, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument3, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument4, 3, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument5, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument6, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument7, 3, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 3, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument9, 3, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument10, 3, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument11, 3, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument12, 3, 12);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument13, 3, 13);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument14, 3, 14);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument15, 3, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument31, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument33, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument32, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage3, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage4, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage5, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage6, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage7, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage8, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage9, 1, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage10, 1, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage11, 1, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage12, 1, 12);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage13, 1, 13);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage14, 1, 14);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentMileage15, 1, 15);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label1, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(runServiceButtonClear, "runServiceButtonClear");
		((Control)(object)runServiceButtonClear).Name = "runServiceButtonClear";
		runServiceButtonClear.ServiceCall = new ServiceCall("TCM01T", "RT_0471_KupplungUeberdrueckt_Zaehler_zuruecksetzen_Start");
		label16.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label16, "label16");
		((Control)(object)label16).Name = "label16";
		label16.Orientation = (TextOrientation)1;
		label16.ShowBorder = false;
		label16.UseSystemColors = true;
		label15.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label15, "label15");
		((Control)(object)label15).Name = "label15";
		label15.Orientation = (TextOrientation)1;
		label15.ShowBorder = false;
		label15.UseSystemColors = true;
		label14.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label14, "label14");
		((Control)(object)label14).Name = "label14";
		label14.Orientation = (TextOrientation)1;
		label14.ShowBorder = false;
		label14.UseSystemColors = true;
		label13.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label13, "label13");
		((Control)(object)label13).Name = "label13";
		label13.Orientation = (TextOrientation)1;
		label13.ShowBorder = false;
		label13.UseSystemColors = true;
		label12.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label12, "label12");
		((Control)(object)label12).Name = "label12";
		label12.Orientation = (TextOrientation)1;
		label12.ShowBorder = false;
		label12.UseSystemColors = true;
		label11.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)(object)label11).Name = "label11";
		label11.Orientation = (TextOrientation)1;
		label11.ShowBorder = false;
		label11.UseSystemColors = true;
		label10.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)(object)label10).Name = "label10";
		label10.Orientation = (TextOrientation)1;
		label10.ShowBorder = false;
		label10.UseSystemColors = true;
		label9.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)(object)label9).Name = "label9";
		label9.Orientation = (TextOrientation)1;
		label9.ShowBorder = false;
		label9.UseSystemColors = true;
		label8.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)(object)label8).Name = "label8";
		label8.Orientation = (TextOrientation)1;
		label8.ShowBorder = false;
		label8.UseSystemColors = true;
		label7.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)(object)label7).Name = "label7";
		label7.Orientation = (TextOrientation)1;
		label7.ShowBorder = false;
		label7.UseSystemColors = true;
		label6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)(object)label6).Name = "label6";
		label6.Orientation = (TextOrientation)1;
		label6.ShowBorder = false;
		label6.UseSystemColors = true;
		label5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)(object)label5).Name = "label5";
		label5.Orientation = (TextOrientation)1;
		label5.ShowBorder = false;
		label5.UseSystemColors = true;
		label4.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)(object)label4).Name = "label4";
		label4.Orientation = (TextOrientation)1;
		label4.ShowBorder = false;
		label4.UseSystemColors = true;
		label3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)(object)label3).Name = "label3";
		label3.Orientation = (TextOrientation)1;
		label3.ShowBorder = false;
		label3.UseSystemColors = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_duration");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument1).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2612_clutch_overload_monitoring_ring_buffer_event_2_duration");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument2).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2613_clutch_overload_monitoring_ring_buffer_event_3_duration");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument3).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2614_clutch_overload_monitoring_ring_buffer_event_4_duration");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument4).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2615_clutch_overload_monitoring_ring_buffer_event_5_duration");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument5).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2616_clutch_overload_monitoring_ring_buffer_event_6_duration");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument6).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2617_clutch_overload_monitoring_ring_buffer_event_7_duration");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument7).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2618_clutch_overload_monitoring_ring_buffer_event_8_duration");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument8).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2619_clutch_overload_monitoring_ring_buffer_event_9_duration");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((SingleInstrumentBase)digitalReadoutInstrument9).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument9).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument10, "digitalReadoutInstrument10");
		digitalReadoutInstrument10.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument10).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261A_clutch_overload_monitoring_ring_buffer_event_10_duration");
		((Control)(object)digitalReadoutInstrument10).Name = "digitalReadoutInstrument10";
		((SingleInstrumentBase)digitalReadoutInstrument10).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument10).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument11, "digitalReadoutInstrument11");
		digitalReadoutInstrument11.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument11).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261B_clutch_overload_monitoring_ring_buffer_event_11_duration");
		((Control)(object)digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
		((SingleInstrumentBase)digitalReadoutInstrument11).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument11).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument12, "digitalReadoutInstrument12");
		digitalReadoutInstrument12.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument12).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261C_clutch_overload_monitoring_ring_buffer_event_12_duration");
		((Control)(object)digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
		((SingleInstrumentBase)digitalReadoutInstrument12).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument12).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument13, "digitalReadoutInstrument13");
		digitalReadoutInstrument13.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument13).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument13).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261D_clutch_overload_monitoring_ring_buffer_event_13_duration");
		((Control)(object)digitalReadoutInstrument13).Name = "digitalReadoutInstrument13";
		((SingleInstrumentBase)digitalReadoutInstrument13).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument13).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument13).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument14, "digitalReadoutInstrument14");
		digitalReadoutInstrument14.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument14).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument14).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261E_clutch_overload_monitoring_ring_buffer_event_14_duration");
		((Control)(object)digitalReadoutInstrument14).Name = "digitalReadoutInstrument14";
		((SingleInstrumentBase)digitalReadoutInstrument14).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument14).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument14).DataChanged += duration_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument15, "digitalReadoutInstrument15");
		digitalReadoutInstrument15.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrument15).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument15).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261F_clutch_overload_monitoring_ring_buffer_event_15_duration");
		((Control)(object)digitalReadoutInstrument15).Name = "digitalReadoutInstrument15";
		((SingleInstrumentBase)digitalReadoutInstrument15).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrument15).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrument15).DataChanged += duration_DataChanged;
		barInstrument1.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument1, 2);
		componentResourceManager.ApplyResources(barInstrument1, "barInstrument1");
		barInstrument1.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument1).FreezeValue = false;
		((SingleInstrumentBase)barInstrument1).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_peak_temperature");
		((Control)(object)barInstrument1).Name = "barInstrument1";
		((AxisSingleInstrumentBase)barInstrument1).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument1).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument1).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument1).DataChanged += temperature_DataChanged;
		barInstrument2.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument2, 2);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2612_clutch_overload_monitoring_ring_buffer_event_2_peak_temperature");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((AxisSingleInstrumentBase)barInstrument2).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument2).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument2).DataChanged += temperature_DataChanged;
		barInstrument3.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument3, 2);
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2613_clutch_overload_monitoring_ring_buffer_event_3_peak_temperature");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((AxisSingleInstrumentBase)barInstrument3).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument3).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument3).DataChanged += temperature_DataChanged;
		barInstrument4.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument4, 2);
		componentResourceManager.ApplyResources(barInstrument4, "barInstrument4");
		barInstrument4.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument4).FreezeValue = false;
		((SingleInstrumentBase)barInstrument4).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2614_clutch_overload_monitoring_ring_buffer_event_4_peak_temperature");
		((Control)(object)barInstrument4).Name = "barInstrument4";
		((AxisSingleInstrumentBase)barInstrument4).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument4).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument4).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument4).DataChanged += temperature_DataChanged;
		barInstrument5.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument5, 2);
		componentResourceManager.ApplyResources(barInstrument5, "barInstrument5");
		barInstrument5.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument5).FreezeValue = false;
		((SingleInstrumentBase)barInstrument5).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2615_clutch_overload_monitoring_ring_buffer_event_5_peak_temperature");
		((Control)(object)barInstrument5).Name = "barInstrument5";
		((AxisSingleInstrumentBase)barInstrument5).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument5).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument5).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument5).DataChanged += temperature_DataChanged;
		barInstrument6.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument6, 2);
		componentResourceManager.ApplyResources(barInstrument6, "barInstrument6");
		barInstrument6.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument6).FreezeValue = false;
		((SingleInstrumentBase)barInstrument6).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2616_clutch_overload_monitoring_ring_buffer_event_6_peak_temperature");
		((Control)(object)barInstrument6).Name = "barInstrument6";
		((AxisSingleInstrumentBase)barInstrument6).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument6).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument6).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument6).DataChanged += temperature_DataChanged;
		barInstrument7.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument7, 2);
		componentResourceManager.ApplyResources(barInstrument7, "barInstrument7");
		barInstrument7.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument7).FreezeValue = false;
		((SingleInstrumentBase)barInstrument7).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2617_clutch_overload_monitoring_ring_buffer_event_7_peak_temperature");
		((Control)(object)barInstrument7).Name = "barInstrument7";
		((AxisSingleInstrumentBase)barInstrument7).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument7).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument7).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument7).DataChanged += temperature_DataChanged;
		barInstrument8.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument8, 2);
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2618_clutch_overload_monitoring_ring_buffer_event_8_peak_temperature");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((AxisSingleInstrumentBase)barInstrument8).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument8).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument8).DataChanged += temperature_DataChanged;
		barInstrument9.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument9, 2);
		componentResourceManager.ApplyResources(barInstrument9, "barInstrument9");
		barInstrument9.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument9).FreezeValue = false;
		((SingleInstrumentBase)barInstrument9).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2619_clutch_overload_monitoring_ring_buffer_event_9_peak_temperature");
		((Control)(object)barInstrument9).Name = "barInstrument9";
		((AxisSingleInstrumentBase)barInstrument9).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument9).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument9).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument9).DataChanged += temperature_DataChanged;
		barInstrument10.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument10, 2);
		componentResourceManager.ApplyResources(barInstrument10, "barInstrument10");
		barInstrument10.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument10).FreezeValue = false;
		((SingleInstrumentBase)barInstrument10).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261A_clutch_overload_monitoring_ring_buffer_event_10_peak_temperature");
		((Control)(object)barInstrument10).Name = "barInstrument10";
		((AxisSingleInstrumentBase)barInstrument10).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument10).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument10).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument10).DataChanged += temperature_DataChanged;
		barInstrument11.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument11, 2);
		componentResourceManager.ApplyResources(barInstrument11, "barInstrument11");
		barInstrument11.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument11).FreezeValue = false;
		((SingleInstrumentBase)barInstrument11).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261B_clutch_overload_monitoring_ring_buffer_event_11_peak_temperature");
		((Control)(object)barInstrument11).Name = "barInstrument11";
		((AxisSingleInstrumentBase)barInstrument11).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument11).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument11).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument11).DataChanged += temperature_DataChanged;
		barInstrument12.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument12, 2);
		componentResourceManager.ApplyResources(barInstrument12, "barInstrument12");
		barInstrument12.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument12).FreezeValue = false;
		((SingleInstrumentBase)barInstrument12).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261C_clutch_overload_monitoring_ring_buffer_event_12_peak_temperature");
		((Control)(object)barInstrument12).Name = "barInstrument12";
		((AxisSingleInstrumentBase)barInstrument12).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument12).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument12).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument12).DataChanged += temperature_DataChanged;
		barInstrument13.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument13, 2);
		componentResourceManager.ApplyResources(barInstrument13, "barInstrument13");
		barInstrument13.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument13).FreezeValue = false;
		((SingleInstrumentBase)barInstrument13).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261D_clutch_overload_monitoring_ring_buffer_event_13_peak_temperature");
		((Control)(object)barInstrument13).Name = "barInstrument13";
		((AxisSingleInstrumentBase)barInstrument13).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument13).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument13).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument13).DataChanged += temperature_DataChanged;
		barInstrument14.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument14, 2);
		componentResourceManager.ApplyResources(barInstrument14, "barInstrument14");
		barInstrument14.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument14).FreezeValue = false;
		((SingleInstrumentBase)barInstrument14).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261E_clutch_overload_monitoring_ring_buffer_event_14_peak_temperature");
		((Control)(object)barInstrument14).Name = "barInstrument14";
		((AxisSingleInstrumentBase)barInstrument14).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument14).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument14).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument14).DataChanged += temperature_DataChanged;
		barInstrument15.BarStyle = (ControlStyle)1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument15, 2);
		componentResourceManager.ApplyResources(barInstrument15, "barInstrument15");
		barInstrument15.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)barInstrument15).FreezeValue = false;
		((SingleInstrumentBase)barInstrument15).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261F_clutch_overload_monitoring_ring_buffer_event_15_peak_temperature");
		((Control)(object)barInstrument15).Name = "barInstrument15";
		((AxisSingleInstrumentBase)barInstrument15).PreferredAxisRange = new AxisRange(-100.0, 500.0, "°C");
		((SingleInstrumentBase)barInstrument15).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)barInstrument15).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)barInstrument15).DataChanged += temperature_DataChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument31, "digitalReadoutInstrument31");
		digitalReadoutInstrument31.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument31).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument31).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_duration");
		((Control)(object)digitalReadoutInstrument31).Name = "digitalReadoutInstrument31";
		((SingleInstrumentBase)digitalReadoutInstrument31).TitleLengthPercentOfControl = 100;
		((SingleInstrumentBase)digitalReadoutInstrument31).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument31).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument33, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument33, "digitalReadoutInstrument33");
		digitalReadoutInstrument33.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument33).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument33).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_peak_temperature");
		((Control)(object)digitalReadoutInstrument33).Name = "digitalReadoutInstrument33";
		((SingleInstrumentBase)digitalReadoutInstrument33).TitleLengthPercentOfControl = 100;
		((SingleInstrumentBase)digitalReadoutInstrument33).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument33).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument32, "digitalReadoutInstrument32");
		digitalReadoutInstrument32.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument32).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument32).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_milage");
		((Control)(object)digitalReadoutInstrument32).Name = "digitalReadoutInstrument32";
		((SingleInstrumentBase)digitalReadoutInstrument32).TitleLengthPercentOfControl = 100;
		((SingleInstrumentBase)digitalReadoutInstrument32).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument32).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage1, "digitalReadoutInstrumentMileage1");
		digitalReadoutInstrumentMileage1.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage1).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_milage");
		((Control)(object)digitalReadoutInstrumentMileage1).Name = "digitalReadoutInstrumentMileage1";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage1).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage2, "digitalReadoutInstrumentMileage2");
		digitalReadoutInstrumentMileage2.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage2).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2612_clutch_overload_monitoring_ring_buffer_event_2_milage");
		((Control)(object)digitalReadoutInstrumentMileage2).Name = "digitalReadoutInstrumentMileage2";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage2).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage3, "digitalReadoutInstrumentMileage3");
		digitalReadoutInstrumentMileage3.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage3).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2613_clutch_overload_monitoring_ring_buffer_event_3_milage");
		((Control)(object)digitalReadoutInstrumentMileage3).Name = "digitalReadoutInstrumentMileage3";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage3).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage4, "digitalReadoutInstrumentMileage4");
		digitalReadoutInstrumentMileage4.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage4).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2614_clutch_overload_monitoring_ring_buffer_event_4_milage");
		((Control)(object)digitalReadoutInstrumentMileage4).Name = "digitalReadoutInstrumentMileage4";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage4).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage5, "digitalReadoutInstrumentMileage5");
		digitalReadoutInstrumentMileage5.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage5).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2615_clutch_overload_monitoring_ring_buffer_event_5_milage");
		((Control)(object)digitalReadoutInstrumentMileage5).Name = "digitalReadoutInstrumentMileage5";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage5).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage6, "digitalReadoutInstrumentMileage6");
		digitalReadoutInstrumentMileage6.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage6).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2616_clutch_overload_monitoring_ring_buffer_event_6_milage");
		((Control)(object)digitalReadoutInstrumentMileage6).Name = "digitalReadoutInstrumentMileage6";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage6).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage7, "digitalReadoutInstrumentMileage7");
		digitalReadoutInstrumentMileage7.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage7).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2617_clutch_overload_monitoring_ring_buffer_event_7_milage");
		((Control)(object)digitalReadoutInstrumentMileage7).Name = "digitalReadoutInstrumentMileage7";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage7).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage8, "digitalReadoutInstrumentMileage8");
		digitalReadoutInstrumentMileage8.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage8).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2618_clutch_overload_monitoring_ring_buffer_event_8_milage");
		((Control)(object)digitalReadoutInstrumentMileage8).Name = "digitalReadoutInstrumentMileage8";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage8).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage9, "digitalReadoutInstrumentMileage9");
		digitalReadoutInstrumentMileage9.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage9).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_2619_clutch_overload_monitoring_ring_buffer_event_9_milage");
		((Control)(object)digitalReadoutInstrumentMileage9).Name = "digitalReadoutInstrumentMileage9";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage9).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage10, "digitalReadoutInstrumentMileage10");
		digitalReadoutInstrumentMileage10.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage10).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage10).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261A_clutch_overload_monitoring_ring_buffer_event_10_milage");
		((Control)(object)digitalReadoutInstrumentMileage10).Name = "digitalReadoutInstrumentMileage10";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage10).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage10).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage11, "digitalReadoutInstrumentMileage11");
		digitalReadoutInstrumentMileage11.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage11).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage11).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261B_clutch_overload_monitoring_ring_buffer_event_11_milage");
		((Control)(object)digitalReadoutInstrumentMileage11).Name = "digitalReadoutInstrumentMileage11";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage11).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage11).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage12, "digitalReadoutInstrumentMileage12");
		digitalReadoutInstrumentMileage12.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage12).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage12).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261C_clutch_overload_monitoring_ring_buffer_event_12_milage");
		((Control)(object)digitalReadoutInstrumentMileage12).Name = "digitalReadoutInstrumentMileage12";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage12).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage12).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage13, "digitalReadoutInstrumentMileage13");
		digitalReadoutInstrumentMileage13.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage13).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage13).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261D_clutch_overload_monitoring_ring_buffer_event_13_milage");
		((Control)(object)digitalReadoutInstrumentMileage13).Name = "digitalReadoutInstrumentMileage13";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage13).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage13).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage14, "digitalReadoutInstrumentMileage14");
		digitalReadoutInstrumentMileage14.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage14).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage14).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261E_clutch_overload_monitoring_ring_buffer_event_14_milage");
		((Control)(object)digitalReadoutInstrumentMileage14).Name = "digitalReadoutInstrumentMileage14";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage14).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage14).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMileage15, "digitalReadoutInstrumentMileage15");
		digitalReadoutInstrumentMileage15.FontGroup = "TCM_clutch_data";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage15).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage15).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "DT_261F_clutch_overload_monitoring_ring_buffer_event_15_milage");
		((Control)(object)digitalReadoutInstrumentMileage15).Name = "digitalReadoutInstrumentMileage15";
		((SingleInstrumentBase)digitalReadoutInstrumentMileage15).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentMileage15).UnitAlignment = StringAlignment.Near;
		label1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)(object)label1).Name = "label1";
		label1.Orientation = (TextOrientation)1;
		label1.ShowBorder = false;
		label1.UseSystemColors = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
