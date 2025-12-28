// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Common.LogFileInfoDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Collections;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Container;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections;
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

#nullable disable
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

  internal LogFileInfoDialog(LogFile logFile)
  {
    this.logFile = logFile;
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.webBrowserLogFileInfo.DocumentClick += new HtmlElementEventHandler(this.WebBrowserLogFileInfo_DocumentClick);
  }

  private void LogFileInfoLoad(object sender, EventArgs e)
  {
    if (this.logFile == null)
      throw new InvalidOperationException("No LogFile loaded.");
    this.webBrowserLogFileInfo.SetWriterFunction(new Action<XmlWriter>(this.WriteLogFileInformation));
    this.webBrowserLogFileInfo.UpdateBrowser();
  }

  private void WriteLogFileInformation(XmlWriter writer)
  {
    string[] files = Directory.GetFiles(Directories.LogFiles, "*.DrumrollLog", SearchOption.TopDirectoryOnly);
    ReadOnlyCollection<LogFileSessionInfo> readOnlyCollection = LogFileSessionInfo.BuildList(this.logFile);
    writer.WriteStartElement("div");
    writer.WriteAttributeString("class", "heading1");
    writer.WriteString(Path.GetFileName(this.logFile.Name));
    writer.WriteEndElement();
    XmlWriter xmlWriter = writer;
    CultureInfo currentCulture = CultureInfo.CurrentCulture;
    DateTime dateTime = this.logFile.StartTime;
    string str1 = dateTime.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);
    dateTime = this.logFile.EndTime;
    string str2 = dateTime.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);
    string str3 = LogFileInfoDialog.FormatDuration(this.logFile.StartTime, this.logFile.EndTime);
    string str4 = string.Format((IFormatProvider) currentCulture, "{0} - {1} - {2}", (object) str1, (object) str2, (object) str3);
    xmlWriter.WriteElementString("div", str4);
    writer.WriteStartElement("div");
    writer.WriteAttributeString("class", "heading2");
    writer.WriteString(Resources.LogFileInformation_Group_SystemInformation);
    writer.WriteEndElement();
    writer.WriteStartElement("table");
    writer.WriteAttributeString("class", "noborder");
    foreach (string key in (IEnumerable) this.logFile.SystemInformation.Keys)
      LogFileInfoDialog.WriteRowKeyValue(writer, key, this.logFile.SystemInformation[key]);
    writer.WriteEndElement();
    List<KeyValuePair<string, DateTime>> filesForVin = (List<KeyValuePair<string, DateTime>>) null;
    Match match = SapiManager.ParseVin.Match(this.logFile.Name);
    if (match.Success)
      filesForVin = LogFileSessionInfo.GetSessionLogFilesForVin((IEnumerable<string>) files, match.Groups["vin"].Value).Select(fileName => new
      {
        fileName = fileName,
        fileDate = LogFileSessionInfo.ExtractSessionLogFileTime(fileName)
      }).Where(_param1 => _param1.fileDate.HasValue).Select(_param1 => new KeyValuePair<string, DateTime>(_param1.fileName, _param1.fileDate.Value)).ToList<KeyValuePair<string, DateTime>>();
    int num = 0;
    foreach (LogFileSessionInfo session in readOnlyCollection)
      LogFileInfoDialog.WriteSession(writer, session, ++num, filesForVin, readOnlyCollection.Count == 1);
    this.UpdateButtons();
  }

  private static void WriteSession(
    XmlWriter writer,
    LogFileSessionInfo session,
    int index,
    List<KeyValuePair<string, DateTime>> filesForVin,
    bool expanded)
  {
    string str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.LogFileInformation_GroupFormat_Session, (object) index);
    CultureInfo currentCulture = CultureInfo.CurrentCulture;
    DateTime dateTime = session.Start;
    string str2 = dateTime.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);
    dateTime = session.End;
    string str3 = dateTime.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);
    string str4 = LogFileInfoDialog.FormatDuration(session.Start, session.End);
    string str5 = string.Format((IFormatProvider) currentCulture, " {0} - {1} - {2}", (object) str2, (object) str3, (object) str4);
    string str6 = str1 + str5;
    WebBrowserList.WriteExpandableContent(writer, expanded, nameof (session), "heading3", str6, (Action) (() =>
    {
      if (filesForVin != null)
      {
        DateTime startSearchTime = session.Start - TimeSpan.FromSeconds(1.0);
        DateTime endSearchTime = session.End + TimeSpan.FromSeconds(1.0);
        IEnumerable<KeyValuePair<string, DateTime>> source = filesForVin.Where<KeyValuePair<string, DateTime>>((Func<KeyValuePair<string, DateTime>, bool>) (s => s.Value >= startSearchTime && s.Value <= endSearchTime));
        if (source.Any<KeyValuePair<string, DateTime>>())
        {
          writer.WriteStartElement("div");
          writer.WriteElementString("span", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}: ", (object) Resources.LogFileInformation_Item_FileName));
          writer.WriteStartElement("span");
          writer.WriteAttributeString("style", "text-decoration: underline; cursor: pointer;");
          writer.WriteString(source.First<KeyValuePair<string, DateTime>>().Key);
          writer.WriteEndElement();
        }
      }
      if (ApplicationInformation.CanAccessDataMiningInformation)
        writer.WriteElementString("div", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}: {1}", (object) Resources.LogFileInformation_Item_DataMiningProcessTag, (object) session.DataMiningTag.ToString()));
      foreach (IGrouping<bool, FaultCodeIncident> incidents in session.Incidents.OrderBy<FaultCodeIncident, bool>((Func<FaultCodeIncident, bool>) (i => i.FaultCode.Channel.IsRollCall)).ThenBy<FaultCodeIncident, bool>((Func<FaultCodeIncident, bool>) (i => !SapiManager.IsFaultActionable(i))).GroupBy<FaultCodeIncident, Tuple<string, string>>((Func<FaultCodeIncident, Tuple<string, string>>) (i => new Tuple<string, string>(i.FaultCode.Number, i.FaultCode.Mode))).Select<IGrouping<Tuple<string, string>, FaultCodeIncident>, FaultCodeIncident>((Func<IGrouping<Tuple<string, string>, FaultCodeIncident>, FaultCodeIncident>) (g => g.First<FaultCodeIncident>())).GroupBy<FaultCodeIncident, bool>((Func<FaultCodeIncident, bool>) (i => SapiManager.IsFaultActionable(i))))
        LogFileInfoDialog.WriteFaultInfo(writer, incidents.Key, (IEnumerable<FaultCodeIncident>) incidents);
      foreach (Session channelSession in (IEnumerable<Session>) session.ChannelSessions.OrderBy<Session, int>((Func<Session, int>) (c => c.Channel.Ecu.Priority)))
        LogFileInfoDialog.WriteChannelSession(writer, channelSession);
    }));
  }

  private static void WriteFaultInfo(
    XmlWriter writer,
    bool active,
    IEnumerable<FaultCodeIncident> incidents)
  {
    WebBrowserList.WriteExpandableContent(writer, true, "faultinfo", "heading3", active ? Resources.LogFileInformation_Group_ActiveFaults : Resources.LogFileInformation_Group_InactiveFaults, (Action) (() =>
    {
      foreach (FaultCodeIncident incident in incidents)
        writer.WriteElementString("div", SapiExtensions.FormatDisplayString(incident.FaultCode, (FaultCodeDisplayIncludeInfo) 1));
    }));
  }

  private static void WriteChannelSession(XmlWriter writer, Session channelSession)
  {
    WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading3", channelSession.Channel.Ecu.DisplayName, (Action) (() =>
    {
      if (!channelSession.Channel.IsRollCall && !string.IsNullOrEmpty(channelSession.DescriptionVersion))
        writer.WriteElementString("div", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}: {1}", channelSession.Channel.Ecu.IsMcd ? (object) Resources.LogFileInformation_Item_SMRVersion : (object) Resources.LogFileInformation_Item_CBFVersion, (object) channelSession.DescriptionVersion));
      XmlWriter xmlWriter = writer;
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      string diagnosticVariant = Resources.LogFileInformation_Item_DiagnosticVariant;
      string variantName = channelSession.VariantName;
      bool? isFixedVariant = channelSession.IsFixedVariant;
      string str1;
      if (isFixedVariant.HasValue)
      {
        isFixedVariant = channelSession.IsFixedVariant;
        if (isFixedVariant.Value)
        {
          str1 = $"({Resources.LogFileInformation_FixedVariant})";
          goto label_6;
        }
      }
      str1 = string.Empty;
label_6:
      string str2 = string.Format((IFormatProvider) currentCulture, "{0}: {1} {2}", (object) diagnosticVariant, (object) variantName, (object) str1);
      xmlWriter.WriteElementString("div", str2);
      if (channelSession.ChannelOptions != 253)
      {
        bool flag1 = (channelSession.ChannelOptions & 1) > 0;
        bool flag2 = (channelSession.ChannelOptions & ConnectionDialog.ChannelOptionsCyclicServices) > 0;
        bool flag3 = (channelSession.ChannelOptions & ConnectionDialog.ChannelOptionsAutoExecuteConfiguredServices) > 0;
        List<string> values = new List<string>();
        if (!flag1)
          values.Add(Resources.LogFileInformation_DontAutoStartStop);
        if (!flag2)
          values.Add(Resources.LogFileInformation_DontCyclicallyRead);
        if (!flag3)
          values.Add(Resources.LogFileInformation_DontAutoExecuteConfiguredServices);
        writer.WriteElementString("div", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}: {1}", (object) Resources.LogFileInformation_AdvancedChannelOptions, (object) string.Join(", ", (IEnumerable<string>) values)));
      }
      writer.WriteElementString("div", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}: {1}", (object) Resources.LogFileInformation_ConnectionResource, (object) SapiExtensions.ToDisplayString(channelSession.Resource)));
      GroupCollection source = new GroupCollection();
      foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) channelSession.Channel.EcuInfos)
      {
        if (ecuInfo.Visible && ecuInfo.Common && !string.Equals(ecuInfo.Qualifier, "DiagnosisVariant"))
        {
          string str3 = ecuInfo.GroupName;
          int num = str3.IndexOf("/", StringComparison.Ordinal);
          if (num != -1)
            str3 = str3.Substring(num + 1);
          source.Add(str3, (object) ecuInfo);
        }
      }
      if (!((IEnumerable<Group>) source).Any<Group>())
        return;
      writer.WriteStartElement("table");
      writer.WriteAttributeString("class", "noborder");
      foreach (Group group in source)
      {
        LogFileInfoDialog.WriteRowHeading(writer, group.Name);
        foreach (EcuInfo ecuInfo in group.Items)
          LogFileInfoDialog.WriteRowKeyValue(writer, ecuInfo.Name, SapiExtensions.GetValueString(ecuInfo, ((IEnumerable<EcuInfoValue>) ecuInfo.EcuInfoValues).FirstOrDefault<EcuInfoValue>((Func<EcuInfoValue, bool>) (v => v.Value != null))));
      }
      writer.WriteEndElement();
    }));
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
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DurationFormat_HoursMinutes, (object) timeSpan.Hours, (object) timeSpan.Minutes);
    return timeSpan > new TimeSpan(0, 1, 0) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DurationFormat_Minutes, (object) timeSpan.Minutes) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DurationFormat_Seconds, (object) timeSpan.Seconds);
  }

  private bool CanPrint => !PrintHelper.Busy && this.CanProvideHtml;

  private void ButtonPrintClick(object sender, EventArgs e)
  {
    if (!this.CanPrint)
      return;
    PrintHelper.ShowPrintDialog(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CaptionFormatPrintLogInfo, (object) Path.GetFileNameWithoutExtension(this.logFile.Name)), (IProvideHtml) this, (IncludeInfo) 0);
  }

  public bool CanProvideHtml => this.webBrowserLogFileInfo.CanProvideHtml;

  public string ToHtml() => this.webBrowserLogFileInfo.ToHtml();

  public string StyleSheet => this.webBrowserLogFileInfo.StyleSheet;

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_LogFileInfoDialog"));
  }

  private void WebBrowserLogFileInfo_DocumentClick(object sender, HtmlElementEventArgs e)
  {
    HtmlElement elementFromPoint = this.webBrowserLogFileInfo.GetElementFromPoint(((Control) this.webBrowserLogFileInfo).PointToClient(Control.MousePosition));
    string str = (string) null;
    if (elementFromPoint != (HtmlElement) null && elementFromPoint.InnerHtml != null && !elementFromPoint.InnerHtml.Contains("<p>") && elementFromPoint.InnerHtml.Contains(".DrumrollLog"))
      str = elementFromPoint.InnerHtml;
    this.SelectedLogFile = str;
    this.UpdateButtons();
  }

  internal string SelectedLogFile { get; set; }

  private void UpdateButtons() => this.buttonCopyLink.Enabled = this.SelectedLogFile != null;

  private void ButtonCopyLinkClick(object sender, EventArgs e)
  {
    if (this.SelectedLogFile != null)
      FileManagement.CopyLink((Control) this, !string.Equals(this.SelectedLogFile, Path.GetFileName(this.logFile.Name), StringComparison.OrdinalIgnoreCase) ? Path.Combine(Directories.LogFiles, this.SelectedLogFile) : this.logFile.Name);
    this.buttonCopyLink.Enabled = false;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (LogFileInfoDialog));
    this.close = new Button();
    this.buttonPrint = new Button();
    this.buttonCopyLink = new Button();
    this.webBrowserLogFileInfo = new WebBrowserList();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.close, "close");
    this.close.DialogResult = DialogResult.Cancel;
    this.close.Name = "close";
    this.close.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonPrint, "buttonPrint");
    this.buttonPrint.Name = "buttonPrint";
    this.buttonPrint.UseVisualStyleBackColor = true;
    this.buttonPrint.Click += new EventHandler(this.ButtonPrintClick);
    componentResourceManager.ApplyResources((object) this.buttonCopyLink, "buttonCopyLink");
    this.buttonCopyLink.Name = "buttonCopyLink";
    this.buttonCopyLink.UseVisualStyleBackColor = true;
    this.buttonCopyLink.Click += new EventHandler(this.ButtonCopyLinkClick);
    componentResourceManager.ApplyResources((object) this.webBrowserLogFileInfo, "webBrowserLogFileInfo");
    ((Control) this.webBrowserLogFileInfo).Name = "webBrowserLogFileInfo";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = SystemColors.Window;
    this.Controls.Add((Control) this.webBrowserLogFileInfo);
    this.Controls.Add((Control) this.buttonCopyLink);
    this.Controls.Add((Control) this.buttonPrint);
    this.Controls.Add((Control) this.close);
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (LogFileInfoDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.Load += new EventHandler(this.LogFileInfoLoad);
    this.ResumeLayout(false);
  }
}
