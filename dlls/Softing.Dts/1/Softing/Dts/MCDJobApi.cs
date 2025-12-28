// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDJobApi
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public interface MCDJobApi
{
  void SendIntermediateResults(MCDResults _results);

  void SendFinalResult(MCDResult _result);

  MCDError CreateError(
    int _code,
    string _text,
    ushort _vendorcode,
    string _information,
    int _severity);

  void ReleaseError(MCDError _error);

  MCDException CreateException(
    int _type,
    int _code,
    string _text,
    ushort _vendorcode,
    string _information,
    int _severity);

  void ReleaseException(MCDException _exception);

  MCDValue CreateValue(int _dataType);

  void ReleaseValue(MCDValue _value);

  void StartDownloadDataBlock(MCDLogicalLink _link, MCDDbFlashDataBlock _datablock);

  void StartDownloadSegment(
    MCDLogicalLink _link,
    MCDDbFlashDataBlock _datablock,
    MCDDbFlashSegment _segment);

  void SetProgress(MCDLogicalLink _link, MCDDiagComPrimitive _primitive, short _progress);

  void SetJobInfo(MCDLogicalLink _link, MCDDiagComPrimitive _primitive, string _info);

  int GetClampState(string _clampName);

  void SetJobInfoAsUnicodeString(
    MCDLogicalLink _link,
    MCDDiagComPrimitive _primitive,
    string _info);
}
