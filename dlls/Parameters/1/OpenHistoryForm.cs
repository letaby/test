// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.OpenHistoryForm
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

#nullable disable
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

  private static StringDictionary LoadParameterFile(string path)
  {
    StringDictionary parameters = new StringDictionary();
    using (MemoryStream memoryStream = new MemoryStream(FileEncryptionProvider.ReadEncryptedFile(path, true)))
    {
      using (StreamReader stream = new StreamReader((Stream) memoryStream))
        ParameterCollection.LoadDictionaryFromStream(stream, ParameterFileFormat.VerFile, parameters);
    }
    return parameters;
  }

  private static bool ParameterFilesAreEqual(string sourceFile, string destinationFile)
  {
    bool flag = false;
    if (!OpenHistoryForm.cachedParameterFilesCompareResults.TryGetValue(new Tuple<string, string>(sourceFile, destinationFile), out flag))
    {
      flag = OpenHistoryForm.CompareParameterFiles(sourceFile, destinationFile);
      OpenHistoryForm.cachedParameterFilesCompareResults.Add(new Tuple<string, string>(sourceFile, destinationFile), flag);
    }
    return flag;
  }

  private static bool CompareParameterFiles(string sourceFile, string destinationFile)
  {
    StringDictionary stringDictionary1 = OpenHistoryForm.LoadParameterFile(sourceFile);
    StringDictionary stringDictionary2 = OpenHistoryForm.LoadParameterFile(destinationFile);
    if (stringDictionary1.Count != stringDictionary2.Count)
      return true;
    foreach (DictionaryEntry dictionaryEntry in stringDictionary1)
    {
      if (!stringDictionary2.ContainsKey(dictionaryEntry.Key as string) || !object.Equals(dictionaryEntry.Value, (object) stringDictionary2[dictionaryEntry.Key as string]))
        return true;
    }
    foreach (DictionaryEntry dictionaryEntry in stringDictionary2)
    {
      if (!stringDictionary1.ContainsKey(dictionaryEntry.Key as string))
        return true;
    }
    return false;
  }

  public OpenHistoryForm()
    : this((string) null)
  {
  }

  public OpenHistoryForm(string ecuName)
  {
    this.ecuName = ecuName;
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.okButton.Enabled = false;
    this.copyLinkButton.Enabled = false;
    bool flag = false;
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      if (channel.Online)
      {
        flag = true;
        break;
      }
    }
    this.checkBoxShowCurrentVehicleOnly.Enabled = this.checkBoxShowCurrentVehicleOnly.Checked = flag;
    this.checkBoxShowUnique.Checked = false;
  }

  public string FileName
  {
    get
    {
      string fileName = string.Empty;
      if (((ListView) this.listView).SelectedItems.Count > 0)
        fileName = ((ListView) this.listView).SelectedItems[0].Tag as string;
      return fileName;
    }
  }

  public string EntryName
  {
    get
    {
      string entryName = string.Empty;
      if (((ListView) this.listView).SelectedItems.Count > 0)
      {
        ListViewItem selectedItem = ((ListView) this.listView).SelectedItems[0];
        entryName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.OpenHistoryForm_EntryNameFormat, (object) selectedItem.SubItems[0].Text, (object) selectedItem.SubItems[3].Text, (object) selectedItem.SubItems[1].Text, (object) selectedItem.SubItems[2].Text);
      }
      return entryName;
    }
  }

  public StringDictionary Identification
  {
    get
    {
      this.identification.Clear();
      if (((ListView) this.listView).SelectedItems.Count > 0)
      {
        ListViewItem selectedItem = ((ListView) this.listView).SelectedItems[0];
        this.identification.Add(Resources.ColumnHeaderTime, selectedItem.SubItems[0].Text);
        this.identification.Add(Resources.ColumnHeaderEquipmentSerialNumber, selectedItem.SubItems[1].Text);
        this.identification.Add(Resources.ColumnHeaderVIN, selectedItem.SubItems[2].Text);
        this.identification.Add(Resources.ColumnHeaderDevice, selectedItem.SubItems[3].Text);
        this.identification.Add(Resources.ColumnHeaderReason, selectedItem.SubItems[4].Text);
      }
      return this.identification;
    }
  }

  protected override void OnLoad(EventArgs e)
  {
    if (((ListView) this.listView).Items.Count == 0)
      this.BuildList();
    base.OnLoad(e);
  }

  private bool MatchesCurrentVehicle(string esn, string vin)
  {
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      string str1 = (string) null;
      if (!this.cachedEngineSerialNumbers.TryGetValue(channel, out str1))
      {
        str1 = SapiManager.GetEngineSerialNumber(channel);
        this.cachedEngineSerialNumbers.Add(channel, str1);
      }
      if (str1 != null && string.Equals(str1.Trim(), esn.Trim(), StringComparison.OrdinalIgnoreCase))
        return true;
      string str2 = (string) null;
      if (!this.cachedVehicleIdentificationNumbers.TryGetValue(channel, out str2))
      {
        str2 = SapiManager.GetVehicleIdentificationNumber(channel);
        this.cachedVehicleIdentificationNumbers.Add(channel, str2);
      }
      if (str2 != null && string.Equals(str2.Trim(), vin.Trim(), StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private void BuildList()
  {
    Cursor.Current = Cursors.WaitCursor;
    bool flag1 = this.checkBoxShowCurrentVehicleOnly.Checked;
    bool flag2 = this.checkBoxShowUnique.Checked;
    this.listView.LockSorting();
    this.listView.BeginUpdate();
    ((ListView) this.listView).Items.Clear();
    SortedList<HistoryFileKey, ListViewExGroupItem> sortedList = new SortedList<HistoryFileKey, ListViewExGroupItem>((IComparer<HistoryFileKey>) new HistoryFileKeyComparer());
    foreach (string file in Directory.GetFiles(Directories.DrumrollHistoryData))
    {
      FileNameInformation fileNameInformation = FileNameInformation.FromName(FileEncryptionProvider.DecryptFileName(Path.GetFileName(file)), (FileNameInformation.FileType) 2);
      if (fileNameInformation.Valid && (!flag1 || this.MatchesCurrentVehicle(fileNameInformation.EngineSerialNumber, fileNameInformation.VehicleIdentity)) && (string.IsNullOrEmpty(this.ecuName) || string.Equals(fileNameInformation.Device, this.ecuName, StringComparison.OrdinalIgnoreCase)))
      {
        DateTime dateTime = Utility.TimeFromString(fileNameInformation.Timestamp);
        ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(dateTime.ToString((IFormatProvider) CultureInfo.CurrentCulture));
        ((ListViewItem) listViewExGroupItem).SubItems.Add(fileNameInformation.EngineSerialNumber);
        ((ListViewItem) listViewExGroupItem).SubItems.Add(fileNameInformation.VehicleIdentity);
        ((ListViewItem) listViewExGroupItem).SubItems.Add(fileNameInformation.Device);
        ((ListViewItem) listViewExGroupItem).SubItems.Add(fileNameInformation.Reason);
        ((ListViewItem) listViewExGroupItem).Tag = (object) file;
        HistoryFileKey key = new HistoryFileKey(dateTime, fileNameInformation.Reason);
        sortedList.Add(key, listViewExGroupItem);
      }
    }
    if (!flag2)
    {
      foreach (ListViewItem listViewItem in (IEnumerable<ListViewExGroupItem>) sortedList.Values)
        ((ListView) this.listView).Items.Insert(0, listViewItem);
    }
    else
    {
      for (int index1 = sortedList.Values.Count - 1; index1 >= 0; --index1)
      {
        ListViewExGroupItem listViewExGroupItem1 = sortedList.Values[index1];
        bool flag3 = false;
        for (int index2 = index1 - 1; index2 >= 0 && !flag3; --index2)
        {
          ListViewExGroupItem listViewExGroupItem2 = sortedList.Values[index2];
          if (string.Equals(((ListViewItem) listViewExGroupItem2).SubItems[this.deviceColumnHeader.Index].Text, ((ListViewItem) listViewExGroupItem1).SubItems[this.deviceColumnHeader.Index].Text, StringComparison.OrdinalIgnoreCase) && (string.Equals(((ListViewItem) listViewExGroupItem2).SubItems[this.esnColumnHeader.Index].Text, ((ListViewItem) listViewExGroupItem1).SubItems[this.esnColumnHeader.Index].Text, StringComparison.OrdinalIgnoreCase) || string.Equals(((ListViewItem) listViewExGroupItem2).SubItems[this.vinColumnHeader.Index].Text, ((ListViewItem) listViewExGroupItem1).SubItems[this.vinColumnHeader.Index].Text, StringComparison.OrdinalIgnoreCase)))
          {
            flag3 = true;
            if (OpenHistoryForm.ParameterFilesAreEqual(((ListViewItem) listViewExGroupItem1).Tag.ToString(), ((ListViewItem) listViewExGroupItem2).Tag.ToString()))
              ((ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem1);
          }
        }
        if (!flag3)
          ((ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem1);
      }
    }
    this.listView.UnlockSorting();
    this.listView.EndUpdate();
    Cursor.Current = Cursors.Default;
  }

  private void listView_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.okButton.Enabled = this.copyLinkButton.Enabled = ((ListView) this.listView).SelectedIndices.Count > 0;
  }

  private void listView_DoubleClick(object sender, EventArgs e)
  {
    if (((ListView) this.listView).SelectedItems.Count <= 0)
      return;
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void checkBoxShowCurrentVehicleOnly_CheckedChanged(object sender, EventArgs e)
  {
    this.BuildList();
  }

  private void checkBoxShowUnique_CheckedChanged(object sender, EventArgs e) => this.BuildList();

  private void buttonCopyLink_Click(object sender, EventArgs e)
  {
    this.copyLinkButton.Enabled = false;
    FileManagement.CopyLink((Control) this, this.FileName);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (OpenHistoryForm));
    this.cancelButton = new Button();
    this.okButton = new Button();
    this.checkBoxShowCurrentVehicleOnly = new CheckBox();
    this.listView = new ListViewEx();
    this.timeStampColumnHeader = new ColumnHeader();
    this.esnColumnHeader = new ColumnHeader();
    this.vinColumnHeader = new ColumnHeader();
    this.deviceColumnHeader = new ColumnHeader();
    this.reasonColumnHeader = new ColumnHeader();
    this.checkBoxShowUnique = new CheckBox();
    this.copyLinkButton = new Button();
    this.SuspendLayout();
    this.cancelButton.DialogResult = DialogResult.Cancel;
    componentResourceManager.ApplyResources((object) this.cancelButton, "cancelButton");
    this.cancelButton.Name = "cancelButton";
    this.cancelButton.UseVisualStyleBackColor = true;
    this.okButton.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.okButton, "okButton");
    this.okButton.Name = "okButton";
    this.okButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.checkBoxShowCurrentVehicleOnly, "checkBoxShowCurrentVehicleOnly");
    this.checkBoxShowCurrentVehicleOnly.Name = "checkBoxShowCurrentVehicleOnly";
    this.checkBoxShowCurrentVehicleOnly.UseVisualStyleBackColor = true;
    this.checkBoxShowCurrentVehicleOnly.CheckedChanged += new EventHandler(this.checkBoxShowCurrentVehicleOnly_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[5]
    {
      this.timeStampColumnHeader,
      this.esnColumnHeader,
      this.vinColumnHeader,
      this.deviceColumnHeader,
      this.reasonColumnHeader
    });
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    ((Control) this.listView).Name = "listView";
    ((ListView) this.listView).ShowGroups = false;
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    ((ListView) this.listView).SelectedIndexChanged += new EventHandler(this.listView_SelectedIndexChanged);
    ((Control) this.listView).DoubleClick += new EventHandler(this.listView_DoubleClick);
    componentResourceManager.ApplyResources((object) this.timeStampColumnHeader, "timeStampColumnHeader");
    componentResourceManager.ApplyResources((object) this.esnColumnHeader, "esnColumnHeader");
    componentResourceManager.ApplyResources((object) this.vinColumnHeader, "vinColumnHeader");
    componentResourceManager.ApplyResources((object) this.deviceColumnHeader, "deviceColumnHeader");
    componentResourceManager.ApplyResources((object) this.reasonColumnHeader, "reasonColumnHeader");
    componentResourceManager.ApplyResources((object) this.checkBoxShowUnique, "checkBoxShowUnique");
    this.checkBoxShowUnique.Name = "checkBoxShowUnique";
    this.checkBoxShowUnique.UseVisualStyleBackColor = true;
    this.checkBoxShowUnique.CheckedChanged += new EventHandler(this.checkBoxShowUnique_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.copyLinkButton, "copyLinkButton");
    this.copyLinkButton.Name = "copyLinkButton";
    this.copyLinkButton.UseVisualStyleBackColor = true;
    this.copyLinkButton.Click += new EventHandler(this.buttonCopyLink_Click);
    this.AcceptButton = (IButtonControl) this.okButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.cancelButton;
    this.Controls.Add((Control) this.copyLinkButton);
    this.Controls.Add((Control) this.checkBoxShowUnique);
    this.Controls.Add((Control) this.checkBoxShowCurrentVehicleOnly);
    this.Controls.Add((Control) this.cancelButton);
    this.Controls.Add((Control) this.okButton);
    this.Controls.Add((Control) this.listView);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (OpenHistoryForm);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
