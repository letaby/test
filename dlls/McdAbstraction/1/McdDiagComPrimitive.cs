// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDiagComPrimitive
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDiagComPrimitive : IDisposable
{
  private McdLogicalLink link;
  private string qualifier;
  private MCDDiagComPrimitive primitive;
  private MCDResult result;
  private List<McdResponseParameter> positiveResponseParameters;
  private McdResponseParameter negativeResponseParameter;
  private List<McdRequestParameter> requestParameters;
  private MCDTransmissionMode transmissionMode;
  private MCDObjectType objectType;
  private List<McdRequestParameter> allRequestParameters;
  private int? pduDataStartPos;
  private DateTime lastRequest = DateTime.MinValue;
  private McdException lastException = (McdException) null;
  private Dictionary<string, string> specialData;
  private bool disposedValue = false;

  internal McdDiagComPrimitive(McdLogicalLink link, MCDDiagComPrimitive primitive)
  {
    this.link = link;
    this.primitive = primitive;
    this.transmissionMode = this.primitive.DbObject.TransmissionMode;
    this.qualifier = this.primitive.DbObject.ShortName;
    this.objectType = this.primitive.ObjectType;
  }

  public string Qualifier => this.qualifier;

  private IEnumerable<McdRequestParameter> RequestParameters
  {
    get
    {
      if (this.requestParameters == null)
        this.requestParameters = this.primitive.Request.RequestParameters.OfType<MCDRequestParameter>().Select<MCDRequestParameter, McdRequestParameter>((Func<MCDRequestParameter, McdRequestParameter>) (p => new McdRequestParameter(p))).ToList<McdRequestParameter>();
      return (IEnumerable<McdRequestParameter>) this.requestParameters;
    }
  }

  private IList<McdRequestParameter> AllRequestParameters
  {
    get
    {
      if (this.allRequestParameters == null)
        this.allRequestParameters = McdRoot.FlattenStructures<McdRequestParameter>(this.RequestParameters, (Func<McdRequestParameter, IEnumerable<McdRequestParameter>>) (p => p.Parameters)).ToList<McdRequestParameter>();
      return (IList<McdRequestParameter>) this.allRequestParameters;
    }
  }

  public bool SetInput(string name, object newValue)
  {
    McdRequestParameter requestParameter = this.AllRequestParameters.FirstOrDefault<McdRequestParameter>((Func<McdRequestParameter, bool>) (rp => rp.Qualifier == name && !rp.Parameters.Any<McdRequestParameter>()));
    if (requestParameter == null)
      return false;
    requestParameter.SetValue(newValue);
    return true;
  }

  public void SetInput(int index, object newValue)
  {
    this.AllRequestParameters[index]?.SetValue(newValue);
  }

  public McdValue GetInput(string name)
  {
    return this.AllRequestParameters.FirstOrDefault<McdRequestParameter>((Func<McdRequestParameter, bool>) (rp => rp.Qualifier == name && !rp.Parameters.Any<McdRequestParameter>()))?.Value;
  }

  public int PduMinimumLength => (int) this.primitive.DbObject.DbRequest.PDUMinLength;

  public int PduActualDataStartPos
  {
    get
    {
      if (!this.pduDataStartPos.HasValue)
        this.pduDataStartPos = new int?((int) this.primitive.DbObject.DbRequest.DbRequestParameters.OfType<MCDDbRequestParameter>().Where<MCDDbRequestParameter>((Func<MCDDbRequestParameter, bool>) (rp => rp.Semantic == "ID" || rp.Semantic == "SERVICE-ID")).Max<MCDDbRequestParameter, uint>((Func<MCDDbRequestParameter, uint>) (rp => rp.BytePos + rp.ByteLength)));
      return this.pduDataStartPos.Value;
    }
  }

  public IEnumerable<byte> RequestMessage
  {
    get
    {
      byte[] requestMessage;
      if (this.objectType == MCDObjectType.eMCDSINGLEECUJOB)
      {
        requestMessage = (byte[]) null;
      }
      else
      {
        MCDRequest request = this.primitive.Request;
        if (request == null)
        {
          requestMessage = (byte[]) null;
        }
        else
        {
          MCDValue pdu = request.PDU;
          if (pdu == null)
          {
            requestMessage = (byte[]) null;
          }
          else
          {
            byte[] bytefield = pdu.Bytefield;
            requestMessage = bytefield != null ? ((IEnumerable<byte>) bytefield).ToArray<byte>() : (byte[]) null;
          }
        }
      }
      return (IEnumerable<byte>) requestMessage;
    }
    set
    {
      MCDValue pdu = Statics.createValue();
      pdu.DataType = MCDDataType.eA_BYTEFIELD;
      pdu.Bytefield = value.ToArray<byte>();
      try
      {
        this.primitive.Request.EnterPDU(pdu);
      }
      catch (MCDException ex)
      {
        throw new McdException(ex, nameof (RequestMessage));
      }
    }
  }

  public void ClearCache()
  {
    this.lastRequest = DateTime.MinValue;
    this.lastException = (McdException) null;
    this.result = (MCDResult) null;
    this.positiveResponseParameters = (List<McdResponseParameter>) null;
    this.negativeResponseParameter = (McdResponseParameter) null;
  }

  public void Execute(int cacheTime) => this.Execute(cacheTime, false);

  public void Execute(int cacheTime, bool preferResultsOverErrors)
  {
    try
    {
      DateTime utcNow = DateTime.UtcNow;
      if (this.result == null || cacheTime == 0 || this.lastRequest + TimeSpan.FromMilliseconds((double) cacheTime) < utcNow)
      {
        this.positiveResponseParameters = (List<McdResponseParameter>) null;
        this.negativeResponseParameter = (McdResponseParameter) null;
        this.lastException = (McdException) null;
        if (this.objectType != MCDObjectType.eMCDSINGLEECUJOB && (this.transmissionMode == MCDTransmissionMode.eSEND || this.transmissionMode == MCDTransmissionMode.eSEND_AND_RECEIVE || this.transmissionMode == MCDTransmissionMode.eSEND_OR_RECEIVE))
          McdRoot.RaiseByteMessageEvent(this.link, this.RequestMessage, true);
        switch (this.objectType)
        {
          case MCDObjectType.eMCDSTARTCOMMUNICATION:
            this.link.TargetState = McdLogicalLinkState.Communication;
            break;
          case MCDObjectType.eMCDSTOPCOMMUNICATION:
            this.link.TargetState = McdLogicalLinkState.Online;
            break;
        }
        if (this.link.Handle.State == MCDLogicalLinkState.eCREATED)
        {
          this.link.Handle.Open();
          try
          {
            this.link.Handle.GotoOnline();
          }
          catch (MCDException ex)
          {
            this.link.Handle.Reset();
            this.lastRequest = DateTime.UtcNow;
            throw this.lastException = this.link.ChannelError = new McdException(ex, nameof (Execute));
          }
        }
        this.result = this.primitive.ExecuteSync();
        this.lastRequest = DateTime.UtcNow;
        if (this.result.HasError && (!preferResultsOverErrors || this.result.Responses.Count == 0U) && (this.result.ResultState.ExecutionState == MCDExecutionState.eALL_FAILED || this.result.ResultState.ExecutionState == MCDExecutionState.eCANCELED_DURING_EXECUTION))
        {
          this.lastException = new McdException(this.result.Error, nameof (Execute));
          if (this.link.IsEthernet && this.result.Error.Code == MCDErrorCodes.eRT_PDU_API_CALL_FAILED && this.result.Error.VendorCode == (ushort) 257 || this.result.Error.Code == MCDErrorCodes.eRT_PDU_API_CALL_FAILED && this.result.Error.VendorCode == (ushort) 262 || this.result.Error.Code == MCDErrorCodes.eRT_NOT_ALLOWED_IN_LL_STATE_OFFLINE)
          {
            McdRoot.RaiseDebugInfoEvent(this.link, $"{this.primitive.DbObject.ShortName}: {this.result.Error.VendorCodeDescription}. Unrecoverable error; link will be reset.");
            this.link.Handle.Reset();
            this.link.ChannelError = this.lastException;
          }
          throw this.lastException;
        }
        MCDResponse mcdResponse = this.result.Responses.OfType<MCDResponse>().FirstOrDefault<MCDResponse>();
        if (mcdResponse == null)
          return;
        if (this.objectType != MCDObjectType.eMCDSINGLEECUJOB && (this.transmissionMode == MCDTransmissionMode.eRECEIVE || this.transmissionMode == MCDTransmissionMode.eSEND_AND_RECEIVE || this.transmissionMode == MCDTransmissionMode.eSEND_OR_RECEIVE))
          McdRoot.RaiseByteMessageEvent(this.link, this.ResponseMessage, false);
        if (mcdResponse.State == MCDResponseState.eACKNOWLEDGED)
        {
          this.positiveResponseParameters = mcdResponse.ResponseParameters.OfType<MCDResponseParameter>().Select<MCDResponseParameter, McdResponseParameter>((Func<MCDResponseParameter, McdResponseParameter>) (p => new McdResponseParameter(p))).ToList<McdResponseParameter>();
          this.negativeResponseParameter = (McdResponseParameter) null;
        }
        else
        {
          this.positiveResponseParameters = (List<McdResponseParameter>) null;
          this.negativeResponseParameter = mcdResponse.ResponseParameters.OfType<MCDResponseParameter>().Select<MCDResponseParameter, McdResponseParameter>((Func<MCDResponseParameter, McdResponseParameter>) (p => new McdResponseParameter(p))).FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.Qualifier == "NRC"));
        }
        this.link.ChannelError = (McdException) null;
      }
      else if (this.lastException != null)
        throw this.lastException;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (Execute));
    }
    finally
    {
      if (this.objectType == MCDObjectType.eMCDSTOPCOMMUNICATION)
      {
        this.link.Handle.Reset();
        this.link.Handle.Open();
        this.link.Handle.GotoOnline();
        this.link.IdentifyVariant();
      }
    }
  }

  public IEnumerable<McdResponseParameter> PositiveResponseParameters
  {
    get => (IEnumerable<McdResponseParameter>) this.positiveResponseParameters;
  }

  public IList<McdResponseParameter> AllPositiveResponseParameters
  {
    get
    {
      return this.positiveResponseParameters != null ? (IList<McdResponseParameter>) McdRoot.FlattenStructures<McdResponseParameter>(this.PositiveResponseParameters, (Func<McdResponseParameter, IEnumerable<McdResponseParameter>>) (p => p.Parameters)).ToList<McdResponseParameter>() : (IList<McdResponseParameter>) new List<McdResponseParameter>();
    }
  }

  public IEnumerable<byte> ResponseMessage
  {
    get
    {
      if (this.objectType != MCDObjectType.eMCDSINGLEECUJOB)
      {
        MCDResponse mcdResponse = this.result.Responses.OfType<MCDResponse>().FirstOrDefault<MCDResponse>();
        if (mcdResponse != null && ((DtsResponse) mcdResponse).HasResponseMessage)
          return (IEnumerable<byte>) mcdResponse.ContainedResponseMessage.Bytefield;
      }
      return (IEnumerable<byte>) null;
    }
  }

  public Dictionary<string, string> SpecialData
  {
    get
    {
      if (this.specialData == null)
        this.specialData = McdDBDiagComPrimitive.GetSpecialData((this.primitive.DbObject as MCDDbService).DbSDGs);
      return this.specialData;
    }
  }

  public bool IsNegativeResponse => this.negativeResponseParameter != null;

  public McdResponseParameter NegativeResponseParameter => this.negativeResponseParameter;

  internal MCDDiagComPrimitive Handle => this.primitive;

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      if (this.link != null)
      {
        this.link.RemoveDiagComPrimitive(this);
        this.link = (McdLogicalLink) null;
      }
      if (this.requestParameters != null)
      {
        foreach (McdRequestParameter requestParameter in this.requestParameters)
          requestParameter.Dispose();
        this.requestParameters = (List<McdRequestParameter>) null;
      }
      if (this.positiveResponseParameters != null)
      {
        foreach (McdResponseParameter responseParameter in this.positiveResponseParameters)
          responseParameter.Dispose();
        this.positiveResponseParameters = (List<McdResponseParameter>) null;
      }
      if (this.primitive != null)
      {
        this.primitive.Dispose();
        this.primitive = (MCDDiagComPrimitive) null;
      }
      if (this.negativeResponseParameter != null)
      {
        this.negativeResponseParameter.Dispose();
        this.negativeResponseParameter = (McdResponseParameter) null;
      }
      if (this.result != null)
      {
        this.result.Dispose();
        this.result = (MCDResult) null;
      }
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
