// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LogFile
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

#nullable disable
namespace SapiLayer1;

public sealed class LogFile : IDisposable
{
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

  public event LogFilePlaybackUpdateEventHandler LogFilePlaybackUpdateEvent;

  public event LogFilePlaybackStateUpdateEventHandler LogFilePlaybackStateUpdateEvent;

  internal LogFile(LogFileCollection logFiles, string name)
  {
    this.logFiles = logFiles;
    this.name = name;
    this.closing = false;
    this.playbackSpeed = 1.0;
    this.startTime = DateTime.MinValue;
    this.endTime = DateTime.MinValue;
    this.currentTime = DateTime.MinValue;
    this.actualPlaybackStart = DateTime.MinValue;
    this.logFilePlaybackStart = DateTime.MinValue;
    this.lastEvent = DateTime.MinValue;
    this.labels = new LabelCollection(this);
    this.activeChannels = new LogFileChannelCollection(this);
    this.allChannels = (ChannelBaseCollection) new LogFileChannelCollection(this);
    this.currentTimeLock = new object();
    this.systemInfo = new StringDictionary();
  }

  internal LogFile(LogFileCollection logFiles, Stream stream, string name, bool summary)
  {
    this.logFiles = logFiles;
    this.name = name;
    this.closing = false;
    this.playbackSpeed = 1.0;
    this.startTime = DateTime.MinValue;
    this.endTime = DateTime.MinValue;
    this.currentTime = DateTime.MinValue;
    this.actualPlaybackStart = DateTime.MinValue;
    this.logFilePlaybackStart = DateTime.MinValue;
    this.lastEvent = DateTime.MinValue;
    this.summary = summary;
    this.logFileStream = stream;
    this.labels = new LabelCollection(this);
  }

