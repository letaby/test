using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 9;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\CTP Factory Reset.panel";

	public override string Guid => "b42457c8-749b-4b84-b818-563af4df5582";

	public override string DisplayName => "CTP Factory Reset";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CTP01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Telematics";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	protected override IEnumerable<string> RequiredDataItemConditionsSource => new string[1] { "EcuInformation.CTP01T.CO_SoftwarePartNumber:(Fault),(4486560001,Ok),(4486560002,Fault),(4486960001,Ok),(4486960002,Fault),(4487460001,Ok),(4487460002,Fault),(14485260001,Ok),(14485260002,Fault),(14485460001,Ok),(14485460002,Fault),(14487260001,Ok),(14487260002,Fault)" };

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CTP01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
