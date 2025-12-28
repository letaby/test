using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 109;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Dynamic Ride Height Adjustment.panel";

	public override string Guid => "ef958949-eeaf-4c69-b402-b1007369fd52";

	public override string DisplayName => "Aerodynamic Height Control Test";

	public override IEnumerable<string> SupportedDevices => new string[2] { "HSV", "XMC02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[23]
	{
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1724"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1722"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1721"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1723")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[16]
	{
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=0", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=6", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=1", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=6", "DiagRqData_OC_NomLvlRqRAx_Enbl=1", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=1" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=6", "DiagRqData_OC_NomLvlRqRAx=6", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=1", "DiagRqData_OC_NomLvlRqRAx=1", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=2", "DiagRqData_OC_NomLvlRqRAx=2", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" }),
		new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>)new string[6] { "DiagRqData_OC_NomLvlRqFAx_Enbl=0", "Nominal_Level_Request_Front_Axle=7", "DiagRqData_OC_NomLvlRqRAx=7", "DiagRqData_OC_NomLvlRqRAx_Enbl=0", "DiagRqData_OC_LvlCtrlMd_Rq=0", "Level_Control_Mode_Request=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "HSV", "XMC02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "DT_1739", "DT_1756", "DT_1740", "DT_1755" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
