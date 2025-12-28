// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.OpenLogFileForm
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class OpenLogFileForm : Form
{
  private OpenLogFileForm.MetadataCache metadataCache;
  private object metadataLock = new object();
  private const int MaximumMatchStrings = 25;
  private const int MaximumMostRecentlyUsedItems = 10;
  private string path;
  private string selectedFilePath = string.Empty;
  private ListViewColumnSorter lvwColumnSorter;
  private static readonly StringSetting DefaultDirectory = new StringSetting(Directories.LogFiles);
  private Dictionary<string, string> vinDescriptionCache = new Dictionary<string, string>();
  private IContainer components;
  private ListViewEx listView;
  private System.Windows.Forms.Button cancelButton;
  private System.Windows.Forms.Button okButton;
  private System.Windows.Forms.Button buttonBrowse;
  private Label labelLookIn;
  private System.Windows.Forms.ComboBox comboBoxLookIn;
  private System.Windows.Forms.Button buttonLogFiles;
  private System.Windows.Forms.Button buttonSummaryFiles;
  private Panel iconPanel;
  private System.Windows.Forms.Button buttonInfo;
  private System.Windows.Forms.Button buttonCopyLink;
  private ColumnHeader timeStampColumnHeader;
  private ColumnHeader vinColumnHeader;
  private ColumnHeader vinDecodeColumnHeader;
  private ColumnHeader sizeColumnHeader;
  private ColumnHeader nameColumnHeader;
  private ColumnHeader columnHeaderImageSpacing;
  private Label labelSearch;
  private System.Windows.Forms.TextBox textBoxSearch;
  private Timer searchTimer;
  private BackgroundWorker backgroundWorker;
  private System.Windows.Forms.ProgressBar progressBar;
  private ImageList stateImages;
  private System.Windows.Forms.Button buttonUpload;
  private System.Windows.Forms.ToolTip toolTipOpenLogFile;

  private string Path
  {
    get => this.path;
    set
    {
      if (!(value != this.path))
        return;
      lock (this.metadataLock)
        this.path = value;
      if (this.comboBoxLookIn.Items.Contains((object) this.path))
        this.comboBoxLookIn.Items.Remove((object) this.path);
      this.comboBoxLookIn.Items.Insert(0, (object) this.path);
      this.comboBoxLookIn.Text = this.path;
      if (this.backgroundWorker.IsBusy)
        return;
      this.progressBar.Visible = true;
      this.backgroundWorker.RunWorkerAsync();
    }
  }

  private static bool Match(LogMetadataItem item, string searchText, LogMetadataType prefixType)
  {
    if (prefixType != null)
    {
      if (prefixType != ((LogMetadataItem) ref item).Type)
        return false;
      if (searchText.Length == 0)
        return true;
    }
    if (((LogMetadataItem) ref item).Ecu == searchText)
      return true;
    switch (((LogMetadataItem) ref item).Type - 1)
    {
      case 0:
        return string.Equals(((LogMetadataItem) ref item).Content, searchText, StringComparison.OrdinalIgnoreCase);
      case 1:
        if (!((LogMetadataItem) ref item).Content.Contains("/") || searchText.Contains("/"))
          return string.Equals(((LogMetadataItem) ref item).Content, searchText, StringComparison.OrdinalIgnoreCase);
        string[] strArray = ((LogMetadataItem) ref item).Content.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.None);
        return strArray[0] == searchText || strArray[1] == searchText;
      case 2:
        return ((LogMetadataItem) ref item).Content.ToUpperInvariant().Contains(searchText);
      default:
        return false;
    }
  }

  private static LogMetadataType GetMetadataTypeFromString(string prefix)
  {
    if (string.Equals(prefix, Resources.MetadataTypeIdentification, StringComparison.OrdinalIgnoreCase))
      return (LogMetadataType) 1;
    if (string.Equals(prefix, Resources.MetadataTypeFaultCode, StringComparison.OrdinalIgnoreCase))
      return (LogMetadataType) 2;
    return string.Equals(prefix, Resources.MetadataTypeLabel, StringComparison.OrdinalIgnoreCase) ? (LogMetadataType) 3 : (LogMetadataType) 0;
  }

  private static string GetMetadataDescription(LogMetadataItem item)
  {
    string str = string.Empty;
    switch (((LogMetadataItem) ref item).Type - 1)
    {
      case 0:
        str = Resources.MetadataTypeIdentification;
        break;
      case 1:
        str = Resources.MetadataTypeFaultCode;
        break;
      case 2:
        str = Resources.MetadataTypeLabel;
        break;
    }
    return !string.IsNullOrEmpty(((LogMetadataItem) ref item).Ecu) ? $"{((LogMetadataItem) ref item).Time.ToString((IFormatProvider) CultureInfo.CurrentCulture)}: {str} - {((LogMetadataItem) ref item).Ecu} - {((LogMetadataItem) ref item).Content}" : $"{((LogMetadataItem) ref item).Time.ToString((IFormatProvider) CultureInfo.CurrentCulture)}: {str} - {((LogMetadataItem) ref item).Content}";
  }

  private bool TryGetMetadataDescription(
    string path,
    string text,
    LogMetadataType prefixType,
    out string metadataDescription)
  {
    metadataDescription = string.Empty;
    IEnumerable<LogMetadataItem> result = (IEnumerable<LogMetadataItem>) null;
    lock (this.metadataLock)
    {
      if (this.metadataCache != null)
        this.metadataCache.TryGetValue(path, out result);
    }
    if (result != null)
    {
      IEnumerable<LogMetadataItem> source = (IEnumerable<LogMetadataItem>) result.Where<LogMetadataItem>((Func<LogMetadataItem, bool>) (x => OpenLogFileForm.Match(x, text, prefixType))).OrderBy<LogMetadataItem, DateTime>((Func<LogMetadataItem, DateTime>) (x => ((LogMetadataItem) ref x).Time));
      if (source.Any<LogMetadataItem>())
      {
        int num = 0;
        StringBuilder stringBuilder = new StringBuilder();
        foreach (LogMetadataItem logMetadataItem in source)
        {
          stringBuilder.AppendLine(OpenLogFileForm.GetMetadataDescription(logMetadataItem));
          if (++num >= 25)
          {
            stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatMaxMatchStrings, (object) (source.Count<LogMetadataItem>() - 25)));
            break;
          }
        }
        metadataDescription = stringBuilder.ToString();
        return true;
      }
    }
    return false;
  }

  private static bool TryGetMetadataDescriptionDeep(
    string path,
    string searchText,
    Regex regex,
    out string metadataDescription)
  {
    metadataDescription = string.Empty;
    StringBuilder stringBuilder = new StringBuilder();
    int num = 0;
    using (Stream stream = SapiManager.OpenLogStream(path, false))
    {
      if (stream != null)
      {
        using (StreamReader streamReader = new StreamReader(stream))
        {
          for (string input = streamReader.ReadLine(); input != null; input = streamReader.ReadLine())
          {
            if ((regex != null ? (regex.Match(input).Success ? 1 : 0) : (input.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1 ? 1 : 0)) != 0)
            {
              stringBuilder.AppendLine(input);
              if (++num >= 25)
              {
                stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatMaxMatchStrings, (object) string.Empty));
                break;
              }
            }
          }
        }
      }
    }
    metadataDescription = stringBuilder.ToString();
    return !string.IsNullOrEmpty(metadataDescription);
  }

  public OpenLogFileForm()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Band.Normal))
      this.iconPanel.BackColor = new VisualStyleRenderer(VisualStyleElement.Rebar.Band.Normal).GetColor(ColorProperty.FillColorHint);
    this.stateImages.Images.Add((Image) Resources.upload_log);
    ((System.Windows.Forms.ListView) this.listView).StateImageList = this.stateImages;
    this.lvwColumnSorter = new ListViewColumnSorter();
    this.lvwColumnSorter.ColumnToSort = this.timeStampColumnHeader.Index;
    this.lvwColumnSorter.OrderOfSort = SortOrder.Descending;
    this.lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.DateTime;
    ((System.Windows.Forms.ListView) this.listView).ListViewItemSorter = (IComparer) this.lvwColumnSorter;
    this.okButton.Enabled = this.buttonInfo.Enabled = this.buttonCopyLink.Enabled = this.buttonUpload.Enabled = false;
  }

  public string FileName => this.selectedFilePath;

  protected override void OnLoad(EventArgs e)
  {
    int num = ((Control) this.listView).ClientSize.Width - SystemInformation.VerticalScrollBarWidth * 2 - this.columnHeaderImageSpacing.Width;
    this.timeStampColumnHeader.Width = num * 3 / 20;
    this.vinColumnHeader.Width = num * 3 / 20;
    this.vinDecodeColumnHeader.Width = num * 6 / 20;
    this.sizeColumnHeader.Width = num / 20;
    this.nameColumnHeader.Width = num * 7 / 20;
    StringSetting stringSetting1 = new StringSetting(string.Empty);
    for (int index = 0; index < 10; ++index)
    {
      StringSetting stringSetting2 = SettingsManager.GlobalInstance.GetValue<StringSetting>("LogFiles", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) "OpenDirectory", (object) index), stringSetting1);
      if (stringSetting2 != stringSetting1)
      {
        if (Directory.Exists(stringSetting2.Value))
          this.comboBoxLookIn.Items.Add((object) stringSetting2.Value);
      }
      else
        break;
    }
    this.Path = this.comboBoxLookIn.Items.Count <= 0 ? OpenLogFileForm.DefaultDirectory.Value : this.comboBoxLookIn.Items[0] as string;
    base.OnLoad(e);
  }

  private void PopulateList()
  {
    Cursor.Current = Cursors.WaitCursor;
    this.listView.LockSorting();
    this.listView.BeginUpdate();
    ((System.Windows.Forms.ListView) this.listView).Items.Clear();
    if (this.backgroundWorker.IsBusy && this.textBoxSearch.Text.Length > 0)
    {
      ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(string.Empty);
      ((ListViewItem) listViewExGroupItem).SubItems.Add(Resources.MessageSearching);
      ((System.Windows.Forms.ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem);
    }
    else
    {
      string[] second = new string[2]
      {
        SapiManager.GlobalInstance.CurrentLogFileInformation.SummaryFilePath,
        SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath
      };
      string[] array = ((IEnumerable<string>) Directory.GetFiles(this.Path, "*" + Directories.LogExtension, SearchOption.TopDirectoryOnly)).Except<string>((IEnumerable<string>) second).ToArray<string>();
      List<\u003C\u003Ef__AnonymousType1<OpenLogFileForm.OurFileInfo, System.Text.RegularExpressions.Match, string>> pathsToAdd = Enumerable.Repeat(new
      {
        info = (OpenLogFileForm.OurFileInfo) null,
        matchVin = (System.Text.RegularExpressions.Match) null,
        metadataDescription = (string) null
      }, 0).ToList();
      if (!this.textBoxSearch.Text.StartsWith(Resources.SearchTypePrefix_Deep, StringComparison.OrdinalIgnoreCase) && !this.textBoxSearch.Text.StartsWith(Resources.SearchTypePrefix_Regex, StringComparison.OrdinalIgnoreCase) && !this.textBoxSearch.Text.Equals("parameters", StringComparison.OrdinalIgnoreCase))
      {
        foreach (string path in array)
        {
          OpenLogFileForm.OurFileInfo ourFileInfo = new OpenLogFileForm.OurFileInfo(path);
          System.Text.RegularExpressions.Match match = SapiManager.ParseVin.Match(ourFileInfo.Name);
          bool flag = true;
          string metadataDescription = string.Empty;
          if (this.textBoxSearch.Text.Length > 0)
          {
            LogMetadataType prefixType = (LogMetadataType) 0;
            string text = this.textBoxSearch.Text.ToUpperInvariant();
            int length = text.IndexOf(':');
            if (length > -1)
            {
              prefixType = OpenLogFileForm.GetMetadataTypeFromString(text.Substring(0, length));
              if (prefixType != null)
                text = text.Substring(length + 1);
            }
            flag = this.TryGetMetadataDescription(path, text, prefixType, out metadataDescription);
            if (!flag && match.Success && prefixType == null)
              flag = match.Groups["vin"].Value.ToUpperInvariant().Contains(text);
          }
          if (flag)
            pathsToAdd.Add(new
            {
              info = ourFileInfo,
              matchVin = match,
              metadataDescription = metadataDescription
            });
        }
      }
      else
      {
        string searchText = this.textBoxSearch.Text.Substring(this.textBoxSearch.Text.IndexOf(':') + 1);
        Regex regex = (Regex) null;
        bool flag = this.textBoxSearch.Text.StartsWith(Resources.SearchTypePrefix_Regex, StringComparison.OrdinalIgnoreCase);
        if (flag)
        {
          try
          {
            regex = new Regex(searchText, RegexOptions.IgnoreCase | RegexOptions.Compiled);
          }
          catch (ArgumentException ex)
          {
          }
        }
        regex = this.textBoxSearch.Text.Equals("parameters", StringComparison.OrdinalIgnoreCase) ? new Regex("<P Q=\".*\">", RegexOptions.IgnoreCase | RegexOptions.Compiled) : regex;
        if (!flag || regex != null)
        {
          int num = (int) LongOperationForm.Execute<OpenLogFileForm.OurFileInfo>(Resources.Message_DeepSearch, (IEnumerable<OpenLogFileForm.OurFileInfo>) ((IEnumerable<string>) array).Select<string, OpenLogFileForm.OurFileInfo>((Func<string, OpenLogFileForm.OurFileInfo>) (path => new OpenLogFileForm.OurFileInfo(path))).OrderByDescending<OpenLogFileForm.OurFileInfo, DateTime>((Func<OpenLogFileForm.OurFileInfo, DateTime>) (i => i.LastWriteTime)), (Func<OpenLogFileForm.OurFileInfo, string>) (info => string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_DeepSearchProgress, (object) info.Name, (object) pathsToAdd.Count)), (Action<OpenLogFileForm.OurFileInfo>) (info =>
          {
            string metadataDescription;
            if (!OpenLogFileForm.TryGetMetadataDescriptionDeep(info.FullName, searchText, regex, out metadataDescription))
              return;
            pathsToAdd.Add(new
            {
              info = info,
              matchVin = SapiManager.ParseVin.Match(info.Name),
              metadataDescription = metadataDescription
            });
          }), true);
        }
      }
      foreach (var data in pathsToAdd)
      {
        OpenLogFileForm.OurFileInfo info = data.info;
        System.Text.RegularExpressions.Match matchVin = data.matchVin;
        string metadataDescription = data.metadataDescription;
        ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(string.Empty);
        if (ServerDataManager.GlobalInstance.IsMarkedForUpload(info.FullName))
          ((ListViewItem) listViewExGroupItem).StateImageIndex = 0;
        ((ListViewItem) listViewExGroupItem).SubItems.Add(info.LastWriteTime.ToString((IFormatProvider) CultureInfo.CurrentCulture));
        if (matchVin.Success)
        {
          string str1 = matchVin.Groups["vin"].Value;
          ((ListViewItem) listViewExGroupItem).SubItems.Add(str1);
          string text;
          if (!this.vinDescriptionCache.TryGetValue(str1, out text))
          {
            VinInformation vinInfo = VinInformation.GetVinInformation(str1);
            Dictionary<string, string> descriptionCache = this.vinDescriptionCache;
            string key = str1;
            IEnumerable<string> values = ((IEnumerable<string>) new string[3]
            {
              "modelyear",
              "make",
              "model"
            }).Select<string, string>((Func<string, string>) (q => vinInfo.GetInformationValue(q)));
            string str2;
            string str3 = str2 = string.Join(" ", values);
            descriptionCache[key] = str2;
            text = str3;
          }
          ((ListViewItem) listViewExGroupItem).SubItems.Add(text);
        }
        else
        {
          ((ListViewItem) listViewExGroupItem).SubItems.Add(Resources.LogFileNameUnknown);
          ((ListViewItem) listViewExGroupItem).SubItems.Add(string.Empty);
        }
        ((ListViewItem) listViewExGroupItem).SubItems.Add(FileManagement.FormatFileSize(info.Length));
        if (metadataDescription.Length > 0)
          ((ListViewItem) listViewExGroupItem).SubItems.Add(info.Name + Environment.NewLine + metadataDescription);
        else
          ((ListViewItem) listViewExGroupItem).SubItems.Add(info.Name);
        ((ListViewItem) listViewExGroupItem).Tag = (object) info;
        ((System.Windows.Forms.ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem);
      }
    }
    if (((System.Windows.Forms.ListView) this.listView).Items.Count == 0)
    {
      ((Control) this.listView).Enabled = false;
      ((System.Windows.Forms.ListView) this.listView).Items.Add((ListViewItem) new ListViewExGroupItem(this.textBoxSearch.Text.Length > 0 ? Resources.MessageNoMatchesFound : Resources.MessageNoLogFilesFound));
    }
    else
      ((Control) this.listView).Enabled = true;
    this.listView.EndUpdate();
    this.listView.UnlockSorting();
    this.okButton.Enabled = this.buttonInfo.Enabled = this.buttonCopyLink.Enabled = false;
    this.UpdateButtonHighlight();
    Cursor.Current = Cursors.Default;
  }

  private void listView_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (((System.Windows.Forms.ListView) this.listView).SelectedItems.Count > 0)
    {
      this.selectedFilePath = (((System.Windows.Forms.ListView) this.listView).SelectedItems[0].Tag as OpenLogFileForm.OurFileInfo).FullName;
      this.okButton.Enabled = this.buttonInfo.Enabled = this.buttonCopyLink.Enabled = true;
      this.buttonUpload.Enabled = !ServerDataManager.GlobalInstance.IsMarkedForUpload(this.selectedFilePath);
    }
    else
    {
      this.selectedFilePath = string.Empty;
      this.okButton.Enabled = this.buttonInfo.Enabled = this.buttonCopyLink.Enabled = this.buttonUpload.Enabled = false;
    }
  }

  private void listView_DoubleClick(object sender, EventArgs e)
  {
    if (((System.Windows.Forms.ListView) this.listView).SelectedItems.Count <= 0)
      return;
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void OnBrowseClick(object sender, EventArgs e)
  {
    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
    folderBrowserDialog.SelectedPath = this.comboBoxLookIn.Text;
    folderBrowserDialog.ShowNewFolderButton = false;
    folderBrowserDialog.Description = Resources.LogFileFolderDialogDescription;
    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
      return;
    try
    {
      this.Path = folderBrowserDialog.SelectedPath;
      this.PopulateList();
    }
    catch (PathTooLongException ex)
    {
      this.PathToLong();
    }
  }

  private void comboBoxLookIn_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.Path = this.comboBoxLookIn.SelectedItem as string;
    try
    {
      this.PopulateList();
    }
    catch (PathTooLongException ex)
    {
      this.PathToLong();
    }
  }

  private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
  {
    if (e.Column == this.lvwColumnSorter.ColumnToSort)
    {
      this.lvwColumnSorter.OrderOfSort = this.lvwColumnSorter.OrderOfSort != SortOrder.Ascending ? SortOrder.Ascending : SortOrder.Descending;
    }
    else
    {
      this.lvwColumnSorter.ColumnToSort = e.Column;
      this.lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
    }
    this.lvwColumnSorter.SortType = e.Column != this.timeStampColumnHeader.Index ? (e.Column != this.sizeColumnHeader.Index ? ListViewColumnSorter.SortBy.Text : ListViewColumnSorter.SortBy.Size) : ListViewColumnSorter.SortBy.DateTime;
    ((System.Windows.Forms.ListView) this.listView).Sort();
  }

  private void buttonLogFiles_Click(object sender, EventArgs e) => this.Path = Directories.LogFiles;

  private void buttonSummaryFiles_Click(object sender, EventArgs e)
  {
    this.Path = Directories.SummaryFiles;
  }

  private void UpdateButtonHighlight()
  {
    this.buttonLogFiles.BackColor = this.buttonSummaryFiles.BackColor = this.iconPanel.BackColor;
    if (string.Equals(this.comboBoxLookIn.Text, Directories.LogFiles, StringComparison.OrdinalIgnoreCase))
    {
      this.buttonLogFiles.BackColor = SystemColors.ControlLight;
    }
    else
    {
      if (!string.Equals(this.comboBoxLookIn.Text, Directories.SummaryFiles, StringComparison.OrdinalIgnoreCase))
        return;
      this.buttonSummaryFiles.BackColor = SystemColors.ControlLight;
    }
  }

  private DialogResult GetLogFileInfo(string filePath)
  {
    DialogResult logFileInfo = DialogResult.None;
    LogFile logFile = SapiManager.GlobalInstance.TryLoadLogFile(filePath, (Control) this);
    if (logFile != null)
    {
      using (LogFileInfoDialog logFileInfoDialog = new LogFileInfoDialog(logFile))
      {
        logFileInfo = logFileInfoDialog.ShowDialog();
        if (logFileInfo == DialogResult.OK)
        {
          this.selectedFilePath = System.IO.Path.Combine(Directories.LogFiles, logFileInfoDialog.SelectedLogFile);
          logFile.Close();
          logFile = SapiManager.GlobalInstance.TryLoadLogFile(filePath, (Control) this);
          logFileInfo = DialogResult.OK;
        }
        else
          logFileInfo = DialogResult.Cancel;
      }
      logFile.Close();
    }
    if (logFileInfo == DialogResult.OK)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
    return logFileInfo;
  }

  private void buttonInfo_Click(object sender, EventArgs ev)
  {
    int logFileInfo = (int) this.GetLogFileInfo(this.selectedFilePath);
  }

  private void buttonCopyLink_Click(object sender, EventArgs e)
  {
    this.buttonCopyLink.Enabled = false;
    FileManagement.CopyLink((Control) this, this.selectedFilePath);
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_OpenLogFileForm"));
  }

  private void textBoxSearch_TextChanged(object sender, EventArgs e)
  {
    if (this.searchTimer.Enabled)
      this.searchTimer.Stop();
    this.searchTimer.Start();
  }

  private void searchTimer_Tick(object sender, EventArgs e)
  {
    this.searchTimer.Stop();
    try
    {
      this.PopulateList();
    }
    catch (PathTooLongException ex)
    {
      this.PathToLong();
    }
  }

  private void PathToLong()
  {
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, Resources.MessageFormat_PathToLong, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
  }

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    this.backgroundWorker.CancelAsync();
    for (int index = 0; index < 10 && index < this.comboBoxLookIn.Items.Count; ++index)
      SettingsManager.GlobalInstance.SetValue<StringSetting>("LogFiles", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) "OpenDirectory", (object) index), new StringSetting(this.comboBoxLookIn.Items[index] as string), false);
    base.OnFormClosing(e);
  }

  private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
  {
    BackgroundWorker backgroundWorker = sender as BackgroundWorker;
    int num = -1;
    Queue<string> stringQueue = new Queue<string>();
    bool flag = false;
    while (!backgroundWorker.CancellationPending && !flag)
    {
      lock (this.metadataLock)
      {
        if (this.metadataCache == null || this.metadataCache.Path != this.Path)
        {
          try
          {
            stringQueue = new Queue<string>((IEnumerable<string>) Directory.GetFiles(this.Path, "*" + Directories.LogExtension, SearchOption.TopDirectoryOnly));
            num = stringQueue.Count;
            this.metadataCache = new OpenLogFileForm.MetadataCache(this.Path);
          }
          catch (PathTooLongException ex)
          {
            return;
          }
        }
        string key = stringQueue.Dequeue();
        if (!this.metadataCache.ContainsKey(key))
        {
          try
          {
            IList<LogMetadataItem> logMetadata = SapiManager.GetLogMetadata(key);
            if (logMetadata != null)
            {
              if (logMetadata.Count > 0)
                this.metadataCache.Add(key, (IList<LogMetadataItem>) logMetadata.Where<LogMetadataItem>((Func<LogMetadataItem, bool>) (i => !((LogMetadataItem) ref i).Content.Contains("02142:TIMEOUTP2CAN"))).ToList<LogMetadataItem>());
            }
          }
          catch (XmlException ex)
          {
          }
          catch (WrongEncryptionTypeException ex)
          {
          }
          catch (FormatException ex)
          {
          }
          catch (InvalidOperationException ex)
          {
          }
          catch (ArgumentException ex)
          {
          }
        }
        if (stringQueue.Count == 0)
          flag = true;
        else
          backgroundWorker.ReportProgress((num - stringQueue.Count) * 100 / num);
      }
    }
    lock (this.metadataLock)
    {
      if (this.metadataCache == null)
        return;
      this.metadataCache.Write();
    }
  }

  private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
  {
    this.progressBar.Value = e.ProgressPercentage;
  }

  private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    this.progressBar.Visible = false;
    if (this.textBoxSearch.Text.Length <= 0)
      return;
    this.searchTimer.Start();
  }

  private void buttonUpload_Click(object sender, EventArgs e)
  {
    if (((System.Windows.Forms.ListView) this.listView).SelectedItems.Count <= 0)
      return;
    ListViewExGroupItem selectedItem = ((System.Windows.Forms.ListView) this.listView).SelectedItems[0] as ListViewExGroupItem;
    ServerDataManager.GlobalInstance.AddUploadRequest(string.Empty, ((ListViewItem) selectedItem).SubItems[2].Text, this.selectedFilePath);
    ((ListViewItem) selectedItem).StateImageIndex = 0;
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_UploadSpecifiedFile, (object) this.selectedFilePath), MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (OpenLogFileForm));
    this.cancelButton = new System.Windows.Forms.Button();
    this.okButton = new System.Windows.Forms.Button();
    this.buttonBrowse = new System.Windows.Forms.Button();
    this.labelLookIn = new Label();
    this.comboBoxLookIn = new System.Windows.Forms.ComboBox();
    this.buttonLogFiles = new System.Windows.Forms.Button();
    this.buttonSummaryFiles = new System.Windows.Forms.Button();
    this.iconPanel = new Panel();
    this.buttonInfo = new System.Windows.Forms.Button();
    this.buttonCopyLink = new System.Windows.Forms.Button();
    this.labelSearch = new Label();
    this.textBoxSearch = new System.Windows.Forms.TextBox();
    this.searchTimer = new Timer(this.components);
    this.backgroundWorker = new BackgroundWorker();
    this.progressBar = new System.Windows.Forms.ProgressBar();
    this.stateImages = new ImageList(this.components);
    this.buttonUpload = new System.Windows.Forms.Button();
    this.toolTipOpenLogFile = new System.Windows.Forms.ToolTip(this.components);
    this.listView = new ListViewEx();
    this.columnHeaderImageSpacing = new ColumnHeader();
    this.timeStampColumnHeader = new ColumnHeader();
    this.vinColumnHeader = new ColumnHeader();
    this.vinDecodeColumnHeader = new ColumnHeader();
    this.sizeColumnHeader = new ColumnHeader();
    this.nameColumnHeader = new ColumnHeader();
    this.iconPanel.SuspendLayout();
    ((ISupportInitialize) this.listView).BeginInit();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.cancelButton, "cancelButton");
    this.cancelButton.DialogResult = DialogResult.Cancel;
    this.cancelButton.Name = "cancelButton";
    this.cancelButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.okButton, "okButton");
    this.okButton.DialogResult = DialogResult.OK;
    this.okButton.Name = "okButton";
    this.okButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonBrowse, "buttonBrowse");
    this.buttonBrowse.Name = "buttonBrowse";
    this.buttonBrowse.UseCompatibleTextRendering = true;
    this.buttonBrowse.UseVisualStyleBackColor = true;
    this.buttonBrowse.Click += new EventHandler(this.OnBrowseClick);
    componentResourceManager.ApplyResources((object) this.labelLookIn, "labelLookIn");
    this.labelLookIn.Name = "labelLookIn";
    componentResourceManager.ApplyResources((object) this.comboBoxLookIn, "comboBoxLookIn");
    this.comboBoxLookIn.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxLookIn.FormattingEnabled = true;
    this.comboBoxLookIn.Name = "comboBoxLookIn";
    this.comboBoxLookIn.SelectedIndexChanged += new EventHandler(this.comboBoxLookIn_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.buttonLogFiles, "buttonLogFiles");
    this.buttonLogFiles.Image = (Image) Resources.LogFiles;
    this.buttonLogFiles.Name = "buttonLogFiles";
    this.buttonLogFiles.UseVisualStyleBackColor = true;
    this.buttonLogFiles.Click += new EventHandler(this.buttonLogFiles_Click);
    componentResourceManager.ApplyResources((object) this.buttonSummaryFiles, "buttonSummaryFiles");
    this.buttonSummaryFiles.Image = (Image) Resources.SummaryFiles;
    this.buttonSummaryFiles.Name = "buttonSummaryFiles";
    this.buttonSummaryFiles.UseVisualStyleBackColor = true;
    this.buttonSummaryFiles.Click += new EventHandler(this.buttonSummaryFiles_Click);
    componentResourceManager.ApplyResources((object) this.iconPanel, "iconPanel");
    this.iconPanel.BackColor = SystemColors.Info;
    this.iconPanel.Controls.Add((Control) this.buttonLogFiles);
    this.iconPanel.Controls.Add((Control) this.buttonSummaryFiles);
    this.iconPanel.Name = "iconPanel";
    componentResourceManager.ApplyResources((object) this.buttonInfo, "buttonInfo");
    this.buttonInfo.Name = "buttonInfo";
    this.buttonInfo.UseVisualStyleBackColor = true;
    this.buttonInfo.Click += new EventHandler(this.buttonInfo_Click);
    componentResourceManager.ApplyResources((object) this.buttonCopyLink, "buttonCopyLink");
    this.buttonCopyLink.Name = "buttonCopyLink";
    this.buttonCopyLink.UseVisualStyleBackColor = true;
    this.buttonCopyLink.Click += new EventHandler(this.buttonCopyLink_Click);
    componentResourceManager.ApplyResources((object) this.labelSearch, "labelSearch");
    this.labelSearch.Name = "labelSearch";
    componentResourceManager.ApplyResources((object) this.textBoxSearch, "textBoxSearch");
    this.textBoxSearch.Name = "textBoxSearch";
    this.toolTipOpenLogFile.SetToolTip((Control) this.textBoxSearch, componentResourceManager.GetString("textBoxSearch.ToolTip"));
    this.textBoxSearch.TextChanged += new EventHandler(this.textBoxSearch_TextChanged);
    this.searchTimer.Interval = 1000;
    this.searchTimer.Tick += new EventHandler(this.searchTimer_Tick);
    this.backgroundWorker.WorkerReportsProgress = true;
    this.backgroundWorker.WorkerSupportsCancellation = true;
    this.backgroundWorker.DoWork += new DoWorkEventHandler(this.backgroundWorker_DoWork);
    this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
    this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
    componentResourceManager.ApplyResources((object) this.progressBar, "progressBar");
    this.progressBar.Name = "progressBar";
    this.stateImages.ColorDepth = ColorDepth.Depth32Bit;
    componentResourceManager.ApplyResources((object) this.stateImages, "stateImages");
    this.stateImages.TransparentColor = Color.Transparent;
    componentResourceManager.ApplyResources((object) this.buttonUpload, "buttonUpload");
    this.buttonUpload.Name = "buttonUpload";
    this.buttonUpload.UseVisualStyleBackColor = true;
    this.buttonUpload.Click += new EventHandler(this.buttonUpload_Click);
    this.toolTipOpenLogFile.AutoPopDelay = 30000;
    this.toolTipOpenLogFile.InitialDelay = 500;
    this.toolTipOpenLogFile.IsBalloon = true;
    this.toolTipOpenLogFile.ReshowDelay = 100;
    this.toolTipOpenLogFile.ShowAlways = true;
    this.toolTipOpenLogFile.ToolTipIcon = ToolTipIcon.Info;
    this.toolTipOpenLogFile.ToolTipTitle = "Help";
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.CanDelete = false;
    ((System.Windows.Forms.ListView) this.listView).Columns.AddRange(new ColumnHeader[6]
    {
      this.columnHeaderImageSpacing,
      this.timeStampColumnHeader,
      this.vinColumnHeader,
      this.vinDecodeColumnHeader,
      this.sizeColumnHeader,
      this.nameColumnHeader
    });
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    this.listView.HeaderStyle = ColumnHeaderStyle.Clickable;
    ((Control) this.listView).Name = "listView";
    this.listView.ShowGlyphs = (GlyphBehavior) 2;
    ((System.Windows.Forms.ListView) this.listView).ShowGroups = false;
    this.listView.ShowItemImages = (ImageBehavior) 1;
    ((System.Windows.Forms.ListView) this.listView).UseCompatibleStateImageBehavior = false;
    ((System.Windows.Forms.ListView) this.listView).ColumnClick += new ColumnClickEventHandler(this.listView_ColumnClick);
    ((System.Windows.Forms.ListView) this.listView).SelectedIndexChanged += new EventHandler(this.listView_SelectedIndexChanged);
    ((Control) this.listView).DoubleClick += new EventHandler(this.listView_DoubleClick);
    componentResourceManager.ApplyResources((object) this.columnHeaderImageSpacing, "columnHeaderImageSpacing");
    componentResourceManager.ApplyResources((object) this.timeStampColumnHeader, "timeStampColumnHeader");
    componentResourceManager.ApplyResources((object) this.vinColumnHeader, "vinColumnHeader");
    componentResourceManager.ApplyResources((object) this.vinDecodeColumnHeader, "vinDecodeColumnHeader");
    componentResourceManager.ApplyResources((object) this.sizeColumnHeader, "sizeColumnHeader");
    componentResourceManager.ApplyResources((object) this.nameColumnHeader, "nameColumnHeader");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.buttonUpload);
    this.Controls.Add((Control) this.progressBar);
    this.Controls.Add((Control) this.textBoxSearch);
    this.Controls.Add((Control) this.labelSearch);
    this.Controls.Add((Control) this.iconPanel);
    this.Controls.Add((Control) this.comboBoxLookIn);
    this.Controls.Add((Control) this.labelLookIn);
    this.Controls.Add((Control) this.buttonBrowse);
    this.Controls.Add((Control) this.buttonCopyLink);
    this.Controls.Add((Control) this.buttonInfo);
    this.Controls.Add((Control) this.cancelButton);
    this.Controls.Add((Control) this.okButton);
    this.Controls.Add((Control) this.listView);
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (OpenLogFileForm);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.iconPanel.ResumeLayout(false);
    ((ISupportInitialize) this.listView).EndInit();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  internal class OurFileInfo
  {
    public readonly string Name;
    public readonly string FullName;
    public readonly DateTime LastWriteTime;
    public readonly long Length;

    public OurFileInfo(string path)
    {
      FileInfo fileInfo = new FileInfo(path);
      this.Name = fileInfo.Name;
      this.FullName = fileInfo.FullName;
      this.LastWriteTime = fileInfo.LastWriteTime;
      this.Length = fileInfo.Length;
    }
  }

  private class MetadataCache
  {
    private const int CacheVersion = 2;
    private Dictionary<string, IEnumerable<LogMetadataItem>> data = new Dictionary<string, IEnumerable<LogMetadataItem>>();
    private string path;
    private const string FileName = "drumroll.logmetadatacache";
    private bool dirty;

    public string Path => this.path;

    public void Add(string key, IList<LogMetadataItem> data)
    {
      this.data.Add(key, (IEnumerable<LogMetadataItem>) new List<LogMetadataItem>((IEnumerable<LogMetadataItem>) data));
      this.dirty = true;
    }

    public bool ContainsKey(string key) => this.data.ContainsKey(key);

    public bool TryGetValue(string key, out IEnumerable<LogMetadataItem> result)
    {
      return this.data.TryGetValue(key, out result);
    }

    public MetadataCache(string path)
    {
      this.path = path;
      string path1 = System.IO.Path.Combine(this.path, "drumroll.logmetadatacache");
      if (!File.Exists(path1))
        return;
      try
      {
        using (Stream stream = (Stream) new FileStream(path1, FileMode.Open))
        {
          using (GZipStream input = new GZipStream(stream, CompressionMode.Decompress))
          {
            using (XmlReader xmlReader = XmlReader.Create((Stream) input))
            {
              xmlReader.ReadToDescendant("Cache");
              int num = 1;
              string attribute1 = xmlReader.GetAttribute("Version");
              if (attribute1 != null)
                num = int.Parse(attribute1, (IFormatProvider) CultureInfo.InvariantCulture);
              xmlReader.ReadToDescendant("File");
              do
              {
                List<LogMetadataItem> logMetadataItemList = new List<LogMetadataItem>();
                string key = System.IO.Path.Combine(path, xmlReader.GetAttribute("Name"));
                xmlReader.ReadToDescendant("Item");
                do
                {
                  string attribute2 = xmlReader.GetAttribute("Type");
                  string attribute3 = xmlReader.GetAttribute("Time");
                  string attribute4 = xmlReader.GetAttribute("Ecu");
                  string str;
                  if (num == 1)
                  {
                    str = xmlReader.GetAttribute("Content");
                    xmlReader.Skip();
                  }
                  else
                    str = xmlReader.ReadElementContentAsString();
                  if (str != null)
                    logMetadataItemList.Add(new LogMetadataItem((LogMetadataType) Enum.Parse(typeof (LogMetadataType), attribute2), attribute4, str, attribute3));
                }
                while (xmlReader.LocalName == "Item" && xmlReader.NodeType == XmlNodeType.Element);
                if (logMetadataItemList.Count > 0 && !this.data.ContainsKey(key))
                  this.data.Add(key, (IEnumerable<LogMetadataItem>) logMetadataItemList);
                xmlReader.Skip();
              }
              while (xmlReader.LocalName == "File" && xmlReader.NodeType == XmlNodeType.Element);
            }
          }
        }
      }
      catch (IOException ex)
      {
        StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType) 1, (object) this));
      }
      catch (XmlException ex)
      {
        StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType) 1, (object) this));
      }
    }

    private static string RemoveInvalidXmlChars(string content)
    {
      return new string(content.Where<char>((Func<char, bool>) (ch => XmlConvert.IsXmlChar(ch))).ToArray<char>());
    }

    public void Write()
    {
      if (!this.dirty)
        return;
      string path = System.IO.Path.Combine(this.path, "drumroll.logmetadatacache");
      try
      {
        using (Stream stream = (Stream) new FileStream(path, FileMode.OpenOrCreate))
        {
          using (GZipStream w = new GZipStream(stream, CompressionMode.Compress))
          {
            using (XmlWriter xmlWriter = (XmlWriter) new XmlTextWriter((Stream) w, (Encoding) null))
            {
              xmlWriter.WriteStartElement("Cache");
              xmlWriter.WriteAttributeString("Version", 2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              foreach (KeyValuePair<string, IEnumerable<LogMetadataItem>> keyValuePair in this.data)
              {
                xmlWriter.WriteStartElement("File");
                xmlWriter.WriteStartAttribute("Name");
                xmlWriter.WriteString(System.IO.Path.GetFileName(keyValuePair.Key));
                xmlWriter.WriteEndAttribute();
                foreach (LogMetadataItem logMetadataItem in keyValuePair.Value)
                {
                  xmlWriter.WriteStartElement("Item");
                  xmlWriter.WriteAttributeString("Type", ((LogMetadataItem) ref logMetadataItem).Type.ToString());
                  xmlWriter.WriteAttributeString("Time", Sapi.TimeToString(((LogMetadataItem) ref logMetadataItem).Time));
                  xmlWriter.WriteAttributeString("Ecu", ((LogMetadataItem) ref logMetadataItem).Ecu != null ? ((LogMetadataItem) ref logMetadataItem).Ecu : string.Empty);
                  xmlWriter.WriteString(OpenLogFileForm.MetadataCache.RemoveInvalidXmlChars(((LogMetadataItem) ref logMetadataItem).Content));
                  xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
              }
              xmlWriter.WriteEndElement();
            }
          }
        }
      }
      catch (IOException ex)
      {
        StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType) 1, (object) this));
      }
      this.dirty = false;
    }
  }
}
