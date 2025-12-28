using System;
using System.Collections.Generic;
using System.Globalization;

namespace SapiLayer1;

public interface IRollCall
{
	Protocol Protocol { get; }

	bool ConnectEnabled { get; set; }

	ConnectionState State { get; }

	float? Load { get; }

	bool Running { get; }

	IDictionary<string, string> SuspectParameters { get; }

	IDictionary<int, string> ParameterGroupLabels { get; }

	IDictionary<int, string> ParameterGroupAcronyms { get; }

	bool IsAutoBaudRate { get; }

	string DeviceName { get; }

	string DeviceLibraryName { get; }

	string DeviceLibraryVersion { get; }

	string DeviceFirmwareVersion { get; }

	string DeviceDriverVersion { get; }

	string DeviceSupportedProtocols { get; }

	IEnumerable<byte> PowertrainAddresses { get; }

	event EventHandler<StateChangedEventArgs> StateChangedEvent;

	event EventHandler<EventArgs> LoadChangedEvent;

	void Start();

	void Stop();

	void WriteTranslationFile(CultureInfo culture, IEnumerable<TranslationEntry> translations);

	void SetRestrictedAddressList(IEnumerable<byte> restrictedSourceAddresses);
}
