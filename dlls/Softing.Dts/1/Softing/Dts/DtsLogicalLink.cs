// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsLogicalLink
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsLogicalLink : MCDLogicalLink, MCDObject, IDisposable, DtsObject
{
  bool AutoSyncWithInternalState { get; set; }

  bool HasDetectedVariant { get; }

  void LockLink();

  MCDProtocolParameterSet ProtocolParameters { set; }

  bool SupportsTimeStamp();

  void UnlockLink();

  MCDService CreateDVServiceByRelationType(string relationType);

  void GotoOffline();

  byte[] ExecuteIoCtl(uint uIoCtlCommandId, byte[] pInputData);

  DtsPhysicalInterfaceLink PhysicalInterfaceLink { get; }

  bool ChannelMonitoring { get; set; }

  MCDAccessKey CreationAccessKey { get; }

  MCDLogicalLinkState InternalState { get; }

  void GotoOnlineWithTimeout(uint timeout);

  void OpenCached(bool useVariant);

  uint OpenCounter { get; }

  uint OnlineCounter { get; }

  uint StartCommCounter { get; }

  uint LockedCounter { get; }
}
