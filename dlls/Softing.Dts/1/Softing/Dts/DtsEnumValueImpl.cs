// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsEnumValueImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsEnumValueImpl : 
  MappedObject,
  DtsEnumValue,
  MCDEnumValue,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsEnumValueImpl(IntPtr handle)
  {
    this.Handle = handle;
    DTS_ObjectMapper.registerObject(this.Handle, (object) this);
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!(this.Handle != IntPtr.Zero))
      return;
    DTS_ObjectMapper.unregisterObject(this.Handle);
    this.Handle = IntPtr.Zero;
  }

  ~DtsEnumValueImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public int GetEnumFromString(string enumString)
  {
    GC.KeepAlive((object) this);
    int returnValue;
    IntPtr enumFromString = CSWrap.CSNIDTS_DtsEnumValue_getEnumFromString(this.Handle, enumString, out returnValue);
    if (enumFromString != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(enumFromString);
    return returnValue;
  }

  public string GetStringFromEnum(int enumValue)
  {
    GC.KeepAlive((object) this);
    String_Struct returnValue = new String_Struct();
    IntPtr stringFromEnum = CSWrap.CSNIDTS_DtsEnumValue_getStringFromEnum(this.Handle, enumValue, out returnValue);
    if (stringFromEnum != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(stringFromEnum);
    return returnValue.makeString();
  }

  public MCDObjectType ObjectType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDObjectType returnValue;
      IntPtr objectType = CSWrap.CSNIDTS_DtsObject_getObjectType(this.Handle, out returnValue);
      if (objectType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectType);
      return returnValue;
    }
  }

  public uint ObjectID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr objectId = CSWrap.CSNIDTS_DtsObject_getObjectID(this.Handle, out returnValue);
      if (objectId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectId);
      return returnValue;
    }
  }
}
