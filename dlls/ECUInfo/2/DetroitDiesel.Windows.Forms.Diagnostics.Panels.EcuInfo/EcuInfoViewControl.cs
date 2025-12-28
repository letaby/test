using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
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

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo;

public class EcuInfoViewControl : ContextHelpControl, ISupportEdit, IProvideHtml, ISearchable, IRefreshable, ISupportExpandCollapseAll
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

	private const int MaxValueLength = 512;

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
		get
		{
			return groupName;
		}
		set
		{
			if (value != groupName)
			{
				groupName = value;
				BuildList();
			}
		}
	}

	[Category("Appearance")]
	[Localizable(false)]
	[Description("Indicates if the connection resource strings should be shown in the tab.")]
	[DefaultValue(false)]
	public bool IncludeConnectionResource
	{
		get
		{
			return includeConnectionResource;
		}
		set
		{
			if (value != includeConnectionResource)
			{
				includeConnectionResource = value;
			}
		}
	}

	[Category("Appearance")]
	[Localizable(false)]
	[Description("Indicates if the 'Stored Data' view style should be used.")]
	[DefaultValue(false)]
	public bool UseStoredDataStyle
	{
		get
		{
			return useStoredDataStyle;
		}
		set
		{
			if (value != useStoredDataStyle)
			{
				useStoredDataStyle = value;
			}
		}
	}

	public bool CanUndo => false;

	public bool CanCopy => editSupport.CanCopy;

	public bool CanDelete => false;

	public bool CanPaste => false;

	public bool CanSelectAll => editSupport.CanSelectAll;

	public bool CanCut => false;

	public bool CanProvideHtml => tabDivElement != null;

	public string StyleSheet => string.Empty;

	public bool CanSearch => searchSupport.CanSearch;

	public bool CanRefreshView
	{
		get
		{
			if (activeChannels != null)
			{
				foreach (Channel activeChannel in activeChannels)
				{
					if (activeChannel.Online && activeChannel.CommunicationsState != CommunicationsState.ReadEcuInfo)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool CanExpandAllItems => CanExpandOrCollapseAll(expand: true);

	public bool CanCollapseAllItems => CanExpandOrCollapseAll(expand: false);

	public EcuInfoViewControl()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		((Control)(object)this).Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		SapiManager.GlobalInstance.ActiveChannelsChanged += GlobalInstance_ActiveChannelsChanged;
		Converter.GlobalInstance.UnitsSelectionChanged += OnUnitsSelectionChanged;
		((ContextHelpControl)this).SetLink(LinkSupport.GetViewLink((PanelIdentifier)0));
		editSupport.SetTarget((object)webBrowser);
		searchSupport.SetTarget((object)webBrowser);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<style type=\"text/css\">td {padding-right: 10px}");
		stringBuilder.Append(".gradientline0 { height:1px; font-size:1pt; background-color:red }");
		stringBuilder.Append(".gradientline1 { height:1px; font-size:1pt; background-color:green }");
		stringBuilder.Append(".gradientline2 { height:1px; font-size:1pt; background-color:blue }");
		string text = ((Control)(object)this).Font.SizeInPoints.ToString(CultureInfo.InvariantCulture);
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ".ecu {{ padding-top: 10px; width: 100%; font-family:{0}; font-size:{1}pt; font-weight:bold }}", ((Control)(object)this).Font.FontFamily.Name, ((double)((Control)(object)this).Font.SizeInPoints * 1.25).ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ".group {{ padding-top: 10px; font-family:{0}; font-size:{1}pt; font-weight:bold }}", ((Control)(object)this).Font.FontFamily.Name, text);
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ".standard {{ font-family:{0}; font-size:{1}pt }}", ((Control)(object)this).Font.FontFamily.Name, text);
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ".resource {{ display:block; font-family:{0}; font-size:{1}pt; font-style: italic; white-space: nowrap; color: Gray; }}", ((Control)(object)this).Font.FontFamily.Name, ((double)((Control)(object)this).Font.SizeInPoints * 0.75).ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ".variant {{ display:block; font-family:{0}; font-size:{1}pt; white-space: nowrap; text-align: right; }}", ((Control)(object)this).Font.FontFamily.Name, ((double)((Control)(object)this).Font.SizeInPoints * 0.75).ToString(CultureInfo.InvariantCulture));
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ".identifier {{ margin:0px 0px 0px 0px; padding:3px 6px 3px 6px; color: white; background-color: darkgray; font-family:{0}; font-size:{1}pt; white-space: nowrap;}}", ((Control)(object)this).Font.FontFamily.Name, text);
		stringBuilder.AppendLine(".identifiertdpadding { padding:0px 5px 0px 0px; }");
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "div.content_expanded {{display: block; padding-left: 0.2in; margin-top: 0px; margin-bottom: 0px;}}");
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "div.content_collapsed {{display: none;}}");
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "a.link_collapse{{ text-decoration: none;}}");
		stringBuilder.AppendLine("\r\n                table.border, table.border tr, table.border td, table.border th { border: 1px solid gray; border-collapse: collapse;  padding: 2px 6px 2px 6px; }\r\n                table.border th { background: #f4f4f4; text-align: left; }\r\n                th.request { white-space: nowrap; }\r\n                table.border, table.border tr, table.border td { background: #f1fdf1;  }\r\n                table.border td.name { background: #f1f1fd;  }\r\n\r\n                .loader {\r\n                    border: 2px solid #E0E0E0; /* Light grey */\r\n                    border-top: 2px solid #3498db; /* Blue */\r\n                    border-radius: 50%;\r\n                    width: " + text + "pt;\r\n                    height: " + text + "pt;\r\n                    animation: spin 1.25s linear infinite;\r\n                }\r\n                @keyframes spin\r\n                {\r\n                    0% { transform: rotate(0deg); }\r\n                    100% { transform: rotate(360deg); }\r\n                }\r\n\r\n                button {\r\n                  background-color: #008CBA; \r\n                  border: none;\r\n                  color: white;\r\n                  padding: 2px 4px;\r\n                  text-align: center;\r\n                  text-decoration: none;\r\n                  display: inline-block;\r\n                  cursor: pointer;\r\n                  margin: 0px 10px;\r\n                }\r\n\r\n            ");
		stringBuilder.Append("</style>");
		string text2 = "<script type='text/javascript'>\r\n                                    function ExpandCollapse(link) { \r\n                                        var parentEcuItemDiv = findParentElementByClass(link, 'ecuitem');\r\n                                        var contentDiv = getElementByClass(parentEcuItemDiv, 'collapsable_content');\r\n                                        if ( contentDiv.className.indexOf('content_expanded') != -1 ) {                                        \r\n                                            _changeContentDiv(link, contentDiv, 'Collapse');\r\n                                        }\r\n                                        else\r\n                                        {\r\n                                            _changeContentDiv(link, contentDiv, 'Expand');\r\n                                        }\r\n                                    }\r\n                                    // traverse up the DOM tree and returns the first element with a matching class if one is found. If a match is not found, it returns null. \r\n                                    function findParentElementByClass(element, parentClass){\r\n                                        var parentEle = null;\r\n                                        var parentElement;\r\n\r\n                                        parentElement = (parentElement == null) ? element.parentElement : parentElement.parentElement;\r\n                                        if ( parentElement.className.indexOf(parentClass) > -1 )\r\n                                        {\r\n                                            parentEle = parentElement;\r\n                                        } else {\r\n                                            parentEle = findParentElementByClass(parentElement, parentClass);\r\n                                        }\r\n                                        return parentEle;\r\n                                    }\r\n                                    // traverse down the DOM tree and returns the first element with a matching class if one if found. If a match is not found, it returns null. \r\n                                    function getElementByClass(element, elementClass) {\r\n                                        var ele = null;\r\n                                        if ( element != null )\r\n                                        {\r\n                                            if ( element.className.indexOf(elementClass) > -1 )\r\n                                            {\r\n                                                ele = element;\r\n                                            } else {\r\n                                                if ( element.children.length != 0 )\r\n                                                {\r\n                                                    for( var x = 0; x < element.children.length; x++ )\r\n                                                    {\r\n                                                        ele = getElementByClass(element.children.item(x), elementClass);\r\n                                                        if ( ele != null )\r\n                                                        {\r\n                                                            break;\r\n                                                        }\r\n                                                    }\r\n                                                }\r\n                                            }\r\n                                        }\r\n                                        return ele;\r\n                                    }\r\n                                    // Swaps the classes on the content div and text in anchor based on the desiredState. \r\n                                    function _changeContentDiv(anchor, contentDiv, desiredState){\r\n                                        var oldClass;\r\n                                        var newClass;\r\n                                        var innerHtmlText;\r\n                                        switch(desiredState)\r\n                                        {\r\n                                            case 'Expand':\r\n                                                oldClass = 'content_collapsed';\r\n                                                newClass = 'content_expanded';\r\n                                                innerHtmlText = '&ndash;';\r\n                                                break;\r\n                                            case 'Collapse':\r\n                                                oldClass = 'content_expanded';\r\n                                                newClass = 'content_collapsed';\r\n                                                innerHtmlText = '+';\r\n                                                break;\r\n                                        }\r\n                                        contentDiv.className = contentDiv.className.replace(oldClass, newClass);\r\n                                        anchor.innerHTML = innerHtmlText;\r\n                                    }\r\n\r\n\t\t                            //When a button is clicked, navigate to the supplied anchor ID. \r\n\t\t                            function clickButton(id)\r\n\t\t                            {\r\n\t\t\t                            window.location.href = '#button_'+id;\r\n\t\t                            }\r\n\r\n                              </script>";
		webBrowser.DocumentText = "<HTML><HEAD><meta http-equiv='X-UA-Compatible' content='IE=edge'>" + stringBuilder.ToString() + "\n" + text2 + "\n</HEAD><BODY><DIV id=\"inputpane\" name=\"inputpane\" class=\"standard\"><br></DIV></BODY></HTML>";
	}

	private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyCode == Keys.F5)
		{
			e.IsInputKey = true;
			if (CanRefreshView)
			{
				RefreshView();
			}
		}
	}

	private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		inputPane = webBrowser.Document.All.GetElementsByName("inputpane")[0];
		webBrowser.Document.ContextMenuShowing += Document_ContextMenuShowing;
		BuildList();
	}

	private void Document_ContextMenuShowing(object sender, HtmlElementEventArgs e)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		HtmlElement elementFromPoint = webBrowser.Document.GetElementFromPoint(e.MousePosition);
		if (!(elementFromPoint != null) || elementFromPoint.Id == null)
		{
			return;
		}
		string[] array = elementFromPoint.Id.Split(".".ToCharArray(), 2);
		if (array.Length == 2)
		{
			DataItem val = DataItem.Create(new Qualifier((QualifierTypes)8, array[0], array[1]), (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
			if (val != null)
			{
				contextMenuStrip.Show(((Control)this).PointToScreen(e.MousePosition));
				manipulationToolStripMenuItem.Visible = ManipulationForm.CanManipulate(val);
				manipulationToolStripMenuItem.Tag = val;
			}
		}
	}

	private void manipulationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		object tag = manipulationToolStripMenuItem.Tag;
		ManipulationForm.Show((DataItem)((tag is DataItem) ? tag : null));
	}

	private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
	{
		if (!(e.Url.ToString() != "about:blank") && string.IsNullOrEmpty(e.Url.Fragment))
		{
			return;
		}
		e.Cancel = true;
		if (!e.Url.Fragment.StartsWith("#button_", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		string[] id = e.Url.Fragment.Substring(8).Split(".".ToCharArray());
		SapiLayer1.EcuInfo ecuInfo = SapiManager.GlobalInstance.ActiveChannels.Where((Channel c) => c.Ecu.Name == id[0]).SelectMany((Channel c) => c.EcuInfos.Where((SapiLayer1.EcuInfo ei) => ei.Qualifier == id[1])).FirstOrDefault();
		if (ecuInfo != null)
		{
			CustomMessageBox.Show((IWin32Window)this, SapiExtensions.GetValueString(ecuInfo, ecuInfo.EcuInfoValues.Current), ecuInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, (CustomMessageBoxOptions)1);
		}
	}

	private void DeferredBuildList()
	{
		Cursor.Current = Cursors.WaitCursor;
		if (!((Control)this).IsDisposed && inputPane != null)
		{
			Dictionary<Channel, GroupCollection> channels = PreBuildData();
			ReadExpansionSettingsForThisTab(groupName, channels);
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder))
			{
				BuildTabXml(xmlWriter, groupName, channels);
			}
			inputPane.InnerHtml = stringBuilder.ToString();
			elements = (from e in inputPane.GetElementsByTagName("div").OfType<HtmlElement>()
				where !string.IsNullOrEmpty(e.Name)
				select e).ToDictionary((HtmlElement k) => k.Name, (HtmlElement v) => v);
			tabDivElement = inputPane.Children.GetElementsByName("TAB").OfType<HtmlElement>().FirstOrDefault();
			foreach (HtmlElement item in from el in tabDivElement.GetElementsByTagName("div").OfType<HtmlElement>()
				where el.GetAttribute("className") == "ecuitem"
				select el)
			{
				item.Click += ecuElement_Click;
			}
		}
		Cursor.Current = Cursors.Default;
	}

	private void ecuElement_Click(object sender, HtmlElementEventArgs e)
	{
		HtmlElement elementFromPoint = webBrowser.Document.GetElementFromPoint(e.MousePosition);
		HtmlElement htmlElement = sender as HtmlElement;
		string attribute = elementFromPoint.GetAttribute("className");
		if (attribute.Equals("link_collapse", StringComparison.Ordinal))
		{
			string attribute2 = htmlElement.GetAttribute("id");
			string attribute3 = htmlElement.Parent.GetAttribute("id");
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", attribute3, attribute2);
			bool flag = htmlElement.Children[htmlElement.Children.Count - 1].GetAttribute("className") == "collapsable_content content_expanded";
			if (!expandedStates.Keys.Contains(text) || expandedStates[text] != flag)
			{
				expandedStates[text] = flag;
				SettingsManager.GlobalInstance.SetValue<bool>("EcuInfoNodeState." + text, "EcuInfo", flag, false);
			}
		}
	}

	private void ReadExpansionSettingsForThisTab(string tabName, Dictionary<Channel, GroupCollection> channels)
	{
		foreach (KeyValuePair<Channel, GroupCollection> channel in channels)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", "EcuInfoNodeState", tabName, channel.Key.Ecu.Name);
			string key = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", tabName, channel.Key.Ecu.Name);
			bool value = SettingsManager.GlobalInstance.GetValue<bool>(text, "EcuInfo", true);
			expandedStates[key] = value;
		}
	}

	private Dictionary<Channel, GroupCollection> PreBuildData()
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		Dictionary<Channel, GroupCollection> dictionary = new Dictionary<Channel, GroupCollection>();
		if (activeChannels != null)
		{
			IEnumerable<Channel> source = activeChannels.Where((Channel channel) => (channel.ConnectionResource != null && channel.CommunicationsState != CommunicationsState.Offline) || channel.LogFile != null);
			foreach (Channel item in source.OrderBy((Channel c) => c.Ecu.Priority))
			{
				foreach (SapiLayer1.EcuInfo ecuInfo in item.EcuInfos)
				{
					if (!ecuInfo.Visible || string.Equals(ecuInfo.Qualifier, "DiagnosisVariant"))
					{
						continue;
					}
					string originalGroupName = ecuInfo.OriginalGroupName;
					string text = ((originalGroupName != null) ? originalGroupName.Split("/".ToArray())[0] : null);
					string text2 = ecuInfo.GroupName;
					int num = ecuInfo.GroupName.IndexOf('/');
					if (num != -1)
					{
						text2 = ecuInfo.GroupName.Substring(num + 1);
					}
					if (text == groupName)
					{
						GroupCollection val;
						if (!dictionary.ContainsKey(item))
						{
							val = new GroupCollection();
							dictionary.Add(item, val);
						}
						else
						{
							val = dictionary[item];
						}
						val.Add(text2, (object)ecuInfo);
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
		BuildItemValueXml(xmlWriter, groupObject);
		xmlWriter.WriteFullEndElement();
	}

	private static void BuildItemValueXml(XmlWriter xmlWriter, SapiLayer1.EcuInfo groupObject)
	{
		string text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", groupObject.Channel.Ecu.Name, groupObject.Qualifier);
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteStartElement("div");
		xmlWriter.WriteStartAttribute("class");
		xmlWriter.WriteString((groupObject.EcuInfoValues.Current != null) ? "standard" : "loader");
		xmlWriter.WriteEndAttribute();
		xmlWriter.WriteStartAttribute("id");
		xmlWriter.WriteString(text);
		xmlWriter.WriteEndAttribute();
		xmlWriter.WriteStartAttribute("name");
		xmlWriter.WriteString(text);
		xmlWriter.WriteEndAttribute();
		string valueString = SapiExtensions.GetValueString(groupObject, groupObject.EcuInfoValues.Current);
		if (valueString.Length < 512)
		{
			xmlWriter.WriteString(valueString);
		}
		else
		{
			xmlWriter.WriteStartElement("span");
			xmlWriter.WriteString(valueString.Substring(0, 512));
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteStartElement("button");
			xmlWriter.WriteAttributeString("onclick", FormattableString.Invariant($"clickButton('{text}')"));
			xmlWriter.WriteString(Resources.EcuInfoViewControl_ShowMore);
			xmlWriter.WriteFullEndElement();
		}
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteFullEndElement();
	}

	private static int GetBitPosition(SapiLayer1.EcuInfo item)
	{
		int result = 0;
		Presentation presentation = item?.Services.FirstOrDefault()?.OutputValues.FirstOrDefault();
		if (presentation != null)
		{
			result = Convert.ToInt32(presentation.BytePosition, CultureInfo.InvariantCulture) * 8 + Convert.ToInt32(presentation.BitPosition, CultureInfo.InvariantCulture);
		}
		return result;
	}

	private static bool ColumnTextMatchesAtIndex(string[] columns, int index, SapiLayer1.EcuInfo compareEcuInfo)
	{
		string[] nameColumns = GetNameColumns(compareEcuInfo);
		if (index >= columns.Length || index >= nameColumns.Length)
		{
			return false;
		}
		return columns[index] == nameColumns[index];
	}

	private static string[] GetNameColumns(SapiLayer1.EcuInfo ecuInfo)
	{
		return ecuInfo.Name.Split(new string[1] { ": " }, StringSplitOptions.None);
	}

	private static void BuildStoredDataGroupXml(XmlWriter xmlWriter, IEnumerable<SapiLayer1.EcuInfo> items)
	{
		xmlWriter.WriteStartElement("tr");
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteAttributeString("colspan", "3");
		xmlWriter.WriteStartElement("table");
		xmlWriter.WriteAttributeString("class", "border");
		List<IGrouping<string, SapiLayer1.EcuInfo>> list = (from e in items
			orderby e.Services[0].BaseRequestMessage.ToString(), GetBitPosition(e)
			group e by e.Services[0].BaseRequestMessage.ToString()).ToList();
		int num = items.Max((SapiLayer1.EcuInfo e) => GetNameColumns(e).Length);
		int[] array = new int[num];
		foreach (IGrouping<string, SapiLayer1.EcuInfo> item in list)
		{
			array = new int[array.Length];
			List<SapiLayer1.EcuInfo> list2 = (from item in item
				group item by GetBitPosition(item) into gbp
				select gbp.First()).ToList();
			Queue<SapiLayer1.EcuInfo> queue = new Queue<SapiLayer1.EcuInfo>(list2);
			bool flag = true;
			while (queue.Count > 0)
			{
				xmlWriter.WriteStartElement("tr");
				if (flag && ApplicationInformation.ProductAccessLevel == 3)
				{
					xmlWriter.WriteStartElement("th");
					xmlWriter.WriteAttributeString("rowspan", list2.Count().ToString(CultureInfo.InvariantCulture));
					xmlWriter.WriteAttributeString("class", "standard request");
					xmlWriter.WriteString(BitConverter.ToString(new Dump(item.Key).Data.ToArray()));
					xmlWriter.WriteFullEndElement();
					flag = false;
				}
				SapiLayer1.EcuInfo ecuInfo = queue.Dequeue();
				string[] nameColumns = GetNameColumns(ecuInfo);
				int i;
				for (i = 0; i < nameColumns.Length; i++)
				{
					string text = nameColumns[i];
					if (array[i] == 0)
					{
						xmlWriter.WriteStartElement((i == 0) ? "th" : "td");
						List<SapiLayer1.EcuInfo> list3 = queue.TakeWhile((SapiLayer1.EcuInfo compareEcuInfo) => ColumnTextMatchesAtIndex(nameColumns, i, compareEcuInfo)).ToList();
						if (i == nameColumns.Length - 1)
						{
							xmlWriter.WriteAttributeString("colspan", (1 + (num - nameColumns.Length)).ToString(CultureInfo.InvariantCulture));
							if (list3.Count() > 0)
							{
								list3 = list3.TakeWhile((SapiLayer1.EcuInfo ecuInfo2) => GetNameColumns(ecuInfo2).Count() == nameColumns.Length).ToList();
							}
						}
						xmlWriter.WriteAttributeString("rowspan", (1 + list3.Count).ToString(CultureInfo.InvariantCulture));
						xmlWriter.WriteAttributeString("class", "name standard");
						xmlWriter.WriteString(text);
						xmlWriter.WriteFullEndElement();
						array[i] = list3.Count;
					}
					else
					{
						array[i]--;
					}
				}
				BuildItemValueXml(xmlWriter, ecuInfo);
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
			BuildStoredDataGroupXml(xmlWriter, g.Items.OfType<SapiLayer1.EcuInfo>());
			return;
		}
		foreach (SapiLayer1.EcuInfo item in g.Items)
		{
			BuildItemXml(xmlWriter, item);
		}
	}

	private static string GetChannelString(Channel channel)
	{
		string text = channel.Ecu.Name;
		if (!string.IsNullOrEmpty(channel.Ecu.ShortDescription))
		{
			text = text + " - " + channel.Ecu.ShortDescription;
		}
		return text;
	}

	private static void BuildChannelXml(XmlWriter xmlWriter, Channel channel, bool expanded, GroupCollection groups, int index, bool includeResourceString, bool storedDataTab)
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
		{
			xmlWriter.WriteEntityRef("ndash");
		}
		else
		{
			xmlWriter.WriteString("+");
		}
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
		Channel channel2 = (from ch in channel.RelatedChannels
			where ch.Ecu.Priority < channel.Ecu.Priority
			orderby ch.Ecu.Priority
			select ch).FirstOrDefault();
		if (channel2 != null)
		{
			xmlWriter.WriteString(GetChannelString(channel) + " (" + GetChannelString(channel2) + ")");
		}
		else
		{
			xmlWriter.WriteString(GetChannelString(channel));
		}
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
				string[] array = channel.LogFile.MissingVariantList.Select((string mv) => mv.Split(".".ToCharArray())).FirstOrDefault((string[] mv) => mv[0] == channel.Ecu.Name);
				if (array != null && array[1] != channel.DiagnosisVariant.Name)
				{
					xmlWriter.WriteRaw("&nbsp;");
					xmlWriter.WriteStartElement("del");
					xmlWriter.WriteAttributeString("style", "color:red");
					xmlWriter.WriteString(array[1]);
					xmlWriter.WriteFullEndElement();
				}
			}
			xmlWriter.WriteFullEndElement();
		}
		if (includeResourceString)
		{
			ConnectionResource activeConnectionResource = SapiExtensions.GetActiveConnectionResource(channel);
			if (activeConnectionResource != null)
			{
				xmlWriter.WriteStartElement("span");
				xmlWriter.WriteStartAttribute("class");
				xmlWriter.WriteString("resource");
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteString(SapiExtensions.ToDisplayString(activeConnectionResource));
				xmlWriter.WriteFullEndElement();
			}
		}
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteStartElement("div");
		xmlWriter.WriteStartAttribute("class");
		xmlWriter.WriteString(string.Format(CultureInfo.InvariantCulture, "gradientline{0}", index % 3));
		xmlWriter.WriteEndAttribute();
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteStartElement("div");
		xmlWriter.WriteStartAttribute("class");
		xmlWriter.WriteString(expanded ? "collapsable_content content_expanded" : "collapsable_content content_collapsed");
		xmlWriter.WriteEndAttribute();
		xmlWriter.WriteStartElement("table");
		foreach (Group group in groups)
		{
			BuildGroupXml(xmlWriter, group, storedDataTab);
		}
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteFullEndElement();
	}

	private void BuildTabXml(XmlWriter xmlWriter, string tabName, Dictionary<Channel, GroupCollection> channels)
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
			if (!expandedStates.TryGetValue(tabName + "." + key.Ecu.Name, out var value))
			{
				value = true;
			}
			BuildChannelXml(xmlWriter, key, value, channels[key], num++, includeConnectionResource, useStoredDataStyle);
		}
		xmlWriter.WriteFullEndElement();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (dirty)
		{
			dirty = false;
			DeferredBuildList();
		}
		((Control)this).OnPaint(e);
	}

	private void BuildList()
	{
		if (((Control)this).IsHandleCreated)
		{
			dirty = true;
			((Control)this).Invalidate();
		}
	}

	private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded && sender is Channel channel)
		{
			channel.EcuInfos.EcuInfoUpdateEvent += EcuInfos_EcuInfoUpdateEvent;
			BuildList();
		}
	}

	private void OnDisconnectCompleteEvent(object sender, EventArgs e)
	{
		if (sender is Channel channel)
		{
			channel.EcuInfos.EcuInfoUpdateEvent -= EcuInfos_EcuInfoUpdateEvent;
			BuildList();
		}
	}

	private void OnUnitsSelectionChanged(object sender, EventArgs e)
	{
		BuildList();
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateActiveChannels();
		((UserControl)this).OnLoad(e);
	}

	private void GlobalInstance_ActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	private void UpdateActiveChannels()
	{
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent -= OnConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent -= OnDisconnectCompleteEvent;
			foreach (Channel activeChannel in activeChannels)
			{
				activeChannel.EcuInfos.EcuInfoUpdateEvent -= EcuInfos_EcuInfoUpdateEvent;
			}
		}
		activeChannels = SapiManager.GlobalInstance.ActiveChannels;
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent += OnConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent += OnDisconnectCompleteEvent;
			foreach (Channel activeChannel2 in activeChannels)
			{
				activeChannel2.EcuInfos.EcuInfoUpdateEvent += EcuInfos_EcuInfoUpdateEvent;
			}
		}
		BuildList();
	}

	private void EcuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
	{
		if (((Control)this).IsDisposed || !(inputPane != null) || dirty || !(sender is SapiLayer1.EcuInfo ecuInfo))
		{
			return;
		}
		string text = ecuInfo.Channel.Ecu.Name + "." + ecuInfo.Qualifier;
		if (elements.ContainsKey(text))
		{
			HtmlElement htmlElement = elements[text];
			htmlElement.SetAttribute("className", "standard");
			string valueString = SapiExtensions.GetValueString(ecuInfo, ecuInfo.EcuInfoValues.Current);
			if (valueString.Length < 512)
			{
				htmlElement.InnerText = valueString;
				return;
			}
			htmlElement.InnerHtml = FormattableString.Invariant($"<span>{valueString.Substring(0, 512)}</span><button onclick=\"clickButton('{text}')\">{Resources.EcuInfoViewControl_ShowMore}</button>");
		}
	}

	public void Undo()
	{
	}

	public void Copy()
	{
		editSupport.Copy();
	}

	public void Delete()
	{
	}

	public void Paste()
	{
	}

	public void SelectAll()
	{
		editSupport.SelectAll();
	}

	public void Cut()
	{
	}

	public string ToHtml()
	{
		StringBuilder stringBuilder = new StringBuilder();
		HtmlElement htmlElement = tabDivElement;
		if (htmlElement != null)
		{
			stringBuilder.Append("<div class=\"pagebreakafter\">");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "<h1>{0}</h1>", htmlElement.Id);
			stringBuilder.Append(htmlElement.InnerHtml);
			stringBuilder.Append("</div>");
		}
		return stringBuilder.ToString();
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return searchSupport.Search(searchText, caseSensitive, direction);
	}

	public void RefreshView()
	{
		if (activeChannels == null)
		{
			return;
		}
		foreach (Channel activeChannel in activeChannels)
		{
			if (activeChannel.Online && activeChannel.CommunicationsState != CommunicationsState.ReadEcuInfo)
			{
				activeChannel.EcuInfos.Read(synchronous: false);
			}
		}
	}

	private bool CanExpandOrCollapseAll(bool expand)
	{
		return expandedStates.Any((KeyValuePair<string, bool> x) => x.Value != expand && string.Equals(TabNameFromKey(x.Key), groupName, StringComparison.Ordinal) && activeChannels.Any((Channel c) => string.Equals(c.Ecu.Name, EcuNameFromKey(x.Key), StringComparison.Ordinal)));
	}

	private void ExpandOrCollapseAllItems(bool expand)
	{
		string b = groupName;
		for (int i = 0; i < expandedStates.Count; i++)
		{
			KeyValuePair<string, bool> expandedState = expandedStates.ElementAt(i);
			if (expandedState.Value != expand && string.Equals(TabNameFromKey(expandedState.Key), b, StringComparison.Ordinal) && activeChannels.Any((Channel c) => string.Equals(c.Ecu.Name, EcuNameFromKey(expandedState.Key), StringComparison.Ordinal)))
			{
				expandedStates[expandedState.Key] = expand;
				SettingsManager.GlobalInstance.SetValue<bool>("EcuInfoNodeState." + expandedState.Key, "EcuInfo", expand, false);
			}
		}
		BuildList();
	}

	private static string TabNameFromKey(string key)
	{
		return key.Split(".".ToArray())[0];
	}

	private static string EcuNameFromKey(string key)
	{
		return key.Split(".".ToArray())[1];
	}

	public void ExpandAllItems()
	{
		ExpandOrCollapseAllItems(expand: true);
	}

	public void CollapseAllItems()
	{
		ExpandOrCollapseAllItems(expand: false);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			SapiManager.GlobalInstance.ActiveChannelsChanged -= GlobalInstance_ActiveChannelsChanged;
			Converter.GlobalInstance.UnitsSelectionChanged -= OnUnitsSelectionChanged;
		}
		((ContextHelpControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(EcuInfoViewControl));
		webBrowser = new WebBrowser();
		contextMenuStrip = new ContextMenuStrip(components);
		manipulationToolStripMenuItem = new ToolStripMenuItem();
		contextMenuStrip.SuspendLayout();
		((Control)this).SuspendLayout();
		webBrowser.AllowWebBrowserDrop = false;
		componentResourceManager.ApplyResources(webBrowser, "webBrowser");
		webBrowser.IsWebBrowserContextMenuEnabled = false;
		webBrowser.Name = "webBrowser";
		webBrowser.ScriptErrorsSuppressed = true;
		webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
		webBrowser.PreviewKeyDown += webBrowser_PreviewKeyDown;
		webBrowser.Navigating += WebBrowser_Navigating;
		contextMenuStrip.ImageScalingSize = new Size(20, 20);
		contextMenuStrip.Items.AddRange(new ToolStripItem[1] { manipulationToolStripMenuItem });
		contextMenuStrip.Name = "contextMenuStrip";
		componentResourceManager.ApplyResources(contextMenuStrip, "contextMenuStrip");
		manipulationToolStripMenuItem.Name = "manipulationToolStripMenuItem";
		componentResourceManager.ApplyResources(manipulationToolStripMenuItem, "manipulationToolStripMenuItem");
		manipulationToolStripMenuItem.Click += manipulationToolStripMenuItem_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)this).Controls.Add(webBrowser);
		((Control)this).Name = "EcuInfoViewControl";
		contextMenuStrip.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
