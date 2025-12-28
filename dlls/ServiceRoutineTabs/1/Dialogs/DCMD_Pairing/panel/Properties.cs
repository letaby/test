// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 55;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DCMD Pairing.panel";
    }
  }

  public virtual string Guid => "79254125-0d0b-4b96-ae88-284b25e26ca4";

  public virtual string DisplayName => "DCMD Keyfob Pairing";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "DCMD02T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 9;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 2, "DCMD02T", "DL_ID_Keyfob_IDs"),
        new Qualifier((QualifierTypes) 8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_1"),
        new Qualifier((QualifierTypes) 8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_2"),
        new Qualifier((QualifierTypes) 8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_3"),
        new Qualifier((QualifierTypes) 8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_4"),
        new Qualifier((QualifierTypes) 8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_5"),
        new Qualifier((QualifierTypes) 2, "DCMD02T", "DJ_SecurityAccess_Config_EOL"),
        new Qualifier((QualifierTypes) 2, "DCMD02T", "FN_HardReset_physical")
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
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 1, "DCMD02T", "DT_RKE_Button_3_IN_RKE_Button3"),
        new Qualifier((QualifierTypes) 1, "DCMD02T", "DT_RKE_Button_1_IN_RKE_Button1"),
        new Qualifier((QualifierTypes) 1, "DCMD02T", "DT_RKE_Button_2_IN_RKE_Button2")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "DCMD02T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[8]
      {
        "DT_STO_ID_Keyfob_IDs_KeyID_1",
        "DT_STO_ID_Keyfob_IDs_KeyID_2",
        "DT_STO_ID_Keyfob_IDs_KeyID_3",
        "DT_STO_ID_Keyfob_IDs_KeyID_4",
        "DT_STO_ID_Keyfob_IDs_KeyID_5",
        "DL_ID_Remote_SC",
        "DL_ID_Keyfob_IDs",
        "FN_HardReset_physical"
      };
    }
  }
}
