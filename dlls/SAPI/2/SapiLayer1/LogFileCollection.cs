using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace SapiLayer1;

public sealed class LogFileCollection : ReadOnlyCollection<LogFile>, IEnumerable<LogFile>, IEnumerable
{
	public LogFile this[string name] => this.FirstOrDefault((LogFile item) => string.Equals(item.Name, name, StringComparison.Ordinal));

	public bool Logging => this.Any((LogFile logFile) => logFile.Recording);

	public LabelCollection LoggedLabels
	{
		get
		{
			using (IEnumerator<LogFile> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LogFile current = enumerator.Current;
					if (current.Recording)
					{
						return current.Labels;
					}
				}
			}
			return null;
		}
	}

	public event LogFileConnectEventHandler LogFileConnectEvent;

	public event LogFileDisconnectEventHandler LogFileDisconnectEvent;

	public event EventHandler LogFileStartedEvent;

	public event EventHandler LogFileUpdateEvent;

	public event EventHandler LogFileLabelsChangedEvent;

	public event EventHandler LogFileStoppedEvent;

	internal LogFileCollection()
		: base((IList<LogFile>)new List<LogFile>())
	{
	}

	private void Add(LogFile logFile)
	{
		lock (base.Items)
		{
			base.Items.Add(logFile);
		}
	}

	internal void LogChannel(Channel channel)
	{
		using (IEnumerator<LogFile> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				LogFile current = enumerator.Current;
				if (current.Recording)
				{
					current.LogChannel(channel);
				}
			}
		}
		FireAndForget.Invoke(this.LogFileUpdateEvent, this, new EventArgs());
	}

	internal void Remove(LogFile logFile)
	{
		lock (base.Items)
		{
			base.Items.Remove(logFile);
		}
		if (logFile.Recording)
		{
			FireAndForget.Invoke(this.LogFileStoppedEvent, logFile, new EventArgs());
		}
		else
		{
			FireAndForget.Invoke(this.LogFileDisconnectEvent, logFile, new EventArgs());
		}
	}

	internal void LabelLog(string text, Ecu ecu, Channel channel = null)
	{
		LabelLog(new Label(text, Sapi.Now, ecu, channel));
	}

	internal void LabelLog(Label label)
	{
		using (IEnumerator<LogFile> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				LogFile current = enumerator.Current;
				if (current.Recording)
				{
					current.LabelLog(label);
				}
			}
		}
		FireAndForget.Invoke(this.LogFileLabelsChangedEvent, this, new EventArgs());
	}

	public LogFile LoadLog(string pathName)
	{
		return LoadLog(pathName, new FileStream(pathName, FileMode.Open));
	}

	public LogFile LoadLog(string name, Stream stream)
	{
		return LoadLog(name, stream, dataErrorsAreFatal: true);
	}

	public LogFile LoadLog(string name, Stream stream, bool dataErrorsAreFatal)
	{
		if (Sapi.GetSapi().InitState != InitState.NotInitialized)
		{
			int ver = 0;
			DateTime dt = DateTime.MinValue;
			using XmlReader xmlReader = Sapi.ReadSapiXmlFile(stream, "Log", out ver, out dt);
			if (ver > 0)
			{
				LogFile logFile = new LogFile(this, name);
				try
				{
					logFile.Read(xmlReader, ver);
				}
				catch (XmlException)
				{
					logFile.Close();
					throw;
				}
				if (dataErrorsAreFatal && (logFile.MissingEcuList.Any() || logFile.MissingVariantList.Any()))
				{
					logFile.Close();
					string s = ((!logFile.MissingEcuList.Any()) ? string.Format(CultureInfo.InvariantCulture, "Could not find the Diagnosis Variant \"{0}\" referenced in the log file.", logFile.MissingVariantList.First()) : string.Format(CultureInfo.InvariantCulture, "Could not find the ECU\"{0}\" referenced in the log file.", logFile.MissingEcuList.First()));
					throw new DataException(s);
				}
				Add(logFile);
				FireAndForget.Invoke(this.LogFileConnectEvent, logFile, new EventArgs());
				return logFile;
			}
			throw new InvalidOperationException("Log file invalid - version not supported");
		}
		throw new InvalidOperationException("Sapi has not been initialized");
	}

	public LogFile StartLog(Stream logFileStream, bool summary)
	{
		LogFile logFile = new LogFile(this, logFileStream, null, summary);
		logFile.StartLogging();
		Add(logFile);
		FireAndForget.Invoke(this.LogFileStartedEvent, logFile, new EventArgs());
		return logFile;
	}

	public LogFile StartLog(Stream logFileStream)
	{
		return StartLog(logFileStream, summary: false);
	}

	public LogFile StartLog(string logFileName)
	{
		LogFile logFile = new LogFile(this, null, logFileName, summary: false);
		logFile.StartLogging();
		Add(logFile);
		FireAndForget.Invoke(this.LogFileStartedEvent, logFile, new EventArgs());
		return logFile;
	}

	public void LabelLog(string text)
	{
		LabelLog(text, null);
	}

	public void StopLog()
	{
		bool flag = false;
		using (IEnumerator<LogFile> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				LogFile current = enumerator.Current;
				if (current.Recording && current.StopLogging())
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			FireAndForget.Invoke(this.LogFileUpdateEvent, this, new EventArgs());
		}
		for (int i = 0; i < base.Count; i++)
		{
			LogFile logFile = base[i];
			if (logFile.Recording)
			{
				logFile.Close();
				i--;
			}
		}
	}

	public new IEnumerator<LogFile> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<LogFile>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<LogFile> IEnumerable<LogFile>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
