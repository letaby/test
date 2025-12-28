// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbOptionItem
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbOptionItem : 
  MCDDbConfigurationItem,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable
{
  MCDDbItemValues DbItemValues { get; }

  ushort DecimalPlaces { get; }

  MCDValue PhysicalDefaultValue { get; }

  MCDAudience ReadAudienceState { get; }

  MCDAudience WriteAudienceState { get; }

  MCDDbAdditionalAudiences DbDisabledReadAdditionalAudiences { get; }

  MCDDbAdditionalAudiences DbEnabledReadAdditionalAudiences { get; }

  MCDDbAdditionalAudiences DbDisabledWriteAdditionalAudiences { get; }

  MCDDbAdditionalAudiences DbEnabledWriteAdditionalAudiences { get; }
}
