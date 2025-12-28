// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashSessionImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbFlashSessionImpl : 
  MappedObject,
  DtsDbFlashSession,
  MCDDbFlashSession,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbFlashSessionImpl(IntPtr handle)
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

  ~DtsDbFlashSessionImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbFlashSessionClasses DbFlashSessionsClasses
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr flashSessionsClasses = CSWrap.CSNIDTS_DtsDbFlashSession_getDbFlashSessionsClasses(this.Handle, out returnValue);
      if (flashSessionsClasses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(flashSessionsClasses);
      return (MCDDbFlashSessionClasses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSessionClasses);
    }
  }

  public string FlashKey
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr flashKey = CSWrap.CSNIDTS_DtsDbFlashSession_getFlashKey(this.Handle, out returnValue);
      if (flashKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(flashKey);
      return returnValue.makeString();
    }
  }

  public MCDDbFlashChecksums Checksums
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr checksums = CSWrap.CSNIDTS_DtsDbFlashSession_getChecksums(this.Handle, out returnValue);
      if (checksums != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(checksums);
      return (MCDDbFlashChecksums) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashChecksums);
    }
  }

  public MCDDbFlashIdents DbExpectedIdents
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbExpectedIdents = CSWrap.CSNIDTS_DtsDbFlashSession_getDbExpectedIdents(this.Handle, out returnValue);
      if (dbExpectedIdents != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbExpectedIdents);
      return (MCDDbFlashIdents) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashIdents);
    }
  }

  public MCDDbFlashSecurities DbSecurities
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSecurities = CSWrap.CSNIDTS_DtsDbFlashSession_getDbSecurities(this.Handle, out returnValue);
      if (dbSecurities != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSecurities);
      return (MCDDbFlashSecurities) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSecurities);
    }
  }

  public MCDDbFlashDataBlocks DbDataBlocks
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDataBlocks = CSWrap.CSNIDTS_DtsDbFlashSession_getDbDataBlocks(this.Handle, out returnValue);
      if (dbDataBlocks != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDataBlocks);
      return (MCDDbFlashDataBlocks) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashDataBlocks);
    }
  }

  public MCDDbFlashJob DbFlashJob
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbFlashJob = CSWrap.CSNIDTS_DtsDbFlashSession_getDbFlashJob(this.Handle, out returnValue);
      if (dbFlashJob != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbFlashJob);
      return (MCDDbFlashJob) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashJob);
    }
  }

  public uint Priority
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr priority = CSWrap.CSNIDTS_DtsDbFlashSession_getPriority(this.Handle, out returnValue);
      if (priority != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(priority);
      return returnValue;
    }
  }

  public bool IsDownload
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashSession_isDownload(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbFlashSession_getDbSDGs(this.Handle, out returnValue);
      if (dbSdGs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSdGs);
      return (MCDDbSpecialDataGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups);
    }
  }

  public MCDDbFlashJob GetDbFlashJobByLocation(MCDDbLocation pDtsDbLocation)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr flashJobByLocation = CSWrap.CSNIDTS_DtsDbFlashSession_getDbFlashJobByLocation(this.Handle, DTS_ObjectMapper.getHandle(pDtsDbLocation as MappedObject), out returnValue);
    if (flashJobByLocation != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(flashJobByLocation);
    return (MCDDbFlashJob) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashJob);
  }

  public string FlashJobName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr flashJobName = CSWrap.CSNIDTS_DtsDbFlashSession_getFlashJobName(this.Handle, out returnValue);
      if (flashJobName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(flashJobName);
      return returnValue.makeString();
    }
  }

  public string[] AllVariantReferences
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr variantReferences = CSWrap.CSNIDTS_DtsDbFlashSession_getAllVariantReferences(this.Handle, out returnValue);
      if (variantReferences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(variantReferences);
      return returnValue.ToStringArray();
    }
  }

  public string[] LayerReferences
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr layerReferences = CSWrap.CSNIDTS_DtsDbFlashSession_getLayerReferences(this.Handle, out returnValue);
      if (layerReferences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(layerReferences);
      return returnValue.ToStringArray();
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
