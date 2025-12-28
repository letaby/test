using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using J2534;

namespace SapiLayer1;

internal sealed class RollCallJ1939 : RollCallSae
{
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

		internal static AddressClaim FromByteArray(byte[] data)
		{
			return new AddressClaim(data);
		}

		private AddressClaim(byte[] data)
		{
			this = default(AddressClaim);
			IdentityNumber = data[6] + (data[7] << 8) + ((data[8] & 0x1F) << 16);
			ManufacturerCode = (data[8] >> 5) + (data[9] << 3);
			Function = data[11];
			FunctionInstance = (byte)(data[10] >> 3);
			EcuInstance = (byte)(data[10] & 0xF);
			VehicleSystem = (byte)(data[12] >> 1);
			VehicleSystemInstance = (byte)(data[13] & 0xF);
			IndustryGroup = (byte)((data[13] >> 4) & 7);
			ArbitraryAddressCapable = data[13] >> 7 != 0;
		}
	}

	internal enum PGN : uint
	{
		ImmediateFaultStatus = 40704u,
		AECDActiveTime = 41216u,
		ScaledTestResults = 41984u,
		MonitorPerformanceRatio = 49664u,
		CalibrationInformation = 54016u,
		ISO15765NormalFixedPhysical = 55808u,
		ISO15765NormalFixedFunctional = 56064u,
		CommandTestResults = 58112u,
		RequestPGN = 59904u,
		Acknowledgement = 59392u,
		TransportProtocolDataTransfer = 60160u,
		TransportProtocolConnectionManagement = 60416u,
		AddressClaim = 60928u,
		PermanentDiagnosticTroubleCodes = 64896u,
		EmissionRelatedPreviouslyActiveDiagnosticTroubleCodes = 64949u,
		SPNSupport = 64950u,
		ExpandedFreezeFrame = 64951u,
		EcuIdentificationInformation = 64965u,
		ActiveDiagnosticTroubleCodes = 65226u,
		PreviouslyActiveDiagnosticTroubleCodes = 65227u,
		ClearPreviouslyActiveDiagnosticTroubleCodes = 65228u,
		DiagnosticReadiness = 65230u,
		PendingDiagnosticTroubleCodes = 65231u,
		ClearActiveDiagnosticTroubleCodes = 65235u,
		EmissionRelatedActiveDiagnosticTroubleCodes = 65236u,
		SoftwareIdentification = 65242u,
		ComponentIdentification = 65259u,
		VehicleIdentification = 65260u
	}

	private struct SPNSupport
	{
		public readonly int SPN;

		public readonly bool SupportedInDataStream;

		public readonly bool SupportedInFreezeFrame;

		public readonly bool SupportedInScaledTests;

		public readonly byte Length;

		public SPNSupport(byte[] item)
		{
			SPN = item[0] + (item[1] << 8) + ((item[2] & 0xE0) << 11);
			SupportedInScaledTests = ((item[2] >> 2) & 1) == 0;
			SupportedInDataStream = ((item[2] >> 1) & 1) == 0;
			SupportedInFreezeFrame = (item[2] & 1) == 0;
			Length = item[3];
		}
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
			TestIdentifier = item[0];
			SPN = item[1] + (item[2] << 8) + ((item[3] & 0xE0) << 11);
			FMI = (byte)(item[3] & 0x1F);
			SlotIdentifier = item[4] + (item[5] << 8);
			TestValue = null;
			TestLimitMaximum = null;
			TestLimitMinimum = null;
			Presentation presentation = GlobalInstance.CreateBaseInstrument(null, "SLOT_" + SlotIdentifier.ToString(CultureInfo.InvariantCulture));
			Units = presentation.Units;
			switch (item[6] + (item[7] << 8))
			{
			case 65024:
				TestValue = "Error";
				return;
			case 64256:
				TestValue = "Test Not Complete";
				return;
			case 64257:
				TestValue = "Test Can Not Be Performed";
				return;
			}
			TestValue = presentation.GetPresentation(item.Skip(6).Take(2).ToArray());
			TestLimitMaximum = ((item[8] == byte.MaxValue && item[9] == byte.MaxValue) ? "No limit" : presentation.GetPresentation(item.Skip(8).Take(2).ToArray()));
			TestLimitMinimum = ((item[10] == byte.MaxValue && item[11] == byte.MaxValue) ? "No limit" : presentation.GetPresentation(item.Skip(10).Take(2).ToArray()));
		}
	}

	private const int HighestStandardSuspectParameterNumber = 5999;

	private const int PropAPDUFormat = 239;

	private const int DestinationAddressBytePos = 5;

	private const int DataStartBytePos = 6;

	private static RollCallJ1939 globalInstance = new RollCallJ1939();

	private static IEnumerable<int> cycleGlobalRequestIds = new List<int>
	{
		60928, 65260, 65259, 65242, 65231, 65226, 65227, 65236, 64949, 64896,
		40704
	};

	private static Dictionary<string, Type> IdentificationTypes = new Tuple<string, Type>[24]
	{
		new Tuple<string, Type>(ID.OnBoardDiagnosticCompliance.ToNumberString(), typeof(Choice)),
		new Tuple<string, Type>(ID.ManufacturerCode.ToNumberString(), typeof(Choice)),
		new Tuple<string, Type>(ID.Function.ToNumberString(), typeof(Choice)),
		new Tuple<string, Type>(ID.SPNOfApplicableSystemMonitor.ToNumberString(), typeof(Choice[])),
		new Tuple<string, Type>(ID.ApplicableSystemMonitorNumerator.ToNumberString(), typeof(int[])),
		new Tuple<string, Type>(ID.ApplicableSystemMonitorDenominator.ToNumberString(), typeof(int[])),
		new Tuple<string, Type>(ID.SPNSupported.ToNumberString(), typeof(Choice[])),
		new Tuple<string, Type>(ID.SupportedInDataStream.ToNumberString(), typeof(bool[])),
		new Tuple<string, Type>(ID.SupportedInExpandedFreezeFrame.ToNumberString(), typeof(bool[])),
		new Tuple<string, Type>(ID.SupportedInScaledTestResults.ToNumberString(), typeof(bool[])),
		new Tuple<string, Type>(ID.SPNDataLength.ToNumberString(), typeof(byte[])),
		new Tuple<string, Type>(ID.CalibrationInformation.ToNumberString(), typeof(string[])),
		new Tuple<string, Type>(ID.CalibrationVerificationNumber.ToNumberString(), typeof(string[])),
		new Tuple<string, Type>(ID.TestIdentifier.ToNumberString(), typeof(int[])),
		new Tuple<string, Type>(ID.SuspectParameterNumber.ToNumberString(), typeof(Choice[])),
		new Tuple<string, Type>(ID.FailureModeIdentifier.ToNumberString(), typeof(Choice[])),
		new Tuple<string, Type>(ID.TestValue.ToNumberString(), typeof(object[])),
		new Tuple<string, Type>(ID.TestLimitMaximum.ToNumberString(), typeof(object[])),
		new Tuple<string, Type>(ID.TestLimitMinimum.ToNumberString(), typeof(object[])),
		new Tuple<string, Type>(ID.SlotIdentifier.ToNumberString(), typeof(int[])),
		new Tuple<string, Type>(ID.UnitSystem.ToNumberString(), typeof(string[])),
		new Tuple<string, Type>(ID.AECDNumber.ToNumberString(), typeof(byte[])),
		new Tuple<string, Type>(ID.AECDEngineHoursTimer1.ToNumberString(), typeof(int[])),
		new Tuple<string, Type>(ID.AECDEngineHoursTimer2.ToNumberString(), typeof(int[]))
	}.ToDictionary((Tuple<string, Type> k) => k.Item1, (Tuple<string, Type> v) => v.Item2);

	private static Dictionary<string, string> IdentificationUnits = new Tuple<string, string>[2]
	{
		new Tuple<string, string>(ID.AECDEngineHoursTimer1.ToNumberString(), "min"),
		new Tuple<string, string>(ID.AECDEngineHoursTimer2.ToNumberString(), "min")
	}.ToDictionary((Tuple<string, string> k) => k.Item1, (Tuple<string, string> v) => v.Item2);

	private static Dictionary<string, Choice.TranslationQualifierType> TranslationQualifierTypes = new Tuple<string, Choice.TranslationQualifierType>[4]
	{
		new Tuple<string, Choice.TranslationQualifierType>(ID.SPNSupported.ToNumberString(), Choice.TranslationQualifierType.GlobalSpn),
		new Tuple<string, Choice.TranslationQualifierType>(ID.SPNOfApplicableSystemMonitor.ToNumberString(), Choice.TranslationQualifierType.GlobalSpn),
		new Tuple<string, Choice.TranslationQualifierType>(ID.SuspectParameterNumber.ToNumberString(), Choice.TranslationQualifierType.GlobalSpn),
		new Tuple<string, Choice.TranslationQualifierType>(ID.FailureModeIdentifier.ToNumberString(), Choice.TranslationQualifierType.GlobalFmi)
	}.ToDictionary((Tuple<string, Choice.TranslationQualifierType> k) => k.Item1, (Tuple<string, Choice.TranslationQualifierType> v) => v.Item2);

	internal static RollCallJ1939 GlobalInstance => globalInstance;

	public override IEnumerable<byte> PowertrainAddresses => new byte[7] { 0, 1, 3, 15, 17, 61, 90 };

	protected override int BetweenGlobalIdRequestInterval => 1000;

	protected override uint BaudRate
	{
		get
		{
			if (base.IsAutoBaudRate)
			{
				uint baudRate = 0u;
				J2534Error baudRate2 = Sid.GetBaudRate(channelId, ref baudRate);
				if (baudRate2 == J2534Error.NoError)
				{
					return baudRate;
				}
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Baud rate could not be retrieved: " + baudRate2);
			}
			return 250000u;
		}
	}

	protected override int TotalMessagesPerSecond => 1500;

	protected override byte GlobalRequestAddress => byte.MaxValue;

	protected override IEnumerable<int> CycleGlobalRequestIds => cycleGlobalRequestIds;

	private RollCallJ1939()
		: base(Protocol.J1939)
	{
	}

	private static bool IsGlobalRequest(int id)
	{
		byte b = BitConverter.GetBytes(id)[1];
		if (b >= 240)
		{
			return b <= byte.MaxValue;
		}
		return false;
	}

	private static bool IsPropA(int id)
	{
		return BitConverter.GetBytes(id)[1] == 239;
	}

	protected override PassThruMsg CreateRequestMessage(int id, byte destinationAddress)
	{
		byte[] data = ((id == 58112) ? new byte[14]
		{
			0, 227, 0, 6, 249, destinationAddress, 246, 214, 22, 31,
			255, 255, 255, 255
		} : new byte[9]
		{
			0,
			234,
			0,
			6,
			249,
			destinationAddress,
			(byte)((ulong)id & 0xFFuL),
			(byte)(((ulong)id & 0xFF00uL) >> 8),
			(byte)(((ulong)id & 0xFF000uL) >> 16)
		});
		if (debugLevel > 1)
		{
			string text = "CreateRequestMessage: " + (PGN)id;
			if (debugLevel > 2)
			{
				text = text + " " + new Dump(data).ToString();
			}
			RaiseDebugInfoEvent(destinationAddress, text);
		}
		return new PassThruMsg(ProtocolId.J1939, data);
	}

	protected override bool RequiresFunction(byte address)
	{
		if (address >= 128)
		{
			return address < byte.MaxValue;
		}
		return false;
	}

	protected override IEnumerable<ID> GetIdentificationIds(byte? address)
	{
		List<ID> list = new List<ID>
		{
			ID.ManufacturerCode,
			ID.Function,
			ID.Make,
			ID.Model,
			ID.SerialNumber,
			ID.UnitNumber,
			ID.SoftwareIdentification,
			ID.VehicleIdentificationNumber,
			ID.EcuPartNumber,
			ID.EcuSerialNumber,
			ID.EcuLocation,
			ID.EcuType,
			ID.EcuManufacturerName
		};
		if (address.HasValue && (address == 0 || address == 1 || address == 61))
		{
			list.AddRange(new ID[22]
			{
				ID.OnBoardDiagnosticCompliance,
				ID.CalibrationInformation,
				ID.CalibrationVerificationNumber,
				ID.SPNSupported,
				ID.SupportedInDataStream,
				ID.SupportedInExpandedFreezeFrame,
				ID.SupportedInScaledTestResults,
				ID.SPNDataLength,
				ID.SPNOfApplicableSystemMonitor,
				ID.ApplicableSystemMonitorNumerator,
				ID.ApplicableSystemMonitorDenominator,
				ID.TestIdentifier,
				ID.SuspectParameterNumber,
				ID.FailureModeIdentifier,
				ID.SlotIdentifier,
				ID.TestValue,
				ID.TestLimitMaximum,
				ID.TestLimitMinimum,
				ID.UnitSystem,
				ID.AECDNumber,
				ID.AECDEngineHoursTimer1,
				ID.AECDEngineHoursTimer2
			});
		}
		return list;
	}

	internal override bool IsRequestIdContentVisible(int id)
	{
		switch (id)
		{
		case 41216:
		case 41984:
		case 49664:
		case 58112:
		case 64950:
			return false;
		default:
			return true;
		}
	}

	protected override int MapIdToRequestId(ID id)
	{
		switch (id)
		{
		case ID.UnitNumber:
		case ID.Make:
		case ID.Model:
		case ID.SerialNumber:
			return 65259;
		case ID.SoftwareIdentification:
			return 65242;
		case ID.VehicleIdentificationNumber:
			return 65260;
		case ID.ManufacturerCode:
		case ID.Function:
			return 60928;
		case ID.CalibrationVerificationNumber:
		case ID.CalibrationInformation:
			return 54016;
		case ID.OnBoardDiagnosticCompliance:
			return 65230;
		case ID.SPNSupported:
		case ID.SupportedInExpandedFreezeFrame:
		case ID.SupportedInDataStream:
		case ID.SupportedInScaledTestResults:
		case ID.SPNDataLength:
			return 64950;
		case ID.SPNOfApplicableSystemMonitor:
		case ID.ApplicableSystemMonitorNumerator:
		case ID.ApplicableSystemMonitorDenominator:
			return 49664;
		case ID.SuspectParameterNumber:
		case ID.FailureModeIdentifier:
		case ID.TestIdentifier:
		case ID.SlotIdentifier:
		case ID.TestValue:
		case ID.TestLimitMaximum:
		case ID.TestLimitMinimum:
		case ID.UnitSystem:
			return 58112;
		case ID.AECDNumber:
		case ID.AECDEngineHoursTimer1:
		case ID.AECDEngineHoursTimer2:
			return 41216;
		case ID.EcuPartNumber:
		case ID.EcuSerialNumber:
		case ID.EcuLocation:
		case ID.EcuType:
		case ID.EcuManufacturerName:
			return 64965;
		default:
			return 0;
		}
	}

	private static void AddEcuInfoValueIfExisting(Channel channel, ID id, List<string> content)
	{
		EcuInfo ecuInfo = channel.EcuInfos[id.ToNumberString()];
		if (ecuInfo == null)
		{
			ecuInfo = channel.EcuInfos.FirstOrDefault((EcuInfo e) => e.Qualifier.IndexOf(id.ToNumberString(), StringComparison.Ordinal) != -1);
		}
		if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null && !string.IsNullOrEmpty(ecuInfo.EcuInfoValues.Current.Value.ToString().Trim()))
		{
			content.Add(ecuInfo.EcuInfoValues.Current.Value.ToString().Trim());
		}
	}

	private static void GetTranslationLookupsForComponents(IEnumerable<string> components, List<string> output)
	{
		for (int num = components.Count(); num > 1; num--)
		{
			output.Add(string.Join(".", components.Take(num).ToArray()) + ".SPN");
		}
	}

	internal override string GetFaultText(Channel channel, string number, string mode)
	{
		bool flag = false;
		if (int.TryParse(number, out var result) && result > 5999)
		{
			flag = true;
		}
		string text = string.Empty;
		if (flag)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			list.Add(number);
			if (channel.SourceAddress.HasValue && (!RequiresFunction(channel.SourceAddress.Value) || !channel.Ecu.Function.HasValue))
			{
				list.Add(channel.SourceAddress.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				list.Add("F" + channel.Ecu.Function.Value);
			}
			AddEcuInfoValueIfExisting(channel, ID.Make, list);
			AddEcuInfoValueIfExisting(channel, ID.Model, list);
			GetTranslationLookupsForComponents(list, list2);
			list.RemoveAt(1);
			GetTranslationLookupsForComponents(list, list2);
			list2.Add(Sapi.MakeTranslationIdentifier(number, "SPN"));
			for (int i = 0; i < list2.Count(); i++)
			{
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
				text = channel.Ecu.Translate(list2[i], string.Empty);
			}
		}
		else
		{
			text = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(number, "SPN"), string.Empty);
		}
		return text + " - " + channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(mode, "FMI"), string.Empty);
	}

	internal override void ClearErrors(Channel channel)
	{
		CaesarException ex = null;
		PGN[] array = new PGN[2]
		{
			PGN.ClearActiveDiagnosticTroubleCodes,
			PGN.ClearPreviouslyActiveDiagnosticTroubleCodes
		};
		foreach (PGN id in array)
		{
			try
			{
				RequestAndWait((int)id, channel.SourceAddress.Value);
			}
			catch (CaesarException ex2)
			{
				ex = ex2;
			}
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	private static int ExtractPGN(byte[] data, int offset)
	{
		return data[offset] + (data[offset + 1] << 8) + (data[offset + 2] << 16);
	}

	internal override bool IsSnapshotSupported(Channel channel)
	{
		if (channel.LogFile == null)
		{
			object identificationValue = GetIdentificationValue(channel.SourceAddress.Value, ID.SupportedInExpandedFreezeFrame.ToNumberString());
			if (identificationValue != null && identificationValue is bool[] source)
			{
				return source.Any((bool v) => v);
			}
			return false;
		}
		return GetCurrentSPNSupport(channel, ID.SupportedInExpandedFreezeFrame).Any();
	}

	private static IEnumerable<Tuple<Choice, byte>> GetCurrentSPNSupport(Channel channel, ID supportType)
	{
		Choice[] overallSPNs = channel.EcuInfos[ID.SPNSupported.ToNumberString()].GetCurrentArraySet<Choice>();
		bool[] supportDataForSPNs = channel.EcuInfos[supportType.ToNumberString()].GetCurrentArraySet<bool>();
		byte[] lengthForSPNs = channel.EcuInfos[ID.SPNDataLength.ToNumberString()].GetCurrentArraySet<byte>();
		for (int i = 0; i < overallSPNs.Length; i++)
		{
			if (supportDataForSPNs[i])
			{
				yield return new Tuple<Choice, byte>(overallSPNs[i], lengthForSPNs[i]);
			}
		}
	}

	private IEnumerable<Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>> ExtractSnapshot(Channel channel, byte[] frame)
	{
		IEnumerable<Tuple<Choice, byte>> spnsSupported = GetCurrentSPNSupport(channel, ID.SupportedInExpandedFreezeFrame);
		while (frame.Length > 4)
		{
			int freezeFrameLength = frame[0];
			if (freezeFrameLength > 0)
			{
				string text = frame.Skip(1).Take(3).ToArray()
					.ToHexString();
				if (text != "000000")
				{
					byte[] array = frame.Skip(5).ToArray();
					int num = spnsSupported.Sum((Tuple<Choice, byte> spn) => spn.Item2);
					if (freezeFrameLength != num + 4)
					{
						RaiseDebugInfoEvent(channel.SourceAddress.Value, "ExtractSnapshot: WARNING: DM25 specified length (" + freezeFrameLength + ") for frame '" + text + "' doesn't match what DM24 specified (" + num + " plus 4)");
					}
					List<Tuple<Instrument, byte[]>> list = new List<Tuple<Instrument, byte[]>>();
					foreach (Tuple<Choice, byte> item in spnsSupported)
					{
						if (item.Item2 > 0)
						{
							if (array.Length < item.Item2)
							{
								RaiseDebugInfoEvent(channel.SourceAddress.Value, string.Concat("ExtractSnapshot: WARNING: DM25 content too short; aborting at SPN ", item.Item1.RawValue, " '", item.Item1.Name, "'"));
								break;
							}
							Instrument rollCallSnapshotDescription = channel.FaultCodes.GetRollCallSnapshotDescription("DT_" + item.Item1.RawValue);
							list.Add(new Tuple<Instrument, byte[]>(rollCallSnapshotDescription, array.Take(item.Item2).ToArray()));
							if (rollCallSnapshotDescription.Type == typeof(Dump))
							{
								RaiseDebugInfoEvent(channel.SourceAddress.Value, string.Concat("ExtractSnapshot: don't have an instrument definition for SPN ", item.Item1.RawValue, " '", item.Item1.Name, "'"));
							}
							array = array.Skip(item.Item2).ToArray();
						}
					}
					yield return new Tuple<string, IEnumerable<Tuple<Instrument, byte[]>>>(text, list);
				}
				frame = frame.Skip(freezeFrameLength + 1).ToArray();
				continue;
			}
			RaiseDebugInfoEvent(channel.SourceAddress.Value, "ExtractSnapshot: DM25 specified freeze-frame length of zero");
			break;
		}
	}

	internal override byte[] ReadInstrument(Channel channel, byte[] data, int responseId, Predicate<Tuple<byte?, byte[]>> additionalResponseCheck, int responseTimeout)
	{
		return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1939, data), responseId, channel.SourceAddress.Value, (byte[] response) => additionalResponseCheck(new Tuple<byte?, byte[]>(response[5], response.Skip(6).ToArray())), channel.Ecu.GetComParameter("CP_REQREPCOUNT", 1), channel.Ecu.GetComParameter("CP_P2_MAX", Math.Min(2000, responseTimeout)), channel.Ecu.GetComParameter("CP_REQREPCOUNTBUSY", 0), channel.Ecu.GetComParameter("CP_P2_EXT_TIMEOUT_BUSY", 0)))?.Skip(6).ToArray();
	}

	internal override void ReadSnapshot(Channel channel)
	{
		RequestAndWait(64951, channel.SourceAddress.Value);
	}

	internal override void ReadFaultCodes(Channel channel)
	{
		RequestAndWait(65226, channel.SourceAddress.Value);
		RequestAndWait(65227, channel.SourceAddress.Value);
		if (channel.SourceAddress.Value == 0 || channel.SourceAddress.Value == 1 || channel.SourceAddress.Value == 61)
		{
			RequestAndWait(65236, channel.SourceAddress.Value);
			RequestAndWait(64949, channel.SourceAddress.Value);
			RequestAndWait(65231, channel.SourceAddress.Value);
			RequestAndWait(64896, channel.SourceAddress.Value);
			RequestAndWait(40704, channel.SourceAddress.Value);
		}
	}

	protected override byte[] RequestAndWait(int id, byte destinationAddress)
	{
		return RequestAndWait(new QueueItem(CreateRequestMessage(id, destinationAddress), (id == 58112) ? 41984 : id, destinationAddress));
	}

	internal override byte[] DoByteMessage(Channel channel, byte[] data, byte[] requiredResponse)
	{
		int num = ExtractPGN(data, 0);
		if (IsGlobalRequest(num) || IsPropA(num) || (num == 58112 && requiredResponse == null))
		{
			PassThruMsg requestMessage = new PassThruMsg(ProtocolId.J1939, data);
			J2534Error j2534Error = Write(requestMessage);
			if (j2534Error == J2534Error.NoError)
			{
				return null;
			}
			Sapi.GetSapi().RaiseDebugInfoEvent(channel, "ID " + num + ": Result from J2534.WriteMsgs is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
			throw new CaesarException(SapiError.CannotSendMessageToDevice);
		}
		if (data.Length == 3)
		{
			return RequestAndWait(ExtractPGN(data, 0), channel.SourceAddress.Value);
		}
		if (data.Length > 6)
		{
			if (data[5] != channel.SourceAddress.Value)
			{
				RaiseDebugInfoEvent(channel.SourceAddress.Value, "WARNING: byte message sent to a different address (" + data[5] + "). Response may not be received.");
			}
			if (requiredResponse == null || requiredResponse.Length == 0)
			{
				if (num == 59904 && data.Length == 9)
				{
					return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1939, data), ExtractPGN(data, 6), channel.SourceAddress.Value));
				}
			}
			else if (requiredResponse.Length == 3)
			{
				int responseId = requiredResponse[0] + (requiredResponse[1] << 8) + (requiredResponse[2] << 16);
				return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1939, data), responseId, channel.SourceAddress.Value));
			}
		}
		throw new CaesarException(SapiError.CannotSendMessageToDevice);
	}

	private static Dictionary<string, byte?> ExtractCodes(IList<byte> data)
	{
		Dictionary<string, byte?> dictionary = new Dictionary<string, byte?>();
		if (data.Count != 6 || !data.Take(3).SequenceEqual(new byte[3]))
		{
			for (int i = 0; i + 3 < data.Count; i += 4)
			{
				byte? value = null;
				if ((data[i + 3] & 0x80) != 128)
				{
					value = (byte)(data[i + 3] & 0x7F);
				}
				string key = new byte[3]
				{
					data[i],
					data[i + 1],
					data[i + 2]
				}.ToHexString();
				dictionary[key] = value;
			}
		}
		return dictionary;
	}

	private static string GetASCIIString(IEnumerable<byte> data, int offset, int length)
	{
		List<byte> list = new List<byte>();
		foreach (byte item in data.Skip(offset).Take(length))
		{
			if (item != 0)
			{
				list.Add((byte)((item > 31 && item < 127) ? item : 63));
				continue;
			}
			break;
		}
		return Encoding.ASCII.GetString(list.ToArray());
	}

	private static IEnumerable<KeyValuePair<string, Dump>> ExtractCalibrationInformation(IEnumerable<byte> data)
	{
		while (data.Count() > 0)
		{
			if (data.Count() >= 20)
			{
				Dump value = new Dump(data.Take(4).Reverse());
				string aSCIIString = GetASCIIString(data, 4, 16);
				yield return new KeyValuePair<string, Dump>(aSCIIString, value);
			}
			data = data.Skip(20);
		}
	}

	private static IEnumerable<SPNSupport> ExtractSPNSupport(IEnumerable<byte> data)
	{
		while (data.Count() >= 4)
		{
			yield return new SPNSupport(data.Take(4).ToArray());
			data = data.Skip(4);
		}
	}

	private IEnumerable<ScaledTestResult> ExtractScaledTestResults(byte sourceAddress, IEnumerable<byte> data)
	{
		while (data.Count() >= 12)
		{
			ScaledTestResult content = new ScaledTestResult(data.Take(12).ToArray());
			yield return content;
			if (content.TestValue.GetType() == typeof(Dump))
			{
				RaiseDebugInfoEvent(sourceAddress, "ExtractScaledTestResults: unable to locate definition for SLOT " + content.SlotIdentifier);
			}
			data = data.Skip(12);
		}
	}

	private static IEnumerable<Tuple<int, int, int>> ExtractMonitorRatio(byte[] data)
	{
		while (data.Length >= 7)
		{
			int num = data[0] + (data[1] << 8) + ((data[2] & 7) << 16);
			if (num != 524287)
			{
				int item = data[3] + (data[4] << 8);
				int item2 = data[5] + (data[6] << 8);
				yield return new Tuple<int, int, int>(num, item, item2);
			}
			data = data.Skip(7).ToArray();
		}
	}

	private static IEnumerable<Tuple<byte, int, int>> ExtractAECDTimers(byte[] data)
	{
		while (data.Length >= 9)
		{
			byte item = data[0];
			int item2 = data[1] + (data[2] << 8) + (data[3] << 16) + (data[4] << 24);
			int item3 = data[5] + (data[6] << 8) + (data[7] << 16) + (data[8] << 24);
			yield return new Tuple<byte, int, int>(item, item2, item3);
			data = data.Skip(9).ToArray();
		}
	}

	protected override bool TryExtractMessage(byte[] source, out byte address, out int id, out byte[] data)
	{
		address = source[4];
		id = ExtractPGN(source, 0);
		data = source;
		int num = id;
		if (num == 55808 || num == 56064)
		{
			return false;
		}
		if (address == 254)
		{
			return false;
		}
		return true;
	}

	private static string GetIdString(int id)
	{
		if (Enum.IsDefined(typeof(PGN), (uint)id))
		{
			return string.Concat("PGN ", id, "(", (PGN)id, ")");
		}
		return "PGN " + id;
	}

	private static Type GetFaultStatusType(PGN pgn)
	{
		return pgn switch
		{
			PGN.ActiveDiagnosticTroubleCodes => typeof(ActiveStatus), 
			PGN.PreviouslyActiveDiagnosticTroubleCodes => typeof(TestFailedSinceLastClearStatus), 
			PGN.PendingDiagnosticTroubleCodes => typeof(PendingStatus), 
			PGN.EmissionRelatedActiveDiagnosticTroubleCodes => typeof(MilStatus), 
			PGN.EmissionRelatedPreviouslyActiveDiagnosticTroubleCodes => typeof(StoredStatus), 
			PGN.ImmediateFaultStatus => typeof(ImmediateStatus), 
			_ => null, 
		};
	}

	protected override Presentation CreatePresentation(Ecu ecu, string qualifier)
	{
		if (IdentificationTypes.TryGetValue(qualifier, out var value))
		{
			ChoiceCollection choices = null;
			if (value == typeof(Choice[]) || value == typeof(Choice))
			{
				if (!TranslationQualifierTypes.TryGetValue(qualifier, out var value2))
				{
					value2 = Choice.TranslationQualifierType.Standard;
				}
				choices = new ChoiceCollection(ecu, qualifier, dynamicCreate: true, value2);
			}
			string value3 = null;
			IdentificationUnits.TryGetValue(qualifier, out value3);
			return new Presentation(ecu, "PRES_" + qualifier, choices, value, value3);
		}
		return null;
	}

	internal static bool TryGetIdentificationInformation(int id, byte[] data, out List<IdentificationInformation> ids)
	{
		ids = null;
		List<IdentificationInformation> foundIds = new List<IdentificationInformation>();
		bool result = false;
		switch ((PGN)id)
		{
		case PGN.AddressClaim:
			if (data.Length > 13)
			{
				AddressClaim addressClaim = AddressClaim.FromByteArray(data);
				AddIdentification(ID.ManufacturerCode, addressClaim.ManufacturerCode);
				int num = addressClaim.Function;
				if (num > 127)
				{
					num = ((addressClaim.IndustryGroup << 7) + addressClaim.VehicleSystem << 8) + num;
				}
				AddIdentification(ID.Function, num);
				result = true;
			}
			break;
		case PGN.ComponentIdentification:
			if (data.Length > 6)
			{
				string aSCIIString = GetASCIIString(data, 6, data.Length - 6);
				string[] array = new string[4]
				{
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
				aSCIIString.Split("*".ToCharArray()).Take(4).ToArray()
					.CopyTo(array, 0);
				AddIdentification(ID.Make, array[0]);
				AddIdentification(ID.Model, array[1]);
				AddIdentification(ID.SerialNumber, array[2]);
				AddIdentification(ID.UnitNumber, array[3]);
				result = true;
			}
			break;
		case PGN.EcuIdentificationInformation:
			if (data.Length > 6)
			{
				string aSCIIString2 = GetASCIIString(data, 6, data.Length - 6);
				string[] array2 = new string[5]
				{
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
				aSCIIString2.Split("*".ToCharArray()).Take(5).ToArray()
					.CopyTo(array2, 0);
				AddIdentification(ID.EcuPartNumber, array2[0]);
				AddIdentification(ID.EcuSerialNumber, array2[1]);
				AddIdentification(ID.EcuLocation, array2[2]);
				AddIdentification(ID.EcuType, array2[3]);
				AddIdentification(ID.EcuManufacturerName, array2[4]);
				result = true;
			}
			break;
		case PGN.SoftwareIdentification:
			if (data.Length > 7)
			{
				AddIdentification(ID.SoftwareIdentification, GetASCIIString(data, 7, data.Length - 7));
				result = true;
			}
			break;
		case PGN.VehicleIdentification:
			if (data.Length > 6)
			{
				AddIdentification(ID.VehicleIdentificationNumber, GetASCIIString(data, 6, Math.Min(data.Length - 6, 17)));
				result = true;
			}
			break;
		case PGN.CalibrationInformation:
			if (data.Length > 6)
			{
				List<KeyValuePair<string, Dump>> source = ExtractCalibrationInformation(data.Skip(6)).ToList();
				AddIdentification(ID.CalibrationInformation, source.Select((KeyValuePair<string, Dump> p) => p.Key).ToArray());
				AddIdentification(ID.CalibrationVerificationNumber, source.Select((KeyValuePair<string, Dump> p) => p.Value.ToString()).ToArray());
				result = true;
			}
			break;
		case PGN.DiagnosticReadiness:
			if (data.Length > 13)
			{
				AddIdentification(ID.OnBoardDiagnosticCompliance, data[8]);
				result = true;
			}
			break;
		}
		ids = foundIds;
		return result;
		void AddIdentification(ID foundId, object value)
		{
			foundIds.Add(new IdentificationInformation(foundId)
			{
				Value = value
			});
		}
	}

	protected override void HandleIncomingMessage(byte address, int id, byte[] data, Channel channel)
	{
		if (debugLevel > 1 && (Enum.IsDefined(typeof(PGN), (uint)id) || debugLevel > 3))
		{
			string text = "HandleIncomingMessage: " + GetIdString(id);
			if (debugLevel > 2)
			{
				text = text + ": " + new Dump(data).ToString();
			}
			RaiseDebugInfoEvent(address, text);
		}
		if (TryGetIdentificationInformation(id, data, out var ids))
		{
			ids.ForEach(delegate(IdentificationInformation fi)
			{
				AddIdentification(address, fi.Id, fi.Value);
			});
		}
		switch ((PGN)id)
		{
		case PGN.Acknowledgement:
		{
			if (data.Length <= 13)
			{
				break;
			}
			int negRespPGN = ExtractPGN(data, 11);
			Acknowledgment acknowledgment = (Acknowledgment)data[6];
			if (debugLevel > 1)
			{
				IEnumerable<ID> source5 = from p in GetIdentificationIds(address)
					where MapIdToRequestId(p) == negRespPGN
					select p;
				RaiseDebugInfoEvent(address, string.Concat("ECU ACK ", acknowledgment, " for ", GetIdString(negRespPGN), " ", string.Join(", ", source5.Select((ID p) => p.ToString()).ToArray())));
			}
			NotifyQueueItem(negRespPGN, address, null, acknowledgment);
			break;
		}
		case PGN.SPNSupport:
			if (data.Length > 6)
			{
				List<SPNSupport> source4 = ExtractSPNSupport(data.Skip(6)).ToList();
				AddIdentification(address, ID.SPNSupported, source4.Select((SPNSupport s) => s.SPN).ToArray());
				AddIdentification(address, ID.SupportedInDataStream, source4.Select((SPNSupport s) => s.SupportedInDataStream).ToArray());
				AddIdentification(address, ID.SupportedInExpandedFreezeFrame, source4.Select((SPNSupport s) => s.SupportedInFreezeFrame).ToArray());
				AddIdentification(address, ID.SupportedInScaledTestResults, source4.Select((SPNSupport s) => s.SupportedInScaledTests).ToArray());
				AddIdentification(address, ID.SPNDataLength, source4.Select((SPNSupport s) => s.Length).ToArray());
				SetDataStreamSpns(address, (from s in source4
					where s.SupportedInDataStream
					select s.SPN).ToArray());
			}
			break;
		case PGN.MonitorPerformanceRatio:
			if (data.Length > 9)
			{
				List<Tuple<int, int, int>> source3 = ExtractMonitorRatio(data.Skip(10).ToArray()).ToList();
				AddIdentification(address, ID.SPNOfApplicableSystemMonitor, source3.Select((Tuple<int, int, int> m) => m.Item1).ToArray());
				AddIdentification(address, ID.ApplicableSystemMonitorNumerator, source3.Select((Tuple<int, int, int> m) => m.Item2).ToArray());
				AddIdentification(address, ID.ApplicableSystemMonitorDenominator, source3.Select((Tuple<int, int, int> m) => m.Item3).ToArray());
			}
			break;
		case PGN.AECDActiveTime:
			if (data.Length > 6)
			{
				List<Tuple<byte, int, int>> source2 = ExtractAECDTimers(data.Skip(6).ToArray()).ToList();
				AddIdentification(address, ID.AECDNumber, source2.Select((Tuple<byte, int, int> m) => m.Item1).ToArray());
				AddIdentification(address, ID.AECDEngineHoursTimer1, source2.Select((Tuple<byte, int, int> m) => m.Item2).ToArray());
				AddIdentification(address, ID.AECDEngineHoursTimer2, source2.Select((Tuple<byte, int, int> m) => m.Item3).ToArray());
			}
			break;
		case PGN.ScaledTestResults:
			if (data.Length > 6)
			{
				List<ScaledTestResult> source = ExtractScaledTestResults(address, data.Skip(6)).ToList();
				AddIdentification(address, ID.TestIdentifier, source.Select((ScaledTestResult s) => s.TestIdentifier).ToArray());
				AddIdentification(address, ID.SuspectParameterNumber, source.Select((ScaledTestResult s) => s.SPN).ToArray());
				AddIdentification(address, ID.FailureModeIdentifier, source.Select((ScaledTestResult s) => s.FMI).ToArray());
				AddIdentification(address, ID.TestValue, source.Select((ScaledTestResult s) => s.TestValue).ToArray());
				AddIdentification(address, ID.TestLimitMaximum, source.Select((ScaledTestResult s) => s.TestLimitMaximum).ToArray());
				AddIdentification(address, ID.TestLimitMinimum, source.Select((ScaledTestResult s) => s.TestLimitMinimum).ToArray());
				AddIdentification(address, ID.SlotIdentifier, source.Select((ScaledTestResult s) => s.SlotIdentifier).ToArray());
				AddIdentification(address, ID.UnitSystem, source.Select((ScaledTestResult s) => s.Units).ToArray());
			}
			break;
		case PGN.ExpandedFreezeFrame:
			channel?.FaultCodes.UpdateSnapshotFromRollCall(ExtractSnapshot(channel, data.Skip(6).ToArray()).ToList());
			break;
		case PGN.ImmediateFaultStatus:
		case PGN.PermanentDiagnosticTroubleCodes:
		case PGN.EmissionRelatedPreviouslyActiveDiagnosticTroubleCodes:
		case PGN.ActiveDiagnosticTroubleCodes:
		case PGN.PreviouslyActiveDiagnosticTroubleCodes:
		case PGN.PendingDiagnosticTroubleCodes:
		case PGN.EmissionRelatedActiveDiagnosticTroubleCodes:
			if (channel != null)
			{
				Dictionary<string, byte?> dictionary = ExtractCodes(data.Skip(8).ToList());
				if (debugLevel > 1)
				{
					RaiseDebugInfoEvent(address, string.Concat((PGN)id, " reports ", dictionary.Any() ? string.Join(", ", dictionary.Keys.ToArray()) : "no codes"));
				}
				channel.FaultCodes.UpdateFromRollCall(dictionary, GetFaultStatusType((PGN)id), id == 64896, TimeSpan.Zero);
			}
			break;
		}
		if (channel != null)
		{
			channel.Instruments.UpdateFromRollCall(id, data[5], data.Skip(6).ToArray());
			channel.EcuInfos.UpdateFromRollCall(id, data.Skip(6).ToArray());
		}
		NotifyQueueItem(id, address, data, Acknowledgment.Positive);
	}
}
