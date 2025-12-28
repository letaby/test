// Decompiled with JetBrains decompiler
// Type: rp1210.DeviceInfo
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.Collections.Generic;

#nullable disable
namespace rp1210;

public class DeviceInfo
{
  public BaudRate SelectedRate;

  public short DeviceId { get; set; }

  public string DeviceDescription { get; set; }

  public string DeviceName { get; set; }

  public string DeviceParams { get; set; }

  public List<ProtocolInfo> SupportedProtocols { get; set; }

  public DeviceInfo() => this.SupportedProtocols = new List<ProtocolInfo>();

  public override string ToString() => $"{this.DeviceName} {this.DeviceDescription}";
}
