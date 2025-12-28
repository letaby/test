// Decompiled with JetBrains decompiler
// Type: Vehicle_Applications.FlashAddress
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

#nullable disable
namespace Vehicle_Applications;

public class FlashAddress
{
  public int StartAddr;
  public int EndAddr;

  public FlashAddress(int Start, int End)
  {
    this.StartAddr = Start;
    this.EndAddr = End;
  }
}
