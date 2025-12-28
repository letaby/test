using System.Collections.Generic;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class HistoryFileKeyComparer : IComparer<HistoryFileKey>
{
	public int Compare(HistoryFileKey x, HistoryFileKey y)
	{
		if (x.CreationDate == y.CreationDate)
		{
			return string.CompareOrdinal(x.Reason, y.Reason);
		}
		return x.CreationDate.CompareTo(y.CreationDate);
	}
}
