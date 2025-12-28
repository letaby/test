using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clear_Checksum_Fault__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 16;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Clear Checksum Fault (MY13).panel";

	public override string Guid => "cc98fac4-6fa8-40ca-8b5e-655a48e7d5d2";

	public override string DisplayName => "Clear Checksum Fault";

	public override IEnumerable<string> SupportedDevices => new string[1] { "TCM01T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)4, (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)0, "TCM01T", "18F3EE")
	});

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start"),
		new Qualifier((QualifierTypes)32, "TCM01T", "18F3EE"),
		new Qualifier((QualifierTypes)32, "TCM01T", "00F1EE")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
