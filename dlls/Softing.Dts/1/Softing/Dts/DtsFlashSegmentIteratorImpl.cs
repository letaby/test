// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsFlashSegmentIteratorImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsFlashSegmentIteratorImpl : 
  MappedObject,
  DtsFlashSegmentIterator,
  MCDFlashSegmentIterator,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsFlashSegmentIteratorImpl(IntPtr handle)
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

  ~DtsFlashSegmentIteratorImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint BinaryDataChunkSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr binaryDataChunkSize = CSWrap.CSNIDTS_DtsFlashSegmentIterator_getBinaryDataChunkSize(this.Handle, out returnValue);
      if (binaryDataChunkSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(binaryDataChunkSize);
      return returnValue;
    }
  }

  public byte[] FirstBinaryDataChunk
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr firstBinaryDataChunk = CSWrap.CSNIDTS_DtsFlashSegmentIterator_getFirstBinaryDataChunk(this.Handle, out returnValue);
      if (firstBinaryDataChunk != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(firstBinaryDataChunk);
      return returnValue.ToByteArray();
    }
  }

  public byte[] NextBinaryDataChunk
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr nextBinaryDataChunk = CSWrap.CSNIDTS_DtsFlashSegmentIterator_getNextBinaryDataChunk(this.Handle, out returnValue);
      if (nextBinaryDataChunk != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(nextBinaryDataChunk);
      return returnValue.ToByteArray();
    }
  }

  public bool HasNextBinaryDataChunk
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsFlashSegmentIterator_hasNextBinaryDataChunk(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
