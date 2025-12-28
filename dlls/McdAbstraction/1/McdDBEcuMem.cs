// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBEcuMem
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBEcuMem
{
  private MCDDbEcuMem ecumem;
  private string[] ecus;
  private IEnumerable<McdDBFlashSession> flashSessions;

  internal McdDBEcuMem(MCDDbEcuMem ecumem)
  {
    this.ecumem = ecumem;
    this.Qualifier = ecumem.ShortName;
    this.Name = ecumem.LongName;
    this.Description = ecumem.Description;
  }

  public string Qualifier { private set; get; }

  public string Name { private set; get; }

  public string Description { private set; get; }

  public IEnumerable<string> Ecus
  {
    get
    {
      if (this.ecus == null)
      {
        this.ecus = this.ecumem.BaseVariants.OfType<MCDDbEcuBaseVariant>().Select<MCDDbEcuBaseVariant, string>((Func<MCDDbEcuBaseVariant, string>) (e => e.ShortName)).ToArray<string>();
        if (!((IEnumerable<string>) this.ecus).Any<string>())
          this.ecus = this.DBFlashSessions.SelectMany<McdDBFlashSession, string>((Func<McdDBFlashSession, IEnumerable<string>>) (session => session.LayerReferences.Select<string, string>((Func<string, string>) (s => ((IEnumerable<string>) s.Split(".".ToCharArray())).Last<string>())))).ToArray<string>();
      }
      return (IEnumerable<string>) this.ecus;
    }
  }

  public IEnumerable<McdDBFlashSession> DBFlashSessions
  {
    get
    {
      if (this.flashSessions == null)
        this.flashSessions = (IEnumerable<McdDBFlashSession>) this.ecumem.FlashSessions.OfType<MCDDbFlashSession>().Select<MCDDbFlashSession, McdDBFlashSession>((Func<MCDDbFlashSession, McdDBFlashSession>) (fs => new McdDBFlashSession(fs))).ToList<McdDBFlashSession>();
      return this.flashSessions;
    }
  }
}
