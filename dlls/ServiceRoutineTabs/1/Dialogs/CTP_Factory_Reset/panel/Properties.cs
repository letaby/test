// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 9;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\CTP Factory Reset.panel";
    }
  }

  public virtual string Guid => "b42457c8-749b-4b84-b818-563af4df5582";

  public virtual string DisplayName => "CTP Factory Reset";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "CTP01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Telematics";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  protected virtual IEnumerable<string> RequiredDataItemConditionsSource
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "EcuInformation.CTP01T.CO_SoftwarePartNumber:(Fault),(4486560001,Ok),(4486560002,Fault),(4486960001,Ok),(4486960002,Fault),(4487460001,Ok),(4487460002,Fault),(14485260001,Ok),(14485260002,Fault),(14485460001,Ok),(14485460002,Fault),(14487260001,Ok),(14487260002,Fault)"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "CTP01T" };
  }
}
