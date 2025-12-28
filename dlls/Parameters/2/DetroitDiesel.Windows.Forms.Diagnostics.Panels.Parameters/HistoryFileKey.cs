using System;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class HistoryFileKey
{
	private DateTime creationDate;

	private string reason;

	public DateTime CreationDate => creationDate;

	public string Reason => reason;

	public HistoryFileKey(DateTime dateTime, string reason)
	{
		creationDate = dateTime;
		this.reason = reason;
	}
}
