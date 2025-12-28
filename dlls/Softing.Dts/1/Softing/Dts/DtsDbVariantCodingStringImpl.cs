// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbVariantCodingStringImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbVariantCodingStringImpl : 
  MappedObject,
  DtsDbVariantCodingString,
  DtsDbObject,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbVariantCodingStringImpl(IntPtr handle)
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

  ~DtsDbVariantCodingStringImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint AccessLevel
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr accessLevel = CSWrap.CSNIDTS_DtsDbVariantCodingString_getAccessLevel(this.Handle, out returnValue);
      if (accessLevel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessLevel);
      return returnValue;
    }
  }

  public string GetSupplementKey(uint index)
  {
    GC.KeepAlive((object) this);
    String_Struct returnValue = new String_Struct();
    IntPtr supplementKey = CSWrap.CSNIDTS_DtsDbVariantCodingString_getSupplementKey(this.Handle, index, out returnValue);
    if (supplementKey != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(supplementKey);
    return returnValue.makeString();
  }

  public uint PartVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr partVersion = CSWrap.CSNIDTS_DtsDbVariantCodingString_getPartVersion(this.Handle, out returnValue);
      if (partVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(partVersion);
      return returnValue;
    }
  }

  public bool HasPartVersion()
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbVariantCodingString_HasPartVersion(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public string PartNumber
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr partNumber = CSWrap.CSNIDTS_DtsDbVariantCodingString_getPartNumber(this.Handle, out returnValue);
      if (partNumber != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(partNumber);
      return returnValue.makeString();
    }
  }

  public MCDValue DefaultStringValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr defaultStringValue = CSWrap.CSNIDTS_DtsDbVariantCodingString_getDefaultStringValue(this.Handle, out returnValue);
      if (defaultStringValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultStringValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
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
