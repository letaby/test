using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 149;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Detroit Maintenance System.panel";

	public override string Guid => "1d511cb7-6bf8-44a6-b2fb-e5dda2d0ba2b";

	public override string DisplayName => "Detroit Maintenance System";

	public override IEnumerable<string> SupportedDevices => new string[2] { "CGW05T", "MS01T" };

	public override IEnumerable<string> ProhibitedEquipment => new string[2] { "EMOBILITY-eDrive Powertrain", "EMOBILITY-eDrive Powertrain" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)8, "CGW05T", "DT_STO_MS_Air_filter_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_LF"),
		new Qualifier((QualifierTypes)8, "MS01T", "DT_STO_Air_filter_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_LF")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)2, "MS01T", "DL_Reset_service_information_selected_channel"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus"),
		new Qualifier((QualifierTypes)4, "MS01T", "Minimum_driven_distance_reset_PAR_FsMinRs_FZG"),
		new Qualifier((QualifierTypes)2, "MS01T", "DL_Reset_service_information_selected_channel"),
		new Qualifier((QualifierTypes)2, "MS01T", "DL_Reset_service_information_selected_channel"),
		new Qualifier((QualifierTypes)2, "MS01T", "DL_Reset_service_information_selected_channel")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[4]
	{
		new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=2", "Filter_condition_Diesel_particle_filter_only=0" }),
		new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=1", "Filter_condition_Diesel_particle_filter_only=0" }),
		new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=6", "Filter_condition_Diesel_particle_filter_only=0" }),
		new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>)new string[2] { "Channel_number=20", "Filter_condition_Diesel_particle_filter_only=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "MS01T", "CGW05T", "MSF01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[14]
	{
		"DL_Reset_service_information_selected_channel", "DT_STO_Engine_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_MOT", "DT_STO_Engine_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_MOT", "DT_STO_Transmission_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_GET", "DT_STO_Transmission_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_GET", "DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA1", "DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA1", "DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA2", "DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA2", "DT_STO",
		"DL_", "DT_STO_", "DT_STO_MS_", "DL_MS_"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