  internal void Read(XmlReader xmlReader, int version)
  {
    List<string> missingEcuList = new List<string>();
    List<string> missingVariantList = new List<string>();
    List<string> missingQualifierList = new List<string>();
    LogFileFormatTagCollection format = new LogFileFormatTagCollection(version);
    this.FormatVersion = version;
    xmlReader.Read();
    if (xmlReader.LocalName == "CBFVersions")
      xmlReader.Skip();
    if (xmlReader.LocalName == format[TagName.System].LocalName)
    {
      if (xmlReader.ReadToDescendant(format[TagName.Information].LocalName))
      {
        do
        {
          XElement xelement = (XElement) XNode.ReadFrom(xmlReader);
          this.systemInfo.Add(xelement.Attribute(format[TagName.Name]).Value, xelement.Attribute(format[TagName.Value]).Value);
        }
        while (xmlReader.NodeType == XmlNodeType.Element);
      }
      xmlReader.Skip();
    }
    object ecusLock = new object();
    object allChannelsLock = new object();
    object missingInfoLock = new object();
    ManualResetEvent doneEvent = new ManualResetEvent(false);
    Dictionary<string, LogFile.LoadTask> dictionary = new Dictionary<string, LogFile.LoadTask>();
    try
    {
      if (xmlReader.LocalName == format[TagName.Channels].LocalName)
      {
        if (xmlReader.ReadToDescendant(format[TagName.Channel].LocalName))
        {
          do
          {
            XElement channelElement = (XElement) XNode.ReadFrom(xmlReader);
            string key = channelElement.Elements(format[TagName.EcuName]).First<XElement>().Value;
            LogFile.LoadTask loadTask;
            if (!dictionary.TryGetValue(key, out loadTask))
              dictionary.Add(key, loadTask = new LogFile.LoadTask(doneEvent));
            loadTask.ProcessingQueue.Enqueue((Action) (() => Channel.LoadFromLog(channelElement, format, this, ecusLock, allChannelsLock, missingEcuList, missingVariantList, missingQualifierList, missingInfoLock)));
          }
          while (xmlReader.NodeType == XmlNodeType.Element);
        }
        xmlReader.Skip();
      }
    }
    catch (XmlException ex)
    {
      this.Close();
      throw;
    }
    doneEvent.Set();
    try
    {
      Task.WaitAll(dictionary.Values.Select<LogFile.LoadTask, Task>((Func<LogFile.LoadTask, Task>) (v => v.Task)).ToArray<Task>());
    }
    catch (AggregateException ex)
    {
      this.Close();
      ExceptionDispatchInfo.Capture(ex.InnerExceptions.First<Exception>()).Throw();
    }
    for (int index = 0; index < this.allChannels.Count; ++index)
    {
      Channel allChannel = this.allChannels[index];
      DateTime startTime = allChannel.StartTime;
      DateTime endTime = allChannel.EndTime;
      if (startTime != DateTime.MinValue && (this.startTime == DateTime.MinValue || startTime < this.startTime))
        this.startTime = startTime;
      if (endTime > this.endTime)
        this.endTime = endTime;
    }
    this.labels.Add(new Label("Log File Start", this.startTime, (Ecu) null));
    if (xmlReader.LocalName == format[TagName.Labels].LocalName)
    {
      if (xmlReader.ReadToDescendant(format[TagName.Label].LocalName))
      {
        do
        {
          Label label = Label.FromXElement((XElement) XNode.ReadFrom(xmlReader), format, this.allChannels);
          string serviceName;
          if (label.Channel != null && Service.IsServiceLabel(label, out serviceName))
          {
            CommunicationsStateValue communicationsStateValue = label.Channel.CommunicationsStateValues.GetCurrentAtTime(label.Time);
            if (communicationsStateValue != null && communicationsStateValue.Value == CommunicationsState.Online)
            {
              int num = label.Channel.CommunicationsStateValues.IndexOf(communicationsStateValue);
              if (num > 0)
                communicationsStateValue = label.Channel.CommunicationsStateValues[num - 1];
            }
            if (communicationsStateValue != null && communicationsStateValue.Value == CommunicationsState.ExecuteService)
            {
              Service service = label.Channel.Services[communicationsStateValue.Additional];
              if (service != (Service) null)
              {
                ServiceExecution serviceExecution = service.Executions.FirstOrDefault<ServiceExecution>((Func<ServiceExecution, bool>) (ex => ex.EndTime == label.Time));
                if (serviceExecution == null)
                  service.ParseFromLog(label.Name.Substring(serviceName.Length), communicationsStateValue.Time, label.Time);
                else
                  label = serviceExecution.CreateLabel();
              }
            }
          }
          this.labels.Add(label);
        }
        while (xmlReader.NodeType == XmlNodeType.Element);
      }
      xmlReader.Skip();
    }
    this.labels.Add(new Label("Log File End", this.endTime, (Ecu) null));
    this.currentTime = this.startTime;
    this.MissingEcuList = (IEnumerable<string>) missingEcuList;
    this.MissingVariantList = (IEnumerable<string>) missingVariantList;
    this.MissingQualifierList = (IEnumerable<string>) missingQualifierList;
  }

  public IEnumerable<string> MissingEcuList { get; private set; }

  public IEnumerable<string> MissingVariantList { get; private set; }

  public IEnumerable<string> MissingQualifierList { get; private set; }

  public int FormatVersion { get; private set; }

  internal void RaiseLogFileUpdate()
  {
    if (this.closing)
      return;
    FireAndForget.Invoke((MulticastDelegate) this.LogFilePlaybackUpdateEvent, (object) this, new EventArgs());
  }

  internal void StartLogging()
  {
    this.ThrowIfRecording();
    this.startTime = Sapi.Now;
    if (this.logFileStream == null && !string.IsNullOrEmpty(this.name))
    {
      this.logFileStream = (Stream) new FileStream(this.name, FileMode.OpenOrCreate);
      this.streamOwner = true;
    }
    this.recording = true;
  }

  internal void LogChannel(Channel c)
  {
    if (this.logFileStream == null)
      return;
    this.UpdateStreamXml((IEnumerable<Channel>) new List<Channel>((IEnumerable<Channel>) new Channel[1]
    {
      c
    }));
  }

  internal void LabelLog(Label label) => this.labels.Add(label);

  public bool Summary
  {
    internal set => this.summary = value;
    get => this.summary;
  }

  public void Close()
  {
    this.closing = true;
    if (!this.recording)
    {
      if (this.thread != null)
        this.thread.Join();
      while (this.allChannels.Count > 0)
        this.allChannels[0].InternalDisconnect();
    }
    else
      this.StopLogging();
    this.logFiles.Remove(this);
  }

