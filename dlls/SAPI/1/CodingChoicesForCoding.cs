// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingChoicesForCoding
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System.Collections.Generic;

#nullable disable
namespace SapiLayer1;

public class CodingChoicesForCoding
{
  internal CodingChoicesForCoding(IEnumerable<CodingChoice> choices, Dump coding)
  {
    this.CodingChoices = choices;
    this.Coding = coding;
  }

  public IEnumerable<CodingChoice> CodingChoices { get; private set; }

  public Dump Coding { get; private set; }
}
