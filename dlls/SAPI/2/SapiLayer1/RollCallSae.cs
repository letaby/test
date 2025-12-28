using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using J2534;

namespace SapiLayer1;

internal abstract class RollCallSae : RollCall, IDisposable, IRollCall
{
	protected new sealed class ChannelInformation : RollCall.ChannelInformation, IDisposable
	{
		internal ChannelInformation(RollCallSae rollCallManager, ConnectionResource resource, IEnumerable<ID> identificationIdentifiers)
			: base(rollCallManager, resource, identificationIdentifiers.Select((ID id) => new IdentificationInformation(id)))
		{
		}

		protected override void ThreadFunc()
		{
			byte sourceAddress = base.Resource.SourceAddress.Value;
			int num = 0;
			IEnumerable<IGrouping<int, IdentificationInformation>> enumerable = from id in Identification
				group id by ((RollCallSae)rollCallManager).MapIdToRequestId(id.Id);
			if (enumerable.Any())
			{
				double num2 = 100 / enumerable.Count();
				foreach (IGrouping<int, IdentificationInformation> item in enumerable)
				{
					Sapi.GetSapi().Channels.RaiseConnectProgressEvent(base.Resource, (double)num++ * num2 + num2 / 2.0);
					try
					{
						((RollCallSae)rollCallManager).RequestAndWait(item.Key, sourceAddress);
					}
					catch (CaesarException ex)
					{
						switch (ex.ErrorNumber)
						{
						case 6607L:
						case 6620L:
							Sapi.GetSapi().Channels.RaiseConnectCompleteEvent(base.Resource, ex);
							return;
						case 6623L:
							foreach (IdentificationInformation item2 in item)
							{
								item2.Value = ex.Message;
							}
							break;
						}
					}
				}
			}
			if (((RollCallSae)rollCallManager).debugLevel > 1)
			{
				((RollCallSae)rollCallManager).RaiseDebugInfoEvent(sourceAddress, "Create channel");
			}
			int? function = null;
			IdentificationInformation identificationInformation = Identification.FirstOrDefault((IdentificationInformation i) => i.Id == ID.Function);
			if (identificationInformation != null && identificationInformation.Value != null)
			{
				function = (int)identificationInformation.Value;
			}
			DiagnosisVariant diagnosisVariant = null;
			IEnumerable<Tuple<ID, object>> readIdBlock = from id in Identification
				where id.Value != null
				select new Tuple<ID, object>(id.Id, id.Value);
			IEnumerable<DiagnosisVariant> source = rollCallManager.Ecus.Where(delegate(Ecu e)
			{
				int? num3 = e.SourceAddress;
				int num4 = sourceAddress;
				if (num3.GetValueOrDefault() != num4 || !num3.HasValue || e.Function != function)
				{
					int? num5 = e.SourceAddress;
					num4 = sourceAddress;
					if (num5.GetValueOrDefault() != num4 || !num5.HasValue || e.Function.HasValue)
					{
						if (!e.SourceAddress.HasValue)
						{
							return e.Function == function;
						}
						return false;
					}
				}
				return true;
			}).SelectMany((Ecu e) => e.DiagnosisVariants.Where((DiagnosisVariant v) => v.IsMatch(readIdBlock)));
			if (source.Any())
			{
				diagnosisVariant = (from v in source
					orderby v.RollCallIdentificationCount, !v.Ecu.IsRollCallBaseEcu
					select v).Last();
				resource = new ConnectionResource(diagnosisVariant.Ecu, rollCallManager.Protocol, base.Resource.HardwareName, (uint)base.Resource.BaudRate, 1, sourceAddress);
			}
			else
			{
				diagnosisVariant = base.Resource.Ecu.DiagnosisVariants["ROLLCALL"];
			}
			if (!Connect(diagnosisVariant))
			{
				Sapi.GetSapi().Channels.RaiseConnectCompleteEvent(base.Resource, new CaesarException(SapiError.DeviceInvalid));
			}
		}
	}

	protected delegate void IdentificationAction(IdentificationInformation identfication);

	protected enum Acknowledgment : uint
	{
		Positive,
		Negative,
		AccessDenied,
		Busy
	}

	internal class MonitorData
	{
		private const string Base = "BASE";

		public Dictionary<string, XElement> definitions = new Dictionary<string, XElement>();

		public Dictionary<Tuple<string, string>, XElement> baseDefinitions = new Dictionary<Tuple<string, string>, XElement>();

		private XElement GetEcuContent(Ecu ecu)
		{
			if (ecu.Function.HasValue)
			{
				string key = string.Format(CultureInfo.InvariantCulture, "F{0}", ecu.Function.Value);
				if (definitions.ContainsKey(key))
				{
					return definitions[key];
				}
			}
			if (ecu.SourceAddress.HasValue)
			{
				string key2 = ecu.SourceAddress.Value.ToString(CultureInfo.InvariantCulture);
				if (definitions.ContainsKey(key2))
				{
					return definitions[key2];
				}
			}
			return null;
		}

		public XElement GetBaseDefinition(string qualifier, Type type)
		{
			if (!baseDefinitions.TryGetValue(new Tuple<string, string>(type.Name, qualifier), out var value))
			{
				return null;
			}
			return value;
		}

		public IEnumerable<XElement> GetDefinitions(Channel channel, Type type)
		{
			XElement typedEcuContent = GetEcuContent(channel.Ecu)?.Element(type.Name + "s");
			foreach (string qualifier in channel.DiagnosisVariant.DiagServiceQualifiers)
			{
				XElement xElement = null;
				if (typedEcuContent != null)
				{
					xElement = typedEcuContent.Elements(type.Name).FirstOrDefault((XElement ac) => ac.Attribute("qualifier").Value == qualifier);
				}
				if (xElement == null)
				{
					xElement = GetBaseDefinition(qualifier, type);
				}
				if (xElement != null)
				{
					yield return xElement;
				}
			}
		}

