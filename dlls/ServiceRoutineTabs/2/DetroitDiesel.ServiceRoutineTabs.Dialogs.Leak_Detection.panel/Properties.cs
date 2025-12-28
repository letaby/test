using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Leak_Detection.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 59;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Leak Detection.panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "Leak Detection";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "DD13", "DD15" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[9]
	{
		new Qualifier((QualifierTypes)4, "MCM", "HP_Leak_Counter"),
		new Qualifier((QualifierTypes)4, "MCM", "HP_Leak_Learned_Value"),
		new Qualifier((QualifierTypes)4, "MCM", "HP_Leak_Learned_Counter"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS114_RPG_COMPENSATION"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS115_HP_Leak_Actual_Value"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS067_Coolant_Temperatures_2"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS014_Fuel_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "RT_SR076_Desired_Rail_Pressure_Start_Status", "RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start", "DT_AS067_Coolant_Temperatures_2", "DT_AS014_Fuel_Temperature", "RT_SR014_SET_EOL_Default_Values_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
