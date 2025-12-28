using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CaesarAbstraction;
using J2534;

namespace SapiLayer1;

public sealed class BusMonitorFrame
{
	[Flags]
	internal enum CustomFrameTypes : byte
	{
		None = 0,
		ChipState = 1,
		ErrorFrame = 2,
		BaudRate = 4
	}

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

	private CustomFrameTypes customFrameType;

	private bool isEthernet;

	public long Timestamp => timestamp;

	public long Address => monitorAddress;

	public IEnumerable<byte> Data => data;

	public int Channel => channel;

	public ByteMessageDirection Direction
	{
		get
		{
			if (isEthernet)
			{
				BusMonitorFrameType frameType = FrameType;
				if (frameType != BusMonitorFrameType.RoutingActivationRequest && frameType != BusMonitorFrameType.VehicleIdentificationRequest && (frameType != BusMonitorFrameType.SingleFrame || (data[12] & 0x40) != 0))
				{
					return ByteMessageDirection.RX;
				}
				return ByteMessageDirection.TX;
			}
			if (IsIso)
			{
				lock (addressEcuMapLock)
				{
					if (addressEcuMap.TryGetValue(monitorAddress, out var value))
					{
						return value.Item2;
					}
				}
				return ByteMessageDirection.RX;
			}
			if (SourceAddress != 249)
			{
				return ByteMessageDirection.RX;
			}
			return ByteMessageDirection.TX;
		}
	}

	public bool IsNegativeResponse
	{
		get
		{
			if (!IsFrameTypeCustom)
			{
				if (IsIso)
				{
					int? isoFirstDataBytePos = IsoFirstDataBytePos;
					if (isoFirstDataBytePos.HasValue)
					{
						if (data.Length > isoFirstDataBytePos.Value)
						{
							return data[isoFirstDataBytePos.Value] == 127;
						}
						return false;
					}
				}
				else if (ParameterGroupNumber == 59392)
				{
					if (data.Length != 0)
					{
						if (data[0] != 0)
						{
							return data[0] != 128;
						}
						return false;
					}
					return false;
				}
			}
			return false;
		}
	}

	public bool IsFrameTypeCustom => customFrameType != CustomFrameTypes.None;

	public BusMonitorFrameType FrameType
	{
		get
		{
			if ((customFrameType & CustomFrameTypes.ChipState) != CustomFrameTypes.None)
			{
				return BusMonitorFrameType.ChipState;
			}
			if ((customFrameType & CustomFrameTypes.ErrorFrame) != CustomFrameTypes.None)
			{
				return BusMonitorFrameType.ErrorFrame;
			}
			if ((customFrameType & CustomFrameTypes.BaudRate) != CustomFrameTypes.None)
			{
				return BusMonitorFrameType.BaudRate;
			}
			if (isEthernet)
			{
				return (uint)((data[2] << 8) + data[3]) switch
				{
					1u => BusMonitorFrameType.VehicleIdentificationRequest, 
					4u => BusMonitorFrameType.VehicleAnnouncementMessage, 
					5u => BusMonitorFrameType.RoutingActivationRequest, 
					6u => BusMonitorFrameType.RoutingActivationResponse, 
					32769u => BusMonitorFrameType.SingleFrame, 
					32770u => BusMonitorFrameType.Acknowledgment, 
					_ => BusMonitorFrameType.Unknown, 
				};
			}
			if (IsIso)
			{
				return (BusMonitorFrameType)(data[0] >> 4);
			}
			switch (ParameterGroupNumber)
			{
			case 60416:
				switch (data[0])
				{
				case 16:
					return BusMonitorFrameType.RequestToSendDestinationSpecific;
				case 17:
					return BusMonitorFrameType.ClearToSendDestinationSpecific;
				case 19:
					return BusMonitorFrameType.EndOfMessageAcknowledge;
				case 32:
					return BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination;
				case byte.MaxValue:
					return BusMonitorFrameType.ConnectionAbort;
				}
				break;
			case 60160:
				return BusMonitorFrameType.TransportProtocolDataTransfer;
			}
			return BusMonitorFrameType.SingleFrame;
		}
	}

