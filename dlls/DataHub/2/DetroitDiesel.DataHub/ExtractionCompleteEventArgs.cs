using System;
using SapiLayer1;

namespace DetroitDiesel.DataHub;

public class ExtractionCompleteEventArgs : EventArgs
{
	public XtrFile XtrFile { get; private set; }

	public Channel Channel { get; private set; }

	public bool Succeeded { get; private set; }

	public ExtractionCompleteEventArgs(bool succeeded, Channel channel, XtrFile xtrFile)
	{
		Succeeded = succeeded;
		Channel = channel;
		XtrFile = xtrFile;
	}
}
