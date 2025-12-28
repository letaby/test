// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsConfigurationRecordsImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

internal class DtsConfigurationRecordsImpl : 
  MappedObject,
  DtsConfigurationRecords,
  MCDConfigurationRecords,
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
  private uint handler_count;
  private uint listener_handle;

  public DtsConfigurationRecordsImpl(IntPtr handle)
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
    if (this.listener_handle != 0U)
    {
      IntPtr Handle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, this.listener_handle);
      if (Handle != IntPtr.Zero)
        CSWrap.CSNIDTS_releaseObject(Handle);
      this.listener_handle = 0U;
    }
    DTS_ObjectMapper.unregisterObject(this.Handle);
    this.Handle = IntPtr.Zero;
  }

  ~DtsConfigurationRecordsImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDConfigurationRecord GetItemByIndex(uint index)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByIndex = CSWrap.CSNIDTS_DtsConfigurationRecords_getItemByIndex(this.Handle, index, out returnValue);
    if (itemByIndex != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByIndex);
    return (MCDConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord);
  }

  public MCDConfigurationRecord GetItemByName(string name)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr itemByName = CSWrap.CSNIDTS_DtsConfigurationRecords_getItemByName(this.Handle, name, out returnValue);
    if (itemByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(itemByName);
    return (MCDConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord);
  }

  public void Remove(MCDConfigurationRecord ConfigurationRecord)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_remove(this.Handle, DTS_ObjectMapper.getHandle(ConfigurationRecord as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveAll()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_removeAll(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveByIndex(uint index)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_removeByIndex(this.Handle, index);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveByName(string name)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_removeByName(this.Handle, name);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDConfigurationRecord AddByConfigurationIDAndDbConfigurationData(
    MCDValue ConfigurationID,
    MCDDbConfigurationData configurationData)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_addByConfigurationIDAndDbConfigurationData(this.Handle, DTS_ObjectMapper.getHandle(ConfigurationID as MappedObject), DTS_ObjectMapper.getHandle(configurationData as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord);
  }

  public MCDConfigurationRecord AddByDbObject(MCDDbConfigurationRecord DbConfigurationRecord)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_addByDbObject(this.Handle, DTS_ObjectMapper.getHandle(DbConfigurationRecord as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord);
  }

  public MCDConfigurationRecord AddByNameAndDbConfigurationData(
    string dbConfigurationRecordName,
    MCDDbConfigurationData dbConfigurationData)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecords_addByNameAndDbConfigurationData(this.Handle, dbConfigurationRecordName, DTS_ObjectMapper.getHandle(dbConfigurationData as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord);
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

  internal event OnConfigurationRecordLoaded __ConfigurationRecordLoaded;

  internal bool _onConfigurationRecordLoaded(MCDConfigurationRecord configurationRecord)
  {
    lock (this)
    {
      if (this.__ConfigurationRecordLoaded != null)
      {
        this.__ConfigurationRecordLoaded((object) this, new ConfigurationRecordLoadedArgs(configurationRecord));
        return true;
      }
    }
    return false;
  }

  public event OnConfigurationRecordLoaded ConfigurationRecordLoaded
  {
    add
    {
      lock (this)
      {
        this.__ConfigurationRecordLoaded += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__ConfigurationRecordLoaded -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public List<MCDConfigurationRecord> ToList()
  {
    List<MCDConfigurationRecord> list = new List<MCDConfigurationRecord>();
    for (uint index = 0; index < this.Count; ++index)
      list.Add(this.GetItemByIndex(index));
    return list;
  }

  public MCDConfigurationRecord[] ToArray()
  {
    MCDConfigurationRecord[] array = new MCDConfigurationRecord[(int) this.Count];
    for (uint index = 0; index < this.Count; ++index)
      array[(int) index] = this.GetItemByIndex(index);
    return array;
  }

  public IEnumerator GetEnumerator()
  {
    return (IEnumerator) new DtsConfigurationRecordsImpl.DtsConfigurationRecordsEnumerator(this);
  }

  private class DtsConfigurationRecordsEnumerator : IEnumerator
  {
    private DtsConfigurationRecordsImpl m_classList;
    private int m_index;

    public DtsConfigurationRecordsEnumerator(DtsConfigurationRecordsImpl classList)
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
