using System;
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
using Softing.Dts;

namespace McdAbstraction;

public static class McdRoot
{
	internal static class NativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetDllDirectory(string lpPathName);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string path);
	}

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

	public static bool Initialized => initialized;

	internal static MCDSystem Dts => dts;

	public static string Description => description;

	public static string ConfigurationFilePath => Path.Combine(((DtsSystem)dts).ConfigurationPath, Environment.Is64BitProcess ? "DtsConfig64.xml" : "DtsConfig.xml");

	public static string JavaVirtualMachineLocation => ((DtsSystem)dts).Configuration.JavaConfig.CurrentJvmLocation.FilePath;

	internal static IEnumerable<Tuple<string, string, string>> DatabaseFileList => (theProject != null) ? (from fl in ((DtsProject)theProject).DatabaseFileList.OfType<DtsFileLocation>()
		select Tuple.Create(fl.FilePath, fl.ShortName, fl.Version)).ToList() : new List<Tuple<string, string, string>>();

	public static string DatabaseLocation
	{
		get
		{
			string text = DatabaseFileList.FirstOrDefault()?.Item1;
			return (text != null) ? Path.GetDirectoryName(text) : null;
		}
	}

	public static bool IsVciAccessLayerPrepared => vciAccessLayerPrepared;

	internal static DateTime FlashFileLastUpdateTime { get; private set; }

	public static IEnumerable<string> DBPhysicalVehicleLinkOrInterfaceNames => (theProject != null) ? theVehicleInformation.DbPhysicalVehicleLinkOrInterfaces.Names : new string[0];

	public static IEnumerable<string> DBConfigurationDataNames => (dbconfigurationdatas != null) ? dbconfigurationdatas.Keys.ToList() : new List<string>();

	public static IEnumerable<string> DBProtocolLocationNames => (protocols != null) ? protocols.Keys.ToList() : new List<string>();

	public static IEnumerable<McdInterface> CurrentInterfaces
	{
		get
		{
			List<MCDInterface> mcdCurrentInterfaces = new List<MCDInterface>();
			foreach (MCDInterface currentInterface in dts.CurrentInterfaces)
			{
				mcdCurrentInterfaces.Add(currentInterface);
			}
			foreach (McdInterface item in currentInterfaces.Where((McdInterface oci) => !mcdCurrentInterfaces.Any((MCDInterface mci) => mci.ShortName == oci.Qualifier)).ToList())
			{
				currentInterfaces.Remove(item);
				item.Dispose();
			}
			foreach (MCDInterface item2 in mcdCurrentInterfaces.Where((MCDInterface mci) => !currentInterfaces.Any((McdInterface oci) => mci.ShortName == oci.Qualifier)).ToList())
			{
				currentInterfaces.Add(new McdInterface(item2));
			}
			return currentInterfaces;
		}
	}

	public static IEnumerable<string> DBEcuMemNames => (dbecumems != null) ? dbecumems.Keys.ToList() : new List<string>();

	public static IEnumerable<string> DBEcuBaseVariantNames => (ecus != null) ? ecus.Keys.ToList() : new List<string>();

	public static IList<string> LocationPriority => new string[3] { "UDS_CAN_EXT", "UDS_CAN_D", "UDS_Ethernet_DoIP" };

	public static IList<string> LocationRestricted => new string[1] { "UDS_Ethernet_DoIP_DOBT" };

	public static event EventHandler<McdByteMessageEventArgs> ByteMessage;

	public static event EventHandler<McdDebugInfoEventArgs> DebugInfo;

	public static string GetProperty(string name)
	{
		if (Initialized)
		{
			return dts.GetProperty(name)?.ValueAsString;
		}
		return null;
	}

	public static void SetProperty(string name, string value)
	{
		MCDValue mCDValue = Statics.createValue();
		mCDValue.DataType = MCDDataType.eA_ASCIISTRING;
		mCDValue.Asciistring = value;
		dts.SetProperty(name, mCDValue);
	}

	public static void Construct(string dtsPath, int procedureId, Func<byte[], byte[]> mcdKeyFunc, byte[] testerId)
	{
		if (!additionalAssemblies.Any())
		{
			GatherAdditionalAssemblies(dtsPath);
		}
		if (!additionalAssemblies.Any())
		{
			return;
		}
		Destruct();
		try
		{
			dts = Statics.getSystem();
			((DtsSystem)dts).Initialize();
			byte[] seed = ((DtsSystem)dts).GetSeed((uint)procedureId, DtsAppID.eAPPID_App14);
			byte[] key = mcdKeyFunc(seed);
			((DtsSystem)dts).SendKey(key);
			((DtsSystem)dts).Initialize();
			description = dts.Description;
			MCDValue mCDValue = Statics.createValue();
			mCDValue.DataType = MCDDataType.eA_BOOLEAN;
			mCDValue.Boolean = false;
			dts.SetProperty("com.softing.CheckExactLinkStates", mCDValue);
			if (testerId != null)
			{
				uint num = BitConverter.ToUInt32(testerId.ToArray().Take(4).Reverse()
					.ToArray(), 0);
				SetProperty("com.softing.Identification.TesterId", (num & 0x3FFFFFF).ToString(CultureInfo.InvariantCulture));
			}
			initialized = true;
		}
		catch (MCDException ex)
		{
			initialized = false;
			throw new McdException(ex, "Construct");
		}
	}

	public static void Destruct()
	{
		DeselectProject();
		if (initialized)
		{
			try
			{
				((DtsSystem)dts).Uninitialize();
				initialized = false;
				dts = null;
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "Destruct");
			}
		}
	}

	public static void SetRootDescriptionFile(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			try
			{
				((DtsSystem)dts).Configuration.RootDescriptionFile = path;
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "SetRootDescriptionFile");
			}
		}
	}

	public static void SetSessionProjectPath(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			try
			{
				((DtsSystem)dts).SessionProjectPath = path;
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "SetSessionProjectPath");
			}
		}
	}

	internal static string GetPreamble(string path)
	{
		using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
		{
			binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = binaryReader.ReadBytes(16384);
			int num = array[24] + (array[25] << 8);
			if (num < 16384)
			{
				return Encoding.UTF8.GetString(array.Skip(88).Take(num - 88).ToArray());
			}
		}
		return null;
	}

	private static Version GetInternalVersion(string path)
	{
		string preamble = GetPreamble(path);
		if (preamble != null)
		{
			using StringReader stringReader = new StringReader(preamble);
			string text;
			while ((text = stringReader.ReadLine()) != null)
			{
				string[] array = text.Split(":".ToCharArray());
				if (array[0] == "REVISION INFO")
				{
					try
					{
						return new Version(array[1].Trim());
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}
		return null;
	}

	private static FileInfo DetermineOldestFile(FileInfo fileInfo1, FileInfo fileInfo2)
	{
		Version version = getFileVersion(fileInfo1.Name);
		Version version2 = getFileVersion(fileInfo2.Name);
		if (version != null && version2 != null)
		{
			return (version > version2) ? fileInfo2 : fileInfo1;
		}
		return (fileInfo1.LastWriteTimeUtc > fileInfo2.LastWriteTimeUtc) ? fileInfo2 : fileInfo1;
		static Version getFileVersion(string name)
		{
			MatchCollection matchCollection = VersionExtract.Matches(name);
			if (matchCollection.Count > 0 && matchCollection[0].Success)
			{
				try
				{
					return new Version(matchCollection[0].Value);
				}
				catch (Exception ex) when (ex is FormatException || ex is ArgumentException || ex is OverflowException)
				{
				}
			}
			return null;
		}
	}

	private static bool IsSubPathOf(string path, DirectoryInfo directory)
	{
		if (directory == null)
		{
			return false;
		}
		if (directory.FullName.Equals(path, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return IsSubPathOf(path, directory.Parent);
	}

	public static void SelectProject(string projectName)
	{
		DeselectProject();
		try
		{
			while (true)
			{
				try
				{
					theProject = dts.SelectProjectByName(projectName);
				}
				catch (MCDException ex)
				{
					if (ex.Error.VendorCode == 54160)
					{
						Match match = NameCollisionExtractPaths.Match(ex.Error.VendorCodeDescription);
						if (match.Success && match.Groups.Count == 3)
						{
							FileInfo fileInfo = new FileInfo(match.Groups[1].Value);
							FileInfo fileInfo2 = new FileInfo(match.Groups[2].Value);
							string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
							if (!IsSubPathOf(directoryName, fileInfo.Directory) && fileInfo.DirectoryName == fileInfo2.DirectoryName)
							{
								Version internalVersion = GetInternalVersion(fileInfo.FullName);
								Version internalVersion2 = GetInternalVersion(fileInfo2.FullName);
								FileInfo fileInfo3;
								if (internalVersion != null && internalVersion2 != null)
								{
									RaiseDebugInfoEvent(null, string.Format(CultureInfo.InvariantCulture, "Found diagnosis files with name collision: {0} ({1}) and {2} ({3})", fileInfo.Name, internalVersion, fileInfo2.Name, internalVersion2));
									fileInfo3 = ((internalVersion > internalVersion2) ? fileInfo2 : fileInfo);
								}
								else
								{
									RaiseDebugInfoEvent(null, string.Format(CultureInfo.InvariantCulture, "Found diagnosis files with name collision: {0} and {1}", fileInfo.Name, fileInfo2.Name));
									fileInfo3 = DetermineOldestFile(fileInfo, fileInfo2);
								}
								RaiseDebugInfoEvent(null, string.Format(CultureInfo.InvariantCulture, "Removing older file {0}", fileInfo3.Name));
								string text = fileInfo3.FullName + ".bak";
								if (!File.Exists(text))
								{
									File.Move(fileInfo3.FullName, text);
								}
								else
								{
									File.Delete(fileInfo3.FullName);
								}
								continue;
							}
						}
					}
					else if (ex.Error.VendorCode == 49417)
					{
						string configurationFilePath = ConfigurationFilePath;
						if (!File.Exists(configurationFilePath))
						{
							throw new McdException(string.Format(CultureInfo.InvariantCulture, "The project '{0}' could not be selected. The MCD-3D Configuration file is missing at {1}.", projectName, configurationFilePath));
						}
					}
					throw;
				}
				break;
			}
			ecus = theProject.DbProject.DbEcuBaseVariants.Names.Select((string n) => new KeyValuePair<string, McdDBEcuBaseVariant>(n, null)).ToDictionary((KeyValuePair<string, McdDBEcuBaseVariant> k) => k.Key, (KeyValuePair<string, McdDBEcuBaseVariant> v) => v.Value);
			protocols = theProject.DbProject.DbProtocolLocations.Names.Select((string n) => new KeyValuePair<string, McdDBLocation>(n, null)).ToDictionary((KeyValuePair<string, McdDBLocation> k) => k.Key, (KeyValuePair<string, McdDBLocation> v) => v.Value);
			dbecumems = theProject.DbProject.DbEcuMems.Names.Select((string n) => new KeyValuePair<string, McdDBEcuMem>(n, null)).ToDictionary((KeyValuePair<string, McdDBEcuMem> k) => k.Key, (KeyValuePair<string, McdDBEcuMem> v) => v.Value);
			dbconfigurationdatas = ((DtsDbProject)theProject.DbProject).DbConfigurationDatas.Names.Select((string n) => new KeyValuePair<string, McdDBConfigurationData>(n, null)).ToDictionary((KeyValuePair<string, McdDBConfigurationData> k) => k.Key, (KeyValuePair<string, McdDBConfigurationData> v) => v.Value);
			projectSelected = true;
		}
		catch (MCDException ex2)
		{
			throw new McdException(ex2, "SelectProject");
		}
	}

	public static string GetDatabaseFileVersion(string path)
	{
		if (theProject != null)
		{
			DtsFileLocation dtsFileLocation = ((DtsProject)theProject).DatabaseFileList.OfType<DtsFileLocation>().FirstOrDefault((DtsFileLocation fl) => fl.FilePath == path);
			if (dtsFileLocation != null)
			{
				return dtsFileLocation.Version;
			}
		}
		return null;
	}

	private static void DeselectProject()
	{
		if (projectSelected)
		{
			DeselectVehicleInformation();
			try
			{
				dts.DeselectProject();
				projectSelected = false;
				theProject = null;
				ecus = null;
				dbecumems = null;
				protocols = null;
				dbconfigurationdatas = null;
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "DeselectProject");
			}
		}
	}

	public static void SelectVehicleInformation(string name)
	{
		DeselectVehicleInformation();
		try
		{
			theVehicleInformation = theProject.SelectDbVehicleInformationByName(name);
			vehicleInformationSelected = true;
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "SelectVit");
		}
	}

	private static void DeselectVehicleInformation()
	{
		if (vehicleInformationSelected)
		{
			try
			{
				theProject.DeselectVehicleInformation();
				theVehicleInformation = null;
				dbLogicalLinks = null;
				dbProtocolLogicalLinks = null;
				vehicleInformationSelected = false;
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "DeselectVehicleInformation");
			}
		}
	}

	public static void PrepareVciAccessLayer()
	{
		UnprepareVciAccessLayer();
		try
		{
			dts.PrepareVciAccessLayer();
			vciAccessLayerPrepared = true;
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "PrepareVciAccessLayer");
		}
	}

	public static void DetectInterfaces(string option)
	{
		try
		{
			lock (detectInterfacesLock)
			{
				dts.DetectInterfaces(option);
			}
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "DetectInterfaces");
		}
	}

	public static void UnprepareVciAccessLayer()
	{
		if (!vciAccessLayerPrepared)
		{
			return;
		}
		try
		{
			foreach (McdInterface currentInterface in currentInterfaces)
			{
				currentInterface.Dispose();
			}
			currentInterfaces.Clear();
			dts.UnprepareVciAccessLayer();
			vciAccessLayerPrepared = false;
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "UnprepareVciAccessLayer");
		}
	}

	public static void LinkFlashFiles()
	{
		if (!projectSelected)
		{
			return;
		}
		try
		{
			DtsProject dtsProject = (DtsProject)theProject;
			dtsProject.UnlinkDatabaseFiles();
			string databaseLocation = DatabaseLocation;
			if (databaseLocation != null)
			{
				foreach (string file in from p in Directory.GetFiles(databaseLocation)
					where Path.GetExtension(p).ToUpperInvariant() == ".SMR-F"
					select Path.GetFullPath(p))
				{
					if (!dtsProject.DatabaseFileList.OfType<DtsFileLocation>().Any((DtsFileLocation fl) => fl.FilePath == file))
					{
						try
						{
							dtsProject.LinkDatabaseFile(file);
						}
						catch (MCDException ex)
						{
							RaiseDebugInfoEvent(null, string.Format(CultureInfo.InvariantCulture, "Failed to link flash file {0}: {1}", file, ex.Message));
						}
					}
				}
			}
		}
		catch (MCDException ex2)
		{
			throw new McdException(ex2, "LinkFlashFiles");
		}
		FlashFileLastUpdateTime = DateTime.Now;
		dbecumems = theProject.DbProject.DbEcuMems.Names.Select((string n) => new KeyValuePair<string, McdDBEcuMem>(n, null)).ToDictionary((KeyValuePair<string, McdDBEcuMem> k) => k.Key, (KeyValuePair<string, McdDBEcuMem> v) => v.Value);
	}

	public static McdLogicalLink CreateLogicalLink(string linkQualifier, McdInterfaceResource theResource, string fixedVariant)
	{
		try
		{
			MCDLogicalLink link = (string.IsNullOrEmpty(fixedVariant) ? theProject.CreateLogicalLinkByNameAndInterfaceResource(linkQualifier, theResource.Handle) : theProject.CreateLogicalLinkByVariantAndInterfaceResource(linkQualifier, fixedVariant, theResource.Handle));
			return new McdLogicalLink(link, !string.IsNullOrEmpty(fixedVariant));
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "CreateLogicalLink");
		}
	}

	public static McdLogicalLink CreateOfflineLogicalLink(string linkQualifier, string fixedVariant, McdInterface mcdInterface)
	{
		try
		{
			return new McdLogicalLink(theProject.CreateLogicalLinkByVariantAndInterface(linkQualifier, fixedVariant, mcdInterface.Handle), !string.IsNullOrEmpty(fixedVariant));
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
			theProject.RemoveLogicalLink(link.Handle);
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "RemoveLogicalLink");
		}
	}

	internal static void RaiseByteMessageEvent(McdLogicalLink link, IEnumerable<byte> message, bool send)
	{
		McdRoot.ByteMessage?.Invoke(link, new McdByteMessageEventArgs(message, send));
	}

	internal static void RaiseDebugInfoEvent(McdLogicalLink link, string message)
	{
		McdRoot.DebugInfo?.Invoke(link, new McdDebugInfoEventArgs(message));
	}

	public static IEnumerable<McdDBLogicalLink> GetDBLogicalLinksForEcu(string ecuName)
	{
		if (dbLogicalLinks == null)
		{
			dbLogicalLinks = (from ll in theVehicleInformation.DbLogicalLinks.OfType<MCDDbLogicalLink>()
				where ll.DbLocation.Type == MCDLocationType.eECU_BASE_VARIANT
				select new McdDBLogicalLink(ll)).ToList();
		}
		return dbLogicalLinks.Where((McdDBLogicalLink ll) => ll.EcuQualifier == ecuName).ToList();
	}

	public static IEnumerable<McdDBLogicalLink> GetDBLogicalLinksForProtocol(string protocolName)
	{
		if (dbProtocolLogicalLinks == null)
		{
			dbProtocolLogicalLinks = (from ll in theVehicleInformation.DbLogicalLinks.OfType<MCDDbLogicalLink>()
				where ll.DbLocation.Type == MCDLocationType.ePROTOCOL
				select new McdDBLogicalLink(ll)).ToList();
		}
		return dbProtocolLogicalLinks.Where((McdDBLogicalLink ll) => ll.ProtocolLocation.Qualifier == protocolName).ToList();
	}

	public static McdDBConfigurationData GetDBConfigurationData(string name)
	{
		if (!dbconfigurationdatas.TryGetValue(name, out var value) || value == null)
		{
			MCDDbConfigurationData itemByName = ((DtsDbProject)theProject.DbProject).DbConfigurationDatas.GetItemByName(name);
			if (itemByName != null)
			{
				value = (dbconfigurationdatas[name] = new McdDBConfigurationData(itemByName));
			}
		}
		return value;
	}

	public static McdDBLocation GetDBProtocolLocation(string name)
	{
		if (!protocols.TryGetValue(name, out var value) || value == null)
		{
			MCDDbLocation itemByName = theProject.DbProject.DbProtocolLocations.GetItemByName(name);
			if (itemByName != null)
			{
				value = (protocols[name] = new McdDBLocation(itemByName));
			}
		}
		return value;
	}

	public static McdDBEcuMem GetDBEcuMem(string name)
	{
		if (!dbecumems.TryGetValue(name, out var value) || value == null)
		{
			MCDDbEcuMem itemByName = theProject.DbProject.DbEcuMems.GetItemByName(name);
			if (itemByName != null)
			{
				value = (dbecumems[name] = new McdDBEcuMem(itemByName));
			}
		}
		return value;
	}

	public static McdDBEcuBaseVariant GetDBEcuBaseVariant(string name)
	{
		if (!ecus.TryGetValue(name, out var value) || value == null)
		{
			MCDDbEcuBaseVariant itemByName = theProject.DbProject.DbEcuBaseVariants.GetItemByName(name);
			if (itemByName != null)
			{
				value = (ecus[name] = new McdDBEcuBaseVariant(itemByName));
			}
		}
		return value;
	}

	internal static Type MapDataType(MCDDataType type)
	{
		return type switch
		{
			MCDDataType.eA_UINT32 => typeof(uint), 
			MCDDataType.eA_UINT16 => typeof(ushort), 
			MCDDataType.eA_UINT64 => typeof(ulong), 
			MCDDataType.eA_UINT8 => typeof(byte), 
			MCDDataType.eA_INT16 => typeof(short), 
			MCDDataType.eA_INT32 => typeof(int), 
			MCDDataType.eA_INT64 => typeof(long), 
			MCDDataType.eA_INT8 => typeof(char), 
			MCDDataType.eTEXTTABLE => typeof(McdTextTableElement), 
			MCDDataType.eA_ASCIISTRING => typeof(string), 
			MCDDataType.eA_UNICODE2STRING => typeof(string), 
			MCDDataType.eA_FLOAT32 => typeof(float), 
			MCDDataType.eA_FLOAT64 => typeof(double), 
			MCDDataType.eA_BYTEFIELD => typeof(byte[]), 
			MCDDataType.eA_BITFIELD => typeof(bool[]), 
			_ => null, 
		};
	}

	internal static IEnumerable<T> FlattenStructures<T>(IEnumerable<T> parameters, Func<T, IEnumerable<T>> getChildren)
	{
		foreach (T parameter in parameters)
		{
			yield return parameter;
			IEnumerable<T> children = getChildren(parameter);
			if (!children.Any())
			{
				continue;
			}
			foreach (T item in FlattenStructures(children, getChildren))
			{
				yield return item;
			}
		}
	}

	public static McdMonitoringLink CreateMonitoringLink(string resourceQualifier)
	{
		McdInterfaceResource mcdInterfaceResource = CurrentInterfaces.SelectMany((McdInterface i) => i.Resources).FirstOrDefault((McdInterfaceResource r) => r.Qualifier == resourceQualifier);
		if (mcdInterfaceResource != null)
		{
			try
			{
				return new McdMonitoringLink(((DtsProject)theProject).CreateMonitoringLink(mcdInterfaceResource.Handle), null);
			}
			catch (MCDException ex)
			{
				throw new McdException(ex, "CreateMonitoringLink");
			}
		}
		return null;
	}

	public static McdMonitoringLink CreateDoIPMonitorLink(string networkInterfaceName, string path)
	{
		try
		{
			return new McdMonitoringLink(((DtsProject)theProject).CreateDoIPMonitorLink(networkInterfaceName), path);
		}
		catch (MCDException ex)
		{
			ValidateNativeLibrary("pthreadvc3.dll", useStandardSearchSemantics: true);
			ValidateNativeLibrary("wpcap.dll", useStandardSearchSemantics: true);
			throw new McdException(ex, "CreateDoIPMonitorLink");
		}
	}

	internal static void RemoveMonitoringLink(MCDMonitoringLink link)
	{
		((DtsProject)theProject).RemoveMonitoringLink(link);
	}

	private static void GatherAdditionalAssemblies(string DtsBinaryPath)
	{
		if (DtsBinaryPath == null || !Directory.Exists(DtsBinaryPath))
		{
			return;
		}
		NativeMethods.SetDllDirectory(DtsBinaryPath);
		foreach (string item in Directory.EnumerateFiles(DtsBinaryPath, "*.dll", SearchOption.TopDirectoryOnly))
		{
			AssemblyName assemblyName = null;
			try
			{
				assemblyName = AssemblyName.GetAssemblyName(item);
			}
			catch (ArgumentNullException)
			{
			}
			catch (ArgumentException)
			{
			}
			catch (BadImageFormatException)
			{
			}
			catch (FileLoadException)
			{
			}
			catch (SecurityException)
			{
			}
			if (assemblyName != null && !additionalAssemblies.ContainsKey(assemblyName.Name))
			{
				additionalAssemblies.Add(assemblyName.Name, item);
			}
		}
		AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
		AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
	}

	private static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
	{
		foreach (KeyValuePair<string, string> additionalAssembly in additionalAssemblies)
		{
			if (e.Name.StartsWith(additionalAssembly.Key, StringComparison.OrdinalIgnoreCase))
			{
				return Assembly.Load(additionalAssembly.Value);
			}
		}
		return null;
	}

	public static void ValidateNativeLibrary(string dllPath, bool useStandardSearchSemantics)
	{
		if (!useStandardSearchSemantics)
		{
			if (!File.Exists(dllPath))
			{
				throw new DllNotFoundException(dllPath + " not found");
			}
			NativeMethods.SetDllDirectory(Path.GetDirectoryName(dllPath));
		}
		IntPtr intPtr = NativeMethods.LoadLibrary(dllPath);
		if (intPtr == IntPtr.Zero)
		{
			Win32Exception ex = new Win32Exception(Marshal.GetLastWin32Error());
			string message = string.Format(CultureInfo.InvariantCulture, "LoadLibrary({0}) failed: {1} (Exception from HRESULT: 0x{2:X8})", dllPath, ex.Message, ex.NativeErrorCode);
			if (ex.NativeErrorCode == 193)
			{
				throw new BadImageFormatException(message, dllPath);
			}
			throw new DllNotFoundException(message);
		}
	}

	public static IDictionary<string, string> GetRootDescriptionFileLibraryPaths(string rootDescriptionFilePath)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		XDocument xDocument = XDocument.Load(rootDescriptionFilePath);
		foreach (XElement item in xDocument.Root.Elements("MVCI_PDU_API"))
		{
			string value = item.Element("SHORT_NAME").Value;
			XAttribute xAttribute = item.Element("LIBRARY_FILE").Attribute("URI");
			dictionary.Add(value, new Uri(xAttribute.Value).LocalPath);
		}
		return dictionary;
	}
}
