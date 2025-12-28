// Decompiled with JetBrains decompiler
// Type: mshtml.IHTMLSelectElement
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace mshtml;

[CompilerGenerated]
[Guid("3050F244-98B5-11CF-BB82-00AA00BDCE0B")]
[DefaultMember("item")]
[TypeIdentifier]
[ComImport]
public interface IHTMLSelectElement : IEnumerable
{
  [SpecialName]
  [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
  sealed extern void _VtblGap1_12();

  [DispId(1011)]
  string value { [DispId(1011), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; [DispId(1011), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

  [SpecialName]
  [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
  sealed extern void _VtblGap2_8();

  [DispId(0)]
  [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
  [return: MarshalAs(UnmanagedType.IDispatch)]
  object item([MarshalAs(UnmanagedType.Struct), In, Optional] object name, [MarshalAs(UnmanagedType.Struct), In, Optional] object index);
}
