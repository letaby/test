// Decompiled with JetBrains decompiler
// Type: SapiLayer1.BusMonitorFrame
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using J2534;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace SapiLayer1;

public sealed class BusMonitorFrame
{
  private static Regex mvciRegex = new Regex("^([A-Z]{2}) (\\d+) (([.|*])([0-9A-F]+) \\d+ )?(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static Regex dtsRegex = new Regex("(\\d+)\\s+([0-9A-F]+)\\s+(\\d+)\\s+([0-9A-F]+)?(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static Regex vectorRegex = new Regex("\\s*(\\d*\\,*\\d+\\.\\d+) (\\d)\\s+([0-9A-F]+)x\\s+[RT]x\\s+d (\\d) ([0-9A-F\\s]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static Regex vectorErrorFrameRegex = new Regex("\\s*(\\d*\\,*\\d+\\.\\d+) (\\d)\\s+ErrorFrame", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static Regex vectorChipstateRegex = new Regex("\\s*(\\d*\\,*\\d+\\.\\d+)\\s*CAN\\s*(\\d)\\s*Status:chip status\\s*(.*)\\s*-\\s*TxErr:\\s*(\\d+)\\s*RxErr:\\s*(\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static object addressEcuMapLock = new object();
  private static Dictionary<long, Tuple<Ecu[], ByteMessageDirection>> addressEcuMap;
  private static Dictionary<Tuple<uint, uint>, Ecu[]> ethernetAddressEcuMap;
  private static DiagnosisProtocol udsProtocol;
  private static RollCallJ1939.PGN[] IdentificationPGNs = new RollCallJ1939.PGN[5]
  {
    RollCallJ1939.PGN.AddressClaim,
    RollCallJ1939.PGN.ComponentIdentification,
    RollCallJ1939.PGN.SoftwareIdentification,
    RollCallJ1939.PGN.VehicleIdentification,
    RollCallJ1939.PGN.EcuIdentificationInformation
  };
  private int channel;
  private long timestamp;
  private long monitorAddress;
  private byte[] data;
  private BusMonitorFrame.CustomFrameTypes customFrameType;
  private bool isEthernet;

  private static uint ToUInt32(byte[] data, int offset)
  {
    return (uint) (((int) data[offset] << 24) + ((int) data[offset + 1] << 16 /*0x10*/) + ((int) data[offset + 2] << 8)) + (uint) data[offset + 3];
  }

  private static ushort ToUInt16(byte[] data, int offset)
  {
    return (ushort) ((uint) (ushort) ((uint) data[offset] << 8) + (uint) data[offset + 1]);
  }

  public static BusMonitorFrame FromBinary(byte[] data, long objectType, long timestamp)
  {
    switch (objectType)
    {
      case 1:
      case 86:
        ushort uint16_1 = BitConverter.ToUInt16(data, 0);
        int num1 = (int) data[2];
        byte length = data[3];
        uint monitorAddress = BitConverter.ToUInt32(data, 4) & 536870911U /*0x1FFFFFFF*/;
        byte[] numArray = new byte[(int) length];
        Array.Copy((Array) data, 8, (Array) numArray, 0, (int) length);
        return new BusMonitorFrame(timestamp, (int) uint16_1, (long) monitorAddress, numArray);
      case 31 /*0x1F*/:
        ushort uint16_2 = BitConverter.ToUInt16(data, 0);
        byte num2 = data[2];
        byte num3 = data[3];
        uint uint32 = BitConverter.ToUInt32(data, 4);
        byte num4 = (byte) uint32;
        switch (uint32)
        {
          case 103:
            num4 = (byte) 2;
            break;
          case 104:
            num4 = (byte) 8;
            break;
          case 115:
            num4 = (byte) 4;
            break;
        }
        return new BusMonitorFrame(timestamp, (int) uint16_2, BusMonitorFrame.CustomFrameTypes.ChipState, new byte[3]
        {
          num4,
          num2,
          num3
        });
      case 73:
        ushort uint16_3 = BitConverter.ToUInt16(data, 0);
        return new BusMonitorFrame(timestamp, (int) uint16_3, BusMonitorFrame.CustomFrameTypes.ErrorFrame, new byte[0]);
      default:
        throw new InvalidOperationException("Unknown object type");
    }
  }

  public static IEnumerable<BusMonitorFrame> FromBinary(byte[] packet, long timestamp)
  {
    ushort uint16_1 = BusMonitorFrame.ToUInt16(packet, 12);
    int offset = 0;
    byte num = 0;
    switch (uint16_1)
    {
      case 2048 /*0x0800*/:
        num = packet[23];
        offset = 34;
        break;
      case 34525:
        num = packet[20];
        offset = 54;
        break;
    }
    if (offset != 0)
    {
      int uint16_2 = (int) BusMonitorFrame.ToUInt16(packet, offset);
      ushort uint16_3 = BusMonitorFrame.ToUInt16(packet, offset + 2);
      if (uint16_2 == 13400 || uint16_3 == (ushort) 13400)
      {
        int doipStartPos = 0;
        switch (num)
        {
          case 6:
            doipStartPos = offset + 20;
            break;
          case 17:
            doipStartPos = offset + 8;
            break;
        }
        if (doipStartPos != 0)
        {
          foreach (BusMonitorFrame busMonitorFrame in BusMonitorFrame.SplitDoIP(packet, doipStartPos, timestamp))
            yield return busMonitorFrame;
        }
      }
    }
  }

  private static IEnumerable<BusMonitorFrame> SplitDoIP(
    byte[] packet,
    int doipStartPos,
    long timestamp)
  {
    while (doipStartPos < packet.Length && (int) packet[doipStartPos + 1] == (int) byte.MaxValue - (int) packet[doipStartPos])
    {
      if (packet.Length >= doipStartPos + 4 + 4)
      {
        uint num1 = BusMonitorFrame.ToUInt32(packet, doipStartPos + 4);
        uint num2 = (uint) (packet.Length - 8 - doipStartPos);
        if (num1 > num2)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) nameof (BusMonitorFrame), $"Error! DoIP data at {(object) timestamp} specifies length {(object) num1} greater than remaining data ({(object) num2}). Content wll be truncated.");
          num1 = num2;
        }
        byte[] doipFrame = new byte[(int) num1 + 8];
        Array.Copy((Array) packet, doipStartPos, (Array) doipFrame, 0, doipFrame.Length);
        yield return new BusMonitorFrame(timestamp, doipFrame);
        doipStartPos += doipFrame.Length;
        doipFrame = (byte[]) null;
      }
      else
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) nameof (BusMonitorFrame), $"Error! DoIP data at {(object) timestamp} is truncated such that the DoIP length cannot be determined. Available length for this frame was {(object) (packet.Length - doipStartPos)}");
        break;
      }
    }
  }

  internal static IEnumerable<BusMonitorFrame> FromDoIPString(string packet)
  {
    Match match = BusMonitorFrame.mvciRegex.Match(packet);
    if (match.Success)
    {
      long timestamp = long.Parse(match.Groups[2].Value, (IFormatProvider) CultureInfo.InvariantCulture);
      foreach (BusMonitorFrame busMonitorFrame in BusMonitorFrame.SplitDoIP(BusMonitorFrame.StringToByteArrayFastest(match.Groups[6].Value), 0, timestamp))
        yield return busMonitorFrame;
    }
  }

  public static BusMonitorFrame FromString(
    string frame,
    BusMonitorFrameStringFormat format,
    int? channel)
  {
    switch (format)
    {
      case BusMonitorFrameStringFormat.Mcd:
        Match match1 = BusMonitorFrame.mvciRegex.Match(frame);
        if (match1.Success && match1.Groups[1].Value == "RX")
        {
          long timestamp = long.Parse(match1.Groups[2].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          long num = long.Parse(match1.Groups[5].Value, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
          int channel1 = channel.Value;
          long monitorAddress = num;
          string data = match1.Groups[6].Value;
          return new BusMonitorFrame(timestamp, channel1, monitorAddress, data);
        }
        break;
      case BusMonitorFrameStringFormat.Dts:
        Match match2 = BusMonitorFrame.dtsRegex.Match(frame);
        if (match2.Success)
        {
          long timestamp = long.Parse(match2.Groups[3].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          long num = long.Parse(match2.Groups[2].Value, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
          int channel2 = channel.Value;
          long monitorAddress = num;
          string data = match2.Groups[5].Value;
          return new BusMonitorFrame(timestamp, channel2, monitorAddress, data);
        }
        break;
      case BusMonitorFrameStringFormat.Vector:
        Match match3 = BusMonitorFrame.vectorRegex.Match(frame);
        if (match3.Success)
        {
          double num1 = double.Parse(match3.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          int num2 = int.Parse(match3.Groups[2].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          long timestamp = (long) (num1 * 1000000.0);
          long num3 = long.Parse(match3.Groups[3].Value, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
          int channel3 = num2;
          long monitorAddress = num3;
          string data = match3.Groups[5].Value;
          return new BusMonitorFrame(timestamp, channel3, monitorAddress, data);
        }
        Match match4 = BusMonitorFrame.vectorErrorFrameRegex.Match(frame);
        if (match4.Success)
          return new BusMonitorFrame((long) (double.Parse(match4.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture) * 1000000.0), int.Parse(match4.Groups[2].Value, (IFormatProvider) CultureInfo.InvariantCulture), BusMonitorFrame.CustomFrameTypes.ErrorFrame, new byte[0]);
        Match match5 = BusMonitorFrame.vectorChipstateRegex.Match(frame);
        if (match5.Success)
        {
          double num4 = double.Parse(match5.Groups[1].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          int channel4 = int.Parse(match5.Groups[2].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          long timestamp = (long) (num4 * 1000000.0);
          int num5 = 0;
          string str = match5.Groups[3].Value.Replace(" ", string.Empty);
          foreach (string name in Enum.GetNames(typeof (ChipStates)))
          {
            if (str.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1)
              num5 |= (int) Enum.Parse(typeof (ChipStates), name);
          }
          byte num6 = byte.Parse(match5.Groups[4].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          byte num7 = byte.Parse(match5.Groups[5].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          return new BusMonitorFrame(timestamp, channel4, BusMonitorFrame.CustomFrameTypes.ChipState, new byte[3]
          {
            (byte) num5,
            num6,
            num7
          });
        }
        break;
    }
    return (BusMonitorFrame) null;
  }

  private BusMonitorFrame(long timestamp, byte[] data)
  {
    this.timestamp = timestamp;
    this.data = data;
    this.isEthernet = true;
  }

  private BusMonitorFrame(long timestamp, int channel, long monitorAddress, byte[] data)
  {
    this.timestamp = timestamp;
    this.channel = channel;
    this.monitorAddress = monitorAddress;
    this.data = data;
  }

  private BusMonitorFrame(long timestamp, int channel, long monitorAddress, string data)
  {
    this.timestamp = timestamp;
    this.channel = channel;
    this.monitorAddress = monitorAddress;
    this.data = BusMonitorFrame.StringToByteArrayFastest(data.Trim());
  }

  private BusMonitorFrame(
    long timestamp,
    int channel,
    BusMonitorFrame.CustomFrameTypes customFrameType,
    byte[] data)
  {
    this.timestamp = timestamp;
    this.channel = channel;
    this.customFrameType = customFrameType;
    this.data = data;
  }

  private static byte[] StringToByteArrayFastest(string hex)
  {
    int length = (hex.Length + 1) / 3;
    byte[] byteArrayFastest = new byte[length];
    int num1 = 0;
    for (int index1 = 0; index1 < length; ++index1)
    {
      byte[] numArray = byteArrayFastest;
      int index2 = index1;
      string str1 = hex;
      int index3 = num1;
      int num2 = index3 + 1;
      int num3 = BusMonitorFrame.GetHexVal(str1[index3]) << 4;
      string str2 = hex;
      int index4 = num2;
      int num4 = index4 + 1;
      int hexVal = BusMonitorFrame.GetHexVal(str2[index4]);
      int num5 = (int) (byte) (num3 + hexVal);
      numArray[index2] = (byte) num5;
      num1 = num4 + 1;
    }
    return byteArrayFastest;
  }

  internal static int GetHexVal(char hex)
  {
    int num = (int) hex;
    return num - (num < 58 ? 48 /*0x30*/ : (num < 97 ? 55 : 87));
  }

  public string AsString(BusMonitorFrameStringFormat format)
  {
    string str1 = BitConverter.ToString(this.data).Replace('-', ' ');
    switch (format)
    {
      case BusMonitorFrameStringFormat.Mcd:
        if (!this.IsFrameTypeCustom)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RX {0:D20} .{1:x8} {2} {3} ", (object) this.timestamp, (object) this.monitorAddress, (object) this.data.Length, (object) str1);
        break;
      case BusMonitorFrameStringFormat.Dts:
        if (!this.IsFrameTypeCustom)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "5000 {0,8:X} {1,10:D} {2:X4} {3} ", (object) this.monitorAddress, (object) this.timestamp, (object) this.data.Length, (object) str1);
        break;
      case BusMonitorFrameStringFormat.Vector:
        double num = (double) this.timestamp / 1000000.0;
        if (!this.IsFrameTypeCustom)
        {
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:X}x", (object) this.monitorAddress).PadRight(15);
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,11:F6} {1}  {2} Rx   d {3} {4}", (object) num, (object) this.channel, (object) str2, (object) this.data.Length, (object) str1);
        }
        switch (this.FrameType)
        {
          case BusMonitorFrameType.ChipState:
            string str3 = string.Empty;
            switch ((ChipStates) this.data[0])
            {
              case ChipStates.BusOff:
                str3 = "busoff       ";
                break;
              case ChipStates.ErrorPassive:
                str3 = "error passive";
                break;
              case ChipStates.ErrorWarning:
                str3 = "error warning";
                break;
              case ChipStates.ErrorActive:
                str3 = "error active ";
                break;
            }
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,11:F6} CAN {1} Status:chip status {2} - TxErr: {3} RxErr: {4}", (object) num, (object) this.channel, (object) str3, (object) this.data[1], (object) this.data[2]);
          case BusMonitorFrameType.ErrorFrame:
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,11:F6} {1}  ErrorFrame", (object) num, (object) this.channel);
        }
        break;
    }
    return (string) null;
  }

  internal BusMonitorFrame(PassThruMsg msg, int channelId)
  {
    byte[] data = msg.GetData();
    this.timestamp = msg.Timestamp;
    this.channel = channelId;
    this.customFrameType = (BusMonitorFrame.CustomFrameTypes) ((msg.IsChipState ? 1 : 0) | (msg.IsErrorFrame ? 2 : 0) | (msg.IsBaudRate ? 4 : 0));
    if (this.customFrameType != BusMonitorFrame.CustomFrameTypes.None)
    {
      this.data = data;
    }
    else
    {
      if (data.Length < 4)
        return;
      this.monitorAddress = (long) (((int) data[0] << 24) + ((int) data[1] << 16 /*0x10*/) + ((int) data[2] << 8) + (int) data[3]);
      this.data = new byte[data.Length - 4];
      Array.Copy((Array) data, 4, (Array) this.data, 0, data.Length - 4);
    }
  }

  internal BusMonitorFrame(CanMonitoringEntry msg)
  {
    this.timestamp = Convert.ToInt64((object) msg.Time, (IFormatProvider) CultureInfo.InvariantCulture);
    this.monitorAddress = Convert.ToInt64((object) msg.Identifier, (IFormatProvider) CultureInfo.InvariantCulture);
    byte[] data = msg.GetData();
    this.data = new byte[data.Length];
    Array.Copy((Array) data, (Array) this.data, data.Length);
  }

  public long Timestamp => this.timestamp;

  public long Address => this.monitorAddress;

  public IEnumerable<byte> Data => (IEnumerable<byte>) this.data;

  public int Channel => this.channel;

  internal static void ClearIdentifierMap()
  {
    lock (BusMonitorFrame.addressEcuMapLock)
    {
      if (BusMonitorFrame.addressEcuMap != null)
      {
        BusMonitorFrame.addressEcuMap.Clear();
        BusMonitorFrame.addressEcuMap = (Dictionary<long, Tuple<Ecu[], ByteMessageDirection>>) null;
      }
      BusMonitorFrame.udsProtocol = (DiagnosisProtocol) null;
    }
  }

  private static void InitializeIdentifierMap()
  {
    lock (BusMonitorFrame.addressEcuMapLock)
    {
      Sapi sapi = Sapi.GetSapi();
      BusMonitorFrame.addressEcuMap = new Dictionary<long, Tuple<Ecu[], ByteMessageDirection>>();
      BusMonitorFrame.ethernetAddressEcuMap = new Dictionary<Tuple<uint, uint>, Ecu[]>();
      List<Ecu> list = sapi.Ecus.Where<Ecu>((Func<Ecu, bool>) (ecu =>
      {
        if (ecu.DiagnosisSource == DiagnosisSource.CaesarDatabase || ecu.DiagnosisSource == DiagnosisSource.McdDatabase)
        {
          uint? nullable = ecu.RequestId;
          if (nullable.HasValue)
          {
            nullable = ecu.ResponseId;
            return nullable.HasValue;
          }
        }
        return false;
      })).ToList<Ecu>();
      foreach (IGrouping<uint, Ecu> source in list.GroupBy<Ecu, uint>((Func<Ecu, uint>) (e => e.RequestId.Value)))
        BusMonitorFrame.addressEcuMap[(long) source.Key] = new Tuple<Ecu[], ByteMessageDirection>(source.ToArray<Ecu>(), ByteMessageDirection.TX);
      foreach (IGrouping<uint, Ecu> source in list.GroupBy<Ecu, uint>((Func<Ecu, uint>) (e => e.ResponseId.Value)))
        BusMonitorFrame.addressEcuMap[(long) source.Key] = new Tuple<Ecu[], ByteMessageDirection>(source.ToArray<Ecu>(), ByteMessageDirection.RX);
      foreach (IGrouping<byte, Ecu> source in sapi.Ecus.Where<Ecu>((Func<Ecu, bool>) (ecu => ecu.DiagnosisSource == DiagnosisSource.RollCallDatabase && ecu.ProtocolName == "J1939" && ecu.SourceAddress.HasValue)).GroupBy<Ecu, byte>((Func<Ecu, byte>) (e => e.SourceAddress.Value)))
        BusMonitorFrame.addressEcuMap[(long) source.Key] = new Tuple<Ecu[], ByteMessageDirection>(source.ToArray<Ecu>(), ByteMessageDirection.RX);
      foreach (IGrouping<Tuple<uint, uint>, Ecu> source in sapi.Ecus.Where<Ecu>((Func<Ecu, bool>) (ecu =>
      {
        if (ecu.DiagnosisSource == DiagnosisSource.McdDatabase)
        {
          uint? nullable = ecu.LogicalEcuAddress;
          if (nullable.HasValue)
          {
            nullable = ecu.LogicalTesterAddress;
            return nullable.HasValue;
          }
        }
        return false;
      })).GroupBy<Ecu, Tuple<uint, uint>>((Func<Ecu, Tuple<uint, uint>>) (e =>
      {
        uint? nullable = e.LogicalEcuAddress;
        int num1 = (int) nullable.Value;
        nullable = e.LogicalTesterAddress;
        int num2 = (int) nullable.Value;
        return Tuple.Create<uint, uint>((uint) num1, (uint) num2);
      })))
        BusMonitorFrame.ethernetAddressEcuMap[source.Key] = source.ToArray<Ecu>();
      BusMonitorFrame.udsProtocol = Sapi.GetSapi().DiagnosisProtocols["UDS"];
    }
  }

  public static void AddEcuCustomIdentifiers(Ecu ecu, long requestId, long responseId)
  {
    lock (BusMonitorFrame.addressEcuMapLock)
    {
      if (BusMonitorFrame.addressEcuMap == null)
        BusMonitorFrame.InitializeIdentifierMap();
      AddId(requestId, ByteMessageDirection.TX);
      AddId(responseId, ByteMessageDirection.RX);
    }

    void AddId(long id, ByteMessageDirection direction)
    {
      List<Ecu> list = Enumerable.Repeat<Ecu>(ecu, 1).ToList<Ecu>();
      Tuple<Ecu[], ByteMessageDirection> tuple;
      if (BusMonitorFrame.addressEcuMap.TryGetValue(id, out tuple))
        list.AddRange((IEnumerable<Ecu>) tuple.Item1);
      BusMonitorFrame.addressEcuMap[id] = Tuple.Create<Ecu[], ByteMessageDirection>(list.ToArray(), direction);
    }
  }

  public bool TryGetEcus(out Ecu[] ecus)
  {
    if (!this.IsFrameTypeCustom)
    {
      lock (BusMonitorFrame.addressEcuMapLock)
      {
        if (BusMonitorFrame.addressEcuMap == null)
          BusMonitorFrame.InitializeIdentifierMap();
        if (this.isEthernet)
        {
          switch (this.FrameType)
          {
            case BusMonitorFrameType.SingleFrame:
              ecus = BusMonitorFrame.ethernetAddressEcuMap.Where<KeyValuePair<Tuple<uint, uint>, Ecu[]>>((Func<KeyValuePair<Tuple<uint, uint>, Ecu[]>, bool>) (ee =>
              {
                long num = (long) ee.Key.Item1;
                long? nullable = ((int) this.data[12] & 64 /*0x40*/) == 0 ? this.EthernetTargetAddress : this.EthernetSourceAddress;
                long valueOrDefault = nullable.GetValueOrDefault();
                return num == valueOrDefault && nullable.HasValue;
              })).Select<KeyValuePair<Tuple<uint, uint>, Ecu[]>, Ecu[]>((Func<KeyValuePair<Tuple<uint, uint>, Ecu[]>, Ecu[]>) (ee => ee.Value)).FirstOrDefault<Ecu[]>();
              return ecus != null;
            case BusMonitorFrameType.VehicleAnnouncementMessage:
            case BusMonitorFrameType.RoutingActivationResponse:
            case BusMonitorFrameType.Acknowledgment:
              ecus = BusMonitorFrame.ethernetAddressEcuMap.Where<KeyValuePair<Tuple<uint, uint>, Ecu[]>>((Func<KeyValuePair<Tuple<uint, uint>, Ecu[]>, bool>) (ee =>
              {
                long num = (long) ee.Key.Item1;
                long? ethernetSourceAddress = this.EthernetSourceAddress;
                long valueOrDefault = ethernetSourceAddress.GetValueOrDefault();
                return num == valueOrDefault && ethernetSourceAddress.HasValue;
              })).Select<KeyValuePair<Tuple<uint, uint>, Ecu[]>, Ecu[]>((Func<KeyValuePair<Tuple<uint, uint>, Ecu[]>, Ecu[]>) (ee => ee.Value)).FirstOrDefault<Ecu[]>();
              return ecus != null;
          }
        }
        else
        {
          long num = this.monitorAddress;
          if (!this.IsIso)
            num = this.SourceAddress == (byte) 249 ? (long) (byte) ((this.monitorAddress & 65280L) >> 8) : (long) this.SourceAddress;
          Tuple<Ecu[], ByteMessageDirection> tuple;
          if (BusMonitorFrame.addressEcuMap.TryGetValue(num, out tuple))
          {
            ecus = tuple.Item1;
            return true;
          }
          if (!this.IsIso)
          {
            ecus = new Ecu[1]
            {
              new Ecu((int) (byte) num, new int?(), RollCall.GetManager(Protocol.J1939))
            };
            BusMonitorFrame.addressEcuMap.Add(num, new Tuple<Ecu[], ByteMessageDirection>(ecus, ByteMessageDirection.RX));
            return true;
          }
        }
      }
    }
    ecus = (Ecu[]) null;
    return false;
  }

  public ByteMessageDirection Direction
  {
    get
    {
      if (this.isEthernet)
      {
        switch (this.FrameType)
        {
          case BusMonitorFrameType.SingleFrame:
            if (((int) this.data[12] & 64 /*0x40*/) == 0)
              goto case BusMonitorFrameType.VehicleIdentificationRequest;
            break;
          case BusMonitorFrameType.VehicleIdentificationRequest:
          case BusMonitorFrameType.RoutingActivationRequest:
            return ByteMessageDirection.TX;
        }
        return ByteMessageDirection.RX;
      }
      if (this.IsIso)
      {
        lock (BusMonitorFrame.addressEcuMapLock)
        {
          Tuple<Ecu[], ByteMessageDirection> tuple;
          if (BusMonitorFrame.addressEcuMap.TryGetValue(this.monitorAddress, out tuple))
            return tuple.Item2;
        }
        return ByteMessageDirection.RX;
      }
      return this.SourceAddress != (byte) 249 ? ByteMessageDirection.RX : ByteMessageDirection.TX;
    }
  }

  public bool TryGetDiagnosisVariant(out DiagnosisVariant variant)
  {
    Ecu[] ecus;
    if (this.TryGetEcus(out ecus))
      return this.TryGetDiagnosisVariant(ecus, out variant);
    variant = (DiagnosisVariant) null;
    return false;
  }

  public bool TryGetDiagnosisVariant(Ecu[] ecus, out DiagnosisVariant variant)
  {
    if (this.IsIso && this.IsIdentificationInitiatingFrame)
    {
      int? firstDataBytePos = this.IsoFirstDataBytePos;
      int diagVerLong = ((int) this.data[firstDataBytePos.Value + 3] << 16 /*0x10*/) + ((int) this.data[firstDataBytePos.Value + 4] << 8) + (int) this.data[firstDataBytePos.Value + 5];
      variant = ((IEnumerable<Ecu>) ecus).SelectMany<Ecu, DiagnosisVariant>((Func<Ecu, IEnumerable<DiagnosisVariant>>) (e => (IEnumerable<DiagnosisVariant>) e.DiagnosisVariants)).FirstOrDefault<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v =>
      {
        long? diagnosticVersionLong = v.DiagnosticVersionLong;
        long num = (long) diagVerLong;
        return diagnosticVersionLong.GetValueOrDefault() == num && diagnosticVersionLong.HasValue;
      }));
      return variant != null;
    }
    variant = (DiagnosisVariant) null;
    return false;
  }

  public static bool TryGetDiagnosticVariant(
    Ecu[] ecus,
    IEnumerable<BusMonitorFrameCollection> completeMessages,
    out DiagnosisVariant variant)
  {
    IEnumerable<Tuple<RollCall.ID, object>> readIdBlock = completeMessages.SelectMany<BusMonitorFrameCollection, RollCall.IdentificationInformation>((Func<BusMonitorFrameCollection, IEnumerable<RollCall.IdentificationInformation>>) (completeMessage => completeMessage.GetRollCallIdentificationInformation())).DistinctBy<RollCall.IdentificationInformation, RollCall.ID>((Func<RollCall.IdentificationInformation, RollCall.ID>) (id => id.Id)).ToList<RollCall.IdentificationInformation>().Where<RollCall.IdentificationInformation>((Func<RollCall.IdentificationInformation, bool>) (id => id.Value != null)).Select<RollCall.IdentificationInformation, Tuple<RollCall.ID, object>>((Func<RollCall.IdentificationInformation, Tuple<RollCall.ID, object>>) (id => new Tuple<RollCall.ID, object>(id.Id, id.Value)));
    IEnumerable<DiagnosisVariant> source = ((IEnumerable<Ecu>) ecus).SelectMany<Ecu, DiagnosisVariant>((Func<Ecu, IEnumerable<DiagnosisVariant>>) (e => e.DiagnosisVariants.Where<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => v.IsMatch(readIdBlock)))));
    variant = !source.Any<DiagnosisVariant>() ? ((IEnumerable<Ecu>) ecus).First<Ecu>().DiagnosisVariants["ROLLCALL"] : source.OrderBy<DiagnosisVariant, int>((Func<DiagnosisVariant, int>) (v => v.RollCallIdentificationCount)).ThenBy<DiagnosisVariant, bool>((Func<DiagnosisVariant, bool>) (v => !v.Ecu.IsRollCallBaseEcu)).Last<DiagnosisVariant>();
    return variant != null;
  }

  public bool TryGetDescription(
    SapiLayer1.Channel channel,
    out string serviceDescription,
    out string symbolicName)
  {
    serviceDescription = symbolicName = string.Empty;
    if (!this.IsFrameTypeCustom)
    {
      if (this.IsIso)
      {
        int? firstDataBytePos = this.IsoFirstDataBytePos;
        ByteMessageDirection direction = this.Direction;
        if (direction == ByteMessageDirection.RX && this.IsNegativeResponse && this.data.Length > 3)
        {
          serviceDescription = "NR: " + BusMonitorFrame.udsProtocol.GetServiceIdentifierDescription(this.data[firstDataBytePos.Value + 1]);
          symbolicName = BusMonitorFrame.udsProtocol.GetNegativeResponseCodeDescription(this.data[firstDataBytePos.Value + 2]);
        }
        else if (firstDataBytePos.HasValue)
        {
          serviceDescription = BusMonitorFrame.udsProtocol.GetServiceIdentifierDescription(direction == ByteMessageDirection.RX ? (byte) ((uint) this.data[firstDataBytePos.Value] - 64U /*0x40*/) : this.data[firstDataBytePos.Value]);
          List<Service> source;
          if (channel != null && channel.TryGetOfflineDiagnosticService(((IEnumerable<byte>) this.data).Skip<byte>(firstDataBytePos.Value).ToArray<byte>(), direction, out source))
            symbolicName = source.First<Service>().Name.Split(":".ToCharArray())[0];
        }
      }
      else if (this.isEthernet)
      {
        switch (this.FrameType)
        {
          case BusMonitorFrameType.VehicleIdentificationRequest:
            serviceDescription = "Vehicle Identification Request";
            break;
          case BusMonitorFrameType.VehicleAnnouncementMessage:
            serviceDescription = "Vehicle Announcement Message/Vehicle Identification Response";
            symbolicName = "VIN: " + Encoding.UTF8.GetString(this.data, 8, 17);
            break;
          case BusMonitorFrameType.RoutingActivationRequest:
            serviceDescription = "Routing activation request";
            break;
          case BusMonitorFrameType.RoutingActivationResponse:
            serviceDescription = "Routing activation response";
            symbolicName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Response code: {0:X2}", (object) this.data[12]);
            break;
          case BusMonitorFrameType.Acknowledgment:
            serviceDescription = "Acknowledgment";
            break;
        }
      }
      else
      {
        int? nullable1 = new int?();
        string str = string.Empty;
        switch (this.FrameType)
        {
          case BusMonitorFrameType.SingleFrame:
            nullable1 = new int?(this.ParameterGroupNumber);
            int? nullable2 = nullable1;
            int num1 = 59904;
            if ((nullable2.GetValueOrDefault() == num1 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
            {
              nullable1 = this.TargetParameterGroupNumber;
              str = "(RQST)";
              break;
            }
            nullable2 = nullable1;
            int num2 = 59392;
            if ((nullable2.GetValueOrDefault() == num2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
            {
              nullable1 = this.TargetParameterGroupNumber;
              if (this.data[0] < (byte) 4 && !RollCallJ1939.GlobalInstance.SuspectParameters.TryGetValue(((int) this.data[0] + 3290).ToString((IFormatProvider) CultureInfo.InvariantCulture), out str))
              {
                str = string.Empty;
                break;
              }
              break;
            }
            break;
          case BusMonitorFrameType.RequestToSendDestinationSpecific:
          case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
            nullable1 = this.TargetParameterGroupNumber;
            break;
        }
        if (nullable1.HasValue)
        {
          Tuple<string, string> tuple;
          if (RollCallJ1939.GlobalInstance.ParameterGroups.TryGetValue(nullable1.Value, out tuple))
          {
            serviceDescription = $"PGN {(object) nullable1.Value} {tuple.Item2} {str}";
            symbolicName = tuple.Item1;
          }
          else
          {
            serviceDescription = "PGN " + (object) nullable1.Value;
            symbolicName = "<not described>";
          }
        }
      }
    }
    else
    {
      switch (this.FrameType)
      {
        case BusMonitorFrameType.ChipState:
          serviceDescription = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} TXERR {1} RXERR {2}", (object) (ChipStates) this.data[0], (object) this.data[1], (object) this.data[2]);
          break;
        case BusMonitorFrameType.BaudRate:
          serviceDescription = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} bps", (object) (((int) this.data[0] << 24) + ((int) this.data[1] << 16 /*0x10*/) + ((int) this.data[2] << 8) + (int) this.data[3]));
          break;
      }
    }
    return !string.IsNullOrEmpty(serviceDescription) || !string.IsNullOrEmpty(symbolicName);
  }

  public bool IsNegativeResponse
  {
    get
    {
      if (!this.IsFrameTypeCustom)
      {
        if (this.IsIso)
        {
          int? firstDataBytePos = this.IsoFirstDataBytePos;
          if (firstDataBytePos.HasValue && this.data.Length > firstDataBytePos.Value)
            return this.data[firstDataBytePos.Value] == (byte) 127 /*0x7F*/;
        }
        else if (this.ParameterGroupNumber == 59392 && this.data.Length != 0 && this.data[0] != (byte) 0)
          return this.data[0] != (byte) 128 /*0x80*/;
      }
      return false;
    }
  }

  public bool IsFrameTypeCustom => this.customFrameType != 0;

  public BusMonitorFrameType FrameType
  {
    get
    {
      if ((this.customFrameType & BusMonitorFrame.CustomFrameTypes.ChipState) != BusMonitorFrame.CustomFrameTypes.None)
        return BusMonitorFrameType.ChipState;
      if ((this.customFrameType & BusMonitorFrame.CustomFrameTypes.ErrorFrame) != BusMonitorFrame.CustomFrameTypes.None)
        return BusMonitorFrameType.ErrorFrame;
      if ((this.customFrameType & BusMonitorFrame.CustomFrameTypes.BaudRate) != BusMonitorFrame.CustomFrameTypes.None)
        return BusMonitorFrameType.BaudRate;
      if (this.isEthernet)
      {
        switch (((uint) this.data[2] << 8) + (uint) this.data[3])
        {
          case 1:
            return BusMonitorFrameType.VehicleIdentificationRequest;
          case 4:
            return BusMonitorFrameType.VehicleAnnouncementMessage;
          case 5:
            return BusMonitorFrameType.RoutingActivationRequest;
          case 6:
            return BusMonitorFrameType.RoutingActivationResponse;
          case 32769:
            return BusMonitorFrameType.SingleFrame;
          case 32770:
            return BusMonitorFrameType.Acknowledgment;
          default:
            return BusMonitorFrameType.Unknown;
        }
      }
      else
      {
        if (this.IsIso)
          return (BusMonitorFrameType) ((int) this.data[0] >> 4);
        switch (this.ParameterGroupNumber)
        {
          case 60160:
            return BusMonitorFrameType.TransportProtocolDataTransfer;
          case 60416:
            switch (this.data[0])
            {
              case 16 /*0x10*/:
                return BusMonitorFrameType.RequestToSendDestinationSpecific;
              case 17:
                return BusMonitorFrameType.ClearToSendDestinationSpecific;
              case 19:
                return BusMonitorFrameType.EndOfMessageAcknowledge;
              case 32 /*0x20*/:
                return BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination;
              case byte.MaxValue:
                return BusMonitorFrameType.ConnectionAbort;
            }
            break;
        }
        return BusMonitorFrameType.SingleFrame;
      }
    }
  }

  public int? IsoFirstDataBytePos
  {
    get
    {
      if (this.isEthernet)
      {
        if (this.FrameType == BusMonitorFrameType.SingleFrame)
          return new int?(12);
      }
      else
      {
        switch (this.FrameType)
        {
          case BusMonitorFrameType.SingleFrame:
            return new int?(((int) this.data[0] & 15) != 0 ? 1 : 2);
          case BusMonitorFrameType.FirstFrame:
            return new int?(((int) this.data[0] & 15) != 0 || this.data[1] != (byte) 0 ? 2 : 6);
          case BusMonitorFrameType.ConsecutiveFrame:
            return new int?(1);
        }
      }
      return new int?();
    }
  }

  public int ActualDataLength
  {
    get
    {
      if (this.IsIso)
        return !this.IsoFirstDataBytePos.HasValue ? 0 : this.data.Length - this.IsoFirstDataBytePos.Value;
      switch (this.FrameType)
      {
        case BusMonitorFrameType.SingleFrame:
          return this.data.Length;
        case BusMonitorFrameType.TransportProtocolDataTransfer:
          return this.data.Length - 1;
        default:
          return 0;
      }
    }
  }

  public bool IsInitiatingFrameFor(BusMonitorFrame frame)
  {
    if (!this.IsFrameTypeCustom && !this.isEthernet)
    {
      if (this.IsIso)
      {
        if (frame.FrameType == BusMonitorFrameType.FlowControl || frame.FrameType == BusMonitorFrameType.ConsecutiveFrame)
        {
          int num1 = frame.FrameType == BusMonitorFrameType.FlowControl ? 1 : 0;
          byte num2 = num1 != 0 ? frame.DestinationAddress : frame.SourceAddress;
          byte num3 = num1 != 0 ? frame.SourceAddress : frame.DestinationAddress;
          if (this.FrameType == BusMonitorFrameType.FirstFrame && (int) this.SourceAddress == (int) num2 && (int) this.DestinationAddress == (int) num3)
            return true;
        }
      }
      else
      {
        int num4 = frame.FrameType == BusMonitorFrameType.ClearToSendDestinationSpecific ? 1 : (frame.FrameType == BusMonitorFrameType.EndOfMessageAcknowledge ? 1 : 0);
        byte num5 = num4 != 0 ? frame.DestinationAddress : frame.SourceAddress;
        byte num6 = num4 != 0 ? frame.SourceAddress : frame.DestinationAddress;
        if (this.FrameType == BusMonitorFrameType.RequestToSendDestinationSpecific && (int) this.SourceAddress == (int) num5 && (int) this.DestinationAddress == (int) num6 || this.FrameType == BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination && (int) this.SourceAddress == (int) num5 && num6 == byte.MaxValue)
          return true;
      }
    }
    return false;
  }

  public bool IsContinuation
  {
    get
    {
      switch (this.FrameType)
      {
        case BusMonitorFrameType.ConsecutiveFrame:
        case BusMonitorFrameType.FlowControl:
        case BusMonitorFrameType.ClearToSendDestinationSpecific:
        case BusMonitorFrameType.ConnectionAbort:
        case BusMonitorFrameType.EndOfMessageAcknowledge:
        case BusMonitorFrameType.TransportProtocolDataTransfer:
          return true;
        default:
          return false;
      }
    }
  }

  public bool IsIso
  {
    get
    {
      if (!this.IsFrameTypeCustom)
      {
        if (this.isEthernet)
          return this.FrameType == BusMonitorFrameType.SingleFrame;
        if ((byte) ((this.monitorAddress & 469762048L /*0x1C000000*/) >> 26) == (byte) 6)
        {
          switch ((byte) ((this.monitorAddress & 16711680L /*0xFF0000*/) >> 16 /*0x10*/))
          {
            case 205:
            case 206:
            case 218:
            case 219:
              return true;
          }
        }
      }
      return false;
    }
  }

  public byte SourceAddress => (byte) ((ulong) this.monitorAddress & (ulong) byte.MaxValue);

  public byte DestinationAddress => (byte) ((this.monitorAddress & 65280L) >> 8);

  public int ParameterGroupNumber
  {
    get
    {
      byte num1 = (byte) ((this.monitorAddress & 33554432L /*0x02000000*/) >> 25);
      byte num2 = (byte) ((this.monitorAddress & 16777216L /*0x01000000*/) >> 24);
      byte num3 = (byte) ((this.monitorAddress & 16711680L /*0xFF0000*/) >> 16 /*0x10*/);
      byte num4 = (byte) ((this.monitorAddress & 65280L) >> 8);
      return num3 < (byte) 240 /*0xF0*/ ? ((int) num3 << 8) + ((int) num2 << 17) + ((int) num1 << 18) : ((int) num3 << 8) + ((int) num2 << 17) + ((int) num1 << 18) + (int) num4;
    }
  }

  public int? TargetParameterGroupNumber
  {
    get
    {
      switch (this.FrameType)
      {
        case BusMonitorFrameType.SingleFrame:
          switch (this.ParameterGroupNumber)
          {
            case 59392:
              return new int?(((int) this.data[7] << 16 /*0x10*/) + ((int) this.data[6] << 8) + (int) this.data[5]);
            case 59904:
              return new int?(((int) this.data[2] << 16 /*0x10*/) + ((int) this.data[1] << 8) + (int) this.data[0]);
          }
          break;
        case BusMonitorFrameType.RequestToSendDestinationSpecific:
        case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
          return new int?(((int) this.data[7] << 16 /*0x10*/) + ((int) this.data[6] << 8) + (int) this.data[5]);
      }
      return new int?();
    }
  }

  public long? EthernetSourceAddress
  {
    get
    {
      switch (this.FrameType)
      {
        case BusMonitorFrameType.SingleFrame:
        case BusMonitorFrameType.RoutingActivationRequest:
        case BusMonitorFrameType.Acknowledgment:
          return new long?((long) ((uint) this.data[8] << 8 | (uint) this.data[9]));
        case BusMonitorFrameType.VehicleAnnouncementMessage:
          return new long?((long) ((uint) this.data[25] << 8 | (uint) this.data[26]));
        case BusMonitorFrameType.RoutingActivationResponse:
          return new long?((long) ((uint) this.data[10] << 8 | (uint) this.data[11]));
        default:
          return new long?();
      }
    }
  }

  public long? EthernetTargetAddress
  {
    get
    {
      switch (this.FrameType)
      {
        case BusMonitorFrameType.SingleFrame:
        case BusMonitorFrameType.Acknowledgment:
          return new long?((long) ((uint) this.data[10] << 8 | (uint) this.data[11]));
        default:
          return new long?();
      }
    }
  }

  public bool IsEthernet => this.isEthernet;

  internal byte? TotalPackets
  {
    get
    {
      return this.FrameType != BusMonitorFrameType.RequestToSendDestinationSpecific ? new byte?() : new byte?(this.data[3]);
    }
  }

  internal byte? SequenceNumber
  {
    get
    {
      return this.FrameType != BusMonitorFrameType.TransportProtocolDataTransfer ? new byte?() : new byte?(this.data[0]);
    }
  }

  public bool IsIdentificationInitiatingFrame
  {
    get
    {
      if (this.IsIso)
      {
        int? firstDataBytePos = this.IsoFirstDataBytePos;
        return firstDataBytePos.HasValue && this.data[firstDataBytePos.Value] == (byte) 98 && this.data[firstDataBytePos.Value + 1] == (byte) 241 && this.data[firstDataBytePos.Value + 2] == (byte) 0;
      }
      int? nullable = new int?();
      switch (this.FrameType)
      {
        case BusMonitorFrameType.SingleFrame:
          nullable = new int?(this.ParameterGroupNumber);
          break;
        case BusMonitorFrameType.RequestToSendDestinationSpecific:
        case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
          nullable = this.TargetParameterGroupNumber;
          break;
      }
      return nullable.HasValue && ((IEnumerable<RollCallJ1939.PGN>) BusMonitorFrame.IdentificationPGNs).Contains<RollCallJ1939.PGN>((RollCallJ1939.PGN) nullable.Value);
    }
  }

  [Flags]
  internal enum CustomFrameTypes : byte
  {
    None = 0,
    ChipState = 1,
    ErrorFrame = 2,
    BaudRate = 4,
  }
}
