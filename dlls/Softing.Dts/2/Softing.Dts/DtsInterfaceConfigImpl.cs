using System;

namespace Softing.Dts;

internal class DtsInterfaceConfigImpl : MappedObject, DtsInterfaceConfig, DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
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

	public string ModuleType
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getModuleType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setModuleType(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string PDUAPIVersion
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getPDUAPIVersion(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setPDUAPIVersion(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool Enabled
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getEnabled(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setEnabled(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public DtsBusSystemInterfaceType BusSystemInterfaceType
	{
		get
		{
			GC.KeepAlive(this);
			DtsBusSystemInterfaceType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getBusSystemInterfaceType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setBusSystemInterfaceType(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public DtsInterfaceInformation InterfaceInformation
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getInterfaceInformation(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceInformation;
		}
	}

	public string SerialNumber
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getSerialNumber(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setSerialNumber(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string IpAddress
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getIpAddress(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setIpAddress(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string Cable
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getCable(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setCable(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public DtsInterfaceLinkInformations InterfaceLinkInformations
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getInterfaceLinkInformations(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceLinkInformations;
		}
	}

	public DtsInterfaceLinkConfigs InterfaceLinkConfigs
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getInterfaceLinkConfigs(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceLinkConfigs;
		}
	}

	public string VendorModuleName
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getVendorModuleName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setVendorModuleName(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool UseForLicensing
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getUseForLicensing(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_setUseForLicensing(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string DetectedSerialNumber
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getDetectedSerialNumber(Handle, out returnValue);
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
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObjectConfig_getShortName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObjectConfig_setShortName(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string LongName
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObjectConfig_getLongName(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObjectConfig_setLongName(Handle, value);
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
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObjectConfig_getDescription(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedObjectConfig_setDescription(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
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

	public DtsInterfaceConfigImpl(IntPtr handle)
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

	~DtsInterfaceConfigImpl()
	{
		Dispose(disposing: false);
	}

	public bool GetConnectedStatus(bool doDetection)
	{
		GC.KeepAlive(this);
		bool returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfig_getConnectedStatus(Handle, doDetection, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}
}
