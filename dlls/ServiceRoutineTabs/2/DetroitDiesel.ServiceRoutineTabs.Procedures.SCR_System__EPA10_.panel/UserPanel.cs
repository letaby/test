using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private BarInstrument BarInstrument13;

	private BarInstrument BarInstrument12;

	private BarInstrument BarInstrument11;

	private BarInstrument BarInstrument10;

	private BarInstrument BarInstrument9;

	private ListInstrument ListInstrument1;

	private BarInstrument BarInstrument6;

	private BarInstrument BarInstrument7;

	private BarInstrument BarInstrument5;

	private BarInstrument BarInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument3;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanelThermometers;

	private TextBox textBoxProgress;

	private Button buttonStart;

	private Button buttonStop;

	private BarInstrument barInstrument8;

	private BarInstrument barInstrument2;

	private BarInstrument barInstrument3;

	private BarInstrument barInstrument4;

	private BarInstrument barInstrument14;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument9;

	private TableLayoutPanel tableLayoutPanelStartandStopButtons;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private Checkmark checkmarkStatusIndicator;

	private Label labelStatusMessage;

	private TableLayoutPanel tableLayoutPanelOutput;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
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
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Expected O, but got Unknown
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Expected O, but got Unknown
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Expected O, but got Unknown
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Expected O, but got Unknown
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1101: Unknown result type (might be due to invalid IL or missing references)
		//IL_116d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1269: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1460: Unknown result type (might be due to invalid IL or missing references)
		//IL_146a: Expected O, but got Unknown
		//IL_14ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1516: Unknown result type (might be due to invalid IL or missing references)
		//IL_1580: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c9: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		ListInstrument1 = new ListInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		BarInstrument1 = new BarInstrument();
		BarInstrument7 = new BarInstrument();
		BarInstrument6 = new BarInstrument();
		tableLayoutPanelThermometers = new TableLayoutPanel();
		BarInstrument9 = new BarInstrument();
		BarInstrument10 = new BarInstrument();
		BarInstrument11 = new BarInstrument();
		BarInstrument12 = new BarInstrument();
		BarInstrument13 = new BarInstrument();
		barInstrument4 = new BarInstrument();
		barInstrument14 = new BarInstrument();
		BarInstrument5 = new BarInstrument();
		barInstrument2 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		barInstrument3 = new BarInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		tableLayoutPanelStartandStopButtons = new TableLayoutPanel();
		buttonStart = new Button();
		buttonStop = new Button();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
		tableLayoutPanelOutput = new TableLayoutPanel();
		checkmarkStatusIndicator = new Checkmark();
		textBoxProgress = new TextBox();
		labelStatusMessage = new Label();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelThermometers).SuspendLayout();
		((Control)(object)tableLayoutPanelStartandStopButtons).SuspendLayout();
		((Control)(object)tableLayoutPanelOutput).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)ListInstrument1, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument6, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelThermometers, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument5, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument2, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument3, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelStartandStopButtons, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument3, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelOutput, 3, 7);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ListInstrument1, 4);
		componentResourceManager.ApplyResources(ListInstrument1, "ListInstrument1");
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Switches", (Qualifier[])(object)new Qualifier[5]
		{
			new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS001_Clutch_Open"),
			new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS001_Parking_Brake"),
			new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS006_Neutral_Switch"),
			new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Heaters", (Qualifier[])(object)new Qualifier[6]
		{
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_1"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_2"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_3"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_4"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS005_Coolant_Valve"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS008_Diffuser_Heater")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("SCR Model Temperatures", (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS117_SCR_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS118_SCR_Heat_Generation")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Zone/Level", (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS024_DEF_Tank_Level"),
			new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS065_Actual_DPF_zone")
		}));
		((Control)(object)ListInstrument1).Name = "ListInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)ListInstrument1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)DigitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument1, 2);
		componentResourceManager.ApplyResources(BarInstrument1, "BarInstrument1");
		BarInstrument1.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)BarInstrument1).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)BarInstrument1).Name = "BarInstrument1";
		((AxisSingleInstrumentBase)BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument7, 2);
		componentResourceManager.ApplyResources(BarInstrument7, "BarInstrument7");
		BarInstrument7.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)BarInstrument7).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure");
		((Control)(object)BarInstrument7).Name = "BarInstrument7";
		((SingleInstrumentBase)BarInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument6, 2);
		componentResourceManager.ApplyResources(BarInstrument6, "BarInstrument6");
		BarInstrument6.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)BarInstrument6).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
		((Control)(object)BarInstrument6).Name = "BarInstrument6";
		((SingleInstrumentBase)BarInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelThermometers, "tableLayoutPanelThermometers");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelThermometers, 4);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)BarInstrument9, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)BarInstrument10, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)BarInstrument11, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)BarInstrument12, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)BarInstrument13, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)barInstrument4, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)barInstrument14, 6, 0);
		((Control)(object)tableLayoutPanelThermometers).Name = "tableLayoutPanelThermometers";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanelThermometers, 5);
		BarInstrument9.BarOrientation = (ControlOrientation)1;
		BarInstrument9.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument9, "BarInstrument9");
		BarInstrument9.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument9).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)BarInstrument9).Name = "BarInstrument9";
		((AxisSingleInstrumentBase)BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument9).UnitAlignment = StringAlignment.Near;
		BarInstrument10.BarOrientation = (ControlOrientation)1;
		BarInstrument10.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument10, "BarInstrument10");
		BarInstrument10.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument10).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)BarInstrument10).Name = "BarInstrument10";
		((AxisSingleInstrumentBase)BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument10).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument10).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument10).UnitAlignment = StringAlignment.Near;
		BarInstrument11.BarOrientation = (ControlOrientation)1;
		BarInstrument11.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument11, "BarInstrument11");
		BarInstrument11.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument11).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)BarInstrument11).Name = "BarInstrument11";
		((AxisSingleInstrumentBase)BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument11).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument11).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument11).UnitAlignment = StringAlignment.Near;
		BarInstrument12.BarOrientation = (ControlOrientation)1;
		BarInstrument12.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument12, "BarInstrument12");
		BarInstrument12.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument12).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument12).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature");
		((Control)(object)BarInstrument12).Name = "BarInstrument12";
		((AxisSingleInstrumentBase)BarInstrument12).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument12).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument12).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument12).UnitAlignment = StringAlignment.Near;
		BarInstrument13.BarOrientation = (ControlOrientation)1;
		BarInstrument13.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument13, "BarInstrument13");
		BarInstrument13.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument13).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument13).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature");
		((Control)(object)BarInstrument13).Name = "BarInstrument13";
		((AxisSingleInstrumentBase)BarInstrument13).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument13).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument13).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument13).UnitAlignment = StringAlignment.Near;
		barInstrument4.BarOrientation = (ControlOrientation)1;
		barInstrument4.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument4, "barInstrument4");
		barInstrument4.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)barInstrument4).FreezeValue = false;
		((SingleInstrumentBase)barInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS021_DEF_Temperature");
		((Control)(object)barInstrument4).Name = "barInstrument4";
		((AxisSingleInstrumentBase)barInstrument4).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((SingleInstrumentBase)barInstrument4).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument4).UnitAlignment = StringAlignment.Near;
		barInstrument14.BarOrientation = (ControlOrientation)1;
		barInstrument14.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument14, "barInstrument14");
		barInstrument14.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)barInstrument14).FreezeValue = false;
		((SingleInstrumentBase)barInstrument14).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS022_DEF_tank_Temperature");
		((Control)(object)barInstrument14).Name = "barInstrument14";
		((AxisSingleInstrumentBase)barInstrument14).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((SingleInstrumentBase)barInstrument14).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument14).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument14).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument5, 2);
		componentResourceManager.ApplyResources(BarInstrument5, "BarInstrument5");
		BarInstrument5.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)BarInstrument5).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)BarInstrument5).Name = "BarInstrument5";
		((AxisSingleInstrumentBase)BarInstrument5).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument2, 2);
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletPressure");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument8, 2);
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineload");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((AxisSingleInstrumentBase)barInstrument8).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument3, 2);
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((AxisSingleInstrumentBase)barInstrument3).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS036_SCR_Inlet_NOx_Sensor");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelStartandStopButtons, "tableLayoutPanelStartandStopButtons");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelStartandStopButtons, 3);
		((TableLayoutPanel)(object)tableLayoutPanelStartandStopButtons).Controls.Add(buttonStart, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelStartandStopButtons).Controls.Add(buttonStop, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelStartandStopButtons).Controls.Add((Control)(object)sharedProcedureSelection1, 0, 0);
		((Control)(object)tableLayoutPanelStartandStopButtons).Name = "tableLayoutPanelStartandStopButtons";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.ForeColor = SystemColors.ControlText;
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanelStartandStopButtons).SetColumnSpan((Control)(object)sharedProcedureSelection1, 2);
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[3] { "SP_ChassisDynoBasicScrConversionCheck_EPA10", "SP_OutputComponentTest_EPA10", "SP_ParkedScrEfficiencyTest_EPA10" });
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS035_SCR_Outlet_NOx_Sensor");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS101_Nox_conversion_efficiency");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS053_Ambient_Air_Temperature");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
		DigitalReadoutInstrument3.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS019_Barometric_Pressure");
		((Control)(object)DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
		((SingleInstrumentBase)DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelOutput, "tableLayoutPanelOutput");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelOutput, 4);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).Controls.Add((Control)(object)checkmarkStatusIndicator, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).Controls.Add(textBoxProgress, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).Controls.Add(labelStatusMessage, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelOutput).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelOutput).Name = "tableLayoutPanelOutput";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanelOutput, 2);
		componentResourceManager.ApplyResources(checkmarkStatusIndicator, "checkmarkStatusIndicator");
		((Control)(object)checkmarkStatusIndicator).Name = "checkmarkStatusIndicator";
		((TableLayoutPanel)(object)tableLayoutPanelOutput).SetColumnSpan((Control)textBoxProgress, 2);
		componentResourceManager.ApplyResources(textBoxProgress, "textBoxProgress");
		textBoxProgress.Name = "textBoxProgress";
		textBoxProgress.ReadOnly = true;
		componentResourceManager.ApplyResources(labelStatusMessage, "labelStatusMessage");
		labelStatusMessage.Name = "labelStatusMessage";
		labelStatusMessage.UseCompatibleTextRendering = true;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatusMessage;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmarkStatusIndicator;
		sharedProcedureIntegrationComponent1.ResultsTarget = textBoxProgress;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent1.StopAllButton = buttonStop;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRSystem");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanelThermometers).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelStartandStopButtons).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelStartandStopButtons).PerformLayout();
		((Control)(object)tableLayoutPanelOutput).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelOutput).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
