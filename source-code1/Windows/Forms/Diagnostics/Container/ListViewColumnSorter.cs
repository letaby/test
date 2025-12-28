// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.ListViewColumnSorter
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using System.Collections;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class ListViewColumnSorter : IComparer
{
  private CaseInsensitiveComparer objectCompare;

  public ListViewColumnSorter()
  {
    this.objectCompare = new CaseInsensitiveComparer(CultureInfo.CurrentCulture);
  }

  public int Compare(object x, object y)
  {
    ListViewItem listViewItem1 = (ListViewItem) x;
    ListViewItem listViewItem2 = (ListViewItem) y;
    int num;
    if (this.SortType == ListViewColumnSorter.SortBy.DateTime || this.SortType == ListViewColumnSorter.SortBy.Size)
    {
      OpenLogFileForm.OurFileInfo tag1 = listViewItem1.Tag as OpenLogFileForm.OurFileInfo;
      OpenLogFileForm.OurFileInfo tag2 = listViewItem2.Tag as OpenLogFileForm.OurFileInfo;
      num = this.SortType != ListViewColumnSorter.SortBy.DateTime ? tag1.Length.CompareTo(tag2.Length) : tag1.LastWriteTime.CompareTo(tag2.LastWriteTime);
    }
    else
      num = this.SortType != ListViewColumnSorter.SortBy.Tag ? this.objectCompare.Compare((object) listViewItem1.SubItems[this.ColumnToSort].Text, (object) listViewItem2.SubItems[this.ColumnToSort].Text) : this.objectCompare.Compare(listViewItem1.SubItems[this.ColumnToSort].Tag, listViewItem2.SubItems[this.ColumnToSort].Tag);
    if (this.OrderOfSort == SortOrder.Ascending)
      return num;
    return this.OrderOfSort == SortOrder.Descending ? -num : 0;
  }

  public int ColumnToSort { set; get; }

  public SortOrder OrderOfSort { set; get; }

  public ListViewColumnSorter.SortBy SortType { set; get; }

  public enum SortBy
  {
    Text,
    DateTime,
    Size,
    Tag,
  }
}
