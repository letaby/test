// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 37;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Radar Alignment.panel";
    }
  }

  public virtual string Guid => "519cdc0d-804d-438f-afef-f60080743cd3";

  public virtual string DisplayName => "Radar Alignment";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "RDF01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Detroit Assurance";

  public virtual FilterTypes Filters => (FilterTypes) 9;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 2, "RDF01T", "RT_Service_Justage_Request_Results_Progress"),
        new Qualifier((QualifierTypes) 2, "RDF01T", "RT_Service_Justage_Start"),
        new Qualifier((QualifierTypes) 2, "RDF01T", "RT_Service_Justage_Stop"),
        new Qualifier((QualifierTypes) 2, "RDF01T", "RT_Service_Justage_Request_Results_Routine_Result_State"),
        new Qualifier((QualifierTypes) 2, "RDF01T", "DJ_SecurityAccess_RepairShop"),
        new Qualifier((QualifierTypes) 2, "RDF01T", "SES_StandStill_P2_CAN_ECU_max_physical")
      };
    }
  }

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[4]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DrivingRadarAlignment"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "RDF01T", "DT_Service_Justage_Progress_service_justage_progress"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake")
      };
    }
  }
}
