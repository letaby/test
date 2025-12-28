// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__45X_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__45X_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 77;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Radar Alignment (45X).panel";
    }
  }

  public virtual string Guid => "519cdc0d-804d-438f-afef-f60080743cd3";

  public virtual string DisplayName => "Radar Alignment";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "RDF03T" };
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
        new Qualifier((QualifierTypes) 2, "RDF03T", "RT_Service_Alignment_Request_Results_Progress"),
        new Qualifier((QualifierTypes) 2, "RDF03T", "RT_Service_Alignment_Request_Results_Routine_result"),
        new Qualifier((QualifierTypes) 2, "RDF03T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_azimuth"),
        new Qualifier((QualifierTypes) 2, "RDF03T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_elevation"),
        new Qualifier((QualifierTypes) 2, "RDF03T", "RT_Service_Alignment_Start"),
        new Qualifier((QualifierTypes) 2, "RDF03T", "RT_Service_Alignment_Stop")
      };
    }
  }

  public virtual IEnumerable<Qualifier> ProhibitedQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[43]
      {
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1733"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1734"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1736"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1737"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1738"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1739"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1740"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_Level_Control_Mode"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1742"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1743"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1744"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1745"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1746"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1754"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1755"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1756"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1822"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1823"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1824"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1825"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1826"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1827"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_5294"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_5296"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_5432"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1721"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1722"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1723"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1724"),
        new Qualifier((QualifierTypes) 8, "HSV", "2838"),
        new Qualifier((QualifierTypes) 8, "HSV", "2841"),
        new Qualifier((QualifierTypes) 8, "HSV", "586"),
        new Qualifier((QualifierTypes) 8, "HSV", "587"),
        new Qualifier((QualifierTypes) 8, "HSV", "588"),
        new Qualifier((QualifierTypes) 8, "HSV", "233"),
        new Qualifier((QualifierTypes) 8, "HSV", "234"),
        new Qualifier((QualifierTypes) 8, "HSV", "237"),
        new Qualifier((QualifierTypes) 8, "HSV", "2901"),
        new Qualifier((QualifierTypes) 8, "HSV", "2902"),
        new Qualifier((QualifierTypes) 8, "HSV", "2903"),
        new Qualifier((QualifierTypes) 8, "HSV", "2904"),
        new Qualifier((QualifierTypes) 8, "HSV", "4304"),
        new Qualifier((QualifierTypes) 2, "HSV", "RT_Enter_Dyno_Mode")
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
      return (IEnumerable<Qualifier>) new Qualifier[5]
      {
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "RDF03T", "RT_Service_Alignment_Request_Results_Progress"),
        new Qualifier((QualifierTypes) 1, "J1939-0", "DT_70"),
        new Qualifier((QualifierTypes) 4, "RDF03T", "VertPos"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DrivingRadarAlignment_45X")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "RDF03T" };
  }
}
