// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBJob
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System.Collections.Generic;

#nullable disable
namespace McdAbstraction;

public class McdDBJob : McdDBDiagComPrimitive
{
  private MCDDbJob job;
  private Dictionary<string, string> specialData;

  internal McdDBJob(MCDDbJob job)
    : base((MCDDbDiagComPrimitive) job)
  {
    this.job = job;
  }

  public Dictionary<string, string> SpecialData
  {
    get
    {
      if (this.specialData == null)
        this.specialData = McdDBDiagComPrimitive.GetSpecialData(this.job.DbSDGs);
      return this.specialData;
    }
  }

  public bool IsFlashJob => this.job.ObjectType == MCDObjectType.eMCDDBFLASHJOB;
}
