using System;

namespace DetroitDiesel.DataHub;

public class DataPageRequestEventArgs : EventArgs
{
	public DataPageRequestType RequestType { get; private set; }

	public bool Succeeded { get; private set; }

	public DataPage Page { get; private set; }

	internal DataPageRequestEventArgs(bool succeeded, DataPageRequestType requestType, DataPage page)
	{
		Succeeded = succeeded;
		RequestType = requestType;
		Page = page;
	}
}
