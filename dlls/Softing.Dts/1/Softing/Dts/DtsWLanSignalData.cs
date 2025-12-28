// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsWLanSignalData
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsWLanSignalData : DtsObject, MCDObject, IDisposable
{
  DtsWLanType Type { get; }

  uint Channel { get; }

  uint ChannelFreq { get; }

  uint ChannelWidth { get; }

  float TxPower { get; }

  uint LinkSpeed { get; }

  int RSSI { get; }

  int SNR { get; }

  int Noise { get; }

  int SigQuality { get; }

  string SSID { get; }

  uint ValidityFlag { get; }
}
