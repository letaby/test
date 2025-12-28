using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class Instrument : Presentation, IComparable, IDiogenesDataItem, IDisposable
{
	private int waitingForUpdate;

	private CaesarDiagServiceIO diagServiceIO;

	private McdDiagComPrimitive mcdDiagComPrimitive;

	private bool summary;

	private object heldValue;

	private object manipulatedValue;

	private InstrumentValueCollection instrumentValues;

	private string qualifier;

	private string mcdQualifier;

	private string name;

	private string description;

	private string groupName;

	private string groupQualifier;

	private bool hideServiceAsInstrument;

	private bool marked;

	private bool periodic;

	private int accessLevel;

	private ushort cacheTime;

	private int? minimumCacheTime;

	private bool forceRequest;

	private Dump requestMessage;

	private Dump proprietaryContent;

	private CaesarAPtr indexTag;

	private bool disposed;

	private ServiceInputValueCollection inputValues;

	private string userQualifier;

	private IEnumerable<string> enabledAdditionalAudiences;

	public int? MessageNumber { get; private set; }

	public Service CombinedService { get; private set; }

	internal CaesarAPtr IndexTag
	{
		get
		{
			return indexTag;
		}
		set
		{
			indexTag = value;
		}
	}

	public Channel Channel => channel;

	public new string Name
	{
		get
		{
			if (channel.Ecu.RollCallManager != null)
			{
				if (Qualifier.StartsWith("DT_", StringComparison.Ordinal))
				{
					string text = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Qualifier.Substring(3), "SPN"), null);
					if (text == null)
					{
						text = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Qualifier, "SPN"), name);
					}
					return text;
				}
				return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Qualifier, "SPN"), name);
			}
			return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Qualifier, "Name"), name);
		}
	}

	public string ShortName => channel.Ecu.ShortName(Name);

	public new string Description => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Qualifier, "Description"), description);

	public string Qualifier
	{
		get
		{
			if (userQualifier != null)
			{
				return userQualifier;
			}
			return qualifier;
		}
	}

	public string ReferenceQualifier
	{
		get
		{
			if (userQualifier != null)
			{
				return qualifier;
			}
			return null;
		}
	}

	public string GroupName => groupName;

	public string GroupQualifier => groupQualifier;

	public bool Visible
	{
		get
		{
			if (Channel.LogFile != null && InstrumentValues.Count == 0)
			{
				return false;
			}
			if (inputValues != null && hideServiceAsInstrument)
			{
				return false;
			}
			if (Channel.IsRollCall && base.SlotType == -1)
			{
				return false;
			}
			return Sapi.GetSapi().ReadAccess >= accessLevel;
		}
	}

	public InstrumentValueCollection InstrumentValues => instrumentValues;

	public bool Marked
	{
		get
		{
			return marked;
		}
		set
		{
			if (marked != value)
			{
				marked = value;
				if (!marked)
				{
					instrumentValues.Age();
				}
			}
		}
	}

	public bool Periodic => periodic;

	public int AccessLevel => accessLevel;

	public int CacheTimeout
	{
		get
		{
			return cacheTime;
		}
		set
		{
			if (minimumCacheTime.HasValue)
			{
				cacheTime = (ushort)Math.Max(value, minimumCacheTime.Value);
			}
			else
			{
				cacheTime = (ushort)value;
			}
		}
	}

	public int? MinimumCacheTimeout => minimumCacheTime;

	public bool ForceRequest => forceRequest;

	public bool Summary
	{
		get
		{
			if (channel.Ecu.SummaryQualifier(qualifier) || summary)
			{
				return true;
			}
			if (channel.IsRollCall && channel.DataStreamSpns != null && int.TryParse(qualifier.Substring(3), out var result))
			{
				return channel.DataStreamSpns.Contains(result);
			}
			return false;
		}
	}

	public Dump RequestMessage => requestMessage;

	public string McdQualifier => mcdQualifier;

	public Dump ProprietaryContent => proprietaryContent;

	public byte? DestinationAddress { get; private set; }

	public override object ManipulatedValue
	{
		get
		{
			return manipulatedValue;
		}
		set
		{
			if (Channel.LogFile != null)
			{
				throw new InvalidOperationException("Cannot manipulate a value for a log file channel");
			}
			if (value != manipulatedValue)
			{
				manipulatedValue = value;
				object newValue = manipulatedValue ?? heldValue;
				instrumentValues.AddOrUpdate(newValue, explicitRead: true);
				RaiseInstrumentUpdateEvent(null, explicitread: false);
				Channel.SetManipulatedState(Qualifier, value != null);
			}
		}
	}

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CacheTime is deprecated due to non-CLS compliance, please use CacheTimeout instead.")]
	public ushort CacheTime
	{
		get
		{
			return (ushort)CacheTimeout;
		}
		set
		{
			CacheTimeout = value;
		}
	}

	public IEnumerable<string> EnabledAdditionalAudiences => enabledAdditionalAudiences;

	public event InstrumentUpdateEventHandler InstrumentUpdateEvent;

	internal Instrument(Channel channel, string qualifier, ushort index = 0)
		: base(index)
	{
		name = string.Empty;
		description = string.Empty;
		groupName = string.Empty;
		groupQualifier = string.Empty;
		base.channel = channel;
		this.qualifier = qualifier;
		cacheTime = 100;
		instrumentValues = new InstrumentValueCollection(this);
		marked = true;
	}

	internal Instrument(Channel channel, string qualifier, string userQualifier, string userName, ServiceInputValueCollection userValues, bool hide, bool marked)
		: base(0)
	{
		name = userName;
		description = string.Empty;
		groupName = string.Empty;
		groupQualifier = string.Empty;
		base.channel = channel;
		this.qualifier = qualifier;
		this.userQualifier = userQualifier;
		cacheTime = 100;
		inputValues = userValues;
		instrumentValues = new InstrumentValueCollection(this);
		hideServiceAsInstrument = hide;
		this.marked = marked;
	}

	~Instrument()
	{
		Dispose(disposing: false);
	}

	internal void AcquireFromRollCall(IDictionary<string, string> content)
	{
		MessageNumber = content.GetNamedPropertyValue("MessageNumber", -1);
		proprietaryContent = (content.ContainsKey("ProprietaryContent") ? new Dump(content.GetNamedPropertyValue("ProprietaryContent", string.Empty)) : null);
		requestMessage = new Dump(content.GetNamedPropertyValue("RequestMessage", string.Empty));
		periodic = content.GetNamedPropertyValue("Periodic", defaultIfNotSet: false);
		cacheTime = (ushort)content.GetNamedPropertyValue("CacheTime", 100);
		name = content.GetNamedPropertyValue("Name", string.Empty);
		description = content.GetNamedPropertyValue("Description", string.Empty);
		summary = content.GetNamedPropertyValue("Summary", defaultIfNotSet: false);
		string namedPropertyValue = content.GetNamedPropertyValue("DestinationAddress", string.Empty);
		if (!string.IsNullOrEmpty(namedPropertyValue))
		{
			DestinationAddress = Convert.ToByte(namedPropertyValue, CultureInfo.InvariantCulture);
		}
		if (channel != null && channel.Ecu.RollCallManager.Protocol == Protocol.J1939 && channel.SourceAddress.HasValue && requestMessage.Data.Count > 6)
		{
			byte[] array = requestMessage.Data.ToArray();
			array[5] = channel.SourceAddress.Value;
			requestMessage = new Dump(array);
		}
		AcquireFromRollCall((channel != null) ? channel.Ecu : null, qualifier, content, MessageNumber == 61184);
	}

	internal void UpdateFromRollCall(byte[] data)
	{
		if (waitingForUpdate != 0)
		{
			return;
		}
		try
		{
			if (GetPresentation(data, explicitread: false) || channel.Instruments.UpdateOnEveryRead)
			{
				RaiseInstrumentUpdateEvent(null, explicitread: false);
			}
		}
		catch (CaesarException ex)
		{
			RaiseInstrumentUpdateEvent(ex, explicitread: false);
		}
	}

	internal void Acquire(string name, Service diagService, ServiceOutputValue responseParameter)
	{
		CombinedService = diagService;
		requestMessage = diagService.RequestMessage;
		this.name = name;
		mcdQualifier = diagService.McdQualifier;
		Acquire(channel, (Presentation)responseParameter);
		enabledAdditionalAudiences = diagService.EnabledAdditionalAudiences;
	}

	internal void Acquire()
	{
		//IL_00c3: Expected O, but got Unknown
		try
		{
			CaesarDiagService val = channel.EcuHandle.OpenDiagServiceHandle(qualifier);
			try
			{
				if (val != null)
				{
					if (val.PresParamCount != 0)
					{
						base.Acquire(channel, val);
					}
					periodic = val.IsSubscriptionSupported;
					if (name.Length == 0)
					{
						name = val.Name;
					}
					description = val.Description;
					IList<byte> list = val.RequestMessage;
					if (list != null)
					{
						requestMessage = new Dump(list);
					}
					accessLevel = val.AccessLevel;
					int? ecuInfoReadAccessLevel = Channel.DiagnosisVariant.GetEcuInfoReadAccessLevel(Qualifier);
					if (ecuInfoReadAccessLevel.HasValue)
					{
						accessLevel = ecuInfoReadAccessLevel.Value;
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (CaesarErrorException ex)
		{
			CaesarErrorException caesarError = ex;
			Sapi.GetSapi().RaiseExceptionEvent(qualifier, new CaesarException(caesarError));
		}
		if (base.Units.Length > 0)
		{
			groupName = base.Units;
			groupQualifier = groupName.CreateQualifierFromName();
		}
		object obj = channel.Ecu.CacheTimeQualifier(qualifier);
		if (obj != null)
		{
			minimumCacheTime = (cacheTime = Convert.ToUInt16(obj, CultureInfo.InvariantCulture));
			forceRequest = channel.Ecu.ForceRequestQualifier(qualifier);
		}
	}

	internal void InternalRead(bool explicitread)
	{
		CaesarException ex = null;
		bool flag = false;
		if (!channel.IsRollCall)
		{
			if (diagServiceIO == null && mcdDiagComPrimitive == null)
			{
				if (channel.ChannelHandle != null)
				{
					diagServiceIO = channel.ChannelHandle.CreateDiagServiceIO(qualifier);
				}
				else if (channel.McdChannelHandle != null)
				{
					mcdDiagComPrimitive = channel.McdChannelHandle.GetService(mcdQualifier);
				}
			}
			if (diagServiceIO != null || mcdDiagComPrimitive != null)
			{
				if (!channel.IsChannelErrorSet)
				{
					if (diagServiceIO != null)
					{
						if (inputValues != null)
						{
							for (int i = 0; i < inputValues.Count; i++)
							{
								if (channel.IsChannelErrorSet)
								{
									break;
								}
								inputValues[i].SetPreparation(diagServiceIO);
							}
						}
						diagServiceIO.Do((ushort)100);
						if (channel.ChannelHandle.IsErrorSet)
						{
							ex = new CaesarException(channel.ChannelHandle);
							if (ex.ErrorNumber == 6058)
							{
								ex = null;
							}
						}
					}
					else
					{
						if (inputValues != null)
						{
							for (int j = 0; j < inputValues.Count; j++)
							{
								if (channel.IsChannelErrorSet)
								{
									break;
								}
								inputValues[j].SetPreparation(mcdDiagComPrimitive);
							}
						}
						try
						{
							mcdDiagComPrimitive.Execute(100);
						}
						catch (McdException mcdError)
						{
							ex = new CaesarException(mcdError);
						}
					}
					if (ex == null)
					{
						try
						{
							flag = GetPresentation(((object)diagServiceIO) ?? ((object)mcdDiagComPrimitive), explicitread);
						}
						catch (CaesarException ex2)
						{
							ex = ex2;
						}
						catch (McdException mcdError2)
						{
							ex = new CaesarException(mcdError2);
						}
					}
				}
				else
				{
					ex = new CaesarException(channel.ChannelHandle);
				}
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "DSIOHandle was NULL during Instrument::InternalRead");
			}
		}
		else
		{
			try
			{
				Interlocked.Increment(ref waitingForUpdate);
				if (base.SlotType != -1)
				{
					byte[] array = channel.Ecu.RollCallManager.ReadInstrument(channel, requestMessage.Data.ToArray(), MessageNumber.Value, (Tuple<byte?, byte[]> data) => IsTarget(data.Item1, data.Item2), cacheTime);
					if (array != null)
					{
						flag = GetPresentation(array, explicitread);
					}
					else
					{
						ex = new CaesarException(SapiError.BytePosGreaterThanMessageLength);
					}
				}
				else
				{
					channel.Ecu.RollCallManager.DoByteMessage(channel, RequestMessage.Data.ToArray(), null);
				}
			}
			catch (CaesarException ex3)
			{
				InstrumentValues.Age();
				ex = ex3;
			}
			finally
			{
				Interlocked.Decrement(ref waitingForUpdate);
			}
		}
		if (ex != null)
		{
			RaiseInstrumentUpdateEvent(ex, explicitread);
		}
		else if (flag || explicitread || channel.Instruments.UpdateOnEveryRead)
		{
			RaiseInstrumentUpdateEvent(null, explicitread);
		}
	}

	internal void ClearDiagServiceIO()
	{
		if (diagServiceIO != null)
		{
			((CaesarHandle_003CCaesar_003A_003ADiagServiceIO__2as_0020_002A_003E)diagServiceIO).Dispose();
			diagServiceIO = null;
		}
		if (mcdDiagComPrimitive != null)
		{
			mcdDiagComPrimitive.Dispose();
			mcdDiagComPrimitive = null;
		}
	}

	internal void RaiseInstrumentUpdateEvent(Exception ex, bool explicitread)
	{
		FireAndForget.Invoke(this.InstrumentUpdateEvent, this, new ResultEventArgs(ex));
		channel.Instruments.RaiseInstrumentUpdateEvent(this, ex);
		if (explicitread)
		{
			channel.SyncDone(ex);
		}
	}

	internal bool GetPresentation(object source, bool explicitread)
	{
		object obj = null;
		if (source is byte[] data)
		{
			obj = GetPresentation(data);
		}
		else
		{
			CaesarDiagServiceIO val = (CaesarDiagServiceIO)((source is CaesarDiagServiceIO) ? source : null);
			if (val != null)
			{
				obj = GetPresentation(val);
			}
			else if (source is McdDiagComPrimitive mcdDiagComPrimitive)
			{
				obj = GetPresentation(mcdDiagComPrimitive);
			}
		}
		if (obj == null)
		{
			throw new CaesarException(SapiError.UnknownPresentationType);
		}
		heldValue = obj;
		return instrumentValues.AddOrUpdate((manipulatedValue != null) ? manipulatedValue : obj, explicitread);
	}

	internal void SetQualifier(string replaceQualifier)
	{
		userQualifier = replaceQualifier;
	}

	internal bool IsMatching(string qualifier)
	{
		if (!userQualifier.CompareNoCase(qualifier))
		{
			return this.qualifier.CompareNoCase(qualifier);
		}
		return true;
	}

	internal void SetAccessLevel(int accessLevel)
	{
		this.accessLevel = accessLevel;
	}

	internal static void LoadFromLog(XElement element, LogFileFormatTagCollection format, Channel channel, List<string> missingQualifierList, object missingInfoLock)
	{
		string value = element.Attribute(format[TagName.Qualifier]).Value;
		Instrument instrument = channel.Instruments[value];
		if (instrument != null)
		{
			if (instrument.DataInterfaceType == null)
			{
				XAttribute xAttribute = element.Attribute(format[TagName.Type]);
				if (xAttribute != null)
				{
					instrument.SetType(Type.GetType(xAttribute.Value));
				}
			}
			XAttribute xAttribute2 = element.Attribute(format[TagName.Precision]);
			if (xAttribute2 != null)
			{
				instrument.SetPrecision(Convert.ToUInt16(xAttribute2.Value, CultureInfo.InvariantCulture));
			}
			DateTime dateTime = DateTime.MinValue;
			if (instrument.instrumentValues.Count > 0)
			{
				dateTime = instrument.instrumentValues[instrument.instrumentValues.Count - 1].LastSampleTime;
			}
			{
				foreach (XElement item in element.Elements(format[TagName.Value]))
				{
					InstrumentValue instrumentValue = InstrumentValue.FromXElement(item, format, instrument);
					if (instrumentValue.FirstSampleTime > dateTime)
					{
						instrument.InstrumentValues.Add(instrumentValue, setcurrent: false);
					}
				}
				return;
			}
		}
		if (!channel.Ecu.IgnoreQualifier(value))
		{
			lock (missingInfoLock)
			{
				missingQualifierList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", channel.Ecu.Name, value));
			}
		}
	}

	internal void WriteXmlTo(bool all, DateTime startTime, DateTime endTime, XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		writer.WriteStartElement(currentFormat[TagName.Instrument].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, Qualifier);
		if (base.DataInterfaceType == null && base.Type != null)
		{
			writer.WriteAttributeString(currentFormat[TagName.Type].LocalName, base.Type.ToString());
		}
		if (base.Precision != null)
		{
			writer.WriteAttributeString(currentFormat[TagName.Precision].LocalName, base.Precision.ToString());
		}
		foreach (InstrumentValue instrumentValue in instrumentValues)
		{
			if (instrumentValue.Value != null)
			{
				instrumentValue.WriteXmlTo(startTime, endTime, writer);
				if (!all)
				{
					break;
				}
			}
		}
		writer.WriteEndElement();
	}

	public void Read(bool synchronous)
	{
		channel.QueueAction(this, synchronous);
	}

	public override string ToString()
	{
		return qualifier;
	}

	internal bool IsOriginalShortNameSame(Instrument match)
	{
		return string.Equals(channel.Ecu.ShortName(name), channel.Ecu.ShortName(match.name), StringComparison.OrdinalIgnoreCase);
	}

	internal bool IsTarget(byte? destinationAddress, IEnumerable<byte> data)
	{
		if (base.SlotType == -1)
		{
			return false;
		}
		if (DestinationAddress.HasValue && (!destinationAddress.HasValue || destinationAddress != DestinationAddress))
		{
			return false;
		}
		if (ProprietaryContent != null)
		{
			IEnumerable<byte> data2 = ProprietaryContent.Data;
			return data2.SequenceEqual(data.Take(data2.Count()));
		}
		return true;
	}

	public int CompareTo(object obj)
	{
		Instrument instrument = obj as Instrument;
		return string.Compare(Qualifier, instrument.Qualifier, StringComparison.Ordinal);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Instrument object1, Instrument object2)
	{
		return object.Equals(object1, object2);
	}

	public static bool operator !=(Instrument object1, Instrument object2)
	{
		return !object.Equals(object1, object2);
	}

	public static bool operator <(Instrument r1, Instrument r2)
	{
		if (r1 == null)
		{
			throw new ArgumentNullException("r1");
		}
		return r1.CompareTo(r2) < 0;
	}

	public static bool operator >(Instrument r1, Instrument r2)
	{
		if (r1 == null)
		{
			throw new ArgumentNullException("r1");
		}
		return r1.CompareTo(r2) > 0;
	}

	internal new void AddStringsForTranslation(Dictionary<string, string> table)
	{
		table[Sapi.MakeTranslationIdentifier(Qualifier, "Name")] = name;
		if (!string.IsNullOrEmpty(description))
		{
			table[Sapi.MakeTranslationIdentifier(Qualifier, "Description")] = description;
		}
		base.AddStringsForTranslation(table);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			if (indexTag != null)
			{
				((CaesarHandle_003Cvoid_0020_002A_003E)indexTag).Dispose();
				indexTag = null;
			}
			ClearDiagServiceIO();
		}
		disposed = true;
	}
}
