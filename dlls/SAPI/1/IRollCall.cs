// Decompiled with JetBrains decompiler
// Type: SapiLayer1.IRollCall
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace SapiLayer1;

public interface IRollCall
{
  Protocol Protocol { get; }

  void Start();

  void Stop();

  bool ConnectEnabled { set; get; }

  event EventHandler<StateChangedEventArgs> StateChangedEvent;

  event EventHandler<EventArgs> LoadChangedEvent;

  ConnectionState State { get; }

  float? Load { get; }

  bool Running { get; }

  IDictionary<string, string> SuspectParameters { get; }

  IDictionary<int, string> ParameterGroupLabels { get; }

  IDictionary<int, string> ParameterGroupAcronyms { get; }

  void WriteTranslationFile(CultureInfo culture, IEnumerable<TranslationEntry> translations);

  bool IsAutoBaudRate { get; }

  string DeviceName { get; }

  string DeviceLibraryName { get; }

  string DeviceLibraryVersion { get; }

  string DeviceFirmwareVersion { get; }

  string DeviceDriverVersion { get; }

  string DeviceSupportedProtocols { get; }

  IEnumerable<byte> PowertrainAddresses { get; }

  void SetRestrictedAddressList(IEnumerable<byte> restrictedSourceAddresses);
}
