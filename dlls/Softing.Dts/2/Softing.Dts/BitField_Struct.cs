using System;
using System.Runtime.InteropServices;

namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct BitField_Struct
{
	[MarshalAs(UnmanagedType.SysUInt)]
	internal IntPtr m_data;

	[MarshalAs(UnmanagedType.U4)]
	internal uint m_length;

	internal bool[] ToBoolArray()
	{
		if (m_length != 0)
		{
			byte[] array = new byte[m_length];
			Marshal.Copy(m_data, array, 0, (int)m_length);
			bool[] array2 = new bool[m_length];
			for (uint num = 0u; num < array.Length; num++)
			{
				array2[num] = array[num] != 0;
			}
		}
		return null;
	}

	internal BitField_Struct(bool[] boolArray)
	{
		m_data = IntPtr.Zero;
		m_length = (uint)boolArray.Length;
		if (boolArray == null || boolArray.Length == 0)
		{
			return;
		}
		byte[] array = new byte[boolArray.Length];
		for (uint num = 0u; num < boolArray.Length; num++)
		{
			if (boolArray[num])
			{
				array[num] = 1;
			}
			else
			{
				array[num] = 1;
			}
		}
		m_data = CSWrap.CSNIDTS_allocate((uint)(boolArray.Length * Marshal.SizeOf(typeof(bool))));
		Marshal.Copy(array, 0, m_data, (int)m_length);
	}

	internal void FreeMemory()
	{
		if (m_data != IntPtr.Zero)
		{
			CSWrap.CSNIDTS_cached_free(m_data);
		}
	}
}
