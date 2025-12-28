// Decompiled with JetBrains decompiler
// Type: SapiLayer1.InstrumentCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

#nullable disable
namespace SapiLayer1;

public sealed class InstrumentCollection : LateLoadReadOnlyCollection<Instrument>, IDisposable
{
  private const int DefaultInvalidationTimeout = 10000;
  private Dictionary<int, List<Instrument>> rollCallInstrumentsById;
  private List<KeyValuePair<string, List<Instrument>>> instrumentsByRequest;
  private List<KeyValuePair<string, List<Instrument>>> instrumentsByForcedRequest;
  private bool instrumentRequestListDirty;
  private static object userForceRequestListLock = new object();
  private static IEnumerable<Tuple<Ecu, string>> userForceRequestList = (IEnumerable<Tuple<Ecu, string>>) new List<Tuple<Ecu, string>>();
  private Dictionary<string, DateTime> lastRequests = new Dictionary<string, DateTime>();
  private readonly TimeSpan channelTimeout;
  private Dictionary<string, Instrument> cache;
  private Dictionary<string, Instrument> referenceCache;
  private Channel channel;
  private bool autoRead;
  private bool updateOnEveryRead;
  private ManualResetEvent closingEvent;
  private ManualResetEvent queueNonEmptyEvent;
  private Queue updatedInstruments;
  private bool disposed;
  private int nextInstrumentToRead;

  internal InstrumentCollection(Channel c)
  {
    this.channel = c;
    this.updatedInstruments = new Queue();
    this.closingEvent = new ManualResetEvent(false);
    this.queueNonEmptyEvent = new ManualResetEvent(false);
    this.channelTimeout = TimeSpan.FromMilliseconds(this.channel.IsRollCall ? (double) this.channel.Ecu.RollCallManager.ChannelTimeout : 10000.0);
    this.instrumentsByForcedRequest = this.instrumentsByRequest = new List<KeyValuePair<string, List<Instrument>>>();
  }

