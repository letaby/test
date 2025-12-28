using System;
using System.Runtime.InteropServices;

namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct StringReturn_Struct
{
	[MarshalAs(UnmanagedType.LPTStr)]
	internal IntPtr m_data;
}
