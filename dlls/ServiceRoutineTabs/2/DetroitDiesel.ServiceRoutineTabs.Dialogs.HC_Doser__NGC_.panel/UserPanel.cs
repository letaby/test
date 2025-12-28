using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HC_Doser__NGC_.panel;

public class UserPanel : CustomPanel
{
	private const string FuelCutOffValveQualifier = "DT_AS077_Fuel_Cut_Off_Valve";

	private Channel Acm = null;

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

	private DigitalReadoutInstrument DigitalReadoutInstrument3;

	private DigitalReadoutInstrument DigitalReadoutInstrument6;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private TextBox textBoxProgress;

	private BarInstrument barInstrument3;

	private BarInstrument barInstrument8;

	private BarInstrument barDoserFuelLineGaugePressure;

	private BarInstrument barFuelCompensationGaugePressure;

	private Button buttonClose;

	private RunSharedProcedureButton hcDoserButton;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
		InitFuelCutOffValveControls();
		((RunSharedProcedureButtonBase)hcDoserButton).ProgressReport += OnProgressReport;
	}

	private void OnProgressReport(object sender, ProgressReportEventArgs e)
	{
		textBoxProgress.AppendText(DateTime.Now.ToShortTimeString() + " " + sender.ToString() + ": " + e.Message + "\r\n");
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		if (e.CloseReason == CloseReason.UserClosing && ((RunSharedProcedureButtonBase)hcDoserButton).InProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			SetAcm(null);
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			((Control)this).Tag = new object[2]
			{
				(int)((RunSharedProcedureButtonBase)hcDoserButton).Result == 1,
				textBoxProgress.Text
			};
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateFuelCutOffValve();
		SetAcm(((CustomPanel)this).GetChannel("ACM21T", (ChannelLookupOptions)7));
	}

	private void SetAcm(Channel acm)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (Acm != acm)
		{
			Acm = acm;
			if (Acm != null)
			{
				((SingleInstrumentBase)BarInstrument6).Instrument = new Qualifier((QualifierTypes)1, Acm.Ecu.Name, "DT_AS006_DPF_Outlet_Pressure");
				((SingleInstrumentBase)BarInstrument7).Instrument = new Qualifier((QualifierTypes)1, Acm.Ecu.Name, "DT_AS005_DOC_Inlet_Pressure");
				((SingleInstrumentBase)BarInstrument9).Instrument = new Qualifier((QualifierTypes)1, Acm.Ecu.Name, "DT_AS007_DOC_Inlet_Temperature");
				((SingleInstrumentBase)BarInstrument10).Instrument = new Qualifier((QualifierTypes)1, Acm.Ecu.Name, "DT_AS008_DOC_Outlet_Temperature");
				((SingleInstrumentBase)BarInstrument11).Instrument = new Qualifier((QualifierTypes)1, Acm.Ecu.Name, "DT_AS009_DPF_Outlet_Temperature");
			}
		}
	}

	private void InitFuelCutOffValveControls()
	{
		SetFuelPressuresVisibility(show: false);
	}

	private void UpdateFuelCutOffValve()
	{
		if (UpdateInstrumentReference("MCM21T", "DT_AS077_Fuel_Cut_Off_Valve", ref fuelCutOffValve, OnFuelCutOffValveDataChanged))
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
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1003: Unknown result type (might be due to invalid IL or missing references)
		//IL_1008: Unknown result type (might be due to invalid IL or missing references)
		//IL_1020: Unknown result type (might be due to invalid IL or missing references)
		//IL_1025: Unknown result type (might be due to invalid IL or missing references)
		//IL_103d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1042: Unknown result type (might be due to invalid IL or missing references)
		//IL_1048: Unknown result type (might be due to invalid IL or missing references)
		//IL_1052: Expected O, but got Unknown
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_1099: Unknown result type (might be due to invalid IL or missing references)
		//IL_109e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1109: Unknown result type (might be due to invalid IL or missing references)
		//IL_110e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1125: Unknown result type (might be due to invalid IL or missing references)
		//IL_112a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1141: Unknown result type (might be due to invalid IL or missing references)
		//IL_1146: Unknown result type (might be due to invalid IL or missing references)
		//IL_115d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1162: Unknown result type (might be due to invalid IL or missing references)
		//IL_117a: Unknown result type (might be due to invalid IL or missing references)
		//IL_117f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1197: Unknown result type (might be due to invalid IL or missing references)
		//IL_119c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c9: Expected O, but got Unknown
		//IL_11f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_120f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1214: Unknown result type (might be due to invalid IL or missing references)
		//IL_121a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1224: Expected O, but got Unknown
		//IL_124e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1253: Unknown result type (might be due to invalid IL or missing references)
		//IL_126a: Unknown result type (might be due to invalid IL or missing references)
		//IL_126f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1286: Unknown result type (might be due to invalid IL or missing references)
		//IL_128b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1291: Unknown result type (might be due to invalid IL or missing references)
		//IL_129b: Expected O, but got Unknown
		//IL_1354: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1421: Unknown result type (might be due to invalid IL or missing references)
		//IL_1488: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		BarInstrument9 = new BarInstrument();
		BarInstrument10 = new BarInstrument();
		BarInstrument11 = new BarInstrument();
		BarInstrument12 = new BarInstrument();
		BarInstrument13 = new BarInstrument();
		tableLayoutPanel1 = new TableLayoutPanel();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument6 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
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
		textBoxProgress = new TextBox();
		barInstrument3 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		barFuelCompensationGaugePressure = new BarInstrument();
		barDoserFuelLineGaugePressure = new BarInstrument();
		buttonClose = new Button();
		hcDoserButton = new RunSharedProcedureButton();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
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
		BarInstrument9.FontGroup = null;
		((SingleInstrumentBase)BarInstrument9).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)BarInstrument9).Name = "BarInstrument9";
		((SingleInstrumentBase)BarInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument9).UnitAlignment = StringAlignment.Near;
		BarInstrument10.BarOrientation = (ControlOrientation)1;
		BarInstrument10.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument10, "BarInstrument10");
		BarInstrument10.FontGroup = null;
		((SingleInstrumentBase)BarInstrument10).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)BarInstrument10).Name = "BarInstrument10";
		((SingleInstrumentBase)BarInstrument10).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument10).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument10).UnitAlignment = StringAlignment.Near;
		BarInstrument11.BarOrientation = (ControlOrientation)1;
		BarInstrument11.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument11, "BarInstrument11");
		BarInstrument11.FontGroup = null;
		((SingleInstrumentBase)BarInstrument11).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)BarInstrument11).Name = "BarInstrument11";
		((SingleInstrumentBase)BarInstrument11).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument11).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument11).UnitAlignment = StringAlignment.Near;
		BarInstrument12.BarOrientation = (ControlOrientation)1;
		BarInstrument12.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument12, "BarInstrument12");
		BarInstrument12.FontGroup = null;
		((SingleInstrumentBase)BarInstrument12).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument12).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature");
		((Control)(object)BarInstrument12).Name = "BarInstrument12";
		((SingleInstrumentBase)BarInstrument12).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument12).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument12).UnitAlignment = StringAlignment.Near;
		BarInstrument13.BarOrientation = (ControlOrientation)1;
		BarInstrument13.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrument13, "BarInstrument13");
		BarInstrument13.FontGroup = null;
		((SingleInstrumentBase)BarInstrument13).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument13).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS017_Inlet_Manifold_Temperature");
		((Control)(object)BarInstrument13).Name = "BarInstrument13";
		((SingleInstrumentBase)BarInstrument13).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrument13).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrument13).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument6, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument3, 1, 1);
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
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxProgress, 2, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument3, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrument8, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barFuelCompensationGaugePressure, 4, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barDoserFuelLineGaugePressure, 4, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 1, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)hcDoserButton, 0, 11);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
		DigitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed");
		((Control)(object)DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
		((SingleInstrumentBase)DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument6, "DigitalReadoutInstrument6");
		DigitalReadoutInstrument6.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS022_Active_Governor_Type");
		((Control)(object)DigitalReadoutInstrument6).Name = "DigitalReadoutInstrument6";
		((SingleInstrumentBase)DigitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
		DigitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State");
		((Control)(object)DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
		((SingleInstrumentBase)DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument1, 2);
		componentResourceManager.ApplyResources(BarInstrument1, "BarInstrument1");
		BarInstrument1.FontGroup = null;
		((SingleInstrumentBase)BarInstrument1).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)BarInstrument1).Name = "BarInstrument1";
		((SingleInstrumentBase)BarInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument5, "DigitalReadoutInstrument5");
		DigitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS019_Barometric_Pressure");
		((Control)(object)DigitalReadoutInstrument5).Name = "DigitalReadoutInstrument5";
		((SingleInstrumentBase)DigitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument2, 2);
		componentResourceManager.ApplyResources(BarInstrument2, "BarInstrument2");
		BarInstrument2.FontGroup = null;
		((SingleInstrumentBase)BarInstrument2).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS018_Inlet_Manifold_Pressure");
		((Control)(object)BarInstrument2).Name = "BarInstrument2";
		((SingleInstrumentBase)BarInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument7, "DigitalReadoutInstrument7");
		DigitalReadoutInstrument7.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS071_Smoke_Control_Status");
		((Control)(object)DigitalReadoutInstrument7).Name = "DigitalReadoutInstrument7";
		((SingleInstrumentBase)DigitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument4, 2);
		componentResourceManager.ApplyResources(BarInstrument4, "BarInstrument4");
		BarInstrument4.FontGroup = null;
		((SingleInstrumentBase)BarInstrument4).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value");
		((Control)(object)BarInstrument4).Name = "BarInstrument4";
		((SingleInstrumentBase)BarInstrument4).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument5, 2);
		componentResourceManager.ApplyResources(BarInstrument5, "BarInstrument5");
		BarInstrument5.FontGroup = null;
		((SingleInstrumentBase)BarInstrument5).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)BarInstrument5).Name = "BarInstrument5";
		((SingleInstrumentBase)BarInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument7, 2);
		componentResourceManager.ApplyResources(BarInstrument7, "BarInstrument7");
		BarInstrument7.FontGroup = null;
		((SingleInstrumentBase)BarInstrument7).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure");
		((Control)(object)BarInstrument7).Name = "BarInstrument7";
		((SingleInstrumentBase)BarInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument6, 2);
		componentResourceManager.ApplyResources(BarInstrument6, "BarInstrument6");
		BarInstrument6.FontGroup = null;
		((SingleInstrumentBase)BarInstrument6).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure");
		((Control)(object)BarInstrument6).Name = "BarInstrument6";
		((SingleInstrumentBase)BarInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
		DigitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS077_Fuel_Cut_Off_Valve");
		((Control)(object)DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
		((SingleInstrumentBase)DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(BarInstrument16, "BarInstrument16");
		BarInstrument16.FontGroup = null;
		((SingleInstrumentBase)BarInstrument16).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument16).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS035_Fuel_Doser_Injection_Status");
		((Control)(object)BarInstrument16).Name = "BarInstrument16";
		((SingleInstrumentBase)BarInstrument16).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barFuelPressure, "barFuelPressure");
		barFuelPressure.FontGroup = null;
		((SingleInstrumentBase)barFuelPressure).FreezeValue = false;
		((SingleInstrumentBase)barFuelPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure");
		((Control)(object)barFuelPressure).Name = "barFuelPressure";
		((SingleInstrumentBase)barFuelPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barFuelPressureAtDoser, "barFuelPressureAtDoser");
		barFuelPressureAtDoser.FontGroup = null;
		((SingleInstrumentBase)barFuelPressureAtDoser).FreezeValue = false;
		((SingleInstrumentBase)barFuelPressureAtDoser).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS038_Doser_Fuel_Line_Pressure");
		((Control)(object)barFuelPressureAtDoser).Name = "barFuelPressureAtDoser";
		((SingleInstrumentBase)barFuelPressureAtDoser).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ListInstrument1, 3);
		componentResourceManager.ApplyResources(ListInstrument1, "ListInstrument1");
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Switches", (Qualifier[])(object)new Qualifier[12]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_ClutchStatus"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_ClutchStatus"),
			new Qualifier((QualifierTypes)1, "CPC502T", "DT_DS255_Blocktransfer_ClutchStatus"),
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
			new Qualifier((QualifierTypes)1, "CPC502T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
			new Qualifier((QualifierTypes)1, "CPC502T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
			new Qualifier((QualifierTypes)1, "SSAM02T", "DT_MSC_GetSwState_033"),
			new Qualifier((QualifierTypes)1, "SSAM02T", "DT_MSC_Diagnostic_Displayables_DDMSC_AutoManSw_Rq_SAM")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Regeneration", (Qualifier[])(object)new Qualifier[12]
		{
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS064_DPF_Regen_State"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS119_Regeneration_Time"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS120_DPF_Target_Temperature"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS122_DOC_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS065_Actual_DPF_zone"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS064_DPF_Regen_State"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS119_Regeneration_Time"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS120_DPF_Target_Temperature"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS122_DOC_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Compressor", (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS055_Temperature_Compressor_In"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS058_Temperature_Compressor_Out")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Engine Brake", (Qualifier[])(object)new Qualifier[3]
		{
			new Qualifier((QualifierTypes)1, "CPC302T", "DT_AS255_Blocktransfer_EnduranceBrakeLeverPosition"),
			new Qualifier((QualifierTypes)1, "CPC501T", "DT_AS255_Blocktransfer_EnduranceBrakeLeverPosition"),
			new Qualifier((QualifierTypes)1, "CPC502T", "DT_AS255_Blocktransfer_EnduranceBrakeLeverPosition")
		}));
		((Control)(object)ListInstrument1).Name = "ListInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)ListInstrument1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxProgress, 3);
		componentResourceManager.ApplyResources(textBoxProgress, "textBoxProgress");
		textBoxProgress.Name = "textBoxProgress";
		textBoxProgress.ReadOnly = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)textBoxProgress, 2);
		componentResourceManager.ApplyResources(barInstrument3, "barInstrument3");
		barInstrument3.FontGroup = null;
		((SingleInstrumentBase)barInstrument3).FreezeValue = false;
		((SingleInstrumentBase)barInstrument3).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressureMY13");
		((Control)(object)barInstrument3).Name = "barInstrument3";
		((SingleInstrumentBase)barInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = null;
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineload");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barFuelCompensationGaugePressure, "barFuelCompensationGaugePressure");
		barFuelCompensationGaugePressure.FontGroup = null;
		((SingleInstrumentBase)barFuelCompensationGaugePressure).FreezeValue = false;
		((SingleInstrumentBase)barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressureMY13");
		((Control)(object)barFuelCompensationGaugePressure).Name = "barFuelCompensationGaugePressure";
		((SingleInstrumentBase)barFuelCompensationGaugePressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barDoserFuelLineGaugePressure, "barDoserFuelLineGaugePressure");
		barDoserFuelLineGaugePressure.FontGroup = null;
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).FreezeValue = false;
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressureMY13");
		((Control)(object)barDoserFuelLineGaugePressure).Name = "barDoserFuelLineGaugePressure";
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).UnitAlignment = StringAlignment.Near;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(hcDoserButton, "hcDoserButton");
		((Control)(object)hcDoserButton).Name = "hcDoserButton";
		hcDoserButton.Qualifier = "SP_HCDoserPurge_MY13";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
