using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_USB_stick_Flashing.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 23;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\CTP USB stick Flashing.panel";

	public override string Guid => "e904e510-09cf-4849-a44e-c6c455fb4f79";

	public override string DisplayName => "CTP USB Stick Flashing";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CTP01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Telematics";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "CTP01T", "RT_Shutdown_Software_Update_via_USB_Start_RoutineStartStatus")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)8, "CTP01T", "CO_HardwarePartNumber"),
		new Qualifier((QualifierTypes)8, "CTP01T", "CO_SoftwarePartNumber"),
		new Qualifier((QualifierTypes)8, "CTP01T", "DT_STO_ID_FBS_Sw_Version_fbsVersion")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CTP01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "DJ_Reprogramming_Flash_Seed", "RT_Shutdown_Software_Update_via_USB_Start_RoutineStartStatus", "SES_Programming_P2_CAN_ECU_max_physical", "RT_Shutdown_Software_Update_via_USB_Request_Results_ShutdownUpdateStatus", "FN_HardReset" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
