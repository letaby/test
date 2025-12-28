// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashSegment
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsDbFlashSegment : 
  MCDDbFlashSegment,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  byte[] GetBinaryDataOffset(uint uOffset, uint uLength);

  [Obsolete("Function is marked as deprecated!")]
  byte[] GetFirstBinaryDataChunk(uint size);

  [Obsolete("Function is marked as deprecated!")]
  bool HasNextBinaryDataChunk { get; }

  [Obsolete("Function is marked as deprecated!")]
  byte[] NextBinaryDataChunk { get; }

  ulong SourceEndAddress64 { get; }

  ulong SourceStartAddress64 { get; }

  ulong UncompressedSize64 { get; }

  ulong CompressedSize64 { get; }

  byte[] GetBinaryDataOffset64(ulong uOffset, ulong uLength);
}
