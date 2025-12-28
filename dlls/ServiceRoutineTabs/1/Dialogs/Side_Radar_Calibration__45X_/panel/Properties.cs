// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 43;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Side Radar Calibration (45X).panel";
    }
  }

  public virtual string Guid => "58aaec29-d99f-40d2-9898-3248280abf02";

  public virtual string DisplayName => "Side Radar Calibration";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "SRRFL02T",
        "SRRFR02T",
        "SRRL02T",
        "SRRR02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Detroit Assurance";

  public virtual FilterTypes Filters => (FilterTypes) 9;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

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
      return (IEnumerable<Qualifier>) new Qualifier[33]
      {
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRRCalibration"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRFRCalibration"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRFLCalibration"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRLCalibration"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRRCalibration"),
        new Qualifier((QualifierTypes) 2, "SRRR02T", "RT_DynamicCalibrationSDA_Start"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "SRRR02T", "RT_DynamicCalibrationSDA_Stop"),
        new Qualifier((QualifierTypes) 2, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRLCalibration"),
        new Qualifier((QualifierTypes) 2, "SRRL02T", "RT_DynamicCalibrationSDA_Start"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "SRRL02T", "RT_DynamicCalibrationSDA_Stop"),
        new Qualifier((QualifierTypes) 2, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRFRCalibration"),
        new Qualifier((QualifierTypes) 2, "SRRFR02T", "RT_DynamicCalibrationSDA_Start"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "SRRFR02T", "RT_DynamicCalibrationSDA_Stop"),
        new Qualifier((QualifierTypes) 2, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SRRFLCalibration"),
        new Qualifier((QualifierTypes) 2, "SRRFL02T", "RT_DynamicCalibrationSDA_Start"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "SRRFL02T", "RT_DynamicCalibrationSDA_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[12]
      {
        new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "SRRR02T",
        "SRRL02T",
        "SRRFR02T",
        "SRRFL02T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus",
        "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress",
        "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationOutOfProfileCause"
      };
    }
  }
}