		private static IEnumerable<Tuple<ID, string>> GetIdBlock(XElement variantElement)
		{
			XElement xElement = variantElement.Element("Identifications");
			if (xElement == null)
			{
				yield break;
			}
			foreach (XElement item in xElement.Elements("Identification"))
			{
				yield return new Tuple<ID, string>((ID)Enum.Parse(typeof(ID), item.Attribute("qualifier").Value, ignoreCase: true), item.Value);
			}
		}

		private static IEnumerable<DiagnosisVariant> GetVariants(XElement ecuElement, Ecu ecu)
		{
			foreach (XElement item in ecuElement.Element("Variants").Elements("Variant"))
			{
				string text = item.GetAttribute("name");
				IEnumerable<Tuple<ID, string>> idBlock = GetIdBlock(item);
				if (idBlock.Any())
				{
					if (text == null)
					{
						if (ecu.IsRollCallBaseEcu)
						{
							throw new InvalidOperationException("Diagnostic data for base Ecu '" + ecu.Name + "' contains an ID block but doesn't specify a Variant name. A Variant name is required.");
						}
						text = ecu.Name;
					}
					else if (text == "ROLLCALL" || text == ecu.Name)
					{
						throw new InvalidOperationException("Diagnostic data for '" + ecu.Name + "' invalidly specifies the reserved Variant name '" + text + "'.");
					}
				}
				else
				{
					if (text != null)
					{
						throw new InvalidOperationException("Diagnostic data for Ecu '" + ecu.Name + "' invalidly specifies a name '" + text + "' for a Variant that has no ID block data. S-API is responsible for assigning the name for such Variants.");
					}
					text = "ROLLCALL";
				}
				yield return new DiagnosisVariant(ecu, text, item.GetAttribute("equipment"), idBlock, from i in item.Element("References").Elements("Reference")
					select i.Attribute("qualifier").Value);
			}
		}

		public IEnumerable<Ecu> GetEcus(RollCallSae manager)
		{
			foreach (XElement item in definitions.Select((KeyValuePair<string, XElement> p) => p.Value))
			{
				XAttribute xAttribute = item.Attribute("address");
				byte? address = ((xAttribute != null) ? new byte?(Convert.ToByte(xAttribute.Value, CultureInfo.InvariantCulture)) : ((byte?)null));
				IEnumerable<byte> alternateAddresses = GetAlternateAddresses(item);
				XAttribute xAttribute2 = item.Attribute("function");
				byte? function = ((xAttribute2 != null) ? new byte?(Convert.ToByte(xAttribute2.Value, CultureInfo.InvariantCulture)) : ((byte?)null));
				string version = item.GetAttribute("version") ?? string.Empty;
				XAttribute xAttribute3 = item.Attribute("otherProtocolAddress");
				byte? otherProtocolAddress = ((xAttribute3 != null) ? new byte?(Convert.ToByte(xAttribute3.Value, CultureInfo.InvariantCulture)) : ((byte?)null));
				foreach (XElement ecuElement in item.Element("Ecus").Elements("Ecu"))
				{
					string name = ecuElement.GetAttribute("name");
					string description = ecuElement.GetAttribute("description");
					string category = ecuElement.GetAttribute("category");
					string family = ecuElement.GetAttribute("family");
					string supportedEquipment = ecuElement.GetAttribute("supportedEquipment");
					if (!alternateAddresses.Any())
					{
						Ecu ecu = new Ecu(address, function, name, DiagnosisSource.RollCallDatabase, manager, description, category, family, supportedEquipment, otherProtocolAddress);
						ecu.AcquireFromRollCall(version, GetVariants(ecuElement, ecu));
						yield return ecu;
						continue;
					}
					int index = 1;
					foreach (byte item2 in Enumerable.Repeat(address.Value, 1).Union(alternateAddresses))
					{
						Ecu ecu2 = new Ecu(ecuName: (name != null) ? (name + "-" + index) : null, shortDescription: (description != null) ? (description + " " + index) : null, sourceAddress: item2, function: function, source: DiagnosisSource.RollCallDatabase, rollCallManager: manager, category: category, family: family, supportedEquipment: supportedEquipment, otherProtocolAddress: null);
						ecu2.AcquireFromRollCall(version, GetVariants(ecuElement, ecu2));
						yield return ecu2;
						index++;
					}
				}
			}
		}

		private static IEnumerable<byte> GetAlternateAddresses(XElement definition)
		{
			int i = 1;
			while (true)
			{
				XAttribute xAttribute = definition.Attribute(string.Format(CultureInfo.InvariantCulture, "alternateAddress{0}", i++));
				if (xAttribute != null)
				{
					yield return Convert.ToByte(xAttribute.Value, CultureInfo.InvariantCulture);
					continue;
				}
				break;
			}
		}

