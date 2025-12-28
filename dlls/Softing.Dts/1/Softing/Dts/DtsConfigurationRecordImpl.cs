// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsConfigurationRecordImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsConfigurationRecordImpl : 
  MappedObject,
  DtsConfigurationRecord,
  MCDConfigurationRecord,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsConfigurationRecordImpl(IntPtr handle)
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

  ~DtsConfigurationRecordImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDConfigurationIdItem ConfigurationIdItem
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationIdItem = CSWrap.CSNIDTS_DtsConfigurationRecord_getConfigurationIdItem(this.Handle, out returnValue);
      if (configurationIdItem != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationIdItem);
      return (MCDConfigurationIdItem) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationIdItem);
    }
  }

  public MCDDbConfigurationRecord DbObject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbObject = CSWrap.CSNIDTS_DtsConfigurationRecord_getDbObject(this.Handle, out returnValue);
      if (dbObject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbObject);
      return (MCDDbConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbConfigurationRecord);
    }
  }

  public MCDError Error
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr error = CSWrap.CSNIDTS_DtsConfigurationRecord_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
    }
  }

  public MCDErrors Errors
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr errors = CSWrap.CSNIDTS_DtsConfigurationRecord_getErrors(this.Handle, out returnValue);
      if (errors != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(errors);
      return (MCDErrors) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsErrors);
    }
  }

  public MCDOptionItems OptionItems
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr optionItems = CSWrap.CSNIDTS_DtsConfigurationRecord_getOptionItems(this.Handle, out returnValue);
      if (optionItems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(optionItems);
      return (MCDOptionItems) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsOptionItems);
    }
  }

  public MCDReadDiagComPrimitives ReadDiagComPrimitives
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr diagComPrimitives = CSWrap.CSNIDTS_DtsConfigurationRecord_getReadDiagComPrimitives(this.Handle, out returnValue);
      if (diagComPrimitives != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(diagComPrimitives);
      return (MCDReadDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsReadDiagComPrimitives);
    }
  }

  public MCDSystemItems SystemItems
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr systemItems = CSWrap.CSNIDTS_DtsConfigurationRecord_getSystemItems(this.Handle, out returnValue);
      if (systemItems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(systemItems);
      return (MCDSystemItems) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsSystemItems);
    }
  }

  public MCDWriteDiagComPrimitives WriteDiagComPrimitives
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr diagComPrimitives = CSWrap.CSNIDTS_DtsConfigurationRecord_getWriteDiagComPrimitives(this.Handle, out returnValue);
      if (diagComPrimitives != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(diagComPrimitives);
      return (MCDWriteDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsWriteDiagComPrimitives);
    }
  }

  public bool HasError
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecord_hasError(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public void LoadCodingData(string filename)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecord_loadCodingData(this.Handle, filename);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDataIdItem DataIdItem
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dataIdItem = CSWrap.CSNIDTS_DtsConfigurationRecord_getDataIdItem(this.Handle, out returnValue);
      if (dataIdItem != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataIdItem);
      return (MCDDataIdItem) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDataIdItem);
    }
  }

  public string ActiveFileName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr activeFileName = CSWrap.CSNIDTS_DtsConfigurationRecord_getActiveFileName(this.Handle, out returnValue);
      if (activeFileName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(activeFileName);
      return returnValue.makeString();
    }
  }

  public byte[] ConfigurationRecord
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr configurationRecord = CSWrap.CSNIDTS_DtsConfigurationRecord_getConfigurationRecord(this.Handle, out returnValue);
      if (configurationRecord != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationRecord);
      return returnValue.ToByteArray();
    }
    set
    {
      GC.KeepAlive((object) this);
      ByteField_Struct _configRecordValue = new ByteField_Struct(value);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecord_setConfigurationRecord(this.Handle, ref _configRecordValue);
      _configRecordValue.FreeMemory();
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string[] MatchingFileNames
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr matchingFileNames = CSWrap.CSNIDTS_DtsConfigurationRecord_getMatchingFileNames(this.Handle, out returnValue);
      if (matchingFileNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(matchingFileNames);
      return returnValue.ToStringArray();
    }
  }

  public void RemoveReadDiagComPrimitives(MCDReadDiagComPrimitives readDiagComs)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecord_removeReadDiagComPrimitives(this.Handle, DTS_ObjectMapper.getHandle(readDiagComs as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveWriteDiagComPrimitives(MCDWriteDiagComPrimitives writeDiagComs)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecord_removeWriteDiagComPrimitives(this.Handle, DTS_ObjectMapper.getHandle(writeDiagComs as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbDataRecord ConfigurationRecordByDbObject
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsConfigurationRecord_setConfigurationRecordByDbObject(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsNamedObject_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsNamedObject_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
  }

  public string LongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longName = CSWrap.CSNIDTS_DtsNamedObject_getLongName(this.Handle, out returnValue);
      if (longName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longName);
      return returnValue.makeString();
    }
  }

  public uint StringID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr stringId = CSWrap.CSNIDTS_DtsNamedObject_getStringID(this.Handle, out returnValue);
      if (stringId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(stringId);
      return returnValue;
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
}
