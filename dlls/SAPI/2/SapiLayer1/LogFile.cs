using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class LogFile : IDisposable
{
	private class LoadTask
	{
		public Task Task;

		public Queue<Action> ProcessingQueue;

		public LoadTask(ManualResetEvent doneEvent)
		{
			LoadTask loadTask = this;
			ProcessingQueue = new Queue<Action>();
			Task = Task.Run(delegate
			{
				while (true)
				{
					if (loadTask.ProcessingQueue.Count > 0)
					{
						loadTask.ProcessingQueue.Dequeue()();
					}
					else if (doneEvent.WaitOne(1000) && loadTask.ProcessingQueue.Count == 0)
					{
						break;
					}
				}
			});
		}
	}

	internal const int CurrentVersion = 17;

	internal static LogFileFormatTagCollection CurrentFormat = new LogFileFormatTagCollection(17);

	internal const int LogFileUpdateInterval = 250;

	private DateTime lastLabelWriteTime;

	private Thread thread;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime currentTime;

	private DateTime actualPlaybackStart;

	private DateTime logFilePlaybackStart;

	private DateTime lastEvent;

	private volatile bool closing;

	private bool playing;

	private double playbackSpeed;

	private object currentTimeLock;

	private LabelCollection labels;

	private StringDictionary systemInfo;

	private Stream logFileStream;

	private bool streamOwner;

	private bool summary;

	private bool recording;

	private bool disposed;

	private string name;

	private LogFileCollection logFiles;

	private LogFileChannelCollection activeChannels;

	private ChannelBaseCollection allChannels;

	public IEnumerable<string> MissingEcuList { get; private set; }

	public IEnumerable<string> MissingVariantList { get; private set; }

	public IEnumerable<string> MissingQualifierList { get; private set; }

	public int FormatVersion { get; private set; }

	public bool Summary
	{
		get
		{
			return summary;
		}
		internal set
		{
			summary = value;
		}
	}

	public string Name => name;

	public LogFileChannelCollection Channels => activeChannels;

	public ChannelBaseCollection AllChannels => allChannels;

	public LabelCollection Labels => labels;

	public DateTime StartTime => startTime;

	public DateTime EndTime => endTime;

	public bool Playing
	{
		get
		{
			return playing;
		}
		set
		{
			ThrowIfRecording();
			if (thread == null)
			{
				thread = new Thread(ThreadFunc);
				thread.Start();
			}
			if (value)
			{
				lock (currentTimeLock)
				{
					actualPlaybackStart = Sapi.Now;
					logFilePlaybackStart = currentTime;
				}
			}
			if (value != playing)
			{
				playing = value;
				FireAndForget.Invoke(this.LogFilePlaybackStateUpdateEvent, this, new EventArgs());
				for (int i = 0; i < activeChannels.Count; i++)
				{
					activeChannels[i].SetCommunicationsState(playing ? CommunicationsState.LogFilePlayback : CommunicationsState.LogFilePaused);
				}
			}
		}
	}

	public DateTime CurrentTime
	{
		get
		{
			return currentTime;
		}
		set
		{
			ThrowIfRecording();
			lock (currentTimeLock)
			{
				currentTime = value;
				actualPlaybackStart = Sapi.Now;
				logFilePlaybackStart = currentTime;
			}
		}
	}

	public bool AtEnd => currentTime >= endTime;

	public double PlaybackSpeed
	{
		get
		{
			return playbackSpeed;
		}
		set
		{
			ThrowIfRecording();
			lock (currentTimeLock)
			{
				actualPlaybackStart = Sapi.Now;
				logFilePlaybackStart = currentTime;
				playbackSpeed = value;
			}
		}
	}

	public StringDictionary SystemInformation => systemInfo;

	public bool Recording => recording;

	public event LogFilePlaybackUpdateEventHandler LogFilePlaybackUpdateEvent;

	public event LogFilePlaybackStateUpdateEventHandler LogFilePlaybackStateUpdateEvent;

	internal LogFile(LogFileCollection logFiles, string name)
	{
		this.logFiles = logFiles;
		this.name = name;
		closing = false;
		playbackSpeed = 1.0;
		startTime = DateTime.MinValue;
		endTime = DateTime.MinValue;
		currentTime = DateTime.MinValue;
		actualPlaybackStart = DateTime.MinValue;
		logFilePlaybackStart = DateTime.MinValue;
		lastEvent = DateTime.MinValue;
		labels = new LabelCollection(this);
		activeChannels = new LogFileChannelCollection(this);
		allChannels = new LogFileChannelCollection(this);
		currentTimeLock = new object();
		systemInfo = new StringDictionary();
	}

	internal LogFile(LogFileCollection logFiles, Stream stream, string name, bool summary)
	{
		this.logFiles = logFiles;
		this.name = name;
		closing = false;
		playbackSpeed = 1.0;
		startTime = DateTime.MinValue;
		endTime = DateTime.MinValue;
		currentTime = DateTime.MinValue;
		actualPlaybackStart = DateTime.MinValue;
		logFilePlaybackStart = DateTime.MinValue;
		lastEvent = DateTime.MinValue;
		this.summary = summary;
		logFileStream = stream;
		labels = new LabelCollection(this);
	}

	internal void Read(XmlReader xmlReader, int version)
	{
		List<string> missingEcuList = new List<string>();
		List<string> missingVariantList = new List<string>();
		List<string> missingQualifierList = new List<string>();
		LogFileFormatTagCollection format = new LogFileFormatTagCollection(version);
		FormatVersion = version;
		xmlReader.Read();
		if (xmlReader.LocalName == "CBFVersions")
		{
			xmlReader.Skip();
		}
		if (xmlReader.LocalName == format[TagName.System].LocalName)
		{
			if (xmlReader.ReadToDescendant(format[TagName.Information].LocalName))
			{
				do
				{
					XElement xElement = (XElement)XNode.ReadFrom(xmlReader);
					systemInfo.Add(xElement.Attribute(format[TagName.Name]).Value, xElement.Attribute(format[TagName.Value]).Value);
				}
				while (xmlReader.NodeType == XmlNodeType.Element);
			}
			xmlReader.Skip();
		}
		object ecusLock = new object();
		object allChannelsLock = new object();
		object missingInfoLock = new object();
		ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		Dictionary<string, LoadTask> dictionary = new Dictionary<string, LoadTask>();
		try
		{
			if (xmlReader.LocalName == format[TagName.Channels].LocalName)
			{
				if (xmlReader.ReadToDescendant(format[TagName.Channel].LocalName))
				{
					do
					{
						XElement channelElement = (XElement)XNode.ReadFrom(xmlReader);
						string value = channelElement.Elements(format[TagName.EcuName]).First().Value;
						if (!dictionary.TryGetValue(value, out var value2))
						{
							dictionary.Add(value, value2 = new LoadTask(manualResetEvent));
						}
						value2.ProcessingQueue.Enqueue(delegate
						{
							Channel.LoadFromLog(channelElement, format, this, ecusLock, allChannelsLock, missingEcuList, missingVariantList, missingQualifierList, missingInfoLock);
						});
					}
					while (xmlReader.NodeType == XmlNodeType.Element);
				}
				xmlReader.Skip();
			}
		}
		catch (XmlException)
		{
			Close();
			throw;
		}
		manualResetEvent.Set();
		try
		{
			Task.WaitAll(dictionary.Values.Select((LoadTask v) => v.Task).ToArray());
		}
		catch (AggregateException ex2)
		{
			Close();
			ExceptionDispatchInfo.Capture(ex2.InnerExceptions.First()).Throw();
		}
		for (int num = 0; num < allChannels.Count; num++)
		{
			Channel channel = allChannels[num];
			DateTime dateTime = channel.StartTime;
			DateTime dateTime2 = channel.EndTime;
			if (dateTime != DateTime.MinValue && (startTime == DateTime.MinValue || dateTime < startTime))
			{
				startTime = dateTime;
			}
			if (dateTime2 > endTime)
			{
				endTime = dateTime2;
			}
		}
		labels.Add(new Label("Log File Start", startTime, null));
		if (xmlReader.LocalName == format[TagName.Labels].LocalName)
		{
			if (xmlReader.ReadToDescendant(format[TagName.Label].LocalName))
			{
				do
				{
					Label label = Label.FromXElement((XElement)XNode.ReadFrom(xmlReader), format, allChannels);
					if (label.Channel != null && Service.IsServiceLabel(label, out var serviceName))
					{
						CommunicationsStateValue communicationsStateValue = label.Channel.CommunicationsStateValues.GetCurrentAtTime(label.Time);
						if (communicationsStateValue != null && communicationsStateValue.Value == CommunicationsState.Online)
						{
							int num2 = label.Channel.CommunicationsStateValues.IndexOf(communicationsStateValue);
							if (num2 > 0)
							{
								communicationsStateValue = label.Channel.CommunicationsStateValues[num2 - 1];
							}
						}
						if (communicationsStateValue != null && communicationsStateValue.Value == CommunicationsState.ExecuteService)
						{
							Service service = label.Channel.Services[communicationsStateValue.Additional];
							if (service != null)
							{
								ServiceExecution serviceExecution = service.Executions.FirstOrDefault((ServiceExecution serviceExecution2) => serviceExecution2.EndTime == label.Time);
								if (serviceExecution == null)
								{
									service.ParseFromLog(label.Name.Substring(serviceName.Length), communicationsStateValue.Time, label.Time);
								}
								else
								{
									label = serviceExecution.CreateLabel();
								}
							}
						}
					}
					labels.Add(label);
				}
				while (xmlReader.NodeType == XmlNodeType.Element);
			}
			xmlReader.Skip();
		}
		labels.Add(new Label("Log File End", endTime, null));
		currentTime = startTime;
		MissingEcuList = missingEcuList;
		MissingVariantList = missingVariantList;
		MissingQualifierList = missingQualifierList;
	}

	internal void RaiseLogFileUpdate()
	{
		if (!closing)
		{
			FireAndForget.Invoke(this.LogFilePlaybackUpdateEvent, this, new EventArgs());
		}
	}

	internal void StartLogging()
	{
		ThrowIfRecording();
		startTime = Sapi.Now;
		if (logFileStream == null && !string.IsNullOrEmpty(name))
		{
			logFileStream = new FileStream(name, FileMode.OpenOrCreate);
			streamOwner = true;
		}
		recording = true;
	}

	internal void LogChannel(Channel c)
	{
		if (logFileStream != null)
		{
			UpdateStreamXml(new List<Channel>(new Channel[1] { c }));
		}
	}

	internal void LabelLog(Label label)
	{
		labels.Add(label);
	}

	public void Close()
	{
		closing = true;
		if (!recording)
		{
			if (thread != null)
			{
				thread.Join();
			}
			while (allChannels.Count > 0)
			{
				allChannels[0].InternalDisconnect();
			}
		}
		else
		{
			StopLogging();
		}
		logFiles.Remove(this);
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
			Close();
		}
		disposed = true;
	}

	internal bool StopLogging()
	{
		bool result = false;
		if (logFileStream != null)
		{
			IEnumerable<Channel> enumerable = Sapi.GetSapi().Channels.Where((Channel x) => x.Sessions.Count > 0);
			if (enumerable.Any())
			{
				UpdateStreamXml(enumerable);
				result = true;
			}
			if (streamOwner)
			{
				logFileStream.Close();
				logFileStream.Dispose();
			}
			logFileStream = null;
		}
		name = null;
		return result;
	}

	public static IList<LogMetadataItem> ExtractMetadata(Stream logFileStream)
	{
		LogFileFormatTagCollection currentFormat = CurrentFormat;
		List<LogMetadataItem> list = new List<LogMetadataItem>();
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreWhitespace = true;
		xmlReaderSettings.IgnoreComments = true;
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.CheckCharacters = false;
		XmlReader xmlReader = XmlReader.Create(logFileStream, xmlReaderSettings);
		if (xmlReader.ReadToFollowing(currentFormat[TagName.Channels].LocalName) && xmlReader.ReadToDescendant(currentFormat[TagName.Channel].LocalName))
		{
			do
			{
				Channel.ExtractMetadata(xmlReader, list);
			}
			while (xmlReader.NodeType == XmlNodeType.Element);
		}
		if (xmlReader.ReadToFollowing(currentFormat[TagName.Labels].LocalName) && xmlReader.ReadToDescendant(currentFormat[TagName.Label].LocalName))
		{
			do
			{
				list.Add(Label.ExtractMetadata(xmlReader));
			}
			while (xmlReader.NodeType == XmlNodeType.Element);
		}
		return list;
	}

	public void ExtractPartialLog(DateTime startTime, DateTime endTime, Stream stream)
	{
		ThrowIfRecording();
		string tempFileName = GetTempFileName();
		try
		{
			using FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
			using XmlTextWriter xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
			WriteLogFileContent(null, allChannels, xmlWriter, startTime, endTime);
			fileStream.CopyAllTo(stream);
		}
		finally
		{
			File.Delete(tempFileName);
		}
	}

	private void ThrowIfRecording()
	{
		if (recording)
		{
			throw new InvalidOperationException("Invalid operation for a log file that is being recorded");
		}
	}

	private void UpdateStreamXml(IEnumerable<Channel> channels)
	{
		XmlTextReader xmlReader = null;
		if (logFileStream.CanRead && logFileStream.Length > 0)
		{
			logFileStream.Position = 0L;
			xmlReader = new XmlTextReader(logFileStream);
		}
		try
		{
			UpdateStreamXml(channels, xmlReader);
		}
		catch (XmlException)
		{
			logFileStream.SetLength(0L);
			UpdateStreamXml(channels, null);
		}
	}

	private void UpdateStreamXml(IEnumerable<Channel> channels, XmlTextReader xmlReader)
	{
		string tempFileName = GetTempFileName();
		try
		{
			using FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
			using XmlTextWriter xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
			WriteLogFileContent(xmlReader, channels, xmlWriter, DateTime.MinValue, DateTime.MinValue);
			fileStream.CopyAllTo(logFileStream);
		}
		catch (XmlException)
		{
			throw;
		}
		finally
		{
			File.Delete(tempFileName);
		}
	}

	private void WriteLogFileContent(XmlTextReader previousContentXmlReader, IEnumerable<Channel> channels, XmlTextWriter xmlWriter, DateTime startTime, DateTime endTime)
	{
		LogFileFormatTagCollection currentFormat = CurrentFormat;
		xmlWriter.Formatting = Formatting.Indented;
		xmlWriter.WriteStartDocument();
		Sapi.GetSapi().InitSapiXmlFile(xmlWriter, "Log", 17, this.startTime, addCBFVersions: true);
		XElement xElement = new XElement(currentFormat[TagName.System], CreateInformationNode("MachineName", Environment.MachineName), CreateInformationNode("Domain", Environment.UserDomainName), CreateInformationNode("User", Environment.UserName), CreateInformationNode("64BitOS", Environment.Is64BitOperatingSystem.ToString()), CreateInformationNode("64BitProcess", Environment.Is64BitProcess.ToString()), CreateInformationNode("AppName", Assembly.GetEntryAssembly().GetName().Name.ToString()), CreateInformationNode("AppVersion", Assembly.GetEntryAssembly().GetName().Version.ToString()), CreateInformationNode("SapiVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString()), CreateInformationNode("ToolId", (Sapi.GetSapi().ToolId != null) ? BitConverter.ToString(Sapi.GetSapi().ToolId.ToArray()) : "<none>"));
		IRollCall manager = RollCall.GetManager(Protocol.J1939);
		if (manager != null && manager.DeviceLibraryName != null && manager.DeviceLibraryVersion != null)
		{
			xElement.Add(CreateInformationNode("RP1210Library", manager.DeviceLibraryName));
			xElement.Add(CreateInformationNode("RP1210LibraryVersion", manager.DeviceLibraryVersion));
			xElement.Add(CreateInformationNode("RP1210FirmwareVersion", manager.DeviceFirmwareVersion));
			xElement.Add(CreateInformationNode("RP1210DriverVersion", manager.DeviceDriverVersion));
		}
		xElement.WriteTo(xmlWriter);
		xmlWriter.WriteStartElement(currentFormat[TagName.Channels].LocalName);
		previousContentXmlReader?.CopyChildrenTo(xmlWriter, currentFormat[TagName.Channels].LocalName, currentFormat[TagName.Channel].LocalName);
		foreach (Channel channel in channels)
		{
			if (recording)
			{
				if (summary)
				{
					channel.WriteSummaryXmlTo(xmlWriter);
				}
				else
				{
					channel.WriteXmlTo(xmlWriter);
				}
			}
			else if (!(channel.EndTime < startTime) && !(StartTime > endTime))
			{
				channel.WriteXmlTo(all: true, startTime, endTime, xmlWriter);
			}
		}
		xmlWriter.WriteEndElement();
		xmlWriter.WriteStartElement(currentFormat[TagName.Labels].LocalName);
		previousContentXmlReader?.CopyChildrenTo(xmlWriter, currentFormat[TagName.Labels].LocalName, currentFormat[TagName.Label].LocalName);
		DateTime dateTime = DateTime.MinValue;
		foreach (Label label in labels)
		{
			if ((recording && label.Time > lastLabelWriteTime) || (label.Time >= startTime && label.Time <= endTime))
			{
				label.XElement.WriteTo(xmlWriter);
				dateTime = label.Time;
			}
		}
		xmlWriter.WriteEndElement();
		if (recording && dateTime != DateTime.MinValue)
		{
			lastLabelWriteTime = dateTime;
		}
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Flush();
	}

	private static XElement CreateInformationNode(string name, string value)
	{
		LogFileFormatTagCollection currentFormat = CurrentFormat;
		return new XElement(currentFormat[TagName.Information], new XAttribute(currentFormat[TagName.Name], name), new XAttribute(currentFormat[TagName.Value], value));
	}

	private void ThreadFunc()
	{
		DateTime minValue = DateTime.MinValue;
		Dictionary<Channel, IEnumerable<Service>> dictionary = new Dictionary<Channel, IEnumerable<Service>>();
		Dictionary<Channel, IEnumerable<ParameterGroup>> dictionary2 = new Dictionary<Channel, IEnumerable<ParameterGroup>>();
		Label label = null;
		try
		{
			while (!closing)
			{
				if (Playing)
				{
					lock (currentTimeLock)
					{
						double value = (Sapi.Now - actualPlaybackStart).TotalMilliseconds * playbackSpeed;
						currentTime = logFilePlaybackStart + TimeSpan.FromMilliseconds(value);
					}
				}
				if (currentTime != minValue)
				{
					minValue = currentTime;
					List<Channel> list = activeChannels.ToList();
					lock (activeChannels.SyncRoot)
					{
						for (int i = 0; i < allChannels.Count; i++)
						{
							Channel channel = allChannels[i];
							if (channel.ActiveAtTime(currentTime))
							{
								if (!activeChannels.ChannelExists(channel))
								{
									list.Add(channel);
									activeChannels.Add(channel);
									channel.SetCommunicationsState(CommunicationsState.LogFilePlayback);
								}
							}
							else if (activeChannels.ChannelExists(channel))
							{
								activeChannels.Remove(channel);
							}
						}
					}
					foreach (Channel item in list)
					{
						for (int j = 0; j < item.EcuInfos.Count; j++)
						{
							item.EcuInfos[j].EcuInfoValues.SetCurrentTime(currentTime);
						}
						for (int k = 0; k < item.Instruments.Count; k++)
						{
							item.Instruments[k].InstrumentValues.SetCurrentTime(currentTime);
						}
						if (!dictionary.TryGetValue(item, out var value2))
						{
							value2 = item.Services.Where((Service s) => s.Executions.Any());
							if (item.StructuredServices != null)
							{
								value2 = value2.Union(item.StructuredServices.Where((Service s) => s.Executions.Any()));
							}
							dictionary.Add(item, value2);
						}
						foreach (Service item2 in value2)
						{
							item2.Executions.SetCurrentTime(currentTime);
							foreach (ServiceInputValue inputValue in item2.InputValues)
							{
								inputValue.ArgumentValues.SetCurrentTime(currentTime);
							}
							foreach (ServiceOutputValue outputValue in item2.OutputValues)
							{
								outputValue.ArgumentValues.SetCurrentTime(currentTime);
							}
						}
						if (!dictionary2.TryGetValue(item, out var value3))
						{
							value3 = item.ParameterGroups.Where((ParameterGroup g) => g.CodingStringValues.Any() || g.Parameters.Any((Parameter s) => s.ParameterValues.Count > 0)).ToList();
							dictionary2.Add(item, value3);
						}
						foreach (ParameterGroup item3 in value3)
						{
							bool flag = item3.CodingStringValues.SetCurrentTime(currentTime);
							foreach (Parameter parameter in item3.Parameters)
							{
								flag |= parameter.ParameterValues.SetCurrentTime(currentTime);
							}
							if (!flag)
							{
								continue;
							}
							item.Parameters.UpdateGroupCodingStringFromLogFile(item3);
							foreach (Parameter parameter2 in item3.Parameters)
							{
								parameter2.RaiseParameterUpdateEvent(null);
							}
						}
						item.FaultCodes.SetCurrentTime(currentTime);
						item.CommunicationsStateValues.SetCurrentTime(currentTime);
						if (label != labels.Current)
						{
							label = labels.Current;
							lastEvent = DateTime.MinValue;
						}
					}
					if (activeChannels.Count == 0)
					{
						if (currentTime > endTime || currentTime < startTime)
						{
							Playing = false;
							if (currentTime < startTime)
							{
								currentTime = startTime;
							}
							if (currentTime > endTime)
							{
								currentTime = endTime;
							}
						}
						else
						{
							DateTime dateTime;
							if (playbackSpeed >= 0.0)
							{
								dateTime = endTime;
								for (int num = 0; num < allChannels.Count; num++)
								{
									Channel channel2 = allChannels[num];
									if (channel2.StartTime > currentTime && channel2.StartTime < dateTime)
									{
										dateTime = channel2.StartTime;
									}
								}
							}
							else
							{
								dateTime = startTime;
								for (int num2 = 0; num2 < allChannels.Count; num2++)
								{
									Channel channel3 = allChannels[num2];
									if (channel3.EndTime < currentTime && channel3.EndTime > dateTime)
									{
										dateTime = channel3.EndTime;
									}
								}
							}
							actualPlaybackStart = Sapi.Now;
							logFilePlaybackStart = (currentTime = dateTime);
						}
						lastEvent = DateTime.MinValue;
					}
					if ((Sapi.Now - lastEvent).TotalMilliseconds > 250.0)
					{
						RaiseLogFileUpdate();
						lastEvent = Sapi.Now;
					}
				}
				Thread.Sleep(5);
			}
		}
		catch (Exception e)
		{
			Playing = false;
			RaiseLogFileUpdate();
			Sapi.GetSapi().RaiseExceptionEvent(this, e);
		}
	}

	private static string GetTempFileName()
	{
		return Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), ".TMP"));
	}
}
