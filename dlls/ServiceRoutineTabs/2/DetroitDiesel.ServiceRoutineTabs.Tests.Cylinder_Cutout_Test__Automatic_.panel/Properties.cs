using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 115;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Cylinder Cutout Test (Automatic).panel";

	public override string Guid => "a7fae696-7489-4f66-8e9e-2482602f4f97";

	public override string DisplayName => "Cylinder Cutout (Automatic)";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[1] { "S60" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS003_Actual_Torque"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS003_Actual_Torque")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[11]
	{
		"RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder", "DT_AS010_Engine_Speed", "DT_AS003_Actual_Torque", "RT_SR004_Engine_Cylinder_Cut_Off_Stop", "DT_AS072_DPF_Zone", "RT_SR015_Idle_Speed_Modification_Start", "RT_SR015_Idle_Speed_Modification_Stop", "DT_AS013_Coolant_Temperature", "DT_AS032_EGR_Actual_Valve_Position", "DT_AS034_Throttle_Valve_Actual_Position",
		"DT_AS117_Percentage_of_current_VGT_position"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
