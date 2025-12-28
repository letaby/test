// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceInformation
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsInterfaceInformation : 
  DtsNamedObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsObject
{
  DtsBusSystemInterfaceType BusSystemInterfaceType { get; }

  bool SupportsIpAddress { get; }

  MCDDbInterfaceCables DbInterfaceCables { get; }

  string PDUAPIVersion { get; }

  DtsInterfaceLinkInformations InterfaceLinks { get; }

  string[] VendorModuleNames { get; }

  string DefaultCable { get; }
}
