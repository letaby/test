using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class BusMonitorFrameCollection : ReadOnlyCollection<BusMonitorFrame>
{
	private BusMonitorFrameCollection()
		: base((IList<BusMonitorFrame>)new List<BusMonitorFrame>())
	{
	}

	public static BusMonitorFrameCollection Create(BusMonitorFrame targetFrame, int index, IList<BusMonitorFrame> frames)
	{
		BusMonitorFrame busMonitorFrame = targetFrame;
		int num = index;
		if (targetFrame.IsContinuation)
		{
			while (--num >= 0)
			{
				BusMonitorFrame busMonitorFrame2 = frames[num];
				if (busMonitorFrame2.IsInitiatingFrameFor(targetFrame))
				{
					busMonitorFrame = busMonitorFrame2;
					break;
				}
			}
		}
		if (num >= 0)
		{
			BusMonitorFrameCollection busMonitorFrameCollection = new BusMonitorFrameCollection();
			switch (busMonitorFrame.FrameType)
			{
			case BusMonitorFrameType.SingleFrame:
				busMonitorFrameCollection.Items.Add(targetFrame);
				return busMonitorFrameCollection;
			case BusMonitorFrameType.FirstFrame:
			case BusMonitorFrameType.RequestToSendDestinationSpecific:
			case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
			{
				bool isIso = busMonitorFrame.IsIso;
				byte? b = ((!isIso) ? busMonitorFrame.TotalPackets : ((byte?)null));
				byte b2 = 1;
				foreach (BusMonitorFrame item in frames.Skip(num))
				{
					if (isIso)
					{
						if (item.Direction == busMonitorFrame.Direction)
						{
							if (busMonitorFrame.IsInitiatingFrameFor(item) || item == busMonitorFrame)
							{
								busMonitorFrameCollection.Items.Add(item);
							}
							else if (busMonitorFrame.SourceAddress == item.SourceAddress && busMonitorFrame.DestinationAddress == item.DestinationAddress)
							{
								break;
							}
						}
					}
					else if (item == busMonitorFrame)
					{
						busMonitorFrameCollection.Items.Add(item);
					}
					else if (item.FrameType == BusMonitorFrameType.TransportProtocolDataTransfer && busMonitorFrame.IsInitiatingFrameFor(item))
					{
						if (item.SequenceNumber.Value == b2)
						{
							busMonitorFrameCollection.Items.Add(item);
							b2++;
						}
						if (item.SequenceNumber.Value == b)
						{
							break;
						}
					}
					else if (item.FrameType == BusMonitorFrameType.ConnectionAbort)
					{
						break;
					}
				}
				return busMonitorFrameCollection;
			}
			}
		}
		return null;
	}

	public bool TryGetPresentations(Channel channel, out Tuple<object, object>[] descriptionValuePairs)
	{
		descriptionValuePairs = null;
		BusMonitorFrame busMonitorFrame = base[0];
		if (busMonitorFrame.IsIso)
		{
			if (busMonitorFrame.FrameType == BusMonitorFrameType.FirstFrame || busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame)
			{
				int? isoFirstDataBytePos = busMonitorFrame.IsoFirstDataBytePos;
				ByteMessageDirection direction = busMonitorFrame.Direction;
				if (isoFirstDataBytePos.HasValue && !busMonitorFrame.IsNegativeResponse)
				{
					byte[] completeData = ToByteArray();
					if (channel.TryGetOfflineDiagnosticService(completeData, direction, out var obj))
					{
						foreach (Service item in obj)
						{
							if (direction == ByteMessageDirection.RX)
							{
								IEnumerable<ServiceOutputValue> source = from ov in item.OutputValues
									where ov.Type != null && (ov.BitLength > 0 || ov.ByteLength > 0) && ov.BytePosition + ov.ByteLength <= completeData.Length
									select (ov);
								descriptionValuePairs = source.Select((ServiceOutputValue p) => Tuple.Create((object)p, p.GetPresentation(completeData))).ToArray();
							}
							else
							{
								IEnumerable<ServiceInputValue> source2 = from iv in item.InputValues
									where iv.BytePosition + iv.ByteLength <= completeData.Length
									select (iv);
								descriptionValuePairs = source2.Select((ServiceInputValue p) => Tuple.Create((object)p, p.GetPreparation(completeData))).ToArray();
							}
							if (obj.Count > 1 && descriptionValuePairs.Count() > 0 && !descriptionValuePairs.Any((Tuple<object, object> v) => v.Item2 is string text && text.IndexOf("Invalid value ($", StringComparison.OrdinalIgnoreCase) == 0))
							{
								return true;
							}
						}
						return true;
					}
				}
			}
		}
		else if (busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame || busMonitorFrame.FrameType == BusMonitorFrameType.RequestToSendDestinationSpecific || busMonitorFrame.FrameType == BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination)
		{
			byte[] completeData2 = ToByteArray();
			if (completeData2.Length != 0)
			{
				int pgn = ((busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame) ? busMonitorFrame.ParameterGroupNumber : busMonitorFrame.TargetParameterGroupNumber.Value);
				if (pgn != 59904 && pgn != 59392)
				{
					List<Instrument> list = channel.Instruments.Where((Instrument i) => i.MessageNumber == pgn).ToList();
					if (list.Count > 0)
					{
						IEnumerable<Presentation> source3 = list.Where((Instrument i) => i.BytePosition + i.ByteLength <= completeData2.Length + 1).Select((Func<Instrument, Presentation>)((Instrument i) => i));
						descriptionValuePairs = source3.Select((Presentation i) => Tuple.Create((object)i, i.GetPresentation(completeData2))).ToArray();
						return true;
					}
					byte[] data = Enumerable.Repeat((byte)0, 6).Concat(completeData2).ToArray();
					if (RollCallJ1939.TryGetIdentificationInformation(pgn, data, out var ids))
					{
						descriptionValuePairs = ids.Select((RollCall.IdentificationInformation id) => GetDescriptionValuePair(id)).ToArray();
						return true;
					}
				}
			}
		}
		return false;
		Tuple<object, object> GetDescriptionValuePair(RollCall.IdentificationInformation id)
		{
			EcuInfo ecuInfo = channel.EcuInfos[id.Id.ToNumberString()];
			if (ecuInfo?.Presentation != null && ecuInfo.Presentation.Type == typeof(Choice))
			{
				return Tuple.Create((object)ecuInfo, (object)ecuInfo.Presentation.Choices.GetItemFromRawValue(id.Value));
			}
			return Tuple.Create((object)ecuInfo, id.Value);
		}
	}

	internal IEnumerable<RollCall.IdentificationInformation> GetRollCallIdentificationInformation()
	{
		BusMonitorFrame busMonitorFrame = base[0];
		int id = ((busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame) ? busMonitorFrame.ParameterGroupNumber : busMonitorFrame.TargetParameterGroupNumber.Value);
		byte[] data = Enumerable.Repeat((byte)0, 6).Concat(ToByteArray()).ToArray();
		if (!RollCallJ1939.TryGetIdentificationInformation(id, data, out var ids))
		{
			yield break;
		}
		foreach (RollCall.IdentificationInformation item in ids)
		{
			yield return item;
		}
	}

	public byte[] ToByteArray()
	{
		BusMonitorFrame busMonitorFrame = base[0];
		if (busMonitorFrame.IsIso)
		{
			return this.SelectMany((BusMonitorFrame f) => f.Data.Skip(f.IsoFirstDataBytePos.Value)).ToArray();
		}
		if (busMonitorFrame.FrameType != BusMonitorFrameType.SingleFrame)
		{
			return this.Skip(1).SelectMany((BusMonitorFrame f) => f.Data.Skip(1)).ToArray();
		}
		return busMonitorFrame.Data.ToArray();
	}

	public int GetBytePosition(BusMonitorFrame targetFrame)
	{
		int num = 0;
		foreach (BusMonitorFrame item in base.Items)
		{
			if (item == targetFrame)
			{
				return num;
			}
			num += item.ActualDataLength;
		}
		return -1;
	}
}
