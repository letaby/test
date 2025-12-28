using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string AcmName = "ACM21T";

	private const string newDefPressureQualifier = "DT_AS014_DEF_Pressure";

	private const string oldDefPressureQualifier = "DT_AS110_ADS_DEF_Pressure_2";

	private const string euro5DD13EquipmentType = "DDEC16-DD13EURO5";

	private const string euro5DD16EquipmentType = "DDEC16-DD16EURO5";

	private const string thermometerFontGroup = "SCRSystem_Thermometers";

	private const string SharedProcedures = "SP_ChassisDynoBasicScrConversionCheck_MY13, SP_OutputComponentTest_MY13, SP_ParkedScrEfficiencyTest_MY13";

	private const string Euro5SharedProcedures = "SP_ChassisDynoBasicScrConversionCheck_MY13, SP_OutputComponentTest_MY13";

	private Channel acm;

	private BarInstrument BarInstrument13;

	private BarInstrument BarInstrument12;

	private BarInstrument BarInstrument11;

	private BarInstrument BarInstrument10;

	private BarInstrument BarInstrument9;

	private ListInstrument ListInstrument1;

	private BarInstrument BarInstrument5;

	private BarInstrument BarInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonStart;

	private Button buttonStop;

	private BarInstrument barInstrument8;

	private BarInstrument barInstrument14;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument9;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private DigitalReadoutInstrument digitalReadoutInstrument10;

	private DigitalReadoutInstrument digitalReadoutInstrumentDefPressure;

	private TableLayoutPanel tableStatusIndicators;

	private Checkmark checkmarkStatus;

	private System.Windows.Forms.Label labelStatus;

	private TextBox textBoxProgress;

	private DigitalReadoutInstrument digitalReadoutInstrument11;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument13;

	private DigitalReadoutInstrument digitalReadoutInstrument12;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	public override void OnChannelsChanged()
	{
		UpdateConnectedEquipmentType();
		ConnectToACM();
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
		});
		if (val != EquipmentType.Empty)
		{
			List<string> list = new List<string>();
			list.Add("DDEC16-DD13EURO5");
			list.Add("DDEC16-DD16EURO5");
			bool hasValue = EquipmentType.MatchPriority((IEnumerable<string>)list, SapiManager.GlobalInstance.ConnectedEquipment).HasValue;
			LayoutAdjustments(hasValue);
			sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection(hasValue ? "SP_ChassisDynoBasicScrConversionCheck_MY13, SP_OutputComponentTest_MY13" : "SP_ChassisDynoBasicScrConversionCheck_MY13, SP_OutputComponentTest_MY13, SP_ParkedScrEfficiencyTest_MY13");
		}
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
		{
			UpdateConnectedEquipmentType();
		}
	}

	private void LayoutAdjustments(bool isEuro5)
	{
		DigitalReadoutInstrument val = null;
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[0].Width = (isEuro5 ? 26f : 20f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[1].Width = (isEuro5 ? 26f : 20f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[2].Width = (isEuro5 ? 0f : 10f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[3].Width = (isEuro5 ? 0f : 10f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[4].Width = (isEuro5 ? 0f : 10f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[5].Width = (isEuro5 ? 16f : 10f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[6].Width = (isEuro5 ? 16f : 10f);
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles[7].Width = (isEuro5 ? 16f : 10f);
		foreach (Control control in ((TableLayoutPanel)(object)tableLayoutPanel1).Controls)
		{
			int column = ((TableLayoutPanel)(object)tableLayoutPanel1).GetColumn(control);
			if (column != 2 && column != 3 && column != 4)
			{
				continue;
			}
			val = (DigitalReadoutInstrument)(object)((control is DigitalReadoutInstrument) ? control : null);
			if (val != null)
			{
				if (isEuro5)
				{
					val.FontGroup = string.Empty;
				}
				else
				{
					val.FontGroup = "SCRSystem_Thermometers";
				}
			}
		}
		((Control)(object)tableLayoutPanel1).ResumeLayout();
	}

	private void ConnectToACM()
	{
		Channel channel = ((CustomPanel)this).GetChannel("ACM21T");
		if (channel != acm)
		{
			acm = channel;
			UpdateDefPressureInstrument();
		}
	}

	private void UpdateDefPressureInstrument()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (acm != null)
		{
			Instrument instrument = acm.Instruments["DT_AS110_ADS_DEF_Pressure_2"];
			if (instrument != null)
			{
				((SingleInstrumentBase)digitalReadoutInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
			}
			else
			{
				((SingleInstrumentBase)digitalReadoutInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS014_DEF_Pressure");
			}
			((Control)(object)digitalReadoutInstrumentDefPressure).Refresh();
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
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
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Expected O, but got Unknown
		//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Expected O, but got Unknown
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9e: Expected O, but got Unknown
		//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf9: Expected O, but got Unknown
		//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c84: Expected O, but got Unknown
		//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_103b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1074: Unknown result type (might be due to invalid IL or missing references)
		//IL_1114: Unknown result type (might be due to invalid IL or missing references)
		//IL_114d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1226: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1364: Unknown result type (might be due to invalid IL or missing references)
		//IL_139d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1409: Unknown result type (might be due to invalid IL or missing references)
		//IL_1442: Unknown result type (might be due to invalid IL or missing references)
		//IL_149b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1542: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1612: Unknown result type (might be due to invalid IL or missing references)
		//IL_167c: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1750: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f8: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableStatusIndicators = new TableLayoutPanel();
		textBoxProgress = new TextBox();
		checkmarkStatus = new Checkmark();
		labelStatus = new System.Windows.Forms.Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument13 = new DigitalReadoutInstrument();
		digitalReadoutInstrument12 = new DigitalReadoutInstrument();
		digitalReadoutInstrument11 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		buttonStop = new Button();
		ListInstrument1 = new ListInstrument();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		BarInstrument11 = new BarInstrument();
		BarInstrument10 = new BarInstrument();
		BarInstrument9 = new BarInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDefPressure = new DigitalReadoutInstrument();
		barInstrument14 = new BarInstrument();
		BarInstrument13 = new BarInstrument();
		BarInstrument12 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		BarInstrument1 = new BarInstrument();
		BarInstrument5 = new BarInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		buttonStart = new Button();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument10 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableStatusIndicators).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableStatusIndicators, "tableStatusIndicators");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableStatusIndicators, 6);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add(textBoxProgress, 0, 1);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((TableLayoutPanel)(object)tableStatusIndicators).Controls.Add(labelStatus, 1, 0);
		((Control)(object)tableStatusIndicators).Name = "tableStatusIndicators";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableStatusIndicators, 3);
		((TableLayoutPanel)(object)tableStatusIndicators).SetColumnSpan((Control)textBoxProgress, 6);
		componentResourceManager.ApplyResources(textBoxProgress, "textBoxProgress");
		textBoxProgress.Name = "textBoxProgress";
		textBoxProgress.ReadOnly = true;
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument13, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument12, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument11, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStop, 1, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)ListInstrument1, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 0, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument11, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument10, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument9, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentDefPressure, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument14, 7, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument13, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument12, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument5, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 0, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableStatusIndicators, 2, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument10, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 1, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 1, 7);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument13, "digitalReadoutInstrument13");
		digitalReadoutInstrument13.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument13).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument13).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS019_Barometric_Pressure");
		((Control)(object)digitalReadoutInstrument13).Name = "digitalReadoutInstrument13";
		((SingleInstrumentBase)digitalReadoutInstrument13).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument12, "digitalReadoutInstrument12");
		digitalReadoutInstrument12.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument12).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletPressure");
		((Control)(object)digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
		((SingleInstrumentBase)digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument11, "digitalReadoutInstrument11");
		digitalReadoutInstrument11.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument11).FreezeValue = false;
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument11.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument11.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrument11.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet");
		((Control)(object)digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
		((SingleInstrumentBase)digitalReadoutInstrument11).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.ForeColor = SystemColors.ControlText;
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ListInstrument1, 6);
		componentResourceManager.ApplyResources(ListInstrument1, "ListInstrument1");
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Switches", (Qualifier[])(object)new Qualifier[10]
		{
			new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Clutch_Open"),
			new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Parking_Brake"),
			new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Neutral_Switch"),
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_ClutchOpen"),
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_ClutchStatus"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Heaters", (Qualifier[])(object)new Qualifier[6]
		{
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_1"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_2"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_3"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_4"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS012_Diffuser_Heater"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS011_heater_state_dosing_unit")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("SCR Model Temperatures", (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS128_SCR_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS129_SCR_Heat_Generation")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Zone/Level", (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS024_DEF_Tank_Level"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS065_Actual_DPF_zone")
		}));
		((Control)(object)ListInstrument1).Name = "ListInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)ListInstrument1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)sharedProcedureSelection1, 2);
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[3] { "SP_ChassisDynoBasicScrConversionCheck_MY13", "SP_OutputComponentTest_MY13", "SP_ParkedScrEfficiencyTest_MY13" });
		BarInstrument11.BarOrientation = (ControlOrientation)1;
		BarInstrument11.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument11, "BarInstrument11");
		BarInstrument11.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument11).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Oultlet_Temperature");
		((Control)(object)BarInstrument11).Name = "BarInstrument11";
		((AxisSingleInstrumentBase)BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument11, 4);
		((SingleInstrumentBase)BarInstrument11).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument11).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument11).UnitAlignment = StringAlignment.Near;
		BarInstrument10.BarOrientation = (ControlOrientation)1;
		BarInstrument10.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument10, "BarInstrument10");
		BarInstrument10.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument10).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)BarInstrument10).Name = "BarInstrument10";
		((AxisSingleInstrumentBase)BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument10, 4);
		((SingleInstrumentBase)BarInstrument10).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument10).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument10).UnitAlignment = StringAlignment.Near;
		BarInstrument9.BarOrientation = (ControlOrientation)1;
		BarInstrument9.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument9, "BarInstrument9");
		BarInstrument9.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument9).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)BarInstrument9).Name = "BarInstrument9";
		((AxisSingleInstrumentBase)BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument9, 4);
		((SingleInstrumentBase)BarInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS080_ADS_DEF_Quantity_Request");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDefPressure, "digitalReadoutInstrumentDefPressure");
		digitalReadoutInstrumentDefPressure.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentDefPressure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure");
		((Control)(object)digitalReadoutInstrumentDefPressure).Name = "digitalReadoutInstrumentDefPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentDefPressure).UnitAlignment = StringAlignment.Near;
		barInstrument14.BarOrientation = (ControlOrientation)1;
		barInstrument14.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument14, "barInstrument14");
		barInstrument14.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)barInstrument14).FreezeValue = false;
		((SingleInstrumentBase)barInstrument14).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS022_DEF_tank_Temperature");
		((Control)(object)barInstrument14).Name = "barInstrument14";
		((AxisSingleInstrumentBase)barInstrument14).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)barInstrument14, 4);
		((SingleInstrumentBase)barInstrument14).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument14).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument14).UnitAlignment = StringAlignment.Near;
		BarInstrument13.BarOrientation = (ControlOrientation)1;
		BarInstrument13.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument13, "BarInstrument13");
		BarInstrument13.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument13).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument13).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS019_SCR_Outlet_Temperature");
		((Control)(object)BarInstrument13).Name = "BarInstrument13";
		((AxisSingleInstrumentBase)BarInstrument13).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument13, 4);
		((SingleInstrumentBase)BarInstrument13).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument13).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument13).UnitAlignment = StringAlignment.Near;
		BarInstrument12.BarOrientation = (ControlOrientation)1;
		BarInstrument12.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument12, "BarInstrument12");
		BarInstrument12.FontGroup = "SCRSystem_Thermometers";
		((SingleInstrumentBase)BarInstrument12).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument12).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS018_SCR_Inlet_Temperature");
		((Control)(object)BarInstrument12).Name = "BarInstrument12";
		((AxisSingleInstrumentBase)BarInstrument12).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)BarInstrument12, 4);
		((SingleInstrumentBase)BarInstrument12).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument12).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument12).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrument8, 2);
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineload");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((AxisSingleInstrumentBase)barInstrument8).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument1, 2);
		componentResourceManager.ApplyResources(BarInstrument1, "BarInstrument1");
		BarInstrument1.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)BarInstrument1).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)BarInstrument1).Name = "BarInstrument1";
		((AxisSingleInstrumentBase)BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument5, 2);
		componentResourceManager.ApplyResources(BarInstrument5, "BarInstrument5");
		BarInstrument5.FontGroup = "SCRSystem HorizontalBars";
		((SingleInstrumentBase)BarInstrument5).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)BarInstrument5).Name = "BarInstrument5";
		((AxisSingleInstrumentBase)BarInstrument5).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument10, "digitalReadoutInstrument10");
		digitalReadoutInstrument10.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument10).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS143_ADS_Pump_Speed");
		((Control)(object)digitalReadoutInstrument10).Name = "digitalReadoutInstrument10";
		((SingleInstrumentBase)digitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS101_Nox_conversion_efficiency");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS053_Ambient_Air_Temperature");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS036_SCR_Inlet_NOx_Sensor");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "SCRSystem Readouts";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS035_SCR_Outlet_NOx_Sensor");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponent1.ResultsTarget = textBoxProgress;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent1.StopAllButton = buttonStop;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRSystem");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableStatusIndicators).ResumeLayout(performLayout: false);
		((Control)(object)tableStatusIndicators).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
