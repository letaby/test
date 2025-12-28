// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashFile
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FlashFile
{
  private string qualifier;
  private string name;
  private string description;
  private string fileName;
  private IList<string> ecus;
  private IEnumerable<FlashArea> flashAreas;
  private DiagnosisSource diagnosisSource;

  internal FlashFile()
  {
  }

  internal void Acquire(CaesarDICff file)
  {
    this.diagnosisSource = DiagnosisSource.CaesarDatabase;
    this.qualifier = file.Qualifier;
    this.name = file.Name;
    this.description = file.Description;
    this.fileName = file.FileName;
    List<string> stringList = new List<string>();
    for (uint index = 0; index < file.EcuRefCount; ++index)
      stringList.Add(file.GetEcuRefByIndex(index));
    this.ecus = (IList<string>) stringList.AsReadOnly();
    List<FlashArea> flashAreaList = new List<FlashArea>();
    uint flashAreaCount = file.FlashAreaCount;
    for (uint areaIndex = 0; areaIndex < flashAreaCount; ++areaIndex)
    {
      try
      {
        using (CaesarDIFlashArea flashAreaByIndex = file.GetFlashAreaByIndex(areaIndex))
        {
          if (flashAreaByIndex != null)
          {
            FlashArea flashArea = new FlashArea((Channel) null, flashAreaByIndex.Qualifier);
            flashArea.Acquire(flashAreaByIndex, areaIndex);
            flashAreaList.Add(flashArea);
          }
        }
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        CaesarException e = new CaesarException(ex, negativeResponseCode);
        Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
      }
    }
    this.flashAreas = (IEnumerable<FlashArea>) flashAreaList.AsReadOnly();
  }

  internal void Acquire(McdDBEcuMem file)
  {
    this.diagnosisSource = DiagnosisSource.McdDatabase;
    this.qualifier = file.Qualifier;
    this.name = file.Name;
    this.description = file.Description;
    this.ecus = (IList<string>) file.Ecus.ToList<string>();
    List<FlashArea> flashAreaList = new List<FlashArea>();
    uint num = 0;
    foreach (McdDBFlashSession dbFlashSession in file.DBFlashSessions)
    {
      FlashArea flashArea = new FlashArea((Channel) null, dbFlashSession.Qualifier);
      flashArea.Acquire(dbFlashSession, (IEnumerable<string>) this.ecus, num++);
      flashAreaList.Add(flashArea);
    }
    this.flashAreas = (IEnumerable<FlashArea>) flashAreaList.AsReadOnly();
    this.fileName = this.flashAreas.SelectMany<FlashArea, FlashMeaning>((Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).Select<FlashMeaning, string>((Func<FlashMeaning, string>) (fm => fm.FileName)).FirstOrDefault<string>();
  }

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string Description => this.description;

  public string FileName => this.fileName;

  public IList<string> Ecus => this.ecus;

  public IEnumerable<FlashArea> FlashAreas => this.flashAreas;

  public DiagnosisSource DiagnosisSource => this.diagnosisSource;
}
