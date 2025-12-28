using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

internal class DtsConfigurationRecordsImpl : MappedObject, DtsConfigurationRecords, MCDConfigurationRecords, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
	private class DtsConfigurationRecordsEnumerator : IEnumerator
	{
		private DtsConfigurationRecordsImpl m_classList;

		private int m_index;

		public object Current => m_classList.GetItemByIndex((uint)m_index);

		public DtsConfigurationRecordsEnumerator(DtsConfigurationRecordsImpl classList)
		{
			m_classList = classList;
			m_index = -1;
		}

		public void Reset()
		{
			m_index = -1;
		}

		public bool MoveNext()
		{
			m_index++;
			if (m_index >= m_classList.Count)
			{
				return false;
			}
			return true;
		}
	}

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

	public string[] Names
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedCollection_getNames(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
		}
	}

	public uint Count
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsCollection_getCount(Handle, out returnValue);
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

	public DtsConfigurationRecordsImpl(IntPtr handle)
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

	~DtsConfigurationRecordsImpl()
	{
		Dispose(disposing: false);
	}

	public MCDConfigurationRecord GetItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_getItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord;
	}

	public MCDConfigurationRecord GetItemByName(string name)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_getItemByName(Handle, name, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord;
	}

	public void Remove(MCDConfigurationRecord ConfigurationRecord)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_remove(Handle, DTS_ObjectMapper.getHandle(ConfigurationRecord as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveAll()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_removeAll(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveByIndex(uint index)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_removeByIndex(Handle, index);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveByName(string name)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_removeByName(Handle, name);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public MCDConfigurationRecord AddByConfigurationIDAndDbConfigurationData(MCDValue ConfigurationID, MCDDbConfigurationData configurationData)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_addByConfigurationIDAndDbConfigurationData(Handle, DTS_ObjectMapper.getHandle(ConfigurationID as MappedObject), DTS_ObjectMapper.getHandle(configurationData as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord;
	}

	public MCDConfigurationRecord AddByDbObject(MCDDbConfigurationRecord DbConfigurationRecord)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_addByDbObject(Handle, DTS_ObjectMapper.getHandle(DbConfigurationRecord as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord;
	}

	public MCDConfigurationRecord AddByNameAndDbConfigurationData(string dbConfigurationRecordName, MCDDbConfigurationData dbConfigurationData)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsConfigurationRecords_addByNameAndDbConfigurationData(Handle, dbConfigurationRecordName, DTS_ObjectMapper.getHandle(dbConfigurationData as MappedObject), out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord;
	}

	public MCDObject GetObjectItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsCollection_getObjectItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject;
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

	public List<MCDConfigurationRecord> ToList()
	{
		List<MCDConfigurationRecord> list = new List<MCDConfigurationRecord>();
		for (uint num = 0u; num < Count; num++)
		{
			list.Add(GetItemByIndex(num));
		}
		return list;
	}

	public MCDConfigurationRecord[] ToArray()
	{
		MCDConfigurationRecord[] array = new MCDConfigurationRecord[Count];
		for (uint num = 0u; num < Count; num++)
		{
			array[num] = GetItemByIndex(num);
		}
		return array;
	}

	public IEnumerator GetEnumerator()
	{
		return new DtsConfigurationRecordsEnumerator(this);
	}
}
