// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbSpecialDataGroupImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

internal class DtsDbSpecialDataGroupImpl : 
  MappedObject,
  DtsDbSpecialDataGroup,
  MCDDbSpecialDataGroup,
  MCDDbSpecialData,
  MCDObject,
  IDisposable,
  DtsDbSpecialData,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbSpecialDataGroupImpl(IntPtr handle)
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

  ~DtsDbSpecialDataGroupImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint Count
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr count = CSWrap.CSNIDTS_DtsDbSpecialDataGroup_getCount(this.Handle, out returnValue);
      if (count != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(count);
      return returnValue;
    }
  }

  public MCDDbSpecialData GetItemByIndex(uint index)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByIndex = CSWrap.CSNIDTS_DtsDbSpecialDataGroup_getItemByIndex(this.Handle, index, out returnValue);
    if (itemByIndex != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByIndex);
    return (MCDDbSpecialData) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialData);
  }

  public MCDDbSpecialDataGroupCaption Caption
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr caption = CSWrap.CSNIDTS_DtsDbSpecialDataGroup_getCaption(this.Handle, out returnValue);
      if (caption != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(caption);
      return (MCDDbSpecialDataGroupCaption) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroupCaption);
    }
  }

  public bool HasCaption
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbSpecialDataGroup_hasCaption(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string SemanticInformation
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr semanticInformation = CSWrap.CSNIDTS_DtsDbSpecialData_getSemanticInformation(this.Handle, out returnValue);
      if (semanticInformation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(semanticInformation);
      return returnValue.makeString();
    }
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

  public List<MCDDbSpecialData> ToList()
  {
    List<MCDDbSpecialData> list = new List<MCDDbSpecialData>();
    for (uint index = 0; index < this.Count; ++index)
      list.Add(this.GetItemByIndex(index));
    return list;
  }

  public MCDDbSpecialData[] ToArray()
  {
    MCDDbSpecialData[] array = new MCDDbSpecialData[(int) this.Count];
    for (uint index = 0; index < this.Count; ++index)
      array[(int) index] = this.GetItemByIndex(index);
    return array;
  }
}
