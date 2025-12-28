// Decompiled with JetBrains decompiler
// Type: Vehicle_Applications.SnifferArgs
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using rp1210;

#nullable disable
namespace Vehicle_Applications;

public class SnifferArgs
{
  public DeviceInfo SelectedDevice;
  public bool J1708Sniff;
  public bool J1939Sniff;

  public SnifferArgs(DeviceInfo selectedDevice, bool j1708Sniff, bool j1939Sniff)
  {
    this.SelectedDevice = selectedDevice;
    this.J1708Sniff = j1708Sniff;
    this.J1939Sniff = j1939Sniff;
  }
}
