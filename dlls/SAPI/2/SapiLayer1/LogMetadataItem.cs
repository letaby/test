using System;

namespace SapiLayer1;

public struct LogMetadataItem
{
	public const char FaultCodeSplitCharacter = '/';

	public LogMetadataType Type { get; private set; }

	public string Ecu { get; private set; }

	public string Content { get; private set; }

	public DateTime Time { get; private set; }

	public LogMetadataItem(LogMetadataType type, string ecu, string content, string time)
	{
		this = default(LogMetadataItem);
		Type = type;
		Ecu = ecu;
		Content = content;
		Time = Sapi.TimeFromString(time);
	}

	public static bool operator ==(LogMetadataItem left, LogMetadataItem right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(LogMetadataItem left, LogMetadataItem right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
