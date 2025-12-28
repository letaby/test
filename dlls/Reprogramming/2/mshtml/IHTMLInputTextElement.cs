using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace mshtml;

[ComImport]
[CompilerGenerated]
[Guid("3050F2A6-98B5-11CF-BB82-00AA00BDCE0B")]
[TypeIdentifier]
public interface IHTMLInputTextElement
{
	void _VtblGap1_1();

	[DispId(-2147413011)]
	string value
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(-2147413011)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(-2147413011)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}
}
