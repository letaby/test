using System;
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
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class OpenLogFileForm : Form
{
	internal class OurFileInfo
	{
		public readonly string Name;

		public readonly string FullName;

		public readonly DateTime LastWriteTime;

		public readonly long Length;

		public OurFileInfo(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			Name = fileInfo.Name;
			FullName = fileInfo.FullName;
			LastWriteTime = fileInfo.LastWriteTime;
			Length = fileInfo.Length;
		}
	}

	private class MetadataCache
	{
		private const int CacheVersion = 2;

		private Dictionary<string, IEnumerable<LogMetadataItem>> data = new Dictionary<string, IEnumerable<LogMetadataItem>>();

		private string path;

		private const string FileName = "drumroll.logmetadatacache";

		private bool dirty;

		public string Path => path;

		public void Add(string key, IList<LogMetadataItem> data)
		{
			this.data.Add(key, new List<LogMetadataItem>(data));
			dirty = true;
		}

		public bool ContainsKey(string key)
		{
			return data.ContainsKey(key);
		}

		public bool TryGetValue(string key, out IEnumerable<LogMetadataItem> result)
		{
			return data.TryGetValue(key, out result);
		}

		public MetadataCache(string path)
		{
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Expected O, but got Unknown
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Expected O, but got Unknown
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			this.path = path;
			string text = System.IO.Path.Combine(this.path, "drumroll.logmetadatacache");
			if (!File.Exists(text))
			{
				return;
			}
			try
			{
				using Stream stream = new FileStream(text, FileMode.Open);
				using GZipStream input = new GZipStream(stream, CompressionMode.Decompress);
				using XmlReader xmlReader = XmlReader.Create(input);
				xmlReader.ReadToDescendant("Cache");
				int num = 1;
				string attribute = xmlReader.GetAttribute("Version");
				if (attribute != null)
				{
					num = int.Parse(attribute, CultureInfo.InvariantCulture);
				}
				xmlReader.ReadToDescendant("File");
				do
				{
					List<LogMetadataItem> list = new List<LogMetadataItem>();
					string key = System.IO.Path.Combine(path, xmlReader.GetAttribute("Name"));
					xmlReader.ReadToDescendant("Item");
					do
					{
						string attribute2 = xmlReader.GetAttribute("Type");
						string attribute3 = xmlReader.GetAttribute("Time");
						string attribute4 = xmlReader.GetAttribute("Ecu");
						string text2;
						if (num == 1)
						{
							text2 = xmlReader.GetAttribute("Content");
							xmlReader.Skip();
						}
						else
						{
							text2 = xmlReader.ReadElementContentAsString();
						}
						if (text2 != null)
						{
							list.Add(new LogMetadataItem((LogMetadataType)Enum.Parse(typeof(LogMetadataType), attribute2), attribute4, text2, attribute3));
						}
					}
					while (xmlReader.LocalName == "Item" && xmlReader.NodeType == XmlNodeType.Element);
					if (list.Count > 0 && !data.ContainsKey(key))
					{
						data.Add(key, list);
					}
					xmlReader.Skip();
				}
				while (xmlReader.LocalName == "File" && xmlReader.NodeType == XmlNodeType.Element);
			}
			catch (IOException ex)
			{
				StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType)1, (object)this));
			}
			catch (XmlException ex2)
			{
				StatusLog.Add(new StatusMessage(ex2.Message, (StatusMessageType)1, (object)this));
			}
		}

		private static string RemoveInvalidXmlChars(string content)
		{
			return new string(content.Where((char ch) => XmlConvert.IsXmlChar(ch)).ToArray());
		}

		public void Write()
		{
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Expected O, but got Unknown
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			if (!dirty)
			{
				return;
			}
			string text = System.IO.Path.Combine(path, "drumroll.logmetadatacache");
			try
			{
				using Stream stream = new FileStream(text, FileMode.OpenOrCreate);
				using GZipStream w = new GZipStream(stream, CompressionMode.Compress);
				using XmlWriter xmlWriter = new XmlTextWriter(w, null);
				xmlWriter.WriteStartElement("Cache");
				xmlWriter.WriteAttributeString("Version", 2.ToString(CultureInfo.InvariantCulture));
				foreach (KeyValuePair<string, IEnumerable<LogMetadataItem>> datum in data)
				{
					xmlWriter.WriteStartElement("File");
					xmlWriter.WriteStartAttribute("Name");
					xmlWriter.WriteString(System.IO.Path.GetFileName(datum.Key));
					xmlWriter.WriteEndAttribute();
					foreach (LogMetadataItem item in datum.Value)
					{
						LogMetadataItem current2 = item;
						xmlWriter.WriteStartElement("Item");
						xmlWriter.WriteAttributeString("Type", ((object)((LogMetadataItem)(ref current2)).Type/*cast due to .constrained prefix*/).ToString());
						xmlWriter.WriteAttributeString("Time", Sapi.TimeToString(((LogMetadataItem)(ref current2)).Time));
						xmlWriter.WriteAttributeString("Ecu", (((LogMetadataItem)(ref current2)).Ecu != null) ? ((LogMetadataItem)(ref current2)).Ecu : string.Empty);
						xmlWriter.WriteString(RemoveInvalidXmlChars(((LogMetadataItem)(ref current2)).Content));
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
			catch (IOException ex)
			{
				StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType)1, (object)this));
			}
			dirty = false;
		}
	}

	private MetadataCache metadataCache;

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

	private Button cancelButton;

	private Button okButton;

	private Button buttonBrowse;

	private Label labelLookIn;

	private ComboBox comboBoxLookIn;

	private Button buttonLogFiles;

	private Button buttonSummaryFiles;

	private Panel iconPanel;

	private Button buttonInfo;

	private Button buttonCopyLink;

	private ColumnHeader timeStampColumnHeader;

	private ColumnHeader vinColumnHeader;

	private ColumnHeader vinDecodeColumnHeader;

	private ColumnHeader sizeColumnHeader;

	private ColumnHeader nameColumnHeader;

	private ColumnHeader columnHeaderImageSpacing;

	private Label labelSearch;

	private TextBox textBoxSearch;

	private Timer searchTimer;

	private BackgroundWorker backgroundWorker;

	private ProgressBar progressBar;

	private ImageList stateImages;

	private Button buttonUpload;

	private ToolTip toolTipOpenLogFile;

	private string Path
	{
		get
		{
			return path;
		}
		set
		{
			if (value != path)
			{
				lock (metadataLock)
				{
					path = value;
				}
				if (comboBoxLookIn.Items.Contains(path))
				{
					comboBoxLookIn.Items.Remove(path);
				}
				comboBoxLookIn.Items.Insert(0, path);
				comboBoxLookIn.Text = path;
				if (!backgroundWorker.IsBusy)
				{
					progressBar.Visible = true;
					backgroundWorker.RunWorkerAsync();
				}
			}
		}
	}

	public string FileName => selectedFilePath;

	private static bool Match(LogMetadataItem item, string searchText, LogMetadataType prefixType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected I4, but got Unknown
		if ((int)prefixType != 0)
		{
			if (prefixType != ((LogMetadataItem)(ref item)).Type)
			{
				return false;
			}
			if (searchText.Length == 0)
			{
				return true;
			}
		}
		if (((LogMetadataItem)(ref item)).Ecu == searchText)
		{
			return true;
		}
		LogMetadataType type = ((LogMetadataItem)(ref item)).Type;
		switch (type - 1)
		{
		case 1:
			if (((LogMetadataItem)(ref item)).Content.Contains("/") && !searchText.Contains("/"))
			{
				string[] array = ((LogMetadataItem)(ref item)).Content.Split(new char[1] { '/' }, StringSplitOptions.None);
				if (!(array[0] == searchText))
				{
					return array[1] == searchText;
				}
				return true;
			}
			return string.Equals(((LogMetadataItem)(ref item)).Content, searchText, StringComparison.OrdinalIgnoreCase);
		case 0:
			return string.Equals(((LogMetadataItem)(ref item)).Content, searchText, StringComparison.OrdinalIgnoreCase);
		case 2:
			return ((LogMetadataItem)(ref item)).Content.ToUpperInvariant().Contains(searchText);
		default:
			return false;
		}
	}

	private static LogMetadataType GetMetadataTypeFromString(string prefix)
	{
		if (!string.Equals(prefix, Resources.MetadataTypeIdentification, StringComparison.OrdinalIgnoreCase))
		{
			if (!string.Equals(prefix, Resources.MetadataTypeFaultCode, StringComparison.OrdinalIgnoreCase))
			{
				if (!string.Equals(prefix, Resources.MetadataTypeLabel, StringComparison.OrdinalIgnoreCase))
				{
					return (LogMetadataType)0;
				}
				return (LogMetadataType)3;
			}
			return (LogMetadataType)2;
		}
		return (LogMetadataType)1;
	}

	private static string GetMetadataDescription(LogMetadataItem item)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		string text = string.Empty;
		LogMetadataType type = ((LogMetadataItem)(ref item)).Type;
		switch (type - 1)
		{
		case 0:
			text = Resources.MetadataTypeIdentification;
			break;
		case 1:
			text = Resources.MetadataTypeFaultCode;
			break;
		case 2:
			text = Resources.MetadataTypeLabel;
			break;
		}
		if (!string.IsNullOrEmpty(((LogMetadataItem)(ref item)).Ecu))
		{
			return ((LogMetadataItem)(ref item)).Time.ToString(CultureInfo.CurrentCulture) + ": " + text + " - " + ((LogMetadataItem)(ref item)).Ecu + " - " + ((LogMetadataItem)(ref item)).Content;
		}
		return ((LogMetadataItem)(ref item)).Time.ToString(CultureInfo.CurrentCulture) + ": " + text + " - " + ((LogMetadataItem)(ref item)).Content;
	}

	private bool TryGetMetadataDescription(string path, string text, LogMetadataType prefixType, out string metadataDescription)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		metadataDescription = string.Empty;
		IEnumerable<LogMetadataItem> result = null;
		lock (metadataLock)
		{
			if (metadataCache != null)
			{
				metadataCache.TryGetValue(path, out result);
			}
		}
		if (result != null)
		{
			IEnumerable<LogMetadataItem> enumerable = from x in result
				where Match(x, text, prefixType)
				orderby ((LogMetadataItem)(ref x)).Time
				select x;
			if (enumerable.Any())
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (LogMetadataItem item in enumerable)
				{
					stringBuilder.AppendLine(GetMetadataDescription(item));
					if (++num >= 25)
					{
						stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.FormatMaxMatchStrings, enumerable.Count() - 25));
						break;
					}
				}
				metadataDescription = stringBuilder.ToString();
				return true;
			}
		}
		return false;
	}

	private static bool TryGetMetadataDescriptionDeep(string path, string searchText, Regex regex, out string metadataDescription)
	{
		metadataDescription = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		using (Stream stream = SapiManager.OpenLogStream(path, false))
		{
			if (stream != null)
			{
				using StreamReader streamReader = new StreamReader(stream);
				for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
				{
					if (regex?.Match(text).Success ?? (text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1))
					{
						stringBuilder.AppendLine(text);
						if (++num >= 25)
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.FormatMaxMatchStrings, string.Empty));
							break;
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
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Band.Normal))
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Rebar.Band.Normal);
			iconPanel.BackColor = visualStyleRenderer.GetColor(ColorProperty.FillColorHint);
		}
		stateImages.Images.Add(Resources.upload_log);
		((ListView)(object)listView).StateImageList = stateImages;
		lvwColumnSorter = new ListViewColumnSorter();
		lvwColumnSorter.ColumnToSort = timeStampColumnHeader.Index;
		lvwColumnSorter.OrderOfSort = SortOrder.Descending;
		lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.DateTime;
		((ListView)(object)listView).ListViewItemSorter = lvwColumnSorter;
		okButton.Enabled = (buttonInfo.Enabled = (buttonCopyLink.Enabled = (buttonUpload.Enabled = false)));
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		int num = ((Control)(object)listView).ClientSize.Width - SystemInformation.VerticalScrollBarWidth * 2 - columnHeaderImageSpacing.Width;
		timeStampColumnHeader.Width = num * 3 / 20;
		vinColumnHeader.Width = num * 3 / 20;
		vinDecodeColumnHeader.Width = num * 6 / 20;
		sizeColumnHeader.Width = num / 20;
		nameColumnHeader.Width = num * 7 / 20;
		StringSetting val = new StringSetting(string.Empty);
		for (int i = 0; i < 10; i++)
		{
			StringSetting value = SettingsManager.GlobalInstance.GetValue<StringSetting>("LogFiles", string.Format(CultureInfo.InvariantCulture, "{0}{1}", "OpenDirectory", i), val);
			if (value == val)
			{
				break;
			}
			if (Directory.Exists(value.Value))
			{
				comboBoxLookIn.Items.Add(value.Value);
			}
		}
		if (comboBoxLookIn.Items.Count > 0)
		{
			Path = comboBoxLookIn.Items[0] as string;
		}
		else
		{
			Path = DefaultDirectory.Value;
		}
		base.OnLoad(e);
	}

	private void PopulateList()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		Cursor.Current = Cursors.WaitCursor;
		listView.LockSorting();
		listView.BeginUpdate();
		((ListView)(object)listView).Items.Clear();
		if (backgroundWorker.IsBusy && textBoxSearch.Text.Length > 0)
		{
			ListViewExGroupItem val = new ListViewExGroupItem(string.Empty);
			((ListViewItem)(object)val).SubItems.Add(Resources.MessageSearching);
			((ListView)(object)listView).Items.Add((ListViewItem)(object)val);
		}
		else
		{
			string[] second = new string[2]
			{
				SapiManager.GlobalInstance.CurrentLogFileInformation.SummaryFilePath,
				SapiManager.GlobalInstance.CurrentLogFileInformation.LogFilePath
			};
			string[] array = Directory.GetFiles(Path, "*" + Directories.LogExtension, SearchOption.TopDirectoryOnly).Except(second).ToArray();
			var pathsToAdd = Enumerable.Repeat(new
			{
				info = (OurFileInfo)null,
				matchVin = (Match)null,
				metadataDescription = (string)null
			}, 0).ToList();
			if (!textBoxSearch.Text.StartsWith(Resources.SearchTypePrefix_Deep, StringComparison.OrdinalIgnoreCase) && !textBoxSearch.Text.StartsWith(Resources.SearchTypePrefix_Regex, StringComparison.OrdinalIgnoreCase) && !textBoxSearch.Text.Equals("parameters", StringComparison.OrdinalIgnoreCase))
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					OurFileInfo ourFileInfo = new OurFileInfo(text);
					Match match = SapiManager.ParseVin.Match(ourFileInfo.Name);
					bool flag = true;
					string metadataDescription = string.Empty;
					if (textBoxSearch.Text.Length > 0)
					{
						LogMetadataType val2 = (LogMetadataType)0;
						string text2 = textBoxSearch.Text.ToUpperInvariant();
						int num = text2.IndexOf(':');
						if (num > -1)
						{
							val2 = GetMetadataTypeFromString(text2.Substring(0, num));
							if ((int)val2 != 0)
							{
								text2 = text2.Substring(num + 1);
							}
						}
						flag = TryGetMetadataDescription(text, text2, val2, out metadataDescription);
						if (!flag && match.Success && (int)val2 == 0)
						{
							flag = match.Groups["vin"].Value.ToUpperInvariant().Contains(text2);
						}
					}
					if (flag)
					{
						pathsToAdd.Add(new
						{
							info = ourFileInfo,
							matchVin = match,
							metadataDescription = metadataDescription
						});
					}
				}
			}
			else
			{
				string searchText = textBoxSearch.Text.Substring(textBoxSearch.Text.IndexOf(':') + 1);
				Regex regex = null;
				bool flag2 = textBoxSearch.Text.StartsWith(Resources.SearchTypePrefix_Regex, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					try
					{
						regex = new Regex(searchText, RegexOptions.IgnoreCase | RegexOptions.Compiled);
					}
					catch (ArgumentException)
					{
					}
				}
				regex = (textBoxSearch.Text.Equals("parameters", StringComparison.OrdinalIgnoreCase) ? new Regex("<P Q=\".*\">", RegexOptions.IgnoreCase | RegexOptions.Compiled) : regex);
				if (!flag2 || regex != null)
				{
					LongOperationForm.Execute<OurFileInfo>(Resources.Message_DeepSearch, (IEnumerable<OurFileInfo>)(from path in array
						select new OurFileInfo(path) into ourFileInfo2
						orderby ourFileInfo2.LastWriteTime descending
						select ourFileInfo2), (Func<OurFileInfo, string>)((OurFileInfo ourFileInfo2) => string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_DeepSearchProgress, ourFileInfo2.Name, pathsToAdd.Count)), (Action<OurFileInfo>)delegate(OurFileInfo ourFileInfo2)
					{
						if (TryGetMetadataDescriptionDeep(ourFileInfo2.FullName, searchText, regex, out var metadataDescription3))
						{
							pathsToAdd.Add(new
							{
								info = ourFileInfo2,
								matchVin = SapiManager.ParseVin.Match(ourFileInfo2.Name),
								metadataDescription = metadataDescription3
							});
						}
					}, true);
				}
			}
			foreach (var item in pathsToAdd)
			{
				OurFileInfo info = item.info;
				Match matchVin = item.matchVin;
				string metadataDescription2 = item.metadataDescription;
				ListViewExGroupItem val3 = new ListViewExGroupItem(string.Empty);
				if (ServerDataManager.GlobalInstance.IsMarkedForUpload(info.FullName))
				{
					((ListViewItem)(object)val3).StateImageIndex = 0;
				}
				((ListViewItem)(object)val3).SubItems.Add(info.LastWriteTime.ToString(CultureInfo.CurrentCulture));
				if (matchVin.Success)
				{
					string value = matchVin.Groups["vin"].Value;
					((ListViewItem)(object)val3).SubItems.Add(value);
					if (!vinDescriptionCache.TryGetValue(value, out var value2))
					{
						VinInformation vinInfo = VinInformation.GetVinInformation(value);
						string text3 = (vinDescriptionCache[value] = string.Join(" ", new string[3] { "modelyear", "make", "model" }.Select((string q) => vinInfo.GetInformationValue(q))));
						value2 = text3;
					}
					((ListViewItem)(object)val3).SubItems.Add(value2);
				}
				else
				{
					((ListViewItem)(object)val3).SubItems.Add(Resources.LogFileNameUnknown);
					((ListViewItem)(object)val3).SubItems.Add(string.Empty);
				}
				((ListViewItem)(object)val3).SubItems.Add(FileManagement.FormatFileSize(info.Length));
				if (metadataDescription2.Length > 0)
				{
					((ListViewItem)(object)val3).SubItems.Add(info.Name + Environment.NewLine + metadataDescription2);
				}
				else
				{
					((ListViewItem)(object)val3).SubItems.Add(info.Name);
				}
				((ListViewItem)(object)val3).Tag = info;
				((ListView)(object)listView).Items.Add((ListViewItem)(object)val3);
			}
		}
		if (((ListView)(object)listView).Items.Count == 0)
		{
			((Control)(object)listView).Enabled = false;
			((ListView)(object)listView).Items.Add((ListViewItem)new ListViewExGroupItem((textBoxSearch.Text.Length > 0) ? Resources.MessageNoMatchesFound : Resources.MessageNoLogFilesFound));
		}
		else
		{
			((Control)(object)listView).Enabled = true;
		}
		listView.EndUpdate();
		listView.UnlockSorting();
		Button button = okButton;
		Button button2 = buttonInfo;
		bool flag3 = (buttonCopyLink.Enabled = false);
		bool enabled = (button2.Enabled = flag3);
		button.Enabled = enabled;
		UpdateButtonHighlight();
		Cursor.Current = Cursors.Default;
	}

	private void listView_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (((ListView)(object)listView).SelectedItems.Count > 0)
		{
			OurFileInfo ourFileInfo = ((ListView)(object)listView).SelectedItems[0].Tag as OurFileInfo;
			selectedFilePath = ourFileInfo.FullName;
			Button button = okButton;
			Button button2 = buttonInfo;
			bool flag = (buttonCopyLink.Enabled = true);
			bool enabled = (button2.Enabled = flag);
			button.Enabled = enabled;
			buttonUpload.Enabled = !ServerDataManager.GlobalInstance.IsMarkedForUpload(selectedFilePath);
		}
		else
		{
			selectedFilePath = string.Empty;
			Button button3 = okButton;
			Button button4 = buttonInfo;
			Button button5 = buttonCopyLink;
			bool flag4 = (buttonUpload.Enabled = false);
			bool flag = (button5.Enabled = flag4);
			bool enabled = (button4.Enabled = flag);
			button3.Enabled = enabled;
		}
	}

	private void listView_DoubleClick(object sender, EventArgs e)
	{
		if (((ListView)(object)listView).SelectedItems.Count > 0)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void OnBrowseClick(object sender, EventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.SelectedPath = comboBoxLookIn.Text;
		folderBrowserDialog.ShowNewFolderButton = false;
		folderBrowserDialog.Description = Resources.LogFileFolderDialogDescription;
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			try
			{
				Path = folderBrowserDialog.SelectedPath;
				PopulateList();
			}
			catch (PathTooLongException)
			{
				PathToLong();
			}
		}
	}

	private void comboBoxLookIn_SelectedIndexChanged(object sender, EventArgs e)
	{
		Path = comboBoxLookIn.SelectedItem as string;
		try
		{
			PopulateList();
		}
		catch (PathTooLongException)
		{
			PathToLong();
		}
	}

	private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		if (e.Column == lvwColumnSorter.ColumnToSort)
		{
			if (lvwColumnSorter.OrderOfSort == SortOrder.Ascending)
			{
				lvwColumnSorter.OrderOfSort = SortOrder.Descending;
			}
			else
			{
				lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
			}
		}
		else
		{
			lvwColumnSorter.ColumnToSort = e.Column;
			lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
		}
		if (e.Column == timeStampColumnHeader.Index)
		{
			lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.DateTime;
		}
		else if (e.Column == sizeColumnHeader.Index)
		{
			lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.Size;
		}
		else
		{
			lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.Text;
		}
		((ListView)(object)listView).Sort();
	}

	private void buttonLogFiles_Click(object sender, EventArgs e)
	{
		Path = Directories.LogFiles;
	}

	private void buttonSummaryFiles_Click(object sender, EventArgs e)
	{
		Path = Directories.SummaryFiles;
	}

	private void UpdateButtonHighlight()
	{
		Button button = buttonLogFiles;
		Color backColor = (buttonSummaryFiles.BackColor = iconPanel.BackColor);
		button.BackColor = backColor;
		if (string.Equals(comboBoxLookIn.Text, Directories.LogFiles, StringComparison.OrdinalIgnoreCase))
		{
			buttonLogFiles.BackColor = SystemColors.ControlLight;
		}
		else if (string.Equals(comboBoxLookIn.Text, Directories.SummaryFiles, StringComparison.OrdinalIgnoreCase))
		{
			buttonSummaryFiles.BackColor = SystemColors.ControlLight;
		}
	}

	private DialogResult GetLogFileInfo(string filePath)
	{
		DialogResult dialogResult = DialogResult.None;
		LogFile val = SapiManager.GlobalInstance.TryLoadLogFile(filePath, (Control)this);
		if (val != null)
		{
			using (LogFileInfoDialog logFileInfoDialog = new LogFileInfoDialog(val))
			{
				dialogResult = logFileInfoDialog.ShowDialog();
				if (dialogResult == DialogResult.OK)
				{
					selectedFilePath = System.IO.Path.Combine(Directories.LogFiles, logFileInfoDialog.SelectedLogFile);
					val.Close();
					val = SapiManager.GlobalInstance.TryLoadLogFile(filePath, (Control)this);
					dialogResult = DialogResult.OK;
				}
				else
				{
					dialogResult = DialogResult.Cancel;
				}
			}
			val.Close();
		}
		if (dialogResult == DialogResult.OK)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
		return dialogResult;
	}

	private void buttonInfo_Click(object sender, EventArgs ev)
	{
		GetLogFileInfo(selectedFilePath);
	}

	private void buttonCopyLink_Click(object sender, EventArgs e)
	{
		buttonCopyLink.Enabled = false;
		FileManagement.CopyLink((Control)this, selectedFilePath);
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_OpenLogFileForm"));
	}

	private void textBoxSearch_TextChanged(object sender, EventArgs e)
	{
		if (searchTimer.Enabled)
		{
			searchTimer.Stop();
		}
		searchTimer.Start();
	}

	private void searchTimer_Tick(object sender, EventArgs e)
	{
		searchTimer.Stop();
		try
		{
			PopulateList();
		}
		catch (PathTooLongException)
		{
			PathToLong();
		}
	}

	private void PathToLong()
	{
		ControlHelpers.ShowMessageBox((Control)this, Resources.MessageFormat_PathToLong, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		backgroundWorker.CancelAsync();
		for (int i = 0; i < 10 && i < comboBoxLookIn.Items.Count; i++)
		{
			SettingsManager.GlobalInstance.SetValue<StringSetting>("LogFiles", string.Format(CultureInfo.InvariantCulture, "{0}{1}", "OpenDirectory", i), new StringSetting(comboBoxLookIn.Items[i] as string), false);
		}
		base.OnFormClosing(e);
	}

	private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker backgroundWorker = sender as BackgroundWorker;
		int num = -1;
		Queue<string> queue = new Queue<string>();
		bool flag = false;
		while (!backgroundWorker.CancellationPending && !flag)
		{
			lock (metadataLock)
			{
				if (metadataCache == null || metadataCache.Path != Path)
				{
					try
					{
						queue = new Queue<string>(Directory.GetFiles(Path, "*" + Directories.LogExtension, SearchOption.TopDirectoryOnly));
						num = queue.Count;
						metadataCache = new MetadataCache(Path);
					}
					catch (PathTooLongException)
					{
						return;
					}
				}
				string text = queue.Dequeue();
				if (!metadataCache.ContainsKey(text))
				{
					try
					{
						IList<LogMetadataItem> logMetadata = SapiManager.GetLogMetadata(text);
						if (logMetadata != null && logMetadata.Count > 0)
						{
							metadataCache.Add(text, logMetadata.Where((LogMetadataItem i) => !((LogMetadataItem)(ref i)).Content.Contains("02142:TIMEOUTP2CAN")).ToList());
						}
					}
					catch (XmlException)
					{
					}
					catch (WrongEncryptionTypeException)
					{
					}
					catch (FormatException)
					{
					}
					catch (InvalidOperationException)
					{
					}
					catch (ArgumentException)
					{
					}
				}
				if (queue.Count == 0)
				{
					flag = true;
				}
				else
				{
					backgroundWorker.ReportProgress((num - queue.Count) * 100 / num);
				}
			}
		}
		lock (metadataLock)
		{
			if (metadataCache != null)
			{
				metadataCache.Write();
			}
		}
	}

	private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		progressBar.Value = e.ProgressPercentage;
	}

	private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		progressBar.Visible = false;
		if (textBoxSearch.Text.Length > 0)
		{
			searchTimer.Start();
		}
	}

	private void buttonUpload_Click(object sender, EventArgs e)
	{
		if (((ListView)(object)listView).SelectedItems.Count > 0)
		{
			ListViewItem listViewItem = ((ListView)(object)listView).SelectedItems[0];
			ListViewExGroupItem val = (ListViewExGroupItem)(object)((listViewItem is ListViewExGroupItem) ? listViewItem : null);
			ServerDataManager.GlobalInstance.AddUploadRequest(string.Empty, ((ListViewItem)(object)val).SubItems[2].Text, selectedFilePath);
			((ListViewItem)(object)val).StateImageIndex = 0;
			ControlHelpers.ShowMessageBox((Control)this, string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_UploadSpecifiedFile, selectedFilePath), MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.OpenLogFileForm));
		this.cancelButton = new System.Windows.Forms.Button();
		this.okButton = new System.Windows.Forms.Button();
		this.buttonBrowse = new System.Windows.Forms.Button();
		this.labelLookIn = new System.Windows.Forms.Label();
		this.comboBoxLookIn = new System.Windows.Forms.ComboBox();
		this.buttonLogFiles = new System.Windows.Forms.Button();
		this.buttonSummaryFiles = new System.Windows.Forms.Button();
		this.iconPanel = new System.Windows.Forms.Panel();
		this.buttonInfo = new System.Windows.Forms.Button();
		this.buttonCopyLink = new System.Windows.Forms.Button();
		this.labelSearch = new System.Windows.Forms.Label();
		this.textBoxSearch = new System.Windows.Forms.TextBox();
		this.searchTimer = new System.Windows.Forms.Timer(this.components);
		this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
		this.progressBar = new System.Windows.Forms.ProgressBar();
		this.stateImages = new System.Windows.Forms.ImageList(this.components);
		this.buttonUpload = new System.Windows.Forms.Button();
		this.toolTipOpenLogFile = new System.Windows.Forms.ToolTip(this.components);
		this.listView = new ListViewEx();
		this.columnHeaderImageSpacing = new System.Windows.Forms.ColumnHeader();
		this.timeStampColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.vinColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.vinDecodeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.sizeColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.iconPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.listView).BeginInit();
		base.SuspendLayout();
		resources.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.okButton, "okButton");
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
		this.buttonBrowse.Name = "buttonBrowse";
		this.buttonBrowse.UseCompatibleTextRendering = true;
		this.buttonBrowse.UseVisualStyleBackColor = true;
		this.buttonBrowse.Click += new System.EventHandler(OnBrowseClick);
		resources.ApplyResources(this.labelLookIn, "labelLookIn");
		this.labelLookIn.Name = "labelLookIn";
		resources.ApplyResources(this.comboBoxLookIn, "comboBoxLookIn");
		this.comboBoxLookIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxLookIn.FormattingEnabled = true;
		this.comboBoxLookIn.Name = "comboBoxLookIn";
		this.comboBoxLookIn.SelectedIndexChanged += new System.EventHandler(comboBoxLookIn_SelectedIndexChanged);
		resources.ApplyResources(this.buttonLogFiles, "buttonLogFiles");
		this.buttonLogFiles.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.LogFiles;
		this.buttonLogFiles.Name = "buttonLogFiles";
		this.buttonLogFiles.UseVisualStyleBackColor = true;
		this.buttonLogFiles.Click += new System.EventHandler(buttonLogFiles_Click);
		resources.ApplyResources(this.buttonSummaryFiles, "buttonSummaryFiles");
		this.buttonSummaryFiles.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.SummaryFiles;
		this.buttonSummaryFiles.Name = "buttonSummaryFiles";
		this.buttonSummaryFiles.UseVisualStyleBackColor = true;
		this.buttonSummaryFiles.Click += new System.EventHandler(buttonSummaryFiles_Click);
		resources.ApplyResources(this.iconPanel, "iconPanel");
		this.iconPanel.BackColor = System.Drawing.SystemColors.Info;
		this.iconPanel.Controls.Add(this.buttonLogFiles);
		this.iconPanel.Controls.Add(this.buttonSummaryFiles);
		this.iconPanel.Name = "iconPanel";
		resources.ApplyResources(this.buttonInfo, "buttonInfo");
		this.buttonInfo.Name = "buttonInfo";
		this.buttonInfo.UseVisualStyleBackColor = true;
		this.buttonInfo.Click += new System.EventHandler(buttonInfo_Click);
		resources.ApplyResources(this.buttonCopyLink, "buttonCopyLink");
		this.buttonCopyLink.Name = "buttonCopyLink";
		this.buttonCopyLink.UseVisualStyleBackColor = true;
		this.buttonCopyLink.Click += new System.EventHandler(buttonCopyLink_Click);
		resources.ApplyResources(this.labelSearch, "labelSearch");
		this.labelSearch.Name = "labelSearch";
		resources.ApplyResources(this.textBoxSearch, "textBoxSearch");
		this.textBoxSearch.Name = "textBoxSearch";
		this.toolTipOpenLogFile.SetToolTip(this.textBoxSearch, resources.GetString("textBoxSearch.ToolTip"));
		this.textBoxSearch.TextChanged += new System.EventHandler(textBoxSearch_TextChanged);
		this.searchTimer.Interval = 1000;
		this.searchTimer.Tick += new System.EventHandler(searchTimer_Tick);
		this.backgroundWorker.WorkerReportsProgress = true;
		this.backgroundWorker.WorkerSupportsCancellation = true;
		this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
		this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
		this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
		resources.ApplyResources(this.progressBar, "progressBar");
		this.progressBar.Name = "progressBar";
		this.stateImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
		resources.ApplyResources(this.stateImages, "stateImages");
		this.stateImages.TransparentColor = System.Drawing.Color.Transparent;
		resources.ApplyResources(this.buttonUpload, "buttonUpload");
		this.buttonUpload.Name = "buttonUpload";
		this.buttonUpload.UseVisualStyleBackColor = true;
		this.buttonUpload.Click += new System.EventHandler(buttonUpload_Click);
		this.toolTipOpenLogFile.AutoPopDelay = 30000;
		this.toolTipOpenLogFile.InitialDelay = 500;
		this.toolTipOpenLogFile.IsBalloon = true;
		this.toolTipOpenLogFile.ReshowDelay = 100;
		this.toolTipOpenLogFile.ShowAlways = true;
		this.toolTipOpenLogFile.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
		this.toolTipOpenLogFile.ToolTipTitle = "Help";
		resources.ApplyResources(this.listView, "listView");
		this.listView.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listView).Columns.AddRange(new System.Windows.Forms.ColumnHeader[6] { this.columnHeaderImageSpacing, this.timeStampColumnHeader, this.vinColumnHeader, this.vinDecodeColumnHeader, this.sizeColumnHeader, this.nameColumnHeader });
		this.listView.EditableColumn = -1;
		this.listView.GridLines = true;
		this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
		((System.Windows.Forms.Control)(object)this.listView).Name = "listView";
		this.listView.ShowGlyphs = (GlyphBehavior)2;
		((System.Windows.Forms.ListView)(object)this.listView).ShowGroups = false;
		this.listView.ShowItemImages = (ImageBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listView).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.ListView)(object)this.listView).ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(listView_ColumnClick);
		((System.Windows.Forms.ListView)(object)this.listView).SelectedIndexChanged += new System.EventHandler(listView_SelectedIndexChanged);
		((System.Windows.Forms.Control)(object)this.listView).DoubleClick += new System.EventHandler(listView_DoubleClick);
		resources.ApplyResources(this.columnHeaderImageSpacing, "columnHeaderImageSpacing");
		resources.ApplyResources(this.timeStampColumnHeader, "timeStampColumnHeader");
		resources.ApplyResources(this.vinColumnHeader, "vinColumnHeader");
		resources.ApplyResources(this.vinDecodeColumnHeader, "vinDecodeColumnHeader");
		resources.ApplyResources(this.sizeColumnHeader, "sizeColumnHeader");
		resources.ApplyResources(this.nameColumnHeader, "nameColumnHeader");
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.buttonUpload);
		base.Controls.Add(this.progressBar);
		base.Controls.Add(this.textBoxSearch);
		base.Controls.Add(this.labelSearch);
		base.Controls.Add(this.iconPanel);
		base.Controls.Add(this.comboBoxLookIn);
		base.Controls.Add(this.labelLookIn);
		base.Controls.Add(this.buttonBrowse);
		base.Controls.Add(this.buttonCopyLink);
		base.Controls.Add(this.buttonInfo);
		base.Controls.Add(this.cancelButton);
		base.Controls.Add(this.okButton);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.listView);
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "OpenLogFileForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		this.iconPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.listView).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
