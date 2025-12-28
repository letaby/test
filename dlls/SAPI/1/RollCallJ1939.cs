// Decompiled with JetBrains decompiler
// Type: SapiLayer1.RollCallJ1939
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

internal sealed class RollCallJ1939 : RollCallSae
{
  private const int HighestStandardSuspectParameterNumber = 5999;
  private const int PropAPDUFormat = 239;
  private const int DestinationAddressBytePos = 5;
  private const int DataStartBytePos = 6;
  private static RollCallJ1939 globalInstance = new RollCallJ1939();
  private static IEnumerable<int> cycleGlobalRequestIds = (IEnumerable<int>) new List<int>()
  {
    60928,
    65260,
    65259,
    65242,
    65231,
    65226,
    65227,
    65236,
    64949,
    64896,
    40704
  };
  private static Dictionary<string, Type> IdentificationTypes = ((IEnumerable<Tuple<string, Type>>) new Tuple<string, Type>[24]
  {
    new Tuple<string, Type>(RollCall.ID.OnBoardDiagnosticCompliance.ToNumberString(), typeof (Choice)),
    new Tuple<string, Type>(RollCall.ID.ManufacturerCode.ToNumberString(), typeof (Choice)),
    new Tuple<string, Type>(RollCall.ID.Function.ToNumberString(), typeof (Choice)),
    new Tuple<string, Type>(RollCall.ID.SPNOfApplicableSystemMonitor.ToNumberString(), typeof (Choice[])),
    new Tuple<string, Type>(RollCall.ID.ApplicableSystemMonitorNumerator.ToNumberString(), typeof (int[])),
    new Tuple<string, Type>(RollCall.ID.ApplicableSystemMonitorDenominator.ToNumberString(), typeof (int[])),
    new Tuple<string, Type>(RollCall.ID.SPNSupported.ToNumberString(), typeof (Choice[])),
    new Tuple<string, Type>(RollCall.ID.SupportedInDataStream.ToNumberString(), typeof (bool[])),
    new Tuple<string, Type>(RollCall.ID.SupportedInExpandedFreezeFrame.ToNumberString(), typeof (bool[])),
    new Tuple<string, Type>(RollCall.ID.SupportedInScaledTestResults.ToNumberString(), typeof (bool[])),
    new Tuple<string, Type>(RollCall.ID.SPNDataLength.ToNumberString(), typeof (byte[])),
    new Tuple<string, Type>(RollCall.ID.CalibrationInformation.ToNumberString(), typeof (string[])),
    new Tuple<string, Type>(RollCall.ID.CalibrationVerificationNumber.ToNumberString(), typeof (string[])),
    new Tuple<string, Type>(RollCall.ID.TestIdentifier.ToNumberString(), typeof (int[])),
    new Tuple<string, Type>(RollCall.ID.SuspectParameterNumber.ToNumberString(), typeof (Choice[])),
    new Tuple<string, Type>(RollCall.ID.FailureModeIdentifier.ToNumberString(), typeof (Choice[])),
    new Tuple<string, Type>(RollCall.ID.TestValue.ToNumberString(), typeof (object[])),
    new Tuple<string, Type>(RollCall.ID.TestLimitMaximum.ToNumberString(), typeof (object[])),
    new Tuple<string, Type>(RollCall.ID.TestLimitMinimum.ToNumberString(), typeof (object[])),
    new Tuple<string, Type>(RollCall.ID.SlotIdentifier.ToNumberString(), typeof (int[])),
    new Tuple<string, Type>(RollCall.ID.UnitSystem.ToNumberString(), typeof (string[])),
    new Tuple<string, Type>(RollCall.ID.AECDNumber.ToNumberString(), typeof (byte[])),
    new Tuple<string, Type>(RollCall.ID.AECDEngineHoursTimer1.ToNumberString(), typeof (int[])),
    new Tuple<string, Type>(RollCall.ID.AECDEngineHoursTimer2.ToNumberString(), typeof (int[]))
  }).ToDictionary<Tuple<string, Type>, string, Type>((Func<Tuple<string, Type>, string>) (k => k.Item1), (Func<Tuple<string, Type>, Type>) (v => v.Item2));
  private static Dictionary<string, string> IdentificationUnits = ((IEnumerable<Tuple<string, string>>) new Tuple<string, string>[2]
  {
    new Tuple<string, string>(RollCall.ID.AECDEngineHoursTimer1.ToNumberString(), "min"),
    new Tuple<string, string>(RollCall.ID.AECDEngineHoursTimer2.ToNumberString(), "min")
  }).ToDictionary<Tuple<string, string>, string, string>((Func<Tuple<string, string>, string>) (k => k.Item1), (Func<Tuple<string, string>, string>) (v => v.Item2));
  private static Dictionary<string, Choice.TranslationQualifierType> TranslationQualifierTypes = ((IEnumerable<Tuple<string, Choice.TranslationQualifierType>>) new Tuple<string, Choice.TranslationQualifierType>[4]
  {
    new Tuple<string, Choice.TranslationQualifierType>(RollCall.ID.SPNSupported.ToNumberString(), Choice.TranslationQualifierType.GlobalSpn),
    new Tuple<string, Choice.TranslationQualifierType>(RollCall.ID.SPNOfApplicableSystemMonitor.ToNumberString(), Choice.TranslationQualifierType.GlobalSpn),
    new Tuple<string, Choice.TranslationQualifierType>(RollCall.ID.SuspectParameterNumber.ToNumberString(), Choice.TranslationQualifierType.GlobalSpn),
    new Tuple<string, Choice.TranslationQualifierType>(RollCall.ID.FailureModeIdentifier.ToNumberString(), Choice.TranslationQualifierType.GlobalFmi)
  }).ToDictionary<Tuple<string, Choice.TranslationQualifierType>, string, Choice.TranslationQualifierType>((Func<Tuple<string, Choice.TranslationQualifierType>, string>) (k => k.Item1), (Func<Tuple<string, Choice.TranslationQualifierType>, Choice.TranslationQualifierType>) (v => v.Item2));

  internal static RollCallJ1939 GlobalInstance => RollCallJ1939.globalInstance;

  private RollCallJ1939()
    : base(Protocol.J1939)
  {
  }

  public override IEnumerable<byte> PowertrainAddresses
  {
    get
    {
      return (IEnumerable<byte>) new byte[7]
      {
        (byte) 0,
        (byte) 1,
        (byte) 3,
        (byte) 15,
        (byte) 17,
        (byte) 61,
        (byte) 90
      };
    }
  }

  protected override int BetweenGlobalIdRequestInterval => 1000;

  protected override uint BaudRate
  {
    get
    {
      if (this.IsAutoBaudRate)
      {
        uint baudRate1 = 0;
        J2534Error baudRate2 = Sid.GetBaudRate(this.channelId, ref baudRate1);
        if (baudRate2 == J2534Error.NoError)
          return baudRate1;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Baud rate could not be retrieved: " + (object) baudRate2);
      }
      return 250000;
    }
  }

