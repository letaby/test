using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

internal class DtsInterfaceConfigsImpl : MappedObject, DtsInterfaceConfigs, DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	private class DtsInterfaceConfigsEnumerator : IEnumerator
	{
		private DtsInterfaceConfigsImpl m_classList;

		private int m_index;

		public object Current => m_classList.GetItemByIndex((uint)m_index);

		public DtsInterfaceConfigsEnumerator(DtsInterfaceConfigsImpl classList)
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

	public DtsInterfaceConfigsImpl(IntPtr handle)
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

	~DtsInterfaceConfigsImpl()
	{
		Dispose(disposing: false);
	}

	public DtsInterfaceConfig GetItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfigs_getItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceConfig;
	}

	public DtsInterfaceConfig CreateInterface()
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfigs_createInterface(Handle, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceConfig;
	}

	public void Remove(DtsInterfaceConfig interfaceConfig)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsInterfaceConfigs_remove(Handle, DTS_ObjectMapper.getHandle(interfaceConfig as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
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

	public List<DtsInterfaceConfig> ToList()
	{
		List<DtsInterfaceConfig> list = new List<DtsInterfaceConfig>();
		for (uint num = 0u; num < Count; num++)
		{
			list.Add(GetItemByIndex(num));
		}
		return list;
	}

	public DtsInterfaceConfig[] ToArray()
	{
		DtsInterfaceConfig[] array = new DtsInterfaceConfig[Count];
		for (uint num = 0u; num < Count; num++)
		{
			array[num] = GetItemByIndex(num);
		}
		return array;
	}

	public IEnumerator GetEnumerator()
	{
		return new DtsInterfaceConfigsEnumerator(this);
	}
}
