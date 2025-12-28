// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.PTIC_Brake_Learn.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.PTIC_Brake_Learn.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 17;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\PTIC Brake Learn.panel";
    }
  }

  public virtual string Guid => "9fd74eb4-0535-4e08-8db6-cc214e548f69";

  public virtual string DisplayName => "PTIC Brake Learn";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "PTIC" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 1;

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
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 1, "PTIC", "DT_Brake_Home_Learn_Status"),
        new Qualifier((QualifierTypes) 1, "PTIC", "DT_521"),
        new Qualifier((QualifierTypes) 2, "PTIC", "RT_Begin_Brake_Home_Learn"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "slopeAdjBrakeTotal"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "actualBrakeTotal"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "homeTotal")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[1]
      {
        new ServiceCall("PTIC", "RT_Begin_Brake_Home_Learn", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "PTIC" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[7]
      {
        "DT_Slope_Adj_Brake_Whole_Number",
        "DT_Slope_Adj_Brake_Remainder",
        "DT_Home_Whole_Number",
        "DT_Home_Remainder",
        "DT_Actual_Brake_Whole_Number",
        "DT_Actual_Brake_Remainder",
        "RT_Enable_Troubleshooting_Manual"
      };
    }
  }
}
