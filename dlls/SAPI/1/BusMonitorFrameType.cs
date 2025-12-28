// Decompiled with JetBrains decompiler
// Type: SapiLayer1.BusMonitorFrameType
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

#nullable disable
namespace SapiLayer1;

public enum BusMonitorFrameType
{
  SingleFrame,
  FirstFrame,
  ConsecutiveFrame,
  FlowControl,
  RequestToSendDestinationSpecific,
  ClearToSendDestinationSpecific,
  ConnectionAbort,
  BroadcastAnnounceMessageGlobalDestination,
  EndOfMessageAcknowledge,
  TransportProtocolDataTransfer,
  VehicleIdentificationRequest,
  VehicleAnnouncementMessage,
  RoutingActivationRequest,
  RoutingActivationResponse,
  Acknowledgment,
  ChipState,
  ErrorFrame,
  BaudRate,
  Unknown,
}
