// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDConfigurationRecords
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

public interface MCDConfigurationRecords : 
  MCDNamedCollection,
  MCDCollection,
  MCDObject,
  IDisposable,
  IEnumerable
{
  List<MCDConfigurationRecord> ToList();

  MCDConfigurationRecord[] ToArray();

  event OnConfigurationRecordLoaded ConfigurationRecordLoaded;

  MCDConfigurationRecord GetItemByIndex(uint index);

  MCDConfigurationRecord GetItemByName(string name);

  void Remove(MCDConfigurationRecord ConfigurationRecord);

  void RemoveAll();

  void RemoveByIndex(uint index);

  void RemoveByName(string name);

  MCDConfigurationRecord AddByConfigurationIDAndDbConfigurationData(
    MCDValue ConfigurationID,
    MCDDbConfigurationData configurationData);

  MCDConfigurationRecord AddByDbObject(MCDDbConfigurationRecord DbConfigurationRecord);

  MCDConfigurationRecord AddByNameAndDbConfigurationData(
    string dbConfigurationRecordName,
    MCDDbConfigurationData dbConfigurationData);
}
