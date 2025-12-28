// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.VRDU_Snapshot.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VRDU_Snapshot.panel;

public class UserPanel : CustomPanel
{
  private const ushort Value_A_Config_1 = 514;
  private const ushort Value_C_Config_1 = 49818;
  private const string VrduSeedAddress = "270D";
  private const string VrduKeyAddress = "270E";
  private const string VrduKeyAcknowledgeResponse = "670E";
  private const string VrduFingerprintAddress = "2EF15C";
  private const string VrduFpAcknowledgeResponse = "6EF15C";
  private const string TOOL_SUPPLIER_ID = "010004";
  private const string InvalidDataMessage = "7f2271";
  private const int SeedMessageLength = 4;
  private const string CO_EcuSerialNumber_Service = "CO_EcuSerialNumber";
  private const string ABA_Function_Counter_Service = "DT_STO_ABA_Function_Counter_ABA_Function_Counter";
  private const string ABA_Data_Service = "DT_STO_ABA_Function_Data_{0}_ABA_Function_Data_{0}";
  private const string _16ByteUintNaN = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
  private const string _16ByteSintNaN = "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
  private string rawData;
  private Channel vrdu;
  private bool extractionInProgress = false;
  private int statusCounter = 0;
  private TableLayoutPanel tableLayoutPanel1;
  private TextBox textBoxGrammar;
  private SeekTimeListView seekTimeListView;
  private System.Windows.Forms.Label labelOutputDirectory;
  private TextBox textBoxDestinationDirectory;
  private Button buttonBrowseDirectory;
  private TableLayoutPanel tableLayoutPanel2;
  private TableLayoutPanel tableLayoutPanel3;
  private Button buttonClose;
  private Button buttonStartExtraction;

  public UserPanel()
  {
    this.InitializeComponent();
    this.textBoxDestinationDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
  }

