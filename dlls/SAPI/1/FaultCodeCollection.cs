// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FaultCodeCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace SapiLayer1;

public sealed class FaultCodeCollection : 
  LateLoadReadOnlyCollection<FaultCode>,
  IEnumerable<FaultCode>,
  IEnumerable
{
  private int faultResetDelay;
  private bool haveLoggedFaultReadException;
  private Service mcdDiagnosticTroubleCodesJob;
  private Channel channel;
  private bool autoRead;
  private bool allowEnvironmentRead;
  private byte udsFilterByte;
  private bool supportsSnapshot;
  private bool supportsPermanent;
  private bool supportsFaultRead;
  private bool hasFaultDescriptions;
  private bool haveBeenReadFromEcu;
  private bool snapshotHasBeenReadFromEcu;
  private uint snapshotRecordNumber;
  private bool uds;
  private ServiceCollection environmentDataDescriptions;
  private List<Instrument> rollcallSnapshotDescriptions = new List<Instrument>();

  internal FaultCodeCollection(Channel c)
  {
    this.channel = c;
    this.snapshotRecordNumber = (uint) byte.MaxValue;
    this.allowEnvironmentRead = true;
    if (this.channel.Ecu.Properties.ContainsKey(nameof (SupportsFaultRead)))
    {
      if (!bool.TryParse(this.channel.Ecu.Properties[nameof (SupportsFaultRead)], out this.supportsFaultRead))
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse SupportsFaultRead property");
    }
    else
      this.supportsFaultRead = true;
    this.uds = this.channel.Ecu.IsUds;
    if (this.uds)
    {
      this.udsFilterByte = (byte) 13;
      if (this.channel.Ecu.Properties.ContainsKey("UDSDTCFilter"))
      {
        string property = this.channel.Ecu.Properties["UDSDTCFilter"];
        try
        {
          this.udsFilterByte = Convert.ToByte(property, 16 /*0x10*/);
        }
        catch (FormatException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Error parsing UDS DTC Filter");
        }
      }
      if (this.channel.Ecu.Properties.ContainsKey(nameof (SupportsSnapshot)) && !bool.TryParse(this.channel.Ecu.Properties[nameof (SupportsSnapshot)], out this.supportsSnapshot))
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse SupportsSnapshot property");
      if (this.supportsSnapshot && this.channel.Ecu.Properties.ContainsKey("SnapshotRecordNumber") && !uint.TryParse(this.channel.Ecu.Properties["SnapshotRecordNumber"], out this.snapshotRecordNumber))
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Error parsing Snapshot Record Number");
      if (this.channel.Ecu.Properties.ContainsKey("SupportsPermanentFaults") && !bool.TryParse(this.channel.Ecu.Properties["SupportsPermanentFaults"], out this.supportsPermanent))
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse SupportsPermanentFaults property");
    }
    else if (this.channel.IsRollCall && this.channel.Online && this.channel.Ecu.RollCallManager.IsSnapshotSupported(this.channel))
      this.SupportsSnapshot = true;
    if (this.channel.Ecu.Properties.ContainsKey("FaultResetDelay") && !int.TryParse(this.channel.Ecu.Properties["FaultResetDelay"], out this.faultResetDelay))
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse FaultResetDelay property");
    this.environmentDataDescriptions = new ServiceCollection(c, ServiceTypes.Environment);
    this.hasFaultDescriptions = true;
    if (!this.channel.Ecu.Properties.ContainsKey(nameof (HasFaultDescriptions)) || bool.TryParse(this.channel.Ecu.Properties[nameof (HasFaultDescriptions)], out this.hasFaultDescriptions))
      return;
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse HasFaultDescriptions property");
  }

  internal void UpdateFromRollCall(
    Dictionary<string, byte?> codes,
    Type type,
    TimeSpan agedPersistFor)
  {
    this.UpdateFromRollCall(codes, type, false, agedPersistFor);
  }

  internal void UpdateFromRollCall(
    Dictionary<string, byte?> codes,
    Type type,
    bool permanent,
    TimeSpan agedPersistFor)
  {
    DateTime now = Sapi.Now;
    bool flag = false;
    foreach (KeyValuePair<string, byte?> code in codes)
    {
      FaultCode faultCode = this.AcquireCode(code.Key);
      flag |= faultCode.FaultCodeIncidents.AddFromRollCall(now, code.Value, type, permanent);
    }
    foreach (FaultCode faultCode in this)
    {
      if (!codes.Keys.Contains<string>(faultCode.Code))
        flag |= faultCode.FaultCodeIncidents.RemoveAgedFromRollCall(now, type, permanent, agedPersistFor);
    }
    if (flag)
      this.RaiseFaultCodeUpdate((Exception) null, false);
    this.haveBeenReadFromEcu = true;
  }

  internal void UpdateSnapshotFromRollCall(
    IEnumerable<Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>> frames)
  {
    DateTime now = Sapi.Now;
    bool flag = false;
    int num = 0;
    foreach (Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>> frame in frames)
    {
      FaultCode faultCode = this.AcquireCode(frame.Item1);
      FaultCodeIncident incident = new FaultCodeIncident(faultCode, now);
      incident.AcquireSnapshotFromRollCall(++num, frame.Item2);
      faultCode.Snapshots.Add(incident, false);
    }
    this.snapshotHasBeenReadFromEcu = true;
    foreach (FaultCode faultCode in this)
    {
      if (faultCode.Snapshots.SetCurrentTime(now))
        flag = true;
    }
    if (!flag)
      return;
    FireAndForget.Invoke((MulticastDelegate) this.SnapshotUpdateEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) null));
  }

  internal void InternalRead(bool explicitread)
  {
    bool flag = false;
    CaesarException e = (CaesarException) null;
    if (this.channel.ChannelHandle != null || this.channel.McdChannelHandle != null)
    {
      DateTime now = Sapi.Now;
      List<FaultCodeIncident> outList = new List<FaultCodeIncident>();
      try
      {
        this.InternalReadByFunction(ReadFunctions.NonPermanent, now, outList);
        if (this.supportsPermanent)
          this.InternalReadByFunction(ReadFunctions.Permanent, now, outList);
      }
      catch (CaesarException ex)
      {
        e = ex;
        if (outList.Count == 0)
        {
          if (Sapi.GetSapi().LogFiles.Logging)
          {
            if (!this.haveLoggedFaultReadException)
            {
              Sapi.GetSapi().LogFiles.LabelLog(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fault codes may be unavailable - {0}", (object) e.Message), this.channel.Ecu);
              this.haveLoggedFaultReadException = true;
            }
          }
        }
      }
      for (int index = 0; index < outList.Count; ++index)
      {
        FaultCodeIncident incident = outList[index];
        if (incident.FaultCode.ManipulatedValue == null)
          incident.FaultCode.FaultCodeIncidents.Add(incident, this.uds && this.allowEnvironmentRead);
      }
      foreach (FaultCode faultCode in this)
      {
        if (faultCode.ManipulatedValue != null)
          faultCode.ManipulatedValue.UpdateEndTime(now);
        if (faultCode.FaultCodeIncidents.SetCurrentTime(now))
          flag = true;
      }
    }
    else if (this.channel.IsRollCall & explicitread)
    {
      try
      {
        this.channel.Ecu.RollCallManager.ReadFaultCodes(this.channel);
      }
      catch (CaesarException ex)
      {
        e = ex;
      }
    }
    if (((flag ? 1 : (e != null ? 1 : 0)) | (explicitread ? 1 : 0)) != 0)
      this.RaiseFaultCodeUpdate((Exception) e, explicitread);
    this.haveBeenReadFromEcu = true;
    this.channel.SetCommunicationsState(CommunicationsState.Online);
  }

  internal void RaiseFaultCodeUpdate(Exception e, bool explicitread)
  {
    FireAndForget.Invoke((MulticastDelegate) this.FaultCodesUpdateEvent, (object) this, (EventArgs) new ResultEventArgs(e));
    if (!explicitread)
      return;
    this.channel.SyncDone(e);
  }

  internal void InternalReset()
  {
    CaesarException e = (CaesarException) null;
    if (this.faultResetDelay > 0)
    {
      int num = 0;
      while (this.channel.ChannelRunning && num++ < this.faultResetDelay)
        Thread.Sleep(1000);
      if (!this.channel.ChannelRunning)
        return;
    }
    if (this.channel.ChannelHandle != null)
    {
      if (this.uds)
        this.channel.ChannelHandle.ClearErrorGroup((ErrorGroup) 5);
      else
        this.channel.ChannelHandle.ClearErrors();
      if (this.channel.IsChannelErrorSet)
        e = new CaesarException(this.channel.ChannelHandle);
    }
    else if (this.channel.McdChannelHandle != null)
    {
      string qualifier = this.channel.Ecu.Properties.GetValue<string>("McdFaultClearService", "JobClearDTC");
      if (qualifier != null)
      {
        try
        {
          this.channel.McdChannelHandle.GetService(qualifier).Execute(0);
        }
        catch (McdException ex)
        {
          e = new CaesarException(ex);
        }
      }
    }
    else if (this.channel.Ecu.RollCallManager != null)
    {
      try
      {
        this.channel.Ecu.RollCallManager.ClearErrors(this.channel);
      }
      catch (CaesarException ex)
      {
        e = ex;
      }
    }
    this.channel.SyncDone((Exception) e);
    this.channel.SetCommunicationsState(CommunicationsState.Online);
  }

  internal void InternalReadSnapshot(bool explicitread)
  {
    CaesarException e = (CaesarException) null;
    bool flag = false;
    if (this.channel.ChannelHandle != null)
    {
      if (this.supportsSnapshot && !this.channel.IsChannelErrorSet)
      {
        DateTime now = Sapi.Now;
        using (CaesarDiagServiceIO dsio = this.channel.ChannelHandle.OpenErrorList((ErrorPartFunction) 5, (ErrorGroup) 5, (ErrorStatusFlag) 13, (ErrorSeverityFlag) (int) ushort.MaxValue, (ErrorExtendedData) (int) this.snapshotRecordNumber, (ErrorEnvType) 0))
        {
          if (dsio != null)
          {
            ushort errorCount = dsio.ErrorCount;
            for (ushort dtcIndex = 0; (int) dtcIndex < (int) errorCount; ++dtcIndex)
            {
              string errorComfortNumber = dsio.GetErrorComfortNumber(dtcIndex);
              FaultCode faultCode = this[this.CaesarCodeToIndex(errorComfortNumber)];
              if (faultCode != null)
              {
                FaultCodeIncident incident = new FaultCodeIncident(faultCode, now);
                incident.AcquireSnapshot(dsio, dtcIndex);
                faultCode.Snapshots.Add(incident, false);
              }
              else
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not associate DTC of snapshot record ({0}) to a fault defined in the CBF", (object) errorComfortNumber));
            }
          }
        }
        foreach (FaultCode faultCode in this)
        {
          if (faultCode.Snapshots.SetCurrentTime(now))
            flag = true;
        }
        this.snapshotHasBeenReadFromEcu = true;
      }
      if (this.channel.IsChannelErrorSet && this.channel.ChannelHandle != null)
        e = new CaesarException(this.channel.ChannelHandle);
    }
    else if (this.channel.McdChannelHandle == null)
    {
      if (this.channel.IsRollCall)
      {
        try
        {
          this.channel.Ecu.RollCallManager.ReadSnapshot(this.channel);
          this.snapshotHasBeenReadFromEcu = true;
        }
        catch (CaesarException ex)
        {
          e = ex;
        }
      }
    }
    if (((flag ? 1 : (e != null ? 1 : 0)) | (explicitread ? 1 : 0)) != 0)
    {
      FireAndForget.Invoke((MulticastDelegate) this.SnapshotUpdateEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) e));
      if (explicitread)
        this.channel.SyncDone((Exception) e);
    }
    this.channel.SetCommunicationsState(CommunicationsState.Online);
  }

  internal void Invalidate()
  {
    foreach (FaultCode faultCode in this)
      faultCode.FaultCodeIncidents.Invalidate();
    this.RaiseFaultCodeUpdate((Exception) null, false);
  }

  protected override void AcquireList()
  {
    if (this.channel.EcuHandle != null)
    {
      uint errorCount = this.channel.EcuHandle.ErrorCount;
      for (uint index = 0; index < errorCount; ++index)
      {
        string errorComfortCode = this.channel.EcuHandle.GetErrorComfortCode(index);
        this.Add(this.CaesarCodeToIndex(errorComfortCode), errorComfortCode);
      }
    }
    else
    {
      if (this.channel.McdEcuHandle == null)
        return;
      foreach (McdDBDiagTroubleCode dbDiagTroubleCode in this.channel.McdEcuHandle.DBDiagTroubleCodes)
        this.Add(dbDiagTroubleCode.DisplayTroubleCode);
      this.mcdDiagnosticTroubleCodesJob = this.channel.Services["DJ_JobDiagnosticTroubleCodes"];
      ServiceOutputValue structuredOutputValue = this.mcdDiagnosticTroubleCodesJob?.StructuredOutputValues["DtcResponseMux"]?.StructuredOutputValues["ReportDTCs"]?.StructuredOutputValues["DTC_Record"]?.StructuredOutputValues["Structure_of_DTC"];
      this.McdEnvironmentData = structuredOutputValue?.StructuredOutputValues["DTC_Environment_Data_Descriptor"];
      this.McdEnvironmentDataDescriptions = this.McdEnvironmentData?.StructuredOutputValues["DTCExtendedDataRecord_Wrapper"]?.StructuredOutputValues["DTC_Environment_Data_Descriptor"];
      if (!this.channel.Ecu.Properties.GetValue<bool>("SupportsEnvironmentSnapshotRead", true))
        return;
      this.McdSnapshotData = structuredOutputValue?.StructuredOutputValues["DTC_Snapshot_Data"];
      this.McdSnapshotDescriptions = this.McdSnapshotData?.StructuredOutputValues["DTCSnapshortData_Wrapper"]?.StructuredOutputValues["DIDs_and_content"]?.StructuredOutputValues["DTCSnapshortRecord_Wrapper"]?.StructuredOutputValues["Content_of_DID"];
      if (this.McdSnapshotDescriptions == null)
        return;
      this.SupportsEnvironmentSnapshot = this.McdSnapshotDescriptions.StructuredOutputValues.Any<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (sov => sov.StructuredOutputValues != null && sov.StructuredOutputValues.Any<ServiceOutputValue>()));
      if (!this.SupportsEnvironmentSnapshot)
        return;
      this.channel.Ecu.Properties["PromotionPrefix"] = "ENV_SnapshotRecordNumber";
    }
  }

  internal void SetCurrentTime(DateTime time)
  {
    bool flag1 = false;
    bool flag2 = false;
    foreach (FaultCode faultCode in this)
    {
      if (faultCode.FaultCodeIncidents.SetCurrentTime(time))
        flag1 = true;
      if (faultCode.Snapshots.SetCurrentTime(time))
        flag2 = true;
    }
    if (flag1)
      this.RaiseFaultCodeUpdate((Exception) null, false);
    if (!flag2)
      return;
    FireAndForget.Invoke((MulticastDelegate) this.SnapshotUpdateEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) null));
  }

  internal FaultCode Add(string errcode, string caesarcode = null)
  {
    FaultCode faultCode = new FaultCode(this.channel, errcode, caesarcode);
    faultCode.AcquireText();
    this.Items.Add(faultCode);
    return faultCode;
  }

  public void Reset(bool synchronous)
  {
    this.channel.QueueAction((object) CommunicationsState.ResetFaults, synchronous);
  }

  public void Read(bool synchronous)
  {
    this.channel.QueueAction((object) CommunicationsState.ReadFaults, synchronous);
  }

  public void ReadSnapshot(bool synchronous)
  {
    if (!this.supportsSnapshot)
      throw new InvalidOperationException("The ECU does not support snapshot read");
    this.channel.QueueAction((object) CommunicationsState.ReadSnapshot, synchronous);
  }

  public IEnumerable<FaultCode> Current => this.GetCurrentByFunction(ReadFunctions.NonPermanent);

  public IEnumerable<FaultCode> GetCurrentByFunction(ReadFunctions match)
  {
    Collection<FaultCode> output = new Collection<FaultCode>();
    this.CopyCurrent(match, output);
    return (IEnumerable<FaultCode>) output;
  }

  public void CopyCurrent(Collection<FaultCode> output)
  {
    this.CopyCurrent(ReadFunctions.NonPermanent, output);
  }

  public void CopyCurrent(ReadFunctions match, Collection<FaultCode> output)
  {
    if (output == null)
      throw new ArgumentNullException(nameof (output));
    lock (this.Items)
    {
      foreach (FaultCode faultCode in this.Where<FaultCode>((Func<FaultCode, bool>) (fc => fc.FaultCodeIncidents.GetCurrentByFunction(match) != null)))
        output.Add(faultCode);
    }
  }

  public Channel Channel => this.channel;

  public FaultCode this[string code]
  {
    get
    {
      return this.FirstOrDefault<FaultCode>((Func<FaultCode, bool>) (item => string.Equals(item.Code, code, StringComparison.Ordinal)));
    }
  }

  public FaultCode GetItemExtended(string code)
  {
    FaultCode itemExtended = this[code];
    if (itemExtended == null && this.uds)
    {
      switch (code.Length)
      {
        case 6:
          string isoCode = FaultCode.ConvertUdsCodeToIsoCode(code);
          if (isoCode != null)
          {
            itemExtended = this[isoCode];
            break;
          }
          break;
        case 7:
          string udsCode = FaultCode.ConvertIsoCodeToUdsCode(code);
          if (udsCode != null)
          {
            itemExtended = this[udsCode];
            break;
          }
          break;
      }
    }
    return itemExtended;
  }

  public bool AutoRead
  {
    get => this.autoRead;
    set => this.autoRead = value;
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;

  public bool AllowEnvironmentRead
  {
    get => this.allowEnvironmentRead;
    set => this.allowEnvironmentRead = value;
  }

  public ServiceCollection EnvironmentDataDescriptions => this.environmentDataDescriptions;

  public bool SupportsSnapshot
  {
    get => this.supportsSnapshot;
    set
    {
      if (value == this.supportsSnapshot)
        return;
      this.supportsSnapshot = value;
      this.Channel.Ecu.Properties[nameof (SupportsSnapshot)] = this.supportsSnapshot.ToString();
    }
  }

  internal Instrument GetRollCallSnapshotDescription(string qualifier)
  {
    Instrument snapshotDescription = this.rollcallSnapshotDescriptions.FirstOrDefault<Instrument>((Func<Instrument, bool>) (i => i.Qualifier == qualifier));
    if (snapshotDescription == (Instrument) null)
    {
      snapshotDescription = this.channel.Ecu.RollCallManager.CreateBaseInstrument(this.channel, qualifier);
      if (snapshotDescription != (Instrument) null)
        this.rollcallSnapshotDescriptions.Add(snapshotDescription);
    }
    return snapshotDescription;
  }

  public bool SupportsFaultRead => this.supportsFaultRead;

  public bool HasFaultDescriptions => this.hasFaultDescriptions;

  public bool HaveBeenReadFromEcu => this.haveBeenReadFromEcu;

  public bool SnapshotHasBeenReadFromEcu => this.snapshotHasBeenReadFromEcu;

  public event FaultCodesUpdateEventHandler FaultCodesUpdateEvent;

  public event SnapshotUpdateEventHandler SnapshotUpdateEvent;

  public new IEnumerator<FaultCode> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<FaultCode>) new List<FaultCode>((IEnumerable<FaultCode>) this.Items).GetEnumerator();
  }

  IEnumerator<FaultCode> IEnumerable<FaultCode>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  private FaultCode AcquireCode(CaesarDiagServiceIO dsio, ushort i)
  {
    string caesarCode = dsio.GetErrorComfortNumber(i);
    if (this.Channel.Ecu.FaultCodeCanBeDuplicate)
    {
      string str = (string) null;
      for (ushort index = 0; (int) index < (int) dsio.GetErrorEnvCount(i); ++index)
      {
        string errorEnvText = dsio.GetErrorEnvText(i, index);
        if (errorEnvText.StartsWith("FMI", StringComparison.Ordinal))
        {
          str = errorEnvText.Split(":".ToCharArray())[0];
          break;
        }
      }
      if (str != null)
        caesarCode = $"{caesarCode}:{str}";
    }
    return this.AcquireCode(this.CaesarCodeToIndex(caesarCode));
  }

  private FaultCode AcquireCode(string errcode)
  {
    FaultCode faultCode = this[errcode];
    if (faultCode == null)
    {
      lock (this.Items)
        faultCode = this.Add(errcode);
    }
    return faultCode;
  }

  private void InternalReadByFunction(
    CaesarDiagServiceIO dsio,
    ReadFunctions function,
    DateTime thisTimeRead,
    List<FaultCodeIncident> outList)
  {
    if (dsio != null)
    {
      if (dsio.IsNegativeResponse)
        throw new CaesarException(dsio);
      for (ushort i = 0; (int) i < (int) dsio.ErrorCount; ++i)
      {
        FaultCode faultCode = this.AcquireCode(dsio, i);
        if (this.uds && faultCode.LongNumber == 0U)
          faultCode.LongNumber = dsio.GetErrorLongNumber(i);
        FaultCodeIncident faultCodeIncident1 = (FaultCodeIncident) null;
        for (int index = 0; index < outList.Count; ++index)
        {
          FaultCodeIncident faultCodeIncident2 = outList[index];
          if (faultCodeIncident2.FaultCode == faultCode)
            faultCodeIncident1 = faultCodeIncident2;
        }
        if (faultCodeIncident1 == null)
        {
          FaultCodeIncident faultCodeIncident3 = new FaultCodeIncident(faultCode, thisTimeRead);
          faultCodeIncident3.Acquire(dsio, i, function);
          outList.Add(faultCodeIncident3);
          if (this.allowEnvironmentRead && !this.uds)
            faultCodeIncident3.AcquireEnvironmentData(dsio, i);
        }
        else
          faultCodeIncident1.UpdatePartFunction(function, true);
      }
    }
    else if (this.channel.IsChannelErrorSet)
      throw new CaesarException(this.channel.ChannelHandle);
  }

  private void InternalReadPermanent(DateTime thisTimeRead, List<FaultCodeIncident> outList)
  {
    if (this.channel.IsChannelErrorSet)
      return;
    ByteMessage byteMessage = new ByteMessage(this.channel, new Dump("1915"));
    byteMessage.InternalDoMessage(true);
    if (byteMessage.Response == null || byteMessage.Response.Data[0] != (byte) 89 || byteMessage.Response.Data.Count <= 3)
      return;
    byte[] array;
    for (array = byteMessage.Response.Data.Skip<byte>(3).ToArray<byte>(); array.Length >= 4; array = ((IEnumerable<byte>) array).Skip<byte>(4).ToArray<byte>())
    {
      FaultCode fc = this.AcquireCode(new Dump(((IEnumerable<byte>) array).Take<byte>(3)).ToString());
      if (fc.LongNumber == 0U)
        fc.LongNumber = (uint) (((int) array[0] << 16 /*0x10*/) + ((int) array[1] << 8)) + (uint) array[2];
      FaultCodeIncident faultCodeIncident = outList.FirstOrDefault<FaultCodeIncident>((Func<FaultCodeIncident, bool>) (i => i.FaultCode == fc));
      if (faultCodeIncident == null)
        outList.Add(new FaultCodeIncident(fc, thisTimeRead, (FaultCodeStatus) ((int) array[3] | 256 /*0x0100*/)));
      else
        faultCodeIncident.UpdatePartFunction(ReadFunctions.Permanent, true);
    }
    if (array.Length == 0)
      return;
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Implausible length of $59 response for {0}", (object) this.channel.Ecu.Name));
  }

  private void InternalReadByFunction(
    McdDiagComPrimitive dsio,
    ReadFunctions function,
    DateTime thisTimeRead,
    List<FaultCodeIncident> outList)
  {
    if (dsio.IsNegativeResponse)
      throw new CaesarException(dsio);
    foreach (McdResponseParameter responseParameter in dsio.AllPositiveResponseParameters.Where<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.IsDiagnosticTroubleCode)))
    {
      McdDBDiagTroubleCode dbDiagTroubleCode = responseParameter.DBDiagTroubleCode;
      FaultCode faultCode = this.AcquireCode(dbDiagTroubleCode != null ? dbDiagTroubleCode.DisplayTroubleCode : Convert.ToUInt32(responseParameter.Value.CodedValue, (IFormatProvider) CultureInfo.InvariantCulture).ToString("X6", (IFormatProvider) CultureInfo.InvariantCulture));
      FaultCodeIncident faultCodeIncident1 = (FaultCodeIncident) null;
      for (int index = 0; index < outList.Count; ++index)
      {
        FaultCodeIncident faultCodeIncident2 = outList[index];
        if (faultCodeIncident2.FaultCode == faultCode)
          faultCodeIncident1 = faultCodeIncident2;
      }
      if (faultCodeIncident1 == null)
      {
        FaultCodeIncident faultCodeIncident3 = new FaultCodeIncident(faultCode, thisTimeRead);
        faultCodeIncident3.Acquire(responseParameter.Parent.Parameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.Qualifier == "DTC_Status_Bits")), function);
        outList.Add(faultCodeIncident3);
      }
      else
        faultCodeIncident1.UpdatePartFunction(function, true);
    }
  }

  private void InternalReadByFunction(
    ReadFunctions function,
    DateTime thisTimeRead,
    List<FaultCodeIncident> outList)
  {
    if (this.uds)
    {
      if (this.channel.ChannelHandle != null)
      {
        if (function == ReadFunctions.Permanent)
        {
          this.InternalReadPermanent(thisTimeRead, outList);
        }
        else
        {
          using (CaesarDiagServiceIO dsio = this.channel.ChannelHandle.OpenErrorList((ErrorPartFunction) 2, (ErrorGroup) 5, (ErrorStatusFlag) (int) this.udsFilterByte, (ErrorSeverityFlag) (int) ushort.MaxValue, (ErrorExtendedData) (int) ushort.MaxValue, (ErrorEnvType) 0))
            this.InternalReadByFunction(dsio, function, thisTimeRead, outList);
        }
      }
      else
      {
        if (this.channel.McdChannelHandle == null)
          return;
        try
        {
          McdDiagComPrimitive service = this.channel.McdChannelHandle.GetService("JobDiagnosticTroubleCodes");
          service.SetInput("PartFunction", function == ReadFunctions.Permanent ? (object) "ReportDTCWithPermanentStatus" : (object) "ReportDTCbyStatusMask");
          service.SetInput("ErrorStatusFlags", (object) "User specific");
          service.SetInput("UserSpecificErrorStatusFlag", (object) this.udsFilterByte);
          service.SetInput("EnvironmentData", (object) "No Environment");
          service.SetInput("SnapshotRecordNumber", (object) "No Records");
          this.channel.McdChannelHandle.SuppressJobInfo = true;
          service.Execute(1000);
          this.InternalReadByFunction(service, function, thisTimeRead, outList);
        }
        catch (McdException ex)
        {
          throw new CaesarException(ex);
        }
        finally
        {
          this.channel.McdChannelHandle.SuppressJobInfo = false;
        }
      }
    }
    else
    {
      if (this.channel.ChannelHandle == null)
        return;
      using (CaesarDiagServiceIO dsio = this.channel.ChannelHandle.OpenErrorList((ErrorPartFunction) (int) ushort.MaxValue, (ErrorGroup) (int) ushort.MaxValue, (ErrorStatusFlag) (int) ushort.MaxValue, (ErrorSeverityFlag) (int) ushort.MaxValue, (ErrorExtendedData) (int) ushort.MaxValue, (ErrorEnvType) -1))
        this.InternalReadByFunction(dsio, function, thisTimeRead, outList);
    }
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ItemExtended is deprecated, please use GetItemExtended(string) instead.")]
  public FaultCode get_ItemExtended(string code) => this.GetItemExtended(code);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("CopyCurrent(ArrayList) is deprecated, please use CopyCurrent(Collection<FaultCode>) instead.")]
  public void CopyCurrent(ArrayList output) => this.CopyCurrent(ReadFunctions.NonPermanent, output);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("CopyCurrent(ReadFunctions, ArrayList) is deprecated, please use CopyCurrent(ReadFunctions, Collection<FaultCode>) instead.")]
  public void CopyCurrent(ReadFunctions match, ArrayList output)
  {
    Collection<FaultCode> output1 = new Collection<FaultCode>();
    this.CopyCurrent(match, output1);
    foreach (FaultCode faultCode in output1)
      output.Add((object) faultCode);
  }

  private string CaesarCodeToIndex(string caesarCode)
  {
    return !this.uds || caesarCode.Length != 7 ? caesarCode : FaultCode.ConvertIsoCodeToUdsCode(caesarCode);
  }

  internal bool SupportsEnvironmentSnapshot { get; private set; }

  internal ServiceOutputValue McdSnapshotDescriptions { get; private set; }

  internal ServiceOutputValue McdSnapshotData { get; private set; }

  internal ServiceOutputValue McdEnvironmentDataDescriptions { get; private set; }

  internal ServiceOutputValue McdEnvironmentData { get; private set; }
}
