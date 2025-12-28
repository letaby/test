// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LogFileCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

#nullable disable
namespace SapiLayer1;

public sealed class LogFileCollection : 
  ReadOnlyCollection<LogFile>,
  IEnumerable<LogFile>,
  IEnumerable
{
  internal LogFileCollection()
    : base((IList<LogFile>) new List<LogFile>())
  {
  }

  private void Add(LogFile logFile)
  {
    lock (this.Items)
      this.Items.Add(logFile);
  }

  internal void LogChannel(Channel channel)
  {
    foreach (LogFile logFile in this)
    {
      if (logFile.Recording)
        logFile.LogChannel(channel);
    }
    FireAndForget.Invoke((MulticastDelegate) this.LogFileUpdateEvent, (object) this, new EventArgs());
  }

  internal void Remove(LogFile logFile)
  {
    lock (this.Items)
      this.Items.Remove(logFile);
    if (logFile.Recording)
      FireAndForget.Invoke((MulticastDelegate) this.LogFileStoppedEvent, (object) logFile, new EventArgs());
    else
      FireAndForget.Invoke((MulticastDelegate) this.LogFileDisconnectEvent, (object) logFile, new EventArgs());
  }

  internal void LabelLog(string text, Ecu ecu, Channel channel = null)
  {
    this.LabelLog(new Label(text, Sapi.Now, ecu, channel));
  }

  internal void LabelLog(Label label)
  {
    foreach (LogFile logFile in this)
    {
      if (logFile.Recording)
        logFile.LabelLog(label);
    }
    FireAndForget.Invoke((MulticastDelegate) this.LogFileLabelsChangedEvent, (object) this, new EventArgs());
  }

  public LogFile LoadLog(string pathName)
  {
    return this.LoadLog(pathName, (Stream) new FileStream(pathName, FileMode.Open));
  }

  public LogFile LoadLog(string name, Stream stream) => this.LoadLog(name, stream, true);

  public LogFile LoadLog(string name, Stream stream, bool dataErrorsAreFatal)
  {
    if (Sapi.GetSapi().InitState == InitState.NotInitialized)
      throw new InvalidOperationException("Sapi has not been initialized");
    int ver = 0;
    DateTime dt = DateTime.MinValue;
    using (XmlReader xmlReader = Sapi.ReadSapiXmlFile(stream, "Log", out ver, out dt))
    {
      if (ver <= 0)
        throw new InvalidOperationException("Log file invalid - version not supported");
      LogFile logFile = new LogFile(this, name);
      try
      {
        logFile.Read(xmlReader, ver);
      }
      catch (XmlException ex)
      {
        logFile.Close();
        throw;
      }
      if (dataErrorsAreFatal && (logFile.MissingEcuList.Any<string>() || logFile.MissingVariantList.Any<string>()))
      {
        logFile.Close();
        throw new DataException(!logFile.MissingEcuList.Any<string>() ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find the Diagnosis Variant \"{0}\" referenced in the log file.", (object) logFile.MissingVariantList.First<string>()) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find the ECU\"{0}\" referenced in the log file.", (object) logFile.MissingEcuList.First<string>()));
      }
      this.Add(logFile);
      FireAndForget.Invoke((MulticastDelegate) this.LogFileConnectEvent, (object) logFile, new EventArgs());
      return logFile;
    }
  }

  public LogFile StartLog(Stream logFileStream, bool summary)
  {
    LogFile logFile = new LogFile(this, logFileStream, (string) null, summary);
    logFile.StartLogging();
    this.Add(logFile);
    FireAndForget.Invoke((MulticastDelegate) this.LogFileStartedEvent, (object) logFile, new EventArgs());
    return logFile;
  }

  public LogFile StartLog(Stream logFileStream) => this.StartLog(logFileStream, false);

  public LogFile StartLog(string logFileName)
  {
    LogFile logFile = new LogFile(this, (Stream) null, logFileName, false);
    logFile.StartLogging();
    this.Add(logFile);
    FireAndForget.Invoke((MulticastDelegate) this.LogFileStartedEvent, (object) logFile, new EventArgs());
    return logFile;
  }

  public void LabelLog(string text) => this.LabelLog(text, (Ecu) null);

  public void StopLog()
  {
    bool flag = false;
    foreach (LogFile logFile in this)
    {
      if (logFile.Recording && logFile.StopLogging())
        flag = true;
    }
    if (flag)
      FireAndForget.Invoke((MulticastDelegate) this.LogFileUpdateEvent, (object) this, new EventArgs());
    for (int index = 0; index < this.Count; ++index)
    {
      LogFile logFile = this[index];
      if (logFile.Recording)
      {
        logFile.Close();
        --index;
      }
    }
  }

  public LogFile this[string name]
  {
    get
    {
      return this.FirstOrDefault<LogFile>((System.Func<LogFile, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal)));
    }
  }

  public bool Logging => this.Any<LogFile>((System.Func<LogFile, bool>) (logFile => logFile.Recording));

  public LabelCollection LoggedLabels
  {
    get
    {
      foreach (LogFile logFile in this)
      {
        if (logFile.Recording)
          return logFile.Labels;
      }
      return (LabelCollection) null;
    }
  }

  public event LogFileConnectEventHandler LogFileConnectEvent;

  public event LogFileDisconnectEventHandler LogFileDisconnectEvent;

  public event EventHandler LogFileStartedEvent;

  public event EventHandler LogFileUpdateEvent;

  public event EventHandler LogFileLabelsChangedEvent;

  public event EventHandler LogFileStoppedEvent;

  public new IEnumerator<LogFile> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<LogFile>) new List<LogFile>((IEnumerable<LogFile>) this.Items).GetEnumerator();
  }

  IEnumerator<LogFile> IEnumerable<LogFile>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
