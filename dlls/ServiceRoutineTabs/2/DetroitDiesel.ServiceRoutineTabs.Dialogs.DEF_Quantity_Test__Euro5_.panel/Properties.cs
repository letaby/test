using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__Euro5_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 38;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Quantity Test (Euro5).panel";

	public override string Guid => "4679ca29-9fcc-4b29-b9f0-57c49671ce62";

	public override string DisplayName => "DEF Quantity Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MR201T" };

	public override bool AllDevicesRequired => false;

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
		new Qualifier((QualifierTypes)1, "MR201T", "DT_ADS_Priming_Request"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_ADS_Pressure_dosing_enable_UPS"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Actual_DEF_Dosing_Quantity"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_ADS_Status_DEF_pump"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_DEF_Pressure"),
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Urea_Pump_Speed"),
		new Qualifier((QualifierTypes)64, "MR201T", "RT_SR029D_EDU_Diagnosis_Routine_Request_Results_Urea_Quantity_Check"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DEFQuantityTest_MR2")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