  protected override int TotalMessagesPerSecond => 1500;

  protected override byte GlobalRequestAddress => byte.MaxValue;

  private static bool IsGlobalRequest(int id)
  {
    byte num = BitConverter.GetBytes(id)[1];
    return num >= (byte) 240 /*0xF0*/ && num <= byte.MaxValue;
  }

  private static bool IsPropA(int id) => BitConverter.GetBytes(id)[1] == (byte) 239;

  protected override PassThruMsg CreateRequestMessage(int id, byte destinationAddress)
  {
    byte[] data;
    if (id == 58112)
      data = new byte[14]
      {
        (byte) 0,
        (byte) 227,
        (byte) 0,
        (byte) 6,
        (byte) 249,
        destinationAddress,
        (byte) 246,
        (byte) 214,
        (byte) 22,
        (byte) 31 /*0x1F*/,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue
      };
    else
      data = new byte[9]
      {
        (byte) 0,
        (byte) 234,
        (byte) 0,
        (byte) 6,
        (byte) 249,
        destinationAddress,
        (byte) ((ulong) id & (ulong) byte.MaxValue),
        (byte) (((ulong) id & 65280UL) >> 8),
        (byte) (((ulong) id & 1044480UL /*0x0FF000*/) >> 16 /*0x10*/)
      };
    if (this.debugLevel > (ushort) 1)
    {
      string text = "CreateRequestMessage: " + (object) (RollCallJ1939.PGN) id;
      if (this.debugLevel > (ushort) 2)
        text = $"{text} {new Dump((IEnumerable<byte>) data).ToString()}";
      this.RaiseDebugInfoEvent((int) destinationAddress, text);
    }
    return new PassThruMsg(ProtocolId.J1939, data);
  }

  protected override bool RequiresFunction(byte address)
  {
    return address >= (byte) 128 /*0x80*/ && address < byte.MaxValue;
  }

