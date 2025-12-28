using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DetroitDiesel.Common;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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
		InitializeComponent();
		textBoxDestinationDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
	}

	private void LogMessage(string message)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, message);
	}

	public override void OnChannelsChanged()
	{
		Channel channel = ((CustomPanel)this).GetChannel("VRDU02T", (ChannelLookupOptions)1);
		if (channel == null)
		{
			channel = ((CustomPanel)this).GetChannel("VRDU01T", (ChannelLookupOptions)1);
		}
		if (vrdu != channel)
		{
			if (vrdu != null)
			{
				vrdu.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			vrdu = channel;
			if (vrdu != null)
			{
				vrdu.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
		UpdateUI();
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		buttonStartExtraction.Enabled = vrdu != null && !extractionInProgress && vrdu.CommunicationsState == CommunicationsState.Online;
		buttonBrowseDirectory.Enabled = vrdu != null && !extractionInProgress && vrdu.CommunicationsState == CommunicationsState.Online;
		buttonClose.Enabled = vrdu == null || !extractionInProgress;
		textBoxDestinationDirectory.Enabled = vrdu != null && !extractionInProgress && vrdu.CommunicationsState == CommunicationsState.Online;
	}

	private Tuple<string, decimal?> Convert(string dataString, string type, int position, int length, int offset, decimal scalingFactor, int precision)
	{
		if (length > 16)
		{
			throw new InvalidDataException($"Data to long:{length}");
		}
		if (dataString.Length >= position * 2 + length * 2)
		{
			string text = dataString.Substring(position * 2, length * 2);
			decimal? num;
			switch (type)
			{
			case "bit":
				num = System.Convert.ToUInt32(text, 16);
				num = ((((uint)num.Value & (uint)scalingFactor) != 0) ? 1 : 0);
				break;
			case "uint":
				num = ((!(text != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF".Substring(0, length * 2))) ? ((decimal?)null) : ((decimal?)System.Convert.ToUInt32(text, 16) * (decimal?)scalingFactor + (decimal?)offset));
				break;
			case "sint":
				num = ((!(text != "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF".Substring(0, length * 2))) ? ((decimal?)null) : (length switch
				{
					1 => System.Convert.ToSByte(text, 16), 
					2 => System.Convert.ToInt16(text, 16), 
					_ => System.Convert.ToInt32(text, 16), 
				} * (decimal?)scalingFactor + (decimal?)offset));
				break;
			default:
				throw new InvalidDataException($"Unknown data type: {type}");
			}
			if (num.HasValue)
			{
				num = Math.Round(num.Value, precision);
				decimal? num2 = num;
				decimal num3 = Math.Truncate(num.Value);
				if (num2.GetValueOrDefault() == num3 && num2.HasValue)
				{
					num = Math.Truncate(num.Value);
				}
			}
			return new Tuple<string, decimal?>(text, num);
		}
		return null;
	}

	private XElement GenerateAbaEventData(XDocument grammar)
	{
		statusCounter = 0;
		XElement xElement = new XElement("AbaEventData");
		for (int i = 0; i < 10; i++)
		{
			Application.DoEvents();
			string text = (string)grammar.Descendants("AbaEvent").First().Attribute("RequestMessage");
			string text2 = string.Format("DT_STO_ABA_Function_Data_{0}_ABA_Function_Data_{0}", i);
			string requestMessageData = GetRequestMessageData(text2, text + (i + 1).ToString("X1"));
			if (!string.IsNullOrEmpty(requestMessageData))
			{
				XElement xElement2 = LoadAbaEventData(requestMessageData, grammar);
				xElement2.Add(new XAttribute("Number", i));
				xElement.Add(xElement2);
				continue;
			}
			throw new InvalidDataException($"Vrdu does not contain qualifier {text2}");
		}
		return xElement;
	}

	private XElement LoadAbaEventData(string data, XDocument grammar)
	{
		XDocument xDocument = new XDocument(grammar.Descendants("AbaEvent").First());
		foreach (XElement item in xDocument.Descendants("AbaEventDataItem"))
		{
			Tuple<string, decimal?> tuple = Convert(data, (string)item.Attribute("Type"), int.Parse((string)item.Attribute("Position")), int.Parse((string)item.Attribute("Length")), int.Parse((string)item.Attribute("Offset")), decimal.Parse((string)item.Attribute("Factor")), string.IsNullOrEmpty((string)item.Attribute("Precision")) ? 2 : ((int)item.Attribute("Precision")));
			if (tuple != null)
			{
				item.Add(new XElement("RawValue", tuple.Item1));
				string specialType;
				if (string.IsNullOrEmpty(specialType = (string)item.Attribute("Special")))
				{
					if (tuple.Item2.HasValue)
					{
						item.Add(new XElement("Value", tuple.Item2.ToString()));
					}
					else
					{
						item.Add(new XElement("Value", "NaN"));
					}
				}
				else
				{
					item.Add(new XElement("Value", ValueToSpecial(specialType, tuple.Item1)));
				}
			}
			else
			{
				item.Add(new XElement("RawValue", "NaN"));
				item.Add(new XElement("Value", "NaN"));
			}
		}
		return xDocument.Descendants("AbaEvent").First();
	}

	private string ValueToSpecial(string specialType, string rawValue)
	{
		string result = string.Empty;
		uint num = System.Convert.ToUInt32(rawValue, 16);
		if (specialType.Equals("DrvActOccured", StringComparison.InvariantCultureIgnoreCase))
		{
			result = "No";
			if (num != 0)
			{
				result = "Yes";
			}
		}
		else
		{
			if (!specialType.Equals("Warning", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new InvalidDataException($"Unknown special value {specialType}");
			}
			if (num < 161)
			{
				uint num2 = (System.Convert.ToUInt32(rawValue, 16) & 0xE0) >> 5;
				uint num3 = (System.Convert.ToUInt32(rawValue, 16) & 0x1C) >> 2;
				if (num2 > 1 && num2 < 5)
				{
					string[] array = new string[6] { "Optic Crash Warn", "Audio Crash Warn", "Optic ABA", "Audio ABA", "Haptic ABA", "Emergency Brake" };
					if (num3 != 0 && num3 < 7)
					{
						result = num2 + "_" + array[num3 - 1];
					}
				}
			}
		}
		return result;
	}

	private bool UnlockEcu(Channel ecu)
	{
		try
		{
			bool flag = false;
			LogMessage(Resources.Message_UnlockingVRDU);
			ByteMessage byteMessage = ecu.SendByteMessage(new Dump("270D"), synchronous: true);
			if (byteMessage.Response.Data.Count == 4)
			{
				if (byteMessage.Response.Data[2] == 0 && byteMessage.Response.Data[3] == 0)
				{
					flag = true;
				}
				else
				{
					ushort num = (ushort)((byteMessage.Response.Data[2] << 8) + byteMessage.Response.Data[3]);
					ByteMessage byteMessage2 = ecu.SendByteMessage(new Dump("270E" + ((514 * num + 49818) % 65536).ToString("X4")), synchronous: true);
					if (byteMessage2.Response.ToString().Equals("670E", StringComparison.InvariantCultureIgnoreCase))
					{
						string text = System.Convert.ToUInt32(DateTime.Now.ToString("yy")).ToString("X2");
						string text2 = DateTime.Now.Month.ToString("X2");
						string text3 = DateTime.Now.Day.ToString("X2");
						string text4 = BitConverter.ToString(Encoding.Default.GetBytes(ApplicationInformation.ComputerId.Replace("-", "").Substring(0, 4))).Replace("-", "");
						ByteMessage byteMessage3 = ecu.SendByteMessage(new Dump("2EF15C010004" + text + text2 + text3 + text4), synchronous: true);
						if (byteMessage3.Response.ToString().Equals("6EF15C", StringComparison.InvariantCultureIgnoreCase))
						{
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				LogMessage(Resources.Message_VRDUUnlocked);
			}
			else
			{
				LogMessage(Resources.Message_CouldNotUnlockVRDU);
				extractionInProgress = false;
				UpdateUI();
			}
			return flag;
		}
		catch (CaesarException ex)
		{
			LogMessage(ex.Message);
			extractionInProgress = false;
			UpdateUI();
			return false;
		}
	}

	private string GetEcuInfo(string qualifierName)
	{
		Application.DoEvents();
		EcuInfo ecuInfo = null;
		if (vrdu != null && (ecuInfo = vrdu.EcuInfos[qualifierName]) != null)
		{
			string text = rawData;
			rawData = text + qualifierName + "," + vrdu.EcuInfos[qualifierName].Value + Environment.NewLine;
			return vrdu.EcuInfos[qualifierName].Value;
		}
		return null;
	}

	private string GetRequestMessageData(string qualifierName, string requestMessage)
	{
		if (vrdu != null)
		{
			Application.DoEvents();
			vrdu.FaultCodes.AutoRead = false;
			ByteMessage byteMessage = vrdu.SendByteMessage(new Dump(requestMessage), synchronous: true);
			object obj = rawData;
			rawData = string.Concat(obj, DateTime.Now, ",", qualifierName, ",", byteMessage.Response.ToString(), Environment.NewLine);
			if (byteMessage.Response.ToString() != "7f2271")
			{
				return byteMessage.Response.ToString().Substring(requestMessage.Length);
			}
		}
		throw new InvalidDataException("Vrdu null");
	}

	private void buttonStartExtraction_Click(object sender, EventArgs e)
	{
		if (vrdu == null || vrdu.CommunicationsState != CommunicationsState.Online)
		{
			LogMessage(Resources.Message_VRDUNotReady);
			extractionInProgress = false;
			UpdateUI();
			return;
		}
		extractionInProgress = true;
		UpdateUI();
		LogMessage(Resources.Message_StartingVRDUExtraction);
		if (UnlockEcu(vrdu))
		{
			LogMessage(Resources.Message_RefreshingData);
			LogMessage(Resources.Message_HoldOnThisWillTakeAbout60Seconds);
			ExtractVrduData();
		}
	}

	private void ExtractVrduData()
	{
		try
		{
			rawData = string.Empty;
			XDocument grammar = XDocument.Parse(textBoxGrammar.Text);
			if (vrdu != null)
			{
				XElement xElement = new XElement("VrduData");
				LogMessage(Resources.Message_ExtractingDiagnosticLinkQualifiers);
				xElement.Add(PopulateDiagnosticLinkQualifiers(grammar));
				LogMessage(Resources.Message_ExtractingVRDUQualifiers);
				xElement.Add(PopulateVrduGrammarQualifiers(grammar));
				LogMessage(Resources.Message_ExtractingABAData);
				xElement.Add(PopulateAbaSheets(grammar));
				string path = vrdu.Ecu.Name + "_" + GetEcuInfo("CO_EcuSerialNumber") + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".vrdureport";
				path = Path.Combine(textBoxDestinationDirectory.Text, path);
				LogMessage(string.Format(Resources.MessageFormat_WritingDataFile, path));
				FileEncryptionProvider.WriteEncryptedFile(Encoding.ASCII.GetBytes(xElement.ToString()), path, (EncryptionType)1);
				LogMessage(Resources.Message_ProcessingComplete);
				extractionInProgress = false;
				UpdateUI();
			}
		}
		catch (Exception ex)
		{
			if (ex is InvalidDataException || ex is ArgumentOutOfRangeException || ex is PathTooLongException || ex is DirectoryNotFoundException || ex is UnauthorizedAccessException || ex is NotSupportedException || ex is CaesarException)
			{
				((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, ex.Message);
				extractionInProgress = false;
				UpdateUI();
				return;
			}
			throw;
		}
		finally
		{
			if (vrdu != null)
			{
				vrdu.FaultCodes.AutoRead = true;
				vrdu.Services.Execute("FN_HardReset", synchronous: false);
			}
		}
	}

	private XElement PopulateDiagnosticLinkQualifiers(XDocument grammar)
	{
		XElement xElement = new XElement("DiagnosticLinkQualifiers");
		foreach (XElement item in grammar.Root.Descendants("DiagnosticLinkQualifiers").Descendants("Qualifier"))
		{
			string qualifierName = (string)item.Attribute("Name");
			string text = (string)item.Attribute("Device");
			if (string.IsNullOrEmpty(text) || text == vrdu.Ecu.Name)
			{
				string ecuInfo = GetEcuInfo(qualifierName);
				XElement xElement2 = new XElement(item);
				if (!string.IsNullOrEmpty(ecuInfo))
				{
					xElement2.Add(new XElement("Value", ecuInfo));
				}
				else
				{
					xElement2.Add(new XElement("Value", "N/A"));
				}
				xElement.Add(xElement2);
			}
		}
		return xElement;
	}

	private XElement PopulateVrduGrammarQualifiers(XDocument grammar)
	{
		statusCounter = 0;
		XElement xElement = new XElement("VrduGrammar");
		foreach (XElement item in grammar.Root.Descendants("VrduGrammar").Descendants("Qualifier"))
		{
			if (++statusCounter > 10)
			{
				statusCounter = 0;
				LogMessage(Resources.Message_Working);
			}
			string text = (string)item.Attribute("Name");
			string text2 = (string)item.Attribute("Device");
			string requestMessage = (string)item.Attribute("RequestMessage");
			int num = ((!string.IsNullOrEmpty((string)item.Attribute("Position"))) ? ((int)item.Attribute("Position")) : 0);
			string type = (string.IsNullOrEmpty((string)item.Attribute("Type")) ? "uint" : ((string)item.Attribute("Type")));
			int offset = ((!string.IsNullOrEmpty((string)item.Attribute("Offset"))) ? ((int)item.Attribute("Offset")) : 0);
			decimal scalingFactor = (string.IsNullOrEmpty((string)item.Attribute("Factor")) ? 1m : ((decimal)item.Attribute("Factor")));
			int num2 = (string.IsNullOrEmpty((string)item.Attribute("Length")) ? 2 : ((int)item.Attribute("Length")));
			int num3 = ((!string.IsNullOrEmpty((string)item.Attribute("ValueCount"))) ? ((int)item.Attribute("ValueCount")) : 0);
			int precision = (string.IsNullOrEmpty((string)item.Attribute("Precision")) ? 2 : ((int)item.Attribute("Precision")));
			string text3 = null;
			XElement xElement2 = new XElement(item);
			if ((string.IsNullOrEmpty(text2) || text2 == vrdu.Ecu.Name) && !string.IsNullOrEmpty(text3 = GetRequestMessageData(text, requestMessage)))
			{
				int num4 = (num + num3 * num2) * 2;
				if (num4 != text3.Length)
				{
					throw new InvalidDataException($"Invalid data length for {text}:{num4}:{text3.Length}");
				}
				for (int i = 0; i < num3; i++)
				{
					Tuple<string, decimal?> tuple = Convert(text3, type, num, num2, offset, scalingFactor, precision);
					num += num2;
					XElement xElement3 = new XElement("Index");
					xElement3.Add(new XAttribute("Number", i));
					if (tuple != null)
					{
						if (tuple.Item2.HasValue)
						{
							xElement3.Add(new XElement("RawValue", tuple.Item1));
							xElement3.Add(new XElement("Value", tuple.Item2));
						}
						else
						{
							xElement3.Add(new XElement("Value", "NaN"));
						}
					}
					xElement2.Add(xElement3);
				}
			}
			else
			{
				for (int i = 0; i < num3; i++)
				{
					XElement xElement3 = new XElement("Index", i);
					xElement3.Add(new XElement("Value", "N/A"));
					xElement2.Add(xElement3);
				}
			}
			xElement.Add(xElement2);
		}
		return xElement;
	}

	private XElement PopulateAbaSheets(XDocument grammar)
	{
		statusCounter = 0;
		if (grammar.Descendants("AbaEvent").Count() != 1)
		{
			throw new InvalidDataException($"Invalid grammar for AbaEvent");
		}
		int result = 0;
		string requestMessage = (string)(from t in grammar.Descendants("Qualifier")
			where (string)t.Attribute("Name") == "DT_STO_ABA_Function_Counter_ABA_Function_Counter"
			select t).First().Attribute("RequestMessage");
		string requestMessageData = GetRequestMessageData("DT_STO_ABA_Function_Counter_ABA_Function_Counter", requestMessage);
		XElement result2 = null;
		if (!string.IsNullOrEmpty(requestMessageData))
		{
			if (!int.TryParse(requestMessageData, out result) || result > 10 || result < 0)
			{
				throw new ArgumentOutOfRangeException($"abaFunctionCounter {result}");
			}
			result2 = GenerateAbaEventData(grammar);
		}
		return result2;
	}

	private void buttonBrowseDirectory_Click(object sender, EventArgs e)
	{
		using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		DialogResult dialogResult = folderBrowserDialog.ShowDialog();
		if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
		{
			textBoxDestinationDirectory.Text = folderBrowserDialog.SelectedPath;
		}
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = extractionInProgress;
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		textBoxGrammar = new TextBox();
		tableLayoutPanel2 = new TableLayoutPanel();
		tableLayoutPanel3 = new TableLayoutPanel();
		labelOutputDirectory = new System.Windows.Forms.Label();
		buttonStartExtraction = new Button();
		textBoxDestinationDirectory = new TextBox();
		buttonBrowseDirectory = new Button();
		buttonClose = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnCount = 4;
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 144f));
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40f));
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148f));
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxGrammar, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 3);
		((Control)(object)tableLayoutPanel1).Location = new Point(4, 4);
		((Control)(object)tableLayoutPanel1).Margin = new Padding(4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).RowCount = 4;
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 63f));
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 85f));
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 41f));
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
		((Control)(object)tableLayoutPanel1).Size = new Size(797, 455);
		((Control)(object)tableLayoutPanel1).TabIndex = 0;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 4);
		((Control)(object)seekTimeListView).Dock = DockStyle.Fill;
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Location = new Point(4, 67);
		((Control)(object)seekTimeListView).Margin = new Padding(4);
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "VRDU Snapshot";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		((Control)(object)seekTimeListView).Size = new Size(789, 343);
		((Control)(object)seekTimeListView).TabIndex = 52;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		textBoxGrammar.Enabled = false;
		textBoxGrammar.Location = new Point(148, 418);
		textBoxGrammar.Margin = new Padding(4);
		textBoxGrammar.Multiline = true;
		textBoxGrammar.Name = "textBoxGrammar";
		textBoxGrammar.Size = new Size(277, 32);
		textBoxGrammar.TabIndex = 12;
		textBoxGrammar.Text = componentResourceManager.GetString("textBoxGrammar.Text");
		textBoxGrammar.Visible = false;
		textBoxGrammar.WordWrap = false;
		((TableLayoutPanel)(object)tableLayoutPanel2).CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
		((TableLayoutPanel)(object)tableLayoutPanel2).ColumnCount = 1;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel3, 0, 0);
		((Control)(object)tableLayoutPanel2).Dock = DockStyle.Fill;
		((Control)(object)tableLayoutPanel2).Location = new Point(4, 4);
		((Control)(object)tableLayoutPanel2).Margin = new Padding(4);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		((TableLayoutPanel)(object)tableLayoutPanel2).RowCount = 1;
		((TableLayoutPanel)(object)tableLayoutPanel2).RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
		((Control)(object)tableLayoutPanel2).Size = new Size(789, 55);
		((Control)(object)tableLayoutPanel2).TabIndex = 56;
		((TableLayoutPanel)(object)tableLayoutPanel3).ColumnCount = 4;
		((TableLayoutPanel)(object)tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.55556f));
		((TableLayoutPanel)(object)tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78.44444f));
		((TableLayoutPanel)(object)tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40f));
		((TableLayoutPanel)(object)tableLayoutPanel3).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155f));
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(labelOutputDirectory, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(buttonStartExtraction, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(textBoxDestinationDirectory, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(buttonBrowseDirectory, 2, 0);
		((Control)(object)tableLayoutPanel3).Dock = DockStyle.Fill;
		((Control)(object)tableLayoutPanel3).Location = new Point(5, 5);
		((Control)(object)tableLayoutPanel3).Margin = new Padding(4);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel3).RowCount = 1;
		((TableLayoutPanel)(object)tableLayoutPanel3).RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
		((Control)(object)tableLayoutPanel3).Size = new Size(779, 45);
		((Control)(object)tableLayoutPanel3).TabIndex = 0;
		labelOutputDirectory.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		labelOutputDirectory.Location = new Point(4, 13);
		labelOutputDirectory.Margin = new Padding(4, 0, 4, 0);
		labelOutputDirectory.Name = "labelOutputDirectory";
		labelOutputDirectory.Size = new Size(117, 32);
		labelOutputDirectory.TabIndex = 53;
		labelOutputDirectory.Text = "Output Directory:";
		labelOutputDirectory.TextAlign = ContentAlignment.MiddleRight;
		labelOutputDirectory.UseCompatibleTextRendering = true;
		buttonStartExtraction.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		buttonStartExtraction.AutoSize = true;
		buttonStartExtraction.ImeMode = ImeMode.NoControl;
		buttonStartExtraction.Location = new Point(627, 4);
		buttonStartExtraction.Margin = new Padding(4);
		buttonStartExtraction.Name = "buttonStartExtraction";
		buttonStartExtraction.Size = new Size(132, 37);
		buttonStartExtraction.TabIndex = 14;
		buttonStartExtraction.Text = "Start Extraction";
		buttonStartExtraction.UseCompatibleTextRendering = true;
		buttonStartExtraction.UseVisualStyleBackColor = true;
		buttonStartExtraction.Click += buttonStartExtraction_Click;
		textBoxDestinationDirectory.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		textBoxDestinationDirectory.Location = new Point(129, 19);
		textBoxDestinationDirectory.Margin = new Padding(4);
		textBoxDestinationDirectory.Name = "textBoxDestinationDirectory";
		textBoxDestinationDirectory.Size = new Size(450, 22);
		textBoxDestinationDirectory.TabIndex = 54;
		buttonBrowseDirectory.Anchor = AnchorStyles.Bottom;
		buttonBrowseDirectory.Location = new Point(588, 14);
		buttonBrowseDirectory.Margin = new Padding(4);
		buttonBrowseDirectory.Name = "buttonBrowseDirectory";
		buttonBrowseDirectory.Size = new Size(29, 27);
		buttonBrowseDirectory.TabIndex = 55;
		buttonBrowseDirectory.Text = "...";
		buttonBrowseDirectory.UseCompatibleTextRendering = true;
		buttonBrowseDirectory.UseVisualStyleBackColor = true;
		buttonBrowseDirectory.Click += buttonBrowseDirectory_Click;
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Location = new Point(653, 418);
		buttonClose.Margin = new Padding(4);
		buttonClose.Name = "buttonClose";
		buttonClose.Size = new Size(115, 33);
		buttonClose.TabIndex = 57;
		buttonClose.Text = "Close";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((CustomPanel)this).AutoScaleDimensions = new SizeF(8f, 16f);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Margin = new Padding(7, 6, 7, 6);
		((Control)(object)this).MaximumSize = new Size(801, 466);
		((Control)(object)this).MinimumSize = new Size(801, 466);
		((Control)this).Name = "UserPanel";
		((Control)this).Size = new Size(801, 466);
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
