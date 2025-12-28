using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__MDEG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 8;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Voltages (MDEG).panel";

	public override string Guid => "66ebe157-c680-431e-b3a8-5a4a06f37bdd";

	public override string DisplayName => "Voltages";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[7] { "DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4", "DDEC16-DD5", "DDEC16-DD8", "DDEC20-MDEG 6-Cylinder StageV", "DDEC16-MDEG 4-Cylinder StageV", "DDEC16-MDEG 6-Cylinder StageV" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[10]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value3"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
