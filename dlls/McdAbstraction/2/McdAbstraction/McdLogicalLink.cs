using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Softing.Dts;

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

	internal MCDLogicalLink Handle => link;

	public bool IsEthernet => ((DtsDbPhysicalVehicleLinkOrInterface)link.InterfaceResource.DbPhysicalInterfaceLink).PILType == DtsPhysicalLinkOrInterfaceType.eETHERNET;

	public bool SuppressJobInfo { get; set; }

	public McdLogicalLinkState State => (TargetState == McdLogicalLinkState.Online && link.State == MCDLogicalLinkState.eCREATED) ? TargetState : ((McdLogicalLinkState)link.State);

	public bool ChannelRunning => State == TargetState && QueueState != McdActivityState.Suspended;

	public McdException ChannelError { get; internal set; }

	public McdLogicalLinkState TargetState { get; internal set; }

	public McdActivityState QueueState => (McdActivityState)link.QueueState;

	public int? SessionType { get; private set; }

	public string VariantName
	{
		get
		{
			if (link.DbObject.DbLocation.Type == MCDLocationType.eECU_VARIANT)
			{
				return link.DbObject.DbLocation.ShortName;
			}
			return null;
		}
	}

	public McdDBLocation DBLocation
	{
		get
		{
			if (variantLocation == null)
			{
				variantLocation = new McdDBLocation(link.DbObject.DbLocation);
			}
			return variantLocation;
		}
	}

	internal McdLogicalLink(MCDLogicalLink link, bool isFixedVariant)
	{
		this.link = link;
		this.link.PrimitiveJobInfo += Link_PrimitiveJobInfo;
		this.isFixedVariant = isFixedVariant;
	}

	public void Close()
	{
		foreach (McdDiagComPrimitive item in primitives.Values.ToList())
		{
			item?.Dispose();
		}
		primitives.Clear();
		if (protocolParametersPrimitive != null)
		{
			protocolParametersPrimitive.Dispose();
			protocolParametersPrimitive = null;
		}
		if (hexServicePrimitive != null)
		{
			hexServicePrimitive.Dispose();
			hexServicePrimitive = null;
		}
		foreach (McdConfigurationRecord item2 in configRecords.Values.ToList())
		{
			item2?.Dispose();
		}
		configRecords.Clear();
		if (link == null)
		{
			return;
		}
		lock (logicalLinkOpenAbortLock)
		{
			MCDLogicalLinkState state = link.State;
			if (state - 770 <= (MCDLogicalLinkState)2u)
			{
				link.Close();
			}
		}
	}

	public void ClearCache()
	{
		foreach (McdDiagComPrimitive value in primitives.Values)
		{
			value.ClearCache();
		}
	}

	public void OpenLogicalLink(bool executeStartCommunication)
	{
		TargetState = (executeStartCommunication ? McdLogicalLinkState.Communication : McdLogicalLinkState.Online);
		try
		{
			lock (logicalLinkOpenAbortLock)
			{
				link.Open();
				protocolParametersPrimitive = new McdDiagComPrimitive(this, link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDPROTOCOLPARAMETERSET));
				foreach (KeyValuePair<string, object> overridenComParameter in overridenComParameters)
				{
					protocolParametersPrimitive.SetInput(overridenComParameter.Key, overridenComParameter.Value);
				}
				protocolParametersPrimitive.Execute(0);
				link.GotoOnline();
			}
			if (!isFixedVariant && !aborting)
			{
				MCDDiagComPrimitive mCDDiagComPrimitive = link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION);
				MCDResult mCDResult = mCDDiagComPrimitive.ExecuteSync();
				link.RemoveDiagComPrimitive(mCDDiagComPrimitive);
				if (mCDResult.HasError && mCDResult.Error.Code == MCDErrorCodes.eRT_PDU_API_CALL_FAILED)
				{
					throw new McdException(mCDResult.Error, "OpenLogicalLink-IdentifyVariant");
				}
			}
			if (executeStartCommunication && !aborting)
			{
				MCDDiagComPrimitive mCDDiagComPrimitive2 = link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDSTARTCOMMUNICATION);
				MCDResult mCDResult2 = mCDDiagComPrimitive2.ExecuteSync();
				link.RemoveDiagComPrimitive(mCDDiagComPrimitive2);
				if (mCDResult2.HasError)
				{
					throw new McdException(mCDResult2.Error, "OpenLogicalLink-StartComm");
				}
			}
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "OpenLogicalLink");
		}
	}

	private void Link_PrimitiveJobInfo(object sender, PrimitiveJobInfoArgs args)
	{
		if (!SuppressJobInfo)
		{
			McdRoot.RaiseDebugInfoEvent(this, args.Primitive.DbObject.ShortName + ": " + args.Info);
		}
	}

	public void IdentifyVariant()
	{
		if (variantIdentResult != null)
		{
			variantIdentResult.Dispose();
			variantIdentResult = null;
		}
		if (!isFixedVariant)
		{
			variantLocation = null;
		}
		if (isFixedVariant && TargetState != McdLogicalLinkState.Communication)
		{
			return;
		}
		try
		{
			MCDDiagComPrimitive mCDDiagComPrimitive = link.CreateDiagComPrimitiveByType((!isFixedVariant) ? MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION : MCDObjectType.eMCDVARIANTIDENTIFICATION);
			variantIdentResult = mCDDiagComPrimitive.ExecuteSync();
			McdValue variantIdentificationResult = GetVariantIdentificationResult("ActiveDiagnosticInformation_Read", "Session");
			if (variantIdentificationResult != null)
			{
				SessionType = Convert.ToByte(variantIdentificationResult.CodedValue, CultureInfo.InvariantCulture);
			}
			link.RemoveDiagComPrimitive(mCDDiagComPrimitive);
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "IdentifyVariant");
		}
	}

	public McdValue GetVariantIdentificationResult(string primitiveQualifier, string parameterQualifier)
	{
		if (variantIdentResult != null)
		{
			foreach (MCDResponse response in variantIdentResult.Responses)
			{
				foreach (MCDResponseParameter responseParameter in response.ResponseParameters)
				{
					if (!(responseParameter.ShortName == primitiveQualifier))
					{
						continue;
					}
					foreach (MCDResponseParameter parameter in responseParameter.Parameters)
					{
						if (parameter.ShortName.IndexOf(parameterQualifier, StringComparison.OrdinalIgnoreCase) != -1)
						{
							MCDValue mcdCodedValue = ((parameter.DataType != MCDDataType.eA_BYTEFIELD && parameter.DataType != MCDDataType.eA_ASCIISTRING && parameter.DataType != MCDDataType.eA_UNICODE2STRING) ? parameter.CodedValue : null);
							return new McdValue(parameter.Value, mcdCodedValue);
						}
					}
				}
			}
		}
		return null;
	}

	public void ResetChannelError()
	{
		ChannelError = null;
	}

	public McdDiagComPrimitive GetService(string qualifier)
	{
		try
		{
			McdDiagComPrimitive value = null;
			if (!primitives.TryGetValue(qualifier, out value))
			{
				MCDDiagComPrimitive primitive = link.CreateDiagComPrimitiveByName(qualifier);
				value = new McdDiagComPrimitive(this, primitive);
				primitives[qualifier] = value;
			}
			return value;
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "GetService");
		}
	}

	public McdFlashJob CreateFlashJobForKey(string flashKey)
	{
		try
		{
			MCDDbFlashSession dbFlashSessionByFlashKey = link.DbObject.DbLocation.DbFlashSessions.GetDbFlashSessionByFlashKey(flashKey);
			MCDDbFlashJob dbFlashJobByLocation = dbFlashSessionByFlashKey.GetDbFlashJobByLocation(link.DbObject.DbLocation);
			MCDDiagComPrimitive mCDDiagComPrimitive = link.CreateDiagComPrimitiveByDbObject(dbFlashJobByLocation);
			MCDFlashJob mCDFlashJob = (MCDFlashJob)mCDDiagComPrimitive;
			mCDFlashJob.Session = dbFlashSessionByFlashKey;
			return new McdFlashJob(link, mCDFlashJob);
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "CreateFlashJobForKey");
		}
	}

	public McdDiagComPrimitive GetHexService()
	{
		try
		{
			if (hexServicePrimitive == null)
			{
				hexServicePrimitive = new McdDiagComPrimitive(this, link.CreateDiagComPrimitiveByType(MCDObjectType.eMCDHEXSERVICE));
			}
			return hexServicePrimitive;
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
			link.RemoveDiagComPrimitive(primitive.Handle);
			if (primitives.Values.Contains(primitive))
			{
				primitives[primitive.Qualifier] = null;
			}
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "RemoveDiagComPrimitive");
		}
	}

	internal void RemoveConfigRecord(McdConfigurationRecord configRecord)
	{
		try
		{
			link.ConfigurationRecords.Remove(configRecord.Handle);
			if (configRecords.Values.Contains(configRecord))
			{
				configRecords[configRecord.Qualifier] = null;
			}
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "RemoveDiagComPrimitive");
		}
	}

	public void SetComParameter(string name, object value)
	{
		overridenComParameters[name] = value;
		if (protocolParametersPrimitive != null)
		{
			protocolParametersPrimitive.SetInput(name, value);
			try
			{
				protocolParametersPrimitive.Execute(0);
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "SetComParameter");
			}
		}
	}

	public void Abort()
	{
		if (aborting)
		{
			return;
		}
		aborting = true;
		lock (logicalLinkOpenAbortLock)
		{
			if ((link.State == MCDLogicalLinkState.eONLINE || link.State == MCDLogicalLinkState.eCOMMUNICATION) && link.QueueState != MCDActivityState.eACTIVITY_SUSPENDED)
			{
				try
				{
					link.Suspend();
					link.ClearQueue();
					return;
				}
				catch (DtsProgramViolationException ex)
				{
					McdRoot.RaiseDebugInfoEvent(this, "Unable to abort logical link. " + ex.Message);
					return;
				}
			}
		}
	}

	public object GetComParameter(string name)
	{
		if (overridenComParameters.TryGetValue(name, out var value))
		{
			return value;
		}
		if (protocolParametersPrimitive != null)
		{
			McdValue input = protocolParametersPrimitive.GetInput(name);
			if (input != null)
			{
				return input.Value;
			}
		}
		return null;
	}

	private void AddDBConfigurationRecords()
	{
		foreach (string dBConfigurationDataName in McdRoot.DBConfigurationDataNames)
		{
			McdDBConfigurationData dBConfigurationData = McdRoot.GetDBConfigurationData(dBConfigurationDataName);
			if (dBConfigurationData == null || !dBConfigurationData.EcuNames.Contains(link.DbObject.DbLocation.AccessKey.EcuBaseVariant))
			{
				continue;
			}
			foreach (McdDBConfigurationRecord dBConfigurationRecord in dBConfigurationData.DBConfigurationRecords)
			{
				try
				{
					MCDConfigurationRecord mCDConfigurationRecord = link.ConfigurationRecords.AddByDbObject(dBConfigurationRecord.Handle);
					configRecords.Add(mCDConfigurationRecord.DbObject.ShortName, new McdConfigurationRecord(this, mCDConfigurationRecord, dBConfigurationRecord));
				}
				catch (MCDException ex)
				{
					McdRoot.RaiseDebugInfoEvent(this, "Unable to add configuration record " + dBConfigurationRecord.Qualifier + " because " + ex.Message);
				}
				catch (McdException ex2)
				{
					McdRoot.RaiseDebugInfoEvent(this, "Unable to add configuration record " + dBConfigurationRecord.Qualifier + " because " + ex2.Message);
				}
			}
		}
		haveAddedDBConfigurationRecords = true;
	}

	public McdConfigurationRecord SetDefaultStringByPartNumber(string partNumber)
	{
		if (!haveAddedDBConfigurationRecords)
		{
			AddDBConfigurationRecords();
		}
		foreach (McdConfigurationRecord value in configRecords.Values)
		{
			McdDBDataRecord defaultString = value.GetDefaultString(partNumber);
			if (defaultString != null)
			{
				value.SetConfigurationRecordByDBObject(defaultString);
				return value;
			}
		}
		return null;
	}

	public McdConfigurationRecord SetFragmentMeaningByPartNumber(string partNumber)
	{
		if (!haveAddedDBConfigurationRecords)
		{
			AddDBConfigurationRecords();
		}
		foreach (McdConfigurationRecord value in configRecords.Values)
		{
			Tuple<McdOptionItem, McdDBItemValue> fragment = value.GetFragment(partNumber);
			if (fragment != null)
			{
				fragment.Item1.SetItemValueByDBObject(fragment.Item2);
				return value;
			}
		}
		return null;
	}

	public McdDiagComPrimitive UpdateDiagCodingString(McdConfigurationRecord configRecord)
	{
		byte[] array = configRecord.ConfigurationRecord.ToArray();
		McdDiagComPrimitive mcdDiagComPrimitive = null;
		if (DBLocation.DBServices.Any((McdDBService s) => s.Qualifier == configRecord.DBRecord.Qualifier))
		{
			mcdDiagComPrimitive = GetService(configRecord.DBRecord.Qualifier);
		}
		else if (configRecord.Handle.WriteDiagComPrimitives.Count != 0)
		{
			MCDDiagComPrimitive itemByIndex = configRecord.Handle.WriteDiagComPrimitives.GetItemByIndex(0u);
			byte[] pdu = itemByIndex.Request.PDU.Bytefield;
			pdu = pdu.Take(pdu.Length - array.Length).ToArray();
			McdDBService mcdDBService = DBLocation.VariantCodingWriteDBServices.FirstOrDefault((McdDBService s) => s.RequestMessage != null && pdu.SequenceEqual(s.RequestMessage.Take(pdu.Length)));
			if (mcdDBService != null)
			{
				mcdDiagComPrimitive = GetService(mcdDBService.Qualifier);
			}
		}
		if (mcdDiagComPrimitive != null)
		{
			byte[] source = mcdDiagComPrimitive.RequestMessage.ToArray();
			mcdDiagComPrimitive.RequestMessage = source.Take(mcdDiagComPrimitive.PduMinimumLength - array.Length).Concat(array);
		}
		return mcdDiagComPrimitive;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			if (variantIdentResult != null)
			{
				variantIdentResult.Dispose();
				variantIdentResult = null;
			}
			if (link != null)
			{
				Close();
				McdRoot.RemoveLogicalLink(this);
				link.PrimitiveJobInfo -= Link_PrimitiveJobInfo;
				link.Dispose();
				link = null;
			}
			variantLocation = null;
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
