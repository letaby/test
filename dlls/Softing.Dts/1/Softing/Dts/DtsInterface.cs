// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterface
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsInterface : 
  MCDInterface,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  bool EthernetActivation { get; set; }

  bool ExecuteBroadcast();

  void SetEthernetPinState(bool State, uint Number);

  uint GetEthernetPinState(uint Number);

  string VendorModuleName { get; }

  bool SetPhysicalLinkId(string keyLink, uint id);

  DtsWLanSignalData WLanSignalData { get; }
}
