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

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string FuelCutOffValveQualifier = "DT_AS077_Fuel_Cut_Off_Valve";

	private Channel cpc;

	private Channel mcm;

	private Channel acm;

	private Instrument fuelCutOffValve = null;

	private BarInstrument BarInstrumentEngineIntakeTemperature;

	private BarInstrument BarInstrument12;

	private BarInstrument BarInstrumentDPFOutletTemperature;

	private BarInstrument BarInstrumentDOCOutletTemperature;

	private BarInstrument BarInstrumentDOCInletTemperature;

	private ListInstrument ListInstrument1;

	private BarInstrument barFuelPressureAtDoser;

	private BarInstrument barFuelPressure;

	private BarInstrument BarInstrumentDoserStatus;

	private DigitalReadoutInstrument DigitalReadoutInstrumentFuelCutOffValve;

	private BarInstrument BarInstrumentDPFOutletPressure;

	private BarInstrument BarInstrumentDOCInletPressure;

	private BarInstrument BarInstrumentThrottleValve;

	private BarInstrument BarInstrumentThrottleControl;

	private DigitalReadoutInstrument DigitalReadoutInstrumentSmokeControlStatus;

	private BarInstrument BarInstrument2;

	private DigitalReadoutInstrument DigitalReadoutInstrumentBarametricPressure;

	private BarInstrument BarInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrumentGovernerType;

	private DigitalReadoutInstrument DigitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonStart;

	private Button buttonStop;

	private BarInstrument barInstrument8;

	private BarInstrument barDoserFuelLineGaugePressure;

	private BarInstrument barFuelCompensationGaugePressure;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private SharedProcedureSelection sharedProcedureSelection;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

	private TableLayoutPanel tableLayoutPanel3;

	private System.Windows.Forms.Label statusLabel;

	private Checkmark checkmarkStatus;

	private TextBox textBoxProgress;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	public UserPanel()
	{
		InitializeComponent();
		InitFuelCutOffValveControls();
	}

	public override void OnChannelsChanged()
	{
		UpdateInstruments();
	}

	private void InitFuelCutOffValveControls()
	{
		SetFuelPressuresVisibility(show: false);
	}

	private void UpdateInstruments()
	{
		bool flag = false;
		flag = SetCpc(((CustomPanel)this).GetChannel("CPC302T", (ChannelLookupOptions)7));
		flag |= SetMcm(((CustomPanel)this).GetChannel("MCM21T", (ChannelLookupOptions)7));
		SetAcm(((CustomPanel)this).GetChannel("ACM21T", (ChannelLookupOptions)7));
		if (flag)
		{
			SetSharedProcedureQualifiers();
		}
	}

	private bool SetCpc(Channel cpc)
	{
		bool flag = this.cpc != cpc;
		if (flag)
		{
			this.cpc = cpc;
		}
		return flag;
	}

	private bool SetMcm(Channel mcm)
	{
		bool flag = this.mcm != mcm;
		if (flag)
		{
			this.mcm = mcm;
			if (this.mcm != null)
			{
				if (UpdateInstrumentReference(this.mcm.Ecu.Name, "DT_AS077_Fuel_Cut_Off_Valve", ref fuelCutOffValve, OnFuelCutOffValveDataChanged))
				{
					UpdateFuelCutOffValveAffectedValues();
				}
				SetFakeInstrumentQualifiers(mcm);
				SetInstrumentQualifiers((Control)(object)this, "MCM", mcm);
			}
		}
		return flag;
	}

	private void SetAcm(Channel acm)
	{
		if (this.acm != acm)
		{
			this.acm = acm;
			if (this.acm != null)
			{
				SetInstrumentQualifiers((Control)(object)this, "ACM", acm);
			}
		}
	}

	private void SetSharedProcedureQualifiers()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		string text = "SP_HCDoserPurge_MY13";
		if (mcm != null && string.Equals("MCM30T", mcm.Ecu.Name, StringComparison.OrdinalIgnoreCase))
		{
			text = "SP_HCDoserPurge_MY25";
		}
		if (cpc != null && string.Equals("CPC302T", cpc.Ecu.Name, StringComparison.OrdinalIgnoreCase))
		{
			sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[4] { text, "SP_OverTheRoadRegen_NGC", "SP_ParkedRegen_NGC", "SP_DisableHcDoserParkedRegen_NGC" });
			((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes)1, "J1939-0", "DT_105");
		}
		else if (cpc != null && string.Equals("CPC501T", cpc.Ecu.Name, StringComparison.OrdinalIgnoreCase))
		{
			sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[4] { text, "SP_OverTheRoadRegen_CPC5", "SP_ParkedRegen_CPC5", "SP_DisableHcDoserParkedRegen_CPC5" });
			((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes)1, "J1939-0", "DT_105");
		}
		else if (cpc != null && string.Equals("CPC502T", cpc.Ecu.Name, StringComparison.OrdinalIgnoreCase))
		{
			sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[4] { text, "SP_OverTheRoadRegen_MY25", "SP_ParkedRegen_MY25", "SP_DisableHcDoserParkedRegen_MY25" });
			((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes)1, "J1939-0", "DT_105");
		}
		else
		{
			sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[4] { text, "SP_OverTheRoadRegen_MY13", "SP_ParkedRegen_MY13", "SP_DisableHcDoserParkedRegen_MY13" });
			((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletTemp");
		}
		((Control)(object)BarInstrumentEngineIntakeTemperature).Refresh();
	}

	private void SetFakeInstrumentQualifiers(Channel mcm)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (mcm != null)
		{
			if (string.Equals("MCM30T", mcm.Ecu.Name, StringComparison.OrdinalIgnoreCase))
			{
				((SingleInstrumentBase)barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressureMY25");
				((SingleInstrumentBase)barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressureMY25");
				((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressureMY25");
			}
			else
			{
				((SingleInstrumentBase)barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressureMY13");
				((SingleInstrumentBase)barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressureMY13");
				((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressureMY13");
			}
		}
	}

	private void SetInstrumentQualifiers(Control parent, string channelPrefix, Channel channel)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (channel == null)
		{
			return;
		}
		foreach (Control control in parent.Controls)
		{
			SingleInstrumentBase val = (SingleInstrumentBase)(object)((control is SingleInstrumentBase) ? control : null);
			if (val != null)
			{
				_ = val.Instrument;
				Qualifier instrument = val.Instrument;
				if (((Qualifier)(ref instrument)).Ecu == null)
				{
					continue;
				}
				instrument = val.Instrument;
				if (((Qualifier)(ref instrument)).Ecu.StartsWith(channelPrefix, StringComparison.OrdinalIgnoreCase))
				{
					InstrumentCollection instruments = channel.Instruments;
					instrument = val.Instrument;
					if (instruments[((Qualifier)(ref instrument)).Name] != null)
					{
						instrument = val.Instrument;
						QualifierTypes type = ((Qualifier)(ref instrument)).Type;
						string name = channel.Ecu.Name;
						instrument = val.Instrument;
						val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
					}
				}
			}
			else if (control is TableLayoutPanel || control is Panel || control is FlowLayoutPanel)
			{
				SetInstrumentQualifiers(control, channelPrefix, channel);
			}
		}
		parent.Refresh();
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
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d45: Expected O, but got Unknown
		//IL_0d70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1003: Unknown result type (might be due to invalid IL or missing references)
		//IL_1008: Unknown result type (might be due to invalid IL or missing references)
		//IL_100e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1018: Expected O, but got Unknown
		//IL_1042: Unknown result type (might be due to invalid IL or missing references)
		//IL_1047: Unknown result type (might be due to invalid IL or missing references)
		//IL_105e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		//IL_107a: Unknown result type (might be due to invalid IL or missing references)
		//IL_107f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1096: Unknown result type (might be due to invalid IL or missing references)
		//IL_109b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ab: Expected O, but got Unknown
		//IL_10d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10da: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1106: Expected O, but got Unknown
		//IL_124c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285: Unknown result type (might be due to invalid IL or missing references)
		//IL_1312: Unknown result type (might be due to invalid IL or missing references)
		//IL_134b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1411: Unknown result type (might be due to invalid IL or missing references)
		//IL_149e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1564: Unknown result type (might be due to invalid IL or missing references)
		//IL_159d: Unknown result type (might be due to invalid IL or missing references)
		//IL_169b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_172e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1767: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1854: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18eb: Expected O, but got Unknown
		//IL_1a95: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
		DigitalReadoutInstrumentGovernerType = new DigitalReadoutInstrument();
		BarInstrument1 = new BarInstrument();
		DigitalReadoutInstrumentBarametricPressure = new DigitalReadoutInstrument();
		BarInstrument2 = new BarInstrument();
		DigitalReadoutInstrumentSmokeControlStatus = new DigitalReadoutInstrument();
		BarInstrumentThrottleControl = new BarInstrument();
		BarInstrumentThrottleValve = new BarInstrument();
		BarInstrumentDOCInletPressure = new BarInstrument();
		BarInstrumentDPFOutletPressure = new BarInstrument();
		DigitalReadoutInstrumentFuelCutOffValve = new DigitalReadoutInstrument();
		BarInstrumentDoserStatus = new BarInstrument();
		barFuelPressure = new BarInstrument();
		barFuelPressureAtDoser = new BarInstrument();
		ListInstrument1 = new ListInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		BarInstrumentDOCInletTemperature = new BarInstrument();
		BarInstrumentDOCOutletTemperature = new BarInstrument();
		BarInstrumentDPFOutletTemperature = new BarInstrument();
		BarInstrument12 = new BarInstrument();
		BarInstrumentEngineIntakeTemperature = new BarInstrument();
		buttonStart = new Button();
		buttonStop = new Button();
		barInstrument8 = new BarInstrument();
		barFuelCompensationGaugePressure = new BarInstrument();
		barDoserFuelLineGaugePressure = new BarInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		sharedProcedureSelection = new SharedProcedureSelection();
		tableLayoutPanel3 = new TableLayoutPanel();
		textBoxProgress = new TextBox();
		statusLabel = new System.Windows.Forms.Label();
		checkmarkStatus = new Checkmark();
		sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrumentGovernerType, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrumentBarametricPressure, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrument2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrumentSmokeControlStatus, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrumentThrottleControl, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrumentThrottleValve, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrumentDOCInletPressure, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrumentDPFOutletPressure, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrumentFuelCutOffValve, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)BarInstrumentDoserStatus, 2, 6);
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
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)DigitalReadoutInstrumentGovernerType, 2);
		componentResourceManager.ApplyResources(DigitalReadoutInstrumentGovernerType, "DigitalReadoutInstrumentGovernerType");
		DigitalReadoutInstrumentGovernerType.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrumentGovernerType).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrumentGovernerType).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS022_Active_Governor_Type");
		((Control)(object)DigitalReadoutInstrumentGovernerType).Name = "DigitalReadoutInstrumentGovernerType";
		((SingleInstrumentBase)DigitalReadoutInstrumentGovernerType).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument1, 2);
		componentResourceManager.ApplyResources(BarInstrument1, "BarInstrument1");
		BarInstrument1.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument1).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)BarInstrument1).Name = "BarInstrument1";
		((AxisSingleInstrumentBase)BarInstrument1).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrumentBarametricPressure, "DigitalReadoutInstrumentBarametricPressure");
		DigitalReadoutInstrumentBarametricPressure.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrumentBarametricPressure).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrumentBarametricPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS019_Barometric_Pressure");
		((Control)(object)DigitalReadoutInstrumentBarametricPressure).Name = "DigitalReadoutInstrumentBarametricPressure";
		((SingleInstrumentBase)DigitalReadoutInstrumentBarametricPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrument2, 2);
		componentResourceManager.ApplyResources(BarInstrument2, "BarInstrument2");
		BarInstrument2.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrument2).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletPressure");
		((Control)(object)BarInstrument2).Name = "BarInstrument2";
		((SingleInstrumentBase)BarInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrumentSmokeControlStatus, "DigitalReadoutInstrumentSmokeControlStatus");
		DigitalReadoutInstrumentSmokeControlStatus.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrumentSmokeControlStatus).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrumentSmokeControlStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS071_Smoke_Control_Status");
		((Control)(object)DigitalReadoutInstrumentSmokeControlStatus).Name = "DigitalReadoutInstrumentSmokeControlStatus";
		((SingleInstrumentBase)DigitalReadoutInstrumentSmokeControlStatus).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrumentThrottleControl, 2);
		componentResourceManager.ApplyResources(BarInstrumentThrottleControl, "BarInstrumentThrottleControl");
		BarInstrumentThrottleControl.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrumentThrottleControl).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentThrottleControl).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value");
		((Control)(object)BarInstrumentThrottleControl).Name = "BarInstrumentThrottleControl";
		((AxisSingleInstrumentBase)BarInstrumentThrottleControl).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrumentThrottleControl).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrumentThrottleValve, 2);
		componentResourceManager.ApplyResources(BarInstrumentThrottleValve, "BarInstrumentThrottleValve");
		BarInstrumentThrottleValve.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrumentThrottleValve).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentThrottleValve).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
		((Control)(object)BarInstrumentThrottleValve).Name = "BarInstrumentThrottleValve";
		((AxisSingleInstrumentBase)BarInstrumentThrottleValve).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)BarInstrumentThrottleValve).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrumentDOCInletPressure, 2);
		componentResourceManager.ApplyResources(BarInstrumentDOCInletPressure, "BarInstrumentDOCInletPressure");
		BarInstrumentDOCInletPressure.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrumentDOCInletPressure).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentDOCInletPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure");
		((Control)(object)BarInstrumentDOCInletPressure).Name = "BarInstrumentDOCInletPressure";
		((AxisSingleInstrumentBase)BarInstrumentDOCInletPressure).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
		((SingleInstrumentBase)BarInstrumentDOCInletPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)BarInstrumentDPFOutletPressure, 2);
		componentResourceManager.ApplyResources(BarInstrumentDPFOutletPressure, "BarInstrumentDPFOutletPressure");
		BarInstrumentDPFOutletPressure.FontGroup = "horizontalBarLarge";
		((SingleInstrumentBase)BarInstrumentDPFOutletPressure).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentDPFOutletPressure).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure");
		((Control)(object)BarInstrumentDPFOutletPressure).Name = "BarInstrumentDPFOutletPressure";
		((AxisSingleInstrumentBase)BarInstrumentDPFOutletPressure).PreferredAxisRange = new AxisRange(0.0, 400.0, "");
		((SingleInstrumentBase)BarInstrumentDPFOutletPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(DigitalReadoutInstrumentFuelCutOffValve, "DigitalReadoutInstrumentFuelCutOffValve");
		DigitalReadoutInstrumentFuelCutOffValve.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)DigitalReadoutInstrumentFuelCutOffValve).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrumentFuelCutOffValve).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS077_Fuel_Cut_Off_Valve");
		((Control)(object)DigitalReadoutInstrumentFuelCutOffValve).Name = "DigitalReadoutInstrumentFuelCutOffValve";
		((SingleInstrumentBase)DigitalReadoutInstrumentFuelCutOffValve).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(BarInstrumentDoserStatus, "BarInstrumentDoserStatus");
		BarInstrumentDoserStatus.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)BarInstrumentDoserStatus).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentDoserStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS035_Fuel_Doser_Injection_Status");
		((Control)(object)BarInstrumentDoserStatus).Name = "BarInstrumentDoserStatus";
		((SingleInstrumentBase)BarInstrumentDoserStatus).UnitAlignment = StringAlignment.Near;
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
		((SingleInstrumentBase)barFuelPressureAtDoser).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS038_Doser_Fuel_Line_Pressure");
		((Control)(object)barFuelPressureAtDoser).Name = "barFuelPressureAtDoser";
		((AxisSingleInstrumentBase)barFuelPressureAtDoser).PreferredAxisRange = new AxisRange(0.0, 10000.0, "");
		((SingleInstrumentBase)barFuelPressureAtDoser).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)ListInstrument1, 3);
		componentResourceManager.ApplyResources(ListInstrument1, "ListInstrument1");
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Switches", (Qualifier[])(object)new Qualifier[7]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
			new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
			new Qualifier((QualifierTypes)1, "virtual", "NeutralSwitch"),
			new Qualifier((QualifierTypes)1, "virtual", "ClutchSwitch"),
			new Qualifier((QualifierTypes)1, "SSAM02T", "DT_MSC_GetSwState_033"),
			new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_DS019_Vehicle_Check_Status")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Regeneration", (Qualifier[])(object)new Qualifier[24]
		{
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS122_DOC_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS125_DPF_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS064_DPF_Regen_State"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
			new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS120_DPF_Target_Temperature"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS027_Turbo_Speed_1"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS090_Wastegate_return_position"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS122_DOC_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS122_DOC_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
			new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS125_DPF_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS125_DPF_Out_Model_Delay"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS064_DPF_Regen_State"),
			new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS064_DPF_Regen_State"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS065_Actual_DPF_zone"),
			new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS065_Actual_DPF_zone"),
			new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS120_DPF_Target_Temperature"),
			new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS120_DPF_Target_Temperature"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS032_EGR_Actual_Valve_Position"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS027_Turbo_Speed_1"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS090_Wastegate_return_position")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Compressor", (Qualifier[])(object)new Qualifier[4]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS055_Temperature_Compressor_In"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS058_Temperature_Compressor_Out"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS055_Temperature_Compressor_In"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_ASL005_Temperature_Compressor_Out")
		}));
		((Collection<QualifierGroup>)(object)ListInstrument1.Groups).Add(new QualifierGroup("Engine Brake", (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS069_Jake_Brake_1_PWM13"),
			new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS069_Jake_Brake_1_PWM13")
		}));
		((Control)(object)ListInstrument1).Name = "ListInstrument1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)ListInstrument1, 3);
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrumentDOCInletTemperature, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrumentDOCOutletTemperature, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrumentDPFOutletTemperature, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument12, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrumentEngineIntakeTemperature, 4, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel2, 5);
		BarInstrumentDOCInletTemperature.BarOrientation = (ControlOrientation)1;
		BarInstrumentDOCInletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrumentDOCInletTemperature, "BarInstrumentDOCInletTemperature");
		BarInstrumentDOCInletTemperature.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrumentDOCInletTemperature).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentDOCInletTemperature).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)BarInstrumentDOCInletTemperature).Name = "BarInstrumentDOCInletTemperature";
		((AxisSingleInstrumentBase)BarInstrumentDOCInletTemperature).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrumentDOCInletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrumentDOCInletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrumentDOCInletTemperature).UnitAlignment = StringAlignment.Near;
		BarInstrumentDOCOutletTemperature.BarOrientation = (ControlOrientation)1;
		BarInstrumentDOCOutletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrumentDOCOutletTemperature, "BarInstrumentDOCOutletTemperature");
		BarInstrumentDOCOutletTemperature.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrumentDOCOutletTemperature).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentDOCOutletTemperature).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)BarInstrumentDOCOutletTemperature).Name = "BarInstrumentDOCOutletTemperature";
		((AxisSingleInstrumentBase)BarInstrumentDOCOutletTemperature).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrumentDOCOutletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrumentDOCOutletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrumentDOCOutletTemperature).UnitAlignment = StringAlignment.Near;
		BarInstrumentDPFOutletTemperature.BarOrientation = (ControlOrientation)1;
		BarInstrumentDPFOutletTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrumentDPFOutletTemperature, "BarInstrumentDPFOutletTemperature");
		BarInstrumentDPFOutletTemperature.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrumentDPFOutletTemperature).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentDPFOutletTemperature).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)BarInstrumentDPFOutletTemperature).Name = "BarInstrumentDPFOutletTemperature";
		((AxisSingleInstrumentBase)BarInstrumentDPFOutletTemperature).PreferredAxisRange = new AxisRange(-17.0, 1025.0, "");
		((SingleInstrumentBase)BarInstrumentDPFOutletTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrumentDPFOutletTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrumentDPFOutletTemperature).UnitAlignment = StringAlignment.Near;
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
		BarInstrumentEngineIntakeTemperature.BarOrientation = (ControlOrientation)1;
		BarInstrumentEngineIntakeTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(BarInstrumentEngineIntakeTemperature, "BarInstrumentEngineIntakeTemperature");
		BarInstrumentEngineIntakeTemperature.FontGroup = "thermometer";
		((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).FreezeValue = false;
		((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "airInletTemp");
		((Control)(object)BarInstrumentEngineIntakeTemperature).Name = "BarInstrumentEngineIntakeTemperature";
		((AxisSingleInstrumentBase)BarInstrumentEngineIntakeTemperature).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)BarInstrumentEngineIntakeTemperature).UnitAlignment = StringAlignment.Near;
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
		((SingleInstrumentBase)barFuelCompensationGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressureMY13");
		((Control)(object)barFuelCompensationGaugePressure).Name = "barFuelCompensationGaugePressure";
		((AxisSingleInstrumentBase)barFuelCompensationGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
		((SingleInstrumentBase)barFuelCompensationGaugePressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barDoserFuelLineGaugePressure, "barDoserFuelLineGaugePressure");
		barDoserFuelLineGaugePressure.FontGroup = "horizontalBarSmall";
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).FreezeValue = false;
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressureMY13");
		((Control)(object)barDoserFuelLineGaugePressure).Name = "barDoserFuelLineGaugePressure";
		((AxisSingleInstrumentBase)barDoserFuelLineGaugePressure).PreferredAxisRange = new AxisRange(-500.0, 8900.0, "");
		((SingleInstrumentBase)barDoserFuelLineGaugePressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "digitalReadouts";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressureMY13");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)sharedProcedureSelection, 2);
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[4] { "SP_HCDoserPurge_MY13", "SP_OverTheRoadRegen_MY13", "SP_ParkedRegen_MY13", "SP_DisableHcDoserParkedRegen_MY13" });
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
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
