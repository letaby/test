// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdRoot
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

#nullable disable
namespace McdAbstraction;

public static class McdRoot
{
  private static bool initialized;
  private static bool projectSelected;
  private static bool vehicleInformationSelected;
  private static bool vciAccessLayerPrepared;
  private static string description;
  private static MCDSystem dts;
  private static MCDProject theProject;
  private static MCDDbVehicleInformation theVehicleInformation;
  private static Regex NameCollisionExtractPaths = new Regex(".*\"(?<path1>.*)\".*\"(?<path2>.*)\"", RegexOptions.Compiled);
  private static Regex VersionExtract = new Regex("\\d*\\.\\d*\\.\\d*", RegexOptions.Compiled);
  private static object detectInterfacesLock = new object();
  private static IEnumerable<McdDBLogicalLink> dbLogicalLinks;
  private static IEnumerable<McdDBLogicalLink> dbProtocolLogicalLinks;
  private static Dictionary<string, McdDBConfigurationData> dbconfigurationdatas;
  private static Dictionary<string, McdDBLocation> protocols;
  private static List<McdInterface> currentInterfaces = new List<McdInterface>();
  private static Dictionary<string, McdDBEcuMem> dbecumems;
  private static Dictionary<string, McdDBEcuBaseVariant> ecus;
  private static Dictionary<string, string> additionalAssemblies = new Dictionary<string, string>();

  public static event EventHandler<McdByteMessageEventArgs> ByteMessage;

  public static event EventHandler<McdDebugInfoEventArgs> DebugInfo;

  public static bool Initialized => McdRoot.initialized;

  public static string GetProperty(string name)
  {
    return McdRoot.Initialized ? McdRoot.dts.GetProperty(name)?.ValueAsString : (string) null;
  }

  public static void SetProperty(string name, string value)
  {
    MCDValue mcdValue = Statics.createValue();
    mcdValue.DataType = MCDDataType.eA_ASCIISTRING;
    mcdValue.Asciistring = value;
    McdRoot.dts.SetProperty(name, mcdValue);
  }

  public static void Construct(
    string dtsPath,
    int procedureId,
    Func<byte[], byte[]> mcdKeyFunc,
    byte[] testerId)
  {
    if (!McdRoot.additionalAssemblies.Any<KeyValuePair<string, string>>())
      McdRoot.GatherAdditionalAssemblies(dtsPath);
    if (!McdRoot.additionalAssemblies.Any<KeyValuePair<string, string>>())
      return;
    McdRoot.Destruct();
    try
    {
      McdRoot.dts = Statics.getSystem();
      ((DtsSystem) McdRoot.dts).Initialize();
      byte[] seed = ((DtsSystem) McdRoot.dts).GetSeed((uint) procedureId, DtsAppID.eAPPID_App14);
      byte[] key = mcdKeyFunc(seed);
      ((DtsSystem) McdRoot.dts).SendKey(key);
      ((DtsSystem) McdRoot.dts).Initialize();
      McdRoot.description = McdRoot.dts.Description;
      MCDValue mcdValue = Statics.createValue();
      mcdValue.DataType = MCDDataType.eA_BOOLEAN;
      mcdValue.Boolean = false;
      McdRoot.dts.SetProperty("com.softing.CheckExactLinkStates", mcdValue);
      if (testerId != null)
        McdRoot.SetProperty("com.softing.Identification.TesterId", (BitConverter.ToUInt32(((IEnumerable<byte>) ((IEnumerable<byte>) testerId).ToArray<byte>()).Take<byte>(4).Reverse<byte>().ToArray<byte>(), 0) & 67108863U /*0x03FFFFFF*/).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      McdRoot.initialized = true;
    }
    catch (MCDException ex)
    {
      McdRoot.initialized = false;
      throw new McdException(ex, nameof (Construct));
    }
  }

  public static void Destruct()
  {
    McdRoot.DeselectProject();
    if (!McdRoot.initialized)
      return;
    try
    {
      ((DtsSystem) McdRoot.dts).Uninitialize();
      McdRoot.initialized = false;
      McdRoot.dts = (MCDSystem) null;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (Destruct));
    }
  }

  internal static MCDSystem Dts => McdRoot.dts;

  public static string Description => McdRoot.description;

  public static string ConfigurationFilePath
  {
    get
    {
      return Path.Combine(((DtsSystem) McdRoot.dts).ConfigurationPath, Environment.Is64BitProcess ? "DtsConfig64.xml" : "DtsConfig.xml");
    }
  }

