// Decompiled with JetBrains decompiler
// Type: rp1210.RP1210BDriverInfo
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.Collections.Generic;

#nullable disable
namespace rp1210;

public class RP1210BDriverInfo
{
  private readonly string[] CANFormats = new string[5]
  {
    "CAN:Baud=X,SampleLocation=Y,SJW=Z,IDSize=S",
    "CAN:Baud=X,PROP_SEG=A,PHASE_SEG1=B,PHASE_SEG2=C,SJW=Z,IDSize=SS",
    "CAN:Baud=X,TSEG1=D,TSEG2=E,SampleTimes=Y,SJW=Z,IDSize=SS",
    "CAN",
    "CAN:Baud=x"
  };
  private readonly string[] J1939Formats = new string[5]
  {
    "J1939:Baud=x",
    "J1939",
    "J1939:Baud=X,SampleLocation=Y,SJW=Z,IDSize=S",
    "J1939:Baud=X,PROP_SEG=A,PHASE_SEG1=B,PHASE_SEG2=C,SJW=Z,IDSize=SS",
    "J1939:Baud=X,TSEG1=D,TSEG2=E,SampleTimes=Y,SJW=Z,IDSize=SS"
  };
  private readonly string[] J1708Formats = new string[2]
  {
    "J1708:Baud=x",
    "J1708"
  };
  private readonly string[] ISO15765Formats = new string[4]
  {
    "ISO15765:Baud=x",
    "ISO15765",
    "ISO15765:Baud=X,PROP_SEG=A,PHASE_SEG1=B,PHASE_SEG2=C,SJW=Z,IDSize=SS",
    "ISO15765:Baud=X,TSEG1=D,TSEG2=E,SampleTimes=Y,SJW=Z,IDSize=SS"
  };
  public string DriverVersion;
  public string RP1210Version;
  public string VendorName;
  public int TimestampWeight;
  public List<short> CANFormatsSupported;
  public List<short> J1939FormatsSupported;
  public List<short> J1708FormatsSupported;
  public List<short> ISO15765FormatsSupported;
  public List<DeviceInfo> RP1210Devices;

  public RP1210BDriverInfo() => this.RP1210Devices = new List<DeviceInfo>();
}
