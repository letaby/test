// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsLogicalLinkMonitor
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsLogicalLinkMonitor : DtsObject, MCDObject, IDisposable
{
  void AddAllLogicalLinkForMonitoring();

  void RemoveAllLogicalLinkForMonitoring();

  void AddLogicalLinkForMonitoring(string NewLogicalLinkShortName);

  void RemoveLogicalLinkFromMonitoring(string RemoveLogicalLinkShortName);

  uint RingBufferSize { set; }

  void Start();

  void Stop();

  void OpenFileTrace(string FileName, bool bOverwrite);

  void StartFileTrace();

  void StopFileTrace();

  void CloseFileTrace();

  MCDCollection GetLatestEvents(uint uMaxNoOfNewEvents);

  void SetFilter(DtsLogicalLinkFilterConfig filterConfig, bool filterView, bool filterTrace);

  void OpenFileTraceInFolder(string outputFolderPath, string FileName);

  void StartSnapshotModeTracing(uint timeInterval);

  void GenerateSnapshotTrace(string outputPath);

  void TakeSnapshotTrace();

  void StopSnapshotModeTracing();
}
