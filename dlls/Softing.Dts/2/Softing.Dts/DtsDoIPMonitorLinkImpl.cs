using System;

namespace Softing.Dts;

internal class DtsDoIPMonitorLinkImpl : MappedObject, DtsDoIPMonitorLink, DtsMonitoringLink, MCDMonitoringLink, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
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

	public MCDInterfaceResource InterfaceResource
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_getInterfaceResource(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceResource;
		}
	}

	public MCDMessageFilters MessageFilters
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_getMessageFilters(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMessageFilters;
		}
	}

	public ushort NoOfSampleToFireEvent
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_setNoOfSampleToFireEvent(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public ushort TimeToFireEvent
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_setTimeToFireEvent(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
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

	public DtsDoIPMonitorLinkImpl(IntPtr handle)
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

	~DtsDoIPMonitorLinkImpl()
	{
		Dispose(disposing: false);
	}

	public string[] FetchMonitoringFrames(uint numReq)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_fetchMonitoringFrames(Handle, numReq, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void Start()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_start(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Stop()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_stop(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StartFileTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_startFileTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StopFileTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_stopFileTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void OpenFileTrace(string FileName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_openFileTrace(Handle, FileName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void CloseFileTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_closeFileTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public byte[] FetchDtsMonitorFrames(uint numReq)
	{
		GC.KeepAlive(this);
		ByteField_Struct returnValue = default(ByteField_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMonitoringLink_fetchDtsMonitorFrames(Handle, numReq, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToByteArray();
	}
}
