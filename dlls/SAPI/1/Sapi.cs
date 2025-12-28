// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Sapi
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using J2534;
using McdAbstraction;
using Microsoft.CSharp;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

#nullable disable
namespace SapiLayer1;

public sealed class Sapi : IDisposable
{
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

  private Sapi()
  {
    this.initState = InitState.NotInitialized;
    this.caesarLibraryVersion = string.Empty;
    this.caesarLibraryCompilationDate = string.Empty;
    Sapi.EmbellishPath(this.GetType().Assembly);
    this.configurationItems = new ConfigurationItemCollection();
    this.ecus = new EcuCollection();
    this.protocols = new DiagnosisProtocolCollection();
    this.channels = new ChannelCollection();
    this.logFiles = new LogFileCollection();
    this.codingFiles = new CodingFileCollection();
    this.flashFiles = new FlashFileCollection();
    this.busMonitors = new BusMonitorCollection();
    Environment.SetEnvironmentVariable("APP_PATH", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
  }

  internal static System.Type GetRealCaesarType(ParamType pt)
  {
    System.Type realCaesarType = (System.Type) null;
    switch ((int) pt)
    {
      case 0:
        realCaesarType = typeof (byte);
        break;
      case 1:
        realCaesarType = typeof (ushort);
        break;
      case 2:
        realCaesarType = typeof (uint);
        break;
      case 3:
        realCaesarType = typeof (sbyte);
        break;
      case 4:
        realCaesarType = typeof (short);
        break;
      case 5:
        realCaesarType = typeof (int);
        break;
      case 6:
        realCaesarType = typeof (float);
        break;
      case 7:
      case 17:
        realCaesarType = typeof (string);
        break;
      case 14:
        realCaesarType = typeof (bool);
        break;
      case 15:
        realCaesarType = typeof (Dump);
        break;
      case 18:
        realCaesarType = typeof (Choice);
        break;
    }
    return realCaesarType;
  }

  internal static XmlNode ReadSapiXmlFile(string path, string cl, out int ver, out DateTime dt)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(path);
    return Sapi.ReadSapiXmlFile(doc, cl, out ver, out dt);
  }

  internal static XmlReader ReadSapiXmlFile(
    Stream stream,
    string cl,
    out int ver,
    out DateTime dt)
  {
    XmlReaderSettings settings = new XmlReaderSettings();
    settings.IgnoreWhitespace = true;
    settings.IgnoreComments = true;
    settings.ConformanceLevel = ConformanceLevel.Fragment;
    settings.CheckCharacters = false;
    settings.ValidationType = ValidationType.Schema;
    settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings;
    settings.ValidationEventHandler += new ValidationEventHandler(Sapi.XmlValidationEventHandler);
    settings.XmlResolver = (XmlResolver) new Sapi.XmlEmbeddedResourceResolver();
    XmlReader xmlReader = XmlReader.Create(stream, settings);
    Sapi.ReadSapiXmlFile(xmlReader, cl, out ver, out dt);
    return xmlReader;
  }

