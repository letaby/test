// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingFile
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingFile
{
  private string qualifier;
  private string name;
  private string description;
  private string fileName;
  private string preamble;
  private string author;
  private string date;
  private string version;
  private CodingParameterGroupCollection codingParameterGroups;
  private IList<Ecu> ecus;

  internal CodingFile()
  {
    this.codingParameterGroups = new CodingParameterGroupCollection((Channel) null);
  }

  internal void Acquire(CaesarDICcf file)
  {
    this.qualifier = file.Qualifier;
    this.name = file.Name;
    this.description = file.Description;
    this.author = file.VarCodeAuthor;
    this.date = file.VarCodeDate;
    this.version = file.VarCodeVersion;
    this.fileName = file.FileName;
    this.preamble = file.Preamble;
    uint varCodeDomainCount = file.VarCodeDomainCount;
    for (uint index = 0; index < varCodeDomainCount; ++index)
    {
      using (CaesarDIVcd varcode = file.OpenVarCodeDomain(index))
      {
        if (varcode != null)
        {
          CodingParameterGroup group = new CodingParameterGroup(this.codingParameterGroups);
          group.Acquire(varcode);
          this.codingParameterGroups.Add(group);
        }
      }
    }
    List<Ecu> ecuList = new List<Ecu>();
    for (uint index = 0; index < file.EcuRefCount; ++index)
    {
      string ecuName = file.GetEcuRefByIndex(index);
      Ecu ecu = Sapi.GetSapi().Ecus.FirstOrDefault<Ecu>((Func<Ecu, bool>) (e => e.Name == ecuName && e.DiagnosisSource == DiagnosisSource.CaesarDatabase));
      if (ecu != null)
        ecuList.Add(ecu);
      else
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"CodingFile {this.fileName} references unknown ECU-qualifier {ecuName}. A CBF is corrupt or missing.");
    }
    this.ecus = (IList<Ecu>) ecuList.AsReadOnly();
  }

  internal void Acquire(McdDBConfigurationData file)
  {
    Sapi sapi = Sapi.GetSapi();
    this.qualifier = file.Qualifier;
    this.name = file.Name;
    this.description = file.Description;
    this.version = file.Version;
    this.fileName = file.DatabaseFile;
    this.preamble = file.Preamble;
    this.ecus = (IList<Ecu>) file.EcuNames.Select<string, Ecu>((Func<string, Ecu>) (name => sapi.Ecus.FirstOrDefault<Ecu>((Func<Ecu, bool>) (ecu => ecu.Name == name && ecu.DiagnosisSource == DiagnosisSource.McdDatabase)))).ToList<Ecu>().AsReadOnly();
    List<DiagnosisVariant> variants = new List<DiagnosisVariant>();
    foreach (Ecu ecu1 in (IEnumerable<Ecu>) this.ecus)
    {
      Ecu ecu = ecu1;
      if (file.EcuVariantNames.Any<string>())
        variants.AddRange(file.EcuVariantNames.Select<string, DiagnosisVariant>((Func<string, DiagnosisVariant>) (evn => ecu.DiagnosisVariants[evn])).Where<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (dv => dv != null)));
      else
        variants.AddRange(ecu.DiagnosisVariants.Where<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => !v.IsBase && !v.IsBoot)));
    }
    foreach (McdDBConfigurationRecord configurationRecord in file.DBConfigurationRecords)
    {
      CodingParameterGroup group = new CodingParameterGroup(this.codingParameterGroups);
      group.Acquire(configurationRecord, (IEnumerable<DiagnosisVariant>) variants);
      this.codingParameterGroups.Add(group);
    }
  }

  internal void Acquire(StreamReader reader, string path, Ecu ecu, Part partNumber)
  {
    this.name = Path.GetFileNameWithoutExtension(path);
    this.fileName = this.qualifier = Path.GetFileName(path);
    this.date = Sapi.TimeToString(File.GetCreationTime(path));
    this.preamble = string.Empty;
    this.ecus = (IList<Ecu>) new ReadOnlyCollection<Ecu>((IList<Ecu>) new List<Ecu>()
    {
      ecu
    });
    CodingParameterGroup group = new CodingParameterGroup(this.codingParameterGroups);
    group.Acquire(reader, this.name, ecu, partNumber);
    this.codingParameterGroups.Add(group);
  }

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string Description => this.description;

  public string FileName => this.fileName;

  public string Preamble => this.preamble;

  public string Author => this.author;

  public string Date => this.date;

  public string Version => this.version;

  public CodingParameterGroupCollection CodingParameterGroups => this.codingParameterGroups;

  public IList<Ecu> Ecus => this.ecus;

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("The GetEcus method is deprecated, please use the Ecus property instead.")]
  public Ecu[] GetEcus() => this.ecus.ToArray<Ecu>();
}
