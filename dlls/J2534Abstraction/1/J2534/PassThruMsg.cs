// Decompiled with JetBrains decompiler
// Type: J2534.PassThruMsg
// Assembly: J2534Abstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: F558D3F4-6D07-4AE0-B148-E7AD8371AFDC
// Assembly location: C:\Users\petra\Downloads\Архив (2)\J2534Abstraction.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace J2534;

public class PassThruMsg
{
  private ProtocolId m_ProtocolId;
  private uint m_RxStatus;
  private uint m_TxFlags;
  private uint m_Timestamp;
  private uint m_DataSize;
  private uint m_ExtraDataIndex;
  private byte[] m_Data;

  internal unsafe PassThruMsg(PASSTHRU_MSG* source)
  {
    this.m_ProtocolId = (ProtocolId) *(int*) source;
    this.m_RxStatus = (uint) *(int*) ((IntPtr) source + 4L);
    this.m_TxFlags = (uint) *(int*) ((IntPtr) source + 8L);
    this.m_Timestamp = (uint) *(int*) ((IntPtr) source + 12L);
    uint length = (uint) *(int*) ((IntPtr) source + 16L /*0x10*/);
    this.m_DataSize = length;
    this.m_ExtraDataIndex = (uint) *(int*) ((IntPtr) source + 20L);
    byte[] numArray = new byte[(int) length];
    this.m_Data = numArray;
    uint index = 0;
    if (0U >= length)
      return;
    uint num = length;
    do
    {
      numArray[(int) index] = *(byte*) ((IntPtr) source + (long) index + 24L);
      ++index;
    }
    while (index < num);
  }

  public PassThruMsg(
    ProtocolId protocolId,
    uint rxStatus,
    uint transmitOptions,
    uint timestamp,
    uint extraDataIndex,
    byte[] data)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    this.m_ProtocolId = protocolId;
    this.m_RxStatus = rxStatus;
    this.m_TxFlags = transmitOptions;
    this.m_Timestamp = timestamp;
    this.m_DataSize = (uint) data.Length;
    this.m_ExtraDataIndex = extraDataIndex;
    this.m_Data = data;
  }

  public PassThruMsg(ProtocolId protocolId, byte[] data)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    this.m_ProtocolId = protocolId;
    this.m_RxStatus = 0U;
    this.m_TxFlags = 0U;
    this.m_Timestamp = 0U;
    this.m_ExtraDataIndex = 0U;
    this.m_DataSize = (uint) data.Length;
    this.m_Data = data;
  }

  internal unsafe PASSTHRU_MSG* Convert()
  {
    PASSTHRU_MSG* passthruMsgPtr1 = (PASSTHRU_MSG*) \u003CModule\u003E.@new(4152UL);
    PASSTHRU_MSG* passthruMsgPtr2;
    if ((IntPtr) passthruMsgPtr1 != IntPtr.Zero)
    {
      // ISSUE: initblk instruction
      __memset((IntPtr) passthruMsgPtr1, 0, 4152);
      passthruMsgPtr2 = passthruMsgPtr1;
    }
    else
      passthruMsgPtr2 = (PASSTHRU_MSG*) 0L;
    *(int*) passthruMsgPtr2 = (int) this.m_ProtocolId;
    *(int*) ((IntPtr) passthruMsgPtr2 + 4L) = (int) this.m_RxStatus;
    *(int*) ((IntPtr) passthruMsgPtr2 + 8L) = (int) this.m_TxFlags;
    *(int*) ((IntPtr) passthruMsgPtr2 + 16L /*0x10*/) = (int) this.m_DataSize;
    *(int*) ((IntPtr) passthruMsgPtr2 + 12L) = (int) this.m_Timestamp;
    *(int*) ((IntPtr) passthruMsgPtr2 + 20L) = (int) this.m_ExtraDataIndex;
    int index = 0;
    byte[] data = this.m_Data;
    if (0 < data.Length)
    {
      PASSTHRU_MSG* passthruMsgPtr3 = (PASSTHRU_MSG*) ((IntPtr) passthruMsgPtr2 + 24L);
      do
      {
        *(sbyte*) passthruMsgPtr3 = (sbyte) data[index];
        ++index;
        ++passthruMsgPtr3;
        data = this.m_Data;
      }
      while (index < data.Length);
    }
    return passthruMsgPtr2;
  }

  public ProtocolId ProtcolId => this.m_ProtocolId;

  public long RXStatus => (long) this.m_RxStatus;

  public long TXFlags => (long) this.m_TxFlags;

  public long Timestamp => (long) this.m_Timestamp;

  public long DataSize => (long) this.m_DataSize;

  public long ExtraDataIndex => (long) this.m_ExtraDataIndex;

  public byte[] GetData() => this.m_Data;

  public bool IsChipState
  {
    [return: MarshalAs(UnmanagedType.U1)] get => (bool) (this.m_RxStatus >> 24 & 1U);
  }

  public bool IsErrorFrame
  {
    [return: MarshalAs(UnmanagedType.U1)] get => (bool) (this.m_RxStatus >> 25 & 1U);
  }

  public bool IsBaudRate
  {
    [return: MarshalAs(UnmanagedType.U1)] get => (bool) (this.m_RxStatus >> 26 & 1U);
  }
}
