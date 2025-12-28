using System;

namespace Softing.Dts;

internal class DtsSystemImpl : MappedObject, DtsSystem, MCDSystem, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	protected IntPtr m_dtsHandle = IntPtr.Zero;

	private uint handler_count;

	private uint listener_handle;

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

	public MCDProject ActiveProject
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getActiveProject(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject;
		}
	}

	public MCDVersion ASAMMCDVersion
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getASAMMCDVersion(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion;
		}
	}

	public DtsClassFactory ClassFactory
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getClassFactory(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsClassFactory;
		}
	}

	public string CurrentTracePath
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getCurrentTracePath(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public MCDDbProjectDescriptions DbProjectDescriptions
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getDbProjectDescriptions(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProjectDescriptions;
		}
	}

	public string InstallationPath
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getInstallationPath(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public MCDLockState LockState
	{
		get
		{
			GC.KeepAlive(this);
			MCDLockState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getLockState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint MaxNoOfClients
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getMaxNoOfClients(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string ProjectPath
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getProjectPath(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public MCDServerType ServerType
	{
		get
		{
			GC.KeepAlive(this);
			MCDServerType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getServerType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDSystemState State
	{
		get
		{
			GC.KeepAlive(this);
			MCDSystemState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDVersion Version
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getVersion(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion;
		}
	}

	public bool IsApiTraceEnabled
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_isApiTraceEnabled(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint ApiTraceLevel
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_setApiTraceLevel(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string TracePath
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getTracePath(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public MCDDbProjectConfiguration DbProjectConfiguration
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getDbProjectConfiguration(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProjectConfiguration;
		}
	}

	public MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalInterfaceLinks
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getDbPhysicalInterfaceLinks(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbPhysicalVehicleLinkOrInterfaces;
		}
	}

	public uint ClientNo
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getClientNo(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint InterfaceNumber
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getInterfaceNumber(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsUnsupportedComParametersAccepted
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_isUnsupportedComParametersAccepted(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public string FlashDataRoot
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_setFlashDataRoot(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string JavaLocation
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_setJavaLocation(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDInterfaces ConnectedInterfaces
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getConnectedInterfaces(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaces;
		}
	}

	public MCDInterfaces CurrentInterfaces
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getCurrentInterfaces(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaces;
		}
	}

	public string[] PropertyNames
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getPropertyNames(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public MCDEnumValue EnumValue
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getEnumValue(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsEnumValue;
		}
	}

	public string ConfigurationPath
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getConfigurationPath(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public DtsSystemConfig Configuration
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getConfiguration(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsSystemConfig;
		}
	}

	public string SessionProjectPath
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_setSessionProjectPath(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public string DebugTracePath
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getDebugTracePath(Handle, out returnValue);
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

	internal event OnDefinableDynIdListChanged __DefinableDynIdListChanged;

	public event OnDefinableDynIdListChanged DefinableDynIdListChanged
	{
		add
		{
			lock (this)
			{
				__DefinableDynIdListChanged += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__DefinableDynIdListChanged -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkActivityStateIdle __LinkActivityStateIdle;

	public event OnLinkActivityStateIdle LinkActivityStateIdle
	{
		add
		{
			lock (this)
			{
				__LinkActivityStateIdle += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkActivityStateIdle -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkActivityStateRunning __LinkActivityStateRunning;

	public event OnLinkActivityStateRunning LinkActivityStateRunning
	{
		add
		{
			lock (this)
			{
				__LinkActivityStateRunning += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkActivityStateRunning -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkActivityStateSuspended __LinkActivityStateSuspended;

	public event OnLinkActivityStateSuspended LinkActivityStateSuspended
	{
		add
		{
			lock (this)
			{
				__LinkActivityStateSuspended += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkActivityStateSuspended -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkError __LinkError;

	public event OnLinkError LinkError
	{
		add
		{
			lock (this)
			{
				__LinkError += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkError -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkLocked __LinkLocked;

	public event OnLinkLocked LinkLocked
	{
		add
		{
			lock (this)
			{
				__LinkLocked += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkLocked -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkOneVariantIdentified __LinkOneVariantIdentified;

	public event OnLinkOneVariantIdentified LinkOneVariantIdentified
	{
		add
		{
			lock (this)
			{
				__LinkOneVariantIdentified += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkOneVariantIdentified -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkOneVariantSelected __LinkOneVariantSelected;

	public event OnLinkOneVariantSelected LinkOneVariantSelected
	{
		add
		{
			lock (this)
			{
				__LinkOneVariantSelected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkOneVariantSelected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkQueueCleared __LinkQueueCleared;

	public event OnLinkQueueCleared LinkQueueCleared
	{
		add
		{
			lock (this)
			{
				__LinkQueueCleared += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkQueueCleared -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkStateCommunication __LinkStateCommunication;

	public event OnLinkStateCommunication LinkStateCommunication
	{
		add
		{
			lock (this)
			{
				__LinkStateCommunication += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkStateCommunication -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkStateCreated __LinkStateCreated;

	public event OnLinkStateCreated LinkStateCreated
	{
		add
		{
			lock (this)
			{
				__LinkStateCreated += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkStateCreated -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkStateOffline __LinkStateOffline;

	public event OnLinkStateOffline LinkStateOffline
	{
		add
		{
			lock (this)
			{
				__LinkStateOffline += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkStateOffline -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkStateOnline __LinkStateOnline;

	public event OnLinkStateOnline LinkStateOnline
	{
		add
		{
			lock (this)
			{
				__LinkStateOnline += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkStateOnline -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkUnlocked __LinkUnlocked;

	public event OnLinkUnlocked LinkUnlocked
	{
		add
		{
			lock (this)
			{
				__LinkUnlocked += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkUnlocked -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkVariantIdentified __LinkVariantIdentified;

	public event OnLinkVariantIdentified LinkVariantIdentified
	{
		add
		{
			lock (this)
			{
				__LinkVariantIdentified += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkVariantIdentified -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkVariantSelected __LinkVariantSelected;

	public event OnLinkVariantSelected LinkVariantSelected
	{
		add
		{
			lock (this)
			{
				__LinkVariantSelected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkVariantSelected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnLinkVariantSet __LinkVariantSet;

	public event OnLinkVariantSet LinkVariantSet
	{
		add
		{
			lock (this)
			{
				__LinkVariantSet += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__LinkVariantSet -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveBufferOverflow __PrimitiveBufferOverflow;

	public event OnPrimitiveBufferOverflow PrimitiveBufferOverflow
	{
		add
		{
			lock (this)
			{
				__PrimitiveBufferOverflow += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveBufferOverflow -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveCanceledDuringExecution __PrimitiveCanceledDuringExecution;

	public event OnPrimitiveCanceledDuringExecution PrimitiveCanceledDuringExecution
	{
		add
		{
			lock (this)
			{
				__PrimitiveCanceledDuringExecution += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveCanceledDuringExecution -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveCanceledFromQueue __PrimitiveCanceledFromQueue;

	public event OnPrimitiveCanceledFromQueue PrimitiveCanceledFromQueue
	{
		add
		{
			lock (this)
			{
				__PrimitiveCanceledFromQueue += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveCanceledFromQueue -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveError __PrimitiveError;

	public event OnPrimitiveError PrimitiveError
	{
		add
		{
			lock (this)
			{
				__PrimitiveError += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveError -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveHasIntermediateResult __PrimitiveHasIntermediateResult;

	public event OnPrimitiveHasIntermediateResult PrimitiveHasIntermediateResult
	{
		add
		{
			lock (this)
			{
				__PrimitiveHasIntermediateResult += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveHasIntermediateResult -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveHasResult __PrimitiveHasResult;

	public event OnPrimitiveHasResult PrimitiveHasResult
	{
		add
		{
			lock (this)
			{
				__PrimitiveHasResult += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveHasResult -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveJobInfo __PrimitiveJobInfo;

	public event OnPrimitiveJobInfo PrimitiveJobInfo
	{
		add
		{
			lock (this)
			{
				__PrimitiveJobInfo += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveJobInfo -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveProgressInfo __PrimitiveProgressInfo;

	public event OnPrimitiveProgressInfo PrimitiveProgressInfo
	{
		add
		{
			lock (this)
			{
				__PrimitiveProgressInfo += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveProgressInfo -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveRepetitionStopped __PrimitiveRepetitionStopped;

	public event OnPrimitiveRepetitionStopped PrimitiveRepetitionStopped
	{
		add
		{
			lock (this)
			{
				__PrimitiveRepetitionStopped += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveRepetitionStopped -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnPrimitiveTerminated __PrimitiveTerminated;

	public event OnPrimitiveTerminated PrimitiveTerminated
	{
		add
		{
			lock (this)
			{
				__PrimitiveTerminated += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__PrimitiveTerminated -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemClampStateChanged __SystemClampStateChanged;

	public event OnSystemClampStateChanged SystemClampStateChanged
	{
		add
		{
			lock (this)
			{
				__SystemClampStateChanged += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemClampStateChanged -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemConfigurationClosed __SystemConfigurationClosed;

	public event OnSystemConfigurationClosed SystemConfigurationClosed
	{
		add
		{
			lock (this)
			{
				__SystemConfigurationClosed += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemConfigurationClosed -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemConfigurationOpened __SystemConfigurationOpened;

	public event OnSystemConfigurationOpened SystemConfigurationOpened
	{
		add
		{
			lock (this)
			{
				__SystemConfigurationOpened += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemConfigurationOpened -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemError __SystemError;

	public event OnSystemError SystemError
	{
		add
		{
			lock (this)
			{
				__SystemError += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemError -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemLocked __SystemLocked;

	public event OnSystemLocked SystemLocked
	{
		add
		{
			lock (this)
			{
				__SystemLocked += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemLocked -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemLogicallyConnected __SystemLogicallyConnected;

	public event OnSystemLogicallyConnected SystemLogicallyConnected
	{
		add
		{
			lock (this)
			{
				__SystemLogicallyConnected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemLogicallyConnected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemLogicallyDisconnected __SystemLogicallyDisconnected;

	public event OnSystemLogicallyDisconnected SystemLogicallyDisconnected
	{
		add
		{
			lock (this)
			{
				__SystemLogicallyDisconnected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemLogicallyDisconnected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemProjectDeselected __SystemProjectDeselected;

	public event OnSystemProjectDeselected SystemProjectDeselected
	{
		add
		{
			lock (this)
			{
				__SystemProjectDeselected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemProjectDeselected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemProjectSelected __SystemProjectSelected;

	public event OnSystemProjectSelected SystemProjectSelected
	{
		add
		{
			lock (this)
			{
				__SystemProjectSelected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemProjectSelected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemUnlocked __SystemUnlocked;

	public event OnSystemUnlocked SystemUnlocked
	{
		add
		{
			lock (this)
			{
				__SystemUnlocked += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemUnlocked -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemVehicleInfoDeselected __SystemVehicleInfoDeselected;

	public event OnSystemVehicleInfoDeselected SystemVehicleInfoDeselected
	{
		add
		{
			lock (this)
			{
				__SystemVehicleInfoDeselected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemVehicleInfoDeselected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnSystemVehicleInfoSelected __SystemVehicleInfoSelected;

	public event OnSystemVehicleInfoSelected SystemVehicleInfoSelected
	{
		add
		{
			lock (this)
			{
				__SystemVehicleInfoSelected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__SystemVehicleInfoSelected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnConfigurationRecordLoaded __ConfigurationRecordLoaded;

	public event OnConfigurationRecordLoaded ConfigurationRecordLoaded
	{
		add
		{
			lock (this)
			{
				__ConfigurationRecordLoaded += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__ConfigurationRecordLoaded -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnStaticInterfaceError __StaticInterfaceError;

	public event OnStaticInterfaceError StaticInterfaceError
	{
		add
		{
			lock (this)
			{
				__StaticInterfaceError += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__StaticInterfaceError -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnInterfaceStatusChanged __InterfaceStatusChanged;

	public event OnInterfaceStatusChanged InterfaceStatusChanged
	{
		add
		{
			lock (this)
			{
				__InterfaceStatusChanged += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__InterfaceStatusChanged -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnMonitoringFramesReady __MonitoringFramesReady;

	public event OnMonitoringFramesReady MonitoringFramesReady
	{
		add
		{
			lock (this)
			{
				__MonitoringFramesReady += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__MonitoringFramesReady -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnInterfaceError __InterfaceError;

	public event OnInterfaceError InterfaceError
	{
		add
		{
			lock (this)
			{
				__InterfaceError += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__InterfaceError -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnInterfacesModified __InterfacesModified;

	public event OnInterfacesModified InterfacesModified
	{
		add
		{
			lock (this)
			{
				__InterfacesModified += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__InterfacesModified -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnDetectionFinished __DetectionFinished;

	public event OnDetectionFinished DetectionFinished
	{
		add
		{
			lock (this)
			{
				__DetectionFinished += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__DetectionFinished -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	internal event OnInterfaceDetected __InterfaceDetected;

	public event OnInterfaceDetected InterfaceDetected
	{
		add
		{
			lock (this)
			{
				__InterfaceDetected += value;
				if (handler_count == 0)
				{
					IntPtr intPtr = CSWrap.CSNIDTS_setEventListener(Handle, out listener_handle);
					if (intPtr != IntPtr.Zero)
					{
						throw DTS_ObjectMapper.createException(intPtr);
					}
				}
				handler_count++;
			}
		}
		remove
		{
			uint? num = null;
			lock (this)
			{
				__InterfaceDetected -= value;
				if (handler_count != 0)
				{
					handler_count--;
					if (handler_count == 0)
					{
						num = listener_handle;
						listener_handle = 0u;
					}
				}
			}
			if (num.HasValue)
			{
				IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, num.Value);
				if (intPtr != IntPtr.Zero)
				{
					throw DTS_ObjectMapper.createException(intPtr);
				}
			}
		}
	}

	public DtsSystemImpl(IntPtr handle)
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
		if (!(Handle != IntPtr.Zero))
		{
			return;
		}
		if (listener_handle != 0)
		{
			IntPtr intPtr = CSWrap.CSNIDTS_releaseEventListener(Handle, listener_handle);
			if (intPtr != IntPtr.Zero)
			{
				CSWrap.CSNIDTS_releaseObject(intPtr);
			}
			listener_handle = 0u;
		}
		DTS_ObjectMapper.unregisterObject(Handle);
		Handle = IntPtr.Zero;
	}

	~DtsSystemImpl()
	{
		Dispose(disposing: false);
	}

	public void CloseByteTrace(string PhysicalInterfaceLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_closeByteTrace(Handle, PhysicalInterfaceLink);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void DeselectProject()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_deselectProject(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void DisableApiTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_disableApiTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void EnableApiTrace()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_enableApiTrace(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public string GetStringFromEnum(uint eConst)
	{
		GC.KeepAlive(this);
		String_Struct returnValue = default(String_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getStringFromEnum(Handle, eConst, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.makeString();
	}

	public string GetStringFromErrorCode(MCDErrorCodes errorCode)
	{
		GC.KeepAlive(this);
		String_Struct returnValue = default(String_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getStringFromErrorCode(Handle, errorCode, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.makeString();
	}

	public void Initialize()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_initialize(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void OpenByteTrace(string PhysicalInterfaceLink, string FileName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_openByteTrace(Handle, PhysicalInterfaceLink, FileName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void PrepareInterface()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_prepareInterface(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDProject SelectProject(MCDDbProjectDescription project)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_selectProject(Handle, DTS_ObjectMapper.getHandle(project as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject;
	}

	public MCDProject SelectProjectByName(string project)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_selectProjectByName(Handle, project, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject;
	}

	public void StartByteTrace(string PhysicalInterfaceLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_startByteTrace(Handle, PhysicalInterfaceLink);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StopByteTrace(string PhysicalInterfaceLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_stopByteTrace(Handle, PhysicalInterfaceLink);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Uninitialize()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_uninitialize(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnprepareInterface()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_unprepareInterface(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public DtsMonitorLink CreateMonitorLinkByName(string PhysicalInterfaceLinkName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_createMonitorLinkByName(Handle, PhysicalInterfaceLinkName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitorLink;
	}

	public DtsMonitorLink CreateMonitorLink(MCDDbPhysicalVehicleLinkOrInterface PhysicalInterfaceLink)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_createMonitorLink(Handle, DTS_ObjectMapper.getHandle(PhysicalInterfaceLink as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitorLink;
	}

	public uint RegisterApp(uint appID, uint reqItem)
	{
		GC.KeepAlive(this);
		uint returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_registerApp(Handle, appID, reqItem, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public uint GetRequiredItem(uint ulID, uint reqItem)
	{
		GC.KeepAlive(this);
		uint returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getRequiredItem(Handle, ulID, reqItem, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public MCDValue GetSystemParameter(string value)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getSystemParameter(Handle, value, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
	}

	public void LockServer()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_lockServer(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnlockServer()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_unlockServer(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnsupportedComParametersAccepted(bool accept)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_unsupportedComParametersAccepted(Handle, accept);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void EnableInterface(string shortName, bool bEnable)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_enableInterface(Handle, shortName, bEnable);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDValue GetProperty(string name)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getProperty(Handle, name, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
	}

	public void SetProperty(string name, MCDValue value)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_setProperty(Handle, name, DTS_ObjectMapper.getHandle(value as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void ResetProperty(string name)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_resetProperty(Handle, name);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public byte[] GetSeed(uint procedureId, DtsAppID appId)
	{
		GC.KeepAlive(this);
		ByteField_Struct returnValue = default(ByteField_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getSeed(Handle, procedureId, appId, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToByteArray();
	}

	public void SendKey(byte[] key)
	{
		GC.KeepAlive(this);
		ByteField_Struct _key = new ByteField_Struct(key);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_sendKey(Handle, ref _key);
		_key.FreeMemory();
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void DetectInterfaces(string optionString)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_detectInterfaces(Handle, optionString);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void PrepareVciAccessLayer()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_prepareVciAccessLayer(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnprepareVciAccessLayer()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_unprepareVciAccessLayer(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void DumpRunningObjects(string outputFile, bool singleObjects)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_dumpRunningObjects(Handle, outputFile, singleObjects);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDDbProject LoadViewerProject(string[] databaseFiles)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		StringArray_Struct _databaseFiles = new StringArray_Struct(databaseFiles);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_loadViewerProject(Handle, ref _databaseFiles, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProject;
	}

	public void UnloadViewerProject(MCDDbProject project)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_unloadViewerProject(Handle, DTS_ObjectMapper.getHandle(project as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void ReloadConfiguration()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_reloadConfiguration(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void DumpMemoryUsage(string outputFile, int flags, bool append)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_dumpMemoryUsage(Handle, outputFile, flags, append);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void WriteTraceEntry(string message)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_writeTraceEntry(Handle, message);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StartSnapshotModeTracing(string PhysicalInterfaceLink, uint timeInterval)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_startSnapshotModeTracing(Handle, PhysicalInterfaceLink, timeInterval);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void TakeSnapshotByteTrace(string PhysicalInterfaceLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_takeSnapshotByteTrace(Handle, PhysicalInterfaceLink);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void GenerateSnapshotByteTrace(string PhysicalInterfaceLink, string outputPath)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_generateSnapshotByteTrace(Handle, PhysicalInterfaceLink, outputPath);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void StopSnapshotModeTracing(string PhysicalInterfaceLink)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_stopSnapshotModeTracing(Handle, PhysicalInterfaceLink);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void WriteExternTraceEntry(string prefix, string message)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_writeExternTraceEntry(Handle, prefix, message);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDProject SelectDynamicProject(string name, string[] files)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		StringArray_Struct _files = new StringArray_Struct(files);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_selectDynamicProject(Handle, name, ref _files, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject;
	}

	public MCDValue GetOptionalProperty(string name)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_getOptionalProperty(Handle, name, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue;
	}

	public void StartInterfaceDetection()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_startInterfaceDetection(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void CreateJVM()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsSystem_createJVM(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	internal bool _onDefinableDynIdListChanged(MCDValues dynIdList, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__DefinableDynIdListChanged != null)
			{
				this.__DefinableDynIdListChanged(this, new DefinableDynIdListChangedArgs(dynIdList, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkActivityStateIdle(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkActivityStateIdle != null)
			{
				this.__LinkActivityStateIdle(this, new LinkActivityStateIdleArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkActivityStateRunning(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkActivityStateRunning != null)
			{
				this.__LinkActivityStateRunning(this, new LinkActivityStateRunningArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkActivityStateSuspended(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkActivityStateSuspended != null)
			{
				this.__LinkActivityStateSuspended(this, new LinkActivityStateSuspendedArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkError(MCDLogicalLink link, MCDError error)
	{
		lock (this)
		{
			if (this.__LinkError != null)
			{
				this.__LinkError(this, new LinkErrorArgs(link, error));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkLocked(MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__LinkLocked != null)
			{
				this.__LinkLocked(this, new LinkLockedArgs(link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkOneVariantIdentified(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkOneVariantIdentified != null)
			{
				this.__LinkOneVariantIdentified(this, new LinkOneVariantIdentifiedArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkOneVariantSelected(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkOneVariantSelected != null)
			{
				this.__LinkOneVariantSelected(this, new LinkOneVariantSelectedArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkQueueCleared(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkQueueCleared != null)
			{
				this.__LinkQueueCleared(this, new LinkQueueClearedArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkStateCommunication(MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__LinkStateCommunication != null)
			{
				this.__LinkStateCommunication(this, new LinkStateCommunicationArgs(link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkStateCreated(MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__LinkStateCreated != null)
			{
				this.__LinkStateCreated(this, new LinkStateCreatedArgs(link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkStateOffline(MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__LinkStateOffline != null)
			{
				this.__LinkStateOffline(this, new LinkStateOfflineArgs(link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkStateOnline(MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__LinkStateOnline != null)
			{
				this.__LinkStateOnline(this, new LinkStateOnlineArgs(link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkUnlocked(MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__LinkUnlocked != null)
			{
				this.__LinkUnlocked(this, new LinkUnlockedArgs(link));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkVariantIdentified(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkVariantIdentified != null)
			{
				this.__LinkVariantIdentified(this, new LinkVariantIdentifiedArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkVariantSelected(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkVariantSelected != null)
			{
				this.__LinkVariantSelected(this, new LinkVariantSelectedArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onLinkVariantSet(MCDLogicalLink link, MCDLogicalLinkState linkstate)
	{
		lock (this)
		{
			if (this.__LinkVariantSet != null)
			{
				this.__LinkVariantSet(this, new LinkVariantSetArgs(link, linkstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveBufferOverflow(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveBufferOverflow != null)
			{
				this.__PrimitiveBufferOverflow(this, new PrimitiveBufferOverflowArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveCanceledDuringExecution(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveCanceledDuringExecution != null)
			{
				this.__PrimitiveCanceledDuringExecution(this, new PrimitiveCanceledDuringExecutionArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveCanceledFromQueue(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveCanceledFromQueue != null)
			{
				this.__PrimitiveCanceledFromQueue(this, new PrimitiveCanceledFromQueueArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveError(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDError error)
	{
		lock (this)
		{
			if (this.__PrimitiveError != null)
			{
				this.__PrimitiveError(this, new PrimitiveErrorArgs(primitive, link, error));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveHasIntermediateResult(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		lock (this)
		{
			if (this.__PrimitiveHasIntermediateResult != null)
			{
				this.__PrimitiveHasIntermediateResult(this, new PrimitiveHasIntermediateResultArgs(primitive, link, resultstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveHasResult(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		lock (this)
		{
			if (this.__PrimitiveHasResult != null)
			{
				this.__PrimitiveHasResult(this, new PrimitiveHasResultArgs(primitive, link, resultstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveJobInfo(MCDDiagComPrimitive primitive, MCDLogicalLink link, string info)
	{
		lock (this)
		{
			if (this.__PrimitiveJobInfo != null)
			{
				this.__PrimitiveJobInfo(this, new PrimitiveJobInfoArgs(primitive, link, info));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveProgressInfo(MCDDiagComPrimitive primitive, MCDLogicalLink link, byte progress)
	{
		lock (this)
		{
			if (this.__PrimitiveProgressInfo != null)
			{
				this.__PrimitiveProgressInfo(this, new PrimitiveProgressInfoArgs(primitive, link, progress));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveRepetitionStopped(MCDDiagComPrimitive primitive, MCDLogicalLink link)
	{
		lock (this)
		{
			if (this.__PrimitiveRepetitionStopped != null)
			{
				this.__PrimitiveRepetitionStopped(this, new PrimitiveRepetitionStoppedArgs(primitive, link));
				return true;
			}
		}
		return false;
	}

	internal bool _onPrimitiveTerminated(MCDDiagComPrimitive primitive, MCDLogicalLink link, MCDResultState resultstate)
	{
		lock (this)
		{
			if (this.__PrimitiveTerminated != null)
			{
				this.__PrimitiveTerminated(this, new PrimitiveTerminatedArgs(primitive, link, resultstate));
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemClampStateChanged(string clamp, MCDClampState clampState)
	{
		lock (this)
		{
			if (this.__SystemClampStateChanged != null)
			{
				this.__SystemClampStateChanged(this, new SystemClampStateChangedArgs(clamp, clampState));
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemConfigurationClosed()
	{
		lock (this)
		{
			if (this.__SystemConfigurationClosed != null)
			{
				this.__SystemConfigurationClosed(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemConfigurationOpened()
	{
		lock (this)
		{
			if (this.__SystemConfigurationOpened != null)
			{
				this.__SystemConfigurationOpened(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemError(MCDError error)
	{
		lock (this)
		{
			if (this.__SystemError != null)
			{
				this.__SystemError(this, new SystemErrorArgs(error));
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemLocked()
	{
		lock (this)
		{
			if (this.__SystemLocked != null)
			{
				this.__SystemLocked(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemLogicallyConnected()
	{
		lock (this)
		{
			if (this.__SystemLogicallyConnected != null)
			{
				this.__SystemLogicallyConnected(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemLogicallyDisconnected()
	{
		lock (this)
		{
			if (this.__SystemLogicallyDisconnected != null)
			{
				this.__SystemLogicallyDisconnected(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemProjectDeselected()
	{
		lock (this)
		{
			if (this.__SystemProjectDeselected != null)
			{
				this.__SystemProjectDeselected(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemProjectSelected()
	{
		lock (this)
		{
			if (this.__SystemProjectSelected != null)
			{
				this.__SystemProjectSelected(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemUnlocked()
	{
		lock (this)
		{
			if (this.__SystemUnlocked != null)
			{
				this.__SystemUnlocked(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemVehicleInfoDeselected()
	{
		lock (this)
		{
			if (this.__SystemVehicleInfoDeselected != null)
			{
				this.__SystemVehicleInfoDeselected(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onSystemVehicleInfoSelected()
	{
		lock (this)
		{
			if (this.__SystemVehicleInfoSelected != null)
			{
				this.__SystemVehicleInfoSelected(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onConfigurationRecordLoaded(MCDConfigurationRecord configurationRecord)
	{
		lock (this)
		{
			if (this.__ConfigurationRecordLoaded != null)
			{
				this.__ConfigurationRecordLoaded(this, new ConfigurationRecordLoadedArgs(configurationRecord));
				return true;
			}
		}
		return false;
	}

	internal bool _onStaticInterfaceError(MCDError error)
	{
		lock (this)
		{
			if (this.__StaticInterfaceError != null)
			{
				this.__StaticInterfaceError(this, new StaticInterfaceErrorArgs(error));
				return true;
			}
		}
		return false;
	}

	internal bool _onInterfaceStatusChanged(MCDInterface interface_, MCDInterfaceStatus status)
	{
		lock (this)
		{
			if (this.__InterfaceStatusChanged != null)
			{
				this.__InterfaceStatusChanged(this, new InterfaceStatusChangedArgs(interface_, status));
				return true;
			}
		}
		return false;
	}

	internal bool _onMonitoringFramesReady(MCDMonitoringLink monLink)
	{
		lock (this)
		{
			if (this.__MonitoringFramesReady != null)
			{
				this.__MonitoringFramesReady(this, new MonitoringFramesReadyArgs(monLink));
				return true;
			}
		}
		return false;
	}

	internal bool _onInterfaceError(MCDInterface interface_, MCDError error)
	{
		lock (this)
		{
			if (this.__InterfaceError != null)
			{
				this.__InterfaceError(this, new InterfaceErrorArgs(interface_, error));
				return true;
			}
		}
		return false;
	}

	internal bool _onInterfacesModified()
	{
		lock (this)
		{
			if (this.__InterfacesModified != null)
			{
				this.__InterfacesModified(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onDetectionFinished()
	{
		lock (this)
		{
			if (this.__DetectionFinished != null)
			{
				this.__DetectionFinished(this, new EventArgs());
				return true;
			}
		}
		return false;
	}

	internal bool _onInterfaceDetected(MCDInterface interface_)
	{
		lock (this)
		{
			if (this.__InterfaceDetected != null)
			{
				this.__InterfaceDetected(this, new InterfaceDetectedArgs(interface_));
				return true;
			}
		}
		return false;
	}
}
