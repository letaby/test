// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashFileCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FlashFileCollection : LateLoadReadOnlyCollection<FlashFile>
{
  internal FlashFileCollection()
  {
  }

  protected override void AcquireList()
  {
    this.Items.Clear();
    Sapi.GetSapi().EnsureFlashFilesLoaded();
    FlashFileCollection.AcquireCaesarList().ForEach((Action<FlashFile>) (ff => this.Items.Add(ff)));
    FlashFileCollection.AcquireMcdList().ForEach((Action<FlashFile>) (ff => this.Items.Add(ff)));
  }

  private static List<FlashFile> AcquireCaesarList()
  {
    List<FlashFile> flashFileList = new List<FlashFile>();
    uint cffFileCount = CaesarRoot.CffFileCount;
    for (uint index = 0; index < cffFileCount; ++index)
    {
      using (CaesarDICff file = CaesarRoot.OpenCffFile(index))
      {
        if (file != null)
        {
          FlashFile flashFile = new FlashFile();
          flashFile.Acquire(file);
          flashFileList.Add(flashFile);
        }
      }
    }
    return flashFileList;
  }

  private static List<FlashFile> AcquireMcdList()
  {
    List<FlashFile> flashFileList = new List<FlashFile>();
    foreach (string dbEcuMemName in McdRoot.DBEcuMemNames)
    {
      FlashFile flashFile = new FlashFile();
      flashFile.Acquire(McdRoot.GetDBEcuMem(dbEcuMemName));
      flashFileList.Add(flashFile);
    }
    return flashFileList;
  }

  internal void ClearList()
  {
    this.Items.Clear();
    this.ResetList();
  }

  internal void RebuildList(DiagnosisSource diagnosisSource)
  {
    List<FlashFile> flashFileList = new List<FlashFile>();
    switch (diagnosisSource)
    {
      case DiagnosisSource.CaesarDatabase:
        flashFileList.AddRange((IEnumerable<FlashFile>) FlashFileCollection.AcquireCaesarList());
        flashFileList.AddRange(this.Items.Where<FlashFile>((Func<FlashFile, bool>) (ff => ff.DiagnosisSource == DiagnosisSource.McdDatabase)));
        break;
      case DiagnosisSource.McdDatabase:
        flashFileList.AddRange(this.Items.Where<FlashFile>((Func<FlashFile, bool>) (ff => ff.DiagnosisSource == DiagnosisSource.CaesarDatabase)));
        flashFileList.AddRange((IEnumerable<FlashFile>) FlashFileCollection.AcquireMcdList());
        break;
    }
    this.Items.Clear();
    flashFileList.ForEach((Action<FlashFile>) (ff => this.Items.Add(ff)));
  }

  public static DiagnosisSource GetFlashFileDiagnosisSource(string path)
  {
    return !(Path.GetExtension(path).ToUpperInvariant() == ".SMR-F") ? DiagnosisSource.CaesarDatabase : DiagnosisSource.McdDatabase;
  }

  public FlashFile this[string qualifier]
  {
    get
    {
      return this.FirstOrDefault<FlashFile>((Func<FlashFile, bool>) (item => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal)));
    }
  }
}
