using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Collections;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Container;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

namespace DetroitDiesel.Common;

public class LogFileInfoDialog : Form, IProvideHtml
{
	private const int LogTimeBoundaryExpansion = 1;

	private LogFile logFile;

	private IContainer components;

	private Button close;

	private Button buttonPrint;

	private Button buttonCopyLink;

	private WebBrowserList webBrowserLogFileInfo;

	private bool CanPrint
	{
		get
		{
			if (!PrintHelper.Busy)
			{
				return CanProvideHtml;
			}
			return false;
		}
	}

	public bool CanProvideHtml => webBrowserLogFileInfo.CanProvideHtml;

	public string StyleSheet => webBrowserLogFileInfo.StyleSheet;

	internal string SelectedLogFile { get; set; }

	internal LogFileInfoDialog(LogFile logFile)
	{
		this.logFile = logFile;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		webBrowserLogFileInfo.DocumentClick += WebBrowserLogFileInfo_DocumentClick;
	}

	private void LogFileInfoLoad(object sender, EventArgs e)
	{
		if (logFile == null)
		{
			throw new InvalidOperationException("No LogFile loaded.");
		}
		webBrowserLogFileInfo.SetWriterFunction((Action<XmlWriter>)WriteLogFileInformation);
		webBrowserLogFileInfo.UpdateBrowser();
	}

	private void WriteLogFileInformation(XmlWriter writer)
	{
		string[] files = Directory.GetFiles(Directories.LogFiles, "*.DrumrollLog", SearchOption.TopDirectoryOnly);
		ReadOnlyCollection<LogFileSessionInfo> readOnlyCollection = LogFileSessionInfo.BuildList(logFile);
		writer.WriteStartElement("div");
		writer.WriteAttributeString("class", "heading1");
		writer.WriteString(Path.GetFileName(logFile.Name));
		writer.WriteEndElement();
		writer.WriteElementString("div", string.Format(CultureInfo.CurrentCulture, "{0} - {1} - {2}", logFile.StartTime.ToString("G", CultureInfo.CurrentCulture), logFile.EndTime.ToString("G", CultureInfo.CurrentCulture), FormatDuration(logFile.StartTime, logFile.EndTime)));
		writer.WriteStartElement("div");
		writer.WriteAttributeString("class", "heading2");
		writer.WriteString(Resources.LogFileInformation_Group_SystemInformation);
		writer.WriteEndElement();
		writer.WriteStartElement("table");
		writer.WriteAttributeString("class", "noborder");
		foreach (string key in logFile.SystemInformation.Keys)
		{
			WriteRowKeyValue(writer, key, logFile.SystemInformation[key]);
		}
		writer.WriteEndElement();
		List<KeyValuePair<string, DateTime>> filesForVin = null;
		Match match = SapiManager.ParseVin.Match(logFile.Name);
		if (match.Success)
		{
			filesForVin = (from fileName in LogFileSessionInfo.GetSessionLogFilesForVin((IEnumerable<string>)files, match.Groups["vin"].Value)
				let fileDate = LogFileSessionInfo.ExtractSessionLogFileTime(fileName)
				where fileDate.HasValue
				select new KeyValuePair<string, DateTime>(fileName, fileDate.Value)).ToList();
		}
		int num = 0;
		foreach (LogFileSessionInfo item in readOnlyCollection)
		{
			WriteSession(writer, item, ++num, filesForVin, readOnlyCollection.Count == 1);
		}
		UpdateButtons();
	}

	private static void WriteSession(XmlWriter writer, LogFileSessionInfo session, int index, List<KeyValuePair<string, DateTime>> filesForVin, bool expanded)
	{
		string text = string.Format(CultureInfo.CurrentCulture, Resources.LogFileInformation_GroupFormat_Session, index) + string.Format(CultureInfo.CurrentCulture, " {0} - {1} - {2}", session.Start.ToString("G", CultureInfo.CurrentCulture), session.End.ToString("G", CultureInfo.CurrentCulture), FormatDuration(session.Start, session.End));
		WebBrowserList.WriteExpandableContent(writer, expanded, "session", "heading3", text, (Action)delegate
		{
			if (filesForVin != null)
			{
				DateTime startSearchTime = session.Start - TimeSpan.FromSeconds(1.0);
				DateTime endSearchTime = session.End + TimeSpan.FromSeconds(1.0);
				IEnumerable<KeyValuePair<string, DateTime>> source = filesForVin.Where((KeyValuePair<string, DateTime> s) => s.Value >= startSearchTime && s.Value <= endSearchTime);
				if (source.Any())
				{
					writer.WriteStartElement("div");
					writer.WriteElementString("span", string.Format(CultureInfo.CurrentCulture, "{0}: ", Resources.LogFileInformation_Item_FileName));
					writer.WriteStartElement("span");
					writer.WriteAttributeString("style", "text-decoration: underline; cursor: pointer;");
					writer.WriteString(source.First().Key);
					writer.WriteEndElement();
				}
			}
			if (ApplicationInformation.CanAccessDataMiningInformation)
			{
				writer.WriteElementString("div", string.Format(CultureInfo.CurrentCulture, "{0}: {1}", Resources.LogFileInformation_Item_DataMiningProcessTag, session.DataMiningTag.ToString()));
			}
			IEnumerable<IGrouping<bool, FaultCodeIncident>> enumerable = from i in session.Incidents
				orderby i.FaultCode.Channel.IsRollCall, !SapiManager.IsFaultActionable(i)
				group i by new Tuple<string, string>(i.FaultCode.Number, i.FaultCode.Mode) into g
				select g.First() into i
				group i by SapiManager.IsFaultActionable(i);
			foreach (IGrouping<bool, FaultCodeIncident> item in enumerable)
			{
				WriteFaultInfo(writer, item.Key, item);
			}
			foreach (Session item2 in session.ChannelSessions.OrderBy((Session c) => c.Channel.Ecu.Priority))
			{
				WriteChannelSession(writer, item2);
			}
		});
	}