  protected override void AcquireList()
  {
    if (this.channel.Ecu.RollCallManager != null)
    {
      foreach (Instrument instrument in this.channel.Ecu.RollCallManager.CreateInstruments(this.channel))
        this.Items.Add(instrument);
      this.rollCallInstrumentsById = this.GroupBy<Instrument, int>((Func<Instrument, int>) (i => i.MessageNumber.Value)).ToDictionary<IGrouping<int, Instrument>, int, List<Instrument>>((Func<IGrouping<int, Instrument>, int>) (k => k.Key), (Func<IGrouping<int, Instrument>, List<Instrument>>) (v => v.ToList<Instrument>()));
      this.instrumentsByRequest = this.Where<Instrument>((Func<Instrument, bool>) (i => !i.Periodic)).GroupBy<Instrument, Dump>((Func<Instrument, Dump>) (i => i.RequestMessage)).ToDictionary<IGrouping<Dump, Instrument>, string, List<Instrument>>((Func<IGrouping<Dump, Instrument>, string>) (k => k.Key.ToString()), (Func<IGrouping<Dump, Instrument>, List<Instrument>>) (v => v.ToList<Instrument>())).ToList<KeyValuePair<string, List<Instrument>>>();
    }
    else
    {
      if (this.channel.EcuHandle != null)
      {
        StringCollection services = this.channel.EcuHandle.GetServices((ServiceTypes) 16 /*0x10*/);
        for (int index = 0; index < services.Count; ++index)
        {
          string qualifier = services[index];
          if (!this.channel.Ecu.IgnoreQualifier(qualifier) && !this.channel.Ecu.MakeStoredQualifier(qualifier))
          {
            Instrument instrument = new Instrument(this.channel, qualifier);
            instrument.Acquire();
            this.Items.Add(instrument);
          }
        }
        foreach (string service in this.channel.EcuHandle.GetServices((ServiceTypes) 2097152 /*0x200000*/))
        {
          if (this.channel.Ecu.MakeInstrumentQualifier(service) && !this.channel.Ecu.IgnoreQualifier(service))
          {
            Instrument instrument = new Instrument(this.channel, service);
            instrument.Acquire();
            this.Items.Add(instrument);
          }
        }
      }
      else if (this.channel.McdEcuHandle != null)
      {
        foreach (IGrouping<ServiceTypes, Tuple<Service, ServiceOutputValue>> grouping in this.channel.CaesarEquivalentServices.OrderBy<Tuple<Service, ServiceOutputValue>, string>((Func<Tuple<Service, ServiceOutputValue>, string>) (s => s.Item1.Qualifier)).GroupBy<Tuple<Service, ServiceOutputValue>, ServiceTypes>((Func<Tuple<Service, ServiceOutputValue>, ServiceTypes>) (s => s.Item1.ServiceTypes)))
        {
          ServiceTypes key = grouping.Key;
          if ((key & ServiceTypes.StoredData) != ServiceTypes.None || (key & ServiceTypes.Data) != ServiceTypes.None)
          {
            foreach (Tuple<Service, ServiceOutputValue> tuple in (IEnumerable<Tuple<Service, ServiceOutputValue>>) grouping)
            {
              if (tuple.Item2 != null)
              {
                string serviceQualifier = tuple.Item2.SingleServiceQualifier;
                if ((key & ServiceTypes.Data) != ServiceTypes.None)
                {
                  if (this.channel.Ecu.IgnoreQualifier(serviceQualifier) || this.channel.Ecu.MakeStoredQualifier(serviceQualifier))
                    continue;
                }
                else if ((key & ServiceTypes.StoredData) != ServiceTypes.None && (!this.channel.Ecu.MakeInstrumentQualifier(serviceQualifier) || this.channel.Ecu.IgnoreQualifier(serviceQualifier)))
                  continue;
                Instrument instrument = new Instrument(this.channel, serviceQualifier, (ushort) tuple.Item2.Index);
                instrument.Acquire(tuple.Item2.SingleServiceName, tuple.Item1, tuple.Item2);
                this.Items.Add(instrument);
              }
            }
          }
        }
      }
      XmlNode xml = this.channel.Ecu.Xml;
      if (xml != null)
      {
        XmlNodeList xmlNodeList1 = xml.SelectNodes("Ecu/Instruments/Instrument[@ReplaceQualifier]");
        if (xmlNodeList1 != null)
        {
          for (int index1 = 0; index1 < xmlNodeList1.Count; ++index1)
          {
            XmlNode xmlNode = xmlNodeList1.Item(index1);
            string innerText1 = xmlNode.Attributes.GetNamedItem("Qualifier").InnerText;
            string innerText2 = xmlNode.Attributes.GetNamedItem("ReplaceQualifier").InnerText;
            XmlNode namedItem = xmlNode.Attributes.GetNamedItem("CheckBitLength");
            bool result;
            if (namedItem == null || !bool.TryParse(namedItem.InnerText, out result))
              result = true;
            if (!string.IsNullOrEmpty(this.channel.Ecu.NameSplit))
            {
              for (int index2 = 0; index2 < this.Count; ++index2)
              {
                Instrument instrument = this[index2];
                if (instrument.Qualifier.StartsWith(innerText1, StringComparison.Ordinal))
                {
                  Instrument instrument2 = this.Matching(innerText2, instrument);
                  if (instrument2 != (Instrument) null)
                  {
                    if (!result || object.Equals((object) instrument.BitLength, (object) instrument2.BitLength))
                    {
                      this.Replace(instrument, instrument2);
                      --index2;
                    }
                    else
                      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Not replacing {0} with {1} because the bit lengths are different", (object) instrument.Qualifier, (object) instrument2.Qualifier));
                  }
                }
              }
            }
            else
            {
              Instrument instrument1 = this[innerText1];
              if (instrument1 != (Instrument) null)
              {
                Instrument instrument2 = this[innerText2];
                if (instrument2 != (Instrument) null)
                  this.Replace(instrument1, instrument2);
                else if (string.IsNullOrEmpty(innerText2))
                  this.Items.Remove(instrument1);
                else
                  Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find .EcuInfo instrument replace qualifier: {0}", (object) innerText2));
              }
              else
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find .EcuInfo instrument qualifier: {0}", (object) innerText1));
            }
          }
        }
        XmlNodeList xmlNodeList2 = xml.SelectNodes("Ecu/ServicesAsInstruments/ServiceAsInstrument");
        if (xmlNodeList2 != null)
        {
          for (int index = 0; index < xmlNodeList2.Count; ++index)
          {
            XmlNode xmlNode = xmlNodeList2.Item(index);
            string innerText3 = xmlNode.Attributes.GetNamedItem("Qualifier").InnerText;
            string innerText4 = xmlNode.Attributes.GetNamedItem("Name").InnerText;
            string innerText5 = xmlNode.Attributes.GetNamedItem("ReferenceQualifier").InnerText;
            XmlNode namedItem1 = xmlNode.Attributes.GetNamedItem("Hide");
            bool hide = namedItem1 != null && Convert.ToBoolean(namedItem1.InnerText, (IFormatProvider) CultureInfo.InvariantCulture);
            XmlNode namedItem2 = xmlNode.Attributes.GetNamedItem("Marked");
            bool marked = namedItem2 == null || Convert.ToBoolean(namedItem2.InnerText, (IFormatProvider) CultureInfo.InvariantCulture);
            XmlNode namedItem3 = xmlNode.Attributes.GetNamedItem("IsRegex");
            if ((namedItem3 != null ? (Convert.ToBoolean(namedItem3.InnerText, (IFormatProvider) CultureInfo.InvariantCulture) ? 1 : 0) : 0) != 0)
            {
              Regex regex = new Regex(innerText5, RegexOptions.Compiled);
              int num = 0;
              foreach (Service service in (ReadOnlyCollection<Service>) this.channel.Services)
              {
                Match match = regex.Match(service.Qualifier);
                if (match.Success && match.Groups.Count > 1)
                {
                  string[] array = match.Groups.OfType<Group>().Select<Group, string>((Func<Group, string>) (g => g.Value)).Skip<string>(1).ToArray<string>();
                  this.AddServiceAsInstrument(string.Format((IFormatProvider) CultureInfo.InvariantCulture, innerText3, (object[]) array), string.Format((IFormatProvider) CultureInfo.InvariantCulture, innerText4, (object[]) array), service, (string) null, hide, marked);
                  ++num;
                }
              }
              if (num == 0)
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not match any services as instrument for reference {0}", (object) innerText5));
            }
            else
            {
              Service service = this.channel.Services[innerText5];
              if (service != (Service) null)
                this.AddServiceAsInstrument(innerText3, innerText4, service, innerText5, hide, marked);
              else
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find service as instrument {0} reference {1}", (object) innerText4, (object) innerText5));
            }
          }
        }
      }
      this.UpdateInstrumentRequestList();
    }
  }

