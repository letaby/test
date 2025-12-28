using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 11;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\CTP Workshop Mode.panel";

	public override string Guid => "4350da79-38f9-4e90-9d00-3d4757018f05";

	public override string DisplayName => "CTP Workshop Mode";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CTP01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Telematics";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	protected override IEnumerable<string> RequiredDataItemConditionsSource => new string[1] { "Instrument.CTP01T.DT_STO_Workshop_Mode_Workshop_Mode:(Default),(0,Default),(1,Ok)" };

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "CTP01T", "DT_STO_Workshop_Mode_Workshop_Mode"),
		new Qualifier((QualifierTypes)2, "CTP01T", "DL_Workshop_Mode")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("CTP01T", "DL_Workshop_Mode", (IEnumerable<string>)new string[1] { "Workshop_Mode=0" })
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
