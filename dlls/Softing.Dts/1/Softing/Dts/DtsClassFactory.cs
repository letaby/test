// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsClassFactory
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsClassFactory : DtsObject, MCDObject, IDisposable
{
  MCDValue CreateValue(MCDDataType type);

  MCDAccessKey CreateAccessKey(string accessKey);

  MCDError CreateError(
    MCDErrorCodes code,
    string text,
    ushort vendorcode,
    string information,
    MCDSeverity severity);
}
