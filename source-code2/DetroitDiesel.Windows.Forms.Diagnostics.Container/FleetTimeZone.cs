using System;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public sealed class FleetTimeZone : IEquatable<FleetTimeZone>
{
	private readonly string name;

	private readonly TimeSpan offset;

	public string Name => name;

	public TimeSpan Offset => offset;

	public override int GetHashCode()
	{
		return name.GetHashCode();
	}

	public override string ToString()
	{
		return name;
	}

	internal FleetTimeZone(string name, int offsetInHours)
	{
		this.name = name;
		offset = new TimeSpan(offsetInHours, 0, 0);
	}

	public bool Equals(FleetTimeZone other)
	{
		if (offset == other.offset)
		{
			return string.Equals(name, other.name, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}
}
