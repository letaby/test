// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashAreaCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
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

  internal FlashAreaCollection(Channel c) => this.channel = c;

  protected override void AcquireList()
  {
    if (this.channel.EcuHandle == null && this.channel.McdEcuHandle == null)
      return;
    Sapi.GetSapi().EnsureFlashFilesLoaded();
    this.Items.Clear();
    List<FlashArea> source = new List<FlashArea>();
    if (this.channel.McdEcuHandle != null)
    {
      uint num = 0;
      foreach (McdDBFlashSession dbFlashSession in this.channel.McdEcuHandle.DBFlashSessions)
      {
        FlashArea flashArea = new FlashArea(this.channel, dbFlashSession.Qualifier);
        flashArea.Acquire(dbFlashSession, Enumerable.Repeat<string>(this.channel.Ecu.Name, 1), num++);
        if (flashArea.FlashMeanings.Any<FlashMeaning>())
          source.Add(flashArea);
      }
    }
    else
    {
      uint flashAreaCount = this.channel.EcuHandle.FlashAreaCount;
      for (uint areaIndex = 0; areaIndex < flashAreaCount; ++areaIndex)
      {
        try
        {
          using (CaesarDIFlashArea flashArea1 = this.channel.EcuHandle.GetFlashArea(areaIndex))
          {
            if (flashArea1 != null)
            {
              string areaQualifier = flashArea1.Qualifier;
              FlashArea flashArea2 = source.FirstOrDefault<FlashArea>((Func<FlashArea, bool>) (a => a.Qualifier == areaQualifier));
              if (flashArea2 == null)
              {
                flashArea2 = new FlashArea(this.channel, areaQualifier);
                source.Add(flashArea2);
              }
              flashArea2.Acquire(flashArea1, areaIndex);
            }
          }
        }
        catch (CaesarErrorException ex)
        {
          byte? negativeResponseCode = new byte?();
          CaesarException e = new CaesarException(ex, negativeResponseCode);
          Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
        }
      }
    }
    foreach (FlashArea flashArea in (IEnumerable<FlashArea>) source.OrderBy<FlashArea, int>((Func<FlashArea, int>) (a => a.FlashMeanings.Min<FlashMeaning>((Func<FlashMeaning, int>) (m => m.Priority)))))
      this.Items.Add(flashArea);
    if (!this.channel.Ecu.Properties.ContainsKey("ProgrammingSequenceFunctionalEcu"))
      return;
    string property = this.channel.Ecu.Properties["ProgrammingSequenceFunctionalEcu"];
    this.functionalEcu = Sapi.GetSapi().Ecus[property];
    if (this.functionalEcu != null && this.functionalEcu.Properties.ContainsKey("PreProgrammingSequence") && this.functionalEcu.Properties.ContainsKey("PostProgrammingSequence"))
    {
      this.preProgrammingSequence = this.functionalEcu.Properties["PreProgrammingSequence"];
      this.postProgrammingSequence = this.functionalEcu.Properties["PostProgrammingSequence"];
    }
    if (this.functionalEcu != null && this.preProgrammingSequence != null && this.postProgrammingSequence != null)
      return;
    CaesarException e1 = new CaesarException(SapiError.FunctionalEcuConfigurationError);
    Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e1);
  }

  internal void InternalFlash()
  {
    CaesarException ex = (CaesarException) null;
    List<FlashMeaning> flashMeaningList = new List<FlashMeaning>();
    List<FlashMeaning> source = new List<FlashMeaning>();
    foreach (FlashArea flashArea in (ReadOnlyCollection<FlashArea>) this)
    {
      FlashMeaning marked = flashArea.Marked;
      if (marked != null)
      {
        source.Add(marked);
        this.FindDuplicateFlashKeys(marked, flashMeaningList);
      }
    }
    this.ModifyFlashKeys(flashMeaningList, false);
    if ((this.channel.ChannelHandle != null ? (this.channel.ChannelHandle.FlashInit() ? 1 : 0) : 1) != 0)
    {
      this.meaningsToFlash = (IEnumerable<FlashMeaning>) source.OrderBy<FlashMeaning, int>((Func<FlashMeaning, int>) (m => m.Priority)).ToList<FlashMeaning>();
      foreach (FlashMeaning flashMeaning in this.meaningsToFlash)
      {
        if (!this.channel.IsChannelErrorSet && this.channel.ChannelHandle != null)
          this.channel.ChannelHandle.SetMeaningByFlashKey(flashMeaning.FlashKey);
      }
      if (!this.channel.IsChannelErrorSet)
      {
        this.thread = new Thread(new ThreadStart(this.ThreadFunc));
        this.thread.Start();
        ex = this.FlashSequence();
      }
      this.channel.SetCommunicationsState(CommunicationsState.Online);
      if (this.thread != null)
      {
        this.thread.Join();
        this.thread = (Thread) null;
      }
      if (this.channel.IsChannelErrorSet)
        ex = new CaesarException(this.channel.ChannelHandle);
      if (this.channel.ChannelHandle != null)
        this.channel.ChannelHandle.FlashExit();
    }
    else
    {
      ex = new CaesarException(SapiError.CannotEnterFlashingMode);
      this.channel.SetCommunicationsState(CommunicationsState.Online);
    }
    this.ModifyFlashKeys(flashMeaningList, true);
    this.RaiseFlashCompleteEvent((Exception) ex);
  }

  private void RaiseFlashProgressUpdateEvent(float fProg)
  {
    FireAndForget.Invoke((MulticastDelegate) this.FlashProgressUpdateEvent, (object) this, (EventArgs) new ProgressEventArgs((double) fProg));
  }

  internal void RaiseFlashCompleteEvent(Exception ex)
  {
    FireAndForget.Invoke((MulticastDelegate) this.FlashCompleteEvent, (object) this, (EventArgs) new ResultEventArgs(ex));
    this.channel.SyncDone(ex);
  }

  internal void SynchronousCheckFailure(object sender, CaesarException exception)
  {
    FireAndForget.Invoke((MulticastDelegate) this.FlashCompleteEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) exception));
  }

  public void Flash(bool synchronous)
  {
    bool flag = false;
    for (int index = 0; index < this.Count && !this.channel.IsChannelErrorSet; ++index)
    {
      if (this[index].Marked != null)
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      throw new ArgumentException("No marked flash meanings on this channel");
    if (this.functionalEcu != null)
    {
      this.functionalEcu.EcuInfoComParameters[(object) "CP_BAUDRATE"] = (object) this.channel.ConnectionResource.BaudRate;
      this.functionalResource = this.functionalEcu.GetConnectionResources().GetEquivalent(this.channel.ConnectionResource);
      if (this.functionalResource == null)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to locate equivalent functional resource for {0}: Desired resource was {1}. Functional pre and post programming sequences will not be sent.", (object) this.functionalEcu.Name, (object) this.Channel.ConnectionResource));
    }
    this.channel.QueueAction((object) CommunicationsState.Flash, synchronous, new SynchronousCheckFailureHandler(this.SynchronousCheckFailure));
  }

  public Channel Channel => this.channel;

  public FlashArea this[string qualifier]
  {
    get
    {
      return this.FirstOrDefault<FlashArea>((Func<FlashArea, bool>) (item => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal)));
    }
  }

  public float Progress => this.progress;

  public event FlashProgressUpdateEventHandler FlashProgressUpdateEvent;

  public event FlashCompleteEventHandler FlashCompleteEvent;

  private CaesarException FlashSequence()
  {
    Channel sender = (Channel) null;
    CaesarException caesarException = (CaesarException) null;
    this.CurrentFlashMeaning = (FlashMeaning) null;
    try
    {
      if (this.functionalResource != null)
      {
        try
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.channel.Ecu.Name, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Opening functional channel {0} on {1}", (object) this.functionalEcu.ToString(), (object) this.functionalResource.ToString()));
          sender = Sapi.GetSapi().Channels.Open(this.functionalResource);
          Sapi.GetSapi().RaiseDebugInfoEvent((object) sender, "Initializing functional channel");
          sender.EcuInfos.AutoRead = false;
          sender.Init(false);
          Sapi.GetSapi().RaiseDebugInfoEvent((object) sender, "Sending pre-programming sequence");
          sender.Services.Execute(this.preProgrammingSequence, true);
        }
        catch (CaesarException ex)
        {
          caesarException = ex;
        }
      }
      if (caesarException == null)
      {
        if (this.channel.ChannelHandle != null)
          this.channel.ChannelHandle.FlashDoCoding();
        else if (this.channel.McdChannelHandle != null)
        {
          try
          {
            foreach (FlashMeaning flashMeaning in this.meaningsToFlash)
            {
              this.CurrentFlashMeaning = flashMeaning;
              using (McdFlashJob flashJobForKey = this.channel.McdChannelHandle.CreateFlashJobForKey(flashMeaning.FlashKey))
              {
                flashJobForKey.Execute();
                while (flashJobForKey.Running)
                {
                  this.progress = (float) flashJobForKey.Progress;
                  Thread.Sleep(500);
                }
                flashJobForKey.FetchResults();
              }
            }
          }
          catch (McdException ex)
          {
            caesarException = new CaesarException(ex);
          }
        }
        this.channel.Services.InternalDereferencedExecute("PostProgrammingSequence");
        if (sender != null)
        {
          try
          {
            Sapi.GetSapi().RaiseDebugInfoEvent((object) sender, "Sending post-programming sequence");
            sender.Services.Execute(this.postProgrammingSequence, true);
          }
          catch (CaesarException ex)
          {
            Sapi.GetSapi().RaiseDebugInfoEvent((object) sender, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failure to send post-programming sequence: {0}", (object) ex.Message));
          }
        }
      }
    }
    finally
    {
      if (sender != null)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) sender, "Closing functional channel");
        sender.Disconnect();
        this.functionalResource = (ConnectionResource) null;
        this.CurrentFlashMeaning = (FlashMeaning) null;
      }
    }
    return caesarException;
  }

  private void FindDuplicateFlashKeys(FlashMeaning meaning, List<FlashMeaning> list)
  {
    foreach (FlashArea flashArea in (ReadOnlyCollection<FlashArea>) this)
    {
      foreach (FlashMeaning flashMeaning in (ReadOnlyCollection<FlashMeaning>) flashArea.FlashMeanings)
      {
        if (flashMeaning != meaning && string.Equals(meaning.FlashKey, flashMeaning.FlashKey))
          list.Add(flashMeaning);
      }
    }
  }

  private void ModifyFlashKeys(List<FlashMeaning> meanings, bool add)
  {
    for (int index = 0; index < meanings.Count; ++index)
    {
      FlashMeaning meaning = meanings[index];
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Modifying CAESAR flash file list for key {0}: {1} {2}", (object) meaning.FlashKey, add ? (object) "Adding" : (object) "Removing", (object) meaning.FileName));
      if (add)
        CaesarRoot.AddFlashFile(meaning.FileName);
      else
        CaesarRoot.RemoveFlashFile(meaning.FileName);
    }
  }

  public FlashMeaning CurrentFlashMeaning
  {
    get
    {
      lock (this.currentFlashMeaningLock)
        return this.currentFlashMeaning;
    }
    private set
    {
      lock (this.currentFlashMeaningLock)
        this.currentFlashMeaning = value;
    }
  }

  internal void SetCurrentFlashJob(string flashJob)
  {
    lock (this.currentFlashMeaningLock)
    {
      if (!this.Acquired)
        return;
      this.CurrentFlashMeaning = this.Select<FlashArea, FlashMeaning>((Func<FlashArea, FlashMeaning>) (fa => fa.Marked)).FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (fm => fm != null && fm.FlashJobName == flashJob));
    }
  }

  private void ThreadFunc()
  {
    while (this.channel.CommunicationsState == CommunicationsState.Flash)
    {
      if (this.channel.ChannelHandle != null)
        this.progress = this.channel.ChannelHandle.FlashDownloadProgress * 100f;
      this.RaiseFlashProgressUpdateEvent(this.progress);
      Thread.Sleep(250);
    }
    this.progress = 0.0f;
  }
}
