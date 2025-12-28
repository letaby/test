using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.VoltagesSCRDPF.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 39;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\VoltagesSCRDPF.panel";

	public override string Guid => "66ebe157-c680-431e-b3a8-5a4a06f37bdd";

	public override string DisplayName => "SCR and DPF Voltages";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ACM02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[12]
	{
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS097_DEF_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS099_DEF_Air_Pressure_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS098_DEF_Pressure_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS093_DEF_Tank_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS094_DEF_Tank_Level_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS095_SCR_Inlet_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS096_SCR_Oulet_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM02T", "RT_Sensor_Voltage_DOC_Inlet_Temp"),
		new Qualifier((QualifierTypes)1, "ACM02T", "RT_Sensor_Voltage_DOC_Outlet_Temp"),
		new Qualifier((QualifierTypes)1, "ACM02T", "RT_Sensor_Voltage_DPF_Outlet_Temp"),
		new Qualifier((QualifierTypes)1, "ACM02T", "RT_Sensor_Voltage_DOC_Inlet_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "RT_Sensor_Voltage_DPF_Outlet_Pressure")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "RT_Sensor_Voltage_DPF_Outlet_Pressure", "RT_Sensor_Voltage_DOC_Inlet_Pressure", "RT_Sensor_Voltage_DOC_Inlet_Temp", "RT_Sensor_Voltage_DOC_Outlet_Temp", "RT_Sensor_Voltage_DPF_Outlet_Temp" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
