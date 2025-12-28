// Decompiled with JetBrains decompiler
// Type: Vehicle_Applications.MaxxAsyncArgs
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using rp1210;

#nullable disable
namespace Vehicle_Applications;

public class MaxxAsyncArgs
{
  public string FlashFile;
  public MaxxModuleType Module;
  public DeviceInfo SelectedDevice;
  public bool FullFlash;

  public MaxxAsyncArgs(
    MaxxModuleType module,
    string flashFile,
    DeviceInfo selectedDevice,
    bool fullFlash)
  {
    this.FlashFile = flashFile;
    this.Module = module;
    this.SelectedDevice = selectedDevice;
    this.FullFlash = fullFlash;
  }
}
