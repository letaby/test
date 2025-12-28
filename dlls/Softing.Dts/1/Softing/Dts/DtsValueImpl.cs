// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsValueImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsValueImpl : MappedObject, DtsValue, MCDValue, MCDObject, IDisposable, DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsValueImpl(IntPtr handle)
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

  ~DtsValueImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public void Clear()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_clear(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool[] Bitfield
  {
    get
    {
      GC.KeepAlive((object) this);
      BitField_Struct returnValue = new BitField_Struct();
      IntPtr bitfield = CSWrap.CSNIDTS_DtsValue_getBitfield(this.Handle, out returnValue);
      if (bitfield != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bitfield);
      return returnValue.ToBoolArray();
    }
    set
    {
      GC.KeepAlive((object) this);
      BitField_Struct _pBitField = new BitField_Struct(value);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setBitfield(this.Handle, ref _pBitField);
      _pBitField.FreeMemory();
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool GetBitfieldValue(uint index)
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr bitfieldValue = CSWrap.CSNIDTS_DtsValue_getBitfieldValue(this.Handle, index, out returnValue);
    if (bitfieldValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(bitfieldValue);
    return returnValue;
  }

  public byte[] Bytefield
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr bytefield = CSWrap.CSNIDTS_DtsValue_getBytefield(this.Handle, out returnValue);
      if (bytefield != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bytefield);
      return returnValue.ToByteArray();
    }
    set
    {
      GC.KeepAlive((object) this);
      ByteField_Struct _pByteField = new ByteField_Struct(value);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setBytefield(this.Handle, ref _pByteField);
      _pByteField.FreeMemory();
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public byte GetBytefieldValue(uint Index)
  {
    GC.KeepAlive((object) this);
    byte returnValue;
    IntPtr bytefieldValue = CSWrap.CSNIDTS_DtsValue_getBytefieldValue(this.Handle, Index, out returnValue);
    if (bytefieldValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(bytefieldValue);
    return returnValue;
  }

  public MCDDataType DataType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr dataType = CSWrap.CSNIDTS_DtsValue_getDataType(this.Handle, out returnValue);
      if (dataType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataType);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setDataType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public float Float32
  {
    get
    {
      GC.KeepAlive((object) this);
      float returnValue;
      IntPtr float32 = CSWrap.CSNIDTS_DtsValue_getFloat32(this.Handle, out returnValue);
      if (float32 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(float32);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setFloat32(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public double Float64
  {
    get
    {
      GC.KeepAlive((object) this);
      double returnValue;
      IntPtr float64 = CSWrap.CSNIDTS_DtsValue_getFloat64(this.Handle, out returnValue);
      if (float64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(float64);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setFloat64(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public short Int16
  {
    get
    {
      GC.KeepAlive((object) this);
      short returnValue;
      IntPtr int16 = CSWrap.CSNIDTS_DtsValue_getInt16(this.Handle, out returnValue);
      if (int16 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(int16);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setInt16(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public int Int32
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr int32 = CSWrap.CSNIDTS_DtsValue_getInt32(this.Handle, out returnValue);
      if (int32 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(int32);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setInt32(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public long Int64
  {
    get
    {
      GC.KeepAlive((object) this);
      long returnValue;
      IntPtr int64 = CSWrap.CSNIDTS_DtsValue_getInt64(this.Handle, out returnValue);
      if (int64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(int64);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setInt64(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public char Int8
  {
    get
    {
      GC.KeepAlive((object) this);
      char returnValue;
      IntPtr int8 = CSWrap.CSNIDTS_DtsValue_getInt8(this.Handle, out returnValue);
      if (int8 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(int8);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setInt8(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public int Length
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr length = CSWrap.CSNIDTS_DtsValue_getLength(this.Handle, out returnValue);
      if (length != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(length);
      return returnValue;
    }
  }

  public string String
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_getString(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue.makeString();
    }
  }

  public ushort Uint16
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr uint16 = CSWrap.CSNIDTS_DtsValue_getUint16(this.Handle, out returnValue);
      if (uint16 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uint16);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setUint16(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint Uint32
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr uint32 = CSWrap.CSNIDTS_DtsValue_getUint32(this.Handle, out returnValue);
      if (uint32 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uint32);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setUint32(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public ulong Uint64
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr uint64 = CSWrap.CSNIDTS_DtsValue_getUint64(this.Handle, out returnValue);
      if (uint64 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uint64);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setUint64(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public byte Uint8
  {
    get
    {
      GC.KeepAlive((object) this);
      byte returnValue;
      IntPtr uint8 = CSWrap.CSNIDTS_DtsValue_getUint8(this.Handle, out returnValue);
      if (uint8 != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uint8);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setUint8(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool IsEmpty
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_isEmpty(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public void SetBitfieldValue(bool value, uint index)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setBitfieldValue(this.Handle, value, index);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void SetBytefieldValue(byte Value, uint Index)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setBytefieldValue(this.Handle, Value, Index);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string ValueAsString
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr valueAsString = CSWrap.CSNIDTS_DtsValue_getValueAsString(this.Handle, out returnValue);
      if (valueAsString != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(valueAsString);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setValueAsString(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool IsValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_isValid(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string Asciistring
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr asciistring = CSWrap.CSNIDTS_DtsValue_getAsciistring(this.Handle, out returnValue);
      if (asciistring != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(asciistring);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setAsciistring(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Unicode2string
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr unicode2string = CSWrap.CSNIDTS_DtsValue_getUnicode2string(this.Handle, out returnValue);
      if (unicode2string != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unicode2string);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setUnicode2string(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool Boolean
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr boolean = CSWrap.CSNIDTS_DtsValue_getBoolean(this.Handle, out returnValue);
      if (boolean != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(boolean);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_setBoolean(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDValue Copy()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsValue_copy(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
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