	private static void WriteFaultInfo(XmlWriter writer, bool active, IEnumerable<FaultCodeIncident> incidents)
	{
		WebBrowserList.WriteExpandableContent(writer, true, "faultinfo", "heading3", active ? Resources.LogFileInformation_Group_ActiveFaults : Resources.LogFileInformation_Group_InactiveFaults, (Action)delegate
		{
			foreach (FaultCodeIncident incident in incidents)
			{
				writer.WriteElementString("div", SapiExtensions.FormatDisplayString(incident.FaultCode, (FaultCodeDisplayIncludeInfo)1));
			}
		});
	}

	private static void WriteChannelSession(XmlWriter writer, Session channelSession)
	{
		WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading3", channelSession.Channel.Ecu.DisplayName, (Action)delegate
		{
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Invalid comparison between Unknown and I4
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Invalid comparison between Unknown and I4
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Invalid comparison between Unknown and I4
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Invalid comparison between Unknown and I4
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Expected O, but got Unknown
			if (!channelSession.Channel.IsRollCall && !string.IsNullOrEmpty(channelSession.DescriptionVersion))
			{
				writer.WriteElementString("div", string.Format(CultureInfo.CurrentCulture, "{0}: {1}", channelSession.Channel.Ecu.IsMcd ? Resources.LogFileInformation_Item_SMRVersion : Resources.LogFileInformation_Item_CBFVersion, channelSession.DescriptionVersion));
			}
			writer.WriteElementString("div", string.Format(CultureInfo.CurrentCulture, "{0}: {1} {2}", Resources.LogFileInformation_Item_DiagnosticVariant, channelSession.VariantName, (channelSession.IsFixedVariant.HasValue && channelSession.IsFixedVariant.Value) ? ("(" + Resources.LogFileInformation_FixedVariant + ")") : string.Empty));
			if ((int)channelSession.ChannelOptions != 253)
			{
				bool flag = (channelSession.ChannelOptions & 1) > 0;
				bool flag2 = (channelSession.ChannelOptions & ConnectionDialog.ChannelOptionsCyclicServices) > 0;
				bool flag3 = (channelSession.ChannelOptions & ConnectionDialog.ChannelOptionsAutoExecuteConfiguredServices) > 0;
				List<string> list = new List<string>();
				if (!flag)
				{
					list.Add(Resources.LogFileInformation_DontAutoStartStop);
				}
				if (!flag2)
				{
					list.Add(Resources.LogFileInformation_DontCyclicallyRead);
				}
				if (!flag3)
				{
					list.Add(Resources.LogFileInformation_DontAutoExecuteConfiguredServices);
				}
				writer.WriteElementString("div", string.Format(CultureInfo.CurrentCulture, "{0}: {1}", Resources.LogFileInformation_AdvancedChannelOptions, string.Join(", ", list)));
			}
			writer.WriteElementString("div", string.Format(CultureInfo.CurrentCulture, "{0}: {1}", Resources.LogFileInformation_ConnectionResource, SapiExtensions.ToDisplayString(channelSession.Resource)));
			GroupCollection val = new GroupCollection();
			foreach (EcuInfo item in (ReadOnlyCollection<EcuInfo>)(object)channelSession.Channel.EcuInfos)
			{
				if (item.Visible && item.Common && !string.Equals(item.Qualifier, "DiagnosisVariant"))
				{
					string text = item.GroupName;
					int num = text.IndexOf("/", StringComparison.Ordinal);
					if (num != -1)
					{
						text = text.Substring(num + 1);
					}
					val.Add(text, (object)item);
				}
			}
			if (((IEnumerable<Group>)val).Any())
			{
				writer.WriteStartElement("table");
				writer.WriteAttributeString("class", "noborder");
				foreach (Group item2 in val)
				{
					WriteRowHeading(writer, item2.Name);
					foreach (EcuInfo item3 in item2.Items)
					{
						EcuInfo val2 = item3;
						WriteRowKeyValue(writer, val2.Name, SapiExtensions.GetValueString(val2, ((IEnumerable<EcuInfoValue>)val2.EcuInfoValues).FirstOrDefault((EcuInfoValue v) => v.Value != null)));
					}
				}
				writer.WriteEndElement();
			}
		});
	}

	private static void WriteRowKeyValue(XmlWriter writer, string key, string value)
	{
		writer.WriteStartElement("tr");
		writer.WriteStartElement("td");
		writer.WriteString(key);
		writer.WriteEndElement();
		writer.WriteStartElement("td");
		writer.WriteRaw("&nbsp;&nbsp;");
		writer.WriteFullEndElement();
		writer.WriteStartElement("td");
		writer.WriteStartAttribute("style");
		writer.WriteString("word-break:break-all");
		writer.WriteEndAttribute();
		writer.WriteString(value);
		writer.WriteEndElement();
		writer.WriteEndElement();
	}

	private static void WriteRowHeading(XmlWriter writer, string heading)
	{
		writer.WriteStartElement("tr");
		writer.WriteStartElement("th", heading);
		writer.WriteStartAttribute("colspan");
		writer.WriteString("3");
		writer.WriteEndAttribute();
		writer.WriteStartAttribute("align");
		writer.WriteString("left");
		writer.WriteEndAttribute();
		writer.WriteString(heading);
		writer.WriteEndElement();
		writer.WriteEndElement();
	}

	private static string FormatDuration(DateTime startTime, DateTime endTime)
	{
		TimeSpan timeSpan = endTime - startTime;
		if (timeSpan > new TimeSpan(1, 0, 0))
		{
			return string.Format(CultureInfo.CurrentCulture, Resources.DurationFormat_HoursMinutes, timeSpan.Hours, timeSpan.Minutes);
		}
		if (timeSpan > new TimeSpan(0, 1, 0))
		{
			return string.Format(CultureInfo.CurrentCulture, Resources.DurationFormat_Minutes, timeSpan.Minutes);
		}
		return string.Format(CultureInfo.CurrentCulture, Resources.DurationFormat_Seconds, timeSpan.Seconds);
	}

	private void ButtonPrintClick(object sender, EventArgs e)
	{
		if (CanPrint)
		{
			PrintHelper.ShowPrintDialog(string.Format(CultureInfo.CurrentCulture, Resources.CaptionFormatPrintLogInfo, Path.GetFileNameWithoutExtension(logFile.Name)), (IProvideHtml)(object)this, (IncludeInfo)0);
		}
	}

	public string ToHtml()
	{
		return webBrowserLogFileInfo.ToHtml();
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_LogFileInfoDialog"));
	}

	private void WebBrowserLogFileInfo_DocumentClick(object sender, HtmlElementEventArgs e)
	{
		HtmlElement elementFromPoint = webBrowserLogFileInfo.GetElementFromPoint(((Control)(object)webBrowserLogFileInfo).PointToClient(Control.MousePosition));
		string selectedLogFile = null;
		if (elementFromPoint != null && elementFromPoint.InnerHtml != null && !elementFromPoint.InnerHtml.Contains("<p>") && elementFromPoint.InnerHtml.Contains(".DrumrollLog"))
		{
			selectedLogFile = elementFromPoint.InnerHtml;
		}
		SelectedLogFile = selectedLogFile;
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		buttonCopyLink.Enabled = SelectedLogFile != null;
	}

	private void ButtonCopyLinkClick(object sender, EventArgs e)
	{
		if (SelectedLogFile != null)
		{
			string text = ((!string.Equals(SelectedLogFile, Path.GetFileName(logFile.Name), StringComparison.OrdinalIgnoreCase)) ? Path.Combine(Directories.LogFiles, SelectedLogFile) : logFile.Name);
			FileManagement.CopyLink((Control)this, text);
		}
		buttonCopyLink.Enabled = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Common.LogFileInfoDialog));
		this.close = new System.Windows.Forms.Button();
		this.buttonPrint = new System.Windows.Forms.Button();
		this.buttonCopyLink = new System.Windows.Forms.Button();
		this.webBrowserLogFileInfo = new WebBrowserList();
		base.SuspendLayout();
		resources.ApplyResources(this.close, "close");
		this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.close.Name = "close";
		this.close.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.buttonPrint, "buttonPrint");
		this.buttonPrint.Name = "buttonPrint";
		this.buttonPrint.UseVisualStyleBackColor = true;
		this.buttonPrint.Click += new System.EventHandler(ButtonPrintClick);
		resources.ApplyResources(this.buttonCopyLink, "buttonCopyLink");
		this.buttonCopyLink.Name = "buttonCopyLink";
		this.buttonCopyLink.UseVisualStyleBackColor = true;
		this.buttonCopyLink.Click += new System.EventHandler(ButtonCopyLinkClick);
		resources.ApplyResources(this.webBrowserLogFileInfo, "webBrowserLogFileInfo");
		((System.Windows.Forms.Control)(object)this.webBrowserLogFileInfo).Name = "webBrowserLogFileInfo";
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Window;
		base.Controls.Add((System.Windows.Forms.Control)(object)this.webBrowserLogFileInfo);
		base.Controls.Add(this.buttonCopyLink);
		base.Controls.Add(this.buttonPrint);
		base.Controls.Add(this.close);
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "LogFileInfoDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		base.Load += new System.EventHandler(LogFileInfoLoad);
		base.ResumeLayout(false);
	}
}