  public string Name => this.name;

  public LogFileChannelCollection Channels => this.activeChannels;

  public ChannelBaseCollection AllChannels => this.allChannels;

  public LabelCollection Labels => this.labels;

  public DateTime StartTime => this.startTime;

  public DateTime EndTime => this.endTime;

  public bool Playing
  {
    get => this.playing;
    set
    {
      this.ThrowIfRecording();
      if (this.thread == null)
      {
        this.thread = new Thread(new ThreadStart(this.ThreadFunc));
        this.thread.Start();
      }
      if (value)
      {
        lock (this.currentTimeLock)
        {
          this.actualPlaybackStart = Sapi.Now;
          this.logFilePlaybackStart = this.currentTime;
        }
      }
      if (value == this.playing)
        return;
      this.playing = value;
      FireAndForget.Invoke((MulticastDelegate) this.LogFilePlaybackStateUpdateEvent, (object) this, new EventArgs());
      for (int index = 0; index < this.activeChannels.Count; ++index)
        this.activeChannels[index].SetCommunicationsState(this.playing ? CommunicationsState.LogFilePlayback : CommunicationsState.LogFilePaused);
    }
  }

  public DateTime CurrentTime
  {
    get => this.currentTime;
    set
    {
      this.ThrowIfRecording();
      lock (this.currentTimeLock)
      {
        this.currentTime = value;
        this.actualPlaybackStart = Sapi.Now;
        this.logFilePlaybackStart = this.currentTime;
      }
    }
  }

  public bool AtEnd => this.currentTime >= this.endTime;

  public double PlaybackSpeed
  {
    get => this.playbackSpeed;
    set
    {
      this.ThrowIfRecording();
      lock (this.currentTimeLock)
      {
        this.actualPlaybackStart = Sapi.Now;
        this.logFilePlaybackStart = this.currentTime;
        this.playbackSpeed = value;
      }
    }
  }

  public StringDictionary SystemInformation => this.systemInfo;

  public bool Recording => this.recording;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void Dispose(bool disposing)
  {
    if (!this.disposed && disposing)
      this.Close();
    this.disposed = true;
  }

  internal bool StopLogging()
  {
    bool flag = false;
    if (this.logFileStream != null)
    {
      IEnumerable<Channel> channels = Sapi.GetSapi().Channels.Where<Channel>((Func<Channel, bool>) (x => x.Sessions.Count > 0));
      if (channels.Any<Channel>())
      {
        this.UpdateStreamXml(channels);
        flag = true;
      }
      if (this.streamOwner)
      {
        this.logFileStream.Close();
        this.logFileStream.Dispose();
      }
      this.logFileStream = (Stream) null;
    }
    this.name = (string) null;
    return flag;
  }

