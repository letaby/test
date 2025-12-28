using System;

namespace SapiLayer1;

public sealed class Session : IComparable
{
	private DateTime startTime;

	private DateTime endTime;

	private Channel channel;

	private string descriptionVersion;

	private ConnectionResource resource;

	private bool? isFixedVariant;

	private ChannelOptions channelOptions;

	private string variantName;

	public DateTime StartTime => startTime;

	public DateTime EndTime => endTime;

	public string DescriptionVersion => descriptionVersion;

	public ConnectionResource Resource => resource;

	public Channel Channel => channel;

	public bool? IsFixedVariant => isFixedVariant;

	public ChannelOptions ChannelOptions => channelOptions;

	public string VariantName => variantName;

	internal Session(Channel channel, DateTime start, DateTime end, string version, ConnectionResource resource, string variantName, bool? isFixedVariant, ChannelOptions channelOptions)
	{
		startTime = start;
		endTime = end;
		this.channel = channel;
		descriptionVersion = version;
		this.resource = resource;
		this.variantName = variantName;
		this.isFixedVariant = isFixedVariant;
		this.channelOptions = channelOptions;
	}

	internal void UpdateEndTime(DateTime endTime)
	{
		this.endTime = endTime;
	}

	public int CompareTo(object obj)
	{
		Session session = (Session)obj;
		return startTime.CompareTo(session.StartTime);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Session object1, Session object2)
	{
		return object.Equals(object1, object2);
	}

	public static bool operator !=(Session object1, Session object2)
	{
		return !object.Equals(object1, object2);
	}

	public static bool operator <(Session object1, Session object2)
	{
		if (object1 == null)
		{
			throw new ArgumentNullException("object1");
		}
		return object1.CompareTo(object2) < 0;
	}

	public static bool operator >(Session object1, Session object2)
	{
		if (object1 == null)
		{
			throw new ArgumentNullException("object1");
		}
		return object1.CompareTo(object2) > 0;
	}
}
