// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ListParameterCompare
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class ListParameterCompare : UserControl, ISearchable, ISupportEdit, IProvideHtml
{
  private static int columnAutoWidth = -1;
  [Category("Behavior")]
  [Description("Show differences only")]
  [DefaultValue(true)]
  private bool showDifferencesOnly = true;
  [Category("Behavior")]
  [Description("Filters out items that only exist in a connected data source.")]
  [DefaultValue(false)]
  private bool showOfflineItemsOnly;
  private CompareSource[] sources;
  private Channel[] relatedOnlineChannels;
  private Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();
  private Color[] colors = new Color[10]
  {
    Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 192 /*0xC0*/),
    Color.FromArgb((int) byte.MaxValue, 192 /*0xC0*/, (int) byte.MaxValue),
    Color.FromArgb((int) byte.MaxValue, 192 /*0xC0*/, 192 /*0xC0*/),
    Color.FromArgb(192 /*0xC0*/, (int) byte.MaxValue, (int) byte.MaxValue),
    Color.FromArgb(192 /*0xC0*/, (int) byte.MaxValue, 192 /*0xC0*/),
    Color.FromArgb(192 /*0xC0*/, 192 /*0xC0*/, (int) byte.MaxValue),
    Color.FromArgb(192 /*0xC0*/, 192 /*0xC0*/, 192 /*0xC0*/),
    Color.FromArgb(192 /*0xC0*/, 192 /*0xC0*/, 160 /*0xA0*/),
    Color.FromArgb(192 /*0xC0*/, 160 /*0xA0*/, 192 /*0xC0*/),
    Color.FromArgb(192 /*0xC0*/, 160 /*0xA0*/, 160 /*0xA0*/)
  };
  private bool updateColumnHeaders;
  private bool dirty;
  private EditSupportHelper editSupport = new EditSupportHelper();
  private IContainer components;
  private ListViewEx listView;
  private ColumnHeader columnParameter;
  private ColumnHeader columnUnits;
  private ImageList stateImages;

  public bool ShowDifferencesOnly
  {
    get => this.showDifferencesOnly;
    set
    {
      if (value == this.showDifferencesOnly)
        return;
      this.showDifferencesOnly = value;
      this.BuildList();
    }
  }

  public bool ShowOfflineItemsOnly
  {
    get => this.showOfflineItemsOnly;
    set
    {
      if (value == this.showOfflineItemsOnly)
        return;
      this.showOfflineItemsOnly = value;
      this.BuildList();
    }
  }

  public CompareSource Source(int index)
  {
    return index < this.SourceCount ? this.sources[index] : (CompareSource) null;
  }

  public int SourceCount
  {
    get => this.sources == null ? 0 : this.sources.Length;
    set
    {
      if (value > this.colors.Length)
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, "Exceeded maximum sources allowed");
      if (this.sources != null && value == this.sources.Length)
        return;
      ((ListView) this.listView).Items.Clear();
      ((ListView) this.listView).Columns.Clear();
      if (this.sources != null)
      {
        foreach (CompareSource source in this.sources)
        {
          source.SourceChanged -= new EventHandler(this.CompareSource_SourceChanged);
          source.SourceContentChanged -= new EventHandler(this.CompareSource_SourceContentChanged);
          source.Clear();
        }
      }
      this.sources = new CompareSource[value];
      this.relatedOnlineChannels = new Channel[value];
      ((ListView) this.listView).Columns.Add(this.columnParameter);
      for (int index = 0; index < value; ++index)
      {
        ((ListView) this.listView).Columns.Add(Resources.ListParameterComparePartHeader, 0);
        this.sources[index] = new CompareSource();
        this.sources[index].SourceChanged += new EventHandler(this.CompareSource_SourceChanged);
        this.sources[index].SourceContentChanged += new EventHandler(this.CompareSource_SourceContentChanged);
        this.sources[index].Color = this.colors[index];
        ((ListView) this.listView).Columns.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ColumnHeaderFormatSourceN, (object) (index + 1)), 100);
      }
      ((ListView) this.listView).Columns.Add(this.columnUnits);
    }
  }

  private void CompareSource_SourceContentChanged(object sender, EventArgs e) => this.BuildList();

  private void CompareSource_SourceChanged(object sender, EventArgs e)
  {
    for (int index = 0; index < this.sources.Length; ++index)
    {
      if (this.sources[index] == sender)
      {
        this.UpdateRelatedOnlineChannel(this.sources[index], ref this.relatedOnlineChannels[index]);
        break;
      }
    }
    this.updateColumnHeaders = true;
    this.BuildList();
  }

  private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateParameterEditState(sender as Parameter, false);
  }

  private void ListView_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.OnSelectedParameterChanged();
  }

  private void GroupItem_ExpandCollapseItem(object sender, ListViewExGroupItemExpandEventArgs e)
  {
    ListViewExGroupItem listViewExGroupItem = sender as ListViewExGroupItem;
    this.expandedStates[((ListViewItem) listViewExGroupItem).Name] = !listViewExGroupItem.Expanded;
  }

  private void SapiManager_ActiveChannelsListChanged(object sender, EventArgs e)
  {
    for (int index = 0; index < this.sources.Length; ++index)
      this.UpdateRelatedOnlineChannel(this.sources[index], ref this.relatedOnlineChannels[index]);
  }

  public ListParameterCompare()
  {
    this.InitializeComponent();
    this.stateImages.Images.Add(ListParameterCompare.ImageIndex.Edited.ToString(), (Image) Resources.edited);
    this.stateImages.Images.Add(ListParameterCompare.ImageIndex.Group.ToString(), (Image) Resources.group_open);
    this.stateImages.Images.Add(ListParameterCompare.ImageIndex.GroupEdited.ToString(), (Image) Resources.group_edit);
    this.SourceCount = 2;
    Converter.GlobalInstance.UnitsSelectionChanged += new EventHandler(this.GlobalInstance_UnitsSelectionChanged);
    SapiManager.GlobalInstance.ActiveChannelsListChanged += new EventHandler(this.SapiManager_ActiveChannelsListChanged);
    SapiManager.GlobalInstance.AccessLevelFilterChanged += new EventHandler(this.SapiManager_AccessLevelFilterChanged);
    ((ListView) this.listView).SelectedIndexChanged += new EventHandler(this.ListView_SelectedIndexChanged);
    this.expandedStates.Add(Resources.GroupNameIdentification, true);
  }

  private void SapiManager_AccessLevelFilterChanged(object sender, EventArgs e) => this.BuildList();

  private bool UpdateRelatedOnlineChannel(CompareSource source, ref Channel targetOnlineChannel)
  {
    Channel channel = source.Loaded ? SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (ac => ac.Ecu == source.Parameters.Channel.Ecu)) : (Channel) null;
    if (channel == targetOnlineChannel)
      return false;
    if (targetOnlineChannel != null)
    {
      targetOnlineChannel.Parameters.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
      foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) targetOnlineChannel.Parameters)
        this.UpdateParameterEditState(parameter, true);
    }
    targetOnlineChannel = channel;
    if (targetOnlineChannel != null)
    {
      targetOnlineChannel.Parameters.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
      foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) targetOnlineChannel.Parameters)
        this.UpdateParameterEditState(parameter, false);
    }
    return true;
  }

  private void UpdateParameterEditState(Parameter parameter, bool reset)
  {
    if (parameter == null || !parameter.Channel.Parameters.HaveBeenReadFromEcu)
      return;
    ListViewExGroupItem listViewExGroupItem1 = (ListViewExGroupItem) ((ListView) this.listView).Items[parameter.GroupName];
    if (listViewExGroupItem1 == null)
      return;
    ListViewExGroupItem listViewExGroupItem2 = listViewExGroupItem1.Children.OfType<ListViewExGroupItem>().FirstOrDefault<ListViewExGroupItem>((Func<ListViewExGroupItem, bool>) (i => ((ListViewItem) i).Name == parameter.Name));
    if (listViewExGroupItem2 == null)
      return;
    ((ListViewItem) listViewExGroupItem2).StateImageIndex = reset || object.Equals(parameter.Value, parameter.OriginalValue) ? -1 : 0;
    ((ListViewItem) listViewExGroupItem1).StateImageIndex = reset || !(parameter.GroupCodingString != parameter.OriginalGroupCodingString) ? 1 : 2;
  }

  private void OnSelectedParameterChanged()
  {
    if (this.SelectedParameterChanged == null)
      return;
    this.SelectedParameterChanged((object) this, new EventArgs());
  }

  public event EventHandler SelectedParameterChanged;

  public string SelectedParameter
  {
    get
    {
      List<string> values = new List<string>();
      if (((ListView) this.listView).SelectedItems.Count > 0)
      {
        ListViewExGroupItem selectedItem = ((ListView) this.listView).SelectedItems[0] as ListViewExGroupItem;
        if (selectedItem.Parent != null)
          values.Add(((ListViewItem) selectedItem.Parent).Name);
        values.Add(((ListViewItem) selectedItem).Name);
      }
      return string.Join("/", (IEnumerable<string>) values);
    }
  }

  private void GlobalInstance_UnitsSelectionChanged(object sender, EventArgs e) => this.BuildList();

  private void BuildList()
  {
    this.dirty = true;
    this.Invalidate();
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    if (this.dirty)
    {
      this.dirty = false;
      Cursor.Current = Cursors.WaitCursor;
      this.InternalBuildList();
      Cursor.Current = Cursors.Default;
      this.updateColumnHeaders = false;
    }
    base.OnPaint(e);
  }

  private static bool ShowParameter(Parameter parameter)
  {
    if (!parameter.Visible)
      return false;
    if (!SapiManager.GlobalInstance.AccessLevelFilter.HasValue)
      return true;
    int readAccess = parameter.ReadAccess;
    int? accessLevelFilter = SapiManager.GlobalInstance.AccessLevelFilter;
    int valueOrDefault = accessLevelFilter.GetValueOrDefault();
    return readAccess <= valueOrDefault && accessLevelFilter.HasValue;
  }

  private void InternalBuildList()
  {
    bool flag1 = false;
    int index1 = ((ListView) this.listView).TopItem != null ? ((ListView) this.listView).TopItem.Index : -1;
    string selectedParameter = this.SelectedParameter;
    ListViewExGroupItem listViewExGroupItem1 = (ListViewExGroupItem) null;
    this.listView.LockSorting();
    this.listView.BeginUpdate();
    ((ListView) this.listView).Items.Clear();
    IEnumerable<EdexConfigurationInformation> configurationInformations = ((IEnumerable<CompareSource>) this.sources).Where<CompareSource>((Func<CompareSource, bool>) (s => s.EdexConfigurationInformation != null)).Select<CompareSource, EdexConfigurationInformation>((Func<CompareSource, EdexConfigurationInformation>) (s => s.EdexConfigurationInformation));
    GroupCollection[] groupCollections = new GroupCollection[this.SourceCount];
    for (int index2 = 0; index2 < this.SourceCount; ++index2)
    {
      groupCollections[index2] = new GroupCollection();
      if (this.sources[index2].Loaded)
      {
        if (this.updateColumnHeaders)
          ((ListView) this.listView).Columns[index2 * 2 + 2].Text = string.Empty;
        flag1 = true;
        foreach (DictionaryEntry identification in this.sources[index2].IdentificationList)
        {
          if (identification.Value != null)
            groupCollections[index2].Add(Resources.GroupNameIdentification, (object) new ListParameterCompare.GroupItem(identification, true));
        }
        if (this.sources[index2].Parameters.Channel.ConnectionResource == null || this.sources[index2].Parameters.HaveBeenReadFromEcu)
        {
          EdexConfigurationInformation factoryConfiguration = this.sources[index2].EdexConfigurationInformation?.DeviceInformation?.EdexFactoryConfiguration;
          Channel channel = this.sources[index2].Parameters.Channel;
          foreach (IGrouping<string, Parameter> source in this.sources[index2].Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.Value != null && ListParameterCompare.ShowParameter(p))).GroupBy<Parameter, string>((Func<Parameter, string>) (p => p.GroupQualifier)))
          {
            CodingInfoForCoding codingInfoForCoding = (CodingInfoForCoding) null;
            CodingChoice defaultStringChoice = (CodingChoice) null;
            string groupCodingString = channel.Parameters.GroupCodingStrings[source.Key];
            CodingParameterGroup codingParameterGroup = channel.CodingParameterGroups[source.Key];
            if (codingParameterGroup != null)
            {
              codingInfoForCoding = ServerDataManager.GetCodingChoicesForParameterGroup(codingParameterGroup, groupCodingString, this.sources[index2].EdexConfigurationInformation != null ? Enumerable.Repeat<EdexConfigurationInformation>(this.sources[index2].EdexConfigurationInformation, 1) : configurationInformations, factoryConfiguration?.GetAggregateSetting(codingParameterGroup)?.PartNumber, factoryConfiguration?.GetEngineeringCorrectionFactors(channel.ParameterGroups[source.Key]));
              defaultStringChoice = codingInfoForCoding != null ? codingInfoForCoding.CodingChoices.FirstOrDefault<CodingChoice>((Func<CodingChoice, bool>) (cc => cc.Parameter == null)) : (CodingChoice) null;
              groupCollections[index2].Add(source.First<Parameter>().GroupName, (object) new ListParameterCompare.GroupItem(codingParameterGroup, defaultStringChoice));
            }
            foreach (Parameter parameter1 in (IEnumerable<Parameter>) source)
            {
              Parameter parameter = parameter1;
              CodingChoice fragmentChoice = (CodingChoice) null;
              bool isEngineeringCorrectionFactor = false;
              if (codingInfoForCoding != null)
              {
                fragmentChoice = codingInfoForCoding.CodingChoices.FirstOrDefault<CodingChoice>((Func<CodingChoice, bool>) (cc => cc.Parameter != null && cc.Parameter.RelatedParameter == parameter));
                if (fragmentChoice == null && defaultStringChoice != null)
                {
                  if (parameter.IsValueEqualInCodingStrings(new Dump(groupCodingString), codingInfoForCoding.Coding))
                    fragmentChoice = defaultStringChoice;
                  else if (codingInfoForCoding.EngineeringCorrectionFactors.Contains<string>(parameter.Qualifier, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
                  {
                    fragmentChoice = defaultStringChoice;
                    isEngineeringCorrectionFactor = true;
                  }
                }
              }
              groupCollections[index2].Add(parameter.GroupName, (object) new ListParameterCompare.GroupItem(parameter, fragmentChoice, isEngineeringCorrectionFactor));
            }
          }
        }
        foreach (DictionaryEntry unknown in this.sources[index2].UnknownList)
        {
          if (unknown.Value != null)
            groupCollections[index2].Add(Resources.GroupNameInvalidData, (object) new ListParameterCompare.GroupItem(unknown, false));
        }
      }
      else if (this.updateColumnHeaders)
        ((ListView) this.listView).Columns[index2 * 2 + 2].Text = Resources.ValueNoSource;
    }
    if (flag1)
    {
      for (int index3 = 0; index3 < groupCollections.Length; ++index3)
      {
        foreach (Group group in groupCollections[index3])
        {
          bool flag2 = false;
          ListViewExGroupItem listViewExGroupItem2 = (ListViewExGroupItem) ((ListView) this.listView).Items[group.Name];
          if (listViewExGroupItem2 == null)
          {
            flag2 = true;
            listViewExGroupItem2 = new ListViewExGroupItem(group.Name);
            ((ListViewItem) listViewExGroupItem2).Name = group.Name;
            ((ListViewItem) listViewExGroupItem2).StateImageIndex = 1;
            ((ListViewItem) listViewExGroupItem2).UseItemStyleForSubItems = false;
            if (((ListViewItem) listViewExGroupItem2).Name == selectedParameter)
              listViewExGroupItem1 = listViewExGroupItem2;
          }
          while (group.Items.Count > 0)
          {
            ListParameterCompare.GroupItem sourceParameter = group.Items[0] as ListParameterCompare.GroupItem;
            if (sourceParameter.IsDefaultString)
            {
              this.PopulateItem(groupCollections, index3, group.Name, listViewExGroupItem2, sourceParameter);
            }
            else
            {
              ListViewExGroupItem listViewExGroupItem3 = new ListViewExGroupItem(sourceParameter.Name);
              ((ListViewItem) listViewExGroupItem3).Name = sourceParameter.Name;
              ((ListViewItem) listViewExGroupItem3).UseItemStyleForSubItems = false;
              Parameter parameter = this.relatedOnlineChannels[index3] != null ? this.relatedOnlineChannels[index3].Parameters[sourceParameter.Qualifier] : (Parameter) null;
              if (parameter != null && !object.Equals(parameter.Value, parameter.OriginalValue))
              {
                ((ListViewItem) listViewExGroupItem3).StateImageIndex = 0;
                ((ListViewItem) listViewExGroupItem2).StateImageIndex = 2;
              }
              if (this.PopulateItem(groupCollections, index3, group.Name, listViewExGroupItem3, sourceParameter))
              {
                listViewExGroupItem2.Add(listViewExGroupItem3);
                if (string.Join("/", group.Name, sourceParameter.Name) == selectedParameter)
                  listViewExGroupItem1 = listViewExGroupItem3;
              }
            }
          }
          if (flag2 && listViewExGroupItem2.Children.Count > 0)
          {
            ((ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem2);
            if (this.expandedStates.ContainsKey(((ListViewItem) listViewExGroupItem2).Name) && this.expandedStates[((ListViewItem) listViewExGroupItem2).Name])
              listViewExGroupItem2.Expand();
            listViewExGroupItem2.ExpandCollapseItem += new EventHandler<ListViewExGroupItemExpandEventArgs>(this.GroupItem_ExpandCollapseItem);
          }
        }
      }
      if (this.updateColumnHeaders)
      {
        this.columnParameter.Width = ListParameterCompare.columnAutoWidth;
        int num = (((Control) this.listView).ClientSize.Width - this.columnParameter.Width - this.columnUnits.Width - SystemInformation.VerticalScrollBarWidth) / (this.SourceCount + ((IEnumerable<CompareSource>) this.sources).Count<CompareSource>((Func<CompareSource, bool>) (s => s.Parameters != null && SapiExtensions.IsDataSourceEdex(s.Parameters.Channel.Ecu))));
        for (int index4 = 0; index4 < this.SourceCount; ++index4)
        {
          ((ListView) this.listView).Columns[index4 * 2 + 1].Width = this.sources[index4].Parameters == null || !SapiExtensions.IsDataSourceEdex(this.sources[index4].Parameters.Channel.Ecu) ? 0 : num;
          ((ListView) this.listView).Columns[index4 * 2 + 2].Width = num;
        }
      }
    }
    try
    {
      if (listViewExGroupItem1 != null)
        ((ListViewItem) listViewExGroupItem1).Selected = true;
      if (index1 != -1)
      {
        if (((ListView) this.listView).Items.Count > index1)
          ((ListView) this.listView).TopItem = ((ListView) this.listView).Items[index1];
      }
    }
    catch (NullReferenceException ex)
    {
    }
    this.listView.EndUpdate();
    this.listView.UnlockSorting();
    this.OnSelectedParameterChanged();
  }

  private static ListParameterCompare.GroupItem FindItem(Group group, string qualifier)
  {
    foreach (ListParameterCompare.GroupItem groupItem in group.Items)
    {
      if (string.Equals(groupItem.Qualifier, qualifier, StringComparison.OrdinalIgnoreCase))
        return groupItem;
    }
    return (ListParameterCompare.GroupItem) null;
  }

  private bool PopulateItem(
    GroupCollection[] groupCollections,
    int index,
    string groupName,
    ListViewExGroupItem item,
    ListParameterCompare.GroupItem sourceParameter)
  {
    bool flag1 = index != 0 && this.sources[0].Loaded;
    bool flag2 = true;
    if (sourceParameter.ReadOnly)
      ((ListViewItem) item).ForeColor = SystemColors.GrayText;
    Conversion conversion = Converter.GlobalInstance.GetConversion(sourceParameter.Units);
    ListParameterCompare.GroupItem[] groupItemArray = new ListParameterCompare.GroupItem[this.SourceCount];
    for (int index1 = 0; index1 < this.SourceCount; ++index1)
    {
      GroupCollection groupCollection = groupCollections[index1];
      Group group = ((IEnumerable<Group>) groupCollection).FirstOrDefault<Group>((Func<Group, bool>) (x => string.Equals(x.Name, groupName, StringComparison.OrdinalIgnoreCase))) ?? groupCollection[groupName];
      ListParameterCompare.GroupItem groupItem = ListParameterCompare.FindItem(group, sourceParameter.Qualifier);
      if (groupItem != null)
      {
        groupItemArray[index1] = groupItem;
        group.Items.Remove((object) groupItem);
        ListViewItem.ListViewSubItem listViewSubItem1 = ((ListViewItem) item).SubItems.Add(groupItem.IsPartNumberInherited ? (groupItem.IsEngineeringCorrectionFactor ? Resources.Message_ParameterValueFromParentEngineeringCorrectionFactor : Resources.Message_ParameterValueFromParent) : groupItem.PartNumber);
        ListViewItem.ListViewSubItem listViewSubItem2 = ((ListViewItem) item).SubItems.Add(conversion != null ? Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, groupItem.Value, conversion, groupItem.Precision) : groupItem.Value.ToString());
        if (groupItem.ReadOnly)
          listViewSubItem1.ForeColor = listViewSubItem2.ForeColor = SystemColors.GrayText;
        if (index1 != index)
          flag2 = false;
      }
      else
      {
        ((ListViewItem) item).SubItems.Add(string.Empty);
        ((ListViewItem) item).SubItems.Add(string.Empty);
      }
    }
    ListViewItem.ListViewSubItem listViewSubItem = ((ListViewItem) item).SubItems.Add(Converter.Translate(conversion != null ? conversion.OutputUnit : sourceParameter.Units));
    if (sourceParameter.ReadOnly)
      listViewSubItem.ForeColor = SystemColors.GrayText;
    for (int index2 = index; index2 < this.SourceCount && !flag1; ++index2)
    {
      ListParameterCompare.GroupItem groupItem1 = groupItemArray[index2];
      if (groupItem1 != null)
      {
        for (int index3 = index2 + 1; index3 < this.SourceCount && !flag1; ++index3)
        {
          ListParameterCompare.GroupItem groupItem2 = groupItemArray[index3];
          if (groupItem2 != null && !object.Equals(groupItem1.Value, groupItem2.Value))
            flag1 = true;
        }
      }
      else if (this.sources[index2].Loaded)
        flag1 = true;
    }
    if (flag1 | flag2)
    {
      for (int index4 = index; index4 < this.SourceCount; ++index4)
      {
        ListParameterCompare.GroupItem groupItem3 = groupItemArray[index4];
        if (groupItem3 != null)
        {
          ListViewItem.ListViewSubItem subItem1 = ((ListViewItem) item).SubItems[2 * index4 + 2];
          subItem1.BackColor = this.sources[index4].Color;
          subItem1.Tag = (object) ListParameterCompare.ChangedStatus.Changed;
          ((ListViewItem) item).SubItems[2 * index4 + 1].BackColor = this.sources[index4].Color;
          if (groupItem3.ModifyHeader && this.updateColumnHeaders)
          {
            ColumnHeader column = ((ListView) this.listView).Columns[index4 * 2 + 2];
            column.Text = $"{column.Text}{groupItem3.Value} ";
          }
          for (int index5 = index4 + 1; index5 < this.SourceCount; ++index5)
          {
            ListParameterCompare.GroupItem groupItem4 = groupItemArray[index5];
            if (groupItem4 != null && object.Equals(groupItem3.Value, groupItem4.Value))
            {
              ListViewItem.ListViewSubItem subItem2 = ((ListViewItem) item).SubItems[index5 * 2 + 2];
              subItem2.BackColor = this.sources[index4].Color;
              subItem2.Tag = (object) ListParameterCompare.ChangedStatus.Changed;
              ((ListViewItem) item).SubItems[2 * index5 + 1].BackColor = this.sources[index4].Color;
              if (groupItem3.ModifyHeader && this.updateColumnHeaders)
              {
                ColumnHeader column = ((ListView) this.listView).Columns[index5 * 2 + 2];
                column.Text = $"{column.Text}{groupItem3.Value} ";
              }
              groupItemArray[index5] = (ListParameterCompare.GroupItem) null;
            }
          }
        }
      }
    }
    if (flag1 | flag2 || !this.showDifferencesOnly || sourceParameter.AlwaysShow)
    {
      if (!flag2)
        return true;
      ListViewItem.ListViewSubItem subItem = ((ListViewItem) item).SubItems[0];
      subItem.BackColor = this.sources[index].Color;
      subItem.Tag = (object) ListParameterCompare.ChangedStatus.Changed;
      if (this.sources[index].Parameters.Channel.ConnectionResource == null || !this.showOfflineItemsOnly)
        return true;
    }
    return false;
  }

  public bool CanSearch => ((ISearchable) this.listView).CanSearch;

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    return ((ISearchable) this.listView).Search(searchText, caseSensitive, direction);
  }

  public bool CanUndo => false;

  public void Undo()
  {
  }

  public bool CanCopy
  {
    get
    {
      this.editSupport.SetTarget((object) this.ActiveControl);
      return this.editSupport.CanCopy;
    }
  }

  public void Copy()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.Copy();
  }

  public bool CanDelete => false;

  public void Delete()
  {
  }

  public bool CanPaste => false;

  public void Paste()
  {
  }

  public bool CanCut => false;

  public void Cut()
  {
  }

  public bool CanSelectAll
  {
    get
    {
      this.editSupport.SetTarget((object) this.ActiveControl);
      return this.editSupport.CanSelectAll;
    }
  }

  public void SelectAll()
  {
    this.editSupport.SetTarget((object) this.ActiveControl);
    this.editSupport.SelectAll();
  }

  public bool CanProvideHtml => this.SourceCount > 0 && this.sources[0].Loaded;

  public string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.CanProvideHtml)
    {
      XmlWriter writer = PrintHelper.CreateWriter(stringBuilder);
      writer.WriteStartElement("table");
      foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.listView).Items)
      {
        writer.WriteStartElement("tr");
        writer.WriteAttributeString("class", listViewExGroupItem.HasChildren ? "group" : "standard");
        for (int index = 0; index < ((ListViewItem) listViewExGroupItem).SubItems.Count; ++index)
        {
          writer.WriteStartElement("td");
          writer.WriteAttributeString("bgcolor", ColorTranslator.ToHtml(((ListViewItem) listViewExGroupItem).SubItems[index].BackColor));
          if (!this.showDifferencesOnly && ((ListViewItem) listViewExGroupItem).SubItems[index].Tag != null && (ListParameterCompare.ChangedStatus) ((ListViewItem) listViewExGroupItem).SubItems[index].Tag == ListParameterCompare.ChangedStatus.Changed)
            writer.WriteString("[*]");
          writer.WriteString(((ListViewItem) listViewExGroupItem).SubItems[index].Text);
          writer.WriteFullEndElement();
          if (index == 0)
          {
            writer.WriteStartElement("td");
            writer.WriteRaw("&nbsp;");
            writer.WriteFullEndElement();
          }
        }
        writer.WriteFullEndElement();
      }
      writer.WriteFullEndElement();
      writer.Close();
    }
    return stringBuilder.ToString();
  }

  public string StyleSheet => string.Empty;

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ListParameterCompare));
    this.listView = new ListViewEx();
    this.columnParameter = new ColumnHeader();
    this.columnUnits = new ColumnHeader();
    this.stateImages = new ImageList(this.components);
    this.SuspendLayout();
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[2]
    {
      this.columnParameter,
      this.columnUnits
    });
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    ((Control) this.listView).Name = "listView";
    this.listView.ShowGlyphs = (GlyphBehavior) 2;
    this.listView.ShowStateImages = (ImageBehavior) 2;
    ((ListView) this.listView).StateImageList = this.stateImages;
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnParameter, "columnParameter");
    componentResourceManager.ApplyResources((object) this.columnUnits, "columnUnits");
    this.stateImages.ColorDepth = ColorDepth.Depth32Bit;
    componentResourceManager.ApplyResources((object) this.stateImages, "stateImages");
    this.stateImages.TransparentColor = Color.Transparent;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.listView);
    this.Name = nameof (ListParameterCompare);
    this.ResumeLayout(false);
  }

  internal enum ImageIndex
  {
    NoImage = -1, // 0xFFFFFFFF
    Edited = 0,
    Group = 1,
    GroupEdited = 2,
    Normal = 3,
  }

  private class GroupItem
  {
    private Parameter parameter;
    private DictionaryEntry entry;
    private CodingChoice codingChoice;
    private bool alwaysShow;
    private bool modifyHeader;
    private CodingParameterGroup codingParameterGroup;
    private bool isEngineeringCorrectionFactor;

    public GroupItem(CodingParameterGroup codingParameterGroup, CodingChoice defaultStringChoice)
    {
      this.codingParameterGroup = codingParameterGroup;
      this.codingChoice = defaultStringChoice;
      this.alwaysShow = true;
    }

    public GroupItem(
      Parameter parameter,
      CodingChoice fragmentChoice,
      bool isEngineeringCorrectionFactor)
    {
      this.parameter = parameter;
      this.codingChoice = fragmentChoice;
      this.isEngineeringCorrectionFactor = isEngineeringCorrectionFactor;
    }

    public GroupItem(DictionaryEntry entry, bool isIdentification)
    {
      this.entry = entry;
      this.alwaysShow = this.modifyHeader = isIdentification;
    }

    public string Qualifier
    {
      get
      {
        if (this.parameter != null)
          return this.parameter.Qualifier;
        if (this.codingChoice != null)
          return this.codingChoice.ParameterGroup.Qualifier;
        return this.codingParameterGroup == null ? this.entry.Key.ToString() : this.codingParameterGroup.Qualifier;
      }
    }

    public string Name
    {
      get
      {
        if (this.parameter != null)
          return this.parameter.Name;
        return this.codingChoice == null ? this.entry.Key.ToString() : this.codingChoice.ParameterGroup.Name;
      }
    }

    public object Value
    {
      get
      {
        if (this.parameter != null)
          return this.parameter.Value;
        if (this.codingChoice != null)
          return (object) this.codingChoice.Meaning;
        return this.codingParameterGroup == null ? this.entry.Value : (object) string.Empty;
      }
    }

    public object Precision => this.parameter == null ? (object) -1 : this.parameter.Precision;

    public string Units => this.parameter == null ? string.Empty : this.parameter.Units;

    public bool AlwaysShow => this.alwaysShow;

    public bool ModifyHeader => this.modifyHeader;

    public bool ReadOnly => this.parameter == null || this.parameter.ReadOnly;

    public string PartNumber
    {
      get
      {
        if (this.codingChoice != null)
          return this.codingChoice.Part.ToString();
        return this.parameter == null && this.codingParameterGroup == null ? string.Empty : "n/a";
      }
    }

    public bool IsDefaultString => this.codingParameterGroup != null;

    public bool IsPartNumberInherited
    {
      get
      {
        return this.codingChoice != null && !this.IsDefaultString && this.codingChoice.Parameter == null;
      }
    }

    public bool IsEngineeringCorrectionFactor => this.isEngineeringCorrectionFactor;
  }

  private enum ChangedStatus
  {
    NotChanged,
    Changed,
  }
}
