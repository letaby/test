using System;
using System.Runtime.InteropServices;

namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct StringArray_Struct
{
	[MarshalAs(UnmanagedType.SysUInt)]
	internal IntPtr m_data;

	[MarshalAs(UnmanagedType.U4)]
	internal uint m_length;

	internal string[] ToStringArray()
	{
		if (m_length != 0)
		{
			string[] array = new string[m_length];
			IntPtr[] array2 = new IntPtr[m_length];
			Marshal.Copy(m_data, array2, 0, (int)m_length);
			for (uint num = 0u; num < array2.Length; num++)
			{
				array[num] = Marshal.PtrToStringUni(array2[num]);
			}
			CSWrap.CSNIDTS_releaseStringArray(ref this);
			return array;
		}
		return new string[0];
	}

	internal StringArray_Struct(string[] stringArray)
	{
		m_data = IntPtr.Zero;
		m_length = 0u;
		if (stringArray != null && stringArray.Length != 0)
		{
			m_length = (uint)stringArray.Length;
			m_data = CSWrap.CSNIDTS_cached_allocate((uint)(stringArray.Length * Marshal.SizeOf(typeof(IntPtr))));
			IntPtr[] array = new IntPtr[stringArray.Length];
			for (uint num = 0u; num < m_length; num++)
			{
				array[num] = CSWrap.CSNIDTS_allocate_string((uint)stringArray[num].Length);
				Marshal.Copy(stringArray[num].ToCharArray(), 0, array[num], stringArray[num].Length);
			}
			Marshal.Copy(array, 0, m_data, array.Length);
		}
	}

	internal void FreeMemory()
	{
		if (m_data != IntPtr.Zero)
		{
			CSWrap.CSNIDTS_cached_free(m_data);
		}
	}
}
