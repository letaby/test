// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuInfoCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;

#nullable disable
namespace SapiLayer1;

public sealed class EcuInfoCollection : LateLoadReadOnlyCollection<EcuInfo>
{
  private Dictionary<string, EcuInfo> cache;
  private bool autoRead;
  private Channel channel;

  internal EcuInfoCollection(Channel c)
  {
    this.channel = c;
    this.autoRead = true;
  }

  internal bool InternalRead(EcuInfoInternalReadType readType)
  {
    bool flag = false;
    for (int index = 0; index < this.Count && !this.channel.Closing && this.channel.ChannelRunning; ++index)
    {
      EcuInfo ecuInfo = this[index];
      if (ecuInfo.Marked && (readType != EcuInfoInternalReadType.CyclicRead || ecuInfo.NeedsUpdate))
      {
        ecuInfo.InternalRead(false);
        flag = true;
        if (readType != EcuInfoInternalReadType.ExplicitRead)
          ecuInfo.LastCyclicAttemptTime = Sapi.Now;
      }
    }
    if (readType != EcuInfoInternalReadType.CyclicRead)
      FireAndForget.Invoke((MulticastDelegate) this.EcuInfosReadCompleteEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) null));
    if (readType == EcuInfoInternalReadType.ExplicitRead)
    {
      if (!this.channel.Closing)
        this.channel.SetCommunicationsState(CommunicationsState.Online);
      this.channel.SyncDone((Exception) null);
    }
    return flag;
  }

  internal void InternalRead(bool explicitread)
  {
    if (!this.channel.Closing && this.channel.ChannelRunning && !this.channel.IsRollCall)
      this.channel.ClearCache();
    this.InternalRead(explicitread ? EcuInfoInternalReadType.ExplicitRead : EcuInfoInternalReadType.ImplicitRead);
  }

  internal void AddFromRollCall(EcuInfo ecuInfo)
  {
    this.Items.Add(ecuInfo);
    if (this.cache == null)
      return;
    this.cache.Add(ecuInfo.Qualifier, ecuInfo);
  }

  protected override void AcquireList()
  {
    if (this.channel.Ecu.RollCallManager != null)
    {
      this.channel.Ecu.RollCallManager.CreateEcuInfos(this);
    }
    else
    {
      if (!this.channel.Ecu.IsMcd)
      {
        if (this.channel.Online)
        {
          if (this.channel.ChannelHandle != null)
          {
            CaesarIdBlock idBlock = this.channel.ChannelHandle.IdBlock;
            if (idBlock.PartNumber != null)
              this.Items.Add(new EcuInfo(this.channel, EcuInfoType.IdBlock, "MBNumber", "MB Number", "Common", "Common/ID Block", string.Empty, (string) null, true));
            if (idBlock.SoftwareVersion.HasValue)
              this.Items.Add(new EcuInfo(this.channel, EcuInfoType.IdBlock, "SWVersionNumber", "Software Version", "Common", "Common/ID Block", string.Empty, (string) null, true));
            if (idBlock.DiagVersion.HasValue)
              this.Items.Add(new EcuInfo(this.channel, EcuInfoType.IdBlock, "DiagVersion", "Diagnostic Version", "Common", "Common/ID Block", string.Empty, (string) null, true));
            if (this.channel.IsChannelErrorSet)
            {
              CaesarException e = new CaesarException(this.channel.ChannelHandle);
              Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
            }
          }
        }
        else
        {
          this.Items.Add(new EcuInfo(this.channel, EcuInfoType.IdBlock, "MBNumber", "MB Number", "Common", "Common/ID Block", string.Empty, (string) null, false));
          this.Items.Add(new EcuInfo(this.channel, EcuInfoType.IdBlock, "SWVersionNumber", "Software Version", "Common", "Common/ID Block", string.Empty, (string) null, false));
          this.Items.Add(new EcuInfo(this.channel, EcuInfoType.IdBlock, "DiagVersion", "Diagnostic Version", "Common", "Common/ID Block", string.Empty, (string) null, false));
        }
      }
      this.Items.Add(new EcuInfo(this.channel, EcuInfoType.DiagnosisVariant, "DiagnosisVariant", "Diagnostic Variant", "Common", "Common/Diagnostic Variant", string.Empty, (string) null, true));
      if (this.channel.DiagnosisVariant.PartNumber != null)
        this.Items.Add(new EcuInfo(this.channel, EcuInfoType.DiagnosisVariant, "DiagnosisVariantPartNumber", "Diagnostic Variant Part Number", "Common", "Common/Diagnostic Variant", string.Empty, (string) null, true));
      IEnumerable<EcuInfo> list = (IEnumerable<EcuInfo>) new ServiceCollection(this.channel, ServiceTypes.StoredData).Where<Service>((Func<Service, bool>) (x => x.OutputValues.Count > 0)).Select<Service, EcuInfo>((Func<Service, EcuInfo>) (s => new EcuInfo(this.channel, s))).ToList<EcuInfo>();
      XmlNode xml = this.channel.Ecu.Xml;
      if (xml != null)
      {
        XmlNodeList xmlNodeList1 = xml.SelectNodes("Ecu/EcuInfos/EcuInfo");
        if (xmlNodeList1 != null)
        {
          foreach (XmlNode xmlNode1 in xmlNodeList1)
          {
            string innerText1 = xmlNode1.Attributes.GetNamedItem("Qualifier").InnerText;
            string innerText2 = xmlNode1.Attributes.GetNamedItem("Name").InnerText;
            string innerText3 = xmlNode1.Attributes.GetNamedItem("GroupQualifier").InnerText;
            string innerText4 = xmlNode1.Attributes.GetNamedItem("GroupName").InnerText;
            string innerText5 = xmlNode1.Attributes.GetNamedItem("FormatString").InnerText;
            string description = string.Empty;
            XmlNode namedItem1 = xmlNode1.Attributes.GetNamedItem("Description");
            if (namedItem1 != null)
              description = namedItem1.InnerText;
            int? presentationIndex = new int?();
            XmlNode namedItem2 = xmlNode1.Attributes.GetNamedItem("PresentationIndex");
            if (namedItem2 != null)
              presentationIndex = new int?(Convert.ToInt32(namedItem2.InnerText, (IFormatProvider) CultureInfo.InvariantCulture));
            bool flag = false;
            List<Tuple<EcuInfo, int?>> references = new List<Tuple<EcuInfo, int?>>();
            XmlNodeList xmlNodeList2 = xmlNode1.SelectNodes("Reference");
            foreach (XmlNode xmlNode2 in xmlNodeList2)
            {
              string[] strArray = xmlNode2.Attributes.GetNamedItem("Qualifier").InnerText.Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
              string serviceQualifier = strArray[0];
              int? nullable = strArray.Length > 1 ? new int?(int.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture)) : new int?();
              string alternateServiceQualifier;
              bool alternateAvailable = this.channel.Ecu.AlternateQualifiers.TryGetValue(serviceQualifier, out alternateServiceQualifier);
              EcuInfo ecuInfo = list.FirstOrDefault<EcuInfo>((Func<EcuInfo, bool>) (ei =>
              {
                if (ei.Qualifier == serviceQualifier)
                  return true;
                return alternateAvailable && ei.Qualifier == alternateServiceQualifier;
              }));
              references.Add(Tuple.Create<EcuInfo, int?>(ecuInfo, nullable));
              if (ecuInfo != null)
                flag = true;
            }
            if (flag || xmlNodeList2.Count == 0)
              this.Items.Add(new EcuInfo(this.channel, EcuInfoType.Compound, innerText1, innerText2, innerText3, innerText4, description, innerText5, true, references, presentationIndex));
          }
        }
      }
      foreach (EcuInfo ecuInfo in list)
        this.Items.Add(ecuInfo);
    }
  }

  internal void RaiseEcuInfoUpdateEvent(EcuInfo i, Exception e)
  {
    FireAndForget.Invoke((MulticastDelegate) this.EcuInfoUpdateEvent, (object) i, (EventArgs) new ResultEventArgs(e));
  }

  internal void UpdateFromRollCall(int id, byte[] data)
  {
    foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) this)
    {
      int? nullable = ecuInfo.MessageNumber;
      int num = id;
      if ((nullable.GetValueOrDefault() == num ? (nullable.HasValue ? 1 : 0) : 0) != 0 && ecuInfo.Presentation != null)
      {
        nullable = ecuInfo.Presentation.BytePosition;
        if (nullable.HasValue)
        {
          try
          {
            ecuInfo.UpdateFromRollCall(ecuInfo.Presentation.GetPresentation(data));
          }
          catch (CaesarException ex)
          {
            ecuInfo.RaiseEcuInfoUpdateEvent((Exception) ex, false);
          }
        }
      }
    }
  }

  public Channel Channel => this.channel;

  public EcuInfo this[string qualifier]
  {
    get
    {
      if (qualifier == null)
        return (EcuInfo) null;
      qualifier = qualifier.RemoveArguments();
      if (this.cache == null)
        this.cache = this.ToDictionary((Func<EcuInfo, string>) (e => e.Qualifier), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      EcuInfo ecuInfo = (EcuInfo) null;
      string key;
      if (!this.cache.TryGetValue(qualifier, out ecuInfo) && this.channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out key))
        this.cache.TryGetValue(key, out ecuInfo);
      return ecuInfo;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ItemContaining is deprecated, please use GetItemContaining(string) instead.")]
  public EcuInfo get_ItemContaining(string index) => this.GetItemContaining(index);

  public EcuInfo GetItemContaining(string index)
  {
    if (index == null)
      return (EcuInfo) null;
    index = index.RemoveArguments();
    string alternateIndex;
    bool alternateAvailable = this.channel.Ecu.AlternateQualifiers.TryGetValue(index, out alternateIndex);
    return this.Where<EcuInfo>((Func<EcuInfo, bool>) (ecuInfo => ecuInfo.Services != null && ecuInfo.Services.Any<Service>((Func<Service, bool>) (service =>
    {
      if (!(service != (Service) null))
        return false;
      if (service.Qualifier.CompareNoCase(index))
        return true;
      return alternateAvailable && service.Qualifier.CompareNoCase(alternateIndex);
    })))).FirstOrDefault<EcuInfo>();
  }

  public bool AutoRead
  {
    get => this.autoRead;
    set => this.autoRead = value;
  }

  public void Read(bool synchronous)
  {
    this.channel.QueueAction((object) CommunicationsState.ReadEcuInfo, synchronous);
  }

  public event EcuInfoUpdateEventHandler EcuInfoUpdateEvent;

  public event EcuInfosReadCompleteEventHandler EcuInfosReadCompleteEvent;
}
