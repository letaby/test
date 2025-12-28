// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EBS_Activation__Econic_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EBS_Activation__Econic_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 210;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EBS Activation (Econic).panel";
    }
  }

  public virtual string Guid => "ada06140-d934-457a-ac23-ecc588f7b068";

  public virtual string DisplayName => "EBS Valve Activation";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "EBS01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Anti-lock Braking System";

  public virtual FilterTypes Filters => (FilterTypes) 9;

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
      return (IEnumerable<Qualifier>) new Qualifier[18]
      {
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd34_Pressure_Rear_Axle_Nominal_Value_Pressure_Rear_Axle_Nominal_Value"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd26_Pressure_Front_Axle_Actual_Value_Pressure_Front_Axle_Actual_Value"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd32_Pressure_Front_Axle_Nominal_Value_Pressure_Front_Axle_Nominal_Value"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd30_Brakevalue_BST_Position_Brakevalue_BST_Position"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd03_Wheel_Speed_Rear_Axle_Left_Wheel_Speed_Rear_Axle_Left"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd04_Wheel_Speed_Rear_Axle_Right_Wheel_Speed_Rear_Axle_Right"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd02_Wheel_Speed_Front_Axle_Right_Wheel_Speed_Front_Axle_Right"),
        new Qualifier((QualifierTypes) 1, "EBS01T", "DT_msd01_Wheel_Speed_Front_Axle_Left_Wheel_Speed_Front_Axle_Left"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "J1939-0", "DT_84"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Bremsdruck_abbauen_VA_rechts_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Bremsdruck_abbauen_VA_links_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Bremsdruck_aufbauen_VA_rechts_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Bremsdruck_aufbauen_VA_links_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Auslassventil_oeffnen_VA_rechts_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Auslassventil_oeffnen_VA_links_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Bremsdruck_halten_VA_rechts_Start"),
        new Qualifier((QualifierTypes) 2, "EBS01T", "RT_Bremsdruck_halten_VA_links_Start")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[8]
      {
        new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_rechts_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Bremsdruck_abbauen_VA_links_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_rechts_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Bremsdruck_aufbauen_VA_links_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_rechts_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Auslassventil_oeffnen_VA_links_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_rechts_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        }),
        new ServiceCall("EBS01T", "RT_Bremsdruck_halten_VA_links_Start", (IEnumerable<string>) new string[1]
        {
          "Timing_Parameter=2000"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "EBS01T" };
  }
}
