// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingFileCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingFileCollection : LateLoadReadOnlyCollection<CodingFile>
{
  internal CodingFileCollection()
  {
  }

  protected override void AcquireList()
  {
    foreach (IGrouping<string, CodingFileCollection.CcfVersionInfo> grouping in (IEnumerable<IGrouping<string, CodingFileCollection.CcfVersionInfo>>) ((IEnumerable<string>) Directory.GetFiles(Sapi.GetSapi().ConfigurationItems["CCFFiles"].Value, "*.ccf")).Select<string, CodingFileCollection.CcfVersionInfo>((Func<string, CodingFileCollection.CcfVersionInfo>) (path => new CodingFileCollection.CcfVersionInfo(path))).ToList<CodingFileCollection.CcfVersionInfo>().OrderBy<CodingFileCollection.CcfVersionInfo, Version>((Func<CodingFileCollection.CcfVersionInfo, Version>) (cv => cv.VarcodeVersion)).GroupBy<CodingFileCollection.CcfVersionInfo, string>((Func<CodingFileCollection.CcfVersionInfo, string>) (cv => cv.EcuQualifier)).OrderBy<IGrouping<string, CodingFileCollection.CcfVersionInfo>, string>((Func<IGrouping<string, CodingFileCollection.CcfVersionInfo>, string>) (g => g.Key)))
    {
      CodingFileCollection.CcfVersionInfo element = grouping.Last<CodingFileCollection.CcfVersionInfo>();
      if (grouping.Count<CodingFileCollection.CcfVersionInfo>() > 1)
      {
        foreach (CodingFileCollection.CcfVersionInfo ccfVersionInfo in grouping.Except<CodingFileCollection.CcfVersionInfo>(Enumerable.Repeat<CodingFileCollection.CcfVersionInfo>(element, 1)))
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Multiple CCFs found for {0}, not adding {1} ({2}) to CAESAR", (object) ccfVersionInfo.EcuQualifier, (object) ccfVersionInfo.VarcodeVersion, (object) Path.GetFileName(ccfVersionInfo.Path)));
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CCF used for {0} is {1} ({2})", (object) element.EcuQualifier, (object) element.VarcodeVersion, (object) Path.GetFileName(element.Path)));
      }
      CaesarRoot.AddCcfFile(element.Path);
    }
    this.Items.Clear();
    uint ccfFileCount = CaesarRoot.CcfFileCount;
    for (uint index = 0; index < ccfFileCount; ++index)
    {
      using (CaesarDICcf file = CaesarRoot.OpenCcfFile(index))
      {
        if (file != null)
        {
          CodingFile codingFile = new CodingFile();
          codingFile.Acquire(file);
          this.Items.Add(codingFile);
        }
      }
    }
    string[] files = Directory.GetFiles(Sapi.GetSapi().ConfigurationItems["CCFFiles"].Value, "*.CPF", SearchOption.TopDirectoryOnly);
    for (int index = 0; index < files.Length; ++index)
    {
      StreamReader streamReader = new StreamReader(files[index]);
      try
      {
        string identificationRecordValue1 = ParameterCollection.GetIdentificationRecordValue("PARTNUMBER", streamReader);
        if (!string.IsNullOrEmpty(identificationRecordValue1))
        {
          string identificationRecordValue2 = ParameterCollection.GetIdentificationRecordValue("ECU", streamReader);
          Ecu ecu = Sapi.GetSapi().Ecus[identificationRecordValue2];
          if (ecu != null)
          {
            CodingFile codingFile = new CodingFile();
            codingFile.Acquire(streamReader, files[index], ecu, new Part(identificationRecordValue1));
            this.Items.Add(codingFile);
          }
          else
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to locate ECU defined in CPF file: {0}", (object) identificationRecordValue2));
        }
      }
      finally
      {
        streamReader.Close();
      }
    }
    foreach (string configurationDataName in McdRoot.DBConfigurationDataNames)
    {
      CodingFile codingFile = new CodingFile();
      codingFile.Acquire(McdRoot.GetDBConfigurationData(configurationDataName));
      this.Items.Add(codingFile);
    }
  }

  internal void ClearList()
  {
    this.Items.Clear();
    this.ResetList();
  }

  public CodingFile this[string qualifier]
  {
    get
    {
      return this.FirstOrDefault<CodingFile>((Func<CodingFile, bool>) (item => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal)));
    }
  }

  private struct CcfVersionInfo
  {
    public readonly string EcuQualifier;
    public readonly string Path;
    public readonly Version VarcodeVersion;

    public CcfVersionInfo(string ccfFile)
    {
      CaesarRoot.AddCcfFile(ccfFile);
      if (CaesarRoot.CcfFileCount != 1U)
        throw new CaesarException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version number of CCF {0} could not be determined because the file could not be loaded into CAESAR. the file may be corrupt.", (object) Path.GetFileName(ccfFile)));
      using (CaesarDICcf caesarDiCcf = CaesarRoot.OpenCcfFile(0U))
      {
        this.EcuQualifier = caesarDiCcf.GetEcuRefByIndex(0U);
        this.Path = caesarDiCcf.FileName;
        if (!Version.TryParse(caesarDiCcf.VarCodeVersion, out this.VarcodeVersion))
        {
          this.VarcodeVersion = new Version();
          Sapi.GetSapi().RaiseDebugInfoEvent((object) caesarDiCcf.FileName, "Version cannot be parsed");
        }
      }
      CaesarRoot.RemoveCcfFile(ccfFile);
    }
  }
}
