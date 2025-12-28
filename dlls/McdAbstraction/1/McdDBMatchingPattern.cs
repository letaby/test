// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBMatchingPattern
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBMatchingPattern
{
  private MCDDbMatchingPattern matchingPattern;
  private IEnumerable<McdDBMatchingParameter> matchingParameters;

  internal McdDBMatchingPattern(MCDDbMatchingPattern pattern) => this.matchingPattern = pattern;

  public IEnumerable<McdDBMatchingParameter> DBMatchingParameters
  {
    get
    {
      if (this.matchingParameters == null)
        this.matchingParameters = (IEnumerable<McdDBMatchingParameter>) this.matchingPattern.DbMatchingParameters.OfType<MCDDbMatchingParameter>().Select<MCDDbMatchingParameter, McdDBMatchingParameter>((Func<MCDDbMatchingParameter, McdDBMatchingParameter>) (mp => new McdDBMatchingParameter(mp))).ToList<McdDBMatchingParameter>();
      return this.matchingParameters;
    }
  }
}
