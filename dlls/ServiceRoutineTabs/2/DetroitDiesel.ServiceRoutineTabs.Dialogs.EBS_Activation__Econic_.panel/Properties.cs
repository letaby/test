using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EBS_Activation__Econic_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 210;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EBS Activation (Econic).panel";

	public override string Guid => "ada06140-d934-457a-ac23-ecc588f7b068";

	public override string DisplayName => "EBS Valve Activation";

	public override IEnumerable<string> SupportedDevices => new string[1] { "EBS01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Anti-lock Braking System";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[18]
	{
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd34_Pressure_Rear_Axle_Nominal_Value_Pressure_Rear_Axle_Nominal_Value"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd26_Pressure_Front_Axle_Actual_Value_Pressure_Front_Axle_Actual_Value"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd32_Pressure_Front_Axle_Nominal_Value_Pressure_Front_Axle_Nominal_Value"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd30_Brakevalue_BST_Position_Brakevalue_BST_Position"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd03_Wheel_Speed_Rear_Axle_Left_Wheel_Speed_Rear_Axle_Left"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd04_Wheel_Speed_Rear_Axle_Right_Wheel_Speed_Rear_Axle_Right"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd02_Wheel_Speed_Front_Axle_Right_Wheel_Speed_Front_Axle_Right"),
		new Qualifier((QualifierTypes)1, "EBS01T", "DT_msd01_Wheel_Speed_Front_Axle_Left_Wheel_Speed_Front_Axle_Left"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "J1939-0", "DT_84"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Bremsdruck_abbauen_VA_rechts_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Bremsdruck_abbauen_VA_links_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Bremsdruck_aufbauen_VA_rechts_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Bremsdruck_aufbauen_VA_links_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Auslassventil_oeffnen_VA_rechts_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Auslassventil_oeffnen_VA_links_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Bremsdruck_halten_VA_rechts_Start"),
		new Qualifier((QualifierTypes)2, "EBS01T", "RT_Bremsdruck_halten_VA_links_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[8]
	{
		new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_rechts_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" }),
		new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_links_Start", (IEnumerable<string>)new string[1] { "Timing_Parameter=2000" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "EBS01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
