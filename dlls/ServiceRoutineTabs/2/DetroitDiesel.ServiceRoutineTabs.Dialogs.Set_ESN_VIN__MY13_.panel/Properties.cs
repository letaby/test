using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 133;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set ESN VIN (MY13).panel";

	public override string Guid => "3b618fde-ca78-4447-a441-117388869096";

	public override string DisplayName => "Set Engine Serial Number/Vehicle Identification Number";

	public override IEnumerable<string> SupportedDevices => new string[14]
	{
		"ACM02T", "ACM21T", "ACM301T", "CPC02T", "CPC04T", "CPC2", "CPC302T", "CPC501T", "CPC502T", "MCM",
		"MCM02T", "MCM21T", "MR201T", "TCM01T"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)8, "MCM21T", "CO_EquipmentType")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[10] { "MCM21T", "ACM21T", "CPC04T", "TCM01T", "MR201T", "CPC302T", "CPC501T", "CPC502T", "J1939-255", "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[6] { "DL_ID_Write_Engine_Serial_Number", "DL_ID_Engine_Serial_Number", "FN_KeyOffOnReset", "FN_HardReset", "DL_ID_Write_VIN_Current", "DL_ID_VIN_Current" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
