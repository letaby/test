using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ChannelCollection : ChannelBaseCollection
{
	internal const int TLSLAVEWAITTIME = 100;

	private List<BackgroundConnect> pendingConnections;

	private List<ConnectionResource> pendingResources;

	private volatile bool autoConnecting;

	private List<EcuAutoConnect> ecusInAutoConnect;

	private List<Channel> offlineOutOfCollection;

	public bool AutoConnecting => autoConnecting;

	public bool ProhibitImplausibleOrNoVariantManualConnections { get; set; }

	internal ChannelCollection()
	{
		autoConnecting = false;
		pendingConnections = new List<BackgroundConnect>();
		pendingResources = new List<ConnectionResource>();
		offlineOutOfCollection = new List<Channel>();
	}

	internal Channel GetItem(CaesarChannel requested)
	{
		if (requested != null)
		{
			using IEnumerator<Channel> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				Channel current = enumerator.Current;
				if (current.IsOwner(requested))
				{
					return current;
				}
			}
		}
		return null;
	}

	public void AbortPendingConnections()
	{
		AbortPendingConnections((ConnectionResource cr) => true);
	}

	internal void AbortPendingConnections(Func<ConnectionResource, bool> connectionResourceMatchFunc)
	{
		lock (pendingConnections)
		{
			foreach (BackgroundConnect item in pendingConnections.Where((BackgroundConnect bg) => connectionResourceMatchFunc(bg.ConnectionResource)).ToList())
			{
				item.Abort();
			}
		}
	}

	public int GetConnectingCountForIdentifier(string identifier)
	{
		return GetConnectingCountForIdentifier(identifier, null);
	}

	private int GetConnectingCountForIdentifier(string identifier, BackgroundConnect previousBackgroundConnect)
	{
		int num = 0;
		lock (pendingConnections)
		{
			return pendingConnections.Count((BackgroundConnect bg) => bg != previousBackgroundConnect && bg.ConnectionResource.Ecu.Identifier == identifier);
		}
	}

	internal void WaitForBackgroundConnection()
	{
		while (true)
		{
			BackgroundConnect backgroundConnect = null;
			lock (pendingConnections)
			{
				if (pendingConnections.Count <= 0)
				{
					break;
				}
				backgroundConnect = pendingConnections[0];
			}
			backgroundConnect.BackgroundThread.Join();
		}
	}

	internal Channel ConnectComplete(BackgroundConnect backgroundConnect)
	{
		Sapi sapi = Sapi.GetSapi();
		BackgroundConnect item = backgroundConnect;
		if (backgroundConnect.Initialised && !backgroundConnect.VariantMatched)
		{
			sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Connected to base variant for {0}, other options may be available", backgroundConnect.ConnectionResource.Ecu.Name));
			string identifier = backgroundConnect.ConnectionResource.Ecu.Identifier;
			DiagnosisVariant diagnosisVariant = null;
			if (backgroundConnect.CaesarChannel != null)
			{
				diagnosisVariant = sapi.Ecus.GetDiagnosisVariantFromIDBlock(identifier, backgroundConnect.CaesarChannel.IdBlock);
			}
			else if (backgroundConnect.MCDLogicalLink != null)
			{
				diagnosisVariant = sapi.Ecus.GetDiagnosisVariantFromIDBlock(identifier, backgroundConnect.MCDLogicalLink);
			}
			if (diagnosisVariant != null)
			{
				sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Got actual match for {0} variant {1} - looking for equivalent connection resource", diagnosisVariant.Ecu.Name, diagnosisVariant.Name));
				backgroundConnect = ReconnectToEquivalentResource(diagnosisVariant.Ecu, backgroundConnect);
			}
			else
			{
				sapi.RaiseDebugInfoEvent(this, "No better ECU found as a direct match for " + backgroundConnect.ConnectionResource.Ecu.Name);
				uint? num = null;
				if (backgroundConnect.CaesarChannel != null)
				{
					num = backgroundConnect.CaesarChannel.IdBlock.DiagVersionLong;
				}
				else if (backgroundConnect.MCDLogicalLink != null)
				{
					McdValue variantIdentificationResult = backgroundConnect.MCDLogicalLink.GetVariantIdentificationResult("ActiveDiagnosticInformation_Read", "Identification");
					if (variantIdentificationResult != null)
					{
						num = Convert.ToUInt32(variantIdentificationResult.Value, CultureInfo.InvariantCulture);
					}
				}
				if (num.HasValue)
				{
					if (!backgroundConnect.ConnectionResource.Ecu.HasMultipleByteDiagnosticVersion)
					{
						sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "IdBlock has long diagnostic version 0x{1:x}, maybe we can try using that?", backgroundConnect.ConnectionResource.Ecu.Name, num));
						uint maskedVersion = num.Value & 0xFFFFFF00u;
						Ecu bestEcuFromVariant = sapi.Ecus.GetBestEcuFromVariant(identifier, (DiagnosisVariant v) => !v.Ecu.IsSameEcuOnDifferentDiagnosisSource(backgroundConnect.ConnectionResource.Ecu) && v.DiagnosticVersionLong.HasValue && (v.DiagnosticVersionLong.Value & 0xFFFFFF00u) == maskedVersion);
						if (bestEcuFromVariant != null)
						{
							if (bestEcuFromVariant == backgroundConnect.ConnectionResource.Ecu)
							{
								sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Masked long diagnostic version 0x{0:x} indicates that the existing connection to {1}.{2} is the best match; remaining connected using original connection resource", maskedVersion, backgroundConnect.ConnectionResource.Ecu.DiagnosisSource, backgroundConnect.ConnectionResource.Ecu.Name));
							}
							else
							{
								sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Got possible match for {0}.{1} using masked long diagnostic version - looking for equivalent connection resource", bestEcuFromVariant.DiagnosisSource, bestEcuFromVariant.Name));
								backgroundConnect = ReconnectToEquivalentResource(bestEcuFromVariant, backgroundConnect);
							}
						}
						else
						{
							sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "No better ECU found for {0} using masked long diagnostic version", backgroundConnect.ConnectionResource.Ecu.Name));
							if ((num.Value & 0x10000) == 65536 && !backgroundConnect.ConnectionResource.Ecu.DiagnosisVariants.Any((DiagnosisVariant v) => v.IsBoot))
							{
								sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "{0}: connection is allowed because it is a boot variant for an ECU whose diagnostic description does not describe boot variants", backgroundConnect.ConnectionResource.Ecu.Name));
							}
							else if (backgroundConnect.AutoConnect || ProhibitImplausibleOrNoVariantManualConnections)
							{
								backgroundConnect.SetConnectCompleteFailure(SapiError.NoPlausibleVariantMatch);
							}
						}
					}
				}
				else
				{
					sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "{0}: No long diagnostic version available", backgroundConnect.ConnectionResource.Ecu.Name));
					if (backgroundConnect.AutoConnect || ProhibitImplausibleOrNoVariantManualConnections)
					{
						backgroundConnect.SetConnectCompleteFailure(SapiError.NoDiagnosticVersionForVariantMatch);
					}
				}
			}
		}
		Channel channel = null;
		if (backgroundConnect != null)
		{
			if (backgroundConnect.Initialised)
			{
				RaiseConnectProgressEvent(backgroundConnect.ConnectionResource, 85.0);
				channel = ((backgroundConnect.CaesarChannel == null) ? Add(backgroundConnect.MCDLogicalLink, backgroundConnect.ConnectionResource, backgroundConnect.ChannelOptions) : Add(backgroundConnect.CaesarChannel, backgroundConnect.ConnectionResource, backgroundConnect.ChannelOptions));
				RaiseConnectProgressEvent(backgroundConnect.ConnectionResource, 100.0);
				RaiseConnectCompleteEvent(channel, backgroundConnect, backgroundConnect.CaesarException);
			}
			else
			{
				Sapi.GetSapi().LogFiles.LabelLog(string.Format(CultureInfo.InvariantCulture, "Failed to connect {0} - {1}", backgroundConnect.ConnectionResource.ToString(), backgroundConnect.CaesarException.Message), backgroundConnect.ConnectionResource.Ecu);
				RaiseConnectCompleteEvent(backgroundConnect.ConnectionResource, backgroundConnect, backgroundConnect.CaesarException);
			}
		}
		lock (pendingConnections)
		{
			pendingConnections.Remove(item);
			return channel;
		}
	}

	private BackgroundConnect ReconnectToEquivalentResource(Ecu matchingEcu, BackgroundConnect backgroundConnect)
	{
		Sapi sapi = Sapi.GetSapi();
		ConnectionResource connectionResource = null;
		ConnectionResourceCollection connectionResources = matchingEcu.GetConnectionResources();
		if (matchingEcu.DiagnosisSource == DiagnosisSource.McdDatabase)
		{
			ConnectionResource connectionResource2 = connectionResources.FirstOrDefault((ConnectionResource cr) => cr.IsEthernet);
			if (connectionResource2 != null)
			{
				bool flag = RollCallDoIP.GlobalInstance.GetActiveAddresses().Any((int addr) => matchingEcu.IsRelated(RollCallDoIP.GlobalInstance, addr, null));
				if (flag)
				{
					connectionResource = connectionResource2;
				}
				sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "An ethernet resource for {0}.{1} exists. A related DoIP entity was {2} found. The ethernet resource will {2} be used.", matchingEcu.DiagnosisSource, matchingEcu.Name, flag ? string.Empty : "not"));
			}
		}
		if (connectionResource == null)
		{
			connectionResource = connectionResources.GetEquivalent(backgroundConnect.ConnectionResource);
		}
		if (connectionResource != null)
		{
			sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Got equivalent connection resource, disconnecting {0}", backgroundConnect.ConnectionResource.Ecu));
			BackgroundConnect backgroundConnect2 = backgroundConnect;
			RaiseConnectCompleteEvent(backgroundConnect.ConnectionResource, backgroundConnect, new CaesarException(connectionResource.Restricted ? SapiError.FoundBetterVariantMatchButResourceRestricted : SapiError.FoundBetterVariantMatch));
			backgroundConnect.Dispose();
			backgroundConnect = null;
			if (connectionResource.Restricted)
			{
				sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Equivalent connection resource {0} for {1} is restricted and will not be used", connectionResource.ToString(), matchingEcu.Name));
			}
			else
			{
				sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Trying to connect to {0}", matchingEcu.Name));
				try
				{
					backgroundConnect = InternalConnect(connectionResource, backgroundConnect2.ChannelOptions, sync: true, ConnectSource.VariantTransfer, backgroundConnect2);
					backgroundConnect2.Child = backgroundConnect;
				}
				catch (CaesarException)
				{
				}
			}
		}
		else
		{
			sapi.RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "No equivalent connection resource to {0} found for ECU {1}. Disconnecting {2} as it is known to be the incorrect variant. ", backgroundConnect.ConnectionResource.ToString(), matchingEcu.Name, backgroundConnect.ConnectionResource.Ecu));
			RaiseConnectCompleteEvent(backgroundConnect.ConnectionResource, backgroundConnect, new CaesarException(SapiError.FoundBetterVariantMatchButResourceUnavailable));
			backgroundConnect.Dispose();
			backgroundConnect = null;
		}
		return backgroundConnect;
	}

	internal Channel AddFromRollCall(ConnectionResource resource, DiagnosisVariant variant)
	{
		Channel channel = new Channel(this, resource, variant);
		lock (base.Items)
		{
			base.Items.Add(channel);
		}
		RaiseConnectCompleteEvent(channel, null);
		return channel;
	}

	internal static void InitComParameterCollection(McdLogicalLink logicalLink, ConnectionResource resource)
	{
		foreach (DictionaryEntry ecuInfoComParameter in resource.EcuInfoComParameters)
		{
			try
			{
				logicalLink.SetComParameter(ecuInfoComParameter.Key.ToString(), ecuInfoComParameter.Value);
			}
			catch (McdException mcdError)
			{
				Sapi sapi = Sapi.GetSapi();
				sapi.RaiseExceptionEvent(sapi.Channels, new CaesarException(mcdError));
			}
		}
	}

	internal static void InitComParameterCollection(CaesarChannel ch, ConnectionResource resource)
	{
		//IL_0037: Expected O, but got Unknown
		foreach (DictionaryEntry ecuInfoComParameter in resource.EcuInfoComParameters)
		{
			try
			{
				ch.SetComParameter(ecuInfoComParameter.Key.ToString(), ecuInfoComParameter.Value.ToString());
			}
			catch (CaesarErrorException ex)
			{
				CaesarErrorException caesarError = ex;
				Sapi sapi = Sapi.GetSapi();
				sapi.RaiseExceptionEvent(sapi.Channels, new CaesarException(caesarError));
			}
		}
		if (resource.Ecu.ProtocolName == "E1939" && resource.SourceAddress.HasValue)
		{
			ch.SetComParameter("CP_DESTADDRESS", resource.SourceAddress.Value.ToString(CultureInfo.InvariantCulture));
		}
	}

	internal static void InitComParameterCollection(CaesarChannel chanHandle, string parameterList)
	{
		//IL_005c: Expected O, but got Unknown
		Sapi sapi = Sapi.GetSapi();
		string[] array = parameterList.Split(";".ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split("=".ToCharArray());
			if (array2.Length != 2)
			{
				continue;
			}
			try
			{
				if (!chanHandle.SetComParameter(array2[0], array2[1]))
				{
					sapi.RaiseExceptionEvent(sapi.Channels, new CaesarException(SapiError.ComParameterSpecUnavailable));
				}
			}
			catch (CaesarErrorException ex)
			{
				CaesarErrorException caesarError = ex;
				sapi.RaiseExceptionEvent(sapi.Channels, new CaesarException(caesarError));
			}
		}
	}

	internal void ClearEcusInAutoConnect()
	{
		StopAutoConnect();
		if (ecusInAutoConnect != null)
		{
			ecusInAutoConnect.Clear();
			ecusInAutoConnect = null;
		}
	}

	internal Channel Open(ConnectionResource resource)
	{
		CaesarChannel ch = OpenCaesarChannel(resource, addToPendingResources: false);
		Thread.Sleep(100);
		InitComParameterCollection(ch, resource);
		BackgroundConnect backgroundConnect = new BackgroundConnect(null, ch, resource);
		backgroundConnect.Start();
		backgroundConnect.BackgroundThread.Join();
		if (backgroundConnect.CaesarException != null)
		{
			throw backgroundConnect.CaesarException;
		}
		return new Channel(ch, resource, null, ChannelOptions.All);
	}

	public Channel Connect(ConnectionResource resource, bool synchronous)
	{
		return Connect(resource, ChannelOptions.All, synchronous, ConnectSource.Manual);
	}

	public Channel Connect(ConnectionResource resource, ChannelOptions options, bool synchronous)
	{
		return Connect(resource, options, synchronous, ConnectSource.Manual);
	}

	internal Channel Connect(ConnectionResource resource, ChannelOptions options, bool synchronous, ConnectSource connectSource)
	{
		if (resource.Ecu.OfflineSupportOnly)
		{
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The ECU ({0}) is only supported in offline mode.", resource.Ecu.Name));
		}
		BackgroundConnect backgroundConnect = InternalConnect(resource, options, synchronous, connectSource, null);
		if (synchronous)
		{
			if (backgroundConnect.Channel != null)
			{
				return backgroundConnect.Channel;
			}
			if (backgroundConnect.CaesarException != null)
			{
				throw backgroundConnect.CaesarException;
			}
		}
		return null;
	}

	public Channel ConnectOffline(DiagnosisVariant diagnosisVariant)
	{
		Channel channel = InternalConnectOffline(diagnosisVariant, null);
		if (channel != null)
		{
			RaiseConnectCompleteEvent(channel, null);
		}
		return channel;
	}

	public Channel ConnectOffline(string parameterFileName, ParameterFileFormat parameterFileFormat, Collection<string> unknownList)
	{
		using StreamReader parameterFile = new StreamReader(parameterFileName);
		return ConnectOffline(parameterFile, parameterFileFormat, unknownList);
	}

	public Channel ConnectOffline(StreamReader parameterFile, ParameterFileFormat parameterFileFormat, Collection<string> unknownList)
	{
		Sapi sapi = Sapi.GetSapi();
		TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(parameterFile, parameterFileFormat);
		Ecu ecu = sapi.Ecus[targetEcuDetails.Ecu];
		if (ecu != null)
		{
			DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
			if (diagnosisVariant == null)
			{
				Ecu ecu2 = sapi.Ecus.FirstOrDefault((Ecu e) => e.Name == targetEcuDetails.Ecu && e != ecu);
				if (ecu2 != null)
				{
					diagnosisVariant = ecu2.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
				}
			}
			if (diagnosisVariant != null)
			{
				Channel channel = InternalConnectOffline(diagnosisVariant, null);
				if (channel != null)
				{
					channel.Parameters.Load(parameterFile, parameterFileFormat, unknownList, respectAccessLevels: false);
					RaiseConnectCompleteEvent(channel, null);
				}
				return channel;
			}
			throw new DataException(string.Format(CultureInfo.InvariantCulture, "Ecu variant data {0} referenced by parameter file was not found.", targetEcuDetails.DiagnosisVariant));
		}
		throw new DataException(string.Format(CultureInfo.InvariantCulture, "Ecu data {0} referenced by parameter file was not found.", targetEcuDetails.Ecu));
	}

	public Channel OpenOffline(DiagnosisVariant diagnosisVariant)
	{
		if (diagnosisVariant != null)
		{
			Channel channel = null;
			if (diagnosisVariant.Ecu.RollCallManager != null)
			{
				channel = new Channel(diagnosisVariant, this, null);
			}
			else if (diagnosisVariant.Ecu.IsMcd)
			{
				channel = new Channel(diagnosisVariant.GetMcdDBLocationForProtocol(diagnosisVariant.Ecu.ProtocolName), diagnosisVariant, this, null);
			}
			else
			{
				CaesarEcu val = diagnosisVariant.OpenEcuHandle();
				if (val != null)
				{
					channel = new Channel(val, diagnosisVariant, this, null);
				}
			}
			lock (offlineOutOfCollection)
			{
				offlineOutOfCollection.Add(channel);
				return channel;
			}
		}
		return null;
	}

	internal override void Remove(Channel channel)
	{
		lock (offlineOutOfCollection)
		{
			if (offlineOutOfCollection.Contains(channel))
			{
				offlineOutOfCollection.Remove(channel);
			}
			else
			{
				base.Remove(channel);
			}
		}
	}

	internal void CleanupOffline()
	{
		while (true)
		{
			Channel channel = null;
			lock (offlineOutOfCollection)
			{
				if (offlineOutOfCollection.Count <= 0)
				{
					break;
				}
				channel = offlineOutOfCollection[0];
			}
			channel.InternalDisconnect();
		}
	}

	public void StartAutoConnect()
	{
		StartAutoConnect(-1);
	}

	public void StartAutoConnect(int numberOfAttemptCycles)
	{
		if (autoConnecting)
		{
			return;
		}
		autoConnecting = true;
		if (ecusInAutoConnect == null)
		{
			Sapi sapi = Sapi.GetSapi();
			ecusInAutoConnect = new List<EcuAutoConnect>();
			foreach (IGrouping<string, Ecu> item in from e in sapi.Ecus
				where !e.IsRollCall && !e.OfflineSupportOnly && !e.IsVirtual && !e.ProhibitAutoConnection
				group e by e.Identifier)
			{
				ecusInAutoConnect.Add(new EcuAutoConnect(this, item));
			}
		}
		for (int num = 0; num < ecusInAutoConnect.Count; num++)
		{
			ecusInAutoConnect[num].Start(numberOfAttemptCycles);
		}
	}

	public void StopAutoConnect()
	{
		if (!autoConnecting)
		{
			return;
		}
		autoConnecting = false;
		if (ecusInAutoConnect == null)
		{
			return;
		}
		AbortPendingConnections();
		foreach (EcuAutoConnect item in ecusInAutoConnect.ToList())
		{
			if (item != null && item.BackgroundThread != null)
			{
				item.BackgroundThread.Join();
			}
		}
	}

	public static IRollCall GetManager(Protocol protocol)
	{
		return RollCall.GetManager(protocol);
	}

	private void ConfirmBaudRateAllowed(ConnectionResource desiredResource)
	{
		lock (pendingResources)
		{
			foreach (ConnectionResource pendingResource in pendingResources)
			{
				if (pendingResource != null && SameResourceDifferentBaudRate(desiredResource, pendingResource))
				{
					throw new CaesarException(SapiError.ConnectionResourceNotAvailableOtherBaudRateInUseByConnectingChannel);
				}
			}
		}
		lock (pendingConnections)
		{
			foreach (BackgroundConnect pendingConnection in pendingConnections)
			{
				ConnectionResource connectionResource = pendingConnection.ConnectionResource;
				if (connectionResource != null && SameResourceDifferentBaudRate(desiredResource, connectionResource))
				{
					throw new CaesarException(SapiError.ConnectionResourceNotAvailableOtherBaudRateInUseByConnectingChannel);
				}
			}
		}
		using IEnumerator<Channel> enumerator3 = GetEnumerator();
		while (enumerator3.MoveNext())
		{
			Channel current2 = enumerator3.Current;
			if (current2.Online && current2.ConnectionResource != null && SameResourceDifferentBaudRate(desiredResource, current2.ConnectionResource))
			{
				throw new CaesarException(SapiError.ConnectionResourceNotAvailableOtherBaudRateInUseByConnectedChannel);
			}
		}
	}

	private BackgroundConnect InternalConnect(ConnectionResource resource, ChannelOptions options, bool sync, ConnectSource connectSource, BackgroundConnect previousBackgroundConnect)
	{
		Sapi sapi = Sapi.GetSapi();
		switch (sapi.InitState)
		{
		case InitState.Online:
		{
			CaesarChannel ch = null;
			McdLogicalLink mcdLogicalLink = null;
			RaiseConnectProgressEvent(resource, 15.0);
			BackgroundConnect backgroundConnect;
			lock (pendingConnections)
			{
				try
				{
					if (sapi.Ecus.GetConnectedCountForIdentifier(resource.Ecu.Identifier) > 0 || GetConnectingCountForIdentifier(resource.Ecu.Identifier, previousBackgroundConnect) > 0)
					{
						throw new CaesarException(SapiError.ChannelAlreadyConnectedToIdentifier);
					}
					if (resource.Ecu.IsMcd)
					{
						mcdLogicalLink = OpenMcdChannel(resource);
					}
					else
					{
						ch = OpenCaesarChannel(resource, addToPendingResources: true);
					}
				}
				catch (CaesarException ce)
				{
					RaiseConnectCompleteEvent(resource, ce);
					if (sync)
					{
						throw;
					}
					return null;
				}
				Thread.Sleep(100);
				if (resource.Ecu.IsMcd)
				{
					InitComParameterCollection(mcdLogicalLink, resource);
				}
				else
				{
					InitComParameterCollection(ch, resource);
				}
				RaiseConnectProgressEvent(resource, 35.0);
				backgroundConnect = ((!resource.Ecu.IsMcd) ? new BackgroundConnect((connectSource != ConnectSource.VariantTransfer) ? this : null, ch, resource, options, connectSource == ConnectSource.Automatic) : new BackgroundConnect((connectSource != ConnectSource.VariantTransfer) ? this : null, mcdLogicalLink, resource, options, connectSource == ConnectSource.Automatic));
				if (connectSource != ConnectSource.VariantTransfer)
				{
					pendingConnections.Add(backgroundConnect);
				}
				backgroundConnect.Start();
				lock (pendingResources)
				{
					pendingResources.Remove(resource);
				}
			}
			if (sync)
			{
				backgroundConnect.BackgroundThread.Join();
				return backgroundConnect;
			}
			return null;
		}
		case InitState.Offline:
			throw new InvalidOperationException("Resource cannot be acquired in offline operation mode");
		case InitState.NotInitialized:
			throw new InvalidOperationException("Sapi not initialized");
		default:
			return null;
		}
	}

	private McdLogicalLink OpenMcdChannel(ConnectionResource resource)
	{
		McdLogicalLink mcdLogicalLink = null;
		McdInterface mcdInterface = McdRoot.CurrentInterfaces.FirstOrDefault((McdInterface i) => i.Qualifier == resource.MCDInterfaceQualifier);
		if (mcdInterface != null)
		{
			McdInterfaceResource mcdInterfaceResource = mcdInterface.Resources.FirstOrDefault((McdInterfaceResource r) => r.Qualifier == resource.MCDResourceQualifier);
			if (mcdInterfaceResource != null)
			{
				try
				{
					lock (pendingResources)
					{
						if (resource.Ecu.IsByteMessaging)
						{
							mcdLogicalLink = McdRoot.CreateLogicalLink(resource.Ecu.DiagnosisProtocol.Name, mcdInterfaceResource, string.Empty);
							if (resource.BaudRate != 0)
							{
								try
								{
									mcdLogicalLink.SetComParameter("CP_Baudrate", resource.BaudRate.ToString(CultureInfo.InvariantCulture));
								}
								catch (McdException mcdError)
								{
									Sapi.GetSapi().RaiseExceptionEvent(this, new CaesarException(mcdError));
								}
							}
						}
						else
						{
							mcdLogicalLink = McdRoot.CreateLogicalLink(resource.Interface.Qualifier, mcdInterfaceResource, resource.Ecu.Properties["FixedDiagnosisVariant"]);
						}
						if (mcdLogicalLink != null && !pendingResources.Contains(resource))
						{
							pendingResources.Add(resource);
						}
					}
				}
				catch (McdException mcdError2)
				{
					throw new CaesarException(mcdError2);
				}
			}
		}
		if (mcdLogicalLink == null)
		{
			throw new CaesarException(SapiError.ConnectionResourceNotAvailable);
		}
		return mcdLogicalLink;
	}

	internal CaesarChannel OpenCaesarChannel(ConnectionResource resource, bool addToPendingResources)
	{
		//IL_0021: Expected O, but got Unknown
		//IL_0186: Expected O, but got Unknown
		//IL_01f7: Expected O, but got Unknown
		CaesarResource val = null;
		CaesarChannel val2 = null;
		ConfirmBaudRateAllowed(resource);
		try
		{
			CaesarRoot.LockResources();
		}
		catch (CaesarErrorException ex)
		{
			throw new CaesarException(ex, null, null);
		}
		try
		{
			uint num = (resource.Ecu.IsByteMessaging ? CaesarRoot.GetAvailableProtocolResourceCount(resource.Ecu.DiagnosisProtocol.Name) : CaesarRoot.GetAvailableEcuResourceCount(resource.Ecu.Name));
			ushort num2 = 0;
			while (num2 < num && val == null)
			{
				try
				{
					CaesarResource val3 = (resource.Ecu.IsByteMessaging ? CaesarRoot.GetAvailableProtocolResource(resource.Ecu.DiagnosisProtocol.Name, num2++) : CaesarRoot.GetAvailableEcuResource(resource.Ecu.Name, num2++));
					ConnectionResource objB = (resource.Ecu.IsByteMessaging ? new ConnectionResource(resource.Ecu.DiagnosisProtocol, val3, resource.SourceAddress.Value, (uint)resource.BaudRate) : new ConnectionResource(resource.Ecu, val3, resource.SourceAddress));
					if (object.Equals(resource, objB))
					{
						val = val3;
					}
				}
				catch (CaesarErrorException)
				{
				}
			}
			if (val != null)
			{
				lock (pendingResources)
				{
					if (resource.Ecu.IsByteMessaging)
					{
						val2 = val.OpenChannel(resource.Ecu.DiagnosisProtocol.Name, resource.SourceAddress.Value);
						if (resource.BaudRate != 0)
						{
							try
							{
								val2.SetComParameter("CP_BAUDRATE", resource.BaudRate.ToString(CultureInfo.InvariantCulture));
							}
							catch (CaesarErrorException ex3)
							{
								CaesarErrorException caesarError = ex3;
								Sapi sapi = Sapi.GetSapi();
								sapi.RaiseExceptionEvent(sapi.Channels, new CaesarException(caesarError));
							}
						}
					}
					else
					{
						val2 = val.OpenChannel(resource.Ecu.Name);
					}
					if (addToPendingResources && !pendingResources.Contains(resource))
					{
						pendingResources.Add(resource);
					}
				}
			}
		}
		catch (CaesarErrorException ex4)
		{
			throw new CaesarException(ex4, null, null);
		}
		finally
		{
			CaesarRoot.UnlockResources();
		}
		if (val == null)
		{
			throw new CaesarException(SapiError.ConnectionResourceNotAvailable);
		}
		return val2;
	}

	private Channel Add(CaesarChannel ch, ConnectionResource cr, ChannelOptions options)
	{
		Channel channel = new Channel(ch, cr, this, options);
		lock (base.Items)
		{
			base.Items.Add(channel);
			return channel;
		}
	}

	private Channel Add(McdLogicalLink ll, ConnectionResource cr, ChannelOptions options)
	{
		Channel channel = new Channel(ll, cr, this, options);
		lock (base.Items)
		{
			base.Items.Add(channel);
			return channel;
		}
	}

	private static bool SameResourceDifferentBaudRate(ConnectionResource source, ConnectionResource test)
	{
		if (string.Equals(source.Type, test.Type, StringComparison.Ordinal) && string.Equals(source.HardwareName, test.HardwareName, StringComparison.Ordinal) && source.PortIndex == test.PortIndex && !source.BaudRate.Equals(test.BaudRate))
		{
			return true;
		}
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ConnectOffline(string, ParameterFileFormat, ArrayList) is deprecated, please use ConnectOffline(string, ParameterFileFormat, Collection<string>) instead.")]
	public Channel ConnectOffline(string parameterFileName, ParameterFileFormat parameterFileFormat, ArrayList unknownList)
	{
		using StreamReader parameterFile = new StreamReader(parameterFileName);
		return ConnectOffline(parameterFile, parameterFileFormat, unknownList);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ConnectOffline(StreamReader, ParameterFileFormat, ArrayList) is deprecated, please use ConnectOffline(StreamReader, ParameterFileFormat, Collection<string>) instead.")]
	public Channel ConnectOffline(StreamReader parameterFile, ParameterFileFormat parameterFileFormat, ArrayList unknownList)
	{
		Collection<string> collection = new Collection<string>();
		Channel result = ConnectOffline(parameterFile, parameterFileFormat, collection);
		foreach (string item in collection)
		{
			unknownList.Add(item);
		}
		return result;
	}
}
