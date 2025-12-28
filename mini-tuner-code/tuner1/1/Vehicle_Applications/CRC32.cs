// Decompiled with JetBrains decompiler
// Type: Vehicle_Applications.CRC32
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;

#nullable disable
namespace Vehicle_Applications;

public class CRC32
{
  private uint[] table;

  public uint ComputeChecksum(byte[] bytes)
  {
    uint num = uint.MaxValue;
    for (int index1 = 0; index1 < bytes.Length; ++index1)
    {
      byte index2 = (byte) (num & (uint) byte.MaxValue ^ (uint) bytes[index1]);
      num = num >> 8 ^ this.table[(int) index2];
    }
    return ~num;
  }

  public uint ComputeChecksum(byte[] bytes, int From, int To)
  {
    uint num = uint.MaxValue;
    for (int index1 = From; index1 < To; ++index1)
    {
      byte index2 = (byte) (num & (uint) byte.MaxValue ^ (uint) bytes[index1]);
      num = num >> 8 ^ this.table[(int) index2];
    }
    return ~num;
  }

  public byte[] ComputeChecksumBytes(byte[] bytes)
  {
    return BitConverter.GetBytes(this.ComputeChecksum(bytes));
  }

  public byte[] ComputeChecksumBytes(byte[] bytes, int From, int To)
  {
    return BitConverter.GetBytes(this.ComputeChecksum(bytes, From, To));
  }

  public CRC32()
  {
    uint num1 = 3988292384;
    this.table = new uint[256 /*0x0100*/];
    for (uint index1 = 0; (long) index1 < (long) this.table.Length; ++index1)
    {
      uint num2 = index1;
      for (int index2 = 8; index2 > 0; --index2)
      {
        if (((int) num2 & 1) == 1)
          num2 = num2 >> 1 ^ num1;
        else
          num2 >>= 1;
      }
      this.table[(int) index1] = num2;
    }
  }
}
