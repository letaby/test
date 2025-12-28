// Decompiled with JetBrains decompiler
// Type: Vehicle_Applications.MaxxFlashSpace
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.Collections.Generic;

#nullable disable
namespace Vehicle_Applications;

public class MaxxFlashSpace
{
  public List<FlashAddress> FlashSect;
  public ushort BaseCount;
  public byte SectionID;

  public MaxxFlashSpace(byte InputSection)
  {
    this.FlashSect = new List<FlashAddress>();
    this.SectionID = InputSection;
  }

  public void Add(int StartAddr, int EndAddr)
  {
    this.FlashSect.Add(new FlashAddress(StartAddr, EndAddr));
  }

  public void Clear() => this.FlashSect.Clear();
}
