// Decompiled with JetBrains decompiler
// Type: SapiLayer1.RollCallJ1708
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using J2534;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace SapiLayer1;

internal sealed class RollCallJ1708 : RollCallSae
{
  private static Dictionary<RollCallJ1708.MidGroup, List<byte>> midGroupMapping;
  private static RollCallJ1708 globalInstance = new RollCallJ1708();
  private static IEnumerable<int> cycleGlobalRequestIds = (IEnumerable<int>) new List<int>()
  {
    237,
    243,
    234,
    194
  };
  private Dictionary<RollCallJ1708.PID, List<byte>> multiSectionData = new Dictionary<RollCallJ1708.PID, List<byte>>();

  private static Dictionary<RollCallJ1708.MidGroup, List<byte>> MidGroupMapping
  {
    get
    {
      if (RollCallJ1708.midGroupMapping == null)
      {
        RollCallJ1708.midGroupMapping = new Dictionary<RollCallJ1708.MidGroup, List<byte>>();
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Engine, new List<byte>()
        {
          (byte) 128 /*0x80*/,
          (byte) 175,
          (byte) 183,
          (byte) 184,
          (byte) 185,
          (byte) 186
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Transmission, new List<byte>()
        {
          (byte) 130,
          (byte) 176 /*0xB0*/,
          (byte) 223
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Brakes, new List<byte>()
        {
          (byte) 136,
          (byte) 137,
          (byte) 138,
          (byte) 139,
          (byte) 246,
          (byte) 247
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.InstrumentPanel, new List<byte>()
        {
          (byte) 140,
          (byte) 234
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.VehicleManagement, new List<byte>()
        {
          (byte) 142,
          (byte) 187,
          (byte) 188
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.FuelSystem, new List<byte>()
        {
          (byte) 143
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.ClimateControl, new List<byte>()
        {
          (byte) 146,
          (byte) 200
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Suspension, new List<byte>()
        {
          (byte) 150,
          (byte) 151
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.ParkBrakeController, new List<byte>()
        {
          (byte) 157
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Navigation, new List<byte>()
        {
          (byte) 162,
          (byte) 191
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Security, new List<byte>()
        {
          (byte) 163
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.Tire, new List<byte>()
        {
          (byte) 166,
          (byte) 167,
          (byte) 168,
          (byte) 169,
          (byte) 186
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.ParticulateTrap, new List<byte>()
        {
          (byte) 177
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.VehicleSensorsToDataConverter, new List<byte>()
        {
          (byte) 178
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.RefrigerantManagement, new List<byte>()
        {
          (byte) 190
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.TractionTrailerBridge, new List<byte>()
        {
          (byte) 217,
          (byte) 218
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.CollisionAvoidance, new List<byte>()
        {
          (byte) 219
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.DrivelineRetarder, new List<byte>()
        {
          (byte) 222
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.SafetyRestraintSystem, new List<byte>()
        {
          (byte) 232,
          (byte) 254
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.ForwardRoadImageProcessor, new List<byte>()
        {
          (byte) 248
        });
        RollCallJ1708.midGroupMapping.Add(RollCallJ1708.MidGroup.BrakeStrokeAlert, new List<byte>()
        {
          (byte) 253
        });
      }
      return RollCallJ1708.midGroupMapping;
    }
  }

  private static RollCallJ1708.MidGroup GetMidGroup(byte sourceAddress)
  {
    foreach (RollCallJ1708.MidGroup key in RollCallJ1708.MidGroupMapping.Keys)
    {
      if (RollCallJ1708.midGroupMapping[key].Contains(sourceAddress))
        return key;
    }
    return RollCallJ1708.MidGroup.None;
  }

  internal static RollCallJ1708 GlobalInstance => RollCallJ1708.globalInstance;

  private RollCallJ1708()
    : base(Protocol.J1708)
  {
  }

  public override IEnumerable<byte> PowertrainAddresses
  {
    get
    {
      return (IEnumerable<byte>) new byte[2]
      {
        (byte) 128 /*0x80*/,
        (byte) 130
      };
    }
  }

  protected override PassThruMsg CreateRequestMessage(int id, byte destinationAddress)
  {
    byte[] data;
    if ((int) destinationAddress == (int) this.GlobalRequestAddress)
      data = new byte[3]{ (byte) 172, (byte) 0, (byte) id };
    else
      data = new byte[4]
      {
        (byte) 172,
        (byte) 128 /*0x80*/,
        (byte) id,
        destinationAddress
      };
    if (this.debugLevel > (ushort) 1)
    {
      string text = "CreateRequestMessage: " + (object) (RollCallJ1708.PID) id;
      if (this.debugLevel > (ushort) 2)
        text = $"{text} {new Dump((IEnumerable<byte>) data).ToString()}";
      this.RaiseDebugInfoEvent((int) destinationAddress, text);
    }
    return new PassThruMsg(ProtocolId.J1708, 0U, 134217728U /*0x08000000*/, 0U, 0U, data);
  }

  internal override string GetFaultText(Channel channel, string number, string mode)
  {
    string str = string.Empty;
    int result;
    if (number.StartsWith("S", StringComparison.Ordinal) && int.TryParse(number.Substring(1), out result) && (result < 151 || result > (int) byte.MaxValue))
    {
      RollCallJ1708.MidGroup midGroup = RollCallJ1708.GetMidGroup(channel.SourceAddress.Value);
      byte num = midGroup != RollCallJ1708.MidGroup.None ? RollCallJ1708.MidGroupMapping[midGroup][0] : channel.SourceAddress.Value;
      str = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(number, num.ToString((IFormatProvider) CultureInfo.InvariantCulture), "SPN"), string.Empty);
    }
    if (string.IsNullOrEmpty(str))
      str = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(number, "SPN"), string.Empty);
    return $"{str} - {channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(mode, "FMI"), string.Empty)}";
  }

  internal override void ClearErrors(Channel channel)
  {
    byte[] data = new byte[6]
    {
      (byte) 172,
      (byte) 195,
      (byte) 3,
      channel.SourceAddress.Value,
      (byte) 0,
      (byte) 128 /*0x80*/
    };
    if (this.debugLevel > (ushort) 1)
    {
      string text = "ClearErrors: " + (object) RollCallJ1708.PID.DiagnosticDataClearCount;
      if (this.debugLevel > (ushort) 2)
        text = $"{text} {new Dump((IEnumerable<byte>) data).ToString()}";
      this.RaiseDebugInfoEvent((int) channel.SourceAddress.Value, text);
    }
    this.RequestAndWait(new RollCallSae.QueueItem(new PassThruMsg(ProtocolId.J1708, 0U, 134217728U /*0x08000000*/, 0U, 0U, data), 196, channel.SourceAddress.Value));
  }

  internal override byte[] ReadInstrument(
    Channel channel,
    byte[] data,
    int responseId,
    Predicate<Tuple<byte?, byte[]>> additionalResponseCheck,
    int responseTimeout)
  {
    int responseStart = RollCallJ1708.IsPage2(responseId) ? 3 : 2;
    uint num = (uint) data[0];
    data = ((IEnumerable<byte>) data).Skip<byte>(1).ToArray<byte>();
    byte[] source = this.RequestAndWait(new RollCallSae.QueueItem(new PassThruMsg(ProtocolId.J1708, 0U, num << 24, 0U, 0U, data), responseId, channel.SourceAddress.Value, (Predicate<byte[]>) (response => additionalResponseCheck(new Tuple<byte?, byte[]>(new byte?(), ((IEnumerable<byte>) response).Skip<byte>(responseStart).ToArray<byte>()))), channel.Ecu.GetComParameter("CP_REQREPCOUNT", 1), channel.Ecu.GetComParameter("CP_P2_MAX", Math.Min(2000, responseTimeout)), channel.Ecu.GetComParameter("CP_REQREPCOUNTBUSY", 0), channel.Ecu.GetComParameter("CP_P2_EXT_TIMEOUT_BUSY", 0)));
    return source != null ? ((IEnumerable<byte>) source).Skip<byte>(responseStart).ToArray<byte>() : (byte[]) null;
  }

  internal override byte[] DoByteMessage(Channel channel, byte[] data, byte[] requiredResponse)
  {
    if (data.Length == 1)
      return this.RequestAndWait((int) data[0], channel.SourceAddress.Value);
    uint num1 = (uint) data[0];
    data = ((IEnumerable<byte>) data).Skip<byte>(1).ToArray<byte>();
    if (requiredResponse == null || requiredResponse.Length == 0)
    {
      if (data[1] == (byte) 128 /*0x80*/ && data.Length == 4 || data[1] == (byte) 0 && data.Length == 3)
      {
        byte? sourceAddress;
        if (data[1] == (byte) 128 /*0x80*/)
        {
          int num2 = (int) data[3];
          sourceAddress = channel.SourceAddress;
          int num3 = (int) sourceAddress.Value;
          if (num2 != num3)
          {
            sourceAddress = channel.SourceAddress;
            this.RaiseDebugInfoEvent((int) sourceAddress.Value, $"WARNING: byte message sent to a different address ({(object) data[3]}). Response may not be received.");
          }
        }
        PassThruMsg requestMessage = new PassThruMsg(ProtocolId.J1708, 0U, num1 << 24, 0U, 0U, data);
        int responseId = (int) data[2];
        sourceAddress = channel.SourceAddress;
        int destinationAddress = (int) sourceAddress.Value;
        return this.RequestAndWait(new RollCallSae.QueueItem(requestMessage, responseId, (byte) destinationAddress));
      }
      J2534Error j2534Error = this.Write(new PassThruMsg(ProtocolId.J1708, 0U, num1 << 24, 0U, 0U, data));
      if (j2534Error == J2534Error.NoError)
        return (byte[]) null;
      Sapi.GetSapi().RaiseDebugInfoEvent((object) channel, $"Result from J2534.WriteMsgs is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
      throw new CaesarException(SapiError.CannotSendMessageToDevice);
    }
    if (requiredResponse.Length == 1)
      return this.RequestAndWait(new RollCallSae.QueueItem(new PassThruMsg(ProtocolId.J1708, 0U, num1 << 24, 0U, 0U, data), (int) requiredResponse[0], channel.SourceAddress.Value));
    if (requiredResponse.Length == 2 && requiredResponse[0] == byte.MaxValue)
      return this.RequestAndWait(new RollCallSae.QueueItem(new PassThruMsg(ProtocolId.J1708, 0U, num1 << 24, 0U, 0U, data), (int) requiredResponse[1] + (int) byte.MaxValue, channel.SourceAddress.Value));
    throw new CaesarException(SapiError.CannotSendMessageToDevice);
  }

  protected override int BetweenGlobalIdRequestInterval => 2500;

  protected override uint BaudRate => 9600;

  protected override int TotalMessagesPerSecond => 100;

  protected override bool RequiresFunction(byte address) => false;

  protected override IEnumerable<RollCall.ID> GetIdentificationIds(byte? address)
  {
    return (IEnumerable<RollCall.ID>) new List<RollCall.ID>()
    {
      RollCall.ID.Make,
      RollCall.ID.Model,
      RollCall.ID.SerialNumber,
      RollCall.ID.SoftwareIdentification,
      RollCall.ID.VehicleIdentificationNumber
    };
  }

  protected override byte GlobalRequestAddress => 0;

  private static bool IsPage2(int id) => id > (int) byte.MaxValue;

  protected override IEnumerable<int> CycleGlobalRequestIds => RollCallJ1708.cycleGlobalRequestIds;

  protected override int MapIdToRequestId(RollCall.ID id)
  {
    switch (id)
    {
      case RollCall.ID.SoftwareIdentification:
        return 234;
      case RollCall.ID.VehicleIdentificationNumber:
        return 237;
      case RollCall.ID.Make:
      case RollCall.ID.Model:
      case RollCall.ID.SerialNumber:
        return 243;
      default:
        return 0;
    }
  }

  private bool TryGetMultiSectionData(
    byte[] data,
    out RollCallJ1708.PID responsePID,
    out byte[] responseData)
  {
    responsePID = RollCallJ1708.PID.MultiSectionParameter;
    responseData = (byte[]) null;
    if (data.Length < 3)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Received Invalid length multi-section data, expected at least 3 bytes");
      return false;
    }
    byte num1 = data[0];
    if (data.Length < (int) num1 + 1)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Received Invalid length multi-section data, expected {0} bytes, received {1}", (object) num1, (object) (data.Length - 1)));
      return false;
    }
    RollCallJ1708.PID key = (RollCallJ1708.PID) data[1];
    byte num2 = (byte) (((int) data[2] & 240 /*0xF0*/) >> 4);
    byte num3 = (byte) ((uint) data[2] & 15U);
    if (num3 == (byte) 0)
    {
      this.multiSectionData[key] = new List<byte>();
      this.multiSectionData[key].Add(num3);
      for (int index = 0; index < (int) num1 - 2; ++index)
        this.multiSectionData[key].Add(data[3 + index]);
    }
    else if (this.multiSectionData.ContainsKey(key) && (int) this.multiSectionData[key][0] == (int) num3 - 1)
    {
      this.multiSectionData[key][0] = num3;
      for (int index = 0; index < (int) num1 - 2; ++index)
        this.multiSectionData[key].Add(data[3 + index]);
    }
    else
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"Received out of sequence PID 192: current section is {(object) num3} previous section is {(this.multiSectionData.ContainsKey(key) ? (object) this.multiSectionData[key][0].ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) "<not found>")}");
      return false;
    }
    if ((int) num3 != (int) num2)
      return false;
    responsePID = key;
    responseData = this.multiSectionData[key].Skip<byte>(1).ToArray<byte>();
    this.multiSectionData.Remove(key);
    return true;
  }

  protected override bool TryExtractMessage(
    byte[] source,
    out byte address,
    out int id,
    out byte[] data)
  {
    if (source.Length < 2 || source[1] == byte.MaxValue && source.Length < 3)
    {
      address = (byte) 0;
      id = 0;
      data = (byte[]) null;
      return false;
    }
    address = source[0];
    id = source[1] != byte.MaxValue ? (int) source[1] : (int) source[2] + 256 /*0x0100*/;
    data = ((IEnumerable<byte>) source).ToArray<byte>();
    if (id == 192 /*0xC0*/)
    {
      RollCallJ1708.PID responsePID;
      byte[] responseData;
      if (!this.TryGetMultiSectionData(((IEnumerable<byte>) data).Skip<byte>(2).ToArray<byte>(), out responsePID, out responseData))
        return false;
      id = (int) responsePID;
      data = ((IEnumerable<byte>) new byte[2]
      {
        address,
        (byte) responsePID
      }).Concat<byte>((IEnumerable<byte>) responseData).ToArray<byte>();
    }
    return true;
  }

  private static string GetIdString(int id)
  {
    if (!Enum.IsDefined(typeof (RollCallJ1708.PID), (object) (uint) id))
      return "PID " + (object) id;
    return $"PID {(object) id}({(object) (RollCallJ1708.PID) id})";
  }

  protected override void HandleIncomingMessage(
    byte address,
    int id,
    byte[] data,
    Channel channel)
  {
    if (this.debugLevel > (ushort) 1 && (Enum.IsDefined(typeof (RollCallJ1708.PID), (object) (uint) id) || this.debugLevel > (ushort) 3))
    {
      string text = "HandleIncomingMessage: " + RollCallJ1708.GetIdString(id);
      if (this.debugLevel > (ushort) 2)
        text = $"{text}: {new Dump((IEnumerable<byte>) data).ToString()}";
      this.RaiseDebugInfoEvent((int) address, text);
    }
    switch ((RollCallJ1708.PID) id)
    {
      case RollCallJ1708.PID.DiagnosticCodeTable:
        if (channel != null)
        {
          Dictionary<string, byte?> codes1 = new Dictionary<string, byte?>();
          Dictionary<string, byte?> codes2 = new Dictionary<string, byte?>();
          bool flag1;
          for (List<byte> list = ((IEnumerable<byte>) data).Skip<byte>(3).ToList<byte>(); list.Count > 1; list = list.Skip<byte>(flag1 ? 3 : 2).ToList<byte>())
          {
            int num1 = (int) list[0];
            int num2 = (int) list[1] & 15;
            int num3 = ((uint) list[1] & 16U /*0x10*/) > 0U ? 1 : 0;
            bool flag2 = ((int) list[1] & 32 /*0x20*/) == 0;
            bool flag3 = ((int) list[1] & 64 /*0x40*/) == 0;
            flag1 = ((uint) list[1] & 128U /*0x80*/) > 0U;
            string key = $"{(num3 != 0 ? "S" : "P")}{(num1 + (flag2 ? 256 /*0x0100*/ : 0)).ToString((IFormatProvider) CultureInfo.InvariantCulture)}:FMI{num2.ToString((IFormatProvider) CultureInfo.InvariantCulture)}";
            byte? nullable = new byte?();
            if (flag1 && list.Count > 2)
              nullable = new byte?(list[2]);
            if (flag3)
              codes1[key] = nullable;
            else
              codes2[key] = nullable;
          }
          channel.FaultCodes.UpdateFromRollCall(codes1, typeof (ActiveStatus), TimeSpan.Zero);
          channel.FaultCodes.UpdateFromRollCall(codes2, typeof (TestFailedSinceLastClearStatus), TimeSpan.FromMilliseconds((double) this.ChannelTimeout));
          break;
        }
        break;
      case RollCallJ1708.PID.SoftwareIdentification:
        if (data.Length > 3)
        {
          string str = Encoding.ASCII.GetString(data, 3, data.Length - 3);
          this.AddIdentification(address, RollCall.ID.SoftwareIdentification, (object) str);
          break;
        }
        break;
      case RollCallJ1708.PID.VehicleIdentification:
        if (data.Length > 3)
        {
          string str = Encoding.ASCII.GetString(data, 3, data.Length - 3);
          this.AddIdentification(address, RollCall.ID.VehicleIdentificationNumber, (object) str);
          break;
        }
        break;
      case RollCallJ1708.PID.ComponentIdentification:
        if (data.Length > 4)
        {
          string str = Encoding.ASCII.GetString(data, 4, data.Length - 4);
          string[] strArray = new string[3]
          {
            string.Empty,
            string.Empty,
            string.Empty
          };
          ((IEnumerable<string>) str.Split("*".ToCharArray())).Take<string>(3).ToArray<string>().CopyTo((Array) strArray, 0);
          this.AddIdentification(address, RollCall.ID.Make, (object) strArray[0]);
          this.AddIdentification(address, RollCall.ID.Model, (object) strArray[1]);
          this.AddIdentification(address, RollCall.ID.SerialNumber, (object) strArray[2]);
          break;
        }
        break;
      default:
        if (channel != null && channel.Instruments.Count > 0)
        {
          IEnumerable<byte> source = ((IEnumerable<byte>) data).Skip<byte>(RollCallJ1708.IsPage2(id) ? 2 : 1);
          while (source.Any<byte>())
          {
            byte num = source.First<byte>();
            source = source.Skip<byte>(1);
            if (source.Any<byte>())
            {
              int count = num < (byte) 128 /*0x80*/ ? 1 : (num < (byte) 192 /*0xC0*/ ? 2 : (int) source.First<byte>() + 1);
              if (source.Count<byte>() >= count)
              {
                int id1 = (int) num + (RollCallJ1708.IsPage2(id) ? 256 /*0x0100*/ : 0);
                channel.Instruments.UpdateFromRollCall(id1, new byte?(), source.Take<byte>(count).ToArray<byte>());
                channel.EcuInfos.UpdateFromRollCall(id1, source.Take<byte>(count).ToArray<byte>());
                source = source.Skip<byte>(count);
              }
              else
                break;
            }
          }
          break;
        }
        break;
    }
    this.NotifyQueueItem(id, address, data, RollCallSae.Acknowledgment.Positive);
  }

  private enum PID : uint
  {
    GlobalParameterRequest = 0,
    ComponentSpecificParameterRequest = 128, // 0x00000080
    MultiSectionParameter = 192, // 0x000000C0
    DiagnosticCodeTable = 194, // 0x000000C2
    DiagnosticDataClearCount = 195, // 0x000000C3
    DiagnosticDataClearCountResponse = 196, // 0x000000C4
    SoftwareIdentification = 234, // 0x000000EA
    VehicleIdentification = 237, // 0x000000ED
    ComponentIdentification = 243, // 0x000000F3
  }

  private enum MidGroup
  {
    None = 0,
    Engine = 1,
    Transmission = 2,
    Brakes = 3,
    InstrumentPanel = 4,
    VehicleManagement = 5,
    FuelSystem = 6,
    ClimateControl = 7,
    Suspension = 8,
    Navigation = 9,
    Security = 10, // 0x0000000A
    Tire = 11, // 0x0000000B
    ParticulateTrap = 12, // 0x0000000C
    RefrigerantManagement = 13, // 0x0000000D
    TractionTrailerBridge = 14, // 0x0000000E
    CollisionAvoidance = 15, // 0x0000000F
    DrivelineRetarder = 16, // 0x00000010
    SafetyRestraintSystem = 17, // 0x00000011
    TransmsissionAGS2 = 19, // 0x00000013
    ForwardRoadImageProcessor = 20, // 0x00000014
    BrakeStrokeAlert = 21, // 0x00000015
    VehicleSensorsToDataConverter = 22, // 0x00000016
    ParkBrakeController = 23, // 0x00000017
  }
}
