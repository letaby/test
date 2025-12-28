using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using J2534;

namespace SapiLayer1;

internal sealed class RollCallJ1708 : RollCallSae
{
	private enum PID : uint
	{
		GlobalParameterRequest = 0u,
		ComponentSpecificParameterRequest = 128u,
		MultiSectionParameter = 192u,
		DiagnosticCodeTable = 194u,
		DiagnosticDataClearCount = 195u,
		DiagnosticDataClearCountResponse = 196u,
		ComponentIdentification = 243u,
		SoftwareIdentification = 234u,
		VehicleIdentification = 237u
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
		Security = 10,
		Tire = 11,
		ParticulateTrap = 12,
		RefrigerantManagement = 13,
		TractionTrailerBridge = 14,
		CollisionAvoidance = 15,
		DrivelineRetarder = 16,
		SafetyRestraintSystem = 17,
		TransmsissionAGS2 = 19,
		ForwardRoadImageProcessor = 20,
		BrakeStrokeAlert = 21,
		VehicleSensorsToDataConverter = 22,
		ParkBrakeController = 23
	}

	private static Dictionary<MidGroup, List<byte>> midGroupMapping;

	private static RollCallJ1708 globalInstance = new RollCallJ1708();

	private static IEnumerable<int> cycleGlobalRequestIds = new List<int> { 237, 243, 234, 194 };

	private Dictionary<PID, List<byte>> multiSectionData = new Dictionary<PID, List<byte>>();

	private static Dictionary<MidGroup, List<byte>> MidGroupMapping
	{
		get
		{
			if (midGroupMapping == null)
			{
				midGroupMapping = new Dictionary<MidGroup, List<byte>>();
				midGroupMapping.Add(MidGroup.Engine, new List<byte> { 128, 175, 183, 184, 185, 186 });
				midGroupMapping.Add(MidGroup.Transmission, new List<byte> { 130, 176, 223 });
				midGroupMapping.Add(MidGroup.Brakes, new List<byte> { 136, 137, 138, 139, 246, 247 });
				midGroupMapping.Add(MidGroup.InstrumentPanel, new List<byte> { 140, 234 });
				midGroupMapping.Add(MidGroup.VehicleManagement, new List<byte> { 142, 187, 188 });
				midGroupMapping.Add(MidGroup.FuelSystem, new List<byte> { 143 });
				midGroupMapping.Add(MidGroup.ClimateControl, new List<byte> { 146, 200 });
				midGroupMapping.Add(MidGroup.Suspension, new List<byte> { 150, 151 });
				midGroupMapping.Add(MidGroup.ParkBrakeController, new List<byte> { 157 });
				midGroupMapping.Add(MidGroup.Navigation, new List<byte> { 162, 191 });
				midGroupMapping.Add(MidGroup.Security, new List<byte> { 163 });
				midGroupMapping.Add(MidGroup.Tire, new List<byte> { 166, 167, 168, 169, 186 });
				midGroupMapping.Add(MidGroup.ParticulateTrap, new List<byte> { 177 });
				midGroupMapping.Add(MidGroup.VehicleSensorsToDataConverter, new List<byte> { 178 });
				midGroupMapping.Add(MidGroup.RefrigerantManagement, new List<byte> { 190 });
				midGroupMapping.Add(MidGroup.TractionTrailerBridge, new List<byte> { 217, 218 });
				midGroupMapping.Add(MidGroup.CollisionAvoidance, new List<byte> { 219 });
				midGroupMapping.Add(MidGroup.DrivelineRetarder, new List<byte> { 222 });
				midGroupMapping.Add(MidGroup.SafetyRestraintSystem, new List<byte> { 232, 254 });
				midGroupMapping.Add(MidGroup.ForwardRoadImageProcessor, new List<byte> { 248 });
				midGroupMapping.Add(MidGroup.BrakeStrokeAlert, new List<byte> { 253 });
			}
			return midGroupMapping;
		}
	}

	internal static RollCallJ1708 GlobalInstance => globalInstance;

	public override IEnumerable<byte> PowertrainAddresses => new byte[2] { 128, 130 };

	protected override int BetweenGlobalIdRequestInterval => 2500;

	protected override uint BaudRate => 9600u;

	protected override int TotalMessagesPerSecond => 100;

	protected override byte GlobalRequestAddress => 0;

