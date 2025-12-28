using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Battery_Management__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 21;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Battery Management (EMG).panel";

	public override string Guid => "58aecf4c-17be-4558-85f2-ce1d6a313ffe";

	public override string DisplayName => "Battery Management";

	public override IEnumerable<string> SupportedDevices => new string[1] { "BMS01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[30]
	{
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)2, "BMS901T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS801T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS701T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS601T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS501T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS401T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS301T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS201T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS901T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS801T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS701T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS601T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS501T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS401T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS301T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS201T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS901T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS801T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS701T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS601T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS501T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS401T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS301T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS201T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "BMS01T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS01T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)2, "BMS01T", "DL_High_Voltage_Lock")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[18]
	{
		new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" }),
		new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=1" }),
		new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "High_Voltage_Lock=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[10] { "BMS01T", "BMS201T", "BMS301T", "BMS401T", "BMS501T", "BMS601T", "BMS701T", "BMS801T", "BMS901T", "ECPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DL_High_Voltage_Lock", "DT_STO_High_Voltage_Lock_High_Voltage_Lock" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
