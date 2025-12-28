// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbDataRecord
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbDataRecord : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  byte[] BinaryData { get; }

  MCDFlashDataFormat DataFormat { get; }

  MCDValue DataID { get; }

  MCDDbCodingData DbCodingData { get; }

  string Key { get; }

  string Rule { get; }

  string UserDefinedFormat { get; }

  MCDAudience AudienceState { get; }

  MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

  MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

  MCDDbSpecialDataGroups DbSDGs { get; }
}
