using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class DiagnosisProtocol
{
	private Dictionary<string, TranslationEntry> translation;

	private string name;

	private object protocolComParameterMapLock = new object();

	private ListDictionary comParameters;

	private bool isMcd;

	internal Dictionary<string, TranslationEntry> Translations => translation;

	public bool IsMcd => isMcd;

	public string Name => name;

	public ListDictionary ComParameters
	{
		get
		{
			lock (protocolComParameterMapLock)
			{
				if (comParameters == null)
				{
					if (isMcd)
					{
						comParameters = new ListDictionary();
						foreach (McdDBRequestParameter comParameter in McdRoot.GetDBProtocolLocation(name).GetComParameters())
						{
							McdValue defaultValue = comParameter.GetDefaultValue();
							if (defaultValue != null && defaultValue.Value != null)
							{
								comParameters[comParameter.Qualifier] = defaultValue.GetValue(defaultValue.Value.GetType(), null);
							}
						}
					}
					else
					{
						CaesarProtocol protocol = CaesarRoot.GetProtocol(Name);
						try
						{
							if (protocol != null)
							{
								comParameters = protocol.ComParameters;
							}
						}
						finally
						{
							((IDisposable)protocol)?.Dispose();
						}
					}
				}
				return comParameters;
			}
		}
	}

	public string DescriptionFileName
	{
		get
		{
			if (!IsMcd)
			{
				return null;
			}
			return McdRoot.GetDBProtocolLocation(name)?.DatabaseFile;
		}
	}

	public string DescriptionDataVersion => McdRoot.GetDatabaseFileVersion(DescriptionFileName);

	public static CultureInfo OriginalCulture => CultureInfo.GetCultureInfo("en-US");

	internal DiagnosisProtocol(string name, bool isMcd = false)
	{
		this.name = name;
		this.isMcd = isMcd;
		CultureInfo culture = Sapi.GetSapi().PresentationCulture;
		GetTranslationFileName(culture);
		if (IsTranslationNecessary(culture) && !IsTranslationFilePresent(culture))
		{
			culture = OriginalCulture;
		}
		if (IsTranslationFilePresent(culture))
		{
			translation = ReadTranslationFile(culture).Reverse().DistinctBy((TranslationEntry e) => e.Qualifier).ToDictionary((TranslationEntry item) => item.Qualifier);
		}
	}

	public override string ToString()
	{
		return name;
	}

	public ConnectionResourceCollection GetConnectionResources(byte sourceAddress)
	{
		return GetConnectionResources(sourceAddress, new long[3] { 250000L, 500000L, 666666L });
	}

	public ConnectionResourceCollection GetConnectionResources(byte sourceAddress, long[] baudRates)
	{
		switch (Sapi.GetSapi().InitState)
		{
		case InitState.Online:
		{
			ConnectionResourceCollection connectionResourceCollection = new ConnectionResourceCollection();
			if (isMcd)
			{
				PopulateMcdConnectionResources(connectionResourceCollection, sourceAddress, baudRates);
			}
			else
			{
				PopulateCaesarConnectionResources(connectionResourceCollection, sourceAddress, baudRates);
			}
			return connectionResourceCollection;
		}
		case InitState.Offline:
			throw new InvalidOperationException("Resource cannot be acquired in offline operation mode");
		case InitState.NotInitialized:
			throw new InvalidOperationException("Sapi not initialized");
		default:
			return null;
		}
	}

	private void PopulateCaesarConnectionResources(ConnectionResourceCollection connectionResources, byte sourceAddress, long[] baudRates)
	{
		//IL_0028: Expected O, but got Unknown
		//IL_004d: Expected O, but got Unknown
		//IL_007a: Expected O, but got Unknown
		lock (Ecu.ResourceLock)
		{
			try
			{
				CaesarRoot.LockResources();
			}
			catch (CaesarErrorException ex)
			{
				throw new CaesarException(ex, null, null);
			}
			uint availableProtocolResourceCount;
			try
			{
				availableProtocolResourceCount = CaesarRoot.GetAvailableProtocolResourceCount(name);
			}
			catch (CaesarErrorException ex2)
			{
				CaesarRoot.UnlockResources();
				throw new CaesarException(ex2, null, null);
			}
			for (ushort num = 0; num < availableProtocolResourceCount; num++)
			{
				CaesarResource availableProtocolResource;
				try
				{
					availableProtocolResource = CaesarRoot.GetAvailableProtocolResource(name, num);
				}
				catch (CaesarErrorException ex3)
				{
					CaesarRoot.UnlockResources();
					throw new CaesarException(ex3, null, null);
				}
				long[] array = ((availableProtocolResource.IsPassThru && RollCallJ1939.GlobalInstance != null && RollCallJ1939.GlobalInstance.IsAutoBaudRate && Sapi.GetSapi().AllowAutoBaudRate) ? new long[1] : baudRates);
				foreach (long num2 in array)
				{
					connectionResources.Add(new ConnectionResource(this, availableProtocolResource, sourceAddress, (uint)num2));
				}
			}
			CaesarRoot.UnlockResources();
		}
	}

	private void PopulateMcdConnectionResources(ConnectionResourceCollection connectionResources, byte sourceAddress, long[] baudRates)
	{
		IEnumerable<McdDBLogicalLink> dBLogicalLinksForProtocol = McdRoot.GetDBLogicalLinksForProtocol(name);
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (McdInterface currentInterface in McdRoot.CurrentInterfaces)
		{
			foreach (McdInterfaceResource theInterfaceResource in currentInterface.Resources)
			{
				if (dBLogicalLinksForProtocol.Any((McdDBLogicalLink ll) => ll.ProtocolType == theInterfaceResource.ProtocolType))
				{
					if (!dictionary.TryGetValue(theInterfaceResource.ProtocolType, out var value))
					{
						dictionary.Add(theInterfaceResource.ProtocolType, 0);
					}
					value = ++dictionary[theInterfaceResource.ProtocolType];
					long[] array = ((RollCallJ1939.GlobalInstance != null && RollCallJ1939.GlobalInstance.IsAutoBaudRate && Sapi.GetSapi().AllowAutoBaudRate) ? new long[1] : baudRates);
					foreach (long num2 in array)
					{
						ConnectionResource connectionResource = new ConnectionResource(this, currentInterface, theInterfaceResource, value, sourceAddress, (uint)num2);
						connectionResources.Add(connectionResource);
					}
				}
			}
		}
	}

	private string GetTranslationFileName(CultureInfo culture)
	{
		return TranslationEntry.GetTranslationFileName(name, culture);
	}

	public IEnumerable<TranslationEntry> ReadTranslationFile(CultureInfo culture)
	{
		return TranslationEntry.ReadTranslationFile(name, culture);
	}

	public bool IsTranslationFilePresent(CultureInfo culture)
	{
		return File.Exists(GetTranslationFileName(culture));
	}

	internal string Translate(string qualifier, string original)
	{
		if (translation != null && translation.TryGetValue(qualifier, out var value))
		{
			return value.Translation;
		}
		return original;
	}

	public static bool IsTranslationNecessary(CultureInfo culture)
	{
		return !OriginalCulture.Neutralize().Name.Equals(culture.Neutralize().Name);
	}

	public string GetServiceIdentifierDescription(byte serviceIdentifier)
	{
		return Translate(string.Format(CultureInfo.InvariantCulture, "{0:X2}.ServiceIdentifier", serviceIdentifier), string.Format(CultureInfo.InvariantCulture, "SID {0:X2}h", serviceIdentifier));
	}

	public string GetNegativeResponseCodeDescription(byte negativeResponseCode)
	{
		return Translate(string.Format(CultureInfo.InvariantCulture, "{0:X2}.NegativeResponseCode", negativeResponseCode), string.Format(CultureInfo.InvariantCulture, "NRC {0:X2}h", negativeResponseCode));
	}
}
