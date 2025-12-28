using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 179;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Injector Codes (MY13).panel";

	public override string Guid => "11a29a85-8936-447f-ade2-d065ffdaf568";

	public override string DisplayName => "Injector Codes";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[14]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_6"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_1"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_6_NOP0"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_5_NOP0"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_4_NOP0"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_3_NOP0"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_2_NOP0"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_1_NOP0"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_5"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_4"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_3"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_2"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[12]
	{
		"DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value", "DJ_Read_E_Trim", "DJ_Write_E_Trim", "RT_SR070_Injector_Change_Start", "RT_SR074_EcuInitPIR_Start", "RT_SR014_SET_EOL_Default_Values_Start", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_1_NOP0", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_2_NOP0", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_3_NOP0", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_4_NOP0",
		"DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_5_NOP0", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_6_NOP0"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