  private void LogMessage(string message)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, message);
  }

  public virtual void OnChannelsChanged()
  {
    Channel channel = this.GetChannel("VRDU02T", (CustomPanel.ChannelLookupOptions) 1) ?? this.GetChannel("VRDU01T", (CustomPanel.ChannelLookupOptions) 1);
    if (this.vrdu != channel)
    {
      if (this.vrdu != null)
        this.vrdu.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.vrdu = channel;
      if (this.vrdu != null)
        this.vrdu.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    this.UpdateUI();
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    this.buttonStartExtraction.Enabled = this.vrdu != null && !this.extractionInProgress && this.vrdu.CommunicationsState == CommunicationsState.Online;
    this.buttonBrowseDirectory.Enabled = this.vrdu != null && !this.extractionInProgress && this.vrdu.CommunicationsState == CommunicationsState.Online;
    this.buttonClose.Enabled = this.vrdu == null || !this.extractionInProgress;
    this.textBoxDestinationDirectory.Enabled = this.vrdu != null && !this.extractionInProgress && this.vrdu.CommunicationsState == CommunicationsState.Online;
  }

  private Tuple<string, Decimal?> Convert(
    string dataString,
    string type,
    int position,
    int length,
    int offset,
    Decimal scalingFactor,
    int precision)
  {
    if (length > 16 /*0x10*/)
      throw new InvalidDataException($"Data to long:{length}");
    if (dataString.Length < position * 2 + length * 2)
      return (Tuple<string, Decimal?>) null;
    string str = dataString.Substring(position * 2, length * 2);
    Decimal? nullable1;
    Decimal? nullable2;
    switch (type)
    {
      case "bit":
        nullable1 = new Decimal?((Decimal) System.Convert.ToUInt32(str, 16 /*0x10*/));
        nullable1 = new Decimal?((Decimal) (((uint) nullable1.Value & (uint) scalingFactor) > 0U ? 1 : 0));
        break;
      case "uint":
        if (str != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF".Substring(0, length * 2))
        {
          nullable2 = new Decimal?((Decimal) System.Convert.ToUInt32(str, 16 /*0x10*/));
          Decimal num1 = scalingFactor;
          nullable2 = nullable2.HasValue ? new Decimal?(nullable2.GetValueOrDefault() * num1) : new Decimal?();
          Decimal num2 = (Decimal) offset;
          nullable1 = nullable2.HasValue ? new Decimal?(nullable2.GetValueOrDefault() + num2) : new Decimal?();
          break;
        }
        nullable1 = new Decimal?();
        break;
      case "sint":
        if (str != "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF".Substring(0, length * 2))
        {
          Decimal? nullable3;
          switch (length)
          {
            case 1:
              nullable3 = new Decimal?((Decimal) System.Convert.ToSByte(str, 16 /*0x10*/));
              break;
            case 2:
              nullable3 = new Decimal?((Decimal) System.Convert.ToInt16(str, 16 /*0x10*/));
              break;
            default:
              nullable3 = new Decimal?((Decimal) System.Convert.ToInt32(str, 16 /*0x10*/));
              break;
          }
          nullable2 = nullable3;
          Decimal num3 = scalingFactor;
          nullable2 = nullable2.HasValue ? new Decimal?(nullable2.GetValueOrDefault() * num3) : new Decimal?();
          Decimal num4 = (Decimal) offset;
          nullable1 = nullable2.HasValue ? new Decimal?(nullable2.GetValueOrDefault() + num4) : new Decimal?();
          break;
        }
        nullable1 = new Decimal?();
        break;
      default:
        throw new InvalidDataException($"Unknown data type: {type}");
    }
    if (nullable1.HasValue)
    {
      nullable1 = new Decimal?(Math.Round(nullable1.Value, precision));
      nullable2 = nullable1;
      Decimal num = Math.Truncate(nullable1.Value);
      if ((!(nullable2.GetValueOrDefault() == num) ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
        nullable1 = new Decimal?(Math.Truncate(nullable1.Value));
    }
    return new Tuple<string, Decimal?>(str, nullable1);
  }

  private XElement GenerateAbaEventData(XDocument grammar)
  {
    this.statusCounter = 0;
    XElement abaEventData = new XElement((XName) "AbaEventData");
    for (int index = 0; index < 10; ++index)
    {
      Application.DoEvents();
      string str = (string) grammar.Descendants((XName) "AbaEvent").First<XElement>().Attribute((XName) "RequestMessage");
      string qualifierName = string.Format("DT_STO_ABA_Function_Data_{0}_ABA_Function_Data_{0}", (object) index);
      string requestMessageData = this.GetRequestMessageData(qualifierName, str + (index + 1).ToString("X1"));
      XElement content = !string.IsNullOrEmpty(requestMessageData) ? this.LoadAbaEventData(requestMessageData, grammar) : throw new InvalidDataException($"Vrdu does not contain qualifier {qualifierName}");
      content.Add((object) new XAttribute((XName) "Number", (object) index));
      abaEventData.Add((object) content);
    }
    return abaEventData;
  }

  private XElement LoadAbaEventData(string data, XDocument grammar)
  {
    XDocument xdocument = new XDocument(new object[1]
    {
      (object) grammar.Descendants((XName) "AbaEvent").First<XElement>()
    });
    foreach (XElement descendant in xdocument.Descendants((XName) "AbaEventDataItem"))
    {
      Tuple<string, Decimal?> tuple = this.Convert(data, (string) descendant.Attribute((XName) "Type"), int.Parse((string) descendant.Attribute((XName) "Position")), int.Parse((string) descendant.Attribute((XName) "Length")), int.Parse((string) descendant.Attribute((XName) "Offset")), Decimal.Parse((string) descendant.Attribute((XName) "Factor")), string.IsNullOrEmpty((string) descendant.Attribute((XName) "Precision")) ? 2 : (int) descendant.Attribute((XName) "Precision"));
      if (tuple != null)
      {
        descendant.Add((object) new XElement((XName) "RawValue", (object) tuple.Item1));
        string specialType;
        if (string.IsNullOrEmpty(specialType = (string) descendant.Attribute((XName) "Special")))
        {
          Decimal? nullable = tuple.Item2;
          if (nullable.HasValue)
          {
            XElement xelement = descendant;
            XName name = (XName) "Value";
            nullable = tuple.Item2;
            string content1 = nullable.ToString();
            XElement content2 = new XElement(name, (object) content1);
            xelement.Add((object) content2);
          }
          else
            descendant.Add((object) new XElement((XName) "Value", (object) "NaN"));
        }
        else
          descendant.Add((object) new XElement((XName) "Value", (object) this.ValueToSpecial(specialType, tuple.Item1)));
      }
      else
      {
        descendant.Add((object) new XElement((XName) "RawValue", (object) "NaN"));
        descendant.Add((object) new XElement((XName) "Value", (object) "NaN"));
      }
    }
    return xdocument.Descendants((XName) "AbaEvent").First<XElement>();
  }

  private string ValueToSpecial(string specialType, string rawValue)
  {
    string special = string.Empty;
    uint uint32 = System.Convert.ToUInt32(rawValue, 16 /*0x10*/);
    if (specialType.Equals("DrvActOccured", StringComparison.InvariantCultureIgnoreCase))
    {
      special = "No";
      if (uint32 > 0U)
        special = "Yes";
    }
    else
    {
      if (!specialType.Equals("Warning", StringComparison.InvariantCultureIgnoreCase))
        throw new InvalidDataException($"Unknown special value {specialType}");
      if (uint32 < 161U)
      {
        uint num1 = (System.Convert.ToUInt32(rawValue, 16 /*0x10*/) & 224U /*0xE0*/) >> 5;
        uint num2 = (System.Convert.ToUInt32(rawValue, 16 /*0x10*/) & 28U) >> 2;
        if (num1 > 1U && num1 < 5U)
        {
          string[] strArray = new string[6]
          {
            "Optic Crash Warn",
            "Audio Crash Warn",
            "Optic ABA",
            "Audio ABA",
            "Haptic ABA",
            "Emergency Brake"
          };
          if (num2 > 0U && num2 < 7U)
            special = $"{(object) num1}_{strArray[(IntPtr) (num2 - 1U)]}";
        }
      }
    }
    return special;
  }

  private bool UnlockEcu(Channel ecu)
  {
    try
    {
      bool flag = false;
      this.LogMessage(Resources.Message_UnlockingVRDU);
      ByteMessage byteMessage = ecu.SendByteMessage(new Dump("270D"), true);
      if (byteMessage.Response.Data.Count == 4)
      {
        if (byteMessage.Response.Data[2] == (byte) 0 && byteMessage.Response.Data[3] == (byte) 0)
        {
          flag = true;
        }
        else
        {
          int num = (514 * (int) (ushort) (((uint) byteMessage.Response.Data[2] << 8) + (uint) byteMessage.Response.Data[3]) + 49818) % 65536 /*0x010000*/;
          if (ecu.SendByteMessage(new Dump("270E" + num.ToString("X4")), true).Response.ToString().Equals("670E", StringComparison.InvariantCultureIgnoreCase))
          {
            DateTime now = DateTime.Now;
            string str1 = System.Convert.ToUInt32(now.ToString("yy")).ToString("X2");
            now = DateTime.Now;
            string str2 = now.Month.ToString("X2");
            now = DateTime.Now;
            string str3 = now.Day.ToString("X2");
            string str4 = BitConverter.ToString(Encoding.Default.GetBytes(ApplicationInformation.ComputerId.Replace("-", "").Substring(0, 4))).Replace("-", "");
            if (ecu.SendByteMessage(new Dump($"2EF15C010004{str1}{str2}{str3}{str4}"), true).Response.ToString().Equals("6EF15C", StringComparison.InvariantCultureIgnoreCase))
              flag = true;
          }
        }
      }
      if (flag)
      {
        this.LogMessage(Resources.Message_VRDUUnlocked);
      }
      else
      {
        this.LogMessage(Resources.Message_CouldNotUnlockVRDU);
        this.extractionInProgress = false;
        this.UpdateUI();
      }
      return flag;
    }
    catch (CaesarException ex)
    {
      this.LogMessage(ex.Message);
      this.extractionInProgress = false;
      this.UpdateUI();
      return false;
    }
  }

  private string GetEcuInfo(string qualifierName)
  {
    Application.DoEvents();
    EcuInfo ecuInfo = (EcuInfo) null;
    if (this.vrdu == null || (ecuInfo = this.vrdu.EcuInfos[qualifierName]) == null)
      return (string) null;
    UserPanel userPanel = this;
    userPanel.rawData = $"{userPanel.rawData}{qualifierName},{this.vrdu.EcuInfos[qualifierName].Value}{Environment.NewLine}";
    return this.vrdu.EcuInfos[qualifierName].Value;
  }

  private string GetRequestMessageData(string qualifierName, string requestMessage)
  {
    if (this.vrdu != null)
    {
      Application.DoEvents();
      this.vrdu.FaultCodes.AutoRead = false;
      ByteMessage byteMessage = this.vrdu.SendByteMessage(new Dump(requestMessage), true);
      UserPanel userPanel = this;
      userPanel.rawData = $"{userPanel.rawData}{(object) DateTime.Now},{qualifierName},{byteMessage.Response.ToString()}{Environment.NewLine}";
      if (byteMessage.Response.ToString() != "7f2271")
        return byteMessage.Response.ToString().Substring(requestMessage.Length);
    }
    throw new InvalidDataException("Vrdu null");
  }

  private void buttonStartExtraction_Click(object sender, EventArgs e)
  {
    if (this.vrdu == null || this.vrdu.CommunicationsState != CommunicationsState.Online)
    {
      this.LogMessage(Resources.Message_VRDUNotReady);
      this.extractionInProgress = false;
      this.UpdateUI();
    }
    else
    {
      this.extractionInProgress = true;
      this.UpdateUI();
      this.LogMessage(Resources.Message_StartingVRDUExtraction);
      if (this.UnlockEcu(this.vrdu))
      {
        this.LogMessage(Resources.Message_RefreshingData);
        this.LogMessage(Resources.Message_HoldOnThisWillTakeAbout60Seconds);
        this.ExtractVrduData();
      }
    }
  }

  private void ExtractVrduData()
  {
    try
    {
      this.rawData = string.Empty;
      XDocument grammar = XDocument.Parse(this.textBoxGrammar.Text);
      if (this.vrdu != null)
      {
        XElement xelement = new XElement((XName) "VrduData");
        this.LogMessage(Resources.Message_ExtractingDiagnosticLinkQualifiers);
        xelement.Add((object) this.PopulateDiagnosticLinkQualifiers(grammar));
        this.LogMessage(Resources.Message_ExtractingVRDUQualifiers);
        xelement.Add((object) this.PopulateVrduGrammarQualifiers(grammar));
        this.LogMessage(Resources.Message_ExtractingABAData);
        xelement.Add((object) this.PopulateAbaSheets(grammar));
        string str = Path.Combine(this.textBoxDestinationDirectory.Text, $"{this.vrdu.Ecu.Name}_{this.GetEcuInfo("CO_EcuSerialNumber")}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.vrdureport");
        this.LogMessage(string.Format(Resources.MessageFormat_WritingDataFile, (object) str));
        FileEncryptionProvider.WriteEncryptedFile(Encoding.ASCII.GetBytes(xelement.ToString()), str, (EncryptionType) 1);
        this.LogMessage(Resources.Message_ProcessingComplete);
        this.extractionInProgress = false;
        this.UpdateUI();
      }
    }
    catch (Exception ex)
    {
      int num;
      switch (ex)
      {
        case InvalidDataException _:
        case ArgumentOutOfRangeException _:
        case PathTooLongException _:
        case DirectoryNotFoundException _:
        case UnauthorizedAccessException _:
        case NotSupportedException _:
          num = 0;
          break;
        default:
          num = !(ex is CaesarException) ? 1 : 0;
          break;
      }
      if (num == 0)
      {
        this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, ex.Message);
        this.extractionInProgress = false;
        this.UpdateUI();
      }
      else
        throw;
    }
    finally
    {
      if (this.vrdu != null)
      {
        this.vrdu.FaultCodes.AutoRead = true;
        this.vrdu.Services.Execute("FN_HardReset", false);
      }
    }
  }

  private XElement PopulateDiagnosticLinkQualifiers(XDocument grammar)
  {
    XElement xelement = new XElement((XName) "DiagnosticLinkQualifiers");
    foreach (XElement descendant in grammar.Root.Descendants((XName) "DiagnosticLinkQualifiers").Descendants<XElement>((XName) "Qualifier"))
    {
      string qualifierName = (string) descendant.Attribute((XName) "Name");
      string str = (string) descendant.Attribute((XName) "Device");
      if (string.IsNullOrEmpty(str) || str == this.vrdu.Ecu.Name)
      {
        string ecuInfo = this.GetEcuInfo(qualifierName);
        XElement content = new XElement(descendant);
        if (!string.IsNullOrEmpty(ecuInfo))
          content.Add((object) new XElement((XName) "Value", (object) ecuInfo));
        else
          content.Add((object) new XElement((XName) "Value", (object) "N/A"));
        xelement.Add((object) content);
      }
    }
    return xelement;
  }

  private XElement PopulateVrduGrammarQualifiers(XDocument grammar)
  {
    this.statusCounter = 0;
    XElement xelement = new XElement((XName) "VrduGrammar");
    foreach (XElement descendant in grammar.Root.Descendants((XName) "VrduGrammar").Descendants<XElement>((XName) "Qualifier"))
    {
      if (++this.statusCounter > 10)
      {
        this.statusCounter = 0;
        this.LogMessage(Resources.Message_Working);
      }
      string qualifierName = (string) descendant.Attribute((XName) "Name");
      string str = (string) descendant.Attribute((XName) "Device");
      string requestMessage = (string) descendant.Attribute((XName) "RequestMessage");
      int position = string.IsNullOrEmpty((string) descendant.Attribute((XName) "Position")) ? 0 : (int) descendant.Attribute((XName) "Position");
      string type = string.IsNullOrEmpty((string) descendant.Attribute((XName) "Type")) ? "uint" : (string) descendant.Attribute((XName) "Type");
      int offset = string.IsNullOrEmpty((string) descendant.Attribute((XName) "Offset")) ? 0 : (int) descendant.Attribute((XName) "Offset");
      Decimal scalingFactor = string.IsNullOrEmpty((string) descendant.Attribute((XName) "Factor")) ? 1M : (Decimal) descendant.Attribute((XName) "Factor");
      int length = string.IsNullOrEmpty((string) descendant.Attribute((XName) "Length")) ? 2 : (int) descendant.Attribute((XName) "Length");
      int num1 = string.IsNullOrEmpty((string) descendant.Attribute((XName) "ValueCount")) ? 0 : (int) descendant.Attribute((XName) "ValueCount");
      int precision = string.IsNullOrEmpty((string) descendant.Attribute((XName) "Precision")) ? 2 : (int) descendant.Attribute((XName) "Precision");
      string dataString = (string) null;
      XElement content1 = new XElement(descendant);
      if ((string.IsNullOrEmpty(str) || str == this.vrdu.Ecu.Name) && !string.IsNullOrEmpty(dataString = this.GetRequestMessageData(qualifierName, requestMessage)))
      {
        int num2 = (position + num1 * length) * 2;
        if (num2 != dataString.Length)
          throw new InvalidDataException($"Invalid data length for {qualifierName}:{num2}:{dataString.Length}");
        for (int index = 0; index < num1; ++index)
        {
          Tuple<string, Decimal?> tuple = this.Convert(dataString, type, position, length, offset, scalingFactor, precision);
          position += length;
          XElement content2 = new XElement((XName) "Index");
          content2.Add((object) new XAttribute((XName) "Number", (object) index));
          if (tuple != null)
          {
            if (tuple.Item2.HasValue)
            {
              content2.Add((object) new XElement((XName) "RawValue", (object) tuple.Item1));
              content2.Add((object) new XElement((XName) "Value", (object) tuple.Item2));
            }
            else
              content2.Add((object) new XElement((XName) "Value", (object) "NaN"));
          }
          content1.Add((object) content2);
        }
      }
      else
      {
        for (int content3 = 0; content3 < num1; ++content3)
        {
          XElement content4 = new XElement((XName) "Index", (object) content3);
          content4.Add((object) new XElement((XName) "Value", (object) "N/A"));
          content1.Add((object) content4);
        }
      }
      xelement.Add((object) content1);
    }
    return xelement;
  }

  private XElement PopulateAbaSheets(XDocument grammar)
  {
    this.statusCounter = 0;
    if (grammar.Descendants((XName) "AbaEvent").Count<XElement>() != 1)
      throw new InvalidDataException(string.Format("Invalid grammar for AbaEvent"));
    int result = 0;
    string requestMessageData = this.GetRequestMessageData("DT_STO_ABA_Function_Counter_ABA_Function_Counter", (string) grammar.Descendants((XName) "Qualifier").Where<XElement>((Func<XElement, bool>) (t => (string) t.Attribute((XName) "Name") == "DT_STO_ABA_Function_Counter_ABA_Function_Counter")).First<XElement>().Attribute((XName) "RequestMessage"));
    XElement xelement = (XElement) null;
    if (!string.IsNullOrEmpty(requestMessageData))
    {
      if (!int.TryParse(requestMessageData, out result) || result > 10 || result < 0)
        throw new ArgumentOutOfRangeException($"abaFunctionCounter {result}");
      xelement = this.GenerateAbaEventData(grammar);
    }
    return xelement;
  }

  private void buttonBrowseDirectory_Click(object sender, EventArgs e)
  {
    using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
    {
      if (folderBrowserDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
        return;
      this.textBoxDestinationDirectory.Text = folderBrowserDialog.SelectedPath;
    }
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.extractionInProgress;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView = new SeekTimeListView();
    this.textBoxGrammar = new TextBox();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.labelOutputDirectory = new System.Windows.Forms.Label();
    this.buttonStartExtraction = new Button();
    this.textBoxDestinationDirectory = new TextBox();
    this.buttonBrowseDirectory = new Button();
    this.buttonClose = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnCount = 4;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 144f));
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40f));
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148f));
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBoxGrammar, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 3);
    ((Control) this.tableLayoutPanel1).Location = new Point(4, 4);
    ((Control) this.tableLayoutPanel1).Margin = new Padding(4);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).RowCount = 4;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 63f));
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 85f));
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 41f));
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
    ((Control) this.tableLayoutPanel1).Size = new Size(797, 455);
    ((Control) this.tableLayoutPanel1).TabIndex = 0;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 4);
    ((Control) this.seekTimeListView).Dock = DockStyle.Fill;
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Location = new Point(4, 67);
    ((Control) this.seekTimeListView).Margin = new Padding(4);
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "VRDU Snapshot";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    ((Control) this.seekTimeListView).Size = new Size(789, 343);
    ((Control) this.seekTimeListView).TabIndex = 52;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    this.textBoxGrammar.Enabled = false;
    this.textBoxGrammar.Location = new Point(148, 418);
    this.textBoxGrammar.Margin = new Padding(4);
    this.textBoxGrammar.Multiline = true;
    this.textBoxGrammar.Name = "textBoxGrammar";
    this.textBoxGrammar.Size = new Size(277, 32 /*0x20*/);
    this.textBoxGrammar.TabIndex = 12;
    this.textBoxGrammar.Text = componentResourceManager.GetString("textBoxGrammar.Text");
    this.textBoxGrammar.Visible = false;
    this.textBoxGrammar.WordWrap = false;
    ((TableLayoutPanel) this.tableLayoutPanel2).CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
    ((TableLayoutPanel) this.tableLayoutPanel2).ColumnCount = 1;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel3, 0, 0);
    ((Control) this.tableLayoutPanel2).Dock = DockStyle.Fill;
    ((Control) this.tableLayoutPanel2).Location = new Point(4, 4);
    ((Control) this.tableLayoutPanel2).Margin = new Padding(4);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    ((TableLayoutPanel) this.tableLayoutPanel2).RowCount = 1;
    ((TableLayoutPanel) this.tableLayoutPanel2).RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
    ((Control) this.tableLayoutPanel2).Size = new Size(789, 55);
    ((Control) this.tableLayoutPanel2).TabIndex = 56;
    ((TableLayoutPanel) this.tableLayoutPanel3).ColumnCount = 4;
    ((TableLayoutPanel) this.tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.55556f));
    ((TableLayoutPanel) this.tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78.44444f));
    ((TableLayoutPanel) this.tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40f));
    ((TableLayoutPanel) this.tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155f));
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelOutputDirectory, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.buttonStartExtraction, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.textBoxDestinationDirectory, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.buttonBrowseDirectory, 2, 0);
    ((Control) this.tableLayoutPanel3).Dock = DockStyle.Fill;
    ((Control) this.tableLayoutPanel3).Location = new Point(5, 5);
    ((Control) this.tableLayoutPanel3).Margin = new Padding(4);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanel3).RowCount = 1;
    ((TableLayoutPanel) this.tableLayoutPanel3).RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
    ((Control) this.tableLayoutPanel3).Size = new Size(779, 45);
    ((Control) this.tableLayoutPanel3).TabIndex = 0;
    this.labelOutputDirectory.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.labelOutputDirectory.Location = new Point(4, 13);
    this.labelOutputDirectory.Margin = new Padding(4, 0, 4, 0);
    this.labelOutputDirectory.Name = "labelOutputDirectory";
    this.labelOutputDirectory.Size = new Size(117, 32 /*0x20*/);
    this.labelOutputDirectory.TabIndex = 53;
    this.labelOutputDirectory.Text = "Output Directory:";
    this.labelOutputDirectory.TextAlign = ContentAlignment.MiddleRight;
    this.labelOutputDirectory.UseCompatibleTextRendering = true;
    this.buttonStartExtraction.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
    this.buttonStartExtraction.AutoSize = true;
    this.buttonStartExtraction.ImeMode = ImeMode.NoControl;
    this.buttonStartExtraction.Location = new Point(627, 4);
    this.buttonStartExtraction.Margin = new Padding(4);
    this.buttonStartExtraction.Name = "buttonStartExtraction";
    this.buttonStartExtraction.Size = new Size(132, 37);
    this.buttonStartExtraction.TabIndex = 14;
    this.buttonStartExtraction.Text = "Start Extraction";
    this.buttonStartExtraction.UseCompatibleTextRendering = true;
    this.buttonStartExtraction.UseVisualStyleBackColor = true;
    this.buttonStartExtraction.Click += new EventHandler(this.buttonStartExtraction_Click);
    this.textBoxDestinationDirectory.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.textBoxDestinationDirectory.Location = new Point(129, 19);
    this.textBoxDestinationDirectory.Margin = new Padding(4);
    this.textBoxDestinationDirectory.Name = "textBoxDestinationDirectory";
    this.textBoxDestinationDirectory.Size = new Size(450, 22);
    this.textBoxDestinationDirectory.TabIndex = 54;
    this.buttonBrowseDirectory.Anchor = AnchorStyles.Bottom;
    this.buttonBrowseDirectory.Location = new Point(588, 14);
    this.buttonBrowseDirectory.Margin = new Padding(4);
    this.buttonBrowseDirectory.Name = "buttonBrowseDirectory";
    this.buttonBrowseDirectory.Size = new Size(29, 27);
    this.buttonBrowseDirectory.TabIndex = 55;
    this.buttonBrowseDirectory.Text = "...";
    this.buttonBrowseDirectory.UseCompatibleTextRendering = true;
    this.buttonBrowseDirectory.UseVisualStyleBackColor = true;
    this.buttonBrowseDirectory.Click += new EventHandler(this.buttonBrowseDirectory_Click);
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Location = new Point(653, 418);
    this.buttonClose.Margin = new Padding(4);
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.Size = new Size(115, 33);
    this.buttonClose.TabIndex = 57;
    this.buttonClose.Text = "Close";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.AutoScaleDimensions = new SizeF(8f, 16f);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Margin = new Padding(7, 6, 7, 6);
    ((Control) this).MaximumSize = new Size(801, 466);
    ((Control) this).MinimumSize = new Size(801, 466);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this).Size = new Size(801, 466);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
