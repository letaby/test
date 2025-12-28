// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__Euro5_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__Euro5_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 109;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Relative Compression Test (Euro5).panel";
    }
  }

  public virtual string Guid => "cb372ec6-6be5-46a0-a980-0cda9046762e";

  public virtual string DisplayName => "Relative Compression Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MR201T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_Parking_Brake")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MR201T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "RT_SR0207_Automatic_Compression_Detection_Start",
        "RT_SR0207_Automatic_Compression_Detection_Request_Results_State_Byte"
      };
    }
  }
}
