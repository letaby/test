// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbFlashDataBlock
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbFlashDataBlock : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  MCDDbFlashSegments DbFlashSegments { get; }

  MCDDbFlashData DbFlashData { get; }

  MCDDbFlashIdents DbOwnIdents { get; }

  MCDDbFlashFilters DbFlashFilters { get; }

  long AddressOffset { get; }

  string DataBlockType { get; }

  MCDDbFlashSecurities DbSecurities { get; }

  MCDDbSpecialDataGroups DbSDGs { get; }

  MCDAudience AudienceState { get; }

  MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

  MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }
}
