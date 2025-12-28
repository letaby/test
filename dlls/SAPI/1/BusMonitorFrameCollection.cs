// Decompiled with JetBrains decompiler
// Type: SapiLayer1.BusMonitorFrameCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class BusMonitorFrameCollection : ReadOnlyCollection<BusMonitorFrame>
{
  private BusMonitorFrameCollection()
    : base((IList<BusMonitorFrame>) new List<BusMonitorFrame>())
  {
  }

  public static BusMonitorFrameCollection Create(
    BusMonitorFrame targetFrame,
    int index,
    IList<BusMonitorFrame> frames)
  {
    BusMonitorFrame busMonitorFrame = targetFrame;
    int num1 = index;
    if (targetFrame.IsContinuation)
    {
      while (--num1 >= 0)
      {
        BusMonitorFrame frame = frames[num1];
        if (frame.IsInitiatingFrameFor(targetFrame))
        {
          busMonitorFrame = frame;
          break;
        }
      }
    }
    if (num1 >= 0)
    {
      BusMonitorFrameCollection monitorFrameCollection = new BusMonitorFrameCollection();
      switch (busMonitorFrame.FrameType)
      {
        case BusMonitorFrameType.SingleFrame:
          monitorFrameCollection.Items.Add(targetFrame);
          return monitorFrameCollection;
        case BusMonitorFrameType.FirstFrame:
        case BusMonitorFrameType.RequestToSendDestinationSpecific:
        case BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination:
          bool isIso = busMonitorFrame.IsIso;
          byte? nullable1 = !isIso ? busMonitorFrame.TotalPackets : new byte?();
          byte num2 = 1;
          foreach (BusMonitorFrame frame in frames.Skip<BusMonitorFrame>(num1))
          {
            if (isIso)
            {
              if (frame.Direction == busMonitorFrame.Direction)
              {
                if (busMonitorFrame.IsInitiatingFrameFor(frame) || frame == busMonitorFrame)
                  monitorFrameCollection.Items.Add(frame);
                else if ((int) busMonitorFrame.SourceAddress == (int) frame.SourceAddress)
                {
                  if ((int) busMonitorFrame.DestinationAddress == (int) frame.DestinationAddress)
                    break;
                }
              }
            }
            else if (frame == busMonitorFrame)
              monitorFrameCollection.Items.Add(frame);
            else if (frame.FrameType == BusMonitorFrameType.TransportProtocolDataTransfer && busMonitorFrame.IsInitiatingFrameFor(frame))
            {
              byte? nullable2 = frame.SequenceNumber;
              if ((int) nullable2.Value == (int) num2)
              {
                monitorFrameCollection.Items.Add(frame);
                ++num2;
              }
              nullable2 = frame.SequenceNumber;
              int num3 = (int) nullable2.Value;
              nullable2 = nullable1;
              int? nullable3 = nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?();
              int valueOrDefault = nullable3.GetValueOrDefault();
              if ((num3 == valueOrDefault ? (nullable3.HasValue ? 1 : 0) : 0) != 0)
                break;
            }
            else if (frame.FrameType == BusMonitorFrameType.ConnectionAbort)
              break;
          }
          return monitorFrameCollection;
      }
    }
    return (BusMonitorFrameCollection) null;
  }

  public bool TryGetPresentations(
    Channel channel,
    out Tuple<object, object>[] descriptionValuePairs)
  {
    descriptionValuePairs = (Tuple<object, object>[]) null;
    BusMonitorFrame busMonitorFrame = this[0];
    if (busMonitorFrame.IsIso)
    {
      if (busMonitorFrame.FrameType == BusMonitorFrameType.FirstFrame || busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame)
      {
        int? firstDataBytePos = busMonitorFrame.IsoFirstDataBytePos;
        ByteMessageDirection direction = busMonitorFrame.Direction;
        if (firstDataBytePos.HasValue && !busMonitorFrame.IsNegativeResponse)
        {
          byte[] completeData = this.ToByteArray();
          List<Service> serviceList;
          if (channel.TryGetOfflineDiagnosticService(completeData, direction, out serviceList))
          {
            foreach (Service service in serviceList)
            {
              if (direction == ByteMessageDirection.RX)
              {
                IEnumerable<ServiceOutputValue> source = service.OutputValues.Where<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov =>
                {
                  if (ov.Type != (Type) null)
                  {
                    int? nullable = ov.BitLength;
                    int num1 = 0;
                    if ((nullable.GetValueOrDefault() > num1 ? (nullable.HasValue ? 1 : 0) : 0) == 0)
                    {
                      nullable = ov.ByteLength;
                      int num2 = 0;
                      if ((nullable.GetValueOrDefault() > num2 ? (nullable.HasValue ? 1 : 0) : 0) == 0)
                        goto label_6;
                    }
                    int? bytePosition = ov.BytePosition;
                    int? byteLength = ov.ByteLength;
                    nullable = bytePosition.HasValue & byteLength.HasValue ? new int?(bytePosition.GetValueOrDefault() + byteLength.GetValueOrDefault()) : new int?();
                    int length = completeData.Length;
                    return nullable.GetValueOrDefault() <= length && nullable.HasValue;
                  }
label_6:
                  return false;
                })).Select<ServiceOutputValue, ServiceOutputValue>((Func<ServiceOutputValue, ServiceOutputValue>) (ov => ov));
                descriptionValuePairs = source.Select<ServiceOutputValue, Tuple<object, object>>((Func<ServiceOutputValue, Tuple<object, object>>) (p => Tuple.Create<object, object>((object) p, p.GetPresentation(completeData)))).ToArray<Tuple<object, object>>();
              }
              else
              {
                IEnumerable<ServiceInputValue> source = service.InputValues.Where<ServiceInputValue>((Func<ServiceInputValue, bool>) (iv =>
                {
                  long? bytePosition = iv.BytePosition;
                  long? byteLength = iv.ByteLength;
                  long? nullable = bytePosition.HasValue & byteLength.HasValue ? new long?(bytePosition.GetValueOrDefault() + byteLength.GetValueOrDefault()) : new long?();
                  long length = (long) completeData.Length;
                  return nullable.GetValueOrDefault() <= length && nullable.HasValue;
                })).Select<ServiceInputValue, ServiceInputValue>((Func<ServiceInputValue, ServiceInputValue>) (iv => iv));
                descriptionValuePairs = source.Select<ServiceInputValue, Tuple<object, object>>((Func<ServiceInputValue, Tuple<object, object>>) (p => Tuple.Create<object, object>((object) p, p.GetPreparation(completeData)))).ToArray<Tuple<object, object>>();
              }
              if (serviceList.Count > 1 && ((IEnumerable<Tuple<object, object>>) descriptionValuePairs).Count<Tuple<object, object>>() > 0 && !((IEnumerable<Tuple<object, object>>) descriptionValuePairs).Any<Tuple<object, object>>((Func<Tuple<object, object>, bool>) (v => v.Item2 is string str && str.IndexOf("Invalid value ($", StringComparison.OrdinalIgnoreCase) == 0)))
                return true;
            }
            return true;
          }
        }
      }
    }
    else if (busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame || busMonitorFrame.FrameType == BusMonitorFrameType.RequestToSendDestinationSpecific || busMonitorFrame.FrameType == BusMonitorFrameType.BroadcastAnnounceMessageGlobalDestination)
    {
      byte[] completeData = this.ToByteArray();
      if (completeData.Length != 0)
      {
        int pgn = busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame ? busMonitorFrame.ParameterGroupNumber : busMonitorFrame.TargetParameterGroupNumber.Value;
        if (pgn != 59904 && pgn != 59392)
        {
          List<Instrument> list = channel.Instruments.Where<Instrument>((Func<Instrument, bool>) (i =>
          {
            int? messageNumber = i.MessageNumber;
            int num = pgn;
            return messageNumber.GetValueOrDefault() == num && messageNumber.HasValue;
          })).ToList<Instrument>();
          if (list.Count > 0)
          {
            IEnumerable<Presentation> source = list.Where<Instrument>((Func<Instrument, bool>) (i =>
            {
              int? bytePosition = i.BytePosition;
              int? byteLength = i.ByteLength;
              int? nullable = bytePosition.HasValue & byteLength.HasValue ? new int?(bytePosition.GetValueOrDefault() + byteLength.GetValueOrDefault()) : new int?();
              int num = completeData.Length + 1;
              return nullable.GetValueOrDefault() <= num && nullable.HasValue;
            })).Select<Instrument, Presentation>((Func<Instrument, Presentation>) (i => (Presentation) i));
            descriptionValuePairs = source.Select<Presentation, Tuple<object, object>>((Func<Presentation, Tuple<object, object>>) (i => Tuple.Create<object, object>((object) i, i.GetPresentation(completeData)))).ToArray<Tuple<object, object>>();
            return true;
          }
          byte[] array = Enumerable.Repeat<byte>((byte) 0, 6).Concat<byte>((IEnumerable<byte>) completeData).ToArray<byte>();
          List<RollCall.IdentificationInformation> ids;
          if (RollCallJ1939.TryGetIdentificationInformation(pgn, array, out ids))
          {
            descriptionValuePairs = ids.Select<RollCall.IdentificationInformation, Tuple<object, object>>((Func<RollCall.IdentificationInformation, Tuple<object, object>>) (id => GetDescriptionValuePair(id))).ToArray<Tuple<object, object>>();
            return true;
          }
        }
      }
    }
    return false;

    Tuple<object, object> GetDescriptionValuePair(RollCall.IdentificationInformation id)
    {
      EcuInfo ecuInfo = channel.EcuInfos[id.Id.ToNumberString()];
      return ecuInfo?.Presentation != null && ecuInfo.Presentation.Type == typeof (Choice) ? Tuple.Create<object, object>((object) ecuInfo, (object) ecuInfo.Presentation.Choices.GetItemFromRawValue(id.Value)) : Tuple.Create<object, object>((object) ecuInfo, id.Value);
    }
  }

  internal IEnumerable<RollCall.IdentificationInformation> GetRollCallIdentificationInformation()
  {
    BusMonitorFrameCollection monitorFrameCollection = this;
    BusMonitorFrame busMonitorFrame = monitorFrameCollection[0];
    List<RollCall.IdentificationInformation> ids;
    if (RollCallJ1939.TryGetIdentificationInformation(busMonitorFrame.FrameType == BusMonitorFrameType.SingleFrame ? busMonitorFrame.ParameterGroupNumber : busMonitorFrame.TargetParameterGroupNumber.Value, Enumerable.Repeat<byte>((byte) 0, 6).Concat<byte>((IEnumerable<byte>) monitorFrameCollection.ToByteArray()).ToArray<byte>(), out ids))
    {
      foreach (RollCall.IdentificationInformation identificationInformation in ids)
        yield return identificationInformation;
    }
  }

  public byte[] ToByteArray()
  {
    BusMonitorFrame busMonitorFrame = this[0];
    if (busMonitorFrame.IsIso)
      return this.SelectMany<BusMonitorFrame, byte>((Func<BusMonitorFrame, IEnumerable<byte>>) (f => f.Data.Skip<byte>(f.IsoFirstDataBytePos.Value))).ToArray<byte>();
    return busMonitorFrame.FrameType != BusMonitorFrameType.SingleFrame ? this.Skip<BusMonitorFrame>(1).SelectMany<BusMonitorFrame, byte>((Func<BusMonitorFrame, IEnumerable<byte>>) (f => f.Data.Skip<byte>(1))).ToArray<byte>() : busMonitorFrame.Data.ToArray<byte>();
  }

  public int GetBytePosition(BusMonitorFrame targetFrame)
  {
    int bytePosition = 0;
    foreach (BusMonitorFrame busMonitorFrame in (IEnumerable<BusMonitorFrame>) this.Items)
    {
      if (busMonitorFrame == targetFrame)
        return bytePosition;
      bytePosition += busMonitorFrame.ActualDataLength;
    }
    return -1;
  }
}
