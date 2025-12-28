using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__MY20_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 33;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Quantity Test (MY20).panel";

	public override string Guid => "4679ca29-9fcc-4b29-b9f0-57c49671ce62";

	public override string DisplayName => "DEF Quantity Test";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ACM301T", "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS079_ADS_priming_request"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_DS002_Enable_ADS"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS104_ADS_Doser_PWM"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS014_DEF_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS143_ADS_Pump_Speed"),
		new Qualifier((QualifierTypes)64, "ACM301T", "RT_SCR_Dosing_Quantity_Check_Request_Results_status_of_service_function"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DEFQuantityTest_MY20")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM301T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_AS014_DEF_Pressure" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[1] { "SP_DEFQuantityTest_MY20" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
