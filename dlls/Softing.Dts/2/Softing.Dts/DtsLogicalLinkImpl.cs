using System;

namespace Softing.Dts;

internal class DtsLogicalLinkImpl : MappedObject, DtsLogicalLink, MCDLogicalLink, MCDObject, IDisposable, DtsObject
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

	public MCDDbLogicalLink DbObject
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getDbObject(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLogicalLink;
		}
	}

	public MCDAccessKeys IdentifiedVariantAccessKeys
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getIdentifiedVariantAccessKeys(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKeys;
		}
	}

	public MCDLockState LockState
	{
		get
		{
			GC.KeepAlive(this);
			MCDLockState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getLockState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDQueueErrorMode QueueErrorMode
	{
		get
		{
			GC.KeepAlive(this);
			MCDQueueErrorMode returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getQueueErrorMode(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_setQueueErrorMode(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public uint QueueFillingLevel
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getQueueFillingLevel(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool AutoSyncWithInternalState
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getAutoSyncWithInternalState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_setAutoSyncWithInternalState(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDActivityState QueueState
	{
		get
		{
			GC.KeepAlive(this);
			MCDActivityState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getQueueState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDAccessKeys SelectedVariantAccessKeys
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getSelectedVariantAccessKeys(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKeys;
		}
	}

	public MCDLogicalLinkState State
	{
		get
		{
			GC.KeepAlive(this);
			MCDLogicalLinkState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDLogicalLinkType Type
	{
		get
		{
			GC.KeepAlive(this);
			MCDLogicalLinkType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool HasDetectedVariant
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_hasDetectedVariant(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDProtocolParameterSet ProtocolParameters
	{
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_setProtocolParameters(Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDValues DefinableDynIds
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getDefinableDynIds(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues;
		}
	}

	public bool IsUnsupportedComParametersAccepted
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_isUnsupportedComParametersAccepted(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public DtsPhysicalInterfaceLink PhysicalInterfaceLink
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getPhysicalInterfaceLink(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsPhysicalInterfaceLink;
		}
	}

	public MCDConfigurationRecords ConfigurationRecords
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getConfigurationRecords(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecords;
		}
	}

	public uint UniqueRuntimeID
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getUniqueRuntimeID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public MCDInterfaceResource InterfaceResource
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getInterfaceResource(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceResource;
		}
	}

	public MCDDbMatchingPattern MatchedDbEcuVariantPattern
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getMatchedDbEcuVariantPattern(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbMatchingPattern;
		}
	}

	public bool ChannelMonitoring
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getChannelMonitoring(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_setChannelMonitoring(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public MCDAccessKey CreationAccessKey
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getCreationAccessKey(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKey;
		}
	}

	public MCDLogicalLinkState InternalState
	{
		get
		{
			GC.KeepAlive(this);
			MCDLogicalLinkState returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getInternalState(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint QueueSize
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getQueueSize(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
		set
		{
			GC.KeepAlive(this);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_setQueueSize(Handle, value);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
		}
	}

	public uint OpenCounter
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getOpenCounter(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint OnlineCounter
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getOnlineCounter(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint StartCommCounter
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getStartCommCounter(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public uint LockedCounter
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getLockedCounter(Handle, out returnValue);
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

	public DtsLogicalLinkImpl(IntPtr handle)
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

	~DtsLogicalLinkImpl()
	{
		Dispose(disposing: false);
	}

	public void ClearQueue()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_clearQueue(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Close()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_close(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void LockLink()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_lockLink(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Open()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_open(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Resume()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_resume(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public bool SupportsTimeStamp()
	{
		GC.KeepAlive(this);
		bool returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_supportsTimeStamp(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public void Suspend()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_suspend(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnlockLink()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_unlockLink(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDDiagComPrimitive CreateDiagComPrimitiveByDbObject(MCDDbDiagComPrimitive dbComPrimitive)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByDbObject(Handle, DTS_ObjectMapper.getHandle(dbComPrimitive as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive;
	}

	public MCDDiagComPrimitive CreateDiagComPrimitiveByName(string shortName)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByName(Handle, shortName, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive;
	}

	public MCDDiagComPrimitive CreateDiagComPrimitiveBySemanticAttribute(string semanticAttribute)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveBySemanticAttribute(Handle, semanticAttribute, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive;
	}

	public MCDDiagComPrimitive CreateDiagComPrimitiveByType(MCDObjectType diagComPrimitiveType)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByType(Handle, diagComPrimitiveType, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive;
	}

	public MCDService CreateDVServiceByRelationType(string relationType)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_createDVServiceByRelationType(Handle, relationType, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsService;
	}

	public void DisableReducedResults()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_disableReducedResults(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void GotoOnline()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_gotoOnline(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void Reset()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_reset(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void SetUnitGroup(string shortName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_setUnitGroup(Handle, shortName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnsupportedComParametersAccepted(bool accept)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_unsupportedComParametersAccepted(Handle, accept);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void EnableReducedResults()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_enableReducedResults(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveDiagComPrimitive(MCDDiagComPrimitive diagComPrimitive)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_removeDiagComPrimitive(Handle, DTS_ObjectMapper.getHandle(diagComPrimitive as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void GotoOffline()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_gotoOffline(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDDbUnitGroup GetUnitGroup()
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_getUnitGroup(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbUnitGroup;
	}

	public MCDDiagService CreateDynIdComPrimitiveByTypeAndDefinitionMode(MCDObjectType type, string definitionMode)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_createDynIdComPrimitiveByTypeAndDefinitionMode(Handle, type, definitionMode, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagService;
	}

	public byte[] ExecuteIoCtl(uint uIoCtlCommandId, byte[] pInputData)
	{
		GC.KeepAlive(this);
		ByteField_Struct returnValue = default(ByteField_Struct);
		ByteField_Struct _pInputData = new ByteField_Struct(pInputData);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_executeIoCtl(Handle, uIoCtlCommandId, ref _pInputData, out returnValue);
		_pInputData.FreeMemory();
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.ToByteArray();
	}

	public MCDValues ExecIOCtrl(string IOCtrlName, MCDValue inputData, uint inputDataItemType, uint outputDataSize)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_execIOCtrl(Handle, IOCtrlName, DTS_ObjectMapper.getHandle(inputData as MappedObject), inputDataItemType, outputDataSize, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues;
	}

	public void SendBreak()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_sendBreak(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void GotoOnlineWithTimeout(uint timeout)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_gotoOnlineWithTimeout(Handle, timeout);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void OpenCached(bool useVariant)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsLogicalLink_openCached(Handle, useVariant);
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
}
