using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 101;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Relative Compression Test (MY13).panel";

	public override string Guid => "cb372ec6-6be5-46a0-a980-0cda9046762e";

	public override string DisplayName => "Relative Compression Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_Start_Function_Name"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "RT_SR006_Automatic_Compression_Test_Start_Status", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0", "RT_SR003_PWM_Routine_by_Function_Start_Function_Name", "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
