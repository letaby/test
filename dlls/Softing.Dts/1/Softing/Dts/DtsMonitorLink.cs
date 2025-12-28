// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsMonitorLink
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsMonitorLink : DtsObject, MCDObject, IDisposable
{
  void Open();

  void Close();

  void OpenTraceFile(string TraceFileName, bool bOverwriteIfFileExists);

  void StartTraceFile();

  void StopTraceFile();

  void CloseTraceFile();

  uint TraceFileLimit { get; set; }

  byte[] GetLastItems(
    uint uNoOfItems,
    uint uLastTotalNoOfItems,
    ref uint puNoOfDeliveredItems,
    ref uint puTotalNoOfItems);

  bool FilterForDisplayAndFileFlag { get; set; }

  bool FilterForDisplayFlag { get; set; }

  bool BusloadFlag { get; set; }

  double CurrentBusLoad { get; }

  string CanFilter { set; }
}
