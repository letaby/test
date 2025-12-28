// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ConnectionResource
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ConnectionResource
{
  private readonly int? sourceAddress;
  private Ecu ecu;
  private EcuInterface ecuInterface;
  private string mcdInterfaceQualifier;
  private string mcdInterfaceResourceQualifier;
  private string type;
  private int portIndex;
  private string hardwareName;
  private bool restricted;
  private uint actualBaudRate;
  private bool isPassthru;

  internal string MCDInterfaceQualifier => this.mcdInterfaceQualifier;

  internal string MCDResourceQualifier => this.mcdInterfaceResourceQualifier;

  internal ConnectionResource(
    Ecu ecu,
    EcuInterface ecuInterface,
    McdInterface theInterface,
    McdInterfaceResource theResource,
    int portIndex)
  {
    this.ecu = ecu;
    this.hardwareName = theInterface.HardwareName;
    this.type = theResource.PhysicalInterfaceLinkType;
    this.portIndex = portIndex;
    this.restricted = false;
    this.ecuInterface = ecuInterface;
    this.mcdInterfaceQualifier = theInterface.Qualifier;
    this.mcdInterfaceResourceQualifier = theResource.Qualifier;
    this.IsEthernet = theResource.IsEthernet;
    this.isPassthru = theResource.IsPassThru;
    if (!this.IsEthernet && (!this.isPassthru || RollCallJ1939.GlobalInstance == null || !RollCallJ1939.GlobalInstance.IsAutoBaudRate || !Sapi.GetSapi().AllowAutoBaudRate))
    {
      object obj = this.ecuInterface?.PrioritizedComParameterValue("CP_Baudrate");
      if (obj != null)
        this.actualBaudRate = Convert.ToUInt32(obj, (IFormatProvider) CultureInfo.InvariantCulture);
    }
    if (this.ecuInterface != null && (!McdRoot.LocationPriority.Contains(this.ecuInterface.ProtocolName) || McdRoot.LocationRestricted.Contains(this.ecuInterface.ProtocolName)))
      this.restricted = true;
    else
      this.restricted = this.ecu != null && !this.ecu.PassesConnectionResourceFilter(this);
  }

  internal ConnectionResource(Ecu ecu, CaesarResource resource, byte? sourceAddress)
  {
    this.ecu = ecu;
    this.type = resource.Name;
    this.portIndex = (int) resource.Number;
    this.hardwareName = resource.HardwareName;
    this.isPassthru = resource.IsPassThru;
    byte? nullable = sourceAddress;
    this.sourceAddress = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
    this.ecuInterface = this.ecu.Interfaces[resource.EcuInterface.Qualifier];
    if (!this.isPassthru || RollCallJ1939.GlobalInstance == null || !RollCallJ1939.GlobalInstance.IsAutoBaudRate || !Sapi.GetSapi().AllowAutoBaudRate)
    {
      object obj = this.ecuInterface.PrioritizedComParameterValue("CP_BAUDRATE");
      if (obj != null)
        this.actualBaudRate = Convert.ToUInt32(obj, (IFormatProvider) CultureInfo.InvariantCulture);
    }
    this.restricted = !this.ecu.PassesConnectionResourceFilter(this);
  }

  internal ConnectionResource(
    DiagnosisProtocol protocol,
    CaesarResource resource,
    byte sourceAddress,
    uint desiredBaudRate)
  {
    this.ecu = new Ecu($"{protocol.Name}-{sourceAddress.ToString((IFormatProvider) CultureInfo.InvariantCulture)}", sourceAddress, protocol);
    this.type = resource.Name;
    this.portIndex = (int) resource.Number;
    this.hardwareName = resource.HardwareName;
    this.isPassthru = resource.IsPassThru;
    this.sourceAddress = new int?((int) sourceAddress);
    this.actualBaudRate = desiredBaudRate;
  }

  internal ConnectionResource(
    DiagnosisProtocol protocol,
    McdInterface theInterface,
    McdInterfaceResource theResource,
    int portIndex,
    byte sourceAddress,
    uint desiredBaudRate)
  {
    this.ecu = new Ecu($"{protocol.Name}-{sourceAddress.ToString((IFormatProvider) CultureInfo.InvariantCulture)}", sourceAddress, protocol);
    this.type = theResource.PhysicalInterfaceLinkType;
    this.portIndex = portIndex;
    this.hardwareName = theInterface.HardwareName;
    this.sourceAddress = new int?((int) sourceAddress);
    this.actualBaudRate = desiredBaudRate;
    this.mcdInterfaceQualifier = theInterface.Qualifier;
    this.mcdInterfaceResourceQualifier = theResource.Qualifier;
    this.IsEthernet = theResource.IsEthernet;
  }

  internal ConnectionResource(
    Ecu ecu,
    Protocol protocol,
    string deviceName,
    uint baudRate,
    int portIndex,
    int sourceAddress)
  {
    this.ecu = ecu;
    this.restricted = true;
    this.actualBaudRate = baudRate;
    this.hardwareName = deviceName;
    this.type = protocol.ToString();
    this.portIndex = portIndex;
    this.sourceAddress = new int?(sourceAddress);
    this.isPassthru = true;
  }

  private ConnectionResource(
    Ecu ecu,
    string type,
    string hardwareName,
    int portIndex,
    uint? baudRate,
    byte? sourceAddress = null,
    string interfaceQualifier = null)
  {
    this.ecu = ecu;
    this.restricted = true;
    this.type = type;
    this.hardwareName = hardwareName;
    this.portIndex = portIndex;
    if (baudRate.HasValue)
      this.actualBaudRate = baudRate.Value;
    byte? nullable = sourceAddress;
    this.sourceAddress = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
    if (!string.IsNullOrEmpty(interfaceQualifier))
      this.ecuInterface = ecu.Interfaces[interfaceQualifier];
    this.IsEthernet = type == "ETHERNET" || type == "DoIP";
  }

  public static ConnectionResource FromString(Ecu ecu, string content)
  {
    if (content.Contains("NEXIQ WVL2 Dev:"))
      content = content.Replace("Dev:", "Dev ");
    string[] strArray = content.Split(":".ToCharArray(), StringSplitOptions.None);
    if (strArray.Length < 4)
      return (ConnectionResource) null;
    int result1 = 0;
    int portIndex = 0;
    if (int.TryParse(strArray[2], out result1))
      portIndex = result1;
    else
      Sapi.GetSapi().RaiseDebugInfoEvent((object) ecu, "Unable to parse the hardware index information");
    string interfaceQualifier = (string) null;
    byte? sourceAddress = new byte?();
    if (strArray.Length > 4)
    {
      byte result2;
      if (byte.TryParse(strArray[4], out result2))
        sourceAddress = new byte?(result2);
      if (strArray.Length > 5)
        interfaceQualifier = strArray[5];
    }
    uint result3;
    return uint.TryParse(strArray[3], out result3) ? new ConnectionResource(ecu, strArray[0], strArray[1], portIndex, new uint?(result3), sourceAddress, interfaceQualifier) : new ConnectionResource(ecu, strArray[0], strArray[1], portIndex, new uint?(), sourceAddress, interfaceQualifier);
  }

  public string Type => this.type;

  public int PortIndex => this.portIndex;

  public string HardwareName => this.hardwareName;

  public bool IsPassThru => this.isPassthru;

  public Ecu Ecu => this.ecu;

  public EcuInterface Interface => this.ecuInterface;

  public bool Restricted
  {
    get => this.restricted;
    internal set => this.restricted = value;
  }

  public int BaudRate => (int) this.actualBaudRate;

  internal byte? SourceAddress
  {
    get => !this.sourceAddress.HasValue ? new byte?() : new byte?((byte) this.sourceAddress.Value);
  }

  internal int? SourceAddressLong => this.sourceAddress;

  public string Identifier
  {
    get
    {
      return !this.SourceAddress.HasValue ? this.Ecu.Identifier : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) this.Ecu.ProtocolName, (object) this.SourceAddress);
    }
  }

  public override string ToString()
  {
    return string.Join<object>(":", ((IEnumerable<object>) new object[6]
    {
      (object) this.type,
      (object) this.hardwareName.Replace(":", " "),
      (object) this.portIndex,
      (object) this.actualBaudRate,
      (object) this.sourceAddress,
      this.ecuInterface == null || this.ecuInterface.Ecu.DiagnosisSource != DiagnosisSource.McdDatabase ? (object) (string) null : (object) this.ecuInterface.Qualifier
    }).Select<object, object>((Func<object, object>) (o => o == null ? (object) string.Empty : o)));
  }

  internal bool IsEquivalent(ConnectionResource other)
  {
    bool flag = this.type == other.type && this.hardwareName == other.hardwareName && this.portIndex == other.portIndex && (int) this.actualBaudRate == (int) other.actualBaudRate;
    if (!flag && this.ecu != null && other.ecu != null && this.ecu.DiagnosisSource != other.ecu.DiagnosisSource)
    {
      ConnectionResource connectionResource1 = ((IEnumerable<ConnectionResource>) new ConnectionResource[2]
      {
        this,
        other
      }).FirstOrDefault<ConnectionResource>((Func<ConnectionResource, bool>) (cr => cr.ecu.DiagnosisSource == DiagnosisSource.CaesarApi1 || cr.ecu.DiagnosisSource == DiagnosisSource.CaesarDatabase));
      ConnectionResource connectionResource2 = ((IEnumerable<ConnectionResource>) new ConnectionResource[2]
      {
        this,
        other
      }).FirstOrDefault<ConnectionResource>((Func<ConnectionResource, bool>) (cr => cr.ecu.DiagnosisSource == DiagnosisSource.McdApi1 || cr.ecu.DiagnosisSource == DiagnosisSource.McdDatabase));
      if (connectionResource1 != null && connectionResource2 != null && connectionResource1.type.StartsWith(connectionResource2.type, StringComparison.Ordinal) && connectionResource2.hardwareName.Equals(connectionResource1.hardwareName + " (PDU-API)", StringComparison.Ordinal))
        flag = this.portIndex == other.portIndex && (int) this.actualBaudRate == (int) other.actualBaudRate;
    }
    return flag;
  }

  public bool IsEthernet { get; }

  internal IEnumerable<DictionaryEntry> EcuInfoComParameters
  {
    get
    {
      foreach (DictionaryEntry infoComParameter in this.Ecu.EcuInfoComParameters)
        yield return infoComParameter;
      if (this.Interface != null)
      {
        foreach (DictionaryEntry infoComParameter in this.Interface.EcuInfoComParameters)
          yield return infoComParameter;
      }
    }
  }

  public override int GetHashCode() => this.ToString().GetHashCode();

  public override bool Equals(object obj)
  {
    return obj != null && string.Equals(this.ToString(), obj.ToString());
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("ActualBaudRate is deprecated due to non-CLS compliance, please use BaudRate instead.")]
  public uint ActualBaudRate => this.actualBaudRate;
}
