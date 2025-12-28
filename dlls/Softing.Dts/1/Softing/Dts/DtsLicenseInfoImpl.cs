// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsLicenseInfoImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsLicenseInfoImpl : MappedObject, DtsLicenseInfo, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsLicenseInfoImpl(IntPtr handle)
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

  ~DtsLicenseInfoImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string HardwareName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr hardwareName = CSWrap.CSNIDTS_DtsLicenseInfo_getHardwareName(this.Handle, out returnValue);
      if (hardwareName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(hardwareName);
      return returnValue.makeString();
    }
  }

  public string[] Licenses
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr licenses = CSWrap.CSNIDTS_DtsLicenseInfo_getLicenses(this.Handle, out returnValue);
      if (licenses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(licenses);
      return returnValue.ToStringArray();
    }
  }

  public string LendingTime
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr lendingTime = CSWrap.CSNIDTS_DtsLicenseInfo_getLendingTime(this.Handle, out returnValue);
      if (lendingTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lendingTime);
      return returnValue.makeString();
    }
  }

  public string HardwareType
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr hardwareType = CSWrap.CSNIDTS_DtsLicenseInfo_getHardwareType(this.Handle, out returnValue);
      if (hardwareType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(hardwareType);
      return returnValue.makeString();
    }
  }

  public string HardwareSerial
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr hardwareSerial = CSWrap.CSNIDTS_DtsLicenseInfo_getHardwareSerial(this.Handle, out returnValue);
      if (hardwareSerial != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(hardwareSerial);
      return returnValue.makeString();
    }
  }

  public string[] Products
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr products = CSWrap.CSNIDTS_DtsLicenseInfo_getProducts(this.Handle, out returnValue);
      if (products != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(products);
      return returnValue.ToStringArray();
    }
  }

  public uint DatabaseEncryptionCount
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr databaseEncryptionCount = CSWrap.CSNIDTS_DtsLicenseInfo_getDatabaseEncryptionCount(this.Handle, out returnValue);
      if (databaseEncryptionCount != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseEncryptionCount);
      return returnValue;
    }
  }

  public uint GetDatabaseEncryption(uint index)
  {
    GC.KeepAlive((object) this);
    uint returnValue;
    IntPtr databaseEncryption = CSWrap.CSNIDTS_DtsLicenseInfo_getDatabaseEncryption(this.Handle, index, out returnValue);
    if (databaseEncryption != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(databaseEncryption);
    return returnValue;
  }

  public string GetDatabaseEncryptionName(uint index)
  {
    GC.KeepAlive((object) this);
    String_Struct returnValue = new String_Struct();
    IntPtr databaseEncryptionName = CSWrap.CSNIDTS_DtsLicenseInfo_getDatabaseEncryptionName(this.Handle, index, out returnValue);
    if (databaseEncryptionName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(databaseEncryptionName);
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
