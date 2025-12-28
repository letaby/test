using System;

namespace Softing.Dts;

internal class DtsProjectImpl : MappedObject, DtsProject, MCDProject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
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

	public MCDDbProject DbProject
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getDbProject(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProject;
		}
	}

	public MCDDbVehicleInformation ActiveDbVehicleInformation
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getActiveDbVehicleInformation(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbVehicleInformation;
		}
	}

	public string[] TraceFilterNames
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getTraceFilterNames(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public int CustomerVersion
	{
		get
		{
			GC.KeepAlive(this);
			int returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getCustomerVersion(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string[] IOControlNames
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getIOControlNames(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public DtsGlobalProtocolParameterSets GlobalProtocolParameterSets
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getGlobalProtocolParameterSets(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsGlobalProtocolParameterSets;
		}
	}

	public DtsFileLocations DatabaseFileList
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getDatabaseFileList(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFileLocations;
		}
	}

	public string ActiveSimFile
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getActiveSimFile(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_setActiveSimFile(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string[] SimFiles
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getSimFiles(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public uint Characteristic
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getCharacteristic(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string[] AglFiles
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getAglFiles(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public string ProjectUid
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getProjectUid(Handle, out returnValue);
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

	public DtsProjectImpl(IntPtr handle)
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

	~DtsProjectImpl()
	{
		Dispose(disposing: false);
	}

	public MCDDbVehicleInformation SelectDbVehicleInformationByName(string shortName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_selectDbVehicleInformationByName(Handle, shortName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbVehicleInformation;
	}

	public void SelectDbVehicleInformation(MCDDbVehicleInformation vehicleInformation)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_selectDbVehicleInformation(Handle, DTS_ObjectMapper.getHandle(vehicleInformation as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDClampState GetClampState(string clampName)
	{
		GC.KeepAlive(this);
		MCDClampState returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_getClampState(Handle, clampName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public void DeselectVehicleInformation()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_deselectVehicleInformation(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDLogicalLink CreateLogicalLink(MCDDbLogicalLink LogicalLink)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLink(Handle, DTS_ObjectMapper.getHandle(LogicalLink as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByAccessKey(string AccessKeyString, string PhysicalVehicleLinkString)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByAccessKey(Handle, AccessKeyString, PhysicalVehicleLinkString, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByName(string LogicalLinkName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByName(Handle, LogicalLinkName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByVariant(string BaseVariantLogicalLinkName, string VariantName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByVariant(Handle, BaseVariantLogicalLinkName, VariantName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public void RemoveLogicalLink(MCDLogicalLink LogicalLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_removeLogicalLink(Handle, DTS_ObjectMapper.getHandle(LogicalLink as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public DtsLogicalLinkMonitor CreateDtsLogicalLinkMonitor(string PilShortName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createDtsLogicalLinkMonitor(Handle, PilShortName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLinkMonitor;
	}

	public string[] ListAddonFiles(string strDirectory, string strBaseVariant, string strVariant, string strIdents, bool bReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_listAddonFiles(Handle, strDirectory, strBaseVariant, strVariant, strIdents, bReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void UnlinkDatabaseFiles()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_unlinkDatabaseFiles(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void ReplaceProjectFlashFile(string strOldFile, string strNewFile)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_replaceProjectFlashFile(Handle, strOldFile, strNewFile);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDValue CreateValue(MCDDataType dataType)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createValue(Handle, dataType, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
	}

	public MCDLogicalLink CreateLogicalLinkByAccessKeyAndInterface(string accessKeyString, string physicalVehicleLinkString, MCDInterface interface_)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByAccessKeyAndInterface(Handle, accessKeyString, physicalVehicleLinkString, DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByAccessKeyAndInterfaceResource(string accessKeyString, string physicalVehicleLinkString, MCDInterfaceResource interfaceResource)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByAccessKeyAndInterfaceResource(Handle, accessKeyString, physicalVehicleLinkString, DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByInterface(MCDDbLogicalLink logicalLink, MCDInterface interface_)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByInterface(Handle, DTS_ObjectMapper.getHandle(logicalLink as MappedObject), DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByInterfaceResource(MCDDbLogicalLink logicalLink, MCDInterfaceResource interfaceResource)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByInterfaceResource(Handle, DTS_ObjectMapper.getHandle(logicalLink as MappedObject), DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByNameAndInterface(string logicalLinkName, MCDInterface interface_)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByNameAndInterface(Handle, logicalLinkName, DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByNameAndInterfaceResource(string logicalLinkName, MCDInterfaceResource interfaceResource)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByNameAndInterfaceResource(Handle, logicalLinkName, DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByVariantAndInterface(string shortNameDbLogicalLink, string variantName, MCDInterface interface_)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByVariantAndInterface(Handle, shortNameDbLogicalLink, variantName, DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDLogicalLink CreateLogicalLinkByVariantAndInterfaceResource(string shortNameDbLogicalLink, string variantName, MCDInterfaceResource interfaceResource)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByVariantAndInterfaceResource(Handle, shortNameDbLogicalLink, variantName, DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink;
	}

	public MCDMonitoringLink CreateMonitoringLink(MCDInterfaceResource ifResource)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createMonitoringLink(Handle, DTS_ObjectMapper.getHandle(ifResource as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitoringLink;
	}

	public MCDValues ExecIOCtrl(string IOCtrlName, MCDValue inputData, uint inputDataItemType, uint outputDataSize)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_execIOCtrl(Handle, IOCtrlName, DTS_ObjectMapper.getHandle(inputData as MappedObject), inputDataItemType, outputDataSize, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues;
	}

	public void RemoveMonitoringLink(MCDMonitoringLink monLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_removeMonitoringLink(Handle, DTS_ObjectMapper.getHandle(monLink as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnlinkDatabaseFile(string strFile)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_unlinkDatabaseFile(Handle, strFile);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDDbEcuMems LinkDatabaseFile(string strFile)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_linkDatabaseFile(Handle, strFile, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuMems;
	}

	public MCDMonitoringLink CreateDtsMonitoringLink(string PilShortName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createDtsMonitoringLink(Handle, PilShortName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitoringLink;
	}

	public DtsDoIPMonitorLink CreateDoIPMonitorLink(string NetworkId)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_createDoIPMonitorLink(Handle, NetworkId, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDoIPMonitorLink;
	}

	public void ClearLinkCache()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProject_clearLinkCache(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}
}
