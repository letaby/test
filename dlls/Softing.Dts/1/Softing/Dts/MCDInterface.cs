// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDInterface
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDInterface : MCDNamedObject, MCDObject, IDisposable
{
  MCDValue GetClampState(uint pinOnInterfaceConnector, string clampName);

  string PDUApiSoftwareName { get; }

  MCDVersion PDUApiSoftwareVersion { get; }

  string VendorName { get; }

  void Connect();

  void Disconnect();

  uint HardwareSerialNumber { get; }

  MCDValues ExecIOCtrl(
    string IOCtrlName,
    MCDValue inputData,
    uint inputDataItemType,
    uint outputDataSize);

  string[] IOControlNames { get; }

  void DetectInterfaces(string optionString);

  MCDDbInterfaceCable CurrentDbInterfaceCable { get; }

  ulong CurrentTime { get; }

  MCDDbInterfaceCables DbInterfaceCables { get; }

  string FirmwareName { get; }

  MCDVersion FirmwareVersion { get; }

  string HardwareName { get; }

  MCDVersion HardwareVersion { get; }

  MCDInterfaceResources InterfaceResources { get; }

  MCDVersion MVCIVersionPart1StandardVersion { get; }

  MCDVersion MVCIVersionPart2StandardVersion { get; }

  MCDInterfaceStatus Status { get; }

  void Reset();

  double GetProgrammingVoltage(uint pinOnInterfaceConnector);

  double BatteryVoltage { get; }

  void SetProgrammingVoltage(uint pinOnInterfaceConnector, double voltage);
}
