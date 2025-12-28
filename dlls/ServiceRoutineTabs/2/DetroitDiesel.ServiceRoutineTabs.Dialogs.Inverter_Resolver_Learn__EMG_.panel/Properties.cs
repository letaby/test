using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 41;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Inverter Resolver Learn (EMG).panel";

	public override string Guid => "93ff5319-c809-4d26-9861-c91dc263af0f";

	public override string DisplayName => "Inverter Resolver Learn";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ECPC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[24]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS125_Actual_E_Motor_Speed_E_Motor_2_Actual_E_Motor_Speed_E_Motor_2"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT1"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT1"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT2"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT2"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT3"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT3")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[9]
	{
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT1", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=1", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=0" }),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT1", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=1", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=0" }),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT2", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=1", "Resolver_teach_in_Var3=0" }),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT2", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=1", "Resolver_teach_in_Var3=0" }),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT3", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=1" }),
		new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT3", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=1" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
