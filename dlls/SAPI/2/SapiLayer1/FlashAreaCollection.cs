using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashAreaCollection : LateLoadReadOnlyCollection<FlashArea>
{
	internal const int FlashProgressUpdateInterval = 250;

	private Channel channel;

	private FlashMeaning currentFlashMeaning;

	private object currentFlashMeaningLock = new object();

	private Thread thread;

	private IEnumerable<FlashMeaning> meaningsToFlash;

	private float progress;

	private Ecu functionalEcu;

	private string preProgrammingSequence;

	private string postProgrammingSequence;

	private ConnectionResource functionalResource;

	public Channel Channel => channel;

	public FlashArea this[string qualifier] => this.FirstOrDefault((FlashArea item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	public float Progress => progress;

	public FlashMeaning CurrentFlashMeaning
	{
		get
		{
			lock (currentFlashMeaningLock)
			{
				return currentFlashMeaning;
			}
		}
		private set
		{
			lock (currentFlashMeaningLock)
			{
				currentFlashMeaning = value;
			}
		}
	}

	public event FlashProgressUpdateEventHandler FlashProgressUpdateEvent;

	public event FlashCompleteEventHandler FlashCompleteEvent;

	internal FlashAreaCollection(Channel c)
	{
		channel = c;
	}

	protected override void AcquireList()
	{
		//IL_0172: Expected O, but got Unknown
		if (channel.EcuHandle == null && channel.McdEcuHandle == null)
		{
			return;
		}
		Sapi.GetSapi().EnsureFlashFilesLoaded();
		base.Items.Clear();
		List<FlashArea> list = new List<FlashArea>();
		if (channel.McdEcuHandle != null)
		{
			uint num = 0u;
			foreach (McdDBFlashSession dBFlashSession in channel.McdEcuHandle.DBFlashSessions)
			{
				FlashArea flashArea = new FlashArea(channel, dBFlashSession.Qualifier);
				flashArea.Acquire(dBFlashSession, Enumerable.Repeat(channel.Ecu.Name, 1), num++);
				if (flashArea.FlashMeanings.Any())
				{
					list.Add(flashArea);
				}
			}
		}
		else
		{
			uint flashAreaCount = channel.EcuHandle.FlashAreaCount;
			for (uint num2 = 0u; num2 < flashAreaCount; num2++)
			{
				try
				{
					CaesarDIFlashArea flashArea2 = channel.EcuHandle.GetFlashArea(num2);
					try
					{
						if (flashArea2 != null)
						{
							string areaQualifier = flashArea2.Qualifier;
							FlashArea flashArea3 = list.FirstOrDefault((FlashArea a) => a.Qualifier == areaQualifier);
							if (flashArea3 == null)
							{
								flashArea3 = new FlashArea(channel, areaQualifier);
								list.Add(flashArea3);
							}
							flashArea3.Acquire(flashArea2, num2);
						}
					}
					finally
					{
						((IDisposable)flashArea2)?.Dispose();
					}
				}
				catch (CaesarErrorException ex)
				{
					CaesarException e = new CaesarException(ex, null, null);
					Sapi.GetSapi().RaiseExceptionEvent(this, e);
				}
			}
		}
		foreach (FlashArea item in list.OrderBy((FlashArea a) => a.FlashMeanings.Min((FlashMeaning m) => m.Priority)))
		{
			base.Items.Add(item);
		}
		if (channel.Ecu.Properties.ContainsKey("ProgrammingSequenceFunctionalEcu"))
		{
			string name = channel.Ecu.Properties["ProgrammingSequenceFunctionalEcu"];
			functionalEcu = Sapi.GetSapi().Ecus[name];
			if (functionalEcu != null && functionalEcu.Properties.ContainsKey("PreProgrammingSequence") && functionalEcu.Properties.ContainsKey("PostProgrammingSequence"))
			{
				preProgrammingSequence = functionalEcu.Properties["PreProgrammingSequence"];
				postProgrammingSequence = functionalEcu.Properties["PostProgrammingSequence"];
			}
			if (functionalEcu == null || preProgrammingSequence == null || postProgrammingSequence == null)
			{
				CaesarException e2 = new CaesarException(SapiError.FunctionalEcuConfigurationError);
				Sapi.GetSapi().RaiseExceptionEvent(this, e2);
			}
		}
	}

	internal void InternalFlash()
	{
		CaesarException ex = null;
		List<FlashMeaning> meanings = new List<FlashMeaning>();
		List<FlashMeaning> list = new List<FlashMeaning>();
		using (IEnumerator<FlashArea> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FlashMeaning marked = enumerator.Current.Marked;
				if (marked != null)
				{
					list.Add(marked);
					FindDuplicateFlashKeys(marked, meanings);
				}
			}
		}
		ModifyFlashKeys(meanings, add: false);
		if (channel.ChannelHandle == null || channel.ChannelHandle.FlashInit())
		{
			meaningsToFlash = list.OrderBy((FlashMeaning m) => m.Priority).ToList();
			foreach (FlashMeaning item in meaningsToFlash)
			{
				if (!channel.IsChannelErrorSet && channel.ChannelHandle != null)
				{
					channel.ChannelHandle.SetMeaningByFlashKey(item.FlashKey);
				}
			}
			if (!channel.IsChannelErrorSet)
			{
				thread = new Thread(ThreadFunc);
				thread.Start();
				ex = FlashSequence();
			}
			channel.SetCommunicationsState(CommunicationsState.Online);
			if (thread != null)
			{
				thread.Join();
				thread = null;
			}
			if (channel.IsChannelErrorSet)
			{
				ex = new CaesarException(channel.ChannelHandle);
			}
			if (channel.ChannelHandle != null)
			{
				channel.ChannelHandle.FlashExit();
			}
		}
		else
		{
			ex = new CaesarException(SapiError.CannotEnterFlashingMode);
			channel.SetCommunicationsState(CommunicationsState.Online);
		}
		ModifyFlashKeys(meanings, add: true);
		RaiseFlashCompleteEvent(ex);
	}

	private void RaiseFlashProgressUpdateEvent(float fProg)
	{
		FireAndForget.Invoke(this.FlashProgressUpdateEvent, this, new ProgressEventArgs(fProg));
	}

	internal void RaiseFlashCompleteEvent(Exception ex)
	{
		FireAndForget.Invoke(this.FlashCompleteEvent, this, new ResultEventArgs(ex));
		channel.SyncDone(ex);
	}

	internal void SynchronousCheckFailure(object sender, CaesarException exception)
	{
		FireAndForget.Invoke(this.FlashCompleteEvent, this, new ResultEventArgs(exception));
	}

	public void Flash(bool synchronous)
	{
		bool flag = false;
		for (int i = 0; i < base.Count; i++)
		{
			if (channel.IsChannelErrorSet)
			{
				break;
			}
			if (base[i].Marked != null)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (functionalEcu != null)
			{
				functionalEcu.EcuInfoComParameters["CP_BAUDRATE"] = channel.ConnectionResource.BaudRate;
				functionalResource = functionalEcu.GetConnectionResources().GetEquivalent(channel.ConnectionResource);
				if (functionalResource == null)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Unable to locate equivalent functional resource for {0}: Desired resource was {1}. Functional pre and post programming sequences will not be sent.", functionalEcu.Name, Channel.ConnectionResource));
				}
			}
			channel.QueueAction(CommunicationsState.Flash, synchronous, SynchronousCheckFailure);
			return;
		}
		throw new ArgumentException("No marked flash meanings on this channel");
	}

	private CaesarException FlashSequence()
	{
		Channel channel = null;
		CaesarException ex = null;
		CurrentFlashMeaning = null;
		try
		{
			if (functionalResource != null)
			{
				try
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this.channel.Ecu.Name, string.Format(CultureInfo.InvariantCulture, "Opening functional channel {0} on {1}", functionalEcu.ToString(), functionalResource.ToString()));
					channel = Sapi.GetSapi().Channels.Open(functionalResource);
					Sapi.GetSapi().RaiseDebugInfoEvent(channel, "Initializing functional channel");
					channel.EcuInfos.AutoRead = false;
					channel.Init(autoread: false);
					Sapi.GetSapi().RaiseDebugInfoEvent(channel, "Sending pre-programming sequence");
					channel.Services.Execute(preProgrammingSequence, synchronous: true);
				}
				catch (CaesarException ex2)
				{
					ex = ex2;
				}
			}
			if (ex == null)
			{
				if (this.channel.ChannelHandle != null)
				{
					this.channel.ChannelHandle.FlashDoCoding();
				}
				else if (this.channel.McdChannelHandle != null)
				{
					try
					{
						using IEnumerator<FlashMeaning> enumerator = meaningsToFlash.GetEnumerator();
						while (enumerator.MoveNext())
						{
							FlashMeaning flashMeaning = (CurrentFlashMeaning = enumerator.Current);
							using McdFlashJob mcdFlashJob = this.channel.McdChannelHandle.CreateFlashJobForKey(flashMeaning.FlashKey);
							mcdFlashJob.Execute();
							while (mcdFlashJob.Running)
							{
								progress = (int)mcdFlashJob.Progress;
								Thread.Sleep(500);
							}
							mcdFlashJob.FetchResults();
						}
					}
					catch (McdException mcdError)
					{
						ex = new CaesarException(mcdError);
					}
				}
				this.channel.Services.InternalDereferencedExecute("PostProgrammingSequence");
				if (channel != null)
				{
					try
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(channel, "Sending post-programming sequence");
						channel.Services.Execute(postProgrammingSequence, synchronous: true);
					}
					catch (CaesarException ex3)
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(channel, string.Format(CultureInfo.InvariantCulture, "Failure to send post-programming sequence: {0}", ex3.Message));
					}
				}
			}
		}
		finally
		{
			if (channel != null)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(channel, "Closing functional channel");
				channel.Disconnect();
				functionalResource = null;
				CurrentFlashMeaning = null;
			}
		}
		return ex;
	}

	private void FindDuplicateFlashKeys(FlashMeaning meaning, List<FlashMeaning> list)
	{
		using IEnumerator<FlashArea> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (FlashMeaning flashMeaning in enumerator.Current.FlashMeanings)
			{
				if (flashMeaning != meaning && string.Equals(meaning.FlashKey, flashMeaning.FlashKey))
				{
					list.Add(flashMeaning);
				}
			}
		}
	}

	private void ModifyFlashKeys(List<FlashMeaning> meanings, bool add)
	{
		for (int i = 0; i < meanings.Count; i++)
		{
			FlashMeaning flashMeaning = meanings[i];
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Modifying CAESAR flash file list for key {0}: {1} {2}", flashMeaning.FlashKey, add ? "Adding" : "Removing", flashMeaning.FileName));
			if (add)
			{
				CaesarRoot.AddFlashFile(flashMeaning.FileName);
			}
			else
			{
				CaesarRoot.RemoveFlashFile(flashMeaning.FileName);
			}
		}
	}

	internal void SetCurrentFlashJob(string flashJob)
	{
		lock (currentFlashMeaningLock)
		{
			if (base.Acquired)
			{
				CurrentFlashMeaning = this.Select((FlashArea fa) => fa.Marked).FirstOrDefault((FlashMeaning fm) => fm != null && fm.FlashJobName == flashJob);
			}
		}
	}

	private void ThreadFunc()
	{
		while (channel.CommunicationsState == CommunicationsState.Flash)
		{
			if (channel.ChannelHandle != null)
			{
				progress = channel.ChannelHandle.FlashDownloadProgress * 100f;
			}
			RaiseFlashProgressUpdateEvent(progress);
			Thread.Sleep(250);
		}
		progress = 0f;
	}
}
