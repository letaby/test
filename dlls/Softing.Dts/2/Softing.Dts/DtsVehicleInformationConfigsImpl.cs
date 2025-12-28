using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

internal class DtsVehicleInformationConfigsImpl : MappedObject, DtsVehicleInformationConfigs, DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	private class DtsVehicleInformationConfigsEnumerator : IEnumerator
	{
		private DtsVehicleInformationConfigsImpl m_classList;

		private int m_index;

		public object Current => m_classList.GetItemByIndex((uint)m_index);

		public DtsVehicleInformationConfigsEnumerator(DtsVehicleInformationConfigsImpl classList)
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

	public DtsVehicleInformationConfigsImpl(IntPtr handle)
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

	~DtsVehicleInformationConfigsImpl()
	{
		Dispose(disposing: false);
	}

	public DtsVehicleInformationConfig GetItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsVehicleInformationConfigs_getItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVehicleInformationConfig;
	}

	public void RemoveItem(DtsVehicleInformationConfig vehicleInfoConfig)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsVehicleInformationConfigs_removeItem(Handle, DTS_ObjectMapper.getHandle(vehicleInfoConfig as MappedObject));
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

	public List<DtsVehicleInformationConfig> ToList()
	{
		List<DtsVehicleInformationConfig> list = new List<DtsVehicleInformationConfig>();
		for (uint num = 0u; num < Count; num++)
		{
			list.Add(GetItemByIndex(num));
		}
		return list;
	}

	public DtsVehicleInformationConfig[] ToArray()
	{
		DtsVehicleInformationConfig[] array = new DtsVehicleInformationConfig[Count];
		for (uint num = 0u; num < Count; num++)
		{
			array[num] = GetItemByIndex(num);
		}
		return array;
	}

	public IEnumerator GetEnumerator()
	{
		return new DtsVehicleInformationConfigsEnumerator(this);
	}
}