		public static MonitorData Load(RollCallSae manager)
		{
			MonitorData monitorData = new MonitorData();
			string path = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}monitor.sbf", Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, manager.Protocol.ToString());
			if (File.Exists(path))
			{
				using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				using GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress);
				foreach (XElement item in XDocument.Load(stream2).Element("Content").Element("Sources")
					.Elements("Source"))
				{
					XAttribute xAttribute = item.Attribute("address");
					string text = ((xAttribute != null) ? xAttribute.Value : string.Empty);
					if (text == "BASE")
					{
						foreach (XElement item2 in item.Elements().SelectMany((XElement t) => t.Elements()))
						{
							monitorData.baseDefinitions.Add(new Tuple<string, string>(item2.Name.ToString(), item2.Attribute("qualifier").Value), item2);
						}
					}
					else
					{
						XAttribute xAttribute2 = item.Attribute("function");
						if (xAttribute2 != null)
						{
							monitorData.definitions.Add("F" + xAttribute2.Value, item);
						}
						else
						{
							monitorData.definitions.Add(text, item);
						}
					}
				}
			}
			return monitorData;
		}

		private static bool IsSourceElementForAddress(XElement element, string address)
		{
			XAttribute xAttribute = element.Attribute("address");
			if (xAttribute != null && xAttribute.Value == address)
			{
				return true;
			}
			return GetAlternateAddresses(element).Contains(Convert.ToByte(address, CultureInfo.InvariantCulture));
		}

		public static void SaveDataItemDescription(Protocol protocol, byte address, Type type, string qualifier, IDictionary<string, string> content)
		{
			string path = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}monitor.sbf", Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, protocol.ToString());
			XDocument xDocument;
			using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress);
				xDocument = XDocument.Load(stream2);
				XElement xElement = xDocument.Element("Content").Element("Sources");
				XElement xElement2 = null;
				XElement xElement3 = null;
				string sourceAddressString = address.ToString(CultureInfo.InvariantCulture);
				XElement xElement4 = xElement.Elements("Source").FirstOrDefault((XElement v) => IsSourceElementForAddress(v, sourceAddressString));
				XElement xElement5;
				if (xElement4 == null)
				{
					xElement4 = new XElement("Source", new XAttribute("address", sourceAddressString));
					xElement.Add(xElement4);
					xElement5 = new XElement("Ecus");
					xElement4.Add(new XAttribute("version", "1"), xElement5);
				}
				else
				{
					xElement5 = xElement4.Element("Ecus");
					xElement2 = xElement4.Element("Services");
					xElement3 = xElement4.Element("Instruments");
					XAttribute xAttribute = xElement4.Attribute("version");
					if (xAttribute != null)
					{
						int num = Convert.ToInt32(xAttribute.Value, CultureInfo.InvariantCulture);
						xAttribute.Value = (num + 1).ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						xElement4.Add(new XAttribute("version", "1"));
					}
				}
				bool flag = false;
				XElement xElement7;
				string text;
				if (type == typeof(Instrument))
				{
					if (Regex.Match(qualifier, "DT_[0-9]*").Value == qualifier)
					{
						XElement xElement6 = xElement.Elements("Source").FirstOrDefault((XElement v) => v.Attribute("address") != null && v.Attribute("address").Value == "BASE");
						if (xElement6 != null)
						{
							xElement3 = xElement6.Element("Instruments");
							flag = true;
						}
					}
					if (xElement3 == null)
					{
						xElement3 = new XElement("Instruments");
						xElement4.Add(xElement3);
					}
					xElement7 = xElement3;
					text = "Instrument";
				}
				else
				{
					if (xElement2 == null)
					{
						xElement2 = new XElement("Services");
						xElement4.Add(xElement2);
					}
					xElement7 = xElement2;
					text = "Service";
				}
				IEnumerable<XElement> enumerable = content.Select((KeyValuePair<string, string> c) => new XElement("Property", new XAttribute("name", c.Key), c.Value));
				XElement xElement8 = xElement7.Elements(text).FirstOrDefault((XElement xe) => xe.Attribute("qualifier").Value == qualifier);
				if (xElement8 != null)
				{
					xElement8.RemoveNodes();
					xElement8.Add(enumerable);
				}
				else
				{
					bool flag2 = false;
					if (flag)
					{
						int num2 = Convert.ToInt32(qualifier.Substring(3), CultureInfo.InvariantCulture);
						foreach (XElement item in xElement7.Elements())
						{
							string text2 = ((item.Attribute("qualifier") != null) ? item.Attribute("qualifier").Value : null);
							if (text2 != null && text2.StartsWith("DT_", StringComparison.Ordinal) && int.TryParse(text2.Substring(3), out var result) && result > num2)
							{
								item.AddBeforeSelf(new XElement(text, new XAttribute("qualifier", qualifier), enumerable));
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						xElement7.Add(new XElement(text, new XAttribute("qualifier", qualifier), enumerable));
					}
				}
				if (xElement5.Elements().Count() == 0)
				{
					xElement5.Add(new XElement("Ecu", new XElement("Variants")));
				}
				foreach (XElement item2 in xElement5.Elements())
				{
					XElement xElement9 = item2.Element("Variants");
					if (xElement9.Elements().Count() == 0)
					{
						xElement9.Add(new XElement("Variant", new XElement("References")));
					}
					foreach (XElement item3 in xElement9.Elements())
					{
						item3.Element("References").Add(new XElement("Reference", new XAttribute("qualifier", qualifier)));
					}
				}
			}
			using (Stream stream3 = new FileStream(path, FileMode.Create))
			{
				using GZipStream stream4 = new GZipStream(stream3, CompressionMode.Compress);
				xDocument.Save(stream4);
			}
			((RollCallSae)RollCall.GetManager(protocol)).ClearEcus();
			Sapi.GetSapi().Ecus.ClearList();
		}
	}

	protected class QueueItem : IDisposable
	{
		private readonly Predicate<byte[]> additionalResponseCheck;

		private readonly int retryCount;

		private readonly int synchronousRequestTimeout;

		private readonly int busyRetryCount;

		private readonly int betweenBusyRequestInterval;

		private object eventLock = new object();

		private bool disposed;

		private PassThruMsg RequestMessage { get; set; }

		private int ResponseId { get; set; }

		private byte DestinationAddress { get; set; }

		private ManualResetEvent Event { get; set; }

		private Acknowledgment Acknowledgement { get; set; }

		private byte[] Response { get; set; }

		internal QueueItem(PassThruMsg requestMessage, int responseId, byte destinationAddress)
			: this(requestMessage, responseId, destinationAddress, null, 3, 2000, 5, 1000)
		{
		}

		internal QueueItem(PassThruMsg requestMessage, int responseId, byte destinationAddress, Predicate<byte[]> additionalResponseCheck, int retryCount, int synchronousRequestTimeout, int busyRetryCount, int betweenBusyRequestInterval)
		{
			RequestMessage = requestMessage;
			ResponseId = responseId;
			DestinationAddress = destinationAddress;
			Event = new ManualResetEvent(initialState: false);
			this.additionalResponseCheck = additionalResponseCheck;
			this.retryCount = retryCount;
			this.synchronousRequestTimeout = synchronousRequestTimeout;
			this.busyRetryCount = busyRetryCount;
			this.betweenBusyRequestInterval = betweenBusyRequestInterval;
		}

		internal byte[] Request(RollCallSae rollCallManager)
		{
			int num = 0;
			int num2 = retryCount;
			CaesarException ex = null;
			ManualResetEvent closingEvent = rollCallManager.closingEvent;
			while (num++ < num2)
			{
				if (closingEvent == null || !rollCallManager.ProtocolAlive)
				{
					throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
				}
				Event.Reset();
				if (rollCallManager.debugLevel > 0)
				{
					rollCallManager.RaiseDebugInfoEvent(DestinationAddress, "ID " + ResponseId + ": synchronous request (attempt " + num + ")");
				}
				J2534Error j2534Error = rollCallManager.Write(RequestMessage);
				if (j2534Error == J2534Error.NoError)
				{
					switch (WaitHandle.WaitAny(new WaitHandle[2] { closingEvent, Event }, synchronousRequestTimeout))
					{
					case 0:
						throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
					case 1:
						if (rollCallManager.debugLevel > 0)
						{
							rollCallManager.RaiseDebugInfoEvent(DestinationAddress, "ID " + ResponseId + ": synchronous response " + Acknowledgement);
						}
						switch (Acknowledgement)
						{
						case Acknowledgment.Negative:
						case Acknowledgment.AccessDenied:
							throw new CaesarException(SapiError.NegativeResponseMessageFromDevice);
						case Acknowledgment.Busy:
							ex = new CaesarException(SapiError.BusyResponseMessageFromDevice);
							if (closingEvent.WaitOne(betweenBusyRequestInterval))
							{
								throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
							}
							num2 = busyRetryCount;
							break;
						case Acknowledgment.Positive:
							return Response;
						}
						break;
					case 258:
						if (rollCallManager.debugLevel > 0)
						{
							rollCallManager.RaiseDebugInfoEvent(DestinationAddress, "ID " + ResponseId + ": synchronous timeout");
						}
						ex = new CaesarException(SapiError.TimeoutReceivingMessageFromDevice);
						break;
					}
					continue;
				}
				Sapi.GetSapi().RaiseDebugInfoEvent(string.Concat(rollCallManager.Protocol, "-", DestinationAddress), "ID " + ResponseId + ": Result from J2534.WriteMsgs is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
				throw new CaesarException(SapiError.CannotSendMessageToDevice);
			}
			throw ex;
		}

		internal bool BelongsTo(int id, byte destinationAddress, byte[] response)
		{
			if (DestinationAddress == destinationAddress && ResponseId == id)
			{
				if (response != null && additionalResponseCheck != null)
				{
					return additionalResponseCheck(response);
				}
				return true;
			}
			return false;
		}

		internal void Notify(byte[] response, Acknowledgment acknowledgment)
		{
			Response = response;
			Acknowledgement = acknowledgment;
			lock (eventLock)
			{
				if (!disposed && Event != null)
				{
					Event.Set();
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (!disposing)
			{
				return;
			}
			lock (eventLock)
			{
				if (Event != null)
				{
					Event.Set();
					Event.Close();
					Event = null;
				}
			}
		}
	}

	private const int J2534ConnectionRetryInterval = 2500;

	private const int PortIndex = 1;

	private static object stopStartLock = new object();

	private Dictionary<string, TranslationEntry> translation;

	private Dictionary<byte?, Dictionary<string, string>> suspectParametersByAddress;

	private Dictionary<string, string> commonSuspectParameters;

	private Dictionary<int, Tuple<string, string>> parameterGroups;

	private const int RetryCount = 3;

	protected const int BetweenBusyRequestInterval = 1000;

	protected const int BusyRetryCount = 5;

	protected const int SynchronousRequestTimeout = 2000;

	private int channelTimeout;

	protected uint channelId;

	private ushort? restrictedAddress;

	private IEnumerable<byte> clientRestrictedAddresses;

	private volatile int messagesThisLoadPeriod;

	private DateTime lastLoadMeasurement;

	private object queuedItemsLock = new object();

	private List<QueueItem> queuedItems = new List<QueueItem>();

	private MonitorData monitorData;

	protected override TimeSpan RollCallValidLastSeenTime => TimeSpan.FromSeconds(2.0);

	internal override Dictionary<string, TranslationEntry> Translations
	{
		get
		{
			if (translation == null)
			{
				translation = new DiagnosisProtocol(base.Protocol.ToString()).Translations;
			}
			return translation;
		}
	}

	public override IDictionary<string, string> SuspectParameters
	{
		get
		{
			if (commonSuspectParameters == null)
			{
				InitializeSuspectParameters();
			}
			return commonSuspectParameters;
		}
	}

	public IDictionary<int, Tuple<string, string>> ParameterGroups
	{
		get
		{
			if (parameterGroups == null)
			{
				parameterGroups = new Dictionary<int, Tuple<string, string>>();
				Dictionary<string, string> dictionary = Translations.Where((KeyValuePair<string, TranslationEntry> q) => q.Key.EndsWith(".PGN", StringComparison.Ordinal) || q.Key.EndsWith(".Acronym", StringComparison.Ordinal)).ToDictionary((KeyValuePair<string, TranslationEntry> k) => k.Key, (KeyValuePair<string, TranslationEntry> v) => v.Value.Translation);
				foreach (KeyValuePair<string, string> item2 in dictionary.Where((KeyValuePair<string, string> q) => q.Key.EndsWith(".PGN", StringComparison.Ordinal)))
				{
					string text = item2.Key.Split(".".ToCharArray())[0];
					string item = string.Empty;
					if (dictionary.TryGetValue(text + ".Acronym", out var value))
					{
						item = value;
					}
					parameterGroups.Add(Convert.ToInt32(text, CultureInfo.InvariantCulture), new Tuple<string, string>(item2.Value, item));
				}
			}
			return parameterGroups;
		}
	}

	public override IDictionary<int, string> ParameterGroupLabels
	{
		get
		{
			if (ParameterGroups == null)
			{
				return null;
			}
			return ParameterGroups.ToDictionary((KeyValuePair<int, Tuple<string, string>> k) => k.Key, (KeyValuePair<int, Tuple<string, string>> v) => v.Value.Item1);
		}
	}

	public override IDictionary<int, string> ParameterGroupAcronyms
	{
		get
		{
			if (ParameterGroups == null)
			{
				return null;
			}
			return ParameterGroups.ToDictionary((KeyValuePair<int, Tuple<string, string>> k) => k.Key, (KeyValuePair<int, Tuple<string, string>> v) => v.Value.Item2);
		}
	}

	protected abstract int BetweenGlobalIdRequestInterval { get; }

	protected abstract uint BaudRate { get; }

	protected abstract int TotalMessagesPerSecond { get; }

	internal override int ChannelTimeout
	{
		get
		{
			if (channelTimeout == 0)
			{
				channelTimeout = Math.Max(10000, CycleGlobalRequestIds.Count() * BetweenGlobalIdRequestInterval) + 2000;
			}
			return channelTimeout;
		}
	}

	protected abstract byte GlobalRequestAddress { get; }

	protected abstract IEnumerable<int> CycleGlobalRequestIds { get; }

	protected RollCallSae(Protocol ProtocolId)
		: base(ProtocolId)
	{
		string value = Sapi.GetSapi().ConfigurationItems["RollCallRestrict" + base.Protocol.ToString() + "Address"].Value;
		if (!string.IsNullOrEmpty(value) && ushort.TryParse(value, out var result))
		{
			restrictedAddress = result;
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "WARNING: Monitoring is restricted to address " + restrictedAddress);
		}
	}

	internal override void Init()
	{
		try
		{
			J2534Error j2534Error = Sid.Connect((ProtocolId)protocolId, 0u, ref channelId);
			if (j2534Error == J2534Error.NoError)
			{
				Sid.SetPassthruCallback(channelId, PassthruCallback);
				if (debugLevel > 0)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.Connect() is " + channelId);
				}
				base.State = ConnectionState.Initialized;
				bool isCapable = false;
				if (Sid.IsAutoBaudRateCapable(channelId, ref isCapable) == J2534Error.NoError)
				{
					base.IsAutoBaudRate = isCapable;
				}
				base.DeviceName = Sid.GetDeviceName(channelId);
				UpdateDeviceVersionInfo();
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.Connect() is " + j2534Error);
				base.State = ConnectionState.NotInitialized;
			}
		}
		catch (SEHException)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Roll-call services are unavailable as SID is not loaded.");
			base.State = ConnectionState.NotInitialized;
		}
	}

	private void UpdateDeviceVersionInfo()
	{
		string libraryName = null;
		string libraryVersion = null;
		string driverVersion = null;
		string firmwareVersion = null;
		string supportedProtocols = null;
		Sid.GetDeviceVersionInfo(channelId, ref libraryName, ref libraryVersion, ref driverVersion, ref firmwareVersion, ref supportedProtocols);
		base.DeviceLibraryName = libraryName;
		base.DeviceLibraryVersion = libraryVersion;
		base.DeviceDriverVersion = driverVersion;
		base.DeviceFirmwareVersion = firmwareVersion;
		base.DeviceSupportedProtocols = supportedProtocols;
	}

	public override void SetRestrictedAddressList(IEnumerable<byte> restrictedSourceAddresses)
	{
		clientRestrictedAddresses = restrictedSourceAddresses;
	}

	internal override void Exit()
	{
		if (channelId != 0)
		{
			Sid.SetPassthruCallback(channelId, null);
			J2534Error j2534Error = Sid.Disconnect(channelId);
			if (j2534Error != J2534Error.NoError)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.Disconnect for channel " + channelId + " is " + j2534Error.ToString());
			}
			channelId = 0u;
			base.State = ConnectionState.NotInitialized;
			base.DeviceName = null;
			base.DeviceLibraryVersion = null;
			base.DeviceFirmwareVersion = null;
			base.DeviceDriverVersion = null;
			base.DeviceSupportedProtocols = null;
			base.IsAutoBaudRate = false;
		}
		monitorData = null;
		ClearEcus();
	}

	private ConnectionResource CreateConnectionResource(byte sourceAddress, int? function, bool overrideFunctionRequirement = false)
	{
		if (!RequiresFunction(sourceAddress) || function.HasValue || overrideFunctionRequirement)
		{
			return new ConnectionResource(GetEcu(sourceAddress, function), protocolId, base.DeviceName, BaudRate, 1, sourceAddress);
		}
		return null;
	}

	protected override IEnumerable<Ecu> LoadMonitorEcus()
	{
		monitorData = MonitorData.Load(this);
		return new List<Ecu>(monitorData.GetEcus(this));
	}

	protected void AddIdentification(byte sourceAddress, ID id, object value)
	{
		lock (addressInformation)
		{
			RollCall.ChannelInformation channelInformation = addressInformation[sourceAddress];
			if (channelInformation.Resource == null && id == ID.Function)
			{
				channelInformation.Resource = CreateConnectionResource(sourceAddress, (int)value);
			}
			if (channelInformation.Invalid)
			{
				return;
			}
			IdentificationInformation identificationInformation = channelInformation.Identification.Where((IdentificationInformation ci) => ci.Id == id).FirstOrDefault();
			if (identificationInformation != null)
			{
				identificationInformation.Value = value;
				if (debugLevel > 1)
				{
					RaiseDebugInfoEvent(sourceAddress, string.Concat("Got identification ", id, ": ", value));
				}
			}
		}
	}

	protected void SetDataStreamSpns(byte sourceAddress, int[] dataStreamSpns)
	{
		lock (addressInformation)
		{
			RollCall.ChannelInformation channelInformation = addressInformation[sourceAddress];
			if (!channelInformation.Invalid)
			{
				channelInformation.SetDataStreamSpns(dataStreamSpns);
			}
		}
	}

	internal override bool IsVirtual(int sourceAddress)
	{
		return GlobalRequestAddress == sourceAddress;
	}

	protected override void ThreadFunc()
	{
		Sapi sapi = Sapi.GetSapi();
		do
		{
			bool flag = false;
			lock (ecusLock)
			{
				flag = ecus != null;
			}
			if (flag && !sapi.Channels.Any((Channel c) => c.Ecu.RollCallManager == this))
			{
				IEnumerable<Tuple<byte[], byte[]>> enumerable = ((base.Protocol != Protocol.J1939 || clientRestrictedAddresses == null) ? Enumerable.Repeat(new Tuple<byte[], byte[]>(new byte[6], new byte[6]), 1) : clientRestrictedAddresses.Select((byte sa) => new Tuple<byte[], byte[]>(new byte[6] { 0, 0, 0, 0, 255, 0 }, new byte[6] { 0, 0, 0, 0, sa, 0 })));
				List<uint> list = new List<uint>();
				J2534Error j2534Error = J2534Error.NoError;
				lock (stopStartLock)
				{
					foreach (Tuple<byte[], byte[]> item in enumerable)
					{
						uint filterId = 0u;
						j2534Error = Sid.StartMsgFilter(channelId, FilterType.Pass, new PassThruMsg((ProtocolId)protocolId, item.Item1), new PassThruMsg((ProtocolId)protocolId, item.Item2), null, ref filterId);
						if (j2534Error != J2534Error.NoError)
						{
							break;
						}
						list.Add(filterId);
					}
				}
				if (j2534Error == J2534Error.NoError)
				{
					if (clientRestrictedAddresses == null || base.Protocol == Protocol.J1708)
					{
						Sid.SetAllFiltersToPass(channelId, 1);
					}
					UpdateDeviceVersionInfo();
					base.State = ConnectionState.TranslatorConnected;
					base.Load = 100f;
					StopReason stopReason = InnerConnectionLoop();
					Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "InnerConnectionLoop has stopped with reason " + stopReason);
					if (stopReason == StopReason.TranslatorDisconnected)
					{
						base.State = ConnectionState.TranslatorDisconnected;
					}
					base.Load = null;
					if (base.Protocol == Protocol.J1939 && stopReason == StopReason.TranslatorDisconnected)
					{
						foreach (Channel channel in sapi.Channels)
						{
							channel.Abort(stopReason);
						}
						sapi.Channels.AbortPendingConnections((ConnectionResource cr) => !cr.IsEthernet);
					}
					base.State = ConnectionState.WaitingForTranslator;
					foreach (uint item2 in list)
					{
						J2534Error j2534Error2;
						lock (stopStartLock)
						{
							j2534Error2 = Sid.StopMsgFilter(channelId, item2);
						}
						if (j2534Error2 != J2534Error.NoError)
						{
							Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.StopMsgFilter() is " + j2534Error2.ToString() + " GetLastError is " + Sid.GetLastError());
						}
					}
					ClearAddressInformation();
					continue;
				}
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.StartMsgFilter() is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
				if (base.Protocol == Protocol.J1939)
				{
					sapi.Channels.AbortPendingConnections((ConnectionResource cr) => !cr.IsEthernet);
				}
			}
			else if (flag)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Waiting for channels to go offline: " + string.Join(", ", from c in sapi.Channels
					where c.Ecu.RollCallManager == this
					select c.Ecu.Name));
			}
		}
		while (!closingEvent.WaitOne(2500));
	}

	protected virtual Presentation CreatePresentation(Ecu ecu, string qualifier)
	{
		return null;
	}

	internal override EcuInfo CreateEcuInfo(EcuInfoCollection ecuInfos, string qualifier)
	{
		int id = Convert.ToInt32(qualifier, CultureInfo.InvariantCulture);
		int num = MapIdToRequestId((ID)id);
		bool visible = IsRequestIdContentVisible(num);
		EcuInfo ecuInfo = new EcuInfo(ecuInfos.Channel, EcuInfoType.RollCall, qualifier, string.Empty, "Common", "Rollcall/Common", CreatePresentation(ecuInfos.Channel.Ecu, qualifier), num, visible, common: true, summary: true);
		ecuInfos.AddFromRollCall(ecuInfo);
		return ecuInfo;
	}

	internal override void CreateEcuInfos(EcuInfoCollection ecuInfos)
	{
		if (ecuInfos.Channel.Online)
		{
			lock (addressInformation)
			{
				if (addressInformation.ContainsKey(ecuInfos.Channel.SourceAddress.Value))
				{
					addressInformation[ecuInfos.Channel.SourceAddress.Value].CreateEcuInfos(ecuInfos);
				}
			}
		}
		else if (ecuInfos.Channel.LogFile == null)
		{
			foreach (ID identificationId in GetIdentificationIds(ecuInfos.Channel.SourceAddress))
			{
				CreateEcuInfo(ecuInfos, identificationId.ToNumberString());
			}
		}
		foreach (XElement definition in monitorData.GetDefinitions(ecuInfos.Channel, typeof(EcuInfo)))
		{
			Instrument instrument = new Instrument(ecuInfos.Channel, definition.Attribute("qualifier").Value, 0);
			Dictionary<string, string> dictionary = definition.Elements("Property").ToDictionary((XElement k) => k.Attribute("name").Value, (XElement v) => v.Value);
			instrument.AcquireFromRollCall(dictionary);
			string namedPropertyValue = dictionary.GetNamedPropertyValue("GroupQualifier", "StoredData");
			string namedPropertyValue2 = dictionary.GetNamedPropertyValue("GroupName", "Stored Data");
			bool namedPropertyValue3 = dictionary.GetNamedPropertyValue("Visible", defaultIfNotSet: false);
			bool namedPropertyValue4 = dictionary.GetNamedPropertyValue("Common", defaultIfNotSet: false);
			bool namedPropertyValue5 = dictionary.GetNamedPropertyValue("Summary", defaultIfNotSet: false);
			int? namedPropertyValue6 = dictionary.GetNamedPropertyValue<int?>("CacheTime", null);
			EcuInfo ecuInfo = new EcuInfo(ecuInfos.Channel, EcuInfoType.RollCall, instrument.Qualifier, instrument.Name, namedPropertyValue, namedPropertyValue2, instrument, instrument.MessageNumber.Value, namedPropertyValue3, namedPropertyValue4, namedPropertyValue5, namedPropertyValue6);
			ecuInfos.AddFromRollCall(ecuInfo);
		}
	}

	internal override IEnumerable<Instrument> CreateInstruments(Channel channel)
	{
		foreach (XElement definition in monitorData.GetDefinitions(channel, typeof(Instrument)))
		{
			Instrument instrument = new Instrument(channel, definition.Attribute("qualifier").Value, 0);
			instrument.AcquireFromRollCall(definition.Elements("Property").ToDictionary((XElement k) => k.Attribute("name").Value, (XElement v) => v.Value));
			yield return instrument;
		}
	}

	internal override Instrument CreateBaseInstrument(Channel channel, string qualifier)
	{
		Instrument instrument = new Instrument(channel, qualifier, 0);
		XElement baseDefinition = monitorData.GetBaseDefinition(qualifier, typeof(Instrument));
		if (baseDefinition != null)
		{
			instrument.AcquireFromRollCall(baseDefinition.Elements("Property").ToDictionary((XElement k) => k.Attribute("name").Value, (XElement v) => v.Value));
		}
		else
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["SlotType"] = "4";
			dictionary["Units"] = "raw";
			instrument.AcquireFromRollCall(dictionary);
		}
		return instrument;
	}

	internal override IEnumerable<Service> CreateServices(Channel channel, ServiceTypes type)
	{
		if (type == ServiceTypes.Environment)
		{
			Service service = new Service(channel, type, 1216.ToString());
			service.AcquireFromRollCall();
			yield return service;
			yield break;
		}
		foreach (XElement definition in monitorData.GetDefinitions(channel, typeof(Service)))
		{
			Service service2 = new Service(channel, ServiceTypes.Routine, definition.Attribute("qualifier").Value);
			service2.AcquireFromRollCall(definition.Elements("Property").ToDictionary((XElement k) => k.Attribute("name").Value, (XElement v) => v.Value));
			yield return service2;
		}
	}

	private StopReason InnerConnectionLoop()
	{
		int num = 0;
		List<int> list = CycleGlobalRequestIds.ToList();
		while (true)
		{
			if (base.State != ConnectionState.TranslatorConnectedNoTraffic)
			{
				lock (addressInformation)
				{
					bool flag = addressInformation.Count > 0;
					bool flag2 = addressInformation.Values.Any((RollCall.ChannelInformation ci) => ci.Resource != null && ci.Channel == null && !ci.Invalid);
					base.State = ((!flag || !base.ConnectEnabled) ? ConnectionState.TranslatorConnected : (flag2 ? ConnectionState.ChannelsConnecting : ConnectionState.ChannelsConnected));
					if (flag && !addressInformation.ContainsKey(GlobalRequestAddress))
					{
						addressInformation.Add(GlobalRequestAddress, new ChannelInformation(this, CreateConnectionResource(GlobalRequestAddress, null), new List<ID>()));
					}
				}
				int id = list[num++];
				if (num == list.Count)
				{
					num = 0;
				}
				if (!RequestId(id, GlobalRequestAddress))
				{
					return StopReason.TranslatorDisconnected;
				}
			}
			if (closingEvent.WaitOne(BetweenGlobalIdRequestInterval))
			{
				return StopReason.Closing;
			}
			UpdateLoadCalculation();
			if ((protocolId != Protocol.J1939 || addressInformation.Count <= 0 || base.Load != 0f) && base.State != ConnectionState.TranslatorConnectedNoTraffic)
			{
				continue;
			}
			byte hardwareStatus = 0;
			byte protocolStatusJ = 0;
			byte protocolStatusJ2 = 0;
			byte protocolStatusCan = 0;
			byte protocolStatusIso = 0;
			if (Sid.GetHardwareStatus(channelId, ref hardwareStatus, ref protocolStatusJ, ref protocolStatusJ2, ref protocolStatusCan, ref protocolStatusIso) != J2534Error.NoError)
			{
				continue;
			}
			if ((hardwareStatus & 1) == 0)
			{
				break;
			}
			CheckStatusAndAbortChannels(protocolStatusJ, RP1210ProtocolId.J1939);
			CheckStatusAndAbortChannels(protocolStatusIso, RP1210ProtocolId.Iso15765);
			CheckStatusAndAbortChannels(protocolStatusCan, RP1210ProtocolId.Can);
			if ((protocolStatusJ & 2) == 0)
			{
				if (base.State != ConnectionState.TranslatorConnectedNoTraffic)
				{
					base.State = ConnectionState.TranslatorConnectedNoTraffic;
					ClearAddressInformation();
				}
			}
			else if (base.State == ConnectionState.TranslatorConnectedNoTraffic)
			{
				base.State = ConnectionState.TranslatorConnected;
			}
		}
		Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "The hardware is not connected, according to GetHardwareStatus. Channels will be aborted.");
		return StopReason.TranslatorDisconnected;
	}

	private void CheckStatusAndAbortChannels(byte statusByte, RP1210ProtocolId protocol)
	{
		if ((statusByte & 2) != 0)
		{
			return;
		}
		List<Channel> list = Sapi.GetSapi().Channels.Where((Channel c) => c.RP1210Protocol == protocol).ToList();
		if (list.Count > 0)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Concat("Traffic stopped for ", protocol, "; aborting channels ", string.Join(" ", list.Select((Channel c) => c.Ecu.Name).ToArray())));
			list.ForEach(delegate(Channel c)
			{
				c.Abort(StopReason.NoTraffic);
			});
		}
	}

	private void UpdateLoadCalculation()
	{
		double totalSeconds = (Sapi.Now - lastLoadMeasurement).TotalSeconds;
		int num = messagesThisLoadPeriod;
		messagesThisLoadPeriod = 0;
		lastLoadMeasurement = Sapi.Now;
		double num2 = totalSeconds * (double)TotalMessagesPerSecond;
		base.Load = (float)num / (float)num2 * 100f;
	}

	internal void PassthruCallback(PassThruMsg message)
	{
		if (!base.ProtocolAlive)
		{
			return;
		}
		messagesThisLoadPeriod++;
		byte[] data = message.GetData();
		if (!TryExtractMessage(data, out var address, out var id, out data) || (restrictedAddress.HasValue && address != restrictedAddress.Value) || (clientRestrictedAddresses != null && !clientRestrictedAddresses.Contains(address)) || !base.ConnectEnabled)
		{
			return;
		}
		Channel channel = null;
		lock (addressInformation)
		{
			if (!addressInformation.ContainsKey(address))
			{
				addressInformation.Add(address, new ChannelInformation(this, CreateConnectionResource(address, null), GetIdentificationIds(address)));
				if (debugLevel > 1)
				{
					RaiseDebugInfoEvent(address, "Adding (connection) channel");
				}
			}
			else
			{
				ChannelInformation channelInformation = (ChannelInformation)addressInformation[address];
				channelInformation.UpdateLastSeen();
				channel = channelInformation.Channel;
				if (channelInformation.Resource == null && RequiresFunction(address) && channelInformation.TimeSinceFirstSeen > TimeSpan.FromSeconds(30.0))
				{
					channelInformation.Resource = CreateConnectionResource(address, null, overrideFunctionRequirement: true);
				}
			}
		}
		HandleIncomingMessage(address, id, data, channel);
	}

	public override void WriteTranslationFile(CultureInfo culture, IEnumerable<TranslationEntry> translations)
	{
		TranslationEntry.WriteTranslationFile(base.Protocol.ToString(), culture, string.Empty, translations, emitEmptyTranslations: true);
	}

	private void InitializeSuspectParameters()
	{
		suspectParametersByAddress = new Dictionary<byte?, Dictionary<string, string>>();
		commonSuspectParameters = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item in Translations.Where((KeyValuePair<string, TranslationEntry> q) => q.Key.EndsWith(".SPN", StringComparison.Ordinal)).ToDictionary((KeyValuePair<string, TranslationEntry> k) => k.Key, (KeyValuePair<string, TranslationEntry> v) => v.Value.Translation))
		{
			string[] array = item.Key.Split(".".ToCharArray());
			string key = array[0];
			byte? b = null;
			Dictionary<string, string> value = commonSuspectParameters;
			if (array.Length > 2 && byte.TryParse(array[1], out var result))
			{
				b = result;
				if (!suspectParametersByAddress.TryGetValue(b, out value))
				{
					value = new Dictionary<string, string>();
					suspectParametersByAddress[b] = value;
				}
			}
			if (!value.ContainsKey(key))
			{
				value.Add(key, item.Value);
			}
		}
	}

	internal override Dictionary<string, string> GetSuspectParametersForEcu(Ecu ecu)
	{
		if (suspectParametersByAddress == null)
		{
			InitializeSuspectParameters();
		}
		if (ecu.SourceAddress.HasValue && suspectParametersByAddress.ContainsKey(ecu.SourceAddress))
		{
			return suspectParametersByAddress[ecu.SourceAddress];
		}
		return null;
	}

	protected abstract bool TryExtractMessage(byte[] source, out byte address, out int id, out byte[] data);

	protected abstract void HandleIncomingMessage(byte address, int id, byte[] data, Channel channel);

	internal virtual bool IsRequestIdContentVisible(int id)
	{
		return true;
	}

	internal override void ReadEcuInfo(EcuInfo ecuInfo)
	{
		RequestAndWait(ecuInfo.MessageNumber.Value, ecuInfo.Channel.SourceAddress.Value);
	}

	protected abstract PassThruMsg CreateRequestMessage(int id, byte destinationAddress);

	private bool RequestId(int id, byte destinationAddress)
	{
		J2534Error j2534Error = Write(CreateRequestMessage(id, destinationAddress));
		if (j2534Error != J2534Error.NoError)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(string.Concat(protocolId, "-", destinationAddress), "ID " + id + ": Result from J2534.WriteMsgs is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
		}
		return j2534Error == J2534Error.NoError;
	}

	protected J2534Error Write(PassThruMsg requestMessage)
	{
		return Sid.WriteMsgs(channelId, new List<PassThruMsg> { requestMessage }, 0u);
	}

	protected virtual byte[] RequestAndWait(int id, byte destinationAddress)
	{
		return RequestAndWait(new QueueItem(CreateRequestMessage(id, destinationAddress), id, destinationAddress));
	}

	protected byte[] RequestAndWait(QueueItem queueItem)
	{
		lock (queuedItemsLock)
		{
			queuedItems.Add(queueItem);
		}
		try
		{
			return queueItem.Request(this);
		}
		finally
		{
			lock (queuedItemsLock)
			{
				queuedItems.Remove(queueItem);
				queueItem.Dispose();
			}
		}
	}

	protected void NotifyQueueItem(int id, byte destinationAddress, byte[] response, Acknowledgment acknowledgement)
	{
		QueueItem queueItem = null;
		lock (queuedItemsLock)
		{
			if (queuedItems.Count > 0)
			{
				queueItem = queuedItems.FirstOrDefault((QueueItem qi) => qi.BelongsTo(id, destinationAddress, response));
			}
		}
		queueItem?.Notify(response, acknowledgement);
	}

	protected abstract IEnumerable<ID> GetIdentificationIds(byte? address);

	protected abstract bool RequiresFunction(byte address);

	protected abstract int MapIdToRequestId(ID id);

	protected override void RaiseDebugInfoEvent(int sourceAddress, string text)
	{
		string text2 = ((sourceAddress == GlobalRequestAddress) ? "(global)" : sourceAddress.ToString(CultureInfo.InvariantCulture));
		Sapi.GetSapi().RaiseDebugInfoEvent(string.Concat(protocolId, "-", text2), text);
	}

	protected override void DisposeInternal()
	{
		lock (queuedItemsLock)
		{
			foreach (QueueItem queuedItem in queuedItems)
			{
				queuedItem.Dispose();
			}
			queuedItems.Clear();
		}
	}
}
