using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class OpenHistoryForm : Form
{
	private string ecuName = string.Empty;

	private Dictionary<Channel, string> cachedEngineSerialNumbers = new Dictionary<Channel, string>();

	private Dictionary<Channel, string> cachedVehicleIdentificationNumbers = new Dictionary<Channel, string>();

	private static Dictionary<Tuple<string, string>, bool> cachedParameterFilesCompareResults = new Dictionary<Tuple<string, string>, bool>();

	private StringDictionary identification = new StringDictionary();

	private IContainer components;

	private ListViewEx listView;

	private ColumnHeader vinColumnHeader;

	private Button cancelButton;

	private Button okButton;

	private ColumnHeader esnColumnHeader;

	private ColumnHeader deviceColumnHeader;

	private ColumnHeader timeStampColumnHeader;

	private ColumnHeader reasonColumnHeader;

	private CheckBox checkBoxShowCurrentVehicleOnly;

	private CheckBox checkBoxShowUnique;

	private Button copyLinkButton;

	public string FileName
	{
		get
		{
			string result = string.Empty;
			if (((ListView)(object)listView).SelectedItems.Count > 0)
			{
				result = ((ListView)(object)listView).SelectedItems[0].Tag as string;
			}
			return result;
		}
	}

	public string EntryName
	{
		get
		{
			string result = string.Empty;
			if (((ListView)(object)listView).SelectedItems.Count > 0)
			{
				ListViewItem listViewItem = ((ListView)(object)listView).SelectedItems[0];
				result = string.Format(CultureInfo.CurrentCulture, Resources.OpenHistoryForm_EntryNameFormat, listViewItem.SubItems[0].Text, listViewItem.SubItems[3].Text, listViewItem.SubItems[1].Text, listViewItem.SubItems[2].Text);
			}
			return result;
		}
	}

	public StringDictionary Identification
	{
		get
		{
			identification.Clear();
			if (((ListView)(object)listView).SelectedItems.Count > 0)
			{
				ListViewItem listViewItem = ((ListView)(object)listView).SelectedItems[0];
				identification.Add(Resources.ColumnHeaderTime, listViewItem.SubItems[0].Text);
				identification.Add(Resources.ColumnHeaderEquipmentSerialNumber, listViewItem.SubItems[1].Text);
				identification.Add(Resources.ColumnHeaderVIN, listViewItem.SubItems[2].Text);
				identification.Add(Resources.ColumnHeaderDevice, listViewItem.SubItems[3].Text);
				identification.Add(Resources.ColumnHeaderReason, listViewItem.SubItems[4].Text);
			}
			return identification;
		}
	}

	private static StringDictionary LoadParameterFile(string path)
	{
		StringDictionary stringDictionary = new StringDictionary();
		byte[] buffer = FileEncryptionProvider.ReadEncryptedFile(path, true);
		using MemoryStream stream = new MemoryStream(buffer);
		using StreamReader stream2 = new StreamReader(stream);
		ParameterCollection.LoadDictionaryFromStream(stream2, ParameterFileFormat.VerFile, stringDictionary);
		return stringDictionary;
	}

	private static bool ParameterFilesAreEqual(string sourceFile, string destinationFile)
	{
		bool value = false;
		if (!cachedParameterFilesCompareResults.TryGetValue(new Tuple<string, string>(sourceFile, destinationFile), out value))
		{
			value = CompareParameterFiles(sourceFile, destinationFile);
			cachedParameterFilesCompareResults.Add(new Tuple<string, string>(sourceFile, destinationFile), value);
		}
		return value;
	}

	private static bool CompareParameterFiles(string sourceFile, string destinationFile)
	{
		StringDictionary stringDictionary = LoadParameterFile(sourceFile);
		StringDictionary stringDictionary2 = LoadParameterFile(destinationFile);
		if (stringDictionary.Count != stringDictionary2.Count)
		{
			return true;
		}
		foreach (DictionaryEntry item in stringDictionary)
		{
			if (stringDictionary2.ContainsKey(item.Key as string))
			{
				if (!object.Equals(item.Value, stringDictionary2[item.Key as string]))
				{
					return true;
				}
				continue;
			}
			return true;
		}
		foreach (DictionaryEntry item2 in stringDictionary2)
		{
			if (!stringDictionary.ContainsKey(item2.Key as string))
			{
				return true;
			}
		}
		return false;
	}

	public OpenHistoryForm()
		: this(null)
	{
	}

	public OpenHistoryForm(string ecuName)
	{
		this.ecuName = ecuName;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		okButton.Enabled = false;
		copyLinkButton.Enabled = false;
		bool flag = false;
		foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
		{
			if (channel.Online)
			{
				flag = true;
				break;
			}
		}
		checkBoxShowCurrentVehicleOnly.Enabled = (checkBoxShowCurrentVehicleOnly.Checked = flag);
		checkBoxShowUnique.Checked = false;
	}

	protected override void OnLoad(EventArgs e)
	{
		if (((ListView)(object)listView).Items.Count == 0)
		{
			BuildList();
		}
		base.OnLoad(e);
	}

	private bool MatchesCurrentVehicle(string esn, string vin)
	{
		foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
		{
			string value = null;
			if (!cachedEngineSerialNumbers.TryGetValue(channel, out value))
			{
				value = SapiManager.GetEngineSerialNumber(channel);
				cachedEngineSerialNumbers.Add(channel, value);
			}
			if (value != null && string.Equals(value.Trim(), esn.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			string value2 = null;
			if (!cachedVehicleIdentificationNumbers.TryGetValue(channel, out value2))
			{
				value2 = SapiManager.GetVehicleIdentificationNumber(channel);
				cachedVehicleIdentificationNumbers.Add(channel, value2);
			}
			if (value2 != null && string.Equals(value2.Trim(), vin.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private void BuildList()
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		Cursor.Current = Cursors.WaitCursor;
		bool flag = checkBoxShowCurrentVehicleOnly.Checked;
		bool flag2 = checkBoxShowUnique.Checked;
		listView.LockSorting();
		listView.BeginUpdate();
		((ListView)(object)listView).Items.Clear();
		SortedList<HistoryFileKey, ListViewExGroupItem> sortedList = new SortedList<HistoryFileKey, ListViewExGroupItem>(new HistoryFileKeyComparer());
		string[] files = Directory.GetFiles(Directories.DrumrollHistoryData);
		string[] array = files;
		foreach (string text in array)
		{
			string text2 = FileEncryptionProvider.DecryptFileName(Path.GetFileName(text));
			FileNameInformation val = FileNameInformation.FromName(text2, (FileType)2);
			if (val.Valid && (!flag || MatchesCurrentVehicle(val.EngineSerialNumber, val.VehicleIdentity)) && (string.IsNullOrEmpty(ecuName) || string.Equals(val.Device, ecuName, StringComparison.OrdinalIgnoreCase)))
			{
				DateTime dateTime = Utility.TimeFromString(val.Timestamp);
				ListViewExGroupItem val2 = new ListViewExGroupItem(dateTime.ToString(CultureInfo.CurrentCulture));
				((ListViewItem)(object)val2).SubItems.Add(val.EngineSerialNumber);
				((ListViewItem)(object)val2).SubItems.Add(val.VehicleIdentity);
				((ListViewItem)(object)val2).SubItems.Add(val.Device);
				((ListViewItem)(object)val2).SubItems.Add(val.Reason);
				((ListViewItem)(object)val2).Tag = text;
				HistoryFileKey key = new HistoryFileKey(dateTime, val.Reason);
				sortedList.Add(key, val2);
			}
		}
		if (!flag2)
		{
			foreach (ListViewExGroupItem value in sortedList.Values)
			{
				((ListView)(object)listView).Items.Insert(0, (ListViewItem)(object)value);
			}
		}
		else
		{
			for (int num = sortedList.Values.Count - 1; num >= 0; num--)
			{
				ListViewExGroupItem val3 = sortedList.Values[num];
				bool flag3 = false;
				int num2 = num - 1;
				while (num2 >= 0 && !flag3)
				{
					ListViewExGroupItem val4 = sortedList.Values[num2];
					if (string.Equals(((ListViewItem)(object)val4).SubItems[deviceColumnHeader.Index].Text, ((ListViewItem)(object)val3).SubItems[deviceColumnHeader.Index].Text, StringComparison.OrdinalIgnoreCase) && (string.Equals(((ListViewItem)(object)val4).SubItems[esnColumnHeader.Index].Text, ((ListViewItem)(object)val3).SubItems[esnColumnHeader.Index].Text, StringComparison.OrdinalIgnoreCase) || string.Equals(((ListViewItem)(object)val4).SubItems[vinColumnHeader.Index].Text, ((ListViewItem)(object)val3).SubItems[vinColumnHeader.Index].Text, StringComparison.OrdinalIgnoreCase)))
					{
						flag3 = true;
						if (ParameterFilesAreEqual(((ListViewItem)(object)val3).Tag.ToString(), ((ListViewItem)(object)val4).Tag.ToString()))
						{
							((ListView)(object)listView).Items.Add((ListViewItem)(object)val3);
						}
					}
					num2--;
				}
				if (!flag3)
				{
					((ListView)(object)listView).Items.Add((ListViewItem)(object)val3);
				}
			}
		}
		listView.UnlockSorting();
		listView.EndUpdate();
		Cursor.Current = Cursors.Default;
	}

	private void listView_SelectedIndexChanged(object sender, EventArgs e)
	{
		Button button = okButton;
		bool enabled = (copyLinkButton.Enabled = ((ListView)(object)listView).SelectedIndices.Count > 0);
		button.Enabled = enabled;
	}

	private void listView_DoubleClick(object sender, EventArgs e)
	{
		if (((ListView)(object)listView).SelectedItems.Count > 0)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void checkBoxShowCurrentVehicleOnly_CheckedChanged(object sender, EventArgs e)
	{
		BuildList();
	}

	private void checkBoxShowUnique_CheckedChanged(object sender, EventArgs e)
	{
		BuildList();
	}

	private void buttonCopyLink_Click(object sender, EventArgs e)
	{
		copyLinkButton.Enabled = false;
		FileManagement.CopyLink((Control)this, FileName);
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.OpenHistoryForm));
		this.cancelButton = new System.Windows.Forms.Button();
		this.okButton = new System.Windows.Forms.Button();
		this.checkBoxShowCurrentVehicleOnly = new System.Windows.Forms.CheckBox();
		this.listView = new ListViewEx();
		this.timeStampColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.esnColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.vinColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.deviceColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.reasonColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.checkBoxShowUnique = new System.Windows.Forms.CheckBox();
		this.copyLinkButton = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.UseVisualStyleBackColor = true;
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		resources.ApplyResources(this.okButton, "okButton");
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.checkBoxShowCurrentVehicleOnly, "checkBoxShowCurrentVehicleOnly");
		this.checkBoxShowCurrentVehicleOnly.Name = "checkBoxShowCurrentVehicleOnly";
		this.checkBoxShowCurrentVehicleOnly.UseVisualStyleBackColor = true;
		this.checkBoxShowCurrentVehicleOnly.CheckedChanged += new System.EventHandler(checkBoxShowCurrentVehicleOnly_CheckedChanged);
		resources.ApplyResources(this.listView, "listView");
		((System.Windows.Forms.ListView)(object)this.listView).Columns.AddRange(new System.Windows.Forms.ColumnHeader[5] { this.timeStampColumnHeader, this.esnColumnHeader, this.vinColumnHeader, this.deviceColumnHeader, this.reasonColumnHeader });
		this.listView.EditableColumn = -1;
		this.listView.GridLines = true;
		((System.Windows.Forms.Control)(object)this.listView).Name = "listView";
		((System.Windows.Forms.ListView)(object)this.listView).ShowGroups = false;
		((System.Windows.Forms.ListView)(object)this.listView).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.ListView)(object)this.listView).SelectedIndexChanged += new System.EventHandler(listView_SelectedIndexChanged);
		((System.Windows.Forms.Control)(object)this.listView).DoubleClick += new System.EventHandler(listView_DoubleClick);
		resources.ApplyResources(this.timeStampColumnHeader, "timeStampColumnHeader");
		resources.ApplyResources(this.esnColumnHeader, "esnColumnHeader");
		resources.ApplyResources(this.vinColumnHeader, "vinColumnHeader");
		resources.ApplyResources(this.deviceColumnHeader, "deviceColumnHeader");
		resources.ApplyResources(this.reasonColumnHeader, "reasonColumnHeader");
		resources.ApplyResources(this.checkBoxShowUnique, "checkBoxShowUnique");
		this.checkBoxShowUnique.Name = "checkBoxShowUnique";
		this.checkBoxShowUnique.UseVisualStyleBackColor = true;
		this.checkBoxShowUnique.CheckedChanged += new System.EventHandler(checkBoxShowUnique_CheckedChanged);
		resources.ApplyResources(this.copyLinkButton, "copyLinkButton");
		this.copyLinkButton.Name = "copyLinkButton";
		this.copyLinkButton.UseVisualStyleBackColor = true;
		this.copyLinkButton.Click += new System.EventHandler(buttonCopyLink_Click);
		base.AcceptButton = this.okButton;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.cancelButton;
		base.Controls.Add(this.copyLinkButton);
		base.Controls.Add(this.checkBoxShowUnique);
		base.Controls.Add(this.checkBoxShowCurrentVehicleOnly);
		base.Controls.Add(this.cancelButton);
		base.Controls.Add(this.okButton);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.listView);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "OpenHistoryForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
