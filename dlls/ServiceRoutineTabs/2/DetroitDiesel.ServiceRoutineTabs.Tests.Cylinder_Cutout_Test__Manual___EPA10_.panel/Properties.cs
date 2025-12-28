using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 34;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Cylinder Cutout Test (Manual) (EPA10).panel";

	public override string Guid => "7e051f68-2dc7-4e63-a96e-89045749e827";

	public override string DisplayName => "Cylinder Cutout (Manual)";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS003_Actual_Torque"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS003_Actual_Torque")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder", "RT_SR004_Engine_Cylinder_Cut_Off_Stop", "RT_SR015_Idle_Speed_Modification_Start", "RT_SR015_Idle_Speed_Modification_Stop" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
