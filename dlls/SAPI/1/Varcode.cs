// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Varcode
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

internal abstract class Varcode : IDisposable
{
  protected bool disposedValue;

  internal bool IsErrorSet => this.Exception != null;

  internal CaesarException Exception { set; get; }

  internal abstract void DoCoding();

  internal abstract bool AllowSetDefaultString(string groupQualifier);

  internal abstract void EnableReadCodingStringFromEcu(bool enableReadCodingStringFromEcu);

  internal abstract byte[] GetCurrentCodingString(string groupQualifier);

  internal abstract void SetCurrentCodingString(string groupQualifier, byte[] content);

  internal abstract void SetDefaultStringByPartNumber(string partNumber);

  internal abstract void SetDefaultStringByPartNumberAndPartVersion(
    string partNumber,
    uint partVersion);

  internal abstract void SetFragmentMeaningByPartNumber(string partNumber);

  internal abstract void SetFragmentMeaningByPartNumberAndPartVersion(
    string partNumber,
    uint partVersion);

  internal abstract void SetFragmentValue(Parameter parameter, object value);

  internal abstract object GetFragmentValue(Parameter parameter);

  protected abstract void Dispose(bool disposing);

  public void Dispose() => this.Dispose(true);
}
