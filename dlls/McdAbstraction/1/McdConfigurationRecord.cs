// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdConfigurationRecord
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdConfigurationRecord : IDisposable
{
  private McdLogicalLink link;
  private MCDConfigurationRecord record;
  private McdDBConfigurationRecord dbRecord;
  private string qualifier;
  private Dictionary<string, McdOptionItem> optionItems;
  private bool disposedValue = false;

  internal McdConfigurationRecord(
    McdLogicalLink link,
    MCDConfigurationRecord record,
    McdDBConfigurationRecord dbRecord)
  {
    this.link = link;
    this.record = record;
    this.dbRecord = dbRecord;
    this.qualifier = record.ShortName;
    this.optionItems = new Dictionary<string, McdOptionItem>();
    foreach (MCDOptionItem optionItem1 in (IEnumerable) record.OptionItems)
    {
      MCDOptionItem optionItem = optionItem1;
      this.optionItems.Add(optionItem.ShortName, new McdOptionItem(optionItem, this.dbRecord.DBOptionItems.FirstOrDefault<McdDBOptionItem>((Func<McdDBOptionItem, bool>) (oi => oi.Qualifier == optionItem.ShortName))));
    }
  }

  internal MCDConfigurationRecord Handle => this.record;

  public McdDBConfigurationRecord DBRecord => this.dbRecord;

  public string Qualifier => this.qualifier;

  public IEnumerable<byte> ConfigurationRecord
  {
    get => (IEnumerable<byte>) this.record.ConfigurationRecord;
  }

  public void SetConfigurationRecordByDBObject(McdDBDataRecord value)
  {
    try
    {
      this.record.ConfigurationRecordByDbObject = value.Handle;
    }
    catch (MCDException ex)
    {
      if (!value.BinaryData.SequenceEqual<byte>(this.ConfigurationRecord))
        throw new McdException(ex, "ConfigurationRecordByDBObject");
    }
  }

  public IEnumerable<McdOptionItem> OptionItems
  {
    get => (IEnumerable<McdOptionItem>) this.optionItems.Values.ToList<McdOptionItem>();
  }

  internal McdDBDataRecord GetDefaultString(string partNumber)
  {
    return this.dbRecord.DBDataRecords.FirstOrDefault<McdDBDataRecord>((Func<McdDBDataRecord, bool>) (dr => dr.PartNumber == partNumber));
  }

  internal Tuple<McdOptionItem, McdDBItemValue> GetFragment(string partNumber)
  {
    foreach (McdOptionItem mcdOptionItem in this.optionItems.Values)
    {
      McdDBItemValue mcdDbItemValue = mcdOptionItem.DBOptionItem.DBItemValues.FirstOrDefault<McdDBItemValue>((Func<McdDBItemValue, bool>) (iv => iv.PartNumber == partNumber));
      if (mcdDbItemValue != null)
        return new Tuple<McdOptionItem, McdDBItemValue>(mcdOptionItem, mcdDbItemValue);
    }
    return (Tuple<McdOptionItem, McdDBItemValue>) null;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      if (this.link != null)
      {
        this.link.RemoveConfigRecord(this);
        this.link = (McdLogicalLink) null;
      }
      foreach (McdOptionItem mcdOptionItem in this.optionItems.Values)
        mcdOptionItem.Dispose();
      this.optionItems.Clear();
      if (this.record != null)
      {
        this.record.Dispose();
        this.record = (MCDConfigurationRecord) null;
      }
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
