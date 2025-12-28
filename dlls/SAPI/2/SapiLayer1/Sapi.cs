using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using CaesarAbstraction;
using J2534;
using McdAbstraction;
using Microsoft.CSharp;
using Microsoft.Win32;

namespace SapiLayer1;

public sealed class Sapi : IDisposable
{
	private class XmlEmbeddedResourceResolver : XmlUrlResolver
	{
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri.Scheme == "file")
			{
				string resourceSchema = GetResourceSchema(Path.GetFileName(absoluteUri.LocalPath));
				if (resourceSchema == null)
				{
					throw new FileNotFoundException(absoluteUri.LocalPath);
				}
				return new MemoryStream(Encoding.UTF8.GetBytes(resourceSchema));
			}
			return base.GetEntity(absoluteUri, role, ofObjectToReturn);
		}
	}

	private int readAccess;

	private int writeAccess;

	private bool allowAutoBaudRate = true;

	private Func<Ecu, string, Stream> getConfiguration;

	private Action<Ecu, Stream, XmlNode> releaseConfiguration;

	private static TimeSpan? utcOffset;

	private bool useMcd = true;

	private int? mcdProcedureId;

	private Func<byte[], byte[]> mcdKeyFunc;

	private static Sapi sapi;

	private InitState initState;

	private int hardwareAccessLevel;

	private ConfigurationItemCollection configurationItems;

	private EcuCollection ecus;

	private DiagnosisProtocolCollection protocols;

	private ChannelCollection channels;

	private LogFileCollection logFiles;

	private CodingFileCollection codingFiles;

	private FlashFileCollection flashFiles;

	private BusMonitorCollection busMonitors;

	private object extensionAssemblyLock = new object();

	private bool haveAttemptedExtensionAssemblyLoad;

	private bool haveAttemptedFileSystemExtensionAssemblyLoad;

	private object flashFilesLock = new object();

	private bool flashFilesLoaded;

	private static bool disposed;

	private Control threadMarshalControl;

	private string caesarLibraryVersion;

	private string caesarLibraryCompilationDate;

	private IList<byte> toolId;

	private Assembly sapiExtensionAssembly;

	private Assembly sapiFileSystemExtensionAssembly;

	internal Control ThreadMarshalControl => threadMarshalControl;

	internal Assembly SapiExtensionAssembly
	{
		get
		{
			lock (extensionAssemblyLock)
			{
				if (!haveAttemptedExtensionAssemblyLoad && sapiExtensionAssembly == null)
				{
					string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sapiextension.dll");
					if (File.Exists(path))
					{
						RaiseDebugInfoEvent(this, "Loading extension assembly");
						sapiExtensionAssembly = Assembly.LoadFile(path);
					}
					haveAttemptedExtensionAssemblyLoad = true;
				}
				return sapiExtensionAssembly;
			}
		}
	}

	internal Assembly SapiFileSystemExtensionAssembly
	{
		get
		{
			lock (extensionAssemblyLock)
			{
				if (!haveAttemptedFileSystemExtensionAssemblyLoad && sapiFileSystemExtensionAssembly == null)
				{
					List<Ecu> source = Ecus.Where((Ecu e) => e.ConfigurationLoadedFromFile && e.ExtensionSource != null).ToList();
					if (source.Any())
					{
						RaiseDebugInfoEvent(this, "Loading extension assembly for file-system sources " + string.Join(", ", source.Select((Ecu e) => e.DiagnosisSource.ToString() + "." + e.Name)));
						sapiFileSystemExtensionAssembly = BuildExtensions(source.Select((Ecu e) => e.ExtensionSource));
					}
					haveAttemptedFileSystemExtensionAssemblyLoad = true;
				}
				return sapiFileSystemExtensionAssembly;
			}
		}
	}

	internal IEnumerable<byte> ToolId => toolId;

	public InitState InitState => initState;

	public int ReadAccess
	{
		get
		{
			return readAccess;
		}
		set
		{
			CaesarRoot.CheckClientStrongName();
			readAccess = value;
		}
	}

	public int WriteAccess
	{
		get
		{
			return writeAccess;
		}
		set
		{
			CaesarRoot.CheckClientStrongName();
			writeAccess = value;
		}
	}

	public ConfigurationItemCollection ConfigurationItems => configurationItems;

	public EcuCollection Ecus => ecus;

	public DiagnosisProtocolCollection DiagnosisProtocols => protocols;

	public ChannelCollection Channels => channels;

	public LogFileCollection LogFiles => logFiles;

	public BusMonitorCollection BusMonitors => busMonitors;

	public string CaesarLibraryVersion => caesarLibraryVersion;

	public string CaesarLibraryCompilationDate => caesarLibraryCompilationDate;

	public static string McdSystemDescription => McdRoot.Description;

	public static string McdDatabaseLocation => McdRoot.DatabaseLocation;

	public CodingFileCollection CodingFiles => codingFiles;

	public FlashFileCollection FlashFiles => flashFiles;

	public int HardwareAccess => hardwareAccessLevel;

	public bool ValidateConfigurationFileChecksums { get; set; }

	public bool AllowAutoBaudRate
	{
		get
		{
			return allowAutoBaudRate;
		}
		set
		{
			if (value != allowAutoBaudRate)
			{
				try
				{
					Sid.SetAllowAutoBaudRate(value);
					allowAutoBaudRate = value;
				}
				catch (SEHException)
				{
					GetSapi().RaiseDebugInfoEvent(this, "Unable to set 'allow auto baud rate' as SID is not loaded");
				}
			}
		}
	}

	internal Func<Ecu, string, Stream> GetConfiguration => getConfiguration;

	internal Action<Ecu, Stream, XmlNode> ReleaseConfiguration => releaseConfiguration;

	internal static DateTime Now
	{
		get
		{
			if (!utcOffset.HasValue)
			{
				utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
			}
			return DateTime.UtcNow + utcOffset.Value;
		}
	}

	public bool UseMcd
	{
		get
		{
			return useMcd;
		}
		set
		{
			useMcd = value;
		}
	}

	internal CultureInfo PresentationCulture { get; private set; }

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadAccessLevel is deprecated due to non-CLS compliance, please use ReadAccess instead.")]
	public ushort ReadAccessLevel
	{
		get
		{
			return (ushort)ReadAccess;
		}
		set
		{
			ReadAccess = value;
		}
	}

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteAccessLevel is deprecated due to non-CLS compliance, please use WriteAccess instead.")]
	public ushort WriteAccessLevel
	{
		get
		{
			return (ushort)WriteAccess;
		}
		set
		{
			WriteAccess = value;
		}
	}

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("HardwareAccessLevel is deprecated due to non-CLS compliance, please use HardwareAccess instead.")]
	public ushort HardwareAccessLevel => (ushort)hardwareAccessLevel;

	public event DebugInfoEventHandler DebugInfoEvent;

	public event ExceptionEventHandler ExceptionEvent;

	public event FatalEventHandler FatalEvent;

	public event ByteMessageEventHandler ByteMessageEvent;

	internal event ByteMessageEventHandler ByteMessageInternalEvent;

	private Sapi()
	{
		initState = InitState.NotInitialized;
		caesarLibraryVersion = string.Empty;
		caesarLibraryCompilationDate = string.Empty;
		EmbellishPath(GetType().Assembly);
		configurationItems = new ConfigurationItemCollection();
		ecus = new EcuCollection();
		protocols = new DiagnosisProtocolCollection();
		channels = new ChannelCollection();
		logFiles = new LogFileCollection();
		codingFiles = new CodingFileCollection();
		flashFiles = new FlashFileCollection();
		busMonitors = new BusMonitorCollection();
		Environment.SetEnvironmentVariable("APP_PATH", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
	}

	internal static Type GetRealCaesarType(ParamType pt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected I4, but got Unknown
		Type result = null;
		switch ((int)pt)
		{
		case 0:
			result = typeof(byte);
			break;
		case 1:
			result = typeof(ushort);
			break;
		case 2:
			result = typeof(uint);
			break;
		case 3:
			result = typeof(sbyte);
			break;
		case 4:
			result = typeof(short);
			break;
		case 5:
			result = typeof(int);
			break;
		case 6:
			result = typeof(float);
			break;
		case 7:
		case 17:
			result = typeof(string);
			break;
		case 18:
			result = typeof(Choice);
			break;
		case 14:
			result = typeof(bool);
			break;
		case 15:
			result = typeof(Dump);
			break;
		}
		return result;
	}

	internal static XmlNode ReadSapiXmlFile(string path, string cl, out int ver, out DateTime dt)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(path);
		return ReadSapiXmlFile(xmlDocument, cl, out ver, out dt);
	}

	internal static XmlReader ReadSapiXmlFile(Stream stream, string cl, out int ver, out DateTime dt)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreWhitespace = true;
		xmlReaderSettings.IgnoreComments = true;
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.CheckCharacters = false;
		xmlReaderSettings.ValidationType = ValidationType.Schema;
		xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings;
		xmlReaderSettings.ValidationEventHandler += XmlValidationEventHandler;
		xmlReaderSettings.XmlResolver = new XmlEmbeddedResourceResolver();
		XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings);
		ReadSapiXmlFile(xmlReader, cl, out ver, out dt);
		return xmlReader;
	}

	internal void InitSapiXmlFile(XmlWriter xmlWriter, string cl, int ver, DateTime dt, bool addCBFVersions)
	{
		xmlWriter.WriteStartElement("SapiFile");
		xmlWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
		xmlWriter.WriteAttributeString("xsi:noNamespaceSchemaLocation", string.Format(CultureInfo.InvariantCulture, "{0}_{1}.xsd", cl, ver));
		xmlWriter.WriteAttributeString("Time", TimeToString(dt));
		if (!addCBFVersions)
		{
			return;
		}
		XElement xElement = new XElement("CBFVersions");
		foreach (Ecu ecu in Ecus)
		{
			xElement.Add(new XElement("CBF", new XAttribute("ecu", ecu.Name), new XAttribute("version", ecu.DescriptionDataVersion)));
		}
		xElement.WriteTo(xmlWriter);
	}

	internal XmlNode InitSapiXmlFile(string cl, int ver, DateTime dt, bool addCBFVersions)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "SapiFile", string.Empty);
		XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("xmlns:xsi");
		XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("xsi:noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
		xmlAttribute.InnerText = "http://www.w3.org/2001/XMLSchema-instance";
		xmlAttribute2.InnerText = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.xsd", cl, ver);
		xmlNode.Attributes.Append(xmlAttribute);
		xmlNode.Attributes.Append(xmlAttribute2);
		XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute("Time");
		xmlAttribute3.InnerText = TimeToString(dt);
		xmlNode.Attributes.Append(xmlAttribute3);
		if (addCBFVersions)
		{
			XmlNode xmlNode2 = xmlDocument.CreateNode(XmlNodeType.Element, "CBFVersions", string.Empty);
			foreach (Ecu ecu in Ecus)
			{
				XmlNode xmlNode3 = xmlDocument.CreateNode(XmlNodeType.Element, "CBF", string.Empty);
				XmlAttribute xmlAttribute4 = xmlDocument.CreateAttribute("ecu");
				XmlAttribute xmlAttribute5 = xmlDocument.CreateAttribute("version");
				xmlAttribute4.InnerText = ecu.Name;
				xmlAttribute5.InnerText = ecu.DescriptionDataVersion;
				xmlNode3.Attributes.Append(xmlAttribute4);
				xmlNode3.Attributes.Append(xmlAttribute5);
				xmlNode2.AppendChild(xmlNode3);
			}
			xmlNode.AppendChild(xmlNode2);
		}
		xmlDocument.AppendChild(xmlNode);
		return xmlNode;
	}

	internal static void XmlValidationEventHandler(object sender, ValidationEventArgs e)
	{
		throw new XmlException(e.Message, e.Exception);
	}

	internal void RaiseDebugInfoEvent(object sender, string s)
	{
		FireAndForget.Invoke(this.DebugInfoEvent, sender, new DebugInfoEventArgs(s));
	}

	internal void RaiseExceptionEvent(object sender, Exception e)
	{
		FireAndForget.Invoke(this.ExceptionEvent, sender, new ResultEventArgs(e));
	}

	internal void ModifyFlashFiles(string path, bool isAdd)
	{
		//IL_0057: Expected O, but got Unknown
		lock (flashFilesLock)
		{
			bool num = flashFilesLoaded;
			EnsureFlashFilesLoaded();
			DiagnosisSource diagnosisSource = FlashFileCollection.GetFlashFileDiagnosisSource(path);
			if (num)
			{
				switch (diagnosisSource)
				{
				case DiagnosisSource.CaesarDatabase:
					try
					{
						if (isAdd)
						{
							CaesarRoot.AddFlashFile(path);
						}
						else
						{
							CaesarRoot.RemoveFlashFile(path);
						}
					}
					catch (CaesarErrorException ex)
					{
						CaesarErrorException ex2 = ex;
						if (ex2.ErrNo != 5064)
						{
							throw new CaesarException(ex2);
						}
					}
					break;
				case DiagnosisSource.McdDatabase:
					McdRoot.LinkFlashFiles();
					break;
				}
			}
			foreach (Channel item in channels.Where((Channel c) => c.Ecu.DiagnosisSource == diagnosisSource))
			{
				item.FlashAreas.ResetList();
			}
			flashFiles.RebuildList(diagnosisSource);
		}
	}

	internal static int CalculatePrecision(double factor)
	{
		int num = -(int)Math.Floor(Math.Log10(factor));
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	internal static string Decrypt(Dump data)
	{
		ICryptoTransform cryptoTransform = new DESCryptoServiceProvider().CreateDecryptor(new Dump("FA6D95B595E06C1B").Data.ToArray(), new Dump("8A494ABD907962A1").Data.ToArray());
		byte[] array = data.Data.ToArray();
		byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.Unicode.GetString(bytes);
	}

	internal Assembly BuildExtensions(IEnumerable<string> source)
	{
		CompilerParameters compilerParameters = new CompilerParameters();
		compilerParameters.GenerateInMemory = true;
		compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
		compilerParameters.ReferencedAssemblies.Add("System.dll");
		compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
		string location = Assembly.GetExecutingAssembly().Location;
		string value = Path.Combine(Path.GetDirectoryName(location), "caesarabstraction.dll");
		compilerParameters.ReferencedAssemblies.Add(location);
		compilerParameters.ReferencedAssemblies.Add(value);
		compilerParameters.IncludeDebugInformation = true;
		CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromSource(compilerParameters, source.ToArray());
		if (compilerResults.Errors.Count > 0)
		{
			for (int i = 0; i < compilerResults.Errors.Count; i++)
			{
				CompilerError compilerError = compilerResults.Errors[i];
				RaiseDebugInfoEvent(this, compilerError.ToString());
			}
			return null;
		}
		return compilerResults.CompiledAssembly;
	}

	internal void DebugInfoCallback(string debugText)
	{
		RaiseDebugInfoEvent(typeof(CaesarRoot), debugText);
	}

	internal void ChannelDebugInfoCallback(CaesarChannel caesarChannel, string debugText)
	{
		Channel item = channels.GetItem(caesarChannel);
		if (item != null)
		{
			if (debugText.StartsWith("InternalDoDiagService() [Calling interpretable program] for ", StringComparison.Ordinal))
			{
				item.FlashAreas.SetCurrentFlashJob(debugText.Substring("InternalDoDiagService() [Calling interpretable program] for ".Length));
			}
			RaiseDebugInfoEvent(item, debugText);
		}
	}

	internal void FatalFunctionCallback(int param1, int param2, string expstr, string file, int lineno)
	{
		FireAndForget.Invoke(this.FatalEvent, this, new FatalEventArgs(param1, param2, expstr, file, lineno));
	}

	internal void ByteReceiveCallback(CaesarChannel caesarChannel, byte[] response)
	{
		Channel item = channels.GetItem(caesarChannel);
		if (item != null)
		{
			RaiseByteMessageEvent(item, ByteMessageDirection.RX, new Dump(response));
		}
	}

	internal void ByteSendCallback(CaesarChannel caesarChannel, byte[] request)
	{
		Channel item = channels.GetItem(caesarChannel);
		if (item != null)
		{
			RaiseByteMessageEvent(item, ByteMessageDirection.TX, new Dump(request));
		}
	}

	internal void PeriodicCallback(CaesarChannel caesarChannel, int index, CaesarDiagServiceIO diagService)
	{
		channels.GetItem(caesarChannel)?.Instruments.PeriodicCallback(index, diagService);
	}

	public static Sapi GetSapi()
	{
		if (sapi == null && !disposed)
		{
			sapi = new Sapi();
		}
		return sapi;
	}

	private bool ValidateCaesarPduApiConfiguration(string rootDescriptionPath)
	{
		RaiseDebugInfoEvent(this, "CAESAR: PDU-API root description path: " + rootDescriptionPath);
		if (File.Exists(rootDescriptionPath))
		{
			IDictionary<string, string> rootDescriptionFileLibraryPaths = McdRoot.GetRootDescriptionFileLibraryPaths(rootDescriptionPath);
			int num = 0;
			foreach (KeyValuePair<string, string> item in rootDescriptionFileLibraryPaths)
			{
				num += (EmitNativeLibraryVersion(item.Key, item.Value) ? 1 : 0);
			}
			return num > 0;
		}
		RaiseDebugInfoEvent(this, "CAESAR: PDU-API: unable to read content as the root description file was not at the specified path");
		return false;
	}

	public void Init(bool marshalEvents)
	{
		//IL_009d: Expected O, but got Unknown
		//IL_0213: Expected O, but got Unknown
		InternalInitOffline(marshalEvents);
		try
		{
			CaesarRoot.CaesarSetOptions(string.Format(CultureInfo.InvariantCulture, "SELECTPASSTHRUDEVICE={0}", configurationItems["PassThruDevice"].Value));
			CaesarRoot.CaesarSetOptions("CPULOADREDUCTION=AJWY");
			CaesarRoot.CaesarSetOptions(string.Format(CultureInfo.InvariantCulture, "PassThru_P2_Offset_CAN={0}", configurationItems["PassThru_P2_Offset_CAN"].Value));
			CaesarRoot.CaesarSetOptions(string.Format(CultureInfo.InvariantCulture, "PassThru_P2_Offset_KLine={0}", configurationItems["PassThru_P2_Offset_KLine"].Value));
		}
		catch (CaesarErrorException ex)
		{
			throw new CaesarException(ex, null, null);
		}
		try
		{
			string text = configurationItems["HardwareType"].RawValue.ToString();
			GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Connecting to CAESAR Parts {0} ...", text));
			if (text == "P" && !ValidateCaesarPduApiConfiguration(CaesarRoot.PduApiRootLocation))
			{
				RaiseExceptionEvent(this, new CaesarException(SapiError.NoLoadableCaesarPduApi));
				return;
			}
			CaesarRoot.RequiredPartJHardwarePrefix = configurationItems["PassThruHardwareNamePrefix"].Value;
			CaesarRoot.CaesarComConstruct(text, configurationItems["PartXIPAddress"].Value, configurationItems["PartXPort"].Value, Convert.ToInt32(configurationItems["PinMapping"].RawValue, CultureInfo.InvariantCulture) != 0, Convert.ToInt32(configurationItems["GPDFlashCaching"].RawValue, CultureInfo.InvariantCulture) != 0, Convert.ToInt32(configurationItems["TLSlave"].RawValue, CultureInfo.InvariantCulture) != 0, Convert.ToInt32(configurationItems["FlashBoot"].RawValue, CultureInfo.InvariantCulture));
			initState = InitState.Online;
			hardwareAccessLevel = CaesarRoot.SGetCurrentSecurityLevel();
			RollCallJ1708.GlobalInstance.Init();
			RollCallJ1939.GlobalInstance.Init();
			if (McdRoot.Initialized)
			{
				McdRoot.PrepareVciAccessLayer();
			}
			RollCallDoIP.GlobalInstance.Init();
		}
		catch (CaesarErrorException ex2)
		{
			CaesarErrorException ex3 = ex2;
			CaesarException ex4 = new CaesarException(ex3);
			if (ex3.ErrNo < 9900 || ex3.ErrNo > 9920)
			{
				throw ex4;
			}
			RaiseExceptionEvent(this, ex4);
			CaesarConstruct();
		}
		catch (McdException mcdError)
		{
			CaesarException e = new CaesarException(mcdError);
			RaiseExceptionEvent(this, e);
		}
	}

	public void InitOffline(bool marshalEvents)
	{
		InternalInitOffline(marshalEvents);
	}

	public static string TimeToString(DateTime time)
	{
		return time.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
	}

	internal static string MakeTranslationIdentifier(params string[] args)
	{
		return string.Join(".", args.ToArray());
	}

	public static DateTime TimeFromString(string time)
	{
		if (time == null)
		{
			throw new ArgumentNullException("time");
		}
		if (time.Length != 17)
		{
			throw new ArgumentException("The time argument is not of the correct length");
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int year = (time[0] - 48) * 1000 + (time[1] - 48) * 100 + (time[2] - 48) * 10 + (time[3] - 48);
		num2 = (time[4] - 48) * 10 + (time[5] - 48);
		num = (time[6] - 48) * 10 + (time[7] - 48);
		num3 = (time[8] - 48) * 10 + (time[9] - 48);
		num4 = (time[10] - 48) * 10 + (time[11] - 48);
		num5 = (time[12] - 48) * 10 + (time[13] - 48);
		num6 = (time[14] - 48) * 100 + (time[15] - 48) * 10 + (time[16] - 48);
		return new DateTime(year, num2, num, num3, num4, num5, num6);
	}

	public void Exit()
	{
		if (initState != InitState.NotInitialized)
		{
			if (initState == InitState.Online)
			{
				channels.StopAutoConnect();
				channels.ClearEcusInAutoConnect();
				RollCallJ1708.GlobalInstance.Stop();
				RollCallJ1939.GlobalInstance.Stop();
				RollCallDoIP.GlobalInstance.Stop();
				channels.WaitForBackgroundConnection();
				while (channels.Count > 0)
				{
					channels[0].Disconnect();
				}
				channels.CleanupOffline();
				while (busMonitors.Count > 0)
				{
					busMonitors[0].Stop();
				}
				BusMonitorFrame.ClearIdentifierMap();
				try
				{
					CaesarRoot.CaesarComDestruct();
				}
				catch (NullReferenceException)
				{
					RaiseDebugInfoEvent(this, "Intentional catch of null reference exception during CaesarComDestruct");
				}
				if (McdRoot.IsVciAccessLayerPrepared)
				{
					McdRoot.UnprepareVciAccessLayer();
				}
			}
			initState = InitState.Offline;
			logFiles.StopLog();
			while (logFiles.Count > 0)
			{
				logFiles[0].Close();
			}
		}
		CaesarRoot.CaesarDestruct();
		RollCallJ1708.GlobalInstance.Exit();
		RollCallJ1939.GlobalInstance.Exit();
		RollCallDoIP.GlobalInstance.Exit();
		ecus.ClearList();
		protocols.ClearList();
		codingFiles.ClearList();
		flashFiles.ClearList();
		CaesarRoot.FreeLists();
		RefreshFlashFiles();
		if (McdRoot.Initialized)
		{
			McdRoot.ByteMessage -= McdRoot_ByteMessage;
			McdRoot.DebugInfo -= McdRoot_DebugInfo;
			McdRoot.Destruct();
		}
		threadMarshalControl = null;
		initState = InitState.NotInitialized;
	}

	public void AddFlashFile(string path)
	{
		ModifyFlashFiles(path, isAdd: true);
	}

	public void RemoveFlashFile(string path)
	{
		ModifyFlashFiles(path, isAdd: false);
	}

	internal void EnsureCodingFilesLoaded()
	{
		if (!codingFiles.Acquired)
		{
			RaiseDebugInfoEvent(this, "EnsureCodingFilesLoaded: " + codingFiles.Count + " available coding files.");
		}
	}

	internal void EnsureFlashFilesLoaded()
	{
		lock (flashFilesLock)
		{
			if (!flashFilesLoaded)
			{
				CaesarRoot.InternalInitAddFlashFiles(configurationItems["CFFFiles"].Value);
				McdRoot.LinkFlashFiles();
				flashFilesLoaded = true;
			}
		}
	}

	public void RefreshFlashFiles()
	{
		lock (flashFilesLock)
		{
			flashFilesLoaded = false;
			foreach (Channel channel in channels)
			{
				channel.FlashAreas.ResetList();
			}
			flashFiles.ClearList();
		}
	}

	public void SetToolId(IEnumerable<byte> toolId)
	{
		if (initState == InitState.NotInitialized)
		{
			this.toolId = new List<byte>(toolId).AsReadOnly();
			return;
		}
		throw new InvalidOperationException("Tool ID cannot be set after S-API is initialised");
	}

	public static void SaveDataItemDescription(Protocol protocol, byte address, Type type, string qualifier, IDictionary<string, string> content)
	{
		RollCallSae.MonitorData.SaveDataItemDescription(protocol, address, type, qualifier, content);
	}

	public void SetConfigurationFileProvider(Func<Ecu, string, Stream> getConfiguration, Action<Ecu, Stream, XmlNode> releaseConfiguration)
	{
		this.getConfiguration = getConfiguration;
		this.releaseConfiguration = releaseConfiguration;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void CaesarConstruct()
	{
		//IL_009f: Expected O, but got Unknown
		CaesarRoot.SetLicenseFile();
		try
		{
			CaesarRoot.CaesarConstruct((toolId != null) ? toolId.ToArray() : null, Convert.ToInt32(configurationItems["Units"].RawValue, CultureInfo.InvariantCulture), Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gbf"), configurationItems["CCFFiles"].Value);
			byte[] array = new byte[256]
			{
				178, 8, 204, 255, 37, 109, 19, 182, 157, 112,
				78, 166, 188, 249, 54, 231, 213, 139, 207, 1,
				44, 73, 230, 61, 200, 100, 114, 235, 182, 164,
				203, 235, 238, 99, 194, 137, 103, 241, 142, 79,
				116, 1, 116, 153, 139, 220, 236, 90, 190, 31,
				93, 82, 37, 244, 126, 235, 98, 6, 185, 92,
				160, 160, 17, 47, 231, 78, 109, 220, 137, 32,
				220, 28, 48, 230, 5, 97, 163, 29, 6, 37,
				150, 252, 95, 91, 71, 170, 66, 61, 36, 8,
				179, 195, 154, 181, 168, 58, 160, 163, 117, 195,
				199, 252, 15, 143, 187, 62, 127, 11, 219, 49,
				255, 46, 101, 146, 71, 61, 73, 194, 133, 75,
				247, 191, 225, 122, 238, 144, 13, 130, 67, 102,
				37, 228, 181, 192, 96, 235, 231, 37, 224, 35,
				113, 23, 123, 43, 41, 130, 128, 125, 157, 59,
				214, 182, 124, 192, 108, 148, 3, 150, 159, 29,
				195, 165, 80, 145, 252, 103, 201, 213, 38, 82,
				221, 233, 123, 40, 111, 50, 21, 110, 181, 180,
				0, 223, 203, 206, 166, 186, 99, 44, 187, 157,
				215, 80, 184, 61, 238, 205, 172, 78, 152, 4,
				107, 184, 214, 140, 26, 120, 78, 180, 61, 219,
				33, 177, 212, 225, 237, 249, 253, 51, 193, 54,
				37, 250, 223, 227, 129, 191, 137, 25, 15, 164,
				209, 172, 120, 234, 12, 200, 17, 12, 34, 147,
				79, 45, 100, 190, 143, 49, 145, 17, 210, 174,
				100, 190, 3, 166, 186, 215
			};
			CaesarRoot.Authenticate("SAPI", array);
		}
		catch (CaesarErrorException ex)
		{
			throw new CaesarException(ex, null, null);
		}
	}

	private bool EmitNativeLibraryVersion(string identifier, string path)
	{
		ProcessModule processModule = null;
		string text = identifier + " path: " + path;
		if (File.Exists(path))
		{
			try
			{
				McdRoot.ValidateNativeLibrary(path, useStandardSearchSemantics: true);
				processModule = Process.GetCurrentProcess().Modules.OfType<ProcessModule>().FirstOrDefault((ProcessModule m) => string.Equals(m.ModuleName, Path.GetFileName(path), StringComparison.OrdinalIgnoreCase));
				text += ((processModule != null) ? (" version: " + processModule.FileVersionInfo.FileVersion) : "<loaded module not located>");
			}
			catch (Exception ex) when (ex is BadImageFormatException || ex is DllNotFoundException)
			{
				text = text + " validation exception: " + ex.Message;
			}
		}
		else
		{
			text += " <file not found>";
		}
		RaiseDebugInfoEvent(this, text);
		return processModule != null;
	}

	private static string GetSidDll()
	{
		string name = "Software\\PassThruSupport\\Detroit Diesel\\Devices\\SID";
		string result = "";
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
		if (registryKey != null)
		{
			result = registryKey.GetValue("FunctionLibrary") as string;
		}
		return result;
	}

	private void InternalInitOffline(bool marshalEvents)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c1: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		if (initState != InitState.NotInitialized)
		{
			Exit();
		}
		if (marshalEvents)
		{
			threadMarshalControl = new Control();
			threadMarshalControl.CreateControl();
		}
		RaiseDebugInfoEvent(this, "S-API path: " + Assembly.GetExecutingAssembly().Location + " version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
		string sidDll = GetSidDll();
		if (!string.IsNullOrEmpty(sidDll))
		{
			EmitNativeLibraryVersion("SID", sidDll);
		}
		else
		{
			RaiseDebugInfoEvent(this, "SID path:  <was not specified in the registry>");
		}
		ushort num = Convert.ToUInt16(configurationItems["DebugLevel"].RawValue, CultureInfo.InvariantCulture);
		CaesarRoot.SetByteMessageCallbacks(new ByteSendCallback(ByteSendCallback), new ByteReceiveCallback(ByteReceiveCallback));
		CaesarRoot.SetDebugInfoCallback(new DebugInfoCallback(DebugInfoCallback), num);
		CaesarRoot.SetChannelDebugInfoCallback(new ChannelDebugInfoCallback(ChannelDebugInfoCallback), num);
		CaesarRoot.SetPeriodicCallback(new PeriodicCallback(PeriodicCallback));
		CaesarRoot.SetFatalFunctionCallback(new FatalFunctionCallback(FatalFunctionCallback));
		CaesarConstruct();
		initState = InitState.Offline;
		caesarLibraryVersion = CaesarRoot.LibraryVersion;
		caesarLibraryCompilationDate = CaesarRoot.LibraryCompilationDate;
		PresentationCulture = Thread.CurrentThread.CurrentUICulture;
		if (!UseMcd)
		{
			return;
		}
		try
		{
			string value = configurationItems["McdDtsPath"].Value;
			if (!string.IsNullOrEmpty(value))
			{
				value = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), value);
				RaiseDebugInfoEvent(this, "MCD: DTS path: " + value);
				SetDtsPathEnvironmentVariable(value);
				if (mcdKeyFunc != null && mcdProcedureId.HasValue)
				{
					McdRoot.Construct(value, mcdProcedureId.Value, mcdKeyFunc, (toolId != null) ? toolId.ToArray() : null);
					if (McdRoot.Initialized)
					{
						RaiseDebugInfoEvent(this, "MCD: ** using MCD-3D " + McdRoot.Description + " **");
						RaiseDebugInfoEvent(this, "MCD: JVM location: " + McdRoot.JavaVirtualMachineLocation);
						string text = configurationItems["McdRootDescriptionFile"].Value;
						if (string.IsNullOrEmpty(text))
						{
							text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "pdu_api_root.xml");
						}
						RaiseDebugInfoEvent(this, "MCD: PDU-API root description path: " + text);
						if (File.Exists(text))
						{
							foreach (KeyValuePair<string, string> rootDescriptionFileLibraryPath in McdRoot.GetRootDescriptionFileLibraryPaths(text))
							{
								EmitNativeLibraryVersion(rootDescriptionFileLibraryPath.Key, rootDescriptionFileLibraryPath.Value);
								if (rootDescriptionFileLibraryPath.Key == "BOSCH_DS_D_PDU_API")
								{
									EmitNativeLibraryVersion(rootDescriptionFileLibraryPath.Key, Path.Combine(Path.GetDirectoryName(rootDescriptionFileLibraryPath.Value), "PDUAPI_Bosch.dll"));
								}
							}
						}
						else
						{
							RaiseDebugInfoEvent(this, "MCD: PDU-API: unable to read content as the root description file was not at the specified path");
						}
						McdRoot.SetRootDescriptionFile(text);
						string value2 = configurationItems["McdSessionProjectPath"].Value;
						if (!string.IsNullOrEmpty(value2))
						{
							RaiseDebugInfoEvent(this, "MCD: Session project path: " + value2);
							McdRoot.SetSessionProjectPath(value2);
						}
						McdRoot.DebugInfo += McdRoot_DebugInfo;
						string value3 = configurationItems["McdProjectName"].Value;
						RaiseDebugInfoEvent(this, "MCD: Project name: " + value3);
						McdRoot.SelectProject(value3);
						string value4 = configurationItems["McdVehicleInformationTable"].Value;
						RaiseDebugInfoEvent(this, "MCD: Vehicle Information Table: " + value4);
						McdRoot.SelectVehicleInformation(value4);
						RaiseDebugInfoEvent(this, "MCD: com.softing.AdditionalClassPath = " + McdRoot.GetProperty("com.softing.AdditionalClassPath"));
						RaiseDebugInfoEvent(this, "MCD: com.softing.GlobalSmrFilePath = " + McdRoot.GetProperty("com.softing.GlobalSmrFilePath"));
						McdRoot.ByteMessage += McdRoot_ByteMessage;
					}
					else
					{
						RaiseDebugInfoEvent(this, "** MCD-3D system could not be initialized **");
					}
				}
				else
				{
					RaiseDebugInfoEvent(this, "** MCD-3D system requires key function and procedure id **");
				}
			}
			else
			{
				RaiseDebugInfoEvent(this, "** MCD-3D system path is not specified **");
			}
		}
		catch (FileNotFoundException ex)
		{
			RaiseDebugInfoEvent(this, "Error attempting to load McdAbstraction: " + ex.Message);
		}
	}

	public void ValidateMcd()
	{
		string value = configurationItems["McdDtsPath"].Value;
		if (!string.IsNullOrEmpty(value))
		{
			ValidateMcd(value);
			return;
		}
		throw new DirectoryNotFoundException("MCD-3D system path is not specified");
	}

	private static void SetDtsPathEnvironmentVariable(string mcdPath)
	{
		if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DTS_PATH")) && File.Exists(Path.Combine(mcdPath, "csWrap.dll")))
		{
			Environment.SetEnvironmentVariable("DTS_PATH", Directory.GetParent(mcdPath).FullName);
		}
	}

	public static void ValidateMcd(string mcdPath)
	{
		SetDtsPathEnvironmentVariable(mcdPath);
		McdRoot.ValidateNativeLibrary(Path.Combine(mcdPath, "csWrap.dll"), useStandardSearchSemantics: false);
	}

	private void McdRoot_DebugInfo(object sender, McdDebugInfoEventArgs e)
	{
		McdLogicalLink link;
		if ((link = sender as McdLogicalLink) != null)
		{
			Channel channel = channels.FirstOrDefault((Channel c) => c.IsOwner(link));
			if (channel != null)
			{
				RaiseDebugInfoEvent(channel, e.Message);
			}
			else
			{
				RaiseDebugInfoEvent(link.DBLocation.Qualifier, e.Message);
			}
		}
		else
		{
			RaiseDebugInfoEvent(typeof(McdRoot), e.Message);
		}
	}

	private void McdRoot_ByteMessage(object sender, McdByteMessageEventArgs e)
	{
		McdLogicalLink link = sender as McdLogicalLink;
		Channel channel = channels.FirstOrDefault((Channel c) => c.IsOwner(link));
		if (channel != null)
		{
			RaiseByteMessageEvent(channel, e.IsSend ? ByteMessageDirection.TX : ByteMessageDirection.RX, new Dump(e.Message));
		}
	}

	public void SetMcdKeyProvider(int mcdProcedureId, Func<byte[], byte[]> mcdKeyFunc)
	{
		this.mcdProcedureId = mcdProcedureId;
		this.mcdKeyFunc = mcdKeyFunc;
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			Exit();
			sapi = null;
			if (threadMarshalControl != null)
			{
				threadMarshalControl.Dispose();
				threadMarshalControl = null;
			}
		}
		disposed = true;
	}

	private void RaiseByteMessageEvent(object sender, ByteMessageDirection direction, Dump data)
	{
		ByteMessageEventArgs e = new ByteMessageEventArgs(direction, data);
		FireAndForget.Invoke(this.ByteMessageEvent, sender, e);
		this.ByteMessageInternalEvent?.Invoke(sender, e);
	}

	internal static XmlNode ReadSapiXmlFile(XmlDocument doc, string cl, out int ver, out DateTime dt)
	{
		XmlNode xmlNode = doc.SelectSingleNode("/SapiFile");
		if (xmlNode == null)
		{
			throw new XmlException("XML data did not contain SapiFile root node");
		}
		string innerText = (xmlNode.Attributes.GetNamedItem("xsi:noNamespaceSchemaLocation") ?? throw new XmlException("XML data did not correctly specify a schema")).InnerText;
		string[] array = innerText.Split("_.".ToCharArray());
		if (string.Equals(array[0], cl, StringComparison.Ordinal))
		{
			XmlNode namedItem = xmlNode.Attributes.GetNamedItem("Time");
			ver = Convert.ToInt32(array[1], CultureInfo.InvariantCulture);
			dt = TimeFromString(namedItem.InnerText);
			XmlReader schemaDocument = XmlReader.Create(new StringReader(GetResourceSchema(innerText) ?? throw new FileNotFoundException(innerText)));
			doc.Schemas.Add(null, schemaDocument);
			doc.Validate(XmlValidationEventHandler);
			return xmlNode;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Class of XML data ({0}) did not match desired ({1})", array[0], cl));
	}

	private static void ReadSapiXmlFile(XmlReader xmlReader, string cl, out int ver, out DateTime dt)
	{
		_ = LogFile.CurrentFormat;
		if (!xmlReader.ReadToFollowing("SapiFile"))
		{
			throw new XmlException("XML data did not contain SapiFile root node");
		}
		XNamespace xNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		string[] array = (xmlReader.GetAttribute("noNamespaceSchemaLocation", xNamespace.NamespaceName) ?? throw new XmlException("XML data did not correctly specify a schema")).Split("_.".ToCharArray());
		if (string.Equals(array[0], cl, StringComparison.Ordinal))
		{
			string attribute = xmlReader.GetAttribute("Time");
			ver = Convert.ToInt32(array[1], CultureInfo.InvariantCulture);
			dt = TimeFromString(attribute);
			return;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Class of XML data ({0}) did not match desired ({1})", array[0], cl));
	}

	private static void EmbellishPath(Assembly assembly)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PATH");
		string location = assembly.Location;
		environmentVariable = string.Format(CultureInfo.InvariantCulture, "{0};{1}", Path.GetDirectoryName(location), environmentVariable);
		Environment.SetEnvironmentVariable("PATH", environmentVariable);
	}

	private static string GetResourceSchema(string name)
	{
		string result = null;
		string name2 = string.Format(CultureInfo.InvariantCulture, "SapiLayer1.{0}", name);
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name2);
		if (manifestResourceStream != null)
		{
			using (StreamReader streamReader = new StreamReader(manifestResourceStream))
			{
				result = streamReader.ReadToEnd();
			}
			manifestResourceStream.Close();
		}
		return result;
	}
}
