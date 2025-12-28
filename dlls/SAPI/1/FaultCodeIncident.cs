// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FaultCodeIncident
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FaultCodeIncident
{
  private const string RecordNumberPrefix = "Case_";
  private const string RecordNumberDefault = "Case_Default";
  private EnvironmentDataCollection environmentDatas;
  private FaultCode faultCode;
  private MilStatus mil;
  private StoredStatus stored;
  private ActiveStatus active;
  private TestNotCompleteStatus testNotComplete;
  private TestFailedSinceLastClearStatus testFailedSinceLastClear;
  private ImmediateStatus immediate;
  private PendingStatus pending;
  private ReadFunctions functions;
  private DateTime startTime;
  private DateTime endTime;

  internal FaultCodeIncident(FaultCode faultCode, DateTime thisTimeRead, FaultCodeStatus status)
    : this(faultCode, thisTimeRead, (status & FaultCodeStatus.Active) != FaultCodeStatus.None ? ActiveStatus.Active : ActiveStatus.NotActive, (status & FaultCodeStatus.Stored) != FaultCodeStatus.None ? StoredStatus.Stored : StoredStatus.NotStored, (status & FaultCodeStatus.Pending) != FaultCodeStatus.None ? PendingStatus.Pending : PendingStatus.NotPending, (status & FaultCodeStatus.Mil) != FaultCodeStatus.None ? MilStatus.On : MilStatus.Off, (status & FaultCodeStatus.TestFailedSinceLastClear) != FaultCodeStatus.None ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear, (status & FaultCodeStatus.Immediate) != FaultCodeStatus.None ? ImmediateStatus.Immediate : ImmediateStatus.NotImmediate, (status & FaultCodeStatus.Permanent) != FaultCodeStatus.None ? ReadFunctions.NonPermanent | ReadFunctions.Permanent : ReadFunctions.NonPermanent)
  {
  }

  internal FaultCodeIncident(
    FaultCode faultCode,
    DateTime thisTimeRead,
    ActiveStatus activeStatus,
    StoredStatus storedStatus,
    PendingStatus pendingStatus,
    MilStatus milStatus,
    TestFailedSinceLastClearStatus testFailedSinceLastClearStatus,
    ImmediateStatus immediateStatus,
    ReadFunctions functions)
    : this(faultCode, thisTimeRead)
  {
    this.active = activeStatus;
    this.stored = storedStatus;
    this.pending = pendingStatus;
    this.testFailedSinceLastClear = testFailedSinceLastClearStatus;
    this.mil = milStatus;
    this.immediate = immediateStatus;
    this.functions = functions;
    this.CheckOnBoardDiagnosticState();
  }

  internal FaultCodeIncident(FaultCode faultCode, DateTime thisTimeRead)
    : this(faultCode)
  {
    this.startTime = thisTimeRead;
    this.endTime = thisTimeRead;
  }

  internal FaultCodeIncident(FaultCode faultCode)
  {
    this.mil = MilStatus.Undefined;
    this.stored = StoredStatus.Undefined;
    this.active = ActiveStatus.Undefined;
    this.testNotComplete = TestNotCompleteStatus.Undefined;
    this.testFailedSinceLastClear = TestFailedSinceLastClearStatus.Undefined;
    this.immediate = ImmediateStatus.Undefined;
    this.pending = PendingStatus.Undefined;
    this.faultCode = faultCode;
    this.environmentDatas = new EnvironmentDataCollection();
  }

  internal void Acquire(CaesarDiagServiceIO dsio, ushort i, ReadFunctions singleFunction)
  {
    this.mil = (MilStatus) dsio.GetErrorMil(i);
    this.stored = (StoredStatus) dsio.GetErrorStored(i);
    this.active = (ActiveStatus) dsio.GetErrorActive(i);
    this.testNotComplete = (TestNotCompleteStatus) dsio.GetErrorTestNotComplete(i);
    this.pending = (PendingStatus) dsio.GetErrorPending(i);
    this.testFailedSinceLastClear = ((int) dsio.GetErrorStatus(i) & 32 /*0x20*/) == 32 /*0x20*/ ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear;
    this.UpdatePartFunction(singleFunction, false);
  }

  internal void Acquire(McdResponseParameter statuses, ReadFunctions singleFunction)
  {
    foreach (McdResponseParameter parameter in statuses.Parameters)
    {
      bool? nullable = new bool?();
      object codedValue = parameter.Value.CodedValue;
      if (codedValue != null)
      {
        Type type = codedValue.GetType();
        if (type == typeof (bool))
          nullable = new bool?((bool) codedValue);
        else if (type == typeof (uint))
          nullable = new bool?((uint) codedValue != 0U);
        else if (type == typeof (string) && codedValue is string s)
        {
          bool result1;
          if (bool.TryParse(s, out result1))
          {
            nullable = new bool?(result1);
          }
          else
          {
            int result2;
            if (int.TryParse(s, out result2))
              nullable = new bool?(result2 != 0);
          }
        }
      }
      if (nullable.HasValue)
      {
        switch (parameter.Qualifier)
        {
          case "DTC_Status_Active":
            this.active = nullable.Value ? ActiveStatus.Active : ActiveStatus.NotActive;
            continue;
          case "DTC_Status_Pending":
            this.pending = nullable.Value ? PendingStatus.Pending : PendingStatus.NotPending;
            continue;
          case "DTC_Status_Stored":
            this.stored = nullable.Value ? StoredStatus.Stored : StoredStatus.NotStored;
            continue;
          case "DTC_Status_FailedSinceLastClear":
            this.testFailedSinceLastClear = nullable.Value ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear;
            continue;
          case "DTC_Status_MIL":
            this.mil = nullable.Value ? MilStatus.On : MilStatus.Off;
            continue;
          default:
            continue;
        }
      }
    }
    this.UpdatePartFunction(singleFunction, false);
  }

  internal void UpdateEndTime(DateTime time) => this.endTime = time;

  internal void UpdateStartTime(DateTime time) => this.startTime = time;

  internal void UpdatePartFunction(ReadFunctions singleFunction, bool append)
  {
    if (append)
      this.functions |= singleFunction;
    else
      this.functions = singleFunction;
  }

  internal static FaultCodeIncident FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    Channel channel)
  {
    string str = element.Attribute(format[TagName.Code]).Value;
    FaultCode faultCode = channel.FaultCodes.GetItemExtended(str);
    if (faultCode == null)
    {
      faultCode = channel.FaultCodes.Add(str);
      if (channel.Ecu.RollCallManager == null)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) Sapi.GetSapi(), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fault code not defined in CBF: {0}", (object) str));
    }
    FaultCodeIncident incident = new FaultCodeIncident(faultCode);
    incident.startTime = Sapi.TimeFromString(element.Attribute(format[TagName.StartTime]).Value);
    incident.endTime = Sapi.TimeFromString(element.Attribute(format[TagName.EndTime]).Value);
    XAttribute xattribute = element.Attribute(format[TagName.ReadFunctions]);
    incident.functions = xattribute != null ? (ReadFunctions) Convert.ToUInt32(xattribute.Value, (IFormatProvider) CultureInfo.InvariantCulture) : ReadFunctions.NonPermanent;
    if (incident.Functions != ReadFunctions.Snapshot)
    {
      incident.mil = (MilStatus) Convert.ToByte(element.Element(format[TagName.MaintenanceIndicatorLamp]).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      incident.stored = (StoredStatus) Convert.ToByte(element.Element(format[TagName.Stored]).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      incident.active = (ActiveStatus) Convert.ToByte(element.Element(format[TagName.Active]).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      XElement xelement1 = element.Element(format[TagName.Pending]);
      XElement xelement2 = element.Element(format[TagName.TestNotComplete]);
      XElement xelement3 = element.Element(format[TagName.TestFailedSinceLastClear]);
      if (xelement1 != null)
        incident.pending = (PendingStatus) Convert.ToByte(xelement1.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (xelement2 != null)
        incident.testNotComplete = (TestNotCompleteStatus) Convert.ToByte(xelement2.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (xelement3 != null)
        incident.testFailedSinceLastClear = (TestFailedSinceLastClearStatus) Convert.ToByte(xelement3.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      XElement xelement4 = element.Element(format[TagName.Immediate]);
      if (xelement4 != null)
        incident.immediate = (ImmediateStatus) Convert.ToByte(xelement4.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }
    else if (channel.IsRollCall)
      channel.FaultCodes.SupportsSnapshot = true;
    EnvironmentDataCollection temporaryEnvData = new EnvironmentDataCollection();
    foreach (XElement element1 in element.Elements(format[TagName.EnvironmentDatas]).First<XElement>().Elements(format[TagName.EnvironmentData]))
    {
      EnvironmentData environmentDataValue = EnvironmentData.FromXElement(element1, format, incident);
      if (environmentDataValue != null)
        temporaryEnvData.Add(environmentDataValue);
    }
    incident.BuildCompoundContent(temporaryEnvData);
    incident.CheckOnBoardDiagnosticState();
    return incident;
  }

  internal XElement GetXElement(DateTime startTime, DateTime endTime)
  {
    if (this.EndTime < startTime || this.StartTime > endTime)
      return (XElement) null;
    DateTime time1 = this.StartTime > startTime ? this.StartTime : startTime;
    DateTime time2 = this.EndTime < endTime ? this.EndTime : endTime;
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    XElement xelement = new XElement(currentFormat[TagName.FaultCode], new object[4]
    {
      (object) new XAttribute(currentFormat[TagName.Code], (object) this.FaultCode.Code),
      (object) new XAttribute(currentFormat[TagName.StartTime], (object) Sapi.TimeToString(time1)),
      (object) new XAttribute(currentFormat[TagName.EndTime], (object) Sapi.TimeToString(time2)),
      (object) new XAttribute(currentFormat[TagName.ReadFunctions], (object) this.Functions.ToNumberString())
    });
    if (this.Functions != ReadFunctions.Snapshot)
    {
      xelement.Add((object) new XElement(currentFormat[TagName.MaintenanceIndicatorLamp], (object) this.Mil.ToNumberString()));
      xelement.Add((object) new XElement(currentFormat[TagName.Stored], (object) this.Stored.ToNumberString()));
      xelement.Add((object) new XElement(currentFormat[TagName.Active], (object) this.Active.ToNumberString()));
      xelement.Add((object) new XElement(currentFormat[TagName.Pending], (object) this.Pending.ToNumberString()));
      xelement.Add((object) new XElement(currentFormat[TagName.TestNotComplete], (object) this.TestNotComplete.ToNumberString()));
      xelement.Add((object) new XElement(currentFormat[TagName.TestFailedSinceLastClear], (object) this.TestFailedSinceLastClear.ToNumberString()));
      xelement.Add((object) new XElement(currentFormat[TagName.Immediate], (object) this.Immediate.ToNumberString()));
    }
    XElement content = new XElement(currentFormat[TagName.EnvironmentDatas]);
    foreach (EnvironmentData environmentData in this.EnvironmentDatas.Where<EnvironmentData>((Func<EnvironmentData, bool>) (ed => ed.CompoundDescription == null)))
      content.Add((object) environmentData.XElement);
    xelement.Add((object) content);
    return xelement;
  }

  internal void InternalReadEnvironmentData()
  {
    if (this.faultCode.Channel.IsChannelErrorSet)
      return;
    if (this.faultCode.Channel.ChannelHandle != null)
    {
      using (CaesarDiagServiceIO dsio = this.faultCode.Channel.ChannelHandle.OpenErrorList((ErrorPartFunction) 6, (ErrorGroup) (int) this.faultCode.LongNumber, (ErrorStatusFlag) 13, (ErrorSeverityFlag) (int) ushort.MaxValue, (ErrorExtendedData) (int) ushort.MaxValue, (ErrorEnvType) -1))
      {
        if (dsio == null)
          return;
        this.AcquireEnvironmentData(dsio, (ushort) 0);
      }
    }
    else
    {
      if (this.faultCode.Channel.McdChannelHandle == null)
        return;
      McdDiagComPrimitive service = this.faultCode.Channel.McdChannelHandle.GetService("JobDiagnosticTroubleCodes");
      service.SetInput("PartFunction", (object) "ReportDTCbyDTCNumber");
      service.SetInput("DTC_or_Group_Number", (object) this.faultCode.Code);
      service.SetInput("EnvironmentData", (object) "All Environment");
      service.SetInput("SnapshotRecordNumber", this.faultCode.Channel.FaultCodes.SupportsEnvironmentSnapshot ? (object) "All Records" : (object) "No Records");
      try
      {
        this.faultCode.Channel.McdChannelHandle.SuppressJobInfo = true;
        service.Execute(0, true);
        this.AcquireEnvironmentData(service);
      }
      catch (McdException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.faultCode.Channel, "InternalReadEnvironmentData: " + ex.Message);
      }
      finally
      {
        this.faultCode.Channel.McdChannelHandle.SuppressJobInfo = false;
      }
    }
  }

  internal void AcquireEnvironmentDataFromRollCall(byte? occurrenceCount)
  {
    if (!occurrenceCount.HasValue)
      return;
    this.EnvironmentDatas.Add(new EnvironmentData(this, this.FaultCode.Channel.FaultCodes.EnvironmentDataDescriptions[RollCall.ID.OccurrenceCount.ToNumberString()], (object) occurrenceCount.Value));
  }

  private void AcquireEnvironmentData(McdDiagComPrimitive dsio)
  {
    if (dsio.IsNegativeResponse)
      return;
    EnvironmentDataCollection temporaryEnvData = new EnvironmentDataCollection();
    ServiceOutputValue envDataRootDesc = this.faultCode.Channel.FaultCodes.McdEnvironmentData;
    McdResponseParameter responseParameter1 = dsio.AllPositiveResponseParameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.QualifierPath == envDataRootDesc.McdParameterQualifierPath));
    if (responseParameter1 != null)
    {
      foreach (McdResponseParameter responseParameter2 in responseParameter1.AllParameters.Where<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.IsEnvironmentalData || p.Qualifier.StartsWith("Case_", StringComparison.OrdinalIgnoreCase))))
      {
        McdResponseParameter envSet = responseParameter2;
        AcquireEnvironmentData(envSet, this.faultCode.Channel.FaultCodes.McdEnvironmentDataDescriptions.StructuredOutputValues.FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (sov => sov.ParameterQualifier == envSet.Qualifier)), 0);
      }
    }
    if (this.faultCode.Channel.FaultCodes.SupportsEnvironmentSnapshot)
    {
      ServiceOutputValue snapshotDataRootDesc = this.faultCode.Channel.FaultCodes.McdSnapshotData;
      McdResponseParameter responseParameter3 = dsio.AllPositiveResponseParameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.QualifierPath == snapshotDataRootDesc.McdParameterQualifierPath));
      if (responseParameter3 != null)
      {
        foreach (McdResponseParameter parameter in responseParameter3.Parameters)
        {
          McdResponseParameter snapshotRecordNumber = parameter.Parameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.Qualifier == "SnapshotRecordNumber"));
          if (snapshotRecordNumber != null && snapshotRecordNumber.Value != null && snapshotRecordNumber.Value.CodedValue != null)
          {
            int int32 = Convert.ToInt32(snapshotRecordNumber.Value.CodedValue, (IFormatProvider) CultureInfo.InvariantCulture);
            EnvironmentData environmentDataValue = new EnvironmentData(this, snapshotDataRootDesc.StructuredOutputValues.First<ServiceOutputValue>().StructuredOutputValues.FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov => ov.ParameterQualifier == snapshotRecordNumber.Qualifier)), int32, (object) null);
            environmentDataValue.Read(snapshotRecordNumber);
            temporaryEnvData.Add(environmentDataValue);
            foreach (McdResponseParameter responseParameter4 in parameter.AllParameters.Where<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.Qualifier.StartsWith("Case_", StringComparison.OrdinalIgnoreCase))))
            {
              McdResponseParameter envSet = responseParameter4;
              AcquireEnvironmentData(envSet, this.faultCode.Channel.FaultCodes.McdSnapshotDescriptions.StructuredOutputValues.FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (sov => sov.ParameterQualifier == envSet.Qualifier)), int32);
            }
          }
        }
      }
    }
    if (!temporaryEnvData.Any<EnvironmentData>())
      return;
    this.BuildCompoundContent(temporaryEnvData);

    void AcquireEnvironmentData(
      McdResponseParameter envSet,
      ServiceOutputValue descriptionStruct,
      int snapshotRecordNumber)
    {
      if (descriptionStruct == null)
        return;
      foreach (McdResponseParameter allParameter in envSet.AllParameters)
      {
        McdResponseParameter item = allParameter;
        ServiceOutputValue description = descriptionStruct.StructuredOutputValues.FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov => ov.ParameterQualifier == item.Qualifier));
        if (description != null)
        {
          EnvironmentData environmentDataValue = new EnvironmentData(this, description, snapshotRecordNumber, (object) null);
          environmentDataValue.Read(item);
          temporaryEnvData.Add(environmentDataValue);
        }
      }
    }
  }

  internal void AcquireEnvironmentData(CaesarDiagServiceIO dsio, ushort i)
  {
    ushort errorEnvCount = dsio.GetErrorEnvCount(i);
    if (errorEnvCount <= (ushort) 0)
      return;
    EnvironmentDataCollection temporaryEnvData = new EnvironmentDataCollection();
    for (ushort environmentDataIndex = 0; (int) environmentDataIndex < (int) errorEnvCount; ++environmentDataIndex)
    {
      Service environmentDataDescription = this.FaultCode.Channel.FaultCodes.EnvironmentDataDescriptions[dsio.GetErrorEnvQualifier(i, environmentDataIndex)];
      if (environmentDataDescription != (Service) null)
      {
        EnvironmentData environmentDataValue = new EnvironmentData(this, environmentDataDescription, (object) null);
        environmentDataValue.Read(dsio, i, environmentDataIndex);
        temporaryEnvData.Add(environmentDataValue);
      }
    }
    this.BuildCompoundContent(temporaryEnvData);
  }

  private void BuildCompoundContent(EnvironmentDataCollection temporaryEnvData)
  {
    foreach (CompoundEnvironmentData compoundEnvironmentData in this.faultCode.Channel.Ecu.CompoundEnvironmentDatas)
    {
      CompoundEnvironmentData compoundData = compoundEnvironmentData;
      if (temporaryEnvData[compoundData.Qualifier] == null)
      {
        EnvironmentData[] referenced = new EnvironmentData[compoundData.Referenced.Count];
        for (int l = 0; l < compoundData.Referenced.Count; l++)
        {
          EnvironmentData environmentData = temporaryEnvData.FirstOrDefault<EnvironmentData>((Func<EnvironmentData, bool>) (item => Regex.Match(item.Qualifier, compoundData.Referenced[l]).Success));
          if (environmentData != null)
          {
            referenced[l] = environmentData;
            if (compoundData.HideComponents)
              environmentData.Visible = false;
          }
        }
        this.EnvironmentDatas.Add(new EnvironmentData(this, compoundData, referenced));
      }
    }
    for (int index = 0; index < temporaryEnvData.Count; ++index)
      this.EnvironmentDatas.Add(temporaryEnvData[index]);
  }

  internal void AcquireSnapshot(CaesarDiagServiceIO dsio, ushort dtcIndex)
  {
    this.UpdatePartFunction(ReadFunctions.Snapshot, false);
    ushort snapshotRecordCount = dsio.GetErrorSnapshotRecordCount(dtcIndex);
    for (ushort index1 = 0; (int) index1 < (int) snapshotRecordCount; ++index1)
    {
      uint errorSnapshotCount = dsio.GetErrorSnapshotCount(dtcIndex, index1);
      uint snapshotRecordNumber = dsio.GetErrorSnapshotRecordNumber(dtcIndex, index1);
      for (ushort index2 = 0; (uint) index2 < errorSnapshotCount; ++index2)
      {
        uint diagServiceIoCount = dsio.GetErrorSnapshotDiagServiceIOCount(dtcIndex, index1, index2);
        for (ushort index3 = 0; (uint) index3 < diagServiceIoCount; ++index3)
        {
          using (CaesarDiagServiceIO snapshotDiagServiceIo = dsio.GetErrorSnapshotDiagServiceIO(dtcIndex, index1, index2, index3))
          {
            if (snapshotDiagServiceIo != null)
            {
              if (this.faultCode.Channel.IsChannelErrorSet)
              {
                CaesarException e = new CaesarException(this.faultCode.Channel.ChannelHandle);
                if (e.ErrorNumber != 6058L)
                  Sapi.GetSapi().RaiseExceptionEvent((object) this.faultCode, (Exception) e);
              }
              CaesarDiagService diagService = snapshotDiagServiceIo.DiagService;
              if (diagService != null)
              {
                Service description = new Service(this.faultCode.Channel, ServiceTypes.None, diagService.Qualifier);
                description.Acquire(diagService);
                object presentation = description.OutputValues[0].GetPresentation(snapshotDiagServiceIo);
                this.environmentDatas.Add(new EnvironmentData(this, (IDiogenesDataItem) description, (int) snapshotRecordNumber, presentation));
              }
            }
          }
        }
      }
    }
  }

  internal void AcquireSnapshotFromRollCall(
    int recordNumber,
    IEnumerable<Tuple<Instrument, byte[]>> content)
  {
    this.UpdatePartFunction(ReadFunctions.Snapshot, false);
    foreach (Tuple<Instrument, byte[]> tuple in content)
    {
      try
      {
        this.environmentDatas.Add(new EnvironmentData(this, (IDiogenesDataItem) tuple.Item1, recordNumber, tuple.Item1.GetPresentation(tuple.Item2, 1, 1)));
      }
      catch (CaesarException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.faultCode.Channel.SourceAddress.Value, $"AcquireSnapshotFromRollCall: {ex.Message} while retrieving presentation for {tuple.Item1.Name}");
      }
    }
  }

  public FaultCode FaultCode => this.faultCode;

  public MilStatus Mil => this.mil;

  public StoredStatus Stored => this.stored;

  public ActiveStatus Active => this.active;

  public TestNotCompleteStatus TestNotComplete => this.testNotComplete;

  public TestFailedSinceLastClearStatus TestFailedSinceLastClear => this.testFailedSinceLastClear;

  public PendingStatus Pending => this.pending;

  public ImmediateStatus Immediate => this.immediate;

  public EnvironmentDataCollection EnvironmentDatas => this.environmentDatas;

  public DateTime StartTime => this.startTime;

  public DateTime EndTime => this.endTime;

  public ReadFunctions Functions => this.functions;

  public Choice Value
  {
    get
    {
      FaultCodeStatus rawValue = FaultCodeStatus.None;
      if (this.Active == ActiveStatus.Active)
        rawValue |= FaultCodeStatus.Active;
      if (this.Stored == StoredStatus.Stored)
        rawValue |= FaultCodeStatus.Stored;
      if (this.Pending == PendingStatus.Pending)
        rawValue |= FaultCodeStatus.Pending;
      if (this.Mil == MilStatus.On)
        rawValue |= FaultCodeStatus.Mil;
      if (this.TestFailedSinceLastClear == TestFailedSinceLastClearStatus.TestFailedSinceLastClear)
        rawValue |= FaultCodeStatus.TestFailedSinceLastClear;
      if (this.Immediate == ImmediateStatus.Immediate)
        rawValue |= FaultCodeStatus.Immediate;
      if ((this.Functions & ReadFunctions.Permanent) != ReadFunctions.None)
        rawValue |= FaultCodeStatus.Permanent;
      return this.FaultCode.Channel.CompleteFaultCodeStatusChoices.GetItemFromRawValue((object) rawValue);
    }
  }

  private string SnapshotContent
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.environmentDatas.Count; ++index)
      {
        EnvironmentData environmentData = this.environmentDatas[index];
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}\r\n", (object) environmentData.ToString());
      }
      return stringBuilder.ToString();
    }
  }

  internal bool IsEquivalent(FaultCodeIncident obj)
  {
    if (obj == null || this.Functions != obj.Functions)
      return false;
    if (this.Functions == ReadFunctions.Snapshot)
      return this.SnapshotContent.Equals(obj.SnapshotContent);
    return this.Mil == obj.Mil && this.Stored == obj.Stored && this.Active == obj.Active && this.Pending == obj.Pending && this.Immediate == obj.Immediate && this.TestFailedSinceLastClear == obj.TestFailedSinceLastClear && this.TestNotComplete == obj.TestNotComplete;
  }

  private void CheckOnBoardDiagnosticState()
  {
    if (this.FaultCode.Channel.Ecu.RollCallManager == null || this.Pending == PendingStatus.Undefined && this.Stored == StoredStatus.Undefined && this.Mil == MilStatus.Undefined && (this.functions & ReadFunctions.Permanent) == ReadFunctions.None || this.FaultCode.Channel.Ecu.Properties.ContainsKey("SupportsPendingFaults"))
      return;
    this.FaultCode.Channel.Ecu.Properties["SupportsPendingFaults"] = true.ToString();
    this.FaultCode.Channel.ResetFaultCodeStatusChoices();
    this.FaultCode.Channel.FaultCodes.RaiseFaultCodeUpdate((Exception) null, false);
  }

  internal bool IsStatusClarifiedBy(FaultCodeIncident newIncident)
  {
    if (this.Functions != newIncident.Functions || this.Active != newIncident.Active || this.Stored != StoredStatus.Undefined && this.Stored != newIncident.Stored || this.Pending != PendingStatus.Undefined && this.Pending != newIncident.Pending || this.Mil != MilStatus.Undefined && this.Mil != newIncident.Mil || this.Immediate != ImmediateStatus.Undefined && this.Immediate != newIncident.Immediate || this.TestFailedSinceLastClear != TestFailedSinceLastClearStatus.Undefined && this.TestFailedSinceLastClear != newIncident.TestFailedSinceLastClear)
      return false;
    if (this.Stored == StoredStatus.Undefined && newIncident.Stored == StoredStatus.NotStored || this.Pending == PendingStatus.Undefined && newIncident.Pending == PendingStatus.NotPending || this.Mil == MilStatus.Undefined && newIncident.Mil == MilStatus.Off || this.Immediate == ImmediateStatus.Undefined && newIncident.Immediate == ImmediateStatus.NotImmediate)
      return true;
    return this.TestFailedSinceLastClear == TestFailedSinceLastClearStatus.Undefined && newIncident.TestFailedSinceLastClear == TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear;
  }
}
