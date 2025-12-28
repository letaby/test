// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 4;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Reset Crank Inhibit (NGC).panel";
    }
  }

  public virtual string Guid => "3d383230-0338-42b7-91c3-d20dba8deb7e";

  public virtual string DisplayName => "Crank Inhibit Reset";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "CTP01T",
        "SSAM02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Telematics";

  public virtual FilterTypes Filters => (FilterTypes) 11;

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
      return (IEnumerable<Qualifier>) new Qualifier[4]
      {
        new Qualifier((QualifierTypes) 64 /*0x40*/, "CTP01T", "RT_Reset_Crank_inhibition_Start_status"),
        new Qualifier((QualifierTypes) 2, "CTP01T", "RT_Reset_Crank_inhibition_Start_status"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_FOTA_InProcess"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_CrankIntrlService_Cmd")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[1]
      {
        new ServiceCall("CTP01T", "RT_Reset_Crank_inhibition_Start_status", (IEnumerable<string>) new string[1]
        {
          "InhibitionStatus=0"
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
        "SSAM02T",
        "CTP01T"
      };
    }
  }
}
