using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string HPLeakActualValueQualifier = "DT_AS115_HP_Leak_Actual_Value";

	private const string HPLeakLearnedCounterQualifier = "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter";

	private const string HPLeakLearnedValueQualifier = "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value";

	private SharedProcedureBase selectedProcedure;

	private Channel mcm;

	private Instrument hpLeakActualValue;

	private EcuInfo hpLeakLearnedCounter;

	private EcuInfo hpLeakLearnedValue;

	private bool adrResult = false;

	private string adrMessage = Resources.Message_Test_Not_Run;

	private bool userCanceled = false;

	private SharedProcedureSelection sharedProcedureSelection1;

	private Button buttonStart;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private Checkmark checkmark1;

	private System.Windows.Forms.Label label1;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakLearnedValue;

	private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakLearnedCounter;

	private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakCounter;

	private DigitalReadoutInstrument digitalReadoutInstrumentHPLeakActualValue;

	private DigitalReadoutInstrument digitalReadoutInstrument11;

	private DigitalReadoutInstrument digitalReadoutInstrument12;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private SeekTimeListView seekTimeListView1;

	private ChartInstrument chartInstrument1;

	private TableLayoutPanel tableLayoutPanel2;

	private TableLayoutPanel tableLayoutPanel1;

	public UserPanel()
	{
		InitializeComponent();
		SubscribeToEvents(sharedProcedureSelection1.SelectedProcedure);
		sharedProcedureSelection1.SelectionChanged += sharedProcedureSelection1_SelectionChanged;
		buttonStart.Click += buttonStart_Click;
	}

	private void sharedProcedureSelection1_SelectionChanged(object sender, EventArgs e)
	{
		SubscribeToEvents(sharedProcedureSelection1.SelectedProcedure);
	}

	private void SubscribeToEvents(SharedProcedureBase procedure)
	{
		if (procedure != selectedProcedure)
		{
			if (selectedProcedure != null)
			{
				selectedProcedure.StopComplete -= SelectedProcedure_StopComplete;
			}
			selectedProcedure = procedure;
			if (selectedProcedure != null)
			{
				selectedProcedure.StopComplete += SelectedProcedure_StopComplete;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM21T", (ChannelLookupOptions)1));
	}

	private void SetMCM(Channel channel)
	{
		if (mcm != channel)
		{
			mcm = channel;
			if (mcm != null)
			{
				hpLeakActualValue = mcm.Instruments["DT_AS115_HP_Leak_Actual_Value"];
				hpLeakLearnedCounter = mcm.EcuInfos["DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter"];
				hpLeakLearnedValue = mcm.EcuInfos["DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"];
			}
		}
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		if ((int)sharedProcedureSelection1.SelectedProcedure.State == 3 || (int)sharedProcedureSelection1.SelectedProcedure.State == 0)
		{
			userCanceled = true;
		}
		else
		{
			userCanceled = false;
		}
	}

	private static object GetInstrumentCurrentValue(Instrument instrument)
	{
		object result = null;
		if (instrument != null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
		{
			result = instrument.InstrumentValues.Current.Value;
		}
		return result;
	}

	private static double InstrumentToDouble(Instrument instrument)
	{
		double result = double.NaN;
		object instrumentCurrentValue = GetInstrumentCurrentValue(instrument);
		if (instrumentCurrentValue != null)
		{
			result = Convert.ToDouble(instrumentCurrentValue, CultureInfo.InvariantCulture);
		}
		return result;
	}

	private static double StoredDataToDouble(EcuInfo ecuInfo)
	{
		double result = double.NaN;
		if (ecuInfo != null && !string.IsNullOrEmpty(ecuInfo.Value))
		{
			result = Convert.ToDouble(ecuInfo.Value, CultureInfo.InvariantCulture);
		}
		return result;
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
	}

	private string EvaluateHPLeakParameters()
	{
		double num = InstrumentToDouble(hpLeakActualValue);
		double num2 = StoredDataToDouble(hpLeakLearnedCounter);
		double num3 = StoredDataToDouble(hpLeakLearnedValue);
		string empty = string.Empty;
		if (num.Equals(double.NaN) || num2.Equals(double.NaN) || num3.Equals(double.NaN))
		{
			return Resources.Message_Error_Reading_Values;
		}
		if (num2 >= 10.0)
		{
			if (num > 2.5)
			{
				return Resources.Message_System_Leaking;
			}
			if (num - num3 > 1.5)
			{
				return Resources.Message_System_Leaking;
			}
			return Resources.Message_System_Not_Leaking;
		}
		if (num < 0.5 || num > 2.25)
		{
			return Resources.Message_System_Leaking;
		}
		return Resources.Message_System_Not_Leaking;
	}

	private void SelectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if ((int)sharedProcedureSelection1.SelectedProcedure.Result == 1)
		{
			if (string.Equals(sharedProcedureSelection1.SelectedProcedure.Name, "FSIC Fuel Filter Pressure Check", StringComparison.Ordinal))
			{
				adrMessage = "Fuel Filter Test complete.";
				adrResult = true;
			}
			else
			{
				adrMessage = EvaluateHPLeakParameters();
				adrResult = adrMessage.Equals(Resources.Message_System_Not_Leaking, StringComparison.Ordinal);
			}
		}
		else
		{
			adrMessage = (userCanceled ? Resources.Message_Selected_Procedure_Canceled : Resources.Message_Selected_Procedure_Aborted);
			adrResult = false;
		}
		LogText(adrMessage);
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			if (buttonStart != null)
			{
				buttonStart.Click -= buttonStart_Click;
			}
			SubscribeToEvents(null);
			SetMCM(null);
			((Control)this).Tag = new object[2] { adrResult, adrMessage };
		}
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
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccc: Expected O, but got Unknown
		//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		chartInstrument1 = new ChartInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHPLeakActualValue = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHPLeakCounter = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHPLeakLearnedCounter = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHPLeakLearnedValue = new DigitalReadoutInstrument();
		digitalReadoutInstrument11 = new DigitalReadoutInstrument();
		digitalReadoutInstrument12 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		label1 = new System.Windows.Forms.Label();
		checkmark1 = new Checkmark();
		buttonStart = new Button();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)chartInstrument1, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentHPLeakActualValue, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentHPLeakCounter, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentHPLeakLearnedCounter, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentHPLeakLearnedValue, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument11, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument12, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCoolantTemperature, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 7);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "FSIC";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView1, 3);
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss";
		componentResourceManager.ApplyResources(chartInstrument1, "chartInstrument1");
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS098_desired_rail_pressure"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS001_KW_NW_validity_signal"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS014_Fuel_Temperature"));
		((Control)(object)chartInstrument1).Name = "chartInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)chartInstrument1, 4);
		chartInstrument1.SelectedTime = null;
		chartInstrument1.ShowButtonPanel = false;
		chartInstrument1.ShowEvents = false;
		chartInstrument1.ShowLabels = false;
		chartInstrument1.ShowLegend = false;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS098_desired_rail_pressure");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHPLeakActualValue, "digitalReadoutInstrumentHPLeakActualValue");
		digitalReadoutInstrumentHPLeakActualValue.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakActualValue).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakActualValue).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS115_HP_Leak_Actual_Value");
		((Control)(object)digitalReadoutInstrumentHPLeakActualValue).Name = "digitalReadoutInstrumentHPLeakActualValue";
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakActualValue).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHPLeakCounter, "digitalReadoutInstrumentHPLeakCounter");
		digitalReadoutInstrumentHPLeakCounter.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakCounter).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakCounter).Instrument = new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Counter");
		((Control)(object)digitalReadoutInstrumentHPLeakCounter).Name = "digitalReadoutInstrumentHPLeakCounter";
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakCounter).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHPLeakLearnedCounter, "digitalReadoutInstrumentHPLeakLearnedCounter");
		digitalReadoutInstrumentHPLeakLearnedCounter.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakLearnedCounter).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakLearnedCounter).Instrument = new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter");
		((Control)(object)digitalReadoutInstrumentHPLeakLearnedCounter).Name = "digitalReadoutInstrumentHPLeakLearnedCounter";
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakLearnedCounter).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHPLeakLearnedValue, "digitalReadoutInstrumentHPLeakLearnedValue");
		digitalReadoutInstrumentHPLeakLearnedValue.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakLearnedValue).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakLearnedValue).Instrument = new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value");
		((Control)(object)digitalReadoutInstrumentHPLeakLearnedValue).Name = "digitalReadoutInstrumentHPLeakLearnedValue";
		((SingleInstrumentBase)digitalReadoutInstrumentHPLeakLearnedValue).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument11, "digitalReadoutInstrument11");
		digitalReadoutInstrument11.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument11).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass");
		((Control)(object)digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
		((SingleInstrumentBase)digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument12, "digitalReadoutInstrument12");
		digitalReadoutInstrument12.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument12).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS001_KW_NW_validity_signal");
		((Control)(object)digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
		((SingleInstrumentBase)digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		digitalReadoutInstrument6.Gradient.Initialize((ValueState)3, 1, "°C");
		digitalReadoutInstrument6.Gradient.Modify(1, 10.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "virtual", "fuelTemp");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
		digitalReadoutInstrumentCoolantTemperature.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
		digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState)3, 1, "°C");
		digitalReadoutInstrumentCoolantTemperature.Gradient.Modify(1, 65.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection1, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStart, 3, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[3] { "SP_FuelSystemIntegrityCheck_Automatic_MY13", "SP_FuelSystemIntegrityCheck_LeakTest_MY13", "SP_FuelSystemIntegrityCheck_Manual_MY13" });
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = label1;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_FuelSystemIntegrityCheck");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
