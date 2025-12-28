// Decompiled with JetBrains decompiler
// Type: rp1210.J1587Message
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;

#nullable disable
namespace rp1210;

public class J1587Message
{
  public uint TimeStamp { get; set; }

  public byte Priority { get; set; }

  public byte MID { get; set; }

  public byte PID { get; set; }

  public ushort DataLength { get; set; }

  public byte[] Data { get; set; }

  public byte[] ToArray()
  {
    byte num1 = 0;
    byte[] destinationArray = new byte[(int) this.DataLength + 2];
    byte[] numArray1 = destinationArray;
    int index1 = (int) num1;
    byte num2 = (byte) (index1 + 1);
    int priority = (int) this.Priority;
    numArray1[index1] = (byte) priority;
    byte[] numArray2 = destinationArray;
    int index2 = (int) num2;
    byte num3 = (byte) (index2 + 1);
    int mid = (int) this.MID;
    numArray2[index2] = (byte) mid;
    byte[] numArray3 = destinationArray;
    int index3 = (int) num3;
    byte destinationIndex = (byte) (index3 + 1);
    int pid = (int) this.PID;
    numArray3[index3] = (byte) pid;
    Array.Copy((Array) this.Data, 0, (Array) destinationArray, (int) destinationIndex, (int) this.DataLength);
    return destinationArray;
  }
}
