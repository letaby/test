// Decompiled with JetBrains decompiler
// Type: rp1210.J1939Message
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace rp1210;

public class J1939Message
{
  public uint TimeStamp { get; set; }

  public short SourceAddress { get; set; }

  public short DestinationAddress { get; set; }

  public byte Priority { get; set; }

  public ushort PGN { get; set; }

  public ushort dataLength { get; set; }

  public byte[] Data { get; set; }

  public J1939Message()
  {
  }

  public J1939Message(
    uint pTimeStamp,
    short pSource,
    short pDestination,
    byte pPriority,
    ushort pPGN,
    ushort pDateLength,
    byte[] pData)
  {
    this.TimeStamp = pTimeStamp;
    this.SourceAddress = pSource;
    this.DestinationAddress = pDestination;
    this.Priority = pPriority;
    this.PGN = pPGN;
    this.dataLength = pDateLength;
    this.Data = pData;
  }

  public static byte[] SerializeMessage<T>(T msg) where T : struct
  {
    int length = Marshal.SizeOf(typeof (T));
    byte[] destination = new byte[length];
    IntPtr num = Marshal.AllocHGlobal(length);
    Marshal.StructureToPtr<T>(msg, num, true);
    Marshal.Copy(num, destination, 0, length);
    Marshal.FreeHGlobal(num);
    return destination;
  }

  public static T DeserializeMsg<T>(byte[] data) where T : struct
  {
    int num1 = Marshal.SizeOf(typeof (T));
    IntPtr num2 = Marshal.AllocHGlobal(num1);
    Marshal.Copy(data, 0, num2, num1);
    T structure = (T) Marshal.PtrToStructure(num2, typeof (T));
    Marshal.FreeHGlobal(num2);
    return structure;
  }

  public byte[] ToArray()
  {
    byte num1 = 0;
    byte[] array = new byte[(int) this.dataLength + 6];
    byte[] numArray1 = array;
    int index1 = (int) num1;
    byte num2 = (byte) (index1 + 1);
    int num3 = (int) (byte) ((uint) this.PGN & (uint) byte.MaxValue);
    numArray1[index1] = (byte) num3;
    byte[] numArray2 = array;
    int index2 = (int) num2;
    byte num4 = (byte) (index2 + 1);
    int num5 = (int) (byte) ((int) this.PGN >> 8 & (int) byte.MaxValue);
    numArray2[index2] = (byte) num5;
    byte[] numArray3 = array;
    int index3 = (int) num4;
    byte num6 = (byte) (index3 + 1);
    int num7 = (int) (byte) ((int) this.PGN >> 16 /*0x10*/ & (int) byte.MaxValue);
    numArray3[index3] = (byte) num7;
    byte[] numArray4 = array;
    int index4 = (int) num6;
    byte num8 = (byte) (index4 + 1);
    numArray4[index4] |= this.Priority;
    byte[] numArray5 = array;
    int index5 = (int) num8;
    byte num9 = (byte) (index5 + 1);
    int sourceAddress = (int) (byte) this.SourceAddress;
    numArray5[index5] = (byte) sourceAddress;
    byte[] numArray6 = array;
    int index6 = (int) num9;
    byte num10 = (byte) (index6 + 1);
    int destinationAddress = (int) (byte) this.DestinationAddress;
    numArray6[index6] = (byte) destinationAddress;
    foreach (byte num11 in this.Data)
      array[(int) num10++] = num11;
    return array;
  }
}
