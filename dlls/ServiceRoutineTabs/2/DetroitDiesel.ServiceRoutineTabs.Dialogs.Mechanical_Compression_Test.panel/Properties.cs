using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 122;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Mechanical Compression Test.panel";

	public override string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

	public override string DisplayName => "Mechanical Compression Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[18]
	{
		"DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC16-DD5", "DDEC16-DD8", "DDEC16-DD13", "DDEC16-DD15", "DDEC16-DD16", "DDEC20-DD13", "DDEC20-DD15",
		"DDEC20-DD16", "DDEC20-MDEG 4-Cylinder StageV", "DDEC20-MDEG 6-Cylinder StageV", "DDEC20-DD11 StageV", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC16-DD13EURO5", "DDEC16-DD16EURO5"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[13]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS021_Battery_Voltage"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_Compression_Test"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_Compression_Test"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS021_Battery_Voltage"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status", (IEnumerable<string>)new string[0]),
		new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status", (IEnumerable<string>)new string[0]),
		new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
