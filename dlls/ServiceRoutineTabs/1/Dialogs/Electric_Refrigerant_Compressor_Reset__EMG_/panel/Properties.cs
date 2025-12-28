// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 25;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Electric Refrigerant Compressor Reset (EMG).panel";
    }
  }

  public virtual string Guid => "56f70650-3d3a-4684-8d3d-2efdfb2e272d";

  public virtual string DisplayName => "Electric Refrigerant Compressor Reset";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "EMOBILITY-eDrive Powertrain"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "ePowertrain";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

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
      return (IEnumerable<Qualifier>) new Qualifier[10]
      {
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ElectricRefrigerantCompressorReset"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results_ERC_Reset_Ctrl"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ElectricRefrigerantCompressorReset"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Start"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }
}
