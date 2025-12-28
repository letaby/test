using System;

namespace DetroitDiesel.DataHub;

public class ChangeDataPagePasswordRequestEventArgs : EventArgs
{
	public ChangePasswordResult Result { get; private set; }

	internal ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult result)
	{
		Result = result;
	}
}
