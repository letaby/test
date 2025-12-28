// Decompiled with JetBrains decompiler
// Type: Vehicle_Applications.MaxxModule
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;

#nullable disable
namespace Vehicle_Applications;

public class MaxxModule
{
  private MaxxModuleType ECMModule;
  public ushort DiagPGN = 65501;
  public ushort FlashPGN = 65502;
  public ushort Seed = 48869;
  public byte DestAddr;
  public byte[] MaxxBasePacket = new byte[16 /*0x10*/];
  public ProtocolType Protocol = ProtocolType.J1939;
  public bool HasParameters = true;
  public byte SoftwareIDOffset;
  public MaxxFlashSpace Protective;
  public MaxxFlashSpace ProtectiveContiguous;
  public MaxxFlashSpace Strategy;
  public MaxxFlashSpace StrategyContiguous;
  public MaxxFlashSpace Calibration;
  public MaxxFlashSpace CalibrationContiguous;
  public byte FillValue = byte.MaxValue;

  public static string ErrorString(byte Code)
  {
    switch (Code)
    {
      case 0:
        return "";
      case 1:
        return "Error 1: Undefined Request Command";
      case 2:
        return "Error 2: Password Didn't Match";
      case 3:
        return "Error 3: Engine Running";
      case 4:
        return "Error 4: Vehicle Moving";
      case 5:
        return "Error 5: Programming Error / Memory Failure";
      case 6:
        return "Error 6: Invalid Strategy/Calibration ROM Checksum";
      case 7:
        return "Error 7: Cannot Write A 'Read Only' Field";
      case 8:
        return "Error 8: Programming Mode Timeout";
      case 9:
        return "Error 9: Multiple Faults During Session / Data Block Too Long To Transmit While Not In Programming Mode";
      case 10:
        return "Error 10: Incorrect PP Command / Length of Data Provided is Too Long/Short";
      case 11:
        return "Error 11: Invalid Parameters / Incorrect PP Programming Command";
      case 13:
        return "Error 13: Improper Command Sequence / PP Programming Command Received Outside of PP Programming session";
      case 14:
        return "Error 14: Invalid Password Level Provided";
      case 15:
        return "Error 15: Data Block Too Long To Transmit";
      case 16 /*0x10*/:
        return "Error 16: Engine Operating Mode Not in 'No-Start mode'";
      case 17:
        return "Error 17: Transparent Parameters Data CRC Mismatch";
      case 18:
        return "Error 18: Flash ROM Segment Not Erased Prior to Programming";
      case 20:
        return "Error 20: Programming Message Packet Received Out of Sequence";
      case byte.MaxValue:
        return "Error -1: No Response from ECM";
      default:
        return $"Error {(object) Code}: Unknown Return Code";
    }
  }

  public static ProtocolType MaxxModuleProtocol(MaxxModuleType Input)
  {
    switch (Input)
    {
      case MaxxModuleType.NECM2:
      case MaxxModuleType.IDM2:
        return ProtocolType.J1708;
      default:
        return ProtocolType.J1939;
    }
  }

  public MaxxModule(MaxxModuleType InputType)
  {
    this.ECMModule = InputType;
    this.Protective = new MaxxFlashSpace((byte) 45);
    this.Strategy = new MaxxFlashSpace((byte) 43);
    this.Calibration = new MaxxFlashSpace((byte) 44);
    this.ProtectiveContiguous = new MaxxFlashSpace((byte) 45);
    this.StrategyContiguous = new MaxxFlashSpace((byte) 43);
    this.CalibrationContiguous = new MaxxFlashSpace((byte) 44);
    this.Protocol = MaxxModule.MaxxModuleProtocol(InputType);
    switch (this.ECMModule)
    {
      case MaxxModuleType.EDC17:
        this.Protective.Add(16384 /*0x4000*/, 81920 /*0x014000*/);
        this.Strategy.Add(131072 /*0x020000*/, 2097152 /*0x200000*/);
        this.Calibration.Add(2162688 /*0x210000*/, 3145728 /*0x300000*/);
        this.FillValue = (byte) 0;
        this.Seed = (ushort) 53159;
        break;
      case MaxxModuleType.EDC7:
        this.Protective.Add(65536 /*0x010000*/, 261888);
        this.Strategy.Add(262144 /*0x040000*/, 1703936 /*0x1A0000*/);
        this.Calibration.Add(1835008 /*0x1C0000*/, 2088960);
        this.DestAddr = (byte) 18;
        this.FlashPGN = (ushort) 65511;
        this.DiagPGN = (ushort) 65510;
        this.SoftwareIDOffset = (byte) 18;
        this.Seed = (ushort) 53159;
        break;
      case MaxxModuleType.EIM7:
        this.Strategy.Add(65600 /*0x010040*/, 327664);
        this.Calibration.Add(393280 /*0x060040*/, 524272);
        this.Seed = (ushort) 53159;
        break;
      case MaxxModuleType.S3V8:
      case MaxxModuleType.S4I6:
        this.Strategy.Add(131072 /*0x020000*/, 262080);
        this.Strategy.Add(786432 /*0x0C0000*/, 1769408);
        this.Strategy.Add(4194304 /*0x400000*/, 4718592 /*0x480000*/);
        this.Calibration.Add(8650816, 9174848);
        this.Seed = (ushort) 53159;
        break;
      case MaxxModuleType.S4I6Special:
        this.Strategy.Add(131072 /*0x020000*/, 262080);
        this.Strategy.Add(1310720 /*0x140000*/, 4063168);
        this.Strategy.Add(4194304 /*0x400000*/, 4718592 /*0x480000*/);
        this.Calibration.Add(8650816, 9699136);
        this.Seed = (ushort) 53159;
        break;
      case MaxxModuleType.NECM2:
        this.Strategy.Add(65600 /*0x010040*/, 393200);
        this.Calibration.Add(393280 /*0x060040*/, 524272);
        this.Seed = (ushort) 53159;
        this.DestAddr = (byte) 128 /*0x80*/;
        break;
      case MaxxModuleType.IDM2:
        this.Strategy.Add(32832, 57328);
        this.Strategy.Add(131072 /*0x020000*/, 196592);
        this.Calibration.Add(65600 /*0x010040*/, 131056);
        this.Seed = (ushort) 53159;
        this.DestAddr = (byte) 143;
        this.HasParameters = false;
        break;
      case MaxxModuleType.S8V8:
        this.Protective.Add(131072 /*0x020000*/, 262144 /*0x040000*/);
        this.Strategy.Add(1048576 /*0x100000*/, 3014656 /*0x2E0000*/);
        this.Calibration.Add(262400 /*0x040100*/, 1032192 /*0x0FC000*/);
        this.Seed = (ushort) 53159;
        break;
      default:
        throw new Exception("Module Type Not Implemented");
    }
  }
}
