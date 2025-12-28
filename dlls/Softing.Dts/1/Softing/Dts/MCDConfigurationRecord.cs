// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDConfigurationRecord
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDConfigurationRecord : MCDNamedObject, MCDObject, IDisposable
{
  MCDConfigurationIdItem ConfigurationIdItem { get; }

  MCDDbConfigurationRecord DbObject { get; }

  MCDError Error { get; }

  MCDErrors Errors { get; }

  MCDOptionItems OptionItems { get; }

  MCDReadDiagComPrimitives ReadDiagComPrimitives { get; }

  MCDSystemItems SystemItems { get; }

  MCDWriteDiagComPrimitives WriteDiagComPrimitives { get; }

  bool HasError { get; }

  void LoadCodingData(string filename);

  MCDDataIdItem DataIdItem { get; }

  string ActiveFileName { get; }

  byte[] ConfigurationRecord { get; set; }

  string[] MatchingFileNames { get; }

  void RemoveReadDiagComPrimitives(MCDReadDiagComPrimitives readDiagComs);

  void RemoveWriteDiagComPrimitives(MCDWriteDiagComPrimitives writeDiagComs);

  MCDDbDataRecord ConfigurationRecordByDbObject { set; }
}