	public int? IsoFirstDataBytePos
	{
		get
		{
			if (isEthernet)
			{
				if (FrameType == BusMonitorFrameType.SingleFrame)
				{
					return 12;
				}
			}
			else
			{
				switch (FrameType)
				{
				case BusMonitorFrameType.SingleFrame:
					return ((data[0] & 0xF) != 0) ? 1 : 2;
				case BusMonitorFrameType.FirstFrame:
					return ((data[0] & 0xF) != 0 || data[1] != 0) ? 2 : 6;
				case BusMonitorFrameType.ConsecutiveFrame:
					return 1;
				}
			}
			return null;
		}
	}

	public int ActualDataLength
	{
		get
		{
			if (IsIso)
			{
				if (!IsoFirstDataBytePos.HasValue)
				{
					return 0;
				}
				return data.Length - IsoFirstDataBytePos.Value;
			}
			return FrameType switch
			{
				BusMonitorFrameType.SingleFrame => data.Length, 
				BusMonitorFrameType.TransportProtocolDataTransfer => data.Length - 1, 
				_ => 0, 
			};
		}
	}

	public bool IsContinuation
	{
		get
		{
			switch (FrameType)
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
			if (!IsFrameTypeCustom)
			{
				if (isEthernet)
				{
					return FrameType == BusMonitorFrameType.SingleFrame;
				}
				if ((byte)((monitorAddress & 0x1C000000) >> 26) == 6)
				{
					byte b = (byte)((monitorAddress & 0xFF0000) >> 16);
					if ((uint)(b - 205) <= 1u || (uint)(b - 218) <= 1u)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public byte SourceAddress => (byte)(monitorAddress & 0xFF);

	public byte DestinationAddress => (byte)((monitorAddress & 0xFF00) >> 8);

	public int ParameterGroupNumber
	{
		get
		{
			byte b = (byte)((monitorAddress & 0x2000000) >> 25);
			byte b2 = (byte)((monitorAddress & 0x1000000) >> 24);
			byte b3 = (byte)((monitorAddress & 0xFF0000) >> 16);
			byte b4 = (byte)((monitorAddress & 0xFF00) >> 8);
			if (b3 < 240)
			{
				return (b3 << 8) + (b2 << 17) + (b << 18);
			}
			return (b3 << 8) + (b2 << 17) + (b << 18) + b4;
		}
	}

	public int? TargetParameterGroupNumber
	{
		get
		{
			switch (FrameType)
			{
			case BusMonitorFrameType.SingleFrame:
				switch (ParameterGroupNumber)
				{
				case 59904:
					return (data[2] << 16) + (data[1] << 8) + data[0];
				case 59392:
					return (data[7] << 16) + (data[6] << 8) + data[5];
				}
				break;
			case BusMonitorFrameType.RequestToSendDestinationSpecific:
			case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
				return (data[7] << 16) + (data[6] << 8) + data[5];
			}
			return null;
		}
	}

	public long? EthernetSourceAddress
	{
		get
		{
			switch (FrameType)
			{
			case BusMonitorFrameType.VehicleAnnouncementMessage:
				return (uint)((data[25] << 8) | data[26]);
			case BusMonitorFrameType.SingleFrame:
			case BusMonitorFrameType.RoutingActivationRequest:
			case BusMonitorFrameType.Acknowledgment:
				return (uint)((data[8] << 8) | data[9]);
			case BusMonitorFrameType.RoutingActivationResponse:
				return (uint)((data[10] << 8) | data[11]);
			default:
				return null;
			}
		}
	}

	public long? EthernetTargetAddress
	{
		get
		{
			BusMonitorFrameType frameType = FrameType;
			if (frameType == BusMonitorFrameType.SingleFrame || frameType == BusMonitorFrameType.Acknowledgment)
			{
				return (uint)((data[10] << 8) | data[11]);
			}
			return null;
		}
	}

	public bool IsEthernet => isEthernet;

	internal byte? TotalPackets
	{
		get
		{
			if (FrameType != BusMonitorFrameType.RequestToSendDestinationSpecific)
			{
				return null;
			}
			return data[3];
		}
	}

	internal byte? SequenceNumber
	{
		get
		{
			if (FrameType != BusMonitorFrameType.TransportProtocolDataTransfer)
			{
				return null;
			}
			return data[0];
		}
	}

	public bool IsIdentificationInitiatingFrame
	{
		get
		{
			if (IsIso)
			{
				int? isoFirstDataBytePos = IsoFirstDataBytePos;
				if (isoFirstDataBytePos.HasValue && data[isoFirstDataBytePos.Value] == 98 && data[isoFirstDataBytePos.Value + 1] == 241 && data[isoFirstDataBytePos.Value + 2] == 0)
				{
					return true;
				}
				return false;
			}
			int? num = null;
			switch (FrameType)
			{
			case BusMonitorFrameType.SingleFrame:
				num = ParameterGroupNumber;
				break;
			case BusMonitorFrameType.RequestToSendDestinationSpecific:
			case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
				num = TargetParameterGroupNumber;
				break;
			}
			if (num.HasValue)
			{
				return IdentificationPGNs.Contains((RollCallJ1939.PGN)num.Value);
			}
			return false;
		}
	}

	private static uint ToUInt32(byte[] data, int offset)
	{
		return (uint)((data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + data[offset + 3]);
	}

	private static ushort ToUInt16(byte[] data, int offset)
	{
		return (ushort)((ushort)(data[offset] << 8) + data[offset + 1]);
	}

	public static BusMonitorFrame FromBinary(byte[] data, long objectType, long timestamp)
	{
		switch (objectType)
		{
		case 1L:
		case 86L:
		{
			ushort num3 = BitConverter.ToUInt16(data, 0);
			_ = data[2];
			byte b4 = data[3];
			uint num4 = BitConverter.ToUInt32(data, 4) & 0x1FFFFFFF;
			byte[] destinationArray = new byte[b4];
			Array.Copy(data, 8, destinationArray, 0, b4);
			return new BusMonitorFrame(timestamp, num3, num4, destinationArray);
		}
		case 73L:
		{
			ushort num5 = BitConverter.ToUInt16(data, 0);
			return new BusMonitorFrame(timestamp, num5, CustomFrameTypes.ErrorFrame, new byte[0]);
		}
		case 31L:
		{
			ushort num = BitConverter.ToUInt16(data, 0);
			byte b = data[2];
			byte b2 = data[3];
			uint num2 = BitConverter.ToUInt32(data, 4);
			byte b3 = (byte)num2;
			switch (num2)
			{
			case 103u:
				b3 = 2;
				break;
			case 104u:
				b3 = 8;
				break;
			case 115u:
				b3 = 4;
				break;
			}
			return new BusMonitorFrame(timestamp, num, CustomFrameTypes.ChipState, new byte[3] { b3, b, b2 });
		}
		default:
			throw new InvalidOperationException("Unknown object type");
		}
	}

	public static IEnumerable<BusMonitorFrame> FromBinary(byte[] packet, long timestamp)
	{
		ushort num = ToUInt16(packet, 12);
		int num2 = 0;
		byte b = 0;
		switch (num)
		{
		case 34525:
			b = packet[20];
			num2 = 54;
			break;
		case 2048:
			b = packet[23];
			num2 = 34;
			break;
		}
		if (num2 == 0)
		{
			yield break;
		}
		ushort num3 = ToUInt16(packet, num2);
		ushort num4 = ToUInt16(packet, num2 + 2);
		if (num3 != 13400 && num4 != 13400)
		{
			yield break;
		}
		int num5 = 0;
		switch (b)
		{
		case 17:
			num5 = num2 + 8;
			break;
		case 6:
			num5 = num2 + 20;
			break;
		}
		if (num5 == 0)
		{
			yield break;
		}
		foreach (BusMonitorFrame item in SplitDoIP(packet, num5, timestamp))
		{
			yield return item;
		}
	}

	private static IEnumerable<BusMonitorFrame> SplitDoIP(byte[] packet, int doipStartPos, long timestamp)
	{
		while (doipStartPos < packet.Length)
		{
			byte b = packet[doipStartPos];
			if (packet[doipStartPos + 1] == 255 - b)
			{
				if (packet.Length >= doipStartPos + 4 + 4)
				{
					uint num = ToUInt32(packet, doipStartPos + 4);
					uint num2 = (uint)(packet.Length - 8 - doipStartPos);
					if (num > num2)
					{
						Sapi.GetSapi().RaiseDebugInfoEvent("BusMonitorFrame", "Error! DoIP data at " + timestamp + " specifies length " + num + " greater than remaining data (" + num2 + "). Content wll be truncated.");
						num = num2;
					}
					byte[] doipFrame = new byte[num + 8];
					Array.Copy(packet, doipStartPos, doipFrame, 0, doipFrame.Length);
					yield return new BusMonitorFrame(timestamp, doipFrame);
					doipStartPos += doipFrame.Length;
					continue;
				}
				Sapi.GetSapi().RaiseDebugInfoEvent("BusMonitorFrame", "Error! DoIP data at " + timestamp + " is truncated such that the DoIP length cannot be determined. Available length for this frame was " + (packet.Length - doipStartPos));
				break;
			}
			break;
		}
	}

	internal static IEnumerable<BusMonitorFrame> FromDoIPString(string packet)
	{
		Match match = mvciRegex.Match(packet);
		if (!match.Success)
		{
			yield break;
		}
		long num = long.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
		byte[] packet2 = StringToByteArrayFastest(match.Groups[6].Value);
		foreach (BusMonitorFrame item in SplitDoIP(packet2, 0, num))
		{
			yield return item;
		}
	}

	public static BusMonitorFrame FromString(string frame, BusMonitorFrameStringFormat format, int? channel)
	{
		switch (format)
		{
		case BusMonitorFrameStringFormat.Mcd:
		{
			Match match4 = mvciRegex.Match(frame);
			if (match4.Success && match4.Groups[1].Value == "RX")
			{
				return new BusMonitorFrame(long.Parse(match4.Groups[2].Value, CultureInfo.InvariantCulture), monitorAddress: long.Parse(match4.Groups[5].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture), channel: channel.Value, data: match4.Groups[6].Value);
			}
			break;
		}
		case BusMonitorFrameStringFormat.Dts:
		{
			Match match5 = dtsRegex.Match(frame);
			if (match5.Success)
			{
				return new BusMonitorFrame(long.Parse(match5.Groups[3].Value, CultureInfo.InvariantCulture), monitorAddress: long.Parse(match5.Groups[2].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture), channel: channel.Value, data: match5.Groups[5].Value);
			}
			break;
		}
		case BusMonitorFrameStringFormat.Vector:
		{
			Match match = vectorRegex.Match(frame);
			if (match.Success)
			{
				double num = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
				int num2 = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
				long num3 = (long)(num * 1000000.0);
				long num4 = long.Parse(match.Groups[3].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				return new BusMonitorFrame(num3, num2, num4, match.Groups[5].Value);
			}
			Match match2 = vectorErrorFrameRegex.Match(frame);
			if (match2.Success)
			{
				double num5 = double.Parse(match2.Groups[1].Value, CultureInfo.InvariantCulture);
				int num6 = int.Parse(match2.Groups[2].Value, CultureInfo.InvariantCulture);
				return new BusMonitorFrame((long)(num5 * 1000000.0), num6, CustomFrameTypes.ErrorFrame, new byte[0]);
			}
			Match match3 = vectorChipstateRegex.Match(frame);
			if (!match3.Success)
			{
				break;
			}
			double num7 = double.Parse(match3.Groups[1].Value, CultureInfo.InvariantCulture);
			int num8 = int.Parse(match3.Groups[2].Value, CultureInfo.InvariantCulture);
			long num9 = (long)(num7 * 1000000.0);
			int num10 = 0;
			string text = match3.Groups[3].Value.Replace(" ", string.Empty);
			string[] names = Enum.GetNames(typeof(ChipStates));
			foreach (string value in names)
			{
				if (text.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1)
				{
					num10 |= (int)Enum.Parse(typeof(ChipStates), value);
				}
			}
			byte b = byte.Parse(match3.Groups[4].Value, CultureInfo.InvariantCulture);
			byte b2 = byte.Parse(match3.Groups[5].Value, CultureInfo.InvariantCulture);
			return new BusMonitorFrame(num9, num8, CustomFrameTypes.ChipState, new byte[3]
			{
				(byte)num10,
				b,
				b2
			});
		}
		}
		return null;
	}

	private BusMonitorFrame(long timestamp, byte[] data)
	{
		this.timestamp = timestamp;
		this.data = data;
		isEthernet = true;
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
		this.data = StringToByteArrayFastest(data.Trim());
	}

	private BusMonitorFrame(long timestamp, int channel, CustomFrameTypes customFrameType, byte[] data)
	{
		this.timestamp = timestamp;
		this.channel = channel;
		this.customFrameType = customFrameType;
		this.data = data;
	}

	private static byte[] StringToByteArrayFastest(string hex)
	{
		int num = (hex.Length + 1) / 3;
		byte[] array = new byte[num];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			array[i] = (byte)((GetHexVal(hex[num2++]) << 4) + GetHexVal(hex[num2++]));
			num2++;
		}
		return array;
	}

	internal static int GetHexVal(char hex)
	{
		return hex - ((hex < ':') ? 48 : ((hex < 'a') ? 55 : 87));
	}

	public string AsString(BusMonitorFrameStringFormat format)
	{
		string text = BitConverter.ToString(data).Replace('-', ' ');
		switch (format)
		{
		case BusMonitorFrameStringFormat.Mcd:
			if (!IsFrameTypeCustom)
			{
				return string.Format(CultureInfo.InvariantCulture, "RX {0:D20} .{1:x8} {2} {3} ", timestamp, monitorAddress, data.Length, text);
			}
			break;
		case BusMonitorFrameStringFormat.Dts:
			if (!IsFrameTypeCustom)
			{
				return string.Format(CultureInfo.InvariantCulture, "5000 {0,8:X} {1,10:D} {2:X4} {3} ", monitorAddress, timestamp, data.Length, text);
			}
			break;
		case BusMonitorFrameStringFormat.Vector:
		{
			double num = (double)timestamp / 1000000.0;
			if (!IsFrameTypeCustom)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0:X}x", monitorAddress).PadRight(15);
				return string.Format(CultureInfo.InvariantCulture, "{0,11:F6} {1}  {2} Rx   d {3} {4}", num, channel, text2, data.Length, text);
			}
			switch (FrameType)
			{
			case BusMonitorFrameType.ChipState:
			{
				string text3 = string.Empty;
				switch ((ChipStates)data[0])
				{
				case ChipStates.BusOff:
					text3 = "busoff       ";
					break;
				case ChipStates.ErrorActive:
					text3 = "error active ";
					break;
				case ChipStates.ErrorPassive:
					text3 = "error passive";
					break;
				case ChipStates.ErrorWarning:
					text3 = "error warning";
					break;
				}
				return string.Format(CultureInfo.InvariantCulture, "{0,11:F6} CAN {1} Status:chip status {2} - TxErr: {3} RxErr: {4}", num, channel, text3, data[1], data[2]);
			}
			case BusMonitorFrameType.ErrorFrame:
				return string.Format(CultureInfo.InvariantCulture, "{0,11:F6} {1}  ErrorFrame", num, channel);
			}
			break;
		}
		}
		return null;
	}

	internal BusMonitorFrame(PassThruMsg msg, int channelId)
	{
		byte[] array = msg.GetData();
		timestamp = msg.Timestamp;
		channel = channelId;
		customFrameType = (CustomFrameTypes)((msg.IsChipState ? 1 : 0) | (msg.IsErrorFrame ? 2 : 0) | (msg.IsBaudRate ? 4 : 0));
		if (customFrameType != CustomFrameTypes.None)
		{
			data = array;
		}
		else if (array.Length >= 4)
		{
			monitorAddress = (array[0] << 24) + (array[1] << 16) + (array[2] << 8) + array[3];
			data = new byte[array.Length - 4];
			Array.Copy(array, 4, data, 0, array.Length - 4);
		}
	}

	internal BusMonitorFrame(CanMonitoringEntry msg)
	{
		timestamp = Convert.ToInt64(msg.Time, CultureInfo.InvariantCulture);
		monitorAddress = Convert.ToInt64(msg.Identifier, CultureInfo.InvariantCulture);
		byte[] array = msg.GetData();
		data = new byte[array.Length];
		Array.Copy(array, data, array.Length);
	}

	internal static void ClearIdentifierMap()
	{
		lock (addressEcuMapLock)
		{
			if (addressEcuMap != null)
			{
				addressEcuMap.Clear();
				addressEcuMap = null;
			}
			udsProtocol = null;
		}
	}

	private static void InitializeIdentifierMap()
	{
		lock (addressEcuMapLock)
		{
			Sapi sapi = Sapi.GetSapi();
			addressEcuMap = new Dictionary<long, Tuple<Ecu[], ByteMessageDirection>>();
			ethernetAddressEcuMap = new Dictionary<Tuple<uint, uint>, Ecu[]>();
			List<Ecu> source = sapi.Ecus.Where((Ecu ecu) => (ecu.DiagnosisSource == DiagnosisSource.CaesarDatabase || ecu.DiagnosisSource == DiagnosisSource.McdDatabase) && ecu.RequestId.HasValue && ecu.ResponseId.HasValue).ToList();
			foreach (IGrouping<uint, Ecu> item in from e in source
				group e by e.RequestId.Value)
			{
				addressEcuMap[item.Key] = new Tuple<Ecu[], ByteMessageDirection>(item.ToArray(), ByteMessageDirection.TX);
			}
			foreach (IGrouping<uint, Ecu> item2 in from e in source
				group e by e.ResponseId.Value)
			{
				addressEcuMap[item2.Key] = new Tuple<Ecu[], ByteMessageDirection>(item2.ToArray(), ByteMessageDirection.RX);
			}
			foreach (IGrouping<byte, Ecu> item3 in from e in sapi.Ecus
				where e.DiagnosisSource == DiagnosisSource.RollCallDatabase && e.ProtocolName == "J1939" && e.SourceAddress.HasValue
				group e by e.SourceAddress.Value)
			{
				addressEcuMap[item3.Key] = new Tuple<Ecu[], ByteMessageDirection>(item3.ToArray(), ByteMessageDirection.RX);
			}
			foreach (IGrouping<Tuple<uint, uint>, Ecu> item4 in from e in sapi.Ecus
				where e.DiagnosisSource == DiagnosisSource.McdDatabase && e.LogicalEcuAddress.HasValue && e.LogicalTesterAddress.HasValue
				group e by Tuple.Create(e.LogicalEcuAddress.Value, e.LogicalTesterAddress.Value))
			{
				ethernetAddressEcuMap[item4.Key] = item4.ToArray();
			}
			udsProtocol = Sapi.GetSapi().DiagnosisProtocols["UDS"];
		}
	}

	public static void AddEcuCustomIdentifiers(Ecu ecu, long requestId, long responseId)
	{
		lock (addressEcuMapLock)
		{
			if (addressEcuMap == null)
			{
				InitializeIdentifierMap();
			}
			AddId(requestId, ByteMessageDirection.TX);
			AddId(responseId, ByteMessageDirection.RX);
		}
		void AddId(long id, ByteMessageDirection direction)
		{
			List<Ecu> list = Enumerable.Repeat(ecu, 1).ToList();
			if (addressEcuMap.TryGetValue(id, out var value))
			{
				list.AddRange(value.Item1);
			}
			addressEcuMap[id] = Tuple.Create(list.ToArray(), direction);
		}
	}

	public bool TryGetEcus(out Ecu[] ecus)
	{
		if (!IsFrameTypeCustom)
		{
			lock (addressEcuMapLock)
			{
				if (addressEcuMap == null)
				{
					InitializeIdentifierMap();
				}
				if (isEthernet)
				{
					switch (FrameType)
					{
					case BusMonitorFrameType.VehicleAnnouncementMessage:
					case BusMonitorFrameType.RoutingActivationResponse:
					case BusMonitorFrameType.Acknowledgment:
						ecus = (from ee in ethernetAddressEcuMap
							where ee.Key.Item1 == EthernetSourceAddress
							select ee.Value).FirstOrDefault();
						return ecus != null;
					case BusMonitorFrameType.SingleFrame:
						ecus = (from ee in ethernetAddressEcuMap
							where ee.Key.Item1 == (((data[12] & 0x40) == 0) ? EthernetTargetAddress : EthernetSourceAddress)
							select ee.Value).FirstOrDefault();
						return ecus != null;
					}
				}
				else
				{
					long num = monitorAddress;
					if (!IsIso)
					{
						num = ((SourceAddress == 249) ? ((byte)((monitorAddress & 0xFF00) >> 8)) : SourceAddress);
					}
					if (addressEcuMap.TryGetValue(num, out var value))
					{
						ecus = value.Item1;
						return true;
					}
					if (!IsIso)
					{
						ecus = new Ecu[1]
						{
							new Ecu((byte)num, null, RollCall.GetManager(Protocol.J1939))
						};
						addressEcuMap.Add(num, new Tuple<Ecu[], ByteMessageDirection>(ecus, ByteMessageDirection.RX));
						return true;
					}
				}
			}
		}
		ecus = null;
		return false;
	}

	public bool TryGetDiagnosisVariant(out DiagnosisVariant variant)
	{
		if (TryGetEcus(out var ecus))
		{
			return TryGetDiagnosisVariant(ecus, out variant);
		}
		variant = null;
		return false;
	}

	public bool TryGetDiagnosisVariant(Ecu[] ecus, out DiagnosisVariant variant)
	{
		if (IsIso && IsIdentificationInitiatingFrame)
		{
			int? isoFirstDataBytePos = IsoFirstDataBytePos;
			int diagVerLong = (data[isoFirstDataBytePos.Value + 3] << 16) + (data[isoFirstDataBytePos.Value + 4] << 8) + data[isoFirstDataBytePos.Value + 5];
			variant = ecus.SelectMany((Ecu e) => e.DiagnosisVariants).FirstOrDefault((DiagnosisVariant v) => v.DiagnosticVersionLong == diagVerLong);
			return variant != null;
		}
		variant = null;
		return false;
	}

	public static bool TryGetDiagnosticVariant(Ecu[] ecus, IEnumerable<BusMonitorFrameCollection> completeMessages, out DiagnosisVariant variant)
	{
		List<RollCall.IdentificationInformation> source = completeMessages.SelectMany((BusMonitorFrameCollection completeMessage) => completeMessage.GetRollCallIdentificationInformation()).DistinctBy((RollCall.IdentificationInformation id) => id.Id).ToList();
		IEnumerable<Tuple<RollCall.ID, object>> readIdBlock = from id in source
			where id.Value != null
			select new Tuple<RollCall.ID, object>(id.Id, id.Value);
		IEnumerable<DiagnosisVariant> source2 = ecus.SelectMany((Ecu e) => e.DiagnosisVariants.Where((DiagnosisVariant v) => v.IsMatch(readIdBlock)));
		if (source2.Any())
		{
			variant = (from v in source2
				orderby v.RollCallIdentificationCount, !v.Ecu.IsRollCallBaseEcu
				select v).Last();
		}
		else
		{
			variant = ecus.First().DiagnosisVariants["ROLLCALL"];
		}
		return variant != null;
	}

	public bool TryGetDescription(Channel channel, out string serviceDescription, out string symbolicName)
	{
		serviceDescription = (symbolicName = string.Empty);
		if (!IsFrameTypeCustom)
		{
			if (IsIso)
			{
				int? isoFirstDataBytePos = IsoFirstDataBytePos;
				ByteMessageDirection direction = Direction;
				if (direction == ByteMessageDirection.RX && IsNegativeResponse && data.Length > 3)
				{
					serviceDescription = "NR: " + udsProtocol.GetServiceIdentifierDescription(data[isoFirstDataBytePos.Value + 1]);
					symbolicName = udsProtocol.GetNegativeResponseCodeDescription(data[isoFirstDataBytePos.Value + 2]);
				}
				else if (isoFirstDataBytePos.HasValue)
				{
					serviceDescription = udsProtocol.GetServiceIdentifierDescription((direction == ByteMessageDirection.RX) ? ((byte)(data[isoFirstDataBytePos.Value] - 64)) : data[isoFirstDataBytePos.Value]);
					if (channel != null && channel.TryGetOfflineDiagnosticService(data.Skip(isoFirstDataBytePos.Value).ToArray(), direction, out var obj))
					{
						symbolicName = obj.First().Name.Split(":".ToCharArray())[0];
					}
				}
			}
			else if (isEthernet)
			{
				switch (FrameType)
				{
				case BusMonitorFrameType.VehicleIdentificationRequest:
					serviceDescription = "Vehicle Identification Request";
					break;
				case BusMonitorFrameType.VehicleAnnouncementMessage:
					serviceDescription = "Vehicle Announcement Message/Vehicle Identification Response";
					symbolicName = "VIN: " + Encoding.UTF8.GetString(data, 8, 17);
					break;
				case BusMonitorFrameType.RoutingActivationRequest:
					serviceDescription = "Routing activation request";
					break;
				case BusMonitorFrameType.RoutingActivationResponse:
					serviceDescription = "Routing activation response";
					symbolicName = string.Format(CultureInfo.CurrentCulture, "Response code: {0:X2}", data[12]);
					break;
				case BusMonitorFrameType.Acknowledgment:
					serviceDescription = "Acknowledgment";
					break;
				}
			}
			else
			{
				int? num = null;
				string value = string.Empty;
				switch (FrameType)
				{
				case BusMonitorFrameType.SingleFrame:
					num = ParameterGroupNumber;
					if (num == 59904)
					{
						num = TargetParameterGroupNumber;
						value = "(RQST)";
					}
					else if (num == 59392)
					{
						num = TargetParameterGroupNumber;
						if (data[0] < 4 && !RollCallJ1939.GlobalInstance.SuspectParameters.TryGetValue((data[0] + 3290).ToString(CultureInfo.InvariantCulture), out value))
						{
							value = string.Empty;
						}
					}
					break;
				case BusMonitorFrameType.RequestToSendDestinationSpecific:
				case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
					num = TargetParameterGroupNumber;
					break;
				}
				if (num.HasValue)
				{
					if (RollCallJ1939.GlobalInstance.ParameterGroups.TryGetValue(num.Value, out var value2))
					{
						serviceDescription = "PGN " + num.Value + " " + value2.Item2 + " " + value;
						symbolicName = value2.Item1;
					}
					else
					{
						serviceDescription = "PGN " + num.Value;
						symbolicName = "<not described>";
					}
				}
			}
		}
		else
		{
			switch (FrameType)
			{
			case BusMonitorFrameType.ChipState:
				serviceDescription = string.Format(CultureInfo.InvariantCulture, "{0} TXERR {1} RXERR {2}", (ChipStates)data[0], data[1], data[2]);
				break;
			case BusMonitorFrameType.BaudRate:
				serviceDescription = string.Format(CultureInfo.InvariantCulture, "{0} bps", (data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3]);
				break;
			}
		}
		if (string.IsNullOrEmpty(serviceDescription))
		{
			return !string.IsNullOrEmpty(symbolicName);
		}
		return true;
	}

	public bool IsInitiatingFrameFor(BusMonitorFrame frame)
	{
		int num;
		byte num2;
		if (!IsFrameTypeCustom && !isEthernet)
		{
			if (!IsIso)
			{
				if (frame.FrameType != BusMonitorFrameType.ClearToSendDestinationSpecific)
				{
					num = ((frame.FrameType == BusMonitorFrameType.EndOfMessageAcknowledge) ? 1 : 0);
					if (num == 0)
					{
						num2 = frame.SourceAddress;
						goto IL_00a5;
					}
				}
				else
				{
					num = 1;
				}
				num2 = frame.DestinationAddress;
				goto IL_00a5;
			}
			if (frame.FrameType == BusMonitorFrameType.FlowControl || frame.FrameType == BusMonitorFrameType.ConsecutiveFrame)
			{
				bool num3 = frame.FrameType == BusMonitorFrameType.FlowControl;
				byte b = (num3 ? frame.DestinationAddress : frame.SourceAddress);
				byte b2 = (num3 ? frame.SourceAddress : frame.DestinationAddress);
				if (FrameType == BusMonitorFrameType.FirstFrame && SourceAddress == b && DestinationAddress == b2)
				{
					return true;
				}
			}
		}
		goto IL_00f0;
		IL_00f0:
		return false;
		IL_00a5:
		byte b3 = num2;
		byte b4 = ((num != 0) ? frame.SourceAddress : frame.DestinationAddress);
		if (FrameType == BusMonitorFrameType.RequestToSendDestinationSpecific && SourceAddress == b3 && DestinationAddress == b4)
		{
			return true;
		}
		if (FrameType == BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination && SourceAddress == b3 && b4 == byte.MaxValue)
		{
			return true;
		}
		goto IL_00f0;
	}
}
