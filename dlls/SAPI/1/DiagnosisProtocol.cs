// Decompiled with JetBrains decompiler
// Type: SapiLayer1.DiagnosisProtocol
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class DiagnosisProtocol
{
  private Dictionary<string, TranslationEntry> translation;
  private string name;
  private object protocolComParameterMapLock = new object();
  private ListDictionary comParameters;
  private bool isMcd;

  internal Dictionary<string, TranslationEntry> Translations => this.translation;

  public bool IsMcd => this.isMcd;

  internal DiagnosisProtocol(string name, bool isMcd = false)
  {
    this.name = name;
    this.isMcd = isMcd;
    CultureInfo culture = Sapi.GetSapi().PresentationCulture;
    this.GetTranslationFileName(culture);
    if (DiagnosisProtocol.IsTranslationNecessary(culture) && !this.IsTranslationFilePresent(culture))
      culture = DiagnosisProtocol.OriginalCulture;
    if (!this.IsTranslationFilePresent(culture))
      return;
    this.translation = this.ReadTranslationFile(culture).Reverse<TranslationEntry>().DistinctBy<TranslationEntry, string>((Func<TranslationEntry, string>) (e => e.Qualifier)).ToDictionary<TranslationEntry, string>((Func<TranslationEntry, string>) (item => item.Qualifier));
  }

  public override string ToString() => this.name;

  public string Name => this.name;

  public ConnectionResourceCollection GetConnectionResources(byte sourceAddress)
  {
    return this.GetConnectionResources(sourceAddress, new long[3]
    {
      250000L,
      500000L,
      666666L
    });
  }

  public ConnectionResourceCollection GetConnectionResources(byte sourceAddress, long[] baudRates)
  {
    switch (Sapi.GetSapi().InitState)
    {
      case InitState.NotInitialized:
        throw new InvalidOperationException("Sapi not initialized");
      case InitState.Online:
        ConnectionResourceCollection connectionResources = new ConnectionResourceCollection();
        if (this.isMcd)
          this.PopulateMcdConnectionResources(connectionResources, sourceAddress, baudRates);
        else
          this.PopulateCaesarConnectionResources(connectionResources, sourceAddress, baudRates);
        return connectionResources;
      case InitState.Offline:
        throw new InvalidOperationException("Resource cannot be acquired in offline operation mode");
      default:
        return (ConnectionResourceCollection) null;
    }
  }

  private void PopulateCaesarConnectionResources(
    ConnectionResourceCollection connectionResources,
    byte sourceAddress,
    long[] baudRates)
  {
    lock (Ecu.ResourceLock)
    {
      try
      {
        CaesarRoot.LockResources();
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
      uint protocolResourceCount;
      try
      {
        protocolResourceCount = CaesarRoot.GetAvailableProtocolResourceCount(this.name);
      }
      catch (CaesarErrorException ex)
      {
        CaesarRoot.UnlockResources();
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
      for (ushort index = 0; (uint) index < protocolResourceCount; ++index)
      {
        CaesarResource protocolResource;
        try
        {
          protocolResource = CaesarRoot.GetAvailableProtocolResource(this.name, index);
        }
        catch (CaesarErrorException ex)
        {
          CaesarRoot.UnlockResources();
          byte? negativeResponseCode = new byte?();
          throw new CaesarException(ex, negativeResponseCode);
        }
        foreach (long desiredBaudRate in !protocolResource.IsPassThru || RollCallJ1939.GlobalInstance == null || !RollCallJ1939.GlobalInstance.IsAutoBaudRate || !Sapi.GetSapi().AllowAutoBaudRate ? baudRates : new long[1])
          connectionResources.Add(new ConnectionResource(this, protocolResource, sourceAddress, (uint) desiredBaudRate));
      }
      CaesarRoot.UnlockResources();
    }
  }

  private void PopulateMcdConnectionResources(
    ConnectionResourceCollection connectionResources,
    byte sourceAddress,
    long[] baudRates)
  {
    IEnumerable<McdDBLogicalLink> linksForProtocol = McdRoot.GetDBLogicalLinksForProtocol(this.name);
    Dictionary<string, int> dictionary = new Dictionary<string, int>();
    foreach (McdInterface currentInterface in McdRoot.CurrentInterfaces)
    {
      foreach (McdInterfaceResource resource in currentInterface.Resources)
      {
        McdInterfaceResource theInterfaceResource = resource;
        if (linksForProtocol.Any<McdDBLogicalLink>((Func<McdDBLogicalLink, bool>) (ll => ll.ProtocolType == theInterfaceResource.ProtocolType)))
        {
          int portIndex;
          if (!dictionary.TryGetValue(theInterfaceResource.ProtocolType, out portIndex))
            dictionary.Add(theInterfaceResource.ProtocolType, 0);
          portIndex = ++dictionary[theInterfaceResource.ProtocolType];
          foreach (long desiredBaudRate in RollCallJ1939.GlobalInstance == null || !RollCallJ1939.GlobalInstance.IsAutoBaudRate || !Sapi.GetSapi().AllowAutoBaudRate ? baudRates : new long[1])
          {
            ConnectionResource connectionResource = new ConnectionResource(this, currentInterface, theInterfaceResource, portIndex, sourceAddress, (uint) desiredBaudRate);
            connectionResources.Add(connectionResource);
          }
        }
      }
    }
  }

  public ListDictionary ComParameters
  {
    get
    {
      lock (this.protocolComParameterMapLock)
      {
        if (this.comParameters == null)
        {
          if (this.isMcd)
          {
            this.comParameters = new ListDictionary();
            foreach (McdDBRequestParameter comParameter in McdRoot.GetDBProtocolLocation(this.name).GetComParameters())
            {
              McdValue defaultValue = comParameter.GetDefaultValue();
              if (defaultValue != null && defaultValue.Value != null)
                this.comParameters[(object) comParameter.Qualifier] = defaultValue.GetValue(defaultValue.Value.GetType(), (ChoiceCollection) null);
            }
          }
          else
          {
            using (CaesarProtocol protocol = CaesarRoot.GetProtocol(this.Name))
            {
              if (protocol != null)
                this.comParameters = protocol.ComParameters;
            }
          }
        }
        return this.comParameters;
      }
    }
  }

  public string DescriptionFileName
  {
    get
    {
      if (!this.IsMcd)
        return (string) null;
      return McdRoot.GetDBProtocolLocation(this.name)?.DatabaseFile;
    }
  }

  public string DescriptionDataVersion => McdRoot.GetDatabaseFileVersion(this.DescriptionFileName);

  private string GetTranslationFileName(CultureInfo culture)
  {
    return TranslationEntry.GetTranslationFileName(this.name, culture);
  }

  public IEnumerable<TranslationEntry> ReadTranslationFile(CultureInfo culture)
  {
    return TranslationEntry.ReadTranslationFile(this.name, culture);
  }

  public bool IsTranslationFilePresent(CultureInfo culture)
  {
    return File.Exists(this.GetTranslationFileName(culture));
  }

  internal string Translate(string qualifier, string original)
  {
    TranslationEntry translationEntry;
    return this.translation != null && this.translation.TryGetValue(qualifier, out translationEntry) ? translationEntry.Translation : original;
  }

  public static bool IsTranslationNecessary(CultureInfo culture)
  {
    return !DiagnosisProtocol.OriginalCulture.Neutralize().Name.Equals(culture.Neutralize().Name);
  }

  public static CultureInfo OriginalCulture => CultureInfo.GetCultureInfo("en-US");

  public string GetServiceIdentifierDescription(byte serviceIdentifier)
  {
    return this.Translate(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:X2}.ServiceIdentifier", (object) serviceIdentifier), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SID {0:X2}h", (object) serviceIdentifier));
  }

  public string GetNegativeResponseCodeDescription(byte negativeResponseCode)
  {
    return this.Translate(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:X2}.NegativeResponseCode", (object) negativeResponseCode), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NRC {0:X2}h", (object) negativeResponseCode));
  }
}
