using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 75;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DOC Face Plug Cleaning (EPA10).panel";

	public override string Guid => "f6578baa-e2b9-4884-aed0-17639d770e09";

	public override string DisplayName => "DOC Face Plug Cleaning";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ACM02T", "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DDEC10-DD13", "DDEC10-DD15", "DDEC10-DD16" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DOC_Face_Plug_Unclogging_Start"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DOC_Face_Plug_Unclogging_Stop")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[22]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS033_Throttle_Valve_Commanded_Value"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS005_DOC_Inlet_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DSL_DPF_Regen_Switch_Status"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS069_Jake_Brake_1_PWM13"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS094_Actual_Torque_Load"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DocFacePlugUncloggingEPA10"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DocFacePlugUncloggingEPA10"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DOC_Face_Plug_Unclogging_Start"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DOC_Face_Plug_Unclogging_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status", (IEnumerable<string>)new string[0]),
		new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Stop", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
