using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 40;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Tilt Sensor.panel";

	public override string Guid => "53dd9aa7-2829-4b73-b91f-539495c446d9";

	public override string DisplayName => "Tilt Sensor";

	public override IEnumerable<string> SupportedDevices => new string[2] { "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)6;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd18_Signal_Neigungssensor_Signal_Neigungssensor"),
		new Qualifier((QualifierTypes)32, "TCM01T", "26F1EE"),
		new Qualifier((QualifierTypes)32, "TCM01T", "26F1ED"),
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0430_Nullpunktabgleich_Neigungssensor_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("TCM01T", "RT_0430_Nullpunktabgleich_Neigungssensor_Start", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "TCM01T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_0430_Nullpunktabgleich_Neigungssensor_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
