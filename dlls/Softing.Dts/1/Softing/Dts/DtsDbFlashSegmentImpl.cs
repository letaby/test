// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashSegmentImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbFlashSegmentImpl : 
  MappedObject,
  DtsDbFlashSegment,
  MCDDbFlashSegment,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbFlashSegmentImpl(IntPtr handle)
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

  ~DtsDbFlashSegmentImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint UncompressedSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr uncompressedSize = CSWrap.CSNIDTS_DtsDbFlashSegment_getUncompressedSize(this.Handle, out returnValue);
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
      IntPtr compressedSize = CSWrap.CSNIDTS_DtsDbFlashSegment_getCompressedSize(this.Handle, out returnValue);
      if (compressedSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compressedSize);
      return returnValue;
    }
  }

  public byte[] BinaryData
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr binaryData = CSWrap.CSNIDTS_DtsDbFlashSegment_getBinaryData(this.Handle, out returnValue);
      if (binaryData != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(binaryData);
      return returnValue.ToByteArray();
    }
  }

  public uint SourceStartAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr sourceStartAddress = CSWrap.CSNIDTS_DtsDbFlashSegment_getSourceStartAddress(this.Handle, out returnValue);
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
      IntPtr sourceEndAddress = CSWrap.CSNIDTS_DtsDbFlashSegment_getSourceEndAddress(this.Handle, out returnValue);
      if (sourceEndAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceEndAddress);
      return returnValue;
    }
  }

  public byte[] GetBinaryDataOffset(uint uOffset, uint uLength)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    IntPtr binaryDataOffset = CSWrap.CSNIDTS_DtsDbFlashSegment_getBinaryDataOffset(this.Handle, uOffset, uLength, out returnValue);
    if (binaryDataOffset != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(binaryDataOffset);
    return returnValue.ToByteArray();
  }

  public byte[] GetFirstBinaryDataChunk(uint size)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    IntPtr firstBinaryDataChunk = CSWrap.CSNIDTS_DtsDbFlashSegment_getFirstBinaryDataChunk(this.Handle, size, out returnValue);
    if (firstBinaryDataChunk != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(firstBinaryDataChunk);
    return returnValue.ToByteArray();
  }

  public bool HasNextBinaryDataChunk
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashSegment_hasNextBinaryDataChunk(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public byte[] NextBinaryDataChunk
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr nextBinaryDataChunk = CSWrap.CSNIDTS_DtsDbFlashSegment_getNextBinaryDataChunk(this.Handle, out returnValue);
      if (nextBinaryDataChunk != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(nextBinaryDataChunk);
      return returnValue.ToByteArray();
    }
  }

  public MCDFlashSegmentIterator CreateFlashSegmentIterator(uint size)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr flashSegmentIterator = CSWrap.CSNIDTS_DtsDbFlashSegment_createFlashSegmentIterator(this.Handle, size, out returnValue);
    if (flashSegmentIterator != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(flashSegmentIterator);
    return (MCDFlashSegmentIterator) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFlashSegmentIterator);
  }

  public void RemoveFlashSegmentIterator(MCDFlashSegmentIterator flashSegmentIterator)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashSegment_removeFlashSegmentIterator(this.Handle, DTS_ObjectMapper.getHandle(flashSegmentIterator as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public ulong SourceEndAddress64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr sourceEndAddress64 = CSWrap.CSNIDTS_DtsDbFlashSegment_getSourceEndAddress64(this.Handle, out returnValue);
      if (sourceEndAddress64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceEndAddress64);
      return returnValue;
    }
  }

  public ulong SourceStartAddress64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr sourceStartAddress64 = CSWrap.CSNIDTS_DtsDbFlashSegment_getSourceStartAddress64(this.Handle, out returnValue);
      if (sourceStartAddress64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceStartAddress64);
      return returnValue;
    }
  }

  public ulong UncompressedSize64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr uncompressedSize64 = CSWrap.CSNIDTS_DtsDbFlashSegment_getUncompressedSize64(this.Handle, out returnValue);
      if (uncompressedSize64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uncompressedSize64);
      return returnValue;
    }
  }

  public ulong CompressedSize64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr compressedSize64 = CSWrap.CSNIDTS_DtsDbFlashSegment_getCompressedSize64(this.Handle, out returnValue);
      if (compressedSize64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compressedSize64);
      return returnValue;
    }
  }

  public byte[] GetBinaryDataOffset64(ulong uOffset, ulong uLength)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    IntPtr binaryDataOffset64 = CSWrap.CSNIDTS_DtsDbFlashSegment_getBinaryDataOffset64(this.Handle, uOffset, uLength, out returnValue);
    if (binaryDataOffset64 != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(binaryDataOffset64);
    return returnValue.ToByteArray();
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
