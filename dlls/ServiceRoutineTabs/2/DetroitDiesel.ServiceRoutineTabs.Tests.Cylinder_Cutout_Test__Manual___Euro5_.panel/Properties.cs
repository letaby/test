using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___Euro5_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 109;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Cylinder Cutout Test (Manual) (Euro5).panel";

	public override string Guid => "7e051f68-2dc7-4e63-a96e-89045749e827";

	public override string DisplayName => "Cylinder Cutout (Manual)";

	public override IEnumerable<string> SupportedDevices => new string[2] { "CPC04T", "MR201T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "MBE-MBE900", "MBE-MBE4000" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Actual_torque_via_CAN"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_ASL_Actual_Torque"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Parking_Brake"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Vehicle_speed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MR201T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_SR0200_Single_Cylinder_Cutoff_Start", "RT_SR0200_Single_Cylinder_Cutoff_Stop" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
