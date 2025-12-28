using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 27;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ATD Desaturation.panel";

	public override string Guid => "0fe4854e-6fd3-42c6-959f-edfef0264047";

	public override string DisplayName => "ATD Desaturation";

	public override IEnumerable<string> SupportedDevices => new string[10] { "ACM02T", "ACM21T", "ACM301T", "CPC02T", "CPC04T", "CPC302T", "CPC501T", "CPC502T", "MCM02T", "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "DOCInletTemperature"),
		new Qualifier((QualifierTypes)1, "virtual", "DOCOutletTemperature"),
		new Qualifier((QualifierTypes)1, "virtual", "DPFOutletTemperature"),
		new Qualifier((QualifierTypes)1, "virtual", "SCRInletTemperature"),
		new Qualifier((QualifierTypes)1, "virtual", "SCROutletTemperature"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DisableHcDoserParkedRegen_NGC")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[6] { "CPC02T", "CPC04T", "CPC302T", "CPC501T", "CPC502T", "ACM301T" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[5] { "SP_DisableHcDoserParkedRegen_NGC", "SP_DisableHcDoserParkedRegen_MY13", "SP_DisableHcDoserParkedRegen_CPC5", "SP_DisableHcDoserParkedRegen_45X", "SP_DisableHcDoserParkedRegen_EPA10" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