  public static IList<LogMetadataItem> ExtractMetadata(Stream logFileStream)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    List<LogMetadataItem> result = new List<LogMetadataItem>();
    XmlReader xmlReader = XmlReader.Create(logFileStream, new XmlReaderSettings()
    {
      IgnoreWhitespace = true,
      IgnoreComments = true,
      ConformanceLevel = ConformanceLevel.Fragment,
      CheckCharacters = false
    });
    if (xmlReader.ReadToFollowing(currentFormat[TagName.Channels].LocalName) && xmlReader.ReadToDescendant(currentFormat[TagName.Channel].LocalName))
    {
      do
      {
        Channel.ExtractMetadata(xmlReader, result);
      }
      while (xmlReader.NodeType == XmlNodeType.Element);
    }
    if (xmlReader.ReadToFollowing(currentFormat[TagName.Labels].LocalName) && xmlReader.ReadToDescendant(currentFormat[TagName.Label].LocalName))
    {
      do
      {
        result.Add(Label.ExtractMetadata(xmlReader));
      }
      while (xmlReader.NodeType == XmlNodeType.Element);
    }
    return (IList<LogMetadataItem>) result;
  }

  public void ExtractPartialLog(DateTime startTime, DateTime endTime, Stream stream)
  {
    this.ThrowIfRecording();
    string tempFileName = LogFile.GetTempFileName();
    try
    {
      using (FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
      {
        using (XmlTextWriter xmlWriter = new XmlTextWriter((Stream) fileStream, Encoding.UTF8))
        {
          this.WriteLogFileContent((XmlTextReader) null, (IEnumerable<Channel>) this.allChannels, xmlWriter, startTime, endTime);
          fileStream.CopyAllTo(stream);
        }
      }
    }
    finally
    {
      File.Delete(tempFileName);
    }
  }

  private void ThrowIfRecording()
  {
    if (this.recording)
      throw new InvalidOperationException("Invalid operation for a log file that is being recorded");
  }

  private void UpdateStreamXml(IEnumerable<Channel> channels)
  {
    XmlTextReader xmlReader = (XmlTextReader) null;
    if (this.logFileStream.CanRead && this.logFileStream.Length > 0L)
    {
      this.logFileStream.Position = 0L;
      xmlReader = new XmlTextReader(this.logFileStream);
    }
    try
    {
      this.UpdateStreamXml(channels, xmlReader);
    }
    catch (XmlException ex)
    {
      this.logFileStream.SetLength(0L);
      this.UpdateStreamXml(channels, (XmlTextReader) null);
    }
  }

  private void UpdateStreamXml(IEnumerable<Channel> channels, XmlTextReader xmlReader)
  {
    string tempFileName = LogFile.GetTempFileName();
    try
    {
      using (FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
      {
        using (XmlTextWriter xmlWriter = new XmlTextWriter((Stream) fileStream, Encoding.UTF8))
        {
          this.WriteLogFileContent(xmlReader, channels, xmlWriter, DateTime.MinValue, DateTime.MinValue);
          fileStream.CopyAllTo(this.logFileStream);
        }
      }
    }
    catch (XmlException ex)
    {
      throw;
    }
    finally
    {
      File.Delete(tempFileName);
    }
  }

  private void WriteLogFileContent(
    XmlTextReader previousContentXmlReader,
    IEnumerable<Channel> channels,
    XmlTextWriter xmlWriter,
    DateTime startTime,
    DateTime endTime)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    xmlWriter.Formatting = Formatting.Indented;
    xmlWriter.WriteStartDocument();
    Sapi.GetSapi().InitSapiXmlFile((XmlWriter) xmlWriter, "Log", 17, this.startTime, true);
    XElement xelement = new XElement(currentFormat[TagName.System], new object[9]
    {
      (object) LogFile.CreateInformationNode("MachineName", Environment.MachineName),
      (object) LogFile.CreateInformationNode("Domain", Environment.UserDomainName),
      (object) LogFile.CreateInformationNode("User", Environment.UserName),
      (object) LogFile.CreateInformationNode("64BitOS", Environment.Is64BitOperatingSystem.ToString()),
      (object) LogFile.CreateInformationNode("64BitProcess", Environment.Is64BitProcess.ToString()),
      (object) LogFile.CreateInformationNode("AppName", Assembly.GetEntryAssembly().GetName().Name.ToString()),
      (object) LogFile.CreateInformationNode("AppVersion", Assembly.GetEntryAssembly().GetName().Version.ToString()),
      (object) LogFile.CreateInformationNode("SapiVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString()),
      (object) LogFile.CreateInformationNode("ToolId", Sapi.GetSapi().ToolId != null ? BitConverter.ToString(Sapi.GetSapi().ToolId.ToArray<byte>()) : "<none>")
    });
    IRollCall manager = (IRollCall) RollCall.GetManager(Protocol.J1939);
    if (manager != null && manager.DeviceLibraryName != null && manager.DeviceLibraryVersion != null)
    {
      xelement.Add((object) LogFile.CreateInformationNode("RP1210Library", manager.DeviceLibraryName));
      xelement.Add((object) LogFile.CreateInformationNode("RP1210LibraryVersion", manager.DeviceLibraryVersion));
      xelement.Add((object) LogFile.CreateInformationNode("RP1210FirmwareVersion", manager.DeviceFirmwareVersion));
      xelement.Add((object) LogFile.CreateInformationNode("RP1210DriverVersion", manager.DeviceDriverVersion));
    }
    xelement.WriteTo((XmlWriter) xmlWriter);
    xmlWriter.WriteStartElement(currentFormat[TagName.Channels].LocalName);
    if (previousContentXmlReader != null)
      previousContentXmlReader.CopyChildrenTo((XmlWriter) xmlWriter, currentFormat[TagName.Channels].LocalName, currentFormat[TagName.Channel].LocalName);
    foreach (Channel channel in channels)
    {
      if (this.recording)
      {
        if (this.summary)
          channel.WriteSummaryXmlTo((XmlWriter) xmlWriter);
        else
          channel.WriteXmlTo((XmlWriter) xmlWriter);
      }
      else if (!(channel.EndTime < startTime) && !(this.StartTime > endTime))
        channel.WriteXmlTo(true, startTime, endTime, (XmlWriter) xmlWriter);
    }
    xmlWriter.WriteEndElement();
    xmlWriter.WriteStartElement(currentFormat[TagName.Labels].LocalName);
    if (previousContentXmlReader != null)
      previousContentXmlReader.CopyChildrenTo((XmlWriter) xmlWriter, currentFormat[TagName.Labels].LocalName, currentFormat[TagName.Label].LocalName);
    DateTime dateTime = DateTime.MinValue;
    foreach (Label label in this.labels)
    {
      if (this.recording && label.Time > this.lastLabelWriteTime || label.Time >= startTime && label.Time <= endTime)
      {
        label.XElement.WriteTo((XmlWriter) xmlWriter);
        dateTime = label.Time;
      }
    }
    xmlWriter.WriteEndElement();
    if (this.recording && dateTime != DateTime.MinValue)
      this.lastLabelWriteTime = dateTime;
    xmlWriter.WriteEndElement();
    xmlWriter.WriteEndDocument();
    xmlWriter.Flush();
  }

  private static XElement CreateInformationNode(string name, string value)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    return new XElement(currentFormat[TagName.Information], new object[2]
    {
      (object) new XAttribute(currentFormat[TagName.Name], (object) name),
      (object) new XAttribute(currentFormat[TagName.Value], (object) value)
    });
  }

  private void ThreadFunc()
  {
    DateTime dateTime1 = DateTime.MinValue;
    Dictionary<Channel, IEnumerable<Service>> dictionary1 = new Dictionary<Channel, IEnumerable<Service>>();
    Dictionary<Channel, IEnumerable<ParameterGroup>> dictionary2 = new Dictionary<Channel, IEnumerable<ParameterGroup>>();
    Label label = (Label) null;
    try
    {
      while (!this.closing)
      {
        if (this.Playing)
        {
          lock (this.currentTimeLock)
            this.currentTime = this.logFilePlaybackStart + TimeSpan.FromMilliseconds((Sapi.Now - this.actualPlaybackStart).TotalMilliseconds * this.playbackSpeed);
        }
        if (this.currentTime != dateTime1)
        {
          dateTime1 = this.currentTime;
          List<Channel> list1 = this.activeChannels.ToList<Channel>();
          lock (this.activeChannels.SyncRoot)
          {
            for (int index = 0; index < this.allChannels.Count; ++index)
            {
              Channel allChannel = this.allChannels[index];
              if (allChannel.ActiveAtTime(this.currentTime))
              {
                if (!this.activeChannels.ChannelExists(allChannel))
                {
                  list1.Add(allChannel);
                  this.activeChannels.Add(allChannel);
                  allChannel.SetCommunicationsState(CommunicationsState.LogFilePlayback);
                }
              }
              else if (this.activeChannels.ChannelExists(allChannel))
                this.activeChannels.Remove(allChannel);
            }
          }
          foreach (Channel key in list1)
          {
            for (int index = 0; index < key.EcuInfos.Count; ++index)
              key.EcuInfos[index].EcuInfoValues.SetCurrentTime(this.currentTime);
            for (int index = 0; index < key.Instruments.Count; ++index)
              key.Instruments[index].InstrumentValues.SetCurrentTime(this.currentTime);
            IEnumerable<Service> first;
            if (!dictionary1.TryGetValue(key, out first))
            {
              first = key.Services.Where<Service>((Func<Service, bool>) (s => s.Executions.Any<ServiceExecution>()));
              if (key.StructuredServices != null)
                first = first.Union<Service>(key.StructuredServices.Where<Service>((Func<Service, bool>) (s => s.Executions.Any<ServiceExecution>())));
              dictionary1.Add(key, first);
            }
            foreach (Service service in first)
            {
              service.Executions.SetCurrentTime(this.currentTime);
              foreach (ServiceInputValue inputValue in (ReadOnlyCollection<ServiceInputValue>) service.InputValues)
                inputValue.ArgumentValues.SetCurrentTime(this.currentTime);
              foreach (ServiceOutputValue outputValue in (ReadOnlyCollection<ServiceOutputValue>) service.OutputValues)
                outputValue.ArgumentValues.SetCurrentTime(this.currentTime);
            }
            IEnumerable<ParameterGroup> list2;
            if (!dictionary2.TryGetValue(key, out list2))
            {
              list2 = (IEnumerable<ParameterGroup>) key.ParameterGroups.Where<ParameterGroup>((Func<ParameterGroup, bool>) (g => g.CodingStringValues.Any<CodingStringValue>() || g.Parameters.Any<Parameter>((Func<Parameter, bool>) (s => s.ParameterValues.Count > 0)))).ToList<ParameterGroup>();
              dictionary2.Add(key, list2);
            }
            foreach (ParameterGroup group in list2)
            {
              bool flag = group.CodingStringValues.SetCurrentTime(this.currentTime);
              foreach (Parameter parameter in group.Parameters)
                flag |= parameter.ParameterValues.SetCurrentTime(this.currentTime);
              if (flag)
              {
                key.Parameters.UpdateGroupCodingStringFromLogFile(group);
                foreach (Parameter parameter in group.Parameters)
                  parameter.RaiseParameterUpdateEvent((Exception) null);
              }
            }
            key.FaultCodes.SetCurrentTime(this.currentTime);
            key.CommunicationsStateValues.SetCurrentTime(this.currentTime);
            if (label != this.labels.Current)
            {
              label = this.labels.Current;
              this.lastEvent = DateTime.MinValue;
            }
          }
          if (this.activeChannels.Count == 0)
          {
            if (this.currentTime > this.endTime || this.currentTime < this.startTime)
            {
              this.Playing = false;
              if (this.currentTime < this.startTime)
                this.currentTime = this.startTime;
              if (this.currentTime > this.endTime)
                this.currentTime = this.endTime;
            }
            else
            {
              DateTime dateTime2;
              if (this.playbackSpeed >= 0.0)
              {
                dateTime2 = this.endTime;
                for (int index = 0; index < this.allChannels.Count; ++index)
                {
                  Channel allChannel = this.allChannels[index];
                  if (allChannel.StartTime > this.currentTime && allChannel.StartTime < dateTime2)
                    dateTime2 = allChannel.StartTime;
                }
              }
              else
              {
                dateTime2 = this.startTime;
                for (int index = 0; index < this.allChannels.Count; ++index)
                {
                  Channel allChannel = this.allChannels[index];
                  if (allChannel.EndTime < this.currentTime && allChannel.EndTime > dateTime2)
                    dateTime2 = allChannel.EndTime;
                }
              }
              this.actualPlaybackStart = Sapi.Now;
              this.logFilePlaybackStart = this.currentTime = dateTime2;
            }
            this.lastEvent = DateTime.MinValue;
          }
          if ((Sapi.Now - this.lastEvent).TotalMilliseconds > 250.0)
          {
            this.RaiseLogFileUpdate();
            this.lastEvent = Sapi.Now;
          }
        }
        Thread.Sleep(5);
      }
    }
    catch (Exception ex)
    {
      this.Playing = false;
      this.RaiseLogFileUpdate();
      Sapi.GetSapi().RaiseExceptionEvent((object) this, ex);
    }
  }

  private static string GetTempFileName()
  {
    return Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), ".TMP"));
  }

  private class LoadTask
  {
    public Task Task;
    public Queue<Action> ProcessingQueue;

    public LoadTask(ManualResetEvent doneEvent)
    {
      LogFile.LoadTask loadTask = this;
      this.ProcessingQueue = new Queue<Action>();
      this.Task = Task.Run((Action) (() =>
      {
        while (true)
        {
          while (loadTask.ProcessingQueue.Count <= 0)
          {
            if (doneEvent.WaitOne(1000) && loadTask.ProcessingQueue.Count == 0)
              return;
          }
          loadTask.ProcessingQueue.Dequeue()();
        }
      }));
    }
  }
}
