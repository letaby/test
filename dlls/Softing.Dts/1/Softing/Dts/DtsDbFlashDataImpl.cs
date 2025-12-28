// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashDataImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbFlashDataImpl : 
  MappedObject,
  DtsDbFlashData,
  MCDDbFlashData,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbFlashDataImpl(IntPtr handle)
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

  ~DtsDbFlashDataImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDFlashDataFormat DataFormat
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDFlashDataFormat returnValue;
      IntPtr dataFormat = CSWrap.CSNIDTS_DtsDbFlashData_getDataFormat(this.Handle, out returnValue);
      if (dataFormat != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataFormat);
      return returnValue;
    }
  }

  public MCDValue EncryptionCompressionMethod
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compressionMethod = CSWrap.CSNIDTS_DtsDbFlashData_getEncryptionCompressionMethod(this.Handle, out returnValue);
      if (compressionMethod != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compressionMethod);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public string DataFileName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr dataFileName = CSWrap.CSNIDTS_DtsDbFlashData_getDataFileName(this.Handle, out returnValue);
      if (dataFileName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataFileName);
      return returnValue.makeString();
    }
  }

  public void ResetTemporaryDataFileName()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashData_resetTemporaryDataFileName(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool IsLateBound
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashData_isLateBound(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string ActiveFileName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr activeFileName = CSWrap.CSNIDTS_DtsDbFlashData_getActiveFileName(this.Handle, out returnValue);
      if (activeFileName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(activeFileName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashData_setActiveFileName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string FileName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr fileName = CSWrap.CSNIDTS_DtsDbFlashData_getFileName(this.Handle, out returnValue);
      if (fileName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(fileName);
      return returnValue.makeString();
    }
  }

  public string TemporaryDataFileName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr temporaryDataFileName = CSWrap.CSNIDTS_DtsDbFlashData_getTemporaryDataFileName(this.Handle, out returnValue);
      if (temporaryDataFileName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(temporaryDataFileName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashData_setTemporaryDataFileName(this.Handle, value);
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
      IntPtr matchingFileNames = CSWrap.CSNIDTS_DtsDbFlashData_getMatchingFileNames(this.Handle, out returnValue);
      if (matchingFileNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(matchingFileNames);
      return returnValue.ToStringArray();
    }
  }

  public string DatabaseFileName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr databaseFileName = CSWrap.CSNIDTS_DtsDbFlashData_getDatabaseFileName(this.Handle, out returnValue);
      if (databaseFileName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseFileName);
      return returnValue.makeString();
    }
  }

  public string LongNameID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longNameId = CSWrap.CSNIDTS_DtsDbObject_getLongNameID(this.Handle, out returnValue);
      if (longNameId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longNameId);
      return returnValue.makeString();
    }
  }

  public string DescriptionID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr descriptionId = CSWrap.CSNIDTS_DtsDbObject_getDescriptionID(this.Handle, out returnValue);
      if (descriptionId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(descriptionId);
      return returnValue.makeString();
    }
  }

  public string UniqueObjectIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectIdentifier = CSWrap.CSNIDTS_DtsDbObject_getUniqueObjectIdentifier(this.Handle, out returnValue);
      if (objectIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectIdentifier);
      return returnValue.makeString();
    }
  }

  public string DatabaseFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr databaseFile = CSWrap.CSNIDTS_DtsDbObject_getDatabaseFile(this.Handle, out returnValue);
      if (databaseFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseFile);
      return returnValue.makeString();
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
