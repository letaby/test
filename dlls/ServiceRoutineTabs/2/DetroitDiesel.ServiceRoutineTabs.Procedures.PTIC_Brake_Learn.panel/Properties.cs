using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.PTIC_Brake_Learn.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 17;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\PTIC Brake Learn.panel";

	public override string Guid => "9fd74eb4-0535-4e08-8db6-cc214e548f69";

	public override string DisplayName => "PTIC Brake Learn";

	public override IEnumerable<string> SupportedDevices => new string[1] { "PTIC" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)1, "PTIC", "DT_Brake_Home_Learn_Status"),
		new Qualifier((QualifierTypes)1, "PTIC", "DT_521"),
		new Qualifier((QualifierTypes)2, "PTIC", "RT_Begin_Brake_Home_Learn"),
		new Qualifier((QualifierTypes)16, "fake", "slopeAdjBrakeTotal"),
		new Qualifier((QualifierTypes)16, "fake", "actualBrakeTotal"),
		new Qualifier((QualifierTypes)16, "fake", "homeTotal")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("PTIC", "RT_Begin_Brake_Home_Learn", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "PTIC" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[7] { "DT_Slope_Adj_Brake_Whole_Number", "DT_Slope_Adj_Brake_Remainder", "DT_Home_Whole_Number", "DT_Home_Remainder", "DT_Actual_Brake_Whole_Number", "DT_Actual_Brake_Remainder", "RT_Enable_Troubleshooting_Manual" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
