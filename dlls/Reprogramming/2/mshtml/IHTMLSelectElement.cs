using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace mshtml;

[ComImport]
[CompilerGenerated]
[Guid("3050F244-98B5-11CF-BB82-00AA00BDCE0B")]
[DefaultMember("item")]
[TypeIdentifier]
public interface IHTMLSelectElement : IEnumerable
{
	void _VtblGap1_12();

	[DispId(1011)]
	string value
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1011)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1011)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	void _VtblGap2_8();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(0)]
	[return: MarshalAs(UnmanagedType.IDispatch)]
	object item([Optional][In][MarshalAs(UnmanagedType.Struct)] object name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object index);
}