	protected override IEnumerable<int> CycleGlobalRequestIds => cycleGlobalRequestIds;

	private static MidGroup GetMidGroup(byte sourceAddress)
	{
		foreach (MidGroup key in MidGroupMapping.Keys)
		{
			if (midGroupMapping[key].Contains(sourceAddress))
			{
				return key;
			}
		}
		return MidGroup.None;
	}

	private RollCallJ1708()
		: base(Protocol.J1708)
	{
	}

	protected override PassThruMsg CreateRequestMessage(int id, byte destinationAddress)
	{
		byte[] data = ((destinationAddress == GlobalRequestAddress) ? new byte[3]
		{
			172,
			0,
			(byte)id
		} : new byte[4]
		{
			172,
			128,
			(byte)id,
			destinationAddress
		});
		if (debugLevel > 1)
		{
			string text = "CreateRequestMessage: " + (PID)id;
			if (debugLevel > 2)
			{
				text = text + " " + new Dump(data).ToString();
			}
			RaiseDebugInfoEvent(destinationAddress, text);
		}
		return new PassThruMsg(ProtocolId.J1708, 0u, 134217728u, 0u, 0u, data);
	}

	internal override string GetFaultText(Channel channel, string number, string mode)
	{
		string text = string.Empty;
		if (number.StartsWith("S", StringComparison.Ordinal) && int.TryParse(number.Substring(1), out var result) && (result < 151 || result > 255))
		{
			MidGroup midGroup = GetMidGroup(channel.SourceAddress.Value);
			byte b = ((midGroup != MidGroup.None) ? MidGroupMapping[midGroup][0] : channel.SourceAddress.Value);
			text = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(number, b.ToString(CultureInfo.InvariantCulture), "SPN"), string.Empty);
		}
		if (string.IsNullOrEmpty(text))
		{
			text = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(number, "SPN"), string.Empty);
		}
		return text + " - " + channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(mode, "FMI"), string.Empty);
	}

	internal override void ClearErrors(Channel channel)
	{
		byte[] data = new byte[6]
		{
			172,
			195,
			3,
			channel.SourceAddress.Value,
			0,
			128
		};
		if (debugLevel > 1)
		{
			string text = "ClearErrors: " + PID.DiagnosticDataClearCount;
			if (debugLevel > 2)
			{
				text = text + " " + new Dump(data).ToString();
			}
			RaiseDebugInfoEvent(channel.SourceAddress.Value, text);
		}
		QueueItem queueItem = new QueueItem(new PassThruMsg(ProtocolId.J1708, 0u, 134217728u, 0u, 0u, data), 196, channel.SourceAddress.Value);
		RequestAndWait(queueItem);
	}

	internal override byte[] ReadInstrument(Channel channel, byte[] data, int responseId, Predicate<Tuple<byte?, byte[]>> additionalResponseCheck, int responseTimeout)
	{
		int responseStart = (IsPage2(responseId) ? 3 : 2);
		uint num = data[0];
		data = data.Skip(1).ToArray();
		return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1708, 0u, num << 24, 0u, 0u, data), responseId, channel.SourceAddress.Value, (byte[] response) => additionalResponseCheck(new Tuple<byte?, byte[]>(null, response.Skip(responseStart).ToArray())), channel.Ecu.GetComParameter("CP_REQREPCOUNT", 1), channel.Ecu.GetComParameter("CP_P2_MAX", Math.Min(2000, responseTimeout)), channel.Ecu.GetComParameter("CP_REQREPCOUNTBUSY", 0), channel.Ecu.GetComParameter("CP_P2_EXT_TIMEOUT_BUSY", 0)))?.Skip(responseStart).ToArray();
	}

	internal override byte[] DoByteMessage(Channel channel, byte[] data, byte[] requiredResponse)
	{
		if (data.Length == 1)
		{
			return base.RequestAndWait(data[0], channel.SourceAddress.Value);
		}
		uint num = data[0];
		data = data.Skip(1).ToArray();
		if (requiredResponse == null || requiredResponse.Length == 0)
		{
			if ((data[1] == 128 && data.Length == 4) || (data[1] == 0 && data.Length == 3))
			{
				if (data[1] == 128 && data[3] != channel.SourceAddress.Value)
				{
					RaiseDebugInfoEvent(channel.SourceAddress.Value, "WARNING: byte message sent to a different address (" + data[3] + "). Response may not be received.");
				}
				return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1708, 0u, num << 24, 0u, 0u, data), data[2], channel.SourceAddress.Value));
			}
			PassThruMsg requestMessage = new PassThruMsg(ProtocolId.J1708, 0u, num << 24, 0u, 0u, data);
			J2534Error j2534Error = Write(requestMessage);
			if (j2534Error == J2534Error.NoError)
			{
				return null;
			}
			Sapi.GetSapi().RaiseDebugInfoEvent(channel, "Result from J2534.WriteMsgs is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
			throw new CaesarException(SapiError.CannotSendMessageToDevice);
		}
		if (requiredResponse.Length == 1)
		{
			return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1708, 0u, num << 24, 0u, 0u, data), requiredResponse[0], channel.SourceAddress.Value));
		}
		if (requiredResponse.Length == 2 && requiredResponse[0] == byte.MaxValue)
		{
			return RequestAndWait(new QueueItem(new PassThruMsg(ProtocolId.J1708, 0u, num << 24, 0u, 0u, data), requiredResponse[1] + 255, channel.SourceAddress.Value));
		}
		throw new CaesarException(SapiError.CannotSendMessageToDevice);
	}

	protected override bool RequiresFunction(byte address)
	{
		return false;
	}

	protected override IEnumerable<ID> GetIdentificationIds(byte? address)
	{
		return new List<ID>
		{
			ID.Make,
			ID.Model,
			ID.SerialNumber,
			ID.SoftwareIdentification,
			ID.VehicleIdentificationNumber
		};
	}

	private static bool IsPage2(int id)
	{
		return id > 255;
	}

	protected override int MapIdToRequestId(ID id)
	{
		switch (id)
		{
		case ID.Make:
		case ID.Model:
		case ID.SerialNumber:
			return 243;
		case ID.SoftwareIdentification:
			return 234;
		case ID.VehicleIdentificationNumber:
			return 237;
		default:
			return 0;
		}
	}

	private bool TryGetMultiSectionData(byte[] data, out PID responsePID, out byte[] responseData)
	{
		responsePID = PID.MultiSectionParameter;
		responseData = null;
		if (data.Length < 3)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Received Invalid length multi-section data, expected at least 3 bytes");
			return false;
		}
		byte b = data[0];
		if (data.Length < b + 1)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, string.Format(CultureInfo.InvariantCulture, "Received Invalid length multi-section data, expected {0} bytes, received {1}", b, data.Length - 1));
			return false;
		}
		PID pID = (PID)data[1];
		byte b2 = (byte)((data[2] & 0xF0) >> 4);
		byte b3 = (byte)(data[2] & 0xF);
		if (b3 == 0)
		{
			multiSectionData[pID] = new List<byte>();
			multiSectionData[pID].Add(b3);
			for (int i = 0; i < b - 2; i++)
			{
				multiSectionData[pID].Add(data[3 + i]);
			}
		}
		else
		{
			if (!multiSectionData.ContainsKey(pID) || multiSectionData[pID][0] != b3 - 1)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Received out of sequence PID 192: current section is " + b3 + " previous section is " + (multiSectionData.ContainsKey(pID) ? multiSectionData[pID][0].ToString(CultureInfo.InvariantCulture) : "<not found>"));
				return false;
			}
			multiSectionData[pID][0] = b3;
			for (int j = 0; j < b - 2; j++)
			{
				multiSectionData[pID].Add(data[3 + j]);
			}
		}
		if (b3 == b2)
		{
			responsePID = pID;
			responseData = multiSectionData[pID].Skip(1).ToArray();
			multiSectionData.Remove(pID);
			return true;
		}
		return false;
	}

	protected override bool TryExtractMessage(byte[] source, out byte address, out int id, out byte[] data)
	{
		if (source.Length < 2 || (source[1] == byte.MaxValue && source.Length < 3))
		{
			address = 0;
			id = 0;
			data = null;
			return false;
		}
		address = source[0];
		if (source[1] == byte.MaxValue)
		{
			id = source[2] + 256;
		}
		else
		{
			id = source[1];
		}
		data = source.ToArray();
		if (id == 192)
		{
			if (!TryGetMultiSectionData(data.Skip(2).ToArray(), out var responsePID, out var responseData))
			{
				return false;
			}
			id = (int)responsePID;
			data = new byte[2]
			{
				address,
				(byte)responsePID
			}.Concat(responseData).ToArray();
		}
		return true;
	}

	private static string GetIdString(int id)
	{
		if (Enum.IsDefined(typeof(PID), (uint)id))
		{
			return string.Concat("PID ", id, "(", (PID)id, ")");
		}
		return "PID " + id;
	}

	protected override void HandleIncomingMessage(byte address, int id, byte[] data, Channel channel)
	{
		if (debugLevel > 1 && (Enum.IsDefined(typeof(PID), (uint)id) || debugLevel > 3))
		{
			string text = "HandleIncomingMessage: " + GetIdString(id);
			if (debugLevel > 2)
			{
				text = text + ": " + new Dump(data).ToString();
			}
			RaiseDebugInfoEvent(address, text);
		}
		switch ((PID)id)
		{
		case PID.ComponentIdentification:
			if (data.Length > 4)
			{
				string text2 = Encoding.ASCII.GetString(data, 4, data.Length - 4);
				string[] array = new string[3]
				{
					string.Empty,
					string.Empty,
					string.Empty
				};
				text2.Split("*".ToCharArray()).Take(3).ToArray()
					.CopyTo(array, 0);
				AddIdentification(address, ID.Make, array[0]);
				AddIdentification(address, ID.Model, array[1]);
				AddIdentification(address, ID.SerialNumber, array[2]);
			}
			break;
		case PID.SoftwareIdentification:
			if (data.Length > 3)
			{
				string value2 = Encoding.ASCII.GetString(data, 3, data.Length - 3);
				AddIdentification(address, ID.SoftwareIdentification, value2);
			}
			break;
		case PID.VehicleIdentification:
			if (data.Length > 3)
			{
				string value3 = Encoding.ASCII.GetString(data, 3, data.Length - 3);
				AddIdentification(address, ID.VehicleIdentificationNumber, value3);
			}
			break;
		case PID.DiagnosticCodeTable:
		{
			if (channel == null)
			{
				break;
			}
			Dictionary<string, byte?> dictionary = new Dictionary<string, byte?>();
			Dictionary<string, byte?> dictionary2 = new Dictionary<string, byte?>();
			List<byte> list = data.Skip(3).ToList();
			while (list.Count > 1)
			{
				int num2 = list[0];
				int num3 = list[1] & 0xF;
				bool num4 = (list[1] & 0x10) != 0;
				bool flag = (list[1] & 0x20) == 0;
				bool flag2 = (list[1] & 0x40) == 0;
				bool flag3 = (list[1] & 0x80) != 0;
				string key = (num4 ? "S" : "P") + (num2 + (flag ? 256 : 0)).ToString(CultureInfo.InvariantCulture) + ":FMI" + num3.ToString(CultureInfo.InvariantCulture);
				byte? value = null;
				if (flag3 && list.Count > 2)
				{
					value = list[2];
				}
				if (flag2)
				{
					dictionary[key] = value;
				}
				else
				{
					dictionary2[key] = value;
				}
				list = list.Skip(flag3 ? 3 : 2).ToList();
			}
			channel.FaultCodes.UpdateFromRollCall(dictionary, typeof(ActiveStatus), TimeSpan.Zero);
			channel.FaultCodes.UpdateFromRollCall(dictionary2, typeof(TestFailedSinceLastClearStatus), TimeSpan.FromMilliseconds(ChannelTimeout));
			break;
		}
		default:
		{
			if (channel == null || channel.Instruments.Count <= 0)
			{
				break;
			}
			IEnumerable<byte> source = data.Skip((!IsPage2(id)) ? 1 : 2);
			while (source.Any())
			{
				byte b = source.First();
				source = source.Skip(1);
				if (source.Any())
				{
					int num = ((b < 128) ? 1 : ((b < 192) ? 2 : (source.First() + 1)));
					if (source.Count() < num)
					{
						break;
					}
					int id2 = b + (IsPage2(id) ? 256 : 0);
					channel.Instruments.UpdateFromRollCall(id2, null, source.Take(num).ToArray());
					channel.EcuInfos.UpdateFromRollCall(id2, source.Take(num).ToArray());
					source = source.Skip(num);
				}
			}
			break;
		}
		}
		NotifyQueueItem(id, address, data, Acknowledgment.Positive);
	}
}
