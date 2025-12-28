using System;

namespace Softing.Dts;

internal class DtsInterfaceLinkConfigImpl : MappedObject, DtsInterfaceLinkConfig, DtsObject, MCDObject, IDisposable
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

	public DtsPhysicalLinkOrInterfaceType LinkType
	{
		get
		{
			GC.KeepAlive(this);
			DtsPhysicalLinkOrInterfaceType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getLinkType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setLinkType(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public DtsPduApiLinkType PduApiLinkType
	{
		get
		{
			GC.KeepAlive(this);
			DtsPduApiLinkType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getPduApiLinkType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setPduApiLinkType(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public int GlobalIndex
	{
		get
		{
			GC.KeepAlive(this);
			int returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getGlobalIndex(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setGlobalIndex(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public int LocalIndex
	{
		get
		{
			GC.KeepAlive(this);
			int returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getLocalIndex(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setLocalIndex(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public uint PinCount
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getPinCount(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string String
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getString(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
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

	public DtsInterfaceLinkConfigImpl(IntPtr handle)
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

	~DtsInterfaceLinkConfigImpl()
	{
		Dispose(disposing: false);
	}

	public void Assign(DtsInterfaceLinkInformation linkInformation)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_assign(Handle, DTS_ObjectMapper.getHandle(linkInformation as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDConnectorPinType GetPinType(uint index)
	{
		GC.KeepAlive(this);
		MCDConnectorPinType returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getPinType(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public uint GetVehiclePin(uint index)
	{
		GC.KeepAlive(this);
		uint returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getVehiclePin(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}
}
