using System;

namespace Softing.Dts;

internal class DtsDbServiceImpl : MappedObject, DtsDbService, MCDDbService, MCDDbDiagService, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbDiagService, DtsDbDataPrimitive, DtsDbDiagComPrimitive, DtsDbObject, DtsNamedObject, DtsObject
{
	protected IntPtr m_dtsHandle = IntPtr.Zero;

	public IntPtr Handle
	{
		get
		{
			return m_dtsHandle;
		}
		set
		{
			m_dtsHandle = value;
		}
	}

	public MCDAddressingMode AddressingMode
	{
		get
		{
			GC.KeepAlive(this);
			MCDAddressingMode returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbService_getAddressingMode(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool HasSuppressPositiveResponseCapability
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbService_hasSuppressPositiveResponseCapability(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDRuntimeMode RuntimeMode
	{
		get
		{
			GC.KeepAlive(this);
			MCDRuntimeMode returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagService_getRuntimeMode(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDDbAccessLevel AccessLevel
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getAccessLevel(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAccessLevel;
		}
	}

	public MCDRepetitionMode RepetitionMode
	{
		get
		{
			GC.KeepAlive(this);
			MCDRepetitionMode returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getRepetitionMode(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDDbSpecialDataGroups DbSDGs
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getDbSDGs(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups;
		}
	}

	public uint RepetitionTime
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getRepetitionTime(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDAudience AudienceState
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getAudienceState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAudience;
		}
	}

	public MCDDbAdditionalAudiences DbDisabledAdditionalAudiences
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getDbDisabledAdditionalAudiences(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences;
		}
	}

	public MCDDbAdditionalAudiences DbEnabledAdditionalAudiences
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getDbEnabledAdditionalAudiences(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences;
		}
	}

	public MCDDbRequest DbRequest
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbRequest(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbRequest;
		}
	}

	public MCDDbResponses DbResponses
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbResponses(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponses;
		}
	}

	public MCDDbFunctionalClasses DbFunctionalClasses
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbFunctionalClasses(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFunctionalClasses;
		}
	}

	public string Semantic
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getSemantic(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public MCDTransmissionMode TransmissionMode
	{
		get
		{
			GC.KeepAlive(this);
			MCDTransmissionMode returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getTransmissionMode(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsApiExecutable
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_isApiExecutable(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsNoOperation
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_isNoOperation(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public DtsComPrimitiveType ComPrimitiveType
	{
		get
		{
			GC.KeepAlive(this);
			DtsComPrimitiveType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getComPrimitiveType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDValue ID
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
		}
	}

	public DtsDbProtocolParameters DbProtocolParameters
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbProtocolParameters(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProtocolParameters;
		}
	}

	public MCDValue DID
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
		}
	}

	public bool HasDID
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_hasDID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string InternalShortName
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getInternalShortName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string LongNameID
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbObject_getLongNameID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string DescriptionID
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbObject_getDescriptionID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string UniqueObjectIdentifier
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbObject_getUniqueObjectIdentifier(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string DatabaseFile
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsDbObject_getDatabaseFile(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string Description
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObject_getDescription(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string ShortName
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObject_getShortName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string LongName
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObject_getLongName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public uint StringID
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObject_getStringID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDObjectType ObjectType
	{
		get
		{
			GC.KeepAlive(this);
			MCDObjectType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsObject_getObjectType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint ObjectID
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsObject_getObjectID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public DtsDbServiceImpl(IntPtr handle)
	{
		Handle = handle;
		DTS_ObjectMapper.registerObject(Handle, this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (Handle != IntPtr.Zero)
		{
			DTS_ObjectMapper.unregisterObject(Handle);
			Handle = IntPtr.Zero;
		}
	}

	~DtsDbServiceImpl()
	{
		Dispose(disposing: false);
	}

	public MCDResult GenerateResult(byte[] pRequest, byte[] pResponse)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		ByteField_Struct _pRequest = new ByteField_Struct(pRequest);
		ByteField_Struct _pResponse = new ByteField_Struct(pResponse);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbService_generateResult(Handle, ref _pRequest, ref _pResponse, out returnValue);
		_pRequest.FreeMemory();
		_pResponse.FreeMemory();
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResult;
	}

	public MCDDbDataPrimitives GetRelatedDataPrimitives(string relationType)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDataPrimitive_getRelatedDataPrimitives(Handle, relationType, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataPrimitives;
	}

	public bool SupportsPDUInformation()
	{
		GC.KeepAlive(this);
		bool returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_supportsPDUInformation(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public MCDDbResponses GetDbResponsesByType(MCDResponseType type)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbResponsesByType(Handle, type, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponses;
	}

	public MCDDbEcuStateTransitions GetDbEcuStateTransitionsByDbObject(MCDDbEcuStateChart chart)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbEcuStateTransitionsByDbObject(Handle, DTS_ObjectMapper.getHandle(chart as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStateTransitions;
	}

	public MCDDbEcuStateTransitions GetDbEcuStateTransitionsBySemantic(string semantic)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbEcuStateTransitionsBySemantic(Handle, semantic, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStateTransitions;
	}

	public MCDDbEcuStates GetDbPreConditionStatesByDbObject(MCDDbEcuStateChart chart)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbPreConditionStatesByDbObject(Handle, DTS_ObjectMapper.getHandle(chart as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStates;
	}

	public MCDDbEcuStates GetDbPreConditionStatesBySemantic(string semantic)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbPreConditionStatesBySemantic(Handle, semantic, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStates;
	}
}
