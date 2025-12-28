// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment_HSV__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment_HSV__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 138;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Radar Alignment HSV (NGC).panel";
    }
  }

  public virtual string Guid => "519cdc0d-804d-438f-afef-f60080743cd3";

  public virtual string DisplayName => "Radar Alignment";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "HSV",
        "RDF02T",
        "XMC02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

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
        new Qualifier((QualifierTypes) 2, "RDF02T", "RT_Service_Alignment_Request_Results_Progress"),
        new Qualifier((QualifierTypes) 2, "RDF02T", "RT_Service_Alignment_Request_Results_Routine_result"),
        new Qualifier((QualifierTypes) 2, "RDF02T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_azimuth"),
        new Qualifier((QualifierTypes) 2, "RDF02T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_elevation"),
        new Qualifier((QualifierTypes) 2, "RDF02T", "RT_Service_Alignment_Start"),
        new Qualifier((QualifierTypes) 2, "RDF02T", "RT_Service_Alignment_Stop")
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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 1, "RDF02T", "DT_Service_Justage_Progress_service_justage_progress"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "RDF02T", "0FFFE9"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "RDF02T", "0FFFF3"),
        new Qualifier((QualifierTypes) 4, "RDF02T", "VertPos"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DrivingRadarAlignment_NGC_HSV"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "RDF02T",
        "XMC02T"
      };
    }
  }
}
