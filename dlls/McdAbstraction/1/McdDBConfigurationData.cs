// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBConfigurationData
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBConfigurationData
{
  private MCDDbConfigurationData configurationData;
  private IEnumerable<McdDBConfigurationRecord> records;
  private string databaseFile;
  private string qualifier;
  private string version;
  private IEnumerable<string> ecus;
  private IEnumerable<string> variants;

  internal McdDBConfigurationData(MCDDbConfigurationData data)
  {
    this.configurationData = data;
    this.qualifier = data.ShortName;
    this.version = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:00}", (object) data.Version.Major, (object) data.Version.Minor, (object) data.Version.Revision);
    this.databaseFile = McdRoot.DatabaseFileList.FirstOrDefault<Tuple<string, string, string>>((Func<Tuple<string, string, string>, bool>) (fl => fl.Item1.EndsWith("smr-e", StringComparison.OrdinalIgnoreCase) && fl.Item2 == this.qualifier && fl.Item3 == this.version))?.Item1;
  }

  public IEnumerable<McdDBConfigurationRecord> DBConfigurationRecords
  {
    get
    {
      if (this.records == null)
        this.records = this.configurationData.DbConfigurationRecords.OfType<MCDDbConfigurationRecord>().Select<MCDDbConfigurationRecord, McdDBConfigurationRecord>((Func<MCDDbConfigurationRecord, McdDBConfigurationRecord>) (cr => new McdDBConfigurationRecord(cr)));
      return this.records;
    }
  }

  public string Name => this.configurationData.LongName;

  public string Qualifier => this.qualifier;

  public string Description => this.configurationData.Description;

  public string Version => this.version;

  public string DatabaseFile => this.databaseFile;

  public string Preamble
  {
    get
    {
      return string.IsNullOrEmpty(this.DatabaseFile) ? (string) null : McdRoot.GetPreamble(this.DatabaseFile);
    }
  }

  public IEnumerable<string> EcuNames
  {
    get
    {
      if (this.ecus == null)
        this.ecus = (IEnumerable<string>) this.configurationData.DbEcuBaseVariants.Names;
      return this.ecus;
    }
  }

  public IEnumerable<string> EcuVariantNames
  {
    get
    {
      if (this.variants == null)
        this.variants = (IEnumerable<string>) this.configurationData.DbEcuVariants.Names;
      return this.variants;
    }
  }
}
