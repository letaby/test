// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset_NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset_NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 73;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance ABA Misuse Reset(NGC).panel";
    }
  }

  public virtual string Guid => "e89f325e-33f6-4d9c-b9b5-53359137ee24";

  public virtual string DisplayName => "ABA Misuse Reset";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "VRDU02T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Detroit Assurance";

  public virtual FilterTypes Filters => (FilterTypes) (int) ushort.MaxValue;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 2, "VRDU02T", "RT_Delete_permanent_errors_Start"),
        new Qualifier((QualifierTypes) 2, "VRDU02T", "RT_Delete_permanent_errors_Request_Results_Delete_Results")
      };
    }
  }

  public virtual FaultCondition RequiredFaultCondition
  {
    get
    {
      return new FaultCondition((FaultConditionType) 3, (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 0, "VRDU02T", "02FBFF")
      });
    }
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 2, "VRDU02T", "SES_Extended_P2_CAN_ECU_max_physical"),
        new Qualifier((QualifierTypes) 2, "VRDU02T", "DJ_SecurityAccess_Routines"),
        new Qualifier((QualifierTypes) 2, "VRDU02T", "RT_Delete_permanent_errors_Start"),
        new Qualifier((QualifierTypes) 2, "VRDU02T", "RT_Delete_permanent_errors_Request_Results_Delete_Results"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "VRDU02T", "RT_Delete_permanent_errors_Request_Results_Delete_Results"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "VRDU02T", "02FBFF")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[4]
      {
        new ServiceCall("VRDU02T", "SES_Extended_P2_CAN_ECU_max_physical", (IEnumerable<string>) new string[0]),
        new ServiceCall("VRDU02T", "DJ_SecurityAccess_Routines", (IEnumerable<string>) new string[0]),
        new ServiceCall("VRDU02T", "RT_Delete_permanent_errors_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("VRDU02T", "RT_Delete_permanent_errors_Request_Results_Delete_Results", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "VRDU02T" };
  }
}
