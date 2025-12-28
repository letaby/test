using System.Collections;
using System.Globalization;
using System.Windows.Forms;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class ListViewColumnSorter : IComparer
{
	public enum SortBy
	{
		Text,
		DateTime,
		Size,
		Tag
	}

	private CaseInsensitiveComparer objectCompare;

	public int ColumnToSort { get; set; }

	public SortOrder OrderOfSort { get; set; }

	public SortBy SortType { get; set; }

	public ListViewColumnSorter()
	{
		objectCompare = new CaseInsensitiveComparer(CultureInfo.CurrentCulture);
	}

	public int Compare(object x, object y)
	{
		ListViewItem listViewItem = (ListViewItem)x;
		ListViewItem listViewItem2 = (ListViewItem)y;
		int num;
		if (SortType != SortBy.DateTime && SortType != SortBy.Size)
		{
			num = ((SortType != SortBy.Tag) ? objectCompare.Compare(listViewItem.SubItems[ColumnToSort].Text, listViewItem2.SubItems[ColumnToSort].Text) : objectCompare.Compare(listViewItem.SubItems[ColumnToSort].Tag, listViewItem2.SubItems[ColumnToSort].Tag));
		}
		else
		{
			OpenLogFileForm.OurFileInfo ourFileInfo = listViewItem.Tag as OpenLogFileForm.OurFileInfo;
			OpenLogFileForm.OurFileInfo ourFileInfo2 = listViewItem2.Tag as OpenLogFileForm.OurFileInfo;
			num = ((SortType != SortBy.DateTime) ? ourFileInfo.Length.CompareTo(ourFileInfo2.Length) : ourFileInfo.LastWriteTime.CompareTo(ourFileInfo2.LastWriteTime));
		}
		if (OrderOfSort == SortOrder.Ascending)
		{
			return num;
		}
		if (OrderOfSort == SortOrder.Descending)
		{
			return -num;
		}
		return 0;
	}
}
