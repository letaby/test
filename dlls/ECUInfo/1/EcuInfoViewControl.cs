// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo.EcuInfoViewControl
// Assembly: EcuInfo, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 02DA79A8-6904-4617-BF9C-5C5A1F77D676
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ECUInfo.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Reflection;
using DetroitDiesel.Settings;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo;

public class EcuInfoViewControl : 
  ContextHelpControl,
  ISupportEdit,
  IProvideHtml,
  ISearchable,
  IRefreshable,
  ISupportExpandCollapseAll
{
  private EditSupportHelper editSupport = new EditSupportHelper();
  private SearchableProxy searchSupport = new SearchableProxy();
  private HtmlElement inputPane;
  private HtmlElement tabDivElement;
  private Dictionary<string, HtmlElement> elements = new Dictionary<string, HtmlElement>();
  private Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();
  private ChannelBaseCollection activeChannels;
  private string groupName;
  private bool includeConnectionResource;
  private bool useStoredDataStyle;
  private const int MaxValueLength = 512 /*0x0200*/;
  private bool dirty;
  private IContainer components;
  private WebBrowser webBrowser;
  private ContextMenuStrip contextMenuStrip;
  private ToolStripMenuItem manipulationToolStripMenuItem;

  [Category("Behavior")]
  [Localizable(false)]
  [Description("Indicates the group for which this control will show data. The group is assigned within the .EcuInfo file.")]
  [DefaultValue(null)]
  public string GroupName
  {
    get => this.groupName;
    set
    {
      if (!(value != this.groupName))
        return;
      this.groupName = value;
      this.BuildList();
    }
  }

  [Category("Appearance")]
  [Localizable(false)]
  [Description("Indicates if the connection resource strings should be shown in the tab.")]
  [DefaultValue(false)]
  public bool IncludeConnectionResource
  {
    get => this.includeConnectionResource;
    set
    {
      if (value == this.includeConnectionResource)
        return;
      this.includeConnectionResource = value;
    }
  }

  [Category("Appearance")]
  [Localizable(false)]
  [Description("Indicates if the 'Stored Data' view style should be used.")]
  [DefaultValue(false)]
  public bool UseStoredDataStyle
  {
    get => this.useStoredDataStyle;
    set
    {
      if (value == this.useStoredDataStyle)
        return;
      this.useStoredDataStyle = value;
    }
  }

  public EcuInfoViewControl()
  {
    ((Control) this).Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    SapiManager.GlobalInstance.ActiveChannelsChanged += new EventHandler(this.GlobalInstance_ActiveChannelsChanged);
    Converter.GlobalInstance.UnitsSelectionChanged += new EventHandler(this.OnUnitsSelectionChanged);
    this.SetLink(LinkSupport.GetViewLink((PanelIdentifier) 0));
    this.editSupport.SetTarget((object) this.webBrowser);
    this.searchSupport.SetTarget((object) this.webBrowser);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<style type=\"text/css\">td {padding-right: 10px}");
    stringBuilder.Append(".gradientline0 { height:1px; font-size:1pt; background-color:red }");
    stringBuilder.Append(".gradientline1 { height:1px; font-size:1pt; background-color:green }");
    stringBuilder.Append(".gradientline2 { height:1px; font-size:1pt; background-color:blue }");
    string str1 = ((Control) this).Font.SizeInPoints.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ".ecu {{ padding-top: 10px; width: 100%; font-family:{0}; font-size:{1}pt; font-weight:bold }}", (object) ((Control) this).Font.FontFamily.Name, (object) ((double) ((Control) this).Font.SizeInPoints * 1.25).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ".group {{ padding-top: 10px; font-family:{0}; font-size:{1}pt; font-weight:bold }}", (object) ((Control) this).Font.FontFamily.Name, (object) str1);
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ".standard {{ font-family:{0}; font-size:{1}pt }}", (object) ((Control) this).Font.FontFamily.Name, (object) str1);
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ".resource {{ display:block; font-family:{0}; font-size:{1}pt; font-style: italic; white-space: nowrap; color: Gray; }}", (object) ((Control) this).Font.FontFamily.Name, (object) ((double) ((Control) this).Font.SizeInPoints * 0.75).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ".variant {{ display:block; font-family:{0}; font-size:{1}pt; white-space: nowrap; text-align: right; }}", (object) ((Control) this).Font.FontFamily.Name, (object) ((double) ((Control) this).Font.SizeInPoints * 0.75).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ".identifier {{ margin:0px 0px 0px 0px; padding:3px 6px 3px 6px; color: white; background-color: darkgray; font-family:{0}; font-size:{1}pt; white-space: nowrap;}}", (object) ((Control) this).Font.FontFamily.Name, (object) str1);
    stringBuilder.AppendLine(".identifiertdpadding { padding:0px 5px 0px 0px; }");
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "div.content_expanded {{display: block; padding-left: 0.2in; margin-top: 0px; margin-bottom: 0px;}}");
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "div.content_collapsed {{display: none;}}");
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "a.link_collapse{{ text-decoration: none;}}");
    stringBuilder.AppendLine($"\r\n                table.border, table.border tr, table.border td, table.border th {{ border: 1px solid gray; border-collapse: collapse;  padding: 2px 6px 2px 6px; }}\r\n                table.border th {{ background: #f4f4f4; text-align: left; }}\r\n                th.request {{ white-space: nowrap; }}\r\n                table.border, table.border tr, table.border td {{ background: #f1fdf1;  }}\r\n                table.border td.name {{ background: #f1f1fd;  }}\r\n\r\n                .loader {{\r\n                    border: 2px solid #E0E0E0; /* Light grey */\r\n                    border-top: 2px solid #3498db; /* Blue */\r\n                    border-radius: 50%;\r\n                    width: {str1}pt;\r\n                    height: {str1}pt;\r\n                    animation: spin 1.25s linear infinite;\r\n                }}\r\n                @keyframes spin\r\n                {{\r\n                    0% {{ transform: rotate(0deg); }}\r\n                    100% {{ transform: rotate(360deg); }}\r\n                }}\r\n\r\n                button {{\r\n                  background-color: #008CBA; \r\n                  border: none;\r\n                  color: white;\r\n                  padding: 2px 4px;\r\n                  text-align: center;\r\n                  text-decoration: none;\r\n                  display: inline-block;\r\n                  cursor: pointer;\r\n                  margin: 0px 10px;\r\n                }}\r\n\r\n            ");
    stringBuilder.Append("</style>");
    string str2 = "<script type='text/javascript'>\r\n                                    function ExpandCollapse(link) { \r\n                                        var parentEcuItemDiv = findParentElementByClass(link, 'ecuitem');\r\n                                        var contentDiv = getElementByClass(parentEcuItemDiv, 'collapsable_content');\r\n                                        if ( contentDiv.className.indexOf('content_expanded') != -1 ) {                                        \r\n                                            _changeContentDiv(link, contentDiv, 'Collapse');\r\n                                        }\r\n                                        else\r\n                                        {\r\n                                            _changeContentDiv(link, contentDiv, 'Expand');\r\n                                        }\r\n                                    }\r\n                                    // traverse up the DOM tree and returns the first element with a matching class if one is found. If a match is not found, it returns null. \r\n                                    function findParentElementByClass(element, parentClass){\r\n                                        var parentEle = null;\r\n                                        var parentElement;\r\n\r\n                                        parentElement = (parentElement == null) ? element.parentElement : parentElement.parentElement;\r\n                                        if ( parentElement.className.indexOf(parentClass) > -1 )\r\n                                        {\r\n                                            parentEle = parentElement;\r\n                                        } else {\r\n                                            parentEle = findParentElementByClass(parentElement, parentClass);\r\n                                        }\r\n                                        return parentEle;\r\n                                    }\r\n                                    // traverse down the DOM tree and returns the first element with a matching class if one if found. If a match is not found, it returns null. \r\n                                    function getElementByClass(element, elementClass) {\r\n                                        var ele = null;\r\n                                        if ( element != null )\r\n                                        {\r\n                                            if ( element.className.indexOf(elementClass) > -1 )\r\n                                            {\r\n                                                ele = element;\r\n                                            } else {\r\n                                                if ( element.children.length != 0 )\r\n                                                {\r\n                                                    for( var x = 0; x < element.children.length; x++ )\r\n                                                    {\r\n                                                        ele = getElementByClass(element.children.item(x), elementClass);\r\n                                                        if ( ele != null )\r\n                                                        {\r\n                                                            break;\r\n                                                        }\r\n                                                    }\r\n                                                }\r\n                                            }\r\n                                        }\r\n                                        return ele;\r\n                                    }\r\n                                    // Swaps the classes on the content div and text in anchor based on the desiredState. \r\n                                    function _changeContentDiv(anchor, contentDiv, desiredState){\r\n                                        var oldClass;\r\n                                        var newClass;\r\n                                        var innerHtmlText;\r\n                                        switch(desiredState)\r\n                                        {\r\n                                            case 'Expand':\r\n                                                oldClass = 'content_collapsed';\r\n                                                newClass = 'content_expanded';\r\n                                                innerHtmlText = '&ndash;';\r\n                                                break;\r\n                                            case 'Collapse':\r\n                                                oldClass = 'content_expanded';\r\n                                                newClass = 'content_collapsed';\r\n                                                innerHtmlText = '+';\r\n                                                break;\r\n                                        }\r\n                                        contentDiv.className = contentDiv.className.replace(oldClass, newClass);\r\n                                        anchor.innerHTML = innerHtmlText;\r\n                                    }\r\n\r\n\t\t                            //When a button is clicked, navigate to the supplied anchor ID. \r\n\t\t                            function clickButton(id)\r\n\t\t                            {\r\n\t\t\t                            window.location.href = '#button_'+id;\r\n\t\t                            }\r\n\r\n                              </script>";
    this.webBrowser.DocumentText = $"<HTML><HEAD><meta http-equiv='X-UA-Compatible' content='IE=edge'>{stringBuilder.ToString()}\n{str2}\n</HEAD><BODY><DIV id=\"inputpane\" name=\"inputpane\" class=\"standard\"><br></DIV></BODY></HTML>";
  }

  private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
  {
    if (e.KeyCode != Keys.F5)
      return;
    e.IsInputKey = true;
    if (!this.CanRefreshView)
      return;
    this.RefreshView();
  }

  private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
  {
    this.inputPane = this.webBrowser.Document.All.GetElementsByName("inputpane")[0];
    this.webBrowser.Document.ContextMenuShowing += new HtmlElementEventHandler(this.Document_ContextMenuShowing);
    this.BuildList();
  }

  private void Document_ContextMenuShowing(object sender, HtmlElementEventArgs e)
  {
    HtmlElement elementFromPoint = this.webBrowser.Document.GetElementFromPoint(e.MousePosition);
    if (!(elementFromPoint != (HtmlElement) null) || elementFromPoint.Id == null)
      return;
    string[] strArray = elementFromPoint.Id.Split(".".ToCharArray(), 2);
    if (strArray.Length != 2)
      return;
    DataItem dataItem = DataItem.Create(new Qualifier((QualifierTypes) 8, strArray[0], strArray[1]), (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    if (dataItem == null)
      return;
    this.contextMenuStrip.Show(((Control) this).PointToScreen(e.MousePosition));
    this.manipulationToolStripMenuItem.Visible = ManipulationForm.CanManipulate(dataItem);
    this.manipulationToolStripMenuItem.Tag = (object) dataItem;
  }

  private void manipulationToolStripMenuItem_Click(object sender, EventArgs e)
  {
    ManipulationForm.Show(this.manipulationToolStripMenuItem.Tag as DataItem);
  }

  private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
  {
    if (!(e.Url.ToString() != "about:blank") && string.IsNullOrEmpty(e.Url.Fragment))
      return;
    e.Cancel = true;
    if (!e.Url.Fragment.StartsWith("#button_", StringComparison.OrdinalIgnoreCase))
      return;
    string[] id = e.Url.Fragment.Substring(8).Split(".".ToCharArray());
    SapiLayer1.EcuInfo ecuInfo = SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((Func<Channel, bool>) (c => c.Ecu.Name == id[0])).SelectMany<Channel, SapiLayer1.EcuInfo>((Func<Channel, IEnumerable<SapiLayer1.EcuInfo>>) (c => c.EcuInfos.Where<SapiLayer1.EcuInfo>((Func<SapiLayer1.EcuInfo, bool>) (ei => ei.Qualifier == id[1])))).FirstOrDefault<SapiLayer1.EcuInfo>();
    if (ecuInfo == null)
      return;
    int num = (int) CustomMessageBox.Show((IWin32Window) this, SapiExtensions.GetValueString(ecuInfo, ecuInfo.EcuInfoValues.Current), ecuInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, (CustomMessageBoxOptions) 1);
  }

  private void DeferredBuildList()
  {
    Cursor.Current = Cursors.WaitCursor;
    if (!((Control) this).IsDisposed && this.inputPane != (HtmlElement) null)
    {
      Dictionary<Channel, GroupCollection> channels = this.PreBuildData();
      this.ReadExpansionSettingsForThisTab(this.groupName, channels);
      StringBuilder stringBuilder = new StringBuilder();
      using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
        this.BuildTabXml(writer, this.groupName, channels);
      this.inputPane.InnerHtml = stringBuilder.ToString();
      this.elements = this.inputPane.GetElementsByTagName("div").OfType<HtmlElement>().Where<HtmlElement>((Func<HtmlElement, bool>) (e => !string.IsNullOrEmpty(e.Name))).ToDictionary<HtmlElement, string, HtmlElement>((Func<HtmlElement, string>) (k => k.Name), (Func<HtmlElement, HtmlElement>) (v => v));
      this.tabDivElement = this.inputPane.Children.GetElementsByName("TAB").OfType<HtmlElement>().FirstOrDefault<HtmlElement>();
      foreach (HtmlElement htmlElement in this.tabDivElement.GetElementsByTagName("div").OfType<HtmlElement>().Where<HtmlElement>((Func<HtmlElement, bool>) (el => el.GetAttribute("className") == "ecuitem")))
        htmlElement.Click += new HtmlElementEventHandler(this.ecuElement_Click);
    }
    Cursor.Current = Cursors.Default;
  }

  private void ecuElement_Click(object sender, HtmlElementEventArgs e)
  {
    HtmlElement elementFromPoint = this.webBrowser.Document.GetElementFromPoint(e.MousePosition);
    HtmlElement htmlElement = sender as HtmlElement;
    if (!elementFromPoint.GetAttribute("className").Equals("link_collapse", StringComparison.Ordinal))
      return;
    string attribute = htmlElement.GetAttribute("id");
    string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) htmlElement.Parent.GetAttribute("id"), (object) attribute);
    bool flag = htmlElement.Children[htmlElement.Children.Count - 1].GetAttribute("className") == "collapsable_content content_expanded";
    if (this.expandedStates.Keys.Contains<string>(key) && this.expandedStates[key] == flag)
      return;
    this.expandedStates[key] = flag;
    SettingsManager.GlobalInstance.SetValue<bool>("EcuInfoNodeState." + key, "EcuInfo", flag, false);
  }

  private void ReadExpansionSettingsForThisTab(
    string tabName,
    Dictionary<Channel, GroupCollection> channels)
  {
    foreach (KeyValuePair<Channel, GroupCollection> channel in channels)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) "EcuInfoNodeState", (object) tabName, (object) channel.Key.Ecu.Name);
      this.expandedStates[string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) tabName, (object) channel.Key.Ecu.Name)] = SettingsManager.GlobalInstance.GetValue<bool>(str, "EcuInfo", true);
    }
  }

  private Dictionary<Channel, GroupCollection> PreBuildData()
  {
    Dictionary<Channel, GroupCollection> dictionary = new Dictionary<Channel, GroupCollection>();
    if (this.activeChannels != null)
    {
      foreach (Channel key in (IEnumerable<Channel>) this.activeChannels.Where<Channel>((Func<Channel, bool>) (channel => channel.ConnectionResource != null && channel.CommunicationsState != CommunicationsState.Offline || channel.LogFile != null)).OrderBy<Channel, int>((Func<Channel, int>) (c => c.Ecu.Priority)))
      {
        foreach (SapiLayer1.EcuInfo ecuInfo in (ReadOnlyCollection<SapiLayer1.EcuInfo>) key.EcuInfos)
        {
          if (ecuInfo.Visible && !string.Equals(ecuInfo.Qualifier, "DiagnosisVariant"))
          {
            string str1 = ecuInfo.OriginalGroupName?.Split("/".ToArray<char>())[0];
            string str2 = ecuInfo.GroupName;
            int num = ecuInfo.GroupName.IndexOf('/');
            if (num != -1)
              str2 = ecuInfo.GroupName.Substring(num + 1);
            if (str1 == this.groupName)
            {
              GroupCollection groupCollection;
              if (!dictionary.ContainsKey(key))
              {
                groupCollection = new GroupCollection();
                dictionary.Add(key, groupCollection);
              }
              else
                groupCollection = dictionary[key];
              groupCollection.Add(str2, (object) ecuInfo);
            }
          }
        }
      }
    }
    return dictionary;
  }

  private static void BuildItemXml(XmlWriter xmlWriter, SapiLayer1.EcuInfo groupObject)
  {
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("standard");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteString(groupObject.Name);
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("standard");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteRaw("&nbsp;");
    xmlWriter.WriteFullEndElement();
    EcuInfoViewControl.BuildItemValueXml(xmlWriter, groupObject);
    xmlWriter.WriteFullEndElement();
  }

  private static void BuildItemValueXml(XmlWriter xmlWriter, SapiLayer1.EcuInfo groupObject)
  {
    string text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) groupObject.Channel.Ecu.Name, (object) groupObject.Qualifier);
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartElement("div");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString(groupObject.EcuInfoValues.Current != null ? "standard" : "loader");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("id");
    xmlWriter.WriteString(text);
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("name");
    xmlWriter.WriteString(text);
    xmlWriter.WriteEndAttribute();
    string valueString = SapiExtensions.GetValueString(groupObject, groupObject.EcuInfoValues.Current);
    if (valueString.Length < 512 /*0x0200*/)
    {
      xmlWriter.WriteString(valueString);
    }
    else
    {
      xmlWriter.WriteStartElement("span");
      xmlWriter.WriteString(valueString.Substring(0, 512 /*0x0200*/));
      xmlWriter.WriteFullEndElement();
      xmlWriter.WriteStartElement("button");
      xmlWriter.WriteAttributeString("onclick", FormattableString.Invariant(FormattableStringFactory.Create("clickButton('{0}')", (object) text)));
      xmlWriter.WriteString(Resources.EcuInfoViewControl_ShowMore);
      xmlWriter.WriteFullEndElement();
    }
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
  }

  private static int GetBitPosition(SapiLayer1.EcuInfo item)
  {
    int bitPosition = 0;
    ServiceOutputValue serviceOutputValue;
    if (item == null)
    {
      serviceOutputValue = (ServiceOutputValue) null;
    }
    else
    {
      Service service = item.Services.FirstOrDefault<Service>();
      serviceOutputValue = (object) service != null ? service.OutputValues.FirstOrDefault<ServiceOutputValue>() : (ServiceOutputValue) null;
    }
    Presentation presentation = (Presentation) serviceOutputValue;
    if (presentation != null)
      bitPosition = Convert.ToInt32((object) presentation.BytePosition, (IFormatProvider) CultureInfo.InvariantCulture) * 8 + Convert.ToInt32((object) presentation.BitPosition, (IFormatProvider) CultureInfo.InvariantCulture);
    return bitPosition;
  }

  private static bool ColumnTextMatchesAtIndex(string[] columns, int index, SapiLayer1.EcuInfo compareEcuInfo)
  {
    string[] nameColumns = EcuInfoViewControl.GetNameColumns(compareEcuInfo);
    return index < columns.Length && index < nameColumns.Length && columns[index] == nameColumns[index];
  }

  private static string[] GetNameColumns(SapiLayer1.EcuInfo ecuInfo)
  {
    return ecuInfo.Name.Split(new string[1]{ ": " }, StringSplitOptions.None);
  }

  private static void BuildStoredDataGroupXml(XmlWriter xmlWriter, IEnumerable<SapiLayer1.EcuInfo> items)
  {
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteAttributeString("colspan", "3");
    xmlWriter.WriteStartElement("table");
    xmlWriter.WriteAttributeString("class", "border");
    List<IGrouping<string, SapiLayer1.EcuInfo>> list1 = items.OrderBy<SapiLayer1.EcuInfo, string>((Func<SapiLayer1.EcuInfo, string>) (e => e.Services[0].BaseRequestMessage.ToString())).ThenBy<SapiLayer1.EcuInfo, int>((Func<SapiLayer1.EcuInfo, int>) (e => EcuInfoViewControl.GetBitPosition(e))).GroupBy<SapiLayer1.EcuInfo, string>((Func<SapiLayer1.EcuInfo, string>) (e => e.Services[0].BaseRequestMessage.ToString())).ToList<IGrouping<string, SapiLayer1.EcuInfo>>();
    int length = items.Max<SapiLayer1.EcuInfo>((Func<SapiLayer1.EcuInfo, int>) (e => EcuInfoViewControl.GetNameColumns(e).Length));
    int[] numArray = new int[length];
    foreach (IGrouping<string, SapiLayer1.EcuInfo> source1 in list1)
    {
      numArray = new int[numArray.Length];
      List<SapiLayer1.EcuInfo> list2 = source1.GroupBy<SapiLayer1.EcuInfo, int>((Func<SapiLayer1.EcuInfo, int>) (i => EcuInfoViewControl.GetBitPosition(i))).Select<IGrouping<int, SapiLayer1.EcuInfo>, SapiLayer1.EcuInfo>((Func<IGrouping<int, SapiLayer1.EcuInfo>, SapiLayer1.EcuInfo>) (gbp => gbp.First<SapiLayer1.EcuInfo>())).ToList<SapiLayer1.EcuInfo>();
      Queue<SapiLayer1.EcuInfo> source2 = new Queue<SapiLayer1.EcuInfo>((IEnumerable<SapiLayer1.EcuInfo>) list2);
      bool flag = true;
      while (source2.Count > 0)
      {
        xmlWriter.WriteStartElement("tr");
        int num;
        if (flag && ApplicationInformation.ProductAccessLevel == 3)
        {
          xmlWriter.WriteStartElement("th");
          XmlWriter xmlWriter1 = xmlWriter;
          num = list2.Count<SapiLayer1.EcuInfo>();
          string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          xmlWriter1.WriteAttributeString("rowspan", str);
          xmlWriter.WriteAttributeString("class", "standard request");
          xmlWriter.WriteString(BitConverter.ToString(new Dump(source1.Key).Data.ToArray<byte>()));
          xmlWriter.WriteFullEndElement();
          flag = false;
        }
        SapiLayer1.EcuInfo ecuInfo1 = source2.Dequeue();
        string[] nameColumns = EcuInfoViewControl.GetNameColumns(ecuInfo1);
        for (int i = 0; i < nameColumns.Length; num = i++)
        {
          string text = nameColumns[i];
          if (numArray[i] == 0)
          {
            xmlWriter.WriteStartElement(i == 0 ? "th" : "td");
            List<SapiLayer1.EcuInfo> list3 = source2.TakeWhile<SapiLayer1.EcuInfo>((Func<SapiLayer1.EcuInfo, bool>) (ecuInfo => EcuInfoViewControl.ColumnTextMatchesAtIndex(nameColumns, i, ecuInfo))).ToList<SapiLayer1.EcuInfo>();
            if (i == nameColumns.Length - 1)
            {
              XmlWriter xmlWriter2 = xmlWriter;
              num = 1 + (length - nameColumns.Length);
              string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              xmlWriter2.WriteAttributeString("colspan", str);
              if (list3.Count<SapiLayer1.EcuInfo>() > 0)
                list3 = list3.TakeWhile<SapiLayer1.EcuInfo>((Func<SapiLayer1.EcuInfo, bool>) (ecuInfo => ((IEnumerable<string>) EcuInfoViewControl.GetNameColumns(ecuInfo)).Count<string>() == nameColumns.Length)).ToList<SapiLayer1.EcuInfo>();
            }
            XmlWriter xmlWriter3 = xmlWriter;
            num = 1 + list3.Count;
            string str1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            xmlWriter3.WriteAttributeString("rowspan", str1);
            xmlWriter.WriteAttributeString("class", "name standard");
            xmlWriter.WriteString(text);
            xmlWriter.WriteFullEndElement();
            numArray[i] = list3.Count;
          }
          else
            numArray[i] = numArray[i] - 1;
        }
        EcuInfoViewControl.BuildItemValueXml(xmlWriter, ecuInfo1);
        xmlWriter.WriteFullEndElement();
      }
    }
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
  }

  private static void BuildGroupXml(XmlWriter xmlWriter, Group g, bool storedDataTab)
  {
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("group");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteString(g.Name);
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("group");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
    if (storedDataTab)
    {
      EcuInfoViewControl.BuildStoredDataGroupXml(xmlWriter, g.Items.OfType<SapiLayer1.EcuInfo>());
    }
    else
    {
      foreach (SapiLayer1.EcuInfo groupObject in g.Items)
        EcuInfoViewControl.BuildItemXml(xmlWriter, groupObject);
    }
  }

  private static string GetChannelString(Channel channel)
  {
    string channelString = channel.Ecu.Name;
    if (!string.IsNullOrEmpty(channel.Ecu.ShortDescription))
      channelString = $"{channelString} - {channel.Ecu.ShortDescription}";
    return channelString;
  }

  private static void BuildChannelXml(
    XmlWriter xmlWriter,
    Channel channel,
    bool expanded,
    GroupCollection groups,
    int index,
    bool includeResourceString,
    bool storedDataTab)
  {
    xmlWriter.WriteStartElement("div");
    xmlWriter.WriteStartAttribute("id");
    xmlWriter.WriteString(channel.Ecu.Name);
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("ecuitem");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("name");
    xmlWriter.WriteString(channel.Ecu.Name);
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartElement("table");
    xmlWriter.WriteStartAttribute("width");
    xmlWriter.WriteString("100%");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("valign");
    xmlWriter.WriteString("bottom");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartElement("a");
    xmlWriter.WriteStartAttribute("onclick");
    xmlWriter.WriteString("ExpandCollapse(this);");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("href");
    xmlWriter.WriteString("javascript:void(0)");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("link_collapse");
    xmlWriter.WriteEndAttribute();
    if (expanded)
      xmlWriter.WriteEntityRef("ndash");
    else
      xmlWriter.WriteString("+");
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
    if (includeResourceString)
    {
      xmlWriter.WriteStartElement("td");
      xmlWriter.WriteStartAttribute("valign");
      xmlWriter.WriteString("bottom");
      xmlWriter.WriteEndAttribute();
      xmlWriter.WriteStartAttribute("class");
      xmlWriter.WriteString("identifiertdpadding");
      xmlWriter.WriteEndAttribute();
      xmlWriter.WriteStartElement("div");
      xmlWriter.WriteStartAttribute("class");
      xmlWriter.WriteString("identifier");
      xmlWriter.WriteEndAttribute();
      xmlWriter.WriteString(channel.Identifier);
      xmlWriter.WriteFullEndElement();
      xmlWriter.WriteFullEndElement();
    }
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("ecu");
    xmlWriter.WriteEndAttribute();
    Channel channel1 = channel.RelatedChannels.Where<Channel>((Func<Channel, bool>) (ch => ch.Ecu.Priority < channel.Ecu.Priority)).OrderBy<Channel, int>((Func<Channel, int>) (ch => ch.Ecu.Priority)).FirstOrDefault<Channel>();
    if (channel1 != null)
      xmlWriter.WriteString($"{EcuInfoViewControl.GetChannelString(channel)} ({EcuInfoViewControl.GetChannelString(channel1)})");
    else
      xmlWriter.WriteString(EcuInfoViewControl.GetChannelString(channel));
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteStartAttribute("style");
    xmlWriter.WriteString("text-align: right");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("valign");
    xmlWriter.WriteString("bottom");
    xmlWriter.WriteEndAttribute();
    if (!channel.IsRollCall || !channel.DiagnosisVariant.IsBase)
    {
      xmlWriter.WriteStartElement("span");
      xmlWriter.WriteStartAttribute("class");
      xmlWriter.WriteString("variant");
      xmlWriter.WriteEndAttribute();
      xmlWriter.WriteString(channel.DiagnosisVariant.Name);
      if (channel.LogFile != null)
      {
        string[] strArray = channel.LogFile.MissingVariantList.Select<string, string[]>((Func<string, string[]>) (mv => mv.Split(".".ToCharArray()))).FirstOrDefault<string[]>((Func<string[], bool>) (mv => mv[0] == channel.Ecu.Name));
        if (strArray != null && strArray[1] != channel.DiagnosisVariant.Name)
        {
          xmlWriter.WriteRaw("&nbsp;");
          xmlWriter.WriteStartElement("del");
          xmlWriter.WriteAttributeString("style", "color:red");
          xmlWriter.WriteString(strArray[1]);
          xmlWriter.WriteFullEndElement();
        }
      }
      xmlWriter.WriteFullEndElement();
    }
    if (includeResourceString)
    {
      ConnectionResource connectionResource = SapiExtensions.GetActiveConnectionResource(channel);
      if (connectionResource != null)
      {
        xmlWriter.WriteStartElement("span");
        xmlWriter.WriteStartAttribute("class");
        xmlWriter.WriteString("resource");
        xmlWriter.WriteEndAttribute();
        xmlWriter.WriteString(SapiExtensions.ToDisplayString(connectionResource));
        xmlWriter.WriteFullEndElement();
      }
    }
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("div");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "gradientline{0}", (object) (index % 3)));
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("div");
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString(expanded ? "collapsable_content content_expanded" : "collapsable_content content_collapsed");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartElement("table");
    foreach (Group group in groups)
      EcuInfoViewControl.BuildGroupXml(xmlWriter, group, storedDataTab);
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteFullEndElement();
  }

  private void BuildTabXml(
    XmlWriter xmlWriter,
    string tabName,
    Dictionary<Channel, GroupCollection> channels)
  {
    xmlWriter.WriteStartElement("div");
    xmlWriter.WriteStartAttribute("id");
    xmlWriter.WriteString(tabName);
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("class");
    xmlWriter.WriteString("tab");
    xmlWriter.WriteEndAttribute();
    xmlWriter.WriteStartAttribute("name");
    xmlWriter.WriteString("TAB");
    xmlWriter.WriteEndAttribute();
    int num = 0;
    foreach (Channel key in channels.Keys)
    {
      bool expanded;
      if (!this.expandedStates.TryGetValue($"{tabName}.{key.Ecu.Name}", out expanded))
        expanded = true;
      EcuInfoViewControl.BuildChannelXml(xmlWriter, key, expanded, channels[key], num++, this.includeConnectionResource, this.useStoredDataStyle);
    }
    xmlWriter.WriteFullEndElement();
  }

  protected virtual void OnPaint(PaintEventArgs e)
  {
    if (this.dirty)
    {
      this.dirty = false;
      this.DeferredBuildList();
    }
    // ISSUE: explicit non-virtual call
    __nonvirtual (((Control) this).OnPaint(e));
  }

  private void BuildList()
  {
    if (!((Control) this).IsHandleCreated)
      return;
    this.dirty = true;
    ((Control) this).Invalidate();
  }

  private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!e.Succeeded || !(sender is Channel channel))
      return;
    channel.EcuInfos.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.EcuInfos_EcuInfoUpdateEvent);
    this.BuildList();
  }

  private void OnDisconnectCompleteEvent(object sender, EventArgs e)
  {
    if (!(sender is Channel channel))
      return;
    channel.EcuInfos.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.EcuInfos_EcuInfoUpdateEvent);
    this.BuildList();
  }

  private void OnUnitsSelectionChanged(object sender, EventArgs e) => this.BuildList();

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateActiveChannels();
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void GlobalInstance_ActiveChannelsChanged(object sender, EventArgs e)
  {
    this.UpdateActiveChannels();
  }

  private void UpdateActiveChannels()
  {
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        activeChannel.EcuInfos.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.EcuInfos_EcuInfoUpdateEvent);
    }
    this.activeChannels = SapiManager.GlobalInstance.ActiveChannels;
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        activeChannel.EcuInfos.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.EcuInfos_EcuInfoUpdateEvent);
    }
    this.BuildList();
  }

  private void EcuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
  {
    if (((Control) this).IsDisposed || !(this.inputPane != (HtmlElement) null) || this.dirty || !(sender is SapiLayer1.EcuInfo ecuInfo))
      return;
    string key = $"{ecuInfo.Channel.Ecu.Name}.{ecuInfo.Qualifier}";
    if (!this.elements.ContainsKey(key))
      return;
    HtmlElement element = this.elements[key];
    element.SetAttribute("className", "standard");
    string valueString = SapiExtensions.GetValueString(ecuInfo, ecuInfo.EcuInfoValues.Current);
    if (valueString.Length < 512 /*0x0200*/)
      element.InnerText = valueString;
    else
      element.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<span>{0}</span><button onclick=\"clickButton('{1}')\">{2}</button>", (object) valueString.Substring(0, 512 /*0x0200*/), (object) key, (object) Resources.EcuInfoViewControl_ShowMore));
  }

  public bool CanUndo => false;

  public void Undo()
  {
  }

  public bool CanCopy => this.editSupport.CanCopy;

  public void Copy() => this.editSupport.Copy();

  public bool CanDelete => false;

  public void Delete()
  {
  }

  public bool CanPaste => false;

  public void Paste()
  {
  }

  public bool CanSelectAll => this.editSupport.CanSelectAll;

  public void SelectAll() => this.editSupport.SelectAll();

  public bool CanCut => false;

  public void Cut()
  {
  }

  public bool CanProvideHtml => this.tabDivElement != (HtmlElement) null;

  public string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    HtmlElement tabDivElement = this.tabDivElement;
    if (tabDivElement != (HtmlElement) null)
    {
      stringBuilder.Append("<div class=\"pagebreakafter\">");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "<h1>{0}</h1>", (object) tabDivElement.Id);
      stringBuilder.Append(tabDivElement.InnerHtml);
      stringBuilder.Append("</div>");
    }
    return stringBuilder.ToString();
  }

  public string StyleSheet => string.Empty;

  public bool CanSearch => this.searchSupport.CanSearch;

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    return this.searchSupport.Search(searchText, caseSensitive, direction);
  }

  public bool CanRefreshView
  {
    get
    {
      if (this.activeChannels != null)
      {
        foreach (Channel activeChannel in this.activeChannels)
        {
          if (activeChannel.Online && activeChannel.CommunicationsState != CommunicationsState.ReadEcuInfo)
            return true;
        }
      }
      return false;
    }
  }

  public void RefreshView()
  {
    if (this.activeChannels == null)
      return;
    foreach (Channel activeChannel in this.activeChannels)
    {
      if (activeChannel.Online && activeChannel.CommunicationsState != CommunicationsState.ReadEcuInfo)
        activeChannel.EcuInfos.Read(false);
    }
  }

  private bool CanExpandOrCollapseAll(bool expand)
  {
    return this.expandedStates.Any<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (x => x.Value != expand && string.Equals(EcuInfoViewControl.TabNameFromKey(x.Key), this.groupName, StringComparison.Ordinal) && this.activeChannels.Any<Channel>((Func<Channel, bool>) (c => string.Equals(c.Ecu.Name, EcuInfoViewControl.EcuNameFromKey(x.Key), StringComparison.Ordinal)))));
  }

  public bool CanExpandAllItems => this.CanExpandOrCollapseAll(true);

  public bool CanCollapseAllItems => this.CanExpandOrCollapseAll(false);

  private void ExpandOrCollapseAllItems(bool expand)
  {
    string groupName = this.groupName;
    for (int index = 0; index < this.expandedStates.Count; ++index)
    {
      KeyValuePair<string, bool> expandedState = this.expandedStates.ElementAt<KeyValuePair<string, bool>>(index);
      if (expandedState.Value != expand && string.Equals(EcuInfoViewControl.TabNameFromKey(expandedState.Key), groupName, StringComparison.Ordinal) && this.activeChannels.Any<Channel>((Func<Channel, bool>) (c => string.Equals(c.Ecu.Name, EcuInfoViewControl.EcuNameFromKey(expandedState.Key), StringComparison.Ordinal))))
      {
        this.expandedStates[expandedState.Key] = expand;
        SettingsManager.GlobalInstance.SetValue<bool>("EcuInfoNodeState." + expandedState.Key, "EcuInfo", expand, false);
      }
    }
    this.BuildList();
  }

  private static string TabNameFromKey(string key) => key.Split(".".ToArray<char>())[0];

  private static string EcuNameFromKey(string key) => key.Split(".".ToArray<char>())[1];

  public void ExpandAllItems() => this.ExpandOrCollapseAllItems(true);

  public void CollapseAllItems() => this.ExpandOrCollapseAllItems(false);

  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (this.components != null)
        this.components.Dispose();
      SapiManager.GlobalInstance.ActiveChannelsChanged -= new EventHandler(this.GlobalInstance_ActiveChannelsChanged);
      Converter.GlobalInstance.UnitsSelectionChanged -= new EventHandler(this.OnUnitsSelectionChanged);
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (EcuInfoViewControl));
    this.webBrowser = new WebBrowser();
    this.contextMenuStrip = new ContextMenuStrip(this.components);
    this.manipulationToolStripMenuItem = new ToolStripMenuItem();
    this.contextMenuStrip.SuspendLayout();
    ((Control) this).SuspendLayout();
    this.webBrowser.AllowWebBrowserDrop = false;
    componentResourceManager.ApplyResources((object) this.webBrowser, "webBrowser");
    this.webBrowser.IsWebBrowserContextMenuEnabled = false;
    this.webBrowser.Name = "webBrowser";
    this.webBrowser.ScriptErrorsSuppressed = true;
    this.webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
    this.webBrowser.PreviewKeyDown += new PreviewKeyDownEventHandler(this.webBrowser_PreviewKeyDown);
    this.webBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.WebBrowser_Navigating);
    this.contextMenuStrip.ImageScalingSize = new Size(20, 20);
    this.contextMenuStrip.Items.AddRange(new ToolStripItem[1]
    {
      (ToolStripItem) this.manipulationToolStripMenuItem
    });
    this.contextMenuStrip.Name = "contextMenuStrip";
    componentResourceManager.ApplyResources((object) this.contextMenuStrip, "contextMenuStrip");
    this.manipulationToolStripMenuItem.Name = "manipulationToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.manipulationToolStripMenuItem, "manipulationToolStripMenuItem");
    this.manipulationToolStripMenuItem.Click += new EventHandler(this.manipulationToolStripMenuItem_Click);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((ContainerControl) this).AutoScaleMode = AutoScaleMode.Font;
    ((Control) this).Controls.Add((Control) this.webBrowser);
    ((Control) this).Name = nameof (EcuInfoViewControl);
    this.contextMenuStrip.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
