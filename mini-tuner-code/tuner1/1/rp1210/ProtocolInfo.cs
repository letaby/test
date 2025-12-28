// Decompiled with JetBrains decompiler
// Type: rp1210.ProtocolInfo
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.Collections.Generic;

#nullable disable
namespace rp1210;

public class ProtocolInfo
{
  public string ProtocolString { get; set; }

  public string ProtocolDescription { get; set; }

  public List<string> ProtocolSpeed { get; set; }

  public string ProtocolParams { get; set; }

  public ProtocolInfo() => this.ProtocolSpeed = new List<string>();
}
