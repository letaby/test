using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Self_Test__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 16;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ICUC Self Test (NGC).panel";

	public override string Guid => "b3ef4c71-5453-49fc-aea6-7daa40b9fb73";

	public override string DisplayName => "ICUC Self Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ICUC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
		new Qualifier((QualifierTypes)2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[8]
	{
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=1", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=4", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=2", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=2", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=1", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=3", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=3", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" }),
		new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=4", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ICUC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "FN_HardReset" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
