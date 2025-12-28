using System;

namespace Softing.Dts;

internal class DtsProjectConfigImpl : MappedObject, DtsProjectConfig, DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
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

	public DtsVehicleInformationConfigs VehicleInformations
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getVehicleInformations(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVehicleInformationConfigs;
		}
	}

	public string Database
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getDatabase(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public string VehicleModelRange
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getVehicleModelRange(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setVehicleModelRange(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string[] ExternalFlashFiles
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getExternalFlashFiles(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public bool UseOptimizedDatabase
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getUseOptimizedDatabase(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setUseOptimizedDatabase(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string OptimizedDatabase
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getOptimizedDatabase(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public DtsProjectType ProjectType
	{
		get
		{
			GC.KeepAlive(this);
			DtsProjectType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getProjectType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setProjectType(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool IsDatabaseODX201Legacy
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_isDatabaseODX201Legacy(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool SynchronizeVitWithDatabase
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getSynchronizeVitWithDatabase(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setSynchronizeVitWithDatabase(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool CreateDefaultVit
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getCreateDefaultVit(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setCreateDefaultVit(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string DCDIFirmware
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getDCDIFirmware(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setDCDIFirmware(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public uint PreferredDbEncryption
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getPreferredDbEncryption(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setPreferredDbEncryption(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool IsOptimizedDatabaseODX201Legacy
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_isOptimizedDatabaseODX201Legacy(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool JavaJob201Legacy
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getJavaJob201Legacy(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setJavaJob201Legacy(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public DtsLogicalLinkFilterConfigs LogicalLinkFilters
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getLogicalLinkFilters(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLinkFilterConfigs;
		}
	}

	public string PreferredOptionSet
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getPreferredOptionSet(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setPreferredOptionSet(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool IsReferencing
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_isReferencing(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool WriteSimFile
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getWriteSimFile(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setWriteSimFile(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public bool SimFileAppend
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getSimFileAppend(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setSimFileAppend(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string ActiveSimFile
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getActiveSimFile(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setActiveSimFile(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public uint Characteristic
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getCharacteristic(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setCharacteristic(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
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

	public DtsProjectConfigImpl(IntPtr handle)
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

	~DtsProjectConfigImpl()
	{
		Dispose(disposing: false);
	}

	public string[] GetModularDatabaseFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getModularDatabaseFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void AddModularDatabaseFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addModularDatabaseFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string[] GetCxfDatabaseFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getCxfDatabaseFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void AddCxfDatabaseFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addCxfDatabaseFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void SetDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setDatabaseFile(Handle, databaseFile, allowMove, overwriteExisting);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveCxfDatabaseFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeCxfDatabaseFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveVariantCodingConfigFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeVariantCodingConfigFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void AddCANFilterFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addCANFilterFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveCANFilterFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeCANFilterFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void SetOptimizedDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_setOptimizedDatabaseFile(Handle, databaseFile, allowMove, overwriteExisting);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void AddExternalFlashFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addExternalFlashFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string[] GetCANFilterFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getCANFilterFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void RemoveExternalFlashFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeExternalFlashFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UpdateVehicleInformationTables()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_updateVehicleInformationTables(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void AddOTXProject(string otxProject, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addOTXProject(Handle, otxProject, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string[] GetOTXProjects(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getOTXProjects(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void RemoveOTXProjects(string[] otxProjects)
	{
		GC.KeepAlive(this);
		StringArray_Struct _otxProjects = new StringArray_Struct(otxProjects);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeOTXProjects(Handle, ref _otxProjects);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void GenerateOptimizedDatabase(uint encryption)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_generateOptimizedDatabase(Handle, encryption);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public int CheckDatabaseConsistency(bool bRunCheck)
	{
		GC.KeepAlive(this);
		int returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_checkDatabaseConsistency(Handle, bRunCheck, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public string[] GetVariantCodingConfigFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getVariantCodingConfigFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void AddVariantCodingConfigFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addVariantCodingConfigFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveModularDatabaseFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeModularDatabaseFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public int CheckOptimizedDatabaseConsistency()
	{
		GC.KeepAlive(this);
		int returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_checkOptimizedDatabaseConsistency(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public void AddAdditionalDatabaseFiles(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addAdditionalDatabaseFiles(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string[] GetAdditionalDatabaseFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getAdditionalDatabaseFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void RemoveAdditionalDatabaseFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeAdditionalDatabaseFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void AddLogicalLinkFilterFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addLogicalLinkFilterFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string[] GetLogicalLinkFilterFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getLogicalLinkFilterFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void RemoveLogicalLinkFilterFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeLogicalLinkFilterFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void AddSimFile(string file, bool allowMove)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_addSimFile(Handle, file, allowMove);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string[] GetSimFiles(bool bForceReload)
	{
		GC.KeepAlive(this);
		StringArray_Struct returnValue = default(StringArray_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_getSimFiles(Handle, bForceReload, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToStringArray();
	}

	public void RemoveSimFiles(string[] files)
	{
		GC.KeepAlive(this);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsProjectConfig_removeSimFiles(Handle, ref _files);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}
}
