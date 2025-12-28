using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System.panel;

public class UserPanel : CustomPanel
{
	private const string FuelCutOffValveQualifier = "DT_AS077_Fuel_Cut_Off_Valve";

	private Instrument fuelCutOffValve = null;

	private BarInstrument BarInstrument13;

	private BarInstrument BarInstrument12;

	private BarInstrument BarInstrument11;

	private BarInstrument BarInstrument10;

	private BarInstrument BarInstrument9;

	private ListInstrument ListInstrument1;

	private BarInstrument barFuelPressureAtDoser;

	private BarInstrument barFuelPressure;

	private BarInstrument BarInstrument16;

	private DigitalReadoutInstrument DigitalReadoutInstrument4;

	private BarInstrument BarInstrument6;

	private BarInstrument BarInstrument7;

	private BarInstrument BarInstrument5;

	private BarInstrument BarInstrument4;

	private DigitalReadoutInstrument DigitalReadoutInstrument7;

	private BarInstrument BarInstrument2;

	private DigitalReadoutInstrument DigitalReadoutInstrument5;

	private BarInstrument BarInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument6;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonStart;

	private Button buttonStop;

	private BarInstrument barInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private SharedProcedureSelection sharedProcedureSelection;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private TableLayoutPanel tableLayoutPanel3;

	private System.Windows.Forms.Label statusLabel;

	private Checkmark checkmarkStatus;

	private BarInstrument barFuelCompensationGaugePressure;

	private BarInstrument barDoserFuelLineGaugePressure;