  public static string JavaVirtualMachineLocation
  {
    get => ((DtsSystem) McdRoot.dts).Configuration.JavaConfig.CurrentJvmLocation.FilePath;
  }

  public static void SetRootDescriptionFile(string path)
  {
    if (string.IsNullOrEmpty(path))
      return;
    try
    {
      ((DtsSystem) McdRoot.dts).Configuration.RootDescriptionFile = path;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (SetRootDescriptionFile));
    }
  }

  public static void SetSessionProjectPath(string path)
  {
    if (string.IsNullOrEmpty(path))
      return;
    try
    {
      ((DtsSystem) McdRoot.dts).SessionProjectPath = path;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (SetSessionProjectPath));
    }
  }

  internal static string GetPreamble(string path)
  {
    using (BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(path)))
    {
      binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
      byte[] source = binaryReader.ReadBytes(16384 /*0x4000*/);
      int num = (int) source[24] + ((int) source[25] << 8);
      if (num < 16384 /*0x4000*/)
        return Encoding.UTF8.GetString(((IEnumerable<byte>) source).Skip<byte>(88).Take<byte>(num - 88).ToArray<byte>());
    }
    return (string) null;
  }

  private static Version GetInternalVersion(string path)
  {
    string preamble = McdRoot.GetPreamble(path);
    if (preamble != null)
    {
      using (StringReader stringReader = new StringReader(preamble))
      {
        string str;
        while ((str = stringReader.ReadLine()) != null)
        {
          string[] strArray = str.Split(":".ToCharArray());
          if (strArray[0] == "REVISION INFO")
          {
            try
            {
              return new Version(strArray[1].Trim());
            }
            catch (ArgumentException ex)
            {
            }
          }
        }
      }
    }
    return (Version) null;
  }

  private static FileInfo DetermineOldestFile(FileInfo fileInfo1, FileInfo fileInfo2)
  {
    Version fileVersion1 = getFileVersion(fileInfo1.Name);
    Version fileVersion2 = getFileVersion(fileInfo2.Name);
    return fileVersion1 != (Version) null && fileVersion2 != (Version) null ? (fileVersion1 > fileVersion2 ? fileInfo2 : fileInfo1) : (fileInfo1.LastWriteTimeUtc > fileInfo2.LastWriteTimeUtc ? fileInfo2 : fileInfo1);

    static Version getFileVersion(string name)
    {
      MatchCollection matchCollection = McdRoot.VersionExtract.Matches(name);
      if (matchCollection.Count > 0 && matchCollection[0].Success)
      {
        try
        {
          return new Version(matchCollection[0].Value);
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case FormatException _:
            case ArgumentException _:
              num = 1;
              break;
            default:
              num = ex is OverflowException ? 1 : 0;
              break;
          }
          if (num != 0)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
        }
      }
      return (Version) null;
    }
  }

  private static bool IsSubPathOf(string path, DirectoryInfo directory)
  {
    if (directory == null)
      return false;
    return directory.FullName.Equals(path, StringComparison.OrdinalIgnoreCase) || McdRoot.IsSubPathOf(path, directory.Parent);
  }

  public static void SelectProject(string projectName)
  {
    McdRoot.DeselectProject();
    try
    {
      while (true)
      {
        try
        {
          McdRoot.theProject = McdRoot.dts.SelectProjectByName(projectName);
          break;
        }
        catch (MCDException ex)
        {
          if (ex.Error.VendorCode == (ushort) 54160)
          {
            Match match = McdRoot.NameCollisionExtractPaths.Match(ex.Error.VendorCodeDescription);
            if (match.Success && match.Groups.Count == 3)
            {
              FileInfo fileInfo1 = new FileInfo(match.Groups[1].Value);
              FileInfo fileInfo2 = new FileInfo(match.Groups[2].Value);
              if (!McdRoot.IsSubPathOf(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileInfo1.Directory) && fileInfo1.DirectoryName == fileInfo2.DirectoryName)
              {
                Version internalVersion1 = McdRoot.GetInternalVersion(fileInfo1.FullName);
                Version internalVersion2 = McdRoot.GetInternalVersion(fileInfo2.FullName);
                FileInfo fileInfo;
                if (internalVersion1 != (Version) null && internalVersion2 != (Version) null)
                {
                  McdRoot.RaiseDebugInfoEvent((McdLogicalLink) null, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found diagnosis files with name collision: {0} ({1}) and {2} ({3})", (object) fileInfo1.Name, (object) internalVersion1, (object) fileInfo2.Name, (object) internalVersion2));
                  fileInfo = internalVersion1 > internalVersion2 ? fileInfo2 : fileInfo1;
                }
                else
                {
                  McdRoot.RaiseDebugInfoEvent((McdLogicalLink) null, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found diagnosis files with name collision: {0} and {1}", (object) fileInfo1.Name, (object) fileInfo2.Name));
                  fileInfo = McdRoot.DetermineOldestFile(fileInfo1, fileInfo2);
                }
                McdRoot.RaiseDebugInfoEvent((McdLogicalLink) null, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Removing older file {0}", (object) fileInfo.Name));
                string str = fileInfo.FullName + ".bak";
                if (!File.Exists(str))
                {
                  File.Move(fileInfo.FullName, str);
                  continue;
                }
                File.Delete(fileInfo.FullName);
                continue;
              }
            }
          }
          else if (ex.Error.VendorCode == (ushort) 49417)
          {
            string configurationFilePath = McdRoot.ConfigurationFilePath;
            if (!File.Exists(configurationFilePath))
              throw new McdException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The project '{0}' could not be selected. The MCD-3D Configuration file is missing at {1}.", (object) projectName, (object) configurationFilePath));
          }
          throw;
        }
      }
      McdRoot.ecus = ((IEnumerable<string>) McdRoot.theProject.DbProject.DbEcuBaseVariants.Names).Select<string, KeyValuePair<string, McdDBEcuBaseVariant>>((Func<string, KeyValuePair<string, McdDBEcuBaseVariant>>) (n => new KeyValuePair<string, McdDBEcuBaseVariant>(n, (McdDBEcuBaseVariant) null))).ToDictionary<KeyValuePair<string, McdDBEcuBaseVariant>, string, McdDBEcuBaseVariant>((Func<KeyValuePair<string, McdDBEcuBaseVariant>, string>) (k => k.Key), (Func<KeyValuePair<string, McdDBEcuBaseVariant>, McdDBEcuBaseVariant>) (v => v.Value));
      McdRoot.protocols = ((IEnumerable<string>) McdRoot.theProject.DbProject.DbProtocolLocations.Names).Select<string, KeyValuePair<string, McdDBLocation>>((Func<string, KeyValuePair<string, McdDBLocation>>) (n => new KeyValuePair<string, McdDBLocation>(n, (McdDBLocation) null))).ToDictionary<KeyValuePair<string, McdDBLocation>, string, McdDBLocation>((Func<KeyValuePair<string, McdDBLocation>, string>) (k => k.Key), (Func<KeyValuePair<string, McdDBLocation>, McdDBLocation>) (v => v.Value));
      McdRoot.dbecumems = ((IEnumerable<string>) McdRoot.theProject.DbProject.DbEcuMems.Names).Select<string, KeyValuePair<string, McdDBEcuMem>>((Func<string, KeyValuePair<string, McdDBEcuMem>>) (n => new KeyValuePair<string, McdDBEcuMem>(n, (McdDBEcuMem) null))).ToDictionary<KeyValuePair<string, McdDBEcuMem>, string, McdDBEcuMem>((Func<KeyValuePair<string, McdDBEcuMem>, string>) (k => k.Key), (Func<KeyValuePair<string, McdDBEcuMem>, McdDBEcuMem>) (v => v.Value));
      McdRoot.dbconfigurationdatas = ((IEnumerable<string>) ((DtsDbProject) McdRoot.theProject.DbProject).DbConfigurationDatas.Names).Select<string, KeyValuePair<string, McdDBConfigurationData>>((Func<string, KeyValuePair<string, McdDBConfigurationData>>) (n => new KeyValuePair<string, McdDBConfigurationData>(n, (McdDBConfigurationData) null))).ToDictionary<KeyValuePair<string, McdDBConfigurationData>, string, McdDBConfigurationData>((Func<KeyValuePair<string, McdDBConfigurationData>, string>) (k => k.Key), (Func<KeyValuePair<string, McdDBConfigurationData>, McdDBConfigurationData>) (v => v.Value));
      McdRoot.projectSelected = true;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (SelectProject));
    }
  }

  internal static IEnumerable<Tuple<string, string, string>> DatabaseFileList
  {
    get
    {
      return McdRoot.theProject != null ? (IEnumerable<Tuple<string, string, string>>) ((DtsProject) McdRoot.theProject).DatabaseFileList.OfType<DtsFileLocation>().Select<DtsFileLocation, Tuple<string, string, string>>((Func<DtsFileLocation, Tuple<string, string, string>>) (fl => Tuple.Create<string, string, string>(fl.FilePath, fl.ShortName, fl.Version))).ToList<Tuple<string, string, string>>() : (IEnumerable<Tuple<string, string, string>>) new List<Tuple<string, string, string>>();
    }
  }

  public static string GetDatabaseFileVersion(string path)
  {
    if (McdRoot.theProject != null)
    {
      DtsFileLocation dtsFileLocation = ((DtsProject) McdRoot.theProject).DatabaseFileList.OfType<DtsFileLocation>().FirstOrDefault<DtsFileLocation>((Func<DtsFileLocation, bool>) (fl => fl.FilePath == path));
      if (dtsFileLocation != null)
        return dtsFileLocation.Version;
    }
    return (string) null;
  }

  public static string DatabaseLocation
  {
    get
    {
      string path = McdRoot.DatabaseFileList.FirstOrDefault<Tuple<string, string, string>>()?.Item1;
      return path != null ? Path.GetDirectoryName(path) : (string) null;
    }
  }

  private static void DeselectProject()
  {
    if (!McdRoot.projectSelected)
      return;
    McdRoot.DeselectVehicleInformation();
    try
    {
      McdRoot.dts.DeselectProject();
      McdRoot.projectSelected = false;
      McdRoot.theProject = (MCDProject) null;
      McdRoot.ecus = (Dictionary<string, McdDBEcuBaseVariant>) null;
      McdRoot.dbecumems = (Dictionary<string, McdDBEcuMem>) null;
      McdRoot.protocols = (Dictionary<string, McdDBLocation>) null;
      McdRoot.dbconfigurationdatas = (Dictionary<string, McdDBConfigurationData>) null;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (DeselectProject));
    }
  }

  public static void SelectVehicleInformation(string name)
  {
    McdRoot.DeselectVehicleInformation();
    try
    {
      McdRoot.theVehicleInformation = McdRoot.theProject.SelectDbVehicleInformationByName(name);
      McdRoot.vehicleInformationSelected = true;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "SelectVit");
    }
  }

  private static void DeselectVehicleInformation()
  {
    if (!McdRoot.vehicleInformationSelected)
      return;
    try
    {
      McdRoot.theProject.DeselectVehicleInformation();
      McdRoot.theVehicleInformation = (MCDDbVehicleInformation) null;
      McdRoot.dbLogicalLinks = (IEnumerable<McdDBLogicalLink>) null;
      McdRoot.dbProtocolLogicalLinks = (IEnumerable<McdDBLogicalLink>) null;
      McdRoot.vehicleInformationSelected = false;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (DeselectVehicleInformation));
    }
  }

  public static bool IsVciAccessLayerPrepared => McdRoot.vciAccessLayerPrepared;

  public static void PrepareVciAccessLayer()
  {
    McdRoot.UnprepareVciAccessLayer();
    try
    {
      McdRoot.dts.PrepareVciAccessLayer();
      McdRoot.vciAccessLayerPrepared = true;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (PrepareVciAccessLayer));
    }
  }

  public static void DetectInterfaces(string option)
  {
    try
    {
      lock (McdRoot.detectInterfacesLock)
        McdRoot.dts.DetectInterfaces(option);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (DetectInterfaces));
    }
  }

  public static void UnprepareVciAccessLayer()
  {
    if (!McdRoot.vciAccessLayerPrepared)
      return;
    try
    {
      foreach (McdInterface currentInterface in McdRoot.currentInterfaces)
        currentInterface.Dispose();
      McdRoot.currentInterfaces.Clear();
      McdRoot.dts.UnprepareVciAccessLayer();
      McdRoot.vciAccessLayerPrepared = false;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (UnprepareVciAccessLayer));
    }
  }

  internal static DateTime FlashFileLastUpdateTime { get; private set; }

  public static void LinkFlashFiles()
  {
    if (!McdRoot.projectSelected)
      return;
    try
    {
      DtsProject theProject = (DtsProject) McdRoot.theProject;
      theProject.UnlinkDatabaseFiles();
      string databaseLocation = McdRoot.DatabaseLocation;
      if (databaseLocation != null)
      {
        foreach (string str in ((IEnumerable<string>) Directory.GetFiles(databaseLocation)).Where<string>((Func<string, bool>) (p => Path.GetExtension(p).ToUpperInvariant() == ".SMR-F")).Select<string, string>((Func<string, string>) (p => Path.GetFullPath(p))))
        {
          string file = str;
          if (!theProject.DatabaseFileList.OfType<DtsFileLocation>().Any<DtsFileLocation>((Func<DtsFileLocation, bool>) (fl => fl.FilePath == file)))
          {
            try
            {
              theProject.LinkDatabaseFile(file);
            }
            catch (MCDException ex)
            {
              McdRoot.RaiseDebugInfoEvent((McdLogicalLink) null, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to link flash file {0}: {1}", (object) file, (object) ex.Message));
            }
          }
        }
      }
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (LinkFlashFiles));
    }
    McdRoot.FlashFileLastUpdateTime = DateTime.Now;
    McdRoot.dbecumems = ((IEnumerable<string>) McdRoot.theProject.DbProject.DbEcuMems.Names).Select<string, KeyValuePair<string, McdDBEcuMem>>((Func<string, KeyValuePair<string, McdDBEcuMem>>) (n => new KeyValuePair<string, McdDBEcuMem>(n, (McdDBEcuMem) null))).ToDictionary<KeyValuePair<string, McdDBEcuMem>, string, McdDBEcuMem>((Func<KeyValuePair<string, McdDBEcuMem>, string>) (k => k.Key), (Func<KeyValuePair<string, McdDBEcuMem>, McdDBEcuMem>) (v => v.Value));
  }

  public static McdLogicalLink CreateLogicalLink(
    string linkQualifier,
    McdInterfaceResource theResource,
    string fixedVariant)
  {
    try
    {
      return new McdLogicalLink(string.IsNullOrEmpty(fixedVariant) ? McdRoot.theProject.CreateLogicalLinkByNameAndInterfaceResource(linkQualifier, theResource.Handle) : McdRoot.theProject.CreateLogicalLinkByVariantAndInterfaceResource(linkQualifier, fixedVariant, theResource.Handle), !string.IsNullOrEmpty(fixedVariant));
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (CreateLogicalLink));
    }
  }

  public static McdLogicalLink CreateOfflineLogicalLink(
    string linkQualifier,
    string fixedVariant,
    McdInterface mcdInterface)
  {
    try
    {
      return new McdLogicalLink(McdRoot.theProject.CreateLogicalLinkByVariantAndInterface(linkQualifier, fixedVariant, mcdInterface.Handle), !string.IsNullOrEmpty(fixedVariant));
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "CreateLogicalLink");
    }
  }

  internal static void RemoveLogicalLink(McdLogicalLink link)
  {
    try
    {
      McdRoot.theProject.RemoveLogicalLink(link.Handle);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (RemoveLogicalLink));
    }
  }

  internal static void RaiseByteMessageEvent(
    McdLogicalLink link,
    IEnumerable<byte> message,
    bool send)
  {
    EventHandler<McdByteMessageEventArgs> byteMessage = McdRoot.ByteMessage;
    if (byteMessage == null)
      return;
    byteMessage((object) link, new McdByteMessageEventArgs(message, send));
  }

  internal static void RaiseDebugInfoEvent(McdLogicalLink link, string message)
  {
    EventHandler<McdDebugInfoEventArgs> debugInfo = McdRoot.DebugInfo;
    if (debugInfo == null)
      return;
    debugInfo((object) link, new McdDebugInfoEventArgs(message));
  }

  public static IEnumerable<McdDBLogicalLink> GetDBLogicalLinksForEcu(string ecuName)
  {
    if (McdRoot.dbLogicalLinks == null)
      McdRoot.dbLogicalLinks = (IEnumerable<McdDBLogicalLink>) McdRoot.theVehicleInformation.DbLogicalLinks.OfType<MCDDbLogicalLink>().Where<MCDDbLogicalLink>((Func<MCDDbLogicalLink, bool>) (ll => ll.DbLocation.Type == MCDLocationType.eECU_BASE_VARIANT)).Select<MCDDbLogicalLink, McdDBLogicalLink>((Func<MCDDbLogicalLink, McdDBLogicalLink>) (ll => new McdDBLogicalLink(ll))).ToList<McdDBLogicalLink>();
    return (IEnumerable<McdDBLogicalLink>) McdRoot.dbLogicalLinks.Where<McdDBLogicalLink>((Func<McdDBLogicalLink, bool>) (ll => ll.EcuQualifier == ecuName)).ToList<McdDBLogicalLink>();
  }

  public static IEnumerable<McdDBLogicalLink> GetDBLogicalLinksForProtocol(string protocolName)
  {
    if (McdRoot.dbProtocolLogicalLinks == null)
      McdRoot.dbProtocolLogicalLinks = (IEnumerable<McdDBLogicalLink>) McdRoot.theVehicleInformation.DbLogicalLinks.OfType<MCDDbLogicalLink>().Where<MCDDbLogicalLink>((Func<MCDDbLogicalLink, bool>) (ll => ll.DbLocation.Type == MCDLocationType.ePROTOCOL)).Select<MCDDbLogicalLink, McdDBLogicalLink>((Func<MCDDbLogicalLink, McdDBLogicalLink>) (ll => new McdDBLogicalLink(ll))).ToList<McdDBLogicalLink>();
    return (IEnumerable<McdDBLogicalLink>) McdRoot.dbProtocolLogicalLinks.Where<McdDBLogicalLink>((Func<McdDBLogicalLink, bool>) (ll => ll.ProtocolLocation.Qualifier == protocolName)).ToList<McdDBLogicalLink>();
  }

  public static IEnumerable<string> DBPhysicalVehicleLinkOrInterfaceNames
  {
    get
    {
      return McdRoot.theProject != null ? (IEnumerable<string>) McdRoot.theVehicleInformation.DbPhysicalVehicleLinkOrInterfaces.Names : (IEnumerable<string>) new string[0];
    }
  }

  public static IEnumerable<string> DBConfigurationDataNames
  {
    get
    {
      return McdRoot.dbconfigurationdatas != null ? (IEnumerable<string>) McdRoot.dbconfigurationdatas.Keys.ToList<string>() : (IEnumerable<string>) new List<string>();
    }
  }

  public static McdDBConfigurationData GetDBConfigurationData(string name)
  {
    McdDBConfigurationData configurationData;
    if (!McdRoot.dbconfigurationdatas.TryGetValue(name, out configurationData) || configurationData == null)
    {
      MCDDbConfigurationData itemByName = ((DtsDbProject) McdRoot.theProject.DbProject).DbConfigurationDatas.GetItemByName(name);
      if (itemByName != null)
        McdRoot.dbconfigurationdatas[name] = configurationData = new McdDBConfigurationData(itemByName);
    }
    return configurationData;
  }

  public static IEnumerable<string> DBProtocolLocationNames
  {
    get
    {
      return McdRoot.protocols != null ? (IEnumerable<string>) McdRoot.protocols.Keys.ToList<string>() : (IEnumerable<string>) new List<string>();
    }
  }

  public static McdDBLocation GetDBProtocolLocation(string name)
  {
    McdDBLocation protocolLocation;
    if (!McdRoot.protocols.TryGetValue(name, out protocolLocation) || protocolLocation == null)
    {
      MCDDbLocation itemByName = McdRoot.theProject.DbProject.DbProtocolLocations.GetItemByName(name);
      if (itemByName != null)
        McdRoot.protocols[name] = protocolLocation = new McdDBLocation(itemByName);
    }
    return protocolLocation;
  }

  public static IEnumerable<McdInterface> CurrentInterfaces
  {
    get
    {
      List<MCDInterface> mcdCurrentInterfaces = new List<MCDInterface>();
      foreach (MCDInterface currentInterface in (IEnumerable) McdRoot.dts.CurrentInterfaces)
        mcdCurrentInterfaces.Add(currentInterface);
      foreach (McdInterface mcdInterface in McdRoot.currentInterfaces.Where<McdInterface>((Func<McdInterface, bool>) (oci => !mcdCurrentInterfaces.Any<MCDInterface>((Func<MCDInterface, bool>) (mci => mci.ShortName == oci.Qualifier)))).ToList<McdInterface>())
      {
        McdRoot.currentInterfaces.Remove(mcdInterface);
        mcdInterface.Dispose();
      }
      foreach (MCDInterface theInterface in mcdCurrentInterfaces.Where<MCDInterface>((Func<MCDInterface, bool>) (mci => !McdRoot.currentInterfaces.Any<McdInterface>((Func<McdInterface, bool>) (oci => mci.ShortName == oci.Qualifier)))).ToList<MCDInterface>())
        McdRoot.currentInterfaces.Add(new McdInterface(theInterface));
      return (IEnumerable<McdInterface>) McdRoot.currentInterfaces;
    }
  }

  public static IEnumerable<string> DBEcuMemNames
  {
    get
    {
      return McdRoot.dbecumems != null ? (IEnumerable<string>) McdRoot.dbecumems.Keys.ToList<string>() : (IEnumerable<string>) new List<string>();
    }
  }

  public static McdDBEcuMem GetDBEcuMem(string name)
  {
    McdDBEcuMem dbEcuMem;
    if (!McdRoot.dbecumems.TryGetValue(name, out dbEcuMem) || dbEcuMem == null)
    {
      MCDDbEcuMem itemByName = McdRoot.theProject.DbProject.DbEcuMems.GetItemByName(name);
      if (itemByName != null)
        McdRoot.dbecumems[name] = dbEcuMem = new McdDBEcuMem(itemByName);
    }
    return dbEcuMem;
  }

  public static IEnumerable<string> DBEcuBaseVariantNames
  {
    get
    {
      return McdRoot.ecus != null ? (IEnumerable<string>) McdRoot.ecus.Keys.ToList<string>() : (IEnumerable<string>) new List<string>();
    }
  }

  public static McdDBEcuBaseVariant GetDBEcuBaseVariant(string name)
  {
    McdDBEcuBaseVariant dbEcuBaseVariant;
    if (!McdRoot.ecus.TryGetValue(name, out dbEcuBaseVariant) || dbEcuBaseVariant == null)
    {
      MCDDbEcuBaseVariant itemByName = McdRoot.theProject.DbProject.DbEcuBaseVariants.GetItemByName(name);
      if (itemByName != null)
        McdRoot.ecus[name] = dbEcuBaseVariant = new McdDBEcuBaseVariant(itemByName);
    }
    return dbEcuBaseVariant;
  }

  internal static Type MapDataType(MCDDataType type)
  {
    switch (type)
    {
      case MCDDataType.eA_ASCIISTRING:
        return typeof (string);
      case MCDDataType.eA_BITFIELD:
        return typeof (bool[]);
      case MCDDataType.eA_BYTEFIELD:
        return typeof (byte[]);
      case MCDDataType.eA_FLOAT32:
        return typeof (float);
      case MCDDataType.eA_FLOAT64:
        return typeof (double);
      case MCDDataType.eA_INT16:
        return typeof (short);
      case MCDDataType.eA_INT32:
        return typeof (int);
      case MCDDataType.eA_INT64:
        return typeof (long);
      case MCDDataType.eA_INT8:
        return typeof (char);
      case MCDDataType.eA_UINT16:
        return typeof (ushort);
      case MCDDataType.eA_UINT32:
        return typeof (uint);
      case MCDDataType.eA_UINT64:
        return typeof (ulong);
      case MCDDataType.eA_UINT8:
        return typeof (byte);
      case MCDDataType.eA_UNICODE2STRING:
        return typeof (string);
      case MCDDataType.eTEXTTABLE:
        return typeof (McdTextTableElement);
      default:
        return (Type) null;
    }
  }

  internal static IEnumerable<T> FlattenStructures<T>(
    IEnumerable<T> parameters,
    Func<T, IEnumerable<T>> getChildren)
  {
    foreach (T parameter1 in parameters)
    {
      T parameter = parameter1;
      yield return parameter;
      IEnumerable<T> children = getChildren(parameter);
      if (children.Any<T>())
      {
        foreach (T flattenStructure in McdRoot.FlattenStructures<T>(children, getChildren))
        {
          T childParameter = flattenStructure;
          yield return childParameter;
          childParameter = default (T);
        }
      }
      children = (IEnumerable<T>) null;
      parameter = default (T);
    }
  }

  public static McdMonitoringLink CreateMonitoringLink(string resourceQualifier)
  {
    McdInterfaceResource interfaceResource = McdRoot.CurrentInterfaces.SelectMany<McdInterface, McdInterfaceResource>((Func<McdInterface, IEnumerable<McdInterfaceResource>>) (i => i.Resources)).FirstOrDefault<McdInterfaceResource>((Func<McdInterfaceResource, bool>) (r => r.Qualifier == resourceQualifier));
    if (interfaceResource == null)
      return (McdMonitoringLink) null;
    try
    {
      return new McdMonitoringLink(McdRoot.theProject.CreateMonitoringLink(interfaceResource.Handle), (string) null);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (CreateMonitoringLink));
    }
  }

  public static McdMonitoringLink CreateDoIPMonitorLink(string networkInterfaceName, string path)
  {
    try
    {
      return new McdMonitoringLink((MCDMonitoringLink) ((DtsProject) McdRoot.theProject).CreateDoIPMonitorLink(networkInterfaceName), path);
    }
    catch (MCDException ex)
    {
      McdRoot.ValidateNativeLibrary("pthreadvc3.dll", true);
      McdRoot.ValidateNativeLibrary("wpcap.dll", true);
      throw new McdException(ex, nameof (CreateDoIPMonitorLink));
    }
  }

  internal static void RemoveMonitoringLink(MCDMonitoringLink link)
  {
    McdRoot.theProject.RemoveMonitoringLink(link);
  }

  public static IList<string> LocationPriority
  {
    get
    {
      return (IList<string>) new string[3]
      {
        "UDS_CAN_EXT",
        "UDS_CAN_D",
        "UDS_Ethernet_DoIP"
      };
    }
  }

  public static IList<string> LocationRestricted
  {
    get
    {
      return (IList<string>) new string[1]
      {
        "UDS_Ethernet_DoIP_DOBT"
      };
    }
  }

  private static void GatherAdditionalAssemblies(string DtsBinaryPath)
  {
    if (DtsBinaryPath == null || !Directory.Exists(DtsBinaryPath))
      return;
    McdRoot.NativeMethods.SetDllDirectory(DtsBinaryPath);
    foreach (string enumerateFile in Directory.EnumerateFiles(DtsBinaryPath, "*.dll", SearchOption.TopDirectoryOnly))
    {
      AssemblyName assemblyName = (AssemblyName) null;
      try
      {
        assemblyName = AssemblyName.GetAssemblyName(enumerateFile);
      }
      catch (ArgumentNullException ex)
      {
      }
      catch (ArgumentException ex)
      {
      }
      catch (BadImageFormatException ex)
      {
      }
      catch (FileLoadException ex)
      {
      }
      catch (SecurityException ex)
      {
      }
      if (assemblyName != null && !McdRoot.additionalAssemblies.ContainsKey(assemblyName.Name))
        McdRoot.additionalAssemblies.Add(assemblyName.Name, enumerateFile);
    }
    AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(McdRoot.ResolveAssembly);
    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(McdRoot.ResolveAssembly);
  }

  private static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
  {
    foreach (KeyValuePair<string, string> additionalAssembly in McdRoot.additionalAssemblies)
    {
      if (e.Name.StartsWith(additionalAssembly.Key, StringComparison.OrdinalIgnoreCase))
        return Assembly.Load(additionalAssembly.Value);
    }
    return (Assembly) null;
  }

  public static void ValidateNativeLibrary(string dllPath, bool useStandardSearchSemantics)
  {
    if (!useStandardSearchSemantics)
    {
      if (!File.Exists(dllPath))
        throw new DllNotFoundException(dllPath + " not found");
      McdRoot.NativeMethods.SetDllDirectory(Path.GetDirectoryName(dllPath));
    }
    if (!(McdRoot.NativeMethods.LoadLibrary(dllPath) == IntPtr.Zero))
      return;
    Win32Exception win32Exception = new Win32Exception(Marshal.GetLastWin32Error());
    string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "LoadLibrary({0}) failed: {1} (Exception from HRESULT: 0x{2:X8})", (object) dllPath, (object) win32Exception.Message, (object) win32Exception.NativeErrorCode);
    if (win32Exception.NativeErrorCode == 193)
      throw new BadImageFormatException(message, dllPath);
    throw new DllNotFoundException(message);
  }

  public static IDictionary<string, string> GetRootDescriptionFileLibraryPaths(
    string rootDescriptionFilePath)
  {
    Dictionary<string, string> fileLibraryPaths = new Dictionary<string, string>();
    foreach (XElement element in XDocument.Load(rootDescriptionFilePath).Root.Elements((XName) "MVCI_PDU_API"))
    {
      string key = element.Element((XName) "SHORT_NAME").Value;
      XAttribute xattribute = element.Element((XName) "LIBRARY_FILE").Attribute((XName) "URI");
      fileLibraryPaths.Add(key, new Uri(xattribute.Value).LocalPath);
    }
    return (IDictionary<string, string>) fileLibraryPaths;
  }

  internal static class NativeMethods
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetDllDirectory(string lpPathName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr LoadLibrary(string path);
  }
}
