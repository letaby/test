// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdLogicalLink
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdLogicalLink : IDisposable
{
  private MCDLogicalLink link;
  private MCDResult variantIdentResult;
  private McdDBLocation variantLocation;
  private Dictionary<string, McdDiagComPrimitive> primitives = new Dictionary<string, McdDiagComPrimitive>();
  private Dictionary<string, object> overridenComParameters = new Dictionary<string, object>();
  private McdDiagComPrimitive protocolParametersPrimitive;
  private McdDiagComPrimitive hexServicePrimitive;
  private Dictionary<string, McdConfigurationRecord> configRecords = new Dictionary<string, McdConfigurationRecord>();
  private bool isFixedVariant;
  private object logicalLinkOpenAbortLock = new object();
  private volatile bool aborting;
  private bool haveAddedDBConfigurationRecords;
  private bool disposedValue = false;

  internal McdLogicalLink(MCDLogicalLink link, bool isFixedVariant)
  {
    this.link = link;
    this.link.PrimitiveJobInfo += new OnPrimitiveJobInfo(this.Link_PrimitiveJobInfo);
    this.isFixedVariant = isFixedVariant;
  }

  public void Close()
  {
    foreach (McdDiagComPrimitive diagComPrimitive in this.primitives.Values.ToList<McdDiagComPrimitive>())
      diagComPrimitive?.Dispose();
    this.primitives.Clear();
    if (this.protocolParametersPrimitive != null)
    {
      this.protocolParametersPrimitive.Dispose();
      this.protocolParametersPrimitive = (McdDiagComPrimitive) null;
    }
    if (this.hexServicePrimitive != null)
    {
      this.hexServicePrimitive.Dispose();
      this.hexServicePrimitive = (McdDiagComPrimitive) null;
    }
    foreach (McdConfigurationRecord configurationRecord in this.configRecords.Values.ToList<McdConfigurationRecord>())
      configurationRecord?.Dispose();
    this.configRecords.Clear();
    if (this.link == null)
      return;
    lock (this.logicalLinkOpenAbortLock)
    {
      switch (this.link.State)
      {
        case MCDLogicalLinkState.eOFFLINE:
        case MCDLogicalLinkState.eONLINE:
        case MCDLogicalLinkState.eCOMMUNICATION:
          this.link.Close();
          break;
      }
    }
  }

  public void ClearCache()
  {
    foreach (McdDiagComPrimitive diagComPrimitive in this.primitives.Values)
      diagComPrimitive.ClearCache();
  }

  internal MCDLogicalLink Handle => this.link;

  public bool IsEthernet
  {
    get
    {
      return ((DtsDbPhysicalVehicleLinkOrInterface) this.link.InterfaceResource.DbPhysicalInterfaceLink).PILType == DtsPhysicalLinkOrInterfaceType.eETHERNET;
    }
  }

  public void OpenLogicalLink(bool executeStartCommunication)
  {
    this.TargetState = executeStartCommunication ? McdLogicalLinkState.Communication : McdLogicalLinkState.Online;
    try
    {
      lock (this.logicalLinkOpenAbortLock)
      {
        this.link.Open();
        this.protocolParametersPrimitive = new McdDiagComPrimitive(this, this.link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDPROTOCOLPARAMETERSET));
        foreach (KeyValuePair<string, object> overridenComParameter in this.overridenComParameters)
          this.protocolParametersPrimitive.SetInput(overridenComParameter.Key, overridenComParameter.Value);
        this.protocolParametersPrimitive.Execute(0);
        this.link.GotoOnline();
      }
      if (!this.isFixedVariant && !this.aborting)
      {
        MCDDiagComPrimitive comPrimitiveByType = this.link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION);
        MCDResult mcdResult = comPrimitiveByType.ExecuteSync();
        this.link.RemoveDiagComPrimitive(comPrimitiveByType);
        if (mcdResult.HasError && mcdResult.Error.Code == MCDErrorCodes.eRT_PDU_API_CALL_FAILED)
          throw new McdException(mcdResult.Error, "OpenLogicalLink-IdentifyVariant");
      }
      if (!executeStartCommunication || this.aborting)
        return;
      MCDDiagComPrimitive comPrimitiveByType1 = this.link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDSTARTCOMMUNICATION);
      MCDResult mcdResult1 = comPrimitiveByType1.ExecuteSync();
      this.link.RemoveDiagComPrimitive(comPrimitiveByType1);
      if (mcdResult1.HasError)
        throw new McdException(mcdResult1.Error, "OpenLogicalLink-StartComm");
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (OpenLogicalLink));
    }
  }

  public bool SuppressJobInfo { get; set; }

  private void Link_PrimitiveJobInfo(object sender, PrimitiveJobInfoArgs args)
  {
    if (this.SuppressJobInfo)
      return;
    McdRoot.RaiseDebugInfoEvent(this, $"{args.Primitive.DbObject.ShortName}: {args.Info}");
  }

  public void IdentifyVariant()
  {
    if (this.variantIdentResult != null)
    {
      this.variantIdentResult.Dispose();
      this.variantIdentResult = (MCDResult) null;
    }
    if (!this.isFixedVariant)
      this.variantLocation = (McdDBLocation) null;
    if (this.isFixedVariant && this.TargetState != McdLogicalLinkState.Communication)
      return;
    try
    {
      MCDDiagComPrimitive comPrimitiveByType = this.link.CreateDiagComPrimitiveByType(!this.isFixedVariant ? MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION : MCDObjectType.eMCDVARIANTIDENTIFICATION);
      this.variantIdentResult = comPrimitiveByType.ExecuteSync();
      McdValue identificationResult = this.GetVariantIdentificationResult("ActiveDiagnosticInformation_Read", "Session");
      if (identificationResult != null)
        this.SessionType = new int?((int) Convert.ToByte(identificationResult.CodedValue, (IFormatProvider) CultureInfo.InvariantCulture));
      this.link.RemoveDiagComPrimitive(comPrimitiveByType);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (IdentifyVariant));
    }
  }

  public McdValue GetVariantIdentificationResult(
    string primitiveQualifier,
    string parameterQualifier)
  {
    if (this.variantIdentResult != null)
    {
      foreach (MCDResponse response in (IEnumerable) this.variantIdentResult.Responses)
      {
        foreach (MCDResponseParameter responseParameter in (IEnumerable) response.ResponseParameters)
        {
          if (responseParameter.ShortName == primitiveQualifier)
          {
            foreach (MCDResponseParameter parameter in (IEnumerable) responseParameter.Parameters)
            {
              if (parameter.ShortName.IndexOf(parameterQualifier, StringComparison.OrdinalIgnoreCase) != -1)
              {
                MCDValue codedValue = parameter.DataType == MCDDataType.eA_BYTEFIELD || parameter.DataType == MCDDataType.eA_ASCIISTRING || parameter.DataType == MCDDataType.eA_UNICODE2STRING ? (MCDValue) null : parameter.CodedValue;
                return new McdValue(parameter.Value, codedValue);
              }
            }
          }
        }
      }
    }
    return (McdValue) null;
  }

  public McdLogicalLinkState State
  {
    get
    {
      return this.TargetState != McdLogicalLinkState.Online || this.link.State != MCDLogicalLinkState.eCREATED ? (McdLogicalLinkState) this.link.State : this.TargetState;
    }
  }

  public bool ChannelRunning
  {
    get => this.State == this.TargetState && this.QueueState != McdActivityState.Suspended;
  }

  public void ResetChannelError() => this.ChannelError = (McdException) null;

  public McdException ChannelError { internal set; get; }

  public McdLogicalLinkState TargetState { get; internal set; }

  public McdActivityState QueueState => (McdActivityState) this.link.QueueState;

  public int? SessionType { get; private set; }

  public string VariantName
  {
    get
    {
      return this.link.DbObject.DbLocation.Type == MCDLocationType.eECU_VARIANT ? this.link.DbObject.DbLocation.ShortName : (string) null;
    }
  }

  public McdDBLocation DBLocation
  {
    get
    {
      if (this.variantLocation == null)
        this.variantLocation = new McdDBLocation(this.link.DbObject.DbLocation);
      return this.variantLocation;
    }
  }

  public McdDiagComPrimitive GetService(string qualifier)
  {
    try
    {
      McdDiagComPrimitive service = (McdDiagComPrimitive) null;
      if (!this.primitives.TryGetValue(qualifier, out service))
      {
        service = new McdDiagComPrimitive(this, this.link.CreateDiagComPrimitiveByName(qualifier));
        this.primitives[qualifier] = service;
      }
      return service;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (GetService));
    }
  }

  public McdFlashJob CreateFlashJobForKey(string flashKey)
  {
    try
    {
      MCDDbFlashSession sessionByFlashKey = this.link.DbObject.DbLocation.DbFlashSessions.GetDbFlashSessionByFlashKey(flashKey);
      MCDFlashJob primitiveByDbObject = (MCDFlashJob) this.link.CreateDiagComPrimitiveByDbObject((MCDDbDiagComPrimitive) sessionByFlashKey.GetDbFlashJobByLocation(this.link.DbObject.DbLocation));
      primitiveByDbObject.Session = sessionByFlashKey;
      return new McdFlashJob(this.link, primitiveByDbObject);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (CreateFlashJobForKey));
    }
  }

  public McdDiagComPrimitive GetHexService()
  {
    try
    {
      if (this.hexServicePrimitive == null)
        this.hexServicePrimitive = new McdDiagComPrimitive(this, this.link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDHEXSERVICE));
      return this.hexServicePrimitive;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "CreateHexService");
    }
  }

  internal void RemoveDiagComPrimitive(McdDiagComPrimitive primitive)
  {
    try
    {
      this.link.RemoveDiagComPrimitive(primitive.Handle);
      if (!this.primitives.Values.Contains<McdDiagComPrimitive>(primitive))
        return;
      this.primitives[primitive.Qualifier] = (McdDiagComPrimitive) null;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (RemoveDiagComPrimitive));
    }
  }

  internal void RemoveConfigRecord(McdConfigurationRecord configRecord)
  {
    try
    {
      this.link.ConfigurationRecords.Remove(configRecord.Handle);
      if (!this.configRecords.Values.Contains<McdConfigurationRecord>(configRecord))
        return;
      this.configRecords[configRecord.Qualifier] = (McdConfigurationRecord) null;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "RemoveDiagComPrimitive");
    }
  }

  public void SetComParameter(string name, object value)
  {
    this.overridenComParameters[name] = value;
    if (this.protocolParametersPrimitive == null)
      return;
    this.protocolParametersPrimitive.SetInput(name, value);
    try
    {
      this.protocolParametersPrimitive.Execute(0);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (SetComParameter));
    }
  }

  public void Abort()
  {
    if (this.aborting)
      return;
    this.aborting = true;
    lock (this.logicalLinkOpenAbortLock)
    {
      if (this.link.State == MCDLogicalLinkState.eONLINE || this.link.State == MCDLogicalLinkState.eCOMMUNICATION)
      {
        if (this.link.QueueState != MCDActivityState.eACTIVITY_SUSPENDED)
        {
          try
          {
            this.link.Suspend();
            this.link.ClearQueue();
          }
          catch (DtsProgramViolationException ex)
          {
            McdRoot.RaiseDebugInfoEvent(this, "Unable to abort logical link. " + ex.Message);
          }
        }
      }
    }
  }

  public object GetComParameter(string name)
  {
    object comParameter;
    if (this.overridenComParameters.TryGetValue(name, out comParameter))
      return comParameter;
    if (this.protocolParametersPrimitive != null)
    {
      McdValue input = this.protocolParametersPrimitive.GetInput(name);
      if (input != null)
        return input.Value;
    }
    return (object) null;
  }

  private void AddDBConfigurationRecords()
  {
    foreach (string configurationDataName in McdRoot.DBConfigurationDataNames)
    {
      McdDBConfigurationData configurationData = McdRoot.GetDBConfigurationData(configurationDataName);
      if (configurationData != null && configurationData.EcuNames.Contains<string>(this.link.DbObject.DbLocation.AccessKey.EcuBaseVariant))
      {
        foreach (McdDBConfigurationRecord configurationRecord in configurationData.DBConfigurationRecords)
        {
          try
          {
            MCDConfigurationRecord record = this.link.ConfigurationRecords.AddByDbObject(configurationRecord.Handle);
            this.configRecords.Add(record.DbObject.ShortName, new McdConfigurationRecord(this, record, configurationRecord));
          }
          catch (MCDException ex)
          {
            McdRoot.RaiseDebugInfoEvent(this, $"Unable to add configuration record {configurationRecord.Qualifier} because {ex.Message}");
          }
          catch (McdException ex)
          {
            McdRoot.RaiseDebugInfoEvent(this, $"Unable to add configuration record {configurationRecord.Qualifier} because {ex.Message}");
          }
        }
      }
    }
    this.haveAddedDBConfigurationRecords = true;
  }

  public McdConfigurationRecord SetDefaultStringByPartNumber(string partNumber)
  {
    if (!this.haveAddedDBConfigurationRecords)
      this.AddDBConfigurationRecords();
    foreach (McdConfigurationRecord configurationRecord in this.configRecords.Values)
    {
      McdDBDataRecord defaultString = configurationRecord.GetDefaultString(partNumber);
      if (defaultString != null)
      {
        configurationRecord.SetConfigurationRecordByDBObject(defaultString);
        return configurationRecord;
      }
    }
    return (McdConfigurationRecord) null;
  }

  public McdConfigurationRecord SetFragmentMeaningByPartNumber(string partNumber)
  {
    if (!this.haveAddedDBConfigurationRecords)
      this.AddDBConfigurationRecords();
    foreach (McdConfigurationRecord configurationRecord in this.configRecords.Values)
    {
      Tuple<McdOptionItem, McdDBItemValue> fragment = configurationRecord.GetFragment(partNumber);
      if (fragment != null)
      {
        fragment.Item1.SetItemValueByDBObject(fragment.Item2);
        return configurationRecord;
      }
    }
    return (McdConfigurationRecord) null;
  }

  public McdDiagComPrimitive UpdateDiagCodingString(McdConfigurationRecord configRecord)
  {
    byte[] array1 = configRecord.ConfigurationRecord.ToArray<byte>();
    McdDiagComPrimitive diagComPrimitive = (McdDiagComPrimitive) null;
    if (this.DBLocation.DBServices.Any<McdDBService>((Func<McdDBService, bool>) (s => s.Qualifier == configRecord.DBRecord.Qualifier)))
      diagComPrimitive = this.GetService(configRecord.DBRecord.Qualifier);
    else if (configRecord.Handle.WriteDiagComPrimitives.Count > 0U)
    {
      byte[] pdu = configRecord.Handle.WriteDiagComPrimitives.GetItemByIndex(0U).Request.PDU.Bytefield;
      pdu = ((IEnumerable<byte>) pdu).Take<byte>(pdu.Length - array1.Length).ToArray<byte>();
      McdDBService mcdDbService = this.DBLocation.VariantCodingWriteDBServices.FirstOrDefault<McdDBService>((Func<McdDBService, bool>) (s => s.RequestMessage != null && ((IEnumerable<byte>) pdu).SequenceEqual<byte>(s.RequestMessage.Take<byte>(pdu.Length))));
      if (mcdDbService != null)
        diagComPrimitive = this.GetService(mcdDbService.Qualifier);
    }
    if (diagComPrimitive != null)
    {
      byte[] array2 = diagComPrimitive.RequestMessage.ToArray<byte>();
      diagComPrimitive.RequestMessage = ((IEnumerable<byte>) array2).Take<byte>(diagComPrimitive.PduMinimumLength - array1.Length).Concat<byte>((IEnumerable<byte>) array1);
    }
    return diagComPrimitive;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      if (this.variantIdentResult != null)
      {
        this.variantIdentResult.Dispose();
        this.variantIdentResult = (MCDResult) null;
      }
      if (this.link != null)
      {
        this.Close();
        McdRoot.RemoveLogicalLink(this);
        this.link.PrimitiveJobInfo -= new OnPrimitiveJobInfo(this.Link_PrimitiveJobInfo);
        this.link.Dispose();
        this.link = (MCDLogicalLink) null;
      }
      this.variantLocation = (McdDBLocation) null;
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
