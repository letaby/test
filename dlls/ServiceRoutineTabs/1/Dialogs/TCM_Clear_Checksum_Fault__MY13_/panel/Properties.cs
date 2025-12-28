// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clear_Checksum_Fault__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clear_Checksum_Fault__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 16 /*0x10*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Clear Checksum Fault (MY13).panel";
    }
  }

  public virtual string Guid => "cc98fac4-6fa8-40ca-8b5e-655a48e7d5d2";

  public virtual string DisplayName => "Clear Checksum Fault";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "TCM01T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Transmission";

  public virtual FilterTypes Filters => (FilterTypes) 4;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 2, "TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start")
      };
    }
  }

  public virtual FaultCondition RequiredFaultCondition
  {
    get
    {
      return new FaultCondition((FaultConditionType) 4, (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 0, "TCM01T", "18F3EE")
      });
    }
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 2, "TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "TCM01T", "18F3EE"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "TCM01T", "00F1EE")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[1]
      {
        new ServiceCall("TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start", (IEnumerable<string>) new string[0])
      };
    }
  }
}
