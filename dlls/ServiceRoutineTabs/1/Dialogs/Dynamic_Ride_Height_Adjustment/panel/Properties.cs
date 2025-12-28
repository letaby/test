// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 109;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Dynamic Ride Height Adjustment.panel";
    }
  }

  public virtual string Guid => "ef958949-eeaf-4c69-b402-b1007369fd52";

  public virtual string DisplayName => "Aerodynamic Height Control Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "HSV",
        "XMC02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[23]
      {
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_RHC_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "virtual", "ignitionStatus"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1724"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1722"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1721"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1723")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[16 /*0x10*/]
      {
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=6",
          "DiagRqData_OC_NomLvlRqRAx=0",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=1",
          "DiagRqData_OC_NomLvlRqRAx=1",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=2",
          "DiagRqData_OC_NomLvlRqRAx=2",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=7",
          "DiagRqData_OC_NomLvlRqRAx=7",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=6",
          "DiagRqData_OC_NomLvlRqRAx=6",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=1",
          "DiagRqData_OC_NomLvlRqRAx=1",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=7",
          "DiagRqData_OC_NomLvlRqRAx=7",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=2",
          "DiagRqData_OC_NomLvlRqRAx=2",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=7",
          "DiagRqData_OC_NomLvlRqRAx=7",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=2",
          "DiagRqData_OC_NomLvlRqRAx=2",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=1",
          "DiagRqData_OC_NomLvlRqRAx=1",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=1",
          "Nominal_Level_Request_Front_Axle=6",
          "DiagRqData_OC_NomLvlRqRAx=6",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=1",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=1"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=6",
          "DiagRqData_OC_NomLvlRqRAx=6",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=0"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=1",
          "DiagRqData_OC_NomLvlRqRAx=1",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=0"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=2",
          "DiagRqData_OC_NomLvlRqRAx=2",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=0"
        }),
        new ServiceCall("XMC02T", "IOC_RHC_OutputCtrl_Control", (IEnumerable<string>) new string[6]
        {
          "DiagRqData_OC_NomLvlRqFAx_Enbl=0",
          "Nominal_Level_Request_Front_Axle=7",
          "DiagRqData_OC_NomLvlRqRAx=7",
          "DiagRqData_OC_NomLvlRqRAx_Enbl=0",
          "DiagRqData_OC_LvlCtrlMd_Rq=0",
          "Level_Control_Mode_Request=0"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "HSV",
        "XMC02T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "DT_1739",
        "DT_1756",
        "DT_1740",
        "DT_1755"
      };
    }
  }
}
