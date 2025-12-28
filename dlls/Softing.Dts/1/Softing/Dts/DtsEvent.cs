// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsEvent
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsEvent : DtsObject, MCDObject, IDisposable
{
  DtsEventType EventType { get; }

  DtsEventId EventId { get; }

  MCDError Error { get; }

  MCDDiagComPrimitive DiagComPrimitive { get; }

  string JobInfo { get; }

  byte Progress { get; }

  MCDResultState ResultState { get; }

  MCDLockState LockState { get; }

  MCDLogicalLink LogicalLink { get; }

  MCDLogicalLinkState LogicalLinkState { get; }

  string Clamp { get; }

  MCDClampState ClampState { get; }

  MCDMonitoringLink MonitoringLink { get; }

  MCDInterface Interface { get; }

  MCDInterfaceStatus InterfaceStatus { get; }

  MCDConfigurationRecord ConfigurationRecord { get; }

  MCDValues DynIdList { get; }
}
