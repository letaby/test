// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBEnvDataDesc
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBEnvDataDesc
{
  private MCDDbEnvDataDesc env;

  internal McdDBEnvDataDesc(MCDDbEnvDataDesc env)
  {
    this.env = env;
    this.Name = this.env.LongName;
    this.Qualifier = this.env.ShortName;
  }

  public string Name { get; private set; }

  public string Qualifier { get; private set; }

  public IEnumerable<McdDBResponseParameter> CommonEnvironmentalDataSet
  {
    get
    {
      return (IEnumerable<McdDBResponseParameter>) this.env.CommonDbEnvDatas.OfType<MCDDbResponseParameter>().Select<MCDDbResponseParameter, McdDBResponseParameter>((Func<MCDDbResponseParameter, McdDBResponseParameter>) (e => new McdDBResponseParameter(e))).ToList<McdDBResponseParameter>();
    }
  }

  public IEnumerable<McdDBResponseParameter> GetFaultSpecificEnvironmentalDataSet(long code)
  {
    return (IEnumerable<McdDBResponseParameter>) this.env.GetSpecificDbEnvDatasForDiagTroubleCode((uint) code).OfType<MCDDbResponseParameter>().Select<MCDDbResponseParameter, McdDBResponseParameter>((Func<MCDDbResponseParameter, McdDBResponseParameter>) (e => new McdDBResponseParameter(e))).ToList<McdDBResponseParameter>();
  }
}
