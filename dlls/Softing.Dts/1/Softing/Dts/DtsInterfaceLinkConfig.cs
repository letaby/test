// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceLinkConfig
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsInterfaceLinkConfig : DtsObject, MCDObject, IDisposable
{
  DtsPhysicalLinkOrInterfaceType LinkType { get; set; }

  DtsPduApiLinkType PduApiLinkType { get; set; }

  int GlobalIndex { get; set; }

  int LocalIndex { get; set; }

  void Assign(DtsInterfaceLinkInformation linkInformation);

  MCDConnectorPinType GetPinType(uint index);

  uint GetVehiclePin(uint index);

  uint PinCount { get; }

  string String { get; }
}
