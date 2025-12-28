// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDMonitoringLink
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDMonitoringLink : MCDNamedObject, MCDObject, IDisposable
{
  string[] FetchMonitoringFrames(uint numReq);

  MCDInterfaceResource InterfaceResource { get; }

  MCDMessageFilters MessageFilters { get; }

  ushort NoOfSampleToFireEvent { set; }

  ushort TimeToFireEvent { set; }

  void Start();

  void Stop();
}
