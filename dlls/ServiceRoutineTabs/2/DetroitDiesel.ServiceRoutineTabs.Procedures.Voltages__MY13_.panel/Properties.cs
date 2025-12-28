using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 8;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Voltages (MY13).panel";

	public override string Guid => "66ebe157-c680-431e-b3a8-5a4a06f37bdd";

	public override string DisplayName => "Voltages";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[15]
	{
		"DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-DD13", "DDEC16-DD15", "DDEC16-DD16", "DDEC20-DD11 StageV",
		"DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16"
	};

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[14]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value12"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9"),
		new Qualifier((QualifierTypes)1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[14]
	{
		"RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5",
		"RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value12"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