	private TextBox textBoxProgress;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
		InitFuelCutOffValveControls();
	}

	public override void OnChannelsChanged()
	{
		UpdateFuelCutOffValve();
	}

	private void InitFuelCutOffValveControls()
	{
		SetFuelPressuresVisibility(show: false);
	}

	private void UpdateFuelCutOffValve()
	{
		if (UpdateInstrumentReference("MCM", "DT_AS077_Fuel_Cut_Off_Valve", ref fuelCutOffValve, OnFuelCutOffValveDataChanged))
		{
			UpdateFuelCutOffValveAffectedValues();
		}
	}

	private void OnFuelCutOffValveDataChanged(object sender, ResultEventArgs e)
	{
		UpdateFuelCutOffValveAffectedValues();
	}

	private void UpdateFuelCutOffValveAffectedValues()
	{
		bool fuelPressuresVisibility = false;
		if (fuelCutOffValve != null)
		{
			double num = InstrumentToDouble(fuelCutOffValve.InstrumentValues.Current, fuelCutOffValve.Units);
			if (!double.IsNaN(num) && num == 100.0)
			{
				fuelPressuresVisibility = true;
			}
		}
		SetFuelPressuresVisibility(fuelPressuresVisibility);
	}

	private static Channel GetActiveChannel(string ecuName)
	{
		Channel result = null;
		if (!string.IsNullOrEmpty(ecuName) && SapiManager.GlobalInstance != null)
		{
			foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
			{
				if (activeChannel.Ecu.Name == ecuName)
				{
					result = activeChannel;
					break;
				}
			}
		}
		return result;
	}

	private static bool UpdateInstrumentReference(string ecuName, string qualifier, ref Instrument instrumentVariable, InstrumentUpdateEventHandler updateHandler)
	{
		Instrument instrument = null;
		Channel activeChannel = GetActiveChannel(ecuName);
		if (activeChannel != null)
		{
			instrument = activeChannel.Instruments[qualifier];
		}
		bool result = false;
		if (instrument != instrumentVariable)
		{
			if (instrumentVariable != null && updateHandler != null)
			{
				instrumentVariable.InstrumentUpdateEvent -= updateHandler;
			}
			instrumentVariable = instrument;
			if (instrumentVariable != null && updateHandler != null)
			{
				instrumentVariable.InstrumentUpdateEvent += updateHandler;
			}
			result = true;
		}
		return result;
	}

	private static double InstrumentToDouble(InstrumentValue value, string units)
	{
		double result = double.NaN;
		if (value != null)
		{
			result = ObjectToDouble(value.Value, units);
		}
		return result;
	}

	private static double ObjectToDouble(object value, string units)
	{
		double num = double.NaN;
		if (value != null)
		{
			Choice choice = value as Choice;
			if (choice != null)
			{
				num = Convert.ToDouble(choice.RawValue);
			}
			else
			{
				try
				{
					num = Convert.ToDouble(value);
					Conversion conversion = Converter.GlobalInstance.GetConversion(units);
					if (conversion != null)
					{
						num = conversion.Convert(num);
					}
				}
				catch (InvalidCastException)
				{
					num = double.NaN;
				}
				catch (FormatException)
				{
					num = double.NaN;
				}
			}
		}
		return num;
	}

	private void SetFuelPressuresVisibility(bool show)
	{
		((Control)(object)barFuelPressure).Visible = show;
		((Control)(object)barFuelPressureAtDoser).Visible = show;
		((Control)(object)barDoserFuelLineGaugePressure).Visible = show;
		((Control)(object)barFuelCompensationGaugePressure).Visible = show;
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
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
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1051: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1125: Unknown result type (might be due to invalid IL or missing references)
		//IL_115e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1256: Unknown result type (might be due to invalid IL or missing references)
		//IL_125b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1272: Unknown result type (might be due to invalid IL or missing references)
		//IL_1277: Unknown result type (might be due to invalid IL or missing references)
		//IL_128e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1293: Unknown result type (might be due to invalid IL or missing references)
		//IL_12aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_12af: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12db: Expected O, but got Unknown
		//IL_1305: Unknown result type (might be due to invalid IL or missing references)
		//IL_130a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1321: Unknown result type (might be due to invalid IL or missing references)
		//IL_1326: Unknown result type (might be due to invalid IL or missing references)
		//IL_133d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1342: Unknown result type (might be due to invalid IL or missing references)
		//IL_1359: Unknown result type (might be due to invalid IL or missing references)
		//IL_135e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1375: Unknown result type (might be due to invalid IL or missing references)
		//IL_137a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1391: Unknown result type (might be due to invalid IL or missing references)
		//IL_1396: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13de: Expected O, but got Unknown
		//IL_1408: Unknown result type (might be due to invalid IL or missing references)
		//IL_140d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1424: Unknown result type (might be due to invalid IL or missing references)
		//IL_1429: Unknown result type (might be due to invalid IL or missing references)
		//IL_1440: Unknown result type (might be due to invalid IL or missing references)
		//IL_1445: Unknown result type (might be due to invalid IL or missing references)
		//IL_144b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1455: Expected O, but got Unknown
		//IL_147f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1484: Unknown result type (might be due to invalid IL or missing references)
		//IL_149b: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cc: Expected O, but got Unknown
		//IL_15bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1650: Unknown result type (might be due to invalid IL or missing references)
		//IL_1689: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_171c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1776: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1805: Expected O, but got Unknown
		//IL_1885: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel3 = new TableLayoutPanel();
		textBoxProgress = new TextBox();
		statusLabel = new System.Windows.Forms.Label();
		checkmarkStatus = new Checkmark();
		tableLayoutPanel2 = new TableLayoutPanel();
		BarInstrument9 = new BarInstrument();
		BarInstrument10 = new BarInstrument();
		BarInstrument11 = new BarInstrument();
		BarInstrument12 = new BarInstrument();
		BarInstrument13 = new BarInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument6 = new DigitalReadoutInstrument();
		BarInstrument1 = new BarInstrument();
		DigitalReadoutInstrument5 = new DigitalReadoutInstrument();
		BarInstrument2 = new BarInstrument();
		DigitalReadoutInstrument7 = new DigitalReadoutInstrument();
		BarInstrument4 = new BarInstrument();
		BarInstrument5 = new BarInstrument();
		BarInstrument7 = new BarInstrument();
		BarInstrument6 = new BarInstrument();
		DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
		BarInstrument16 = new BarInstrument();
		barFuelPressure = new BarInstrument();
		barFuelPressureAtDoser = new BarInstrument();
		ListInstrument1 = new ListInstrument();
		buttonStart = new Button();
		buttonStop = new Button();
		barInstrument8 = new BarInstrument();
		barFuelCompensationGaugePressure = new BarInstrument();
		barDoserFuelLineGaugePressure = new BarInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		sharedProcedureSelection = new SharedProcedureSelection();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument6, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument5, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument7, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument4, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument5, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument7, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument6, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument4, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument16, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barFuelPressure, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barFuelPressureAtDoser, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)ListInstrument1, 2, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 0, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStop, 1, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barFuelCompensationGaugePressure, 4, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barDoserFuelLineGaugePressure, 4, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection, 0, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 2, 10);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(textBoxProgress, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(statusLabel, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).SetColumnSpan((Control)textBoxProgress, 3);
		componentResourceManager.ApplyResources(textBoxProgress, "textBoxProgress");
		textBoxProgress.Name = "textBoxProgress";
		textBoxProgress.ReadOnly = true;
		componentResourceManager.ApplyResources(statusLabel, "statusLabel");
		statusLabel.Name = "statusLabel";
		statusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument9, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument10, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument11, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument12, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument13, 4, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel2, 5);
		BarInstrument9.BarOrientation = (ControlOrientation)1;
		BarInstrument9.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument9, "BarInstrument9");
		BarInstrument9.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrument9).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument9).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS040_DOC_Inlet_Temperature");
		((Control)(object)BarInstrument9).Name = "BarInstrument9";
		((AxisSingleInstrumentBase)BarInstrument9).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument9).UnitAlignment = StringAlignment.Near;
		BarInstrument10.BarOrientation = (ControlOrientation)1;
		BarInstrument10.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument10, "BarInstrument10");
		BarInstrument10.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrument10).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument10).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS041_DOC_Outlet_Temperature");
		((Control)(object)BarInstrument10).Name = "BarInstrument10";
		((AxisSingleInstrumentBase)BarInstrument10).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument10).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument10).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument10).UnitAlignment = StringAlignment.Near;
		BarInstrument11.BarOrientation = (ControlOrientation)1;
		BarInstrument11.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument11, "BarInstrument11");
		BarInstrument11.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrument11).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument11).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS053_DPF_Outlet_Temperature");
		((Control)(object)BarInstrument11).Name = "BarInstrument11";
		((AxisSingleInstrumentBase)BarInstrument11).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrument11).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument11).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument11).UnitAlignment = StringAlignment.Near;
		BarInstrument12.BarOrientation = (ControlOrientation)1;
		BarInstrument12.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument12, "BarInstrument12");
		BarInstrument12.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrument12).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument12).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)BarInstrument12).Name = "BarInstrument12";
		((AxisSingleInstrumentBase)BarInstrument12).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((SingleInstrumentBase)BarInstrument12).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument12).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument12).UnitAlignment = StringAlignment.Near;
		BarInstrument13.BarOrientation = (ControlOrientation)1;
		BarInstrument13.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument13, "BarInstrument13");
		BarInstrument13.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrument13).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument13).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletTemp");
		((Control)(object)BarInstrument13).Name = "BarInstrument13";
		((AxisSingleInstrumentBase)BarInstrument13).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((SingleInstrumentBase)BarInstrument13).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument13).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument13).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)DigitalReadoutInstrument6, 2);
		componentResourceManager.ApplyResources(DigitalReadoutInstrument6, "DigitalReadoutInstrument6");
		DigitalReadoutInstrument6.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS022_Active_Governor_Type");
		((Control)(object)DigitalReadoutInstrument6).Name = "DigitalReadoutInstrument6";
		((SingleInstrumentBase)DigitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument1, 2);
		componentResourceManager.ApplyResources(BarInstrument1, "BarInstrument1");
		BarInstrument1.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument1).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)BarInstrument1).Name = "BarInstrument1";
		((AxisSingleInstrumentBase)BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument5, "DigitalReadoutInstrument5");
		DigitalReadoutInstrument5.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS019_Barometric_Pressure");
		((Control)(object)DigitalReadoutInstrument5).Name = "DigitalReadoutInstrument5";
		((SingleInstrumentBase)DigitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument2, 2);
		componentResourceManager.ApplyResources(BarInstrument2, "BarInstrument2");
		BarInstrument2.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument2).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletPressure");
		((Control)(object)BarInstrument2).Name = "BarInstrument2";
		((SingleInstrumentBase)BarInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS071_Smoke_Control_Status");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument4, 2);
		componentResourceManager.ApplyResources(BarInstrument4, "BarInstrument4");
		BarInstrument4.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument4).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS033_Throttle_Valve_Commanded_Value");
		((Control)(object)BarInstrument4).Name = "BarInstrument4";
		((AxisSingleInstrumentBase)BarInstrument4).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument4).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument5, 2);
		componentResourceManager.ApplyResources(BarInstrument5, "BarInstrument5");
		BarInstrument5.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument5).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)BarInstrument5).Name = "BarInstrument5";
		((AxisSingleInstrumentBase)BarInstrument5).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument7, 2);
		componentResourceManager.ApplyResources(BarInstrument7, "BarInstrument7");
		BarInstrument7.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument7).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS036_DPF_Inlet_Pressure");
		((Control)(object)BarInstrument7).Name = "BarInstrument7";
		((AxisSingleInstrumentBase)BarInstrument7).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
		((SingleInstrumentBase)BarInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument6, 2);
		componentResourceManager.ApplyResources(BarInstrument6, "BarInstrument6");
		BarInstrument6.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument6).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument6).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS037_DPF_Outlet_Pressure");
		((Control)(object)BarInstrument6).Name = "BarInstrument6";
		((AxisSingleInstrumentBase)BarInstrument6).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
		((SingleInstrumentBase)BarInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS077_Fuel_Cut_Off_Valve");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(BarInstrument16, "BarInstrument16");
		BarInstrument16.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)BarInstrument16).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument16).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS035_Fuel_Doser_Injection_Status");
		((Control)(object)BarInstrument16).Name = "BarInstrument16";
		((SingleInstrumentBase)BarInstrument16).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barFuelPressure, "barFuelPressure");
		barFuelPressure.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)barFuelPressure).FreezeValue = false;
		((SingleInstrumentBase)barFuelPressure).Instrument = new Qualifier((QualifierTypes)1, "virtual", "fuelPressure");
		((Control)(object)barFuelPressure).Name = "barFuelPressure";
		((AxisSingleInstrumentBase)barFuelPressure).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
		((SingleInstrumentBase)barFuelPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barFuelPressureAtDoser, "barFuelPressureAtDoser");
		barFuelPressureAtDoser.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)barFuelPressureAtDoser).FreezeValue = false;
		((SingleInstrumentBase)barFuelPressureAtDoser).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS038_Doser_Fuel_Line_Pressure");
		((Control)(object)barFuelPressureAtDoser).Name = "barFuelPressureAtDoser";
		((AxisSingleInstrumentBase)barFuelPressureAtDoser).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
		((SingleInstrumentBase)barFuelPressureAtDoser).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ListInstrument1, 3);
		componentResourceManager.ApplyResources(ListInstrument1, "ListInstrument1");
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Switches", (Qualifier[])(object)new Qualifier[5]
		{
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS001_Clutch_Open"),
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS001_Parking_Brake"),
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS006_Neutral_Switch"),
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS008_DPF_Regen_Switch_Status"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Regeneration", (Qualifier[])(object)new Qualifier[8]
		{
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS072_DPF_Zone"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_DS014_DPF_Regen_Flag"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_DS014_DPF_CAN_manual_regen"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_DS014_DPF_CAN_high_idle_regen"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS073_Regeneration_Time"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS074_DPF_Target_Temperature"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS075_DOC_Out_Model_No_Delay"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS076_DOC_Out_Model_Delay")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Compressor", (Qualifier[])(object)new Qualifier[3]
		{
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS055_Temperature_Compressor_In"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS058_Temperature_Compressor_Out"),
			new Qualifier((QualifierTypes)0, "MCM", "DT_AS056_Pressure_Compressor_Out")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Engine Brake", (Qualifier[])(object)new Qualifier[3]
		{
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS003_Engine_Brake_Disable"),
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS003_Engine_Brake_Low"),
			new Qualifier((QualifierTypes)0, "CPC2", "DT_DS003_Engine_Brake_Medium")
		}));
		((Control)(object)ListInstrument1).Name = "ListInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)ListInstrument1, 3);
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.ForeColor = SystemColors.ControlText;
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineload");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((AxisSingleInstrumentBase)barInstrument8).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barFuelCompensationGaugePressure, "barFuelCompensationGaugePressure");
		barFuelCompensationGaugePressure.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)barFuelCompensationGaugePressure).FreezeValue = false;
		((SingleInstrumentBase)barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressure");
		((Control)(object)barFuelCompensationGaugePressure).Name = "barFuelCompensationGaugePressure";
		((AxisSingleInstrumentBase)barFuelCompensationGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
		((SingleInstrumentBase)barFuelCompensationGaugePressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barDoserFuelLineGaugePressure, "barDoserFuelLineGaugePressure");
		barDoserFuelLineGaugePressure.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).FreezeValue = false;
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressure");
		((Control)(object)barDoserFuelLineGaugePressure).Name = "barDoserFuelLineGaugePressure";
		((AxisSingleInstrumentBase)barDoserFuelLineGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressure");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)sharedProcedureSelection, 2);
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[3] { "SP_HCDoserPurge_EPA07", "SP_OverTheRoadRegen_EPA07", "SP_ParkedRegen_EPA07" });
		sharedProcedureIntegrationComponent.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = statusLabel;
		sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponent.ResultsTarget = textBoxProgress;
		sharedProcedureIntegrationComponent.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent.StopAllButton = buttonStop;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DPFSystem");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