  internal void InitSapiXmlFile(
    XmlWriter xmlWriter,
    string cl,
    int ver,
    DateTime dt,
    bool addCBFVersions)
  {
    xmlWriter.WriteStartElement("SapiFile");
    xmlWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
    xmlWriter.WriteAttributeString("xsi:noNamespaceSchemaLocation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.xsd", (object) cl, (object) ver));
    xmlWriter.WriteAttributeString("Time", Sapi.TimeToString(dt));
    if (!addCBFVersions)
      return;
    XElement xelement = new XElement((XName) "CBFVersions");
    foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) this.Ecus)
      xelement.Add((object) new XElement((XName) "CBF", new object[2]
      {
        (object) new XAttribute((XName) "ecu", (object) ecu.Name),
        (object) new XAttribute((XName) "version", (object) ecu.DescriptionDataVersion)
      }));
    xelement.WriteTo(xmlWriter);
  }

  internal XmlNode InitSapiXmlFile(string cl, int ver, DateTime dt, bool addCBFVersions)
  {
    XmlDocument xmlDocument = new XmlDocument();
    XmlNode node1 = xmlDocument.CreateNode(XmlNodeType.Element, "SapiFile", string.Empty);
    XmlAttribute attribute1 = xmlDocument.CreateAttribute("xmlns:xsi");
    XmlAttribute attribute2 = xmlDocument.CreateAttribute("xsi:noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
    attribute1.InnerText = "http://www.w3.org/2001/XMLSchema-instance";
    attribute2.InnerText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.xsd", (object) cl, (object) ver);
    node1.Attributes.Append(attribute1);
    node1.Attributes.Append(attribute2);
    XmlAttribute attribute3 = xmlDocument.CreateAttribute("Time");
    attribute3.InnerText = Sapi.TimeToString(dt);
    node1.Attributes.Append(attribute3);
    if (addCBFVersions)
    {
      XmlNode node2 = xmlDocument.CreateNode(XmlNodeType.Element, "CBFVersions", string.Empty);
      foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) this.Ecus)
      {
        XmlNode node3 = xmlDocument.CreateNode(XmlNodeType.Element, "CBF", string.Empty);
        XmlAttribute attribute4 = xmlDocument.CreateAttribute("ecu");
        XmlAttribute attribute5 = xmlDocument.CreateAttribute("version");
        attribute4.InnerText = ecu.Name;
        attribute5.InnerText = ecu.DescriptionDataVersion;
        node3.Attributes.Append(attribute4);
        node3.Attributes.Append(attribute5);
        node2.AppendChild(node3);
      }
      node1.AppendChild(node2);
    }
    xmlDocument.AppendChild(node1);
    return node1;
  }

  internal static void XmlValidationEventHandler(object sender, ValidationEventArgs e)
  {
    throw new XmlException(e.Message, (Exception) e.Exception);
  }

  internal void RaiseDebugInfoEvent(object sender, string s)
  {
    FireAndForget.Invoke((MulticastDelegate) this.DebugInfoEvent, sender, (EventArgs) new DebugInfoEventArgs(s));
  }

  internal void RaiseExceptionEvent(object sender, Exception e)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ExceptionEvent, sender, (EventArgs) new ResultEventArgs(e));
  }

  internal Control ThreadMarshalControl => this.threadMarshalControl;

  internal void ModifyFlashFiles(string path, bool isAdd)
  {
    lock (this.flashFilesLock)
    {
      int num = this.flashFilesLoaded ? 1 : 0;
      this.EnsureFlashFilesLoaded();
      DiagnosisSource diagnosisSource = FlashFileCollection.GetFlashFileDiagnosisSource(path);
      if (num != 0)
      {
        switch (diagnosisSource)
        {
          case DiagnosisSource.CaesarDatabase:
            try
            {
              if (isAdd)
              {
                CaesarRoot.AddFlashFile(path);
                break;
              }
              CaesarRoot.RemoveFlashFile(path);
              break;
            }
            catch (CaesarErrorException ex)
            {
              if (ex.ErrNo != 5064L)
                throw new CaesarException(ex);
              break;
            }
          case DiagnosisSource.McdDatabase:
            McdRoot.LinkFlashFiles();
            break;
        }
      }
      foreach (Channel channel in this.channels.Where<Channel>((Func<Channel, bool>) (c => c.Ecu.DiagnosisSource == diagnosisSource)))
        channel.FlashAreas.ResetList();
      this.flashFiles.RebuildList(diagnosisSource);
    }
  }

  internal static int CalculatePrecision(double factor)
  {
    int precision = -(int) Math.Floor(Math.Log10(factor));
    if (precision < 0)
      precision = 0;
    return precision;
  }

  internal Assembly SapiExtensionAssembly
  {
    get
    {
      lock (this.extensionAssemblyLock)
      {
        if (!this.haveAttemptedExtensionAssemblyLoad && this.sapiExtensionAssembly == (Assembly) null)
        {
          string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sapiextension.dll");
          if (File.Exists(path))
          {
            this.RaiseDebugInfoEvent((object) this, "Loading extension assembly");
            this.sapiExtensionAssembly = Assembly.LoadFile(path);
          }
          this.haveAttemptedExtensionAssemblyLoad = true;
        }
        return this.sapiExtensionAssembly;
      }
    }
  }

  internal Assembly SapiFileSystemExtensionAssembly
  {
    get
    {
      lock (this.extensionAssemblyLock)
      {
        if (!this.haveAttemptedFileSystemExtensionAssemblyLoad && this.sapiFileSystemExtensionAssembly == (Assembly) null)
        {
          List<Ecu> list = this.Ecus.Where<Ecu>((Func<Ecu, bool>) (e => e.ConfigurationLoadedFromFile && e.ExtensionSource != null)).ToList<Ecu>();
          if (list.Any<Ecu>())
          {
            this.RaiseDebugInfoEvent((object) this, "Loading extension assembly for file-system sources " + string.Join(", ", list.Select<Ecu, string>((Func<Ecu, string>) (e => $"{e.DiagnosisSource.ToString()}.{e.Name}"))));
            this.sapiFileSystemExtensionAssembly = this.BuildExtensions(list.Select<Ecu, string>((Func<Ecu, string>) (e => e.ExtensionSource)));
          }
          this.haveAttemptedFileSystemExtensionAssemblyLoad = true;
        }
        return this.sapiFileSystemExtensionAssembly;
      }
    }
  }

  internal static string Decrypt(Dump data)
  {
    ICryptoTransform decryptor = new DESCryptoServiceProvider().CreateDecryptor(new Dump("FA6D95B595E06C1B").Data.ToArray<byte>(), new Dump("8A494ABD907962A1").Data.ToArray<byte>());
    byte[] array = data.Data.ToArray<byte>();
    byte[] inputBuffer = array;
    int length = array.Length;
    return Encoding.Unicode.GetString(decryptor.TransformFinalBlock(inputBuffer, 0, length));
  }

  internal Assembly BuildExtensions(IEnumerable<string> source)
  {
    CompilerParameters options = new CompilerParameters();
    options.GenerateInMemory = true;
    options.ReferencedAssemblies.Add("mscorlib.dll");
    options.ReferencedAssemblies.Add("System.dll");
    options.ReferencedAssemblies.Add("System.Core.dll");
    string location = Assembly.GetExecutingAssembly().Location;
    string str = Path.Combine(Path.GetDirectoryName(location), "caesarabstraction.dll");
    options.ReferencedAssemblies.Add(location);
    options.ReferencedAssemblies.Add(str);
    options.IncludeDebugInformation = true;
    CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromSource(options, source.ToArray<string>());
    if (compilerResults.Errors.Count <= 0)
      return compilerResults.CompiledAssembly;
    for (int index = 0; index < compilerResults.Errors.Count; ++index)
      this.RaiseDebugInfoEvent((object) this, compilerResults.Errors[index].ToString());
    return (Assembly) null;
  }

  internal void DebugInfoCallback(string debugText)
  {
    this.RaiseDebugInfoEvent((object) typeof (CaesarRoot), debugText);
  }

  internal void ChannelDebugInfoCallback(CaesarChannel caesarChannel, string debugText)
  {
    Channel sender = this.channels.GetItem(caesarChannel);
    if (sender == null)
      return;
    if (debugText.StartsWith("InternalDoDiagService() [Calling interpretable program] for ", StringComparison.Ordinal))
      sender.FlashAreas.SetCurrentFlashJob(debugText.Substring("InternalDoDiagService() [Calling interpretable program] for ".Length));
    this.RaiseDebugInfoEvent((object) sender, debugText);
  }

  internal void FatalFunctionCallback(
    int param1,
    int param2,
    string expstr,
    string file,
    int lineno)
  {
    FireAndForget.Invoke((MulticastDelegate) this.FatalEvent, (object) this, (EventArgs) new FatalEventArgs(param1, param2, expstr, file, lineno));
  }

  internal void ByteReceiveCallback(CaesarChannel caesarChannel, byte[] response)
  {
    Channel sender = this.channels.GetItem(caesarChannel);
    if (sender == null)
      return;
    this.RaiseByteMessageEvent((object) sender, ByteMessageDirection.RX, new Dump((IEnumerable<byte>) response));
  }

  internal void ByteSendCallback(CaesarChannel caesarChannel, byte[] request)
  {
    Channel sender = this.channels.GetItem(caesarChannel);
    if (sender == null)
      return;
    this.RaiseByteMessageEvent((object) sender, ByteMessageDirection.TX, new Dump((IEnumerable<byte>) request));
  }

  internal void PeriodicCallback(
    CaesarChannel caesarChannel,
    int index,
    CaesarDiagServiceIO diagService)
  {
    this.channels.GetItem(caesarChannel)?.Instruments.PeriodicCallback(index, diagService);
  }

  public static Sapi GetSapi()
  {
    if (Sapi.sapi == null && !Sapi.disposed)
      Sapi.sapi = new Sapi();
    return Sapi.sapi;
  }

  private bool ValidateCaesarPduApiConfiguration(string rootDescriptionPath)
  {
    this.RaiseDebugInfoEvent((object) this, "CAESAR: PDU-API root description path: " + rootDescriptionPath);
    if (File.Exists(rootDescriptionPath))
    {
      IDictionary<string, string> fileLibraryPaths = McdRoot.GetRootDescriptionFileLibraryPaths(rootDescriptionPath);
      int num = 0;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) fileLibraryPaths)
        num += this.EmitNativeLibraryVersion(keyValuePair.Key, keyValuePair.Value) ? 1 : 0;
      return num > 0;
    }
    this.RaiseDebugInfoEvent((object) this, "CAESAR: PDU-API: unable to read content as the root description file was not at the specified path");
    return false;
  }

  public void Init(bool marshalEvents)
  {
    this.InternalInitOffline(marshalEvents);
    try
    {
      CaesarRoot.CaesarSetOptions(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECTPASSTHRUDEVICE={0}", (object) this.configurationItems["PassThruDevice"].Value));
      CaesarRoot.CaesarSetOptions("CPULOADREDUCTION=AJWY");
      CaesarRoot.CaesarSetOptions(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PassThru_P2_Offset_CAN={0}", (object) this.configurationItems["PassThru_P2_Offset_CAN"].Value));
      CaesarRoot.CaesarSetOptions(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PassThru_P2_Offset_KLine={0}", (object) this.configurationItems["PassThru_P2_Offset_KLine"].Value));
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      throw new CaesarException(ex, negativeResponseCode);
    }
    try
    {
      string str = this.configurationItems["HardwareType"].RawValue.ToString();
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Connecting to CAESAR Parts {0} ...", (object) str));
      if (str == "P" && !this.ValidateCaesarPduApiConfiguration(CaesarRoot.PduApiRootLocation))
      {
        this.RaiseExceptionEvent((object) this, (Exception) new CaesarException(SapiError.NoLoadableCaesarPduApi));
      }
      else
      {
        CaesarRoot.RequiredPartJHardwarePrefix = this.configurationItems["PassThruHardwareNamePrefix"].Value;
        CaesarRoot.CaesarComConstruct(str, this.configurationItems["PartXIPAddress"].Value, this.configurationItems["PartXPort"].Value, Convert.ToInt32(this.configurationItems["PinMapping"].RawValue, (IFormatProvider) CultureInfo.InvariantCulture) != 0, Convert.ToInt32(this.configurationItems["GPDFlashCaching"].RawValue, (IFormatProvider) CultureInfo.InvariantCulture) != 0, Convert.ToInt32(this.configurationItems["TLSlave"].RawValue, (IFormatProvider) CultureInfo.InvariantCulture) != 0, Convert.ToInt32(this.configurationItems["FlashBoot"].RawValue, (IFormatProvider) CultureInfo.InvariantCulture));
        this.initState = InitState.Online;
        this.hardwareAccessLevel = (int) CaesarRoot.SGetCurrentSecurityLevel();
        RollCallJ1708.GlobalInstance.Init();
        RollCallJ1939.GlobalInstance.Init();
        if (McdRoot.Initialized)
          McdRoot.PrepareVciAccessLayer();
        RollCallDoIP.GlobalInstance.Init();
      }
    }
    catch (CaesarErrorException ex)
    {
      CaesarException e = new CaesarException(ex);
      if (ex.ErrNo < 9900L || ex.ErrNo > 9920L)
        throw e;
      this.RaiseExceptionEvent((object) this, (Exception) e);
      this.CaesarConstruct();
    }
    catch (McdException ex)
    {
      this.RaiseExceptionEvent((object) this, (Exception) new CaesarException(ex));
    }
  }

  public void InitOffline(bool marshalEvents) => this.InternalInitOffline(marshalEvents);

  public static string TimeToString(DateTime time)
  {
    return time.ToString("yyyyMMddHHmmssfff", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal static string MakeTranslationIdentifier(params string[] args)
  {
    return string.Join(".", ((IEnumerable<string>) args).ToArray<string>());
  }

  public static DateTime TimeFromString(string time)
  {
    if (time == null)
      throw new ArgumentNullException(nameof (time));
    if (time.Length != 17)
      throw new ArgumentException("The time argument is not of the correct length");
    int year = ((int) time[0] - 48 /*0x30*/) * 1000 + ((int) time[1] - 48 /*0x30*/) * 100 + ((int) time[2] - 48 /*0x30*/) * 10 + ((int) time[3] - 48 /*0x30*/);
    int num1 = ((int) time[4] - 48 /*0x30*/) * 10 + ((int) time[5] - 48 /*0x30*/);
    int num2 = ((int) time[6] - 48 /*0x30*/) * 10 + ((int) time[7] - 48 /*0x30*/);
    int num3 = ((int) time[8] - 48 /*0x30*/) * 10 + ((int) time[9] - 48 /*0x30*/);
    int num4 = ((int) time[10] - 48 /*0x30*/) * 10 + ((int) time[11] - 48 /*0x30*/);
    int num5 = ((int) time[12] - 48 /*0x30*/) * 10 + ((int) time[13] - 48 /*0x30*/);
    int num6 = ((int) time[14] - 48 /*0x30*/) * 100 + ((int) time[15] - 48 /*0x30*/) * 10 + ((int) time[16 /*0x10*/] - 48 /*0x30*/);
    int month = num1;
    int day = num2;
    int hour = num3;
    int minute = num4;
    int second = num5;
    int millisecond = num6;
    return new DateTime(year, month, day, hour, minute, second, millisecond);
  }

  public void Exit()
  {
    if (this.initState != InitState.NotInitialized)
    {
      if (this.initState == InitState.Online)
      {
        this.channels.StopAutoConnect();
        this.channels.ClearEcusInAutoConnect();
        RollCallJ1708.GlobalInstance.Stop();
        RollCallJ1939.GlobalInstance.Stop();
        RollCallDoIP.GlobalInstance.Stop();
        this.channels.WaitForBackgroundConnection();
        while (this.channels.Count > 0)
          this.channels[0].Disconnect();
        this.channels.CleanupOffline();
        while (this.busMonitors.Count > 0)
          this.busMonitors[0].Stop();
        BusMonitorFrame.ClearIdentifierMap();
        try
        {
          CaesarRoot.CaesarComDestruct();
        }
        catch (NullReferenceException ex)
        {
          this.RaiseDebugInfoEvent((object) this, "Intentional catch of null reference exception during CaesarComDestruct");
        }
        if (McdRoot.IsVciAccessLayerPrepared)
          McdRoot.UnprepareVciAccessLayer();
      }
      this.initState = InitState.Offline;
      this.logFiles.StopLog();
      while (this.logFiles.Count > 0)
        this.logFiles[0].Close();
    }
    CaesarRoot.CaesarDestruct();
    RollCallJ1708.GlobalInstance.Exit();
    RollCallJ1939.GlobalInstance.Exit();
    RollCallDoIP.GlobalInstance.Exit();
    this.ecus.ClearList();
    this.protocols.ClearList();
    this.codingFiles.ClearList();
    this.flashFiles.ClearList();
    CaesarRoot.FreeLists();
    this.RefreshFlashFiles();
    if (McdRoot.Initialized)
    {
      McdRoot.ByteMessage -= new EventHandler<McdByteMessageEventArgs>(this.McdRoot_ByteMessage);
      McdRoot.DebugInfo -= new EventHandler<McdDebugInfoEventArgs>(this.McdRoot_DebugInfo);
      McdRoot.Destruct();
    }
    this.threadMarshalControl = (Control) null;
    this.initState = InitState.NotInitialized;
  }

  public void AddFlashFile(string path) => this.ModifyFlashFiles(path, true);

  public void RemoveFlashFile(string path) => this.ModifyFlashFiles(path, false);

  internal void EnsureCodingFilesLoaded()
  {
    if (this.codingFiles.Acquired)
      return;
    this.RaiseDebugInfoEvent((object) this, $"EnsureCodingFilesLoaded: {(object) this.codingFiles.Count} available coding files.");
  }

  internal void EnsureFlashFilesLoaded()
  {
    lock (this.flashFilesLock)
    {
      if (this.flashFilesLoaded)
        return;
      CaesarRoot.InternalInitAddFlashFiles(this.configurationItems["CFFFiles"].Value);
      McdRoot.LinkFlashFiles();
      this.flashFilesLoaded = true;
    }
  }

  public void RefreshFlashFiles()
  {
    lock (this.flashFilesLock)
    {
      this.flashFilesLoaded = false;
      foreach (Channel channel in (ChannelBaseCollection) this.channels)
        channel.FlashAreas.ResetList();
      this.flashFiles.ClearList();
    }
  }

  public void SetToolId(IEnumerable<byte> toolId)
  {
    if (this.initState != InitState.NotInitialized)
      throw new InvalidOperationException("Tool ID cannot be set after S-API is initialised");
    this.toolId = (IList<byte>) new List<byte>(toolId).AsReadOnly();
  }

  public static void SaveDataItemDescription(
    Protocol protocol,
    byte address,
    System.Type type,
    string qualifier,
    IDictionary<string, string> content)
  {
    RollCallSae.MonitorData.SaveDataItemDescription(protocol, address, type, qualifier, content);
  }

  internal IEnumerable<byte> ToolId => (IEnumerable<byte>) this.toolId;

  public InitState InitState => this.initState;

  public int ReadAccess
  {
    get => this.readAccess;
    set
    {
      CaesarRoot.CheckClientStrongName();
      this.readAccess = value;
    }
  }

  public int WriteAccess
  {
    get => this.writeAccess;
    set
    {
      CaesarRoot.CheckClientStrongName();
      this.writeAccess = value;
    }
  }

  public ConfigurationItemCollection ConfigurationItems => this.configurationItems;

  public EcuCollection Ecus => this.ecus;

  public DiagnosisProtocolCollection DiagnosisProtocols => this.protocols;

  public ChannelCollection Channels => this.channels;

  public LogFileCollection LogFiles => this.logFiles;

  public BusMonitorCollection BusMonitors => this.busMonitors;

  public string CaesarLibraryVersion => this.caesarLibraryVersion;

  public string CaesarLibraryCompilationDate => this.caesarLibraryCompilationDate;

  public static string McdSystemDescription => McdRoot.Description;

  public static string McdDatabaseLocation => McdRoot.DatabaseLocation;

  public CodingFileCollection CodingFiles => this.codingFiles;

  public FlashFileCollection FlashFiles => this.flashFiles;

  public int HardwareAccess => this.hardwareAccessLevel;

  public bool ValidateConfigurationFileChecksums { get; set; }

  public bool AllowAutoBaudRate
  {
    get => this.allowAutoBaudRate;
    set
    {
      if (value == this.allowAutoBaudRate)
        return;
      try
      {
        int num = (int) Sid.SetAllowAutoBaudRate(value);
        this.allowAutoBaudRate = value;
      }
      catch (SEHException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to set 'allow auto baud rate' as SID is not loaded");
      }
    }
  }

  internal Func<Ecu, string, Stream> GetConfiguration => this.getConfiguration;

  internal Action<Ecu, Stream, XmlNode> ReleaseConfiguration => this.releaseConfiguration;

  public void SetConfigurationFileProvider(
    Func<Ecu, string, Stream> getConfiguration,
    Action<Ecu, Stream, XmlNode> releaseConfiguration)
  {
    this.getConfiguration = getConfiguration;
    this.releaseConfiguration = releaseConfiguration;
  }

  internal static DateTime Now
  {
    get
    {
      if (!Sapi.utcOffset.HasValue)
        Sapi.utcOffset = new TimeSpan?(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));
      return DateTime.UtcNow + Sapi.utcOffset.Value;
    }
  }

  public event DebugInfoEventHandler DebugInfoEvent;

  public event ExceptionEventHandler ExceptionEvent;

  public event FatalEventHandler FatalEvent;

  public event ByteMessageEventHandler ByteMessageEvent;

  internal event ByteMessageEventHandler ByteMessageInternalEvent;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void CaesarConstruct()
  {
    CaesarRoot.SetLicenseFile();
    try
    {
      CaesarRoot.CaesarConstruct(this.toolId != null ? this.toolId.ToArray<byte>() : (byte[]) null, Convert.ToInt32(this.configurationItems["Units"].RawValue, (IFormatProvider) CultureInfo.InvariantCulture), Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gbf"), this.configurationItems["CCFFiles"].Value);
      CaesarRoot.Authenticate("SAPI", new byte[256 /*0x0100*/]
      {
        (byte) 178,
        (byte) 8,
        (byte) 204,
        byte.MaxValue,
        (byte) 37,
        (byte) 109,
        (byte) 19,
        (byte) 182,
        (byte) 157,
        (byte) 112 /*0x70*/,
        (byte) 78,
        (byte) 166,
        (byte) 188,
        (byte) 249,
        (byte) 54,
        (byte) 231,
        (byte) 213,
        (byte) 139,
        (byte) 207,
        (byte) 1,
        (byte) 44,
        (byte) 73,
        (byte) 230,
        (byte) 61,
        (byte) 200,
        (byte) 100,
        (byte) 114,
        (byte) 235,
        (byte) 182,
        (byte) 164,
        (byte) 203,
        (byte) 235,
        (byte) 238,
        (byte) 99,
        (byte) 194,
        (byte) 137,
        (byte) 103,
        (byte) 241,
        (byte) 142,
        (byte) 79,
        (byte) 116,
        (byte) 1,
        (byte) 116,
        (byte) 153,
        (byte) 139,
        (byte) 220,
        (byte) 236,
        (byte) 90,
        (byte) 190,
        (byte) 31 /*0x1F*/,
        (byte) 93,
        (byte) 82,
        (byte) 37,
        (byte) 244,
        (byte) 126,
        (byte) 235,
        (byte) 98,
        (byte) 6,
        (byte) 185,
        (byte) 92,
        (byte) 160 /*0xA0*/,
        (byte) 160 /*0xA0*/,
        (byte) 17,
        (byte) 47,
        (byte) 231,
        (byte) 78,
        (byte) 109,
        (byte) 220,
        (byte) 137,
        (byte) 32 /*0x20*/,
        (byte) 220,
        (byte) 28,
        (byte) 48 /*0x30*/,
        (byte) 230,
        (byte) 5,
        (byte) 97,
        (byte) 163,
        (byte) 29,
        (byte) 6,
        (byte) 37,
        (byte) 150,
        (byte) 252,
        (byte) 95,
        (byte) 91,
        (byte) 71,
        (byte) 170,
        (byte) 66,
        (byte) 61,
        (byte) 36,
        (byte) 8,
        (byte) 179,
        (byte) 195,
        (byte) 154,
        (byte) 181,
        (byte) 168,
        (byte) 58,
        (byte) 160 /*0xA0*/,
        (byte) 163,
        (byte) 117,
        (byte) 195,
        (byte) 199,
        (byte) 252,
        (byte) 15,
        (byte) 143,
        (byte) 187,
        (byte) 62,
        (byte) 127 /*0x7F*/,
        (byte) 11,
        (byte) 219,
        (byte) 49,
        byte.MaxValue,
        (byte) 46,
        (byte) 101,
        (byte) 146,
        (byte) 71,
        (byte) 61,
        (byte) 73,
        (byte) 194,
        (byte) 133,
        (byte) 75,
        (byte) 247,
        (byte) 191,
        (byte) 225,
        (byte) 122,
        (byte) 238,
        (byte) 144 /*0x90*/,
        (byte) 13,
        (byte) 130,
        (byte) 67,
        (byte) 102,
        (byte) 37,
        (byte) 228,
        (byte) 181,
        (byte) 192 /*0xC0*/,
        (byte) 96 /*0x60*/,
        (byte) 235,
        (byte) 231,
        (byte) 37,
        (byte) 224 /*0xE0*/,
        (byte) 35,
        (byte) 113,
        (byte) 23,
        (byte) 123,
        (byte) 43,
        (byte) 41,
        (byte) 130,
        (byte) 128 /*0x80*/,
        (byte) 125,
        (byte) 157,
        (byte) 59,
        (byte) 214,
        (byte) 182,
        (byte) 124,
        (byte) 192 /*0xC0*/,
        (byte) 108,
        (byte) 148,
        (byte) 3,
        (byte) 150,
        (byte) 159,
        (byte) 29,
        (byte) 195,
        (byte) 165,
        (byte) 80 /*0x50*/,
        (byte) 145,
        (byte) 252,
        (byte) 103,
        (byte) 201,
        (byte) 213,
        (byte) 38,
        (byte) 82,
        (byte) 221,
        (byte) 233,
        (byte) 123,
        (byte) 40,
        (byte) 111,
        (byte) 50,
        (byte) 21,
        (byte) 110,
        (byte) 181,
        (byte) 180,
        (byte) 0,
        (byte) 223,
        (byte) 203,
        (byte) 206,
        (byte) 166,
        (byte) 186,
        (byte) 99,
        (byte) 44,
        (byte) 187,
        (byte) 157,
        (byte) 215,
        (byte) 80 /*0x50*/,
        (byte) 184,
        (byte) 61,
        (byte) 238,
        (byte) 205,
        (byte) 172,
        (byte) 78,
        (byte) 152,
        (byte) 4,
        (byte) 107,
        (byte) 184,
        (byte) 214,
        (byte) 140,
        (byte) 26,
        (byte) 120,
        (byte) 78,
        (byte) 180,
        (byte) 61,
        (byte) 219,
        (byte) 33,
        (byte) 177,
        (byte) 212,
        (byte) 225,
        (byte) 237,
        (byte) 249,
        (byte) 253,
        (byte) 51,
        (byte) 193,
        (byte) 54,
        (byte) 37,
        (byte) 250,
        (byte) 223,
        (byte) 227,
        (byte) 129,
        (byte) 191,
        (byte) 137,
        (byte) 25,
        (byte) 15,
        (byte) 164,
        (byte) 209,
        (byte) 172,
        (byte) 120,
        (byte) 234,
        (byte) 12,
        (byte) 200,
        (byte) 17,
        (byte) 12,
        (byte) 34,
        (byte) 147,
        (byte) 79,
        (byte) 45,
        (byte) 100,
        (byte) 190,
        (byte) 143,
        (byte) 49,
        (byte) 145,
        (byte) 17,
        (byte) 210,
        (byte) 174,
        (byte) 100,
        (byte) 190,
        (byte) 3,
        (byte) 166,
        (byte) 186,
        (byte) 215
      });
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      throw new CaesarException(ex, negativeResponseCode);
    }
  }

  private bool EmitNativeLibraryVersion(string identifier, string path)
  {
    ProcessModule processModule = (ProcessModule) null;
    string str = $"{identifier} path: {path}";
    string s;
    if (File.Exists(path))
    {
      try
      {
        McdRoot.ValidateNativeLibrary(path, true);
        processModule = Process.GetCurrentProcess().Modules.OfType<ProcessModule>().FirstOrDefault<ProcessModule>((Func<ProcessModule, bool>) (m => string.Equals(m.ModuleName, Path.GetFileName(path), StringComparison.OrdinalIgnoreCase)));
        s = str + (processModule != null ? " version: " + processModule.FileVersionInfo.FileVersion : "<loaded module not located>");
      }
      catch (Exception ex) when (ex is BadImageFormatException || ex is DllNotFoundException)
      {
        s = $"{str} validation exception: {ex.Message}";
      }
    }
    else
      s = str + " <file not found>";
    this.RaiseDebugInfoEvent((object) this, s);
    return processModule != null;
  }

  private static string GetSidDll()
  {
    string name = "Software\\PassThruSupport\\Detroit Diesel\\Devices\\SID";
    string sidDll = "";
    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
    if (registryKey != null)
      sidDll = registryKey.GetValue("FunctionLibrary") as string;
    return sidDll;
  }

  private void InternalInitOffline(bool marshalEvents)
  {
    if (this.initState != InitState.NotInitialized)
      this.Exit();
    if (marshalEvents)
    {
      this.threadMarshalControl = new Control();
      this.threadMarshalControl.CreateControl();
    }
    this.RaiseDebugInfoEvent((object) this, $"S-API path: {Assembly.GetExecutingAssembly().Location} version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
    string sidDll = Sapi.GetSidDll();
    if (!string.IsNullOrEmpty(sidDll))
      this.EmitNativeLibraryVersion("SID", sidDll);
    else
      this.RaiseDebugInfoEvent((object) this, "SID path:  <was not specified in the registry>");
    ushort uint16 = Convert.ToUInt16(this.configurationItems["DebugLevel"].RawValue, (IFormatProvider) CultureInfo.InvariantCulture);
    CaesarRoot.SetByteMessageCallbacks(new CaesarAbstraction.ByteSendCallback(this.ByteSendCallback), new CaesarAbstraction.ByteReceiveCallback(this.ByteReceiveCallback));
    CaesarRoot.SetDebugInfoCallback(new CaesarAbstraction.DebugInfoCallback(this.DebugInfoCallback), uint16);
    CaesarRoot.SetChannelDebugInfoCallback(new CaesarAbstraction.ChannelDebugInfoCallback(this.ChannelDebugInfoCallback), uint16);
    CaesarRoot.SetPeriodicCallback(new CaesarAbstraction.PeriodicCallback(this.PeriodicCallback));
    CaesarRoot.SetFatalFunctionCallback(new CaesarAbstraction.FatalFunctionCallback(this.FatalFunctionCallback));
    this.CaesarConstruct();
    this.initState = InitState.Offline;
    this.caesarLibraryVersion = CaesarRoot.LibraryVersion;
    this.caesarLibraryCompilationDate = CaesarRoot.LibraryCompilationDate;
    this.PresentationCulture = Thread.CurrentThread.CurrentUICulture;
    if (!this.UseMcd)
      return;
    try
    {
      string path2 = this.configurationItems["McdDtsPath"].Value;
      if (!string.IsNullOrEmpty(path2))
      {
        string str1 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path2);
        this.RaiseDebugInfoEvent((object) this, "MCD: DTS path: " + str1);
        Sapi.SetDtsPathEnvironmentVariable(str1);
        if (this.mcdKeyFunc != null && this.mcdProcedureId.HasValue)
        {
          McdRoot.Construct(str1, this.mcdProcedureId.Value, this.mcdKeyFunc, this.toolId != null ? this.toolId.ToArray<byte>() : (byte[]) null);
          if (McdRoot.Initialized)
          {
            this.RaiseDebugInfoEvent((object) this, $"MCD: ** using MCD-3D {McdRoot.Description} **");
            this.RaiseDebugInfoEvent((object) this, "MCD: JVM location: " + McdRoot.JavaVirtualMachineLocation);
            string str2 = this.configurationItems["McdRootDescriptionFile"].Value;
            if (string.IsNullOrEmpty(str2))
              str2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "pdu_api_root.xml");
            this.RaiseDebugInfoEvent((object) this, "MCD: PDU-API root description path: " + str2);
            if (File.Exists(str2))
            {
              foreach (KeyValuePair<string, string> descriptionFileLibraryPath in (IEnumerable<KeyValuePair<string, string>>) McdRoot.GetRootDescriptionFileLibraryPaths(str2))
              {
                this.EmitNativeLibraryVersion(descriptionFileLibraryPath.Key, descriptionFileLibraryPath.Value);
                if (descriptionFileLibraryPath.Key == "BOSCH_DS_D_PDU_API")
                  this.EmitNativeLibraryVersion(descriptionFileLibraryPath.Key, Path.Combine(Path.GetDirectoryName(descriptionFileLibraryPath.Value), "PDUAPI_Bosch.dll"));
              }
            }
            else
              this.RaiseDebugInfoEvent((object) this, "MCD: PDU-API: unable to read content as the root description file was not at the specified path");
            McdRoot.SetRootDescriptionFile(str2);
            string path = this.configurationItems["McdSessionProjectPath"].Value;
            if (!string.IsNullOrEmpty(path))
            {
              this.RaiseDebugInfoEvent((object) this, "MCD: Session project path: " + path);
              McdRoot.SetSessionProjectPath(path);
            }
            McdRoot.DebugInfo += new EventHandler<McdDebugInfoEventArgs>(this.McdRoot_DebugInfo);
            string projectName = this.configurationItems["McdProjectName"].Value;
            this.RaiseDebugInfoEvent((object) this, "MCD: Project name: " + projectName);
            McdRoot.SelectProject(projectName);
            string name = this.configurationItems["McdVehicleInformationTable"].Value;
            this.RaiseDebugInfoEvent((object) this, "MCD: Vehicle Information Table: " + name);
            McdRoot.SelectVehicleInformation(name);
            this.RaiseDebugInfoEvent((object) this, "MCD: com.softing.AdditionalClassPath = " + McdRoot.GetProperty("com.softing.AdditionalClassPath"));
            this.RaiseDebugInfoEvent((object) this, "MCD: com.softing.GlobalSmrFilePath = " + McdRoot.GetProperty("com.softing.GlobalSmrFilePath"));
            McdRoot.ByteMessage += new EventHandler<McdByteMessageEventArgs>(this.McdRoot_ByteMessage);
          }
          else
            this.RaiseDebugInfoEvent((object) this, "** MCD-3D system could not be initialized **");
        }
        else
          this.RaiseDebugInfoEvent((object) this, "** MCD-3D system requires key function and procedure id **");
      }
      else
        this.RaiseDebugInfoEvent((object) this, "** MCD-3D system path is not specified **");
    }
    catch (FileNotFoundException ex)
    {
      this.RaiseDebugInfoEvent((object) this, "Error attempting to load McdAbstraction: " + ex.Message);
    }
  }

  public void ValidateMcd()
  {
    string mcdPath = this.configurationItems["McdDtsPath"].Value;
    if (string.IsNullOrEmpty(mcdPath))
      throw new DirectoryNotFoundException("MCD-3D system path is not specified");
    Sapi.ValidateMcd(mcdPath);
  }

  private static void SetDtsPathEnvironmentVariable(string mcdPath)
  {
    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DTS_PATH")) || !File.Exists(Path.Combine(mcdPath, "csWrap.dll")))
      return;
    Environment.SetEnvironmentVariable("DTS_PATH", Directory.GetParent(mcdPath).FullName);
  }

  public static void ValidateMcd(string mcdPath)
  {
    Sapi.SetDtsPathEnvironmentVariable(mcdPath);
    McdRoot.ValidateNativeLibrary(Path.Combine(mcdPath, "csWrap.dll"), false);
  }

  private void McdRoot_DebugInfo(object sender, McdDebugInfoEventArgs e)
  {
    McdLogicalLink link;
    if ((link = sender as McdLogicalLink) != null)
    {
      Channel sender1 = this.channels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.IsOwner(link)));
      if (sender1 != null)
        this.RaiseDebugInfoEvent((object) sender1, e.Message);
      else
        this.RaiseDebugInfoEvent((object) link.DBLocation.Qualifier, e.Message);
    }
    else
      this.RaiseDebugInfoEvent((object) typeof (McdRoot), e.Message);
  }

  private void McdRoot_ByteMessage(object sender, McdByteMessageEventArgs e)
  {
    McdLogicalLink link = sender as McdLogicalLink;
    Channel sender1 = this.channels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.IsOwner(link)));
    if (sender1 == null)
      return;
    this.RaiseByteMessageEvent((object) sender1, e.IsSend ? ByteMessageDirection.TX : ByteMessageDirection.RX, new Dump(e.Message));
  }

  public bool UseMcd
  {
    get => this.useMcd;
    set => this.useMcd = value;
  }

  public void SetMcdKeyProvider(int mcdProcedureId, Func<byte[], byte[]> mcdKeyFunc)
  {
    this.mcdProcedureId = new int?(mcdProcedureId);
    this.mcdKeyFunc = mcdKeyFunc;
  }

  internal CultureInfo PresentationCulture { private set; get; }

  private void Dispose(bool disposing)
  {
    if (!Sapi.disposed && disposing)
    {
      this.Exit();
      Sapi.sapi = (Sapi) null;
      if (this.threadMarshalControl != null)
      {
        this.threadMarshalControl.Dispose();
        this.threadMarshalControl = (Control) null;
      }
    }
    Sapi.disposed = true;
  }

  private void RaiseByteMessageEvent(object sender, ByteMessageDirection direction, Dump data)
  {
    ByteMessageEventArgs e = new ByteMessageEventArgs(direction, data);
    FireAndForget.Invoke((MulticastDelegate) this.ByteMessageEvent, sender, (EventArgs) e);
    ByteMessageEventHandler messageInternalEvent = this.ByteMessageInternalEvent;
    if (messageInternalEvent == null)
      return;
    messageInternalEvent(sender, e);
  }

  internal static XmlNode ReadSapiXmlFile(
    XmlDocument doc,
    string cl,
    out int ver,
    out DateTime dt)
  {
    XmlNode xmlNode = doc.SelectSingleNode("/SapiFile");
    string innerText = ((xmlNode != null ? xmlNode.Attributes.GetNamedItem("xsi:noNamespaceSchemaLocation") : throw new XmlException("XML data did not contain SapiFile root node")) ?? throw new XmlException("XML data did not correctly specify a schema")).InnerText;
    string[] strArray = innerText.Split("_.".ToCharArray());
    if (!string.Equals(strArray[0], cl, StringComparison.Ordinal))
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Class of XML data ({0}) did not match desired ({1})", (object) strArray[0], (object) cl));
    XmlNode namedItem = xmlNode.Attributes.GetNamedItem("Time");
    ver = Convert.ToInt32(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
    dt = Sapi.TimeFromString(namedItem.InnerText);
    XmlReader schemaDocument = XmlReader.Create((TextReader) new StringReader(Sapi.GetResourceSchema(innerText) ?? throw new FileNotFoundException(innerText)));
    doc.Schemas.Add((string) null, schemaDocument);
    doc.Validate(new ValidationEventHandler(Sapi.XmlValidationEventHandler));
    return xmlNode;
  }

  private static void ReadSapiXmlFile(
    XmlReader xmlReader,
    string cl,
    out int ver,
    out DateTime dt)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    if (!xmlReader.ReadToFollowing("SapiFile"))
      throw new XmlException("XML data did not contain SapiFile root node");
    XNamespace xnamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
    string[] strArray = (xmlReader.GetAttribute("noNamespaceSchemaLocation", xnamespace.NamespaceName) ?? throw new XmlException("XML data did not correctly specify a schema")).Split("_.".ToCharArray());
    if (!string.Equals(strArray[0], cl, StringComparison.Ordinal))
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Class of XML data ({0}) did not match desired ({1})", (object) strArray[0], (object) cl));
    string attribute = xmlReader.GetAttribute("Time");
    ver = Convert.ToInt32(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
    dt = Sapi.TimeFromString(attribute);
  }

  private static void EmbellishPath(Assembly assembly)
  {
    string environmentVariable = Environment.GetEnvironmentVariable("PATH");
    Environment.SetEnvironmentVariable("PATH", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0};{1}", (object) Path.GetDirectoryName(assembly.Location), (object) environmentVariable));
  }

  private static string GetResourceSchema(string name)
  {
    string resourceSchema = (string) null;
    string name1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SapiLayer1.{0}", (object) name);
    Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name1);
    if (manifestResourceStream != null)
    {
      using (StreamReader streamReader = new StreamReader(manifestResourceStream))
        resourceSchema = streamReader.ReadToEnd();
      manifestResourceStream.Close();
    }
    return resourceSchema;
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("ReadAccessLevel is deprecated due to non-CLS compliance, please use ReadAccess instead.")]
  public ushort ReadAccessLevel
  {
    get => (ushort) this.ReadAccess;
    set => this.ReadAccess = (int) value;
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("WriteAccessLevel is deprecated due to non-CLS compliance, please use WriteAccess instead.")]
  public ushort WriteAccessLevel
  {
    get => (ushort) this.WriteAccess;
    set => this.WriteAccess = (int) value;
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("HardwareAccessLevel is deprecated due to non-CLS compliance, please use HardwareAccess instead.")]
  public ushort HardwareAccessLevel => (ushort) this.hardwareAccessLevel;

  private class XmlEmbeddedResourceResolver : XmlUrlResolver
  {
    public override object GetEntity(Uri absoluteUri, string role, System.Type ofObjectToReturn)
    {
      if (!(absoluteUri.Scheme == "file"))
        return base.GetEntity(absoluteUri, role, ofObjectToReturn);
      return (object) new MemoryStream(Encoding.UTF8.GetBytes(Sapi.GetResourceSchema(Path.GetFileName(absoluteUri.LocalPath)) ?? throw new FileNotFoundException(absoluteUri.LocalPath)));
    }
  }
}