  protected override IEnumerable<RollCall.ID> GetIdentificationIds(byte? address)
  {
    List<RollCall.ID> identificationIds = new List<RollCall.ID>()
    {
      RollCall.ID.ManufacturerCode,
      RollCall.ID.Function,
      RollCall.ID.Make,
      RollCall.ID.Model,
      RollCall.ID.SerialNumber,
      RollCall.ID.UnitNumber,
      RollCall.ID.SoftwareIdentification,
      RollCall.ID.VehicleIdentificationNumber,
      RollCall.ID.EcuPartNumber,
      RollCall.ID.EcuSerialNumber,
      RollCall.ID.EcuLocation,
      RollCall.ID.EcuType,
      RollCall.ID.EcuManufacturerName
    };
    if (address.HasValue)
    {
      byte? nullable1 = address;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num1 = 0;
      if ((nullable2.GetValueOrDefault() == num1 ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
      {
        nullable1 = address;
        nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        int num2 = 1;
        if ((nullable2.GetValueOrDefault() == num2 ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
        {
          nullable1 = address;
          nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
          int num3 = 61;
          if ((nullable2.GetValueOrDefault() == num3 ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
            goto label_5;
        }
      }
      identificationIds.AddRange((IEnumerable<RollCall.ID>) new RollCall.ID[22]
      {
        RollCall.ID.OnBoardDiagnosticCompliance,
        RollCall.ID.CalibrationInformation,
        RollCall.ID.CalibrationVerificationNumber,
        RollCall.ID.SPNSupported,
        RollCall.ID.SupportedInDataStream,
        RollCall.ID.SupportedInExpandedFreezeFrame,
        RollCall.ID.SupportedInScaledTestResults,
        RollCall.ID.SPNDataLength,
        RollCall.ID.SPNOfApplicableSystemMonitor,
        RollCall.ID.ApplicableSystemMonitorNumerator,
        RollCall.ID.ApplicableSystemMonitorDenominator,
        RollCall.ID.TestIdentifier,
        RollCall.ID.SuspectParameterNumber,
        RollCall.ID.FailureModeIdentifier,
        RollCall.ID.SlotIdentifier,
        RollCall.ID.TestValue,
        RollCall.ID.TestLimitMaximum,
        RollCall.ID.TestLimitMinimum,
        RollCall.ID.UnitSystem,
        RollCall.ID.AECDNumber,
        RollCall.ID.AECDEngineHoursTimer1,
        RollCall.ID.AECDEngineHoursTimer2
      });
    }
label_5:
    return (IEnumerable<RollCall.ID>) identificationIds;
  }

  protected override IEnumerable<int> CycleGlobalRequestIds => RollCallJ1939.cycleGlobalRequestIds;

  internal override bool IsRequestIdContentVisible(int id)
  {
    if (id <= 41984)
    {
      if (id != 41216 && id != 41984)
        goto label_4;
    }
    else if (id != 49664 && id != 58112 && id != 64950)
      goto label_4;
    return false;
label_4:
    return true;
  }

  protected override int MapIdToRequestId(RollCall.ID id)
  {
    switch (id)
    {
      case RollCall.ID.UnitNumber:
      case RollCall.ID.Make:
      case RollCall.ID.Model:
      case RollCall.ID.SerialNumber:
        return 65259;
      case RollCall.ID.SoftwareIdentification:
        return 65242;
      case RollCall.ID.VehicleIdentificationNumber:
        return 65260;
      case RollCall.ID.SuspectParameterNumber:
      case RollCall.ID.FailureModeIdentifier:
      case RollCall.ID.TestIdentifier:
      case RollCall.ID.SlotIdentifier:
      case RollCall.ID.TestValue:
      case RollCall.ID.TestLimitMaximum:
      case RollCall.ID.TestLimitMinimum:
      case RollCall.ID.UnitSystem:
        return 58112;
      case RollCall.ID.OnBoardDiagnosticCompliance:
        return 65230;
      case RollCall.ID.CalibrationVerificationNumber:
      case RollCall.ID.CalibrationInformation:
        return 54016;
      case RollCall.ID.ManufacturerCode:
      case RollCall.ID.Function:
        return 60928;
      case RollCall.ID.EcuPartNumber:
      case RollCall.ID.EcuSerialNumber:
      case RollCall.ID.EcuLocation:
      case RollCall.ID.EcuType:
      case RollCall.ID.EcuManufacturerName:
        return 64965;
      case RollCall.ID.SPNOfApplicableSystemMonitor:
      case RollCall.ID.ApplicableSystemMonitorNumerator:
      case RollCall.ID.ApplicableSystemMonitorDenominator:
        return 49664;
      case RollCall.ID.SPNSupported:
      case RollCall.ID.SupportedInExpandedFreezeFrame:
      case RollCall.ID.SupportedInDataStream:
      case RollCall.ID.SupportedInScaledTestResults:
      case RollCall.ID.SPNDataLength:
        return 64950;
      case RollCall.ID.AECDNumber:
      case RollCall.ID.AECDEngineHoursTimer1:
      case RollCall.ID.AECDEngineHoursTimer2:
        return 41216;
      default:
        return 0;
    }
  }

  private static void AddEcuInfoValueIfExisting(
    Channel channel,
    RollCall.ID id,
    List<string> content)
  {
    EcuInfo ecuInfo = channel.EcuInfos[id.ToNumberString()] ?? channel.EcuInfos.FirstOrDefault<EcuInfo>((Func<EcuInfo, bool>) (e => e.Qualifier.IndexOf(id.ToNumberString(), StringComparison.Ordinal) != -1));
    if (ecuInfo == null || ecuInfo.EcuInfoValues.Current == null || ecuInfo.EcuInfoValues.Current.Value == null || string.IsNullOrEmpty(ecuInfo.EcuInfoValues.Current.Value.ToString().Trim()))
      return;
    content.Add(ecuInfo.EcuInfoValues.Current.Value.ToString().Trim());
  }

  private static void GetTranslationLookupsForComponents(
    IEnumerable<string> components,
    List<string> output)
  {
    for (int count = components.Count<string>(); count > 1; --count)
      output.Add(string.Join(".", components.Take<string>(count).ToArray<string>()) + ".SPN");
  }

  internal override string GetFaultText(Channel channel, string number, string mode)
  {
    bool flag = false;
    int result;
    if (int.TryParse(number, out result) && result > 5999)
      flag = true;
    string str1 = string.Empty;
    if (flag)
    {
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      stringList1.Add(number);
      byte? sourceAddress = channel.SourceAddress;
      if (sourceAddress.HasValue)
      {
        sourceAddress = channel.SourceAddress;
        if (!this.RequiresFunction(sourceAddress.Value) || !channel.Ecu.Function.HasValue)
        {
          List<string> stringList3 = stringList1;
          sourceAddress = channel.SourceAddress;
          string str2 = sourceAddress.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          stringList3.Add(str2);
          goto label_7;
        }
      }
      stringList1.Add("F" + (object) channel.Ecu.Function.Value);
label_7:
      RollCallJ1939.AddEcuInfoValueIfExisting(channel, RollCall.ID.Make, stringList1);
      RollCallJ1939.AddEcuInfoValueIfExisting(channel, RollCall.ID.Model, stringList1);
      RollCallJ1939.GetTranslationLookupsForComponents((IEnumerable<string>) stringList1, stringList2);
      stringList1.RemoveAt(1);
      RollCallJ1939.GetTranslationLookupsForComponents((IEnumerable<string>) stringList1, stringList2);
      stringList2.Add(Sapi.MakeTranslationIdentifier(number, "SPN"));
      for (int index = 0; index < stringList2.Count<string>() && string.IsNullOrEmpty(str1); ++index)
        str1 = channel.Ecu.Translate(stringList2[index], string.Empty);
    }
    else
      str1 = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(number, "SPN"), string.Empty);
    return $"{str1} - {channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(mode, "FMI"), string.Empty)}";
  }

  internal override void ClearErrors(Channel channel)
  {
    CaesarException caesarException = (CaesarException) null;
    RollCallJ1939.PGN[] pgnArray = new RollCallJ1939.PGN[2]
    {
      RollCallJ1939.PGN.ClearActiveDiagnosticTroubleCodes,
      RollCallJ1939.PGN.ClearPreviouslyActiveDiagnosticTroubleCodes
    };
    foreach (RollCallJ1939.PGN id in pgnArray)
    {
      try
      {
        this.RequestAndWait((int) id, channel.SourceAddress.Value);
      }
      catch (CaesarException ex)
      {
        caesarException = ex;
      }
    }
    if (caesarException != null)
      throw caesarException;
  }

  private static int ExtractPGN(byte[] data, int offset)
  {
    return (int) data[offset] + ((int) data[offset + 1] << 8) + ((int) data[offset + 2] << 16 /*0x10*/);
  }

  internal override bool IsSnapshotSupported(Channel channel)
  {
    if (channel.LogFile != null)
      return RollCallJ1939.GetCurrentSPNSupport(channel, RollCall.ID.SupportedInExpandedFreezeFrame).Any<Tuple<Choice, byte>>();
    object identificationValue = this.GetIdentificationValue((int) channel.SourceAddress.Value, RollCall.ID.SupportedInExpandedFreezeFrame.ToNumberString());
    return identificationValue != null && identificationValue is bool[] source && ((IEnumerable<bool>) source).Any<bool>((Func<bool, bool>) (v => v));
  }

  private static IEnumerable<Tuple<Choice, byte>> GetCurrentSPNSupport(
    Channel channel,
    RollCall.ID supportType)
  {
    Choice[] overallSPNs = channel.EcuInfos[RollCall.ID.SPNSupported.ToNumberString()].GetCurrentArraySet<Choice>();
    bool[] supportDataForSPNs = channel.EcuInfos[supportType.ToNumberString()].GetCurrentArraySet<bool>();
    byte[] lengthForSPNs = channel.EcuInfos[RollCall.ID.SPNDataLength.ToNumberString()].GetCurrentArraySet<byte>();
    for (int i = 0; i < overallSPNs.Length; ++i)
    {
      if (supportDataForSPNs[i])
        yield return new Tuple<Choice, byte>(overallSPNs[i], lengthForSPNs[i]);
    }
  }

  private IEnumerable<Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>> ExtractSnapshot(
    Channel channel,
    byte[] frame)
  {
    RollCallJ1939 rollCallJ1939_1 = this;
    IEnumerable<Tuple<Choice, byte>> spnsSupported = RollCallJ1939.GetCurrentSPNSupport(channel, RollCall.ID.SupportedInExpandedFreezeFrame);
    int freezeFrameLength;
    for (; frame.Length > 4; frame = ((IEnumerable<byte>) frame).Skip<byte>(freezeFrameLength + 1).ToArray<byte>())
    {
      freezeFrameLength = (int) frame[0];
      if (freezeFrameLength > 0)
      {
        string hexString = ((IList<byte>) ((IEnumerable<byte>) frame).Skip<byte>(1).Take<byte>(3).ToArray<byte>()).ToHexString();
        if (hexString != "000000")
        {
          byte[] array = ((IEnumerable<byte>) frame).Skip<byte>(5).ToArray<byte>();
          int num = spnsSupported.Sum<Tuple<Choice, byte>>((Func<Tuple<Choice, byte>, int>) (spn => (int) spn.Item2));
          byte? sourceAddress1;
          if (freezeFrameLength != num + 4)
          {
            RollCallJ1939 rollCallJ1939_2 = rollCallJ1939_1;
            sourceAddress1 = channel.SourceAddress;
            int sourceAddress2 = (int) sourceAddress1.Value;
            string text = $"ExtractSnapshot: WARNING: DM25 specified length ({(object) freezeFrameLength}) for frame '{hexString}' doesn't match what DM24 specified ({(object) num} plus 4)";
            rollCallJ1939_2.RaiseDebugInfoEvent(sourceAddress2, text);
          }
          List<Tuple<Instrument, byte[]>> tupleList = new List<Tuple<Instrument, byte[]>>();
          foreach (Tuple<Choice, byte> tuple in spnsSupported)
          {
            if (tuple.Item2 > (byte) 0)
            {
              if (array.Length >= (int) tuple.Item2)
              {
                Instrument snapshotDescription = channel.FaultCodes.GetRollCallSnapshotDescription("DT_" + tuple.Item1.RawValue);
                tupleList.Add(new Tuple<Instrument, byte[]>(snapshotDescription, ((IEnumerable<byte>) array).Take<byte>((int) tuple.Item2).ToArray<byte>()));
                if (snapshotDescription.Type == typeof (Dump))
                {
                  RollCallJ1939 rollCallJ1939_3 = rollCallJ1939_1;
                  sourceAddress1 = channel.SourceAddress;
                  int sourceAddress3 = (int) sourceAddress1.Value;
                  string text = $"ExtractSnapshot: don't have an instrument definition for SPN {tuple.Item1.RawValue} '{tuple.Item1.Name}'";
                  rollCallJ1939_3.RaiseDebugInfoEvent(sourceAddress3, text);
                }
                array = ((IEnumerable<byte>) array).Skip<byte>((int) tuple.Item2).ToArray<byte>();
              }
              else
              {
                RollCallJ1939 rollCallJ1939_4 = rollCallJ1939_1;
                sourceAddress1 = channel.SourceAddress;
                int sourceAddress4 = (int) sourceAddress1.Value;
                string text = $"ExtractSnapshot: WARNING: DM25 content too short; aborting at SPN {tuple.Item1.RawValue} '{tuple.Item1.Name}'";
                rollCallJ1939_4.RaiseDebugInfoEvent(sourceAddress4, text);
                break;
              }
            }
          }
          yield return new Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>(hexString, (IEnumerable<Tuple<Instrument, byte[]>>) tupleList);
        }
      }
      else
      {
        rollCallJ1939_1.RaiseDebugInfoEvent((int) channel.SourceAddress.Value, "ExtractSnapshot: DM25 specified freeze-frame length of zero");
        break;
      }
    }
  }

  internal override byte[] ReadInstrument(
    Channel channel,
    byte[] data,
    int responseId,
    Predicate<Tuple<byte?, byte[]>> additionalResponseCheck,
    int responseTimeout)
  {
    byte[] source = this.RequestAndWait(new RollCallSae.QueueItem(new PassThruMsg(ProtocolId.J1939, data), responseId, channel.SourceAddress.Value, (Predicate<byte[]>) (response => additionalResponseCheck(new Tuple<byte?, byte[]>(new byte?(response[5]), ((IEnumerable<byte>) response).Skip<byte>(6).ToArray<byte>()))), channel.Ecu.GetComParameter("CP_REQREPCOUNT", 1), channel.Ecu.GetComParameter("CP_P2_MAX", Math.Min(2000, responseTimeout)), channel.Ecu.GetComParameter("CP_REQREPCOUNTBUSY", 0), channel.Ecu.GetComParameter("CP_P2_EXT_TIMEOUT_BUSY", 0)));
    return source != null ? ((IEnumerable<byte>) source).Skip<byte>(6).ToArray<byte>() : (byte[]) null;
  }

  internal override void ReadSnapshot(Channel channel)
  {
    this.RequestAndWait(64951, channel.SourceAddress.Value);
  }

  internal override void ReadFaultCodes(Channel channel)
  {
    this.RequestAndWait(65226, channel.SourceAddress.Value);
    this.RequestAndWait(65227, channel.SourceAddress.Value);
    if (channel.SourceAddress.Value != (byte) 0 && channel.SourceAddress.Value != (byte) 1 && channel.SourceAddress.Value != (byte) 61)
      return;
    this.RequestAndWait(65236, channel.SourceAddress.Value);
    this.RequestAndWait(64949, channel.SourceAddress.Value);
    byte? sourceAddress = channel.SourceAddress;
    this.RequestAndWait(65231, sourceAddress.Value);
    sourceAddress = channel.SourceAddress;
    this.RequestAndWait(64896, sourceAddress.Value);
    sourceAddress = channel.SourceAddress;
    this.RequestAndWait(40704, sourceAddress.Value);
  }

  protected override byte[] RequestAndWait(int id, byte destinationAddress)
  {
    return this.RequestAndWait(new RollCallSae.QueueItem(this.CreateRequestMessage(id, destinationAddress), id == 58112 ? 41984 : id, destinationAddress));
  }

  internal override byte[] DoByteMessage(Channel channel, byte[] data, byte[] requiredResponse)
  {
    int pgn1 = RollCallJ1939.ExtractPGN(data, 0);
    if (RollCallJ1939.IsGlobalRequest(pgn1) || RollCallJ1939.IsPropA(pgn1) || pgn1 == 58112 && requiredResponse == null)
    {
      J2534Error j2534Error = this.Write(new PassThruMsg(ProtocolId.J1939, data));
      if (j2534Error == J2534Error.NoError)
        return (byte[]) null;
      Sapi.GetSapi().RaiseDebugInfoEvent((object) channel, $"ID {(object) pgn1}: Result from J2534.WriteMsgs is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
      throw new CaesarException(SapiError.CannotSendMessageToDevice);
    }
    if (data.Length == 3)
      return this.RequestAndWait(RollCallJ1939.ExtractPGN(data, 0), channel.SourceAddress.Value);
    if (data.Length > 6)
    {
      int num1 = (int) data[5];
      byte? sourceAddress = channel.SourceAddress;
      int num2 = (int) sourceAddress.Value;
      if (num1 != num2)
      {
        sourceAddress = channel.SourceAddress;
        this.RaiseDebugInfoEvent((int) sourceAddress.Value, $"WARNING: byte message sent to a different address ({(object) data[5]}). Response may not be received.");
      }
      if (requiredResponse == null || requiredResponse.Length == 0)
      {
        if (pgn1 == 59904 && data.Length == 9)
        {
          PassThruMsg requestMessage = new PassThruMsg(ProtocolId.J1939, data);
          int pgn2 = RollCallJ1939.ExtractPGN(data, 6);
          sourceAddress = channel.SourceAddress;
          int destinationAddress = (int) sourceAddress.Value;
          return this.RequestAndWait(new RollCallSae.QueueItem(requestMessage, pgn2, (byte) destinationAddress));
        }
      }
      else if (requiredResponse.Length == 3)
      {
        int num3 = (int) requiredResponse[0] + ((int) requiredResponse[1] << 8) + ((int) requiredResponse[2] << 16 /*0x10*/);
        PassThruMsg requestMessage = new PassThruMsg(ProtocolId.J1939, data);
        int responseId = num3;
        sourceAddress = channel.SourceAddress;
        int destinationAddress = (int) sourceAddress.Value;
        return this.RequestAndWait(new RollCallSae.QueueItem(requestMessage, responseId, (byte) destinationAddress));
      }
    }
    throw new CaesarException(SapiError.CannotSendMessageToDevice);
  }

  private static Dictionary<string, byte?> ExtractCodes(IList<byte> data)
  {
    Dictionary<string, byte?> codes = new Dictionary<string, byte?>();
    if (data.Count != 6 || !data.Take<byte>(3).SequenceEqual<byte>((IEnumerable<byte>) new byte[3]))
    {
      for (int index = 0; index + 3 < data.Count; index += 4)
      {
        byte? nullable = new byte?();
        if (((int) data[index + 3] & 128 /*0x80*/) != 128 /*0x80*/)
          nullable = new byte?((byte) ((uint) data[index + 3] & (uint) sbyte.MaxValue));
        string hexString = ((IList<byte>) new byte[3]
        {
          data[index],
          data[index + 1],
          data[index + 2]
        }).ToHexString();
        codes[hexString] = nullable;
      }
    }
    return codes;
  }

  private static string GetASCIIString(IEnumerable<byte> data, int offset, int length)
  {
    List<byte> byteList = new List<byte>();
    foreach (byte num in data.Skip<byte>(offset).Take<byte>(length))
    {
      if (num != (byte) 0)
        byteList.Add(num <= (byte) 31 /*0x1F*/ || num >= (byte) 127 /*0x7F*/ ? (byte) 63 /*0x3F*/ : num);
      else
        break;
    }
    return Encoding.ASCII.GetString(byteList.ToArray());
  }

  private static IEnumerable<KeyValuePair<string, Dump>> ExtractCalibrationInformation(
    IEnumerable<byte> data)
  {
    for (; data.Count<byte>() > 0; data = data.Skip<byte>(20))
    {
      if (data.Count<byte>() >= 20)
      {
        Dump dump = new Dump(data.Take<byte>(4).Reverse<byte>());
        yield return new KeyValuePair<string, Dump>(RollCallJ1939.GetASCIIString(data, 4, 16 /*0x10*/), dump);
      }
    }
  }

  private static IEnumerable<RollCallJ1939.SPNSupport> ExtractSPNSupport(IEnumerable<byte> data)
  {
    for (; data.Count<byte>() >= 4; data = data.Skip<byte>(4))
      yield return new RollCallJ1939.SPNSupport(data.Take<byte>(4).ToArray<byte>());
  }

  private IEnumerable<RollCallJ1939.ScaledTestResult> ExtractScaledTestResults(
    byte sourceAddress,
    IEnumerable<byte> data)
  {
    RollCallJ1939 rollCallJ1939 = this;
    while (data.Count<byte>() >= 12)
    {
      RollCallJ1939.ScaledTestResult content = new RollCallJ1939.ScaledTestResult(data.Take<byte>(12).ToArray<byte>());
      yield return content;
      if (content.TestValue.GetType() == typeof (Dump))
        rollCallJ1939.RaiseDebugInfoEvent((int) sourceAddress, "ExtractScaledTestResults: unable to locate definition for SLOT " + (object) content.SlotIdentifier);
      data = data.Skip<byte>(12);
      content = new RollCallJ1939.ScaledTestResult();
    }
  }

  private static IEnumerable<Tuple<int, int, int>> ExtractMonitorRatio(byte[] data)
  {
    for (; data.Length >= 7; data = ((IEnumerable<byte>) data).Skip<byte>(7).ToArray<byte>())
    {
      int num1 = (int) data[0] + ((int) data[1] << 8) + (((int) data[2] & 7) << 16 /*0x10*/);
      if (num1 != 524287 /*0x07FFFF*/)
      {
        int num2 = (int) data[3] + ((int) data[4] << 8);
        int num3 = (int) data[5] + ((int) data[6] << 8);
        yield return new Tuple<int, int, int>(num1, num2, num3);
      }
    }
  }

  private static IEnumerable<Tuple<byte, int, int>> ExtractAECDTimers(byte[] data)
  {
    for (; data.Length >= 9; data = ((IEnumerable<byte>) data).Skip<byte>(9).ToArray<byte>())
      yield return new Tuple<byte, int, int>(data[0], (int) data[1] + ((int) data[2] << 8) + ((int) data[3] << 16 /*0x10*/) + ((int) data[4] << 24), (int) data[5] + ((int) data[6] << 8) + ((int) data[7] << 16 /*0x10*/) + ((int) data[8] << 24));
  }

  protected override bool TryExtractMessage(
    byte[] source,
    out byte address,
    out int id,
    out byte[] data)
  {
    address = source[4];
    id = RollCallJ1939.ExtractPGN(source, 0);
    data = source;
    switch (id)
    {
      case 55808:
      case 56064:
        return false;
      default:
        return address != (byte) 254;
    }
  }

  private static string GetIdString(int id)
  {
    if (!Enum.IsDefined(typeof (RollCallJ1939.PGN), (object) (uint) id))
      return "PGN " + (object) id;
    return $"PGN {(object) id}({(object) (RollCallJ1939.PGN) id})";
  }

  private static Type GetFaultStatusType(RollCallJ1939.PGN pgn)
  {
    switch (pgn)
    {
      case RollCallJ1939.PGN.ImmediateFaultStatus:
        return typeof (ImmediateStatus);
      case RollCallJ1939.PGN.EmissionRelatedPreviouslyActiveDiagnosticTroubleCodes:
        return typeof (StoredStatus);
      case RollCallJ1939.PGN.ActiveDiagnosticTroubleCodes:
        return typeof (ActiveStatus);
      case RollCallJ1939.PGN.PreviouslyActiveDiagnosticTroubleCodes:
        return typeof (TestFailedSinceLastClearStatus);
      case RollCallJ1939.PGN.PendingDiagnosticTroubleCodes:
        return typeof (PendingStatus);
      case RollCallJ1939.PGN.EmissionRelatedActiveDiagnosticTroubleCodes:
        return typeof (MilStatus);
      default:
        return (Type) null;
    }
  }

  protected override Presentation CreatePresentation(Ecu ecu, string qualifier)
  {
    Type type;
    if (!RollCallJ1939.IdentificationTypes.TryGetValue(qualifier, out type))
      return (Presentation) null;
    ChoiceCollection choices = (ChoiceCollection) null;
    if (type == typeof (Choice[]) || type == typeof (Choice))
    {
      Choice.TranslationQualifierType qualifierFormat;
      if (!RollCallJ1939.TranslationQualifierTypes.TryGetValue(qualifier, out qualifierFormat))
        qualifierFormat = Choice.TranslationQualifierType.Standard;
      choices = new ChoiceCollection(ecu, qualifier, true, qualifierFormat);
    }
    string units = (string) null;
    RollCallJ1939.IdentificationUnits.TryGetValue(qualifier, out units);
    return new Presentation(ecu, "PRES_" + qualifier, choices, type, units);
  }

  internal static bool TryGetIdentificationInformation(
    int id,
    byte[] data,
    out List<RollCall.IdentificationInformation> ids)
  {
    ids = (List<RollCall.IdentificationInformation>) null;
    List<RollCall.IdentificationInformation> foundIds = new List<RollCall.IdentificationInformation>();
    bool identificationInformation = false;
    switch ((RollCallJ1939.PGN) id)
    {
      case RollCallJ1939.PGN.CalibrationInformation:
        if (data.Length > 6)
        {
          List<KeyValuePair<string, Dump>> list = RollCallJ1939.ExtractCalibrationInformation(((IEnumerable<byte>) data).Skip<byte>(6)).ToList<KeyValuePair<string, Dump>>();
          AddIdentification(RollCall.ID.CalibrationInformation, (object) list.Select<KeyValuePair<string, Dump>, string>((Func<KeyValuePair<string, Dump>, string>) (p => p.Key)).ToArray<string>());
          AddIdentification(RollCall.ID.CalibrationVerificationNumber, (object) list.Select<KeyValuePair<string, Dump>, string>((Func<KeyValuePair<string, Dump>, string>) (p => p.Value.ToString())).ToArray<string>());
          identificationInformation = true;
          break;
        }
        break;
      case RollCallJ1939.PGN.AddressClaim:
        if (data.Length > 13)
        {
          RollCallJ1939.AddressClaim addressClaim = RollCallJ1939.AddressClaim.FromByteArray(data);
          AddIdentification(RollCall.ID.ManufacturerCode, (object) addressClaim.ManufacturerCode);
          int num = (int) addressClaim.Function;
          if (num > (int) sbyte.MaxValue)
            num = (((int) addressClaim.IndustryGroup << 7) + (int) addressClaim.VehicleSystem << 8) + num;
          AddIdentification(RollCall.ID.Function, (object) num);
          identificationInformation = true;
          break;
        }
        break;
      case RollCallJ1939.PGN.EcuIdentificationInformation:
        if (data.Length > 6)
        {
          string asciiString = RollCallJ1939.GetASCIIString((IEnumerable<byte>) data, 6, data.Length - 6);
          string[] strArray = new string[5]
          {
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
          };
          ((IEnumerable<string>) asciiString.Split("*".ToCharArray())).Take<string>(5).ToArray<string>().CopyTo((Array) strArray, 0);
          AddIdentification(RollCall.ID.EcuPartNumber, (object) strArray[0]);
          AddIdentification(RollCall.ID.EcuSerialNumber, (object) strArray[1]);
          AddIdentification(RollCall.ID.EcuLocation, (object) strArray[2]);
          AddIdentification(RollCall.ID.EcuType, (object) strArray[3]);
          AddIdentification(RollCall.ID.EcuManufacturerName, (object) strArray[4]);
          identificationInformation = true;
          break;
        }
        break;
      case RollCallJ1939.PGN.DiagnosticReadiness:
        if (data.Length > 13)
        {
          AddIdentification(RollCall.ID.OnBoardDiagnosticCompliance, (object) data[8]);
          identificationInformation = true;
          break;
        }
        break;
      case RollCallJ1939.PGN.SoftwareIdentification:
        if (data.Length > 7)
        {
          AddIdentification(RollCall.ID.SoftwareIdentification, (object) RollCallJ1939.GetASCIIString((IEnumerable<byte>) data, 7, data.Length - 7));
          identificationInformation = true;
          break;
        }
        break;
      case RollCallJ1939.PGN.ComponentIdentification:
        if (data.Length > 6)
        {
          string asciiString = RollCallJ1939.GetASCIIString((IEnumerable<byte>) data, 6, data.Length - 6);
          string[] strArray = new string[4]
          {
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
          };
          ((IEnumerable<string>) asciiString.Split("*".ToCharArray())).Take<string>(4).ToArray<string>().CopyTo((Array) strArray, 0);
          AddIdentification(RollCall.ID.Make, (object) strArray[0]);
          AddIdentification(RollCall.ID.Model, (object) strArray[1]);
          AddIdentification(RollCall.ID.SerialNumber, (object) strArray[2]);
          AddIdentification(RollCall.ID.UnitNumber, (object) strArray[3]);
          identificationInformation = true;
          break;
        }
        break;
      case RollCallJ1939.PGN.VehicleIdentification:
        if (data.Length > 6)
        {
          AddIdentification(RollCall.ID.VehicleIdentificationNumber, (object) RollCallJ1939.GetASCIIString((IEnumerable<byte>) data, 6, Math.Min(data.Length - 6, 17)));
          identificationInformation = true;
          break;
        }
        break;
    }
    ids = foundIds;
    return identificationInformation;

    void AddIdentification(RollCall.ID foundId, object value)
    {
      foundIds.Add(new RollCall.IdentificationInformation(foundId)
      {
        Value = value
      });
    }
  }

  protected override void HandleIncomingMessage(
    byte address,
    int id,
    byte[] data,
    Channel channel)
  {
    if (this.debugLevel > (ushort) 1 && (Enum.IsDefined(typeof (RollCallJ1939.PGN), (object) (uint) id) || this.debugLevel > (ushort) 3))
    {
      string text = "HandleIncomingMessage: " + RollCallJ1939.GetIdString(id);
      if (this.debugLevel > (ushort) 2)
        text = $"{text}: {new Dump((IEnumerable<byte>) data).ToString()}";
      this.RaiseDebugInfoEvent((int) address, text);
    }
    List<RollCall.IdentificationInformation> ids;
    if (RollCallJ1939.TryGetIdentificationInformation(id, data, out ids))
      ids.ForEach((Action<RollCall.IdentificationInformation>) (fi => this.AddIdentification(address, fi.Id, fi.Value)));
    switch ((RollCallJ1939.PGN) id)
    {
      case RollCallJ1939.PGN.ImmediateFaultStatus:
      case RollCallJ1939.PGN.PermanentDiagnosticTroubleCodes:
      case RollCallJ1939.PGN.EmissionRelatedPreviouslyActiveDiagnosticTroubleCodes:
      case RollCallJ1939.PGN.ActiveDiagnosticTroubleCodes:
      case RollCallJ1939.PGN.PreviouslyActiveDiagnosticTroubleCodes:
      case RollCallJ1939.PGN.PendingDiagnosticTroubleCodes:
      case RollCallJ1939.PGN.EmissionRelatedActiveDiagnosticTroubleCodes:
        if (channel != null)
        {
          RollCallJ1939.PGN pgn = (RollCallJ1939.PGN) id;
          Dictionary<string, byte?> codes = RollCallJ1939.ExtractCodes((IList<byte>) ((IEnumerable<byte>) data).Skip<byte>(8).ToList<byte>());
          if (this.debugLevel > (ushort) 1)
            this.RaiseDebugInfoEvent((int) address, $"{(object) pgn} reports {(codes.Any<KeyValuePair<string, byte?>>() ? (object) string.Join(", ", codes.Keys.ToArray<string>()) : (object) "no codes")}");
          channel.FaultCodes.UpdateFromRollCall(codes, RollCallJ1939.GetFaultStatusType(pgn), pgn == RollCallJ1939.PGN.PermanentDiagnosticTroubleCodes, TimeSpan.Zero);
          break;
        }
        break;
      case RollCallJ1939.PGN.AECDActiveTime:
        if (data.Length > 6)
        {
          List<Tuple<byte, int, int>> list = RollCallJ1939.ExtractAECDTimers(((IEnumerable<byte>) data).Skip<byte>(6).ToArray<byte>()).ToList<Tuple<byte, int, int>>();
          this.AddIdentification(address, RollCall.ID.AECDNumber, (object) list.Select<Tuple<byte, int, int>, byte>((Func<Tuple<byte, int, int>, byte>) (m => m.Item1)).ToArray<byte>());
          this.AddIdentification(address, RollCall.ID.AECDEngineHoursTimer1, (object) list.Select<Tuple<byte, int, int>, int>((Func<Tuple<byte, int, int>, int>) (m => m.Item2)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.AECDEngineHoursTimer2, (object) list.Select<Tuple<byte, int, int>, int>((Func<Tuple<byte, int, int>, int>) (m => m.Item3)).ToArray<int>());
          break;
        }
        break;
      case RollCallJ1939.PGN.ScaledTestResults:
        if (data.Length > 6)
        {
          List<RollCallJ1939.ScaledTestResult> list = this.ExtractScaledTestResults(address, ((IEnumerable<byte>) data).Skip<byte>(6)).ToList<RollCallJ1939.ScaledTestResult>();
          this.AddIdentification(address, RollCall.ID.TestIdentifier, (object) list.Select<RollCallJ1939.ScaledTestResult, int>((Func<RollCallJ1939.ScaledTestResult, int>) (s => s.TestIdentifier)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.SuspectParameterNumber, (object) list.Select<RollCallJ1939.ScaledTestResult, int>((Func<RollCallJ1939.ScaledTestResult, int>) (s => s.SPN)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.FailureModeIdentifier, (object) list.Select<RollCallJ1939.ScaledTestResult, byte>((Func<RollCallJ1939.ScaledTestResult, byte>) (s => s.FMI)).ToArray<byte>());
          this.AddIdentification(address, RollCall.ID.TestValue, (object) list.Select<RollCallJ1939.ScaledTestResult, object>((Func<RollCallJ1939.ScaledTestResult, object>) (s => s.TestValue)).ToArray<object>());
          this.AddIdentification(address, RollCall.ID.TestLimitMaximum, (object) list.Select<RollCallJ1939.ScaledTestResult, object>((Func<RollCallJ1939.ScaledTestResult, object>) (s => s.TestLimitMaximum)).ToArray<object>());
          this.AddIdentification(address, RollCall.ID.TestLimitMinimum, (object) list.Select<RollCallJ1939.ScaledTestResult, object>((Func<RollCallJ1939.ScaledTestResult, object>) (s => s.TestLimitMinimum)).ToArray<object>());
          this.AddIdentification(address, RollCall.ID.SlotIdentifier, (object) list.Select<RollCallJ1939.ScaledTestResult, int>((Func<RollCallJ1939.ScaledTestResult, int>) (s => s.SlotIdentifier)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.UnitSystem, (object) list.Select<RollCallJ1939.ScaledTestResult, string>((Func<RollCallJ1939.ScaledTestResult, string>) (s => s.Units)).ToArray<string>());
          break;
        }
        break;
      case RollCallJ1939.PGN.MonitorPerformanceRatio:
        if (data.Length > 9)
        {
          List<Tuple<int, int, int>> list = RollCallJ1939.ExtractMonitorRatio(((IEnumerable<byte>) data).Skip<byte>(10).ToArray<byte>()).ToList<Tuple<int, int, int>>();
          this.AddIdentification(address, RollCall.ID.SPNOfApplicableSystemMonitor, (object) list.Select<Tuple<int, int, int>, int>((Func<Tuple<int, int, int>, int>) (m => m.Item1)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.ApplicableSystemMonitorNumerator, (object) list.Select<Tuple<int, int, int>, int>((Func<Tuple<int, int, int>, int>) (m => m.Item2)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.ApplicableSystemMonitorDenominator, (object) list.Select<Tuple<int, int, int>, int>((Func<Tuple<int, int, int>, int>) (m => m.Item3)).ToArray<int>());
          break;
        }
        break;
      case RollCallJ1939.PGN.Acknowledgement:
        if (data.Length > 13)
        {
          int negRespPGN = RollCallJ1939.ExtractPGN(data, 11);
          RollCallSae.Acknowledgment acknowledgement = (RollCallSae.Acknowledgment) data[6];
          if (this.debugLevel > (ushort) 1)
          {
            IEnumerable<RollCall.ID> source = this.GetIdentificationIds(new byte?(address)).Where<RollCall.ID>((Func<RollCall.ID, bool>) (p => this.MapIdToRequestId(p) == negRespPGN));
            this.RaiseDebugInfoEvent((int) address, $"ECU ACK {(object) acknowledgement} for {RollCallJ1939.GetIdString(negRespPGN)} {string.Join(", ", source.Select<RollCall.ID, string>((Func<RollCall.ID, string>) (p => p.ToString())).ToArray<string>())}");
          }
          this.NotifyQueueItem(negRespPGN, address, (byte[]) null, acknowledgement);
          break;
        }
        break;
      case RollCallJ1939.PGN.SPNSupport:
        if (data.Length > 6)
        {
          List<RollCallJ1939.SPNSupport> list = RollCallJ1939.ExtractSPNSupport(((IEnumerable<byte>) data).Skip<byte>(6)).ToList<RollCallJ1939.SPNSupport>();
          this.AddIdentification(address, RollCall.ID.SPNSupported, (object) list.Select<RollCallJ1939.SPNSupport, int>((Func<RollCallJ1939.SPNSupport, int>) (s => s.SPN)).ToArray<int>());
          this.AddIdentification(address, RollCall.ID.SupportedInDataStream, (object) list.Select<RollCallJ1939.SPNSupport, bool>((Func<RollCallJ1939.SPNSupport, bool>) (s => s.SupportedInDataStream)).ToArray<bool>());
          this.AddIdentification(address, RollCall.ID.SupportedInExpandedFreezeFrame, (object) list.Select<RollCallJ1939.SPNSupport, bool>((Func<RollCallJ1939.SPNSupport, bool>) (s => s.SupportedInFreezeFrame)).ToArray<bool>());
          this.AddIdentification(address, RollCall.ID.SupportedInScaledTestResults, (object) list.Select<RollCallJ1939.SPNSupport, bool>((Func<RollCallJ1939.SPNSupport, bool>) (s => s.SupportedInScaledTests)).ToArray<bool>());
          this.AddIdentification(address, RollCall.ID.SPNDataLength, (object) list.Select<RollCallJ1939.SPNSupport, byte>((Func<RollCallJ1939.SPNSupport, byte>) (s => s.Length)).ToArray<byte>());
          this.SetDataStreamSpns(address, list.Where<RollCallJ1939.SPNSupport>((Func<RollCallJ1939.SPNSupport, bool>) (s => s.SupportedInDataStream)).Select<RollCallJ1939.SPNSupport, int>((Func<RollCallJ1939.SPNSupport, int>) (s => s.SPN)).ToArray<int>());
          break;
        }
        break;
      case RollCallJ1939.PGN.ExpandedFreezeFrame:
        if (channel != null)
        {
          channel.FaultCodes.UpdateSnapshotFromRollCall((IEnumerable<Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>>) this.ExtractSnapshot(channel, ((IEnumerable<byte>) data).Skip<byte>(6).ToArray<byte>()).ToList<Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>>());
          break;
        }
        break;
    }
    if (channel != null)
    {
      channel.Instruments.UpdateFromRollCall(id, new byte?(data[5]), ((IEnumerable<byte>) data).Skip<byte>(6).ToArray<byte>());
      channel.EcuInfos.UpdateFromRollCall(id, ((IEnumerable<byte>) data).Skip<byte>(6).ToArray<byte>());
    }
    this.NotifyQueueItem(id, address, data, RollCallSae.Acknowledgment.Positive);
  }

  private struct AddressClaim
  {
    internal readonly int IdentityNumber;
    internal readonly int ManufacturerCode;
    internal readonly byte Function;
    internal readonly byte FunctionInstance;
    internal readonly byte EcuInstance;
    internal readonly byte VehicleSystem;
    internal readonly byte VehicleSystemInstance;
    internal readonly byte IndustryGroup;
    internal readonly bool ArbitraryAddressCapable;

    internal static RollCallJ1939.AddressClaim FromByteArray(byte[] data)
    {
      return new RollCallJ1939.AddressClaim(data);
    }

    private AddressClaim(byte[] data)
      : this()
    {
      this.IdentityNumber = (int) data[6] + ((int) data[7] << 8) + (((int) data[8] & 31 /*0x1F*/) << 16 /*0x10*/);
      this.ManufacturerCode = ((int) data[8] >> 5) + ((int) data[9] << 3);
      this.Function = data[11];
      this.FunctionInstance = (byte) ((uint) data[10] >> 3);
      this.EcuInstance = (byte) ((uint) data[10] & 15U);
      this.VehicleSystem = (byte) ((uint) data[12] >> 1);
      this.VehicleSystemInstance = (byte) ((uint) data[13] & 15U);
      this.IndustryGroup = (byte) ((int) data[13] >> 4 & 7);
      this.ArbitraryAddressCapable = (uint) data[13] >> 7 > 0U;
    }
  }

  internal enum PGN : uint
  {
    ImmediateFaultStatus = 40704, // 0x00009F00
    AECDActiveTime = 41216, // 0x0000A100
    ScaledTestResults = 41984, // 0x0000A400
    MonitorPerformanceRatio = 49664, // 0x0000C200
    CalibrationInformation = 54016, // 0x0000D300
    ISO15765NormalFixedPhysical = 55808, // 0x0000DA00
    ISO15765NormalFixedFunctional = 56064, // 0x0000DB00
    CommandTestResults = 58112, // 0x0000E300
    Acknowledgement = 59392, // 0x0000E800
    RequestPGN = 59904, // 0x0000EA00
    TransportProtocolDataTransfer = 60160, // 0x0000EB00
    TransportProtocolConnectionManagement = 60416, // 0x0000EC00
    AddressClaim = 60928, // 0x0000EE00
    PermanentDiagnosticTroubleCodes = 64896, // 0x0000FD80
    EmissionRelatedPreviouslyActiveDiagnosticTroubleCodes = 64949, // 0x0000FDB5
    SPNSupport = 64950, // 0x0000FDB6
    ExpandedFreezeFrame = 64951, // 0x0000FDB7
    EcuIdentificationInformation = 64965, // 0x0000FDC5
    ActiveDiagnosticTroubleCodes = 65226, // 0x0000FECA
    PreviouslyActiveDiagnosticTroubleCodes = 65227, // 0x0000FECB
    ClearPreviouslyActiveDiagnosticTroubleCodes = 65228, // 0x0000FECC
    DiagnosticReadiness = 65230, // 0x0000FECE
    PendingDiagnosticTroubleCodes = 65231, // 0x0000FECF
    ClearActiveDiagnosticTroubleCodes = 65235, // 0x0000FED3
    EmissionRelatedActiveDiagnosticTroubleCodes = 65236, // 0x0000FED4
    SoftwareIdentification = 65242, // 0x0000FEDA
    ComponentIdentification = 65259, // 0x0000FEEB
    VehicleIdentification = 65260, // 0x0000FEEC
  }

  private struct SPNSupport(byte[] item)
  {
    public readonly int SPN = (int) item[0] + ((int) item[1] << 8) + (((int) item[2] & 224 /*0xE0*/) << 11);
    public readonly bool SupportedInDataStream = ((int) item[2] >> 1 & 1) == 0;
    public readonly bool SupportedInFreezeFrame = ((int) item[2] & 1) == 0;
    public readonly bool SupportedInScaledTests = ((int) item[2] >> 2 & 1) == 0;
    public readonly byte Length = item[3];
  }

  private struct ScaledTestResult
  {
    public readonly int TestIdentifier;
    public readonly int SPN;
    public readonly byte FMI;
    public readonly int SlotIdentifier;
    public readonly object TestValue;
    public readonly object TestLimitMaximum;
    public readonly object TestLimitMinimum;
    public readonly string Units;

    public ScaledTestResult(byte[] item)
    {
      this.TestIdentifier = (int) item[0];
      this.SPN = (int) item[1] + ((int) item[2] << 8) + (((int) item[3] & 224 /*0xE0*/) << 11);
      this.FMI = (byte) ((uint) item[3] & 31U /*0x1F*/);
      this.SlotIdentifier = (int) item[4] + ((int) item[5] << 8);
      this.TestValue = (object) null;
      this.TestLimitMaximum = (object) null;
      this.TestLimitMinimum = (object) null;
      Presentation baseInstrument = (Presentation) RollCallJ1939.GlobalInstance.CreateBaseInstrument((Channel) null, "SLOT_" + this.SlotIdentifier.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.Units = baseInstrument.Units;
      switch ((int) item[6] + ((int) item[7] << 8))
      {
        case 64256:
          this.TestValue = (object) "Test Not Complete";
          break;
        case 64257:
          this.TestValue = (object) "Test Can Not Be Performed";
          break;
        case 65024:
          this.TestValue = (object) "Error";
          break;
        default:
          this.TestValue = baseInstrument.GetPresentation(((IEnumerable<byte>) item).Skip<byte>(6).Take<byte>(2).ToArray<byte>());
          this.TestLimitMaximum = item[8] != byte.MaxValue || item[9] != byte.MaxValue ? baseInstrument.GetPresentation(((IEnumerable<byte>) item).Skip<byte>(8).Take<byte>(2).ToArray<byte>()) : (object) "No limit";
          this.TestLimitMinimum = item[10] != byte.MaxValue || item[11] != byte.MaxValue ? baseInstrument.GetPresentation(((IEnumerable<byte>) item).Skip<byte>(10).Take<byte>(2).ToArray<byte>()) : (object) "No limit";
          break;
      }
    }
  }
}
