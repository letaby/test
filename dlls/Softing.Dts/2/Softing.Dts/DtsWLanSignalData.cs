using System;

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
