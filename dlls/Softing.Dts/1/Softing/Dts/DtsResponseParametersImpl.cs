// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsResponseParametersImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

internal class DtsResponseParametersImpl : 
  MappedObject,
  DtsResponseParameters,
  MCDResponseParameters,
  MCDNamedCollection,
  MCDCollection,
  MCDObject,
  IDisposable,
  IEnumerable,
  DtsNamedCollection,
  DtsCollection,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsResponseParametersImpl(IntPtr handle)
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

  ~DtsResponseParametersImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDResponseParameter GetItemByIndex(uint index)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByIndex = CSWrap.CSNIDTS_DtsResponseParameters_getItemByIndex(this.Handle, index, out returnValue);
    if (itemByIndex != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByIndex);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter GetItemByName(string name)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByName = CSWrap.CSNIDTS_DtsResponseParameters_getItemByName(this.Handle, name, out returnValue);
    if (itemByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByName);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddElement()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addElement(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddElementWithContent(MCDResponseParameter pPattern)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addElementWithContent(this.Handle, DTS_ObjectMapper.getHandle(pPattern as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddMuxBranch(string strBranch)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addMuxBranch(this.Handle, strBranch, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddMuxBranchByIndex(byte Index)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addMuxBranchByIndex(this.Handle, Index, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddMuxBranchByIndexWithContent(
    byte Index,
    MCDResponseParameters pContentList)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addMuxBranchByIndexWithContent(this.Handle, Index, DTS_ObjectMapper.getHandle(pContentList as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddMuxBranchWithContent(
    string strBranch,
    MCDResponseParameters pContentList)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addMuxBranchWithContent(this.Handle, strBranch, DTS_ObjectMapper.getHandle(pContentList as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter SetParameterWithName(string name, MCDValue value)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_setParameterWithName(this.Handle, name, DTS_ObjectMapper.getHandle(value as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddMuxBranchByMuxValue(MCDValue muxValue)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addMuxBranchByMuxValue(this.Handle, DTS_ObjectMapper.getHandle(muxValue as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDResponseParameter AddMuxBranchByMuxValueWithContent(
    MCDValue muxValue,
    MCDResponseParameters pContentList)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addMuxBranchByMuxValueWithContent(this.Handle, DTS_ObjectMapper.getHandle(muxValue as MappedObject), DTS_ObjectMapper.getHandle(pContentList as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public MCDObject Parent
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parent = CSWrap.CSNIDTS_DtsResponseParameters_getParent(this.Handle, out returnValue);
      if (parent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parent);
      return (MCDObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject);
    }
  }

  public MCDResponseParameter AddEnvDataByDTC(uint dtc)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameters_addEnvDataByDTC(this.Handle, dtc, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
  }

  public string[] Names
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr names = CSWrap.CSNIDTS_DtsNamedCollection_getNames(this.Handle, out returnValue);
      if (names != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(names);
      return returnValue.ToStringArray();
    }
  }

  public uint Count
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr count = CSWrap.CSNIDTS_DtsCollection_getCount(this.Handle, out returnValue);
      if (count != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(count);
      return returnValue;
    }
  }

  public MCDObject GetObjectItemByIndex(uint index)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectItemByIndex = CSWrap.CSNIDTS_DtsCollection_getObjectItemByIndex(this.Handle, index, out returnValue);
    if (objectItemByIndex != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectItemByIndex);
    return (MCDObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject);
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

  public List<MCDResponseParameter> ToList()
  {
    List<MCDResponseParameter> list = new List<MCDResponseParameter>();
    for (uint index = 0; index < this.Count; ++index)
      list.Add(this.GetItemByIndex(index));
    return list;
  }

  public MCDResponseParameter[] ToArray()
  {
    MCDResponseParameter[] array = new MCDResponseParameter[(int) this.Count];
    for (uint index = 0; index < this.Count; ++index)
      array[(int) index] = this.GetItemByIndex(index);
    return array;
  }

  public IEnumerator GetEnumerator()
  {
    return (IEnumerator) new DtsResponseParametersImpl.DtsResponseParametersEnumerator(this);
  }

  private class DtsResponseParametersEnumerator : IEnumerator
  {
    private DtsResponseParametersImpl m_classList;
    private int m_index;

    public DtsResponseParametersEnumerator(DtsResponseParametersImpl classList)
    {
      this.m_classList = classList;
      this.m_index = -1;
    }

    public void Reset() => this.m_index = -1;

    public object Current => (object) this.m_classList.GetItemByIndex((uint) this.m_index);

    public bool MoveNext()
    {
      ++this.m_index;
      return (long) this.m_index < (long) this.m_classList.Count;
    }
  }
}
