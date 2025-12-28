// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace SapiLayer1;

public sealed class EcuCollection : LateLoadReadOnlyCollection<Ecu>
{
  internal EcuCollection()
  {
  }

  private IEnumerable<EcuCollection.CbfVersionInfo> GetVersionInfo(string[] paths)
  {
    List<EcuCollection.CbfVersionInfo> versionInfo = new List<EcuCollection.CbfVersionInfo>();
    foreach (string path in paths)
    {
      try
      {
        versionInfo.Add(new EcuCollection.CbfVersionInfo(path));
      }
      catch (CaesarException ex)
      {
        if (this.IgnoreDiagnosisDescriptionLoadFailure)
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, ex.Message);
        else
          throw;
      }
    }
    return (IEnumerable<EcuCollection.CbfVersionInfo>) versionInfo;
  }

  protected override void AcquireList()
  {
    List<string> stringList = new List<string>();
    foreach (IGrouping<string, EcuCollection.CbfVersionInfo> grouping in (IEnumerable<IGrouping<string, EcuCollection.CbfVersionInfo>>) this.GetVersionInfo(Directory.GetFiles(Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, "*.cbf")).OrderBy<EcuCollection.CbfVersionInfo, Version>((Func<EcuCollection.CbfVersionInfo, Version>) (cv => cv.DescriptionVersion)).ThenBy<EcuCollection.CbfVersionInfo, bool>((Func<EcuCollection.CbfVersionInfo, bool>) (cv => cv.Is64BitCompatible)).GroupBy<EcuCollection.CbfVersionInfo, string>((Func<EcuCollection.CbfVersionInfo, string>) (cv => cv.EcuQualifier)).OrderBy<IGrouping<string, EcuCollection.CbfVersionInfo>, string>((Func<IGrouping<string, EcuCollection.CbfVersionInfo>, string>) (g => g.Key)))
    {
      EcuCollection.CbfVersionInfo element = grouping.LastOrDefault<EcuCollection.CbfVersionInfo>((Func<EcuCollection.CbfVersionInfo, bool>) (cbf => cbf.Is64BitCompatible || !Environment.Is64BitProcess));
      EcuCollection.CbfVersionInfo cbfVersionInfo1 = grouping.LastOrDefault<EcuCollection.CbfVersionInfo>();
      if (element == null && !cbfVersionInfo1.HasDiagjobs)
        element = cbfVersionInfo1;
      foreach (EcuCollection.CbfVersionInfo cbfVersionInfo2 in grouping.Except<EcuCollection.CbfVersionInfo>(Enumerable.Repeat<EcuCollection.CbfVersionInfo>(element, 1)))
      {
        if (cbfVersionInfo2 == cbfVersionInfo1)
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Greatest CBF version found for {0} is not 64-bit compatible. Not adding {1} ({2}) to CAESAR. A compatible lower version {3} available.", (object) cbfVersionInfo2.EcuQualifier, (object) cbfVersionInfo2.DescriptionVersion, (object) Path.GetFileName(cbfVersionInfo2.CbfPath), element != null ? (object) "is" : (object) "is not"));
        else if (grouping.Count<EcuCollection.CbfVersionInfo>() > 1)
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Multiple CBFs found for {0}, not adding {1} ({2}) to CAESAR", (object) cbfVersionInfo2.EcuQualifier, (object) cbfVersionInfo2.DescriptionVersion, (object) Path.GetFileName(cbfVersionInfo2.CbfPath)));
      }
      if (element != null)
      {
        if (grouping.Count<EcuCollection.CbfVersionInfo>() > 1)
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CBF used for {0} is {1} ({2})", (object) element.EcuQualifier, (object) element.DescriptionVersion, (object) Path.GetFileName(element.CbfPath)));
        stringList.Add(element.CbfPath);
      }
      else
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There are no 64-bit compatible CBFs for {0}", (object) grouping.Key));
    }
    stringList.ForEach((Action<string>) (usedCbf => CaesarRoot.AddCbfFile(usedCbf)));
    this.Items.Clear();
    foreach (string ecu in CaesarRoot.Ecus)
    {
      Ecu sender = new Ecu(ecu, string.Empty, DiagnosisSource.CaesarDatabase);
      try
      {
        sender.AcquireInfo();
        this.Items.Add(sender);
      }
      catch (CaesarErrorException ex)
      {
        Sapi.GetSapi().RaiseExceptionEvent((object) sender, (Exception) new CaesarException(ex));
      }
    }
    foreach (string ecuBaseVariantName in McdRoot.DBEcuBaseVariantNames)
    {
      if (McdRoot.GetDBEcuBaseVariant(ecuBaseVariantName).DBEcuVariantNames.Any<string>())
      {
        Ecu ecu = new Ecu(ecuBaseVariantName, string.Empty, DiagnosisSource.McdDatabase);
        ecu.AcquireInfoMCD();
        this.Items.Add(ecu);
      }
    }
    foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) this)
    {
      if (ecu.Properties.ContainsKey("ViaEcu"))
      {
        foreach (string str in ecu.Properties["ViaEcu"].Split(";".ToCharArray()))
        {
          int length = str.IndexOf(',');
          Ecu viaEcu = this[length != -1 ? str.Substring(0, length) : str];
          if (viaEcu != null)
            ecu.AddViaEcu(viaEcu, length != -1 ? str.Substring(length + 1) : (string) null);
        }
      }
    }
    foreach (Ecu ecu in RollCallJ1708.GlobalInstance.Ecus.Union<Ecu>(RollCallJ1939.GlobalInstance.Ecus).Union<Ecu>(RollCallDoIP.GlobalInstance.Ecus))
      this.Items.Add(ecu);
    foreach (IGrouping<string, Ecu> source in this.Items.GroupBy<Ecu, string>((Func<Ecu, string>) (e => e.Name)).Where<IGrouping<string, Ecu>>((Func<IGrouping<string, Ecu>, bool>) (g => g.Count<Ecu>() > 1)))
    {
      if (source.Count<Ecu>() != 2 || source.Count<Ecu>((Func<Ecu, bool>) (e => e.DiagnosisSource == DiagnosisSource.CaesarDatabase)) != 1 || source.Count<Ecu>((Func<Ecu, bool>) (e => e.DiagnosisSource == DiagnosisSource.McdDatabase)) != 1)
        throw new InvalidOperationException($"Ecu names must be unique. The Ecu '{source.Key}' is defined in multiple locations.");
    }
    FireAndForget.Invoke((MulticastDelegate) this.EcusUpdateEvent, (object) this, new EventArgs());
  }

  internal void Remove(Ecu ecu) => this.Items.Remove(ecu);

  public Ecu AddDescriptionFile(string path)
  {
    try
    {
      IEnumerable<string> ecus = CaesarRoot.Ecus;
      CaesarRoot.AddCbfFile(path);
      Ecu ecu = new Ecu(CaesarRoot.Ecus.Except<string>(ecus).FirstOrDefault<string>(), string.Empty, DiagnosisSource.CaesarDatabase);
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Adding CBF: {0}", (object) ecu.Name));
      ecu.AcquireInfo();
      this.Items.Add(ecu);
      FireAndForget.Invoke((MulticastDelegate) this.EcusUpdateEvent, (object) this, new EventArgs());
      return ecu;
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      throw new CaesarException(ex, negativeResponseCode);
    }
  }

  internal void ClearList()
  {
    if (this.Acquired)
    {
      string cbfPath = Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value;
      foreach (string str in (IEnumerable<string>) this.Items.Select<Ecu, string>((Func<Ecu, string>) (e => Path.Combine(cbfPath, e.DescriptionFileName))).ToList<string>())
        CaesarRoot.RemoveCbfFile(str);
    }
    this.Items.Clear();
    this.ResetList();
    FireAndForget.Invoke((MulticastDelegate) this.EcusUpdateEvent, (object) this, new EventArgs());
  }

  internal DiagnosisVariant GetDiagnosisVariantFromIDBlock(object identifier, CaesarIdBlock idBlock)
  {
    foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) this)
    {
      if (object.Equals(identifier, (object) ecu.Identifier))
      {
        DiagnosisVariant variantFromIdBlock = ecu.GetDiagnosisVariantFromIDBlock(idBlock);
        if (variantFromIdBlock != null)
          return variantFromIdBlock;
      }
    }
    return (DiagnosisVariant) null;
  }

  internal DiagnosisVariant GetDiagnosisVariantFromIDBlock(object identifier, McdLogicalLink link)
  {
    foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) this)
    {
      if (object.Equals(identifier, (object) ecu.Identifier))
      {
        DiagnosisVariant variantFromIdBlock = ecu.DiagnosisVariants.FirstOrDefault<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => v.IsMatch(link)));
        if (variantFromIdBlock != null)
          return variantFromIdBlock;
      }
    }
    return (DiagnosisVariant) null;
  }

  internal Ecu GetBestEcuFromVariant(string identifier, Predicate<DiagnosisVariant> predicate)
  {
    IEnumerable<\u003C\u003Ef__AnonymousType9<Ecu, int>> source = this.Where<Ecu>((Func<Ecu, bool>) (e => e.Identifier == identifier)).Select(ecu => new
    {
      ecu = ecu,
      matchCount = ecu.DiagnosisVariants.Count<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => predicate(v)))
    }).Where(_param1 => _param1.matchCount > 0).Select(_param1 => new
    {
      Ecu = _param1.ecu,
      Count = _param1.matchCount
    });
    return !source.Any() ? (Ecu) null : source.OrderBy(r => r.Count).Last().Ecu;
  }

  public Ecu this[string name]
  {
    get
    {
      return this.FirstOrDefault<Ecu>((Func<Ecu, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal)));
    }
  }

  public bool IgnoreDiagnosisDescriptionLoadFailure { get; set; }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ConnectedCountForIdentifier is deprecated, please use GetConnectedCountForIdentifier(string) instead.")]
  public int get_ConnectedCountForIdentifier(string identifier)
  {
    return this.GetConnectedCountForIdentifier(identifier);
  }

  public int GetConnectedCountForIdentifier(string identifier)
  {
    return this.Where<Ecu>((Func<Ecu, bool>) (ecu => identifier == ecu.Identifier)).Sum<Ecu>((Func<Ecu, int>) (ecu => ecu.ConnectedChannelCount));
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_MarkedForAutoConnect is deprecated, please use GetMarkedForAutoConnect(string) instead.")]
  public bool get_MarkedForAutoConnect(string identifier)
  {
    return this.GetMarkedForAutoConnect(identifier);
  }

  public bool GetMarkedForAutoConnect(string identifier)
  {
    List<Ecu> list = this.Where<Ecu>((Func<Ecu, bool>) (ecu => object.Equals((object) identifier, (object) ecu.Identifier))).ToList<Ecu>();
    if (list.Count == 0)
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Identifier {0} is not assigned to any Ecus", (object) identifier));
    return !list.Any<Ecu>((Func<Ecu, bool>) (ecu => !ecu.MarkedForAutoConnect));
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("set_MarkedForAutoConnect is deprecated, please use SetMarkedForAutoConnect(string, bool) instead.")]
  public void set_MarkedForAutoConnect(string identifier, bool marked)
  {
    this.SetMarkedForAutoConnect(identifier, marked);
  }

  public void SetMarkedForAutoConnect(string identifier, bool marked)
  {
    List<Ecu> list = this.Where<Ecu>((Func<Ecu, bool>) (ecu => object.Equals((object) identifier, (object) ecu.Identifier))).ToList<Ecu>();
    if (list.Count == 0)
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Identifier {0} is not assigned to any Ecus", (object) identifier));
    foreach (Ecu ecu in (IEnumerable<Ecu>) list)
      ecu.MarkedForAutoConnect = marked;
  }

  public event EcusUpdateEventHandler EcusUpdateEvent;

  private class CbfVersionInfo
  {
    private static Regex regexGPD = new Regex("\\(\\(VERSION (?<ver>\\d+.\\d+.\\d+), (?<date>\\d+.\\d+.\\d+),", RegexOptions.Compiled);
    public readonly string EcuQualifier;
    public readonly string CbfPath;
    public readonly Version DescriptionVersion;
    public readonly bool Is64BitCompatible;

    public CbfVersionInfo(string cbfFile)
    {
      CaesarRoot.AddCbfFile(cbfFile);
      IEnumerable<string> ecus = CaesarRoot.Ecus;
      if (!ecus.Any<string>())
        throw new CaesarException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version number of CBF {0} could not be determined because the file could not be loaded into CAESAR. the file may be corrupt.", (object) Path.GetFileName(cbfFile)));
      using (CaesarEcu caesarEcu = Ecu.OpenEcuHandle(ecus.Single<string>()))
      {
        this.Is64BitCompatible = false;
        this.EcuQualifier = caesarEcu.Name;
        this.CbfPath = caesarEcu.FileName;
        this.DescriptionVersion = EcuCollection.CbfVersionInfo.GetCBFVersion(cbfFile, caesarEcu.DescriptionDataVersion);
        Match match = EcuCollection.CbfVersionInfo.regexGPD.Match(caesarEcu.GpdVersion);
        if (match.Success)
        {
          Version result;
          if (Version.TryParse(match.Groups["ver"].Value, out result))
            this.Is64BitCompatible = result.Major >= 4;
        }
      }
      CaesarRoot.RemoveCbfFile(cbfFile);
    }

    public bool HasDiagjobs
    {
      get
      {
        bool hasDiagjobs = false;
        CaesarRoot.AddCbfFile(this.CbfPath);
        IEnumerable<string> ecus = CaesarRoot.Ecus;
        if (ecus.Any<string>())
        {
          using (CaesarEcu caesarEcu = Ecu.OpenEcuHandle(ecus.Single<string>()))
          {
            foreach (string variant in caesarEcu.Variants)
            {
              if (caesarEcu.SetVariant(variant) && caesarEcu.GetServices((ServiceTypes) 262144 /*0x040000*/).Count > 0)
              {
                hasDiagjobs = true;
                break;
              }
            }
          }
        }
        CaesarRoot.RemoveCbfFile(this.CbfPath);
        return hasDiagjobs;
      }
    }

    private static Version GetCBFVersion(string cbffilename, string version)
    {
      try
      {
        return new Version(version);
      }
      catch (FormatException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) cbffilename, "Version cannot be parsed: " + ex.Message);
      }
      catch (ArgumentException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) cbffilename, "Version cannot be parsed: " + ex.Message);
      }
      catch (OverflowException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) cbffilename, "Version cannot be parsed: " + ex.Message);
      }
      return new Version();
    }
  }
}
