// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.All_Faults.panel.Properties
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.All_Faults.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 38;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Fault Code Tabs\\General\\All Faults.panel";
    }
  }

  public virtual string Guid => "8eb4f40b-7a70-49ec-b19a-09cbe2d9b07f";

  public virtual string DisplayName => "All Faults";

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) (int) ushort.MaxValue;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }
}
