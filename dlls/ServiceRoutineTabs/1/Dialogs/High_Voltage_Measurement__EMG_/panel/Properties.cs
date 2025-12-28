// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.High_Voltage_Measurement__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.High_Voltage_Measurement__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 27;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\High Voltage Measurement (EMG).panel";
    }
  }

  public virtual string Guid => "341d2647-af02-44cb-91fd-6448e7402af1";

  public virtual string DisplayName => "High Voltage Measurement";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "ePowertrain";

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
      return (IEnumerable<Qualifier>) new Qualifier[24]
      {
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_VoltageReadoutStat"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_OTF_Readout"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_HV_Readout_Start"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_HV_Readout_Stop"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_OTF_Readout"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab2"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS01"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS02"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS03"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS04"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS05"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS06"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS07"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS08"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS09"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti1ActDcVolt"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti2ActDcVolt"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti3ActDcVolt_3"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalDCL"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab1"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt1HvVoltage"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt2HvVoltage"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalEComp"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_ErcHvVolt")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Request_Results_VoltageReadoutStat", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }
}
