// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbPhysicalDimensionImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbPhysicalDimensionImpl : 
  MappedObject,
  DtsDbPhysicalDimension,
  MCDDbPhysicalDimension,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbPhysicalDimensionImpl(IntPtr handle)
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

  ~DtsDbPhysicalDimensionImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public int CurrentExponent
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr currentExponent = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getCurrentExponent(this.Handle, out returnValue);
      if (currentExponent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(currentExponent);
      return returnValue;
    }
  }

  public int LengthExponent
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr lengthExponent = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getLengthExponent(this.Handle, out returnValue);
      if (lengthExponent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lengthExponent);
      return returnValue;
    }
  }

  public int LuminousIntensity
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr luminousIntensity = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getLuminousIntensity(this.Handle, out returnValue);
      if (luminousIntensity != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(luminousIntensity);
      return returnValue;
    }
  }

  public int MassExponent
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr massExponent = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getMassExponent(this.Handle, out returnValue);
      if (massExponent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(massExponent);
      return returnValue;
    }
  }

  public int MolarAmountExponent
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr molarAmountExponent = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getMolarAmountExponent(this.Handle, out returnValue);
      if (molarAmountExponent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(molarAmountExponent);
      return returnValue;
    }
  }

  public int TemperatureExponent
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr temperatureExponent = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getTemperatureExponent(this.Handle, out returnValue);
      if (temperatureExponent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(temperatureExponent);
      return returnValue;
    }
  }

  public int TimeExponent
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr timeExponent = CSWrap.CSNIDTS_DtsDbPhysicalDimension_getTimeExponent(this.Handle, out returnValue);
      if (timeExponent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(timeExponent);
      return returnValue;
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
