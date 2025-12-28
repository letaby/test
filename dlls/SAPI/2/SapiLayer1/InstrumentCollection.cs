using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using CaesarAbstraction;

namespace SapiLayer1;

public sealed class InstrumentCollection : LateLoadReadOnlyCollection<Instrument>, IDisposable
{
	private const int DefaultInvalidationTimeout = 10000;

	private Dictionary<int, List<Instrument>> rollCallInstrumentsById;

	private List<KeyValuePair<string, List<Instrument>>> instrumentsByRequest;

	private List<KeyValuePair<string, List<Instrument>>> instrumentsByForcedRequest;

	private bool instrumentRequestListDirty;

	private static object userForceRequestListLock = new object();

	private static IEnumerable<Tuple<Ecu, string>> userForceRequestList = new List<Tuple<Ecu, string>>();

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

	public static IEnumerable<string> ForceRequestSet
	{
		get
		{
			lock (userForceRequestListLock)
			{
				return userForceRequestList.Select((Tuple<Ecu, string> t) => t.Item1.Name + "." + t.Item2);
			}
		}
		set
		{
			List<Tuple<Ecu, string>> list = new List<Tuple<Ecu, string>>();
			if (value != null)
			{
				foreach (string item in value)
				{
					string[] array = item.Split(".".ToCharArray());
					if (array.Length > 1)
					{
						Ecu ecu = Sapi.GetSapi().Ecus[array[0]];
						if (ecu != null)
						{
							list.Add(new Tuple<Ecu, string>(ecu, array[1]));
						}
					}
				}
			}
			lock (userForceRequestListLock)
			{
				userForceRequestList = list;
			}
			foreach (Channel item2 in Sapi.GetSapi().Channels.Where((Channel c) => c.Online && !c.IsRollCall))
			{
				item2.Instruments.UpdateInstrumentRequestList();
			}
		}
	}

	private bool ContinueReading
	{
		get
		{
			if (channel.ChannelRunning && (channel.IsRollCall || !channel.IsChannelErrorSet))
			{
				return !channel.Closing;
			}
			return false;
		}
	}

