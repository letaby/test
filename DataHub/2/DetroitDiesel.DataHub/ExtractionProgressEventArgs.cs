using System;
using SapiLayer1;

namespace DetroitDiesel.DataHub;

public class ExtractionProgressEventArgs : EventArgs
{
	public double Percent { get; private set; }

	public Channel Channel { get; private set; }

	public ExtractionProgressEventArgs(Channel channel, double percent)
	{
		Channel = channel;
		Percent = percent;
	}
}
