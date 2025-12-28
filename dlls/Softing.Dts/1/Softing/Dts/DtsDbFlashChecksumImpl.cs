// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashChecksumImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbFlashChecksumImpl : 
  MappedObject,
  DtsDbFlashChecksum,
  MCDDbFlashChecksum,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbFlashChecksumImpl(IntPtr handle)
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

  ~DtsDbFlashChecksumImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string ChecksumAlgorithm
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr checksumAlgorithm = CSWrap.CSNIDTS_DtsDbFlashChecksum_getChecksumAlgorithm(this.Handle, out returnValue);
      if (checksumAlgorithm != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(checksumAlgorithm);
      return returnValue.makeString();
    }
  }

  public MCDValue ChecksumResult
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr checksumResult = CSWrap.CSNIDTS_DtsDbFlashChecksum_getChecksumResult(this.Handle, out returnValue);
      if (checksumResult != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(checksumResult);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public uint UncompressedSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr uncompressedSize = CSWrap.CSNIDTS_DtsDbFlashChecksum_getUncompressedSize(this.Handle, out returnValue);
      if (uncompressedSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uncompressedSize);
      return returnValue;
    }
  }

  public uint CompressedSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr compressedSize = CSWrap.CSNIDTS_DtsDbFlashChecksum_getCompressedSize(this.Handle, out returnValue);
      if (compressedSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compressedSize);
      return returnValue;
    }
  }

  public byte[] FillByte
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr fillByte = CSWrap.CSNIDTS_DtsDbFlashChecksum_getFillByte(this.Handle, out returnValue);
      if (fillByte != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(fillByte);
      return returnValue.ToByteArray();
    }
  }

  public uint SourceStartAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr sourceStartAddress = CSWrap.CSNIDTS_DtsDbFlashChecksum_getSourceStartAddress(this.Handle, out returnValue);
      if (sourceStartAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceStartAddress);
      return returnValue;
    }
  }

  public uint SourceEndAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr sourceEndAddress = CSWrap.CSNIDTS_DtsDbFlashChecksum_getSourceEndAddress(this.Handle, out returnValue);
      if (sourceEndAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceEndAddress);
      return returnValue;
    }
  }

  public ulong UncompressedSize64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr uncompressedSize64 = CSWrap.CSNIDTS_DtsDbFlashChecksum_getUncompressedSize64(this.Handle, out returnValue);
      if (uncompressedSize64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uncompressedSize64);
      return returnValue;
    }
  }

  public ulong SourceStartAddress64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr sourceStartAddress64 = CSWrap.CSNIDTS_DtsDbFlashChecksum_getSourceStartAddress64(this.Handle, out returnValue);
      if (sourceStartAddress64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceStartAddress64);
      return returnValue;
    }
  }

  public ulong SourceEndAddress64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr sourceEndAddress64 = CSWrap.CSNIDTS_DtsDbFlashChecksum_getSourceEndAddress64(this.Handle, out returnValue);
      if (sourceEndAddress64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceEndAddress64);
      return returnValue;
    }
  }

  public ulong CompressedSize64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr compressedSize64 = CSWrap.CSNIDTS_DtsDbFlashChecksum_getCompressedSize64(this.Handle, out returnValue);
      if (compressedSize64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compressedSize64);
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
