// Decompiled with JetBrains decompiler
// Type: Softing.Dts.ObjectInfo_Struct
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct ObjectInfo_Struct
{
  [MarshalAs(UnmanagedType.SysUInt)]
  internal IntPtr m_handle;
  [MarshalAs(UnmanagedType.I4)]
  internal MCDObjectType m_type;

  internal MCDObject ToObject()
  {
    return this.m_handle != IntPtr.Zero ? DTS_ObjectMapper.createObject(this.m_handle, this.m_type) : (MCDObject) null;
  }
}
