using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

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

	private McdException lastException = null;

	private Dictionary<string, string> specialData;

	private bool disposedValue = false;

	public string Qualifier => qualifier;

	private IEnumerable<McdRequestParameter> RequestParameters
	{
		get
		{
			if (requestParameters == null)
			{
				requestParameters = (from p in primitive.Request.RequestParameters.OfType<MCDRequestParameter>()
					select new McdRequestParameter(p)).ToList();
			}
			return requestParameters;
		}
	}

	private IList<McdRequestParameter> AllRequestParameters
	{
		get
		{
			if (allRequestParameters == null)
			{
				allRequestParameters = McdRoot.FlattenStructures(RequestParameters, (McdRequestParameter p) => p.Parameters).ToList();
			}
			return allRequestParameters;
		}
	}

	public int PduMinimumLength => primitive.DbObject.DbRequest.PDUMinLength;

	public int PduActualDataStartPos
	{
		get
		{
			if (!pduDataStartPos.HasValue)
			{
				pduDataStartPos = (int)(from rp in primitive.DbObject.DbRequest.DbRequestParameters.OfType<MCDDbRequestParameter>()
					where rp.Semantic == "ID" || rp.Semantic == "SERVICE-ID"
					select rp).Max((MCDDbRequestParameter rp) => rp.BytePos + rp.ByteLength);
			}
			return pduDataStartPos.Value;
		}
	}

	public IEnumerable<byte> RequestMessage
	{
		get
		{
			return (objectType == MCDObjectType.eMCDSINGLEECUJOB) ? null : primitive.Request?.PDU?.Bytefield?.ToArray();
		}
		set
		{
			MCDValue mCDValue = Statics.createValue();
			mCDValue.DataType = MCDDataType.eA_BYTEFIELD;
			mCDValue.Bytefield = value.ToArray();
			try
			{
				primitive.Request.EnterPDU(mCDValue);
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "RequestMessage");
			}
		}
	}

	public IEnumerable<McdResponseParameter> PositiveResponseParameters => positiveResponseParameters;

	public IList<McdResponseParameter> AllPositiveResponseParameters => (positiveResponseParameters != null) ? McdRoot.FlattenStructures(PositiveResponseParameters, (McdResponseParameter p) => p.Parameters).ToList() : new List<McdResponseParameter>();

	public IEnumerable<byte> ResponseMessage
	{
		get
		{
			if (objectType != MCDObjectType.eMCDSINGLEECUJOB)
			{
				MCDResponse mCDResponse = result.Responses.OfType<MCDResponse>().FirstOrDefault();
				if (mCDResponse != null && ((DtsResponse)mCDResponse).HasResponseMessage)
				{
					return mCDResponse.ContainedResponseMessage.Bytefield;
				}
			}
			return null;
		}
	}

	public Dictionary<string, string> SpecialData
	{
		get
		{
			if (specialData == null)
			{
				specialData = McdDBDiagComPrimitive.GetSpecialData((primitive.DbObject as MCDDbService).DbSDGs);
			}
			return specialData;
		}
	}

	public bool IsNegativeResponse => negativeResponseParameter != null;

	public McdResponseParameter NegativeResponseParameter => negativeResponseParameter;

	internal MCDDiagComPrimitive Handle => primitive;

	internal McdDiagComPrimitive(McdLogicalLink link, MCDDiagComPrimitive primitive)
	{
		this.link = link;
		this.primitive = primitive;
		transmissionMode = this.primitive.DbObject.TransmissionMode;
		qualifier = this.primitive.DbObject.ShortName;
		objectType = this.primitive.ObjectType;
	}

	public bool SetInput(string name, object newValue)
	{
		McdRequestParameter mcdRequestParameter = AllRequestParameters.FirstOrDefault((McdRequestParameter rp) => rp.Qualifier == name && !rp.Parameters.Any());
		if (mcdRequestParameter != null)
		{
			mcdRequestParameter.SetValue(newValue);
			return true;
		}
		return false;
	}

	public void SetInput(int index, object newValue)
	{
		AllRequestParameters[index]?.SetValue(newValue);
	}

	public McdValue GetInput(string name)
	{
		return AllRequestParameters.FirstOrDefault((McdRequestParameter rp) => rp.Qualifier == name && !rp.Parameters.Any())?.Value;
	}

	public void ClearCache()
	{
		lastRequest = DateTime.MinValue;
		lastException = null;
		result = null;
		positiveResponseParameters = null;
		negativeResponseParameter = null;
	}

	public void Execute(int cacheTime)
	{
		Execute(cacheTime, preferResultsOverErrors: false);
	}

	public void Execute(int cacheTime, bool preferResultsOverErrors)
	{
		try
		{
			DateTime utcNow = DateTime.UtcNow;
			if (result == null || cacheTime == 0 || lastRequest + TimeSpan.FromMilliseconds(cacheTime) < utcNow)
			{
				positiveResponseParameters = null;
				negativeResponseParameter = null;
				lastException = null;
				if (objectType != MCDObjectType.eMCDSINGLEECUJOB && (transmissionMode == MCDTransmissionMode.eSEND || transmissionMode == MCDTransmissionMode.eSEND_AND_RECEIVE || transmissionMode == MCDTransmissionMode.eSEND_OR_RECEIVE))
				{
					McdRoot.RaiseByteMessageEvent(link, RequestMessage, send: true);
				}
				switch (objectType)
				{
				case MCDObjectType.eMCDSTARTCOMMUNICATION:
					link.TargetState = McdLogicalLinkState.Communication;
					break;
				case MCDObjectType.eMCDSTOPCOMMUNICATION:
					link.TargetState = McdLogicalLinkState.Online;
					break;
				}
				if (link.Handle.State == MCDLogicalLinkState.eCREATED)
				{
					link.Handle.Open();
					try
					{
						link.Handle.GotoOnline();
					}
					catch (MCDException ex)
					{
						link.Handle.Reset();
						lastRequest = DateTime.UtcNow;
						McdException ex2 = (link.ChannelError = new McdException(ex, "Execute"));
						throw lastException = ex2;
					}
				}
				result = primitive.ExecuteSync();
				lastRequest = DateTime.UtcNow;
				if (result.HasError && (!preferResultsOverErrors || result.Responses.Count == 0) && (result.ResultState.ExecutionState == MCDExecutionState.eALL_FAILED || result.ResultState.ExecutionState == MCDExecutionState.eCANCELED_DURING_EXECUTION))
				{
					lastException = new McdException(result.Error, "Execute");
					if ((link.IsEthernet && result.Error.Code == MCDErrorCodes.eRT_PDU_API_CALL_FAILED && result.Error.VendorCode == 257) || (result.Error.Code == MCDErrorCodes.eRT_PDU_API_CALL_FAILED && result.Error.VendorCode == 262) || result.Error.Code == MCDErrorCodes.eRT_NOT_ALLOWED_IN_LL_STATE_OFFLINE)
					{
						McdRoot.RaiseDebugInfoEvent(link, primitive.DbObject.ShortName + ": " + result.Error.VendorCodeDescription + ". Unrecoverable error; link will be reset.");
						link.Handle.Reset();
						link.ChannelError = lastException;
					}
					throw lastException;
				}
				MCDResponse mCDResponse = result.Responses.OfType<MCDResponse>().FirstOrDefault();
				if (mCDResponse == null)
				{
					return;
				}
				if (objectType != MCDObjectType.eMCDSINGLEECUJOB && (transmissionMode == MCDTransmissionMode.eRECEIVE || transmissionMode == MCDTransmissionMode.eSEND_AND_RECEIVE || transmissionMode == MCDTransmissionMode.eSEND_OR_RECEIVE))
				{
					McdRoot.RaiseByteMessageEvent(link, ResponseMessage, send: false);
				}
				if (mCDResponse.State == MCDResponseState.eACKNOWLEDGED)
				{
					positiveResponseParameters = (from p in mCDResponse.ResponseParameters.OfType<MCDResponseParameter>()
						select new McdResponseParameter(p)).ToList();
					negativeResponseParameter = null;
				}
				else
				{
					positiveResponseParameters = null;
					negativeResponseParameter = (from p in mCDResponse.ResponseParameters.OfType<MCDResponseParameter>()
						select new McdResponseParameter(p)).FirstOrDefault((McdResponseParameter p) => p.Qualifier == "NRC");
				}
				link.ChannelError = null;
			}
			else if (lastException != null)
			{
				throw lastException;
			}
		}
		catch (MCDException ex4)
		{
			throw new McdException(ex4, "Execute");
		}
		finally
		{
			if (objectType == MCDObjectType.eMCDSTOPCOMMUNICATION)
			{
				link.Handle.Reset();
				link.Handle.Open();
				link.Handle.GotoOnline();
				link.IdentifyVariant();
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			if (link != null)
			{
				link.RemoveDiagComPrimitive(this);
				link = null;
			}
			if (requestParameters != null)
			{
				foreach (McdRequestParameter requestParameter in requestParameters)
				{
					requestParameter.Dispose();
				}
				requestParameters = null;
			}
			if (positiveResponseParameters != null)
			{
				foreach (McdResponseParameter positiveResponseParameter in positiveResponseParameters)
				{
					positiveResponseParameter.Dispose();
				}
				positiveResponseParameters = null;
			}
			if (primitive != null)
			{
				primitive.Dispose();
				primitive = null;
			}
			if (negativeResponseParameter != null)
			{
				negativeResponseParameter.Dispose();
				negativeResponseParameter = null;
			}
			if (result != null)
			{
				result.Dispose();
				result = null;
			}
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
