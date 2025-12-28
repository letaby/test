// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBConfigurationRecord
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBConfigurationRecord
{
  private MCDDbConfigurationRecord configurationRecord;
  private IEnumerable<McdDBDataRecord> dataRecords;
  private IEnumerable<McdDBOptionItem> optionItems;

  internal McdDBConfigurationRecord(MCDDbConfigurationRecord data)
  {
    this.configurationRecord = data;
  }

  public IEnumerable<McdDBDataRecord> DBDataRecords
  {
    get
    {
      if (this.dataRecords == null)
        this.dataRecords = (IEnumerable<McdDBDataRecord>) this.configurationRecord.DbDataRecords.OfType<MCDDbDataRecord>().Select<MCDDbDataRecord, McdDBDataRecord>((Func<MCDDbDataRecord, McdDBDataRecord>) (cr => new McdDBDataRecord(cr))).ToList<McdDBDataRecord>();
      return this.dataRecords;
    }
  }

  public IEnumerable<McdDBOptionItem> DBOptionItems
  {
    get
    {
      if (this.optionItems == null)
      {
        try
        {
          this.optionItems = (IEnumerable<McdDBOptionItem>) this.configurationRecord.DbOptionItems.OfType<MCDDbOptionItem>().Select<MCDDbOptionItem, McdDBOptionItem>((Func<MCDDbOptionItem, McdDBOptionItem>) (oi => new McdDBOptionItem(oi))).ToList<McdDBOptionItem>();
        }
        catch (DtsSystemException ex)
        {
          throw new McdException(ex.Message, (Exception) ex);
        }
        catch (DtsProgramViolationException ex)
        {
          throw new McdException(ex.Message, (Exception) ex);
        }
      }
      return this.optionItems;
    }
  }

  public string Name => this.configurationRecord.LongName;

  internal MCDDbConfigurationRecord Handle => this.configurationRecord;

  public string Qualifier => this.configurationRecord.ShortName;

  public string Description => this.configurationRecord.Description;

  public int ByteLength => (int) this.configurationRecord.ByteLength;
}
