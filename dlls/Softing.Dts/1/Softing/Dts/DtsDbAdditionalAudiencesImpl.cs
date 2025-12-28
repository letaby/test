// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbAdditionalAudiencesImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

internal class DtsDbAdditionalAudiencesImpl : 
  MappedObject,
  DtsDbAdditionalAudiences,
  MCDDbAdditionalAudiences,
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

  public DtsDbAdditionalAudiencesImpl(IntPtr handle)
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

  ~DtsDbAdditionalAudiencesImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbAdditionalAudience GetItemByIndex(uint uIndex)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByIndex = CSWrap.CSNIDTS_DtsDbAdditionalAudiences_getItemByIndex(this.Handle, uIndex, out returnValue);
    if (itemByIndex != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByIndex);
    return (MCDDbAdditionalAudience) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudience);
  }

  public MCDDbAdditionalAudience GetItemByName(string name)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByName = CSWrap.CSNIDTS_DtsDbAdditionalAudiences_getItemByName(this.Handle, name, out returnValue);
    if (itemByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByName);
    return (MCDDbAdditionalAudience) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudience);
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

  public List<MCDDbAdditionalAudience> ToList()
  {
    List<MCDDbAdditionalAudience> list = new List<MCDDbAdditionalAudience>();
    for (uint uIndex = 0; uIndex < this.Count; ++uIndex)
      list.Add(this.GetItemByIndex(uIndex));
    return list;
  }

  public MCDDbAdditionalAudience[] ToArray()
  {
    MCDDbAdditionalAudience[] array = new MCDDbAdditionalAudience[(int) this.Count];
    for (uint uIndex = 0; uIndex < this.Count; ++uIndex)
      array[(int) uIndex] = this.GetItemByIndex(uIndex);
    return array;
  }

  public IEnumerator GetEnumerator()
  {
    return (IEnumerator) new DtsDbAdditionalAudiencesImpl.DtsDbAdditionalAudiencesEnumerator(this);
  }

  private class DtsDbAdditionalAudiencesEnumerator : IEnumerator
  {
    private DtsDbAdditionalAudiencesImpl m_classList;
    private int m_index;

    public DtsDbAdditionalAudiencesEnumerator(DtsDbAdditionalAudiencesImpl classList)
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
