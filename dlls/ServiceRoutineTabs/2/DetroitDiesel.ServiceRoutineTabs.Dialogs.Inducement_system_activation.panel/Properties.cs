using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inducement_system_activation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 34;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Inducement system activation.panel";

	public override string Guid => "5885acbe-b9ff-46ff-9068-2a54b9e8bfd7";

	public override string DisplayName => "Inducement System Activation";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[12]
	{
		"DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC20-MDEG 4-Cylinder StageV", "DDEC20-MDEG 6-Cylinder StageV", "DDEC20-DD11 StageV", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV",
		"DDEC16-MDEG 4-Cylinder StageV", "DDEC16-MDEG 6-Cylinder StageV"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Off-Highway";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start"),
		new Qualifier((QualifierTypes)32, "ACM21T", "96100E"),
		new Qualifier((QualifierTypes)32, "MCM21T", "7E140E")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("ACM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("MCM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