	public Instrument this[string qualifier]
	{
		get
		{
			if (cache == null)
			{
				cache = ToDictionary((Instrument e) => e.Qualifier, StringComparer.OrdinalIgnoreCase);
			}
			Instrument value = null;
			if (cache.TryGetValue(qualifier, out value))
			{
				return value;
			}
			if (channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out var value2) && cache.TryGetValue(value2, out value))
			{
				return value;
			}
			if (referenceCache == null)
			{
				referenceCache = this.Where((Instrument e) => e.ReferenceQualifier != null).DistinctBy((Instrument e) => e.ReferenceQualifier).ToDictionary((Instrument e) => e.ReferenceQualifier, StringComparer.OrdinalIgnoreCase);
			}
			if (referenceCache.TryGetValue(qualifier, out value))
			{
				return value;
			}
			if (value2 != null && referenceCache.TryGetValue(value2, out value))
			{
				return value;
			}
			return null;
		}
	}

	public bool AutoRead
	{
		get
		{
			return autoRead;
		}
		set
		{
			autoRead = value;
		}
	}

	public bool UpdateOnEveryRead
	{
		get
		{
			return updateOnEveryRead;
		}
		set
		{
			updateOnEveryRead = value;
		}
	}

	public event InstrumentUpdateEventHandler InstrumentUpdateEvent;

	internal InstrumentCollection(Channel c)
	{
		channel = c;
		updatedInstruments = new Queue();
		closingEvent = new ManualResetEvent(initialState: false);
		queueNonEmptyEvent = new ManualResetEvent(initialState: false);
		channelTimeout = TimeSpan.FromMilliseconds(channel.IsRollCall ? channel.Ecu.RollCallManager.ChannelTimeout : 10000);
		instrumentsByForcedRequest = (instrumentsByRequest = new List<KeyValuePair<string, List<Instrument>>>());
	}

	protected override void AcquireList()
	{
		if (channel.Ecu.RollCallManager != null)
		{
			foreach (Instrument item in channel.Ecu.RollCallManager.CreateInstruments(channel))
			{
				base.Items.Add(item);
			}
			rollCallInstrumentsById = (from i in this
				group i by i.MessageNumber.Value).ToDictionary((IGrouping<int, Instrument> k) => k.Key, (IGrouping<int, Instrument> v) => v.ToList());
			instrumentsByRequest = (from i in this
				where !i.Periodic
				group i by i.RequestMessage).ToDictionary((IGrouping<Dump, Instrument> k) => k.Key.ToString(), (IGrouping<Dump, Instrument> v) => v.ToList()).ToList();
			return;
		}
		if (channel.EcuHandle != null)
		{
			StringCollection services = channel.EcuHandle.GetServices((ServiceTypes)16);
			for (int num = 0; num < services.Count; num++)
			{
				string qualifier = services[num];
				if (!channel.Ecu.IgnoreQualifier(qualifier) && !channel.Ecu.MakeStoredQualifier(qualifier))
				{
					Instrument instrument = new Instrument(channel, qualifier, 0);
					instrument.Acquire();
					base.Items.Add(instrument);
				}
			}
			StringEnumerator enumerator2 = channel.EcuHandle.GetServices((ServiceTypes)2097152).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					if (channel.Ecu.MakeInstrumentQualifier(current2) && !channel.Ecu.IgnoreQualifier(current2))
					{
						Instrument instrument2 = new Instrument(channel, current2, 0);
						instrument2.Acquire();
						base.Items.Add(instrument2);
					}
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		else if (channel.McdEcuHandle != null)
		{
			foreach (IGrouping<ServiceTypes, Tuple<Service, ServiceOutputValue>> item2 in from s in channel.CaesarEquivalentServices
				orderby s.Item1.Qualifier
				group s by s.Item1.ServiceTypes)
			{
				ServiceTypes key = item2.Key;
				if ((key & ServiceTypes.StoredData) == 0 && (key & ServiceTypes.Data) == 0)
				{
					continue;
				}
				foreach (Tuple<Service, ServiceOutputValue> item3 in item2)
				{
					if (item3.Item2 == null)
					{
						continue;
					}
					string singleServiceQualifier = item3.Item2.SingleServiceQualifier;
					if ((key & ServiceTypes.Data) != ServiceTypes.None)
					{
						if (channel.Ecu.IgnoreQualifier(singleServiceQualifier) || channel.Ecu.MakeStoredQualifier(singleServiceQualifier))
						{
							continue;
						}
					}
					else if ((key & ServiceTypes.StoredData) != ServiceTypes.None && (!channel.Ecu.MakeInstrumentQualifier(singleServiceQualifier) || channel.Ecu.IgnoreQualifier(singleServiceQualifier)))
					{
						continue;
					}
					Instrument instrument3 = new Instrument(channel, singleServiceQualifier, (ushort)item3.Item2.Index);
					instrument3.Acquire(item3.Item2.SingleServiceName, item3.Item1, item3.Item2);
					base.Items.Add(instrument3);
				}
			}
		}
		XmlNode xml = channel.Ecu.Xml;
		if (xml != null)
		{
			XmlNodeList xmlNodeList = xml.SelectNodes("Ecu/Instruments/Instrument[@ReplaceQualifier]");
			if (xmlNodeList != null)
			{
				for (int num2 = 0; num2 < xmlNodeList.Count; num2++)
				{
					XmlNode xmlNode = xmlNodeList.Item(num2);
					string innerText = xmlNode.Attributes.GetNamedItem("Qualifier").InnerText;
					string innerText2 = xmlNode.Attributes.GetNamedItem("ReplaceQualifier").InnerText;
					XmlNode namedItem = xmlNode.Attributes.GetNamedItem("CheckBitLength");
					if (namedItem == null || !bool.TryParse(namedItem.InnerText, out var result))
					{
						result = true;
					}
					if (!string.IsNullOrEmpty(channel.Ecu.NameSplit))
					{
						for (int num3 = 0; num3 < base.Count; num3++)
						{
							Instrument instrument4 = base[num3];
							if (!instrument4.Qualifier.StartsWith(innerText, StringComparison.Ordinal))
							{
								continue;
							}
							Instrument instrument5 = Matching(innerText2, instrument4);
							if (instrument5 != null)
							{
								if (!result || object.Equals(instrument4.BitLength, instrument5.BitLength))
								{
									Replace(instrument4, instrument5);
									num3--;
								}
								else
								{
									Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Not replacing {0} with {1} because the bit lengths are different", instrument4.Qualifier, instrument5.Qualifier));
								}
							}
						}
						continue;
					}
					Instrument instrument6 = this[innerText];
					if (instrument6 != null)
					{
						Instrument instrument7 = this[innerText2];
						if (instrument7 != null)
						{
							Replace(instrument6, instrument7);
						}
						else if (string.IsNullOrEmpty(innerText2))
						{
							base.Items.Remove(instrument6);
						}
						else
						{
							Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not find .EcuInfo instrument replace qualifier: {0}", innerText2));
						}
					}
					else
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not find .EcuInfo instrument qualifier: {0}", innerText));
					}
				}
			}
			XmlNodeList xmlNodeList2 = xml.SelectNodes("Ecu/ServicesAsInstruments/ServiceAsInstrument");
			if (xmlNodeList2 != null)
			{
				for (int num4 = 0; num4 < xmlNodeList2.Count; num4++)
				{
					XmlNode xmlNode2 = xmlNodeList2.Item(num4);
					string innerText3 = xmlNode2.Attributes.GetNamedItem("Qualifier").InnerText;
					string innerText4 = xmlNode2.Attributes.GetNamedItem("Name").InnerText;
					string innerText5 = xmlNode2.Attributes.GetNamedItem("ReferenceQualifier").InnerText;
					XmlNode namedItem2 = xmlNode2.Attributes.GetNamedItem("Hide");
					bool hide = namedItem2 != null && Convert.ToBoolean(namedItem2.InnerText, CultureInfo.InvariantCulture);
					XmlNode namedItem3 = xmlNode2.Attributes.GetNamedItem("Marked");
					bool marked = namedItem3 == null || Convert.ToBoolean(namedItem3.InnerText, CultureInfo.InvariantCulture);
					XmlNode namedItem4 = xmlNode2.Attributes.GetNamedItem("IsRegex");
					if (namedItem4 != null && Convert.ToBoolean(namedItem4.InnerText, CultureInfo.InvariantCulture))
					{
						Regex regex = new Regex(innerText5, RegexOptions.Compiled);
						int num5 = 0;
						foreach (Service service2 in channel.Services)
						{
							Match match = regex.Match(service2.Qualifier);
							if (match.Success && match.Groups.Count > 1)
							{
								string[] args = (from g in match.Groups.OfType<Group>()
									select g.Value).Skip(1).ToArray();
								string qualifier2 = string.Format(CultureInfo.InvariantCulture, innerText3, args);
								string name = string.Format(CultureInfo.InvariantCulture, innerText4, args);
								AddServiceAsInstrument(qualifier2, name, service2, null, hide, marked);
								num5++;
							}
						}
						if (num5 == 0)
						{
							Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not match any services as instrument for reference {0}", innerText5));
						}
					}
					else
					{
						Service service = channel.Services[innerText5];
						if (service != null)
						{
							AddServiceAsInstrument(innerText3, innerText4, service, innerText5, hide, marked);
						}
						else
						{
							Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not find service as instrument {0} reference {1}", innerText4, innerText5));
						}
					}
				}
			}
		}
		UpdateInstrumentRequestList();
	}

	private void AddServiceAsInstrument(string qualifier, string name, Service service, string definedInputValues, bool hide, bool marked)
	{
		ServiceInputValueCollection serviceInputValueCollection = new ServiceInputValueCollection(service);
		for (int i = 0; i < service.InputValues.Count; i++)
		{
			ServiceInputValue serviceInputValue = service.InputValues[i].Clone();
			serviceInputValueCollection.Add(serviceInputValue);
		}
		Exception ex = ((definedInputValues != null) ? serviceInputValueCollection.InternalParseValues(definedInputValues) : null);
		if (ex != null)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(service, string.Format(CultureInfo.InvariantCulture, "Could not set service as instrument input value: {0} {1}", service.Name, ex.Message));
			return;
		}
		Instrument instrument = new Instrument(channel, service.Qualifier, qualifier, name, serviceInputValueCollection, hide, marked);
		if (channel.McdEcuHandle != null)
		{
			instrument.Acquire(name, service, service.OutputValues[0]);
		}
		else
		{
			instrument.Acquire();
		}
		base.Items.Add(instrument);
	}

	private void UpdateInstrumentRequestList()
	{
		instrumentRequestListDirty = true;
	}

	private void InternalUpdateInstrumentRequestList()
	{
		IEnumerable<Tuple<Ecu, string>> userForceRequestListCopy = null;
		lock (userForceRequestListLock)
		{
			userForceRequestListCopy = userForceRequestList;
		}
		IEnumerable<IGrouping<bool, KeyValuePair<string, List<Instrument>>>> enumerable = from v in (from i in this
				where !i.Periodic && Sapi.GetSapi().HardwareAccess >= i.AccessLevel
				group i by i.RequestMessage.ToString()).ToDictionary((IGrouping<string, Instrument> k) => k.Key.ToString(), (IGrouping<string, Instrument> v) => v.ToList()).ToList()
			group v by v.Value.Any((Instrument i) => i.ForceRequest || userForceRequestListCopy.Any((Tuple<Ecu, string> u) => u.Item1 == i.Channel.Ecu && u.Item2 == i.Qualifier));
		instrumentsByForcedRequest = new List<KeyValuePair<string, List<Instrument>>>();
		instrumentsByRequest = new List<KeyValuePair<string, List<Instrument>>>();
		foreach (IGrouping<bool, KeyValuePair<string, List<Instrument>>> item in enumerable)
		{
			if (item.Key)
			{
				instrumentsByForcedRequest = item.ToList();
			}
			else
			{
				instrumentsByRequest = item.ToList();
			}
		}
		instrumentRequestListDirty = false;
	}

	internal void UpdateFromRollCall(int id, byte? destinationAddress, byte[] data)
	{
		if (base.Count <= 0 || rollCallInstrumentsById == null || !rollCallInstrumentsById.TryGetValue(id, out var value))
		{
			return;
		}
		foreach (Instrument item in value)
		{
			if (item.IsTarget(destinationAddress, data))
			{
				item.UpdateFromRollCall(data);
			}
		}
	}

	internal bool InternalRead()
	{
		bool result = false;
		if (instrumentRequestListDirty)
		{
			InternalUpdateInstrumentRequestList();
		}
		int i;
		for (i = nextInstrumentToRead; i < instrumentsByRequest.Count; i++)
		{
			if (!ContinueReading)
			{
				break;
			}
			if (channel.ActionWaiting)
			{
				break;
			}
			if (instrumentRequestListDirty)
			{
				break;
			}
			KeyValuePair<string, List<Instrument>> messageset = instrumentsByRequest[i];
			if (InternalReadMessageSet(messageset))
			{
				result = true;
			}
			foreach (KeyValuePair<string, List<Instrument>> item in instrumentsByForcedRequest)
			{
				if (ContinueReading && InternalReadMessageSet(item))
				{
					result = true;
				}
			}
		}
		nextInstrumentToRead = ((i < instrumentsByRequest.Count) ? i : 0);
		return result;
	}

	private bool InternalReadMessageSet(KeyValuePair<string, List<Instrument>> messageset)
	{
		bool result = false;
		IEnumerable<Instrument> enumerable = messageset.Value.Where((Instrument i) => i.Marked);
		if (enumerable.Any())
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(channel.IsRollCall ? messageset.Value.Max((Instrument i) => i.CacheTimeout) : messageset.Value.Min((Instrument i) => i.CacheTimeout));
			DateTime now = Sapi.Now;
			if (!lastRequests.ContainsKey(messageset.Key) || lastRequests[messageset.Key] + timeSpan < now)
			{
				lastRequests[messageset.Key] = now;
				foreach (Instrument item in channel.IsRollCall ? enumerable.Take(1) : enumerable)
				{
					item.InternalRead(explicitread: false);
					result = true;
					if (!ContinueReading)
					{
						return result;
					}
				}
			}
		}
		return result;
	}

	internal void RaiseInstrumentUpdateEvent(Instrument i, Exception ex)
	{
		FireAndForget.Invoke(this.InstrumentUpdateEvent, i, new ResultEventArgs(ex));
	}

	internal void RegisterPeriodicListeners(bool reg)
	{
		if (reg)
		{
			new Thread(ThreadFunc).Start();
		}
		if (!channel.IsRollCall)
		{
			for (int i = 0; i < base.Count; i++)
			{
				Instrument instrument = base[i];
				if (!instrument.Periodic)
				{
					continue;
				}
				if (channel.ChannelHandle.SetPresentationListener(instrument.Qualifier))
				{
					if (reg)
					{
						instrument.IndexTag = CaesarRoot.RegisterPeriodicCallback(channel.ChannelHandle, i);
						if (instrument.IndexTag == null)
						{
							Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not register event callback for {0}", instrument.Qualifier));
						}
					}
					else if (instrument.IndexTag != null)
					{
						if (!CaesarRoot.UnregisterPeriodicCallback(channel.ChannelHandle, instrument.IndexTag))
						{
							Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not unregister event callback for {0}", instrument.Qualifier));
						}
						((CaesarHandle_003Cvoid_0020_002A_003E)instrument.IndexTag).Dispose();
						instrument.IndexTag = null;
					}
					else
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "No item tag exists to unregister event callback for {0}", instrument.Qualifier));
					}
				}
				else
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not set presentation listener for {0} reg={1}", instrument.Qualifier, reg));
				}
			}
		}
		if (!reg)
		{
			closingEvent.Set();
		}
	}

	internal void PeriodicCallback(int index, CaesarDiagServiceIO diagServiceIO)
	{
		if (index >= 0 && index < base.Count)
		{
			Instrument instrument = base[index];
			if (instrument.GetPresentation(diagServiceIO, explicitread: false))
			{
				Queue queue = Queue.Synchronized(updatedInstruments);
				if (!queue.Contains(instrument))
				{
					queue.Enqueue(instrument);
					queueNonEmptyEvent.Set();
				}
			}
		}
		else
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Got periodic callback for an unknown instrument (index {0})", index));
		}
	}

	internal void Invalidate()
	{
		using IEnumerator<Instrument> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Instrument current = enumerator.Current;
			current.InstrumentValues.Invalidate();
			current.ClearDiagServiceIO();
		}
	}

	internal void AgeAll()
	{
		using IEnumerator<Instrument> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.InstrumentValues.Age();
		}
	}

	internal void InvalidateAged()
	{
		DateTime dateTime = Sapi.Now - channelTimeout;
		foreach (Instrument item in this.Where((Instrument i) => i.ManipulatedValue == null))
		{
			DateTime dateTime2 = dateTime - TimeSpan.FromMilliseconds(item.CacheTimeout);
			InstrumentValue current2 = item.InstrumentValues.Current;
			if (current2 != null && current2.LastSampleTime < dateTime2)
			{
				item.InstrumentValues.Invalidate();
				item.InstrumentValues.Age();
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private Instrument Matching(string requiredQualifierPrefix, Instrument match)
	{
		using (IEnumerator<Instrument> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Instrument current = enumerator.Current;
				if (current != match && current.Qualifier.StartsWith(requiredQualifierPrefix, StringComparison.Ordinal) && current.IsOriginalShortNameSame(match))
				{
					return current;
				}
			}
		}
		return null;
	}

	private void Replace(Instrument instrument1, Instrument instrument2)
	{
		instrument2.SetQualifier(instrument1.Qualifier);
		instrument2.SetAccessLevel(instrument1.AccessLevel);
		base.Items.Remove(instrument1);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			if (closingEvent != null)
			{
				closingEvent.Set();
				closingEvent.Close();
				closingEvent = null;
			}
			if (queueNonEmptyEvent != null)
			{
				queueNonEmptyEvent.Close();
				queueNonEmptyEvent = null;
			}
		}
		disposed = true;
	}

	private void ThreadFunc()
	{
		bool flag = false;
		Queue queue = Queue.Synchronized(updatedInstruments);
		WaitHandle[] waitHandles = new WaitHandle[2] { closingEvent, queueNonEmptyEvent };
		while (!flag)
		{
			switch (WaitHandle.WaitAny(waitHandles))
			{
			case 0:
				flag = true;
				break;
			case 1:
				if (queue.Count == 0)
				{
					queueNonEmptyEvent.Reset();
				}
				else
				{
					((Instrument)queue.Dequeue()).RaiseInstrumentUpdateEvent(null, explicitread: false);
				}
				break;
			}
		}
	}
}
