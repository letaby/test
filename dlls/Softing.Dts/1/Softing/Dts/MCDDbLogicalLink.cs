// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbLogicalLink
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbLogicalLink : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  MCDDbLocation DbLocation { get; }

  MCDDbPhysicalVehicleLinkOrInterface DbPhysicalVehicleLinkOrInterface { get; }

  bool IsAccessedViaGateway { get; }

  MCDGatewayMode GatewayMode { get; }

  MCDDbLogicalLinks DbLogicalLinksOfGateways { get; }

  MCDDbPhysicalVehicleLink DbPhysicalVehicleLink { get; }

  string ProtocolType { get; }
}
