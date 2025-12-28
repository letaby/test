using System;

namespace SapiLayer1;

public sealed class ProgressEventArgs : EventArgs
{
	private double percentComplete;

	public double PercentComplete => percentComplete;

	internal ProgressEventArgs(double pc)
	{
		percentComplete = pc;
	}
}