  private void AddServiceAsInstrument(
    string qualifier,
    string name,
    Service service,
    string definedInputValues,
    bool hide,
    bool marked)
  {
    ServiceInputValueCollection userValues = new ServiceInputValueCollection(service);
    for (int index = 0; index < service.InputValues.Count; ++index)
    {
      ServiceInputValue serviceInputValue = service.InputValues[index].Clone();
      userValues.Add(serviceInputValue);
    }
    Exception values = definedInputValues != null ? userValues.InternalParseValues(definedInputValues) : (Exception) null;
    if (values != null)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) service, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not set service as instrument input value: {0} {1}", (object) service.Name, (object) values.Message));
    }
    else
    {
      Instrument instrument = new Instrument(this.channel, service.Qualifier, qualifier, name, userValues, hide, marked);
      if (this.channel.McdEcuHandle != null)
        instrument.Acquire(name, service, service.OutputValues[0]);
      else
        instrument.Acquire();
      this.Items.Add(instrument);
    }
  }

  private void UpdateInstrumentRequestList() => this.instrumentRequestListDirty = true;

  private void InternalUpdateInstrumentRequestList()
  {
    IEnumerable<Tuple<Ecu, string>> userForceRequestListCopy = (IEnumerable<Tuple<Ecu, string>>) null;
    lock (InstrumentCollection.userForceRequestListLock)
      userForceRequestListCopy = InstrumentCollection.userForceRequestList;
    IEnumerable<IGrouping<bool, KeyValuePair<string, List<Instrument>>>> groupings = this.Where<Instrument>((Func<Instrument, bool>) (i => !i.Periodic && Sapi.GetSapi().HardwareAccess >= i.AccessLevel)).GroupBy<Instrument, string>((Func<Instrument, string>) (i => i.RequestMessage.ToString())).ToDictionary<IGrouping<string, Instrument>, string, List<Instrument>>((Func<IGrouping<string, Instrument>, string>) (k => k.Key.ToString()), (Func<IGrouping<string, Instrument>, List<Instrument>>) (v => v.ToList<Instrument>())).ToList<KeyValuePair<string, List<Instrument>>>().GroupBy<KeyValuePair<string, List<Instrument>>, bool>((Func<KeyValuePair<string, List<Instrument>>, bool>) (v => v.Value.Any<Instrument>((Func<Instrument, bool>) (i => i.ForceRequest || userForceRequestListCopy.Any<Tuple<Ecu, string>>((Func<Tuple<Ecu, string>, bool>) (u => u.Item1 == i.Channel.Ecu && u.Item2 == i.Qualifier))))));
    this.instrumentsByForcedRequest = new List<KeyValuePair<string, List<Instrument>>>();
    this.instrumentsByRequest = new List<KeyValuePair<string, List<Instrument>>>();
    foreach (IGrouping<bool, KeyValuePair<string, List<Instrument>>> source in groupings)
    {
      if (source.Key)
        this.instrumentsByForcedRequest = source.ToList<KeyValuePair<string, List<Instrument>>>();
      else
        this.instrumentsByRequest = source.ToList<KeyValuePair<string, List<Instrument>>>();
    }
    this.instrumentRequestListDirty = false;
  }

  public static IEnumerable<string> ForceRequestSet
  {
    set
    {
      List<Tuple<Ecu, string>> tupleList = new List<Tuple<Ecu, string>>();
      if (value != null)
      {
        foreach (string str in value)
        {
          string[] strArray = str.Split(".".ToCharArray());
          if (strArray.Length > 1)
          {
            Ecu ecu = Sapi.GetSapi().Ecus[strArray[0]];
            if (ecu != null)
              tupleList.Add(new Tuple<Ecu, string>(ecu, strArray[1]));
          }
        }
      }
      lock (InstrumentCollection.userForceRequestListLock)
        InstrumentCollection.userForceRequestList = (IEnumerable<Tuple<Ecu, string>>) tupleList;
      foreach (Channel channel in Sapi.GetSapi().Channels.Where<Channel>((Func<Channel, bool>) (c => c.Online && !c.IsRollCall)))
        channel.Instruments.UpdateInstrumentRequestList();
    }
    get
    {
      lock (InstrumentCollection.userForceRequestListLock)
        return InstrumentCollection.userForceRequestList.Select<Tuple<Ecu, string>, string>((Func<Tuple<Ecu, string>, string>) (t => $"{t.Item1.Name}.{t.Item2}"));
    }
  }

  internal void UpdateFromRollCall(int id, byte? destinationAddress, byte[] data)
  {
    List<Instrument> instrumentList;
    if (this.Count <= 0 || this.rollCallInstrumentsById == null || !this.rollCallInstrumentsById.TryGetValue(id, out instrumentList))
      return;
    foreach (Instrument instrument in instrumentList)
    {
      if (instrument.IsTarget(destinationAddress, (IEnumerable<byte>) data))
        instrument.UpdateFromRollCall(data);
    }
  }

  internal bool InternalRead()
  {
    bool flag = false;
    if (this.instrumentRequestListDirty)
      this.InternalUpdateInstrumentRequestList();
    int instrumentToRead;
    for (instrumentToRead = this.nextInstrumentToRead; instrumentToRead < this.instrumentsByRequest.Count && this.ContinueReading && !this.channel.ActionWaiting && !this.instrumentRequestListDirty; ++instrumentToRead)
    {
      if (this.InternalReadMessageSet(this.instrumentsByRequest[instrumentToRead]))
        flag = true;
      foreach (KeyValuePair<string, List<Instrument>> messageset in this.instrumentsByForcedRequest)
      {
        if (this.ContinueReading && this.InternalReadMessageSet(messageset))
          flag = true;
      }
    }
    this.nextInstrumentToRead = instrumentToRead < this.instrumentsByRequest.Count ? instrumentToRead : 0;
    return flag;
  }

  private bool InternalReadMessageSet(KeyValuePair<string, List<Instrument>> messageset)
  {
    bool flag = false;
    IEnumerable<Instrument> source = messageset.Value.Where<Instrument>((Func<Instrument, bool>) (i => i.Marked));
    if (source.Any<Instrument>())
    {
      TimeSpan timeSpan = TimeSpan.FromMilliseconds(this.channel.IsRollCall ? (double) messageset.Value.Max<Instrument>((Func<Instrument, int>) (i => i.CacheTimeout)) : (double) messageset.Value.Min<Instrument>((Func<Instrument, int>) (i => i.CacheTimeout)));
      DateTime now = Sapi.Now;
      if (!this.lastRequests.ContainsKey(messageset.Key) || this.lastRequests[messageset.Key] + timeSpan < now)
      {
        this.lastRequests[messageset.Key] = now;
        foreach (Instrument instrument in this.channel.IsRollCall ? source.Take<Instrument>(1) : source)
        {
          instrument.InternalRead(false);
          flag = true;
          if (!this.ContinueReading)
            return flag;
        }
      }
    }
    return flag;
  }

  private bool ContinueReading
  {
    get
    {
      return this.channel.ChannelRunning && (this.channel.IsRollCall || !this.channel.IsChannelErrorSet) && !this.channel.Closing;
    }
  }

  internal void RaiseInstrumentUpdateEvent(Instrument i, Exception ex)
  {
    FireAndForget.Invoke((MulticastDelegate) this.InstrumentUpdateEvent, (object) i, (EventArgs) new ResultEventArgs(ex));
  }

  internal void RegisterPeriodicListeners(bool reg)
  {
    if (reg)
      new Thread(new ThreadStart(this.ThreadFunc)).Start();
    if (!this.channel.IsRollCall)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        Instrument instrument = this[index];
        if (instrument.Periodic)
        {
          if (this.channel.ChannelHandle.SetPresentationListener(instrument.Qualifier))
          {
            if (reg)
            {
              instrument.IndexTag = CaesarRoot.RegisterPeriodicCallback(this.channel.ChannelHandle, index);
              if (instrument.IndexTag == null)
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not register event callback for {0}", (object) instrument.Qualifier));
            }
            else if (instrument.IndexTag != null)
            {
              if (!CaesarRoot.UnregisterPeriodicCallback(this.channel.ChannelHandle, instrument.IndexTag))
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not unregister event callback for {0}", (object) instrument.Qualifier));
              ((CaesarHandle\u003Cvoid\u0020\u002A\u003E) instrument.IndexTag).Dispose();
              instrument.IndexTag = (CaesarAPtr) null;
            }
            else
              Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No item tag exists to unregister event callback for {0}", (object) instrument.Qualifier));
          }
          else
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not set presentation listener for {0} reg={1}", (object) instrument.Qualifier, (object) reg));
        }
      }
    }
    if (reg)
      return;
    this.closingEvent.Set();
  }

  internal void PeriodicCallback(int index, CaesarDiagServiceIO diagServiceIO)
  {
    if (index >= 0 && index < this.Count)
    {
      Instrument instrument = this[index];
      if (!instrument.GetPresentation((object) diagServiceIO, false))
        return;
      Queue queue = Queue.Synchronized(this.updatedInstruments);
      if (queue.Contains((object) instrument))
        return;
      queue.Enqueue((object) instrument);
      this.queueNonEmptyEvent.Set();
    }
    else
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got periodic callback for an unknown instrument (index {0})", (object) index));
  }

  internal void Invalidate()
  {
    foreach (Instrument instrument in (ReadOnlyCollection<Instrument>) this)
    {
      instrument.InstrumentValues.Invalidate();
      instrument.ClearDiagServiceIO();
    }
  }

  internal void AgeAll()
  {
    foreach (Instrument instrument in (ReadOnlyCollection<Instrument>) this)
      instrument.InstrumentValues.Age();
  }

  internal void InvalidateAged()
  {
    DateTime dateTime1 = Sapi.Now - this.channelTimeout;
    foreach (Instrument instrument in this.Where<Instrument>((Func<Instrument, bool>) (i => i.ManipulatedValue == null)))
    {
      DateTime dateTime2 = dateTime1 - TimeSpan.FromMilliseconds((double) instrument.CacheTimeout);
      InstrumentValue current = instrument.InstrumentValues.Current;
      if (current != null && current.LastSampleTime < dateTime2)
      {
        instrument.InstrumentValues.Invalidate();
        instrument.InstrumentValues.Age();
      }
    }
  }

  public Instrument this[string qualifier]
  {
    get
    {
      if (this.cache == null)
        this.cache = this.ToDictionary((Func<Instrument, string>) (e => e.Qualifier), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Instrument instrument = (Instrument) null;
      string key;
      if (this.cache.TryGetValue(qualifier, out instrument) || this.channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out key) && this.cache.TryGetValue(key, out instrument))
        return instrument;
      if (this.referenceCache == null)
        this.referenceCache = this.Where<Instrument>((Func<Instrument, bool>) (e => e.ReferenceQualifier != null)).DistinctBy<Instrument, string>((Func<Instrument, string>) (e => e.ReferenceQualifier)).ToDictionary<Instrument, string>((Func<Instrument, string>) (e => e.ReferenceQualifier), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return this.referenceCache.TryGetValue(qualifier, out instrument) || key != null && this.referenceCache.TryGetValue(key, out instrument) ? instrument : (Instrument) null;
    }
  }

  public bool AutoRead
  {
    get => this.autoRead;
    set => this.autoRead = value;
  }

  public bool UpdateOnEveryRead
  {
    get => this.updateOnEveryRead;
    set => this.updateOnEveryRead = value;
  }

  public event InstrumentUpdateEventHandler InstrumentUpdateEvent;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private Instrument Matching(string requiredQualifierPrefix, Instrument match)
  {
    foreach (Instrument instrument in (ReadOnlyCollection<Instrument>) this)
    {
      if (instrument != match && instrument.Qualifier.StartsWith(requiredQualifierPrefix, StringComparison.Ordinal) && instrument.IsOriginalShortNameSame(match))
        return instrument;
    }
    return (Instrument) null;
  }

  private void Replace(Instrument instrument1, Instrument instrument2)
  {
    instrument2.SetQualifier(instrument1.Qualifier);
    instrument2.SetAccessLevel(instrument1.AccessLevel);
    this.Items.Remove(instrument1);
  }

  private void Dispose(bool disposing)
  {
    if (!this.disposed && disposing)
    {
      if (this.closingEvent != null)
      {
        this.closingEvent.Set();
        this.closingEvent.Close();
        this.closingEvent = (ManualResetEvent) null;
      }
      if (this.queueNonEmptyEvent != null)
      {
        this.queueNonEmptyEvent.Close();
        this.queueNonEmptyEvent = (ManualResetEvent) null;
      }
    }
    this.disposed = true;
  }

  private void ThreadFunc()
  {
    bool flag = false;
    Queue queue = Queue.Synchronized(this.updatedInstruments);
    WaitHandle[] waitHandles = new WaitHandle[2]
    {
      (WaitHandle) this.closingEvent,
      (WaitHandle) this.queueNonEmptyEvent
    };
    while (!flag)
    {
      switch (WaitHandle.WaitAny(waitHandles))
      {
        case 0:
          flag = true;
          continue;
        case 1:
          if (queue.Count == 0)
          {
            this.queueNonEmptyEvent.Reset();
            continue;
          }
          ((Instrument) queue.Dequeue()).RaiseInstrumentUpdateEvent((Exception) null, false);
          continue;
        default:
          continue;
      }
    }
  }
}
